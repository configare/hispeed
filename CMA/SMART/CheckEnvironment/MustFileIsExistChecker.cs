using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.AppEnv
{
    public class MustFileIsExistChecker : IQueueTaskItem,IStartChecker
    {
        private string[] _mustFilenames = null;
        private object _data = null;
        private bool _isOK = true;

        public MustFileIsExistChecker()
        {
            _mustFilenames = new string[] 
                                        {
                                            //BEGIN MEF
                                            "GeoDo.MEF.dll",
                                            //END MEF
                                            //BEGIN GDAL
                                            "gdal19.dll",
                                            "gdal_wrap.dll",
                                            "gdalconst_wrap.dll",
                                            "hd426m.dll",
                                            "hdf5dll.dll",
                                            "hm426m.dll",
                                            "msvcp100.dll",
                                            "msvcr100.dll",
                                            "ogr_wrap.dll",     
                                            "osr_wrap.dll",           
                                            "proj.dll",                                         
                                            "szip.dll",                                    
                                            "zlib1.dll",
                                            "zlib.dll",
                                            //--------------BEGIN GDAL CSHARP
                                            "gdal_csharp.dll",
                                            "gdalconst_csharp.dll",
                                            "ogr_csharp.dll",
                                            "osr_csharp.dll",
                                            //--------------END GDAL CSHARP
                                            //BEGIN UNIVERSALREADWRITE
                                            "HDF5DotNet.dll",  
                                            "LoadOrder.xml",
                                            "GeoDo.RSS.Core.DF.dll",
                                            "GeoDo.Oribit.Algorithm.dll",
                                            "GeoDo.RSS.DF.GDAL.dll",
                                            "GeoDo.RSS.DF.GDAL.H5BandPrd.dll",
                                            "GeoDo.RSS.DF.GDAL.H4BandPrd.dll",
                                            "GeoDo.RSS.DF.GDAL.H5BandPrd.xml",
                                            "GeoDo.RSS.DF.GDAL.H4BandPrd.xml",
                                            "GeoDo.HDF.dll",
                                            "GeoDo.HDF5.dll",
                                            "GeoDo.HDF4.dll",
                                            "GeoDo.RSS.DF.NOAA.dll",
                                            "GeoDo.RSS.DF.NOAA.BandPrd.dll",
                                            "GeoDo.Rss.DF.NOAA.BandNames.xml",
                                            "GeoDo.RSS.DF.MVG.dll",
                                            "GeoDo.RSS.DF.LDF.dll",
                                            "GeoDo.RSS.DF.MEM.dll",
                                            //END UNIVERSALREADWRITE
                                            //BEGIN 临近点查找
                                            "ANN.dll",
                                            "AnnSearch.dll",
                                            //END 临近点查找
                                            //BEGIN AVI
                                            "GeoDo.RSS.VideoMark.dll",
                                            //END AVI
                                            //BEGIN MIF
                                            "GeoDo.RSS.MIF.Core.dll",
                                            //"GeoDo.RSS.MIF.Prds.BAG.dll",
                                            //"GeoDo.RSS.MIF.Prds.DRT.dll",
                                            //"GeoDo.RSS.MIF.Prds.DST.dll",
                                            //"GeoDo.RSS.MIF.Prds.FIR.dll",
                                            //"GeoDo.RSS.MIF.Prds.FLD.dll",
                                            //"GeoDo.RSS.MIF.Prds.FOG.dll",
                                            //"GeoDo.RSS.MIF.Prds.ICE.dll",
                                            //"GeoDo.RSS.MIF.Prds.LST.dll",
                                            //"GeoDo.RSS.MIF.Prds.SNW.dll",
                                            //"GeoDo.RSS.MIF.Prds.UHI.dll",
                                            //"GeoDo.RSS.MIF.Prds.VGT.dll",
                                            "GeoDo.RSS.MIF.UI.dll",
                                            //END MIF
                                            //BEGIN PROJECT
                                            "GeoDo.Project.Cnfg.xml",
                                            "GeoDo.Project.dll",
                                            "GeoDo.FileProject.dll",
                                            "GeoDo.ProjectDefine.dll",
                                            "GeoDo.Radiation.dll",
                                            "GeoDo.RasterProject.dll",
                                            //END PROJECT
                                            //BEGIN IMAGE PROCESS
                                            "GeoDo.RSS.CA.dll",
                                            "GeoDo.RSS.Core.CA.dll",
                                            //END IMAGE PROCESS
                                            //BEGIN BLOCKOPER
                                            "GeoDo.RSS.BlockOper.dll",
                                            //END BLOCKMOPER
                                            //BEGIN DRAWENGINE
                                            "GeoDo.RSS.Core.DrawEngine.dll",
                                            "GeoDo.RSS.Core.DrawEngine.GDIPlus.dll", 
                                            "GeoDo.RSS.Core.DrawEngine.GDIPlus.cnfg.xml",
                                            "GeoDo.RSS.Core.RasterDrawing.dll",
                                            "GeoDo.RSS.Core.VectorDrawing.dll",
                                            "GeoDo.RSS.Core.View.dll",
                                            "GeoDo.RSS.Core.Grid.dll",
                                            //END DRAWENGINE
                                            //BEGIN PYTHONENGINE
                                            "IronPython.dll",
                                            "IronPython.Modules.dll",
                                            "IronPython.Modules.xml",
                                            "Microsoft.Dynamic.dll",
                                            "Microsoft.Scripting.dll",
                                            "Microsoft.Scripting.Metadata.dll",
                                            "FastColoredTextBox.dll",
                                            "FastColoredTextBox.xml",
                                            "GeoDo.PythonEngine.dll",
                                            //END PYTHONENGINE
                                            //BEGIN GEOVIS
                                            "gdal14.dll",
                                            "GeoLImage.dll",
                                            "ICSharpCode.SharpZipLib.dll",
                                            "ImageIODll.dll",
                                            "zlib1d.dll",
                                            //END GEOVIS
                                            //BEGIN AGILEMAP
                                            "CodeCell.Bricks.dll",
                                            "CodeCell.AgileMap.Core.dll",
                                            "CodeCell.AgileMap.Components.dll",
                                            //END AGILEMAP
                                            //BEGIN LAYOUT
                                            "GeoDo.RSS.Layout.dll",
                                            "GeoDo.RSS.Layout.GDIPlus.dll",
                                            "GeoDo.RSS.Layout.Elements.dll",
                                            "GeoDo.RSS.Layout.DataFrm.dll",
                                            //END LAYOUT
                                            //BEGIN TELERIK
                                            "Telerik.WinControls.dll",
                                            "Telerik.WinControls.UI.dll",
                                            "TelerikCommon.dll",
                                            "Telerik.WinControls.RadDock.dll",
                                            //END TELERIK
                                            //BEGIN WINEXCEL
                                            "Interop.Excel.dll",
                                            "Interop.Office.dll",
                                            "Interop.VBIDE.dll",
                                            "Interop.Word.dll",
                                            //END WINEXCEL
                                            //BEGIN UI
                                            "DefaultUI.xml",
                                            //@"SystemData\CatalogCN.xml",
                                            @"SystemData\DataStretchers.xml",
                                            @"SystemData\MefConfig.xml",
                                            @"数据引用\基础矢量\SimpleMap.mcd",
                                            "GeoVisSDK.dll",
                                            "GeoDo.RSS.Core.UI.dll",
                                            "GeoDo.RSS.UI.AddIn.AddInMgr.dll",
                                            "GeoDo.RSS.UI.AddIn.CanvasViewer.dll",
                                            "GeoDo.RSS.UI.AddIn.CMA.dll",
                                            "GeoDo.RSS.UI.AddIn.DataPro.dll",
                                            "GeoDo.RSS.UI.AddIn.DataRef.dll",
                                            "GeoDo.RSS.UI.AddIn.GeoVIS.dll",
                                            "GeoDo.RSS.UI.AddIn.ImgPro.dll",
                                            "GeoDo.RSS.UI.AddIn.Layout.dll",
                                            "GeoDo.RSS.UI.AddIn.Office.dll",
                                            "GeoDo.RSS.UI.AddIn.Python.dll",
                                            "GeoDo.RSS.UI.AddIn.Theme.dll",
                                            "GeoDo.RSS.UI.AddIn.Windows.dll",
                                            "GeoDo.RSS.UI.Bricks.dll",
                                            "GeoDo.RSS.UI.WinForm.dll",
                                            "GeoDo.RSS.UI.WinForm.Resource.dll",
                                            //END UI
                                            //BEGIN Help
                                            "GeoDo.RSS.UI.AddIn.Help.dll",
                                            //END Help
                                            //BEGIN BAG
                                            //"BAGConfig.xml"
                                            //END BAG
                                        };
        }

        #region IQueueTaskItem 成员

        public string Name
        {
            get { return "检查必要的文件是否存在?"; }
        }

        public void Do(IProgressTracker tracker)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory ;
            List<string> lostFilenames = new List<string>();
            string tipFrt = "正在检查文件\"{0}\"......";
            foreach (string file in _mustFilenames)
            {
                tracker.Tracking(string.Format(tipFrt,file));
                try
                {
                    if (!File.Exists(dir + file))
                        lostFilenames.Add(file);
                }
                catch
                {
                    lostFilenames.Add(file);
                }
            }
            if (lostFilenames.Count > 0)
            {
                _isOK = false;
                _data = lostFilenames.ToArray();
            }
        }

        #endregion

        #region IStartChecker 成员

        public bool IsOK
        {
            get { return _isOK; }
        }

        public bool IsCanContinue
        {
            get { return _isOK; }
        }

        public object Data
        {
            get { return _data ; }
        }

        public string Message
        {
            get { return "应用程序应用必须的文件丢失,应用程序将终止启动。"; }
        }

        public Exception Exception
        {
            get 
            {
                string str = "丢失的文件列表:\r\n";
                string[] fs = _data as string[];
                foreach(string f in fs)
                    str+=("  "+f+"\r\n");
                return new Exception(str);
            }
        }

        #endregion
    }
}
