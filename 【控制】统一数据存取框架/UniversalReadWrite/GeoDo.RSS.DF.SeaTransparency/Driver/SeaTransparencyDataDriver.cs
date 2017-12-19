#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/22 10:55:14
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
using GeoDo.RSS.Core.DF;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.DF.SeaTransparency
{
    /// <summary>
    /// 类名：SeaTransparencyDataDriver
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/22 10:55:14
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class SeaTransparencyDataDriver : GeoDataDriver
    {
        public SeaTransparencyDataDriver()
            : base()
        {
            _fullName = "SEATRANSPARENCY";
            _name = "SeaTransparency Data Driver";
        }

        public SeaTransparencyDataDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            return new SeaTransparencyDataProvider(fileName, this);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            return new SeaTransparencyDataProvider(fileName, this);
        }

        /// <summary>
        /// 判断依据：
        /// 1、文件名符合“globalzvdbjmonth1.txt”、“zvdbjmonth1.txt”模式
        /// 2、文件为1200（中国）或8640（全球）列
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="header1024"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @"(GLOBAL)?ZVDBJMONTH(\d|\d[10-12]).TXT");
            if (match.Success)
            {
                return IsCompatibleColumnCount(fileName);
            }
            return false;
        }

        private bool IsCompatibleColumnCount(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
            {
                string firstLine = sr.ReadLine();
                string[] columns = firstLine.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
                return (columns.Length == 1200 || columns.Length == 8640);
            }
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
