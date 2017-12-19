#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Zhangyb     时间：2014-1-7 09:23:20 
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
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using System.IO;
using GeoDo.HDF5;

namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    /// <summary>
    /// 类名：IceConDriver
    /// 属性描述：实现对MWRI的L2级产品极区海冰覆盖度数据的支持
    /// 创建者：Zhangyb   创建日期：2014-1-7 09:23:20 
    /// 修改者：             修改日期：2014-1-15 15:55:00
    /// 修改描述：优化实现逻辑，删除不必要的函数
    /// 备注：
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class IceConDriver : RasterDataDriver, IFY3BIceConDriver
    {
        public IceConDriver()
            : base()
        {
            _name = "FY3B_IceCon";
            _fullName = "FY3B IceCon Data Driver";
        }

        public IceConDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            return IsCompatible(fileName, header1024);
        }

        public static bool IsCompatible(string fileName, byte[] header1024)
        {
            if (header1024 ==null)
            {
                header1024 =new byte[1024];
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(header1024, 0, 1024);
                    fs.Close();
                }
            } 
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            Hdf5Operator hdfic = new Hdf5Operator(fileName);
            string[] HDFdatasets = hdfic.GetDatasetNames;
            string[] alldatasets = new string[6] { "icecon_north_asc", "icecon_north_avg", "icecon_north_des", "icecon_south_asc", "icecon_south_avg", "icecon_south_des" };
            foreach (object set in alldatasets)
            {
                if (!HDFdatasets.Contains(set))//数据集必须完全匹配，才能用此数据集
                    return false;
            }
            return false;
        }


        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new IceConDataProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new IceConDataProvider(fileName, null, this, args);
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
