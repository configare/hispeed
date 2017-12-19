using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal static class SymbolTypeItemProvider
    {
        internal static SymbolTypeItem[] GetAllSymbolTypeItems()
        {
            List<SymbolTypeItem> items = new List<SymbolTypeItem>();
            items.Add(new SymbolTypeItem("简单线符号", typeof(SimpleLineSymbol)));
            items.Add(new SymbolTypeItem("简单填充符号", typeof(SimpleFillSymbol)));
            items.Add(new SymbolTypeItem("简单点符号", typeof(SimpleMarkerSymbol)));
            items.Add(new SymbolTypeItem("字体符号", typeof(TrueTypeFontSymbol)));
            items.Add(new SymbolTypeItem("图片符号", typeof(PictureMarkerSymbol)));
            items.Add(new SymbolTypeItem("铁路符号", typeof(RailwayBrush)));
            items.Add(new SymbolTypeItem("填充线符号", typeof(FillLineSymbol)));
            return items.ToArray();
        }

        internal static SymbolTypeItem[] GetSymbolTypeItemsByShapeType(enumShapeType shapeType)
        {
            SymbolTypeItem[] Items = GetAllSymbolTypeItems();
            List<SymbolTypeItem> retItems = new List<SymbolTypeItem>();
            foreach (SymbolTypeItem it in Items)
            {
                switch (shapeType)
                {
                    case enumShapeType.Point:
                    case enumShapeType.MultiPoint:
                        if(Array.IndexOf(it.SymbolType.GetInterfaces(),typeof(IMarkerSymbol)) >= 0)
                            retItems.Add(it);
                        break;
                    case enumShapeType.Polyline:
                    case enumShapeType.PolylineM:
                    case enumShapeType.PolylineZ:
                        if (Array.IndexOf(it.SymbolType.GetInterfaces(), typeof(ILineSymbol)) >= 0)
                            retItems.Add(it);
                        break;
                    case enumShapeType.Polygon:
                        if (Array.IndexOf(it.SymbolType.GetInterfaces(), typeof(IFillSymbol)) >= 0)
                            retItems.Add(it);
                        break;
                    default:
                        continue ;
                }
            }
            return retItems.ToArray();
        }
    }
}
