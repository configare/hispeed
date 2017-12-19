using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Windows.Forms;
using GeoDo.RSS.Layout;
using GeoDo.RSS.RasterTools;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class SubProduct0IMGCLD : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;
        private string _SVDLRModeNo = null;
        private double _ratio = 1;

        public SubProduct0IMGCLD(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
            AOITemplateStat<float> aoiTempStat = new AOITemplateStat<float>();
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
            {
                PrintInfo("参数配置不正确！");
                return null;
            }
            IRasterDataProvider prd = _argumentProvider.DataProvider as IRasterDataProvider;
            string fname =prd.fileName;
            if (fname.Contains("ISCCP_D2")&& Path.GetExtension(fname).ToUpper()==".LDF")
            {
                //PrintInfo("目前仅支持ISCCP_D2格式数据！");
                //return null;
                if (fname.Contains("MCA"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "TCAM");
                }
                else if (fname.Contains("MTAU"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "TAUM");
                }
                else if (fname.Contains("MPC"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "0PCM");
                }
                else if (fname.Contains("MTC"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "0TCM");
                }
                else if (fname.Contains("MWP"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "0WPM");
                }
                _argumentProvider.SetArg("SelectedPrimaryFiles", fname);
            }
            else if (fname.Contains("SVD") && fname.Contains("模态"))
            {
                _argumentProvider.SetArg("OutFileIdentify", "MSVD");
                _SVDLRModeNo = Path.GetFileNameWithoutExtension(fname).Split('_')[1];
                string tempf = CreateNewColorTable(prd, fname,progressTracker);
                _argumentProvider.SetArg("SelectedPrimaryFiles", tempf);                
            }

            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string outId = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (outId != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(outId);
                if (instance != null)
                {
                    return ThemeGraphyResult(null);
                }
                PrintInfo("指定的子产品\"" + outId + "\"的instance没有实现。");
                return null;
            }
            PrintInfo("指定的子产品\"" + outId + "\"没有实现。");
            return null;
        }


        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (instanceIdentify == "MSVD")
            {
                ILayout layout = template.Layout;
                Dictionary<string, string> vars = new Dictionary<string, string>();
                if (_SVDLRModeNo==null)
                    vars.Add("{SVDLRModeNo}", "请输入左右场模态数");
                else
                    vars.Add("{SVDLRModeNo}", _SVDLRModeNo);
                vars.Add("{ExggrationRatio}", "放大系数:" + _ratio.ToString("f2"));
                template.ApplyVars(vars);
            }
        }

        private string  CreateNewColorTable(IRasterDataProvider prd,string fname, Action<int, string> progressTracker)
        {
            double minValue,maxValue;
            prd.GetRasterBand(1).ComputeMinMax(out minValue,out  maxValue, false, progressTracker);
            IMaxMinValueComputer computer = MaxMinValueComputerFactory.GetMaxMinValueComputer(prd.DataType);
            IRasterBand[] bands = new IRasterBand[1] { prd.GetRasterBand(1) };
            double[] minValues = new double[1];
            double[] maxValues = new double[1];
            double[] meanValues = new double[1];
            computer.Compute(bands, null, out minValues, out maxValues, out meanValues, progressTracker);
            double pRatio = 1 / Math.Abs(maxValues[0]);
            double mRatio = 1 /  Math.Abs(minValues[0]);
            if (maxValues[0]>=0&&minValues[0]<=0)
            {
                _ratio = pRatio > mRatio ? mRatio : pRatio;
                int width =bands[0].Width;
                int height =bands[0].Height;
                double[] oldband = new double[width * height];
                unsafe
                {
                    fixed (Double* ptr = oldband)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        bands[0].Read(0, 0, width, height, bufferPtr, enumDataType.Double, width, height);
                    }
                }
                double[] newband = new double[width * height];
                for (int i = 0; i < newband.Length;i++ )
                {
                    newband[i] = oldband[i] * _ratio;
                }
                //string name = Path.ChangeExtension(fname,".ldf");
                string tempf = Path.GetFileNameWithoutExtension(fname)+ string.Format("_{0}_{1}.ldf", _ratio.ToString("F2"), DateTime.Now.ToString("yyyyMMddHHmm"));
                tempf = Path.Combine(Path.GetDirectoryName(fname), tempf);
                if (File.Exists(tempf))
                    File.Delete(tempf);
                using(IRasterDataProvider tempraster = CreateRaster(tempf, prd,fname))
                {
                    IRasterBand newband1 = tempraster.GetRasterBand(1);
                    unsafe
                    {
                        fixed (Double* ptr = newband)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            newband1.Write(0, 0, width, height, bufferPtr, enumDataType.Double, width, height);
                        }
                    }
                }
                return tempf;
            }
            return null;
        }

        public static IRasterDataProvider CreateRaster(string outFileName,IRasterDataProvider referProvider,string fname)
        {
            int width = referProvider.Width;
            int height = referProvider.Height;
            Project.ISpatialReference spatialRef = referProvider.SpatialRef;
            enumDataType datatype = referProvider.DataType;
            List<string> options = new List<string>();
            options.Add("INTERLEAVE=BSQ");
            options.Add("VERSION=LDF");
            options.Add("WITHHDR=TRUE");
            options.Add("SPATIALREF=" + spatialRef.ToProj4String());
            options.Add("MAPINFO={" + 1 + "," + 1 + "}:{" + referProvider.CoordEnvelope.MinX + "," + referProvider.CoordEnvelope.MaxY + "}:{" + referProvider.ResolutionX + "," + referProvider.ResolutionY+ "}"); //=env.ToMapInfoString(new Size(width, height));
            string hdrfile = HdrFile.GetHdrFileName(Path.ChangeExtension(fname,".hdr"));
            if (!string.IsNullOrWhiteSpace(hdrfile) && File.Exists(hdrfile))
            {
                HdrFile hdr = HdrFile.LoadFrom(hdrfile);
                if (hdr != null && hdr.BandNames != null)
                    options.Add("BANDNAMES=" + string.Join(",", hdr.BandNames));
            }
            if(!Directory.Exists(Path.GetDirectoryName(outFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            }
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, datatype, options.ToArray()) as RasterDataProvider;
            return outRaster;
        }

    }
}
