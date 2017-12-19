using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.ModelFabric;


namespace CodeCell.AgileMap.Components
{
    public class CatalogDatabaseConn : CatalogItem
    {
        public CatalogDatabaseConn()
            :base()
        { 
        }

        public CatalogDatabaseConn(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogDatabaseConn(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogDatabaseConn(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogServer.png");
            //
            _oprItems.Add(new ContextOprItem("新建空间数据集", ResourceLoader.GetBitmap("CatalogDataset.png"), enumContextKeys.AddFeatureDataset));
            _oprItems.Add(new ContextOprItem("新建空间数据类", ResourceLoader.GetBitmap("CatalogFeatureClass.png"), enumContextKeys.AddFeatureClass));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("导入...", null, enumContextKeys.ImportData));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("导入地图", null, enumContextKeys.AddServerSource));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("刷新", ResourceLoader.GetBitmap("cmdRefresh.gif"), enumContextKeys.Refresh));
        }

        public override void Click(enumContextKeys key)
        {
            switch (key)
            { 
                case enumContextKeys.AddFeatureDataset:
                    using (frmFeatureDatasetsEditor frm = new frmFeatureDatasetsEditor())
                    {
                        ICatalogEntity entity = new SpatialFeatureDataset();
                        (entity as CatalogEntityBase)._connString = (_tag as SpatialDatabaseConn).ConnectionString;
                        if (frm.ShowDialog(ref entity, true,false))
                        {
                            entity.Store();
                            AddChild(new CatalogFeatureDataset(entity.Name, entity, entity.Description));
                        }
                    }
                    Refresh();
                    break;
                case enumContextKeys.AddFeatureClass:
                    using (frmFetClassDlg frm = new frmFetClassDlg())
                    {
                        ICatalogEntity entity = new SpatialFeatureClass();
                        frm.SetSpatialDataset(null);
                        (entity as CatalogEntityBase)._connString = (_tag as SpatialDatabaseConn).ConnectionString;
                        if (frm.ShowDialog(ref entity, true,false))
                        {
                            entity.Store();
                            AddChild(new CatalogFeatureClass(entity.Name, entity, entity.Description));
                        }
                    }
                    Refresh();
                    break;
                case enumContextKeys.ImportData:
                    ImportData();
                    break;
                default:
                    if (key == enumContextKeys.Refresh)
                        (_tag as SpatialDatabaseConn).Refresh();
                    base.Click(key);
                    break;
            }
        }

        private void ImportData()
        {
            ActionImportFetClass imp = new ActionImportFetClass();
            imp.OutputDataSource = this;
            SimpleActionExecutor.Execute(imp);
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                LoadSubCatalogItems();
                _isLoaded = true;
            }
        }

        private void LoadSubCatalogItems()
        {
            SpatialDatabaseConn conn = _tag as SpatialDatabaseConn;
            if (conn == null)
                return;
            //datasets
            ICatalogEntity[] datasets = conn.GetSpatialFeatureDatasets();
            if (datasets != null && datasets.Length > 0)
            {
                foreach (SpatialFeatureDataset ds in datasets)
                {
                    CatalogFeatureDataset dsIt = new CatalogFeatureDataset(ds.Name, ds, ds.Description);
                    AddChild(dsIt);
                }
            }
            //fetclass
            ICatalogEntity[] fetclasses = conn.GetSpatialFeatureClasses();
            if (fetclasses != null && fetclasses.Length > 0)
            {
                foreach (SpatialFeatureClass fetc in fetclasses)
                {
                    CatalogFeatureClass fetcIt = new CatalogFeatureClass(fetc.Name, fetc, fetc.Description);
                    AddChild(fetcIt);
                }
            }
            //
            Refresh();
        }
    }
}
