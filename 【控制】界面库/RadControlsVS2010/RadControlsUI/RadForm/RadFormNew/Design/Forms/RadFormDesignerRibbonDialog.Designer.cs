namespace Telerik.WinControls.UI
{
    partial class RadFormDesignerRibbonDialog
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
            this.radLblQuestion = new Telerik.WinControls.UI.RadLabel();
            this.radLblInfo = new Telerik.WinControls.UI.RadLabel();
            this.radBtnYes = new Telerik.WinControls.UI.RadButton();
            this.radBtnNo = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLblQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLblInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnYes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLblQuestion
            // 
            this.radLblQuestion.AutoSize = false;
            this.radLblQuestion.Location = new System.Drawing.Point(12, 60);
            this.radLblQuestion.Name = "radLblQuestion";
            this.radLblQuestion.Size = new System.Drawing.Size(402, 68);
            this.radLblQuestion.TabIndex = 0;
            this.radLblQuestion.Text = "If you choose to activate the RadRibbonFormBehavior it will be automatically deac" +
                "tivated when you remove the RadRibbonBar from the Form.\r\n\r\nWould you like to use" +
                " the RadRibbonForm behavior?";
            // 
            // radLblInfo
            // 
            this.radLblInfo.AutoSize = false;
            this.radLblInfo.Location = new System.Drawing.Point(12, 12);
            this.radLblInfo.Name = "radLblInfo";
            this.radLblInfo.Size = new System.Drawing.Size(402, 48);
            this.radLblInfo.TabIndex = 0;
            this.radLblInfo.Text = "When using the RadRibbonBar control it is recommended to either use the RadRibbon" +
                "Form or activate the RadRibbonForm behavior on this form.";
            // 
            // radBtnYes
            // 
            this.radBtnYes.Location = new System.Drawing.Point(120, 134);
            this.radBtnYes.Name = "radBtnYes";
            this.radBtnYes.Size = new System.Drawing.Size(75, 23);
            this.radBtnYes.TabIndex = 1;
            this.radBtnYes.Text = "Yes";
            this.radBtnYes.Click += new System.EventHandler(this.radBtnYes_Click);
            // 
            // radBtnNo
            // 
            this.radBtnNo.Location = new System.Drawing.Point(231, 134);
            this.radBtnNo.Name = "radBtnNo";
            this.radBtnNo.Size = new System.Drawing.Size(75, 23);
            this.radBtnNo.TabIndex = 1;
            this.radBtnNo.Text = "No";
            this.radBtnNo.Click += new System.EventHandler(this.radBtnNo_Click);
            // 
            // RadFormDesignerRibbonDialog
            // 
            this.ClientSize = new System.Drawing.Size(426, 184);
            this.Controls.Add(this.radBtnNo);
            this.Controls.Add(this.radBtnYes);
            this.Controls.Add(this.radLblInfo);
            this.Controls.Add(this.radLblQuestion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "RadFormDesignerRibbonDialog";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.RootElement.MinSize = new System.Drawing.Size(150, 36);
            this.Text = "RadForm Designer";
            ((System.ComponentModel.ISupportInitialize)(this.radLblQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLblInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnYes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radBtnNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RadLabel radLblQuestion;
        private RadLabel radLblInfo;
        private RadButton radBtnYes;
        private RadButton radBtnNo;
    }
}