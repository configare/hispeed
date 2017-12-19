using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdScale:BaseControlItem
    {
        protected const string cstExp = @"^(?<percent>\d+)%";
        protected Regex _regex = new Regex(cstExp, RegexOptions.Compiled);
        protected ToolStripComboBox _box = null;
        private bool _isAttached = false;

        public cmdScale()
        {
            Init();
        }

        public cmdScale(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _box = new ToolStripComboBox();
            _box.Width = 120;
            _control = _box;
            //
            _box.Items.Add("40%");
            _box.Items.Add("50%");
            _box.Items.Add("60%");
            _box.Items.Add("70%");
            _box.Items.Add("75%");
            _box.Items.Add("80%");
            _box.Items.Add("90%");
            _box.Items.Add("100%");
            _box.Items.Add("200%");
            _box.Items.Add("400%");
            //
            _box.SelectedIndex = 7;
            //
            _box.KeyDown += new KeyEventHandler(box_KeyDown);
            _box.SelectedIndexChanged += new EventHandler(box_SelectedIndexChanged);
        }

        void box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_box.Text.Trim() != string.Empty)
            {
                if (!_box.Text.EndsWith("%"))
                    _box.Text += "%";
            }
            Match m = _regex.Match(_box.Text);
            if (m.Success)
            {
                string percentString = m.Groups["percent"].Value;
                double percent = 0;
                if (double.TryParse(percentString, out percent))
                {
                    (_hook as IHookOfModelEditor).ModelEditor.Scale = (float)percent / 100f;
                    (_hook as IHookOfModelEditor).ModelEditor.Render();
                }
            }
            else 
            {
                _box.SelectionStart = 0;
                _box.SelectionLength = _box.Text.Length;
            }
       }

        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                box_SelectedIndexChanged(null, null);
        }

        public override bool Visible
        {
            get
            {
                if (!_isAttached)
                {
                    (_hook as IHookOfModelEditor).ModelEditor.OnScaleChanged += new OnScaleChangedHandler(ScaleChanged);
                    _isAttached = true;
                }
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        private void ScaleChanged(object sender, float scale)
        {
            float f = scale * 100;
            _box.Text = f.ToString("0.##") + "%"; 
        }
    }
}
