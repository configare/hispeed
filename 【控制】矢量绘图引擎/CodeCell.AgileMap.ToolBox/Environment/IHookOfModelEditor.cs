using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.ToolBox
{
    public interface IHookOfModelEditor
    {
        IModelEditor ModelEditor { get; }
    }
}
