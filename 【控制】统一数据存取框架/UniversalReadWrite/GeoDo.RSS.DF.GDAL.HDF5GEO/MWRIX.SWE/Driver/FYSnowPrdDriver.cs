#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-1-15 14:41:13
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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using System.IO;
using GeoDo.HDF;
using GeoDo.HDF5;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    /// <summary>
    /// 类名：FYSnowPrdDriver
    /// 属性描述：
    /// 创建者：LiXJ   创建日期：2014-1-15 14:41:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
   [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
   public class FYSnowPrdDriver: RasterDataDriver,IFYSnowPrdDriver
    {
        public FYSnowPrdDriver()
            :base()
       {
          _name = "FY3SnowPrd";
          _fullName = "FY3 Snow Product";
       }
        public FYSnowPrdDriver(string name, string fullName)
            : base(name, fullName)
        {

        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            return IsCompatible(fileName, header1024);
        }

        public static bool IsCompatible(string fileName, byte[] header1024)
        {
            if (header1024 == null)
            {
                header1024 = new byte[1024];
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(header1024, 0, 1024);
                    fs.Close();
                }
            }
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            Hdf5Operator oper = new Hdf5Operator(fileName);
            string[] datasets = oper.GetDatasetNames;
            string[] alldatasets = new string[16] { "SD_Flags_NorthernDaily_A", "SD_Flags_NorthernDaily_D", "SD_Flags_SouthernDaily_A", "SD_Flags_SouthernDaily_D", 
                          "SD_NorthernDaily_A", "SD_NorthernDaily_D" ,"SD_SouthernDaily_A","SD_SouthernDaily_D",
                           "SWE_Flags_NorthernDaily_A", "SWE_Flags_NorthernDaily_D", "SWE_Flags_SouthernDaily_A", "SWE_Flags_SouthernDaily_D", 
                          "SWE_NorthernDaily_A", "SWE_NorthernDaily_D" ,"SWE_SouthernDaily_A","SWE_SouthernDaily_D"};
            foreach (object set in alldatasets)
            {
                if (!datasets.Contains(set))
                    return false;
            }
            return false;
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new FYSnowPrdDataProvider(fileName, header1024, this, args);
        }
        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new FYSnowPrdDataProvider(fileName, null, this, args);
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

    }
}
