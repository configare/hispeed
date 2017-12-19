namespace Telerik.WinControls.Themes.ControlDefault
{
    partial class ControlDefault_RadPageView_Telerik_WinControls_RootRadElement
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
            Telerik.WinControls.RadStylesheetRelation radStylesheetRelation1 = new Telerik.WinControls.RadStylesheetRelation();
            // 
            // ControlDefault_RadPageView_Telerik_WinControls_RootRadElement
            // 
            xmlPropertySettingGroup1.BasedOn = "SegoeUIDefaultText";
            xmlTypeSelector1.ElementType = "Telerik.WinControls.RootRadElement";
            xmlPropertySettingGroup1.Selectors.AddRange(new Telerik.WinControls.XmlElementSelector[] {
            xmlTypeSelector1});
            xmlStyleSheet1.PropertySettingGroups.AddRange(new Telerik.WinControls.XmlPropertySettingGroup[] {
            xmlPropertySettingGroup1});
            xmlStyleBuilderRegistration1.BuilderData = xmlStyleSheet1;
            xmlStyleBuilderRegistration1.BuilderType = typeof(Telerik.WinControls.DefaultStyleBuilder);
            radStylesheetRelation1.ControlType = "Telerik.WinControls.UI.RadPageView";
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
