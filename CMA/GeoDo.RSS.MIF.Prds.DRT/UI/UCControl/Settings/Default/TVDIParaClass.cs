using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    [Serializable]
    public class TVDIParaClass
    {
        private ArgumentItem _ndviFile = new ArgumentItem();
        private ArgumentItem _lstFile = new ArgumentItem();
        private int _length = 0;
        private int _zoom = 0;
        private int _lstFreq = 0;
        private float _fLimit = 0;
        private float _hgYZ = 0;

        [DisplayName("1 放大倍数")]
        [Category("1 基础设置")]
        public int Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        [DisplayName("2 步长")]
        [Category("1 基础设置")]
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        [DisplayName("3 LST频次")]
        [Category("1 基础设置")]
        public int LstFreq
        {
            get { return _lstFreq; }
            set { _lstFreq = value; }
        }

        [DisplayName("4 最小相关系数")]
        [Category("1 基础设置")]
        public float FLimit
        {
            get { return _fLimit; }
            set { _fLimit = value; }
        }

        [DisplayName("5 回归阈值")]
        [Category("1 基础设置")]
        public float HGYZ
        {
            get { return _hgYZ; }
            set { _hgYZ = value; }
        }

        [DisplayName("植被指数参数")]
        [Category("2 植被指数参数"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ArgumentItem NdviFile
        {
            get { return _ndviFile; }
            set { _ndviFile = value; }
        }

        [DisplayName("陆表温度参数")]
        [Category("3 陆表温度参数"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ArgumentItem LstFile
        {
            get { return _lstFile; }
            set { _lstFile = value; }
        }

        public TVDIParaClass()
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    [Serializable]
    public class ArgumentItem
    {
        private int _band = 0;
        private int _max = 0;
        private int _min = 0;
        private int _zoom = 0;
        private int[] _invaild = new int[] { 0 };
        private int _cloudy = 0;

        [DisplayName("1 放大倍数")]
        public int Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        [DisplayName("2 波段")]
        public int Band
        {
            get { return _band; }
            set { _band = value; }
        }

        [DisplayName("3 最小值")]
        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        [DisplayName("4 最大值")]
        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        [DisplayName("5 云区")]
        public int Cloudy
        {
            get { return _cloudy; }
            set { _cloudy = value; }
        }

        [DisplayName("6 无效值")]
        public int[] Invaild
        {
            get { return _invaild; }
            set { _invaild = value; }
        }

        public ArgumentItem()
        { }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
