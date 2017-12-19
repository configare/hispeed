using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public class ExportStaticResultToChart
    {
        public static string ExportStaticResult(string fileName, IStatResult result, masExcelDrawStatType drawType, string title, int colOffset, bool hasLegend, string xName, string yName)
        {
            try
            {
                using (StatResultToChartInExcelFile excelControl = new StatResultToChartInExcelFile())
                {
                    excelControl.Init(drawType);
                    excelControl.Add(title, result, true, colOffset, false, xName, yName);
                    if (!fileName.ToUpper().EndsWith(".XLSX"))
                        fileName += ".XLSX";
                    excelControl.SaveFile(fileName);
                }
            }
            catch(Exception)
            {
                using (StatResultToTxtFile txtControl = new StatResultToTxtFile())
                {
                    if (!fileName.ToUpper().EndsWith(".TXT"))
                        fileName += ".TXT";
                    txtControl.WriteResultToTxt(title + "\n");
                    txtControl.WriteResultToTxt("统计日期：" + DateTime.Today.Date.ToShortDateString() + "\n");
                    txtControl.WriteResultToTxt(result);
                    bool isSave = txtControl.SaveFile(fileName);
                    if (!isSave)
                        return String.Empty;
                }
            }
            return fileName;
        }
    }
}
