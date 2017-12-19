using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The RadItem containing <see cref="RadRotatorItem"/>, Border and Fill primitives
    /// </summary>
    public class RadRotatorElement : RadItem
    {
        #region Private data

        private RadRotatorItem rotator;
        private FillPrimitive backFill;
        private BorderPrimitive border;

        #endregion

        #region Constructors and initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.ShouldHandleMouseInput = true;
        }

        /// <commentsfrom cref="RadElement.CreateChildElements" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        protected override void CreateChildElements()
        {
            rotator = new RadRotatorItem();
            border = new BorderPrimitive();
            backFill = new FillPrimitive();

            backFill.Visibility = ElementVisibility.Hidden;
            border.Visibility = ElementVisibility.Visible;

            border.ZIndex = int.MaxValue;
            backFill.ZIndex = int.MinValue;

            this.Children.Add(backFill);
            this.Children.Add(border);
            this.Children.Add(rotator);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="RadItem"/>s that will be rotated.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get { return this.rotator.Items; }
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
            get { return this.rotator.DefaultItem; }
            set { this.rotator.DefaultItem = value; }
        }

        /// <summary>
        /// Gets or Sets the interval between consequetive rotation animations.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(2000)]
        public int Interval
        {
            get { return this.rotator.Interval; }
            set { this.rotator.Interval = value; }
        }

        /// <summary>
        /// Gets or Sets the swap animation's frames number
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(10)]
        public int AnimationFrames
        {
            get { return this.rotator.AnimationFrames; }
            set { this.rotator.AnimationFrames = value; }
        }

        /// <summary>
        /// Gets or sets whether RadRotator should stop rotating on MouseOver
        /// </summary>
        [DefaultValue(true)]
        public bool ShouldStopOnMouseOver
        {
            get { return this.rotator.ShouldStopOnMouseOver; }
            set { this.rotator.ShouldStopOnMouseOver = value; }
        }

        /// <summary>
        /// Gets or Sets value indicating whether opacity will be animated when switching frames
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool OpacityAnimation
        {
            get { return this.rotator.OpacityAnimation; }
            set { this.rotator.OpacityAnimation = value; }
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
            get { return this.rotator.LocationAnimation; }
            set { this.rotator.LocationAnimation = value; }
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
            get { return this.rotator.CurrentIndex; }
            set { this.rotator.CurrentIndex = value; }
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <seealso cref="Goto(int)"/>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItem CurrentItem
        {
            get { return this.rotator.CurrentItem; }
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
            get { return this.rotator.Running; }
            set { this.rotator.Running = value; }
        }

        /// <summary>
        /// Gets the <see cref="RadRotatorItem"/> in the current <see cref="RadElement"/>
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRotatorItem RotatorItem
        {
            get
            {
                return this.rotator;
            }
        }

        #endregion

        #region Public methods

        /// <commentsfrom cref="RadRotatorItem.Start(bool)" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public bool Start(bool startImmediately)
        {
            return this.RotatorItem.Start(startImmediately);
        }

        /// <commentsfrom cref="RadRotatorItem.Stop" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public void Stop()
        {
            this.RotatorItem.Stop();
        }

        /// <commentsfrom cref="RadRotatorItem.Goto" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public bool Goto(int index)
        {
            return this.RotatorItem.Goto(index);
        }

        /// <commentsfrom cref="RadRotatorItem.GotoDefault" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public bool GotoDefault()
        {
            return this.RotatorItem.GotoDefault();
        }

        /// <commentsfrom cref="RadRotatorItem.Next" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public bool Next()
        {
            return this.RotatorItem.Next();
        }

        /// <commentsfrom cref="RadRotatorItem.Previous" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public bool Previous()
        {
            return this.RotatorItem.Previous();
        }

        #endregion

        #region Events

        /// <commentsfrom cref="RadRotatorItem.ItemClicked" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public event EventHandler ItemClicked
        {
            add { this.RotatorItem.ItemClicked += value; }
            remove { this.RotatorItem.ItemClicked -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.StartRotation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public event CancelEventHandler StartRotation
        {
            add { this.RotatorItem.StartRotation += value; }
            remove { this.RotatorItem.StartRotation -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.StopRotation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public event EventHandler StopRotation
        {
            add { this.RotatorItem.StopRotation += value; }
            remove { this.RotatorItem.StopRotation -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.BeginRotate" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public event BeginRotateEventHandler BeginRotate
        {
            add { this.RotatorItem.BeginRotate += value; }
            remove { this.RotatorItem.BeginRotate -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.EndRotate" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        public event EventHandler EndRotate
        {
            add { this.RotatorItem.EndRotate += value; }
            remove { this.RotatorItem.EndRotate -= value; }
        }

        #endregion

    }
}