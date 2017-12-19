namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class CursorInfoWndContent
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
            this.txtInfo = new System.Windows.Forms.Label();
            this.txtSecondaryInfo = new System.Windows.Forms.CheckBox();
            this.txtCoordInfo = new System.Windows.Forms.CheckBox();
            this.txtOriginChannels = new System.Windows.Forms.CheckBox();
            this.txtSelectChannels = new System.Windows.Forms.CheckBox();
            this.rdDegreeMintueSecond = new System.Windows.Forms.RadioButton();
            this.rd10DecimalDegree = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.AutoSize = true;
            this.txtInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtInfo.Location = new System.Drawing.Point(3, 75);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(45, 17);
            this.txtInfo.TabIndex = 0;
            this.txtInfo.Text = "txtInfo";
            // 
            // txtSecondaryInfo
            // 
            this.txtSecondaryInfo.AutoSize = true;
            this.txtSecondaryInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtSecondaryInfo.Location = new System.Drawing.Point(4, 4);
            this.txtSecondaryInfo.Name = "txtSecondaryInfo";
            this.txtSecondaryInfo.Size = new System.Drawing.Size(75, 21);
            this.txtSecondaryInfo.TabIndex = 1;
            this.txtSecondaryInfo.Text = "辅助信息";
            this.txtSecondaryInfo.UseVisualStyleBackColor = true;
            this.txtSecondaryInfo.CheckedChanged += new System.EventHandler(this.txtSecondaryInfo_CheckedChanged);
            // 
            // txtCoordInfo
            // 
            this.txtCoordInfo.AutoSize = true;
            this.txtCoordInfo.Checked = true;
            this.txtCoordInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.txtCoordInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtCoordInfo.Location = new System.Drawing.Point(82, 4);
            this.txtCoordInfo.Name = "txtCoordInfo";
            this.txtCoordInfo.Size = new System.Drawing.Size(75, 21);
            this.txtCoordInfo.TabIndex = 2;
            this.txtCoordInfo.Text = "坐标信息";
            this.txtCoordInfo.UseVisualStyleBackColor = true;
            // 
            // txtOriginChannels
            // 
            this.txtOriginChannels.AutoSize = true;
            this.txtOriginChannels.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtOriginChannels.Location = new System.Drawing.Point(82, 30);
            this.txtOriginChannels.Name = "txtOriginChannels";
            this.txtOriginChannels.Size = new System.Drawing.Size(87, 21);
            this.txtOriginChannels.TabIndex = 3;
            this.txtOriginChannels.Text = "所有通道值";
            this.txtOriginChannels.UseVisualStyleBackColor = true;
            // 
            // txtSelectChannels
            // 
            this.txtSelectChannels.AutoSize = true;
            this.txtSelectChannels.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtSelectChannels.Location = new System.Drawing.Point(4, 30);
            this.txtSelectChannels.Name = "txtSelectChannels";
            this.txtSelectChannels.Size = new System.Drawing.Size(63, 21);
            this.txtSelectChannels.TabIndex = 4;
            this.txtSelectChannels.Text = "通道值";
            this.txtSelectChannels.UseVisualStyleBackColor = true;
            // 
            // rdDegreeMintueSecond
            // 
            this.rdDegreeMintueSecond.AutoSize = true;
            this.rdDegreeMintueSecond.Checked = true;
            this.rdDegreeMintueSecond.Location = new System.Drawing.Point(4, 55);
            this.rdDegreeMintueSecond.Name = "rdDegreeMintueSecond";
            this.rdDegreeMintueSecond.Size = new System.Drawing.Size(59, 16);
            this.rdDegreeMintueSecond.TabIndex = 5;
            this.rdDegreeMintueSecond.TabStop = true;
            this.rdDegreeMintueSecond.Text = "度分秒";
            this.rdDegreeMintueSecond.UseVisualStyleBackColor = true;
            // 
            // rd10DecimalDegree
            // 
            this.rd10DecimalDegree.AutoSize = true;
            this.rd10DecimalDegree.Location = new System.Drawing.Point(82, 55);
            this.rd10DecimalDegree.Name = "rd10DecimalDegree";
            this.rd10DecimalDegree.Size = new System.Drawing.Size(71, 16);
            this.rd10DecimalDegree.TabIndex = 6;
            this.rd10DecimalDegree.Text = "十进制度";
            this.rd10DecimalDegree.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.txtInfo);
            this.panel1.Controls.Add(this.txtSecondaryInfo);
            this.panel1.Controls.Add(this.txtCoordInfo);
            this.panel1.Controls.Add(this.txtOriginChannels);
            this.panel1.Controls.Add(this.txtSelectChannels);
            this.panel1.Controls.Add(this.rdDegreeMintueSecond);
            this.panel1.Controls.Add(this.rd10DecimalDegree);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(562, 267);
            this.panel1.TabIndex = 7;
            // 
            // CursorInfoWndContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "CursorInfoWndContent";
            this.Size = new System.Drawing.Size(562, 267);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label txtInfo;
        private System.Windows.Forms.CheckBox txtSecondaryInfo;
        private System.Windows.Forms.CheckBox txtCoordInfo;
        private System.Windows.Forms.CheckBox txtOriginChannels;
        private System.Windows.Forms.CheckBox txtSelectChannels;
        private System.Windows.Forms.RadioButton rdDegreeMintueSecond;
        private System.Windows.Forms.RadioButton rd10DecimalDegree;
        private System.Windows.Forms.Panel panel1;
    }
}
