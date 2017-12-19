#region Version Info
/*========================================================================
* 功能概述：
* 面积统计设置窗体类。
* 创建者：董玮     时间：2013/8/13
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.UI;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    /// <summary>
    /// 类名：frmAreaStatSetting
    /// 属性描述：
    /// 创建者：董玮         创建日期：2013/8/13 
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class frmAreaStatSetting : Form
    {
        public const string VECTOR_FILE_NAME = @"SystemData\VectorTemplate\";
        public const string RASTER_FILE_NAME = @"SystemData\RasterTemplate\";
        private List<AreaStatItem> _statItemList = new List<AreaStatItem>();

        public frmAreaStatSetting()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            //根据配置文件初始化控件
            AreaStatItem[] statItems = AreaStatProvider.GetAreaStatItems();
            if (statItems == null || statItems.Length < 1)
                return;
            _statItemList.AddRange(statItems);
            for (int i = 0; i < statItems.Length; i++)
            {
                cmbStatItems.Items.Add(statItems[i].Name);
            }
            cmbStatItems.SelectedIndex = 0;
            txtName.Text = statItems[0].MenuName;
            ChangeControlsVisibleByStatType(statItems[0]);
        }


        private void ChangeControlsVisibleByStatType(AreaStatItem statItem)
        {
            if (statItem.StatFileType == enumStatTemplateType.Raster)
            {
                btnOpenInfoFile.Enabled = true;
                labFields.Visible = true;
                txtInfoFile.Visible = true;
                btnOpenInfoFile.Visible = true;
                labFields.Visible = false;
                cmbFields.Visible = false;
                labInfoFile.Visible = true;
                labColumnNames.Enabled = true;
                txtColumnNames.Enabled = true;
                if(statItem.ColumnNames!=null&&statItem.ColumnNames.Length==3)
                {
                    string namesString=null;
                    string[] columnNames=statItem.ColumnNames;
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        namesString = namesString + columnNames[i] + ",";
                    }
                    namesString = namesString.Remove(namesString.Length - 1);
                    txtColumnNames.Text = namesString;
                }
            }
            else
            {
                labFields.Visible = true;
                labInfoFile.Visible = false;
                cmbFields.Visible = true;
                btnOpenInfoFile.Visible = false;
                labInfoFile.Visible = false;
                txtInfoFile.Visible = false;
                labColumnNames.Enabled = false;
                txtColumnNames.Enabled = false;
                txtColumnNames.Text = "";
                using (IVectorFeatureDataReader vdr = VectorDataReaderFactory.GetUniversalDataReader(statItem.FileName) as IVectorFeatureDataReader)
                {
                    if (vdr == null || vdr.ShapeType != enumShapeType.Polygon)
                        return;
                    cmbFields.Items.Clear();
                    if (vdr.Fields != null && vdr.Fields.Length > 0)
                    {
                        foreach (string field in vdr.Fields)
                        {
                            cmbFields.Items.Add(field);
                        }
                        string settedField = statItem.StatField;
                        if (cmbFields.Items.Contains(settedField))
                            cmbFields.SelectedItem = settedField;
                        else
                            cmbFields.SelectedIndex = 0;
                    }
                }
            }
            if (string.IsNullOrEmpty(statItem.InfoFileName))
                txtInfoFile.Text = "";
            else
            {
                txtInfoFile.Text = statItem.InfoFileName;
            }
            ChangeFileList(statItem.StatFileType);
        }

        private void ChangeFileList(enumStatTemplateType templateType)
        {
            cmbStatFiles.Items.Clear();
            foreach (AreaStatItem item in _statItemList)
            {
                if (item.StatFileType == templateType)
                {
                    cmbStatFiles.Items.Add(item.FileName);
                    if (item.Name == cmbStatItems.SelectedItem.ToString())
                        cmbStatFiles.SelectedItem = item.FileName;
                }
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //点击保存仅保存当前修改的统计分析项
            string fileName = cmbStatFiles.SelectedItem.ToString();
            string infoFileName = txtInfoFile.Text;
            string[] newFileNames = CopyFileToSystemFolder(new string[] { fileName, infoFileName });
            //修改_statItemList
            if (newFileNames == null || newFileNames.Length != 2)
                return;
            int index = cmbStatItems.SelectedIndex;
            _statItemList[index].MenuName = txtName.Text;
            _statItemList[index].FileName = newFileNames[0];
            if(cmbFields.Visible)
                _statItemList[index].StatField = cmbFields.SelectedItem.ToString();
            else
                _statItemList[index].InfoFileName = newFileNames[1];
            if(txtColumnNames.Enabled)
            {
                string names=txtColumnNames.Text;
                if(!string.IsNullOrEmpty(names))
                {
                    string[] nameArray=names.Split(new char[]{',','，'});
                    _statItemList[index].ColumnNames = nameArray;
                }
            }
            //保存至配置文件
            AreaStatProvider.SaveToXML(new AreaStatItem[] { _statItemList[cmbStatItems.SelectedIndex] });
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                bool isRasterFile = false;
                int selectIndex = cmbStatItems.SelectedIndex;
                if (_statItemList[selectIndex].StatFileType == enumStatTemplateType.Raster)
                {
                    dialog.Filter = SupportedFileFilters.SrfFilterString + "|" +
                        SupportedFileFilters.AllFilterString;
                    isRasterFile = true;
                }
                else
                {
                    dialog.Filter = SupportedFileFilters.VectorFilterString + "|" +
                        SupportedFileFilters.AllFilterString;
                }
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //检查是否是面状矢量
                    if (!isRasterFile)
                    {
                        if (CheckVectorFile(dialog.FileName))
                        {
                            string fileName = dialog.FileName;
                            cmbStatFiles.Items.Add(fileName);
                            cmbStatFiles.SelectedItem = fileName;
                        }
                        else
                        {
                            MsgBox.ShowError("请选择面状矢量！");
                        }
                    }
                    else
                    {
                        string fileName = dialog.FileName;
                        cmbStatFiles.Items.Add(fileName);
                        cmbStatFiles.SelectedItem = fileName;
                        _statItemList[cmbStatItems.SelectedIndex].FileName = fileName;
                    }
                }
            }
        }

        private string[] CopyFileToSystemFolder(params string[] fileNames)
        {
            if (fileNames == null || fileNames.Length < 1)
                return null;
            if (string.IsNullOrEmpty(fileNames[0]) || !File.Exists(fileNames[0]))
                return null; 
            string extension = Path.GetExtension(fileNames[0]).ToUpper();
            if (extension == ".SHP")
            {
                if (Path.GetFullPath(fileNames[0]).Contains(AppDomain.CurrentDomain.BaseDirectory))
                    return new string[] {fileNames[0],null};
                string systomVectorDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VECTOR_FILE_NAME);
                string newShpFileName = systomVectorDir + Path.GetFileName(fileNames[0]);
                string fileName = Path.GetFileNameWithoutExtension(fileNames[0]);
                string dbfFileName = Path.Combine(Path.GetDirectoryName(fileNames[0]), fileName + ".dbf");
                if (!File.Exists(dbfFileName))
                    return null;
                string newDbfFileName=newShpFileName.Replace("shp","dbf");
                string shxFileName = Path.Combine(Path.GetDirectoryName(fileNames[0]), fileName + ".shx");
                if (!File.Exists(shxFileName))
                    return null;
                string newShxFileName=newShpFileName.Replace("shp","shx");
                //矢量文件拷贝至系统目录
                if (!Directory.Exists(systomVectorDir))
                    Directory.CreateDirectory(systomVectorDir);
                File.Copy(fileNames[0], newShpFileName,true);
                File.Copy(dbfFileName, newDbfFileName,true);
                File.Copy(shxFileName, newShxFileName,true);
                return new string[] { newShpFileName, null };
            }
            else
            {
                string systomRasterDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RASTER_FILE_NAME);
                string newRasterFileName = systomRasterDir + Path.GetFileName(fileNames[0]);
                if (fileNames[0] == newRasterFileName)
                    return new string[] { newRasterFileName, fileNames[1] };
                string newInfoFileName=systomRasterDir+Path.GetFileName(fileNames[1]);
                File.Copy(fileNames[0], newRasterFileName);
                File.Copy(fileNames[1], newInfoFileName);
                return new string[] { newRasterFileName, newInfoFileName };
            }
        }

        private bool CheckVectorFile(string fileName)
        {
            using (IVectorFeatureDataReader vdr = VectorDataReaderFactory.GetUniversalDataReader(fileName) as IVectorFeatureDataReader)
            {
                if (vdr == null)
                    return false;
                if (vdr.ShapeType != enumShapeType.Polygon)
                    return false;
                else
                    return true;
            }
        }

        private void cmbStatItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStatItems.SelectedIndex < 0)
                return;
            int i = cmbStatItems.SelectedIndex;
            txtName.Text = _statItemList[i].MenuName;
            ChangeControlsVisibleByStatType(_statItemList[i]);
        }

        private void btnOpenInfoFile_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dialog=new OpenFileDialog())
            {
                dialog.Filter = SupportedFileFilters.TextFilterString;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtInfoFile.Text = dialog.FileName;
                    int selectIndex = cmbStatItems.SelectedIndex;
                    _statItemList[selectIndex].InfoFileName = dialog.FileName;
                }
            }
        }

        private void cmbStatFiles_TextChanged(object sender, EventArgs e)
        {
            string fileName = cmbStatFiles.Text;
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return;
            int selectIndex = cmbStatItems.SelectedIndex;
            //添加字段
            if (_statItemList[selectIndex].StatFileType == enumStatTemplateType.Vector)
            {  
                using (IVectorFeatureDataReader vdr = VectorDataReaderFactory.GetUniversalDataReader(fileName) as IVectorFeatureDataReader)
               {
                   if (vdr == null || vdr.ShapeType != enumShapeType.Polygon)
                      return ;
                   cmbFields.Items.Clear();
                    if(vdr.Fields!=null&&vdr.Fields.Length>0)
                    {
                        foreach (string field in vdr.Fields)
                        {
                            cmbFields.Items.Add(field);
                        }
                        string settedField = _statItemList[selectIndex].StatField;
                        if (cmbFields.Items.Contains(settedField))
                            cmbFields.SelectedItem = settedField;
                        else
                            cmbFields.SelectedIndex = 0;
                   }
               }
            }
        }
    }
}
