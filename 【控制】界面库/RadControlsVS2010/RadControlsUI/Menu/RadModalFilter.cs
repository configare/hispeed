using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    class RadModalFilter : IMessageFilter
    {
        // fields
        private static RadModalFilter instance = null;

        private bool suspended;
        private bool menuAutoExpand;
        //private Point oldMousePosition;
        private WindowsHook hookMouse;

        internal List<RadDropDownMenu> DropDowns;
        internal RadDropDownMenu ActiveDropDown;
        internal static bool InDesignMode;
        internal static bool Animating;

        protected RadModalFilter()
        {
            DropDowns = new List<RadDropDownMenu>();
        }

        #region Properties

        public static RadModalFilter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RadModalFilter();
                }
                return instance;
            }
        }

        public bool Suspended
        {
            get
            {
                return suspended;
            }
            set
            {
                if (suspended != value)
                {
                    suspended = value;
                    if (ActiveDropDown != null)
                    {
                        //if (!InDesignMode)
                        //{
                        //    ActiveDropDown.Capture = !suspended;
                        //}
                        //ActiveDropDown.Suspended = suspended;
                    }
                }
            }
        }

        public bool DisableAutoClose;

        public bool MenuAutoExpand
        {
            get { return menuAutoExpand; }
            set
            {
                menuAutoExpand = value;
            }
        }



        public event EventHandler MenuHierarchyClosing;

        #endregion

        #region Public methods

        public void Register(RadDropDownMenu menu)
        {
            if (menu == ActiveDropDown || DropDowns.Contains(menu))
            {
                return;
            }

            Suspended = true;
            if (ActiveDropDown != null)
            {
                //ActiveDropDown.Capture = false;
                if (menu != null && menu.OwnerElement != null && menu.OwnerElement.ElementTree != null)
                {
                    RadDropDownMenu menuOwner = menu.OwnerElement.ElementTree.Control as RadDropDownMenu;
                    if (menuOwner != null)
                    {
                        int index = DropDowns.IndexOf(menuOwner);
                        while (index < DropDowns.Count - 1)
                            DropDowns[DropDowns.Count - 1].ClosePopup(RadPopupCloseReason.CloseCalled);
                    }
                }
            }
            else
            {
                if (!InDesignMode)
                {
                    Application.AddMessageFilter(this);
                }
                else
                {
                    hookMouse = new WindowsHook(WindowsHook.HookType.WH_MOUSE_LL);
                    hookMouse.HookInvoked += new WindowsHook.HookEventHandler(hookMouse_HookInvoked);
                    hookMouse.Install();
                }
            }
            this.DropDowns.Add(menu);
            ActiveDropDown = menu;
            Suspended = false;

            //if (InDesignMode && selectionService == null)
            //{
            //    if (ActiveDropDown.OwnerElement is ISiteProvider)
            //    {
            //        ISite site = (ActiveDropDown.OwnerElement as ISiteProvider).GetSite();
            //        selectionService = (ISelectionService)site.GetService(typeof(ISelectionService));
            //        selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
            //    }
            //}
        }

        public void UnRegister(RadDropDownMenu menu)
        {
            //autoCloseElement = null;
            //autoCloseTimer.Stop();

            if (!DropDowns.Contains(menu))
            {
                return;
            }

            Suspended = true;
            RadMenu rootMenu = null;
            if (DropDowns.Count > 0)
            {
                if (DropDowns[0].OwnerElement != null && DropDowns[0].OwnerElement.ElementState == ElementState.Loaded)
                    rootMenu = DropDowns[0].OwnerElement.ElementTree.Control as RadMenu;
                this.DropDowns.Remove(menu);
            }
            if (this.DropDowns.Count > 0)
            {
                ActiveDropDown = this.DropDowns[this.DropDowns.Count - 1];
            }
            else
            {
                Cleanup();
            }
            Suspended = false;
        }

        public void UnRegisterActiveMenu()
        {
            UnRegister(ActiveDropDown);
        }

        public void UnRegisterAllMenus()
        {
            if (DropDowns.Count > 0)
            {
                Suspended = true;


                if (DropDowns[0].OwnerElement == null)
                    MenuAutoExpand = false;

                while (DropDowns.Count > 0)
                {
                    if (DropDowns[DropDowns.Count - 1].Visible)
                    {
                        if (DropDowns[DropDowns.Count - 1].OwnerElement is RadDropDownButtonElement &&
                            DropDowns[DropDowns.Count - 1].OwnerElement.ElementTree.Control is RadDropDownMenu)
                        {
                            DropDowns[DropDowns.Count - 1].ClosePopup(RadPopupCloseReason.CloseCalled);
                            return;
                        }

                        DropDowns[DropDowns.Count - 1].ClosePopup(RadPopupCloseReason.CloseCalled);
                    }
                    else
                    {
                        DropDowns.RemoveAt(DropDowns.Count - 1);
                    }
                }

                Cleanup();
                Suspended = false;
            }
        }

        public void SetActiveDropDown(RadDropDownMenu menu)
        {
            int index = this.DropDowns.IndexOf(menu);
            if (index >= 0 && menu != ActiveDropDown)
            {
                for (int k = DropDowns.Count - 1; k > index; k--)
                {
                    DropDowns[k].ClosePopup(RadPopupCloseReason.CloseCalled);
                }
            }
        }

        #endregion

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (Animating)
                return false;

            if (ActiveDropDown != null)
            {
                if (ActiveDropDown.Visible == false)
                    UnRegisterAllMenus();

                if (!InDesignMode)
                {
                    switch (m.Msg)
                    {
                        case NativeMethods.WM_LBUTTONDOWN:
                        case NativeMethods.WM_RBUTTONDOWN:
                        case NativeMethods.WM_MBUTTONDOWN:
                        case NativeMethods.WM_NCLBUTTONDOWN:
                        case NativeMethods.WM_NCRBUTTONDOWN:
                        case NativeMethods.WM_NCMBUTTONDOWN:
                            {
                                if (!Suspended)
                                    return OnMouseDown(ref m);

                                Point pt = ActiveDropDown.PointToClient(Control.MousePosition);
                                if (!ActiveDropDown.ClientRectangle.Contains(pt))
                                {
                                    if (MenuHierarchyClosing != null)
                                        MenuHierarchyClosing(this, EventArgs.Empty);

                                    if (!DisableAutoClose)
                                    {
                                        UnRegisterAllMenus();
                                        return true;
                                    }
                                }

                                return false;
                            }

                    }
                }
            }

            return false;
        }

        #endregion

        private bool OnMouseDown(ref Message m)
        {
            if (ActiveDropDown == null || ActiveDropDown.IsDisposed)
                return false;

            if (!ActiveDropDown.ClientRectangle.Contains(ActiveDropDown.PointToClient(Control.MousePosition)))
            {

                for (int i = DropDowns.Count - 2; i >= 0; i--)
                {
                    Point point = DropDowns[i].PointToClient(Control.MousePosition);
                    if (DropDowns[i].ClientRectangle.Contains(point))
                    {
                        if (ActiveDropDown.OwnerElement != null)
                        {
                            RadElement elementAtPoint = ActiveDropDown.OwnerElement.ElementTree.ComponentTreeHandler.ElementTree.GetElementAtPoint(point);

                            if ((DropDowns[i] == ActiveDropDown.OwnerElement.ElementTree.Control) &&
                                elementAtPoint == ActiveDropDown.OwnerElement)
                            {
                                return true;
                            }
                        }

                        for (int k = DropDowns.Count - 1; k > i; k--)
                            this.ActiveDropDown.ClosePopup(RadPopupCloseReason.Mouse);

                        return false;
                    }
                }

                UnRegisterAllMenus();
            }

            return false;
        }

        private bool hookMouse_HookInvoked(object sender, HookEventArgs e)
        {
            if (e.wParam == (IntPtr)NativeMethods.WM_LBUTTONDOWN ||
                e.wParam == (IntPtr)NativeMethods.WM_MBUTTONDOWN ||
                e.wParam == (IntPtr)NativeMethods.WM_RBUTTONDOWN ||
                e.wParam == (IntPtr)NativeMethods.WM_NCLBUTTONDOWN ||
                e.wParam == (IntPtr)NativeMethods.WM_NCMBUTTONDOWN ||
                e.wParam == (IntPtr)NativeMethods.WM_NCRBUTTONDOWN)
            {
                WindowsHook.MSLLHOOKSTRUCT hookStruct = (WindowsHook.MSLLHOOKSTRUCT)Marshal.PtrToStructure(e.lParam, typeof(WindowsHook.MSLLHOOKSTRUCT));
                Point point = new Point(hookStruct.pt.x, hookStruct.pt.y);

                try
                {
                    if (!ActiveDropDown.ClientRectangle.Contains(ActiveDropDown.PointToClient(point)))
                    {
                        for (int i = DropDowns.Count - 2; i >= 0; i--)
                        {
                            Point p = DropDowns[i].PointToClient(point);
                            if (DropDowns[i].ClientRectangle.Contains(p))
                            {
                                for (int k = DropDowns.Count - 1; k > i; k--)
                                {
                                    DropDowns[k].ClosePopup(RadPopupCloseReason.Mouse);
                                }

                                return true;
                            }
                        }
                        UnRegisterAllMenus();
                    }
                }
                catch
                {
                    UnRegisterAllMenus();
                }
            }

            return true;
        }

        private void Cleanup()
        {
            DropDowns.Clear();
            if (ActiveDropDown != null && ActiveDropDown.activeHwnd != IntPtr.Zero)
                NativeMethods.SetActiveWindow(new HandleRef(null, ActiveDropDown.activeHwnd));
            ActiveDropDown = null;
            MenuAutoExpand = false;
            if (!InDesignMode)
            {
                Application.RemoveMessageFilter(this);
            }
            else
            {
                hookMouse.Uninstall();
            }
        }

        private bool ActivateNextRootMenuItem(Keys key, bool force)
        {
            //RadMenu rootMenu = null;

            //if (ActiveDropDown.OwnerElement != null && ActiveDropDown.OwnerElement.ElementTree.Control is RadMenu)
            //    rootMenu = ActiveDropDown.OwnerElement.ElementTree.Control as RadMenu;

            //if (force && rootMenu == null && DropDowns[0].OwnerElement != null)
            //    rootMenu = DropDowns[0].OwnerElement.ElementTree.Control as RadMenu;

            //if (rootMenu != null)
            //{
            //    rootMenu.ignoreMouseLeave = true;
            //    UnRegisterAllMenus();
            //    rootMenu.ignoreMouseLeave = false;

            //    MenuAutoExpand = true;
            //    rootMenu.Activated = true;
            //    rootMenu.ProcessKeyboard = true;

            //    rootMenu.DoCmdKey(key);

            //    if (ActiveDropDown == null || !ActiveDropDown.Visible)
            //        rootMenu.DoCmdKey(Keys.Down);

            //    if (ActiveDropDown != null)
            //        ActiveDropDown.SelectFirstVisibleItem();

            //    return true;
            //}

            return false;
        }
    }
}
