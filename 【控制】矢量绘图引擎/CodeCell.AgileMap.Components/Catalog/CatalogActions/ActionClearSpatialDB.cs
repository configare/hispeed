using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.Data;
using CodeCell.AgileMap.Core;


namespace CodeCell.AgileMap.Components
{
    [ActionAttribute("清空空间数据库", "目录管理", "清空空间数据库,包括元数据表和要素表")]
    public class ActionClearSpatialDB:ActionBase
    {
        private IDbConnection _dbConnection = null;
        private BudGISMetadata _metadata = null;
        private bool _clearDataTables = true;

        public ActionClearSpatialDB()
            : base()
        {
            Name = "清空空间数据库";
        }

        [Binding("数据库连接",enumBindingDirection.Input,typeof(ArgDBConnection),false)]
        public IDbConnection DbConnection
        {
            get { return _dbConnection; }
            set { _dbConnection = value; }
        }

        [Binding("是否清空已入库数据", enumBindingDirection.InputOutput, typeof(ArgValueType),typeof(bool), false)]
        public bool IsClearDataTables
        {
            get { return _clearDataTables; }
            set { _clearDataTables = value; }
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
                builder.Clear(_dbConnection, _metadata, _clearDataTables,_tracker);
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
