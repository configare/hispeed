using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using System.Xml.Linq;

namespace GeoDo.RSS.UI.AddIn.AddInMgr
{
    public partial class frmAddPlugin : Form
    {        
        private ISmartSession _smartSession = null;
        private XmlDocument _xmlDoc = null;
        private RadRibbonBarGroup _rbgroup = null;
        private bool _isExchanged = false;
        private BindingList<CustomPlugin> _plugins = new BindingList<CustomPlugin>();
        private int num = 1;
        BindingSource fBindingSource = new BindingSource();

        public frmAddPlugin(ISmartSession session, RadRibbonBarGroup rbgroup)
        {
            InitializeComponent();
            _smartSession = session;
            _rbgroup = rbgroup;
            InitForm();
        }

        private void InitForm()
        {
            InitLoadXML();
            fBindingSource.DataSource = _plugins;
            lstToolName.DataSource = fBindingSource;
            lstToolName.DisplayMember = "Title";
            lstToolName.ValueMember = "Title";
            txtTitle.DataBindings.Add("Text", fBindingSource, "Title");
            txtCommand.DataBindings.Add("Text", fBindingSource, "Command");
            txtInitDir.DataBindings.Add("Text", fBindingSource, "InitDir");
            txtParameter.DataBindings.Add("Text", fBindingSource, "Parameter");
            cbxParameter.Items.Add("路径");
            cbxParameter.Items.Add("目录");
            cbxParameter.Items.Add("文件名");  
            cbxInitDir.Items.Add("项目录");
            cbxInitDir.Items.Add("二进制目录");
            cbxInitDir.Items.Add("目标文件目录");
            if (_plugins.Count > 0)
            {
                lstToolName.SelectedIndex = 0;
                btnUp.Enabled = false;
                if (_plugins.Count == 1)
                {
                    btnDown.Enabled = false;
                }
                else btnDown.Enabled = true;
            }
            lstToolName.SelectedIndexChanged += new EventHandler(lstToolName_SelectedIndexChanged);
            cbxParameter.SelectedIndexChanged+=new EventHandler(cbxParameter_SelectedIndexChanged);
            cbxInitDir.SelectedIndexChanged+=new EventHandler(cbxInitDir_SelectedIndexChanged);
            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnDelete.Click += new EventHandler(btnDelete_Click);
            btnUp.Click += new EventHandler(btnUp_Click);
            btnCommand.Click += new EventHandler(btnCommand_Click);
            btnOK.Click+=new EventHandler(btnOK_Click);
            btnCancel.Click+=new EventHandler(btnCancel_Click);
            btnApply.Click+=new EventHandler(btnApply_Click);
            txtTitle.TextChanged += new EventHandler(txtTitle_TextChanged);
            txtCommand.TextChanged+=new EventHandler(txtCommand_TextChanged);
            txtInitDir.TextChanged+=new EventHandler(txtInitDir_TextChanged);
            txtParameter.TextChanged+=new EventHandler(txtParameter_TextChanged);
            btnDown.Click += new EventHandler(btnDown_Click);     
        }

        private void InitLoadXML()
        {
            if (_xmlDoc == null)
            {
                _xmlDoc = new XmlDocument();
                string filename = System.AppDomain.CurrentDomain.BaseDirectory + "DefaultUI.xml";
                _xmlDoc.Load(filename);
            }
            XElement root = XElement.Load(System.AppDomain.CurrentDomain.BaseDirectory + "DefaultUI.xml");
            IEnumerable<XElement> node = from item in root.Element("UITabs").Elements("UITab")
                                         where item.Attribute("name").Value.Equals("addins")
                                         select item;
            IEnumerable<XElement> botton = from bu in node.First().Element("UIItems").Elements("UICommandGroup").Elements("UIButton")
                                           select bu;
            foreach (XElement it in botton)
            {
                string commandAttribute = it.Attribute("argument").Value;
                string[] arguments = commandAttribute.Split(new char[] { ':' });
                string command, parameter;
                if (arguments.Count() > 2)
                {
                    command = arguments[0] + ":" + arguments[1];
                    parameter = arguments[2];
                }
                else
                {
                    command = arguments[0];
                    parameter = arguments[1];
                }
                _plugins.Add(new CustomPlugin(it.Attribute("name").Value,command, parameter, it.Attribute("initDir").Value));
            }          
        }

        private void lstToolName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex == -1)
            {
                txtTitle.Text = null;
                return;
            }
            else
            {              
                if (lstToolName.SelectedIndex != 0)
                {
                    btnUp.Enabled = true;
                }
                else
                    btnUp.Enabled = false;
                if (lstToolName.SelectedIndex != lstToolName.Items.Count - 1)
                {
                    btnDown.Enabled = true;
                }
                else
                    btnDown.Enabled = false;
                foreach (CustomPlugin item in _plugins)
                {
                    if (item.Title == lstToolName.SelectedItem.ToString())
                    {
                        txtCommand.Text = item.Command;
                        txtParameter.Text = item.Parameter;
                        txtInitDir.Text = item.InitDir;
                        break;
                    }
                }
                txtTitle.Text = lstToolName.SelectedItem.ToString();
            }
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {   
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "打开文件";
                dlg.Multiselect = false;
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                dlg.Filter = "所有可执行文件(*.exe;*.pif;*.bit;*.cmd)|*.exe;*.pif;*.bit;*.cmd|批处理文件(*.bat;*cmd)|*.bat;*cmd|所有文件(*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtCommand.Text = dlg.FileName;                   
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _plugins.Add(new CustomPlugin(lstToolName.Items.Count.ToString(),"[新插件"+ num+"]",null,null,null));
            lstToolName.SelectedIndex = lstToolName.Items.Count-1;
            num++;
            _isExchanged = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex > -1)
            { 
                foreach (CustomPlugin plugin in _plugins)
                {
                    if (plugin.Title == lstToolName.SelectedItem.ToString())
                    {
                        _plugins.Remove(plugin);
                        break;
                    }
                } 
                lstToolName.ClearSelected();
                lstToolName.SelectedIndex = lstToolName.Items.Count - 1;
            }
            if (num > 0)
            {
                num--;
            }
            _isExchanged = true;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex > 0)
            {
                //foreach (CustomPlugin plugin in _plugins)
                for(int k = _plugins.Count-1;k>=0;k--)
                {
                    CustomPlugin plugin = _plugins[k];
                    if (plugin.Title == lstToolName.SelectedItem.ToString())
                    {
                        int index = int.Parse(plugin.Index)-1;
                        foreach (CustomPlugin item in _plugins)
                        {
                            if (int.Parse(item.Index) == index)
                            {
                                item.Index = (index + 1).ToString();
                            }
                            break;
                        }
                        plugin.Index = index.ToString();
                        break;
                    }
                }
                int i = lstToolName.SelectedIndex;
                object li = lstToolName.SelectedItem;
                lstToolName.Items.Remove(li);
                lstToolName.Items.Insert(i - 1, li);
                lstToolName.SelectedIndex = i - 1;
            }

            _isExchanged = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex > 0)
            {
                foreach (CustomPlugin plugin in _plugins)
                {
                    if (plugin.Title == lstToolName.SelectedItem.ToString())
                    {
                        int index = int.Parse(plugin.Index) + 1;
                        foreach (CustomPlugin item in _plugins)
                        {
                            if (int.Parse(item.Index) == index)
                            {
                                item.Index = (index - 1).ToString();
                            }
                            break;
                        }
                        plugin.Index = index.ToString();
                        break;
                    }
                }
                int i = lstToolName.SelectedIndex;
                object li = lstToolName.SelectedItem;
                lstToolName.Items.Remove(li);
                lstToolName.Items.Insert(i + 1, li);
                lstToolName.SelectedIndex = i + 1;
            }
            _isExchanged = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (txtCommand.Text != "")
            {
                string iconDir = AppDomain.CurrentDomain.BaseDirectory + "\\icon\\" + txtTitle.Text + ".png";
                Icon icon = GetFileIcoHelper.GetFileIcon(txtCommand.Text, false);
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\icon"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\icon");
                GetFileIcoHelper.SaveFileIcon(iconDir, icon);
            }
            if (_isExchanged)
            {
                List<CustomPlugin> pluginList = new List<CustomPlugin>();
                foreach (CustomPlugin item in _plugins)
                    pluginList.Add(item);
                WriteToXML(pluginList);
            }
            if (txtTitle.Text != null)
               ResetTab();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if (txtCommand.Text != "")
            {
                string iconDir = AppDomain.CurrentDomain.BaseDirectory + "\\icon\\" + txtTitle.Text + ".png";
                if (File.Exists(iconDir))
                    File.Delete(iconDir);
                Icon icon = GetFileIcoHelper.GetFileIcon(txtCommand.Text, false);
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\icon"))
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\icon");
                GetFileIcoHelper.SaveFileIcon(iconDir, icon);
            }
            if (_isExchanged)
            {
                List<CustomPlugin> pluginList = new List<CustomPlugin>();
                foreach (CustomPlugin item in _plugins)
                    pluginList.Add(item);
                WriteToXML(pluginList);
            }
            if(txtTitle.Text!=null)
                ResetTab();
            Close();
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex != -1)
            {
                foreach (CustomPlugin item in _plugins)
                {
                    if (item.Title == lstToolName.SelectedItem.ToString())
                    {
                        item.Title = txtTitle.Text;
                        fBindingSource.ResetBindings(true);
                        _isExchanged = true;
                        break;
                    }
                }
            }
        }

        private void txtCommand_TextChanged(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex != -1)
            {
                foreach (CustomPlugin item in _plugins)
                {
                    if (item.Title == lstToolName.SelectedItem.ToString())
                    {
                        item.Command = txtCommand.Text;
                        fBindingSource.ResetBindings(true);
                        _isExchanged = true;
                        break;
                    }
                }
            }
        }

        private void txtInitDir_TextChanged(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex != -1)
            {
                foreach (CustomPlugin item in _plugins)
                {
                 if (item.Title == lstToolName.SelectedItem.ToString())
                 {
                    item.InitDir = txtInitDir.Text;
                    fBindingSource.ResetBindings(true);
                    _isExchanged = true;
                    break;
                 }
                }
            }
        }

        private void txtParameter_TextChanged(object sender, EventArgs e)
        {
            if (lstToolName.SelectedIndex != -1)
            {
                foreach (CustomPlugin item in _plugins)
                {
                    if (item.Title == lstToolName.SelectedItem.ToString())
                    {
                        item.Parameter = txtParameter.Text;
                        fBindingSource.ResetBindings(true);
                        _isExchanged = true;
                        break;
                    }
                }
            }
        }

        private void cbxParameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbxParameter.SelectedIndex)
            {
                case 0: txtParameter.Text = "$(ItemPath)";
                        break;
                case 1: txtParameter.Text = "$(ItemDir)";
                        break;
                case 2: txtParameter.Text = "$(ItemFileName)";
                        break;
            }
        }

        private void cbxInitDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbxParameter.SelectedIndex)
            {
                case 0: txtInitDir.Text = "$(ItemDir)";
                    break;
                case 1: txtInitDir.Text = "$(TargetDir)";
                    break;
                case 2: txtInitDir.Text = "$(BinDir)";
                    break;
            }
        }

        private void WriteToXML(List<CustomPlugin> plugins) 
        {
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "DefaultUI.xml";
            if (_xmlDoc == null)
            {
                _xmlDoc = new XmlDocument();
                _xmlDoc.Load(filename);
            }
            XmlNode uiItemNode = _xmlDoc.SelectSingleNode("//UITab[@name='addins']//UIItems");
            if (uiItemNode == null)
                return;
            XmlNodeList childNodes = uiItemNode.SelectNodes("//UICommandGroup");
            if (childNodes != null)
            {
                foreach (XmlNode item in childNodes)
                {
                    XmlNode btnNode = item.SelectSingleNode("//UIButton");
                    if (btnNode == null)
                        continue;
                    XmlAttribute at = btnNode.Attributes["identify"];
                    if (at == null)
                        continue;
                    if (at.Value == "8200")
                        uiItemNode.RemoveChild(item);
                }
            }
            foreach (CustomPlugin plugin in plugins)
            {
                XmlElement xe = _xmlDoc.CreateElement("UICommandGroup");
                xe.SetAttribute("name", plugin.Title);
                xe.SetAttribute("text", plugin.Title);
                XmlElement child = _xmlDoc.CreateElement("UIButton");
                child.SetAttribute("name", plugin.Title);
                child.SetAttribute("text", plugin.Title);
                child.SetAttribute("image", "systemroot:"+"icon\\"+plugin.Title+".png");
                child.SetAttribute("identify", "8200");
                child.SetAttribute("argument", plugin.Command + ":" + plugin.Parameter);
                child.SetAttribute("imagealignment", "TopCenter");
                child.SetAttribute("textalignment", "BottomCenter");
                child.SetAttribute("initDir", plugin.InitDir);
                xe.AppendChild(child);
                uiItemNode.AppendChild(xe);
            }
            _xmlDoc.Save(filename);
        }

        private void ResetTab()
        {
            RadRibbonBarGroup rbgNew = new RadRibbonBarGroup();
            rbgNew.Text = txtTitle.Text;
            RadButtonElement btnUserAdd = new RadButtonElement(txtTitle.Text);
            btnUserAdd.Margin = new Padding(2, 2, 2, 2);
            btnUserAdd.TextAlignment = ContentAlignment.BottomCenter;
            btnUserAdd.ImageAlignment = ContentAlignment.TopCenter;
            string iconDir = AppDomain.CurrentDomain.BaseDirectory + "\\icon\\" + txtTitle.Text + ".png";
            if(File.Exists(iconDir))
               btnUserAdd.Image = Image.FromFile(iconDir);
            btnUserAdd.Click += new EventHandler(btnUserAdd_Click);
            rbgNew.Items.Add(btnUserAdd);
            _rbgroup.Parent.Children.Add(rbgNew);
        }

        private void btnUserAdd_Click(object sender, EventArgs e)
        {
            ICommand cmd = _smartSession.CommandEnvironment.Get(8200);
            if (cmd != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                string filename = System.AppDomain.CurrentDomain.BaseDirectory + "DefaultUI.xml";
                xmlDoc.Load(filename);
                XmlNodeList nodeList = _xmlDoc.SelectNodes("//UICommandGroup");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value == (sender as RadButtonElement).Text)
                    {
                        cmd.Execute(node.ChildNodes[0].Attributes["argument"].Value);
                        break;
                    }
                }  

            }
        }
    }
}
