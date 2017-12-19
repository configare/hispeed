//using GeoDo.RSS.UI.AddIn.DataPro;
namespace GeoDo.RSS.MIF.Prds.BAG
{
    partial class frmArgNDVIManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmArgNDVIManager));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtAvgMax = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtAvgMin = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMaxLat = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMinLat = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMaxLon = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.txtMinLon = new GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox();
            this.labAvgMax = new System.Windows.Forms.Label();
            this.labAvgMin = new System.Windows.Forms.Label();
            this.labMinLon = new System.Windows.Forms.Label();
            this.labMaxLon = new System.Windows.Forms.Label();
            this.labMaxLat = new System.Windows.Forms.Label();
            this.labMinLat = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.labName = new System.Windows.Forms.Label();
            this.btnAddRegion = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.labName);
            this.groupBox1.Location = new System.Drawing.Point(275, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 281);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "端元值编辑";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtAvgMax);
            this.panel2.Controls.Add(this.txtAvgMin);
            this.panel2.Controls.Add(this.txtMaxLat);
            this.panel2.Controls.Add(this.txtMinLat);
            this.panel2.Controls.Add(this.txtMaxLon);
            this.panel2.Controls.Add(this.txtMinLon);
            this.panel2.Controls.Add(this.labAvgMax);
            this.panel2.Controls.Add(this.labAvgMin);
            this.panel2.Controls.Add(this.labMinLon);
            this.panel2.Controls.Add(this.labMaxLon);
            this.panel2.Controls.Add(this.labMaxLat);
            this.panel2.Controls.Add(this.labMinLat);
            this.panel2.Location = new System.Drawing.Point(0, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(214, 222);
            this.panel2.TabIndex = 12;
            // 
            // txtAvgMax
            // 
            this.txtAvgMax.Location = new System.Drawing.Point(85, 182);
            this.txtAvgMax.Name = "txtAvgMax";
            this.txtAvgMax.Size = new System.Drawing.Size(100, 21);
            this.txtAvgMax.TabIndex = 14;
            this.txtAvgMax.Text = "0";
            this.txtAvgMax.Value = 0D;
            this.txtAvgMax.Leave += new System.EventHandler(this.txtAvgMax_Leave);
            // 
            // txtAvgMin
            // 
            this.txtAvgMin.Location = new System.Drawing.Point(85, 148);
            this.txtAvgMin.Name = "txtAvgMin";
            this.txtAvgMin.Size = new System.Drawing.Size(100, 21);
            this.txtAvgMin.TabIndex = 13;
            this.txtAvgMin.Text = "0";
            this.txtAvgMin.Value = 0D;
            this.txtAvgMin.Leave += new System.EventHandler(this.txtAvgMin_Leave);
            // 
            // txtMaxLat
            // 
            this.txtMaxLat.Location = new System.Drawing.Point(85, 114);
            this.txtMaxLat.Name = "txtMaxLat";
            this.txtMaxLat.Size = new System.Drawing.Size(100, 21);
            this.txtMaxLat.TabIndex = 12;
            this.txtMaxLat.Text = "0";
            this.txtMaxLat.Value = 0D;
            this.txtMaxLat.Leave += new System.EventHandler(this.txtMaxLat_Leave);
            // 
            // txtMinLat
            // 
            this.txtMinLat.Location = new System.Drawing.Point(85, 80);
            this.txtMinLat.Name = "txtMinLat";
            this.txtMinLat.Size = new System.Drawing.Size(100, 21);
            this.txtMinLat.TabIndex = 11;
            this.txtMinLat.Text = "0";
            this.txtMinLat.Value = 0D;
            this.txtMinLat.Leave += new System.EventHandler(this.txtMinLat_Leave);
            // 
            // txtMaxLon
            // 
            this.txtMaxLon.Location = new System.Drawing.Point(85, 46);
            this.txtMaxLon.Name = "txtMaxLon";
            this.txtMaxLon.Size = new System.Drawing.Size(100, 21);
            this.txtMaxLon.TabIndex = 10;
            this.txtMaxLon.Text = "0";
            this.txtMaxLon.Value = 0D;
            this.txtMaxLon.Leave += new System.EventHandler(this.txtMaxLon_Leave);
            // 
            // txtMinLon
            // 
            this.txtMinLon.Location = new System.Drawing.Point(85, 12);
            this.txtMinLon.Name = "txtMinLon";
            this.txtMinLon.Size = new System.Drawing.Size(100, 21);
            this.txtMinLon.TabIndex = 9;
            this.txtMinLon.Text = "0";
            this.txtMinLon.Value = 0D;
            this.txtMinLon.Leave += new System.EventHandler(this.txtMinLon_Leave);
            // 
            // labAvgMax
            // 
            this.labAvgMax.AutoSize = true;
            this.labAvgMax.Location = new System.Drawing.Point(11, 186);
            this.labAvgMax.Name = "labAvgMax";
            this.labAvgMax.Size = new System.Drawing.Size(71, 12);
            this.labAvgMax.TabIndex = 8;
            this.labAvgMax.Text = "最大端元值:";
            // 
            // labAvgMin
            // 
            this.labAvgMin.AutoSize = true;
            this.labAvgMin.Location = new System.Drawing.Point(11, 152);
            this.labAvgMin.Name = "labAvgMin";
            this.labAvgMin.Size = new System.Drawing.Size(71, 12);
            this.labAvgMin.TabIndex = 5;
            this.labAvgMin.Text = "最小端元值:";
            // 
            // labMinLon
            // 
            this.labMinLon.AutoSize = true;
            this.labMinLon.Location = new System.Drawing.Point(11, 16);
            this.labMinLon.Name = "labMinLon";
            this.labMinLon.Size = new System.Drawing.Size(59, 12);
            this.labMinLon.TabIndex = 1;
            this.labMinLon.Text = "最小经度:";
            // 
            // labMaxLon
            // 
            this.labMaxLon.AutoSize = true;
            this.labMaxLon.Location = new System.Drawing.Point(11, 50);
            this.labMaxLon.Name = "labMaxLon";
            this.labMaxLon.Size = new System.Drawing.Size(59, 12);
            this.labMaxLon.TabIndex = 2;
            this.labMaxLon.Text = "最大经度:";
            // 
            // labMaxLat
            // 
            this.labMaxLat.AutoSize = true;
            this.labMaxLat.Location = new System.Drawing.Point(11, 118);
            this.labMaxLat.Name = "labMaxLat";
            this.labMaxLat.Size = new System.Drawing.Size(59, 12);
            this.labMaxLat.TabIndex = 3;
            this.labMaxLat.Text = "最大纬度:";
            // 
            // labMinLat
            // 
            this.labMinLat.AutoSize = true;
            this.labMinLat.Location = new System.Drawing.Point(11, 84);
            this.labMinLat.Name = "labMinLat";
            this.labMinLat.Size = new System.Drawing.Size(59, 12);
            this.labMinLat.TabIndex = 4;
            this.labMinLat.Text = "最小纬度:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(65, 27);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(132, 21);
            this.txtName.TabIndex = 9;
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Location = new System.Drawing.Point(13, 31);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(35, 12);
            this.labName.TabIndex = 0;
            this.labName.Text = "名称:";
            // 
            // btnAddRegion
            // 
            this.btnAddRegion.Location = new System.Drawing.Point(222, 323);
            this.btnAddRegion.Name = "btnAddRegion";
            this.btnAddRegion.Size = new System.Drawing.Size(75, 23);
            this.btnAddRegion.TabIndex = 11;
            this.btnAddRegion.Text = "添加范围";
            this.btnAddRegion.UseVisualStyleBackColor = true;
            this.btnAddRegion.Click += new System.EventHandler(this.btnAddRegion_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(403, 323);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消编辑";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(312, 323);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "保存结果";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete,
            this.btnReload,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(495, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(69, 22);
            this.btnDelete.Text = "删除(&U)";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnReload
            // 
            this.btnReload.Image = ((System.Drawing.Image)(resources.GetObject("btnReload.Image")));
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(76, 22);
            this.btnReload.Text = "重新加载";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存(&S)";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(269, 281);
            this.panel1.TabIndex = 10;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmArgNDVIManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 356);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAddRegion);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmArgNDVIManager";
            this.ShowIcon = false;
            this.Text = "端元值设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtName;
        //private DoubleTextBox doubleTextBox4;
        //private DoubleTextBox doubleTextBox3;
        //private DoubleTextBox doubleTextBox2;
        //private DoubleTextBox doubleTextBox1;
        private System.Windows.Forms.Label labMinLat;
        private System.Windows.Forms.Label labMaxLat;
        private System.Windows.Forms.Label labMaxLon;
        private System.Windows.Forms.Label labMinLon;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton btnReload;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.Button btnAddRegion;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labAvgMax;
        private System.Windows.Forms.Label labAvgMin;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtAvgMax;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtAvgMin;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtMaxLat;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtMinLat;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtMaxLon;
        private GeoDo.RSS.UI.AddIn.DataPro.DoubleTextBox txtMinLon;
    }
}