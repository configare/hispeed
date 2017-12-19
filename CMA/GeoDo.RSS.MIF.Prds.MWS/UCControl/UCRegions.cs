using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
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
    public partial class UCRegions : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        //private string areaNams = null;
        private string radio = "no";
        private List<string> Infos = new List<string>();
        public UCRegions()
        {
            InitializeComponent();
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
            return Infos;
            
        }
        #endregion
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
            string areaNams = "";
            while (myEnumerator.MoveNext() != false)
            {
                index = (int)myEnumerator.Current;
                checkedListBox1.SelectedItem = checkedListBox1.Items[index];
                areaNams = areaNams + checkedListBox1.Text + ",";

            }
            return areaNams;
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Infos.Clear();
            //areaNams = null; 
            string areaIsnull = GetCheckedItemsText(checkedListBox1); //获得所选区域名称 
            if (string.IsNullOrEmpty(areaIsnull))
                MessageBox.Show("请选择区域");
            if (radioButton1.Checked)
                radio = "yes";
            Infos.Add(areaIsnull);
            Infos.Add(radio);
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Infos.Clear();
            //areaNams = null; 
            string areaIsnull = GetCheckedItemsText(checkedListBox1); //获得所选区域名称 
            if (string.IsNullOrEmpty(areaIsnull))
                MessageBox.Show("请选择区域");
            if (radioButton3.Checked)
                radio = "no";
            Infos.Add(areaIsnull);
            Infos.Add(radio);
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }
}
