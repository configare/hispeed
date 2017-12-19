using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.Win32;
using System.Drawing;

namespace Telerik.WinControls
{
    public class SystemSkinManager
    {
        #region Constructor

        public SystemSkinManager()
        {
            themeHandles = new Dictionary<string, IntPtr>();

            //handle this notification to properly close all theme handles
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the Color defined for the current element.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public Color GetThemeColor(ColorProperty color)
        {
            if (this.currentElement == null || currentHTheme == IntPtr.Zero)
            {
                return Color.Empty;
            }

            int themeColor = 0;
            UXTheme.GetThemeColor(this.currentHTheme, currentElement.Part, currentElement.State, (int)color, ref themeColor);

            return ColorTranslator.FromWin32(themeColor);
        }

        public Size GetPartPreferredSize(Graphics g, Rectangle bounds, ThemeSizeType type)
        {
            if (this.currentElement == null || currentHTheme == IntPtr.Zero)
            {
                return Size.Empty;
            }

            RadHdcWrapper wrapper = new RadHdcWrapper(g, false);
            IntPtr hdc = wrapper.GetHdc();

            NativeMethods.SIZE sz = new NativeMethods.SIZE();
            UXTheme.GetThemePartSize(this.currentHTheme, hdc, this.currentElement.Part, this.currentElement.State, IntPtr.Zero, type, sz);

            wrapper.Dispose();

            return new Size(sz.cx, sz.cy);
        }

        /// <summary>
        /// Sets the specified element as the "Current" for painting.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True if the element may be painted (there is a theme part defined for it), false otherwise.</returns>
        public bool SetCurrentElement(VisualStyleElement element)
        {
            this.currentHTheme = IntPtr.Zero;
            this.currentElement = null;

            if (element == null)
            {
                return false;
            }

            IntPtr hTheme = GetHTheme(element.ClassName);
            if (hTheme == IntPtr.Zero)
            {
                return false;
            }

            if (!UXTheme.IsThemePartDefined(hTheme, element.Part, 0))
            {
                return false;
            }

            this.currentHTheme = hTheme;
            this.currentElement = element;

            return true;
        }

        /// <summary>
        /// Paints the current element (previously specified by the SetCurrentElement method)
        /// on the provided graphics surface, within the desired bounds.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        public void PaintCurrentElement(Graphics g, Rectangle bounds)
        {
            if (this.currentHTheme == IntPtr.Zero)
            {
                return;
            }

            RadHdcWrapper hdcWrapper = new RadHdcWrapper(g, true);
            IntPtr hdc = hdcWrapper.GetHdc();

            NativeMethods.RECT rect = new NativeMethods.RECT();
            rect.left = bounds.Left;
            rect.top = bounds.Top;
            rect.bottom = rect.top + bounds.Height;
            rect.right = rect.left + bounds.Width;

            UXTheme.DrawThemeBackground(this.currentHTheme, hdc, this.currentElement.Part, this.currentElement.State, ref rect, IntPtr.Zero);

            hdcWrapper.Dispose();
        }

        /// <summary>
        /// Invalidates all opened forms upon a user-triggered change in this class.
        /// </summary>
        public void NotifyChange()
        {
            //this is a global change, we need to repaint all opened forms
            foreach (Form form in Application.OpenForms)
            {
                form.Invalidate(true);
            }
        }

        /// <summary>
        /// closes all currently opened HTheme handles
        /// </summary>
        private void CloseAllHandles()
        {
            try
            {
                //release all htheme handles
                foreach (KeyValuePair<string, IntPtr> pair in this.themeHandles)
                {
                    UXTheme.CloseThemeData(pair.Value);
                }

                this.themeHandles.Clear();

                if (this.vistaExplorerThemeOwner != null)
                {
                    this.vistaExplorerThemeOwner.Dispose();
                    this.vistaExplorerThemeOwner = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Looks-up a HTheme handle.
        /// </summary>
        /// <param name="windowClass"></param>
        /// <returns></returns>
        private IntPtr GetHTheme(string windowClass)
        {
            IntPtr hTheme;
            if (!themeHandles.TryGetValue(windowClass, out hTheme))
            {
                if (vistaExplorerThemeOwner == null)
                {
                    vistaExplorerThemeOwner = new Control();
                    vistaExplorerThemeOwner.CreateControl();
                    UXTheme.SetWindowTheme(vistaExplorerThemeOwner.Handle, Explorer, null);
                }

                //special case for TreeView and ListView classes
                //load Vista's Explorer theme if needed
                IntPtr handle = IntPtr.Zero;
                string classToLower = windowClass.ToLower();
                if (classToLower == VisualStyleElement.TreeView.Item.Normal.ClassName.ToLower() ||
                    classToLower == VisualStyleElement.ListView.Item.Normal.ClassName.ToLower())
                {
                    handle = vistaExplorerThemeOwner.Handle;
                }
                hTheme = UXTheme.OpenThemeData(handle, windowClass);
                this.themeHandles.Add(windowClass, hTheme);
            }

            return hTheme;
        }

        #endregion

        #region Event Handlers

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //we need to close all opened handles here
            CloseAllHandles();
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.VisualStyle)
            {
                //we need to close all opened handles here
                this.CloseAllHandles();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently attached element.
        /// </summary>
        public VisualStyleElement CurrentElement
        {
            get
            {
                return this.currentElement;
            }
        }

        /// <value>
        /// Returns true on Windows Vista or newer operating systems; otherwise, false.
        /// </value>
        public static bool IsVistaOrLater
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && 
                       Environment.OSVersion.Version.Major >= 6;
            }
        }

        /// <summary>
        /// Determines whether system skins will be applied on RadControls
        /// </summary>
        public bool UseSystemSkin
        {
            get
            {
                return this.useSystemSkin;
            }
            set
            {
                if (this.useSystemSkin == value)
                {
                    return;
                }

                this.useSystemSkin = value;
                NotifyChange();
            }
        }

        /// <summary>
        /// Gets the only instance of this manager.
        /// </summary>
        public static SystemSkinManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SystemSkinManager();
                }

                return instance;
            }
        }

        public VisualStyleRenderer Renderer
        {
            get
            {
                if (renderer == null)
                {
                    renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
                }
                return this.renderer;
            }
        }

        #endregion

        #region Fields

        private VisualStyleElement currentElement;
        private IntPtr currentHTheme;

        private Dictionary<string, IntPtr> themeHandles;
        private const string Explorer = "explorer";

        private Control vistaExplorerThemeOwner;
        private bool useSystemSkin;
        private VisualStyleRenderer renderer;

        private static SystemSkinManager instance;

        /// <summary>
        /// Used internally by the framework to determine whether we just want to skip TPF's drawing.
        /// </summary>
        public static readonly VisualStyleElement EmptyElement = VisualStyleElement.CreateElement("", 0, 0);

        /// <summary>
        /// Used to instruct the system skin painting mechanism that a custom painting will be performed
        /// in the PaintElementSkin method.
        /// </summary>
        public static readonly VisualStyleElement DefaultElement = VisualStyleElement.Button.PushButton.Default;

        #endregion
    }
}
