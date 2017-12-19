using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using FastColoredTextBoxNS;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Python
{
    public partial class PythonEditorWindow : ToolWindow, ISmartViewer, IPythonEditorWindow
    {
        private int _id = 20003;
        private ISmartSession _session = null;
        Style KeywordsStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        FastColoredTextBox tb;
        EventHandler _onWindowClosed;

        public PythonEditorWindow()
        {
            InitializeComponent();
        }

        public void CreateTB(string fileName)
        {
            try
            {
                tb = new FastColoredTextBox();
                tb.Dock = DockStyle.Fill;
                tb.BorderStyle = BorderStyle.Fixed3D;
                tb.LeftPadding = 17;
                tb.Language = Language.CSharp;
                tb.AddStyle(new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray))));//same words style
                this.Tag = fileName;
                if (fileName != null)
                    try
                    {
                        tb.Text = File.ReadAllText(fileName);
                    }
                    catch (Exception e)
                    {
                    }
                tb.ClearUndo();
                tb.IsChanged = false;
                //
                tb.ImeMode = System.Windows.Forms.ImeMode.On;

                tb.Focus();
                tb.DelayedTextChangedInterval = 1000;
                tb.DelayedEventsInterval = 1000;
                tb.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(tb_TextChangedDelayed);
                tb.HighlightingRangeType = HighlightingRangeType.VisibleRange;
                AutocompleteMenu popupMenu = new AutocompleteMenu(tb);
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                    CreateTB(fileName);
            }
            Controls.Add(this.tb);
        }

        private void tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            //clear styles
            tb.Range.ClearStyle(KeywordsStyle);
            //highlight keywords of python
            tb.Range.SetStyle(KeywordsStyle, @"\b(and|continue|else|for|import|not|raise|assert|def|except|from|in|or|return|break|del|exec|global|is|pass|try|class|elif|finally|if|lambda|print|while|log)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;

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

        string ISmartViewer.Title
        {
            get { throw new NotImplementedException(); }
        }

        object ISmartViewer.ActiveObject
        {
            get { throw new NotImplementedException(); }
        }


        public FastColoredTextBox getTB()
        {
            return tb;

        }

        public void CloseWnd()
        {
        }

        public void DisposeViewer()
        {
        }
    }
}
