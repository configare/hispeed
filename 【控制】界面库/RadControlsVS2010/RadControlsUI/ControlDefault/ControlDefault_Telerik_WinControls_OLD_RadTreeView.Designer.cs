namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class OLDTreeView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.WinControls.XmlTheme xmlTheme1 = new Telerik.WinControls.XmlTheme();
            Telerik.WinControls.XmlStyleBuilderRegistration xmlStyleBuilderRegistration1 = new Telerik.WinControls.XmlStyleBuilderRegistration();
            Telerik.WinControls.XmlStyleSheet xmlStyleSheet1 = new Telerik.WinControls.XmlStyleSheet();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup1 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector1 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup2 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector1 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector2 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup4 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector3 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup5 = new Telerik.WinControls.XmlPropertySettingGroup();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeView));
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup6 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector4 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // TreeView
            // 
            xmlPropertySettingGroup1.BasedOn = "SegoeUIDefaultText,BlackText";
            xmlTypeSelector1.ElementType = "Telerik.WinControls.RootRadElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlPropertySettingGroup2.BasedOn = "TreeViewNodeMouseOverFill,TreeViewNodeMouseOverBorder";
            xmlVisualStateSelector1.VisualState = "TreeNodeUI.MouseOver";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector1});
            xmlPropertySettingGroup3.BasedOn = "NodeSelectedBorder,TreeViewNodeSelectedFill";
            xmlVisualStateSelector2.VisualState = "TreeNodeUI.Selected";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector2});
            xmlPropertySettingGroup4.BasedOn = "TransparentBorder,TransparentBackground";
            xmlVisualStateSelector3.VisualState = "TreeNodeUI";
            xmlPropertySettingGroup4.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector3});
            xmlPropertySettingGroup6.BasedOn = "NodeSelectedBorder,TreeViewNodeSelectedFill";
            xmlVisualStateSelector4.VisualState = "TreeNodeUI.Selected.MouseOver";
            xmlPropertySettingGroup6.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector4});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3,
            xmlPropertySettingGroup4,
            xmlPropertySettingGroup5,
            xmlPropertySettingGroup6});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.OLD.RadTreeView";
            radStylesheetRelation1.ElementType = "Telerik.WinControls.RootRadElement";
            xmlStyleBuilderRegistration1.StylesheetRelations.Add(radStylesheetRelation1);
            xmlTheme1.BuilderRegistrations = new Telerik.WinControls.XmlStyleBuilderRegistration[] {
        xmlStyleBuilderRegistration1};
            xmlTheme1.ThemeName = "ControlDefault,*";
            xmlTheme1.ThemeVersion = "2.0";
            this.SerializableThemes.Add(xmlTheme1);

        }

        #endregion

    }
}
