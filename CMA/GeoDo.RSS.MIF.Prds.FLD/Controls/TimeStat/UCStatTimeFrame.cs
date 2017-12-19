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
    public partial class UCStatTimeFrame : UserControl
    {
        public TimeFrameClass TimeFrame = null;
        public delegate void ArgChangedDelegate();
        public ArgChangedDelegate ArgChanged = null;

        public UCStatTimeFrame()
        {
            InitializeComponent();
            ClearMonthSelect();
            ClearDaySelect();
            Init();
            TimeFrame = new TimeFrameClass();
        }

        private void Init()
        {
            cbSeason.SelectedIndex = 0;
            cb10Days.SelectedIndex = 0;
        }

        private void ClearMonthSelect()
        {
            cbMonthBegin.SelectedIndex = 0;
            cbMonthEnd.SelectedIndex = 0;
            TimeFrame.BeginMonth = -1;
            TimeFrame.EndMonth = -1;
        }

        private void ClearDaySelect()
        {
            cbDayBegin.SelectedIndex = 0;
            cbDayEnd.SelectedIndex = 0;
            TimeFrame.BeginDay = -1;
            TimeFrame.EndDay = -1;
        }

        private void SetEnableMonthSelect(bool isEnable)
        {
            cbMonthBegin.Enabled = isEnable;
            cbMonthEnd.Enabled = isEnable;
        }

        private void SetEnableDaySelect(bool isEnable)
        {
            cbDayBegin.Enabled = isEnable;
            cbDayEnd.Enabled = isEnable;
        }

        private void cbSeason_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEnableMonthSelect(false);
            switch (cbSeason.Text)
            {
                case "不分":
                    ClearMonthSelect();
                    SetEnableMonthSelect(true);
                    break;
                case "一季度":
                    cbMonthBegin.Text = "01月";
                    cbMonthEnd.Text = "03月";
                    break;
                case "二季度":
                    cbMonthBegin.Text = "04月";
                    cbMonthEnd.Text = "06月";
                    break;
                case "三季度":
                    cbMonthBegin.Text = "07月";
                    cbMonthEnd.Text = "09月";
                    break;
                case "四季度":
                    cbMonthBegin.Text = "10月";
                    cbMonthEnd.Text = "12月";
                    break;
                default:
                    break;
            }
        }

        private void cb10Days_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEnableDaySelect(false);
            switch (cb10Days.Text)
            {
                case "不分":
                    ClearDaySelect();
                    SetEnableDaySelect(true);
                    break;
                case "上旬":
                    cbDayBegin.Text = "01日";
                    cbDayEnd.Text = "10日";
                    break;
                case "中旬":
                    cbDayBegin.Text = "11日";
                    cbDayEnd.Text = "20日";
                    break;
                case "下旬":
                    cbDayBegin.Text = "21日";
                    cbDayEnd.Text = "31日";
                    break;
                default:
                    break;
            }
        }

        private void cbMonthBegin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeFrame == null)
                TimeFrame = new TimeFrameClass();
            TimeFrame.BeginMonth = cbMonthBegin.Text == "不区分" ? -1 : int.Parse(cbMonthBegin.Text.Replace("月", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbMonthEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeFrame == null)
                TimeFrame = new TimeFrameClass();
            TimeFrame.EndMonth = cbMonthEnd.Text == "不区分" ? -1 : int.Parse(cbMonthEnd.Text.Replace("月", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbDayBegin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeFrame == null)
                TimeFrame = new TimeFrameClass();
            TimeFrame.BeginDay = cbDayBegin.Text == "不区分" ? -1 : int.Parse(cbDayBegin.Text.Replace("日", ""));
            if (ArgChanged != null)
                ArgChanged();
        }

        private void cbDayEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeFrame == null)
                TimeFrame = new TimeFrameClass();
            TimeFrame.EndDay = cbDayEnd.Text == "不区分" ? -1 : int.Parse(cbDayEnd.Text.Replace("日", ""));
            if (ArgChanged != null)
                ArgChanged();
        }
    }
}
