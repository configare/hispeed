using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.UI
{
    public partial class MultiFileSelect : UserControl
    {
        public event Action<object> FileChanged;
        
        public MultiFileSelect()
        {
            InitializeComponent();
        }

        public string Title
        {
            set { label1.Text = value; }
        }

        public string[] Files
        {
            get
            {
                List<string> fs = new List<string>();
                foreach (FileItem f in listBox1.Items)
                {
                    fs.Add(f.FileName);
                }
                return fs.Count == 0 ? null : fs.ToArray();
            }
            set
            {
                listBox1.Items.Clear();
                AddFiles(value);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Multiselect = true;
            f.Filter = FileFilter;
            if (f.ShowDialog() == DialogResult.OK)
            {
                AddFiles(f.FileNames);
            }
        }

        private void AddFiles(string[] value)
        {
            if (value != null && value.Length != 0)
            {
                foreach (string f in value)
                {
                    if (!Exist(f))
                        listBox1.Items.Add(new FileItem(f));
                }
            }
            if (FileChanged != null)
                FileChanged(this);
        }

        private bool Exist(string filename)
        {
            if (listBox1.Items == null || listBox1.Items.Count == 0)
                return false;
            foreach (FileItem item in listBox1.Items)
            {
                if (item.FileName == filename)
                    return true;
            }
            return false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                List<FileItem> items = new List<FileItem>();
                foreach (FileItem item in listBox1.SelectedItems)
                    items.Add(item);
                foreach (FileItem item in items)
                    listBox1.Items.Remove(item);
            }
            if (FileChanged != null)
                FileChanged(this);
        }

        public string FileFilter { get; set; }
    }

    internal class FileItem
    {
        public FileItem(string filename)
        {
            FileName = filename; 
        }

        public string FileName;

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return base.ToString();
            else
                return Path.GetFileName(FileName);
        }
    }
}
