using System;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.MIF.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class ExtractPanelFlowWindow : ToolWindow, ISmartToolWindow, IExtractPanelWindow
    {
        protected int _id;
        protected ISmartSession _session;
        private OnActiveWindowChangedHandler _activeWindowChanged;
        private ExtractPanelWindowContent _extratPanelWindowContent;
        private EventHandler _onWindowClosed;

        public ExtractPanelFlowWindow()
            : base()
        {
            _id = 90191;
            Text = "流程面板";
        }

        private void AddFlowPanel()
        {
            Panel panel1 = new Panel();
            panel1.AutoScroll = true;
            panel1.Size = new Size(461, 90);
            panel1.Dock = DockStyle.Bottom;
            panel1.BorderStyle = BorderStyle.FixedSingle;

            Controls.Add(panel1);
            Button button = new Button();
            button.Location = new System.Drawing.Point(24, 24);
            button.Text = "button2";
            button.Size = new System.Drawing.Size(75, 23);
            button.UseVisualStyleBackColor = true;
            panel1.Controls.Add(button);

            int currentBtnAnchorPointX = 99;
            for (int i = 0; i < 3; i++)
            {
                AddNextBtns(panel1, currentBtnAnchorPointX);
                currentBtnAnchorPointX += 123;
            }
        }


        private static void AddNextBtns(Panel panel1, int currentBtnAnchorPointX)
        {
            Label label = new Label();
            label.AutoSize = true;
            int labelx = currentBtnAnchorPointX + 6;
            label.Location = new System.Drawing.Point(labelx, 29);
            label.Size = new System.Drawing.Size(17, 12);
            label.Text = "→";
            panel1.Controls.Add(label);

            ComboBox comboBox = new ComboBox();
            comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox.FormattingEnabled = true;
            comboBox.Items.AddRange(new object[]
            {
                "一",
                "23",
                "二",
                "三"
            });
            int comboBoxx = currentBtnAnchorPointX + 29;
            comboBox.Location = new Point(comboBoxx, 26);
            comboBox.SelectedIndex = 0;
            comboBox.Size = new Size(94, 20);
            panel1.Controls.Add(comboBox);
        }

        void ExtractPanelWindow_Disposed(object sender, EventArgs e)
        {
            ISmartWindowManager mgr = _session.SmartWindowManager;
            if (_activeWindowChanged != null)
            {
                mgr.OnActiveWindowChanged -= _activeWindowChanged;
                _activeWindowChanged = null;
            }
            if (_extratPanelWindowContent != null)
            {
                _extratPanelWindowContent.Dispose();
                _extratPanelWindowContent.Free();
                _extratPanelWindowContent = null;
                _session = null;
            }
        }

        public int Id
        {
            get { return _id; }
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public bool DisableIntimeExtracting
        {
            get { return _extratPanelWindowContent.DisableIntimeExtracting; }
            set { _extratPanelWindowContent.DisableIntimeExtracting = value; }
        }

        public bool IsShowSaveButton
        {
            get { return _extratPanelWindowContent.IsShowSaveButton; }
            set { _extratPanelWindowContent.IsShowSaveButton = value; }

        }

        public bool IsShowIntimeCheckBox
        {
            get { return _extratPanelWindowContent.IsShowIntimeCheckBox; }
            set { _extratPanelWindowContent.IsShowIntimeCheckBox = value; }
        }

        public void CanResetUserControl()
        {
            _extratPanelWindowContent.CanResetUserControl();
        }

        public void DoOk()
        {
            _extratPanelWindowContent.DoOk();
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;

            _extratPanelWindowContent = new ExtractPanelWindowContent();
            _extratPanelWindowContent.Dock = DockStyle.Fill;
            this.Controls.Add(_extratPanelWindowContent);

            //AddFlowPanel();

            _activeWindowChanged = ActiveWindowChanged;
            Disposed += ExtractPanelWindow_Disposed;

            _extratPanelWindowContent.Apply(session);
            ISmartWindowManager mgr = _session.SmartWindowManager;
            mgr.OnActiveWindowChanged += _activeWindowChanged;
        }

        public void Apply(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            _extratPanelWindowContent.Apply(wks, subProduct);
        }

        void ActiveWindowChanged(object sender, ISmartWindow oldWindow, ISmartWindow newWindow)
        {
            //解决第一次打开判识面板打不开的问题
            if (newWindow != null && newWindow.Equals(oldWindow))
                return;
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            if (cv == null || !cv.Equals(msession.CurrentCanvasViewer))
            {
                if (!IsKeepUserControl)
                    this.Close();
            }
        }
        
        /// <summary>
        /// 是否需要保持原面板
        /// </summary>
        public bool IsKeepUserControl
        {
            get
            {
                bool brs = false;
                Control control = _extratPanelWindowContent.Panel2Control0;
                if (control is UCExtractPanel)
                {
                    UCExtractPanel ucExtract = control as UCExtractPanel;
                    try
                    {
                        brs = ucExtract.MonitoringSubProduct.Definition.IsKeepUserControl;
                    }
                    catch
                    {
                    }
                }
                return brs;
            }
        }
    }
}
