using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Type of animation to be applied on carousel items
    /// </summary>
    [Flags]
    public enum Animations
    {
        None = 0,
        Location = 1,
        Opacity = 2,
        Scale = 4,
        All = 7
    }

    public class CarouselItemsContainer : RadItem, IVirtualViewport
    {
        #region BitState Keys

        internal const ulong SpecialCaseStateKey = RadItemLastStateKey << 1;
        internal const ulong UpToDateStateKey = SpecialCaseStateKey << 1;
        internal const ulong EnableLoopingStateKey = UpToDateStateKey << 1;
        internal const ulong VirtualModeStateKey = EnableLoopingStateKey << 1;
        internal const ulong EnableAutoLoopStateKey = VirtualModeStateKey << 1;

        #endregion

        private class AnimationEntry
        {
            public AnimationEntry(AnimatedPropertySetting PropertySetting, RadElement Element)
            {
                this.PropertySetting = PropertySetting;
                this.Element = Element;
            }

            public AnimatedPropertySetting PropertySetting;
            public RadElement Element;

            public override bool Equals(object obj)
            {
                return base.Equals(obj) ||
                    ((AnimationEntry)obj).PropertySetting == this.PropertySetting &&
                    ((AnimationEntry)obj).Element == this.Element;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #region Static members

        public readonly static RadProperty CarouselAnimationData =
            RadProperty.Register("CarouselAnimationData", typeof(CarouselPathAnimationData), typeof(CarouselItemsContainer),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.None));

        public readonly static RadProperty CarouselLocationProperty =
            RadProperty.Register("CarouselLocationProperty", typeof(double), typeof(CarouselItemsContainer),
                new RadElementPropertyMetadata(double.NaN, ElementPropertyOptions.None));

        #endregion

        #region Fields

        private int update = 0;

        private ICarouselPath carouselPath = null;
        private RadEasingType easingType = RadEasingType.OutQuad;
        private Animations animationsApplied = Animations.All;

        private RadItemVirtualizationCollection items = null;        
        private int animationFrames = 30;
        private int visibleItemCount = 10;
        
        private RangeMapper mapper = new CircularRangeMapper();
        private int selectedIndex = 0;
        //private int selectedIndexChange = 0;
        private Range currentRange = new Range();
        private Range newRange = new Range();
        private RadCarouselElement owner;
        private List<AnimationEntry> currentlyRunningAnimations = new List<AnimationEntry>();
        private AutoLoopDirections autoLoopDirection;
        private double minFadeOpacity = 0.33;
        protected OpacityChangeConditions opacityChangeCondition = OpacityChangeConditions.ZIndex;

        private CarouselItemsLocationBehavior carouselItemBehavior = null;
        private int animationDelay = 40;
        private AutoLoopPauseConditions autoLoopPauseCondition = AutoLoopPauseConditions.OnMouseOverCarousel;

        #endregion

        #region Constructors & Initialization

        public CarouselItemsContainer(RadCarouselElement owner)
        {
            this.owner = owner;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[EnableLoopingStateKey] = true;
            this.BitState[VirtualModeStateKey] = true;

            CarouselEllipsePath defaultPath = new CarouselEllipsePath();
            defaultPath.Center = new Telerik.WinControls.UI.Point3D(50, 50, 0);
            defaultPath.FinalAngle = -100;
            defaultPath.InitialAngle = -90;
            defaultPath.U = new Telerik.WinControls.UI.Point3D(-20.0, -17.0, -50.0);
            defaultPath.V = new Telerik.WinControls.UI.Point3D(30.0, -25.0, -60.0);
            defaultPath.ZScale = 500;
            this.carouselPath = defaultPath;

            this.BypassLayoutPolicies = true;
            this.carouselItemBehavior = new CarouselItemsLocationBehavior(this);

            this.items = new RadItemVirtualizationCollection(this);
            this.items.ItemsChanged += new ItemChangedDelegate(OnItemsChanged);
            this.items.ItemTypes = new Type[] { 
                typeof(RadButtonElement),
                typeof(RadImageItem),
                typeof(RadImageButtonElement),
                typeof(RadLabelElement),
                typeof(RadItemsContainer),
                typeof(RadRotatorElement),                
                typeof(RadComboBoxElement),                
                typeof(RadWebBrowserElement),
                typeof(RadTextBoxElement),
                typeof(RadCheckBoxElement),
                typeof(RadMaskedEditBoxElement)                
            };
        }

        private void OnItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (this.IsDisposing || this.IsDisposed)
                return;

            if (operation == ItemsChangeOperation.Inserted)
            {
                target.Disposing += new EventHandler(OnItem_Disposing);
            }
            else if (operation == ItemsChangeOperation.Removed)
            {
                target.Disposing -= new EventHandler(OnItem_Disposing);
            }
            else if (operation == ItemsChangeOperation.Clearing)
            {
                foreach (RadItem item in changed)
                {
                    item.Disposing -= new EventHandler(this.OnItem_Disposing);
                }
            }
        }

        private void OnItem_Disposing(object sender, EventArgs e)
        {
            RadItem item = sender as RadItem;

            if (item == null)
            {
                return;
            }

            this.items.Remove(sender as RadItem);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.SynchronizeCollections();
        }


        #endregion

        #region Properties


        /// <summary>
        /// Sets the way opacity is applied to carousel items
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public OpacityChangeConditions OpacityChangeCondition
        {
            get
            {
                return this.opacityChangeCondition;
            }
            set
            {
                if (value != this.opacityChangeCondition)
                {
                    this.opacityChangeCondition = value;
                    this.OnNotifyPropertyChangedAndUpdate("OpacityChangeCondition");
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get
            {
                int i = this.Items.Count > 0 ? this.selectedIndex % this.Items.Count : 0;
                return i < 0 ? i + this.Items.Count : i;
            }
            set
            {
                int nv = CalculateClosest(value);

                if (this.selectedIndex != nv)
                {
                    //this.selectedIndexChange = nv - this.selectedIndex;
                    this.selectedIndex = nv;
                    this.BitState[UpToDateStateKey] = false;
                    this.SyncItemsAndUpdateCarousel(this.selectedIndex);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double MinFadeOpacity
        {
            get { return minFadeOpacity; }
            set
            {
                minFadeOpacity = value;
                this.OnNotifyPropertyChangedAndUpdate("MinFadeOpacity");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableLooping
        {
            get
            {
                return this.GetBitState(EnableLoopingStateKey);
            }
            set
            {
                this.SetBitState(EnableLoopingStateKey, value);
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == EnableLoopingStateKey)
            {
                if (newValue)
                {
                    this.mapper = new CircularRangeMapper();
                }
                else
                {
                    this.mapper = new RestrictedRangeMapper();
                }

                this.RecalculateItemCount();
                this.OnNotifyPropertyChanged("EnableLooping");
            }
            else if (key == VirtualModeStateKey)
            {
                this.OnNotifyPropertyChanged("Virtualized");
            }
            else if (key == EnableAutoLoopStateKey)
            {
                this.OnNotifyPropertyChanged("EnableAutoLoop");
            }
        }

        private void RecalculateItemCount()
        {
            this.mapper.ItemsCount = this.Items.Count;//CalculateVisibleItemsCount();
        }

        /// <summary>
        /// Gets the owner RadCarouselElement.
        /// </summary>
        /// <value>The owner.</value>
        public RadCarouselElement Owner
        {
            get { return this.owner; }
        }

        public int VisibleItemCount
        {
            get { return this.visibleItemCount; }

            set
            {
                if (this.visibleItemCount != value &&
                    value > 0)
                {
                    this.visibleItemCount = value;
                    RecalculateItemCount();
                    this.OnNotifyPropertyChanged("VisibleItemCount");
                    this.SynchronizeCollections();
                }
            }
        }

        /// <summary>
        /// Gets or sets CarouselPath object that defines the curve which carousel items will animate through
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICarouselPath CarouselPath
        {
            get { return this.carouselPath; }

            set
            {
                if (this.carouselPath != value)
                {
                    this.BitState[UpToDateStateKey] = false;
                    SetPathCalculator(value);
                    this.OnNotifyPropertyChanged("CarouselPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets carousel items' animation easing.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadEasingType EasingType
        {
            get { return this.easingType; }

            set
            {
                if (this.easingType != value)
                {
                    this.easingType = value;
                    this.OnNotifyPropertyChanged("EasingType");
                }
            }
        }

        /// <exclude />
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItemCollection Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Gets or sets the set of animations to be applied on carousel items
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animations AnimationsApplied
        {
            get { return this.animationsApplied; }
            set 
            {
                if (this.animationsApplied != value)
                {
                    this.animationsApplied = value;
                    this.OnNotifyPropertyChanged("AnimationsApplied");
                }
            }
        }
        
        /// <summary>
        /// Set ot get the Carousel animation frames
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AnimationFrames
        {
            get { return this.animationFrames; }
            set 
            {
                if (this.animationFrames != value)
                {
                    this.animationFrames = value;
                    this.OnNotifyPropertyChanged("AnimationFrames");
                }
            }
        }

        /// <summary>
        /// Set ot get the Carousel animation frames
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int AnimationDelay
        {
            get { return animationDelay; }
            set
            {
                if (this.animationDelay != value)
                {
                    animationDelay = value;
                    this.OnNotifyPropertyChanged("AnimationDelay");
                }
            }
        }

        #endregion

        #region Overrides

        private void PathCalculator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ResetItemsPositions();
            this.SyncItemsAndUpdateCarousel(this.selectedIndex);
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            bool shouldUpdate = false;

            switch (e.PropertyName)
            {
                case "CarouselPath":
                    ResetItemsPositions();
                    shouldUpdate = true;
                    break;
                case "Virtualized":
                case "VisibleItemCount":
                case "EnableLooping":
                    shouldUpdate = true;
                    break;
                case "EnableAutoLoop":
                    this.Owner.ChangedAutoLoop();
                    break;
            }

            if (shouldUpdate)
            {
                this.SyncItemsAndUpdateCarousel(this.selectedIndex);
            }
            this.SynchronizeCollections();
            base.OnNotifyPropertyChanged(e);
        }

        private void OnNotifyPropertyChangedAndUpdate(string propertyName)
        {
            this.OnNotifyPropertyChanged(propertyName);

            this.SyncItemsAndUpdateCarousel(this.selectedIndex);
            this.SynchronizeCollections();
        }

        #endregion

        private void ResetItemsPositions()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                CarouselPathAnimationData data = GetItemAnimationData(i);
                data.From = double.NegativeInfinity;
            }
        }

        private void AnimateToPositions()
        {
            if (this.Visibility != ElementVisibility.Visible || this.GetBitState(UpToDateStateKey))
                return;

            int centerItemIndex = currentRange.FromRangeIndexRestricted(this.selectedIndex);

            if (centerItemIndex == -1)
                centerItemIndex = 0;

            double step, t;

            bool localZsource = !this.CarouselPath.ZindexFromPath();
            CarouselPathAnimationData data = null;

            step = this.CarouselPath.Step((int)this.currentRange.Length - 1);

            t = 1.0;

            for (int i = 0; i < this.currentRange.Length; i++)
            {
                data = GetItemAnimationData(i);
                data.To = Math.Max(0.0, Math.Min(1.0, t));

                if (this.GetBitState(SpecialCaseStateKey))
                    data.UpdateSpecialHandling();

                if (localZsource)
                    SetZIndex(this.Children[i],
                        i <= centerItemIndex ? i + (int)this.currentRange.Length - centerItemIndex : (int)this.currentRange.Length - i + centerItemIndex);

                t -= step;
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                Animate((VisualElement)this.Children[i]);
            }
            this.BitState[UpToDateStateKey] = true;
        }

        private void Animate(VisualElement element)
        {
            CarouselPathAnimationData data = GetItemAnimationData(element);

            if (!AnimationRemoveCurrent(element, data))
                return;

            AnimationCreate(element, data);

            data.Interrupt = !data.IsOutAnimation();

            if (data.CurrentAnimation != null)
            {
                data.CurrentAnimation.AnimationStarted += new AnimationStartedEventHandler(CurrentAnimation_AnimationStarted);
                data.CurrentAnimation.AnimationFinished += new AnimationFinishedEventHandler(CurrentAnimation_AnimationFinished); 
                data.CurrentAnimation.ApplyValue(element);
            }
        }

        public void CancelAniamtion()
        {
            bool fireEvent = this.currentlyRunningAnimations.Count > 0;

            while(this.currentlyRunningAnimations.Count > 0)
            {
                AnimationEntry animation = this.currentlyRunningAnimations[0];
                animation.PropertySetting.Stop(animation.Element);
            }

#if DEBUG
            if (this.currentlyRunningAnimations.Count > 0)
                throw new InvalidOperationException("Animation list should be empty.");
#endif
            if (fireEvent)
                this.Owner.OnAnimationFinished();
        }

        private void CurrentAnimation_AnimationFinished(object sender, AnimationStatusEventArgs e)
        {
            AnimationEntry animationEntry = new AnimationEntry((AnimatedPropertySetting)sender, e.Element);
            int index = this.currentlyRunningAnimations.IndexOf(animationEntry);
            if (index > -1)
            {
                this.currentlyRunningAnimations.RemoveAt(index);
            }
            else
            {
                int count = this.currentlyRunningAnimations.Count;
            	if(count>0)
                {
                    this.currentlyRunningAnimations.RemoveAt(count - 1);
                }
            }

            if (this.currentlyRunningAnimations.Count == 0)
            {
                this.Owner.OnAnimationFinished();
            }
        }

        private void CurrentAnimation_AnimationStarted(object sender, AnimationStatusEventArgs e)
        {
            if (this.currentlyRunningAnimations.Count == 0)
            {
                this.Owner.OnAnimationStarted();
            }

            this.currentlyRunningAnimations.Add(new AnimationEntry((AnimatedPropertySetting)sender, e.Element));
        }

        private void AnimationCreate(VisualElement element, CarouselPathAnimationData data)
        {
            this.CarouselPath.CreateAnimation(
                element, data, animationFrames, animationDelay
                );

            data.CurrentAnimation.ApplyEasingType = easingType;

            if (data.IsOutAnimation())
            {
                data.CurrentAnimation.AnimationFinished += delegate
                {
                    CarouselPathAnimationData animationData = GetItemAnimationData(element);
                    bool interrupted = animationData.Interrupt;

                    AnimationRemoveCurrent(element, animationData);

                    if (interrupted) 
                    {
                        return;
                    }

                    Rectangle r = element.ControlBoundingRectangle;

                    int index = this.Children.IndexOf(element);
                    if (index != -1)
                        this.Children.RemoveAt(index);

                    this.ElementTree.Control.Invalidate(r, true);
                };
            }
            else
            {
                data.CurrentAnimation.AnimationFinished += delegate
                {
                    AnimationRemoveCurrent(element, null);
                };

            }
        }

        #region IVirtualViewport Members

        public bool Virtualized
        {
            get
            {
                return this.GetBitState(VirtualModeStateKey);
            }
            set
            {
                this.SetBitState(VirtualModeStateKey, value);
            }
        }

        public void SetVirtualItemsCollection(IVirtualizationCollection virtualItemsCollection)
        {
            //this.items = virtualItemsCollection;
        }

        public void StartInitialAnimation()
        {
            this.Children.Clear();
            this.BitState[UpToDateStateKey] = false;
            SyncItemsAndUpdateCarousel(0);
        }

        public void OnItemDataInserted(int index, object itemData)
        {
            Range max = GetMaxRange(true);

            //TODO: temp fix
            int lastItemCount = this.mapper.ItemsCount;
            this.mapper.ItemsCount = this.CalculateVisibleItemsCount();

            bool isInRange = this.mapper.IsInRange(max, index);

            this.mapper.ItemsCount = lastItemCount;
            //-----temp fix

            if (isInRange)
            {
                int newIndex = this.EnsureItemToChildIndex(index);

                UpdateAnimationOnItemInsertRemove(newIndex, 1, double.NaN, null);

                this.BitState[UpToDateStateKey] = false;

                SyncItemsAndUpdateCarousel(index);

                if (this.update > 0)
                {
                    this.Owner.CallOnItemEntering(new ItemEnteringEventArgs(newIndex));
                }
            }
        }

        public void OnItemDataRemoved(int index, object itemData)
        {
            Range max = GetMaxRange(true);

            if (!max.IsInRange(index)) return;

            if (this.currentRange.IsInRange(index))
                UpdateAnimationOnItemInsertRemove(index, -1, null, double.NaN);
            else
                this.BitState[UpToDateStateKey] = false;

            SyncItemsAndUpdateCarousel(index);
        }

        public void OnItemDataSet(int index, object oldItemData, object newItemData)
        {
            Range max = GetMaxRange(true);

            for (int i = 0; i < this.currentRange.Length; i++)
            {
                int mappedIndex = this.mapper.MapFromRangeIndex(this.currentRange.ToRangeIndex(i));

                if (mappedIndex == index)
                {
                    if (i < this.Children.Count)
                    {
                        ((CarouselContentItem)this.Children[i]).HostedItem = (RadElement)newItemData;
                    }

                    break;
                }
            }
            this.BitState[UpToDateStateKey] = false;

            SyncItemsAndUpdateCarousel(this.selectedIndex);
        }

        public void OnItemsDataClear()
        {
            Reset();
        }

        void IVirtualViewport.OnItemsDataClearComplete()
        {
        }

        void IVirtualViewport.OnItemsDataSort()
        {
        }

        public void OnItemsDataSortComplete()
        {
            SyncItemsAndUpdateCarousel(0);
        }

        public void BeginUpdate()
        {
            this.update++;
        }

        public void EndUpdate()
        {
            if (this.update <= 0)
                return;

            if (--this.update == 0)
            {
                SyncItemsAndUpdateCarousel(this.selectedIndex);
            }
        }

        #endregion

        private Range GetMaxRange(bool normalize)
        {
            int visibleCount = this.CalculateVisibleItemsCount();

            this.mapper.ItemsCount = this.Items.Count;

            Range res = normalize ?
                this.mapper.Normalize(Range.CenterAt(0, visibleCount, this.selectedIndex)) :
                Range.CenterAt(0, visibleCount, this.selectedIndex);

            return res;
        }

        private int CalculateVisibleItemsCount()
        {
            if (!this.GetBitState(VirtualModeStateKey))
                return this.Items.Count;

            return Math.Max(Math.Min(this.visibleItemCount, this.Items.Count), 1);
        }

        protected int EnsureItemToChildIndex(int itemIndex)
        {
            RadElement element = (RadElement)this.items.GetVirtualData(itemIndex);

            int childIndex = this.Children.IndexOf(element);

            if ( childIndex != -1)
            {
                return childIndex;
            }
            CarouselContentItem childContainer = new CarouselContentItem(element);
            childContainer.SetOwner(this.owner);
            childContainer.AddBehavior(this.carouselItemBehavior);
            this.CarouselPath.InitializeItem(childContainer, this.AnimationsApplied);
            this.Children.Add(childContainer);

//#if Marker
//            RadButtonElement e = element as RadButtonElement;
//            if (e != null && e.Tag != null)
//            {
//                int i = (int)e.Tag;
//                CarouselPathAnimationData data = GetItemAnimationData(this.Children.Count - 1);
//                data.marker = i;
//            }
//#endif

            return this.Children.Count - 1;
        }

        protected int GetChildIndexFromItemIndex(int itemIndex)
        {
            RadElement element = (RadElement)this.items.GetVirtualData(itemIndex);

            return this.Children.IndexOf(element);
        }

        private void SynchronizeCollections()
        {
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            Range newCrange = this.mapper.Normalize(this.newRange);

            this.BitState[SpecialCaseStateKey] = newCrange.Length != this.Items.Count;

            SetUpItemsAnimationParameters();

            bool shouldAnimate = this.currentRange.Length + newRange.Length > 0;

            this.currentRange = newCrange;

            this.selectedIndex = this.mapper.Normalize(this.currentRange, this.selectedIndex);

            for (int i = 0; i < this.currentRange.Length; i++)
            {
                int mappedIndex = this.mapper.MapFromRangeIndex(this.currentRange.ToRangeIndex(i));
                int index = this.GetChildIndexFromItemIndex(mappedIndex);

                //temp fix
                if (index == -1) return;// throw new Exception("Reached item that is not pre-processed!");
                if (this.Children.Count <= index ) return;
                    //throw new Exception("Items in CarouselItemsContainer are not in synch with children!");                
                
                if (index != i)
                {
                    this.Children.Move(index, i);
                }
            }

            if (shouldAnimate)
                AnimateToPositions();
        }

        private void SetUpItemsAnimationParameters()
        {
            // Items removal:

            for (int i = 0; i < this.Children.Count; i++)
            {
                CarouselPathAnimationData data = GetItemAnimationData(i);
                data.ResetSpecialHandling();
                data.Interrupt = !data.IsOutAnimation();
            }

            Range removeThroughPathEnd = this.mapper.CreateLeft(this.currentRange, this.newRange); // Range.Intersection(this.currentRange, int.MinValue, newRange.From);

            for (int i = removeThroughPathEnd.From; i < removeThroughPathEnd.To; i++)
            {
                int itemIndex = this.mapper.MapFromRangeIndex(i);
                int index = GetChildIndexFromItemIndex(itemIndex);

                if (index != -1)
                {
                    // remove item: remove_through_path_end
                    CarouselPathAnimationData data = GetItemAnimationData(index);

                    data.Interrupt = !data.IsOutAnimation();
                    data.To = double.PositiveInfinity;

                    this.Owner.CallOnItemLeaving(new ItemLeavingEventArgs(itemIndex));
                }
            }

            Range removeThroughPathStart = this.mapper.CreateRight(this.currentRange, this.newRange); // Range.Intersection(this.currentRange, newRange.To, int.MaxValue);

            for (int i = removeThroughPathStart.From; i < removeThroughPathStart.To; i++)
            {
                int itemIndex = this.mapper.MapFromRangeIndex(i);
                int index = GetChildIndexFromItemIndex(itemIndex);

                if (index != -1)
                {
                    // remove item: remove_through_path_start
                    CarouselPathAnimationData data = GetItemAnimationData(index);

                    data.Interrupt = !data.IsOutAnimation();
                    data.To = double.NegativeInfinity;

                    this.Owner.CallOnItemLeaving(new ItemLeavingEventArgs(itemIndex));
                }
            }

            // Items addition:

            Range addThroughPathStart = this.mapper.CreateRight(this.newRange, this.currentRange);// Range.Intersection(newRange, this.currentRange.To, int.MaxValue);

            for (int i = addThroughPathStart.From; i < addThroughPathStart.To; i++)
            {
                int itemIndex = this.mapper.MapFromRangeIndex(i);

                int index = this.EnsureItemToChildIndex(itemIndex);

                if (index == -1)
                {
                    throw new Exception("EnsureItemToChildIndex failed!");
                }

                // add item: add_through_path_start
                CarouselPathAnimationData data = GetItemAnimationData(index);

                data.Interrupt = true;
                data.From = double.NegativeInfinity;
                data.UpdateSpecialHandling();

                this.Owner.CallOnItemEntering(new ItemEnteringEventArgs(itemIndex));
            }

            Range addThroughPathEnd = this.mapper.CreateLeft(this.newRange, this.currentRange); // Range.Intersection(newRange, int.MinValue, this.currentRange.From);

            for (int i = addThroughPathEnd.From; i < addThroughPathEnd.To; i++)
            {
                int itemIndex = this.mapper.MapFromRangeIndex(i);
                int index = this.EnsureItemToChildIndex(itemIndex);

                if (index == -1)
                {
                    throw new Exception("EnsureItemToChildIndex failed!");
                }

                // add item: add_through_path_end
                CarouselPathAnimationData data = GetItemAnimationData(index);

                data.Interrupt = true;
                data.From = double.PositiveInfinity;
                data.UpdateSpecialHandling();

                this.Owner.CallOnItemEntering(new ItemEnteringEventArgs(itemIndex));
            }
        }

        private CarouselPathAnimationData GetItemAnimationData(int index)
        {
            if (this.ElementState != ElementState.Loaded && this.ElementState != ElementState.Constructed)
            {
                return null;
            }

            return CarouselItemsContainer.GetItemAnimationData(this.Children[index]);
        }

        private static CarouselPathAnimationData GetItemAnimationData(RadObject element)
        {
            CarouselPathAnimationData data =
                (CarouselPathAnimationData)element.GetValue(CarouselItemsContainer.CarouselAnimationData);

            if (data == null)
            {
                data = new CarouselPathAnimationData();
                element.SetValue(CarouselItemsContainer.CarouselAnimationData, data);
            }

            return data;

        }

        private void SetPathCalculator(ICarouselPath value)
        {
            if (this.carouselPath != null)
                this.carouselPath.PropertyChanged -= new PropertyChangedEventHandler(PathCalculator_PropertyChanged);

            this.carouselPath = value;

            if (this.carouselPath != null)
                this.carouselPath.PropertyChanged += new PropertyChangedEventHandler(PathCalculator_PropertyChanged);
        }

        private void UpdateAnimationOnItemInsertRemove(int index, int change, double? from, double? to)
        {
            switch (index.CompareTo(this.selectedIndex))
            {
                case -1:
                    this.currentRange.Extend(change, 0);
                    break;

                case 0:
                case 1:
                    this.currentRange.Extend(0, change);
                    break;
            }

            CarouselPathAnimationData data = GetItemAnimationData(index);

            if (data != null)
            {
                data.From = from;
                data.To = to;
            }
        }

        private void SyncItemsAndUpdateCarousel(int index)
        {
            this.newRange = this.GetMaxRange(false);

            this.BitState[UpToDateStateKey] &= !(this.mapper.IsInRange(this.currentRange, index) || this.mapper.IsInRange(this.newRange, index));

            if (this.GetBitState(UpToDateStateKey) || this.update != 0)
                return;

            if (this.Items.Count > 0)
                SynchronizeCollections();
            else
                Reset();

            this.BitState[UpToDateStateKey] = true;
            this.SynchronizeCollections();
        }

        public virtual bool UpdateCarousel()
        {
            this.BitState[UpToDateStateKey] = false;

            if (this.update != 0)
                return false;

            if (this.items.Count > 0)
                SynchronizeCollections();
            else
                Reset();

            return true;
        }

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            base.OnBoundsChanged(e);
            this.ForceUpdate();
        }

        public virtual void ForceUpdate()
        {
            const double pathCenterOffset = 3.25777;

            PropertySetting pos = new PropertySetting(CarouselItemsContainer.CarouselLocationProperty, pathCenterOffset);

            for (int i = 0; i < this.currentRange.Length; i++)
            {
                int index = this.GetChildIndexFromItemIndex(
                    this.mapper.MapFromRangeIndex(this.currentRange.ToRangeIndex(i))
                    );

                if (index == -1) return;//throw new Exception("Reached item that is not pre-processed!");

                this.Children[index].ResetValue(CarouselItemsContainer.CarouselLocationProperty, ValueResetFlags.Animation);

                pos.ApplyValue(this.Children[index]);
            }
            this.UpdateCarousel();
        }

        private static bool AnimationRemoveCurrent(RadElement element, CarouselPathAnimationData data)
        {
            if (data == null)
                data = (CarouselPathAnimationData)element.GetValue(CarouselItemsContainer.CarouselAnimationData);

            if (data == null)
                return true;

            if (data.CurrentAnimation == null)
                return true;

            //TODO: fix?
            bool animating = true;// data.CurrentAnimation.IsAnimating(element);

            if (data.Interrupt /*&& animating*/)
            {
                data.CurrentAnimation.Stop(element);
                data.CurrentAnimation = null;
                animating = false;
            }

            return !animating;
        }

        private void Reset()
        {
            for (int i = this.Children.Count - 1; i >= 0; i--)
            {
                CarouselPathAnimationData data = GetItemAnimationData(i);
                data.Interrupt = true;
                if (data.CurrentAnimation != null)
                {
                    data.CurrentAnimation.Stop(this.Children[i]);
                }
            }

            this.Children.Clear();
            this.currentRange.SetRange(0, 0);
            this.newRange.SetRange(0, 0);
            this.selectedIndex = 0;
            this.BitState[UpToDateStateKey] = false;
            this.mapper.ItemsCount = 0;
        }

        private int CalculateClosest(int value)
        {
            return this.mapper.Closest(this.selectedIndex, value);
        }

        private void SetZIndex(RadElement element, int zIndex)
        {
            PropertySetting setting = new PropertySetting(
                VisualElement.ZIndexProperty,
                zIndex
            );

            setting.ApplyValue(element);
        }

        [Description("Get or set using the relative point coordinate. If set to true each point should be between 0 and 100")]
        public bool EnableRelativePath
        {
            set
            {
                this.CarouselPath.EnableRelativePath = value;
            }
            get
            {
                return this.CarouselPath.EnableRelativePath;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether carousel will increnment or decrement item indexes when in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating whether carousel will increnment or decrement item indexes when in auto-loop mode.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public AutoLoopDirections AutoLoopDirection
        {
            get { return autoLoopDirection; }
            set
            {
                autoLoopDirection = value;
                this.OnNotifyPropertyChanged("AutoLoopDirection");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the Carousel will loop items automatically
        /// </summary>
        [Description("Gets or sets a value indicating that the Carousel will loop items automatically.")]
        [DefaultValue(false)]
        public bool EnableAutoLoop
        {
            get
            {
                return this.GetBitState(EnableAutoLoopStateKey);
            }
            set
            {
                this.SetBitState(EnableAutoLoopStateKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating when carousel will pause looping if in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating when carousel will pause looping if in auto-loop mode.")]
        [DefaultValue(AutoLoopPauseConditions.OnMouseOverCarousel)]
        [Category("AutoLoopBehavior")]
        public AutoLoopPauseConditions AutoLoopPauseCondition
        {
            get { return this.autoLoopPauseCondition; }
            set
            {
                this.autoLoopPauseCondition = value;
                this.OnNotifyPropertyChanged("AutoLoopPauseCondition");
            }
        }

        protected override void DisposeManagedResources()
        {
            this.items.ItemsChanged -= new ItemChangedDelegate(this.OnItemsChanged);

            base.DisposeManagedResources();
        }
    }
}
