namespace GeoDo.FileProject
{
    partial class frmSelectBands
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("反射 1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("反射 2");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("辐射 3");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("辐射 4");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("辐射 5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("反射 6");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("反射 7");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("反射 8");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("反射 9");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("反射 10");
            this.TvBandList = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // TvBandList
            // 
            this.TvBandList.CheckBoxes = true;
            this.TvBandList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TvBandList.FullRowSelect = true;
            this.TvBandList.Location = new System.Drawing.Point(3, 17);
            this.TvBandList.Name = "TvBandList";
            treeNode1.Name = "节点1";
            treeNode1.Text = "反射 1";
            treeNode2.Name = "节点2";
            treeNode2.Text = "反射 2";
            treeNode3.Name = "节点3";
            treeNode3.Text = "辐射 3";
            treeNode4.Name = "节点4";
            treeNode4.Text = "辐射 4";
            treeNode5.Name = "节点5";
            treeNode5.Text = "辐射 5";
            treeNode6.Name = "节点12";
            treeNode6.Text = "反射 6";
            treeNode7.Name = "节点11";
            treeNode7.Text = "反射 7";
            treeNode8.Name = "节点12";
            treeNode8.Text = "反射 8";
            treeNode9.Name = "节点13";
            treeNode9.Text = "反射 9";
            treeNode10.Name = "节点14";
            treeNode10.Text = "反射 10";
            this.TvBandList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10});
            this.TvBandList.Size = new System.Drawing.Size(206, 300);
            this.TvBandList.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(184, 379);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(279, 379);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(62, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(289, 21);
            this.textBox1.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "方案名称";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(2, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 320);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "轨道数据类型";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TvBandList);
            this.groupBox2.Location = new System.Drawing.Point(142, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(212, 320);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "通道挑选";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Items.AddRange(new object[] {
            "FY-3A/B VIRR",
            "FY-3A/B MERSI",
            "NOAA  AVHRR",
            "EOSA/EOST MODIS"});
            this.listBox1.Location = new System.Drawing.Point(3, 17);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(131, 300);
            this.listBox1.TabIndex = 3;
            // 
            // frmSelectBands
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(366, 417);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "frmSelectBands";
            this.Text = "挑选通道";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView TvBandList;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;

    }
}