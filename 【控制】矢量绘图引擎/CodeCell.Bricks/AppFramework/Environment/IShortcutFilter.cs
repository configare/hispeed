using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public interface IShortcutFilter
    {
        bool AcceptShortcut(PreviewKeyDownEventArgs previewKeyDownEventArgs);
    }
}
