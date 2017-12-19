using CodeCell.AgileMap.Components;
namespace CodeCell.AgileMap.Express
{
    partial class frmAgileMapDesktop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAgileMapDesktop));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.mapControl1 = new AgileMapControl();
            this.ucQueryResultContainer1 = new UCQueryResultContainer();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 385);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(710, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(466, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 385);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // mapControl1
            // 
            this.mapControl1.BackColor = System.Drawing.Color.White;
            this.mapControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl1.ExtentPrj = ((System.Drawing.RectangleF)(resources.GetObject("mapControl1.ExtentPrj")));
            this.mapControl1.Location = new System.Drawing.Point(0, 0);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.OnMapScaleChanged = null;
            this.mapControl1.OnViewExtentChanged = null;
            this.mapControl1.ScaleDenominator = 0;
            this.mapControl1.Size = new System.Drawing.Size(466, 385);
            this.mapControl1.SpatialReference = null;
            this.mapControl1.TabIndex = 0;
            // 
            // ucQueryResultContainer1
            // 
            this.ucQueryResultContainer1.AutoScroll = true;
            this.ucQueryResultContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ucQueryResultContainer1.Dock = System.Windows.Forms.DockStyle.Right;
            this.ucQueryResultContainer1.Location = new System.Drawing.Point(469, 0);
            this.ucQueryResultContainer1.Name = "ucQueryResultContainer1";
            this.ucQueryResultContainer1.Size = new System.Drawing.Size(241, 385);
            this.ucQueryResultContainer1.TabIndex = 4;
            this.ucQueryResultContainer1.Visible = false;
            // 
            // frmAgileMapDesktop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 407);
            this.Controls.Add(this.mapControl1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.ucQueryResultContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAgileMapDesktop";
            this.Text = "AgileMap Express 1.1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AgileMapControl mapControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private UCQueryResultContainer ucQueryResultContainer1;
        private System.Windows.Forms.Splitter splitter1;
    }
}