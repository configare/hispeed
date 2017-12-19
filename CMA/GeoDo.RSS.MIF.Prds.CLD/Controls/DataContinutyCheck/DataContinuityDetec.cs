using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class DataContinuityDetec
    {

        public static bool MOD06ContinutyDetec(string[] mod06files, out List<DateTime> lostdataDay, out List<DateTime> lostdataTime)
        {
            //MOD03.A2011001.0350.hdf
            int year,daysofyear;
            List<DateTime> dataTime;//
            Dictionary<DateTime, List<DateTime>> dayAndHourMin = new Dictionary<DateTime, List<DateTime>>();
            lostdataTime = new List<DateTime>();
            lostdataDay = new List<DateTime>();
            //for (int f=0;f<mod06files.Length;f++)
            foreach (string file in mod06files)
            {
                string fName = Path.GetFileName(file).ToUpper();
                Match match = Regex.Match(fName, @".(?<year>\d{4})(?<days>\d{3}).(?<hour>\d{2})(?<minutes>\d{2}).");
                if (match.Success)
                {
                    year = Int32.Parse(match.Groups["year"].Value);
                    daysofyear = Int32.Parse(match.Groups["days"].Value);
                    DateTime date = GetDateFormDaysOfYear(year, daysofyear);
                    DateTime dateAndTime = date.AddHours(double.Parse(match.Groups["hour"].Value)).AddMinutes(double.Parse(match.Groups["minutes"].Value));
                    if (!dayAndHourMin.Keys.Contains(date))
                    {
                        dataTime = new List<DateTime>();
                        dataTime.Add(dateAndTime);
                        dayAndHourMin.Add(date, dataTime);
                    }
                    else
                    {
                        if (!dayAndHourMin[date].Contains(dateAndTime))
                        {
                            dayAndHourMin[date].Add(dateAndTime);
                        }
                    }
                }
                else
                {
                    LogFactory.WriteLine("MOD06检测", "文件" + file + "命名不规范，无法解析数据日期！");
                }
            }
            #region 时间连续性检测
            if (dayAndHourMin.Count > 0)
            {
                #region 日期连续性检测
                DateTime maxtime = dayAndHourMin.Keys.Max();//最大日期
                DateTime mintime = dayAndHourMin.Keys.Min();//最小日期
                DateTime midtime;
                if (mintime.Year == maxtime.Year)//同一年
                {
                    for (int i = 1; i < (maxtime.DayOfYear - mintime.DayOfYear); i++)
                    {
                        if (!dayAndHourMin.Keys.Contains(mintime.AddDays(i)))
                        {
                            lostdataDay.Add(mintime.AddDays(i));
                        }
                    }
                }
                else
                {
                    midtime = mintime;
                    for (int y = mintime.Year; y <= maxtime.Year; y++)
                    {
                        int daysofYyear = DateTime.IsLeapYear(y) ? 366 : 365;
                        int daystart;
                        if (y == mintime.Year)
                        {
                            daystart = mintime.DayOfYear;
                            for (int i = 1; i < daysofYyear - daystart; i++)
                            {
                                if (!dayAndHourMin.Keys.Contains(midtime.AddDays(i)))
                                {
                                    lostdataDay.Add(midtime.AddDays(i));
                                }
                            }
                        }
                        else
                        {
                            daystart = 1;
                            midtime = new DateTime(y, 1, 1);
                            for (int i = daystart; i < daysofYyear; i++)
                            {
                                if (!dayAndHourMin.Keys.Contains(midtime.AddDays(i)))
                                {
                                    lostdataDay.Add(midtime.AddDays(i));
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 每天的5分钟段数据连续性检测
                foreach (List<DateTime> alldayTime in dayAndHourMin.Values)
                {
                    if (alldayTime.Count > 1)//数据多于1个
                    {
                        maxtime = alldayTime.Max();
                        mintime = alldayTime.Min();
                        for (midtime = mintime.AddMinutes(5); midtime <= maxtime; midtime = midtime.AddMinutes(5))
                        {
                            if (!alldayTime.Contains(midtime))
                            {
                                lostdataTime.Add(midtime);
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            if (lostdataTime.Count != 0 || lostdataDay.Count != 0)
            {
                return false;
            }
            return true;
        }

        public static DateTime GetDateFormDaysOfYear(int year, int daysofyear)
        {
            int[,] tab = { { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }, { 0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 } };
            int k;
            int leap = DateTime.IsLeapYear(year) ? 1 : 0;
            for (k = 1; daysofyear > tab[leap, k]; k++)
                daysofyear = daysofyear - tab[leap, k];
            DateTime date = new DateTime(year, k, daysofyear);
            return date;
        }

        public static DateTime GetMOD06OribitTime(string fileName)
        {
            int year;
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @".(?<year>\d{4})(?<days>\d{3}).(?<hour>\d{2})(?<minutes>\d{2}).");
            if (match.Success)
            {
                year = Int32.Parse(match.Groups["year"].Value);
                int daysofyear = Int32.Parse(match.Groups["days"].Value);
                DateTime date = GetDateFormDaysOfYear(year, daysofyear);
                date = date.AddHours(double.Parse(match.Groups["hour"].Value)).AddMinutes(double.Parse(match.Groups["minutes"].Value));
                return date;
            }
            return DateTime.MinValue;
        }


    }
}
