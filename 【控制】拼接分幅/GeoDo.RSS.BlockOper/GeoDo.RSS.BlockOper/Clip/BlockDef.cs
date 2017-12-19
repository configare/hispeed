using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    public class BlockDef
    {
        private string _name = null;
        private double _minX = 0;
        private double _minY = 0;
        private double _maxX = 0;
        private double _maxY = 0;
        private CoordEnvelope _envelope = null;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double MinX
        {
            get { return _minX; }
        }

        public double MinY
        {
            get { return _minY; }
        }

        public double MaxX
        {
            get { return _maxX; }
        }

        public double MaxY
        {
            get { return _maxY; }
        }

        public BlockDef(string name,double minX,double minY,double maxX,double maxY)
        {
            _name = name;
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

        }

        public BlockDef(string name, double minX, double minY, double span)
            :this(name,minX,minY,minX+span,minY+span)
        {

        }

        public CoordEnvelope ToEnvelope()
        {
            if (_envelope == null)
                _envelope = new CoordEnvelope(MinX, MaxX, MinY, MaxY);
            return _envelope;
        }
    }

    //area of interest
    public class BlockDefWithAOI:BlockDef
    {
        private int[] _aOIIndexes = null;

        public int[] AOIIndexes
        {
            get { return _aOIIndexes; }
            set { _aOIIndexes = value; }
        }

        public BlockDefWithAOI(string name, double minX, double minY, double maxX, double maxY)
            : base(name, minX, minY, maxX, maxY)
        {
        }
    }
}
