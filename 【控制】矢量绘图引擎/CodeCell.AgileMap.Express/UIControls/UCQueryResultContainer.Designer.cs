

using CodeCell.Bricks.UIs;
namespace CodeCell.AgileMap.Express
{
    partial class UCQueryResultContainer
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCQueryResultContainer));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ucHierarchicalListBox1 = new UCHierarchicalListBox();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "PaintDotNet.Icons.MenuWindowLayersIcon.png");
            // 
            // ucHierarchicalListBox1
            // 
            this.ucHierarchicalListBox1.CountPerPage = 20;
            this.ucHierarchicalListBox1.ImageList = null;
            this.ucHierarchicalListBox1.Indent = 24;
            this.ucHierarchicalListBox1.Location = new System.Drawing.Point(0, 0);
            this.ucHierarchicalListBox1.Name = "ucHierarchicalListBox1";
            this.ucHierarchicalListBox1.Size = new System.Drawing.Size(241, 428);
            this.ucHierarchicalListBox1.TabIndex = 0;
            this.ucHierarchicalListBox1.OnAfterDrawHierItems += new UCHierarchicalListBox.OnAfterDrawHierItemsHandler(this.ucHierarchicalListBox1_OnAfterDrawHierItems);
            this.ucHierarchicalListBox1.OnClickHierItem += new UCHierarchicalListBox.OnClickHierItemHandler(this.ucHierarchicalListBox1_OnClickHierItem);
            // 
            // UCQueryResultContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.ucHierarchicalListBox1);
            this.Name = "UCQueryResultContainer";
            this.Size = new System.Drawing.Size(241, 428);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private UCHierarchicalListBox ucHierarchicalListBox1;
    }
}
