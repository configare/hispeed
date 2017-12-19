using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.RedoUndo;


namespace CodeCell.AgileMap.Components
{
    public class cmdPreviousOpr : BaseCommand
    {
        public cmdPreviousOpr()
            :base()
        {
            Init();
        }

        public cmdPreviousOpr(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        protected void Init()
        {
            _text = "撤销";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdUndo.png");
        }

        public override bool Enabled
        {
            get
            {
                IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
                IOperationStack oprStack = mapControl.OperationStack;
                return base.Enabled && ((oprStack.Status & dssStackStatus.HasUndoOperation) == dssStackStatus.HasUndoOperation);
            }
        }

        public override void Click()
        {
            IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
            IOperationStack oprStack = mapControl.OperationStack;
            oprStack.Undo();
        }
    }
}
