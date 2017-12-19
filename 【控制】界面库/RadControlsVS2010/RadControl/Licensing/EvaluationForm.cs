using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
	internal partial class EvaluationForm : Form
	{
		internal EvaluationForm()
		{
			InitializeComponent();

			this.labelProductName.Text = "RadControls for WinForms";
			this.labelVersion.Text = String.Format("Version {0}", VersionNumber.Number);
			this.labelCopyright.Text = Telerik.WinControls.VersionNumber.CopyrightText;
			this.labelCompanyName.Text = "Telerik";
		}
	}
}