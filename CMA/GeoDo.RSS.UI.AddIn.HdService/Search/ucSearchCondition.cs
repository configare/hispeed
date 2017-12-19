using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.Windows;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class ucSearchCondition : UserControl, IToolWindowContent
    {
        private ISmartSession _smartSession;
        private string _fileType; //= "RasterData"
        private Dictionary<string, string> _productDic = new Dictionary<string, string>();
        private string _tempArgsFile = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\TempQueryFile.txt";

        public ucSearchCondition()
        {
            InitializeComponent();
            LoadProduct();
            InitProduct();
        }

        public void Apply(Core.UI.ISmartSession session)
        {
            _smartSession = session;
        }

        public void Free()
        {
            _smartSession = null;
        }

        internal void SetFileType(string argument)
        {
            _fileType = argument;
        }

        private void InitProduct()
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-1);
            dateTimePicker2.Value = DateTime.Now;
            Dictionary<string, string> initInfoDic = TryReadArgs();
            if (initInfoDic != null)
            {
                SetArg(comboBox1, "卫星", initInfoDic);
                SetArg(comboBox2, "传感器", initInfoDic);
                SetArg(comboBox3, "分辨率", initInfoDic);
                SetArg(comboBox4, "产品名", initInfoDic);
                SetArg(comboBox5, "统计周期", initInfoDic);
                SetArg(comboBox6, "统计标识", initInfoDic);
            }
        }

        private void SetArg(ComboBox initControl, string dicKey, Dictionary<string, string> initInfoDic)
        {
            if (initInfoDic.ContainsKey(dicKey))
                initControl.Text = initInfoDic[dicKey];
        }

        private Dictionary<string, string> TryReadArgs()
        {
            Dictionary<string, string> resultStr = new Dictionary<string, string>();
            if (!File.Exists(_tempArgsFile))
                return null;
            string[] argContexts = File.ReadAllLines(_tempArgsFile, Encoding.Default);
            if (argContexts == null || argContexts.Length == 0)
                return null;
            string[] tempContext = null;
            foreach (string item in argContexts)
            {
                tempContext = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempContext == null || tempContext.Length == 0)
                    continue;
                resultStr.Add(tempContext[0], tempContext[1]);
            }
            return resultStr.Count == 0 ? null : resultStr;
        }

        private string GetcountIdentify(string countIdentify)
        {
            switch (countIdentify)
            {
                case "日自动":
                    return GetproductName(comboBox4.Text) + "Stat";
                case "面积统计":
                    return "AreaStat";
                case "频次统计":
                    return "FreqStat";
                default:
                    return null;
            }
        }

        private string GetcountPeroid(string countPeroid)
        {
            if (comboBox6.Text == "日自动")
                return "mosaicEveryDay";
            switch (countPeroid)
            {
                case "上旬":
                    return "firstTenDay";
                case "中旬":
                    return "middleTenDay";
                case "下旬":
                    return "lastTenDay";
                case "月":
                    return "month";
                case "季":
                    return "season";
                case "年":
                    return "year";
                default:
                    return null;
            }

        }

        private string GetproductName(string productName)
        {
            if (_productDic.ContainsKey(productName))
                return _productDic[productName];
            else
                return productName;
        }

        private void LoadProduct()
        {
            ThemeDef def = MonitoringThemeFactory.GetAllThemes()[0];//"CMA"
            ProductDef[] products = def.Products;
            foreach (ProductDef prd in products)
            {
                if (string.IsNullOrWhiteSpace(prd.Name))
                    continue;
                string name = prd.Name.Replace(" ", "");
                if (_productDic.ContainsKey(name))
                    continue;
                _productDic.Add(name, prd.Identify);
            }
            comboBox4.Items.Clear();
            foreach (string product in _productDic.Keys)
            {
                comboBox4.Items.Add(product);
            }
            comboBox4.SelectedItem = comboBox1.Items[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string argument = string.Format("satellite={0};sensor={1};resolution={2};productName={3};countPeroid={4};countIdentify={5};beginDate={6};endDate={7};DataType={8}",
                    comboBox1.Text,
                    comboBox2.Text,
                    comboBox3.Text,
                    GetproductName(comboBox4.Text),
                    GetcountPeroid(comboBox5.Text),
                    GetcountIdentify(comboBox6.Text),
                    dateTimePicker1.Value.ToString("yyyy-MM-dd"),
                    dateTimePicker2.Value.ToString("yyyy-MM-dd"),
                    _fileType);
            ICommand cmd = _smartSession.CommandEnvironment.Get(39000);
            cmd.Execute(argument);
        }

        private void btSaveArgs_Click(object sender, EventArgs e)
        {
            List<string> argsList = new List<string>();
            argsList.Add("卫星:" + comboBox1.Text);
            argsList.Add("传感器:" + comboBox2.Text);
            argsList.Add("分辨率:" + comboBox3.Text);
            argsList.Add("产品名:" + comboBox4.Text);
            argsList.Add("统计周期:" + comboBox5.Text);
            argsList.Add("统计标识:" + comboBox6.Text);
            File.WriteAllLines(_tempArgsFile, argsList.ToArray(), Encoding.Default);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            switch (cb.Text.ToLower())
            {
                case "fy2e":
                    comboBox2.Text = "VISSR";
                    break;
                case "fy3a":
                case "fy3b":
                case "fy3c":
                    comboBox2.Text = "VIRR";
                    break;
                case "aqua":
                case "terra":
                    comboBox2.Text = "MODIS";
                    break;
                case "NOAA16":
                case "NOAA17":
                case "NOAA18":
                case "NOAA19":
                    comboBox2.Text = "AVHRR";
                    break;
                default:
                    break;
            }
        }
    }
}
