namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class UCLayerManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCLayerManager));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btmRename = new System.Windows.Forms.ToolStripMenuItem();
            this.TspMenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "FolderClose.gif");
            this.imageList.Images.SetKeyName(1, "TriangleDown.png");
            this.imageList.Images.SetKeyName(2, "TriangleRight.png");
            this.imageList.Images.SetKeyName(3, "EyeOpen.png");
            this.imageList.Images.SetKeyName(4, "EyeClose.png");
            this.imageList.Images.SetKeyName(5, "exportBmp.png");
            this.imageList.Images.SetKeyName(6, "OrbitData.png");
            this.imageList.Images.SetKeyName(7, "VectorData.png");
            this.imageList.Images.SetKeyName(8, "Edit.png");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btmRename,
            this.TspMenuSave});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 70);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // btmRename
            // 
            this.btmRename.Name = "btmRename";
            this.btmRename.Size = new System.Drawing.Size(152, 22);
            this.btmRename.Text = "重命名";
            // 
            // TspMenuSave
            // 
            this.TspMenuSave.Name = "TspMenuSave";
            this.TspMenuSave.Size = new System.Drawing.Size(152, 22);
            this.TspMenuSave.Text = "存储显示方案";
            this.TspMenuSave.Visible = false;
            // 
            // UCLayerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UCLayerManager";
            this.Size = new System.Drawing.Size(256, 279);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem btmRename;
        private System.Windows.Forms.ToolStripMenuItem TspMenuSave;
    }
}
