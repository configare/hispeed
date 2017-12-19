using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Components;

namespace CodeCell.AgileMap.Express
{
    public class UIBuilderAgileMap:UIBuilder
    {
        public UIBuilderAgileMap(Form hostForm, IApplication application) :
            base(hostForm, application)
        { 
        }

        protected override void CreateHook(IApplication application, ICommandHelper commandHelper)
        {
            _defaultHook = new HookAgileMap((application as ApplicationAgileMap).MapControl,
                                                                 application, null, commandHelper);
        }
    }
}
