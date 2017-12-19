using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.MEF;

namespace GeoDo.RSS.UI.WinForm
{
    internal class SmartToolWindowFactory:ISmartToolWindowFactory
    {
        private Dictionary<int, ISmartToolWindow> _openedToolWindows = new Dictionary<int, ISmartToolWindow>();
        private Dictionary<int, Type> _registeredToolWindows = new Dictionary<int, Type>();
        private ISmartSession _session = null;

        public SmartToolWindowFactory(ISmartSession session)
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("系统窗口");
            _session = session;
            using (IComponentLoader<ISmartToolWindow> loader = new ComponentLoader<ISmartToolWindow>())
            {
                ISmartToolWindow[] wnds = loader.LoadComponents(dlls);
                if (wnds != null)
                    foreach (ISmartToolWindow w in wnds)
                        _registeredToolWindows.Add(w.Id, w.GetType());
            }
        }

        public ISmartToolWindow GetSmartToolWindow(int identify)
        {
            if (_openedToolWindows.ContainsKey(identify))
                return _openedToolWindows[identify];
            if (_registeredToolWindows.ContainsKey(identify))
            {
                ISmartToolWindow wnd= Activator.CreateInstance(_registeredToolWindows[identify]) as ISmartToolWindow;
                if (wnd == null)
                    return null;
                wnd.Init(_session);
                _openedToolWindows.Add(identify, wnd);
                (wnd as ToolWindow).Disposed+=new EventHandler((sender,args)=>
                {
                    _openedToolWindows[identify].Free();
                    _openedToolWindows.Remove(identify);
                });
                return wnd;
            }
            return null;
        }

        public bool IsDisplayed(int identify)
        {
            return _openedToolWindows.ContainsKey(identify);
        }

        public bool IsDisplayed(Type type)
        {
            foreach (object wnd in _openedToolWindows.Values)
                if (wnd.GetType().Equals(type))
                    return true;
            return false;
        }
    }
}
