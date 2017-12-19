namespace GeoDo.FileProject.UI
{
    partial class frmBandSelectTemplete
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
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("水情方案1");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("火情方案1");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("积雪方案1");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("沙尘方案1");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("大雾方案1");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("公共方案2");
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.FullRowSelect = true;
            this.treeView1.Location = new System.Drawing.Point(2, 12);
            this.treeView1.Name = "treeView1";
            treeNode19.Name = "节点2";
            treeNode19.Text = "水情方案1";
            treeNode20.Name = "节点3";
            treeNode20.Text = "火情方案1";
            treeNode21.Name = "节点4";
            treeNode21.Text = "积雪方案1";
            treeNode22.Name = "节点5";
            treeNode22.Text = "沙尘方案1";
            treeNode23.Name = "节点12";
            treeNode23.Text = "大雾方案1";
            treeNode24.Name = "节点0";
            treeNode24.Text = "公共方案2";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24});
            this.treeView1.Size = new System.Drawing.Size(287, 145);
            this.treeView1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(110, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(214, 169);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // frmBandSelectTemplete
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(292, 204);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeView1);
            this.Name = "frmBandSelectTemplete";
            this.Text = "通道挑选方案";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}