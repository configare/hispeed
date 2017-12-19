using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class ISCCPProcess
    {
        /// <summary>
        /// ISCCP D2 GPC格式数据的时间连续性检测
        /// </summary>
        /// <param name="ISCCPfiles"></param>
        /// <param name="logName"></param>
        public static bool ISCCPContinutyDetec(string[] ISCCPfiles, Action<string> callProBack, out Dictionary<int, Dictionary<int, List<string>>> lostdataTime)
        {
            //ISCCP.D2.0.GLOBAL.2008.07.99.0000.GPC
            lostdataTime = new Dictionary<int, Dictionary<int, List<string>>>();
            int year, month;
            string hour;
            Dictionary<int, Dictionary<int, List<string>>> alldataTime = new Dictionary<int, Dictionary<int, List<string>>>();
            Dictionary<int, List<string>> montht;
            List<string> utctime;
            #region 解析数据文件名中的日期
            if (callProBack!=null)
                callProBack("开始解析数据文件名中的时间信息...");
            foreach (string file in ISCCPfiles)
            {
                string fName = Path.GetFileName(file).ToUpper();
                Match match = Regex.Match(fName, @".(?<year>\d{4}).(?<month>\d{2}).(?<day>\d{2}).(?<time>\d{4}).");
                if (match.Success)
                {
                    year = Int32.Parse(match.Groups["year"].Value);
                    month = Int32.Parse(match.Groups["month"].Value);
                    hour = match.Groups["time"].Value;
                    if (alldataTime.Keys.Contains(year))
                    {
                        if (alldataTime[year].Keys.Contains(month))
                        {
                            if (!alldataTime[year][month].Contains(hour))
                            {
                                alldataTime[year][month].Add(hour);
                            }
                        }
                        else
                        {
                            utctime = new List<string>();
                            utctime.Add(hour);
                            alldataTime[year].Add(month, utctime);
                        }
                    }
                    else
                    {
                        utctime = new List<string>();
                        utctime.Add(hour);
                        montht = new Dictionary<int, List<string>>();
                        montht.Add(month, utctime);
                        alldataTime.Add(year, montht);
                    }
                }
                else
                {
                    if (callProBack != null)
                        callProBack("文件" + file + "命名不规范，无法解析数据日期！");
                }
            }
            if (callProBack != null)
                callProBack("解析数据文件名中的时间信息完成！");
            #endregion
            if (alldataTime.Count > 0)
            {
                if (callProBack != null)
                    callProBack("开始检测数据时间连续性...");
                int maxyear, minyear, maxmonth, minmonth;
                string[] allutc = new string[] { "0000", "0300", "0600", "0900", "1200", "1500", "1800", "2100", "9999" };
                maxyear = alldataTime.Keys.Max();//最大年份
                minyear = alldataTime.Keys.Min();//最小年份
                for (int y = minyear; y <= maxyear; y++)
                {
                    if (y != minyear)
                        minmonth = 1;//最小月份
                    else
                        minmonth = alldataTime[y].Keys.Min();//最小月份
                    if (y != maxyear)
                        maxmonth = 12;//最小月份
                    else
                        maxmonth = alldataTime[y].Keys.Max();//最大月份

                    if (alldataTime.Keys.Contains(y))
                    {
                        for (int i = minmonth; i <= maxmonth; i++)
                        {
                            if (alldataTime[y].Keys.Contains(i))
                            {
                                foreach (string utct in allutc)
                                {
                                    if (!alldataTime[y][i].Contains(utct))
                                    {

                                        if (!lostdataTime.Keys.Contains(y))//不含y年的记录
                                        {
                                            List<string> lostDayUTC = new List<string>();
                                            lostDayUTC.Add(utct);
                                            Dictionary<int, List<string>> lostMonth = new Dictionary<int, List<string>>();
                                            lostMonth.Add(i, lostDayUTC);
                                            lostdataTime.Add(y, lostMonth);
                                        }
                                        else if (!lostdataTime[y].Keys.Contains(i))//不含y年i月的记录
                                        {
                                            List<string> lostDayUTC = new List<string>();
                                            lostDayUTC.Add(utct);
                                            lostdataTime[y].Add(i, lostDayUTC);
                                        }
                                        else if (!lostdataTime[y][i].Contains(utct))//不含y年i月utct时的记录
                                        {
                                            lostdataTime[y][i].Add(utct);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!lostdataTime.Keys.Contains(y))//不含y年的记录
                                {
                                    Dictionary<int, List<string>> lostMonth = new Dictionary<int, List<string>>();
                                    lostMonth.Add(i, null);
                                    lostdataTime.Add(y, lostMonth);
                                }
                                else
                                {
                                    lostdataTime[y].Add(i, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        lostdataTime.Add(y, null);
                    }
                }
                if (callProBack != null)
                    callProBack("检测数据时间连续性结束！");
                if (lostdataTime.Count != 0)
                {
                    return false;
                }
                return true;
            }
            if (callProBack != null)
                callProBack("没有合格的待检测数据！");
            return false;
        }
    }
}
