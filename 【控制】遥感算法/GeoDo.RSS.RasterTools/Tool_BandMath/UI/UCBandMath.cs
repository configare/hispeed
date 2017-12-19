using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace GeoDo.RSS.RasterTools
{
    public partial class UCBandMath : UserControl
    {
        private static Regex _bandRegex = new Regex(@"(?<BandVar>b(?<BandNo>\d{1,4}))", RegexOptions.Compiled);
        public event EventHandler ApplyClicked = null;
        public event EventHandler CancelClicked = null;
        public const string SAVE_FILE_NAME = @"SystemData\BandMathExpressions.txt";

        public UCBandMath()
        {
            InitializeComponent();
            Load += new EventHandler(UCBandMath_Load);
            Disposed += new EventHandler(UCBandMath_Disposed); 
        }

        void UCBandMath_Disposed(object sender, EventArgs e)
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + SAVE_FILE_NAME;
            List<string> expressions = new List<string>();
            foreach (string exp in lvExpressions.Items)
                expressions.Add(exp);
            if (expressions.Count > 0)
                File.WriteAllLines(fname, expressions.ToArray());
        }

        void UCBandMath_Load(object sender, EventArgs e)
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + SAVE_FILE_NAME;
            try
            {
                if (File.Exists(fname))
                {
                    string[] exps = File.ReadAllLines(fname, Encoding.Default);
                    if (exps != null && exps.Length > 0)
                        foreach (string exp in exps)
                            lvExpressions.Items.Add(exp);
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
             }
        }

        public void SetExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return;
            if (lvExpressions.Items.Contains(expression))
                lvExpressions.SelectedItem = expression;
            else
                if (ExpressIsOK(expression))
                    lvExpressions.Items.Add(expression);
        }

        private void btnAdd2List_Click(object sender, EventArgs e)
        {
            string expression = txtExpression.Text;
            if (lvExpressions.Items.Contains(expression))
            {
                MessageBox.Show("列表中已存在表达式" + txtExpression.Text + "！");
                return;
            }
            if (ExpressIsOK(txtExpression.Text))
                lvExpressions.Items.Add(txtExpression.Text);
            else
            {
                txtExpression.Focus();
                txtExpression.SelectAll();
            }
        }

        private bool ExpressIsOK(string exp)
        {
            if (string.IsNullOrWhiteSpace(exp))
                return false;
            Match m = _bandRegex.Match(exp);
            return m.Success;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvExpressions.SelectedIndices.Count == 0)
                return;
            for (int i = 0; i < lvExpressions.SelectedItems.Count; i++)
                lvExpressions.Items.Remove(lvExpressions.SelectedItems[i]);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvExpressions.Items.Clear();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (lvExpressions.SelectedItem == null)
                return;
            if (ApplyClicked != null)
                ApplyClicked(lvExpressions.SelectedItem, null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(null, null);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int index=lvExpressions.SelectedIndex;
            if ( index>= 0)
            {
                string expression = txtExpression.Text;
                if (string.IsNullOrEmpty(expression))
                    return;
                lvExpressions.Items.RemoveAt(index);
                lvExpressions.Items.Insert(index, expression);
            }
        }

        private void lvExpressions_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lvExpressions.SelectedIndex;
            if (index >= 0)
                txtExpression.Text = lvExpressions.Items[index].ToString();
        }
    }
}
