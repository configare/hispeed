namespace GeoDo.ProjectDefine
{
    partial class SpatialReferenceSelection
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpatialReferenceSelection));
            this.btnModify = new System.Windows.Forms.Button();
            this.lblSpatialRefSel = new System.Windows.Forms.Label();
            this.lblSpatialRefDisplay = new System.Windows.Forms.Label();
            this.imlList = new System.Windows.Forms.ImageList(this.components);
            this.txtSpatialRefInfo = new System.Windows.Forms.TextBox();
            this.btnCreatGeo = new System.Windows.Forms.Button();
            this.btnCreatPrj = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmdDisplayBlank = new System.Windows.Forms.ComboBox();
            this.btnCancle = new System.Windows.Forms.Button();
            this.pctFold = new System.Windows.Forms.PictureBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tvSpatialRefNames = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.pctFold)).BeginInit();
            this.SuspendLayout();
            // 
            // btnModify
            // 
            this.btnModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnModify.Enabled = false;
            this.btnModify.Location = new System.Drawing.Point(230, 330);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(57, 23);
            this.btnModify.TabIndex = 1;
            this.btnModify.Text = "修改";
            this.btnModify.UseVisualStyleBackColor = true;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // lblSpatialRefSel
            // 
            this.lblSpatialRefSel.AutoSize = true;
            this.lblSpatialRefSel.Location = new System.Drawing.Point(8, 32);
            this.lblSpatialRefSel.Name = "lblSpatialRefSel";
            this.lblSpatialRefSel.Size = new System.Drawing.Size(89, 12);
            this.lblSpatialRefSel.TabIndex = 4;
            this.lblSpatialRefSel.Text = "选择坐标系统：";
            // 
            // lblSpatialRefDisplay
            // 
            this.lblSpatialRefDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpatialRefDisplay.AutoSize = true;
            this.lblSpatialRefDisplay.Location = new System.Drawing.Point(404, 31);
            this.lblSpatialRefDisplay.Name = "lblSpatialRefDisplay";
            this.lblSpatialRefDisplay.Size = new System.Drawing.Size(89, 12);
            this.lblSpatialRefDisplay.TabIndex = 5;
            this.lblSpatialRefDisplay.Text = "当前坐标系统：";
            this.lblSpatialRefDisplay.Visible = false;
            // 
            // imlList
            // 
            this.imlList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlList.ImageStream")));
            this.imlList.TransparentColor = System.Drawing.Color.Transparent;
            this.imlList.Images.SetKeyName(0, "folder.png");
            this.imlList.Images.SetKeyName(1, "CoordinateSystem16.png");
            this.imlList.Images.SetKeyName(2, "folder-open.png");
            // 
            // txtSpatialRefInfo
            // 
            this.txtSpatialRefInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSpatialRefInfo.Location = new System.Drawing.Point(402, 46);
            this.txtSpatialRefInfo.Multiline = true;
            this.txtSpatialRefInfo.Name = "txtSpatialRefInfo";
            this.txtSpatialRefInfo.ReadOnly = true;
            this.txtSpatialRefInfo.Size = new System.Drawing.Size(304, 278);
            this.txtSpatialRefInfo.TabIndex = 9;
            // 
            // btnCreatGeo
            // 
            this.btnCreatGeo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreatGeo.Location = new System.Drawing.Point(7, 330);
            this.btnCreatGeo.Name = "btnCreatGeo";
            this.btnCreatGeo.Size = new System.Drawing.Size(109, 23);
            this.btnCreatGeo.TabIndex = 10;
            this.btnCreatGeo.Text = "新建地理坐标系统";
            this.btnCreatGeo.UseVisualStyleBackColor = true;
            this.btnCreatGeo.Click += new System.EventHandler(this.btnCreatGeo_Click);
            // 
            // btnCreatPrj
            // 
            this.btnCreatPrj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreatPrj.Location = new System.Drawing.Point(118, 330);
            this.btnCreatPrj.Name = "btnCreatPrj";
            this.btnCreatPrj.Size = new System.Drawing.Size(109, 23);
            this.btnCreatPrj.TabIndex = 11;
            this.btnCreatPrj.Text = "新建投影坐标系统";
            this.btnCreatPrj.UseVisualStyleBackColor = true;
            this.btnCreatPrj.Click += new System.EventHandler(this.btnCreatPrj_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(589, 330);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmdDisplayBlank
            // 
            this.cmdDisplayBlank.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDisplayBlank.FormattingEnabled = true;
            this.cmdDisplayBlank.Location = new System.Drawing.Point(10, 7);
            this.cmdDisplayBlank.Name = "cmdDisplayBlank";
            this.cmdDisplayBlank.Size = new System.Drawing.Size(631, 20);
            this.cmdDisplayBlank.TabIndex = 13;
            this.cmdDisplayBlank.SelectedIndexChanged += new System.EventHandler(this.cmdDisplayBlank_SelectedIndexChanged);
            this.cmdDisplayBlank.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmdDisplayBlank_KeyUp);
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancle.Location = new System.Drawing.Point(650, 330);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(58, 23);
            this.btnCancle.TabIndex = 15;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // pctFold
            // 
            this.pctFold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pctFold.Image = global::GeoDo.ProjectDefine.Properties.Resources.arrow_down_9x10;
            this.pctFold.Location = new System.Drawing.Point(381, 46);
            this.pctFold.Name = "pctFold";
            this.pctFold.Size = new System.Drawing.Size(21, 278);
            this.pctFold.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pctFold.TabIndex = 16;
            this.pctFold.TabStop = false;
            this.pctFold.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Image = global::GeoDo.ProjectDefine.Properties.Resources.ZoomGeneric_B_16;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(649, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 21);
            this.btnSearch.TabIndex = 14;
            this.btnSearch.Text = "搜索";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tvSpatialRefNames
            // 
            this.tvSpatialRefNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvSpatialRefNames.ImageIndex = 0;
            this.tvSpatialRefNames.ImageList = this.imlList;
            this.tvSpatialRefNames.Location = new System.Drawing.Point(10, 46);
            this.tvSpatialRefNames.Name = "tvSpatialRefNames";
            this.tvSpatialRefNames.SelectedImageIndex = 0;
            this.tvSpatialRefNames.Size = new System.Drawing.Size(372, 278);
            this.tvSpatialRefNames.TabIndex = 17;
            this.tvSpatialRefNames.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSpatialRefNames_AfterSelect);
            this.tvSpatialRefNames.DoubleClick += new System.EventHandler(this.tvSpatialRefNames_DoubleClick);
            // 
            // SpatialReferenceSelection
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(716, 360);
            this.Controls.Add(this.tvSpatialRefNames);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cmdDisplayBlank);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCreatPrj);
            this.Controls.Add(this.btnCreatGeo);
            this.Controls.Add(this.txtSpatialRefInfo);
            this.Controls.Add(this.pctFold);
            this.Controls.Add(this.lblSpatialRefDisplay);
            this.Controls.Add(this.lblSpatialRefSel);
            this.Controls.Add(this.btnModify);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SpatialReferenceSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpatialReferenceSelection";
            ((System.ComponentModel.ISupportInitialize)(this.pctFold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Label lblSpatialRefSel;
        private System.Windows.Forms.Label lblSpatialRefDisplay;
        private System.Windows.Forms.TextBox txtSpatialRefInfo;
        private System.Windows.Forms.ImageList imlList;
        private System.Windows.Forms.Button btnCreatGeo;
        private System.Windows.Forms.Button btnCreatPrj;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmdDisplayBlank;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.PictureBox pctFold;
        private System.Windows.Forms.TreeView tvSpatialRefNames;
    }
}