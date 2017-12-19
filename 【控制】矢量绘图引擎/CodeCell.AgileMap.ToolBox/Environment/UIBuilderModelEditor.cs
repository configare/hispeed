using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class UIBuilderModelEditor:UIBuilder
    {
        public UIBuilderModelEditor(Form hostForm, IApplication application) :
            base(hostForm, application)
        { 
        }

        protected override void CreateHook(IApplication application, ICommandHelper commandHelper)
        {
            _defaultHook = new HookModelEditor((application as ApplicationModelEditor).ModelEditor,
                application, null, commandHelper);
        }
    }
}
