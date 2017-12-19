using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Layout.GDIPlus;
using System.IO;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace GeoDo.RSS.Layout.Elements
{
    public partial class frmCustomTemplate : Form
    {
        private TreeNode _imageNode;
        private TreeNode _textNode;

        public frmCustomTemplate()
        {
            InitializeComponent();
            LoadCustomElement();
        }

        private void LoadCustomElement()
        {
            treeView1.Nodes.Clear();
            LoadImageNode();
            LoadTextNode();
            treeView1.ExpandAll();
        }

        private void LoadTextNode()
        {
            _textNode = new TreeNode("文本元素");
            _textNode.Checked = true;
            TextElement txtElement = new TextElement();
            txtElement.Name = "制作单位";
            txtElement.Text = "制作单位：中国气象局国家卫星气象中心";
            TreeNode txtNode = new TreeNode(txtElement.Name);
            txtNode.Tag = txtElement;
            txtNode.Checked = true;
            _textNode.Nodes.Add(txtNode);
            treeView1.Nodes.Add(_textNode);
        }

        private void LoadImageNode()
        {
            _imageNode = new TreeNode("图片元素");
            _imageNode.Checked = true;
            PictureElementGJWXQXZX picEle1 = new PictureElementGJWXQXZX();
            PictureElementZGQXJ picEls2 = new PictureElementZGQXJ();
            TreeNode node1 = new TreeNode(picEle1.Name);
            node1.Tag = picEle1;
            node1.Checked = true;
            TreeNode node2 = new TreeNode(picEls2.Name);
            node2.Tag = picEls2;
            node2.Checked = true;
            _imageNode.Nodes.Add(node1);
            _imageNode.Nodes.Add(node2);
            treeView1.Nodes.Add(_imageNode);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            groupBox2.Controls.Clear();
            TreeNode node = e.Node;
            if (node.Tag is IElement)
            {
                IElement element = node.Tag as IElement;
                if (element is TextElement)
                {
                    EditTextElement edit = new EditTextElement();
                    edit.Dock = DockStyle.Fill;
                    edit.ElementChanged += new Action<object, IElement>(edit_ElementChanged);
                    edit.SetElement(node.Tag as IElement);
                    edit.Tag = node;
                    groupBox2.Controls.Add(edit);
                }
                else if (element is PictureElement)
                {
                    EditPictureElement edit = new EditPictureElement();
                    edit.Dock = DockStyle.Fill;
                    edit.ElementChanged+=new Action<object,IElement>(edit_ElementChanged);
                    edit.SetElement(node.Tag as IElement);
                    edit.Tag = node;
                    groupBox2.Controls.Add(edit);
                }               
            }

        }

        List<IElement> editEles = new List<IElement>();

        private void GetEditElements()
        {
            editEles.Clear();
            foreach (TreeNode node in treeView1.Nodes)
            {
                foreach (TreeNode eleNode in node.Nodes)
                {
                    if (eleNode.Checked && eleNode.Tag is IElement)
                    {
                        editEles.Add(eleNode.Tag as IElement);
                    }
                }
            }
        }

        void edit_ElementChanged(object sender, IElement e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (e is TextElement)
            {
                e.Name = (node.Tag as IElement).Name;
                node.Tag = (e as TextElement);
            }
            else if (e is PictureElement)
            {
                (e as IElement).Name = (node.Tag as IElement).Name;
                node.Tag = e;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetEditElements();
            if (editEles.Count == 0)
                return;
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (Directory.Exists(path))
                EditTemplate(path, editEles.ToArray());
            MessageBox.Show("完成模版更新。", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void EditTemplate(string path,IElement[] elements)
        {
            string[] fnames = Directory.GetFiles(path, "*.gxt", SearchOption.AllDirectories);
            if (fnames == null || fnames.Length == 0)
                return;
            foreach (string fname in fnames)
            {
                XElement element = XElement.Load(fname);
                if (element != null)
                {
                    foreach (IElement ele in elements)
                    {
                        if(ele is TextElement)
                            ReplaceElement(element, ele as TextElement);
                        else if (ele is PictureElement)
                            ReplaceElement(element, ele as PictureElement);
                    }
                    element.Save(fname);
                }
            }
        }

        //中国气象局图标,国家卫星气象中心图标
        private void ReplaceElement(XElement element, PictureElement pictureElement)
        {
            string name = pictureElement.Name;
            Bitmap bitmap = pictureElement.Bitmap;
            var eles = element.Elements("PictureElementZGQXJ").Where((ele) => { return ele.Attribute("name") != null && ele.Attribute("name").Value == name; });
            if (eles != null && eles.Count() != 0)
            {
                foreach (XElement subEle in eles)
                {
                    subEle.Attribute("bitmap").Value = GetBinaryString(pictureElement.GetType().InvokeMember("Bitmap", BindingFlags.GetProperty, null, pictureElement, null)); ;
                }
            }
            else
            {
                eles = element.Elements("PictureElementGJWXQXZX").Where((ele) => { return ele.Attribute("name") != null && ele.Attribute("name").Value == name; });
                if (eles != null && eles.Count() != 0)
                {
                    foreach (XElement subEle in eles)
                    {
                        subEle.Attribute("bitmap").Value = GetBinaryString(pictureElement.GetType().InvokeMember("Bitmap", BindingFlags.GetProperty, null, pictureElement, null)); ;
                    }
                }
            }
        }

        //对象到二进制流，再转为64位字符串
        private static string GetBinaryString(object value)
        {
            string result = null;
            using (Stream st = new MemoryStream())
            {
                result = SerializeObj(value, st);
            }
            return result;
        }

        private static string SerializeObj(object value, Stream st)
        {
            IFormatter formatter = (IFormatter)new BinaryFormatter();
            formatter.Serialize(st, value);
            st.Position = 0;
            st.Flush();
            using (BinaryReader br = new BinaryReader(st))
            {
                byte[] cache = br.ReadBytes((int)st.Length);
                return Convert.ToBase64String(cache, 0, cache.Length);
            }
        }

        private void ReplaceElement(XElement element, TextElement txtElement)
        {
            ReplaceText(element, txtElement.Name, txtElement.Text);
        }

        private void ReplaceText(XElement element,string name,string value)
        {
           var txtEle = element.Elements("TextElement").Where((ele) => { return ele.Attribute("name") != null && ele.Attribute("name").Value == name; });
           if (txtEle != null && txtEle.Count() != 0)
           {
               foreach (XElement subEle in txtEle)
               {
                   subEle.Attribute("text").Value = value;
               }
           }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
