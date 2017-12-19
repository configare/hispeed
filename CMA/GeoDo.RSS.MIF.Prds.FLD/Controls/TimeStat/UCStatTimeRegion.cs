using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCStatTimeRegion : UserControl
    {
        public TimeRegionClass TimeRegion = null;
        public delegate void ArgChangedDelegate();
        public ArgChangedDelegate ArgChanged = null;

        public UCStatTimeRegion()
        {
            InitializeComponent();
            ClearBeginRegion();
            ClearEndRegion();
            TimeRegion = new TimeRegionClass();
        }


        private void ClearBeginRegion()
        {
            cbMonthBegin.SelectedIndex = 0;
            cbDayBegin.SelectedIndex = 0;
            TimeRegion.BeginMonth = -1;
            TimeRegion.BeginDay = -1;
        }

        private void ClearEndRegion()
        {
            cbMonthEnd.SelectedIndex = 0;
            cbDayEnd.SelectedIndex = 0;
            TimeRegion.EndMonth = -1;
            TimeRegion.EndDay = -1;
        }

        private void cbMonthBegin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeRegion == null)
                TimeRegion = new TimeRegionClass();
            TimeRegion.BeginMonth = cbMonthBegin.Text == "不区分" ? -1 : int.Parse(cbMonthBegin.Text.Replace("月", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbDayBegin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeRegion == null)
                TimeRegion = new TimeRegionClass();
            TimeRegion.BeginDay = cbDayBegin.Text == "不区分" ? -1 : int.Parse(cbDayBegin.Text.Replace("日", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbMonthEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeRegion == null)
                TimeRegion = new TimeRegionClass();
            TimeRegion.EndMonth = cbMonthEnd.Text == "不区分" ? -1 : int.Parse(cbMonthEnd.Text.Replace("月", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbDayEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeRegion == null)
                TimeRegion = new TimeRegionClass();
            TimeRegion.EndDay = cbDayEnd.Text == "不区分" ? -1 : int.Parse(cbDayEnd.Text.Replace("日", ""));
            if (ArgChanged != null)
                ArgChanged();
        }
    }
}
