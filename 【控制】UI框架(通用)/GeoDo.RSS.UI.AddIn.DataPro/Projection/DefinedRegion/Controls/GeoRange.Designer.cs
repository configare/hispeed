namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class GeoRange
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMinLat = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMaxLon = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMinLon = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMaxLat = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(8, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "最大纬度";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "最小经度";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(8, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "最大经度";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(8, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "最小纬度";
            // 
            // txtMinLat
            // 
            this.txtMinLat.Location = new System.Drawing.Point(68, 77);
            this.txtMinLat.Name = "txtMinLat";
            this.txtMinLat.Size = new System.Drawing.Size(64, 21);
            this.txtMinLat.TabIndex = 6;
            this.txtMinLat.Text = "0";
            this.txtMinLat.Value = 0D;
            // 
            // txtMaxLon
            // 
            this.txtMaxLon.Location = new System.Drawing.Point(68, 43);
            this.txtMaxLon.Name = "txtMaxLon";
            this.txtMaxLon.Size = new System.Drawing.Size(64, 21);
            this.txtMaxLon.TabIndex = 4;
            this.txtMaxLon.Text = "0";
            this.txtMaxLon.Value = 0D;
            // 
            // txtMinLon
            // 
            this.txtMinLon.Location = new System.Drawing.Point(68, 10);
            this.txtMinLon.Name = "txtMinLon";
            this.txtMinLon.Size = new System.Drawing.Size(64, 21);
            this.txtMinLon.TabIndex = 2;
            this.txtMinLon.Text = "0";
            this.txtMinLon.Value = 0D;
            // 
            // txtMaxLat
            // 
            this.txtMaxLat.Location = new System.Drawing.Point(68, 110);
            this.txtMaxLat.Name = "txtMaxLat";
            this.txtMaxLat.Size = new System.Drawing.Size(64, 21);
            this.txtMaxLat.TabIndex = 0;
            this.txtMaxLat.Text = "0";
            this.txtMaxLat.Value = 0D;
            // 
            // UCGeoRangeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMinLat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMaxLon);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMinLon);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMaxLat);
            this.Name = "UCGeoRangeControl";
            this.Size = new System.Drawing.Size(144, 140);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DoubleTextBox txtMaxLat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DoubleTextBox txtMinLon;
        private System.Windows.Forms.Label label3;
        private DoubleTextBox txtMaxLon;
        private System.Windows.Forms.Label label4;
        private DoubleTextBox txtMinLat;
    }
}
