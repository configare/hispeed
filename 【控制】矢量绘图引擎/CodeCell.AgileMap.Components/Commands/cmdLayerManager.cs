using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdLayerManager : BaseCommand
    {
        public cmdLayerManager()
        {
            Init();
        }

        public cmdLayerManager(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "层管理器";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdLayerManager.png");
        }

        public override void Click()
        {
            IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
            if (mapControl != null && mapControl.Map != null)
            {
                if (LayerMgrIsExisted(mapControl))
                    return;
                frmLayerManager frm = new frmLayerManager();
                {
                    frm.Apply(mapControl.Map);
                    frm.StartPosition = FormStartPosition.Manual;
                    SetLayerMgrDefaultLocation(frm, mapControl);
                    frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                    frm.Owner = (mapControl as Control).FindForm();
                    frm.Show();
                }
            }
        }

        private bool LayerMgrIsExisted(IMapControl mapControl)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.GetType().Equals(typeof(frmLayerManager)))
                {
                    SetLayerMgrDefaultLocation(frm,mapControl);
                    frm.Activate();
                    return true;
                }
            }
            return false;
        }

        private void SetLayerMgrDefaultLocation(Form frm, IMapControl mapControl)
        {
            Point pt = (mapControl as Control).PointToScreen(new Point(0, 0));
            frm.Location = pt;
            frm.Height = (mapControl as Control).Height - 4;
        }
    }
}
