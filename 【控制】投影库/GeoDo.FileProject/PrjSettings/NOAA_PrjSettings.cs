using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.FileProject
{
    public class NOAA_PrjSettings : FilePrjSettings
    {
        private bool _isRadiation = true;         //执行辐射定标
        private bool _isSolarZenith = true;       //执行太阳天顶角订正
        private bool _isOutMapTable = false;      //输出原始行列号
        public bool IsSensorZenith = false;

        public NOAA_PrjSettings()
            : base()
        { }

        /// <summary>
        /// <value>true</value>:执行亮温计算
        /// <value>false</value>:不执行亮温计算
        /// </summary>
        public bool IsRadiation
        {
            get { return _isRadiation; }
            set { _isRadiation = value; }
        }

        /// <summary>
        /// 是否对反射通道执行太阳高度角订正
        /// </summary>
        public bool IsSolarZenith
        {
            get { return _isSolarZenith; }
            set { _isSolarZenith = value; }
        }

        /// <summary>
        /// 是否输出原始行列号（暂未支持）
        /// </summary>
        public bool IsOutMapTable
        {
            get { return _isOutMapTable; }
            set { _isOutMapTable = value; }
        }
    }
}
