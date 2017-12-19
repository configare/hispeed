namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    partial class frmIceConDataSelect
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lvComponents = new System.Windows.Forms.ListBox();
            this.asc = new System.Windows.Forms.CheckBox();
            this.avg = new System.Windows.Forms.CheckBox();
            this.des = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOk);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(269, 174);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(173, 37);
            this.panel1.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOk.Location = new System.Drawing.Point(6, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.Location = new System.Drawing.Point(87, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lvComponents
            // 
            this.lvComponents.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvComponents.FormattingEnabled = true;
            this.lvComponents.ItemHeight = 17;
            this.lvComponents.Location = new System.Drawing.Point(10, 14);
            this.lvComponents.Name = "lvComponents";
            this.lvComponents.Size = new System.Drawing.Size(432, 157);
            this.lvComponents.TabIndex = 4;
            // 
            // asc
            // 
            this.asc.AutoSize = true;
            this.asc.Checked = true;
            this.asc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.asc.Location = new System.Drawing.Point(275, 28);
            this.asc.Name = "asc";
            this.asc.Size = new System.Drawing.Size(78, 16);
            this.asc.TabIndex = 5;
            this.asc.Text = "checkBox1";
            this.asc.UseVisualStyleBackColor = true;
            this.asc.Visible = false;
            // 
            // avg
            // 
            this.avg.AutoSize = true;
            this.avg.Checked = true;
            this.avg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.avg.Location = new System.Drawing.Point(275, 71);
            this.avg.Name = "avg";
            this.avg.Size = new System.Drawing.Size(78, 16);
            this.avg.TabIndex = 6;
            this.avg.Text = "checkBox2";
            this.avg.UseVisualStyleBackColor = true;
            this.avg.Visible = false;
            // 
            // des
            // 
            this.des.AutoSize = true;
            this.des.Checked = true;
            this.des.CheckState = System.Windows.Forms.CheckState.Checked;
            this.des.Location = new System.Drawing.Point(275, 113);
            this.des.Name = "des";
            this.des.Size = new System.Drawing.Size(78, 16);
            this.des.TabIndex = 7;
            this.des.Text = "checkBox3";
            this.des.UseVisualStyleBackColor = true;
            this.des.Visible = false;
            // 
            // frmIceConDataSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 215);
            this.Controls.Add(this.des);
            this.Controls.Add(this.avg);
            this.Controls.Add(this.asc);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lvComponents);
            this.Name = "frmIceConDataSelect";
            this.Text = "选择南/北极数据...";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lvComponents;
        private System.Windows.Forms.CheckBox asc;
        private System.Windows.Forms.CheckBox avg;
        private System.Windows.Forms.CheckBox des;
    }
}