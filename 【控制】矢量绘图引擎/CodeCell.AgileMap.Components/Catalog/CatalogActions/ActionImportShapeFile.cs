using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using CodeCell.Bricks.ModelFabric;
using CodeCell.AgileMap.Core;


namespace CodeCell.AgileMap.Components
{
    [ActionAttribute("导入ShapeFile", "目录管理", "将ShapeFile导入到空间数据库")]
    public class ActionImportShapeFile:ActionBase
    {
        private IDbConnection _dbConnection = null;
        private string _filename = null;
        private string _tablename = null;
        private MetaFeatureClassDef _featureClassDef = null;

        public ActionImportShapeFile()
            : base()
        {
            Name = "导入ShapeFile";
            _featureClassDef = new MetaFeatureClassDef();
            _featureClassDef.DataTime = DateTime.Now;
        }

        [Binding("矢量文件名", enumBindingDirection.InputOutput, typeof(ArgShpFile), false)]
        public string ShapeFile
        {
            get { return _filename; }
            set
            {
                _filename = value;
            }
        }

        [Binding("要素类名称", enumBindingDirection.InputOutput, typeof(ArgValueType),typeof(string), false)]
        public string Tablename
        {
            get { return _tablename; }
            set
            {
                _tablename = value;
                _featureClassDef.DataTable = _tablename;
                _featureClassDef.Name = _tablename;
            }
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
            if (_filename == null)
            {
                contextMessage.AddError("矢量文件名为空。");
                return false;
            }
            try
            {
                if (!File.Exists(_filename))
                {
                    contextMessage.AddError("矢量文件\"" + _filename + "\"不存在。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                contextMessage.AddError(ex.Message);
                return false;
            }
            if (string.IsNullOrEmpty(_tablename))
            {
                contextMessage.AddError("要素类名称为空。");
                return false;
            }
            return true;
        }

        protected override bool Execute(IContextMessage contextMessage)
        {
            try
            {
                IUniversalVectorDataReader urd = VectorDataReaderFactory.GetUniversalDataReader(_filename);
                if (urd == null)
                {
                    contextMessage.AddError("读取矢量文件\""+_filename+"\"失败。");
                    return false;
                }
                Feature[] fets = (urd as IVectorFeatureDataReader).Features;
                using (IDataImporter imp = new DataImporter())
                {
                    imp.Import(fets, _featureClassDef, _dbConnection, _tracker);
                }
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
