using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 异常显示类
    /// </summary>
    internal class frmException : Form
    {
        private System.ComponentModel.Container components = null;

        private System.Windows.Forms.TextBox txtStackTrace;
        private System.Windows.Forms.Button btnShowDetail;

        protected const int NormalHeight = 190;
        private System.Windows.Forms.PictureBox picTip;
        private System.Windows.Forms.TextBox lblErrMessage;
        protected const int ExtandHeight = 352;

        public frmException()
        {
            InitializeComponent();
        }

        public Exception ErrException
        {
            set
            {
                if (value != null)
                {
                    Exception exp = value;
                    while (exp != null)
                    {
                        this.txtStackTrace.Text += (exp.Message + "\n");
                        exp = exp.InnerException;
                    }
                    this.txtStackTrace.Text += "\n\n";
                    this.txtStackTrace.Text += value.StackTrace;
                }
            }
        }

        public string ActionDescription
        {
            set
            {
                this.lblErrMessage.Text = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmException));
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            this.btnShowDetail = new System.Windows.Forms.Button();
            this.picTip = new System.Windows.Forms.PictureBox();
            this.lblErrMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.Location = new Point(8, 168);
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ReadOnly = true;
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStackTrace.Size = new Size(408, 152);
            this.txtStackTrace.TabIndex = 0;
            this.txtStackTrace.Text = "";
            // 
            // btnShowDetail
            // 
            this.btnShowDetail.Location = new System.Drawing.Point(288, 136);
            this.btnShowDetail.Name = "btnShowDetail";
            this.btnShowDetail.Size = new System.Drawing.Size(128, 24);
            this.btnShowDetail.TabIndex = 1;
            this.btnShowDetail.Text = "详细错误信息(&D)>>";
            this.btnShowDetail.Click += new System.EventHandler(this.btnShowDetail_Click);
            // 
            // picTip
            // 
            this.picTip.Image = ((System.Drawing.Image)(resources.GetObject("picTip.Image")));
            this.picTip.Location = new System.Drawing.Point(8, 8);
            this.picTip.Name = "picTip";
            this.picTip.Size = new System.Drawing.Size(46, 46);
            this.picTip.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTip.TabIndex = 3;
            this.picTip.TabStop = false;
            // 
            // lblErrMessage
            // 
            this.lblErrMessage.BackColor = System.Drawing.SystemColors.Control;
            this.lblErrMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblErrMessage.Location = new System.Drawing.Point(64, 24);
            this.lblErrMessage.Multiline = true;
            this.lblErrMessage.Name = "lblErrMessage";
            this.lblErrMessage.ReadOnly = true;
            this.lblErrMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lblErrMessage.Size = new System.Drawing.Size(352, 104);
            this.lblErrMessage.TabIndex = 4;
            this.lblErrMessage.Text = "";
            // 
            // frmException
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(426, 328);
            this.Controls.Add(this.lblErrMessage);
            this.Controls.Add(this.picTip);
            this.Controls.Add(this.btnShowDetail);
            this.Controls.Add(this.txtStackTrace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmException";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "错误信息";
            this.Load += new System.EventHandler(this.frmException_Load);
            this.ResumeLayout(false);

        }
        #endregion

        private void frmException_Load(object sender, System.EventArgs e)
        {
            this.Height = NormalHeight;
        }

        private void btnShowDetail_Click(object sender, System.EventArgs e)
        {
            if (this.Size.Height == ExtandHeight)
            {
                this.Height = NormalHeight;
            }
            else
            {
                this.Height = ExtandHeight;
            }
        }
    }
}
