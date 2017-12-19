namespace Telerik.WinControls.UI.ControlDefault
{
    partial class ControlDefault_Telerik_WinControls_UI_RadLabel
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
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting2 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector1 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting3 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting4 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector2 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ControlDefault_Telerik_WinControls_UI_RadLabel
            // 
            xmlPropertySettingGroup1.BasedOn = "SegoeUIDefaultText";
            xmlTypeSelector1.ElementType = "Telerik.WinControls.UI.RadLabel+RadLabelRootElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlPropertySettingGroup2.BasedOn = "SegoeUIDefaultText";
            xmlPropertySetting1.Property = "Telerik.WinControls.RadItem.UseDefaultDisabledPaint";
            xmlPropertySetting1.Value = false;
            xmlPropertySetting2.Property = "Telerik.WinControls.VisualElement.SmoothingMode";
            xmlPropertySetting2.Value = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            xmlPropertySettingGroup2.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting1,
            xmlPropertySetting2});
            xmlVisualStateSelector1.VisualState = "RadLabelElement";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector1});
            xmlPropertySettingGroup3.BasedOn = "SegoeUIDefaultDisabledText";
            xmlPropertySetting3.Property = "Telerik.WinControls.VisualElement.ForeColor";
            xmlPropertySetting3.Value = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(130)))), ((int)(((byte)(136)))));
            xmlPropertySetting4.Property = "Telerik.WinControls.VisualElement.SmoothingMode";
            xmlPropertySetting4.Value = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            xmlPropertySettingGroup3.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting3,
            xmlPropertySetting4});
            xmlVisualStateSelector2.VisualState = "RadLabelElement.Disabled";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector2});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadLabel";
            radStylesheetRelation1.ElementType = "Telerik.WinControls.UI.RadLabel+RadLabelRootElement";
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
