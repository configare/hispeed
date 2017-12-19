using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public static class SaveMapDialog
    {
        public static string Save(IMap map, bool useRelativePath)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = Constants.cstMapFileFilter;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    map.SaveTo(dlg.FileName, useRelativePath);
                    return dlg.FileName;
                }
            }
            return null;
        }
    }
}
