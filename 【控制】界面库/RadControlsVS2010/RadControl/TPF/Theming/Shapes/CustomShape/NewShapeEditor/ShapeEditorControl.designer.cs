namespace Telerik.WinControls
{
    partial class RadShapeEditorControl
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
            this.pointMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topLeftCornerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topRightCorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomLeftCrnerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomRightCornerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointMenuItemLocked = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCurve = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lineMenuItemConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuGeneral = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.actualPixelSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitToScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fitBoundsToScreenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.extendBoundsToFitShapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuPoint.SuspendLayout();
            this.contextMenuCurve.SuspendLayout();
            this.contextMenuGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuPoint
            // 
            this.contextMenuPoint.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pointMenuItemDelete,
            this.lockToolStripMenuItem,
            this.pointMenuItemLocked});
            this.contextMenuPoint.Name = "contextMenuPoint";
            this.contextMenuPoint.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuPoint.Size = new System.Drawing.Size(127, 70);
            // 
            // pointMenuItemDelete
            // 
            this.pointMenuItemDelete.Name = "pointMenuItemDelete";
            this.pointMenuItemDelete.Size = new System.Drawing.Size(126, 22);
            this.pointMenuItemDelete.Text = "Delete";
            this.pointMenuItemDelete.Click += new System.EventHandler(this.Point_Delete);
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topLeftCornerToolStripMenuItem,
            this.topRightCorToolStripMenuItem,
            this.bottomLeftCrnerToolStripMenuItem,
            this.bottomRightCornerToolStripMenuItem});
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            this.lockToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.lockToolStripMenuItem.Text = "Move To";
            // 
            // topLeftCornerToolStripMenuItem
            // 
            this.topLeftCornerToolStripMenuItem.Name = "topLeftCornerToolStripMenuItem";
            this.topLeftCornerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.topLeftCornerToolStripMenuItem.Text = "Top, Left corner";
            // 
            // topRightCorToolStripMenuItem
            // 
            this.topRightCorToolStripMenuItem.Name = "topRightCorToolStripMenuItem";
            this.topRightCorToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.topRightCorToolStripMenuItem.Text = "Top, Right corner";
            // 
            // bottomLeftCrnerToolStripMenuItem
            // 
            this.bottomLeftCrnerToolStripMenuItem.Name = "bottomLeftCrnerToolStripMenuItem";
            this.bottomLeftCrnerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.bottomLeftCrnerToolStripMenuItem.Text = "Bottom, Left corner";
            // 
            // bottomRightCornerToolStripMenuItem
            // 
            this.bottomRightCornerToolStripMenuItem.Name = "bottomRightCornerToolStripMenuItem";
            this.bottomRightCornerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.bottomRightCornerToolStripMenuItem.Text = "Bottom, Right corner";
            // 
            // pointMenuItemLocked
            // 
            this.pointMenuItemLocked.Name = "pointMenuItemLocked";
            this.pointMenuItemLocked.Size = new System.Drawing.Size(126, 22);
            this.pointMenuItemLocked.Text = "Locked";
            this.pointMenuItemLocked.Click += new System.EventHandler(this.Point_Lock);
            // 
            // contextMenuCurve
            // 
            this.contextMenuCurve.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineMenuItemConvert,
            this.insertPointToolStripMenuItem});
            this.contextMenuCurve.Name = "contextMenuLine";
            this.contextMenuCurve.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.contextMenuCurve.Size = new System.Drawing.Size(142, 48);
            // 
            // lineMenuItemConvert
            // 
            this.lineMenuItemConvert.Name = "lineMenuItemConvert";
            this.lineMenuItemConvert.Size = new System.Drawing.Size(141, 22);
            this.lineMenuItemConvert.Text = "Convert";
            this.lineMenuItemConvert.Click += new System.EventHandler(this.Curve_Convert);
            // 
            // insertPointToolStripMenuItem
            // 
            this.insertPointToolStripMenuItem.Name = "insertPointToolStripMenuItem";
            this.insertPointToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.insertPointToolStripMenuItem.Text = "Insert Point";
            this.insertPointToolStripMenuItem.Click += new System.EventHandler(this.Point_Insert);
            // 
            // contextMenuGeneral
            // 
            this.contextMenuGeneral.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.toolStripSeparator1,
            this.actualPixelSizeToolStripMenuItem,
            this.fitToScreenToolStripMenuItem,
            this.fitBoundsToScreenMenuItem,
            this.toolStripSeparator2,
            this.extendBoundsToFitShapeToolStripMenuItem});
            this.contextMenuGeneral.Name = "contextMenuGeneral";
            this.contextMenuGeneral.Size = new System.Drawing.Size(219, 170);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(215, 6);
            // 
            // actualPixelSizeToolStripMenuItem
            // 
            this.actualPixelSizeToolStripMenuItem.Name = "actualPixelSizeToolStripMenuItem";
            this.actualPixelSizeToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.actualPixelSizeToolStripMenuItem.Text = "Actual Pixel Size";
            this.actualPixelSizeToolStripMenuItem.Click += new System.EventHandler(this.actualPixelSizeToolStripMenuItem_Click);
            // 
            // fitToScreenToolStripMenuItem
            // 
            this.fitToScreenToolStripMenuItem.Name = "fitToScreenToolStripMenuItem";
            this.fitToScreenToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.fitToScreenToolStripMenuItem.Text = "Fit Shape to Window";
            this.fitToScreenToolStripMenuItem.Click += new System.EventHandler(this.fitToScreenToolStripMenuItem_Click);
            // 
            // fitBoundsToScreenMenuItem
            // 
            this.fitBoundsToScreenMenuItem.Name = "fitBoundsToScreenMenuItem";
            this.fitBoundsToScreenMenuItem.Size = new System.Drawing.Size(218, 22);
            this.fitBoundsToScreenMenuItem.Text = "Fit Bounds to Window";
            this.fitBoundsToScreenMenuItem.Click += new System.EventHandler(this.fitBoundsToScreenToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(215, 6);
            // 
            // extendBoundsToFitShapeToolStripMenuItem
            // 
            this.extendBoundsToFitShapeToolStripMenuItem.Name = "extendBoundsToFitShapeToolStripMenuItem";
            this.extendBoundsToFitShapeToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.extendBoundsToFitShapeToolStripMenuItem.Text = "Extend Bounds to Fit Shape";
            this.extendBoundsToFitShapeToolStripMenuItem.Click += new System.EventHandler(this.extendBoundsToFitShapeToolStripMenuItem_Click);
            // 
            // ShapeEditorControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(60, 60);
            this.Name = "ShapeEditorControl";
            this.Size = new System.Drawing.Size(458, 326);
            this.Load += new System.EventHandler(this.ShapeEditorControl_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            this.Resize += new System.EventHandler(this.OnResize);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.contextMenuPoint.ResumeLayout(false);
            this.contextMenuCurve.ResumeLayout(false);
            this.contextMenuGeneral.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuPoint;
        private System.Windows.Forms.ToolStripMenuItem pointMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem lockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topLeftCornerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointMenuItemLocked;
        private System.Windows.Forms.ToolStripMenuItem topRightCorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomLeftCrnerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomRightCornerToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuCurve;
        private System.Windows.Forms.ToolStripMenuItem lineMenuItemConvert;
        private System.Windows.Forms.ToolStripMenuItem insertPointToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuGeneral;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem actualPixelSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitToScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitBoundsToScreenMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem extendBoundsToFitShapeToolStripMenuItem;
    }
}
