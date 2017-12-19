using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCTimeSeries : UserControl, IArgumentEditorUI
    {
        private Action<object> _handle = null;
        private Dictionary<string, object> _result = null;
        private GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
        List<string> fieldValues = new List<string>();
        private string IsSeniorQuery = "NO";
        private string IsAllContry = "NO";
        public UCTimeSeries()
        {
            InitializeComponent();
            InitialParas();
            this.radibtnPeriodMonth.Checked = true;
        }

        private void  InitialParas()
        {
            #region 起始终止年
            this.combxYearStart.Items.Clear();
            this.combxYearEnd.Items.Clear();
            for (int i = 1987; i <= DateTime.Now.Year;i++ )
            {
                this.combxYearStart.Items.Add(i);
                this.combxYearEnd.Items.Add(i);
            }
            this.combxYearStart.SelectedIndex = 0;
            if (DateTime.Now.Year>=2013)
                this.combxYearEnd.SelectedIndex = (2013 - 1987);
            else
                this.combxYearEnd.SelectedIndex = this.combxYearEnd.Items.Count-1;
            #endregion
            #region 起始终止月
            this.combxTimeMonthStart.Items.Clear();
            this.combxTimeMonthEnd.Items.Clear();
            this.combxWinterMonthStart.Items.Clear();
            this.combxWinterMonthEnd.Items.Clear();
            for (int i = 1; i <=12; i++)
            {
                this.combxTimeMonthStart.Items.Add(i);
                this.combxTimeMonthEnd.Items.Add(i);
                this.combxWinterMonthStart.Items.Add(i);
                this.combxWinterMonthEnd.Items.Add(i);
            }
            this.combxTimeMonthStart.SelectedIndex = 0;
            this.combxWinterMonthStart.SelectedIndex = 10;
            this.combxWinterMonthEnd.SelectedIndex = 1;
            this.combxTimeTenStart.Items.Clear();
            this.combxTimeTenEnd.Items.Clear();
            for (int i = 1; i <= 3; i++)
            {
                this.combxTimeTenStart.Items.Add(i);
                this.combxTimeTenEnd.Items.Add(i);
            }
            this.combxTimeTenStart.SelectedIndex = 0;
            this.combxTimePentadStart.Items.Clear();
            this.combxTimePentadEnd.Items.Clear();
            for (int i = 1; i <= 6; i++)
            {
                this.combxTimePentadStart.Items.Add(i);
                this.combxTimePentadEnd.Items.Add(i);
            }
            this.combxTimePentadStart.SelectedIndex = 0;
            #endregion
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handle = handler;
        }

        public object GetArgumentValue()
        {
            if (_result != null)
                _result.Clear();
            else
                _result = new Dictionary<string, object>();
            //MWS界面参数
            //prdTypes，string=SD或SWE
            if (radioSD.Checked)
                _result.Add("prdTypes", "SD");
            else
                _result.Add("prdTypes", "SWE");
            //orbTypes，string[]=ASD/DSD
            List<string> types = new List<string>();
            if (cbxAsend.Checked)
                types.Add("ASD");
            if (cbxDesend.Checked)
                types.Add("DSD");
            _result.Add("orbTypes", types.ToArray());
            //startEndyears,int []=1987,1988,...
            types.Clear();
            types.Add(combxYearStart.SelectedItem.ToString());
            types.Add(combxYearEnd.SelectedItem.ToString());
            _result.Add("startEndyears", types.ToArray());
            //periodTypes，string=PENTAD,TEN,MONTH,WINTER
            foreach (Control rdb in groupBox10.Controls)
            {
                RadioButton rdbi =rdb as RadioButton;
                if (rdbi != null && rdbi.Checked==true)
                    _result.Add("periodTypes", rdbi.Name.Replace("radibtnPeriod", "").ToUpper());
            }
            //startEndTime,string[2]={startMonth_startTen_startPentad,EndMonth_EndTen_EndPentad}
            string monthstart,monthend;
            monthstart=combxTimeMonthStart.SelectedItem.ToString();
            monthend=combxTimeMonthEnd.SelectedItem.ToString();
            if (radibtnPeriod.Checked)
                _result.Add("queryTypes", "sametime");
            else
                _result.Add("queryTypes", "continue");
            StringBuilder time = new StringBuilder();
            if (radibtnPeriodMonth.Checked)
            {
                time.Append(monthstart);
                time.Append("_0_0,");
                time.Append(monthend);
                time.Append("_0_0");
                _result.Add("startEndTime", time.ToString().Split(','));
            }
            else if (radibtnPeriodWinter.Checked)
            {
                string winterMonthStart = combxWinterMonthStart.SelectedItem.ToString();
                string winterMonthEnd = combxWinterMonthEnd.SelectedItem.ToString();
                time.Append(winterMonthStart);
                time.Append("_0_0,");
                time.Append(winterMonthEnd);
                time.Append("_0_0");
                _result.Add("startEndTime", time.ToString().Split(','));
            }
            else if (radibtnPeriodTen.Checked)
            {
                string tenstart = combxTimeTenStart.SelectedItem.ToString();
                string tenend = combxTimeTenEnd.SelectedItem.ToString();
                time.Append(monthstart);
                time.Append("_");
                time.Append(tenstart);
                time.Append("_0,");
                time.Append(monthend);
                time.Append("_");
                time.Append(tenend);
                time.Append("_0");
                _result.Add("startEndTime", time.ToString().Split(','));
            }
            else if (radibtnPeriodPentad.Checked)
            {
                string pentadstart = combxTimePentadStart.SelectedItem.ToString();
                string pentadend = combxTimePentadEnd.SelectedItem.ToString();
                time.Append(monthstart);
                time.Append("_0_");
                time.Append(pentadstart);
                time.Append(",");
                time.Append(monthend);
                time.Append("_0_");
                time.Append(pentadend);
                _result.Add("startEndTime", time.ToString().Split(','));
            }           
            //timesAll, string[]={YEAR-MONTH-TEN-PENTAD格式的数组}
            //statTypes，string =AVG;MIN;MAX;
            if (radibtnAVG.Checked)
                _result.Add("statTypes", "AVG");
            else if (radibtnMIN.Checked)
                _result.Add("statTypes", "MIN");
            else if (radibtnMAX.Checked)
                _result.Add("statTypes", "MAX");
            //timeSeriesOpts,string []=AvgLayout/AvgStat/JupingLayout/JupingStat
            types.Clear();
            if(cbxHistoryDataLayout.Checked)
                types.Add("AvgLayout");
            if (cbxHistoryDataStat.Checked)
                types.Add("AvgStat");
            if (cbxJuPingLayout.Checked)
                types.Add("JupingLayout");
            if (cbxJuPingStat.Checked)
                types.Add("JupingStat");
            _result.Add("timeSeriesOpts", types.ToArray());
            //outAOIs,object[]=AOI的数组，类型自定
            _result.Add("outAOIs", aoiContainer);
            //isMergeAois,bool=是否合成
            _result.Add("isMergeAois", cbxAOIMerge.Checked?1:0);
            //MergeAoisName,string =合成AOI名字
            if (cbxAOIMerge.Checked || aoiContainer.AOIs.Count() == 1)
                _result.Add("MergeAoisName", txtRegionName.Text);
            else
                _result.Add("MergeAoisName", fieldValues.ToArray());
            _result.Add("ChoseAois", txtRegionName.Text);
            //IsSeniorQuery = "yes"; 是否调用了高级检索
            _result.Add("IsSeniorQuery", IsSeniorQuery);
            //IsAllContry = "YES";  是否是全国范围
            _result.Add("IsAllContry", IsAllContry);
            //outPath,string =输出路径；
            _result.Add("outPath", txtOutDir.Text);
            return _result;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement xml)
        {
            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckArgsIsOK())
                    throw new ArgumentException("参数选择不全,请重试！");
                if (_handle != null)
                    _handle(GetArgumentValue());
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckArgsIsOK()
        {
            if(!cbxAsend.Checked&&!cbxDesend.Checked)
                throw new ArgumentException("请选择数据轨道类型！");
            if (!CheckTimeIsOK())
                return false;
            if (!cbxHistoryDataLayout.Checked&&!cbxHistoryDataStat.Checked&&!cbxJuPingLayout.Checked&&!cbxJuPingStat.Checked)
                throw new ArgumentException("请选择时序分析类型！");
            if (string.IsNullOrWhiteSpace(txtOutDir.Text) || !Directory.Exists(txtOutDir.Text))
                throw new ArgumentException("请选择正确的输出路径！");
            if (cbxAOIMerge.Checked&&string.IsNullOrWhiteSpace(txtRegionName.Text))
                throw new ArgumentException("请设置正确的合并区域名称！");
            if (aoiContainer == null || aoiContainer.AOIs.Count() <= 0)
                throw new ArgumentException("请选择输出区域！");
            return true;
        }

        private bool CheckTimeIsOK()
        {
            string yearstart,yearend,monthstart,monthend;
            #region 检查时间选择项
            if (combxYearStart.SelectedIndex==-1)
                throw new ArgumentException("请正确选择数据开始年份！");
            yearstart=combxYearStart.SelectedItem.ToString();
            if (combxYearEnd.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据结束年份！");
            yearend=combxYearEnd.SelectedItem.ToString();
            if (combxTimeMonthStart.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据开始月份！");
            monthstart=combxTimeMonthStart.SelectedItem.ToString();
            if (combxTimeMonthEnd.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据结束月份！");
            monthend=combxTimeMonthEnd.SelectedItem.ToString();
            if (combxTimeTenStart.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据开始旬！");
            string tenstart = combxTimeTenStart.SelectedItem.ToString();
            if (combxTimeTenEnd.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据结束旬！");
            string tenend = combxTimeTenEnd.SelectedItem.ToString();
            if (combxTimePentadStart.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据开始侯！");
            string pentadstart = combxTimePentadStart.SelectedItem.ToString();
            if (combxTimePentadEnd.SelectedIndex == -1)
                throw new ArgumentException("请正确选择数据结束侯！");
            string pentadend = combxTimePentadEnd.SelectedItem.ToString();
            int monthID = FormerIsSmaller(monthstart, monthend);
            int yearID =FormerIsSmaller(yearstart,yearend);
            int tenID = FormerIsSmaller(tenstart, tenend);
            int pentadID =FormerIsSmaller(pentadstart,pentadend);
            #endregion
            if (yearID < 0)
                throw new ArgumentException("数据开始年份不能大于结束年份！");
            if (radibtnContinue.Checked)
            {
                if (FormerIsSmaller(yearstart, yearend) == 0)
                {
                   if(monthID <0)
                       throw new ArgumentException("同一年的连续数据查询开始月份不能大于结束月份！");
                   //if (monthID == 0 && tenID < 0)
                   //    throw new ArgumentException("同一个月的数据开始旬不能大于结束旬！");    
                   //if (monthID == 0 && pentadID < 0)
                   //    throw new ArgumentException("同一个月的数据开始侯不能大于结束侯！");
                }
                if (monthID < 0)
                    throw new ArgumentException("连续数据查询开始月份大于结束月份时，请选择高级查询实现！");
                if (monthID == 0 && tenID < 0)
                    throw new ArgumentException("同一个月的数据开始旬不能大于结束旬！");
                if (monthID == 0 && pentadID < 0)
                    throw new ArgumentException("同一个月的数据开始侯不能大于结束侯！");
                return true;
            }
            if (radibtnPeriodMonth.Checked)
            {
                if (monthID < 0)
                    throw new ArgumentException("非雪季数据,数据开始月份不能大于结束月份！");      
            }
            else if (radibtnPeriodTen.Checked)
            {
                if (monthID < 0)
                    throw new ArgumentException("非雪季数据,数据开始月份不能大于结束月份！");
                if (monthID == 0 && tenID< 0)
                    throw new ArgumentException("同一个月的数据开始旬不能大于结束旬！");      
            }
            else if (radibtnPeriodPentad.Checked)
            {
                if (monthID < 0)
                    throw new ArgumentException("非雪季数据,数据开始月份不能大于结束月份！");
                if (monthID == 0 && pentadID< 0)
                    throw new ArgumentException("同一个月的数据开始侯不能大于结束侯！");
            }            
            return true;
        }

        private bool CheckContinueTime(string yearstart,string yearend,string monthstart, string monthend)
        {
            if (FormerIsSmaller(yearstart, yearend) == 0 && FormerIsSmaller(monthstart, monthend) == -1)
                throw new ArgumentException("同一年的连续数据查询开始月份不能大于结束月份！");
            return true;
        }

        /// <summary>
        /// para1小于para2返回1，para1=para2返回0，para1>para2返回-1;
        /// </summary>
        /// <param name="former"></param>
        /// <param name="later"></param>
        /// <returns></returns>
        private int FormerIsSmaller(string former,string later)
        {
            int form =int.Parse(former) ;
            int late =int.Parse(later);
            if (form < late)
                return 1;
            else if(form == late)
                return 0;
            else
                return -1;
        }

        private void btnSeniorQuery_Click(object sender, EventArgs e)
        {
            try
            {
                IsSeniorQuery = "yes";
                GeoDo.RSS.MicroWaveFYDataRetrieval.FormMain frm = new GeoDo.RSS.MicroWaveFYDataRetrieval.FormMain();
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                frm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnBrowseOutDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void radibtnPeriodMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnPeriodPentad.Checked || radibtnPeriodTen.Checked || radibtnPeriodMonth.Checked)
            {
                combxWinterMonthStart.Enabled = false;
                combxWinterMonthEnd.Enabled = false;
                radibtnContinue.Enabled = true;
                if (radibtnPeriodPentad.Checked)
                {
                    combxTimePentadStart.Enabled = true;
                    combxTimePentadEnd.Enabled = true;
                    combxTimeTenStart.Enabled = false;
                    combxTimeTenEnd.Enabled = false;
                    combxTimeMonthStart.Enabled = true;
                    combxTimeMonthEnd.Enabled = false;
                    if (radibtnContinue.Checked)
                        combxTimeMonthEnd.Enabled = true;
                } 
                else if (radibtnPeriodTen.Checked)
                {
                    combxTimePentadStart.Enabled = false;
                    combxTimePentadEnd.Enabled = false;
                    combxTimeTenStart.Enabled = true;
                    combxTimeTenEnd.Enabled = true;
                    combxTimeMonthStart.Enabled = true;
                    combxTimeMonthEnd.Enabled = false;
                    if (radibtnContinue.Checked)
                        combxTimeMonthEnd.Enabled = true;
                }
                else if (radibtnPeriodMonth.Checked)
                {
                    combxTimePentadStart.Enabled = false;
                    combxTimePentadEnd.Enabled = false;
                    combxTimeTenStart.Enabled = false;
                    combxTimeTenEnd.Enabled = false;
                    combxTimeMonthStart.Enabled = true;
                    combxTimeMonthEnd.Enabled = true;
                }
            }
            else if (radibtnPeriodWinter.Checked)
            {
                radibtnContinue.Enabled = false;
                radibtnContinue.Checked = false;
                radibtnPeriod.Checked = true;
                combxTimePentadStart.Enabled = false;
                combxTimePentadEnd.Enabled = false;
                combxTimeTenStart.Enabled = false;
                combxTimeTenEnd.Enabled = false;
                combxTimeMonthStart.Enabled = false;
                combxTimeMonthEnd.Enabled = false;
                combxWinterMonthStart.Enabled = true;
                combxWinterMonthEnd.Enabled = true;
            }
        }

        private void radibtnAVG_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnAVG.Checked)
            {
                cbxHistoryDataLayout.Enabled = true;
                cbxHistoryDataStat.Enabled = true;
                cbxJuPingLayout.Enabled = true;
                cbxJuPingStat.Enabled = true;
            }
        }

        private void radibtnMIN_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnMAX.Checked||radibtnMIN.Checked)
            {
                cbxJuPingLayout.Enabled = false;
                cbxJuPingStat.Enabled = false;
                cbxJuPingLayout.Checked = false;
                cbxJuPingStat.Checked = false;
            }
        }

        private void combxTimeMonthStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            combxTimeMonthEnd.SelectedIndex = combxTimeMonthStart.SelectedIndex;
        }

        private void combxTimeTenStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            combxTimeTenEnd.SelectedIndex = combxTimeTenStart.SelectedIndex;
        }

        private void combxTimePentadStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            combxTimePentadEnd.SelectedIndex = combxTimePentadStart.SelectedIndex;
        }

        #region AOi选择
        private void cbxAOIMerge_CheckedChanged(object sender, EventArgs e)
        {
            //checked
            //if (cbxAOIMerge.Checked == true)
            //    MessageBox.Show("请输入合并区域简称");
        }

        private void btnChooseAOI_Click(object sender, EventArgs e)
        {
            int fieldIndex = -1; string fieldName; string shapeFilename; string regionName = "";
            if(aoiContainer != null)
                aoiContainer.Dispose();
            if(fieldValues !=null )
                fieldValues.Clear();
            using (frmStatSubRegionTemplatesMWS frm = new frmStatSubRegionTemplatesMWS())
            {
                frm.listView1.MultiSelect = true;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Feature[] fets = frm.GetSelectedFeatures();
                    fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                    if (fets == null)
                    {
                        MessageBox.Show("未选定目标区域，请选择区域");
                    }
                    else
                    {
                        string chinafieldValue = fets[0].GetFieldValue(fieldIndex);
                        if (chinafieldValue == "中国")
                        {
                            aoiContainer.AddAOI(fets[0]);
                            regionName = "全国";
                            fieldValues.Add("全国");
                            IsAllContry = "YES";
                        }
                        else
                        {
                            foreach (Feature fet in fets)
                            {
                                fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                aoiContainer.AddAOI(fet);
                            }
                            regionName = "";
                            foreach (string region in fieldValues)
                            {
                                regionName += region;
                            }
                        }
                    }
                    txtRegionName.Text = regionName;
                    if (aoiContainer != null)
                    {
                        if (aoiContainer.AOIs.Count() > 1)
                        {
                            if ((radibtnPeriod.Checked) && (combxTimeMonthStart.SelectedItem.ToString() != combxTimeMonthEnd.SelectedItem.ToString()
                               || combxTimeTenStart.SelectedItem.ToString() != combxTimeTenEnd.SelectedItem.ToString()
                               || combxTimePentadStart.SelectedItem.ToString() != combxTimePentadEnd.SelectedItem.ToString()))
                            {
                                cbxAOIMerge.Checked = true; cbxAOIMerge.Enabled = false;
                                //MessageBox.Show("选择为多周期段和多区域时，所选区域将自动合成，请输入简称");
                            }
                            else
                            {
                                cbxAOIMerge.Checked = false; cbxAOIMerge.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择区域");
                }
            }
        }
        #endregion


        private void radibtnContinue_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnContinue.Checked && (radibtnPeriodPentad.Checked || radibtnPeriodTen.Checked || radibtnPeriodMonth.Checked))
            {
                combxTimeMonthEnd.Enabled = true;
                cbxJuPingLayout.Enabled = false;
                cbxJuPingStat.Enabled = false;
            }
        }

        private void radibtnPeriod_CheckedChanged(object sender, EventArgs e)
        {
            if (radibtnPeriod.Checked && (radibtnPeriodPentad.Checked || radibtnPeriodTen.Checked ))    //|| radibtnPeriodMonth.Checked))
            {
                combxTimeMonthEnd.Enabled = false;
                combxTimeMonthEnd.SelectedIndex = combxTimeMonthStart.SelectedIndex;
            }
            if (radibtnPeriod.Checked)
            {
                cbxJuPingLayout.Enabled = true;
                cbxJuPingStat.Enabled = true;
            }
        }
    }
}
