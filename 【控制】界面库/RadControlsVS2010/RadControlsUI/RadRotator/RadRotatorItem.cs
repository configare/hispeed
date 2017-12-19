using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using DesignerSerializationVisibility = System.ComponentModel.DesignerSerializationVisibility;
using System.Diagnostics;
using Telerik.WinControls.Collections;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The RadItem that implements the actual <see cref="RadRotator"/>'s functionality.
    /// </summary>
    public class RadRotatorItem : RadItem
    {
        #region Private static initialization

        private static SizeF fullScale = new SizeF(1, 1);

        private static RadProperty AnimationSetting = RadProperty.Register(
            "AnimationSetting", typeof(List<AnimatedPropertySetting>), typeof(RadRotatorItem),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.None)
            );

        private static readonly object ItemClickedEventKey;
        private static readonly object StartRotationEventKey;
        private static readonly object StopRotationEventKey;
        private static readonly object BeginRotateEventKey;
        private static readonly object EndRotateEventKey;

        static RadRotatorItem()
        {
            ItemClickedEventKey = new object();
            StartRotationEventKey = new object();
            StopRotationEventKey = new object();
            BeginRotateEventKey = new object();
            EndRotateEventKey = new object();
        }

        #endregion

        #region Private data

        private RadItemOwnerCollection items;
        private RadItem defaultItem;


        private int currentIndex = -2;
        private int animationFrames = 10;

        private Timer timer;

        private int suspendRotate = 0;
        private int animationsRunning = 0;

        private bool shouldStop = false;

        private bool opacityAnimation = true;
        private SizeF locationAnimation = new SizeF(0, -1);
        private SizeF scaleAnimation = new SizeF(0.5f, 0.5f);

        #endregion

        #region Constructors & Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.ShouldHandleMouseInput = true;

            items = new RadItemOwnerCollection();
            items.ItemTypes = new Type[] { 
                typeof(RadItemsContainer),
                typeof(RadRotatorElement),
                typeof(RadArrowButtonElement),
                typeof(RadComboBoxElement),
                typeof(RadButtonElement),
                typeof(RadWebBrowserElement),
                typeof(RadTextBoxElement),
                typeof(RadImageButtonElement),
                typeof(RadImageItem),
                typeof(RadCheckBoxElement),
                typeof(RadMaskedEditBoxElement),
                typeof(RadLabelElement)

            };
            items.ItemsChanged += new ItemChangedDelegate(ItemsChanged);
            items.Owner = this;

            this.timer = new Timer();
            this.timer.Tick += new EventHandler(Animate);
            this.timer.Interval = 2000;
        }

        private int removedIndex = -2;

        private void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            bool isCurrent;

            switch (operation)
            {
                case ItemsChangeOperation.Inserted:

                    int newIndex = changed.IndexOf(target);
                    if (newIndex <= currentIndex) currentIndex++;

                    target.Visibility = ElementVisibility.Hidden;
                    target.NotifyParentOnMouseInput = true;
                    break;

                case ItemsChangeOperation.Set:
                    isCurrent = changed.IndexOf(target) == currentIndex;

                    target.Visibility = isCurrent ? ElementVisibility.Visible : ElementVisibility.Hidden;
                    target.NotifyParentOnMouseInput = true;
                    break;

                case ItemsChangeOperation.Removing:
                    removedIndex = changed.IndexOf(target);

                    break;

                case ItemsChangeOperation.Removed:

                    if (removedIndex != currentIndex)
                    {
                        removedIndex = -2;
                        break;
                    }

                    removedIndex = -2;

                    this.RemoveAnimations(target);

                    if (this.items.Count > 0)
                    {
                        int next = (currentIndex + 1) % this.Items.Count;
                        currentIndex = -2;
                        this.Goto(next);
                    }

                    break;

                case ItemsChangeOperation.Clearing:
                    for (int i = 0; i < this.Items.Count; i++)
                        this.RemoveAnimations(this.Items[i]);

                    if (defaultItem != null)
                    {
                        currentIndex = -1;
                        defaultItem.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        currentIndex = -2;
                    }

                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="RadItem"/>s that will be rotated.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="RadItem"/> that is to be displayed while loading the rest items. It is not cycled through when rotation starts.
        /// If you want to have initial item that will be cycled through, add it to the <see cref="Items"/> collection
        /// and advance to it using <see cref="Goto"/>
        /// </summary>
        /// <seealso cref="Goto"/>
        /// <seealso cref="CurrentIndex"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        public RadItem DefaultItem
        {
            get { return defaultItem; }
            set
            {
                if (defaultItem == value) return;

                int i = Children.IndexOf(defaultItem);
                if (i != -1)
                    Children.RemoveAt(i);

                defaultItem = value;

                this.Children.Add(defaultItem);

                OnNotifyPropertyChanged("DefaultItem");
            }
        }

        /// <summary>
        /// Gets or Sets the interval between consequetive rotation animations.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(2000)]
        public int Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        private bool shouldStopOnMouseOver = true;

        /// <summary>
        /// Gets or sets whether RadRotator should stop rotating on MouseOver
        /// </summary>
        [DefaultValue(true)]
        public bool ShouldStopOnMouseOver
        {
            get { return shouldStopOnMouseOver; }
            set { shouldStopOnMouseOver = value; }
        }

        /// <summary>
        /// Gets or Sets the swap animation's frames number
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(10)]
        public int AnimationFrames
        {
            get { return animationFrames; }
            set { if (value > 0) animationFrames = value; }
        }

        /// <summary>
        /// Gets or Sets value indicating whether opacity will be animated when switching frames
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool OpacityAnimation
        {
            get { return this.opacityAnimation; }
            set
            {
                if (this.opacityAnimation != value)
                {
                    this.opacityAnimation = value;
                    this.OnNotifyPropertyChanged("OpacityAnimation");
                }
            }
        }

        /// <summary>
        /// Gets or Sets value defining the initial position of the incomming item
        /// and the final position of the outgoing item.
        /// Note: The position is relative, in the range [-1, 1] for each of the components (X, Y).
        /// Value of positive or negative 1 means that the object will not be in the visible area
        /// before the animation begins (for the incomming item) or after it ends (for the outgoing item)
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SizeF LocationAnimation
        {
            get { return this.locationAnimation; }
            set
            {
                if (this.locationAnimation != value)
                {
                    this.locationAnimation = value;
                    this.OnNotifyPropertyChanged("LocationAnimation");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the index of the current item.
        /// <b>Note:</b> When setting the current item, the old and the new item will be swapped.
        /// </summary>
        /// <seealso cref="Goto(int)"/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { Goto(value); }


        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <seealso cref="Goto(int)"/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItem CurrentItem
        {
            get
            {
                if (currentIndex < -1 || currentIndex >= this.items.Count)
                    return null;

                return currentIndex == -1 ? this.defaultItem : this.items[currentIndex];
            }
        }

        /// <summary>
        /// Gets or Sets value indicating whether the <see cref="RadRotator"/> is started/stopped.
        /// </summary>
        /// <seealso cref="Start"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Stop"/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Running
        {
            get { return this.timer.Enabled; }
            set
            {
                if (this.timer.Enabled == value) return;

                if (value)
                    this.Start(false);
                else
                    this.Stop();
            }
        }

        #endregion

        #region Private methods

        private void ResetOpacity(RadItem target, double opacity)
        {
            target.ResetValue(RadItem.OpacityProperty, ValueResetFlags.Local);
            target.Opacity = opacity;
        }

        private void Continue()
        {
            if (suspendRotate == 2)
            {
                AdvanceFrame();
                timer.Start();
            }

            suspendRotate = 0;
        }

        private void Pause()
        {
            if (suspendRotate == 0)
                suspendRotate = 1;
        }

        private void Animate(object sender, EventArgs e)
        {
            AdvanceFrame();
        }

        private void AdvanceFrame()
        {
            if (suspendRotate == 1)
            {
                suspendRotate = 2;
                timer.Stop();
                return;
            }

            if (!Next())
                Stop();
        }

        private bool SwapItems(int to)
        {
            if (currentIndex == to)
            {
                timer.Stop();
                return false;
            }

            BeginRotateEventArgs ea = new BeginRotateEventArgs(currentIndex, to);

            this.OnBeginRotate(this, ea);

            if (ea.Cancel)
                return false;

            if (ea.To < -1 || ea.To >= items.Count)
                throw new ArgumentOutOfRangeException("ea.TO", "Should be a valid index in the Items collection, or -1 for default item");

            RadItem fadeOutItem = GetItem(currentIndex);
            RadItem fadeInItem = GetItem(ea.To);

            SwapItems(fadeOutItem, fadeInItem);

            currentIndex = ea.To;

            return true;
        }

        private void SwapItems(RadItem itemToHide, RadItem itemToShow)
        {
            List<AnimatedPropertySetting> fadeOut = itemToHide != null ?
                CreateAnimations(itemToHide, false) : null;

            List<AnimatedPropertySetting> fadeIn = itemToShow != null ?
                CreateAnimations(itemToShow, true) : null;

            //int oldAnimations = animationsRunning;

            if (fadeOut != null)
            {
                for (int i = 0; i < fadeOut.Count; i++)
                {
                    if (fadeOut[i].IsAnimating(itemToHide)) continue;

                    animationsRunning++;
                    fadeOut[i].ApplyValue(itemToHide);
                }
            }

            if (fadeIn != null)
            {
                for (int i = 0; i < fadeIn.Count; i++)
                {
                    if (fadeIn[i].IsAnimating(itemToShow)) continue;

                    animationsRunning++;
                    fadeIn[i].ApplyValue(itemToShow);
                }
            }
        }

        private List<AnimatedPropertySetting> CreateAnimations(RadItem item, bool show)
        {
            List<AnimatedPropertySetting> animations = GetAnimations(item);

            if (animations == null)
            {
                animations = new List<AnimatedPropertySetting>();
                SetAnimations(item, animations);
            }

                AddLocationAnimation(item, animations, show);

            if (this.opacityAnimation)
                AddOpacityAnimation(item, animations, show);
            else
                ResetOpacity(item, 1d);

                AddScaleAnimation(item, animations, show);

            return animations;
        }

        private void AddOpacityAnimation(RadItem item, List<AnimatedPropertySetting> animations, bool show)
        {

            item.Visibility = show ? ElementVisibility.Hidden : ElementVisibility.Visible;

            double to = show ? 1d : 0d;
            double from = 1d - to;

            AnimatedPropertySetting animation = new AnimatedPropertySetting(
                VisualElement.OpacityProperty,
                from,
                to,
                animationFrames,
                40);

            animation.ApplyEasingType = RadEasingType.InOutQuad;

            if (show)
            {
                animation.AnimationStarted += delegate { item.Visibility = ElementVisibility.Visible; };

                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    animationsRunning--;
                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }
            else
            {
                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    if (animations.Count == 0)
                        item.Visibility = ElementVisibility.Hidden;

                    animationsRunning--;
                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }

            animations.Add(animation);
        }

        private void AddLocationAnimation(RadItem item, List<AnimatedPropertySetting> animations, bool show)
        {
            Rectangle source;
            Rectangle destination;

            if (show)
            {
                item.Visibility = ElementVisibility.Hidden;

                source = new Rectangle(
                    (int)(item.Bounds.Width * this.locationAnimation.Width),
                    (int)(item.Bounds.Height * this.locationAnimation.Height),
                    item.Bounds.Width,
                    item.Bounds.Height
                    );

                destination = new Rectangle(0, 0, item.Bounds.Width, item.Bounds.Height);
            }
            else
            {
                item.Visibility = ElementVisibility.Visible;

                source = item.Bounds;

                destination = new Rectangle(
                    -(int)(this.Bounds.Width * this.locationAnimation.Width),
                    -(int)(this.Bounds.Height * this.locationAnimation.Height),
                    item.Bounds.Width,
                    item.Bounds.Height
                    );
            }

            AnimatedPropertySetting animation = new AnimatedPropertySetting(
                VisualElement.BoundsProperty,
                source,
                destination,
                animationFrames,
                40);


            animation.ApplyEasingType = RadEasingType.InOutQuad;

            if (show)
            {
                animation.AnimationStarted += delegate { item.Visibility = ElementVisibility.Visible; };

                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    animationsRunning--;

                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }
            else
            {
                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    if (animations.Count == 0)
                        item.Visibility = ElementVisibility.Hidden;

                    animationsRunning--;

                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }

            animations.Add(animation);
        }

        private void AddScaleAnimation(RadItem item, List<AnimatedPropertySetting> animations, bool show)
        {
            SizeF source;
            SizeF destination;

            if (show)
            {
                item.Visibility = ElementVisibility.Hidden;

                source = this.scaleAnimation;
                destination = fullScale;
            }
            else
            {
                item.Visibility = ElementVisibility.Visible;

                source = fullScale;
                destination = this.scaleAnimation;
            }

            AnimatedPropertySetting animation = new AnimatedPropertySetting(
                VisualElement.ScaleTransformProperty,
                source,
                destination,
                animationFrames,
                40);

            animation.ApplyEasingType = RadEasingType.InOutQuad;

            if (show)
            {
                animation.AnimationStarted += delegate { item.Visibility = ElementVisibility.Visible; };

                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    animationsRunning--;

                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }
            else
            {
                animation.AnimationFinished += delegate
                {
                    animations.Remove(animation);

                    if (animations.Count == 0)
                        item.Visibility = ElementVisibility.Hidden;

                    animationsRunning--;

                    if (animationsRunning == 0) OnEndRotate(this, EventArgs.Empty);
                };
            }

            animations.Add(animation);
        }

        private List<AnimatedPropertySetting> GetAnimations(RadItem item)
        {
            object animations = item.GetValue(RadRotatorItem.AnimationSetting);
            return animations != null ? (List<AnimatedPropertySetting>)animations : null;
        }

        private void SetAnimations(RadItem item, List<AnimatedPropertySetting> animations)
        {
            item.SetValue(RadRotatorItem.AnimationSetting, animations);
        }

        private void RemoveAnimations(RadItem item)
        {
            List<AnimatedPropertySetting> animations = GetAnimations(item);

            if (animations == null) return;

            for (int i = animations.Count - 1; i >= 0; i--)
                animations[i].Stop(item);

            animations.Clear();

            SetAnimations(item, null);
        }

        private RadItem GetItem(int index)
        {
            if (index == -1)
                return defaultItem;

            if (index >= 0 && index < this.Items.Count)
                return this.items[index];

            return null;
        }

        protected override void DisposeManagedResources()
        {
            this.timer.Dispose();

            base.DisposeManagedResources();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts cycling through the elements in the <see cref="Items"/> collection
        /// </summary>
        /// <param name="startImmediately">set to true to initiate rotation immediately, or set to false to rotate after the time <see cref="Interval"/></param>
        /// <returns>there are no elements to rotate (Items collection is empty)</returns>
        public bool Start(bool startImmediately)
        {
            if (items.Count < 1) return false;

            if (timer.Enabled) return true;

            CancelEventArgs ea = new CancelEventArgs();
            this.OnStartRotation(this, ea);

            if (ea.Cancel)
                return false;

            if (startImmediately)
                AdvanceFrame();

            timer.Start();

            return true;
        }

        /// <summary>
        /// Stops the rotation process. If swap is under way, it will be completed.
        /// </summary>
        public void Stop()
        {
            timer.Stop();

            if (animationsRunning > 0)
                this.shouldStop = true;
            else
                this.OnStopRotation(this, new EventArgs());
        }

        /// <summary>
        /// Initiates swap between the current item and the one whose index is supplied.
        /// </summary>
        /// <param name="index">the index of the item in the <see cref="Items"/> collection. The index of the home element is -1.</param>
        /// <returns>true on successful swap</returns>
        public bool Goto(int index)
        {
            if (index < -2 || index >= this.Items.Count)
                return false;

            //if (timer.Enabled)
            //    timer.Stop();

            if (currentIndex == index)
            {
                return true;
            }

            BeginRotateEventArgs ea = new BeginRotateEventArgs(currentIndex, index);

            this.OnBeginRotate(this, ea);

            if (ea.Cancel)
                return true;

            RadItem itemToHide = GetItem(currentIndex);
            RadItem itemToShow = GetItem(ea.To);

            currentIndex = index;

            SwapItems(itemToHide, itemToShow);

            return true;
        }

        /// <summary>
        /// Makes transition to the default element.
        /// </summary>
        public bool GotoDefault()
        {
            return this.Goto(-1);
        }

        /// <summary>
        /// Advances to the next item
        /// </summary>
        public bool Next()
        {
            if (items.Count < 0)
                return false;

            return SwapItems(currentIndex < 0 ? 0 : (currentIndex + 1) % items.Count);
        }

        /// <summary>
        /// Advances to the previous item
        /// </summary>
        public bool Previous()
        {
            return SwapItems(currentIndex < 1 ? items.Count - 1 : currentIndex - 1);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when an Item is clicked
        /// </summary>
        public event EventHandler ItemClicked
        {
            add { this.Events.AddHandler(ItemClickedEventKey, value); }
            remove { this.Events.RemoveHandler(ItemClickedEventKey, value); }
        }

        /// <summary>
        /// Fires when <see cref="RadRotator"/> is started.
        /// </summary>
        public event CancelEventHandler StartRotation
        {
            add { this.Events.AddHandler(StartRotationEventKey, value); }
            remove { this.Events.RemoveHandler(StartRotationEventKey, value); }
        }

        /// <summary>
        /// Fires when <see cref="RadRotator"/> is stopped.
        /// </summary>
        public event EventHandler StopRotation
        {
            add { this.Events.AddHandler(StopRotationEventKey, value); }
            remove { this.Events.RemoveHandler(StopRotationEventKey, value); }
        }

        /// <summary>
        /// Fires before <see cref="RadItem"/>s' swap begins.
        /// </summary>
        public event BeginRotateEventHandler BeginRotate
        {
            add { this.Events.AddHandler(BeginRotateEventKey, value); }
            remove { this.Events.RemoveHandler(BeginRotateEventKey, value); }
        }

        /// <summary>
        /// Fires when <see cref="RadItem"/>s' swap has finished.
        /// </summary>
        public event EventHandler EndRotate
        {
            add { this.Events.AddHandler(EndRotateEventKey, value); }
            remove { this.Events.RemoveHandler(EndRotateEventKey, value); }
        }

        protected virtual void OnItemClicked(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[ItemClickedEventKey];

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnStartRotation(object sender, CancelEventArgs e)
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[StartRotationEventKey];

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnStopRotation(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[StopRotationEventKey];

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnBeginRotate(object sender, BeginRotateEventArgs e)
        {
            BeginRotateEventHandler handler = (BeginRotateEventHandler)this.Events[BeginRotateEventKey];

            if (handler != null)
            {
                handler(sender, e);
            }
        }

        protected virtual void OnEndRotate(object sender, EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EndRotateEventKey];

            if (handler != null)
            {
                handler(sender, e);
            }

            if (shouldStop)
            {
                shouldStop = false;
                this.OnStopRotation(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Mouse Event Overrides

        /// <commentsfrom cref="RadElement.OnMouseEnter" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (shouldStopOnMouseOver)
            {
                this.Pause();
            }
        }

        /// <commentsfrom cref="RadElement.OnMouseLeave" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.Continue();
        }

        /// <commentsfrom cref="RadElement.OnBubbleEvent" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);

            Console.WriteLine(args.RoutedEvent.EventName);

            switch (args.RoutedEvent.EventName)
            {
                case "MouseClickedEvent":
                    RadItem s = this.CurrentItem;

                    if (s != null)
                        this.OnItemClicked(s, new EventArgs());

                    break;

                case "MouseEnterEvent":
                    break;
            }
        }

        #endregion
    }
}
