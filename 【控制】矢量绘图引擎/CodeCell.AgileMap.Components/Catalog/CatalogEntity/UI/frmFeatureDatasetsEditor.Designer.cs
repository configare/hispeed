namespace CodeCell.AgileMap.Components
{
    partial class frmFeatureDatasetsEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ucSpatialRef1 = new UCSpatialRef();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDecription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMapScale = new System.Windows.Forms.NumericUpDown();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapScale)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtMapScale);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtDecription);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtSource);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Size = new System.Drawing.Size(437, 203);
            this.tabPage1.Text = "基本属性";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(382, 246);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(301, 246);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Size = new System.Drawing.Size(445, 228);
            this.tabControl1.Controls.SetChildIndex(this.tabPage2, 0);
            this.tabControl1.Controls.SetChildIndex(this.tabPage1, 0);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ucSpatialRef1);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(437, 203);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "空间参考";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ucSpatialRef1
            // 
            this.ucSpatialRef1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSpatialRef1.Location = new System.Drawing.Point(0, 0);
            this.ucSpatialRef1.Name = "ucSpatialRef1";
            this.ucSpatialRef1.Size = new System.Drawing.Size(437, 203);
            this.ucSpatialRef1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(22, 32);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(398, 21);
            this.txtName.TabIndex = 0;
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(22, 126);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(398, 21);
            this.txtSource.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "测绘比例尺:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "来源:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "描述:";
            // 
            // txtDecription
            // 
            this.txtDecription.Location = new System.Drawing.Point(22, 170);
            this.txtDecription.Name = "txtDecription";
            this.txtDecription.Size = new System.Drawing.Size(398, 21);
            this.txtDecription.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "1 : ";
            // 
            // txtMapScale
            // 
            this.txtMapScale.Location = new System.Drawing.Point(49, 83);
            this.txtMapScale.Maximum = new decimal(new int[] {
            1410065408,
            2,
            0,
            0});
            this.txtMapScale.Name = "txtMapScale";
            this.txtMapScale.Size = new System.Drawing.Size(371, 21);
            this.txtMapScale.TabIndex = 8;
            this.txtMapScale.ThousandsSeparator = true;
            this.txtMapScale.Value = new decimal(new int[] {
            5000000,
            0,
            0,
            0});
            // 
            // frmFeatureDatasetsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 278);
            this.Name = "frmFeatureDatasetsEditor";
            this.Text = "";
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMapScale)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.NumericUpDown txtMapScale;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDecription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label2;
        private UCSpatialRef ucSpatialRef1;
    }
}