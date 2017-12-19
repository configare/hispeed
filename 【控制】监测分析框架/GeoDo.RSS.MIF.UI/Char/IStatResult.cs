using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Geodo.RSS.MIF.UI
{
    public interface IStatResult
    {
        string Title { get; }
        string[] Columns { get; }
        string[][] Rows { get; }
    }

    public class StatResult : IStatResult
    {
        protected string _title = null;
        protected string[] _columns = null;
        protected string[][] _rows = null;
        private List<int> _indexies = null;
        private string[] HideColName = null;
        private int roundCount = 2;

        public StatResult(string title, string[] columns, string[][] rows)
        {
            _title = title;
            _columns = columns;
            _rows = rows;
            HideColName = new string[] { "累计覆盖", "变化", "时间", "相邻时次", "累计最大" };
        }

        #region IStatResult Members

        public string Title
        {
            get { return _title != null ? UpdateTitle() : string.Empty; }
        }

        private string UpdateTitle()
        {
            int start = _title.IndexOf("统计文件：");
            if (start == -1)
                start = _title.IndexOf("统计文件:");
            if (start == -1)
                return _title;
            start += 5;
            int end = _title.IndexOf("统计日期：");
            if (end == -1)
                end = _title.IndexOf("统计日期;");
            if (end == -1)
                return _title;
            int dateEnd = _title.IndexOf("统计区域：");
            if (dateEnd == -1)
                dateEnd = _title.IndexOf("统计区域:");
            if (dateEnd == -1)
                return _title;
            int length = end - start;
            string FileNamesTitle = _title.Substring(start, length);
            string FilenamesStatTime = _title.Substring(end, dateEnd - end - 5);
            string[] fileList = FileNamesTitle.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (fileList == null)
                return _title;
            string satellite = string.Empty;
            string sensor = string.Empty;
            string satelliteLast = string.Empty;
            string sensorLast = string.Empty;
            return _title.Replace(FileNamesTitle, FileNamesTitle + "卫星：" + satellite + "  " + "传感器：" + sensor + "\r\n").Replace("\r\n", "\n");
        }

        public string[] Columns
        {
            get { return _columns; }
        }

        public string[][] Rows
        {
            get { return _rows; }
        }

        private void GetHideCols(bool singleFile, IStatResult result)
        {
            if (_indexies == null)
                _indexies = new List<int>();
            else
                _indexies.Clear();
            if (singleFile)
            {
                int length = result.Columns.Length;
                int hideCount = HideColName.Length;
                for (int j = 0; j < length; j++)
                {
                    for (int i = 0; i < hideCount; i++)
                    {
                        if (result.Columns[j].IndexOf(HideColName[i]) != -1)
                        {
                            _indexies.Add(j);
                            break;
                        }
                    }
                }
            }
        }

        private string SetRound(string colContext, out bool isdouble)
        {
            double result = 0;
            if (!double.TryParse(colContext, out result))
            {
                isdouble = false;
                return colContext;
            }
            else
            {
                isdouble = true;
                return Math.Round(result, roundCount).ToString("#.00");
            }
        }

        #endregion
    }
}
