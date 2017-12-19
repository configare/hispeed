using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class ContextMessageWindow : ToolWindowBase, ISmartToolWindow, IContextMessage
    {
        private RichTextBox _textBox;

        public ContextMessageWindow()
        {
            InitializeComponent();
            _id = 9006;
            Text = "上下文消息窗口";
            AddControls();
        }

        private void AddControls()
        {
            _textBox = new RichTextBox();
            _textBox.MouseUp += new MouseEventHandler(_textBox_MouseUp);
            _textBox.Dock = DockStyle.Fill;
            this.Controls.Add(_textBox);
        }

        void _textBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
                MenuItem it = new MenuItem();
                it.Text = "清除";
                it.Click += new EventHandler(it_Click);
                contextMenu.MenuItems.Add(it);
                contextMenu.Show(_textBox, e.Location);
            }
        }

        void it_Click(object sender, EventArgs e)
        {
            _textBox.Text = string.Empty;
        }

        public void PrintMessage(string message)
        {
            if (message != null)
            {
                _textBox.AppendText(message + "\n");
                _textBox.SelectionStart = _textBox.Text.Length;
                _textBox.Focus();
            }
        }
    }
}
