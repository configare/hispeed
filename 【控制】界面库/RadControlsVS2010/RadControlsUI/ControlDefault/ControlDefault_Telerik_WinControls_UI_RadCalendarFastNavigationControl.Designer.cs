namespace Telerik.WinControls.Themes.ControlDefault
{
    partial class ControlDefault_Telerik_WinControls_UI_RadCalendarFastNavigationControl
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
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting1 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting2 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting3 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector1 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup2 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting4 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting5 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector2 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting6 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector1 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ControlDefault_Telerik_WinControls_UI_RadCalendarFastNavigationControl
            // 
            xmlPropertySettingGroup1.BasedOn = "TransparentBackground,TransparentBorder";
            xmlPropertySetting1.Property = "Telerik.WinControls.UI.LightVisualElement.BorderBoxStyle";
            xmlPropertySetting1.Value = Telerik.WinControls.BorderBoxStyle.OuterInnerBorders;
            xmlPropertySetting2.Property = "Telerik.WinControls.UI.LightVisualElement.BorderColor";
            xmlPropertySetting2.Value = System.Drawing.Color.White;
            xmlPropertySetting3.Property = "Telerik.WinControls.VisualElement.Font";
            xmlPropertySetting3.Value = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
            xmlPropertySettingGroup1.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting1,
            xmlPropertySetting2,
            xmlPropertySetting3});
            xmlVisualStateSelector1.VisualState = "FastNavigationItem";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector1});
            xmlPropertySettingGroup2.BasedOn = "CalNavigationItemFill,TransparentBorder";
            xmlPropertySetting4.Property = "Telerik.WinControls.RadElement.Shape";
            xmlPropertySetting4.Value = null;
            xmlPropertySetting5.Property = "Telerik.WinControls.UI.LightVisualElement.BorderColor";
            xmlPropertySetting5.Value = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(140)))), ((int)(((byte)(60)))));
            xmlPropertySettingGroup2.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting4,
            xmlPropertySetting5});
            xmlVisualStateSelector2.VisualState = "FastNavigationItem.Selected";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector2});
            xmlPropertySettingGroup3.BasedOn = "NavigationElementBorder,WhiteBackground";
            xmlPropertySetting6.Property = "Telerik.WinControls.RadElement.Shape";
            xmlPropertySetting6.Value = null;
            xmlPropertySettingGroup3.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting6});
            xmlTypeSelector1.ElementType = "Telerik.WinControls.UI.RadCalendarFastNavigationElement";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadCalendarFastNavigationControl";
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
