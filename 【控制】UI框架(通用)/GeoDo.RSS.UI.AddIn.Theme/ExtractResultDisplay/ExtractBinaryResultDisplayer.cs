using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    internal class ExtractBinaryResultDisplayer:IExtractResultDisplayer
    {
        private ISmartSession _session;

        public ExtractBinaryResultDisplayer(ISmartSession session)
        {
            _session = session;
        }

        public void Display(IMonitoringSubProduct subProduct, IExtractResult result)
        {
            DisplayIndexResult(subProduct, subProduct.Identify, result as IPixelIndexMapper, subProduct.ArgumentProvider.DataProvider);
        }

        private void DisplayIndexResult(IMonitoringSubProduct subProduct, string name, IPixelIndexMapper pixelIndexMapper, IRasterDataProvider dataProvider)
        {
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            using (IBinaryBitmapBuilder builder = new BinaryBitmapBuilder())
            {
                Size bmSize = new Size(dataProvider.Width, dataProvider.Height);
                Color productColor = GetBinaryColor(subProduct);
                Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, productColor, Color.Transparent);
                int[] indexes = pixelIndexMapper.Indexes.ToArray();
                builder.Fill(indexes, new Size(dataProvider.Width, dataProvider.Height), ref bitmap);
                //
                IBinaryBitampLayer layer = cv.Canvas.LayerContainer.GetByName(name) as IBinaryBitampLayer;
                if (layer == null)
                {
                    layer = new BinaryBitmapLayer(name, bitmap, GetCoordEnvelope(dataProvider), dataProvider.CoordType == enumCoordType.GeoCoord);
                    cv.Canvas.LayerContainer.Layers.Add(layer);
                }
                else
                    layer.Bitmap = bitmap;
                cv.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
            }
        }

        private Color GetBinaryColor(IMonitoringSubProduct subProduct)
        {
            if (subProduct == null)
                return Color.Red;
            return subProduct.Definition.Color;
        }

        private Core.DrawEngine.CoordEnvelope GetCoordEnvelope(IRasterDataProvider dataProvider)
        {
            Core.DrawEngine.CoordEnvelope evp = new Core.DrawEngine.CoordEnvelope(
                dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxY);
            return evp;
        }

    }
}
