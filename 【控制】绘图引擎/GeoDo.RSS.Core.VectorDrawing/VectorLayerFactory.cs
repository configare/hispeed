using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public static class VectorLayerFactory
    {
       static VectorLayerFactory()
        { 
        }

        public static IFeatureLayer CreateFeatureLayer(string fname,object arguments)
        {
            IFeatureDataSource ds = new FileDataSource(Path.GetFileNameWithoutExtension(fname), fname);
            IFeatureClass fetclass = new FeatureClass(ds as FeatureDataSourceBase);
            string dataName = Path.GetFileNameWithoutExtension(fname);
            IFeatureLayer layer = new FeatureLayer(dataName, fetclass);
            layer.LabelDef = GetLabelDef(fname,layer);
            layer.Renderer = GetRender(fname, layer);
            return layer;
        }

        private static IFeatureRenderer GetRender(string dataName, IFeatureLayer layer)
        {
            return new SimpleFeatureRenderer(GetSymbol(dataName, (layer.Class as FeatureClass).ShapeType));
        }

        private static ILabelDef GetLabelDef(string dataName, IFeatureLayer layer)
        {
            string[] fields =  (layer.Class as IFeatureClass).FieldNames;
            string lbField = GetLabelFieldName(fields); 
            LabelDef def = new LabelDef(lbField, fields);
            def.LabelFont = new Font("宋体", 9);
            def.EnableLabeling = !string.IsNullOrEmpty(lbField);
            def.MaskColor = Color.FromArgb(192, 192, 255);
            def.EnableLabeling = false;
            return def;
        }

        public static string GetLabelFieldName(string[] fields)
        {
            if (fields == null || fields.Length ==0)
                return null;
            string[] names = new string[] {"CHINESE_CH", "CH_NAME", "NAME", "名称" ,"分幅编号"};
            foreach (string fld in fields)
            {
                if (Array.IndexOf<string>(names, fld.ToUpper()) >= 0)
                    return fld;
            }
            return null;
        }

        private static ISymbol GetSymbol(string dataName, enumShapeType shapeType)
        {
            switch (shapeType)
            { 
                case enumShapeType.Point:
                    return new SimpleMarkerSymbol(masSimpleMarkerStyle.Circle);
                case enumShapeType.Polyline:
                    return new SimpleLineSymbol(Color.Blue, 1);
                case enumShapeType.Polygon:
                    return new SimpleFillSymbol(Color.Transparent, new SimpleLineSymbol(Color.Blue, 1));
                default:
                    return null;
            }
        }
    }
}
