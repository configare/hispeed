using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.Project;
using GeoDo.RSS.UI.AddIn.CanvasViewer;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class AVILayerDisplay : IAVILayerDisplay
    {
        private ILayoutHost _host = null;
        private bool _isCustom = false;
        private string _extInfos = null;
        private DateTime[] _dateTimes = null;

        public bool IsCustom
        {
            set { _isCustom = value; }
        }

        public void DisplayAvi(ISmartSession session, string wndName, string[] fnames, string templateName,
                               string subIdentify, out string outputFname)
        {
            string satellite = null;
            string sensor = null;
            outputFname = GetOutputGxdFileName(fnames, subIdentify, out satellite, out sensor);
            bool isOk = false;
            FileIsSupportable(fnames);
            ILayoutTemplate temp = GetTemplateByArg(templateName);
            if (temp == null)
                return;
            ILayoutViewer viewer = new LayoutViewer(wndName);
            ApplyLayoutTemplate(viewer, ref temp); //在这里应用模板，初始化数据框的dataProvider
            TryApplyVars(temp, subIdentify, satellite, sensor);
            if (viewer.LayoutHost == null)
                return;
            ICanvas canvas = GetCanvasByTemplate(temp);
            if (canvas == null)
                return;
            Size dataSize = Size.Empty;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewPrjEvp = null;
            GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp = null;
            float resolution = 0;
            int maxSize = GetBorderSizeByTemplate(viewer.LayoutHost.LayoutRuntime, temp);
            IAVILayer aviLyr = AVILayerBuilder.CreatAVILayer(fnames, canvas, maxSize, out dataSize, out viewPrjEvp, out viewGeoEvp, out resolution);
            if (aviLyr == null)
                return;
            aviLyr.OnTicked += new EventHandler(RefreshDataFrame);
            if (_isCustom)
                TryApplyCustom(canvas, dataSize, subIdentify, viewPrjEvp, viewGeoEvp, resolution);
            isOk = TryAddAviLayerToCanvas(canvas, aviLyr, viewPrjEvp);
            if (isOk)
            {
                session.SmartWindowManager.DisplayWindow(viewer);
                _host.ToSuitedSize(_host.LayoutRuntime.Layout);
                TryExportToGIF(session, subIdentify, outputFname);
            }
        }

        private void TryExportToGIF(ISmartSession session, string subIdentify, string outputFname)
        {
            ICommand cmd = session.CommandEnvironment.Get(6038);
            if (cmd == null)
                return;
            if (string.IsNullOrEmpty(outputFname))
                return;
            cmd.Execute(outputFname);
        }

        private string GetOutputGxdFileName(string[] dataFileNames, string outIdentify, out string satellite, out string sensor)
        {
            RasterIdentify rstIdentify = new RasterIdentify(dataFileNames);
            _dateTimes = rstIdentify.ObritTiems;
            satellite = rstIdentify.Satellite;
            sensor = rstIdentify.Sensor;
            rstIdentify.SubProductIdentify = outIdentify;
            rstIdentify.ExtInfos = _extInfos;
            return rstIdentify.ToWksFullFileName(".gif");
        }

        private bool TryAddAviLayerToCanvas(ICanvas canvas, IAVILayer aviLyr, GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewPrjEvp)
        {
            canvas.LayerContainer.Layers.Add(aviLyr);
            canvas.CurrentEnvelope = viewPrjEvp;
            canvas.Refresh(enumRefreshType.All);
            return true;
        }

        private void TryApplyCustom(ICanvas canvas, Size size, string subIdentify,
                                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewPrjEvp,
                                    GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp, float resolution)
        {
            if (canvas == null)
                return;
            if (string.IsNullOrEmpty(subIdentify))
                return;
            if (subIdentify != "CMED")
                return;
            string name = null;
            int[] aoi = null;
            GetAOIArgument(size, viewGeoEvp, resolution, out name, out aoi);
            if (name == null || aoi == null || aoi.Length == 0)
                return;
            _extInfos = "_";
            _extInfos = _extInfos + name;
            ILayer lry = canvas.LayerContainer.GetByName("蒙板层");
            if (lry == null)
            {
                lry = new MaskLayer();
                canvas.LayerContainer.Layers.Add(lry);
            }
            IMaskLayer mask = lry as IMaskLayer;
            mask.Update(Color.White, size, viewPrjEvp, false, aoi);
        }

        private void GetAOIArgument(Size maxSize, GeoDo.RSS.Core.DF.CoordEnvelope viewGeoEvp, float resolution, out string name, out int[] aoi)
        {
            Dictionary<string, int[]> aoiDic = null;
            name = null;
            aoi = null;
            if (viewGeoEvp == null)
                return;
            using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(maxSize, viewGeoEvp, resolution))
            {
                frm.listView1.MultiSelect = false;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    aoiDic = frm.GetFeatureAOIIndex();
                    if (aoiDic == null)
                        return;
                    foreach (string key in aoiDic.Keys)
                    {
                        name = key;
                        aoi = aoiDic[key];
                        return;
                    }
                }
            }
        }

        private int _index = 0;
        private string _preDateString = "{OrbitDateTime}";
        private void RefreshDataFrame(object sender, EventArgs e)
        {
            if (_dateTimes == null)
                return;
            //vars.Add("{OrbitDateTime}", dateTime.ToLongDateString() + dateTime.ToString(" HH:mm") + " (北京时)");
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (_dateTimes != null && _dateTimes.Length > _index && _dateTimes[_index] != DateTime.MinValue)
                vars.Add(_preDateString, _dateTimes[_index].AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
            else
                vars.Add(_preDateString, "");
            if (vars.Count != 0 && _host.Template != null)
            {
                _host.Template.ApplyVars(vars);
                _preDateString = _dateTimes[_index].AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
                if (_index < _dateTimes.Length - 1)
                    _index++;
                else if (_index == _dateTimes.Length - 1)
                    _index = 0;
            }
            _host.Render(true);
        }

        private void ApplyLayoutTemplate(ILayoutViewer viewer, ref ILayoutTemplate temp)
        {
            _host = viewer.LayoutHost;
            _host.ApplyTemplate(temp);
        }

        private void TryApplyVars(ILayoutTemplate temp, string subIdentify, string satellite, string sensor)
        {
            if (temp == null)
                return;
            Dictionary<string, string> vars = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(satellite))
            {
                string sate = satellite.ToUpper();
                if (sate.Contains("FY3"))
                    sate = sate.Replace("FY3", "FY-3");
                vars.Add("{Satellite}", sate);
            }
            if (!string.IsNullOrEmpty(sensor))
                vars.Add("{Sensor}", sensor);
            if (_dateTimes != null && _dateTimes.Length > _index && _dateTimes[_index] != DateTime.MinValue)
            {
                vars.Add("{OrbitDateTime}", _dateTimes[_index].AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)");
                _preDateString = _dateTimes[_index].AddHours(8).ToString("yyyy年MM月dd日 HH:mm") + " (北京时)";
            }else
                vars.Add(_preDateString, "");
            temp.ApplyVars(vars);
        }

        private ICanvas GetCanvasByTemplate(ILayoutTemplate temp)
        {
            if (temp == null)
                return null;
            IDataFrame df = FindDataFrameByTemplate(ref temp);
            if (df == null)
                return null;
            if (df.Provider == null)
                return null;
            if (!(df.Provider is IDataFrameDataProvider))
                return null;
            return (df.Provider as IDataFrameDataProvider).Canvas;
        }

        private IDataFrame FindDataFrameByTemplate(ref ILayoutTemplate temp)
        {
            if (temp == null)
                return null;
            ILayout layout = temp.Layout;
            if (layout == null)
                return null;
            IElement[] eles = layout.QueryElements((df) => df is IDataFrame);
            if (eles == null || eles.Length == 0)
                return null;
            return GetDataFrame(eles[0] as IDataFrame);
        }

        private IDataFrame GetDataFrame(IDataFrame df)
        {
            if (df == null)
                return null;
            if (df.Provider != null)
            {
                ICanvas canvas = (df.Provider as IDataFrameDataProvider).Canvas;
                if (canvas == null)
                    return null;
                if (canvas.LayerContainer.VectorHost == null)
                {
                    IVectorHostLayer vectorH = new VectorHostLayer(null);
                    canvas.LayerContainer.Layers.Add(vectorH as ILayer);
                }
            }
            return df;
        }

        private int GetBorderSizeByTemplate(ILayoutRuntime runtime, ILayoutTemplate temp)
        {
            if (temp == null)
                return 0;
            ILayout layout = temp.Layout;
            if (layout == null)
                return 0;
            List<IElement> eles = layout.Elements;
            if (layout.GetBorder() == null)
                return 0;
            SizeF size = layout.GetBorder().Size;
            if (size == SizeF.Empty)
                return 0;
            float w = size.Width;
            float h = size.Height;
            runtime.Layout2Pixel(ref w, ref h);
            return Math.Max((int)w, (int)h);
        }

        private ILayoutTemplate GetTemplateByArg(string templateName)
        {
            string[] parts = templateName.Split(':');
            if (parts == null || parts.Length < 2)
                return null;
            templateName = parts[1];
            ILayoutTemplate t = LayoutTemplate.FindTemplate(templateName);
            if (t == null)
                t = LayoutTemplate.FindTemplate("缺省二值图模版");
            return t;
        }

        //是否为支持的文件类型
        private void FileIsSupportable(string[] fnames)
        {
            if (fnames == null || fnames.Length == 0)
                throw new ArgumentException("请选择需要生成动画的栅格文件。");
            FileTypeIsSupport(fnames);
            FileSpatialReferenceSupport(fnames);
        }

        private void FileTypeIsSupport(string[] fnames)
        {
            foreach (string fname in fnames)
            {
                if (!File.Exists(fname))
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”不存在。");
                if (!OpenFileFactory.SupportOpenFile(fname))
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(fname) + "”类型不正确，无法生成动画。");
            }
        }

        private bool FileSpatialReferenceSupport(string[] fnames)
        {
            ISpatialReference sr;
            using (IRasterDataProvider prd = GeoDataDriver.Open(fnames[0]) as IRasterDataProvider)
                sr = prd.SpatialRef;
            for (int i = 1; i < fnames.Length; i++)
            {
                using (IRasterDataProvider prd = GeoDataDriver.Open(fnames[i]) as IRasterDataProvider)
                {
                    if (sr == null)
                    {
                        if (prd.SpatialRef != null)
                            throw new ArgumentException("选择的文件空间参考类型不同，无法生成动画。");
                    }
                    if (prd.SpatialRef == null)
                        throw new ArgumentException("选择的文件空间参考类型不同，无法生成动画。");
                    if (!sr.IsSame(prd.SpatialRef))
                        throw new ArgumentException("选择的文件空间参考不同，无法生成动画。");
                }
            }
            return true;
        }
    }
}
