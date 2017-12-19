namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    partial class frmPointInterpolation
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
            this.label_field = new System.Windows.Forms.Label();
            this.FieldName = new System.Windows.Forms.ComboBox();
            this.label_resX = new System.Windows.Forms.Label();
            this.ResolutionX = new System.Windows.Forms.TextBox();
            this.label_resY = new System.Windows.Forms.Label();
            this.resolutionY = new System.Windows.Forms.TextBox();
            this.label_output = new System.Windows.Forms.Label();
            this.OutPath = new System.Windows.Forms.TextBox();
            this.Browse = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_field
            // 
            this.label_field.AutoSize = true;
            this.label_field.Location = new System.Drawing.Point(14, 25);
            this.label_field.Name = "label_field";
            this.label_field.Size = new System.Drawing.Size(65, 12);
            this.label_field.TabIndex = 0;
            this.label_field.Text = "插值字段：";
            // 
            // FieldName
            // 
            this.FieldName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FieldName.FormattingEnabled = true;
            this.FieldName.Location = new System.Drawing.Point(89, 20);
            this.FieldName.Name = "FieldName";
            this.FieldName.Size = new System.Drawing.Size(269, 20);
            this.FieldName.TabIndex = 1;
            // 
            // label_resX
            // 
            this.label_resX.AutoSize = true;
            this.label_resX.Location = new System.Drawing.Point(14, 72);
            this.label_resX.Name = "label_resX";
            this.label_resX.Size = new System.Drawing.Size(59, 12);
            this.label_resX.TabIndex = 2;
            this.label_resX.Text = "X分辨率：";
            // 
            // ResolutionX
            // 
            this.ResolutionX.Location = new System.Drawing.Point(89, 67);
            this.ResolutionX.Name = "ResolutionX";
            this.ResolutionX.Size = new System.Drawing.Size(98, 21);
            this.ResolutionX.TabIndex = 3;
            // 
            // label_resY
            // 
            this.label_resY.AutoSize = true;
            this.label_resY.Location = new System.Drawing.Point(198, 70);
            this.label_resY.Name = "label_resY";
            this.label_resY.Size = new System.Drawing.Size(59, 12);
            this.label_resY.TabIndex = 4;
            this.label_resY.Text = "Y分辨率：";
            // 
            // resolutionY
            // 
            this.resolutionY.Location = new System.Drawing.Point(260, 67);
            this.resolutionY.Name = "resolutionY";
            this.resolutionY.Size = new System.Drawing.Size(98, 21);
            this.resolutionY.TabIndex = 5;
            // 
            // label_output
            // 
            this.label_output.AutoSize = true;
            this.label_output.Location = new System.Drawing.Point(14, 120);
            this.label_output.Name = "label_output";
            this.label_output.Size = new System.Drawing.Size(65, 12);
            this.label_output.TabIndex = 6;
            this.label_output.Text = "输出路径：";
            // 
            // OutPath
            // 
            this.OutPath.Location = new System.Drawing.Point(89, 114);
            this.OutPath.Name = "OutPath";
            this.OutPath.Size = new System.Drawing.Size(212, 21);
            this.OutPath.TabIndex = 7;
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(307, 112);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(54, 23);
            this.Browse.TabIndex = 8;
            this.Browse.Text = "浏览";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(70, 159);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(241, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmPointInterpolation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 183);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.OutPath);
            this.Controls.Add(this.label_output);
            this.Controls.Add(this.resolutionY);
            this.Controls.Add(this.label_resY);
            this.Controls.Add(this.ResolutionX);
            this.Controls.Add(this.label_resX);
            this.Controls.Add(this.FieldName);
            this.Controls.Add(this.label_field);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPointInterpolation";
            this.Text = "空间插值";
            this.Load += new System.EventHandler(this.frmPointInterpolation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_field;
        private System.Windows.Forms.ComboBox FieldName;
        private System.Windows.Forms.Label label_resX;
        private System.Windows.Forms.TextBox ResolutionX;
        private System.Windows.Forms.Label label_resY;
        private System.Windows.Forms.TextBox resolutionY;
        private System.Windows.Forms.Label label_output;
        private System.Windows.Forms.TextBox OutPath;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}