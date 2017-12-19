using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Collections;
using Telerik.WinControls.UI;
using System.Diagnostics;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// Defines methods and properties for a collapsible element. For example, 
    /// RadRibonBarChunk is a collapsible element.
    /// </summary>
    public abstract class CollapsibleElement : RadItem
    {
        //private Dictionary<int, SizeF> stateSizeTable = new Dictionary<int, SizeF>(4);
        private SizeF sizeAfterCollapsing;
        private SizeF sizeBeforeCollapsing;
        protected int collapseStep = 1;
        protected bool allowCollapsed = true;

        #region State sizes initialization
       

        protected void ResetStateSizes()
        {
            for (int i = 0; i < this.CollapseMaxSteps; i++)
            {
                if (this.CanCollapseToStep(i + 1))
                {
                    this.CollapseElementToStep(i + 1);                   
                }
            }

            for (int i = this.CollapseMaxSteps - 1 ; i >0; i--)            
            {
                this.ExpandElementToStep( i );
                this.CollapseStep = i;
            }
        }

        #endregion

        #region ICollapsibleElement Members

        public abstract bool ExpandElementToStep(int collapseStep);

        public abstract bool CollapseElementToStep(int collapseStep);


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public abstract int CollapseMaxSteps
        {
            get;
        }

        [DefaultValue(1)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CollapseStep
        {
            get
            {
                return this.collapseStep;
            }
            set
            {
                if (value >= 1 && value <= CollapseMaxSteps)
                {
                    this.collapseStep = value;                                        
                }
            }
        }

        [DefaultValue(true)]
        [Browsable(false)]       
        public bool AllowCollapsed
        {
            get
            {
                return allowCollapsed;
            }
            set
            {
                allowCollapsed = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF SizeAfterCollapsing
        {
            get { return sizeAfterCollapsing; }
            set { sizeAfterCollapsing = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF SizeBeforeCollapsing
        {
            get { return sizeBeforeCollapsing; }
            set { sizeBeforeCollapsing = value; }
        }

        #endregion

        #region Expandable helpers
        protected bool invalidateCollapsableChildrenCollection = true;
        public IList<CollapsibleElement> collapsableChildren = new List<CollapsibleElement>();

        protected void FillCollapsableChildrenCollection(RadElement baseItem)
        {
            foreach (RadElement item in baseItem.Children)
            {
                CollapsibleElement collapsibleElement = CollapsibleAdapterFactory.CreateAdapter(item);

                if (collapsibleElement != null)
                {
                    collapsableChildren.Add(collapsibleElement);
                }
                else
                {
                    FillCollapsableChildrenCollection(item);
                }
            }
        }

        protected virtual bool CollapseCollection(int nextStep)
        {
            bool result = false;
            foreach (CollapsibleElement item in this.collapsableChildren)
            {
                bool itemCollapseResult = item.CollapseElementToStep(nextStep);   
                result |= itemCollapseResult;
            }
            this.CollapseStep = nextStep;
            //this.ExpandedSize = this.DesiredSize.ToSize();
            return result;
        }

        protected virtual bool ExpandCollection(int nextStep)
        {
            bool result = false;
            foreach (CollapsibleElement item in this.collapsableChildren)
            {
                bool itemExpandResult = item.ExpandElementToStep(nextStep);
                result |= itemExpandResult;
            }            
            this.CollapseStep=nextStep;
            //this.ExpandedSize = this.DesiredSize.ToSize();
            return result;
        }
        #endregion

#region CanCollapse
        //abstract public bool CanCollapseMore();

        abstract public bool CanCollapseToStep(int nextStep);
        abstract public bool CanExpandToStep(int nextStep);
        
#endregion



        protected virtual void InvalidateIfNeeded()
        {
            if (this.invalidateCollapsableChildrenCollection)
            {
                this.invalidateCollapsableChildrenCollection = false;
                this.collapsableChildren.Clear();
                this.FillCollapsableChildrenCollection(this);
                //this.ResetStateSizes();
            }
        }
        
    }
}
