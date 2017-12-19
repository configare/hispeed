using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    //判识水体面积统计
    public class SubProductAnalysisFldTStat : CmaMonitoringSubProduct
    {
        public SubProductAnalysisFldTStat(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TSTAAlgorithmByDB")
            {
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "TSTAAlgorithmByFiles")
            {
                string outFileIdentifyInstance = _argumentProvider.GetArg("OutFileIdentifyInstance").ToString();
                if (!string.IsNullOrEmpty(outFileIdentifyInstance))
                    _argumentProvider.SetArg("OutFileIdentify", outFileIdentifyInstance);
                TimeStatInfo timeStatInfo = new TimeStatInfo();
                Dictionary<string, Dictionary<string, StatInfoBase[]>> result = timeStatInfo.GetStatInfo(_subProductDef, _argumentProvider, progressTracker);
                if (progressTracker != null)
                    progressTracker.Invoke(90, "开始进行单文件面积整合,请稍后...");
                if (result == null || result.Count == 0)
                    return null;
                Dictionary<enumStatCompoundType, IStatResult> statResult = IntegrateInfo(result);
                string[] files = GetStringArray("SelectedPrimaryFiles");

                string outFileIdentify = GetStringArgument("OutFileIdentify");
                Dictionary<enumStatCompoundType, string> outFileIdentifys = new Dictionary<enumStatCompoundType, string>();
                outFileIdentifys.Add(enumStatCompoundType.最大, outFileIdentify.Substring(0, 3) + "X");
                outFileIdentifys.Add(enumStatCompoundType.最小, outFileIdentify.Substring(0, 3) + "N");
                IExtractResultArray array = new ExtractResultArray("LST");

                enumStatCompoundType[] keys = statResult.Keys.ToArray();
                foreach (enumStatCompoundType key in keys)
                {
                    List<RowDisplayDef> rowRulers = new List<RowDisplayDef>();
                    List<int> displayRowNum = new List<int>();
                    int count = -1;
                    string befStr = statResult[key].Rows[0].First();
                    for (int row = 0; row < statResult[key].Rows.Length; row++)
                    {
                        count++;
                        if (statResult[key].Rows[row].First() == befStr)
                        {
                            displayRowNum.Add(count + 3);
                            continue;
                        }
                        statResult[key] = CalcAvg(statResult[key], displayRowNum);
                        AddRowRuler(ref rowRulers, displayRowNum, statResult[key].Rows[row - 1][0], true, false, false);
                        befStr = statResult[key].Rows[row].First();
                        displayRowNum.Add(count + 3);
                    }
                    statResult[key] = CalcAvg(statResult[key], displayRowNum);
                    AddRowRuler(ref rowRulers, displayRowNum, befStr, true, false, false);
                    array.Add(new FileExtractResult("FLD", StatResultToFile(files, statResult[key], "FLD", outFileIdentifys[key], "", "", 1, rowRulers, false,false, 7, 1,3,1)));
                }
                return array;
            }
            return null;
        }

        private IStatResult CalcAvg(IStatResult srcStatResult, List<int> displayRowNum)
        {
            string temp;
            float value = 0;
            List<string> newColumns = new List<string>();
            newColumns.AddRange(srcStatResult.Columns);
            List<string[]> newRows = new List<string[]>();
            newRows.AddRange(srcStatResult.Rows);
            List<string> rowTemp = new List<string>();
            int startCol = 0;
            int addColIndex = srcStatResult.Rows[displayRowNum[0] - 3].Length;
            for (int col = 0; col < srcStatResult.Columns.Length; col++)
            {
                if (col >= srcStatResult.Rows[displayRowNum[0] - 3].Length)
                    break;
                temp = srcStatResult.Rows[displayRowNum[0] - 3][col];
                if (float.TryParse(temp, out value))
                {
                    foreach (int rowNum in displayRowNum)
                    {
                        rowTemp.AddRange(newRows[rowNum - 3]);
                        rowTemp.Add(AverageCell(displayRowNum[0] + 4, displayRowNum[displayRowNum.Count - 1] + 4, col + startCol));
                        rowTemp.Add(AverageDenCell(rowNum + 4, col + startCol, addColIndex + startCol));
                        rowTemp.Add(AverageDenPeCell(rowNum + 4, addColIndex + 1 + startCol, addColIndex + startCol));
                        newRows[rowNum - 3] = rowTemp.ToArray();
                        rowTemp.Clear();
                    }
                    addColIndex += 3;
                    if (!newColumns.Contains(newColumns[col] + "均值"))
                        newColumns.AddRange(new string[] { newColumns[col] + "均值", newColumns[col] + "距平",  newColumns[col] + "距平率" });
                }
                else
                    startCol++;
            }
            return new StatResult("", newColumns.ToArray(), newRows.ToArray());
        }

        private string AverageDenPeCell(int averDenRow, int averDenCol, int averCol)
        {
            return "=ROUND(" + GetColValue(averDenCol) + averDenRow + "/" + GetColValue(averCol) + averDenRow + ",3)";
        }

        private string AverageDenCell(int srcRow, int srcCol, int averCol)
        {
            return "=ROUND(" + GetColValue(srcCol) + srcRow + "-" + GetColValue(averCol) + srcRow + ",1)";
        }

        private string AverageCell(int brow, int endrow, int col)
        {
            return "=ROUND(AVERAGE(" + GetColValue(col) + brow + ":" + GetColValue(col) + endrow + "),1)";
        }

        private string GetColValue(int index)
        {
            index--;
            string column = string.Empty;
            do
            {
                if (column.Length > 0)
                {
                    index--;
                }
                column = ((char)(index % 26 + (int)'A')).ToString() + column;
                index = (int)((index - index % 26) / 26);
            }
            while (index > 0);
            return column;
        }


        private void AddRowRuler(ref List<RowDisplayDef> rowRulers, List<int> displayRowNum, string pagename, bool dataLabel, bool average, bool max)
        {
            RowDisplayDef rowRuler = new RowDisplayDef();
            rowRuler.DisplayRow.AddRange(displayRowNum);
            rowRuler.PageName = pagename;
            rowRuler.DataLabel = dataLabel;
            rowRulers.Add(rowRuler);
            rowRuler.calcAverage = average;
            rowRuler.calcMaxValue = max;
            displayRowNum.Clear();
        }

        private Dictionary<enumStatCompoundType, IStatResult> IntegrateInfo(Dictionary<string, Dictionary<string, StatInfoBase[]>> srcResult)
        {
            Dictionary<string, Dictionary<enumStatCompoundType, IStatResult>> dstResult = new Dictionary<string, Dictionary<enumStatCompoundType, IStatResult>>();
            StatDimClass dimClass = _argumentProvider.GetArg("StatDim") == null ? null : _argumentProvider.GetArg("StatDim") as StatDimClass;
            switch (dimClass.DayMosaicType)
            {
                case enumStatDayMosaicType.面积:
                    foreach (string dimKey in srcResult.Keys)
                    {
                        switch (dimClass.CompoundType)
                        {
                            case enumStatCompoundType.全部:
                                GetDstResultByCompoundArea(srcResult, dstResult, dimKey, enumStatCompoundType.最大);
                                GetDstResultByCompoundArea(srcResult, dstResult, dimKey, enumStatCompoundType.最小);
                                break;
                            case enumStatCompoundType.最大:
                                GetDstResultByCompoundArea(srcResult, dstResult, dimKey, enumStatCompoundType.最大);
                                break;
                            case enumStatCompoundType.最小:
                                GetDstResultByCompoundArea(srcResult, dstResult, dimKey, enumStatCompoundType.最小);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case enumStatDayMosaicType.空间合成:

                    break;
                default:
                    break;
            }
            if (dstResult == null)
                return null;
            Dictionary<enumStatCompoundType, IStatResult> resultDic = new Dictionary<enumStatCompoundType, IStatResult>();
            Dictionary<enumStatCompoundType, IStatResult> tempDic = null;
            IStatResult tempResult = null;
            string proTempResult = dstResult.Keys.First();
            foreach (string item in dstResult.Keys)
            {
                tempDic = dstResult[item];
                foreach (enumStatCompoundType compundType in tempDic.Keys)
                {
                    switch (compundType)
                    {
                        case enumStatCompoundType.最大:
                            if (resultDic.ContainsKey(enumStatCompoundType.最大))
                                resultDic[enumStatCompoundType.最大] = MergeResult(resultDic[enumStatCompoundType.最大], tempDic[enumStatCompoundType.最大], proTempResult, item);
                            else
                                resultDic.Add(enumStatCompoundType.最大, AddDate(tempDic[enumStatCompoundType.最大], item));
                            break;
                        case enumStatCompoundType.最小:
                            if (resultDic.ContainsKey(enumStatCompoundType.最小))
                                resultDic[enumStatCompoundType.最小] = MergeResult(resultDic[enumStatCompoundType.最小], tempDic[enumStatCompoundType.最小], proTempResult, item);
                            else
                                resultDic.Add(enumStatCompoundType.最小, AddDate(tempDic[enumStatCompoundType.最小], item));
                            break;
                    }
                }
                proTempResult = item;
            }
            if (resultDic.ContainsKey(enumStatCompoundType.最小))
            {
                List<string> colums = new List<string>();
                foreach (string item in resultDic[enumStatCompoundType.最小].Columns)
                    colums.Add(item.Replace("最大覆盖", "最小覆盖"));
                resultDic[enumStatCompoundType.最小] = new StatResult(resultDic[enumStatCompoundType.最小].Title, colums.ToArray(), resultDic[enumStatCompoundType.最小].Rows);
            }
            return resultDic;
        }

        private IStatResult AddDate(IStatResult curStatResult, string date)
        {
            List<string> columns = new List<string>();
            List<string[]> statRows = new List<string[]>();
            List<string> rowTemp = new List<string>();
            columns.AddRange(curStatResult.Columns);
            columns.Insert(1, "时间");
            for (int i = 0; i < curStatResult.Rows.Length; i++)
            {
                rowTemp.AddRange(curStatResult.Rows[i]);
                rowTemp.Insert(1, date);
                statRows.Add(rowTemp.ToArray());
                rowTemp.Clear();
            }
            StringArrayComparer comparer = new StringArrayComparer(true);
            statRows.Sort(comparer);
            return new StatResult("", columns.ToArray(), statRows.ToArray());
        }

        private IStatResult MergeResult(IStatResult befStatResult, IStatResult lastStatResult, string proDate, string date)
        {
            List<string> columns = new List<string>();
            List<int> AddColumnIndex = new List<int>();
            Dictionary<int, int> DYColumnIndex = new Dictionary<int, int>();
            List<string[]> statRows = new List<string[]>();
            columns.AddRange(befStatResult.Columns.ToArray());
            bool dySuccess = false;
            for (int i = 0; i < lastStatResult.Columns.Length; i++)
            {
                dySuccess = false;
                for (int j = 0; j < befStatResult.Columns.Length; j++)
                {
                    if (lastStatResult.Columns[i] == befStatResult.Columns[j])
                    {
                        DYColumnIndex.Add(j, i);
                        dySuccess = true;
                        break;
                    }
                }
                if (!dySuccess)
                {
                    columns.Add(lastStatResult.Columns[i]);
                    AddColumnIndex.Add(i);
                }
            }
            bool containtTime = columns.Contains("时间");
            List<string> rowTemp = new List<string>();
            if (!containtTime)
            {
                columns.Insert(1, "时间");
                for (int i = 0; i < befStatResult.Rows.Length; i++)
                {
                    rowTemp.AddRange(befStatResult.Rows[i]);
                    rowTemp.Insert(1, proDate);
                    statRows.Add(rowTemp.ToArray());
                    rowTemp.Clear();
                }
            }
            else
                statRows.AddRange(befStatResult.Rows);
            for (int i = 0; i < lastStatResult.Rows.Length; i++)
            {
                for (int index = 0; index < columns.Count; index++)
                {
                    if (DYColumnIndex.ContainsKey(index))
                    {
                        rowTemp.Add(lastStatResult.Rows[i][DYColumnIndex[index]]);
                        continue;
                    }
                    if (AddColumnIndex.Contains(index))
                        rowTemp.Add(lastStatResult.Rows[i][index]);
                }
                rowTemp.Insert(1, date);
                statRows.Add(rowTemp.ToArray());
                rowTemp.Clear();
            }
            StringArrayComparer comparer = new StringArrayComparer(true);
            statRows.Sort(comparer);
            return new StatResult("", columns.ToArray(), statRows.ToArray());
        }

        private void GetDstResultByCompoundArea(Dictionary<string, Dictionary<string, StatInfoBase[]>> srcResult, Dictionary<string, Dictionary<enumStatCompoundType, IStatResult>> dstResult, string dimKey, enumStatCompoundType statCompoundType)
        {
            Dictionary<enumStatCompoundType, IStatResult> tempResult = null; ;
            Dictionary<enumStatCompoundType, StatInfoBase> sibDic = new Dictionary<enumStatCompoundType, StatInfoBase>();
            StatInfoBase daySibTemp = null;
            Dictionary<string, StatInfoBase[]> daysInfoTemp = srcResult[dimKey];
            StatInfoBase dimSibTemp = srcResult[dimKey].First().Value[0];
            foreach (var dayKey in daysInfoTemp.Keys)
            {
                if (daysInfoTemp[dayKey].Length > 1)
                {
                    daySibTemp = daysInfoTemp[dayKey][0];
                    for (int index = 1; index < daysInfoTemp[dayKey].Length; index++)
                        CompareStatBaseInfoBySum(ref daySibTemp, daysInfoTemp[dayKey][index - 1], statCompoundType);
                }
                else
                    CompareStatBaseInfoBySum(ref dimSibTemp, daysInfoTemp[dayKey][0], statCompoundType);
            }
            tempResult = new Dictionary<enumStatCompoundType, IStatResult>();
            tempResult.Add(statCompoundType, dimSibTemp.DayStatResult);
            if (dstResult.ContainsKey(dimKey))
                dstResult[dimKey].Add(statCompoundType, dimSibTemp.DayStatResult);
            else
                dstResult.Add(dimKey, tempResult);
        }

        private void CompareStatBaseInfoBySum(ref StatInfoBase compoundSib, StatInfoBase compareSib, enumStatCompoundType statCompoundType)
        {
            int rowLength = compoundSib.DayStatResult.Rows.Length;
            int colLength = compoundSib.DayStatResult.Columns.Length; ;
            string[] tempSib = null;
            float areaTemp = 0;
            float compoundSum = 0;
            float compareSum = 0;
            for (int row = 0; row < rowLength; row++)
            {
                tempSib = compoundSib.DayStatResult.Rows[row];
                for (int col = 0; col < colLength; col++)
                    compoundSum += float.TryParse(tempSib[col], out areaTemp) ? areaTemp : 0;
                tempSib = compareSib.DayStatResult.Rows[row];
                for (int col = 0; col < colLength; col++)
                    compareSum += float.TryParse(tempSib[col], out areaTemp) ? areaTemp : 0;
            }
            switch (statCompoundType)
            {
                case enumStatCompoundType.全部:
                    break;
                case enumStatCompoundType.最大:
                    if (compareSum > compoundSum)
                        compoundSib = compareSib;
                    break;
                case enumStatCompoundType.最小:
                    if (compareSum < compoundSum)
                        compoundSib = compareSib;
                    break;
            }
        }
    }

    public class StringArrayComparer : IComparer<string[]>
    {
        private bool bASC;
        public StringArrayComparer(bool asceding)
        {
            bASC = asceding;
        }

        public int Compare(string[] bef, string[] last)
        {
            if (!bASC)
            {
                //降序  
                string[] temp;
                temp = last;
                last = bef;
                bef = temp;
            }

            int flg = -1;
            flg = Comparer<string>.Default.Compare(bef[0], last[0]);
            if (flg == 0)
            {
                string befTemp = bef[1].Replace("上旬", "010").Replace("中旬", "020").Replace("下旬", "030");
                string lastTemp = last[1].Replace("上旬", "010").Replace("中旬", "020").Replace("下旬", "030");
                flg = Comparer<string>.Default.Compare(befTemp, lastTemp);
            }
            return flg;
        }
    }

    public class AvgCalcInfo
    {
        public float AVG;
        public float JP;
        public float JPL;

        public AvgCalcInfo()
        { }
    }
}
