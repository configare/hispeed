namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    partial class UCAeronetSiteArgs
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.grbMethods = new System.Windows.Forms.GroupBox();
            this.chbHistogram = new System.Windows.Forms.CheckBox();
            this.chbRmse = new System.Windows.Forms.CheckBox();
            this.chbTimeSeq = new System.Windows.Forms.CheckBox();
            this.chbScatter = new System.Windows.Forms.CheckBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtToValDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labForVal = new System.Windows.Forms.Label();
            this.grpFiles = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lstForValFiles = new System.Windows.Forms.ListBox();
            this.grbMethods.SuspendLayout();
            this.grpFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(226, 294);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grbMethods
            // 
            this.grbMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbMethods.Controls.Add(this.chbHistogram);
            this.grbMethods.Controls.Add(this.chbRmse);
            this.grbMethods.Controls.Add(this.chbTimeSeq);
            this.grbMethods.Controls.Add(this.chbScatter);
            this.grbMethods.Location = new System.Drawing.Point(4, 218);
            this.grbMethods.Name = "grbMethods";
            this.grbMethods.Size = new System.Drawing.Size(303, 70);
            this.grbMethods.TabIndex = 16;
            this.grbMethods.TabStop = false;
            this.grbMethods.Text = "评估方式";
            // 
            // chbHistogram
            // 
            this.chbHistogram.AutoSize = true;
            this.chbHistogram.Location = new System.Drawing.Point(19, 42);
            this.chbHistogram.Name = "chbHistogram";
            this.chbHistogram.Size = new System.Drawing.Size(84, 16);
            this.chbHistogram.TabIndex = 3;
            this.chbHistogram.Text = "偏差直方图";
            this.chbHistogram.UseVisualStyleBackColor = true;
            // 
            // chbRmse
            // 
            this.chbRmse.AutoSize = true;
            this.chbRmse.Location = new System.Drawing.Point(133, 42);
            this.chbRmse.Name = "chbRmse";
            this.chbRmse.Size = new System.Drawing.Size(120, 16);
            this.chbRmse.TabIndex = 2;
            this.chbRmse.Text = "均方根误差(RMSE)";
            this.chbRmse.UseVisualStyleBackColor = true;
            // 
            // chbTimeSeq
            // 
            this.chbTimeSeq.AutoSize = true;
            this.chbTimeSeq.Location = new System.Drawing.Point(133, 20);
            this.chbTimeSeq.Name = "chbTimeSeq";
            this.chbTimeSeq.Size = new System.Drawing.Size(84, 16);
            this.chbTimeSeq.TabIndex = 1;
            this.chbTimeSeq.Text = "时间序列图";
            this.chbTimeSeq.UseVisualStyleBackColor = true;
            // 
            // chbScatter
            // 
            this.chbScatter.AutoSize = true;
            this.chbScatter.Location = new System.Drawing.Point(19, 20);
            this.chbScatter.Name = "chbScatter";
            this.chbScatter.Size = new System.Drawing.Size(60, 16);
            this.chbScatter.TabIndex = 0;
            this.chbScatter.Text = "散点图";
            this.chbScatter.UseVisualStyleBackColor = true;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(271, 165);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(26, 23);
            this.btnOpen.TabIndex = 14;
            this.btnOpen.Text = "...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtToValDir
            // 
            this.txtToValDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToValDir.Location = new System.Drawing.Point(123, 175);
            this.txtToValDir.Name = "txtToValDir";
            this.txtToValDir.Size = new System.Drawing.Size(146, 21);
            this.txtToValDir.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "观测点数据文件夹:";
            // 
            // labForVal
            // 
            this.labForVal.AutoSize = true;
            this.labForVal.Location = new System.Drawing.Point(10, 29);
            this.labForVal.Name = "labForVal";
            this.labForVal.Size = new System.Drawing.Size(83, 12);
            this.labForVal.TabIndex = 9;
            this.labForVal.Text = "待验卫星产品:";
            // 
            // grpFiles
            // 
            this.grpFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFiles.Controls.Add(this.btnAdd);
            this.grpFiles.Controls.Add(this.btnRemove);
            this.grpFiles.Controls.Add(this.btnOpen);
            this.grpFiles.Controls.Add(this.lstForValFiles);
            this.grpFiles.Location = new System.Drawing.Point(4, 8);
            this.grpFiles.Name = "grpFiles";
            this.grpFiles.Size = new System.Drawing.Size(303, 204);
            this.grpFiles.TabIndex = 15;
            this.grpFiles.TabStop = false;
            this.grpFiles.Text = "选择数据";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Image = global::GeoDo.RSS.MIF.Prds.PrdVal.Properties.Resources.cmdOpen;
            this.btnAdd.Location = new System.Drawing.Point(271, 40);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Image = global::GeoDo.RSS.MIF.Prds.PrdVal.Properties.Resources._002;
            this.btnRemove.Location = new System.Drawing.Point(271, 69);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(26, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lstForValFiles
            // 
            this.lstForValFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstForValFiles.FormattingEnabled = true;
            this.lstForValFiles.ItemHeight = 12;
            this.lstForValFiles.Location = new System.Drawing.Point(6, 40);
            this.lstForValFiles.Name = "lstForValFiles";
            this.lstForValFiles.Size = new System.Drawing.Size(259, 112);
            this.lstForValFiles.TabIndex = 4;
            // 
            // UCAeronetSiteArgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grbMethods);
            this.Controls.Add(this.txtToValDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labForVal);
            this.Controls.Add(this.grpFiles);
            this.Name = "UCAeronetSiteArgs";
            this.Size = new System.Drawing.Size(310, 325);
            this.grbMethods.ResumeLayout(false);
            this.grbMethods.PerformLayout();
            this.grpFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox grbMethods;
        private System.Windows.Forms.CheckBox chbHistogram;
        private System.Windows.Forms.CheckBox chbRmse;
        private System.Windows.Forms.CheckBox chbTimeSeq;
        private System.Windows.Forms.CheckBox chbScatter;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtToValDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labForVal;
        private System.Windows.Forms.GroupBox grpFiles;
        private System.Windows.Forms.ListBox lstForValFiles;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
    }
}
