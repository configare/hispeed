using CodeCell.Bricks.ModelFabric;
namespace CodeCell.AgileMap.ToolBox
{
    partial class frmModelEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModelEditor));
            this.panelLeft = new System.Windows.Forms.Panel();
            this.ucActionBox1 = new UCActionBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelRight = new System.Windows.Forms.Panel();
            this.ucModelEditorView1 = new UCModelEditorView();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.ucActionBox1);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(282, 398);
            this.panelLeft.TabIndex = 2;
            // 
            // ucActionBox1
            // 
            this.ucActionBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucActionBox1.Location = new System.Drawing.Point(0, 0);
            this.ucActionBox1.Name = "ucActionBox1";
            this.ucActionBox1.Size = new System.Drawing.Size(282, 398);
            this.ucActionBox1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(282, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 398);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.ucModelEditorView1);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(285, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(394, 398);
            this.panelRight.TabIndex = 4;
            // 
            // ucModelEditorView1
            // 
            this.ucModelEditorView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucModelEditorView1.Location = new System.Drawing.Point(0, 0);
            this.ucModelEditorView1.Name = "ucModelEditorView1";
            this.ucModelEditorView1.Size = new System.Drawing.Size(394, 398);
            this.ucModelEditorView1.TabIndex = 0;
            // 
            // frmModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(679, 398);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelLeft);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmModelEditor";
            this.Text = "AgileMap ToolBox 1.1";
            this.panelLeft.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private UCActionBox ucActionBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelRight;
        private UCModelEditorView ucModelEditorView1;
    }
}