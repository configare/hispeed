using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.Layout.Elements
{
    public partial class EditPictureElement : UserControl
    {
        private IElement _element;

        public EditPictureElement()
        {
            InitializeComponent();
        }
        public event Action<object, IElement> ElementChanged;

        public void SetElement(IElement element)
        {
            if (element is PictureElement)
            {
                _element = element;
                pictureBox1.Image = (_element as PictureElement).Bitmap;
            }
        }

        public IElement Element
        {
            get { return _element; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_element != null)
            {
                using (OpenFileDialog diag = new OpenFileDialog())
                {
                    diag.Filter = "png(*.png)|*.png|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg";
                    if (diag.ShowDialog() == DialogResult.OK)
                    {
                        string bmpFile = diag.FileName;
                        _element = new PictureElement(bmpFile);
                        pictureBox1.Image = new Bitmap(bmpFile);
                        if (ElementChanged != null)
                            ElementChanged(this, _element);
                    }
                }
            }
        }
    }
}
