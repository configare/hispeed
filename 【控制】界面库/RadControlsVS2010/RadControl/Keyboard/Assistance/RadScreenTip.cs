using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Elements;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    [Designer(DesignerConsts.RadControlDesignerString)]
    [Designer(DesignerConsts.RadScreenTipDocumentDesignerString, typeof(IRootDesigner))]
	//[Designer(typeof(RadControlDesigner),typeof(ComponentDesigner))]
	//[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public abstract class RadScreenTip : RadControl, IScreenTipContent
	{
		static RadScreenTip()
		{
			ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.ScreenTipThemes.Office2007Silver.xml");
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(210,150);
			}
		}

        /// <summary>
        /// Gets the instance of RadScreenTipElement wrapped by this control. RadScreenTipElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadScreenTip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public abstract RadScreenTipElement ScreenTipElement
        {
            get;
        }

		[RadEditItemsAction]
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.ScreenTipElement.Items;
			}
		}

        #region IScreenTipContent Members

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadItemReadOnlyCollection TipItems
		{
			get 
			{
                return this.ScreenTipElement.TipItems; 
			}
		}

		//TODO: "Override this property and provide custom screentip template description in DesignTime.";
		public virtual string Description
		{
			get
			{
                return this.ScreenTipElement.Description;
			}
			set
			{
                this.ScreenTipElement.Description = value;
			}
		}

		public virtual Size TipSize
		{
			get 
			{
                return this.ScreenTipElement.TipSize;
			}
		}

        public abstract Type TemplateType { get; set; }

		#endregion
	}
}
