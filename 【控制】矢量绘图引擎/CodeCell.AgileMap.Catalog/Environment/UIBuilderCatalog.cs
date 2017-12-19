using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Catalog
{
    public class UIBuilderCatalog:UIBuilder
    {
        public UIBuilderCatalog(Form hostForm, IApplication application) :
            base(hostForm, application)
        { 
        }

        protected override void CreateHook(IApplication application, ICommandHelper commandHelper)
        {
            _defaultHook = new HookCatalog((application as ApplicationCatalog).MapControl,
                                                                 application, null, commandHelper);
        }
    }
}
