/* 封装对HDF4数据读取
 * http://www.hdfgroup.org/release4/doc/UsrGuide_html/UG_Intro.html
 * 通用光栅图像（GR API）
 * 科学数据集（SD API）
 * 虚拟数据Vdata（VS API）
 * 注解Annotation
 * 虚拟组合Vgroup（V API）
 * File Naming Convention
YYYYDDDHHMMSS_NNNNN_CS_2B-TAU_GRANULE_S_RVV_EVV.hdf
YYYYDDDHHMMSS =	Year, Julian day, hour, minute, second of the first data contained in the file (UTC)
NNNNN =	Granule number
CS =	Literal "CS" indicating the CloudSat mission
2B-TAU =	Product name
GRANULE =	Indicates the data subset ("GRANULE" if the data are not subset)
S =	Data Set identifier ("P" for production release)
RVV =	Release number
EVV =	Epoch number
.hdf =	HDF-EOS file suffix
2007101034511_05065_CS_1B-CPR_GRANULE_P_R04_E02.hdf
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using System.Diagnostics;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class CloudsatDataProvider : RasterDataProvider
    {
        private const string FileNamePattern = @"(?<YYYY>\d{4})(?<DDD>\d{3})(?<HH>\d{2})(?<MM>\d{2})(?<SS>\d{2})_(?<Granulenumber>\d+)_CS_(?<product>[^_]+)_(?<GRANULE>GRANULE)_(?<P>\w+)_R(?<R>\d*)_E(?<E>\d*).hdf";
        private static Regex FileNameReg = new Regex(FileNamePattern, RegexOptions.Compiled);
        private DateTime _fileNameDateTime;
        private string _fileNameProductName = "";
        private Dictionary<string, string> _fileMateData = null;
        private string _filename;
        private Raster[] _allRasters = null;
        private Table[] _allTables = null;
        private H4File file = null;
        private string[] SDSNames = null;

        public CloudsatDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
            : base(fileName, driver)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _filename = fileName;
            TryParseFileName(fileName, out _fileNameDateTime, out _fileNameProductName);
            _fileMateData = _attributes.CreateAttributeDomain("FileAttributes");
            _fileMateData.Add("DateTime", _fileNameDateTime.ToString("yyyyMMddHHmmss"));
            _fileMateData.Add("ProductName", _fileNameProductName);
            BuilderRastersAndTables();
            _dataType = enumDataType.Int16;
            _bandCount = _rasterBands.Count;
            _width = _rasterBands[0].Width;
            _height = _rasterBands[0].Height;
            _coordEnvelope = new CoordEnvelope(new CoordPoint(0, 0), _width, _height);
            _coordTransform = CoordTransoformFactory.GetCoordTransform(_spatialRef, null, _width, _height);
            TrySetEnvelopeAndResolutions();
            sw.Stop();
            long em = sw.ElapsedMilliseconds;
            Console.WriteLine(em + "毫秒");
        }

        private void BuilderRastersAndTables()
        {
            Dictionary<string, string> dss = new Dictionary<string, string>();
            dss.Add("1B-CPR", "ReceivedEchoPowers");//这个没有height
            dss.Add("2B-CLDCLASS", "cloud_scenario");
            dss.Add("2B-CWC-RO", "RO_liq_effective_radius,RO_ice_phase_fraction,RO_radar_uncertainty,LO_RO_AP_geo_mean_radius,LO_RO_AP_sdev_geo_mean_radius,");
            dss.Add("2B-CWC-RVOD", "");
            dss.Add("2B-FLXHR", "");
            dss.Add("2B-GEOPROF", "Radar_Reflectivity");
            dss.Add("2B-GEOPROF-LIDAR", "CloudFraction,UncertaintyCF");
            dss.Add("2B-TAU", "layer_optical_depth");
            dss.Add("2C-PRECIP-COLUMN", "");
            dss.Add("ECMWF-AUX", "Pressure,Temperature,Specific_humidity,Ozone");
            dss.Add("MODIS-AUX", "");
            if (dss.ContainsKey(_fileNameProductName))
            {
                file = new H4File(null, null, null, new long[] { 0, 0 });
                file.Load(_filename);
                string[] vas = dss[_fileNameProductName].Split(',');
                if (vas == null || vas.Length == 0)
                    return;
                H4SDS[] sds = file.Datasets;
                for (int i = 0; i < sds.Length; i++)
                {
                    if (vas.Contains(sds[i].Name))
                    {
                        CloudSatRasterBand rasterBand = new CloudSatRasterBand(this, sds[i], i + 1);
                        _rasterBands.Add(rasterBand);
                        break;
                    }
                }
            }
        }

        public static bool TryParseFileName(string fileName, out DateTime datetime, out string productName)
        {
            if (fileName == null)
            {
                datetime = DateTime.MinValue;
                productName = "";
                return false;
            }
            Match match = FileNameReg.Match(fileName);
            if (match.Success)
            {
                int yyyy, ddd, hh, mm, ss;
                int.TryParse(match.Groups["YYYY"].Value, out yyyy);
                int.TryParse(match.Groups["DDD"].Value, out ddd);
                int.TryParse(match.Groups["HH"].Value, out hh);
                int.TryParse(match.Groups["MM"].Value, out mm);
                int.TryParse(match.Groups["SS"].Value, out ss);
                datetime = new DateTime(yyyy, 1, 1, hh, mm, ss);
                datetime = datetime.AddDays(ddd - 1);
                productName = match.Groups["product"].Value;
                return true;
            }
            else
            {
                datetime = DateTime.MinValue;
                productName = "";
                return false;
            }
        }

        public static bool IsSupport(string filename, byte[] header1024)
        {
            bool status = false;
            if (header1024 != null)
                status = HDF4Helper.IsHdf4(header1024);
            else
                status = HDF4Helper.IsHdf4(filename);
            if (status)
            {
                DateTime dt;
                string ptname;
                status = TryParseFileName(filename, out dt, out ptname);
            }
            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vdataName"></param>
        /// <param name="field">field为空则读取整个字段</param>
        /// <param name="nStart">0开始</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public object ReadVdata(string vdataName, string field, int nStart, int count)
        {
            if (file == null)
                return null;
            H4Vdata[] vdatas = file.VDatas;
            foreach (H4Vdata vdata in vdatas)
            {
                if (vdata.Vdata_name == vdataName)
                {
                    if (string.IsNullOrWhiteSpace(field))
                        field = string.Join(",", vdata.Fields);
                    return vdata.Read(nStart, count, field);
                }
            }
            return null;
        }

        public H4Vdata FindVdata(string vdataName)
        {
            if (file == null)
                return null;
            H4Vdata[] vdatas = file.VDatas;
            foreach (H4Vdata vdata in vdatas)
            {
                if (vdata.Vdata_name == vdataName)
                {
                    return vdata;
                }
            }
            return null;
        }

        public bool ReadSDS(string name, int[] start, int[] count, IntPtr ptr)
        {
            if (file == null)
                return false;
            H4SDS[] sds = file.Datasets;
            for (int i = 0; i < sds.Length; i++)
            {
                if (sds[i].Name == name)
                {
                    sds[i].Read(start, null, count, ptr);
                    return true;
                }
            }
            return false;
        }

        public object ReadAttribute(string name)
        { 
            HDFAttribute[] attrs = file.GlobalAttrs;
            for (int i = 3; i < attrs.Length; i++)
            {
                if (attrs[i].Name == name)
                {
                    return attrs[i].Value;
                }
            }
            return null;
        }

        public Raster[] GetAllRaster
        {
            get
            {
                if (_allRasters == null)
                { }
                return _allRasters;
            }
        }

        public Table[] GetAllTable
        {
            get
            {
                if (_allTables == null)
                { }
                return _allTables;
            }
        }

        #region test
        public unsafe void Test()
        {
            //TestVData();
            TestHelper();
        }

        private void TestHelper()
        {
            int major_v;
            int minor_v;
            int release;
            StringBuilder libraryversion = new StringBuilder();
            HDF4API.Hgetlibversion(out major_v, out minor_v, out release, libraryversion);
            string fullfilename = @"E:\Smart\CloudArgs\cloudsat\2007101034511_05065_CS_2B-GEOPROF_GRANULE_P_R04_E02.hdf";
            int file_id = HDF4API.Hopen(fullfilename, DFACC.DFACC_READ, 0);
            HDF4API.Hgetfileversion(file_id, out major_v, out minor_v, out release, libraryversion);

            H4File hdf = new H4File(null, null, null, null);
            hdf.Load(fullfilename);
            //测试读取的全局属性
            for (int i = 0; i < hdf.Num_Global_Attrs; i++)
            {
                dynamic attValue = hdf.GlobalAttrs[i].Value;
            }
            //测试读取的科学数据集及其属性
            for (int i = 0; i < hdf.Num_Datasets; i++)
            {
                H4SDS sd = hdf.Datasets[i];
                HDFAttribute[] attrs = sd.SDAttributes;
                if (sd.Rank == 2)
                {
                    int buffersize = (int)sd.Dimsizes[0] * sd.Dimsizes[1];
                    int typesize = HDFDataType.GetSize(sd.Datatype);
                    IntPtr ptr = Marshal.AllocHGlobal(buffersize * typesize);
                    sd.Read(new int[] { 0, 0 }, null, sd.Dimsizes, ptr);
                    short[] buffer = new short[buffersize];
                    Marshal.Copy(ptr, buffer, 0, buffersize);
                    Marshal.FreeHGlobal(ptr);
                }
            }
            //测试读取的Vdata

        }

        public unsafe void TestVData()
        {
            StringBuilder vdata_name = new StringBuilder();
            StringBuilder vdata_class = new StringBuilder();
            StringBuilder fields;
            int file_id, vdata_id, istat;
            int n_records, interlace, vdata_size, vdata_ref;
            string filename = @"E:\Smart\CloudArgs\cloudsat\2007101034511_05065_CS_2B-GEOPROF_GRANULE_P_R04_E02.hdf";
            file_id = HDF4API.Hopen(filename, DFACC.DFACC_READ, 0);
            istat = HDF4API.Vstart(file_id);
            vdata_ref = -1;
            vdata_ref = HDF4API.VSgetid(file_id, vdata_ref);
            vdata_id = HDF4API.VSattach(file_id, vdata_ref, "r");
            fields = new StringBuilder(60);
            istat = HDF4API.VSinquire(vdata_id, out n_records, out interlace, fields, out vdata_size, vdata_name);
            istat = HDF4API.VSgetclass(vdata_id, vdata_class);
            istat = HDF4API.VSsetfields(vdata_id, fields.ToString());

            //fixed (float* bp = databuf)
            //{
            //    IntPtr ptr = (IntPtr)bp;
            //    istat = HDF4API.VSread(vdata_id, ptr, n_records, INTERLACE_MODE.FULL_INTERLACE);
            //}
            IntPtr ptr = Marshal.AllocHGlobal(n_records * vdata_size * 8);
            //GCHandle handle = GCHandle.Alloc(databuf);
            istat = HDF4API.VSread(vdata_id, ptr, n_records, INTERLACE_MODE.FULL_INTERLACE);
            //handle.Free();
            float[] databuf = new float[vdata_size];
            Marshal.Copy(ptr, databuf, 0, vdata_size);
            Marshal.FreeHGlobal(ptr);

            istat = HDF4API.VSdetach(vdata_id);
            istat = HDF4API.Vend(file_id);
            istat = HDF4API.Hclose(file_id);
            Console.WriteLine(istat);
        }
        #endregion

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override object GetStretcher(int bandNo)
        {
            return base.GetStretcher(bandNo);
        }

        public override int[] GetDefaultBands()
        {
            return new int[] { 1, 1, 1 };
        }

        public override void Dispose()
        {
            if (file != null)
                file.Dispose();
            base.Dispose();
        }
    }
}
