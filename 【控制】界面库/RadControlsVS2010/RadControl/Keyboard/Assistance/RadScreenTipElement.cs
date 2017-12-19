using System;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RadScreenTipElement : RadItem, IScreenTipContent
    {
        #region Fields

        private string desc = "Override this property and provide custom screentip template description in DesignTime.";
        private RadItemOwnerCollection items;
        private Size tipSize = new Size(210, 50);
        private bool enableCustomSize = true;
        private Type parentType;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RadScreenTipElement"/> class.
        /// </summary>
        public RadScreenTipElement()
        {

        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadItem) };
            this.items.DefaultType = typeof(RadItem);
        }

        protected override void CreateChildElements()
        {
            this.items.Owner = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets value indicating whether Office 2007 UI compliant screen tip sizing should be used (value of false)
        /// </summary>
        [DefaultValue(true)]
        public bool EnableCustomSize
        {
            get { return enableCustomSize; }
            set { enableCustomSize = value; }
        }

        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadNewItem("Type here", true)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        public RadItemReadOnlyCollection TipItems
        {
            get
            {
                return new RadItemReadOnlyCollection(this.Items);
            }
        }



        /// <summary>
        /// Override this property and provide custom screentip template description in DesignTime
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual string Description
        {
            get
            {
                return desc;
            }
            set
            {
                desc = value;
            }
        }

        /// <summary>
        /// Gets the screen tip actual template type. Used for component serialization.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Type TemplateType
        {
            get
            {
                if (this.ElementTree != null)
                {
                    return (this.ElementTree.Control as RadScreenTip).TemplateType;
                }
                return parentType;
            }
            set
            {
                parentType = value;
            }
        }

        /// <summary>
        /// Gets a value indicating screen tip preset size.
        /// </summary>
        public virtual Size TipSize
        {
            get
            {
                if (EnableCustomSize)
                {
                    return tipSize;
                }
                return Size.Empty;
            }
            set
            {
                tipSize = value;
            }
        }

        #endregion

    }
}
