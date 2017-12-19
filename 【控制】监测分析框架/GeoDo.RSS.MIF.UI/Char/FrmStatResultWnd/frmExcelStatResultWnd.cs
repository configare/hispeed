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

namespace Geodo.RSS.MIF.UI
{
    public partial class frmExcelStatResultWnd : Form, IStatResultWindow
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

        //
        bool _singleFile = false;
        string _windowText;
        IStatResult _result;
        int _displayCol;
        bool _dislayDateLabel = false;
        bool _isTotal = false;
        int _statImage;
        enumAppendExcelType _appendType = enumAppendExcelType.Base;
        bool _needLoad = false;
        //

        public frmExcelStatResultWnd()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(frmExcelStatResultWnd_FormClosing);
            //
            HideColName = new string[] { "累计覆盖", "变化", "时间", "相邻时次", "累计最大" };
            bRow = initBRow;
            bCol = initBRow;
            Load += new EventHandler(frmExcelStatResultWnd_Load);
        }

        void frmExcelStatResultWnd_Load(object sender, EventArgs e)
        {
            try
            {
                CreateExcelWnd();
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
                        case enumAppendExcelType.DisplayLabel:
                            Append(_singleFile, _result, _dislayDateLabel, _isTotal, _statImage);
                            break;
                        case enumAppendExcelType.DisplayColLable:
                            Append(_singleFile, _result, _displayCol, _dislayDateLabel, _isTotal, _statImage);
                            break;
                    }
                    winExcelControl1.UnLock();
                    _needLoad = false;
                }
                this.Refresh();
            }
            catch (Exception ex)
            {
                frmTextStatResultWnd frm = new frmTextStatResultWnd();
                switch (_appendType)
                {
                    case enumAppendExcelType.Base:
                        frm.Add(_singleFile, _windowText, _result, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayCol:
                        frm.Add(_singleFile, _windowText, _result, _displayCol, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayLabel:
                        frm.Add(_singleFile, _windowText, _result, _dislayDateLabel, _isTotal, _statImage);
                        break;
                    case enumAppendExcelType.DisplayColLable:
                        frm.Add(_singleFile, _windowText, _result, _displayCol, _dislayDateLabel, _isTotal, _statImage);
                        break;
                }
                this.Close();
                frm.Show();
            }
        }

        public void SetActiveExcel()
        {
            winExcelControl1.DoActive();
        }

        void frmExcelStatResultWnd_FormClosing(object sender, FormClosingEventArgs e)
        {
            winExcelControl1.Kill();
        }

        public void CreateExcelWnd()
        {
            winExcelControl1.OnNewCreateXls();
        }

        public void DoActive()
        {
            winExcelControl1.DoActive();
        }

        #region IOperationWnd Members

        public void Display()
        {
            Show();
        }

        #endregion


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
                    _masExcelDrawStatType, false);
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
                        _masExcelDrawStatType, false);
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
                    _masExcelDrawStatType, displayDataLabel);
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
                return Math.Round(result, roundCount).ToString("#.00");
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
                    _masExcelDrawStatType, displayDataLabel);
            bRow += 3;
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
    }

    enum enumAppendExcelType
    {
        Base,
        DisplayCol,
        DisplayLabel,
        DisplayColLable
    }
}
