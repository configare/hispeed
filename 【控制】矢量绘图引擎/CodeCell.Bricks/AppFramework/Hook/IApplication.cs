using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.AppFramework
{
    public interface IApplication
    {
        TemporalFileManager TemporalFileManager { get; }
        TaskQueue ExitTaskQueue { get; }
        IShortcutProvider ShortcutProvider { get; }
        IShortcutProcessor ShortcutProcessor { get; }
        IClipboardEnvironment ClipboardEnvironment { get; }
        IProgressTracker ProgressTracker { get; }
        IHook Hook { get; }
        RecentUsedFiles RecentUsedFilesMgr { get; }
    }
}
