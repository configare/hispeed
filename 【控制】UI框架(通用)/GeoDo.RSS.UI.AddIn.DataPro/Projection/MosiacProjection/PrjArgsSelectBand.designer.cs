namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class PrjArgsSelectBand
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
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("反射 1");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("反射 2");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("辐射 3");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("辐射 4");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("辐射 5");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("反射 6");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("反射 7");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("反射 8");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("反射 9");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("反射 10");
            this.tvBandList = new System.Windows.Forms.TreeView();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvBandList
            // 
            this.tvBandList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvBandList.CheckBoxes = true;
            this.tvBandList.FullRowSelect = true;
            this.tvBandList.Location = new System.Drawing.Point(0, 32);
            this.tvBandList.Name = "tvBandList";
            treeNode21.Name = "节点1";
            treeNode21.Text = "反射 1";
            treeNode22.Name = "节点2";
            treeNode22.Text = "反射 2";
            treeNode23.Name = "节点3";
            treeNode23.Text = "辐射 3";
            treeNode24.Name = "节点4";
            treeNode24.Text = "辐射 4";
            treeNode25.Name = "节点5";
            treeNode25.Text = "辐射 5";
            treeNode26.Name = "节点12";
            treeNode26.Text = "反射 6";
            treeNode27.Name = "节点11";
            treeNode27.Text = "反射 7";
            treeNode28.Name = "节点12";
            treeNode28.Text = "反射 8";
            treeNode29.Name = "节点13";
            treeNode29.Text = "反射 9";
            treeNode30.Name = "节点14";
            treeNode30.Text = "反射 10";
            this.tvBandList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30});
            this.tvBandList.Size = new System.Drawing.Size(223, 340);
            this.tvBandList.TabIndex = 3;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(52, 4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 4;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectNone.Location = new System.Drawing.Point(134, 4);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(75, 23);
            this.btnSelectNone.TabIndex = 5;
            this.btnSelectNone.Text = "全不选";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // PrjArgsSelectBand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.tvBandList);
            this.Name = "PrjArgsSelectBand";
            this.Size = new System.Drawing.Size(223, 372);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvBandList;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
    }
}
