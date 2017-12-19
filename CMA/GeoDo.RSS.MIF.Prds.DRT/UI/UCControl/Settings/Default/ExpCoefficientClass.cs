using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    [Serializable]
    public class DRTExpCoefficientCollection
    {
        private string _egdesFilename;
        private DRTExpCoefficientItem[] _exps = new DRTExpCoefficientItem[] { new DRTExpCoefficientItem() };

        [DisplayName("边界文件"), Category("基础信息"), EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string EgdesFilename
        {
            get { return _egdesFilename; }
            set { _egdesFilename = value; }
        }

        [DisplayName("对照信息"), Category("基础信息")]
        public DRTExpCoefficientItem[] Exps
        {
            get { return _exps; }
            set { _exps = value; }
        }

        public DRTExpCoefficientCollection()
        { }

        public DRTExpCoefficientItem GetExpItemByNum(int num)
        {
            if (Exps == null || Exps.Length == 0)
                return null;
            foreach (DRTExpCoefficientItem item in Exps)
            {
                if (item.Num == num)
                    return item;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    [Serializable]
    public class DRTExpCoefficientItem
    {
        private string _name = string.Empty;
        private int _num = 0;
        private double _aPara = 0;
        private double _bPara = 0;
        private double _cPara = 0;

        [DisplayName("1 名称"), Category("基础信息")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DisplayName("0 编号"), Category("基础信息")]
        public int Num
        {
            get { return _num; }
            set { _num = value; }
        }

        [DisplayName("2 系数A：斜率/参数1"), Category("查对关系")]
        public double APara
        {
            get { return _aPara; }
            set { _aPara = value; }
        }

        [DisplayName("3 系数B：截距/参数2"), Category("查对关系")]
        public double BPara
        {
            get { return _bPara; }
            set { _bPara = value; }
        }

        [DisplayName("3 系数C：修正值"), Category("查对关系")]
        public double CPara
        {
            get { return _cPara; }
            set { _cPara = value; }
        }

        public DRTExpCoefficientItem()
        { }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_name))
                return string.Empty;
            else
                return string.Format("{0} {1} {2} {3}", _num.ToString() + "_" + _name, _aPara, _bPara, _cPara);
        }
    }
}
