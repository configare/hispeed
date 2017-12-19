using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;


namespace CodeCell.AgileMap.Components
{
    [ActionAttribute("导入数据", "目录管理", "从一个数据源导入到另一个数据源")]
    public class ActionImportFetClass:ActionBase
    {
        protected ICatalogItem _inputDataSource = null;
        protected ICatalogItem _outputDataSource = null;
        protected string _tablename = null;
        protected string _displayName = null;
        protected string _description = null;

        public ActionImportFetClass()
            : base()
        {
            Name = "导入数据";
        }

        [Binding("导入?数据", enumBindingDirection.Input, typeof(ArgCatalogItemFetClass), false)]
        public ICatalogItem InputDataSource
        {
            get { return _inputDataSource; }
            set { _inputDataSource = value; }
        }

        [Binding("导入到哪里...?", enumBindingDirection.Input, typeof(ArgCatalogItemLocation), false)]
        public ICatalogItem OutputDataSource
        {
            get { return _outputDataSource; }
            set { _outputDataSource = value; }
        }

        [Binding("要素类名称", enumBindingDirection.InputOutput, typeof(ArgValueType), typeof(string), false)]
        public string TableName
        {
            get { return _tablename; }
            set { _tablename = value; }
        }

        [Binding("显示名称", enumBindingDirection.InputOutput, typeof(ArgValueType), typeof(string), false)]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        [Binding("要素类描述", enumBindingDirection.InputOutput, typeof(ArgValueType), typeof(string), false)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        protected  override void Sleep(IContextMessage contextMessage)
        {
        }

        protected override bool ConditionsIsEnough(IContextMessage contextMessage)
        {
            if (_inputDataSource == null)
            {
                contextMessage.AddError("要导入的数据对象没有设置。");
                return false;
            }
            if (_outputDataSource == null)
            {
                contextMessage.AddError("目标位置对象没有设置。");
                return false;
            }
            if (string.IsNullOrEmpty(_name))
            {
                contextMessage.AddError("要素类名称没有输入。");
                return false;
            }
            if (string.IsNullOrEmpty(_displayName))
            {
                contextMessage.AddError("显示名称没有输入。");
                return false;
            }
            return true;
        }

        protected override bool Execute(IContextMessage contextMessage)
        {
            try
            {
                using (IFetClassImporter imp = FetClassImporterFactory.GetImporter(_inputDataSource, _outputDataSource))
                {
                    if (imp != null)
                    {
                        imp.Import(_inputDataSource, _outputDataSource, _tracker,_tablename,_displayName,_description);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                contextMessage.AddError(ex.Message);
                return false;
            }
            return false;
        }

        public override void Reset()
        {
            _outputDataSource = null;
            _inputDataSource = null;
        }

        public override void Dispose()
        {
            Reset();
        }
    }
}
