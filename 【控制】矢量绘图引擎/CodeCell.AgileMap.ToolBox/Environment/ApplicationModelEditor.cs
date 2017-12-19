using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.ToolBox
{
    public class ApplicationModelEditor:ApplicationDefault
    {
        protected IModelEditor _modelEditor = null;

        public ApplicationModelEditor(IModelEditor modelEditor, string tempDir)
            :base(tempDir)
        {
            _modelEditor = modelEditor;
        }

        public IModelEditor ModelEditor
        {
            get { return _modelEditor; }
        }
    }
}
