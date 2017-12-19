using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdAddLayer:BaseCommand
    {
        public cmdAddLayer()
        {
            Init();
        }

        public cmdAddLayer(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "添加图层";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdAddLayer.png");
        }

        public override void Click()
        {
             IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
             if (mapControl != null)
             {
                 using (IDataSource ds = DataSourceDialog.OpenDataSource())
                 {
                     if (ds == null)
                         return;
                     ILayer layer = null;
                     if (ds is IFeatureDataSource)
                     {
                         IFeatureClass fetc = new FeatureClass(ds as FeatureDataSourceBase);
                         layer = new FeatureLayer(ds.Name, fetc);
                     }
                     //else if (ds is IRasterDataSource)
                     //{
                     //    layer = new RasterLayer(ds.Name, new RasterClass((ds as IRasterDataSource).Url));
                     //}
                     if (mapControl.Map == null)
                     {
                         IMap map = new Map(new ILayer[] { layer });
                         map.Name = "新地图";
                         mapControl.Apply(map);
                     }
                     else
                     {
                         mapControl.Map.LayerContainer.Append(layer as ILayer);
                     }
                     mapControl.ReRender();
                 }
             }
        }
    }
}
