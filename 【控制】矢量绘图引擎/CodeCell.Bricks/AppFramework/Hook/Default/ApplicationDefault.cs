using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.AppFramework
{
    public class ApplicationDefault: IApplication
    {
        private RecentUsedFiles _recentUsedFilesMgr = null;
        private TemporalFileManager _temporalFileManager = null;
        private TaskQueue _exitTaskQueue = null;
        protected IShortcutProcessor _shortcutProcessor = null;
        private IShortcutProvider _shortcutProvider = null;
        private IClipboardEnvironment _clipboardEnvironment = null;
        private IProgressTracker _progressTracker = null;
        private IHook _hook = null;

        public ApplicationDefault(string tempDir)
        {
            _exitTaskQueue = new TaskQueue(_progressTracker);
            //_temporalFileManager = new TemporalFileManager(tempDir);
            _recentUsedFilesMgr = new RecentUsedFiles(20,tempDir);
            _exitTaskQueue.AddTask(_temporalFileManager);
        }

 
        internal void SetHook(IHook hook)
        {
            _hook = hook;
        }

        #region IApplication 成员

        public RecentUsedFiles RecentUsedFilesMgr
        {
            get { return _recentUsedFilesMgr; }
        }

        public TemporalFileManager TemporalFileManager
        {
            get { return _temporalFileManager; }
        }

        public TaskQueue ExitTaskQueue
        {
            get { return _exitTaskQueue; }
        }

        public IHook Hook
        {
            get { return _hook; }
        }

        public IShortcutProcessor ShortcutProcessor
        {
            get
            {
                if (_shortcutProcessor == null)
                    _shortcutProcessor = new ShortcutProcessorDefault();
                return _shortcutProcessor;
            }
        }

        public IShortcutProvider ShortcutProvider
        {
            get
            {
                if (_shortcutProvider == null)
                    _shortcutProvider = new ShortcutProviderDefault();
                return _shortcutProvider;
            }
        }

        public IClipboardEnvironment ClipboardEnvironment
        {
            get
            {
                if (_clipboardEnvironment == null)
                    _clipboardEnvironment = new ClipboardEnvironmentDefault();
                return _clipboardEnvironment;
            }
        }

        public IProgressTracker ProgressTracker
        {
            get 
            {
                return _progressTracker;
            }
        }

        #endregion
    }
}
