using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.RedoUndo;


namespace CodeCell.AgileMap.Components
{
    public class cmdNextOpr : BaseCommand
    {
        public cmdNextOpr()
            : base()
        {
            Init();
        }

        public cmdNextOpr(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        protected void Init()
        {
            _text = "重做";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdRedo.png");
        }

        public override bool Enabled
        {
            get
            {
                IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
                IOperationStack oprStack = mapControl.OperationStack;
                return base.Enabled && ((oprStack.Status & dssStackStatus.HasRedoOperation) == dssStackStatus.HasRedoOperation);
            }
        }

        public override void Click()
        {
            IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
            IOperationStack oprStack = mapControl.OperationStack;
            oprStack.Redo();
        }
    }
}
