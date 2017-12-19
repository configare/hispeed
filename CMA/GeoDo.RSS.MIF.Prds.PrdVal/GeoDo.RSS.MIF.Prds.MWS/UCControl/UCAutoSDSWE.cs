using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.Project;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using GeoDo.MicroWaveSnow.FYJoin;
using System.Collections;
using DataProductDownload;
using FTPLibrary;
using GeoDo.FileProcess.Utility;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public partial class UCAutoSDSWE : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private static Regex DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
        private string[] filenames = new string[] { };
        private List<string> fileInfo = new List<string>();
        private string beginDay = null;
        private string endDay = null;
        private string orbit = null;
        private string area = null;
        private string otherInformation = null;
        private string areaNams = null;
        private string radio = "no";
        private string ExtractParameters = "";
        private string sdParameters = "";
        public UCAutoSDSWE()
        {
            InitializeComponent();
            GetFilePath();
            
        }
        private void GetFilePath()
        {
            string autopath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\MWS\AutoPath.txt");
            if (File.Exists(autopath))
            {
                FileStream fauto = new FileStream(autopath, FileMode.Open, FileAccess.Read);
                StreamReader rauto = new StreamReader(fauto);
                txtFiles.Text = rauto.ReadLine();
                rauto.Close();
                fauto.Close();
            }
            
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            //打开文件夹
            using (FolderBrowserDialog frm = new FolderBrowserDialog())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtFiles.Text = frm.SelectedPath;
                    string autopath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SystemData\ProductArgs\MWS\AutoPath.txt");
                    FileStream fauto = new FileStream(autopath, FileMode.Create, FileAccess.Write);
                    StreamWriter rauto = new StreamWriter(fauto);
                    rauto.WriteLine(txtFiles.Text);
                    rauto.Close();
                    fauto.Close();
                    string pathinput = "";
                    if (txtFiles.Text.Trim() != "")
                    {
                        pathinput = txtFiles.Text.Trim();
                    }
                    filenames = Directory.GetFiles(pathinput, "*.hdf", SearchOption.AllDirectories);
                }
            }
        }
        # region IArgumentEditorUI 成员
        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
        public object GetArgumentValue()
        {
            return fileInfo;
        }
        #endregion
        private void btnOK_Click(object sender, EventArgs e)
        {
            fileInfo = new List<string>(fileToPrj());
            otherInformation = orbit + ":" + areaNams + ":"+ radio + ":"+ beginDay + ":" + endDay;
            fileInfo.Add(otherInformation);
            ParseXml(txtsdPara.Text);
            fileInfo.Add(ExtractParameters);
            fileInfo.Add(sdParameters);
            if (_handler != null)
                _handler(GetArgumentValue());
        }
        private void ParseXml(string xmlfile)
        {
            string xmlfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
            xmlfile = Path.Combine(xmlfilePath,xmlfile);
            if (!File.Exists(xmlfile))
                throw new FileNotFoundException("参数文件不存在" + xmlfile, xmlfile);
            if (string.IsNullOrWhiteSpace(xmlfile))
                throw new ArgumentNullException("argXml", "参数文件为空");
            XElement xml = XElement.Load(xmlfile);
            XElement extractPara = xml.Element("ExtractParameters");
            if (extractPara != null)
                ExtractParameters = extractPara.Value;
            XElement sdPara = xml.Element("sdParameters");
            if (sdPara != null)
                sdParameters = sdPara.Value;
        }
       
        /// <summary>
        /// 获得所选区域项
        /// </summary>
        /// <param name="checkedListBox1"></param>
        /// <returns></returns>
        private string GetCheckedItemsText(CheckedListBox checkedListBox1)
        {
            IEnumerator myEnumerator;
            myEnumerator = checkedListBox1.CheckedIndices.GetEnumerator();
            int index;
            while (myEnumerator.MoveNext() != false)
            {
                index = (int)myEnumerator.Current;
                checkedListBox1.SelectedItem = checkedListBox1.Items[index];
                areaNams = areaNams + checkedListBox1.Text + ",";

            }
            return areaNams;
        }
        /// <summary>
        /// ftp下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string year = dateTimePickerBegin.Text.Substring(0, 4);
            string month = dateTimePickerBegin.Text.Substring(5, 2);
            string day = dateTimePickerBegin.Text.Substring(8, 2);
            beginDay = year + month + day;
            string year2 = dateTimePickerEnd.Text.Substring(0, 4);
            string month2 = dateTimePickerEnd.Text.Substring(5, 2);
            string day2 = dateTimePickerEnd.Text.Substring(8, 2);
            endDay = year2 + month2 + day2;
            FormDataDownload fdd = new FormDataDownload();
            fdd.SavePath = txtFiles.Text;
            fdd.DateTimeBegin = beginDay;
            fdd.DateTimeEnd = endDay;
            fdd.Sensor = "MWRI";
            fdd.Show();
        }
        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

            string[] filesPrj = fileToPrj();
            int nday = Convert.ToInt32(endDay) - Convert.ToInt32(beginDay);
            if (orbit == "MWRIA" || orbit == "MWRID")
            {
                if (filesPrj.Length<= nday * 13)
                    MessageBox.Show(" 数据可能不足，请从ftp下载数据 ");
            }
            else
            {
                MessageBox.Show(" 数据检查完毕 ");
            }
        }
        private string[] fileToPrj()
        {
            string pathinput = "";
            if (txtFiles.Text.Trim() != "")
            {
                pathinput = txtFiles.Text.Trim();
            }
            filenames = Directory.GetFiles(pathinput, "*.hdf", SearchOption.AllDirectories);
            //先筛选条件
            if (checkBoxA.Checked)//升降轨
            {
                orbit = "MWRIA";
            }
            else
            {
                if (checkBoxD.Checked)
                    orbit = "MWRID";
                else
                {
                    orbit = "MWRI";
                }
            }
            if (checkBoxA.Checked && checkBoxD.Checked)
                orbit = "MWRI";

            string areaIsnull = GetCheckedItemsText(checkedListBox1); //获得所选区域名称 
            if (string.IsNullOrEmpty(areaIsnull))
                MessageBox.Show("请选择区域");
            if (radioButton1.Checked) //默认不拼接
                radio = "yes";
            else
            {
                if (radioButton3.Checked)
                    radio = "no";
            }
            string year = dateTimePickerBegin.Text.Substring(0, 4);
            string month = dateTimePickerBegin.Text.Substring(5, 2);
            string day = dateTimePickerBegin.Text.Substring(8, 2);
            beginDay = year + month + day;
            Int32 dateBegin = Convert.ToInt32(year+ month+day);
            string year2 = dateTimePickerEnd.Text.Substring(0, 4);
            string month2 = dateTimePickerEnd.Text.Substring(5, 2);
            string day2 = dateTimePickerEnd.Text.Substring(8, 2);
            endDay = year2 + month2 + day2;
            //判断文件夹里是否包含满足条件的文件
            Int32 dateEnd = Convert.ToInt32(endDay);
            string filedate = "";
            List<string> filesPrj = new List<string>();
            foreach (string file in filenames)
            {
                Match m = DataReg.Match(Path.GetFileName(file));
                if (m.Success)
                {
                    filedate = m.Value;
                }
                Int32 filedateDig = Convert.ToInt32(filedate);
                string ext = Path.GetExtension(file).ToUpper();
                Regex r = new Regex("MWRI");
                Match m1 = r.Match(file);
                if (ext != ".HDF" || !GeoDo.HDF5.HDF5Helper.IsHdf5(file) || !m1.Success)
                {

                }
                else
                {
                    if (file.Contains(orbit) && (filedateDig <= dateEnd && filedateDig >= dateBegin) && !file.Contains("OBCXX") && !file.Contains("GEOXX"))
                    {
                        filesPrj.Add(file);
                    }
                }
            }
            return filesPrj.ToArray();
        }
        private void btnOpen1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS");
                frm.Filter = "雪深计算参数文件（sdParas_*.xml）|sdParas_*.xml"; 
                if(frm.ShowDialog() ==DialogResult.OK)
                {
                    txtsdPara.Text = Path.GetFileName(frm.FileName);
                }
            }
        }
    }
}
