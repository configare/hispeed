using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Elements;
using System.ComponentModel;
using System.Diagnostics;
using Telerik.WinControls.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadCommandBarMultilineToolstripHolderPanel : RadCommandBarVisualElement, IItemsOwner
    {
        protected List<CommandBarRowElement> lines;
        protected RadItemOwnerCollection items;
        protected StackLayoutPanel stackLayout;

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.stackLayout = new StackLayoutPanel();
            this.stackLayout.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.items = new RadItemOwnerCollection();
            this.Items.ItemTypes = new Type[] { typeof(CommandBarStripElement) };           
            
            this.lines = new List<CommandBarRowElement>(1);
            
            this.AddLine();

            this.items = new RadItemOwnerCollection(this.stackLayout);
            this.Items.ItemTypes = new Type[] { typeof(CommandBarStripElement) };
            this.SyncStackLayoutWithLines();

            this.Children.Add(this.stackLayout);
        }

        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF constraint)
        {
           return base.MeasureOverride(constraint);
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF arrangeSize)
        {            
            return base.ArrangeOverride(arrangeSize);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the elements orientation inside the stacklayout. 
        /// Possible values are horizontal and vertical.
        /// </summary>
        [RadPropertyDefaultValue("Orientation", typeof(StackLayoutPanel))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public override Orientation Orientation
        {
            get
            {
                return this.stackLayout.Orientation;
            }
            set
            {
                this.stackLayout.Orientation = value;
            }
        }

        #endregion

        #region IItemsOwner Members

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        #endregion

        #region Helpers

        protected virtual void AddLine()
        {
            Debug.Assert(this.lines != null, "Lines cannot be null");

            //CommandBarRowElement newPanel = RadCommandBarToolstripsHolderFactory.CreateLayoutPanel(this);
           // this.lines.Add(newPanel);
        }

        protected virtual void SyncStackLayoutWithLines()
        {
            this.stackLayout.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.stackLayout.Children.Clear();
            foreach (CommandBarRowElement panel in lines)
            {
                this.stackLayout.Children.Add(panel);
            }

            this.InvalidateMeasure(true);
            this.UpdateLayout();
        }

        public virtual void MoveToUpperLine(CommandBarStripElement element, CommandBarRowElement currentHolder)
        {
            Debug.Assert(currentHolder.Children.Contains(element), "Current holder must contains element");
            int index = this.lines.IndexOf(currentHolder);
            Debug.Assert(index > -1, "Lines must contains currentHolder");

            int prevLineIndex = index - 1;
            if (prevLineIndex < 0)
            {
                //this.lines.Insert(0,RadCommandBarToolstripsHolderFactory.CreateLayoutPanel(this));
                prevLineIndex = 0;
            }

            currentHolder.Children.Remove(element);
            if (currentHolder.Children.Count == 0)
            {
                this.lines.Remove(currentHolder);
            }

            lines[prevLineIndex].Children.Add(element);
            
            SyncStackLayoutWithLines();
        }
        #endregion
    }    
}
