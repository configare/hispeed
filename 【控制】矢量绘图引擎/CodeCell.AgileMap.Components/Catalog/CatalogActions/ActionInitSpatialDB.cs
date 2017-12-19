using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using CodeCell.Bricks.ModelFabric;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    [ActionAttribute("初始化空间数据库", "目录管理", "创建管理空间数据所需要的元数据表")]
    public class ActionInitSpatialDB:ActionBase
    {
        private IDbConnection _dbConnection = null;
        private BudGISMetadata _metadata = null;

        public ActionInitSpatialDB()
            : base()
        {
            Name = "初始化空间数据库";
        }

        [Binding("数据库连接",enumBindingDirection.Input,typeof(ArgDBConnection),false)]
        public IDbConnection DbConnection
        {
            get { return _dbConnection; }
            set { _dbConnection = value; }
        }

        protected  override void Sleep(IContextMessage contextMessage)
        {
        }

        protected override bool ConditionsIsEnough(IContextMessage contextMessage)
        {
            if (_dbConnection == null)
            {
                contextMessage.AddError("数据库连接对象为空。");
                return false;
            }
            try
            {
                _dbConnection.Open();
            }
            catch (Exception ex)
            {
                contextMessage.AddError(ex.Message);
                return false;
            }
            try
            {
                _metadata = new BudGISMetadata();
            }
            catch (Exception ex)
            {
                contextMessage.AddError(ex.Message);
                return false;
            }
            return true;
        }

        protected override bool Execute(IContextMessage contextMessage)
        {
            try
            {
                SpatialDbBuilder builder = new SpatialDbBuilder(_dbConnection);
                builder.Build(_dbConnection, _metadata, _tracker);
                return true;
            }
            catch (Exception ex)
            {
                contextMessage.AddError(ex.Message);
                return false;
            }
        }

        public override void Reset()
        {
            _dbConnection.Close();
        }

        public override void Dispose()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }
    }
}
