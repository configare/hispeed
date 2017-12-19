using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.WinForm
{
    public static class LayoutDataFrameInitlializer
    {
        public static void Init()
        {
            //为数据框注册添加栅格数据的委托
            GeoDo.RSS.Layout.DataFrm.DataFrame.AddFileToCanvasViewerExecutor = (fname, argument, canvas, fileOpenargs, colorTableName) =>
            {
                //IRgbStretcherProvider rgbStretcherpProvider = null;
                //if (fname.ToUpper().EndsWith(".DAT"))
                //    rgbStretcherpProvider = new RgbStretcherProvider();
                IRgbStretcherProvider rgbStretcherpProvider = new RgbStretcherProvider();
                if (string.IsNullOrWhiteSpace(colorTableName))
                    colorTableName = GetColorTableName(argument);//兼容最早colortablename放在argument中的模式
                else
                    colorTableName = "colortablename=" + colorTableName;
                List<string> options = new List<string>();
                if(!string.IsNullOrWhiteSpace(colorTableName))
                    options.Add(colorTableName);
                if(fileOpenargs!=null)
                    options.AddRange(fileOpenargs);
                IRasterDrawing drawing = new RasterDrawing(fname, canvas, rgbStretcherpProvider, options.ToArray());
                drawing.SelectedBandNos = GetDefaultBands(drawing);
                IRasterLayer lyr = new RasterLayer(drawing);
                canvas.LayerContainer.Layers.Add(lyr);
                canvas.PrimaryDrawObject = drawing;
                canvas.CurrentEnvelope = drawing.OriginalEnvelope;
                drawing.StartLoading(null);
                TryLoadAOIMaskLayer(canvas, drawing,fname);
            };
            //为栅格图例注册颜色表获取委托
            GeoDo.RSS.Layout.RasterLegendElement.RasterLegendItemsGetter = (colorTableName) => 
            {
                ProductColorTable colorTable = ProductColorTableFactory.GetColorTable(colorTableName);
                if (colorTable == null)
                    return null;
                List<LegendItem> items = new List<LegendItem>();
                foreach (ProductColor c in colorTable.ProductColors)
                {
                    if (!c.DisplayLengend)
                        continue;
                    string txt = c.LableText;
                    items.Add(new LegendItem(txt,c.Color));
                }
                return items.Count > 0 ? items.ToArray() : null;
            };
        }

        private static string[] GetOptions(object argument)
        {
            if (argument == null)
                return null;
            if (argument is string[])
            {
                return argument as string[];
            }
            return null;
        }

        private static string GetColorTableName(object argument)
        {
            if (argument == null)
                return null;
            if (argument.ToString().ToLower().Contains("colortablename"))
                return argument.ToString();
            return null;
        }

        private static void TryLoadAOIMaskLayer(ICanvas canvas, IRasterDrawing drawing, string fname)
        {
            string aoiFileName = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + ".aoi");
            if (!File.Exists(aoiFileName))
                return;
            int[] aoi;
            using (FileStream fs = new FileStream(aoiFileName,FileMode.Open))
            {
                int count = (int)(fs.Length / Marshal.SizeOf(typeof(int)));
                aoi = new int[count];
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < count; i++)
                        aoi[i] = br.ReadInt32();
                }
            }
            ILayer lyr = canvas.LayerContainer.GetByName("蒙板层");
            if (lyr == null)
            {
                lyr = new MaskLayer();
                canvas.LayerContainer.Layers.Add(lyr);
            }
            IMaskLayer maskLayer = lyr as IMaskLayer;
            maskLayer.Update(Color.White,drawing.Size, drawing.OriginalEnvelope.Clone(), false, aoi);
        }

        public static int[] GetDefaultBands(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            int[] defaultBands = prd.GetDefaultBands();
            if (defaultBands == null)
                defaultBands = new int[] { 1, 2, 3 };
            return defaultBands;
        }
    }
}
