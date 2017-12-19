using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.Data;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Components
{
    public class CatalogFeatureClass : CatalogItem
    {
        public CatalogFeatureClass()
            : base()
        {
        }

        public CatalogFeatureClass(string name, object tag)
            : base(name, tag)
        {
        }

        public CatalogFeatureClass(string name, object tag, string description)
            : base(name, tag, description)
        {
        }

        public CatalogFeatureClass(string name, object tag, string description, Image image)
            : base(name, tag, description, image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogFeatureClass.png");
            _oprItems.Add(new ContextOprItem("加载数据", ResourceLoader.GetBitmap("CatalogFeatureClass.png"), enumContextKeys.AddFeatureClass));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("删除", ResourceLoader.GetBitmap("cmdDelete.png"), enumContextKeys.Delete));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("刷新", ResourceLoader.GetBitmap("cmdRefresh.gif"), enumContextKeys.Refresh));
            _oprItems.Add(null);
            _oprItems.Add(new ContextOprItem("属性", ResourceLoader.GetBitmap("cmdProperty.png"), enumContextKeys.Property));
        }

        public override void Click(enumContextKeys key)
        {
            switch (key)
            { 
                case enumContextKeys.Delete:
                    if (MsgBox.ShowQuestionYesNo("确定要删除要素类\"" + _name + "\"吗？\n\n删除以后不能恢复。") == System.Windows.Forms.DialogResult.No)
                        return;
                    SpatialFeatureClass ds = _tag as SpatialFeatureClass;
                     using (ICatalogEntityClass c = new CatalogEntityClassFeatureClass(ds._connString))
                     {
                         c.Delete(ds);
                         ICatalogItem pIt = _parent;
                         _parent.Remove(this);
                         pIt.Refresh();
                         break;
                     }
                case enumContextKeys.Property:
                     using (frmFetClassDlg frm = new frmFetClassDlg())
                     {
                         ICatalogEntity entity = _tag as ICatalogEntity;
                         if (frm.ShowDialog(ref entity, false, false))
                         {
                             entity.Store();
                             Refresh();
                         }
                     }
                     break;
                default:
                    base.Click(key);
                    break;
            }
        }
    }
}
