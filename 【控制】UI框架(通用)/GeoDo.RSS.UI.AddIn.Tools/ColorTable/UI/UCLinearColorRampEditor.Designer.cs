namespace GeoDo.RSS.UI.AddIn.Tools
{
    partial class UCLinearColorRampEditor
    {
        private System.ComponentModel.IContainer components = null;

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
            this.txtColor = new System.Windows.Forms.Label();
            this.btnInsertColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ucColorPanel1 = new GeoDo.RSS.UI.AddIn.Tools.UCColorPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txtColor
            // 
            this.txtColor.AutoSize = true;
            this.txtColor.BackColor = System.Drawing.Color.Gray;
            this.txtColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColor.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtColor.Location = new System.Drawing.Point(5, 5);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(91, 22);
            this.txtColor.TabIndex = 1;
            this.txtColor.Text = "        ";
            this.txtColor.Click += new System.EventHandler(this.txtColor_Click);
            // 
            // btnInsertColor
            // 
            this.btnInsertColor.Location = new System.Drawing.Point(103, 5);
            this.btnInsertColor.Name = "btnInsertColor";
            this.btnInsertColor.Size = new System.Drawing.Size(69, 23);
            this.btnInsertColor.TabIndex = 2;
            this.btnInsertColor.Text = "插入颜色";
            this.btnInsertColor.UseVisualStyleBackColor = true;
            this.btnInsertColor.Click += new System.EventHandler(this.btnInsertColor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(351, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 4;
            // 
            // ucColorPanel1
            // 
            this.ucColorPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucColorPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ucColorPanel1.IsDrawScales = true;
            this.ucColorPanel1.Location = new System.Drawing.Point(5, 34);
            this.ucColorPanel1.Name = "ucColorPanel1";
            this.ucColorPanel1.Size = new System.Drawing.Size(509, 84);
            this.ucColorPanel1.TabIndex = 0;
            // 
            // UCLinearColorRampEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInsertColor);
            this.Controls.Add(this.txtColor);
            this.Controls.Add(this.ucColorPanel1);
            this.Name = "UCLinearColorRampEditor";
            this.Size = new System.Drawing.Size(517, 145);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UCColorPanel ucColorPanel1;
        private System.Windows.Forms.Label txtColor;
        private System.Windows.Forms.Button btnInsertColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
