using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel;
using System.IO;

namespace Geodo.RSS.MIF.UI
{
    public partial class WinExcelControl : UserControl
    {
        #region "API usage declarations"
        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            int hWnd,
            int X,
            int Y,
            int hWidth,
            int hHeight,
            bool bRepaint
            );

        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        #endregion

        public Excel.Application xlApp = null;
        public _Workbook workbook = null;
        private bool deactivateevents = false;
        public int xlsWnd = 0;
        public string xlsFileName = null;
        public _Worksheet worksheet;
        private object missing = System.Reflection.Missing.Value;
        private int _count = 0;

        public WinExcelControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭当前Excel文档
        /// </summary>
        public void CloseControl()
        {
            if (workbook != null)
            {
                deactivateevents = true;
                object dummy = null;
                object dummyFalse = (object)false;
                ((Excel._Workbook)workbook).Close(dummy, dummy, dummy);
                xlApp.Quit();
                Kill();
                deactivateevents = false;
            }
        }

        /// <summary>
        /// catches Excel's close event 
        /// starts a Thread that send a ESC to the Excel window ;)
        /// </summary>
        private void OnClose(Excel.Workbook xls, ref bool cancel)
        {
            if (!deactivateevents)
            {
                cancel = true;
            }
        }

        /// <summary>
        /// catches Excel's open event
        /// just close
        /// </summary>
        public void OnNewCreateXls()
        {
            LoadNewWorkbook();
        }


        /// <summary>
        /// catches Excel's open event
        /// just close
        /// </summary>
        private void OnOpenXls(Excel.Workbook xls)
        {
            OnNewXls(xls);
        }

        /// <summary>
        /// catches Excel's newworkbook event
        /// just close
        /// </summary>
        private void OnNewXls(Excel.Workbook xls)
        {
            if (!deactivateevents)
            {
                deactivateevents = true;
                object dummy = null;
                ((Excel._Workbook)workbook).Close(dummy, dummy, dummy);
                deactivateevents = false;
            }
        }

        /// <summary>
        /// catches Excel's quit event
        /// normally it should not fire, but just to be shure
        /// safely release the internal Excel Instance 
        /// </summary>
        private void OnQuit()
        {
            xlApp = null;
        }

        public void LoadNewWorkbook()
        {
            deactivateevents = true;

            if (xlApp == null)
                xlApp = new Excel.Application();
            try
            {
                xlApp.CommandBars.AdaptiveMenus = false;
                xlApp.WorkbookBeforeClose += new Excel.AppEvents_WorkbookBeforeCloseEventHandler(OnClose);
                xlApp.NewWorkbook += new Excel.AppEvents_NewWorkbookEventHandler(OnNewXls);
                xlApp.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(OnOpenXls);
            }
            catch { }

            if (workbook != null)
            {
                try
                {
                    xlApp.Workbooks.Close();
                    Kill();
                }
                catch { }
            }

            if (xlsWnd == 0)
                xlsWnd = FindWindow("XLMAIN", null);
            if (xlsWnd != 0)
            {
                SetParent(xlsWnd, this.Handle.ToInt32());
                try
                {
                    if (xlApp == null)
                        throw new ExcelInstanceException();

                    if (xlApp.Workbooks == null)
                        throw new WorkbookInstanceException();

                    if (xlApp != null && xlApp.Workbooks != null)
                    {
                        Workbooks workbooks = xlApp.Workbooks;
                        workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                        Sheets sheets = workbook.Worksheets;
                        worksheet = (_Worksheet)sheets.get_Item(1);
                        worksheet.Name = "表格";
                    }

                    if (workbook == null)
                        throw new ValidWorkbookException();
                }
                catch
                { }

                try
                {
                    xlApp.Visible = true;
                    xlApp.UserControl = true;
                    SetWindowPos(xlsWnd, this.Handle.ToInt32(), 0, 0, this.Bounds.Width, this.Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);
                    OnResize();
                }
                catch
                {
                    //MsgBox.ShowInfo("不能加载Excel文档!");
                }
            }
            deactivateevents = false;
        }

        public void DoActive()
        {
            xlApp.SendKeys("{F2}", missing);
        }

        /// <summary>
        /// internal resize function
        /// utilizes the size of the surrounding control
        /// 
        /// optimzed for Excel2000 but it works pretty good with WordXP too.
        /// </summary>
        private void OnResize()
        {
            //The original one that I used is shown below. Shows the complete window, but its buttons (min, max, restore) are disabled
            //// MoveWindow(xlsWnd,0,0,this.Bounds.Width,this.Bounds.Height,true);

            ///Change below
            ///The following one is better, if it works for you. We donot need the title bar any way. Based on a suggestion.
            int borderWidth = SystemInformation.Border3DSize.Width;
            int borderHeight = SystemInformation.Border3DSize.Height;
            int captionHeight = SystemInformation.CaptionHeight;
            int statusHeight = SystemInformation.ToolWindowCaptionHeight;
            MoveWindow(
                xlsWnd,
                -2 * borderWidth,
                -2 * borderHeight - captionHeight,
                this.Bounds.Width + 4 * borderWidth,
                this.Bounds.Height + captionHeight + 4 * borderHeight + statusHeight,
                true);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            OnResize();
        }

        public class ExcelInstanceException : Exception
        { }

        public class WorkbookInstanceException : Exception
        { }

        public class ValidWorkbookException : Exception
        { }

        /// <summary>
        /// 合并单元格并返回Excel.Range
        /// </summary>
        /// <param name="bRow">开始行</param>
        /// <param name="bCol">开始列</param>
        /// <param name="eRow">结束行</param>
        /// <param name="eCol">结束列</param>
        /// <param name="lineStyle">是否绘制边框</param>
        /// <param name="format">格式</param>
        /// <returns>Excel.range</returns>
        public Range MergeCellsReRange(int bRow, int bCol, int eRow, int eCol, int isDrawBorder, string format)
        {
            Range range = worksheet.get_Range(worksheet.Cells[bRow, bCol] as Excel.Range, worksheet.Cells[eRow, eCol] as Excel.Range);
            range.Merge(0);
            if (isDrawBorder == 1)
                range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
            range.NumberFormatLocal = format;
            return range;
        }

        /// <summary>
        /// 合并单元格并绘制边框、设置单元格值
        /// </summary>
        /// <param name="bRow">开始行</param>
        /// <param name="bCol">开始列</param>
        /// <param name="eRow">结束行</param>
        /// <param name="eCol">结束列</param>
        /// <param name="lineStyle">是否绘制边框</param>
        /// <param name="value">单元格值</param>
        /// <param name="format">格式</param>
        public void SetCellValue(int bRow, int bCol, int eRow, int eCol, int isDrawBorder, masExcelAlignType alignType, string value, string format)
        {
            try
            {
                Range range = MergeCellsReRange(bRow, bCol, eRow, eCol, isDrawBorder, format);
                range.HorizontalAlignment = GetExcelXlHAlignment(alignType);
                range.NumberFormatLocal = format;
                range.Worksheet.Cells[bRow, bCol] = value;
                range.EntireColumn.AutoFit();     //自动调整列宽
                range.EntireRow.AutoFit();
            }
            catch (Exception ex)
            {
                //if (ex.Message == "异常来自 HRESULT:0x800A03EC")
                //    MsgBox.ShowInfo("当前单元格正处于编辑状态，无法修改其值!");
            }
        }

        private object GetExcelXlHAlignment(masExcelAlignType alignType)
        {
            switch (alignType)
            {
                case masExcelAlignType.Center:
                    return XlHAlign.xlHAlignCenter;
                case masExcelAlignType.CenterAcrossSelection:
                    return XlHAlign.xlHAlignCenterAcrossSelection;
                case masExcelAlignType.Distributed:
                    return XlHAlign.xlHAlignDistributed;
                case masExcelAlignType.Fill:
                    return XlHAlign.xlHAlignFill;
                case masExcelAlignType.General:
                    return XlHAlign.xlHAlignGeneral;
                case masExcelAlignType.Justify:
                    return XlHAlign.xlHAlignJustify;
                case masExcelAlignType.Left:
                    return XlHAlign.xlHAlignLeft;
                case masExcelAlignType.Right:
                    return XlHAlign.xlHAlignRight;
                default:
                    return XlHAlign.xlHAlignGeneral;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="isDrawBorder">是否绘制边框 0：不绘制 1：绘制</param>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public void SetCellValue(int row, int col, int isDrawBorder, masExcelAlignType alignType, string value, string format)
        {
            try
            {
                Range range = worksheet.get_Range(worksheet.Cells[row, col] as Excel.Range, worksheet.Cells[row, col] as Excel.Range);
                range.HorizontalAlignment = GetExcelXlHAlignment(alignType);
                if (isDrawBorder == 1)
                    range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
                range.NumberFormatLocal = format;
                range.Worksheet.Cells[row, col] = value;
                range.EntireColumn.AutoFit();     //自动调整列宽
            }
            catch (Exception ex)
            {
                //if (ex.Message == "异常来自 HRESULT:0x800A03EC")
                //    MsgBox.ShowInfo("当前单元格正处于编辑状态，无法修改其值!");
            }
        }

        public void SetCellValue(int row, int col, int colCount, int isDrawBorder, masExcelAlignType alignType, object values, string format)
        {
            Range range = worksheet.get_Range(worksheet.Cells[row, col] as Excel.Range, worksheet.Cells[row + 1, col + colCount - 1] as Excel.Range);
            range.HorizontalAlignment = GetExcelXlHAlignment(alignType);
            if (isDrawBorder == 1)
                range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
            range.NumberFormatLocal = format;
            range.Value2 = values;
            range.EntireColumn.AutoFit();     //自动调整列宽
        }

        public void DrawStat(int dataBRow, int dataBCol, int dataERow, int dataECol, string workSheetName, string title, string xName, string yName, masExcelDrawStatType charType, bool displayDataLabel)
        {
            Excel.Worksheet xlSheet = null;
            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name == workSheetName)
                {
                    xlSheet = sheet;
                    break;
                }
            }
            if (xlSheet == null)
                xlSheet = AddNewSheet(workSheetName);
            if (xlSheet == null)
            {
                //MsgBox.ShowInfo("Excel添加新工作薄失败!");
                return;
            }
            Excel.ChartObjects chartObjects = (Excel.ChartObjects)xlSheet.ChartObjects(Type.Missing);
            Excel.ChartObject chartObj = chartObjects.Add(5, 300 * _count + 20, (dataECol - dataBCol) * GetWidth() * (dataERow - dataBRow) + 200, 250);
            Excel.Chart xlChart = chartObj.Chart;
            XlChartType xlCharType = GetXlCharTypeFromDrawStatType(charType);
            Range range = worksheet.get_Range(worksheet.Cells[dataBRow, dataBCol] as Excel.Range, worksheet.Cells[dataERow, dataECol] as Excel.Range);
            xlChart.ChartWizard(range, xlCharType, Type.Missing, Excel.XlRowCol.xlRows,
              1, 1, true, title, xName, yName, "");

            if (displayDataLabel)
                for (int i = 0; i < dataERow - dataBRow; i++)
                    ((Excel.Series)xlChart.SeriesCollection(i + 1)).ApplyDataLabels(Excel.XlDataLabelsType.xlDataLabelsShowValue,
                                missing, missing, missing);
            _count++;
        }

        private int GetWidth()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "\\ExcelWidth.txt";
            if (!File.Exists(filename))
                return 20;
            else
                return int.Parse(File.ReadAllText(filename, Encoding.Default));
        }

        private Excel.Worksheet AddNewSheet(string workSheetName)
        {
            Excel.Worksheet xlSheet = null;
            try
            {
                object obj = workbook.Worksheets.Add(Type.Missing, workbook.ActiveSheet, Type.Missing, Type.Missing);
                xlSheet = obj as Excel.Worksheet;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (xlSheet != null)
                xlSheet.Name = workSheetName;
            return xlSheet;
        }

        private XlChartType GetXlCharTypeFromDrawStatType(masExcelDrawStatType charType)
        {
            switch (charType)
            {
                case masExcelDrawStatType.xl3DArea:
                    return XlChartType.xl3DArea;
                case masExcelDrawStatType.xl3DAreaStacked:
                    return XlChartType.xl3DAreaStacked;
                case masExcelDrawStatType.xl3DAreaStacked100:
                    return XlChartType.xl3DAreaStacked100;
                case masExcelDrawStatType.xl3DBarClustered:
                    return XlChartType.xl3DBarClustered;
                case masExcelDrawStatType.xl3DBarStacked:
                    return XlChartType.xl3DBarStacked;
                case masExcelDrawStatType.xl3DBarStacked100:
                    return XlChartType.xl3DBarStacked100;
                case masExcelDrawStatType.xl3DColumn:
                    return XlChartType.xl3DColumn;
                case masExcelDrawStatType.xl3DColumnClustered:
                    return XlChartType.xl3DColumnClustered;
                case masExcelDrawStatType.xl3DColumnStacked:
                    return XlChartType.xl3DColumnStacked;
                case masExcelDrawStatType.xl3DColumnStacked100:
                    return XlChartType.xl3DColumnStacked100;
                case masExcelDrawStatType.xl3DLine:
                    return XlChartType.xl3DLine;
                case masExcelDrawStatType.xl3DPie:
                    return XlChartType.xl3DPie;
                case masExcelDrawStatType.xl3DPieExploded:
                    return XlChartType.xl3DPieExploded;
                case masExcelDrawStatType.xlArea:
                    return XlChartType.xlArea;
                case masExcelDrawStatType.xlAreaStacked:
                    return XlChartType.xlAreaStacked;
                case masExcelDrawStatType.xlAreaStacked100:
                    return XlChartType.xlAreaStacked100;
                case masExcelDrawStatType.xlBarClustered:
                    return XlChartType.xlBarClustered;
                case masExcelDrawStatType.xlBarOfPie:
                    return XlChartType.xlBarOfPie;
                case masExcelDrawStatType.xlBarStacked:
                    return XlChartType.xlBarStacked;
                case masExcelDrawStatType.xlBarStacked100:
                    return XlChartType.xlBarStacked100;
                case masExcelDrawStatType.xlBubble:
                    return XlChartType.xlBubble;
                case masExcelDrawStatType.xlBubble3DEffect:
                    return XlChartType.xlBubble3DEffect;
                case masExcelDrawStatType.xlColumnClustered:
                    return XlChartType.xlColumnClustered;
                case masExcelDrawStatType.xlColumnStacked:
                    return XlChartType.xlColumnStacked;
                case masExcelDrawStatType.xlColumnStacked100:
                    return XlChartType.xlColumnStacked100;
                case masExcelDrawStatType.xlConeBarClustered:
                    return XlChartType.xlConeBarClustered;
                case masExcelDrawStatType.xlConeBarStacked:
                    return XlChartType.xlConeBarStacked;
                case masExcelDrawStatType.xlConeBarStacked100:
                    return XlChartType.xlConeBarStacked100;
                case masExcelDrawStatType.xlConeCol:
                    return XlChartType.xlConeCol;
                case masExcelDrawStatType.xlConeColClustered:
                    return XlChartType.xlConeColClustered;
                case masExcelDrawStatType.xlConeColStacked:
                    return XlChartType.xlConeColStacked;
                case masExcelDrawStatType.xlConeColStacked100:
                    return XlChartType.xlConeColStacked100;
                case masExcelDrawStatType.xlCylinderBarClustered:
                    return XlChartType.xlCylinderBarClustered;
                case masExcelDrawStatType.xlCylinderBarStacked:
                    return XlChartType.xlCylinderBarStacked;
                case masExcelDrawStatType.xlCylinderBarStacked100:
                    return XlChartType.xlCylinderBarStacked100;
                case masExcelDrawStatType.xlCylinderCol:
                    return XlChartType.xlCylinderCol;
                case masExcelDrawStatType.xlCylinderColClustered:
                    return XlChartType.xlCylinderColClustered;
                case masExcelDrawStatType.xlCylinderColStacked:
                    return XlChartType.xlCylinderColStacked;
                case masExcelDrawStatType.xlCylinderColStacked100:
                    return XlChartType.xlCylinderColStacked100;
                case masExcelDrawStatType.xlDoughnut:
                    return XlChartType.xlDoughnut;
                case masExcelDrawStatType.xlDoughnutExploded:
                    return XlChartType.xlDoughnutExploded;
                case masExcelDrawStatType.xlLine:
                    return XlChartType.xlLine;
                case masExcelDrawStatType.xlLineMarkers:
                    return XlChartType.xlLineMarkers;
                case masExcelDrawStatType.xlLineMarkersStacked:
                    return XlChartType.xlLineMarkersStacked;
                case masExcelDrawStatType.xlLineMarkersStacked100:
                    return XlChartType.xlLineMarkersStacked100;
                case masExcelDrawStatType.xlLineStacked:
                    return XlChartType.xlLineStacked;
                case masExcelDrawStatType.xlLineStacked100:
                    return XlChartType.xlLineStacked100;
                case masExcelDrawStatType.xlPie:
                    return XlChartType.xlPie;
                case masExcelDrawStatType.xlPieExploded:
                    return XlChartType.xlPieExploded;
                case masExcelDrawStatType.xlPieOfPie:
                    return XlChartType.xlPieOfPie;
                case masExcelDrawStatType.xlPyramidBarClustered:
                    return XlChartType.xlPyramidBarClustered;
                case masExcelDrawStatType.xlPyramidBarStacked:
                    return XlChartType.xlPyramidBarStacked;
                case masExcelDrawStatType.xlPyramidBarStacked100:
                    return XlChartType.xlPyramidBarStacked100;
                case masExcelDrawStatType.xlPyramidCol:
                    return XlChartType.xlPyramidCol;
                case masExcelDrawStatType.xlPyramidColClustered:
                    return XlChartType.xlPyramidColClustered;
                case masExcelDrawStatType.xlPyramidColStacked:
                    return XlChartType.xlPyramidColStacked;
                case masExcelDrawStatType.xlPyramidColStacked100:
                    return XlChartType.xlPyramidColStacked100;
                case masExcelDrawStatType.xlRadar:
                    return XlChartType.xlRadar;
                case masExcelDrawStatType.xlRadarFilled:
                    return XlChartType.xlRadarFilled;
                case masExcelDrawStatType.xlRadarMarkers:
                    return XlChartType.xlRadarMarkers;
                case masExcelDrawStatType.xlStockHLC:
                    return XlChartType.xlStockHLC;
                case masExcelDrawStatType.xlStockOHLC:
                    return XlChartType.xlStockOHLC;
                case masExcelDrawStatType.xlStockVHLC:
                    return XlChartType.xlStockVHLC;
                case masExcelDrawStatType.xlStockVOHLC:
                    return XlChartType.xlStockVOHLC;
                case masExcelDrawStatType.xlSurface:
                    return XlChartType.xlSurface;
                case masExcelDrawStatType.xlSurfaceTopView:
                    return XlChartType.xlSurfaceTopView;
                case masExcelDrawStatType.xlSurfaceTopViewWireframe:
                    return XlChartType.xlSurfaceTopViewWireframe;
                case masExcelDrawStatType.xlSurfaceWireframe:
                    return XlChartType.xlSurfaceWireframe;
                case masExcelDrawStatType.xlXYScatter:
                    return XlChartType.xlXYScatter;
                case masExcelDrawStatType.xlXYScatterLines:
                    return XlChartType.xlXYScatterLines;
                case masExcelDrawStatType.xlXYScatterLinesNoMarkers:
                    return XlChartType.xlXYScatterLinesNoMarkers;
                case masExcelDrawStatType.xlXYScatterSmooth:
                    return XlChartType.xlXYScatterSmooth;
                case masExcelDrawStatType.xlXYScatterSmoothNoMarkers:
                    return XlChartType.xlXYScatterSmoothNoMarkers;
                default:
                    return XlChartType.xl3DArea;
            }
        }

        List<ExcelRangeColor> _rangColorExceptions = null;
        public void SetBackColor(int bRow, int bCol, int eRow, int eCol, byte red, byte green, byte blue)
        {
            Range range = worksheet.get_Range(worksheet.Cells[bRow, bCol] as Excel.Range, worksheet.Cells[eRow, eCol] as Excel.Range);
            try
            {
                range.Cells.Interior.Color = Color.FromArgb(red, green, blue).ToArgb();
            }
            catch (Exception ex)
            {
                if (_rangColorExceptions == null)
                    _rangColorExceptions = new List<ExcelRangeColor>();
                ExcelRangeColor temp = new ExcelRangeColor(bRow, bCol, eRow, eCol, red, green, blue);
                if (!_rangColorExceptions.Contains(temp))
                    _rangColorExceptions.Add(temp);
            }
        }

        /// <summary>
        ///  绘制表格外边框
        /// </summary>
        /// <param name="bRow">开始行</param>
        /// <param name="bCol">开始列</param>
        /// <param name="eRow">结束行</param>
        /// <param name="eCol">结束列</param>
        public void DrawTableBorder(int bRow, int bCol, int eRow, int eCol)
        {
            Range range = worksheet.get_Range(worksheet.Cells[bRow, bCol] as Excel.Range, worksheet.Cells[eRow, eCol] as Excel.Range);
            range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
        }


        public void Kill()
        {
            IntPtr t = new IntPtr(xlsWnd); //得到这个句柄，具体作用是得到这块内存入口
            int k = 0;
            GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k
            if (k != 0)
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k); //得到对进程k的引用
                p.Kill(); //关闭进程k
            }
        }

        //by chennan 2011-4-21 增加Excel统计合计行
        public void SetTotalCellValue(int brow, int bcol, int erow, int ecol, int maxCol, int isDrawBorder, masExcelAlignType alignType, string format)
        {
            try
            {
                int row = erow + 1;
                for (int col = bcol; col < maxCol; col++)
                {
                    Range range = worksheet.get_Range(worksheet.Cells[row, col] as Excel.Range, worksheet.Cells[row, col] as Excel.Range);
                    range.HorizontalAlignment = GetExcelXlHAlignment(alignType);
                    if (isDrawBorder == 1)
                        range.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, System.Drawing.Color.Black.ToArgb());
                    range.NumberFormatLocal = format;
                    if (col == bcol)
                        range.Worksheet.Cells[row, col] = "合计";
                    else if (col > ecol)
                        range.Worksheet.Cells[row, col] = "";
                    else
                        range.Worksheet.Cells[row, col] = "=SUM(" + Int2Letter(col) + (brow + 1) + ":" + Int2Letter(col) + erow + ")";
                    range.EntireColumn.AutoFit();     //自动调整列宽
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message == "异常来自 HRESULT:0x800A03EC")
                //    MsgBox.ShowInfo("当前单元格正处于编辑状态，无法修改其值!");
            }
        }

        private string[] letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private string Int2Letter(int num)
        {
            num = num - 1;
            if (num / 26 == 0)
            {
                return letters[num % 26];
            }
            else
            {
                return letters[num / 26] + letters[num % 26];
            }
        }

        public void UnLock()
        {
            ProcessException();
            SendKeys.Send("{F2}");
            SendKeys.Send("{ENTER}");
            System.Windows.Forms.Application.DoEvents();
        }

        private void ProcessException()
        {
            if (_rangColorExceptions != null && _rangColorExceptions.Count != 0)
            {
                foreach (ExcelRangeColor item in _rangColorExceptions)
                    SetBackColor(item.bRow, item.bCol, item.eRow, item.eCol, item.red, item.green, item.blue);
            }
            _rangColorExceptions = null;
        }

        public void Save(string saveFile)
        {
            workbook.SaveAs(saveFile, missing, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing);
            string fileTxt = saveFile.Replace(".XLSX", ".TXT");
            Worksheet tempWorksheet = (Worksheet)workbook.Sheets.get_Item(1);
            worksheet.SaveAs(fileTxt, XlFileFormat.xlCSV, missing, missing, missing, missing, XlSaveAsAccessMode.xlNoChange, missing, missing);
        }

        public void SaveXLS(string filename)
        {
            workbook.SaveAs(filename, missing, missing, missing, missing, missing, Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing);
        }
    }
}
