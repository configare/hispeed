/***************************************************************************
 *   CopyRight (C) 2008 by SC Crom-Osec SRL                                *
 *   Contact:  contact@osec.ro                                             *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the Crom Free License as published by           *
 *   the SC Crom-Osec SRL; version 1 of the License                        *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   Crom Free License for more details.                                   *
 *                                                                         *
 *   You should have received a copy of the Crom Free License along with   *
 *   this program; if not, write to the contact@osec.ro                    *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
    public delegate void OnSelectedToolWindowChanged(DockableToolWindow toolWindow);
    /// <summary>
    /// Container for auto-dockable tool windows. Place this on your form to allow auto-docking for the tool windows.
    /// </summary>
    public partial class DockContainer : UserControl
    {
        #region Fields.

        private const int HooverMouseDelta = 10;

        private DockPanelsResizer _panels = null;
        private DockPreviewEngine _dockPreviewEngine = new DockPreviewEngine();

        private List<DockableToolWindow> _undockedToolWindows = new List<DockableToolWindow>();
        private List<DockableToolWindow> _dockableToolWindows = new List<DockableToolWindow>();
        private DockableToolWindow _movedToolWindow = null;
        private List<TabButton> _tabButtons = new List<TabButton>();

        private bool _closeCenterButtonHoover = false;
        private bool _menuCenterButtonHoover = false;

        private Color _tabButtonSelectedBackColor2 = Color.FromArgb(255, 215, 157);
        private Color _tabButtonSelectedBackColor1 = Color.FromArgb(255, 242, 200);
        private Color _tabButtonSelectedColor = Color.Black;
        private Color _tabButtonSelectedBorderColor = Color.FromArgb(75, 75, 111);
        private Color _tabButtonNotSelectedColor = Color.DarkGray;
        //private bool _tabButtonShowHoover = false;
        private bool _tabButtonShowSelection = false;

        private Point _lastMouseLocation = new Point();
        private bool _selectToolWindowsOnHoover = false;
        public OnSelectedToolWindowChanged SelectedToolWindowChanged = null;
        static ToolTip _toolTip = new ToolTip();
        private TabButton _toolTipButton = null;

        #endregion Fields.

        #region Instance.

        /// <summary>
        /// Default constructor which creates a new instance of <see cref="DockContainer"/>
        /// </summary>
        public DockContainer()
        {
            InitializeComponent();

            _panels = new DockPanelsResizer(this);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            BorderStyle = BorderStyle.None;
            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            MouseMove += new MouseEventHandler(DockContainer_MouseMove);
            _dockPreviewEngine.VisibleChanged += OnDockPreviewVisibleChanged;
            _mouseCheckTimer.Tick += OnTestMouseState;
            _panels.MinimumSizeChanged += OnMinimumSizeChanged;
            _toolTip.AutomaticDelay = 100000;
        }


        void DockContainer_MouseMove(object sender, MouseEventArgs e)
        {
            foreach(TabButton btn in _tabButtons)
                if (btn.Bounds.Contains(PointToClient(Control.MousePosition)))
                {
                    if (!btn.Equals(_toolTipButton))
                    {
                        _toolTip.SetToolTip(this, btn.TitleData.Title());
                    }
                    break;
                }
        }

        #endregion Instance.

        #region Public section.

        #region Tab buttons properties.

        /// <summary>
        /// Color of the tab button text when the button is not selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the tab button text when the button is not selected.")]
        public Color TabButtonNotSelectedColor
        {
            get { return _tabButtonNotSelectedColor; }
            set
            {
                _tabButtonNotSelectedColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.NotSelectedColor = value;
                }
            }
        }

        /// <summary>
        /// Color of the tab button text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the tab button text when the button is selected.")]
        public Color TabButtonSelectedColor
        {
            get { return _tabButtonSelectedColor; }
            set
            {
                _tabButtonSelectedColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedColor = value;
                }
            }
        }

        /// <summary>
        /// Selected border color of the tab button.
        /// </summary>
        [Category("Appearance")]
        [Description("Border color of the tab button when is selected.")]
        public Color TabButtonSelectedBorderColor
        {
            get { return _tabButtonSelectedBorderColor; }
            set
            {
                _tabButtonSelectedBorderColor = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// First gradient color of the text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("First gradient color of the tab button when is selected.")]
        public Color TabButtonSelectedBackColor1
        {
            get { return _tabButtonSelectedBackColor1; }
            set
            {
                _tabButtonSelectedBackColor1 = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBackColor1 = value;
                }
            }
        }

        /// <summary>
        /// Second gradient color of the text when the button is selected.
        /// </summary>
        [Category("Appearance")]
        [Description("Second gradient color of the tab button when is selected.")]
        public Color TabButtonSelectedBackColor2
        {
            get { return _tabButtonSelectedBackColor2; }
            set
            {
                _tabButtonSelectedBackColor2 = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.SelectedBackColor2 = value;
                }
            }
        }

        /// <summary>
        /// Flag indicating if the button should draw border and gradient background when is selected. Default false.
        /// </summary>
        [Category("Behavior")]
        [Description("Flag indicating if the button should draw border and gradient background when is selected. Default false.")]
        public bool TabButtonShowSelection
        {
            get { return _tabButtonShowSelection; }
            set
            {
                _tabButtonShowSelection = value;
                foreach (TabButton button in _tabButtons)
                {
                    button.ShowSelection = value;
                }
            }
        }

        #endregion Tab buttons properties.

        /// <summary>
        /// This event occurs when the minimum allowed size for the container was changed.
        /// The form on which this container is placed should be sized to display the entire container
        /// </summary>
        /// <example>
        /// Here is a sample of how this event should be handled. It assumes that _dockContainer1 is 
        /// docked on the form with DockStyle.Fill.
        /// <para></para>
        /// In this example is computed the difference between the dock container width and height and then add these differences
        /// to the minimum size of the form. In this way, when the form has minimum size, the container will
        /// have ensured its required minimum size:
        /// <code>
        /// private void OnDockContainerMinSizeChanged (object sender, EventArgs e)
        /// {
        ///    int deltaX = Width  - _dockContainer1.Width;
        ///    int deltaY = Height - _dockContainer1.Height;
        /// 
        ///    MinimumSize = new Size (
        ///       _dockContainer1.MinimumSize.Width  + deltaX,
        ///       _dockContainer1.MinimumSize.Height + deltaY);
        /// }
        /// </code>
        /// </example>
        [Category("Layout")]
        [Description("Occurs when the minimum size of the container is changed. Use this to set the minimum size of the form.")]
        public event EventHandler MinimumSizeChanged;

        /// <summary>
        /// Event raised when context menu request was made.
        /// </summary>
        /// <example>
        /// Here is an example of how to handle this event to display a context menu for <c>Fill</c> panel and 
        /// to select a docked tool window from that panel:
        /// <code>
        /// private void OnToolWindowContextMenuRequest (object sender, ContextMenuEventArg e)
        /// {
        ///    if (e.Selection.DockMode == zDockMode.Fill &amp;&amp; e.MouseButtons == MouseButtons.Left)
        ///    {
        ///       ClearSelectWindowsMenu (_centerContextMenu.Items);
        /// 
        ///       DockableToolWindow[] panelWindows = _dockContainer.GetVisibleDockedWindows (e.Selection.DockMode);
        ///       foreach (DockableToolWindow panelWindow in panelWindows)
        ///       {
        ///          AddSelectWindowMenu (_centerContextMenu.Items, panelWindow);
        ///       }
        /// 
        ///       e.Selection.ContextMenuStrip = _centerContextMenu;
        /// 
        ///       e.Selection.ContextMenuStrip.Show (e.MouseLocation);
        ///    }
        /// }
        /// 
        /// private void AddSelectWindowMenu (ToolStripItemCollection items, DockableToolWindow window)
        /// {
        ///    ToolStripMenuItem item = new ToolStripMenuItem (window.Text);
        ///    items.Add (item);
        /// 
        ///    item.Tag = window;
        ///    item.Click += OnSelectToolWindowByMenu;
        /// }
        /// 
        /// private void ClearSelectWindowsMenu (ToolStripItemCollection items)
        /// {
        ///    foreach (ToolStripMenuItem item in items)
        ///    {
        ///       item.Tag = null;
        ///       item.Click -= OnSelectToolWindowByMenu;
        ///    }
        /// 
        ///    items.Clear ();
        /// }
        /// 
        /// private void OnSelectToolWindowByMenu (object sender, EventArgs e)
        /// {
        ///    ToolStripMenuItem item = sender as ToolStripMenuItem;
        ///    if (item == null)
        ///    {
        ///       return;
        ///    }
        /// 
        ///    DockableToolWindow window = item.Tag as DockableToolWindow;
        ///    if (window == null)
        ///    {
        ///       return;
        ///    }
        /// 
        ///    _dockContainer.SelectToolWindow (window);
        /// }
        /// </code>
        /// </example>
        [Category("Action")]
        [Description("Occurs when the context menu should be shown for the selected tool window.")]
        public event EventHandler<ContextMenuEventArg> ContextMenuRequest;

        /// <summary>
        /// Event raised when auto-hide was toggled for a panel
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the auto-hide state was changed for the selected panel.")]
        public event EventHandler<AutoHideEventArgs> AutoHidePanelToggled;

        /// <summary>
        /// Occurs when a tool window was selected
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a tool window was selected.")]
        public event EventHandler<ToolSelectionChangedEventArgs> ToolWindowSelected;

        /// <summary>
        /// Flag indicating if the tool windows should be selected on hoover over their tab button when the panel
        /// is in auto-hide mode.
        /// </summary>
        [Category("Behavior")]
        [Description("Flag indicating if the tool windows should be selected on hoover over their tab button when the panel is in auto-hide mode.")]
        public bool SelectToolWindowsOnHoover
        {
            get { return _selectToolWindowsOnHoover; }
            set { _selectToolWindowsOnHoover = value; }
        }

        /// <summary>
        /// Get / set the width of the left panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the left panel.")]
        public int LeftPanelWidth
        {
            get { return _panels.LeftPanelWidth; }
            set { _panels.LeftPanelWidth = value; }
        }

        /// <summary>
        /// Get / set the width of the right panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the right panel.")]
        public int RightPanelWidth
        {
            get { return _panels.RightPanelWidth; }
            set { _panels.RightPanelWidth = value; }
        }

        /// <summary>
        /// Get / set the height of the top panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the top panel.")]
        public int TopPanelHeight
        {
            get { return _panels.TopPanelHeight; }
            set { _panels.TopPanelHeight = value; }
        }

        /// <summary>
        /// Get / set the height of the bottom panel
        /// </summary>
        [Category("Layout")]
        [Description("Get/set the width of the bottom panel.")]
        public int BottomPanelHeight
        {
            get { return _panels.BottomPanelHeight; }
            set { _panels.BottomPanelHeight = value; }
        }

        /// <summary>
        /// Checks if a tool window is added in this container
        /// </summary>
        /// <param name="toolWindow">tool window to be checked</param>
        /// <returns>true if the given tool window is added in this container</returns>
        public bool IsInContainer(DockableToolWindow toolWindow)
        {
            return _dockableToolWindows.Contains(toolWindow);
        }

        /// <summary>
        /// Add a tool window to be managed by this dock container. 
        /// The caller must show the form to make it visible.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A NullReferenceException will be thrown if the tool window is null.
        /// <br/><br/>
        /// The window is not docked when is added with this method. To add the form with initial dock, use
        /// the <see cref="DockToolWindow">DockToolWindow</see> method.
        /// <br/><br/>
        /// The following properties are forced to restricted values:
        /// <ul>
        /// <li> Dock       is forced to <c>DockStyle.None</c> (an exception will be thrown if this value is changed outside of this container).</li>
        /// <li> Parent     is forced to <c>this</c> (changing it outside of the instance will cause the removal of the tool window).</li>
        /// <li> TopLevel   is forced <c>false</c> (an exception will be thrown if this value is changed outside of this container).</li>
        /// </ul>
        /// <para></para>
        /// <br/>
        /// <para></para>
        /// DockContainer is not the owner of the added toolWindow so is not responsable with
        /// disposing it.
        /// <para></para>
        /// <br/>
        /// <para></para>
        /// When a toolWindow (which was added to this container) is disposed or when its parent is 
        /// changed, that toolWindow is automatically removed from this container.
        /// </remarks>
        /// <example>
        /// Here is a sample of how to use this method. It will add a floating form to the dock container.
        /// <code>
        /// private void ShowNewForm ()
        /// {
        ///    // Create a new instance of the child form.
        ///    DockableToolWindow childForm = new DockableToolWindow ();
        /// 
        ///    // Add the form to the dock container
        ///    _dockContainer1.AddToolWindow (childForm);
        /// 
        ///    // Show the form
        ///    childForm.Show ();
        /// }
        /// </code>
        /// </example>
        /// <param name="toolWindow">tool window to be added</param>
        public void AddToolWindow(DockableToolWindow toolWindow)
        {
            if (IsInContainer(toolWindow))
            {
                return;
            }

            toolWindow.Move += OnToolWindowMove;
            toolWindow.Disposed += OnDisposeToolWindow;
            toolWindow.ParentChanged += OnToolWindowParentChanged;
            toolWindow.FormClosed += OnToolWindowClose;
            toolWindow.DockChanged += OnToolWindowDockChanged;
            toolWindow.AutoHideButtonClick += OnToolWindowAutoHideChanged;
            toolWindow.ContextMenuForToolWindow += OnToolWindowContextMenuRequest;

            toolWindow.Dock = DockStyle.None;
            toolWindow.TopLevel = false;
            toolWindow.Parent = this;

            _dockableToolWindows.Add(toolWindow);
            _undockedToolWindows.Add(toolWindow);

            CreateTabButton(toolWindow);

            SelectToolWindow(toolWindow);

            MoveInFront(toolWindow);
        }

        /// <summary>
        /// Removes a tool window from this dock container.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// </remarks>
        /// <param name="toolWindow">tool window to be removed</param>
        public void RemoveToolWindow(DockableToolWindow toolWindow)
        {
            if (toolWindow.Parent == this)
            {
                toolWindow.Parent = null;
            }

            toolWindow.Move -= OnToolWindowMove;
            toolWindow.Disposed -= OnDisposeToolWindow;
            toolWindow.ParentChanged -= OnToolWindowParentChanged;
            toolWindow.FormClosed -= OnToolWindowClose;
            toolWindow.DockChanged -= OnToolWindowDockChanged;
            toolWindow.AutoHideButtonClick -= OnToolWindowAutoHideChanged;
            toolWindow.ContextMenuForToolWindow -= OnToolWindowContextMenuRequest;

            _dockableToolWindows.Remove(toolWindow);
            _undockedToolWindows.Remove(toolWindow);

            _panels.UndockToolWindow(toolWindow);

            RemoveTabButton(toolWindow);
        }

        /// <summary>
        /// Dock the given tool window inside the dock container.
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// If the dock mode is None or is not Left, Right, Top, Bottom or Fill, then the tool window is floating.
        /// </remarks>
        /// <example>
        /// Here is a sample of how to use this method. It will add a form to the dock container with initial docking on Left.
        /// <code>
        /// private void OnCreateNewToolWindowDockedLeft (object sender, EventArgs e)
        /// {
        ///    // Create a new instance of the child form.
        ///    DockableToolWindow childForm = new DockableToolWindow ();
        /// 
        ///    // Add and dock the form in the left panel of the container
        ///    _dockContainer1.DockToolWindow (childForm, zDockMode.Left);
        /// 
        ///    // Show the form
        ///    childForm.Show ();
        /// }
        /// </code>
        /// </example>
        /// <param name="toolWindow">tool window to be docked. If the tool window is not in the container
        /// when this method is called, then the window is added as if 
        /// <see cref="AddToolWindow">AddToolWindow</see> method was invoked. </param>
        /// <param name="dockMode">dock mode of the tool window can be Left, Right, Top, Bottom, Fill or None, but not a combination
        /// of these.</param>
        public void DockToolWindow(DockableToolWindow toolWindow, zDockMode dockMode)
        {
            toolWindow._onSelectedChanged += new OnDockableToolWindowSelectedChanged(ToolWindowSelectedChanged);
            if (IsInContainer(toolWindow) == false)
            {
                AddToolWindow(toolWindow);
            }

            _panels.DockToolWindow(toolWindow, dockMode);

            if (dockMode != zDockMode.None)
            {
                _undockedToolWindows.Remove(toolWindow);

                SelectToolWindow(toolWindow);

                MoveInFront(_undockedToolWindows);
            }
        }

        private void ToolWindowSelectedChanged(object sender, bool isSelected)
        {
            if (isSelected)
            {
                if (SelectedToolWindowChanged != null)
                    SelectedToolWindowChanged(sender as DockableToolWindow);
            }
        }

        /// <summary>
        /// Undock the tool window but doesn't remove it from conainer
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// </remarks>
        /// <param name="toolWindow">tool window to be undocked</param>
        public void UndockToolWindow(DockableToolWindow toolWindow)
        {
            _panels.UndockToolWindow(toolWindow);

            toolWindow.TabButton.Reset();

            if (_undockedToolWindows.Contains(toolWindow) == false)
            {
                _undockedToolWindows.Add(toolWindow);
            }
        }

        /// <summary>
        /// Select the given tool window. 
        /// </summary>
        /// <remarks>
        /// This method assume that tool window is not null. A <exception cref="NullReferenceException">NullReferenceException</exception> 
        /// will be thrown if the tool window is null.
        /// <br/>
        /// If the given tool window is floating, then is moved on z-axis in the top over all other tool windows.
        /// <br/>
        /// If the given tool window is docked, then is moved on z-axis in the top over all other tool windows docked
        /// in that panel. The floating tool windows remain top most.
        /// </remarks>
        /// <param name="toolWindow">tool window to be selected</param>
        public void SelectToolWindow(DockableToolWindow toolWindow)
        {
            if (toolWindow.DockMode == zDockMode.None)
            {
                MoveInFront(toolWindow);
                toolWindow.Select();
                return;
            }

            DockableToolWindow[] panelToolWindows = _panels.GetPanelToolWindows(toolWindow.DockMode);

            foreach (DockableToolWindow panelToolWindow in panelToolWindows)
            {
                panelToolWindow.TabButton.Selected = false;
            }

            toolWindow.TabButton.Selected = true;

            MoveInFront(toolWindow);
            MoveInFront(_undockedToolWindows);
            toolWindow.Select();

            if (_panels.IsVisible(toolWindow) == false)
            {
                _panels.SetAutoHidden(toolWindow.DockMode, false);
            }

            Invalidate();
        }

        /// <summary>
        /// Detects if the panel identified by given dock mode is auto-hidden
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <returns>true if the dock mode specifies a valid panel and that panel is auto-hidden</returns>
        public bool IsAutoHidden(zDockMode dockMode)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            return sidePanel.AutoHidden;
        }

        /// <summary>
        /// Detects if the panel identified by given dock mode is auto-hide
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <returns>true if the dock mode specifies a valid panel and that panel is auto-hide</returns>
        public bool IsAutoHide(zDockMode dockMode)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            return sidePanel.AutoHide;
        }

        /// <summary>
        /// Change the auto-hide state of given panel
        /// </summary>
        /// <param name="dockMode">dock mode can be Left, Rigth, Top, Bottom. 
        /// Other values will be ignored and the method will return false</param>
        /// <param name="autoHideValue">new auto-hide value</param>
        /// <returns>true if the dock mode specifies a valid panel and auto-hide state was changed</returns>
        public bool SetAutoHide(zDockMode dockMode, bool autoHideValue)
        {
            SideDockPanel sidePanel = _panels.GetPanel(dockMode) as SideDockPanel;
            if (sidePanel == null)
            {
                return false;
            }

            if (sidePanel.AutoHide == autoHideValue)
            {
                return false;
            }

            if (autoHideValue)
            {
                _panels.SetAutoHide(dockMode, true);
                _panels.SetAutoHidden(dockMode, true);
            }
            else
            {
                _panels.SetAutoHidden(dockMode, false);
                _panels.SetAutoHide(dockMode, false);
            }

            return true;
        }

        /// <summary>
        /// Get all the tool windows which are docked in the panel identified by the given dock mode.
        /// </summary>
        /// <remarks>
        /// If the dock mode identifies a valid panel, the result will be an array of tool windows. If no tool 
        /// window is docked on the panel, an empty array will be returned.
        /// <br/>
        /// If the dock mode doesn't identify a valid panel, then null is returned.
        /// </remarks>
        /// <param name="dockMode">dock mode which identifies the panel for which tool windows are requested.
        /// Valid values are Left, Right, Top, Bottom or Fill</param>
        /// <returns>Vector of tool windows from the identified panel.</returns>
        public DockableToolWindow[] GetDockedWindows(zDockMode dockMode)
        {
            DockPanel panel = _panels.GetPanel(dockMode);
            if (panel == null)
            {
                return null;
            }

            return panel.ToolWindows;
        }

        /// <summary>
        /// Get the visible tool windows which are docked in the panel identified by the given dock mode.
        /// </summary>
        /// <remarks>
        /// If the dock mode identifies a valid panel, the result will be an array of tool windows. If no tool 
        /// window is docked on the panel, an empty array will be returned.
        /// <br/>
        /// If the dock mode doesn't identify a valid panel, then null is returned.
        /// </remarks>
        /// <param name="dockMode">dock mode which identifies the panel for which tool windows are requested.
        /// Valid values are Left, Right, Top, Bottom or Fill</param>
        /// <returns>Vector of tool windows from the identified panel.</returns>
        public DockableToolWindow[] GetVisibleDockedWindows(zDockMode dockMode)
        {
            DockPanel panel = _panels.GetPanel(dockMode);
            if (panel == null)
            {
                //by fdc
                List<DockableToolWindow> wnds = new List<DockableToolWindow>();
                foreach (DockPanel p in _panels.DockPanelsLayout.DockPanels)
                    if (p.VisibleToolWindows != null && p.VisibleToolWindows.Length > 0)
                        wnds.AddRange(p.VisibleToolWindows);
                return wnds.Count > 0 ? wnds.ToArray() : null;
            }

            return panel.VisibleToolWindows;
        }

        //by fdc
        public bool IsLocked(DockableToolWindow wnd)
        {
            if (wnd == null)
                return false;
            return !_undockedToolWindows.Contains(wnd);
        }
        
        //by fdc
        public Rectangle GetDockPanelBounds(zDockMode model)
        {

            return _panels.GetPanelNonHiddenBounds(model);
        }

        /// <summary>
        /// Gets the top tool window from the panel identified by given dock mode
        /// </summary>
        /// <param name="dockMode">dock mode to identify the panel from which the top tool window is requested</param>
        /// <returns>top tool window or null if no visible window is in the panel</returns>
        public DockableToolWindow GetTopToolWindow(zDockMode dockMode)
        {
            return _panels.GetTopMostToolWindow(dockMode);
        }

        public DockableToolWindow GetActiveToolWindow(zDockMode dockMode)
        {
            DockPanel panel = _panels.GetPanel(dockMode);
            if (panel.ToolWindows != null && panel.ToolWindows.Length > 0)
            {
                foreach (DockableToolWindow wnd in panel.ToolWindows)
                    if (wnd.IsSelected)
                        return wnd;
            }
            return null;
        }

        #endregion Public section.

        #region Protected section.

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Paint -= OnPaint;
                MouseDown -= OnMouseDown;
                MouseUp -= OnMouseUp;
                MouseMove -= OnMouseMove;

                if (_mouseCheckTimer != null)
                {
                    _mouseCheckTimer.Enabled = false;
                    _mouseCheckTimer.Tick -= OnTestMouseState;
                    _mouseCheckTimer.Dispose();
                    _mouseCheckTimer = null;
                }

                while (_dockableToolWindows.Count > 0)
                {
                    RemoveToolWindow(_dockableToolWindows[0]);
                }

                if (_dockPreviewEngine != null)
                {
                    _dockPreviewEngine.VisibleChanged -= OnDockPreviewVisibleChanged;
                    _dockPreviewEngine = null;
                }

                if (_panels != null)
                {
                    _panels.MinimumSizeChanged -= OnMinimumSizeChanged;
                    _panels.Dispose();
                    _panels = null;
                }

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion Protected section.

        #region Private section.
        #region Received events.

        /// <summary>
        /// Handler for the mouse down event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            SelectToolWindowFromPoint(e.Location);
            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handler for the mouse up event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_closeCenterButtonHoover)
                {
                    if (CenterCloseButtonBounds.Contains(e.Location))
                    {
                        DockableToolWindow topmostToolWindow = _panels.GetTopMostToolWindow(zDockMode.Fill);
                        if (topmostToolWindow != null)
                        {
                            topmostToolWindow.Close();
                        }
                    }
                }
                else if (_menuCenterButtonHoover)
                {
                    if (CenterMenuButtonBounds.Contains(e.Location))
                    {
                        DockableToolWindow topmostToolWindow = _panels.GetTopMostToolWindow(zDockMode.Fill);
                        if (topmostToolWindow != null)
                        {
                            RaiseContextMenuRequest(topmostToolWindow, e.Button);
                        }
                    }
                }
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handler for the mouse move event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = MousePosition;

            DetectMouseHoover(mousePosition, CenterCloseButtonBounds, ref _closeCenterButtonHoover);
            DetectMouseHoover(mousePosition, CenterMenuButtonBounds, ref _menuCenterButtonHoover);

            if (SelectToolWindowsOnHoover)
            {
                if (Math.Abs(_lastMouseLocation.X - mousePosition.X) > HooverMouseDelta ||
                    Math.Abs(_lastMouseLocation.Y - mousePosition.Y) > HooverMouseDelta)
                {
                    _lastMouseLocation = mousePosition;

                    Point location = PointToClient(mousePosition);
                    SelectToolWindowFromPoint(location);
                }
            }

            if (IsSplitterCursor() || _menuCenterButtonHoover || _closeCenterButtonHoover)
            {
                IncreaseStateCheckFrequency();
            }
        }

        /// <summary>
        /// Handler for painting the control
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);

            // Draw splitters
            Rectangle splitterBounds;
            splitterBounds = _panels.GetPanelSplitterBounds(zDockMode.Left);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(zDockMode.Top);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(zDockMode.Right);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);

            splitterBounds = _panels.GetPanelSplitterBounds(zDockMode.Bottom);
            g.FillRectangle(SystemBrushes.Control, splitterBounds);
            g.DrawRectangle(SystemPens.ControlLight, splitterBounds);


            // Draw tab bouttons
            DrawHorizontalTabButtons(zDockMode.Left, _panels.GetFixedButtonsBounds(zDockMode.Left), _panels.GetPanelVisibleToolWindows(zDockMode.Left), g);
            DrawHorizontalTabButtons(zDockMode.Right, _panels.GetFixedButtonsBounds(zDockMode.Right), _panels.GetPanelVisibleToolWindows(zDockMode.Right), g);

            DrawVerticalTabButtons(zDockMode.Left, _panels.GetPanelButtonsBounds(zDockMode.Left), _panels.GetPanelVisibleToolWindows(zDockMode.Left), g);
            DrawVerticalTabButtons(zDockMode.Right, _panels.GetPanelButtonsBounds(zDockMode.Right), _panels.GetPanelVisibleToolWindows(zDockMode.Right), g);

            DrawHorizontalTabButtons(zDockMode.Top, _panels.GetPanelButtonsBounds(zDockMode.Top), _panels.GetPanelVisibleToolWindows(zDockMode.Top), g);
            DrawHorizontalTabButtons(zDockMode.Bottom, _panels.GetPanelButtonsBounds(zDockMode.Bottom), _panels.GetPanelVisibleToolWindows(zDockMode.Bottom), g);
            DrawHorizontalTabButtons(zDockMode.Fill, _panels.GetPanelButtonsBounds(zDockMode.Fill), _panels.GetPanelVisibleToolWindows(zDockMode.Fill), g);
        }

        /// <summary>
        /// Timed handler used to check the mouse state
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnTestMouseState(object sender, EventArgs e)
        {
            Point mousePosition = MousePosition;
            MouseButtons button = MouseButtons;

            // Save processor
            Application.DoEvents();
            System.Threading.Thread.Sleep(1);

            if (button != MouseButtons.Left)
            {
                if (_panels.IsCursorChanged)
                {
                    _panels.UpdateMouseCursor(PointToClient(mousePosition));
                }

                _dockPreviewEngine.HideDockGuiders();

                UnlockMovedToolWindow();

                DetectMouseHoover(mousePosition, CenterCloseButtonBounds, ref _closeCenterButtonHoover);
                DetectMouseHoover(mousePosition, CenterMenuButtonBounds, ref _menuCenterButtonHoover);
            }

            UpdatePreviewEngine(mousePosition);

            if (button != MouseButtons.None)
            {
                return;
            }

            UpdateAutoHiddenState(zDockMode.Left, mousePosition);
            UpdateAutoHiddenState(zDockMode.Right, mousePosition);
            UpdateAutoHiddenState(zDockMode.Top, mousePosition);
            UpdateAutoHiddenState(zDockMode.Bottom, mousePosition);

            if (IsAutoHidden(zDockMode.Left) == false && IsAutoHide(zDockMode.Left))
            {
                return;
            }

            if (IsAutoHidden(zDockMode.Right) == false && IsAutoHide(zDockMode.Right))
            {
                return;
            }

            if (IsAutoHidden(zDockMode.Top) == false && IsAutoHide(zDockMode.Top))
            {
                return;
            }

            if (IsAutoHidden(zDockMode.Bottom) == false && IsAutoHide(zDockMode.Bottom))
            {
                return;
            }

            if (IsSplitterCursor())
            {
                return;
            }

            if (_closeCenterButtonHoover || _menuCenterButtonHoover)
            {
                return;
            }

            DecreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the disposal of tool window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnDisposeToolWindow(object sender, EventArgs e)
        {
            DockableToolWindow toolWindow = sender as DockableToolWindow;
            if (toolWindow != null)
            {
                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the change of tool window parent
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowParentChanged(object sender, EventArgs e)
        {
            DockableToolWindow toolWindow = sender as DockableToolWindow;
            if (toolWindow != null)
            {
                if (toolWindow.Parent == this)
                {
                    return;
                }

                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the move of tool window using the mouse
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowMove(object sender, EventArgs e)
        {
            Point mousePosition = MousePosition;
            MouseButtons button = MouseButtons;

            if (button != MouseButtons.Left)
            {
                return;
            }

            _movedToolWindow = sender as DockableToolWindow;
            if (_movedToolWindow == null)
            {
                return;
            }

            MoveInFront(_movedToolWindow);

            if (_dockPreviewEngine.Visibile)
            {
                return;
            }

            if (_movedToolWindow.TitleBarScreenBounds.Contains(mousePosition) == false)
            {
                _movedToolWindow = null;
                return;
            }

            if (_panels.IsVisible(_movedToolWindow) == false)
            {
                _movedToolWindow = null;
                return;
            }

            _dockPreviewEngine.ShowDockGuiders(_movedToolWindow.AllowedDock, RectangleToScreen(ClientRectangle));
            UndockToolWindow(_movedToolWindow);

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the visibility change in dock preview
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnDockPreviewVisibleChanged(object sender, EventArgs e)
        {
            if (_dockPreviewEngine.Visibile == false)
            {
                if (_movedToolWindow == null)
                {
                    return;
                }

                DockToolWindow(_movedToolWindow, _dockPreviewEngine.DockMode);
                _movedToolWindow = null;
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the close of the tool window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowClose(object sender, FormClosedEventArgs e)
        {
            DockableToolWindow toolWindow = sender as DockableToolWindow;
            if (toolWindow != null)
            {
                RemoveToolWindow(toolWindow);
            }

            IncreaseStateCheckFrequency();
        }

        /// <summary>
        /// Handle the dock change for tool windows. The dock is not allowed.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowDockChanged(object sender, EventArgs e)
        {
            DockableToolWindow toolWindow = sender as DockableToolWindow;
            if (toolWindow != null)
            {
                if (toolWindow.Dock != DockStyle.None)
                {
                    throw new NotSupportedException("DockableToolWindow can't be docked while in DockContainer");
                }
            }
        }

        /// <summary>
        /// Handle the auto-hide state change for tool windows.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowAutoHideChanged(object sender, EventArgs e)
        {
            IncreaseStateCheckFrequency();

            DockableToolWindow window = sender as DockableToolWindow;
            if (AutoHidePanelToggled != null && window != null)
            {
                AutoHideEventArgs args = new AutoHideEventArgs(window.DockMode);
                AutoHidePanelToggled(this, args);
            }
        }

        /// <summary>
        /// Handle the request for tool window context menu
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnToolWindowContextMenuRequest(object sender, EventArgs e)
        {
            DockableToolWindow window = sender as DockableToolWindow;
            if (window != null && ContextMenuRequest != null)
            {
                ContextMenuEventArg args = new ContextMenuEventArg(window, MousePosition, MouseButtons);
                ContextMenuRequest(this, args);
            }
        }

        /// <summary>
        /// Handle the minimum container size changed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnMinimumSizeChanged(object sender, EventArgs e)
        {
            if (MinimumSizeChanged != null)
            {
                MinimumSizeChanged(this, EventArgs.Empty);
            }

            IncreaseStateCheckFrequency();
        }

        #endregion Received events.

        /// <summary>
        /// Raise context menu request
        /// </summary>
        /// <param name="selection">selection</param>
        /// <param name="buttons">mouse buttons</param>
        private void RaiseContextMenuRequest(DockableToolWindow selection, MouseButtons buttons)
        {
            EventHandler<ContextMenuEventArg> handler = ContextMenuRequest;
            if (handler != null)
            {
                ContextMenuEventArg args = new ContextMenuEventArg(selection, MousePosition, buttons);
                handler(this, args);
            }
        }

        /// <summary>
        /// Draw the horizontal tab buttons
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel from which are the buttons</param>
        /// <param name="bounds">bounds of the area in which buttons can be drawn</param>
        /// <param name="toolWindows">collection of the tool windows in the panel</param>
        /// <param name="graphics">graphics context</param>
        private void DrawHorizontalTabButtons(zDockMode dockMode, Rectangle bounds, DockableToolWindow[] toolWindows, Graphics graphics)
        {
            graphics.FillRectangle(SystemBrushes.Control, bounds);

            int windowsCount = toolWindows.Length;
            if (windowsCount == 0)
            {
                return;
            }

            if (dockMode == zDockMode.Left || dockMode == zDockMode.Right)
            {
                if (_panels.IsAutoHide(dockMode))
                {
                    return;
                }
            }

            RectangleF clip = graphics.ClipBounds;

            int width = bounds.Width;
            if (dockMode == zDockMode.Fill)
            {
                width -= 70;  // give space for buttons: close, autohide and context menu
            }
            int maxButtonWidth = width / windowsCount;
            Rectangle buttonBounds = new Rectangle(bounds.Left, bounds.Top, maxButtonWidth, bounds.Height - 2);

            for (int index = 0; index < windowsCount; index++)
            {
                TabButton button = (TabButton)toolWindows[index].TabButton;
                button.Draw(buttonBounds, Font, false, graphics);
                buttonBounds.X = button.Bounds.Right;
            }

            graphics.SetClip(clip);

            if (dockMode == zDockMode.Fill)
            {
                DrawUtility.DrawCloseButton(CenterCloseButtonBounds, _closeCenterButtonHoover, graphics);
                DrawUtility.DrawContextMenuButton(CenterMenuButtonBounds, _menuCenterButtonHoover, graphics);
            }
        }

        /// <summary>
        /// Draw the vertical tab buttons
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel from which are the buttons</param>
        /// <param name="bounds">bounds of the area in which buttons can be drawn</param>
        /// <param name="toolWindows">collection of the tool windows in the panel</param>
        /// <param name="graphics">graphics context</param>
        private void DrawVerticalTabButtons(zDockMode dockMode, Rectangle bounds, DockableToolWindow[] toolWindows, Graphics graphics)
        {
            graphics.FillRectangle(SystemBrushes.Control, bounds);

            int windowsCount = toolWindows.Length;
            if (windowsCount == 0)
            {
                return;
            }

            if (dockMode == zDockMode.Left || dockMode == zDockMode.Right)
            {
                if (_panels.IsAutoHide(dockMode) == false)
                {
                    return;
                }
            }

            RectangleF clip = graphics.ClipBounds;

            int maxButtonHeight = bounds.Height / windowsCount;
            Rectangle buttonBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width - 2, maxButtonHeight);

            for (int index = 0; index < windowsCount; index++)
            {
                TabButton button = (TabButton)toolWindows[index].TabButton;
                button.Draw(buttonBounds, Font, true, graphics);
                buttonBounds.Y = button.Bounds.Bottom;
            }

            graphics.SetClip(clip);
        }

        /// <summary>
        /// Update the auto-hidden state when the mouse position is outside of the panel
        /// </summary>
        /// <param name="dockMode">dock mode used to identify the panel</param>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        private void UpdateAutoHiddenState(zDockMode dockMode, Point mousePosition)
        {
            if (_panels.IsAutoHide(dockMode) == false)
            {
                return;
            }

            Rectangle bounds = _panels.GetPanelNonHiddenBounds(dockMode);
            if (bounds.Contains(mousePosition))
            {
                return;
            }

            if (RectangleToScreen(_panels.GetPanelButtonsBounds(dockMode)).Contains(mousePosition))
            {
                return;
            }

            if (RectangleToScreen(_panels.GetPanelSplitterBounds(dockMode)).Contains(mousePosition))
            {
                return;
            }

            _panels.SetAutoHidden(dockMode, true);
        }

        /// <summary>
        /// Move the given tool window in the front of the view
        /// </summary>
        /// <param name="toolWindow">tool window to be moved in front of the view</param>
        private void MoveInFront(DockableToolWindow toolWindow)
        {
            Controls.SetChildIndex(toolWindow, 0);

            if (ToolWindowSelected != null)
            {
                ToolSelectionChangedEventArgs e = new ToolSelectionChangedEventArgs(toolWindow);
                ToolWindowSelected(this, e);
            }
        }

        /// <summary>
        /// Move the entire collection of tool windows in front of the view.
        /// The last tool window from the collection will became the top most window
        /// </summary>
        /// <param name="toolWindows">tool windows collection to be moved in front of the view</param>
        private void MoveInFront(List<DockableToolWindow> toolWindows)
        {
            foreach (DockableToolWindow toolWindow in toolWindows)
            {
                MoveInFront(toolWindow);
            }
        }

        /// <summary>
        /// Select tool window from point
        /// </summary>
        /// <param name="location">location</param>
        private void SelectToolWindowFromPoint(Point location)
        {
            foreach (TabButton button in _tabButtons)
            {
                if (button.Bounds.Contains(location))
                {
                    SelectToolWindow((DockableToolWindow)button.TitleData);
                    IncreaseStateCheckFrequency();
                    return;
                }
            }
        }

        /// <summary>
        /// Create a tab button associated with the given tool window
        /// </summary>
        /// <param name="toolWindow">tool window for which the tab button will be created</param>
        private void CreateTabButton(DockableToolWindow toolWindow)
        {
            TabButton button = new TabButton(this, toolWindow);
            toolWindow.TabButton = button;

            button.NotSelectedColor = TabButtonNotSelectedColor;
            button.SelectedBackColor1 = TabButtonSelectedBackColor1;
            button.SelectedBackColor2 = TabButtonSelectedBackColor2;
            button.SelectedBorderColor = TabButtonSelectedBorderColor;
            button.SelectedColor = TabButtonSelectedColor;
            button.ShowSelection = TabButtonShowSelection;

            _tabButtons.Add(button);
        }

        /// <summary>
        /// Remove the tab button associated with the given tool window
        /// </summary>
        /// <param name="toolWindow">tool window for which the tab button must be removed</param>
        private void RemoveTabButton(DockableToolWindow toolWindow)
        {
            _tabButtons.Remove((TabButton)toolWindow.TabButton);

            toolWindow.TabButton = null;
        }

        /// <summary>
        /// Unlocks the moved tool window
        /// </summary>
        private void UnlockMovedToolWindow()
        {
            if (_movedToolWindow != null)
            {
                if (_movedToolWindow.IsDocked == false)
                {
                    _movedToolWindow.UnlockFormSize();
                }
                _movedToolWindow = null;
            }
        }

        /// <summary>
        /// Detect mouse hoover over given bounds
        /// </summary>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        /// <param name="hooverBounds">hoover bounds in client coordinates</param>
        /// <param name="hoover">hoover flag indicating if mouse was hoovering over the given bounds</param>
        private void DetectMouseHoover(Point mousePosition, Rectangle hooverBounds, ref bool hoover)
        {
            if (hooverBounds.Contains(PointToClient(mousePosition)))
            {
                if (hoover == false)
                {
                    hoover = true;
                    Invalidate();
                }
            }
            else if (hoover)
            {
                hoover = false;
                Invalidate();
            }
        }

        /// <summary>
        /// Update the preview engine
        /// </summary>
        /// <param name="mousePosition">mouse position in screen coordinates</param>
        private void UpdatePreviewEngine(Point mousePosition)
        {
            if (_dockPreviewEngine.Visibile == false)
            {
                return;
            }

            _dockPreviewEngine.LeftPreviewBounds = _panels.GetPanelNonHiddenBounds(zDockMode.Left);
            _dockPreviewEngine.RightPreviewBounds = _panels.GetPanelNonHiddenBounds(zDockMode.Right);
            _dockPreviewEngine.TopPreviewBounds = _panels.GetPanelNonHiddenBounds(zDockMode.Top);
            _dockPreviewEngine.BottomPreviewBounds = _panels.GetPanelNonHiddenBounds(zDockMode.Bottom);
            _dockPreviewEngine.FillPreviewBounds = _panels.GetPanelNonHiddenBounds(zDockMode.Fill);

            _dockPreviewEngine.UpdateDockPreviewOnMouseMove(mousePosition);
        }

        /// <summary>
        /// Bounds of the close button from the center panel
        /// </summary>
        private Rectangle CenterCloseButtonBounds
        {
            get
            {
                Rectangle bounds = _panels.GetPanelButtonsBounds(zDockMode.Fill);
                return new Rectangle(bounds.Right - 18, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
            }
        }

        /// <summary>
        /// Bounds of the menu button from the center panel
        /// </summary>
        private Rectangle CenterMenuButtonBounds
        {
            get
            {
                Rectangle bounds = _panels.GetPanelButtonsBounds(zDockMode.Fill);
                return new Rectangle(bounds.Right - 38, bounds.Y + (bounds.Height - 12) / 2, 12, 12);
            }
        }

        /// <summary>
        /// Increase state check frequency
        /// </summary>
        private void IncreaseStateCheckFrequency()
        {
            if (_mouseCheckTimer != null)
            {
                _mouseCheckTimer.Interval = 100;
            }
        }

        /// <summary>
        /// Decrease state check frequency
        /// </summary>
        private void DecreaseStateCheckFrequency()
        {
            if (_mouseCheckTimer != null)
            {
                _mouseCheckTimer.Interval = 10000;
            }
        }

        /// <summary>
        /// Checks if the cursor is splitter
        /// </summary>
        /// <returns>true if the cursor is splitter</returns>
        private bool IsSplitterCursor()
        {
            return Cursor == Cursors.HSplit || Cursor == Cursors.VSplit;
        }

        #endregion Private section.
    }
}
