using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdSave:BaseCommand
    {
        public cmdSave()
        {
            Init();
        }

        public cmdSave(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "保存";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdSave.png");
        }

        public override void Click()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Task Script Files(*.xml)|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    (_hook as IHookOfModelEditor).ModelEditor.ToScriptFile(dlg.FileName);
                }
            }
        }
    }
}
