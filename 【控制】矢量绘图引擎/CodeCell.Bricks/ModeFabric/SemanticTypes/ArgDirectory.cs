using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CodeCell.Bricks.ModelFabric
{
    public class ArgDirectory : ArgRefType
    {
        public override object GetValue(object sender)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.SelectedPath;
                }
            }
            return base.GetValue(sender);
        }

        public override bool TryParse(string text, out object value)
        {
            value = null;
            if (string.IsNullOrEmpty(text))
                return false;
            try
            {
                if (Directory.Exists(text))
                {
                    value = text;
                    return true;
                }
            }
            catch 
            {
            }
            return false;
        }
    }
}
