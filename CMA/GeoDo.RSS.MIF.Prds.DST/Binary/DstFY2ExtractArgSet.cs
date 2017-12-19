using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class DstFY2ExtractArgSet
    {
        bool _isSmooth;

        public bool IsSmooth
        {
            get { return _isSmooth; }
            set { _isSmooth = value; }
        }
        bool _isNightExtract;

        public bool IsNightExtract
        {
            get { return _isNightExtract; }
            set { _isNightExtract = value; }
        }
        double[] _argValueArray;

        public double[] ArgValueArray
        {
            get { return _argValueArray; }
            set { _argValueArray = value; }
        }

        string _backFileName;

        public string BackFileName
        {
            get { return _backFileName; }
            set { _backFileName = value; }
        }

    }
}
