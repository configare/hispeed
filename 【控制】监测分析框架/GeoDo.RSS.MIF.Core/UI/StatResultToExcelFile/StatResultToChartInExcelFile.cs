using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public partial class StatResultToChartInExcelFile : Form
    {
        private WinExcelControl _winExcelControl;
        private masExcelDrawStatType _masExcelDrawStatType ;
        private string _windowText;
        private IStatResult _result;
        private bool _isDrawChart;
        private bool _needLoad = false;
        private int initBCol = 2;
        int bCol ;
        int bRow ;

        public StatResultToChartInExcelFile()
        {
            InitializeComponent();
        }

        public void Init(masExcelDrawStatType drawType)
        {
            _masExcelDrawStatType = drawType;
            CreateExcelWnd();
        }

        public void CreateExcelWnd()
        {
            _winExcelControl = new WinExcelControl();
            _winExcelControl.Dock = DockStyle.Fill;
            this.Controls.Add(_winExcelControl);
            _winExcelControl.OnNewCreateXls();
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
                        _winExcelControl.SaveXLS(filename);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            else
                _winExcelControl.SaveXLS(filename);
            _winExcelControl.CloseControl();
            this.Close();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowText"></param>
        /// <param name="result"></param>
        /// <param name="drawChart"></param>
        /// <param name="colOffset">开始绘制图表的偏移列数，0为从第一列开始绘制</param>
        /// <param name="xName">横轴标题</param>
        /// <param name="yName">纵轴标题</param>
        public void Add(string windowText, IStatResult result, bool drawChart, int colOffset, bool hasLegend,string xName,string yName)
        {
            _windowText = windowText;
            _result = result;
            _isDrawChart = drawChart;
            _needLoad = true;
            Display(result, drawChart, colOffset, hasLegend, xName, yName);
        }

        private void Display(IStatResult result, bool drawChart, int colOffset, bool hasLegend,string xName,string yName)
        {
            if (_needLoad)
            {
                if (!string.IsNullOrEmpty(_windowText))
                    Text = _windowText;
                //GetHideCols(_singleFile, _result);
                Append(result,drawChart,colOffset,hasLegend,xName,yName);
                _winExcelControl.UnLock();
                _needLoad = false;
            }
            this.Refresh();
        }

        private void Append(IStatResult result, bool drawChart, int colOffset, bool hasLegend,string xName,string yName)
        {
            if (result == null)
                return;
            bCol = initBCol;
            bRow = 1;
            int currentCol = -1;
            _winExcelControl.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 , 1, masExcelAlignType.Center, Text.Replace(".", ""), "@");
            bRow++;
            _winExcelControl.SetCellValue(bRow, bCol, bRow, bCol + result.Columns.Length - 1 , 1, masExcelAlignType.General, result.Title, null);
            if (result.Columns != null && result.Columns.Length > 0)
            {
                bRow++;
                foreach (string colname in result.Columns)
                {
                    currentCol++;
                    _winExcelControl.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, colname, null);
                    _winExcelControl.SetBackColor(bRow, bCol, bRow, bCol, 216, 216, 216);
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
                        _winExcelControl.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, SetRound(colContext), null);
                        bCol++;
                    }
                    SupplySpace(result);
                }
            }
            if (drawChart)
                _winExcelControl.DrawStat(statBRow - 1, initBCol+colOffset, bRow, initBCol + result.Columns.Length - 1 , "图表", Text.Replace(".", ""), xName,
                    yName,
                    _masExcelDrawStatType, false,hasLegend);
            bRow += 3;
        }

        private string SetRound(string colContext)
        {
            double result = 0;
            if (!double.TryParse(colContext, out result))
                return colContext;
            else
                return Math.Round(result, 2).ToString("#.00");
        }

        private void SupplySpace(IStatResult result)
        {
            int length = result.Columns.Length + initBCol - bCol;
            for (int surplusCol = 0; surplusCol < length; surplusCol++)
            {
                _winExcelControl.SetCellValue(bRow, bCol, 1, masExcelAlignType.General, "", null);
                bCol++;
            }
        }
    }
}
