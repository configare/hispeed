using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IPropertyEditorDialog
    {
        bool ShowDialog(IAction action,IBindingEnvironment bindingEnvironment);
    }
}
