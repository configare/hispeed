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
    public partial class EditTextElement : UserControl
    {
        private IElement _element;

        public EditTextElement()
        {
            InitializeComponent();
        }

        public event Action<object, IElement> ElementChanged;

        public void SetElement(IElement element)
        {
            if (element is TextElement)
            {
                _element = element;
                textBox1.Text = (element as TextElement).Text;
            }
        }

        public IElement Element
        {
            get { return _element; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_element != null)
            {
                (_element as TextElement).Text = textBox1.Text;
                if (ElementChanged != null)
                    ElementChanged(this, _element as TextElement);
            }
        }
    }
}
