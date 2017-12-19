#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-08-16 15:38:44
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
using System.IO;
using GeoDo.HDF5;
using OSGeo.GDAL;

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 类名：HDF5Driver
    /// 属性描述：专用于读取hdf5的通用读取驱动
    /// 创建者：luozhanke   创建日期：2013-08-16 15:38:44
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class HDF5Driver : RasterDataDriver
    {
        public HDF5Driver()
            :base()
        {
            Gdal.AllRegister();
            _name = "HDF5";
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            if (args == null || args.Length==0)
                return false;
            string ext = Path.GetExtension(fileName).ToUpper();
            if (!HDF5Helper.IsHdf5(header1024))
                return false;
            return true;
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new HdfRasterDataProvider(fileName, header1024, this, args);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new HdfRasterDataProvider(fileName, null, this, args);
        }
    }
}
