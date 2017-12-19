namespace GeoDo.RSS.UI.AddIn.Windows
{
    partial class UCLayerManagerPanel
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnDisplayProperty = new System.Windows.Forms.ToolStripButton();
            this.btnLockDrag = new System.Windows.Forms.ToolStripButton();
            this.btnLayerUp = new System.Windows.Forms.ToolStripButton();
            this.btnLayerDown = new System.Windows.Forms.ToolStripButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.ucLayerManager = new GeoDo.RSS.UI.AddIn.Windows.UCLayerManager();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.toolStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.btnAdd,
            this.btnDelete,
            this.btnDisplayProperty,
            this.btnLockDrag,
            this.btnLayerUp,
            this.btnLayerDown});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(258, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.Refresh;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "刷新";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.open_file;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(23, 22);
            this.btnAdd.Text = "添加图层";
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelete.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.Delete;
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(23, 22);
            this.btnDelete.Text = "删除图层";
            // 
            // btnDisplayProperty
            // 
            this.btnDisplayProperty.Checked = true;
            this.btnDisplayProperty.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnDisplayProperty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDisplayProperty.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.property_blue;
            this.btnDisplayProperty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDisplayProperty.Name = "btnDisplayProperty";
            this.btnDisplayProperty.Size = new System.Drawing.Size(23, 22);
            this.btnDisplayProperty.Text = "显示属性窗口";
            this.btnDisplayProperty.ToolTipText = "是否显示属性窗口";
            // 
            // btnLockDrag
            // 
            this.btnLockDrag.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLockDrag.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources._lock;
            this.btnLockDrag.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLockDrag.Name = "btnLockDrag";
            this.btnLockDrag.Size = new System.Drawing.Size(23, 22);
            this.btnLockDrag.Text = "允许调整顺序";
            this.btnLockDrag.ToolTipText = "是否允许通过拖拽调整顺序";
            // 
            // btnLayerUp
            // 
            this.btnLayerUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLayerUp.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.arrow_up;
            this.btnLayerUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLayerUp.Name = "btnLayerUp";
            this.btnLayerUp.Size = new System.Drawing.Size(23, 22);
            this.btnLayerUp.Text = "图层上移一层";
            // 
            // btnLayerDown
            // 
            this.btnLayerDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLayerDown.Image = global::GeoDo.RSS.UI.AddIn.Windows.Properties.Resources.arrow_down;
            this.btnLayerDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLayerDown.Name = "btnLayerDown";
            this.btnLayerDown.Size = new System.Drawing.Size(23, 22);
            this.btnLayerDown.Text = "图层下移一层";
            // 
            // panelTop
            // 
            this.panelTop.AutoScroll = true;
            this.panelTop.Controls.Add(this.ucLayerManager);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 25);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(258, 252);
            this.panelTop.TabIndex = 12;
            // 
            // ucLayerManager
            // 
            this.ucLayerManager.AllowDrag = false;
            this.ucLayerManager.CurrentLayerItem = null;
            this.ucLayerManager.Location = new System.Drawing.Point(0, 0);
            this.ucLayerManager.Name = "ucLayerManager";
            this.ucLayerManager.Size = new System.Drawing.Size(241, 262);
            this.ucLayerManager.TabIndex = 8;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(258, 196);
            this.propertyGrid.TabIndex = 13;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.propertyGrid);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 280);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(258, 196);
            this.panelBottom.TabIndex = 15;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 277);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(258, 3);
            this.splitter1.TabIndex = 16;
            this.splitter1.TabStop = false;
            // 
            // UCLayerManagerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCLayerManagerPanel";
            this.Size = new System.Drawing.Size(258, 476);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private UCLayerManager ucLayerManager;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnDisplayProperty;
        private System.Windows.Forms.ToolStripButton btnLockDrag;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton btnLayerUp;
        private System.Windows.Forms.ToolStripButton btnLayerDown;

    }
}
