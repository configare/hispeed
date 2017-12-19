#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-03-25 14:06:37
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
using System.Xml.Linq;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoDo.Tools;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 类名：CLDParaPeriodicCompute
    /// 属性描述：云参数周期统计计算最大值、最小值、平均值
    /// 创建者：张延冰   创建日期：2014-03-25 14:06:37
    /// 修改者：           修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
  public class PeriodicComputeAlg
    {
      private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
      private static string _periodPrdsArgsXml = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\CLD\PeriodPrdsArgs.xml";
      private string _inputDataPath;
      private string _satellite="AQUA",_sensor = "AIRS",_dataOrigin ="AIRS",_tableName = "CP_PeriodicSynthesis_TB";
      ConnectMySqlCloud _con;
      InputArg _args;
      private static string[] _statics =null;
      private static bool _overwritefiles = false;
      PeriodicSynPrds2Base _periodicsyn;
      private static Regex DayDateReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
      public int _singleyear,_regionID;//某一年
      public long _prdID, _datasetID;
      public  string[] _validsets = null;//数据集数组
      public string _periodPrdsArgs=null;//YEAR/MONTH
      Dictionary<string, List<string>> monthFiles = new Dictionary<string, List<string>>();


      //标记每个日产品的日期，方便后续处理
      public struct DayFileWithDate
      {
          public string path;
          public DateTime date;

          //主要用于linq查询出特定产品的文件后，去掉日期信息，再转换回来
          public static string[] GetFiles(DayFileWithDate[] array)
          {
              string[] files = new string[array.Length];
              for (int i = 0; i < array.Length; ++i)
              {
                  files[i] = array[i].path;
              }
              return files;
          }
      }

      public PeriodicComputeAlg(string sensor, InputArg arg)
      {
          //_sensor = sensor;
          _dataOrigin = sensor;
          _args = arg;
          if (_dataOrigin == "MOD06")
          {
              _sensor = "MODIS";
              _satellite = "TERRA";
          }
          else if (_dataOrigin == "MYD06")
          {
              _sensor = "MODIS";
              _satellite = "AQUA";
          }
      }

      public void Compute(Action<int, string> progressCallback)
      {
          if (File.Exists(_dataBaseXml))
              _con = new ConnectMySqlCloud();
          else
          {
              if (progressCallback != null)
                  progressCallback(-1, "数据库配置文件"+@"\SystemData\ProductArgs\CLD\CPDataBaseArgs.xml"+"不存在！");
              return;
          }
          _periodicsyn = new PeriodicSynPrds2Base(_args.InputDir, "周期合成入库", _con);
          _statics = _args.StatisticsTypes;//统计类型
          _overwritefiles = _args.OverWriteHistoryFiles;
          string[] periods=null;
          if (_periodPrdsArgs == "YEAR")
              periods = new string[3] { "YEAR", "MONTH", "TEN" };
          else if(_periodPrdsArgs =="MONTH")
              periods = new string[2] {"MONTH", "TEN" };
          //string[] periods = _args.PeriodTypes;
          _inputDataPath = Path.Combine(_args.InputDir, "日拼接产品");
          if (!Directory.Exists(_inputDataPath))
          {
              if (progressCallback != null)
                  progressCallback(-1, "输入文件目录" + _inputDataPath + "不存在！请重试！");
              return;
          }
          if (progressCallback != null)
              progressCallback(-1, "输入文件夹：" + _inputDataPath);
          string outputdir = Path.Combine(_args.OutputDir, "周期合成产品");
          if (progressCallback != null)
              progressCallback(-1, "输出文件夹：" + outputdir);
          if (!Directory.Exists(outputdir))
              Directory.CreateDirectory(outputdir);
          //日拼接产品\CloudEffectiveEmissivity\TERRA\MODIS\2011\1\day或night\resl
          int al = _validsets.Length;
          if (al == 0)
              return;
          float interval = 99 / al/2;
          int i = -1,pp;
          string childpath;
          List<string> childPath = new List<string>();
          foreach (string set in _validsets)
          {
              i++;
              pp = i + 1;
              string setName = set.Replace("_", "");
              double filvalue, invalidvalue, setfill;
              //childpath = Path.Combine(_inputDataPath, set.Replace("_", ""));
              childpath = Path.Combine(new string[] { _inputDataPath, set.Replace("_", ""), _satellite, _sensor, _singleyear.ToString() });
              if (progressCallback != null)
                  progressCallback((int)(i * interval*2) + 1, "共" + _validsets.Length+ "个数据集,开始处理第"+pp+"个，" + set + ",文件夹：" + childpath);
              if (progressCallback != null)
                  progressCallback(-1, "\t链接数据库,查询数据集ID及无效值...");
              if (!_con.QueryDatasetsID(_dataOrigin, setName, out _prdID, out _datasetID))//_sensor
              {
                  if (progressCallback != null)
                      progressCallback(-1, "\t链接数据库,查询数据集ID失败！");
                  continue;
              }
              if (!_con.QueryDatasetsInvalidValue(_dataOrigin, setName, out setfill, out filvalue, out invalidvalue))//_sensor
              {
                  if (progressCallback != null)
                      progressCallback(-1, "\t链接数据库,查询数据集无效值失败！");
                  continue;
              }
              RasterStatics sta = new RasterStatics();
              sta.SetDstFillValues(filvalue.ToString());
              sta.SetDstInvalidValues(invalidvalue.ToString());
              //sta.SetDstInvalidValues("0");

              #region 原有算法
              string dayfileformat=null;
               //CloudFractionDay_MOD06_china_day_20060101_0.05.LDF
               string[] danightlabels = new string[2] { "day", "night"};
               int dnc = 0,kk;
              foreach (string label in danightlabels)
              {                  
                  if (progressCallback != null)
                      progressCallback((int)(interval * (i + dnc++/2.0f)) + 1, "\t开始扫描数据集" + set + "的" + _singleyear + "年的" + label + "文件...");
                  kk = dnc;
                  //CloudFractionDay_MOD06_china_day_20060101_0.05.LDF
                  DirectoryInfo yearDirInfo = new DirectoryInfo(childpath);
                  int monthCount = yearDirInfo.GetDirectories().Length;
                  if (progressCallback != null)
                      progressCallback(-1, "\t共" + monthCount + "个月待处理...");
                  if (monthFiles != null)
                      monthFiles.Clear();
                  dayfileformat = string.Format("{0}*{1}*{2}_{3}*.ldf", setName, _dataOrigin, label,_singleyear);
                  foreach (DirectoryInfo d in yearDirInfo.GetDirectories())     //查找子目录，月   
                  {
                      //日拼接产品\CloudEffectiveRadius\TERRA\MODIS\2006\1\day\0.01
                      if (progressCallback != null)
                          progressCallback(-1, "\t开始扫描" + _singleyear + "年" + d.Name + "月的旬文件...");
                      TryComputeXunFiles(Path.Combine(yearDirInfo.FullName, d.ToString()), dayfileformat, progressCallback);
                  }

                  //string[] parafiles = Directory.GetFiles(childpath, string.Format(dayfileformat, setName,_dataOrigin, label,_singleyear), SearchOption.AllDirectories);
                  //if (parafiles.Length == 0)
                  //{
                  //    if (progressCallback != null)
                  //        progressCallback(-1, "\t没有符合数据集" + set + "的" + _singleyear + "年的" + label + "的文件！");
                  //    continue;
                  //}
                  ////按月进行分组
                  //Dictionary<string, List<string>> monthFiles = new Dictionary<string, List<string>>();
                  //foreach (string file in parafiles.ToArray())
                  //{
                  //    string path = Path.GetDirectoryName(file);
                  //    if (!path.Replace(childpath, "").Contains("\\" + _singleyear.ToString() + "\\"))
                  //        continue;
                  //    if (!monthFiles.ContainsKey(path))
                  //        monthFiles.Add(path, new List<string>(new string[1] { file }));
                  //    else
                  //        monthFiles[path].Add(file);
                  //}
                  //if (progressCallback != null)
                  //    progressCallback(-1, "\t共" + monthFiles.Count + "个月的" + parafiles.Length + "个日拼接文件...");
                  //旬文件名，产品类型_MOD06_region_旬产品_yyyy-mm-N_MAX/MIN/AVG_day或night_0.05.LDF
                  Dictionary<string, string> dirxunlist = new Dictionary<string, string>();
                  //将数据按区域进行分组；
                  float m = -1;
                  int ml = monthFiles.Keys.Count;
                  if (ml == 0)
                  {
                      if (progressCallback != null)
                          progressCallback(-1, "\t没有符合数据集" + set + "的" + _singleyear + "年的" + label + "文件！");
                      continue;
                  }
                  string[] monthdirparts = null;
                  float reslf =0;
                  foreach (string monthdir in monthFiles.Keys)
                  {
                      int start = monthdir.LastIndexOf("\\") + 1;
                      string resl = monthdir.Substring(start, monthdir.Length - start);
                      monthdirparts = monthdir.Split('\\');
                      if (!float.TryParse(monthdirparts[monthdirparts.Length - 1], out reslf) || !danightlabels.Contains(monthdirparts[monthdirparts.Length - 2].ToLower()))
                      {
                          continue;
                      }
                      string dirXun  = "{0}\\{1}\\{2}\\{3}\\{4}";
                      dirXun = string.Format(dirXun, outputdir, set.Replace("_", ""), _satellite, _sensor, label + "\\" + resl + "\\Ten");
                      if (!dirxunlist.ContainsKey(dirXun))
                          dirxunlist.Add(dirXun,resl);
                      m++;
                      Dictionary<string, List<string>> regionFiles = new Dictionary<string, List<string>>();
                      foreach (string file in monthFiles[monthdir])
                      {
                          string fileName = Path.GetFileNameWithoutExtension(file);
                          string[] parts = fileName.Split('_');
                          //CloudMultiLayerFlag_MOD06_china_day_20110101_0.01.LDF
                          if (parts.Length < 6)
                              continue;
                          string region = parts[2];
                          if (!regionFiles.ContainsKey(region))
                              regionFiles.Add(region, new List<string>(new string[1] { file }));
                          else
                              regionFiles[region].Add(file);
                      }
                      if (progressCallback != null)
                          progressCallback((int)(interval * (i + kk/2.0f+ m / ml * 0.8)) + 1, "\t\t正在合成" + monthdir + "的旬产品...");
                      ComputeTenPeriodSyn(setName, dirXun, regionFiles, sta,resl, progressCallback, label);
                  }
                  string dirnext;
                  if (periods.Contains("MONTH"))
                  {
                      if (dirxunlist.Count < 1)
                          continue;
                      if (progressCallback != null)
                          progressCallback(-1, "\t开始合成" + setName + "的" + _singleyear + "年" + label + "的月产品...");
                      int xlength = dirxunlist.Count;
                      int xcur = 0;
                      foreach (string dirXun in dirxunlist.Keys)
                      {
                          string resl = dirxunlist[dirXun];
                          string flabel = string.Format("_{0}_{1}.LDF", label, resl);
                          if (progressCallback != null)
                              progressCallback((int)(interval * (i + kk / 2.0f + xcur++/(1.0f*xlength)*0.2  + 0.8)) + 1, "\t\t正在合成" + dirXun + "的月产品...");
                          UpdateDataNext(dirXun, "Ten", "Month", out dirnext, sta, progressCallback, flabel);
                          if (periods.Contains("YEAR"))
                          {
                              if (progressCallback != null)
                                  progressCallback(-1, "\t\t正在合成" + dirnext + "的年产品...");
                              UpdateDataNext(dirnext, "Month", "Year", out dirnext, sta, progressCallback, flabel);
                          }
                      }
                  }
              }
              #endregion
          }
          if (progressCallback != null)
              progressCallback(100, "周期合成完成！");
          return;
      }

      private void TryComputeXunFiles(string dir,string dayfileformat,Action<int, string> progressCallback=null)
      {
          try
          {
              bool isFinaldir = true;
              //在指定目录及子目录下查找文件
              DirectoryInfo Dir = new DirectoryInfo(dir);
              foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
              {
                  //isFinaldir = false;
                  TryComputeXunFiles(Path.Combine(Dir.FullName, d.ToString()), dayfileformat);
              }
              if (isFinaldir)
              {
                  string[] parafiles = Directory.GetFiles(dir, dayfileformat, SearchOption.TopDirectoryOnly);
                  if (parafiles.Length != 0)
                  {
                      if (!monthFiles.ContainsKey(dir))
                          monthFiles.Add(dir, parafiles.ToList());
                      else
                          monthFiles[dir].AddRange(parafiles);
                      return;
                  }
              }
          }
          catch (System.Exception ex)
          {
              if (progressCallback != null)
                  progressCallback(-1, "\t\t扫描" + dir + "的日产品出错！");
          }
      }


      /// <summary>
      /// 对每个月文件夹下的日拼接产品数据进行旬的周期合成
      /// </summary>
      /// <param name="tendir"></param>
      /// <param name="regionFiles"></param>
      private void ComputeTenPeriodSyn(string setname, string dirXun, Dictionary<string, List<string>> regionFiles, RasterStatics sta,string resl, Action<int, string> progressCallback,string label ="day")
      {
        DateTime datebegion, dateend;
          foreach (string region in regionFiles.Keys)
          {
              if (!_con.QueryRegionID(region.ToLower(), out _regionID))
              {
                  if (progressCallback != null)
                      progressCallback(-1, "\t\t查询" + region + "区域的ID失败！");
                  return;
              }
              if (_regionID == 0)
                  _regionID = 1;
            StringBuilder filePrefixXun = new StringBuilder();
            filePrefixXun.AppendFormat("{0}_{1}_{2}_Ten_", setname, _dataOrigin, region);
            string[] parafiles = regionFiles[region].ToArray();
            DayFileWithDate[] newDays = MarkDate(parafiles, out datebegion,out dateend,progressCallback);
            int year = datebegion.Year, month = datebegion.Month;
            string[] tenNO = new string[3] { "上","中","下"};
            for (int ten = 1; ten <= 3; ++ten)
            {
                if (progressCallback != null)
                    progressCallback(-1, "\t\t\t开始合成" + region + "区域的" + tenNO [ten-1]+ "旬产品...");
                //得到每个xun的起始和终止日期
                DateTime xunBegin = new DateTime(year, month, (ten - 1) * 10 + 1);
                DateTime xunEnd = new DateTime(year, month, (ten == 3) ? DateTime.DaysInMonth(year, month) : (ten * 10));
                //对于每一个xun，查询其对应的日产品文件
                IEnumerable<DayFileWithDate> query =
                    from newDay in newDays
                    where newDay.date.CompareTo(xunBegin) >= 0 && newDay.date.CompareTo(xunEnd) <= 0
                    select newDay;
                if (query.Count<DayFileWithDate>() != 0)
                {
                    DayFileWithDate[] newDaysSelect = query.ToArray<DayFileWithDate>();
                    string[] filesSelect = DayFileWithDate.GetFiles(newDaysSelect);
                    StringBuilder outnamePrefix = new StringBuilder();
                    outnamePrefix.Append(filePrefixXun.ToString()).Append(year).Append("-").Append(month.ToString("D2")).Append("-").Append(ten);
                    //统计开始
                    StatisticTen(dirXun, outnamePrefix, filesSelect, sta, resl, label, progressCallback);
                }
                else
                {
                    if (progressCallback != null)
                        progressCallback(-1, "\t\t\t未找到符合" + region + "区域的" + tenNO[ten - 1] + "旬的数据！");
                }
            }
          }
      }

      private void StatisticTen(string dirXun, StringBuilder outnamePrefix, string[] filesSelect, RasterStatics sta,string resl,string label ="day", Action<int, string> progressCallback=null)
      {
          try
          {
              string flabel = string.Format("_{0}_{1}.LDF", label, resl);
              int statistic = -1;
              StringBuilder outnamebuilder = new StringBuilder();
              string outname = "";
              foreach (string stype in _statics)
              {
                  outnamebuilder.Clear();
                  if (_statics.Contains("MAX") || _statics.Contains("max"))
                  {
                      statistic = 0;
                      outnamebuilder.Append(dirXun).Append("\\Max\\").Append(outnamePrefix.ToString()).Append("_MAX").Append(flabel);
                      outname = outnamebuilder.ToString();
                  }
                  else if (_statics.Contains("MIN") || _statics.Contains("min"))
                  {
                      statistic = 1;
                      outnamebuilder.Append(dirXun).Append("\\Min\\").Append(outnamePrefix.ToString()).Append("_MIN").Append(flabel);
                      outname = outnamebuilder.ToString();
                  }
                  else if (_statics.Contains("AVG") || _statics.Contains("avg"))
                  {
                      statistic = 2;
                      outnamebuilder.Append(dirXun).Append("\\Avg\\").Append(outnamePrefix.ToString()).Append("_AVG").Append(flabel);
                      outname = outnamebuilder.ToString();
                  }
                  if (progressCallback != null)
                      progressCallback(-1, "\t\t\t\t开始合成" + stype + "旬产品,输出文件：" + outname);
                  if (File.Exists(outname))
                  {
                      if (_overwritefiles == false && _con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                      {
                          if (!File.Exists(Path.Combine(Path.GetDirectoryName(outname), Path.ChangeExtension(outname, ".overview.png"))))
                              OverViewHelper.OverView(outname, 800);
                          if (progressCallback != null)
                              progressCallback(-1, "\t\t\t\t文件已存在，跳过合成处理！");//+ Path.GetFileName(outname)
                          continue;
                      }
                      else
                      {
                          File.Delete(outname);
                          if (progressCallback != null)
                              progressCallback(-1, "\t\t\t\t重新合成处理开始..."); //+ Path.GetFileName(outname)
                      }
                  }
                  if (!Directory.Exists(Path.GetDirectoryName(outname)))
                      Directory.CreateDirectory(Path.GetDirectoryName(outname));
                  if ((statistic == 0 && !sta.PeriodicMaxStat(filesSelect, outname, progressCallback)) || (statistic == 1 && !sta.PeriodicMinStat(filesSelect, outname, progressCallback))
                      || (statistic == 2 && !sta.PeriodicAvgStat(filesSelect, outname,null, progressCallback)))
                  {
                      if (progressCallback != null)
                          progressCallback(-1, "\t\t\t\t合成处理失败！");// + Path.GetFileName(outname));
                      if (File.Exists(outname))
                      {
                          File.Delete(outname);
                          if (progressCallback != null)
                              progressCallback(-1, "\t\t\t\t删除合成失败文件！");// + Path.GetFileName(outname));
                      }
                      continue;
                  }
                  if (File.Exists(outname))
                  {
                      //生成快视图
                      string filename = OverViewHelper.OverView(outname, 800);
                      if (progressCallback != null && File.Exists(filename))
                          progressCallback(-1, "\t\t\t\t生成快视图成功！");// 文件:+ Path.GetFileName(outname));
                      if (!_con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                      {
                          TryCreatePeriodRecord(outname, progressCallback);
                          if (progressCallback != null && File.Exists(filename))
                              progressCallback(-1, "\t\t\t\t入库成功！");//文件:+ Path.GetFileName(outname));
                      }
                      else
                      {
                          if (progressCallback != null && File.Exists(filename))
                              progressCallback(-1, "\t\t\t\t数据库记录已存在，入库跳过！");//文件:+ Path.GetFileName(outname));
                      }
                  }
              }
          }
          catch (Exception e)
          {
              if (progressCallback != null)
                  progressCallback(-5, "合成" + dirXun + "的旬产品失败！" + e.Message);
              throw e;
          }
      }

      /// <summary>
      /// 从日产品文件提取日期信息
      /// </summary>
      /// <param name="dayPrds"></param>
      /// <returns></returns>
      private static DayFileWithDate[] MarkDate(string[] dayPrds, out DateTime datebegin, out DateTime dateend,Action<int, string> progressCallback=null)
      {
          datebegin = DateTime.Now;
          dateend = DateTime.MinValue;
          DayFileWithDate[] newDays = new DayFileWithDate[dayPrds.Length];
          for (int i = 0; i < dayPrds.Length; ++i)
          {
              newDays[i].path = dayPrds[i];
              //CldFrcTot_MOD06_china_日产品_20131204.LDF
              string[] splitsAll = Path.GetFileNameWithoutExtension(dayPrds[i]).Split('_');
              newDays[i].date = DateTime.ParseExact(splitsAll[4], "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
              if (newDays[i].date > dateend)
              {
                  dateend = newDays[i].date;
              }
              if (newDays[i].date < datebegin)
              {
                  datebegin = newDays[i].date;
              }
          }
          return newDays;
      }

      //根据已有的前一个产品，生成新的下一个产品
      private void UpdateDataNext(string dir, string prev, string next, out string dirnext, RasterStatics sta, Action<int, string> progressCallback,string flabel)
      {
          dirnext = dir.Replace("\\" + prev, "\\" + next);
          string[] dirWhich =new string[]{};
          Dictionary<string, int> listSatics = new Dictionary<string,int>();
          foreach (string stat in _statics)
          {
              if (stat.ToUpper() == "AVG")
              {
                  listSatics.Add("\\Avg", 2);
                  dirWhich = new string[] { "\\Avg" };
              }
              else if (stat.ToUpper() == "MAX")
              {
                  listSatics.Add("\\Max",0);
              }
              else if (stat.ToUpper() == "MIN")
              {
                  listSatics.Add("\\Min", 1);
              }
          }
          foreach(string listSatic in listSatics.Keys)
          {
              string stdir = dir + listSatic;
              if (!Directory.Exists(stdir))
                  continue;
              string[] files = Directory.GetFiles(stdir, "*.ldf", SearchOption.TopDirectoryOnly);
              if (files.Length == 0)
                  return;
              //初始化输出目录、下个产品的文件名前缀、起始/终止年
              //MWS_MWSD_China_Xun_0SD_A_2011_1_1_avg.dat
              //MWS_MWSD_China_Month_0SD_A_2011_1_avg.dat
              string dirNext = Path.GetDirectoryName(files[0]).Replace("\\" + prev, "\\" + next);
              //CloudTopTemperature_MOD06_china_Ten_2011-1-1_AVG.LDF
              string[] splitsFirst = Path.GetFileNameWithoutExtension(files[0]).Split('_');
              StringBuilder filePrefixNext = new StringBuilder();
              filePrefixNext.AppendFormat("{0}_{1}_{2}_{3}_", splitsFirst[0], splitsFirst[1], splitsFirst[2], next);
              //int yearBegin = Convert.ToInt32((Path.GetFileNameWithoutExtension(files[0]).Split('_')[4]).Split('-')[0]);
              //int yearEnd = Convert.ToInt32((Path.GetFileNameWithoutExtension(files[files.Length - 1]).Split('_')[4]).Split('-')[0]);
              int yearBegin = _singleyear;
              int yearEnd = _singleyear;
              FindFiles(prev, yearBegin, yearEnd, files, filePrefixNext, listSatics[listSatic], dirNext, sta, flabel, progressCallback);
          }
      }

      //分情况找文件
      private void FindFiles(string prev, int yearBegin, int yearEnd, string[] files, StringBuilder filePrefixNext, int statistic, string dirNext, RasterStatics sta, string flabel, Action<int, string> progressCallback = null)
      {
          if (prev == "Ten")
          {
              for (int year = yearBegin; year <= yearEnd; ++year)
              {
                  for (int month = 1; month <= 12; ++month)
                  {
                      StringBuilder[] contain = new StringBuilder[3];
                      for (int i = 0; i < 3; ++i)
                      {
                          contain[i] = new StringBuilder();
                          contain[i].Append(year).Append("-").Append(month.ToString("D2")).Append("-").Append(i + 1);
                      }
                      IEnumerable<string> query =
                          from file in files
                          where file.Contains(contain[0].ToString()) || file.Contains(contain[1].ToString()) || file.Contains(contain[2].ToString())
                          select file;
                      if (query.Count<string>() != 0)
                      {
                          if (progressCallback != null)
                              progressCallback(-1, "\t\t正在由旬数据合成" + year + "年" + month + "月的月产品...");
                          string[] filenames = query.ToArray<string>();
                          StringBuilder outnamePrefix = new StringBuilder();
                          outnamePrefix.Append(filePrefixNext.ToString()).Append(year).Append("-").Append(month.ToString("D2")).Append("_");
                          //开始计算
                          StatisticNext(statistic, dirNext, outnamePrefix.ToString(), filenames, sta, flabel, progressCallback);
                      }
                  }
              }
          }
          else if (prev == "Month")
          {
              for (int year = yearBegin; year <= yearEnd; ++year)
              {
                  StringBuilder[] contain = new StringBuilder[12];
                  for (int i = 0; i < 12; ++i)
                  {
                      contain[i] = new StringBuilder();
                      contain[i].Append(year).Append("-").Append((i + 1).ToString("D2"));
                  }
                  IEnumerable<string> query =
                      from file in files
                      where file.Contains(contain[0].ToString()) || file.Contains(contain[1].ToString()) || file.Contains(contain[2].ToString()
                      ) || file.Contains(contain[3].ToString()) || file.Contains(contain[4].ToString()) || file.Contains(contain[5].ToString())
                      || file.Contains(contain[6].ToString()) || file.Contains(contain[7].ToString()) || file.Contains(contain[8].ToString())
                      || file.Contains(contain[9].ToString()) || file.Contains(contain[10].ToString()) || file.Contains(contain[11].ToString())
                      select file;
                  if (query.Count<string>() != 0)
                  {
                      if (progressCallback != null)
                          progressCallback(-1, "\t\t正在由月数据合成" + year + "年的年产品...");
                      string[] filenames = query.ToArray<string>();
                      StringBuilder outnamePrefix = new StringBuilder();
                      outnamePrefix.Append(filePrefixNext.ToString()).Append(year).Append("_");
                      //开始计算
                      StatisticNext(statistic, dirNext, outnamePrefix.ToString(), filenames, sta, flabel, progressCallback);
                  }
              }
          }
      }

      //计算统计结果
      private void StatisticNext(int statistic, string dirNext, string outnamePrefix, string[] filenames, RasterStatics sta, string flabel, Action<int, string> progressCallback=null)
      {
          try
          {
              StringBuilder outstaname = new StringBuilder();
              string outname="";
              switch (statistic)
              {
                  case 0:
                      outstaname.Append(dirNext).Append("\\").Append(outnamePrefix).Append("MAX").Append(flabel);
                      outname = outstaname.ToString();
                      break;
                  case 1:
                      outstaname.Append(dirNext).Append("\\").Append(outnamePrefix).Append("MIN").Append(flabel);
                      outname = outstaname.ToString();
                      break;
                  case 2:
                      outstaname.Append(dirNext).Append("\\").Append(outnamePrefix).Append("AVG").Append(flabel);
                      outname = outstaname.ToString();
                      break;
              }
              if (progressCallback != null)
                  progressCallback(-1, "\t\t\t\t输出文件：" + outname);
              if (File.Exists(outname) )
              {
                  if (_overwritefiles == false && _con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                  {
                      if (!File.Exists(Path.Combine(Path.GetDirectoryName(outname), Path.ChangeExtension(outname, ".overview.png"))))
                          OverViewHelper.OverView(outname, 800);
                      if (progressCallback != null)
                          progressCallback(-1, "\t\t\t文件已存在，跳过合成处理！");// + Path.GetFileName(outname));
                      return;
                  }
                  else
                  {
                      File.Delete(outname);
                      if (progressCallback != null)
                          progressCallback(-1, "\t\t\t重新合成处理开始...");//+ Path.GetFileName(outname));
                  }
              }
              string outdir = Path.GetDirectoryName(outname);
              if (!Directory.Exists(outdir))
                  Directory.CreateDirectory(outdir);
              if ((statistic == 0 && !sta.PeriodicMaxStat(filenames, outname, progressCallback)) || (statistic == 1 && !sta.PeriodicMinStat(filenames, outname, progressCallback))
    || (statistic == 2 && !sta.PeriodicAvgStat(filenames, outname, null, progressCallback)))
              {
                  if (progressCallback != null)
                      progressCallback(-1, "\t\t\t\t合成处理失败！");//+ Path.GetFileName(outname));
                  if (File.Exists(outname))
                  {
                      File.Delete(outname);
                      if (progressCallback != null)
                          progressCallback(-1, "\t\t\t\t删除合成失败文件！");// + outname);
                  }
                  return;
              }
              if (File.Exists(outname))
              {
                  //生成快视图
                  string filename = OverViewHelper.OverView(outname, 800);
                  if (progressCallback != null && File.Exists(filename))
                      progressCallback(-1, "\t\t\t\t生成快视图成功！");// + Path.GetFileName(outname));
                   if (!_con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                   {
                       TryCreatePeriodRecord(outname, progressCallback);
                       if (progressCallback != null && File.Exists(filename))
                           progressCallback(-1, "\t\t\t\t入库成功！");// + Path.GetFileName(outname));
                   }
                   else
                   {
                       if (progressCallback != null && File.Exists(filename))
                           progressCallback(-1, "\t\t\t\t数据库记录已存在，入库跳过！");// + Path.GetFileName(outname));
                   }
              }
          }
          catch (Exception e)
          {
              if (progressCallback != null)
                  progressCallback(-5, "合成" + dirNext + "的产品失败！" + e.Message);
              throw e;
          }
      }

      private void TryCreatePeriodRecord(string file, Action<int, string> progressCallback = null)
      {
          string fname, localfname, region, sensor = "", statName, period;
          string[] parts;
          long prdID = _prdID, peridID, datasetID = _datasetID;
          int year = 0, month = 0, xun = 0, regionID = _regionID, staticsID;
          //double validPercent;
          //CloudTopTemperature_MOD06_china_Ten_2011-11-2_AVG_day_0.05.LDF
          //CloudTopTemperature_MOD06_china_Month_2011-11_AVG_day_0.05.LDF
          //CloudTopTemperature_MOD06_china_Year_2011_AVG_day_0.05.LDF
          if (progressCallback != null)
              progressCallback(-1, "\t\t\t\t" + Path.GetFileName(file) + "入库开始...");
          fname = Path.GetFileNameWithoutExtension(file);
          localfname = file.Replace(_args.InputDir, "");
          parts = fname.Split('_');
          period = parts[3].ToLower();
          if (period != "ten" && period != "month" && period != "year")
              return;
          if (!_periodicsyn._periodTypeID.ContainsKey(period))
              return;
          peridID = _periodicsyn._periodTypeID[period];//周期类型
          string sta = parts[5].ToUpper();
          if (sta != "MIN" && sta != "MAX" && sta != "AVG")
              return;
          statName = sta;
          staticsID = _periodicsyn._statName2ID[sta];//统计类型
          string label =parts[1].ToUpper();
          if ( label== "MOD06"||label== "MYD06"||label== "AIRS")
              sensor = label;
          else
              return;
          {
              region = parts[2];
              if (period == "ten")
              {
                  Regex XunDateReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})-(?<Xun>\d{1})", RegexOptions.Compiled);
                  Match match = XunDateReg.Match(parts[4]);
                  if (!match.Success)
                      return;
                  year = Int16.Parse(match.Groups["year"].Value);
                  month = Int16.Parse(match.Groups["month"].Value);
                  xun = Int16.Parse(match.Groups["Xun"].Value);
              }
              else if (period == "month")
              {
                  Regex MonthDateReg = new Regex(@"(?<year>\d{4})-(?<month>\d{2})", RegexOptions.Compiled);
                  Match match = MonthDateReg.Match(parts[4]);
                  if (!match.Success)
                      return;
                  year = Int16.Parse(match.Groups["year"].Value);
                  month = Int16.Parse(match.Groups["month"].Value);
              }
              else//period == "year"
              {
                  year = Int16.Parse(parts[4]);
              }
              //GetRsEnvolop(file, out resol, out validPercent);
              float resol = float.Parse(parts[parts.Length - 1]);
              string datasource = "";
              if (parts[parts.Length - 2].ToLower() == "day")
                  datasource = "D";
              else
                  datasource = "N";
              if (!_con.IshasRecord(_tableName, "ImageName", Path.GetFileName(file)))
              {
                  _con.InsertPeriodicSynProductsTable(prdID, datasetID, localfname, regionID, region, sensor, resol, peridID, year, month, xun, staticsID, datasource);
              }
              else
              {
                  _con.DeleteCLDParatableRecord(_tableName, "ImageName", Path.GetFileName(file));
                  _con.InsertPeriodicSynProductsTable(prdID, datasetID, localfname, regionID, region, sensor, resol, peridID, year, month, xun, staticsID, datasource);
              }
              if (progressCallback != null)
                  progressCallback(-1, "\t\t\t\t" + Path.GetFileName(file) + "入库成功！");

          }
      }


  }
}
