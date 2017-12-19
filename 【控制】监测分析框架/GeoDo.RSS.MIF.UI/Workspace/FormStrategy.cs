using System;
using System.Linq;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.UI
{
    public partial class FormStrategy : Form
    {
        private WorkspaceDef _wksdef;
        public FormStrategy()
        {
            InitializeComponent();
        }

        public void Init(WorkspaceDef wksdef)
        {
            _wksdef = wksdef;
            var strategyFilter = UWorkspace.StrategyFilter(wksdef);

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                var item = checkedListBox1.Items[i].ToString();
                var check = strategyFilter.Sensors.Contains(item);
                checkedListBox1.SetItemChecked(i, check);  
            }

            numericUpDown1.Value = Convert.ToDecimal(strategyFilter.Days);
        }

        public int Day
        {
            get { return Convert.ToInt32(numericUpDown1.Value); }
        }

        public string[] Sensors
        {
            get
            {
                var seneors = new string[checkedListBox1.CheckedItems.Count];
                for (int i = 0; i < seneors.Length; i++)
                {
                    var item = checkedListBox1.CheckedItems[i];
                    seneors[i] = Convert.ToString(item);
                }
                return seneors;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var strategyFilter = new StrategyFilter();
            strategyFilter.Days = Day;
            strategyFilter.Sensors = Sensors;
            var str = UXmlConvert.GetString(strategyFilter);
            var filename = UWorkspace.GetStrategyXmlName(_wksdef);
            UIO.SaveFile(str, filename);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
