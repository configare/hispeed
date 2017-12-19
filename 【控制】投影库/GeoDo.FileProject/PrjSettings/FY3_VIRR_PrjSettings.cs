using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class FY3_VIRR_PrjSettings:FilePrjSettings
    {
        //要投影的数据集，包含太阳天顶角，太阳方位角，传感器天顶角,传感器方位角
        private bool _isRadiation = true;         //执行辐射定标
        private bool _isSolarZenith = true;       //执行太阳天顶角订正
        private bool _isOutMapTable = false;      //输出原始行列号
        private bool _isSensorZenith = false;     //执行临边变暗订正
        private IRasterDataProvider _geoFile = null;

        public FY3_VIRR_PrjSettings()
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
        /// <summary>
        /// 2013年9月24日添加属性
        /// 定位文件,目前仅对FY3C有效
        /// </summary>
        public IRasterDataProvider GeoFile
        {
            get { return _geoFile; }
            set { _geoFile = value; }
        }
    }
}
