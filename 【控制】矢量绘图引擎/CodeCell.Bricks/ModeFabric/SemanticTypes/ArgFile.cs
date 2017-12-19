using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgFile : ArgRefType
    {
        public override object GetValue(object sender)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = GetFilter();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.FileName;
                }
            }
            return null;
        }

        protected virtual string GetFilter()
        {
            return string.Empty;
        }

        public override bool TryParse(string text, out object value)
        {
            value = text;
            return true;
        }
    }
}
