using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geodo.RSS.MIF.UI
{
    public partial class frmTextStatResultWnd : Form, IStatResultWindow
    {

        public frmTextStatResultWnd()
        {
            InitializeComponent();
        }

        #region IOperationWnd Members

        public void Display()
        {
            Show();
        }

        #endregion

        #region IStatResultWindow Members

        public void Add(bool singleFile, string windowText, IStatResult result, bool isTotal, int statImage)
        {
            if (!string.IsNullOrEmpty(windowText))
                Text = windowText;
            richTextBox1.Text = string.Empty;
            Append(singleFile, result, isTotal, statImage);
        }

        public void Append(bool singleFile, IStatResult result, bool isTotal, int statImage)
        {
            if (result == null)
                return;
            StringBuilder sb = new StringBuilder();
            if (richTextBox1.Text != string.Empty)
                sb.AppendLine(string.Empty);
            sb.AppendLine(result.Title.PadLeft(50));
            //
            if (result.Columns != null && result.Columns.Length > 0)
            {
                string colLineString = null;
                foreach (string colname in result.Columns)
                    colLineString += colname.PadRight(20);
                sb.AppendLine(colLineString);
                sb.AppendLine("-".PadRight(24 * result.Columns.Length, '-'));
            }
            //
            if (result.Rows != null && result.Rows.Length > 0)
            {
                foreach (string[] row in result.Rows)
                {
                    string rowLineString = null;
                    foreach (string v in row)
                        if (v == null)
                            rowLineString += string.Empty.PadRight(30);
                        else
                            rowLineString += v.PadRight(30);
                    sb.AppendLine(rowLineString);
                }
            }
            //
            richTextBox1.Text += sb.ToString();
            sb = null;
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int dislayCol, bool isTotal, int statImage)
        {
            Add(singleFile, windowText, result, isTotal, statImage);
        }

        public void Add(bool singleFile, string windowText, IStatResult result, int displayCol, bool displayDataLabel, bool isTotal, int statImage)
        {
            Add(singleFile, windowText, result, isTotal, statImage);
        }

        public void Add(bool singleFile, string windowText, IStatResult result, bool displayDataLabel, bool isTotal, int statImage)
        {
            Add(singleFile, windowText, result, isTotal, statImage);
        }

        #endregion
    }
}
