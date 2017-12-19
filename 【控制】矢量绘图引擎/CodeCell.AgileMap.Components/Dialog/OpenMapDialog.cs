using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public static class OpenMapDialog
    {
        public static IMap Open()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = Constants.cstMapFileFilter;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return MapFactory.LoadMapFrom(dlg.FileName);
                }
            }
            return null;
        }
    }
}
