#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-18 08:28:41
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
using System.IO;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL.HDF4Universal;
using System.Text.RegularExpressions;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：SubProductDataProcess
    /// 属性描述：MODLST 数据预处理，无效值就是0.
    /// 创建者：lxj   创建日期：2014-6-18 08:28:41
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductDataProcess:CmaMonitoringSubProduct
    {
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private static Regex[] DataReg = new Regex[]
        {
            new Regex (@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled),
            new Regex (@"A(?<year>\d{4})(?<day>\d{3})", RegexOptions.Compiled),
        };

        public SubProductDataProcess(SubProductDef subProductDef)
            : base(subProductDef)
        { 

        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "MODLSTAlgorithm")
            {
                return MODLSTAlgorithm(progressTracker);
            }
            if (_curArguments.GetArg("AlgorithmName").ToString() == "CLDExtractAlgorithm")
            {
                return CLDExtractAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult MODLSTAlgorithm(Action<int, string> progressTracker)
        {
            ValArguments args = _argumentProvider.GetArg("DataArgs") as ValArguments;
            if (args == null)
                return null;
            string[] files = Directory.GetFiles(args.InputDir, "MOD11A1*.hdf", SearchOption.AllDirectories);//args.FileNamesForVal;
            float minX = Convert.ToSingle(args.MinX);
            float maxX = Convert.ToSingle(args.MaxX);
            float minY = Convert.ToSingle(args.MinY);
            float maxY = Convert.ToSingle(args.MaxY);
            float res = Convert.ToSingle(args.OutRes);
            string outdir = args.Outdir;
            PrjEnvelope env = new PrjEnvelope(minX, maxX, minY, maxY);
            string regionNam = args.RegionNam;
            UniversalDataProcess modLST = new UniversalDataProcess();
            foreach (string file in files)
            {
                string prjfile = modLST.PrjMODSIN(file, outdir, env, res, regionNam, progressTracker);//投影
                //提取出文件日期编名
                string dateString = "0000000";
                foreach (Regex dt in DataReg)
                {
                    Match m = dt.Match(Path.GetFileName(prjfile));
                    if (m.Success)
                        dateString = m.Value;
                }
                if (dateString.Length == 8 && !dateString.Contains("A"))
                {
                    int year = Int16.Parse(dateString.Substring(0, 4));
                    int month = Int16.Parse(dateString.Substring(4, 2));
                    int day = Int16.Parse(dateString.Substring(6, 2));
                    DateTime datetime = new DateTime(year, month, day);
                    string days = Convert.ToString(datetime.DayOfYear);
                    dateString = Convert.ToString(year) + days;
                    dateString = "A" + dateString;
                }
                string outfile = outdir + "\\" + "MOD11A1." + dateString + "." + regionNam + ".join.ldf"; //形如MOD11A1.A2013231.China.join.ldf
                modLST.MODJoin(prjfile,outdir,res,regionNam,outfile); 
            }
            return null;
        }
        private IExtractResult CLDExtractAlgorithm(Action<int, string> progressTracker)
        {
            string info = _argumentProvider.GetArg("DataSets") as string;
            if (String.IsNullOrWhiteSpace(info))
            {
                MessageBox.Show("没有选择数据集");
                return null;
            }
            ValArguments args = _argumentProvider.GetArg("DataArgs") as ValArguments;
            if (args == null)
                return null;
            string[] files = Directory.GetFiles(args.InputDir, "FY3*L2*.hdf", SearchOption.AllDirectories);
            float minX = Convert.ToSingle(args.MinX);
            float maxX = Convert.ToSingle(args.MaxX);
            float minY = Convert.ToSingle(args.MinY);
            float maxY = Convert.ToSingle(args.MaxY);
            float res = Convert.ToSingle(args.OutRes);
            string outdir = args.Outdir;
            string regionNam = args.RegionNam;
            PrjEnvelope env = new PrjEnvelope(minX, maxX, minY, maxY);
            UniversalDataProcess cldGlobal = new UniversalDataProcess();
            foreach (string file in files)
            {
                cldGlobal.ExtractFile(file,info,env,regionNam,outdir);
            }
            return null;
        }
        
    }
}
