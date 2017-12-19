using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdSetProjection : BaseCommand
    {
        public cmdSetProjection()
        {
            Init();
        }

        public cmdSetProjection(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "设置坐标系统";
            _tooltips = _text;
            // _image = ResourceLoader.GetBitmap("cmdOpen.png");
            _displayStyle = ToolStripItemDisplayStyle.Text;
        }

        public override void Click()
        {
            IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
            if (mapControl != null)
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "ESRI Projection Files(*.prj)|*.prj";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        ISpatialReference sref = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(dlg.FileName);
                        mapControl.SpatialReference = sref;
                    }
                }
            }
        }
    }
}
