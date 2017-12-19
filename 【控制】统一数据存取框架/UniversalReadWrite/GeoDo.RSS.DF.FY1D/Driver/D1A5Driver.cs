#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/27 10:59:46
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

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：D1A5Driver
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/27 10:59:46
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class D1A5Driver : GDALRasterDriver, I1A5Driver
    {
        public D1A5Driver()
            : base()
        {
            _name = "FY1D_1A5";
            _fullName = "FY1D 1A5 Data Driver";
        }

        public D1A5Driver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new FY1_1A5DataProvider(fileName, this, access);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new FY1_1A5DataProvider(fileName, null, this, access);
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fileExtension = Path.GetExtension(fileName).ToUpper();
            return D1A5Header.Is1A5(header1024, fileExtension);
        }
    }
}
