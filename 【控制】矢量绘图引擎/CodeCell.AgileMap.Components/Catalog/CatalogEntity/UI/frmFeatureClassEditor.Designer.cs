using CodeCell.AgileMap.Components;
namespace CodeCell.AgileMap.Components
{
    partial class frmFeatureClassEditor
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
            this.label6 = new System.Windows.Forms.Label();
            this.txtDataset = new System.Windows.Forms.TextBox();
            this.btnDataset = new System.Windows.Forms.Button();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapScale)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnDataset);
            this.tabPage1.Controls.Add(this.txtDataset);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.txtMapScale);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtDecription);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtSource);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Size = new System.Drawing.Size(437, 243);
            this.tabPage1.Text = "基本属性";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(382, 286);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(301, 286);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Size = new System.Drawing.Size(445, 268);
            this.tabControl1.Controls.SetChildIndex(this.tabPage2, 0);
            this.tabControl1.Controls.SetChildIndex(this.tabPage1, 0);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ucSpatialRef1);
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(437, 243);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "空间参考";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ucSpatialRef1
            // 
            this.ucSpatialRef1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSpatialRef1.Location = new System.Drawing.Point(0, 0);
            this.ucSpatialRef1.Name = "ucSpatialRef1";
            this.ucSpatialRef1.Size = new System.Drawing.Size(437, 243);
            this.ucSpatialRef1.SpatialReference = null;
            this.ucSpatialRef1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(19, 69);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(398, 21);
            this.txtName.TabIndex = 0;
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(19, 163);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(398, 21);
            this.txtSource.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "测绘比例尺:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "来源:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "描述:";
            // 
            // txtDecription
            // 
            this.txtDecription.Location = new System.Drawing.Point(19, 207);
            this.txtDecription.Name = "txtDecription";
            this.txtDecription.Size = new System.Drawing.Size(398, 21);
            this.txtDecription.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "1 : ";
            // 
            // txtMapScale
            // 
            this.txtMapScale.Location = new System.Drawing.Point(46, 120);
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "所属数据集:";
            // 
            // txtDataset
            // 
            this.txtDataset.Location = new System.Drawing.Point(19, 30);
            this.txtDataset.Name = "txtDataset";
            this.txtDataset.ReadOnly = true;
            this.txtDataset.Size = new System.Drawing.Size(365, 21);
            this.txtDataset.TabIndex = 10;
            // 
            // btnDataset
            // 
            this.btnDataset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDataset.Location = new System.Drawing.Point(390, 28);
            this.btnDataset.Name = "btnDataset";
            this.btnDataset.Size = new System.Drawing.Size(27, 23);
            this.btnDataset.TabIndex = 11;
            this.btnDataset.UseVisualStyleBackColor = true;
            this.btnDataset.Click += new System.EventHandler(this.btnDataset_Click);
            // 
            // frmFeatureClassEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 318);
            this.Name = "frmFeatureClassEditor";
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
        private System.Windows.Forms.TextBox txtDataset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDataset;
    }
}