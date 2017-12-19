using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This control is transfers the web-based rotators' functionality to the Windows forms work space.
    /// </summary>
    [Designer(DesignerConsts.RadRotatorDesignerString)]
    [ToolboxItem(true)]
    [RadToolboxItem(false)]
    [Description("A multipurpose component for content rotation and customization")]
	[DefaultProperty("Items"), DefaultEvent("ItemClicked"), Docking(DockingBehavior.Ask)]
	public class RadRotator : RadControl
    {
        #region Private members

        private RadRotatorElement rotatorElement;

        #endregion

        #region Constructors & Initialization

        /// <summary>
        /// Initializes the RadRotator control
        /// </summary>
        public RadRotator()
        {
            // PATCH - for double click in design-time
            Size sz = this.DefaultSize;
            this.ElementTree.PerformInnerLayout(true, 0, 0, sz.Width, sz.Height);
        }


        #endregion

        /// <summary>
        /// Initializes the Childs Items
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.rotatorElement = new RadRotatorElement();

            parent.Children.Add(rotatorElement);
        }

        #region Properties

        /// <summary>
        /// Gets the instance of RadRotatorElement wrapped by this control. RadRotatorElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadRotator.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadRotatorElement RotatorElement
        {
            get
            {
                return this.rotatorElement;
            }
        }

        /// <summary>
        /// Gets or sets whether RadRotator should stop rotating on MouseOver
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true), Description("Gets or sets whether RadRotator should stop rotating on MouseOver")]
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        public bool ShouldStopOnMouseOver
        {
            get { return this.RotatorElement.RotatorItem.ShouldStopOnMouseOver; }
            set { this.RotatorElement.RotatorItem.ShouldStopOnMouseOver = value; }
        }

        // / <summary>
        // / Retrieves the <see cref="RadRotatorElement"/> contained into the control
        // / </summary>
        // / <seealso cref="RadRotatorElement"/>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private RadRotatorItem RotatorItem
        {
            get
            {
                return this.rotatorElement.RotatorItem;
            }
        }

        /// <commentsfrom cref="RadRotatorItem.Items" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.Items"/>
        /// <seealso cref="RadRotatorItem"/>
        /// <seealso cref="DefaultItem"/>
        /// <seealso cref="Goto"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Stop"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        public RadItemCollection Items
        {
            get { return this.RotatorItem.Items; }
        }

        /// <commentsfrom cref="RadRotatorItem.DefaultItem" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.DefaultItem"/>
        /// <seealso cref="Items"/>
        /// <seealso cref="GotoDefault"/>
        /// <seealso cref="Goto"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Stop"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), Category(RadDesignCategory.DataCategory)]
        [RadNewItem("DefaultItem", false), DefaultValue(null)]
        public RadItem DefaultItem
        {
            get { return this.RotatorItem.DefaultItem; }
            set { this.RotatorItem.DefaultItem = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.Interval" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.Interval"/>
        /// <seealso cref="RadRotatorItem"/>
        /// <seealso cref="AnimationFrames"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Stop"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(2000)]
        public int Interval
        {
            get { return this.RotatorItem.Interval; }
            set { this.RotatorItem.Interval = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.AnimationFrames" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.AnimationFrames"/>
        /// <seealso cref="RadRotatorItem"/>
        /// <seealso cref="Interval"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Stop"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(10)]
        public int AnimationFrames
        {
            get { return this.RotatorItem.AnimationFrames; }
            set { this.RotatorItem.AnimationFrames = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.OpacityAnimation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.OpacityAnimation"/>
        /// <seealso cref="LocationAnimation"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool OpacityAnimation
        {
            get { return this.RotatorItem.OpacityAnimation; }
            set { this.RotatorItem.OpacityAnimation = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.LocationAnimation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.LocationAnimation"/>
        /// <seealso cref="OpacityAnimation"/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [ExtenderProvidedProperty]
        public SizeF LocationAnimation
        {
            get { return this.RotatorItem.LocationAnimation; }
            set { this.RotatorItem.LocationAnimation = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.CurrentIndex" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.CurrentIndex"/>
        /// <seealso cref="RadRotatorItem"/>
        /// <seealso cref="CurrentItem"/>
        /// <seealso cref="Goto"/>
        /// <seealso cref="GotoDefault"/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentIndex
        {
            get { return this.RotatorItem.CurrentIndex; }
            set { this.RotatorItem.CurrentIndex = value; }
        }

        /// <commentsfrom cref="RadRotatorItem.CurrentItem" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.CurrentItem"/>
        /// <seealso cref="CurrentIndex"/>
        /// <seealso cref="RadRotatorItem"/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadItem CurrentItem
        {
            get { return this.RotatorItem.CurrentItem; }
        }

        /// <commentsfrom cref="RadRotatorItem.Running" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Stop"/>
        public bool Running
        {
            get { return this.RotatorItem.Running; }
            set { this.RotatorItem.Running = value; }
        }

        /// <commentsfrom cref="System.Windows.Forms.Control.DefaultSize" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        protected override Size DefaultSize
        {
            get { return new Size(200, 180); }
        }

        #endregion

        #region Public methods

        /// <commentsfrom cref="RadRotatorElement.Start(bool)" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorElement.Start(bool)"/>
        /// <seealso cref="Start()"/>
        /// <seealso cref="Stop"/>
        public bool Start(bool startImmediately)
        {
            return this.RotatorItem.Start(startImmediately);
        }

        /// <commentsfrom cref="RadRotatorElement.Start" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <returns></returns>
        /// <seealso cref="RadRotatorElement.Start"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Stop"/>
        public bool Start()
        {
            return this.Start(false);
        }

        /// <commentsfrom cref="RadRotatorElement.Stop" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorElement.Stop"/>
        /// <seealso cref="Start(bool)"/>
        /// <seealso cref="Start()"/>
        public void Stop()
        {
            this.RotatorItem.Stop();
        }

        /// <commentsfrom cref="RadRotatorElement.Goto(int)" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorElement.Goto(int)"/>
        /// <seealso cref="GotoDefault"/>
        /// <seealso cref="Previous"/>
        /// <seealso cref="Next"/>
        public bool Goto(int index)
        {
            return this.RotatorItem.Goto(index);
        }

        /// <commentsfrom cref="RadRotatorElement.GotoDefault" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorElement.GotoDefault"/>
        /// <seealso cref="Goto(int)"/>
        /// <seealso cref="Previous"/>
        /// <seealso cref="Next"/>
        public void GotoDefault()
        {
            this.RotatorItem.GotoDefault();
        }

        /// <commentsfrom cref="RadRotatorElement.Previous" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>        /// 
        /// <seealso cref="RadRotatorElement.Previous"/>
        /// <seealso cref="Next"/>
        /// <seealso cref="Goto"/>
        /// <seealso cref="GotoDefault"/>
        public bool Previous()
        {
            return this.RotatorItem.Previous();
        }

        /// <commentsfrom cref="RadRotatorElement.Next" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>        /// 
        /// <seealso cref="RadRotatorElement.Next"/>
        /// <seealso cref="Previous"/>
        /// <seealso cref="Goto"/>
        /// <seealso cref="GotoDefault"/>
        public bool Next()
        {
            return this.RotatorItem.Next();
        }

        #endregion

        #region Events

        /// <commentsfrom cref="RadRotatorItem.ItemClicked" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.ItemClicked"/>
        public event EventHandler ItemClicked
        {
            add { this.RotatorItem.ItemClicked += value; }
            remove { this.RotatorItem.ItemClicked -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.StartRotation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.StartRotation"/>
        /// <seealso cref="StopRotation"/>
        /// <seealso cref="BeginRotate"/>
        /// <seealso cref="EndRotate"/>
        public event CancelEventHandler StartRotation
        {
            add { this.RotatorItem.StartRotation += value; }
            remove { this.RotatorItem.StartRotation -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.StopRotation" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.StopRotation"/>
        /// <seealso cref="StartRotation"/>
        /// <seealso cref="BeginRotate"/>
        /// <seealso cref="EndRotate"/>
        public event EventHandler StopRotation
        {
            add { this.RotatorItem.StopRotation += value; }
            remove { this.RotatorItem.StopRotation -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.BeginRotate" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadRotatorItem.BeginRotate"/>
        /// <seealso cref="EndRotate"/>
        /// <seealso cref="StartRotation"/>
        /// <seealso cref="StopRotation"/>
        public event BeginRotateEventHandler BeginRotate
        {
            add { this.RotatorItem.BeginRotate += value; }
            remove { this.RotatorItem.BeginRotate -= value; }
        }

        /// <commentsfrom cref="RadRotatorItem.EndRotate" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <see cref="RadRotatorItem.EndRotate"/>
        /// <seealso cref="BeginRotate"/>
        /// <seealso cref="StartRotation"/>
        /// <seealso cref="StopRotation"/>
        public event EventHandler EndRotate
        {
            add { this.RotatorItem.EndRotate += value; }
            remove { this.RotatorItem.EndRotate -= value; }
        }

        #endregion
    }
}