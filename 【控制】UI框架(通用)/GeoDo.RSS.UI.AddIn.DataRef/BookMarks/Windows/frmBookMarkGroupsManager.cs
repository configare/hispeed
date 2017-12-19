using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public partial class frmBookMarkGroupsManager : Form
    {
        private Dictionary<string, CoordEnvelope> _bookMarks = null;
        private CoordEnvelope _evp = null;
        private BookMarkParser _parser = null;
        private CoordEnvelope _applyEvp = null;
        private bool _isCanvasNull = true;

        public frmBookMarkGroupsManager()
        {
            InitializeComponent();
            InitRows();
        }

        public frmBookMarkGroupsManager(bool isCanvasNull)
        {
            InitializeComponent();
            InitArgs(isCanvasNull);
        }

        public frmBookMarkGroupsManager(bool isCanvasNull,CoordEnvelope envelop)
        {
            InitializeComponent();           
            _evp = envelop;
            InitArgs(isCanvasNull);
        }

        private void InitArgs(bool isCanvasNull)
        {
            _isCanvasNull = isCanvasNull;
            if (_isCanvasNull)
                btnLocate.Enabled = false;
            InitRows();
        }

        public CoordEnvelope ApplyEnvelope
        {
            get { return _applyEvp; }
        }

        private void InitRows()
        {
            _parser = new BookMarkParser();
            _bookMarks = _parser.BookMarks;
            if (_bookMarks == null || _bookMarks.Count == 0)
            {
                dgvBmList.Rows.Clear();
                return;
            }
            dgvBmList.RowCount = _bookMarks.Keys.Count;
            string[] keys = _bookMarks.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                dgvBmList.Rows[i].SetValues(keys[i]);
                dgvBmList.Rows[i].Tag = keys[i];
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreat_Click(object sender, EventArgs e)
        {            
            frmCreatBookMarkGroup creat = new frmCreatBookMarkGroup(_evp);
            if (creat.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                InitRows();
                dgvBmList.Invalidate();
            }
        }

        private void btnLocate_Click(object sender, EventArgs e)
        {
            string bookMarkName = dgvBmList.CurrentCell.Value.ToString();
            if (string.IsNullOrEmpty(bookMarkName))
                return;
            Dictionary<string, CoordEnvelope> bookmarks = _parser.BookMarks;
            if (bookmarks == null || bookmarks.Count == 0)
                return;
            foreach (string key in bookmarks.Keys)
            {
                if (key == bookMarkName)
                {
                    _applyEvp = bookmarks[key];
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    return;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_parser == null)
                return;
            string bookMarkName = dgvBmList.CurrentCell.Value.ToString();
            if (string.IsNullOrEmpty(bookMarkName))
                return;
            _parser.DeleteBookMarkElement(bookMarkName);
            InitRows();
            dgvBmList.Invalidate();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (_parser == null)
                return;
            _parser.DeleteAllBookMarkElements();
            InitRows();
            dgvBmList.Invalidate();
        }
    }
}
