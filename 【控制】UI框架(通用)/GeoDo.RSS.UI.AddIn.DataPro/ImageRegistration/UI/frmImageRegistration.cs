using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmImageRegistration : Form
    {
        private ISpatialReference _spatial = null;
        private Size _imgSize = Size.Empty;

        public frmImageRegistration()
        {
            InitializeComponent();
        }

        public string Filename
        {
            get { return textBox1.Text; }
        }

        private void ShowInputFileDialog()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Filter = FileFilter.Image;
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = diag.FileName;
                    InputImage(file);
                }
            }
        }

        private void InputImage(string file)
        {
            textBox1.Text = file;
            Image img = Image.FromFile(file);
            pictureBox1.Image = img;
            _imgSize = img.Size;
            //textBox5.Text = _imgSize.Width.ToString();
            //textBox6.Text = _imgSize.Height.ToString();
            doubleTextBox3.Value = _imgSize.Width;
            doubleTextBox4.Value = _imgSize.Height;
        }

        private void ShowInputRegisterFileDialog()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.Filter = FileFilter.RegisterFile;
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = diag.FileName;
                    InputRegisterFile(file);
                }
            }
        }

        private void InputRegisterFile(string file)
        {
            string fileExt = Path.GetExtension(file);
            fileExt = fileExt.ToLower();
            switch (fileExt)
            {
                case ".ldf":
                    IRasterDataProvider prd = RasterDataDriver.Open(file) as IRasterDataProvider;
                    if (prd == null)
                        break;
                    _spatial = prd.SpatialRef;
                    textBox2.Text = file;
                    //textBox3.Text = prd.CoordEnvelope.MinX.ToString();
                    //textBox4.Text = prd.CoordEnvelope.MaxY.ToString();
                    //textBox7.Text = (prd.CoordEnvelope.Width / _imgSize.Width).ToString();
                    //textBox8.Text = (prd.CoordEnvelope.Height / _imgSize.Height).ToString();
                    doubleTextBox1.Value = prd.CoordEnvelope.MinX;
                    doubleTextBox2.Value = prd.CoordEnvelope.MaxY;
                    doubleTextBox5.Value = (prd.CoordEnvelope.Width / _imgSize.Width);
                    doubleTextBox6.Value = (prd.CoordEnvelope.Height / _imgSize.Height);
                    break;
                case ".hdr":
                    break;
                default:
                    break;
            }
        }

        private void Reset()
        {
            //textBox1.Text = "";
            //textBox2.Text = "";
            //textBox3.Text = "";
            //textBox4.Text = "";
            //textBox5.Text = "";
            //textBox6.Text = "";
            //textBox7.Text = "";
            //textBox8.Text = "";
            doubleTextBox1.Text = "";
            doubleTextBox2.Text = "";
            doubleTextBox3.Text = "";
            doubleTextBox4.Text = "";
            doubleTextBox5.Text = "";
            doubleTextBox6.Text = "";
        }

        private void WriteWorldFile()
        {
            double minx = doubleTextBox1.Value; //double.Parse(textBox3.Text);
            double maxy = doubleTextBox2.Value; //double.Parse(textBox4.Text);

            double xResolution = double.Parse(doubleTextBox5.Text);
            double yResolution = double.Parse(doubleTextBox6.Text);

            WorldFile worldFile = new WorldFile();
            worldFile.CreatWorldFile(xResolution, -yResolution, minx, maxy, Filename);
            worldFile.CreatXmlFile(_spatial == null ? SpatialReference.GetDefault() : _spatial, Filename);
        }

        private void btnInputFile_Click(object sender, EventArgs e)
        {
            ShowInputFileDialog();
        }

        private void btnInputRegisterFile_Click(object sender, EventArgs e)
        {
            ShowInputRegisterFileDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                WriteWorldFile();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
