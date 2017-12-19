using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public class ValArguments
    {
        private string[] _fileNamesForVal;  //待校验数据文件名列表
        private string[] _fileNamesToVal;   //校验数据文件名列表
        private bool _creatScatter;         //是否生成散点图
        private bool _creatTimeSeq;         //是否生成时间序列图
        private bool _creatHistogram;       //是否生成偏差直方图
        private bool _creatRMSE;            //是否计算RMSE
        private string _minX;               //最小经度
        private string _maxX;               //最大经度
        private string _minY;               //最小纬度
        private string _maxY;               //最大纬度
        private string _invalid;            //待验证数据无效值
        private string _forinvalid;         //验证数据无效值
        private string _outdir;             //输出文件夹
        private string _outRes;             //输出分辨率
        private string _maxColumns;         //偏差直方图最大分组数
        private string _regionNam;          //范围名称
        private string _inputDir;           //输入文件夹
        private string _cldprd;             //选择的云产品

        public string[] FileNamesForVal
        {
            get { return _fileNamesForVal; }
            set { _fileNamesForVal = value; }
        }

        public string[] FileNamesToVal
        {
            get { return _fileNamesToVal; }
            set { _fileNamesToVal = value; }
        }

        public bool CreatScatter
        {
            get { return _creatScatter; }
            set { _creatScatter = value; }
        }


        public bool CreatTimeSeq
        {
            get { return _creatTimeSeq; }
            set { _creatTimeSeq = value; }
        }

        public bool CreatHistogram
        {
            get { return _creatHistogram; }
            set { _creatHistogram = value; }
        }

        public bool CreatRMSE
        {
            get { return _creatRMSE; }
            set { _creatRMSE = value; }
        }

        public string Invalid
        {
            get { return _invalid; }
            set { _invalid = value; }
        }

        public string ForInvalid
        {
            get { return _forinvalid; }
            set { _forinvalid = value; }
        }

        public string Outdir
        {
            get { return _outdir; }
            set { _outdir = value; }
        }

        public string OutRes
        {
            get { return _outRes; }
            set { _outRes = value; }
        }
        public string MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        public string MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public string MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public string MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }
        public string MaxColumns
        {
            get { return _maxColumns; }
            set { _maxColumns = value; }
        }

        public string RegionNam
        {
            get { return _regionNam; }
            set { _regionNam = value; }
        }
        public string InputDir
        {
            get { return _inputDir; }
            set { _inputDir = value; }
        }

        public string CLDPrd
        {
            get { return _cldprd; }
            set { _cldprd = value; }
        }
    }
}
