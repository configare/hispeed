using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdOpenFile:BaseCommand
    {
        public cmdOpenFile()
        {
            Init();
        }

        public cmdOpenFile(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "打开文件";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdOpen.png");
        }

        public override void Click()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Task Script Files(*.xml)|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    (_hook as IHookOfModelEditor).ModelEditor.LoadScriptFile(dlg.FileName);
                }
            }
        }
    }
}
