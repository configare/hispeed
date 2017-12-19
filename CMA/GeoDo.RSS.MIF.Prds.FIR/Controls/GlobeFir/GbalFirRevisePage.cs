/*========================================================================
* 功能概述：全球火点数据修正界面,实现矢量火点的读取,显示,可视化编辑,保存等功能
* 
* 创建者：张延冰    时间：2013.08.12
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;


namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class GbalFirRevisePage :ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private UCGbalFirRevise _firstPageContent;

        public GbalFirRevisePage()
        {
            InitializeComponent();
        }

        public GbalFirRevisePage(ISmartSession smartSession):this()
        {
            Load_firstPageContent(smartSession);
        }

        private void Load_firstPageContent(ISmartSession smartSession)
        {
            _firstPageContent = new UCGbalFirRevise(smartSession);
            _firstPageContent.Dock = DockStyle.Fill;
            this.Text =_firstPageContent.Text;
            this.Controls.Add(_firstPageContent);            
        }
       
        public EventHandler OnWindowClosed
        {
            get
            {
                return _onWindowClosed;
            }
            set
            {
                _onWindowClosed = value;
            }
        }

        public new void Free()
        {
            if(_firstPageContent!=null)
                _firstPageContent.Free();
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}
