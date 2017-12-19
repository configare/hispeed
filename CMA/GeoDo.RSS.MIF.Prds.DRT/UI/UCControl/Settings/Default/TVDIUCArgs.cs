using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class TVDIUCArgs
    {
        private string _ndviFile;
        private string _lstFile;
        private string _ecLstFile;
        private string _demFile;
        private TVDIParaClass _tvdiParas = null;
        private DryWetEdgeArgs _dryWetEdgesFitting = null;

        public TVDIUCArgs()
        {

        }

        public string NDVIFile
        {
            get
            {
                return _ndviFile;
            }
            set
            {
                _ndviFile = value;
            }
        }

        public string LSTFile
        {
            get
            {
                return _lstFile;
            }
            set
            {
                _lstFile = value;
            }
        }

        /// <summary>
        /// 高程订正后的LST
        /// </summary>
        public string ECLstFile
        {
            get
            {
                return _ecLstFile;
            }
            set
            {
                _ecLstFile = value;
            }
        }

        public string DEMFile
        {
            get
            {
                return _demFile;
            }
            set
            {
                _demFile = value;
            }
        }

        public TVDIParaClass TVDIParas
        {
            get
            {
                return _tvdiParas;
            }
            set
            {
                _tvdiParas = value;
            }
        }

        public DryWetEdgeArgs DryWetEdgesFitting
        {
            get
            {
                return _dryWetEdgesFitting;
            }
            set
            {
                _dryWetEdgesFitting = value;
            }
        }
    }
}
