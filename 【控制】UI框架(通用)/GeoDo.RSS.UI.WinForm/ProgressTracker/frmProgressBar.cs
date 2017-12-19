using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.WinForm
{
    public class frmProgressBar : RadForm, IProgressMonitor, IProgressTrackerSupport
    {
        private RadProgressBar radProgressBar1;
        private Form _owner = null;
        private RadLabel label1;
        private int _perTimes =0;
        private int _totalTimes = 100;

        public frmProgressBar()
        {
            InitializeComponent();
            //this.TopMost = true;
            this.TopLevel = true;
            this.Height = 44;
            label1 = new RadLabel();
            label1.AutoSize = false;
            label1.Height = 22;
            label1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            label1.Dock = DockStyle.Fill;
            radProgressBar1.Height = 22;
            radProgressBar1.Dock = DockStyle.Bottom;
            this.Controls.Add(label1);
            this.Controls.Add(radProgressBar1);
        }

        public frmProgressBar(Form owner)
            : this()
        {
            _owner = owner;
        }

        public void Reset(string text, int totalTimes)
        {
            _totalTimes = totalTimes;
            radProgressBar1.Minimum = 0;
            radProgressBar1.Maximum = totalTimes;
            label1.Text = text;
        }

        public void Reset(string text, int totalTimes, params ProgressSegment[] segments)
        {
            _totalTimes = totalTimes;
            radProgressBar1.Minimum = 0;
            radProgressBar1.Maximum = totalTimes;
            //radProgressBar1.Text = text;
            label1.Text = text;
        }

        public void Start(bool isAuto)
        {
            if (_owner == null)
                this.StartPosition = FormStartPosition.CenterScreen;
            else
                this.StartPosition = FormStartPosition.CenterParent;
            if (this.Visible)
                this.Visible = false;
            this.Show(_owner);
        }

        public void Boost(int iTimes)
        {
            int iProgress = (int)(iTimes * 100f / _totalTimes);
            int perProgress = (int)(_perTimes * 100f / _totalTimes);
            if (iProgress > perProgress)
            {
                UpdateProgressInvokeRequired(new ProgressArg { Progress = iTimes });
            }
        }

        public void Boost(int iTimes, string segmentText)
        {
            int iProgress = (int)(iTimes * 100f / _totalTimes);
            int perProgress = (int)(_perTimes * 100f / _totalTimes);
            if (iProgress > perProgress)
            {
                UpdateProgressInvokeRequired(new ProgressArg { Progress = iTimes, Text = segmentText });
            }
        }

        public void Boost(int iTimes, string text, string segmentText)
        {
            int iProgress = (int)(iTimes * 100f / _totalTimes);
            int perProgress = (int)(_perTimes * 100f / _totalTimes);
            if (iProgress > perProgress)
            {
                UpdateProgressInvokeRequired(new ProgressArg { Progress = iTimes, Text = text });
            }
        }

        public void Finish()
        {
            this.Hide();
            UpdateProgressInvokeRequired(new ProgressArg { });
        }

        private void UpdateProgressInvokeRequired(object args)
        {
            if (args != null)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action<int, string>(UpdateProgress), (args as ProgressArg).Progress, (args as ProgressArg).Text);
                else
                    UpdateProgress((args as ProgressArg).Progress, (args as ProgressArg).Text);
            }
        }

        private void UpdateProgress(int iTimes, string text)
        {
            if (iTimes < 0)
                iTimes = radProgressBar1.Minimum;
            else if (iTimes > radProgressBar1.Maximum)
                iTimes = radProgressBar1.Maximum;
            _perTimes = iTimes;
            radProgressBar1.Value1 = iTimes;
            label1.Text = text;
            this.Invalidate();//;
            Application.DoEvents();
        }

        private void InitializeComponent()
        {
            this.radProgressBar1 = new Telerik.WinControls.UI.RadProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radProgressBar1
            // 
            this.radProgressBar1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.radProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radProgressBar1.ImageIndex = -1;
            this.radProgressBar1.ImageKey = "";
            this.radProgressBar1.ImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.radProgressBar1.Location = new System.Drawing.Point(0, 0);
            this.radProgressBar1.Name = "radProgressBar1";
            this.radProgressBar1.SeparatorColor1 = System.Drawing.Color.White;
            this.radProgressBar1.SeparatorColor2 = System.Drawing.Color.White;
            this.radProgressBar1.SeparatorColor3 = System.Drawing.Color.White;
            this.radProgressBar1.SeparatorColor4 = System.Drawing.Color.White;
            this.radProgressBar1.Size = new System.Drawing.Size(0, 20);
            this.radProgressBar1.TabIndex = 0;
            // 
            // frmProgressBar3
            // 
            this.ClientSize = new System.Drawing.Size(588, 44);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmProgressBar3";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "";
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
    }

    internal class ProgressArg
    {
        public int Progress = 0;
        public string Text = "";
    }
}
