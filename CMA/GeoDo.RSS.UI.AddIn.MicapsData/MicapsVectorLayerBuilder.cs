#region Version Info
/*========================================================================
* 功能概述：Micaps数据矢量层创建类
* 
* 创建者：DongW     时间：2013/9/5 14:09:04
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;
using CodeCell.AgileMap.Core;
using System.IO;
using GeoDo.RSS.DF.MicapsData;
using System.Xml.Linq;

namespace GeoDo.RSS.UI.AddIn.MicapsData
{
    /// <summary>
    /// 类名：MicapsVectorLayerBuilder
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/5 14:09:04
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public static class MicapsVectorLayerBuilder
    {
        public static CodeCell.AgileMap.Core.IFeatureLayer CreateAndLoadVectorLayerForMicaps(ISmartSession session, ICanvas canvas, string fname, string dataTypeId, params object[] args)
        {
            if (string.IsNullOrEmpty(fname) || !File.Exists(fname))
                return null;
            using (IVectorFeatureDataReader reader = MicapsDataReaderFactory.GetVectorFeatureDataReader(fname, dataTypeId))
            {
                if (reader == null)
                    return null;
                if (reader.Features != null)
                {
                    MemoryDataSource mds = new MemoryDataSource(fname, reader.ShapeType, enumCoordinateType.Geographic, reader.Envelope, reader.Fields);
                    IFeatureClass fetClass = new FeatureClass(mds);
                    mds.AddFeatures(reader.Features);
                    CodeCell.AgileMap.Core.IFeatureLayer fetLayer = new FeatureLayer(fname, fetClass);
                    TryApplyStyle(fetLayer, dataTypeId);
                    IVectorHostLayer host = canvas.LayerContainer.VectorHost as IVectorHostLayer;
                    if (host != null)
                    {
                       host.Set(canvas);
                       IMapRuntime mapRuntime = host.MapRuntime as IMapRuntime;
                       if (mapRuntime != null)
                       {
                           IMap map = mapRuntime.Map as IMap;
                           if (map != null)
                           {
                               map.LayerContainer.Append(fetLayer);
                               FeatureLayer fetL = map.LayerContainer.Layers[0] as FeatureLayer;
                               FeatureClass fetC = fetL.Class as FeatureClass;
                               Envelope evp = fetC.FullEnvelope.Clone() as Envelope;
                               CoordEnvelope cvEvp = new CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
                               canvas.CurrentEnvelope = cvEvp;
                               canvas.Refresh(enumRefreshType.All);
                           }
                       }
                       return fetLayer;
                    }
                }
            }
            return null;
        }

        private static void TryApplyStyle(CodeCell.AgileMap.Core.IFeatureLayer fetLayer, string dataTypeId)
        {
            //根据文件类型应用显示方案
            string[] mcdfnames = Directory.GetFiles(Path.GetDirectoryName(MicapsFileProcessor.DATATYPE_CONFIG_DIR), "*.mcd");
            //应用默认方案
            if (mcdfnames == null || mcdfnames.Length < 1)
            {
                SimpleMarkerSymbol sym = new SimpleMarkerSymbol(masSimpleMarkerStyle.Circle);
                sym.Size = new System.Drawing.Size(3, 3);
                fetLayer.Renderer = new SimpleFeatureRenderer(sym);
                return;
            }
            else
            {
                string fname = null;
                foreach (string item in mcdfnames)
                {
                    if(dataTypeId==Path.GetFileNameWithoutExtension(item))
                    {
                        fname = item;
                        break;
                    }
                }
                if (fname != null)
                {
                    XDocument doc = XDocument.Load(fname);
                    var result = doc.Element("Map").Element("Layers").Elements();
                    if (result == null)
                        return;
                    LabelDef labelDef = null;
                    IFeatureRenderer featureRender = null;
                    GetFeatureRenderFromMcd(doc, out labelDef, out featureRender);
                    if (labelDef != null)
                        fetLayer.LabelDef = labelDef;
                    if (featureRender != null)
                        fetLayer.Renderer = featureRender;
                }
            } 
        }

        private static void GetFeatureRenderFromMcd(XDocument doc, out LabelDef labelDef, out  IFeatureRenderer featureRender)
        {
            labelDef = null;
            featureRender = null;
            try
            {
                var result = doc.Element("Map").Element("Layers");
                if (result == null)
                    return;
                var layerXmls = result.Elements("Layer");
                if (layerXmls == null || layerXmls.Count() == 0)
                    return;
                var layerXml = layerXmls.First();

                var labelXmls = layerXml.Elements("LabelDef");
                if (labelXmls != null && labelXmls.Count() != 0)
                {
                    XElement labelXml = labelXmls.First();
                    labelDef = LabelDef.FromXElement(labelXml);
                }

                var renderXmls = layerXml.Elements("Renderer");
                if (renderXmls != null && renderXmls.Count() != 0)
                {
                    XElement renderXml = renderXmls.First();
                    XAttribute renderTypeAtt = renderXml.Attribute("type");
                    if (renderTypeAtt != null)
                    {
                        string renderType = renderTypeAtt.Value;
                        if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.SimpleFeatureRenderer")
                            featureRender = SimpleFeatureRenderer.FromXElement(renderXml);
                        else if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.SimpleTwoStepFeatureRenderer")
                            featureRender = SimpleTwoStepFeatureRenderer.FromXElement(renderXml);
                        else if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.UniqueFeatureRenderer")
                            featureRender = UniqueFeatureRenderer.FromXElement(renderXml);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
