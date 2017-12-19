#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 9:00:42
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
using System.IO;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.DF.SeaWave
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class SeaWaveDriver : GeoDataDriver
    {
        public SeaWaveDriver()
            : base()
        {
            _fullName = "SEA_EAVE";
            _name = "SEA_EAVE";
        }

        public SeaWaveDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new SeaWaveDataProvider(fileName, this);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new SeaWaveDataProvider(fileName, this);
        }

        /// <summary>
        /// 判断依据：
        /// 1、文件名符合“2013081303y.dat”模式
        /// 2、文件第一行包含文件名包含的日期
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="header1024"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @"(?<date>\d{10}(?<xy>\S)).DAT");
            return match.Success;
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
