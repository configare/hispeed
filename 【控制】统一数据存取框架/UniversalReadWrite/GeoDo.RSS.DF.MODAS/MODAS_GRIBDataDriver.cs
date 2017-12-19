#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:54:58
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


namespace GeoDo.RSS.DF.MODAS
{
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class MODAS_GRIBDataDriver : GeoDataDriver
    {
        public MODAS_GRIBDataDriver()
            : base()
        {
            _fullName = "MODAS";
            _name = "MODAS";
        }

        public MODAS_GRIBDataDriver(string name, string fullName)
            : base(name, fullName)
        {
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            string featureName, deepValue;
            TryExtractDeepValueArg(args, out deepValue, out featureName);
            return new MODAS_GRIBDataProvider(fileName, this, deepValue, featureName);
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName, enumDataProviderAccess access, params object[] args)
        {
            string featureName, deepValue;
            TryExtractDeepValueArg(args, out deepValue, out featureName);
            return new MODAS_GRIBDataProvider(fileName, this, deepValue, featureName);
        }

        private string TryExtractDeepValueArg(object[] args, out string deepValue, out string featureName)
        {
            deepValue = null;
            featureName = null;
            if (args == null || args.Length == 0)
                return null;
            string[] ps = args[0].ToString().ToUpper().Split(new char[] { ',' });
            foreach (string p in ps)
            {
                string[] parts = p.Split('=');
                if (parts.Length < 2)
                    continue;
                switch (parts[0])
                {
                    case "DEEPVALUE":
                        deepValue = parts[1];
                        break;
                    case "FEATURENAME":
                        featureName = parts[1];
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断依据：
        /// 1、文件名符合“ra20121229.dat”模式
        /// 2、文件第一行包含文件名包含的日期
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="header1024"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @"RA(?<date>\d{8}).DAT");
            if (match.Success)
            {
                return FirstRowContainDate(fileName, match.Groups["date"].Value);
            }
            return false;
        }

        private bool FirstRowContainDate(string fileName, string date)
        {
            using (StreamReader sr = new StreamReader(fileName, Encoding.ASCII))
            {
                string firstLine = sr.ReadLine();
                firstLine = firstLine.Replace(" ", "");
                return firstLine.Contains(date);
            }
        }

        public override void Delete(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
