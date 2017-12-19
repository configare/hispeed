namespace Telerik.WinControls.OldShapeEditor
{
    public partial class RadShapeEditorControl
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
			this.contextMenuPoint = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemRemovePoint = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemConvert = new System.Windows.Forms.ToolStripMenuItem();
			this.anchorStylesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAnchorLeft = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAnchorRight = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAnchorTop = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAnchorBottom = new System.Windows.Forms.ToolStripMenuItem();
			this.snapToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLeftTopCorner = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRightTopCorner = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLeftBottomCorner = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemRightBottomCorner = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLocked = new System.Windows.Forms.ToolStripMenuItem();
			this.creToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.makeSymmetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontallyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.verticallyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuLine = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemRemoveLine = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemConvertLine = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAddPoint = new System.Windows.Forms.ToolStripMenuItem();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.contextMenuPoint.SuspendLayout();
			this.contextMenuLine.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuPoint
			// 
			this.contextMenuPoint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemRemovePoint,
            this.menuItemConvert,
            this.anchorStylesToolStripMenuItem,
            this.snapToToolStripMenuItem,
            this.menuItemLocked,
            this.creToolStripMenuItem,
            this.makeSymmetricToolStripMenuItem});
			this.contextMenuPoint.Name = "contextMenuPoint";
			this.contextMenuPoint.Size = new System.Drawing.Size(217, 158);
			// 
			// menuItemRemovePoint
			// 
			this.menuItemRemovePoint.Name = "menuItemRemovePoint";
			this.menuItemRemovePoint.Size = new System.Drawing.Size(216, 22);
			this.menuItemRemovePoint.Text = "Remove Point";
			// 
			// menuItemConvert
			// 
			this.menuItemConvert.Name = "menuItemConvert";
			this.menuItemConvert.Size = new System.Drawing.Size(216, 22);
			this.menuItemConvert.Text = "Convert to Bezier Curve";
			// 
			// anchorStylesToolStripMenuItem
			// 
			this.anchorStylesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAnchorLeft,
            this.menuItemAnchorRight,
            this.menuItemAnchorTop,
            this.menuItemAnchorBottom});
			this.anchorStylesToolStripMenuItem.Name = "anchorStylesToolStripMenuItem";
			this.anchorStylesToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.anchorStylesToolStripMenuItem.Text = "Anchor styles";
			// 
			// menuItemAnchorLeft
			// 
			this.menuItemAnchorLeft.Name = "menuItemAnchorLeft";
			this.menuItemAnchorLeft.Size = new System.Drawing.Size(119, 22);
			this.menuItemAnchorLeft.Text = "Left";
			// 
			// menuItemAnchorRight
			// 
			this.menuItemAnchorRight.Name = "menuItemAnchorRight";
			this.menuItemAnchorRight.Size = new System.Drawing.Size(119, 22);
			this.menuItemAnchorRight.Text = "Right";
			// 
			// menuItemAnchorTop
			// 
			this.menuItemAnchorTop.Name = "menuItemAnchorTop";
			this.menuItemAnchorTop.Size = new System.Drawing.Size(119, 22);
			this.menuItemAnchorTop.Text = "Top";
			// 
			// menuItemAnchorBottom
			// 
			this.menuItemAnchorBottom.Name = "menuItemAnchorBottom";
			this.menuItemAnchorBottom.Size = new System.Drawing.Size(119, 22);
			this.menuItemAnchorBottom.Text = "Bottom";
			// 
			// snapToToolStripMenuItem
			// 
			this.snapToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLeftTopCorner,
            this.menuItemRightTopCorner,
            this.menuItemLeftBottomCorner,
            this.menuItemRightBottomCorner});
			this.snapToToolStripMenuItem.Name = "snapToToolStripMenuItem";
			this.snapToToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.snapToToolStripMenuItem.Text = "Snap to";
			// 
			// menuItemLeftTopCorner
			// 
			this.menuItemLeftTopCorner.Name = "menuItemLeftTopCorner";
			this.menuItemLeftTopCorner.Size = new System.Drawing.Size(177, 22);
			this.menuItemLeftTopCorner.Text = "LeftTop Corner";
			// 
			// menuItemRightTopCorner
			// 
			this.menuItemRightTopCorner.Name = "menuItemRightTopCorner";
			this.menuItemRightTopCorner.Size = new System.Drawing.Size(177, 22);
			this.menuItemRightTopCorner.Text = "RightTop Corner";
			// 
			// menuItemLeftBottomCorner
			// 
			this.menuItemLeftBottomCorner.Name = "menuItemLeftBottomCorner";
			this.menuItemLeftBottomCorner.Size = new System.Drawing.Size(177, 22);
			this.menuItemLeftBottomCorner.Text = "LeftBottom Corner";
			// 
			// menuItemRightBottomCorner
			// 
			this.menuItemRightBottomCorner.Name = "menuItemRightBottomCorner";
			this.menuItemRightBottomCorner.Size = new System.Drawing.Size(177, 22);
			this.menuItemRightBottomCorner.Text = "RightBottomCorner";
			// 
			// menuItemLocked
			// 
			this.menuItemLocked.Name = "menuItemLocked";
			this.menuItemLocked.Size = new System.Drawing.Size(216, 22);
			this.menuItemLocked.Text = "Locked";
			// 
			// creToolStripMenuItem
			// 
			this.creToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontallyToolStripMenuItem,
            this.verticallyToolStripMenuItem});
			this.creToolStripMenuItem.Name = "creToolStripMenuItem";
			this.creToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.creToolStripMenuItem.Text = "Create Symmetric Point";
			// 
			// horizontallyToolStripMenuItem
			// 
			this.horizontallyToolStripMenuItem.Name = "horizontallyToolStripMenuItem";
			this.horizontallyToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.horizontallyToolStripMenuItem.Text = "Horizontally";
			this.horizontallyToolStripMenuItem.Click += new System.EventHandler(this.horizontallyToolStripMenuItem_Click);
			// 
			// verticallyToolStripMenuItem
			// 
			this.verticallyToolStripMenuItem.Name = "verticallyToolStripMenuItem";
			this.verticallyToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.verticallyToolStripMenuItem.Text = "Vertically";
			this.verticallyToolStripMenuItem.Click += new System.EventHandler(this.verticallyToolStripMenuItem_Click);
			// 
			// makeSymmetricToolStripMenuItem
			// 
			this.makeSymmetricToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.horizontallyToolStripMenuItem1,
            this.verticallyToolStripMenuItem1});
			this.makeSymmetricToolStripMenuItem.Name = "makeSymmetricToolStripMenuItem";
			this.makeSymmetricToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.makeSymmetricToolStripMenuItem.Text = "Make Point Symmetric To...";
			// 
			// horizontallyToolStripMenuItem1
			// 
			this.horizontallyToolStripMenuItem1.Name = "horizontallyToolStripMenuItem1";
			this.horizontallyToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
			this.horizontallyToolStripMenuItem1.Text = "Horizontally";
			this.horizontallyToolStripMenuItem1.Click += new System.EventHandler(this.horizontallyToolStripMenuItem1_Click);
			// 
			// verticallyToolStripMenuItem1
			// 
			this.verticallyToolStripMenuItem1.Name = "verticallyToolStripMenuItem1";
			this.verticallyToolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
			this.verticallyToolStripMenuItem1.Text = "Vertically";
			this.verticallyToolStripMenuItem1.Click += new System.EventHandler(this.verticallyToolStripMenuItem1_Click);
			// 
			// contextMenuLine
			// 
			this.contextMenuLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemRemoveLine,
            this.menuItemConvertLine,
            this.menuItemAddPoint});
			this.contextMenuLine.Name = "contextMenuLine";
			this.contextMenuLine.Size = new System.Drawing.Size(202, 70);
			// 
			// menuItemRemoveLine
			// 
			this.menuItemRemoveLine.Name = "menuItemRemoveLine";
			this.menuItemRemoveLine.Size = new System.Drawing.Size(201, 22);
			this.menuItemRemoveLine.Text = "Remove Line";
			// 
			// menuItemConvertLine
			// 
			this.menuItemConvertLine.Name = "menuItemConvertLine";
			this.menuItemConvertLine.Size = new System.Drawing.Size(201, 22);
			this.menuItemConvertLine.Text = "Convert to Bezier Curve";
			// 
			// menuItemAddPoint
			// 
			this.menuItemAddPoint.Name = "menuItemAddPoint";
			this.menuItemAddPoint.Size = new System.Drawing.Size(201, 22);
			this.menuItemAddPoint.Text = "Add Point";
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar1.Location = new System.Drawing.Point(0, 133);
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(133, 16);
			this.hScrollBar1.TabIndex = 2;
			this.hScrollBar1.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
			this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.vScrollBar1.Location = new System.Drawing.Point(133, 0);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 134);
			this.vScrollBar1.TabIndex = 3;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
			this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
			// 
			// RadShapeEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.hScrollBar1);
			this.Name = "RadShapeEditorControl";
			this.Load += new System.EventHandler(this.RadShapeEditorControl_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RadShapeEditorControl_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RadShapeEditorControl_MouseMove);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RadShapeEditorControl_KeyPress);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RadShapeEditorControl_MouseUp);
			this.contextMenuPoint.ResumeLayout(false);
			this.contextMenuLine.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuPoint;
        private System.Windows.Forms.ContextMenuStrip contextMenuLine;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemovePoint;
        private System.Windows.Forms.ToolStripMenuItem menuItemConvert;
        private System.Windows.Forms.ToolStripMenuItem anchorStylesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemAnchorLeft;
        private System.Windows.Forms.ToolStripMenuItem menuItemAnchorRight;
        private System.Windows.Forms.ToolStripMenuItem menuItemAnchorTop;
        private System.Windows.Forms.ToolStripMenuItem menuItemAnchorBottom;
        private System.Windows.Forms.ToolStripMenuItem menuItemRemoveLine;
        private System.Windows.Forms.ToolStripMenuItem menuItemAddPoint;
        private System.Windows.Forms.ToolStripMenuItem menuItemConvertLine;
        private System.Windows.Forms.ToolStripMenuItem snapToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemLeftTopCorner;
        private System.Windows.Forms.ToolStripMenuItem menuItemRightTopCorner;
        private System.Windows.Forms.ToolStripMenuItem menuItemLeftBottomCorner;
        private System.Windows.Forms.ToolStripMenuItem menuItemRightBottomCorner;
		private System.Windows.Forms.ToolStripMenuItem menuItemLocked;
		private System.Windows.Forms.ToolStripMenuItem creToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem makeSymmetricToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem horizontallyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticallyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem horizontallyToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem verticallyToolStripMenuItem1;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
    }
}
