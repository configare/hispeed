using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.FileProject
{
    public class Fy2_NOM_PrjSettings:FilePrjSettings
    {
        //要投影的数据集，包含太阳天顶角，太阳方位角，传感器天顶角,传感器方位角
        private bool _isRadiation = true;         //执行辐射定标
        private bool _isSolarZenith = true;       //执行太阳天顶角订正
        private bool _isOutMapTable = false;      //输出原始行列号

        private bool _isSensorZenith = false;     //执行临边变暗订正

        public Fy2_NOM_PrjSettings()
            : base()
        {
        }

        public bool IsRadiation
        {
            get { return _isRadiation; }
            set { _isRadiation = value; }
        }

        public bool IsSolarZenith
        {
            get { return _isSolarZenith; }
            set { _isSolarZenith = value; }
        }

        public bool IsOutMapTable
        {
            get { return _isOutMapTable; }
            set { _isOutMapTable = value; }
        }

        public bool IsSensorZenith
        {
            get { return _isSensorZenith; }
            set { _isSensorZenith = value; }
        }
    }
}
