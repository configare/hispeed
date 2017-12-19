namespace Telerik.WinControls.Themes.ControlDefault
{
    partial class ScrollablePanel
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
            Telerik.WinControls.XmlClassSelector xmlClassSelector1 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup2 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector2 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector3 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector2 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ScrollablePanel
            // 
            xmlPropertySettingGroup1.BasedOn = "TransparentBackground";
            xmlClassSelector1.ElementClass = "RadScrollablePanelFill";
            xmlTypeSelector1.ChildSelector = xmlClassSelector1;
            xmlTypeSelector1.ElementType = "Telerik.WinControls.UI.RadScrollablePanelElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlPropertySettingGroup2.BasedOn = "SegoeUIDefaultText,BlackText";
            xmlTypeSelector2.ElementType = "Telerik.WinControls.RootRadElement";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector2});
            xmlPropertySettingGroup3.BasedOn = "ButtonNormalBorder";
            xmlClassSelector2.ElementClass = "RadScrollablePanelBorder";
            xmlTypeSelector3.ChildSelector = xmlClassSelector2;
            xmlTypeSelector3.ElementType = "Telerik.WinControls.UI.RadScrollablePanelElement";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector3});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadScrollablePanel";
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
