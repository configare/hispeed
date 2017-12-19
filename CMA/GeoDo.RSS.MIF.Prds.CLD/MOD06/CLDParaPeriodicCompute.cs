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
  public class CLDParaPeriodicCompute
    {
      private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
      private static string _periodPrdsArgsXml = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\CLD\PeriodPrdsArgs.xml";
      private string _inputDataPath;
      private string _satellite="AQUA";
      private string _sensor = "AIRS";
      private string _dataOrigin ="AIRS";
      private string _tableName = "CP_PeriodicSynthesis_TB";
      ConnectMySqlCloud _con;
      InputArg _args;
      private static string[] _statics =null;
      private static bool _overwritefiles = false;
      PeriodicSynPrds2Base _periodicsyn;
      private static Regex DayDateReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);

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

      public CLDParaPeriodicCompute(string sensor,InputArg arg)
      {
          _sensor = sensor;
          _args = arg;
          if (_sensor == "MODIS")
          {
              _dataOrigin = "MOD06";
              _satellite = "TERRA";
          }
      }

      public void Compute(Action<int, string> progressCallback)
      {
          if (progressCallback != null)
              progressCallback(1, "周期合成开始,链接数据库...");
          if (File.Exists(_dataBaseXml))
              _con = new ConnectMySqlCloud();
          else
              return;
          _periodicsyn = new PeriodicSynPrds2Base(_args.InputDir, "周期合成入库", _con);
          _statics = _args.StatisticsTypes;//统计类型
          string[] periods = _args.PeriodTypes;
          _inputDataPath = Path.Combine(_args.InputDir, "日拼接产品");
          _overwritefiles = _args.OverWriteHistoryFiles;
          if (!Directory.Exists(_inputDataPath))
          {
              MessageBox.Show("配置的输入文件目录不存在！请重试！");
              return;
          }
          string outputdir = Path.Combine(_args.OutputDir, "周期合成产品");
          if (!Directory.Exists(outputdir))
              Directory.CreateDirectory(outputdir);
          DirectoryInfo dir = new DirectoryInfo(_inputDataPath);
          List<string> childPath = new List<string>();
          //日拼接产品\Cloud_Effective_Emissivity\AQUA\MODIS\2011\1\day或nigh\resl
          int al = dir.GetDirectories("*").Length;
          if (al == 0)
              return;
          float interval = 99 / al;
          int i = -1;
          foreach (DirectoryInfo dChild in dir.GetDirectories("*"))
          {
              i++;
              childPath.Add(dChild.FullName); //存放每个子产品
              string setName = dChild.Name;
              double filvalue, invalidvalue, setfill;
              if (!_con.QueryDatasetsInvalidValue(_dataOrigin, setName, out setfill, out filvalue, out invalidvalue))
                  continue;              
              if (progressCallback != null)
                  progressCallback((int)(i * interval)+1, "开始扫描" + dChild.FullName + "的待处理文件...");
              RasterStatics sta = new RasterStatics();
              sta.SetDstFillValues(filvalue.ToString());
              sta.SetDstInvalidValues(invalidvalue.ToString());
               string dayfileformat = "*{0}*{1}*.ldf";
               string[] danightlabels = new string[2] { "day", "night"};
              foreach (string label in danightlabels)
              {
                  string[] parafiles = Directory.GetFiles(dChild.FullName, string.Format(dayfileformat, _dataOrigin, label), SearchOption.AllDirectories);
                  if (parafiles.Length ==0)
                      continue;
                  //按月进行分组
                  Dictionary<string, List<string>> monthFiles = new Dictionary<string, List<string>>();
                  foreach (string file in parafiles.ToArray())
                  {
                      string path = Path.GetDirectoryName(file);
                      if (!monthFiles.ContainsKey(path))
                          monthFiles.Add(path, new List<string>(new string[1] { file }));
                      else
                          monthFiles[path].Add(file);
                  }
                  //旬文件名，产品类型_MOD06_region_旬产品_yyyy-mm-N_MAX/MIN/AVG_day或night_0.05.LDF
                  Dictionary<string, string> dirxunlist = new Dictionary<string, string>();

                  //将数据按区域进行分组；
                  float m = -1;
                  int ml = monthFiles.Keys.Count;
                  foreach (string monthdir in monthFiles.Keys)
                  {
                      int start = monthdir.LastIndexOf("\\") + 1;
                      string resl = monthdir.Substring(start, monthdir.Length - start);
                      string dirXun  = "{0}\\{1}\\{2}\\{3}\\{4}\\{5}";
                      dirXun = string.Format(dirXun, dir.Parent.FullName, "周期合成产品", dChild.Name, _satellite, _sensor, label + "\\" + resl + "\\Ten");
                      if (!dirxunlist.ContainsKey(dirXun))
                          dirxunlist.Add(dirXun,resl);
                      m++;
                      Dictionary<string, List<string>> regionFiles = new Dictionary<string, List<string>>();
                      foreach (string file in monthFiles[monthdir])
                      {
                          string fileName = Path.GetFileNameWithoutExtension(file);
                          string[] parts = fileName.Split('_');
                          if (parts.Length != 6)
                              continue;
                          string region = parts[2];
                          if (!regionFiles.ContainsKey(region))
                              regionFiles.Add(region, new List<string>(new string[1] { file }));
                          else
                              regionFiles[region].Add(file);
                      }
                      if (progressCallback != null)
                          progressCallback((int)(interval * (i + m / ml * 0.5)) + 1, "正在处理" + monthdir + "的旬产品...");
                      ComputeTenPeriodSyn(setName, dirXun, regionFiles, sta,resl, progressCallback, label);
                  }
                  string dirnext;
                  if (periods.Contains("MONTH"))
                  {
                      if (dirxunlist.Count < 1)
                          continue;
                      int xlength = dirxunlist.Count;
                      int xcur = 0;
                      foreach (string dirXun in dirxunlist.Keys)
                      {
                          string resl = dirxunlist[dirXun];
                          string flabel = string.Format("_{0}_{1}.LDF", label, resl);
                          if (progressCallback != null)
                              progressCallback((int)(interval * (i * 1.0 / xlength * xcur++ + 0.5)) + 1, "正在处理" + dirXun + "的月产品...");
                          UpdateDataNext(dirXun, "Ten", "Month", out dirnext, sta, progressCallback, flabel);
                          if (periods.Contains("YEAR"))
                          {
                              if (progressCallback != null)
                                  progressCallback((int)(interval * (i * 1.0 / xlength * xcur + 0.8)) + 1, "正在处理" + dirnext + "的年产品...");
                              UpdateDataNext(dirnext, "Month", "Year", out dirnext, sta, progressCallback, flabel);
                          }
                      }
                  }
              }
          }
          if (progressCallback != null)
              progressCallback(100, "周期合成完成！");
          return;
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
            StringBuilder filePrefixXun = new StringBuilder();
            filePrefixXun.AppendFormat("{0}_{1}_{2}_Ten_", setname, _dataOrigin, region);
              string[] parafiles = regionFiles[region].ToArray();
              DayFileWithDate[] newDays = MarkDate(parafiles, out datebegion,out dateend);
              int year = datebegion.Year, month = datebegion.Month;
              for (int ten = 1; ten <= 3; ++ten)
              {
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
                      StatisticTen(dirXun, outnamePrefix, filesSelect, sta, resl,label);
                  }
              }
          }
      }

      private void StatisticTen(string dirXun, StringBuilder outnamePrefix, string[] filesSelect, RasterStatics sta,string resl,string label ="day")
      {
          try
          {
              string flabel = string.Format("_{0}_{1}.LDF", label, resl);
              foreach (string stype in _statics)
              {
                  string outname="";
                  if (_statics.Contains("MAX") || _statics.Contains("max"))
                  {
                      StringBuilder outmaxname = new StringBuilder();
                      outmaxname.Append(dirXun).Append("\\Max\\").Append(outnamePrefix.ToString()).Append("_MAX").Append(flabel);
                      outname = outmaxname.ToString();
                  }
                  else if (_statics.Contains("MIN") || _statics.Contains("min"))
                  {
                      StringBuilder outminname = new StringBuilder();
                      outminname.Append(dirXun).Append("\\Min\\").Append(outnamePrefix.ToString()).Append("_MIN").Append(flabel);
                      outname = outminname.ToString();
                  }
                  else if (_statics.Contains("AVG") || _statics.Contains("avg"))
                  {
                      StringBuilder outAvgname = new StringBuilder();
                      outAvgname.Append(dirXun).Append("\\Avg\\").Append(outnamePrefix.ToString()).Append("_AVG").Append(flabel);
                      outname = outAvgname.ToString();
                  }
                  if (File.Exists(outname))
                  {
                      if (_overwritefiles == false && _con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                          continue;
                      else
                          File.Delete(outname);
                  }
                  if (!Directory.Exists(Path.GetDirectoryName(outname)))
                      Directory.CreateDirectory(Path.GetDirectoryName(outname));
                  if (sta.PeriodicAvgStat(filesSelect, outname))
                  {
                      if (File.Exists(outname))// && !_con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                          _periodicsyn.TryCreatePeriodRecord(outname);
                  }                  
              }
          }
          catch (Exception e)
          {
              throw e;
              //LogFactory.WriteLine(_dataOrigin, e.Message + "\n");
          }
      }

      /// <summary>
      /// 从日产品文件提取日期信息
      /// </summary>
      /// <param name="dayPrds"></param>
      /// <returns></returns>
      private static DayFileWithDate[] MarkDate(string[] dayPrds, out DateTime datebegin, out DateTime dateend)
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
              string[] files = Directory.GetFiles(dir + listSatic, "*.ldf", SearchOption.TopDirectoryOnly);
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
              int yearBegin = Convert.ToInt32((Path.GetFileNameWithoutExtension(files[0]).Split('_')[4]).Split('-')[0]);
              int yearEnd = Convert.ToInt32((Path.GetFileNameWithoutExtension(files[files.Length - 1]).Split('_')[4]).Split('-')[0]);
              FindFiles(prev, yearBegin, yearEnd, files, filePrefixNext, listSatics[listSatic], dirNext, sta, flabel);
          }
      }

      //分情况找文件
      private void FindFiles(string prev, int yearBegin, int yearEnd, string[] files, StringBuilder filePrefixNext, int statistic, string dirNext, RasterStatics sta,string flabel )
      {
          if (prev == "Ten")
          {
              for (int year = yearBegin; year <= yearEnd; ++year)
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
                          string[] filenames = query.ToArray<string>();
                          StringBuilder outnamePrefix = new StringBuilder();
                          outnamePrefix.Append(filePrefixNext.ToString()).Append(year).Append("-").Append(month.ToString("D2")).Append("_");
                          //开始计算
                          StatisticNext(statistic, dirNext, outnamePrefix.ToString(), filenames, sta, flabel);
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
                      string[] filenames = query.ToArray<string>();
                      StringBuilder outnamePrefix = new StringBuilder();
                      outnamePrefix.Append(filePrefixNext.ToString()).Append(year).Append("_");
                      //开始计算
                      StatisticNext(statistic, dirNext, outnamePrefix.ToString(), filenames, sta, flabel);
                  }
              }
          }
      }

      //计算统计结果
      private void StatisticNext(int statistic, string dirNext, string outnamePrefix, string[] filenames, RasterStatics sta, string flabel)
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
              if (File.Exists(outname) )
              {
                  if (_overwritefiles == false && _con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                      return;
                  else
                      File.Delete(outname);
              }
              if (!Directory.Exists(Path.GetDirectoryName(outname)))
                  Directory.CreateDirectory(Path.GetDirectoryName(outname));
              if (sta.PeriodicAvgStat(filenames, outname))
              {
                  if (File.Exists(outname) && !_con.IshasRecord(_tableName, "ImageName", Path.GetFileName(outname)))
                      _periodicsyn.TryCreatePeriodRecord(outname);
              }
          }
          catch (Exception e)
          {
              throw e;
          }
      }


    }
}
