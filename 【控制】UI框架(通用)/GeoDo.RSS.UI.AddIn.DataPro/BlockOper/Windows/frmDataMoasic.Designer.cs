namespace GeoDo.RSS.UI.AddIn.DataPro
{
    partial class frmDataMoasic
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataMoasic));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnJoin = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageFiles = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRemoveFiles = new System.Windows.Forms.Button();
            this.btnOpenFiles = new System.Windows.Forms.Button();
            this.btnFullSelect = new System.Windows.Forms.Button();
            this.txtFileList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPageArgs = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.PanelInInfos = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelFileInfo = new System.Windows.Forms.Panel();
            this.tvBands = new System.Windows.Forms.TreeView();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.tvInfo = new System.Windows.Forms.TreeView();
            this.panelOutInfos = new System.Windows.Forms.Panel();
            this.gbOutInfos = new System.Windows.Forms.GroupBox();
            this.btnDefault = new System.Windows.Forms.Button();
            this.groupLine2 = new GeoDo.RSS.UI.AddIn.DataPro.GroupLine();
            this.groupLine1 = new GeoDo.RSS.UI.AddIn.DataPro.GroupLine();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.txtCenterLon = new System.Windows.Forms.TextBox();
            this.txtVaild = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutFileName = new System.Windows.Forms.TextBox();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.ckResolution = new System.Windows.Forms.CheckBox();
            this.ckProcessInvaild = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.OutGeoRangeControl = new GeoDo.RSS.UI.AddIn.DataPro.UCGeoRangeControl();
            this.ckCenterLon = new System.Windows.Forms.CheckBox();
            this.labTip = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageFiles.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPageArgs.SuspendLayout();
            this.PanelInInfos.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelFileInfo.SuspendLayout();
            this.panelOutInfos.SuspendLayout();
            this.gbOutInfos.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "OK.png");
            this.imageList1.Images.SetKeyName(1, "NoOK.png");
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.progressBar1);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Controls.Add(this.btnJoin);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 294);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(621, 29);
            this.panelBottom.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(336, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(171, 21);
            this.progressBar1.TabIndex = 3;
            this.progressBar1.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(105, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "关闭";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnJoin
            // 
            this.btnJoin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnJoin.Location = new System.Drawing.Point(10, 3);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(89, 23);
            this.btnJoin.TabIndex = 1;
            this.btnJoin.Text = "开始拼接";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(339, 291);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(282, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.splitter1);
            this.panelTop.Controls.Add(this.panelRight);
            this.panelTop.Controls.Add(this.splitter3);
            this.panelTop.Controls.Add(this.panelLeft);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(621, 294);
            this.panelTop.TabIndex = 3;
            // 
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(339, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(282, 294);
            this.panelRight.TabIndex = 5;
            // 
            // splitter3
            // 
            this.splitter3.Location = new System.Drawing.Point(336, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 294);
            this.splitter3.TabIndex = 28;
            this.splitter3.TabStop = false;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.tabControl1);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(336, 294);
            this.panelLeft.TabIndex = 29;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageFiles);
            this.tabControl1.Controls.Add(this.tabPageArgs);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 294);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageFiles
            // 
            this.tabPageFiles.Controls.Add(this.panel1);
            this.tabPageFiles.Location = new System.Drawing.Point(4, 22);
            this.tabPageFiles.Name = "tabPageFiles";
            this.tabPageFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFiles.Size = new System.Drawing.Size(328, 268);
            this.tabPageFiles.TabIndex = 2;
            this.tabPageFiles.Text = "待拼接文件";
            this.tabPageFiles.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.txtFileList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(322, 262);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.btnRemoveFiles);
            this.panel2.Controls.Add(this.btnOpenFiles);
            this.panel2.Controls.Add(this.btnFullSelect);
            this.panel2.Location = new System.Drawing.Point(0, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(322, 44);
            this.panel2.TabIndex = 27;
            // 
            // btnRemoveFiles
            // 
            this.btnRemoveFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveFiles.Image")));
            this.btnRemoveFiles.Location = new System.Drawing.Point(239, 12);
            this.btnRemoveFiles.Name = "btnRemoveFiles";
            this.btnRemoveFiles.Size = new System.Drawing.Size(34, 23);
            this.btnRemoveFiles.TabIndex = 4;
            this.btnRemoveFiles.UseVisualStyleBackColor = true;
            this.btnRemoveFiles.Click += new System.EventHandler(this.btnRemoveFiles_Click);
            // 
            // btnOpenFiles
            // 
            this.btnOpenFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFiles.Image")));
            this.btnOpenFiles.Location = new System.Drawing.Point(286, 12);
            this.btnOpenFiles.Name = "btnOpenFiles";
            this.btnOpenFiles.Size = new System.Drawing.Size(34, 23);
            this.btnOpenFiles.TabIndex = 3;
            this.btnOpenFiles.UseVisualStyleBackColor = true;
            this.btnOpenFiles.Click += new System.EventHandler(this.btnOpenFiles_Click);
            // 
            // btnFullSelect
            // 
            this.btnFullSelect.Location = new System.Drawing.Point(1, 12);
            this.btnFullSelect.Name = "btnFullSelect";
            this.btnFullSelect.Size = new System.Drawing.Size(52, 23);
            this.btnFullSelect.TabIndex = 4;
            this.btnFullSelect.Text = "全选";
            this.btnFullSelect.UseVisualStyleBackColor = true;
            this.btnFullSelect.Click += new System.EventHandler(this.btnFullSelect_Click);
            // 
            // txtFileList
            // 
            this.txtFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.txtFileList.HideSelection = false;
            this.txtFileList.Location = new System.Drawing.Point(0, 50);
            this.txtFileList.Name = "txtFileList";
            this.txtFileList.Size = new System.Drawing.Size(322, 206);
            this.txtFileList.SmallImageList = this.imageList1;
            this.txtFileList.TabIndex = 26;
            this.txtFileList.UseCompatibleStateImageBehavior = false;
            this.txtFileList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "文件名";
            this.columnHeader1.Width = 429;
            // 
            // tabPageArgs
            // 
            this.tabPageArgs.Controls.Add(this.splitter2);
            this.tabPageArgs.Controls.Add(this.PanelInInfos);
            this.tabPageArgs.Controls.Add(this.panelOutInfos);
            this.tabPageArgs.Location = new System.Drawing.Point(4, 22);
            this.tabPageArgs.Name = "tabPageArgs";
            this.tabPageArgs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageArgs.Size = new System.Drawing.Size(328, 268);
            this.tabPageArgs.TabIndex = 1;
            this.tabPageArgs.Text = "拼接参数设置";
            this.tabPageArgs.UseVisualStyleBackColor = true;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(3, 244);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(322, 3);
            this.splitter2.TabIndex = 0;
            this.splitter2.TabStop = false;
            // 
            // PanelInInfos
            // 
            this.PanelInInfos.Controls.Add(this.groupBox1);
            this.PanelInInfos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelInInfos.Location = new System.Drawing.Point(3, 244);
            this.PanelInInfos.Name = "PanelInInfos";
            this.PanelInInfos.Size = new System.Drawing.Size(322, 21);
            this.PanelInInfos.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panelFileInfo);
            this.groupBox1.Location = new System.Drawing.Point(0, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 15);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "文件信息";
            // 
            // panelFileInfo
            // 
            this.panelFileInfo.Controls.Add(this.tvBands);
            this.panelFileInfo.Controls.Add(this.splitter4);
            this.panelFileInfo.Controls.Add(this.tvInfo);
            this.panelFileInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFileInfo.Location = new System.Drawing.Point(3, 17);
            this.panelFileInfo.Name = "panelFileInfo";
            this.panelFileInfo.Size = new System.Drawing.Size(316, 0);
            this.panelFileInfo.TabIndex = 0;
            // 
            // tvBands
            // 
            this.tvBands.CheckBoxes = true;
            this.tvBands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvBands.Location = new System.Drawing.Point(165, 0);
            this.tvBands.Name = "tvBands";
            this.tvBands.Size = new System.Drawing.Size(151, 0);
            this.tvBands.TabIndex = 2;
            this.tvBands.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvBands_AfterCheck);
            // 
            // splitter4
            // 
            this.splitter4.Location = new System.Drawing.Point(162, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 0);
            this.splitter4.TabIndex = 1;
            this.splitter4.TabStop = false;
            // 
            // tvInfo
            // 
            this.tvInfo.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvInfo.Location = new System.Drawing.Point(0, 0);
            this.tvInfo.Name = "tvInfo";
            this.tvInfo.Size = new System.Drawing.Size(162, 0);
            this.tvInfo.TabIndex = 0;
            // 
            // panelOutInfos
            // 
            this.panelOutInfos.Controls.Add(this.gbOutInfos);
            this.panelOutInfos.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelOutInfos.Location = new System.Drawing.Point(3, 3);
            this.panelOutInfos.Name = "panelOutInfos";
            this.panelOutInfos.Size = new System.Drawing.Size(322, 241);
            this.panelOutInfos.TabIndex = 1;
            // 
            // gbOutInfos
            // 
            this.gbOutInfos.Controls.Add(this.labTip);
            this.gbOutInfos.Controls.Add(this.btnDefault);
            this.gbOutInfos.Controls.Add(this.groupLine2);
            this.gbOutInfos.Controls.Add(this.groupLine1);
            this.gbOutInfos.Controls.Add(this.txtResolution);
            this.gbOutInfos.Controls.Add(this.txtCenterLon);
            this.gbOutInfos.Controls.Add(this.txtVaild);
            this.gbOutInfos.Controls.Add(this.label4);
            this.gbOutInfos.Controls.Add(this.label5);
            this.gbOutInfos.Controls.Add(this.txtOutFileName);
            this.gbOutInfos.Controls.Add(this.btnSaveFile);
            this.gbOutInfos.Controls.Add(this.ckResolution);
            this.gbOutInfos.Controls.Add(this.ckProcessInvaild);
            this.gbOutInfos.Controls.Add(this.label3);
            this.gbOutInfos.Controls.Add(this.label2);
            this.gbOutInfos.Controls.Add(this.OutGeoRangeControl);
            this.gbOutInfos.Controls.Add(this.ckCenterLon);
            this.gbOutInfos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbOutInfos.Location = new System.Drawing.Point(0, 0);
            this.gbOutInfos.Name = "gbOutInfos";
            this.gbOutInfos.Size = new System.Drawing.Size(322, 241);
            this.gbOutInfos.TabIndex = 0;
            this.gbOutInfos.TabStop = false;
            this.gbOutInfos.Text = "输出设置";
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefault.Location = new System.Drawing.Point(275, 118);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(41, 23);
            this.btnDefault.TabIndex = 25;
            this.btnDefault.Text = "默认";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // groupLine2
            // 
            this.groupLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLine2.Caption = "";
            this.groupLine2.Checked = false;
            this.groupLine2.Image = null;
            this.groupLine2.LineColor = System.Drawing.Color.Black;
            this.groupLine2.Location = new System.Drawing.Point(-9, 140);
            this.groupLine2.Name = "groupLine2";
            this.groupLine2.RadioButtonSize = new System.Drawing.Size(12, 24);
            this.groupLine2.RadioButtonVisible = false;
            this.groupLine2.Size = new System.Drawing.Size(348, 11);
            this.groupLine2.TabIndex = 24;
            // 
            // groupLine1
            // 
            this.groupLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLine1.Caption = "";
            this.groupLine1.Checked = false;
            this.groupLine1.Image = null;
            this.groupLine1.LineColor = System.Drawing.Color.Black;
            this.groupLine1.Location = new System.Drawing.Point(-9, 43);
            this.groupLine1.Name = "groupLine1";
            this.groupLine1.RadioButtonSize = new System.Drawing.Size(12, 24);
            this.groupLine1.RadioButtonVisible = false;
            this.groupLine1.Size = new System.Drawing.Size(341, 10);
            this.groupLine1.TabIndex = 23;
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(98, 211);
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.ReadOnly = true;
            this.txtResolution.Size = new System.Drawing.Size(149, 21);
            this.txtResolution.TabIndex = 11;
            this.txtResolution.Text = "1";
            this.txtResolution.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtResolution_KeyPress);
            // 
            // txtCenterLon
            // 
            this.txtCenterLon.Location = new System.Drawing.Point(98, 151);
            this.txtCenterLon.Name = "txtCenterLon";
            this.txtCenterLon.ReadOnly = true;
            this.txtCenterLon.Size = new System.Drawing.Size(149, 21);
            this.txtCenterLon.TabIndex = 16;
            this.txtCenterLon.Text = "150";
            this.txtCenterLon.TextChanged += new System.EventHandler(this.txtCenterLon_TextChanged);
            this.txtCenterLon.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCenterLon_KeyPress);
            // 
            // txtVaild
            // 
            this.txtVaild.Location = new System.Drawing.Point(98, 181);
            this.txtVaild.Name = "txtVaild";
            this.txtVaild.ReadOnly = true;
            this.txtVaild.Size = new System.Drawing.Size(149, 21);
            this.txtVaild.TabIndex = 13;
            this.txtVaild.Text = "0";
            this.txtVaild.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVaild_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(257, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "度";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(257, 216);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 17;
            this.label5.Text = "倍";
            // 
            // txtOutFileName
            // 
            this.txtOutFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutFileName.Location = new System.Drawing.Point(63, 17);
            this.txtOutFileName.Name = "txtOutFileName";
            this.txtOutFileName.ReadOnly = true;
            this.txtOutFileName.Size = new System.Drawing.Size(207, 21);
            this.txtOutFileName.TabIndex = 22;
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFile.Location = new System.Drawing.Point(275, 16);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(41, 23);
            this.btnSaveFile.TabIndex = 21;
            this.btnSaveFile.Text = "...";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // ckResolution
            // 
            this.ckResolution.AutoSize = true;
            this.ckResolution.Location = new System.Drawing.Point(8, 214);
            this.ckResolution.Name = "ckResolution";
            this.ckResolution.Size = new System.Drawing.Size(72, 16);
            this.ckResolution.TabIndex = 12;
            this.ckResolution.Text = "降分辨率";
            this.ckResolution.UseVisualStyleBackColor = true;
            this.ckResolution.CheckedChanged += new System.EventHandler(this.ckResolution_CheckedChanged);
            // 
            // ckProcessInvaild
            // 
            this.ckProcessInvaild.AutoSize = true;
            this.ckProcessInvaild.Location = new System.Drawing.Point(8, 184);
            this.ckProcessInvaild.Name = "ckProcessInvaild";
            this.ckProcessInvaild.Size = new System.Drawing.Size(84, 16);
            this.ckProcessInvaild.TabIndex = 14;
            this.ckProcessInvaild.Text = "处理无效值";
            this.ckProcessInvaild.UseVisualStyleBackColor = true;
            this.ckProcessInvaild.CheckedChanged += new System.EventHandler(this.ckProcessInvaild_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "输出范围";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 20;
            this.label2.Text = "输出文件";
            // 
            // OutGeoRangeControl
            // 
            this.OutGeoRangeControl.Location = new System.Drawing.Point(63, 50);
            this.OutGeoRangeControl.MaxX = 0D;
            this.OutGeoRangeControl.MaxY = 0D;
            this.OutGeoRangeControl.MinX = 0D;
            this.OutGeoRangeControl.MinY = 0D;
            this.OutGeoRangeControl.Name = "OutGeoRangeControl";
            this.OutGeoRangeControl.Size = new System.Drawing.Size(230, 94);
            this.OutGeoRangeControl.TabIndex = 0;
            // 
            // ckCenterLon
            // 
            this.ckCenterLon.AutoSize = true;
            this.ckCenterLon.Location = new System.Drawing.Point(8, 152);
            this.ckCenterLon.Name = "ckCenterLon";
            this.ckCenterLon.Size = new System.Drawing.Size(72, 16);
            this.ckCenterLon.TabIndex = 10;
            this.ckCenterLon.Text = "中心经度";
            this.ckCenterLon.UseVisualStyleBackColor = true;
            this.ckCenterLon.CheckedChanged += new System.EventHandler(this.ckCenterLon_CheckedChanged);
            // 
            // labTip
            // 
            this.labTip.AutoSize = true;
            this.labTip.Location = new System.Drawing.Point(251, 185);
            this.labTip.Name = "labTip";
            this.labTip.Size = new System.Drawing.Size(101, 12);
            this.labTip.TabIndex = 26;
            this.labTip.Text = "（以“，”间隔）";
            this.labTip.Visible = false;
            // 
            // frmDataMoasic
            // 
            this.ClientSize = new System.Drawing.Size(621, 323);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "frmDataMoasic";
            this.Text = "数据镶嵌/拼接...";
            this.panelBottom.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageFiles.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabPageArgs.ResumeLayout(false);
            this.PanelInInfos.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panelFileInfo.ResumeLayout(false);
            this.panelOutInfos.ResumeLayout(false);
            this.gbOutInfos.ResumeLayout(false);
            this.gbOutInfos.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageFiles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRemoveFiles;
        private System.Windows.Forms.Button btnOpenFiles;
        private System.Windows.Forms.Button btnFullSelect;
        private System.Windows.Forms.ListView txtFileList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TabPage tabPageArgs;
        private System.Windows.Forms.Panel panelOutInfos;
        private System.Windows.Forms.GroupBox gbOutInfos;
        private GroupLine groupLine2;
        private GroupLine groupLine1;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.TextBox txtCenterLon;
        private System.Windows.Forms.TextBox txtVaild;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutFileName;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.CheckBox ckResolution;
        private System.Windows.Forms.CheckBox ckProcessInvaild;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private UCGeoRangeControl OutGeoRangeControl;
        private System.Windows.Forms.CheckBox ckCenterLon;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel PanelInInfos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Panel panelFileInfo;
        private System.Windows.Forms.TreeView tvBands;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.TreeView tvInfo;
        private System.Windows.Forms.Label labTip;
    }
}