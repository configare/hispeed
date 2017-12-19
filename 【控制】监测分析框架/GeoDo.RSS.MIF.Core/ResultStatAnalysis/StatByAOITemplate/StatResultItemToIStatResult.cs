using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class StatResultItemToIStatResult
    {
        public static IStatResult ItemsToResults(StatResultItem[] items, params string[] arguments)
        {
            if (items == null || items.Length == 0)
                return null;
            List<IStatResult> results = new List<IStatResult>();
            string[] columns = null;
            if (arguments == null)
                columns = arguments;
            else
                columns = new string[] { "矢量分区", "最大覆盖（平方公里）","最大覆盖（亩）" };
            List<string[]> names = new List<string[]>();
            List<string> row;
            foreach (StatResultItem item in items)
            {
                row = new List<string>();
                row.Add(item.Name);
                row.Add(item.Value.ToString());
                row.Add((item.Value * 1500).ToString());
                names.Add(row.ToArray());
            }
            string title = "统计日期：" + DateTime.Now.ToShortDateString();
            //
            if (arguments != null&&arguments.Length>=1)
            {
                string orbitTimes=string.Empty;
                foreach (string item in arguments)
                {
                    if (!File.Exists(item))
                        break;
                    RasterIdentify rasterId = new RasterIdentify(item);
                    orbitTimes += rasterId.OrbitDateTime.ToShortDateString() + " ";
                }
                title += "\n" + "轨道时间：" + orbitTimes;
            }
            if (names == null || names.Count == 0)
                return null;
            else
                return new StatResult(title, columns, names.ToArray());
        }
    }
}
