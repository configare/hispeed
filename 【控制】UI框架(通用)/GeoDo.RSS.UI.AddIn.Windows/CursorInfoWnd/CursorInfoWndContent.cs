using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class CursorInfoWndContent : UserControl,
        IToolWindowContent, IPixelInfoSubscriber,
        ICursorInfoDisplayer
    {
        private ISmartSession _session = null;
        private ICanvasViewer _canvasViewer = null;
        private delegate void HandleNofityHandler(PixelInfo pixelInfo);
        private HandleNofityHandler _notifyHandler = null;
        private const string ITEM_EXTAND_INFO = "    {0} : {1}";
        private const string ITEM_SCREEN_COORD = "    屏幕坐标 : {0} , {1}";
        private const string ITEM_RASTER_COORD = "    栅格坐标 : {0} , {1}";
        private const string ITEM_PRJ_COORD = "    投影坐标 : {0} , {1}";
        private const string ITEM_GEO_COORD = "    地理坐标 : {0} , {1}";
        private const string ITEM_RGB_BANDS3 = "    RGB合成 : {0} , {1} , {2}";
        private const string ITEM_RGB_BANDS1 = "    单通道 : {0}";
        private const string ITEM_RGB_VALUES3 = "    RGB值 : {0} , {1} , {2}";
        private const string ITEM_DATA_VALUES3 = "    通道值 : {0} , {1} , {2}";
        private const string ITEM_RGB_VALUES1 = "    灰度值 : {0}";
        private const string ITEM_DATA_VALUES1 = "    原始值 : {0}";
        private const string ITEM_BAND_VALUE = "    通道 {0} : {1}";
        private const string ITEM_BAND_VALUE_WITHNAME = "    通道 {0} : {1} ({2})";
        private StringBuilder _sb = new StringBuilder();
        private IRasterDictionaryTemplate<byte> _landTypeDictionary = null;
        private IRasterDictionaryTemplate<int> _xianJieDictionary = null;
        private double[] bandValues = null;
        private List<ICursorInfoProvider> _cursorInfoProviders = new List<ICursorInfoProvider>();

        public CursorInfoWndContent()
        {
            InitializeComponent();
            _notifyHandler = new HandleNofityHandler(PrintPixelInfo);
            txtInfo.Text = string.Empty;
            Load += new EventHandler(CursorInfoWndContent_Load);
        }

        void CursorInfoWndContent_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;
        }

        private void LoadSecondaryInfos()
        {
            _xianJieDictionary = RasterDictionaryTemplateFactory.CreateXjRasterTemplate();
            _landTypeDictionary = RasterDictionaryTemplateFactory.CreateLandRasterTemplate();
        }

        public void Free()
        {
            if (_canvasViewer != null && _canvasViewer.Canvas != null && _canvasViewer.Canvas.PixelInfoSubscribers != null)
            {
                _canvasViewer.Canvas.PixelInfoSubscribers.Remove(this);
                _canvasViewer = null;
            }
            if (_xianJieDictionary != null)
            {
                _xianJieDictionary.Dispose();
                _xianJieDictionary = null;
                _landTypeDictionary.Dispose();
                _landTypeDictionary = null;
            }
            if (_cursorInfoProviders != null)
            {
                _cursorInfoProviders.Clear();
            }
        }

        public void Apply(Core.UI.ISmartSession session)
        {
            _session = session;
            ICanvasViewer v = session.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (v == null || v.ActiveObject == null)
            {
                txtInfo.Text = string.Empty;
                return;
            }
            bandValues = new double[(v.ActiveObject as IRasterDrawing).BandCount];
            _canvasViewer = v;
            if (!v.Canvas.PixelInfoSubscribers.Contains(this))
                v.Canvas.PixelInfoSubscribers.Add(this);
        }

        public void Notify(PixelInfo pixelInfo)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(_notifyHandler, pixelInfo);
            else
                _notifyHandler(pixelInfo);
        }

        private unsafe void PrintPixelInfo(PixelInfo pixelInfo)
        {
            _sb.Clear();
            //
            ICanvasViewer v = _session.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (v == null)
                return;
            IRasterDrawing drawing = v.ActiveObject as IRasterDrawing;
            int bandCount = drawing.BandCount;
            if (txtOriginChannels.Checked || txtSelectChannels.Checked)
            {
                fixed (double* ptr = bandValues)
                {
                    //Stopwatch sw1 = new Stopwatch();
                    //sw1.Start();
                    drawing.ReadPixelValues(pixelInfo.RasterX, pixelInfo.RasterY, ptr);
                    //sw1.Stop();
                    //Console.WriteLine("read data:" +sw1.ElapsedMilliseconds.ToString());
                }
            }
            int[] selectedBandNos = drawing.SelectedBandNos;
            //
            _sb.AppendLine("-------------------------------");
            //
            if (txtSecondaryInfo.Checked)
            {
                _sb.AppendLine("辅助信息");
                if (drawing.DataProviderCopy.CoordType != enumCoordType.Raster)
                {
                    if (_landTypeDictionary != null)
                        _sb.AppendLine(string.Format(ITEM_EXTAND_INFO, "土地利用类型", _landTypeDictionary.GetPixelName(pixelInfo.GeoX, pixelInfo.GeoY)));
                    if (_xianJieDictionary != null)
                        _sb.AppendLine(string.Format(ITEM_EXTAND_INFO, "行政区划", _xianJieDictionary.GetPixelName(pixelInfo.GeoX, pixelInfo.GeoY)));
                }
            }
            //
            if (txtCoordInfo.Checked)
            {
                _sb.AppendLine("坐标信息");
                _sb.AppendLine(string.Format(ITEM_SCREEN_COORD, pixelInfo.ScreenX, pixelInfo.ScreenY));
                _sb.AppendLine(string.Format(ITEM_RASTER_COORD, pixelInfo.RasterX, pixelInfo.RasterY));
                _sb.AppendLine(string.Format(ITEM_PRJ_COORD, pixelInfo.PrjX.ToString("0.##"), pixelInfo.PrjY.ToString("0.##")));
                if (rd10DecimalDegree.Checked)
                    _sb.AppendLine(string.Format(ITEM_GEO_COORD, pixelInfo.GeoX.ToString("0.####"), pixelInfo.GeoY.ToString("0.####")));
                else
                {
                    _sb.AppendLine(string.Format(ITEM_GEO_COORD, DegreeToString(pixelInfo.GeoX), DegreeToString(pixelInfo.GeoY)));
                }
            }
            //
            if (txtSelectChannels.Checked)
            {
                Color rgb = drawing.GetColorAt(pixelInfo.ScreenX, pixelInfo.ScreenY);
                _sb.AppendLine("显示通道");
                if (drawing.SelectedBandNos.Length == 1)
                {
                    _sb.AppendLine(string.Format(ITEM_RGB_BANDS1, selectedBandNos[0]));
                    _sb.AppendLine(string.Format(ITEM_RGB_VALUES1, rgb.R));
                    _sb.AppendLine(string.Format(ITEM_DATA_VALUES1, bandValues[selectedBandNos[0] - 1]));
                }
                else
                {
                    _sb.AppendLine(string.Format(ITEM_RGB_BANDS3, selectedBandNos[0], selectedBandNos[1], selectedBandNos[2]));
                    _sb.AppendLine(string.Format(ITEM_RGB_VALUES3, rgb.R, rgb.G, rgb.B));
                    _sb.AppendLine(string.Format(ITEM_DATA_VALUES3, bandValues[selectedBandNos[0] - 1], bandValues[selectedBandNos[1] - 1], bandValues[selectedBandNos[2] - 1]));
                }
            }
            //
            if (txtOriginChannels.Checked)
            {
                _sb.AppendLine("原始通道值");
                for (int i = 1; i <= bandCount; i++)
                {
                    string bandDesc = drawing.DataProviderCopy.GetRasterBand(i).Description;
                    if (string.IsNullOrEmpty(bandDesc))
                    {
                        _sb.AppendLine(string.Format(ITEM_BAND_VALUE, i, bandValues[i - 1]));
                    }
                    else
                    {
                        _sb.AppendLine(string.Format(ITEM_BAND_VALUE_WITHNAME, i, bandValues[i - 1], bandDesc));
                    }
                }
            }
            //判识面积
            string extractingArea = TryGetExtractingArea();
            string tempStr = _sb.ToString() + extractingArea + TryGetExtInfoFromInfoProvider(pixelInfo);
            //面板显示参数
            string argInfos = TryGetExtractingArgInfos();
            txtInfo.Text = tempStr + argInfos;
        }

        private string TryGetExtractingArgInfos()
        {
            try
            {
                MonitoringSession mss = (_session.MonitoringSession as MonitoringSession);
                if (mss != null && mss.CurrentCanvasViewer != null && mss.CurrentCanvasViewer.ActiveObject != null)
                {
                    IExtractingSession ess = mss.ExtractingSession;
                    if (ess != null && mss.ActiveMonitoringProduct != null && mss.ActiveMonitoringSubProduct != null
                        && ess.CurrentSubProduct != null && ess.CurrentSubProduct.ArgumentProvider != null
                        && ess.CurrentSubProduct.ArgumentProvider.ArgNames != null)
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendLine("参数信息");
                        foreach (string argname in ess.CurrentSubProduct.ArgumentProvider.ArgNames)
                        {
                            if (argname.IndexOf("CursorInfo:") != -1)
                                str.AppendLine("    " + argname.Replace("CursorInfo:", "") + " : " + ess.CurrentSubProduct.ArgumentProvider.GetArg(argname));
                        }
                        return str.ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private string TryGetExtractingArea()
        {
            try
            {
                MonitoringSession mss = (_session.MonitoringSession as MonitoringSession);
                if (mss != null && mss.CurrentCanvasViewer != null && mss.CurrentCanvasViewer.ActiveObject != null)
                {
                    IExtractingSession ess = mss.ExtractingSession;
                    if (ess != null && mss.ActiveMonitoringProduct != null && mss.ActiveMonitoringSubProduct != null)
                    {
                        IPixelIndexMapper extResult = ess.GetBinaryValuesMapper(mss.ActiveMonitoringProduct.Identify, mss.ActiveMonitoringSubProduct.Identify);
                        if (extResult != null)
                        {
                            string areaString = "0";
                            long count = extResult.Indexes.LongCount();
                            double area = 0;
                            if (count != 0)
                            {
                                IRasterDataProvider raster = (mss.CurrentCanvasViewer.ActiveObject as RasterDrawing).DataProvider;
                                if (raster != null)
                                {
                                    float resolutionX = raster.ResolutionX;
                                    float resolutionY = raster.ResolutionY;
                                    double piexArea = 1;
                                    if (raster.SpatialRef == null || (raster.SpatialRef != null && raster.SpatialRef.ProjectionCoordSystem == null))
                                        piexArea = MIF.Core.AreaCountHelper.CalcArea(extResult.CoordEnvelope.Center.X, extResult.CoordEnvelope.Center.Y, resolutionX, resolutionY);
                                    else
                                        piexArea = resolutionX * resolutionY;
                                    area = count * piexArea;//m²
                                    if (area > 1000000 * 100)
                                    {
                                        areaString = (area / 1000000).ToString("f2") + "（KM²）";//m²->km²
                                    }
                                    if (area > 1000000)
                                    {
                                        areaString = (area / 1000000).ToString("f4") + "（KM²）";//m²->km²
                                    }
                                    else
                                    {
                                        areaString = (int)area + "（M²）";
                                    }
                                }
                            }
                            StringBuilder str = new StringBuilder();
                            str.AppendLine("判识信息");
                            str.AppendLine("    像元个数 : " + count);
                            str.AppendLine("    判识面积 : " + areaString);
                            return str.ToString();
                        }
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private string TryGetExtInfoFromInfoProvider(PixelInfo pixelInfo)
        {
            if (_cursorInfoProviders == null || _cursorInfoProviders.Count == 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (ICursorInfoProvider prd in _cursorInfoProviders)
            {
                sb.AppendLine((prd.Name ?? string.Empty) + ":");
                sb.AppendLine(prd.GetInfo(pixelInfo.RasterY, pixelInfo.RasterX) ?? string.Empty);
            }
            return "-------------------------------\n" + sb.ToString();
        }

        private void txtSecondaryInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (txtSecondaryInfo.Checked)
            {
                LoadSecondaryInfos();
            }
            else
            {
                _xianJieDictionary.Dispose();
                _xianJieDictionary = null;
                _landTypeDictionary.Dispose();
                _landTypeDictionary = null;
                GC.Collect();
            }
        }

        void ICursorInfoDisplayer.RegisterProvider(ICursorInfoProvider infoProvider)
        {
            if (infoProvider == null || _cursorInfoProviders.Contains(infoProvider))
                return;
            _cursorInfoProviders.Add(infoProvider);
        }

        void ICursorInfoDisplayer.UnregisterProvider(ICursorInfoProvider infoProvider)
        {
            if (infoProvider == null || !_cursorInfoProviders.Contains(infoProvider))
                return;
            _cursorInfoProviders.Remove(infoProvider);
        }

        public static string DegreeToString(double degree)
        {
            if (degree > 1000 || double.IsNaN(degree) || double.IsInfinity(degree))
                return degree.ToString();
            degree = Math.Abs(degree);
            int d = Math.Abs((int)Math.Floor(degree));
            int m = Math.Abs((int)((degree - d) * 60));
            if (m > 60)
                m = 60;
            int s = Math.Abs((int)((degree * 3600 - d * 3600 - m * 60)));
            return d.ToString().PadLeft(3, ' ') + "°" + m.ToString().PadLeft(2, ' ') + "′" + s.ToString("#.##").PadLeft(4, ' ') + "″";
        }
    }
}
