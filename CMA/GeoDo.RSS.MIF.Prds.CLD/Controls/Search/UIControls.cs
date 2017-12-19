using System.Collections.Generic;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    //界面控件集合单例
    public class UIControls
    {
        private GroupBox _groupBoxProductDataSource;
        private GroupBox _groupBoxProductType;
        private GroupBox _groupBoxDataSet;
        private GroupBox _groupBoxProductPeriod;
        private GroupBox _groupBoxDayNight;
        private GroupBox _groupBoxProductTime;

        private static UIControls _instance = null;

        public GroupBox GroupBoxProductDataSource { get { return _groupBoxProductDataSource; } }
        public GroupBox GroupBoxProductType { get { return _groupBoxProductType; } }
        public GroupBox GroupBoxDataSet { get { return _groupBoxDataSet; } }
        public GroupBox GroupBoxProductPeriod { get { return _groupBoxProductPeriod; } }
        public GroupBox GroupBoxDayNight { get { return _groupBoxDayNight; } }
        public GroupBox GroupBoxProductTime { get { return _groupBoxProductTime; } }

        private UIControls() { }

        public static UIControls GetInstance()
        {
            if (_instance == null)
                _instance = new UIControls();
            return _instance;
        }

        public void SetUIControls(GroupBox groupBoxProductDataSource, GroupBox groupBoxProductType, GroupBox groupBoxDataSet, GroupBox groupBoxProductPeriod, GroupBox groupBoxDayNight, GroupBox groupBoxProductTime)
        {
            _groupBoxProductDataSource = groupBoxProductDataSource;
            _groupBoxProductType = groupBoxProductType;
            _groupBoxDataSet = groupBoxDataSet;
            _groupBoxProductPeriod = groupBoxProductPeriod;
            _groupBoxDayNight = groupBoxDayNight;
            _groupBoxProductTime = groupBoxProductTime;
        }

        //检查产品数据源合法性
        public bool CheckValidProductDataSource()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxProductDataSource.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null)
                {
                    bits.Add(rb.Checked);
                }
            }
            return CheckJustOne(bits);
        }

        //检查产品类型合法性
        public bool CheckValidProductType()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxProductType.Controls)
            {
                ucRadioBoxList ucrbl = c as ucRadioBoxList;
                if (ucrbl != null)
                {
                    foreach (RadioButton rb in ucrbl)
                    {
                        bits.Add(rb.Checked);
                    }
                    break;
                }
            }
            return CheckJustOne(bits);
        }

        //检查数据集合法性
        public bool CheckValidDataSet()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxDataSet.Controls)
            {
                ucCheckBoxList uccbl = c as ucCheckBoxList;
                if (uccbl != null)
                {
                    foreach (CheckBox cb in uccbl)
                    {
                        bits.Add(cb.Checked);
                    }
                    break;
                }
            }
            return CheckAtLeastOne(bits);
        }

        //检查产品周期合法性
        public bool CheckValidProductPeriod()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null)
                {
                    bits.Add(rb.Checked);
                }
            }
            return CheckJustOne(bits);
        }

        //检查昼夜合法性
        public bool CheckValidDayNight()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxDayNight.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null)
                {
                    bits.Add(rb.Checked);
                }
            }
            return CheckJustOne(bits);
        }

        //检查产品时间合法性
        public bool CheckValidProductTime()
        {
            if (!CheckValidSearchType())
            {
                MessageBox.Show("请选择查询类型");
                return false;
            }
            else
            {
                if (IsSearchTypeSamePeriodChecked())
                {
                    if (IsProductPeriodChecked("月产品"))
                    {
                        if (!CheckValidMonth())
                        {
                            MessageBox.Show("请选择月");
                            return false;
                        }
                    }
                    else if (IsProductPeriodChecked("日产品") || IsProductPeriodChecked("年产品"))
                    {
                        MessageBox.Show("请选择月产品");
                        return false;
                    }
                }
            }
            return true;
        }

        //检查统计类型合法性
        private bool CheckValidSearchType()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxProductTime.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null)
                {
                    bits.Add(rb.Checked);
                }
            }
            return CheckJustOne(bits);
        }

        //检查月份合法性
        private bool CheckValidMonth()
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in _groupBoxProductTime.Controls)
            {
                Panel p = c as Panel;
                if (p != null && p.Name == "panelPrdsTimeMonth")
                {
                    foreach (Control cc in p.Controls)
                    {
                        ucMonths ucM = cc as ucMonths;
                        if (ucM != null)
                        {
                            foreach (CheckBox cb in ucM)
                            {
                                bits.Add(cb.Checked);
                            }
                        }
                    }
                    break;
                }
            }
            return CheckAtLeastOne(bits);
        }

        //是否选中了日、月、年产品
        private bool IsProductPeriodChecked(string product)
        {
            foreach (Control c in _groupBoxProductPeriod.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Text == product && rb.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        //是否选中了同期查询
        private bool IsSearchTypeSamePeriodChecked()
        {
            foreach (Control c in _groupBoxProductTime.Controls)
            {
                RadioButton rb = c as RadioButton;
                if (rb != null && rb.Text == "同期查询" && rb.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        //CheckBox只能选择一个，不能为空
        public static bool CheckJustOne(List<bool> bits)
        {
            int count = 0;
            foreach (bool bit in bits)
            {
                if (bit == true)
                    ++count;
            }
            if (count == 1)
                return true;
            return false;
        }

        //CheckBox至少得选择一个
        public static bool CheckAtLeastOne(List<bool> bits)
        {
            int count = 0;
            foreach (bool bit in bits)
            {
                if (bit == true)
                    ++count;
            }
            if (count >= 1)
                return true;
            return false;
        }

        public bool CheckCloudSATValidProductType(GroupBox group)
        {
            List<bool> bits = new List<bool>();
            foreach (Control c in group.Controls)
            {
                ucRadioBoxList ucrbl = c as ucRadioBoxList;
                if (ucrbl != null)
                {
                    foreach (RadioButton rb in ucrbl)
                    {
                        bits.Add(rb.Checked);
                    }
                    break;
                }
            }
            return CheckJustOne(bits);
        }

    }
}
