using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class DataFileterControl : Control
    {
        public event Action<HdDataFilter> CheckedFilterChanged;

        private System.Windows.Forms.FlowLayoutPanel pnlSatellite;
        private HdDataFilter _checkedFilter;
        private List<RadioButton> btns = new List<RadioButton>();


        public DataFileterControl()
        {
            pnlSatellite = new FlowLayoutPanel();
            pnlSatellite.Dock = DockStyle.Fill;
            this.Controls.Add(pnlSatellite);

            HdDataFilter[] filters = HdDataFilter.FilterColl();
            for (int i = 0; i < filters.Length; i++)
            {
                System.Windows.Forms.RadioButton btnFy3avirr = new System.Windows.Forms.RadioButton();
                btnFy3avirr.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                btnFy3avirr.Location = new System.Drawing.Point(3, 3);
                btnFy3avirr.Name = "btnFy3avirr";
                btnFy3avirr.Size = new System.Drawing.Size(147, 33);
                btnFy3avirr.TabIndex = 25;
                btnFy3avirr.TabStop = true;
                //btnFy3avirr.Text = "FY-3A VIRR";
                btnFy3avirr.Text = filters[i].Text;
                btnFy3avirr.Tag = filters[i];
                btnFy3avirr.UseVisualStyleBackColor = true;
                if (_checkedFilter == null)
                {
                    btnFy3avirr.Checked = true;
                    _checkedFilter = filters[i];
                }
                btnFy3avirr.CheckedChanged += new EventHandler(btnFy3avirr_CheckedChanged);
                btns.Add(btnFy3avirr);
                this.pnlSatellite.Controls.Add(btnFy3avirr);
            }
        }

        public RadioButton[] Items
        {
            get { return btns.ToArray(); }
        }

        void btnFy3avirr_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                _checkedFilter = (sender as RadioButton).Tag as HdDataFilter;
                if (CheckedFilterChanged != null)
                    CheckedFilterChanged(CheckedFilter);
            }
        }

        public HdDataFilter CheckedFilter
        {
            get { return _checkedFilter; }
            private set { _checkedFilter = value; }
        }
    }
}
