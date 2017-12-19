namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ScrollViewer
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
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting4 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlClassSelector xmlClassSelector1 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup2 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting5 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting6 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlClassSelector xmlClassSelector2 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ScrollViewer
            // 
            xmlPropertySettingGroup1.BasedOn = null;
            xmlPropertySetting1.Property = "Telerik.WinControls.VisualElement.BackColor";
            xmlPropertySetting1.Value = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(218)))), ((int)(((byte)(238)))));
            xmlPropertySetting2.Property = "Telerik.WinControls.Primitives.FillPrimitive.BackColor2";
            xmlPropertySetting2.Value = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(249)))), ((int)(((byte)(255)))));
            xmlPropertySetting3.Property = "Telerik.WinControls.Primitives.FillPrimitive.BackColor3";
            xmlPropertySetting3.Value = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(218)))), ((int)(((byte)(238)))));
            xmlPropertySetting4.Property = "Telerik.WinControls.Primitives.FillPrimitive.NumberOfColors";
            xmlPropertySetting4.Value = 3;
            xmlPropertySettingGroup1.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting1,
            xmlPropertySetting2,
            xmlPropertySetting3,
            xmlPropertySetting4});
            xmlClassSelector1.ElementClass = "RadScrollViewFill";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlClassSelector1});
            xmlPropertySettingGroup2.BasedOn = null;
            xmlPropertySetting5.Property = "Telerik.WinControls.VisualElement.ForeColor";
            xmlPropertySetting5.Value = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(176)))), ((int)(((byte)(216)))));
            xmlPropertySetting6.Property = "Telerik.WinControls.Primitives.BorderPrimitive.GradientStyle";
            xmlPropertySetting6.Value = Telerik.WinControls.GradientStyles.Solid;
            xmlPropertySettingGroup2.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting5,
            xmlPropertySetting6});
            xmlClassSelector2.ElementClass = "RadScrollViewBorder";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlClassSelector2});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadTestScrollView";
            radStylesheetRelation1.ElementType = "Telerik.WinControls.RootRadElement";
            xmlStyleBuilderRegistration1.StylesheetRelations.Add(radStylesheetRelation1);
            xmlTheme1.BuilderRegistrations = new Telerik.WinControls.XmlStyleBuilderRegistration[] {
        xmlStyleBuilderRegistration1};
            xmlTheme1.ThemeName = "ControlDefault, *";
            this.SerializableThemes.Add(xmlTheme1);

        }

        #endregion
    }
}
