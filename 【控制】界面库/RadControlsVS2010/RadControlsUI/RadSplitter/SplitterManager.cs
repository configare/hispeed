using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class SplitterManager
    {
        private static Dictionary<Control, SplitterManager> managers;
        //private Dictionary<RadSplitter, SplitterInfo> splitters = new Dictionary<RadSplitter, SplitterInfo>();
        private Control container;
        
        //TODO: Uncomment when implemented
        //class SplitterInfo
        //{
        //    public RadSplitter splitter;
        //    public Control prev;
        //    public Control next;
        //}

        static SplitterManager()
        {
            managers = new Dictionary<Control, SplitterManager>();
        }

        protected SplitterManager(Control container)
        {
            this.container = container;
            this.container.ControlAdded += new ControlEventHandler(container_ControlAdded);
            this.container.ControlRemoved += new ControlEventHandler(container_ControlRemoved);

            this.Initialize();
        }

        private void Initialize()
        {
            //foreach (Control child in ControlHelper.EnumChildControls(this.container, false))
            //{
            //    RadSplitter splitter = child as RadSplitter;
            //    if (splitter != null)
            //    {
            //        SplitterInfo splitterInfo = new SplitterInfo();
            //        splitterInfo.splitter = splitter;
            //        this.splitters.Add(splitter, splitterInfo);
            //    }
            //}
        }

        private void container_ControlRemoved(object sender, ControlEventArgs e)
        {
            
        }

        private void container_ControlAdded(object sender, ControlEventArgs e)
        {
            
        }

        public static SplitterManager CreateManager(Control container)
        {
            if (managers.ContainsKey(container))
            {
                return managers[container];
            }

            SplitterManager manager = new SplitterManager(container);
            managers.Add(container, manager);
            return manager;
        }

        public bool Collapse(RadSplitter splitter, SplitterCollapsedState collapsedState)
        {
            return false;
        }

        public bool Expand(RadSplitter splitter)
        {
            return false;
        }

        public bool IsCollapsed(RadSplitter splitter)
        {
            return false;
        }
    }
}
