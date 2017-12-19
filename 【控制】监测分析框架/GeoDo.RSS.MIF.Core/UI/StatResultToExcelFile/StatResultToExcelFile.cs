using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Core
{
    public partial class StatResultToExcelFile : Form, IStatResultWindow
    {
        private int initBRow = 4;
        private int initBCol = 2;
        private int bRow = 1;
        private int bCol = 1;
        private int currentBRow = 0;
        private int currentBCol = 0;
        private masExcelDrawStatType _masExcelDrawStatType = masExcelDrawStatType.xlColumnClustered;
        private List<int> _indexies = null;
        private string[] HideColName = null;
        private int currentCol = 0;
        private int roundCount = 2;
        private WinExcelControl winExcelControl1;
        public WinExcelControl WinExcelControl{get { return winExcelControl1; }}
        //
        private int _startCol = 0;
        bool _singleFile = false;
        string _windowText;
        IStatResult _result;
        int _displayCol;
        bool _dislayDateLabel = false;
        bool _isTotal = false;
        int _totalAll = 0;          //是否合计不在统计图中显示的列
        int _statImage;
        enumAppendExcelType _appendType = enumAppendExcelType.Base;
        bool _needLoad = false;
        public int Zoom = 1;
        private byte _baseCol = 1;
        private List<RowDisplayDef> _rowRulers = null;
        int _uionBCol = 0;

        public StatResultToExcelFile()
        {
            InitializeComponent();
            //
            HideColName = new string[] { "累计覆盖", "变化", "时间", "相邻时次", "累计最大" };
            bRow = initBRow;
            bCol = initBRow;
        }

        public void Init()
        {
            CreateExcelWnd();
        }

        public void Display()
        {
            if (_needLoad)
            {
                if (!string.IsNullOrEmpty(_windowText))
                    Text = _windowText;
                GetHideCols(_singleFile, _result);
                switch (_appendType)
                {
                    case enumAppendExcelType.Base:
                        Append(_singleFile, _result, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayCol:
                        Append(_singleFile, _result, _displayCol, false, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayColAllTotal:
                        Append(_singleFile, _result, _displayCol, false, _isTotal, _totalAll, _statImage, _baseCol);
                        break;
                    case enumAppendExcelType.DisplayLabel:
                        Append(_singleFile, _result, _dislayDateLabel, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayColLable:
                        Append(_singleFile, _result, _displayCol, _dislayDateLabel, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayColLableAndRowRuler:
                        Append(_singleFile, _result, _displayCol, _dislayDateLabel, _isTotal, _statImage, _rowRulers);
                        break;
                }
                winExcelControl1.UnLock();
                _needLoad = false;
            }
            this.Refresh();
        }

        public bool SaveFile(string filename)
        {
            if (File.Exists(filename))
            {
                if (MessageBox.Show("文件[" + filename + "]" + "已存在,是否替换?", "文件已存在", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(filename);
                        winExcelControl1.SaveXLS(filename);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            else
                winExcelControl1.SaveXLS(filename);

            winExcelControl1.CloseControl();
            this.Close();
            return true;
        }

        public void SetActiveExcel()
        {
            winExcelControl1.DoActive();
        }

        public void Free()
        {
            winExcelControl1.CloseControl();
        }

        public void CreateExcelWnd()
        {
            winExcelControl1 = new WinExcelControl();
            winExcelControl1.Dock = DockStyle.Fill;
            this.Controls.Add(winExcelControl1);
            winExcelControl1.OnNewCreateXls();
        }

        public void DoActive()
        {
            winExcelControl1.DoActive();
        }

        #region IStatResultWindow Members

        public void Add(bool singleFile, string windowText, IStatResult result, bool isTotal, int statImage)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.Base;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int statImage)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayCol;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool dislayDateLabel, bool isTotal, int statImage)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _dislayDateLabel = dislayDateLabel;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColLable;
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int totalAll, int statImage)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _isTotal = isTotal;
            _totalAll = totalAll;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColAllTotal;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool isTotal, int totalAll, int statImage, byte baseCol)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _isTotal = isTotal;
            _totalAll = totalAll;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColAllTotal;
            _baseCol = baseCol;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int startCol, int displayCol, bool isTotal, int totalAll, int statImage, byte baseCol, int chartType)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _isTotal = isTotal;
            _totalAll = totalAll;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColAllTotal;
            _startCol = startCol;
            _baseCol = baseCol;
            _masExcelDrawStatType = chartType == 1 ? masExcelDrawStatType.xlColumnClustered : masExcelDrawStatType.xlLine;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, bool dislayDateLabel, bool isTotal, int statImage)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _dislayDateLabel = dislayDateLabel;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayLabel;
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int startCol, int displayCol, bool dislayDateLabel, bool isTotal, int statImage, List<RowDisplayDef> rowRuler, int uionBCol, byte baseCol)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _dislayDateLabel = dislayDateLabel;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColLableAndRowRuler;
            _rowRulers = rowRuler;
            _startCol = startCol;
            _baseCol = baseCol;
            _uionBCol = uionBCol;
            Display();
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool dislayDateLabel, bool isTotal, int statImage, List<RowDisplayDef> rowRuler)
        {
            _singleFile = singleFile;
            _windowText = windowText;
            _result = result;
            _displayCol = displayCol;
            _dislayDateLabel = dislayDateLabel;
            _isTotal = isTotal;
            _statImage = statImage;
            _needLoad = true;
            _appendType = enumAppendExcelType.DisplayColLableAndRowRuler;
            _rowRulers = rowRuler;
            Display();
        }


        public void Append(bool singleFile, IStatResult result, bool isTotal, int statImage)
        {
            if (result == null)
                return;
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                    winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + result.Columns.Length - 1 - _indexies.Count, bCol, 1, masExcelAlignType.General, null);
            }
            if (statImage != 0)
                winExcelControl1.DrawStat(statBRow - 1, initBCol, bRow, initBCol + result.Columns.Length - 1 - _indexies.Count, "图表", Text.Replace(".", ""), "",
                    result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                    _masExcelDrawStatType, false, true);
            bRow += 3;
        }

        public void Append(bool singleFile, IStatResult result, int displayCol, bool isTotal, int statImage)
        {
            if (result == null)
                return;
            if (displayCol == -1)
            {
                Append(singleFile, result, false, isTotal, 1);
                return;
            }
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                    winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), bCol, 1, masExcelAlignType.General, null);
            }
            if (displayCol > 0)
            {
                if (statImage != 0 && !_indexies.Contains(displayCol))
                    winExcelControl1.DrawStat(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), "图表", Text.Replace(".", ""), "",
                        result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                        _masExcelDrawStatType, false, true);
            }
            bRow += 3;
        }

        public void Append(bool singleFile, IStatResult result, int displayCol, bool displayDataLabel, bool isTotal, int isTotalAll, int statImage, byte baseCol)
        {
            if (result == null)
                return;
            if (displayCol == -1)
            {
                Append(singleFile, result, displayDataLabel, isTotal, 1);
                return;
            }
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;

            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    //winExcelControl1.SetCellValue(bRow, bCol, row.Length,1, masExcelAlignType.General, row, "01");
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                {
                    if (isTotalAll == 1)
                        winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + result.Columns.Length - 1 - _indexies.Count, bCol, 1, masExcelAlignType.General, null);
                    else
                        winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), bCol, 1, masExcelAlignType.General, null);
                }
            }
            if (displayCol > 0)
            {
                if (statImage != 0 && !_indexies.Contains(displayCol))
                {
                    winExcelControl1.DrawStat(statBRow - 1, _startCol == 0 ? initBCol : _startCol, bRow, (_startCol == 0 ? initBCol : _startCol) + displayCol - GetHideColCount(displayCol), "图表", Text.Replace(".", ""), "",
                    result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                    _masExcelDrawStatType, displayDataLabel, baseCol, true);
                }
            }

            bRow += 3;
        }

        public void Append(bool singleFile, IStatResult result, int displayCol, bool displayDataLabel, bool isTotal, int statImage)
        {
            if (result == null)
                return;
            if (displayCol == -1)
            {
                Append(singleFile, result, displayDataLabel, isTotal, 1);
                return;
            }
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    //winExcelControl1.SetCellValue(bRow, bCol, row.Length,1, masExcelAlignType.General, row, "01");
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                    winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), bCol, 1, masExcelAlignType.General, null);
            }
            if (displayCol > 0)
            {
                if (statImage != 0 && !_indexies.Contains(displayCol))
                {
                    winExcelControl1.DrawStat(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), "图表", Text.Replace(".", ""), "",
                    result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                    _masExcelDrawStatType, displayDataLabel, true);
                }
            }

            bRow += 3;
        }

        private string SetRound(string colContext)
        {
            double result = 0;
            if (!double.TryParse(colContext, out result))
                return colContext;
            else
                return Math.Round(result / Zoom, roundCount).ToString("#.00");
        }

        private int GetHideColCount(int displayCol)
        {
            int result = 0;
            int length = _indexies.Count;
            for (int i = 0; i < length; i++)
            {
                if (_indexies[i] <= displayCol)
                    result++;
            }
            return result;
        }


        public void Append(bool singleFile, IStatResult result, bool displayDataLabel, bool isTotal, int statImage)
        {
            if (result == null)
                return;
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                    winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + result.Columns.Length - 1 - _indexies.Count, bCol, 1, masExcelAlignType.General, null);
            }
            if (statImage != 0)
                winExcelControl1.DrawStat(statBRow - 1, initBCol, bRow, initBCol + result.Columns.Length - 1 - _indexies.Count, "图表", Text.Replace(".", ""), "",
                    result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                    _masExcelDrawStatType, displayDataLabel, true);
            bRow += 3;
        }

        private void Append(bool singleFile, IStatResult result, int displayCol, bool displayDataLabel, bool isTotal, int statImage, List<RowDisplayDef> rowRuler)
        {
            if (result == null)
                return;
            if (displayCol == -1)
            {
                Append(singleFile, result, displayDataLabel, isTotal, 1);
                return;
            }
            bCol = initBCol;
            currentBRow = bRow;
            currentBCol = bCol;
            currentCol = -1;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            winExcelControl1.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 - _indexies.Count, 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    if (_indexies.Contains(currentCol))
                        continue;
                    winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    winExcelControl1.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
                    bCol++;
                }
            }
            //
            int statBRow = bRow + 1;
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    currentCol = -1;
                    bCol = initBCol;
                    bRow++;
                    //winExcelControl1.SetCellValue(bRow, bCol, row.Length,1, masExcelAlignType.General, row, "01");
                    foreach (string colContext in row)
                    {
                        currentCol++;
                        if (_indexies.Contains(currentCol))
                            continue;
                        winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), colContext.IndexOf("日") == -1 ? null : "@");
                        bCol++;
                    }
                    SupplySpace(singleFile, result);
                }
                if (isTotal && result.Rows.Length != 1)
                {
                    winExcelControl1.SetTotalCellValue(statBRow - 1, initBCol, bRow, initBCol + displayCol - GetHideColCount(displayCol), bCol, 1, masExcelAlignType.General, null);
                }

                List<int> displayNum = new List<int>();
                for (int i = 0; i < _rowRulers.Count; i++)
                {
                    if (_rowRulers[i].calcAverage)
                    {
                        bRow += 1;
                        winExcelControl1.SetCellValue(bRow, _startCol == 0 ? initBCol : _startCol, 1, masExcelAlignType.General, _rowRulers[i].PageName + "\n均值", null);
                        for (int j = 1; j < result.Columns.Length; j++)
                            winExcelControl1.SetCellValue(bRow, (_startCol == 0 ? initBCol : _startCol) + j, 1, masExcelAlignType.General, AverageCell((_startCol == 0 ? initBCol : _startCol) + 1, (_startCol == 0 ? initBCol : _startCol) + displayCol - GetHideColCount(displayCol), statBRow + _rowRulers[i].DisplayRow[0]), null);

                        bRow += 1;
                        winExcelControl1.SetCellValue(bRow, _startCol == 0 ? initBCol : _startCol, 1, masExcelAlignType.General, _rowRulers[i].PageName + "\n距平", null);
                        for (int j = 1; j < result.Columns.Length; j++)
                            winExcelControl1.SetCellValue(bRow, (_startCol == 0 ? initBCol : _startCol) + j, 1, masExcelAlignType.General, AverageDenCell(statBRow + _rowRulers[i].DisplayRow[0], (_startCol == 0 ? initBCol : _startCol) + j, bRow - 1), null);

                        bRow += 1;
                        winExcelControl1.SetCellValue(bRow, _startCol == 0 ? initBCol : _startCol, 1, masExcelAlignType.General, _rowRulers[i].PageName + "\n距平百分比", null);
                        for (int j = 1; j < result.Columns.Length; j++)
                            winExcelControl1.SetCellValue(bRow, (_startCol == 0 ? initBCol : _startCol) + j, 1, masExcelAlignType.General, AverageDenPeCell(bRow - 1, (_startCol == 0 ? initBCol : _startCol) + j, bRow - 2), null);
                        displayNum.Add(bRow - statBRow);
                        _rowRulers.Add(new RowDisplayDef(displayNum, _rowRulers[i].PageName + "距平"));
                    }
                }

            }
            //statBRow = initBRow;
            if (displayCol > 0)
            {
                if (statImage != 0 && !_indexies.Contains(displayCol))
                {
                    for (int i = 0; i < _rowRulers.Count; i++)
                    {

                        winExcelControl1.DrawStat(initBRow + _rowRulers[i].DisplayRow[0], _startCol == 0 ? initBCol : _startCol, initBRow - 1 + _rowRulers[i].DisplayRow[0] + _rowRulers[i].DisplayRow.Count, (_startCol == 0 ? initBCol : _startCol) + displayCol - GetHideColCount(displayCol), _rowRulers[i].PageName, Text.Replace(".", "").Replace("统计结果", _rowRulers[i].PageName.Replace("分析", "") + "统计图") + "\n" + result.Title.Replace("统计周期：", ""), "",
                                                  result.Columns[1].IndexOf("(") != -1 ? (result.Columns[1].Substring(result.Columns[1].IndexOf("(") + 1, result.Columns[1].IndexOf(")") - result.Columns[1].IndexOf("(") - 1)) : "",
                                                  _masExcelDrawStatType, displayDataLabel && _rowRulers[i].DataLabel, true, statBRow - 1, _uionBCol == 0 ? (_startCol == 0 ? initBCol : _startCol) : _uionBCol, _baseCol);

                    }
                }
            }
            bRow += 3;
        }

        private string AverageDenPeCell(int averDenRow, int col, int averRow)
        {
            return "=ROUND(" + GetColValue(col) + averDenRow + "/" + GetColValue(col) + averRow + ",3)";
        }

        private string AverageDenCell(int vRow, int col, int averRow)
        {
            return "=ROUND(" + GetColValue(col) + vRow + "-" + GetColValue(col) + averRow + ",1)";
        }

        private string AverageCell(int bcol, int endcol, int row)
        {
            return "=ROUND(AVERAGE(" + GetColValue(bcol) + row + ":" + GetColValue(endcol) + row + "),1)";
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

        private void SupplySpace(bool singleFile, IStatResult result)
        {
            int length = result.Columns.Length + initBCol - bCol - _indexies.Count;
            for (int surplusCol = 0; surplusCol < length; surplusCol++)
            {
                winExcelControl1.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, "", null);
                bCol++;
            }
        }

        public void Unlock()
        {
            //winExcelControl1.UnLock();
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

        /// <summary>
        /// 默认:masExcelDrawStatType.xl3DColumn
        /// </summary>
        /// <param name="masExcelDrawStatType"></param>
        public void SetExcelDrawStatType(masExcelDrawStatType masExcelDrawStatType)
        {
            _masExcelDrawStatType = masExcelDrawStatType;
        }

        #endregion


        public string Title
        {
            get { return "统计"; }
        }

        public object ActiveObject
        {
            get { return null; }
        }
    }

    enum enumAppendExcelType
    {
        Base,
        DisplayCol,
        DisplayColAllTotal,
        DisplayLabel,
        DisplayColLable,
        DisplayColLableAndRowRuler
    }
}
