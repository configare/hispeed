using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.CA
{
    public partial class DblBarTrackWithBoxs : UserControl
    {
        public delegate void BarValueChangedHandler(object sender, double minValue, double maxValue);
        public delegate void BarValueChangedFinishedHandler(object sender, double minValue, double maxValue);
        public delegate void BarValueBoxDoubleClickedHandler(object sender, TextBox textbox,bool isMinValue);
        public event BarValueChangedHandler EndValueChanged = null;
        public event BarValueChangedFinishedHandler BarValueChangedFinished = null;
        public event EventHandler DoubleClickTrackLine = null;
        public event BarValueBoxDoubleClickedHandler BarValueDoubleClicked = null;
        [NonSerialized]
        private List<EndpointValueItem> _endpointValueItems = new List<EndpointValueItem>();

        public DblBarTrackWithBoxs()
        {
            InitializeComponent();
            Resize += new EventHandler(DblBarTrackWithBoxs_Resize);
            txtMaxValue.TextChanged += new EventHandler(txtEndValue_TextChanged);
            txtMinValue.TextChanged += new EventHandler(txtEndValue_TextChanged);
            txtMinValue.KeyDown += new KeyEventHandler(txtEndValue_KeyDown);
            txtMaxValue.KeyDown += new KeyEventHandler(txtEndValue_KeyDown);
            multiBarTrack1.BarValueChanged += new MultiBarTrack.BarValueChangedHandler(multiBarTrack1_BarValueChanged);
            multiBarTrack1.BarValueChangedFinished += new MultiBarTrack.BarValueChangedFinishedHandler(multiBarTrack1_BarValueChangedFinished);
            Load += new EventHandler(DblBarTrackWithBoxs_Load);
            txtMinValue.Tag = 0;
            txtMaxValue.Tag = 1;
        }

        [Browsable(false)]
        public List<EndpointValueItem> EndpointValueItems
        {
            get { return _endpointValueItems; }
        }

        void multiBarTrack1_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            if (BarValueChangedFinished != null)
            {
                double minV = 0, maxV = 0;
                if (barIndex == 0)
                {
                    minV = value;
                    maxV = MaxValue;
                }
                else
                {
                    minV = MinValue;
                    maxV = value;
                }
                BarValueChangedFinished(this, minV, maxV);
            }
        }
        private bool NotRaiseDoubleClickEvent = false;
        void txtEndValue_KeyDown(object sender, KeyEventArgs e)
        {
            NotRaiseDoubleClickEvent = true;
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TextBox txtBox = sender as TextBox;
                    int idx = (int)txtBox.Tag;
                    double newValue = multiBarTrack1.SetValueAt(idx, GetEndValue(txtBox));
                    txtBox.Text = newValue.ToString("0.###");
                }
            }
            finally 
            {
                NotRaiseDoubleClickEvent = false;
            }
       }

        private double GetEndValue(TextBox txtBox)
        {
            string strV = txtBox.Text.ToString();
            if (strV == "-" || strV == "0." || strV == "-0." || strV == string.Empty)
                return 0;
            double v = 0;
            if (!double.TryParse(txtBox.Text, out v))
                return 0;
            return v;
        }

        void DblBarTrackWithBoxs_Load(object sender, EventArgs e)
        {
            double v = multiBarTrack1.GetValueAt(0);
            txtMinValue.Text = v.ToString("0.###");
            v = multiBarTrack1.GetValueAt(1);
            txtMaxValue.Text = v.ToString("0.###");
            //
            Form frm = FindForm();
            if (frm != null)
            {
                frm.KeyPreview = true;
                frm.KeyDown += new KeyEventHandler(frm_KeyDown);
            }
        }

        void frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (multiBarTrack1.IsOverLeftBar)
            {
                if (e.KeyCode == Keys.Left)
                {
                    MinValue -= 1;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    MinValue += 1;
                    e.Handled = true;
                }
            }
            else if (multiBarTrack1.IsOverRightBar)
            {
                if (e.KeyCode == Keys.Left)
                {
                    MaxValue -= 1;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    MaxValue += 1;
                    e.Handled = true;
                }
            }
        }

        void multiBarTrack1_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            switch (barIndex)
            { 
                case 0:
                    txtMinValue.Text = value.ToString("0.###");
                    break;
                case 1:
                    txtMaxValue.Text = value.ToString("0.###");
                    break;
                default:
                    return;
            }
            if (EndValueChanged != null)
            {
                double minValue = 0,maxValue =0;
                if (barIndex == 0)
                {
                    minValue = value;
                    maxValue = GetEndValue(txtMaxValue);
                }
                else 
                {
                    maxValue = value;
                    minValue = GetEndValue(txtMinValue);
                }
                EndValueChanged(this, minValue, maxValue);
            }
        }

        void txtEndValue_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            string strV = txtBox.Text.ToString();
            if (strV == "-" || strV == "0." || strV == "-0." || strV == string.Empty)
                return;
            double v = 0;
            if (!double.TryParse(txtBox.Text, out v))
                txtBox.Text = "0";
        }

        void DblBarTrackWithBoxs_Resize(object sender, EventArgs e)
        {
            int top = Height / 2 - txtMaxValue.Height / 2;
            txtMaxValue.Top = txtMinValue.Top = pictureBox1.Top = top;
            AdjustCaptionWidht();
        }

        private void AdjustCaptionWidht()
        {
            using (Graphics g = this.CreateGraphics())
            {
                SizeF size = g.MeasureString(txtCaption.Text, Font);
                panelLeft.Width = (int)Math.Ceiling(size.Width);
                txtCaption.Width = panelLeft.Width;
                panelLeft.Left = 0;
                txtCaption.Left = 0;
                txtCaption.Top = Height / 2 - (int)size.Height / 2 + 1;
            } 
        }

        [Category("自定义属性")]
        public Color TrackLineFillColor
        {
            get { return multiBarTrack1.TrackLineColor; }
            set { multiBarTrack1.TrackLineColor = value; }
        }

        [Category("自定义属性")]
        public string Caption
        {
            get { return txtCaption.Text; }
            set
            {
                if(txtCaption.Text != value)
                {
                    txtCaption.Text = value;
                    AdjustCaptionWidht();
                }
            }
        }
        private double _minValue;
        private double _maxValue;
        [Browsable(false)]
        public double MinValue
        {
            get { return _minValue = GetEndValue(txtMinValue); }
            set 
            {
                //edit by luoke 设置的最小值大于当前最大值时，自动变更最大值。
                if (value > _maxValue)
                    _maxValue = value;
                txtMinValue.Text = value.ToString();
                txtEndValue_KeyDown(txtMinValue, new KeyEventArgs(Keys.Enter));
                _minValue = value;
            }
        }

        [Browsable(false)]
        public double MaxValue
        {
            get { return _maxValue = GetEndValue(txtMaxValue); }
            set
            {
                //edit by luoke 设置的最大值小于当前最小值时，自动变更最小值。
                if (value < _minValue)
                    _minValue = value;
                txtMaxValue.Text = value.ToString();
                txtEndValue_KeyDown(txtMaxValue, new KeyEventArgs(Keys.Enter));
                _maxValue = value;
            }
        }

        [Browsable(true)]
        public int MaxLengthMinValue
        {
            get
            {
                return txtMinValue.MaxLength;
            }
            set
            {
                txtMinValue.MaxLength = value;
            }
        }

        [Browsable(true)]
        public int MaxLengthMaxValue
        {
            get
            {
                return txtMaxValue.MaxLength;
            }
            set
            {
                txtMaxValue.MaxLength = value;
            }
        }


        //[Category("自定义属性"),Description("两个滑块之间允许的最小距离，单位:像素")]
        public int MinSpan
        {
            get { return multiBarTrack1.MinSpan; }
            //by chennan 2011-6-9 修改设定值变化问题
            //set { multiBarTrack1.MinSpan = value; }
            set { multiBarTrack1.MinSpan = 0; }
        }

        [Category("自定义属性")]
        public double MaxEndPointValue
        {
            get { return multiBarTrack1.MaxEndPointValue; }
            set 
            {
                multiBarTrack1.MaxEndPointValue = value;
                multiBarTrack1.SetValueAt(0, MinValue);
                multiBarTrack1.SetValueAt(1, MaxValue);
            }
        }

        [Category("自定义属性")]
        public double MinEndPointValue
        {
            get { return multiBarTrack1.MinEndPointValue; }
            set 
            {
                multiBarTrack1.MinEndPointValue = value;
                multiBarTrack1.SetValueAt(0, MinValue);
                multiBarTrack1.SetValueAt(1, MaxValue);
            }
        }

        private void multiBarTrack1_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClickTrackLine != null)
                DoubleClickTrackLine(this, e);
        }

        private void panelLeft_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClickTrackLine != null)
                DoubleClickTrackLine(this, e);
        }

        private void txtMinValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (BarValueDoubleClicked != null)
                BarValueDoubleClicked(this, txtMinValue,true);
        }

        private void txtMaxValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (BarValueDoubleClicked != null)
                BarValueDoubleClicked(this, txtMaxValue,false);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            ToolStripMenuItem it = new ToolStripMenuItem("保存当前阈值");
            it.Click += new EventHandler(SaveCurtValueMenuItem_Click);
            contextMenuStrip1.Items.Add(it);
            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            if (_endpointValueItems != null && _endpointValueItems.Count > 0)
            {
                foreach (EndpointValueItem evit in _endpointValueItems)
                {
                    it = new ToolStripMenuItem(evit.Name +" " + evit.MinValue.ToString() + "," +evit.MaxValue.ToString());
                    it.Tag = evit;
                    it.Click += new EventHandler(it_Click);
                    contextMenuStrip1.Items.Add(it);
                }
                contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }
            //
            it = new ToolStripMenuItem("管理已保存阈值");
            it.Click += new EventHandler(MamageSavedItems_Click);
            contextMenuStrip1.Items.Add(it);
            //
            contextMenuStrip1.Show(pictureBox1, label1.PointToClient(Control.MousePosition));
        }

        void it_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem it = sender as ToolStripMenuItem;
            EndpointValueItem v = it.Tag as EndpointValueItem;
            MinValue = v.MinValue;
            MaxValue = v.MaxValue;
        }

        private void SaveCurtValueMenuItem_Click(object sender, EventArgs e)
        {
            string name = GetNameFromDialog();
            if (name == string.Empty)
                name = GetOnlyName();
            EndpointValueItem it = new EndpointValueItem(MinValue, MaxValue, name);
            _endpointValueItems.Add(it);
        }

        private void MamageSavedItems_Click(object sender, EventArgs e)
        {
            if (_endpointValueItems == null || _endpointValueItems.Count == 0)
                return;
            frmEndpointValueManager mgr = new frmEndpointValueManager(_endpointValueItems);
            mgr.Location = Control.MousePosition;
            if (mgr.Location.X + mgr.Width > Screen.GetBounds(this).Width)
                mgr.Location = new Point(Screen.GetBounds(this).Width - mgr.Width, Control.MousePosition.Y);
            mgr.ShowDialog();
        }
        

        private string GetNameFromDialog()
        {
            Form frm = new Form();
            frm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frm.StartPosition = FormStartPosition.Manual;
            frm.Text = "命名当前阈值...";
            Label l = new Label();
            l.Left = 10;
            l.Top = 14;
            l.Width = 32;
            l.Text = "名称";
            frm.Controls.Add(l);
            TextBox txt = new TextBox();
            txt.Left = l.Left + l.Width + 10;
            txt.Width = 120;
            txt.Top = 10;
            txt.Text = GetOnlyName();
            Button btn = new Button();
            btn.Text = "确定";
            btn.Left = txt.Left + txt.Width + 6;
            btn.Top = 8;
            btn.Width = 60;
            frm.Controls.Add(btn);
            frm.Controls.Add(txt);
            frm.Width = 48 + l.Width + txt.Width + btn.Width;
            btn.Click += new EventHandler(btn_Click);
            btn.Tag = frm;
            frm.Height = txt.Height + 44;
            frm.Location = Control.MousePosition;
            if (frm.Location.X + frm.Width > Screen.GetBounds(this).Width)
                frm.Location = new Point(Screen.GetBounds(this).Width - frm.Width, Control.MousePosition.Y);
            frm.ShowDialog();
            return txt.Text;
        }

        private string GetOnlyName()
        {
            string exp = @"\S+\((?<idx>\d+)\)$";
            int idx = 0;
            foreach (EndpointValueItem it in _endpointValueItems)
            {
                Match m = Regex.Match(it.Name, exp);
                if (m.Success)
                { 
                    int i = int.Parse(m.Groups["idx"].Value);
                    if (i > idx)
                        idx = i;
                }
            }
            idx++;
            return "阈值(" + idx.ToString() + ")";
        }

        void btn_Click(object sender, EventArgs e)
        {
            ((sender as Button).Tag as Form).Close();
        }

         private void multiBarTrack1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Items.Clear();
                ToolStripMenuItem mnuSetEndpointValue = new ToolStripMenuItem("设置端值");
                mnuSetEndpointValue.Click += new EventHandler(mnuSetEndpointValue_Click);
                contextMenuStrip1.Items.Add(mnuSetEndpointValue);
                contextMenuStrip1.Show(this, e.Location);
            }
        }

         void mnuSetEndpointValue_Click(object sender, EventArgs e)
         {
             frmSetEndpointValue frm = new frmSetEndpointValue(multiBarTrack1.MinEndPointValue,multiBarTrack1.MaxEndPointValue);
             frm.Location = Control.MousePosition;
             if (frm.Location.X + frm.Width > Screen.GetBounds(this).Width)
                 frm.Location = new Point(Screen.GetBounds(this).Width - frm.Width, Control.MousePosition.Y);
             frm.ShowDialog();
             //frm.TopMost = true;
             multiBarTrack1.MinEndPointValue = frm.MinValue;
             multiBarTrack1.MaxEndPointValue = frm.MaxValue;
             txtEndValue_KeyDown(txtMinValue, new KeyEventArgs(Keys.Enter));
             txtEndValue_KeyDown(txtMaxValue, new KeyEventArgs(Keys.Enter));
         }

         private void label1_Click(object sender, EventArgs e)
         {

         }

         private void label1_DoubleClick(object sender, EventArgs e)
         {
             pictureBox1_MouseDown(null, null);
         }
    }
}
