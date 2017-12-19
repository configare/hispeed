using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class frmMultiClip : Form
    {
        public frmMultiClip()
        {
            InitializeComponent();
        }
        private string ConfigFileName = "cutconfig.xml";

        private  List<CoordInfo> ListCoords = new List<CoordInfo>();
        private string OutDir = "";
        private void button2_Click(object sender, EventArgs e)
        {
            if (!this.IsDisposed)
            {
                this.Close();
            }
        }
        
        private void LoadConfigXml()
        {
            ClipData clip = new ClipData();
            InputArgs modelinput = clip.GetConfigByXml(ConfigFileName);
            string inputstr = modelinput.InputDir;
            string[] fileInfo = inputstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (fileInfo == null || fileInfo.Length != 2)
            {
                return;
            }
            else
            {
                this.txtinput.Text = fileInfo[0];
                this.txtextend.Text = string.IsNullOrEmpty(fileInfo[1]) ? "*.*" : fileInfo[1];
            }
            this.txtoutput.Text = modelinput.OutDir;
            //列表
            this.ListCoords = modelinput.ListCoord;
            LoadListBoxByCoords();

        }
        private void  LoadListBoxByCoords()
        {
            this.lbcoord.Items.Clear();
            for(int i=0;i<this.ListCoords.Count;i++)
            {
                object item = string.Format("{0}:MinX:{1},MaxX:{2},MinY:{3},MaxY:{4}", ListCoords[i].CoordName, ListCoords[i].MinX, ListCoords[i].MaxX, ListCoords[i].MinY, ListCoords[i].MaxY);
                this.lbcoord.Items.Add(item);
            }
        }
        private void SetConfigXml()
        {
            InputArgs inputmodel = PackageModel();
            if(inputmodel==null)
            {
                MessageBox.Show("输入参数有误！", "提示信息");
                return;
            }
            else
            {
                ClipData clip = new ClipData();
                clip.SetXmlConfig(ConfigFileName, inputmodel);
                //
                OutDir = inputmodel.OutDir;//操作完成方便打开
            }
        }
        private InputArgs PackageModel()
        {
            if(!CheckInputValue())
            {
                return null;
            }
            try
            {
                InputArgs inputmodel = new InputArgs();
                string inputdir = string.Format("{0}|{1}", this.txtinput.Text.Trim(), string.IsNullOrEmpty(this.txtextend.Text) ? "*.*" : this.txtextend.Text);
                inputmodel.InputDir = inputdir;
                inputmodel.OutDir = this.txtoutput.Text;
                if (this.ListCoords.Count > 0)
                    inputmodel.ListCoord = ListCoords;
                return inputmodel;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        private bool CheckInputValue()
        {
            if(string.IsNullOrEmpty(this.txtinput.Text))
            {
                MessageBox.Show("裁切文件夹不能为空！", "提示信息");
                return false;
            }
            if(string.IsNullOrEmpty(this.txtoutput.Text))
            {
                MessageBox.Show("裁切输出文件夹不能为空！", "提示信息");
                return false;
            }

            if (this.ListCoords.Count <= 0)
            {
                MessageBox.Show("输出范围不能为空！", "提示信息");
                return false;
            }
            return true;
        }

        private void btnmuticlip_Click(object sender, EventArgs e)
        {
            SetConfigXml();
            System.Diagnostics.Process.Start("GeoDo.Smart.MutiClip.exe", Application.StartupPath+"\\"+ ConfigFileName);
            
            
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadConfigXml();
        }

        private void btninputselect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                this.txtinput.Text = dlg.SelectedPath;
            }
            
        }

        private void btnoutputselect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.txtoutput.Text = dlg.SelectedPath;
            }
        }

        private void btnAddcoord_Click(object sender, EventArgs e)
        {
            try
            {
                CoordInfo coord = new CoordInfo();
                coord.CoordName = string.IsNullOrEmpty(this.txtoutputname.Text) ? "DXX" : this.txtoutputname.Text;
                coord.MinX = double.Parse(this.txtminx.Text.Trim());
                coord.MaxX = double.Parse(this.txtmaxx.Text.Trim());
                coord.MinY = double.Parse(this.txtminy.Text.Trim());
                coord.MaxY = double.Parse(this.txtmaxy.Text.Trim());
                if(ListCoords.Count(o=>o.CoordName==coord.CoordName)>0)
                {
                    MessageBox.Show("坐标范围名称不能重复！", "提示信息");
                    return;
                }
                this.ListCoords.Add(coord);
                LoadListBoxByCoords();
            }
            catch(Exception ex)
            {
                MessageBox.Show("输入内容不符合规范", "提示信息");
            }
            
        }

        private void btndelcoord_Click(object sender, EventArgs e)
        {
            this.ListCoords.RemoveAt(this.lbcoord.SelectedIndex);
            LoadListBoxByCoords();
        }

        private void lbcoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lbcoord.SelectedIndex!=-1)
            {
                CoordInfo crood = ListCoords[this.lbcoord.SelectedIndex];
                this.txtoutputname.Text = crood.CoordName;
                this.txtminx.Text = crood.MinX.ToString();
                this.txtmaxx.Text = crood.MaxX.ToString();
                this.txtminy.Text = crood.MinY.ToString();
                this.txtmaxy.Text = crood.MaxY.ToString();
            }
        }

        private void btnedit_Click(object sender, EventArgs e)
        {
            if (lbcoord.SelectedIndex != -1)
            {
                int i = lbcoord.SelectedIndex;
                try
                {

                    ListCoords[i].CoordName = this.txtoutputname.Text;
                    ListCoords[i].MinX = double.Parse(this.txtminx.Text.Trim());
                    ListCoords[i].MaxX = double.Parse(this.txtmaxx.Text.Trim());
                    ListCoords[i].MinY = double.Parse(this.txtminy.Text.Trim());
                    ListCoords[i].MaxY = double.Parse(this.txtmaxy.Text.Trim());
                    if(ListCoords.Count(o=>o.CoordName==this.txtoutputname.Text.Trim())>1)
                    {
                        MessageBox.Show("修改的名称与现有名称重复！", "提示信息");
                        return;
                    }
                    LoadListBoxByCoords();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("修改内容不正确！","提示信息");
                    return;
                }
            }
        }

       
    }
}
