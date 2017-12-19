namespace TestDst
{
    partial class VGTFileSelector
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
            this.ucFilesSelector1 = new GeoDo.RSS.MIF.Prds.VGT.UCFilesSelector();
            this.SuspendLayout();
            // 
            // ucFilesSelector1
            // 
            this.ucFilesSelector1.Location = new System.Drawing.Point(3, 3);
            this.ucFilesSelector1.Name = "ucFilesSelector1";
            this.ucFilesSelector1.Size = new System.Drawing.Size(230, 274);
            this.ucFilesSelector1.TabIndex = 0;
            // 
            // VGTFileSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 279);
            this.Controls.Add(this.ucFilesSelector1);
            this.Name = "VGTFileSelector";
            this.Text = "VGTFileSelector";
            this.ResumeLayout(false);

        }

        #endregion

        private GeoDo.RSS.MIF.Prds.VGT.UCFilesSelector ucFilesSelector1;
    }
}