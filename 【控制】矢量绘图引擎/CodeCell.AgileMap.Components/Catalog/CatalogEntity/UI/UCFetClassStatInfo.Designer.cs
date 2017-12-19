namespace CodeCell.AgileMap.Components
{
    partial class UCFetClassStatInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtShapeType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFeatureCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxY = new System.Windows.Forms.TextBox();
            this.txtMinY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMaxX = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMinX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "几何形状:";
            // 
            // txtShapeType
            // 
            this.txtShapeType.Location = new System.Drawing.Point(79, 9);
            this.txtShapeType.Name = "txtShapeType";
            this.txtShapeType.ReadOnly = true;
            this.txtShapeType.Size = new System.Drawing.Size(251, 21);
            this.txtShapeType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "要素个数:";
            // 
            // txtFeatureCount
            // 
            this.txtFeatureCount.Location = new System.Drawing.Point(78, 43);
            this.txtFeatureCount.Name = "txtFeatureCount";
            this.txtFeatureCount.ReadOnly = true;
            this.txtFeatureCount.Size = new System.Drawing.Size(251, 21);
            this.txtFeatureCount.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "最小外包矩形:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(131, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "(北,最大纬度)";
            // 
            // txtMaxY
            // 
            this.txtMaxY.Location = new System.Drawing.Point(133, 107);
     
            this.txtMaxY.Name = "txtMaxY";
            this.txtMaxY.ReadOnly = true;
            this.txtMaxY.Size = new System.Drawing.Size(83, 21);
            this.txtMaxY.TabIndex = 6;
            // 
            // txtMinY
            // 
            this.txtMinY.Location = new System.Drawing.Point(133, 195);
        
            this.txtMinY.Name = "txtMinY";
            this.txtMinY.ReadOnly = true;
            this.txtMinY.Size = new System.Drawing.Size(83, 21);
            this.txtMinY.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(133, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "(南,最小纬度)";
            // 
            // txtMaxX
            // 
            this.txtMaxX.Location = new System.Drawing.Point(201, 152);
         
            this.txtMaxX.Name = "txtMaxX";
            this.txtMaxX.ReadOnly = true;
            this.txtMaxX.Size = new System.Drawing.Size(83, 21);
            this.txtMaxX.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(199, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "(东,最大经度)";
            // 
            // txtMinX
            // 
            this.txtMinX.Location = new System.Drawing.Point(66, 152);
          
            this.txtMinX.Name = "txtMinX";
            this.txtMinX.ReadOnly = true;
            this.txtMinX.Size = new System.Drawing.Size(83, 21);
            this.txtMinX.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(63, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "(西,最小经度)";
            // 
            // UCFetClassStatInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtMinX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtMaxX);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMinY);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMaxY);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFeatureCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtShapeType);
            this.Controls.Add(this.label1);
            this.Name = "UCFetClassStatInfo";
            this.Size = new System.Drawing.Size(349, 236);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtShapeType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFeatureCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMaxY;
        private System.Windows.Forms.TextBox txtMinY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxX;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMinX;
        private System.Windows.Forms.Label label7;
    }
}
