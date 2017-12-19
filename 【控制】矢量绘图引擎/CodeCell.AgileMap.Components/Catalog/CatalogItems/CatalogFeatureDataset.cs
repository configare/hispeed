using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.UIs;
using CodeCell.Bricks.ModelFabric;


namespace CodeCell.AgileMap.Components
{
    public class CatalogFeatureDataset:CatalogItem
    {
         public CatalogFeatureDataset()
            :base()
        { 
        }

        public CatalogFeatureDataset(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogFeatureDataset(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogFeatureDataset(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogDataset.png");
            _oprItems.Add(new ContextOprItem("新建空间数据类", ResourceLoader.GetBitmap("CatalogFeatureClass.png"), enumContextKeys.AddFeatureClass));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("删除", ResourceLoader.GetBitmap("cmdDelete.png"), enumContextKeys.Delete));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("导入...", ResourceLoader.GetBitmap("ImportData.gif"), enumContextKeys.ImportData));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("刷新", ResourceLoader.GetBitmap("cmdRefresh.gif"), enumContextKeys.Refresh));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("属性", ResourceLoader.GetBitmap("cmdProperty.png"), enumContextKeys.Property));
        }

        public override void Click(enumContextKeys key)
        {
            switch (key)
            {
                case enumContextKeys.AddFeatureClass:
                    using (frmFetClassDlg frm = new frmFetClassDlg())
                    {
                        ICatalogEntity entity = new SpatialFeatureClass();
                        frm.SetSpatialDataset(_tag as SpatialFeatureDataset);
                        (entity as CatalogEntityBase)._connString = (_tag as CatalogEntityBase)._connString;
                        if (frm.ShowDialog(ref entity, true,false))
                        {
                            if((entity as SpatialFeatureClass).SpatialRef == null)
                                (entity as SpatialFeatureClass).SpatialRef = (_tag as SpatialFeatureDataset).SpatialRef;
                            entity.Store();
                            AddChild(new CatalogFeatureClass(entity.Name, entity, entity.Description));
                        }
                    }
                    Refresh();
                    break;
                case enumContextKeys.Property:
                    using (frmFeatureDatasetsEditor frm = new frmFeatureDatasetsEditor())
                    {
                        ICatalogEntity entity = _tag as ICatalogEntity;
                        if (frm.ShowDialog(ref entity, false, false))
                        {
                            entity.Store();
                            Refresh();
                        }
                    }
                    break;
                case enumContextKeys.Delete:
                    if (MsgBox.ShowQuestionYesNo("确定要删除要素集\"" + _name + "\"吗？\n\n删除以后不能恢复。") == System.Windows.Forms.DialogResult.No)
                        return;
                    SpatialFeatureDataset ds = _tag as SpatialFeatureDataset;
                    using (ICatalogEntityClass c = new CatalogEntityClassFeatureDataset(ds._connString))
                    {
                        c.Delete(ds);
                        ICatalogItem pIt = _parent;
                        _parent.Remove(this);
                        pIt.Refresh();
                    }
                    break;
                case enumContextKeys.ImportData:
                    ImportData();
                    break;
                default:
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
                ICatalogEntity[] fetcs = (_tag as SpatialFeatureDataset).SpatialFeatureClasses;
                if (fetcs != null && fetcs.Length > 0)
                {
                    foreach (ICatalogEntity c in fetcs)
                    {
                        AddChild(new CatalogFeatureClass(c.Name, c, c.Description));
                    }
                    Refresh();
                }
                _isLoaded = true;
            }
        }
    }
}
