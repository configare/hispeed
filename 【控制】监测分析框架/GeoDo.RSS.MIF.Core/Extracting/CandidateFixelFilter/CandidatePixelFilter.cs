using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class CandidatePixelFilter : ICandidatePixelFilter
    {
        protected string _name;
        protected bool _isEnabled = true;
        protected bool _isFiltered = false;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public bool IsFiltered
        {
            get { return _isFiltered; }
        }

        public void Reset()
        {
            _isFiltered = false;
        }

        public int[] Filter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi)
        {
            if (dataProvider == null)
                return null;
            try
            {
                return DoFilter(dataProvider, aoiRect, aoi);
            }
            finally
            {
                _isFiltered = true;
            }
        }

        protected abstract int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi);

        public int[] Filter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo)
        {
            if (dataProvider == null)
                return null;
            try
            {
                return DoFilter(dataProvider, aoiRect, aoi, assistInfo);
            }
            finally
            {
                _isFiltered = true;
            }
        }

        protected abstract int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo);
    }
}
