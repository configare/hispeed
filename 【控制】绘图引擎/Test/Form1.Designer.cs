namespace Test
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScreenCoord = new System.Windows.Forms.Label();
            this.txtPrjCoord = new System.Windows.Forms.Label();
            this.btnZoomBox = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDummy = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCreateRasterDrawing = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.txtRasterCoord = new System.Windows.Forms.Label();
            this.btnChangeBands = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.btnGDALInfo = new System.Windows.Forms.Button();
            this.btnActive = new System.Windows.Forms.Button();
            this.btnDeactive = new System.Windows.Forms.Button();
            this.btnAddData = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLyrMgr = new System.Windows.Forms.Button();
            this.btnAddVector = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.btnGetVisibleTiles = new System.Windows.Forms.Button();
            this.btn = new System.Windows.Forms.Button();
            this.btnSchedulerII = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.AVILayer = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.button23 = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.button25 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.button28 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.canvasHost1 = new GeoDo.RSS.Core.View.CanvasHost();
            this.button30 = new System.Windows.Forms.Button();
            this.button31 = new System.Windows.Forms.Button();
            this.button32 = new System.Windows.Forms.Button();
            this.button33 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPan
            // 
            this.btnPan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPan.Location = new System.Drawing.Point(742, 13);
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(84, 23);
            this.btnPan.TabIndex = 0;
            this.btnPan.Text = "Default Pan";
            this.btnPan.UseVisualStyleBackColor = true;
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 524);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "屏幕坐标:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 555);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "投影坐标:";
            // 
            // txtScreenCoord
            // 
            this.txtScreenCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtScreenCoord.AutoSize = true;
            this.txtScreenCoord.Location = new System.Drawing.Point(78, 524);
            this.txtScreenCoord.Name = "txtScreenCoord";
            this.txtScreenCoord.Size = new System.Drawing.Size(0, 12);
            this.txtScreenCoord.TabIndex = 4;
            // 
            // txtPrjCoord
            // 
            this.txtPrjCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtPrjCoord.AutoSize = true;
            this.txtPrjCoord.Location = new System.Drawing.Point(80, 554);
            this.txtPrjCoord.Name = "txtPrjCoord";
            this.txtPrjCoord.Size = new System.Drawing.Size(0, 12);
            this.txtPrjCoord.TabIndex = 5;
            // 
            // btnZoomBox
            // 
            this.btnZoomBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomBox.Location = new System.Drawing.Point(742, 42);
            this.btnZoomBox.Name = "btnZoomBox";
            this.btnZoomBox.Size = new System.Drawing.Size(84, 23);
            this.btnZoomBox.TabIndex = 6;
            this.btnZoomBox.Text = "Zoom In";
            this.btnZoomBox.UseVisualStyleBackColor = true;
            this.btnZoomBox.Click += new System.EventHandler(this.btnZoomBox_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(742, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Zoom Out";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDummy
            // 
            this.btnDummy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDummy.Location = new System.Drawing.Point(730, 142);
            this.btnDummy.Name = "btnDummy";
            this.btnDummy.Size = new System.Drawing.Size(103, 23);
            this.btnDummy.TabIndex = 14;
            this.btnDummy.Text = "100%";
            this.btnDummy.UseVisualStyleBackColor = true;
            this.btnDummy.Click += new System.EventHandler(this.btnDummy_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Location = new System.Drawing.Point(742, 115);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(84, 21);
            this.numericUpDown1.TabIndex = 15;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(742, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "Scale:";
            // 
            // btnCreateRasterDrawing
            // 
            this.btnCreateRasterDrawing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateRasterDrawing.Location = new System.Drawing.Point(730, 172);
            this.btnCreateRasterDrawing.Name = "btnCreateRasterDrawing";
            this.btnCreateRasterDrawing.Size = new System.Drawing.Size(103, 40);
            this.btnCreateRasterDrawing.TabIndex = 17;
            this.btnCreateRasterDrawing.Text = "Create RstDrawing";
            this.btnCreateRasterDrawing.UseVisualStyleBackColor = true;
            this.btnCreateRasterDrawing.Click += new System.EventHandler(this.btnCreateRasterDrawing_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(569, 529);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(649, 529);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 21;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(488, 529);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 22;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Location = new System.Drawing.Point(407, 528);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 23;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.Location = new System.Drawing.Point(287, 529);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(114, 23);
            this.button7.TabIndex = 24;
            this.button7.Text = "Load all tiles";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // txtRasterCoord
            // 
            this.txtRasterCoord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtRasterCoord.AutoSize = true;
            this.txtRasterCoord.Location = new System.Drawing.Point(616, 555);
            this.txtRasterCoord.Name = "txtRasterCoord";
            this.txtRasterCoord.Size = new System.Drawing.Size(41, 12);
            this.txtRasterCoord.TabIndex = 26;
            this.txtRasterCoord.Text = "label4";
            // 
            // btnChangeBands
            // 
            this.btnChangeBands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeBands.Location = new System.Drawing.Point(731, 216);
            this.btnChangeBands.Name = "btnChangeBands";
            this.btnChangeBands.Size = new System.Drawing.Size(102, 23);
            this.btnChangeBands.TabIndex = 27;
            this.btnChangeBands.Text = "Change Bands";
            this.btnChangeBands.UseVisualStyleBackColor = true;
            this.btnChangeBands.Click += new System.EventHandler(this.btnChangeBands_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(731, 245);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 24);
            this.button2.TabIndex = 28;
            this.button2.Text = "Change Bands";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.Location = new System.Drawing.Point(730, 275);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(102, 23);
            this.button8.TabIndex = 29;
            this.button8.Text = "MemoryLayer";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // btnGDALInfo
            // 
            this.btnGDALInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGDALInfo.Location = new System.Drawing.Point(731, 304);
            this.btnGDALInfo.Name = "btnGDALInfo";
            this.btnGDALInfo.Size = new System.Drawing.Size(102, 23);
            this.btnGDALInfo.TabIndex = 30;
            this.btnGDALInfo.Text = "GDAL Infos";
            this.btnGDALInfo.UseVisualStyleBackColor = true;
            this.btnGDALInfo.Click += new System.EventHandler(this.btnGDALInfo_Click);
            // 
            // btnActive
            // 
            this.btnActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActive.Location = new System.Drawing.Point(731, 333);
            this.btnActive.Name = "btnActive";
            this.btnActive.Size = new System.Drawing.Size(102, 23);
            this.btnActive.TabIndex = 31;
            this.btnActive.Text = "Active";
            this.btnActive.UseVisualStyleBackColor = true;
            this.btnActive.Click += new System.EventHandler(this.btnActive_Click);
            // 
            // btnDeactive
            // 
            this.btnDeactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeactive.Location = new System.Drawing.Point(731, 362);
            this.btnDeactive.Name = "btnDeactive";
            this.btnDeactive.Size = new System.Drawing.Size(102, 23);
            this.btnDeactive.TabIndex = 32;
            this.btnDeactive.Text = "Deactive";
            this.btnDeactive.UseVisualStyleBackColor = true;
            this.btnDeactive.Click += new System.EventHandler(this.btnDeactive_Click);
            // 
            // btnAddData
            // 
            this.btnAddData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddData.Location = new System.Drawing.Point(730, 473);
            this.btnAddData.Name = "btnAddData";
            this.btnAddData.Size = new System.Drawing.Size(102, 23);
            this.btnAddData.TabIndex = 40;
            this.btnAddData.Text = "Add Data";
            this.btnAddData.UseVisualStyleBackColor = true;
            this.btnAddData.Click += new System.EventHandler(this.btnAddData_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(730, 445);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 23);
            this.btnSave.TabIndex = 39;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLyrMgr
            // 
            this.btnLyrMgr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLyrMgr.Location = new System.Drawing.Point(730, 420);
            this.btnLyrMgr.Name = "btnLyrMgr";
            this.btnLyrMgr.Size = new System.Drawing.Size(102, 23);
            this.btnLyrMgr.TabIndex = 38;
            this.btnLyrMgr.Text = "Layer Manager";
            this.btnLyrMgr.UseVisualStyleBackColor = true;
            this.btnLyrMgr.Click += new System.EventHandler(this.btnLyrMgr_Click);
            // 
            // btnAddVector
            // 
            this.btnAddVector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddVector.Location = new System.Drawing.Point(729, 391);
            this.btnAddVector.Name = "btnAddVector";
            this.btnAddVector.Size = new System.Drawing.Size(102, 23);
            this.btnAddVector.TabIndex = 37;
            this.btnAddVector.Text = "VectorHost";
            this.btnAddVector.UseVisualStyleBackColor = true;
            this.btnAddVector.Click += new System.EventHandler(this.btnAddVector_Click);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button9.Location = new System.Drawing.Point(730, 529);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(102, 23);
            this.button9.TabIndex = 41;
            this.button9.Text = "OrbitProjection";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // btnGetVisibleTiles
            // 
            this.btnGetVisibleTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetVisibleTiles.Location = new System.Drawing.Point(622, 500);
            this.btnGetVisibleTiles.Name = "btnGetVisibleTiles";
            this.btnGetVisibleTiles.Size = new System.Drawing.Size(102, 23);
            this.btnGetVisibleTiles.TabIndex = 44;
            this.btnGetVisibleTiles.Text = "Visible Tiles";
            this.btnGetVisibleTiles.UseVisualStyleBackColor = true;
            this.btnGetVisibleTiles.Click += new System.EventHandler(this.btnGetVisibleTiles_Click);
            // 
            // btn
            // 
            this.btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn.Location = new System.Drawing.Point(622, 471);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(102, 23);
            this.btn.TabIndex = 43;
            this.btn.Text = "Reduce Memory";
            this.btn.UseVisualStyleBackColor = true;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // btnSchedulerII
            // 
            this.btnSchedulerII.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSchedulerII.Location = new System.Drawing.Point(622, 442);
            this.btnSchedulerII.Name = "btnSchedulerII";
            this.btnSchedulerII.Size = new System.Drawing.Size(102, 23);
            this.btnSchedulerII.TabIndex = 42;
            this.btnSchedulerII.Text = "Scheduler II";
            this.btnSchedulerII.UseVisualStyleBackColor = true;
            this.btnSchedulerII.Click += new System.EventHandler(this.btnSchedulerII_Click);
            // 
            // button10
            // 
            this.button10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button10.Location = new System.Drawing.Point(730, 502);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(102, 23);
            this.button10.TabIndex = 45;
            this.button10.Text = "Add Map";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button11.Location = new System.Drawing.Point(622, 413);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(102, 23);
            this.button11.TabIndex = 46;
            this.button11.Text = "Dispose";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button12.Location = new System.Drawing.Point(623, 384);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(102, 23);
            this.button12.TabIndex = 47;
            this.button12.Text = "GLL Grid";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 500);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 48;
            this.label4.Text = "label4";
            // 
            // button13
            // 
            this.button13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button13.Location = new System.Drawing.Point(623, 355);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(102, 23);
            this.button13.TabIndex = 49;
            this.button13.Text = "Pan Adjust";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button14.Location = new System.Drawing.Point(623, 326);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(102, 23);
            this.button14.TabIndex = 50;
            this.button14.Text = "Tool bar";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button15
            // 
            this.button15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button15.Location = new System.Drawing.Point(623, 297);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(102, 23);
            this.button15.TabIndex = 51;
            this.button15.Text = "draw curve";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button16
            // 
            this.button16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button16.Location = new System.Drawing.Point(623, 268);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(102, 23);
            this.button16.TabIndex = 52;
            this.button16.Text = "flash";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button17
            // 
            this.button17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button17.Location = new System.Drawing.Point(623, 239);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(102, 23);
            this.button17.TabIndex = 53;
            this.button17.Text = "To Bitmap";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // button18
            // 
            this.button18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button18.Location = new System.Drawing.Point(623, 210);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(102, 23);
            this.button18.TabIndex = 54;
            this.button18.Text = "Color Map";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // button19
            // 
            this.button19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button19.Location = new System.Drawing.Point(623, 181);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(102, 23);
            this.button19.TabIndex = 55;
            this.button19.Text = "To Bitmap";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // button20
            // 
            this.button20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button20.Location = new System.Drawing.Point(623, 152);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(102, 23);
            this.button20.TabIndex = 56;
            this.button20.Text = "To Full Bitmap";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // button21
            // 
            this.button21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button21.Location = new System.Drawing.Point(623, 123);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(102, 23);
            this.button21.TabIndex = 57;
            this.button21.Text = "Feature AOI";
            this.button21.UseVisualStyleBackColor = true;
            this.button21.Click += new System.EventHandler(this.button21_Click);
            // 
            // AVILayer
            // 
            this.AVILayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AVILayer.Location = new System.Drawing.Point(623, 94);
            this.AVILayer.Name = "AVILayer";
            this.AVILayer.Size = new System.Drawing.Size(102, 23);
            this.AVILayer.TabIndex = 58;
            this.AVILayer.Text = "AVILayer";
            this.AVILayer.UseVisualStyleBackColor = true;
            this.AVILayer.Click += new System.EventHandler(this.AVILayer_Click);
            // 
            // button22
            // 
            this.button22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button22.Location = new System.Drawing.Point(623, 65);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(102, 23);
            this.button22.TabIndex = 59;
            this.button22.Text = "GetScale";
            this.button22.UseVisualStyleBackColor = true;
            this.button22.Click += new System.EventHandler(this.button22_Click);
            // 
            // button23
            // 
            this.button23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button23.Location = new System.Drawing.Point(623, 36);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(102, 23);
            this.button23.TabIndex = 60;
            this.button23.Text = "Refresh";
            this.button23.UseVisualStyleBackColor = true;
            this.button23.Click += new System.EventHandler(this.button23_Click);
            // 
            // button24
            // 
            this.button24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button24.Location = new System.Drawing.Point(623, 7);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(102, 23);
            this.button24.TabIndex = 61;
            this.button24.Text = "Mem Vector";
            this.button24.UseVisualStyleBackColor = true;
            this.button24.Click += new System.EventHandler(this.button24_Click);
            // 
            // button25
            // 
            this.button25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button25.Location = new System.Drawing.Point(515, 7);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(102, 23);
            this.button25.TabIndex = 62;
            this.button25.Text = "StrongRefresh";
            this.button25.UseVisualStyleBackColor = true;
            this.button25.Click += new System.EventHandler(this.button25_Click);
            // 
            // button26
            // 
            this.button26.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button26.Location = new System.Drawing.Point(515, 500);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(102, 23);
            this.button26.TabIndex = 63;
            this.button26.Text = "grid";
            this.button26.UseVisualStyleBackColor = true;
            this.button26.Click += new System.EventHandler(this.button26_Click);
            // 
            // button27
            // 
            this.button27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button27.Location = new System.Drawing.Point(468, 42);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(148, 23);
            this.button27.TabIndex = 64;
            this.button27.Text = "Add BackgroudLayer";
            this.button27.UseVisualStyleBackColor = true;
            this.button27.Click += new System.EventHandler(this.button27_Click);
            // 
            // button28
            // 
            this.button28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button28.Location = new System.Drawing.Point(515, 384);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(102, 23);
            this.button28.TabIndex = 65;
            this.button28.Text = "GLL Grid II";
            this.button28.UseVisualStyleBackColor = true;
            this.button28.Click += new System.EventHandler(this.button28_Click);
            // 
            // button29
            // 
            this.button29.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button29.Location = new System.Drawing.Point(515, 355);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(102, 23);
            this.button29.TabIndex = 66;
            this.button29.Text = "To Full Envelope";
            this.button29.UseVisualStyleBackColor = true;
            this.button29.Click += new System.EventHandler(this.button29_Click);
            // 
            // canvasHost1
            // 
            this.canvasHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvasHost1.Location = new System.Drawing.Point(13, 13);
            this.canvasHost1.Name = "canvasHost1";
            this.canvasHost1.Size = new System.Drawing.Size(603, 510);
            this.canvasHost1.TabIndex = 25;
            this.canvasHost1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvasHost1_MouseMove);
            // 
            // button30
            // 
            this.button30.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button30.Location = new System.Drawing.Point(468, 71);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(148, 23);
            this.button30.TabIndex = 67;
            this.button30.Text = "Global Cacher";
            this.button30.UseVisualStyleBackColor = true;
            this.button30.Click += new System.EventHandler(this.button30_Click);
            // 
            // button31
            // 
            this.button31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button31.Location = new System.Drawing.Point(515, 326);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(102, 23);
            this.button31.TabIndex = 68;
            this.button31.Text = "Ruler";
            this.button31.UseVisualStyleBackColor = true;
            this.button31.Click += new System.EventHandler(this.button31_Click);
            // 
            // button32
            // 
            this.button32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button32.Location = new System.Drawing.Point(515, 297);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(102, 23);
            this.button32.TabIndex = 69;
            this.button32.Text = "Mem Estimator";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.button32_Click);
            // 
            // button33
            // 
            this.button33.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button33.Location = new System.Drawing.Point(515, 413);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(102, 23);
            this.button33.TabIndex = 70;
            this.button33.Text = "1:1 Bitmap";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Click += new System.EventHandler(this.button33_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 585);
            this.Controls.Add(this.button33);
            this.Controls.Add(this.button32);
            this.Controls.Add(this.button31);
            this.Controls.Add(this.button30);
            this.Controls.Add(this.button29);
            this.Controls.Add(this.button28);
            this.Controls.Add(this.button27);
            this.Controls.Add(this.button26);
            this.Controls.Add(this.button25);
            this.Controls.Add(this.button24);
            this.Controls.Add(this.button23);
            this.Controls.Add(this.button22);
            this.Controls.Add(this.AVILayer);
            this.Controls.Add(this.button21);
            this.Controls.Add(this.button20);
            this.Controls.Add(this.button19);
            this.Controls.Add(this.button18);
            this.Controls.Add(this.button17);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.btnGetVisibleTiles);
            this.Controls.Add(this.btn);
            this.Controls.Add(this.btnSchedulerII);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.btnAddData);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLyrMgr);
            this.Controls.Add(this.btnAddVector);
            this.Controls.Add(this.btnDeactive);
            this.Controls.Add(this.btnActive);
            this.Controls.Add(this.btnGDALInfo);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnChangeBands);
            this.Controls.Add(this.txtRasterCoord);
            this.Controls.Add(this.canvasHost1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnCreateRasterDrawing);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.btnDummy);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnZoomBox);
            this.Controls.Add(this.txtPrjCoord);
            this.Controls.Add(this.txtScreenCoord);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPan);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txtScreenCoord;
        private System.Windows.Forms.Label txtPrjCoord;
        private System.Windows.Forms.Button btnZoomBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDummy;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCreateRasterDrawing;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private GeoDo.RSS.Core.View.CanvasHost canvasHost1;
        private System.Windows.Forms.Label txtRasterCoord;
        private System.Windows.Forms.Button btnChangeBands;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button btnGDALInfo;
        private System.Windows.Forms.Button btnActive;
        private System.Windows.Forms.Button btnDeactive;
        private System.Windows.Forms.Button btnAddData;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLyrMgr;
        private System.Windows.Forms.Button btnAddVector;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button btnGetVisibleTiles;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Button btnSchedulerII;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button AVILayer;
        private System.Windows.Forms.Button button22;
        private System.Windows.Forms.Button button23;
        private System.Windows.Forms.Button button24;
        private System.Windows.Forms.Button button25;
        private System.Windows.Forms.Button button26;
        private System.Windows.Forms.Button button27;
        private System.Windows.Forms.Button button28;
        private System.Windows.Forms.Button button29;
        private System.Windows.Forms.Button button30;
        private System.Windows.Forms.Button button31;
        private System.Windows.Forms.Button button32;
        private System.Windows.Forms.Button button33;
    }
}

