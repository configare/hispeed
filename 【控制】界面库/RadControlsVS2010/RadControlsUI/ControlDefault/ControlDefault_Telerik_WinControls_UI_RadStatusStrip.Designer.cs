namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class StatusStrip
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
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting1 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector2 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector1 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector3 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup4 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting2 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector4 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector2 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup5 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting3 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector5 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector3 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // StatusStrip
            // 
            xmlPropertySettingGroup1.BasedOn = "NormalButtonText,SegoeUIDefaultText";
            xmlTypeSelector1.ElementType = "Telerik.WinControls.RootRadElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlPropertySettingGroup2.BasedOn = "GripImage";
            xmlPropertySetting1.Property = "Telerik.WinControls.RadElement.Margin";
            xmlPropertySetting1.Value = new System.Windows.Forms.Padding(-3, -3, 0, 0);
            xmlPropertySettingGroup2.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting1});
            xmlClassSelector1.ElementClass = "GripImage";
            xmlTypeSelector2.ChildSelector = xmlClassSelector1;
            xmlTypeSelector2.ElementType = "Telerik.WinControls.UI.RadStatusBarElement";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector2});
            xmlPropertySettingGroup3.BasedOn = "RoundRectShape5Bottom";
            xmlTypeSelector3.ElementType = "Telerik.WinControls.UI.RadStatusBarElement";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector3});
            xmlPropertySettingGroup4.BasedOn = "StatusStripActiveFill";
            xmlPropertySetting2.Property = "Telerik.WinControls.RadElement.Margin";
            xmlPropertySetting2.Value = new System.Windows.Forms.Padding(-1, 0, -1, -1);
            xmlPropertySettingGroup4.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting2});
            xmlClassSelector2.ElementClass = "StatusBarFill";
            xmlTypeSelector4.ChildSelector = xmlClassSelector2;
            xmlTypeSelector4.ElementType = "Telerik.WinControls.UI.RadStatusBarElement";
            xmlPropertySettingGroup4.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector4});
            xmlPropertySettingGroup5.BasedOn = "StatusBarBorder";
            xmlPropertySetting3.Property = "Telerik.WinControls.VisualElement.ForeColor";
            xmlPropertySetting3.Value = System.Drawing.Color.White;
            xmlPropertySettingGroup5.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting3});
            xmlClassSelector3.ElementClass = "StatusBarBorder";
            xmlTypeSelector5.ChildSelector = xmlClassSelector3;
            xmlTypeSelector5.ElementType = "Telerik.WinControls.UI.RadStatusBarElement";
            xmlPropertySettingGroup5.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector5});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3,
            xmlPropertySettingGroup4,
            xmlPropertySettingGroup5});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadStatusStrip";
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
