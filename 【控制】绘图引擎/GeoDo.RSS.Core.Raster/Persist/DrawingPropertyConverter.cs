using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Core;
using GeoDo.RSS.Core.DrawEngine;
using System.Xml.Linq;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class DrawingPropertyConverter:PropertyConverter
    {
        protected override object CreateAndFillObject(System.Xml.Linq.XElement propertyXml)
        {
            //    public RasterDrawing(string fname, ICanvas canvas,
            //            IRgbStretcherProvider stretcerProvider, params string[] options)
            string fname = AttToString(propertyXml, "filename");
            ICanvas canvas = Object2Xml.PersistContextEnv.Get("Canvas") as ICanvas;
            int[] bandNos = GetSelectedBandNos(propertyXml);
            if(bandNos != null && bandNos.Length >0)
                return new RasterDrawing(fname, canvas, bandNos,null);
            else
                return new RasterDrawing(fname, canvas, null);
        }

        private int[] GetSelectedBandNos(XElement propertyXml)
        {
            XElement bandsElement = propertyXml.Element("SelectedBandNos");
            if (bandsElement == null)
                return null;
            string bands = bandsElement.Value;
            if (string.IsNullOrEmpty(bands))
                return null;
            string[] parts = bands.Split(',');
            int[] bandNos = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                bandNos[i] = int.Parse(parts[i]);
            return bandNos;
        }

        protected override void SetAttributes(System.Xml.Linq.XElement ele, object propertyValue)
        {
            IRasterDrawing drawing = propertyValue as IRasterDrawing;
            ele.SetAttributeValue("filename", drawing.FileName);
            XElement bandsElement = new XElement("SelectedBandNos");
            bandsElement.SetValue(GetSelectedBandNosString(drawing));
            ele.Add(bandsElement);
        }

        private unsafe object GetSelectedBandNosString(IRasterDrawing drawing)
        {
            string bandsString = string.Empty;
            int[] bandNos = drawing.SelectedBandNos;
            if (bandNos != null && bandNos.Length > 0)
            {
                foreach (int bNo in bandNos)
                    bandsString += (bNo.ToString() + ",");
                bandsString = bandsString.Substring(0, bandsString.Length - 1);
            }
            return bandsString;
        }
    }
}
