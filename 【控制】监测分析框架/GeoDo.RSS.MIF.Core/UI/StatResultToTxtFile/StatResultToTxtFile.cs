using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class StatResultToTxtFile : IDisposable
    {
        private List<string> _strList;
        public int Zoom = 1;

        public StatResultToTxtFile()
        {
            _strList = new List<string>();
        }

        public void WriteResultToTxt(string strInfo)
        {
            _strList.Add(strInfo);
        }

        public void WriteResultToTxt(string[] strInfos, string fenge)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strInfos.Length; i++)
            {
                string str = strInfos[i];
                str = (str == null ? "" : str);
                sb.Append((str.PadRight(15, ' ') + fenge));
            }
            _strList.Add(sb.ToString());
        }

        public void WriteResultToTxt(IStatResult statResult)
        {
            StringBuilder colStrBud = new StringBuilder();
            foreach (string s in statResult.Columns)
            {
                colStrBud.Append(s.PadRight(15, ' ') + "\t");
            }
            colStrBud.Append("\n");
            _strList.Add(colStrBud.ToString());
            int col = statResult.Rows[0].Count();
            int row = statResult.Rows.Length;
            for (int i = 0; i < row; i++)
            {
                WriteResultToTxt(statResult.Rows[i], "\t");
                _strList.Add("\n");
            }
        }

        public void WriteResultToTxt(StatResultItem item)
        {
            _strList.Add(item.Name.PadRight(15, ' ') + "\t" + (item.Value / Zoom).ToString("0.##") + "\n");
        }

        public void WriteResultToTxt(StatResultItem[] resultItems)
        {
            foreach (StatResultItem item in resultItems)
            {
                _strList.Add(item.Name.PadRight(15, ' ') + "\t" + (item.Value / Zoom).ToString("0.##") + "\n");
            }
        }

        public bool SaveFile(string fileName)
        {
            try
            {
                File.WriteAllLines(fileName, _strList.ToArray());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        void IDisposable.Dispose()
        {
            _strList.Clear();
            _strList = null;
        }
    }
}
