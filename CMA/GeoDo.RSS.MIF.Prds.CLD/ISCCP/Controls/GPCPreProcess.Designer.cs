using GeoDo.RSS.UI.AddIn.DataPro;
namespace GeoDo.RSS.MIF.Prds.CLD
{
    partial class GPCPreProcess
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GPCPreProcess));
            this.panel1 = new System.Windows.Forms.Panel();
            this.数据集选择 = new System.Windows.Forms.GroupBox();
            this.treeviewdataset = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbxRegionlist = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAddRegion = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtInDir = new System.Windows.Forms.TextBox();
            this.txtOutDir = new System.Windows.Forms.TextBox();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnSaveNewRegion = new System.Windows.Forms.Button();
            this.btnCancelNewReg = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.dtbxMaxY = new GeoDo.RSS.MIF.Prds.CLD.DoubleTextBox();
            this.dtbxMinY = new GeoDo.RSS.MIF.Prds.CLD.DoubleTextBox();
            this.dtbxMinX = new GeoDo.RSS.MIF.Prds.CLD.DoubleTextBox();
            this.dtbxMaxX = new GeoDo.RSS.MIF.Prds.CLD.DoubleTextBox();
            this.panel1.SuspendLayout();
            this.数据集选择.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.数据集选择);
            this.panel1.Location = new System.Drawing.Point(2, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(347, 306);
            this.panel1.TabIndex = 1;
            // 
            // 数据集选择
            // 
            this.数据集选择.Controls.Add(this.treeviewdataset);
            this.数据集选择.Location = new System.Drawing.Point(0, 3);
            this.数据集选择.Name = "数据集选择";
            this.数据集选择.Size = new System.Drawing.Size(347, 298);
            this.数据集选择.TabIndex = 4;
            this.数据集选择.TabStop = false;
            this.数据集选择.Text = "数据集选择";
            // 
            // treeviewdataset
            // 
            this.treeviewdataset.CheckBoxes = true;
            this.treeviewdataset.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeviewdataset.Location = new System.Drawing.Point(3, 17);
            this.treeviewdataset.Name = "treeviewdataset";
            this.treeviewdataset.Size = new System.Drawing.Size(341, 262);
            this.treeviewdataset.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancelNewReg);
            this.groupBox1.Controls.Add(this.btnSaveNewRegion);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.btnAddRegion);
            this.groupBox1.Location = new System.Drawing.Point(363, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 267);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "输出范围";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(425, 369);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "开始处理";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtRegionName);
            this.panel2.Controls.Add(this.cbxRegionlist);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.dtbxMaxY);
            this.panel2.Controls.Add(this.dtbxMinY);
            this.panel2.Controls.Add(this.dtbxMinX);
            this.panel2.Controls.Add(this.dtbxMaxX);
            this.panel2.Location = new System.Drawing.Point(12, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(274, 210);
            this.panel2.TabIndex = 12;
            // 
            // cbxRegionlist
            // 
            this.cbxRegionlist.FormattingEnabled = true;
            this.cbxRegionlist.Location = new System.Drawing.Point(86, 19);
            this.cbxRegionlist.Name = "cbxRegionlist";
            this.cbxRegionlist.Size = new System.Drawing.Size(121, 20);
            this.cbxRegionlist.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "区域名称:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "最小经度";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "最大经度";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(117, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "最大纬度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(117, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "最小纬度";
            // 
            // btnAddRegion
            // 
            this.btnAddRegion.Location = new System.Drawing.Point(30, 236);
            this.btnAddRegion.Name = "btnAddRegion";
            this.btnAddRegion.Size = new System.Drawing.Size(75, 23);
            this.btnAddRegion.TabIndex = 11;
            this.btnAddRegion.Text = "添加范围";
            this.btnAddRegion.UseVisualStyleBackColor = true;
            this.btnAddRegion.Click += new System.EventHandler(this.btnAddRegion_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "输入目录：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 8;
            this.label7.Text = "输出目录：";
            // 
            // txtInDir
            // 
            this.txtInDir.Location = new System.Drawing.Point(86, 20);
            this.txtInDir.Name = "txtInDir";
            this.txtInDir.Size = new System.Drawing.Size(340, 21);
            this.txtInDir.TabIndex = 9;
            // 
            // txtOutDir
            // 
            this.txtOutDir.Location = new System.Drawing.Point(86, 52);
            this.txtOutDir.Name = "txtOutDir";
            this.txtOutDir.Size = new System.Drawing.Size(340, 21);
            this.txtOutDir.TabIndex = 10;
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAs.Image")));
            this.btnSaveAs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSaveAs.Location = new System.Drawing.Point(456, 52);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(23, 23);
            this.btnSaveAs.TabIndex = 11;
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpen.Location = new System.Drawing.Point(456, 18);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 23);
            this.btnOpen.TabIndex = 12;
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSaveNewRegion
            // 
            this.btnSaveNewRegion.Enabled = false;
            this.btnSaveNewRegion.Location = new System.Drawing.Point(111, 236);
            this.btnSaveNewRegion.Name = "btnSaveNewRegion";
            this.btnSaveNewRegion.Size = new System.Drawing.Size(75, 23);
            this.btnSaveNewRegion.TabIndex = 13;
            this.btnSaveNewRegion.Text = "保存";
            this.btnSaveNewRegion.UseVisualStyleBackColor = true;
            this.btnSaveNewRegion.Click += new System.EventHandler(this.btnSaveNewRegion_Click);
            // 
            // btnCancelNewReg
            // 
            this.btnCancelNewReg.Enabled = false;
            this.btnCancelNewReg.Location = new System.Drawing.Point(192, 236);
            this.btnCancelNewReg.Name = "btnCancelNewReg";
            this.btnCancelNewReg.Size = new System.Drawing.Size(75, 23);
            this.btnCancelNewReg.TabIndex = 14;
            this.btnCancelNewReg.Text = "取消编辑";
            this.btnCancelNewReg.UseVisualStyleBackColor = true;
            this.btnCancelNewReg.Click += new System.EventHandler(this.btnCancelNewReg_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(532, 369);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(86, 22);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(121, 21);
            this.txtRegionName.TabIndex = 14;
            this.txtRegionName.Visible = false;
            // 
            // dtbxMaxY
            // 
            this.dtbxMaxY.Location = new System.Drawing.Point(113, 61);
            this.dtbxMaxY.Name = "dtbxMaxY";
            this.dtbxMaxY.Size = new System.Drawing.Size(61, 21);
            this.dtbxMaxY.TabIndex = 0;
            this.dtbxMaxY.Text = "60";
            this.dtbxMaxY.Value = 60D;
            // 
            // dtbxMinY
            // 
            this.dtbxMinY.Location = new System.Drawing.Point(113, 173);
            this.dtbxMinY.Name = "dtbxMinY";
            this.dtbxMinY.Size = new System.Drawing.Size(61, 21);
            this.dtbxMinY.TabIndex = 0;
            this.dtbxMinY.Text = "10";
            this.dtbxMinY.Value = 10D;
            // 
            // dtbxMinX
            // 
            this.dtbxMinX.Location = new System.Drawing.Point(50, 112);
            this.dtbxMinX.Name = "dtbxMinX";
            this.dtbxMinX.Size = new System.Drawing.Size(61, 21);
            this.dtbxMinX.TabIndex = 0;
            this.dtbxMinX.Text = "65";
            this.dtbxMinX.Value = 65D;
            // 
            // dtbxMaxX
            // 
            this.dtbxMaxX.Location = new System.Drawing.Point(171, 112);
            this.dtbxMaxX.Name = "dtbxMaxX";
            this.dtbxMaxX.Size = new System.Drawing.Size(61, 21);
            this.dtbxMaxX.TabIndex = 0;
            this.dtbxMaxX.Text = "145";
            this.dtbxMaxX.Value = 145D;
            // 
            // GPCPreProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 404);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.txtOutDir);
            this.Controls.Add(this.txtInDir);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "GPCPreProcess";
            this.Text = "ISCCP数据预处理";
            this.panel1.ResumeLayout(false);
            this.数据集选择.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnAddRegion;
        private DoubleTextBox dtbxMaxY;
        private DoubleTextBox dtbxMinY;
        private DoubleTextBox dtbxMaxX;
        private DoubleTextBox dtbxMinX;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox 数据集选择;
        private System.Windows.Forms.TreeView treeviewdataset;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.TextBox txtOutDir;
        private System.Windows.Forms.TextBox txtInDir;
        private System.Windows.Forms.ComboBox cbxRegionlist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancelNewReg;
        private System.Windows.Forms.Button btnSaveNewRegion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtRegionName;

    }
}

