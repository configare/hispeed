namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    partial class CanvasViewer
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
            this.canvasHost1 = new GeoDo.RSS.Core.View.CanvasHost();
            this.SuspendLayout();
            // 
            // canvasHost1
            // 
            this.canvasHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasHost1.Location = new System.Drawing.Point(0, 0);
            this.canvasHost1.Name = "canvasHost1";
            this.canvasHost1.Size = new System.Drawing.Size(655, 384);
            this.canvasHost1.TabIndex = 0;
            // 
            // CanvasViewer
            // 
            this.Controls.Add(this.canvasHost1);
            this.Size = new System.Drawing.Size(655, 384);
            this.ResumeLayout(false);

        }

        #endregion

        private Core.View.CanvasHost canvasHost1;
    }
}
