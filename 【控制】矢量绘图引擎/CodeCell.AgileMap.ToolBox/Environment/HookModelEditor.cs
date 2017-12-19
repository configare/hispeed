using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.ToolBox
{
    public class HookModelEditor:HookDefault,IHookOfModelEditor
    {
        protected IModelEditor _modelEditor = null;

        public HookModelEditor(IModelEditor modelEditor, IApplication application, ToolStripItem control, ICommandHelper commandHelper)
            : base(application, control, commandHelper)
        {
            _modelEditor = modelEditor;
        }

        public IModelEditor ModelEditor
        {
            get { return _modelEditor; }
        }
    }
}
