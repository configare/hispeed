using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls.UI
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class RibbonBarLocalizationSettings
    {
        private RadRibbonBarElement ribbonBarElement;

        private string showQuickAccessMenuBelowItemText = "显示在工具栏下方";
        private string showQuickAccessMenuAboveItemText = "显示在工具栏上方";
        private string minimizeRibbonItemText = "收起工具栏";
        private string maximizeRibbonItemText = "展开工具栏";

        public RibbonBarLocalizationSettings(RadRibbonBarElement ribbonBarElement)
        {
            this.ribbonBarElement = ribbonBarElement;
        }

        [DefaultValue("显示在工具栏下方"), Localizable(true)]
        public string ShowQuickAccessMenuBelowItemText
        {
            get{ return showQuickAccessMenuBelowItemText; }
            set{ showQuickAccessMenuBelowItemText = value; }
        }

        [DefaultValue("显示在工具栏上方"), Localizable(true)]
        public string ShowQuickAccessMenuAboveItemText
        {
            get { return showQuickAccessMenuAboveItemText; }
            set { showQuickAccessMenuAboveItemText = value; }
        }

        [DefaultValue("收起工具栏"), Localizable(true)]
        public string MinimizeRibbonItemText
        {
            get { return minimizeRibbonItemText; }
            set { minimizeRibbonItemText = value; }
        }

        [DefaultValue("展开工具栏"), Localizable(true)]
        public string MaximizeRibbonItemText
        {
            get { return maximizeRibbonItemText; }
            set { maximizeRibbonItemText = value; }
        }
    }
}
