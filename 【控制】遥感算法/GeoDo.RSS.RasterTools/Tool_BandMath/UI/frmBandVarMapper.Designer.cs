namespace GeoDo.RSS.RasterTools
{
    partial class frmBandVarMapper
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
            this.ucBandVarSetter1 = new GeoDo.RSS.RasterTools.UCBandVarSetter();
            this.SuspendLayout();
            // 
            // ucBandVarSetter1
            // 
            this.ucBandVarSetter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBandVarSetter1.Location = new System.Drawing.Point(0, 0);
            this.ucBandVarSetter1.Name = "ucBandVarSetter1";
            this.ucBandVarSetter1.Size = new System.Drawing.Size(573, 306);
            this.ucBandVarSetter1.TabIndex = 0;
            this.ucBandVarSetter1.ApplyClicked += new System.EventHandler(this.ucBandVarSetter1_ApplyClicked);
            this.ucBandVarSetter1.CancelClicked += new System.EventHandler(this.ucBandVarSetter1_CancelClicked);
            // 
            // frmBandVarMapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 306);
            this.Controls.Add(this.ucBandVarSetter1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmBandVarMapper";
            this.ShowIcon = false;
            this.Text = "波段序号变量映射...";
            this.ResumeLayout(false);

        }

        #endregion

        private UCBandVarSetter ucBandVarSetter1;
    }
}