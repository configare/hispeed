namespace CodeCell.Bricks.UIs
{
    partial class UCLayersControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCLayersControl));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btmRename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "FolderClose.gif");
            this.imageList1.Images.SetKeyName(1, "TriangleDown.png");
            this.imageList1.Images.SetKeyName(2, "TriangleRight.png");
            this.imageList1.Images.SetKeyName(3, "EyeOpen.png");
            this.imageList1.Images.SetKeyName(4, "EyeClose.png");
            this.imageList1.Images.SetKeyName(5, "UnknowLayer.png");
            this.imageList1.Images.SetKeyName(6, "OrbitData.png");
            this.imageList1.Images.SetKeyName(7, "VectorData.png");
            this.imageList1.Images.SetKeyName(8, "DigitalNumber.png");
            this.imageList1.Images.SetKeyName(9, "RoutineObser.png");
            this.imageList1.Images.SetKeyName(10, "AllColorChannelsIcon.png");
            this.imageList1.Images.SetKeyName(11, "Edit.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btmRename});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip1.Size = new System.Drawing.Size(107, 26);
            // 
            // btmRename
            // 
            this.btmRename.Name = "btmRename";
            this.btmRename.Size = new System.Drawing.Size(106, 22);
            this.btmRename.Text = "重命名";
            this.btmRename.Click += new System.EventHandler(this.btmRename_Click);
            // 
            // UCLayerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UCLayerManager";
            this.Size = new System.Drawing.Size(250, 365);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btmRename;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
