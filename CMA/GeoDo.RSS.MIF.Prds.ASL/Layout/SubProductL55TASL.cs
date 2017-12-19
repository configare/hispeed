using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
//using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ASL
{
    /// <summary>
    /// 类名：SubProductU5TTASL
    /// 属性描述：
    /// 创建者：zhangyb   创建日期：2013/11/22 15:10:12
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：0.55微米通道的气溶胶光学厚度（无单位），U5TT
    /// </summary>
    public class SubProductU5TTASL : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        string _055TauFname = "";
        //IRasterDataProvider _prd = null;
        //IBandProvider _brd = null;

        public SubProductU5TTASL(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            IRasterDataProvider prd = _argumentProvider.DataProvider as IRasterDataProvider;
            if (!prd.fileName.Contains("MOD04"))
            {
                return null;
            }
            GenDatFile(prd);
            _argumentProvider.SetArg("SelectedPrimaryFiles",_055TauFname);
            _argumentProvider.SetArg("OutFileIdentify", "U5TT");

            return TauL55Algorithm();
        }

        private IExtractResult TauL55Algorithm()
        {
            IFileExtractResult U5TTResult=null;
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (string.IsNullOrWhiteSpace(instanceIdentify))
                return null;
            SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
            if (instance != null)
            {
                U5TTResult = ThemeGraphyResult(null) as IFileExtractResult;
                IExtractResultArray array = new ExtractResultArray("气溶胶产品");
                IFileExtractResult U5TIResult = new FileExtractResult("U5TI", _055TauFname, true);
                U5TIResult.SetDispaly(false);
                array.Add(U5TTResult);
                array.Add(U5TIResult);
                return array;
            }
            return null;
        }

        private string GenDatFilename(string imgFilename)
        {
            RasterIdentify id = new RasterIdentify(imgFilename);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "ASL";
            id.SubProductIdentify = "U5TI";
            id.IsOutput2WorkspaceDir = true;
            return id.ToWksFullFileName(".dat");
        }

        private void GenDatFile(IRasterDataProvider prd)
        {
            string name = prd.fileName;
            _055TauFname = GenDatFilename(name);
            ReadAndWriteOneBandRaw(prd, _055TauFname);
        }

        private unsafe void ReadAndWriteOneBandRaw(IRasterDataProvider dataPrd, string outfName)
        {
            int bandNO = 4;
            IRasterBand bands = dataPrd.GetRasterBand(bandNO);
            if (bands != null)
            {
                int height = dataPrd.Height;
                int width = dataPrd.Width;
                CoordEnvelope envelope = dataPrd.CoordEnvelope;
                IRasterDataProvider outdataPrd = null;
                try
                {
                    IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                    string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                    outdataPrd = driver.Create(outfName, width, height, 1, dataPrd.DataType, mapInfo);
                    float[] buffer = new float[dataPrd.Width * dataPrd.Height];
                    fixed (float* ptr = buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        bands.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, dataPrd.DataType, dataPrd.Width, dataPrd.Height);
                        outdataPrd.GetRasterBand(1).Write(0, 0, width, height, bufferPtr, dataPrd.DataType, width, height);
                    }                
                    string hdrFile = Path.ChangeExtension(outfName, ".hdr");
                    SaveHdrFile(hdrFile, width, height, dataPrd.DataType);
                }
                catch (System.Exception ex)
                {
                	
                }
                finally
                {
                    if (dataPrd != null)
                        dataPrd.Dispose();
                    if (outdataPrd != null)
                        outdataPrd.Dispose();
                }     
            }
        }

        private void SaveHdrFile(string hdrFile, int width, int height, enumDataType dataType)
        {
            int dt = 0;
            if (dataType == enumDataType.Float)
            {
                dt = 4;
            }
            using (StreamWriter sw = new StreamWriter(hdrFile, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {File Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", width));
                sw.WriteLine(string.Format("lines = {0}", height));
                sw.WriteLine("bands = 1");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", dt));
                sw.WriteLine("interleave = bip");
                sw.WriteLine("byte order = 0");
            }
        }



    }
}
