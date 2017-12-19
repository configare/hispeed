namespace Telerik.WinControls.Themes.ControlDefault
{
    partial class ControlDefault_DockSplitContainer
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
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector1 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector1 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup2 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlClassSelector xmlClassSelector2 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup3 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlClassSelector xmlClassSelector3 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup4 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting2 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector2 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector1 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup5 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlPropertySetting xmlPropertySetting3 = new Telerik.WinControls.XmlPropertySetting();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector3 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlTypeSelector xmlTypeSelector2 = new Telerik.WinControls.XmlTypeSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup6 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector4 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector4 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.XmlPropertySettingGroup xmlPropertySettingGroup7 = new Telerik.WinControls.XmlPropertySettingGroup();
            Telerik.WinControls.XmlVisualStateSelector xmlVisualStateSelector5 = new Telerik.WinControls.XmlVisualStateSelector();
            Telerik.WinControls.XmlClassSelector xmlClassSelector5 = new Telerik.WinControls.XmlClassSelector();
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ControlDefault_DockSplitContainer
            // 
            xmlPropertySettingGroup1.BasedOn = null;
            xmlPropertySetting1.Property = "Telerik.WinControls.RadElement.ShouldPaint";
            xmlPropertySetting1.Value = false;
            xmlPropertySettingGroup1.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting1});
            xmlClassSelector1.ElementClass = "SplitterFill";
            xmlVisualStateSelector1.ChildSelector = xmlClassSelector1;
            xmlVisualStateSelector1.VisualState = "SplitContainerElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector1});
            xmlPropertySettingGroup2.BasedOn = "NoFill(Hidden)";
            xmlClassSelector2.ElementClass = "SplitContainerFill";
            xmlPropertySettingGroup2.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlClassSelector2});
            xmlPropertySettingGroup3.BasedOn = "NoBorder(Hidden)";
            xmlClassSelector3.ElementClass = "SplitContainerBorder";
            xmlPropertySettingGroup3.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlClassSelector3});
            xmlPropertySettingGroup4.BasedOn = "DockSplitGrip";
            xmlPropertySetting2.Property = "Telerik.WinControls.RadElement.ShouldPaint";
            xmlPropertySetting2.Value = true;
            xmlPropertySettingGroup4.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting2});
            xmlTypeSelector1.ElementType = "Telerik.WinControls.UI.SplitterElement";
            xmlVisualStateSelector2.ChildSelector = xmlTypeSelector1;
            xmlVisualStateSelector2.VisualState = "SplitContainerElement";
            xmlPropertySettingGroup4.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector2});
            xmlPropertySettingGroup5.BasedOn = "DockSplitGripVertical";
            xmlPropertySetting3.Property = "Telerik.WinControls.RadElement.ShouldPaint";
            xmlPropertySetting3.Value = true;
            xmlPropertySettingGroup5.PropertySettings.AddRange(new Telerik.WinControls.XmlPropertySetting[] {
            xmlPropertySetting3});
            xmlTypeSelector2.ElementType = "Telerik.WinControls.UI.SplitterElement";
            xmlVisualStateSelector3.ChildSelector = xmlTypeSelector2;
            xmlVisualStateSelector3.VisualState = "SplitContainerElement.IsVertical";
            xmlPropertySettingGroup5.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector3});
            xmlPropertySettingGroup6.BasedOn = "NoFill(Hidden)";
            xmlClassSelector4.ElementClass = "SplitterFill";
            xmlVisualStateSelector4.ChildSelector = xmlClassSelector4;
            xmlVisualStateSelector4.VisualState = "SplitContainerElement";
            xmlPropertySettingGroup6.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector4});
            xmlPropertySettingGroup7.BasedOn = "NoFill(Hidden)";
            xmlClassSelector5.ElementClass = "SplitterFill";
            xmlVisualStateSelector5.ChildSelector = xmlClassSelector5;
            xmlVisualStateSelector5.VisualState = "SplitContainerElement.IsVertical";
            xmlPropertySettingGroup7.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlVisualStateSelector5});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1,
            xmlPropertySettingGroup2,
            xmlPropertySettingGroup3,
            xmlPropertySettingGroup4,
            xmlPropertySettingGroup5,
            xmlPropertySettingGroup6,
            xmlPropertySettingGroup7});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "DockSplitContainer";
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
