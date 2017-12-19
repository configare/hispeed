using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.OldShapeEditor
{
    public partial class CustomShapeEditorForm : Form
    {
        public RadShapeEditorControl EditorControl
        {
            get { return this.radShapeEditorControl1; }
        }

        public CustomShapeEditorForm()
        {
            InitializeComponent();
            this.radShapeEditorControl1.propertyGrid = this.propertyGrid1;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            MessageBox.Show("blabla");
        }
    }
}