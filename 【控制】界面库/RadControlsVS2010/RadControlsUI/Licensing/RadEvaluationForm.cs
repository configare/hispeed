using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI.Licensing
{
    public partial class RadEvaluationForm : RadForm
    {
        public RadEvaluationForm()
        {
            InitializeComponent();

            this.radLabelProductName.Text = "RadControls for WinForms";
            this.radLabelVersion.Text = String.Format("Version {0}", VersionNumber.Number);
            this.radLabelCopyright.Text = "Copyright © Telerik 2009";
            this.radLabelCompanyName.Text = "Telerik";

            this.BackColor = Color.FromArgb(79, 79, 79);

            this.FormElement.TitleBar.Children[1].Visibility = ElementVisibility.Collapsed;
            (this.FormElement.TitleBar.Children[0] as FillPrimitive).BackColor = Color.FromArgb(79, 79, 79);
            (this.FormElement.TitleBar.Children[0] as FillPrimitive).BackColor2 = Color.FromArgb(135,135,135);
            (this.FormElement.TitleBar.Children[0] as FillPrimitive).BackColor3 = Color.FromArgb(90,90,90);
            (this.FormElement.TitleBar.Children[0] as FillPrimitive).GradientStyle = GradientStyles.Linear;
            (this.FormElement.TitleBar.Children[0] as FillPrimitive).NumberOfColors = 3;
            (this.FormElement.TitleBar.TitlePrimitive).ForeColor = Color.White;
            

            this.FormElement.TitleBar.BackColor = Color.FromArgb(79, 79, 79);
            //(this.FormElement.TitleBar.Children[2].Children[1] as ImagePrimitive).Visibility =
            //    ElementVisibility.Collapsed;
        }

        private void RadEvaluationForm_Load(object sender, EventArgs e)
        {
            
        }

        private void radButtonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}