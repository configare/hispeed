#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/20 10:53:34
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

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 类名：NA141BDriver
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/20 10:53:34
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class NA141BDriver : GDALRasterDriver, INA141BDriver
    {
        public NA141BDriver()
            : base()
        {
            _name = "NOAA14_1B";
            _fullName = "NOAA14 1B Data Driver";
        }

        public NA141BDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, Core.DF.enumDataProviderAccess access, params object[] args)
        {
            return new NA141BDataProvider(fileName, header1024, this, access);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new NA141BDataProvider(fileName, this, access);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            return NA141BHeader.Is1B(header1024);
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

    }
}
