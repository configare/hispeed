using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class EOS_MODIS_PrjSettings:FilePrjSettings
    {
        private IRasterDataProvider[] _secondaryOrbitRaster = null;
        private IRasterDataProvider _locationFile = null;      //定位文件,03文件
        private bool _isRadiation = true;         //执行辐射定标
        private bool _isSolarZenith = true;       //执行太阳天顶角订正
        private bool _isOutMapTable = false;      //输出原始行列号
        public bool IsSensorZenith = false;

        public EOS_MODIS_PrjSettings()
            : base()
        {
        }

        public IRasterDataProvider[] SecondaryOrbitRaster
        {
            get { return _secondaryOrbitRaster; }
            set { _secondaryOrbitRaster = value; }
        }

        /// <summary>
        /// 经纬度坐标文件
        /// ***.MOD03.hdf文件
        /// </summary>
        public IRasterDataProvider LocationFile
        {
            get { return _locationFile; }
            set { _locationFile = value; }
        }

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
