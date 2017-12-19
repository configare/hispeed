using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    public class Grid:IGrid,IDisposable,ISupportOutsideIndicator,IIdentifyFeatures
    {
        protected int _gridNo = -1;
        protected List<Feature> _features = null;
        protected Envelope _envelope = null;
        protected Envelope _gridEnvelope = null;
        protected int _estimateSize = 0;
        protected bool _coordIsConverted = false;
        protected IOutsideIndicator _outsideIndicator = null;
        internal bool _isfullInternal = false;//session,一次绘制会话内有效

        public Grid(int gridNo, Envelope gridEnvelope, Feature[] features)
        {
            _gridEnvelope = gridEnvelope;
            _gridNo = gridNo;
            _features = new List<Feature>(features);
            UpdateEnvelope();
            DoEstimateSize();
            _outsideIndicator = new OutsideIndicator();
        }

        private void DoEstimateSize()
        {
            if (_features == null || _features.Count == 0)
            {
                _estimateSize = 0;
                return;
            }
            _estimateSize = 0;
            foreach (Feature fet in _features)
                _estimateSize += fet.EstimateSize();
        }

        internal void UpdateEnvelope()
        {
            if (_features == null || _features.Count == 0)
            {
                _envelope = null;
                return;
            }
            _envelope = null;
            foreach (Feature fet in _features)
            {
                if (fet == null || fet.Geometry == null)
                    continue;
                if (_envelope == null)
                    _envelope = fet.Geometry.Envelope.Clone() as Envelope;
                else
                    _envelope.UnionWith(fet.Geometry.Envelope);
            }
        }

        #region IGrid Members

        public bool CoordIsConverted
        {
            get { return _coordIsConverted; }
            set { _coordIsConverted = value; }
        }

        public int EstimateSize
        {
            get { return _estimateSize; }
        }

        public int GridNo
        {
            get { return _gridNo; }
        }

        public Envelope GridEnvelope
        {
            get { return _gridEnvelope; }
        }

        public Envelope Envelope
        {
            get { return _envelope; }
        }

        public List<Feature> VectorFeatures
        {
            get { return _features; }
        }

        public bool IsEmpty()
        {
            return _features == null || _features.Count == 0;
        }

        #endregion

        #region IIdentifyFeatures 成员

        public Feature[] Identify(Shape geometry, double tolerance)
        {
            if (geometry == null || IsEmpty())
                return null;
            List<Feature> retfets = new List<Feature>();
            foreach (Feature fet in _features)
            {
                if (fet.Geometry.HitTest(geometry, tolerance))
                    retfets.Add(fet);
            }
            return retfets.Count > 0 ? retfets.ToArray() : null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _features.Clear();
        }

        #endregion

        #region ISupportOutsideIndicator Members

        [Browsable(false)]
        public IOutsideIndicator OutsideIndicator
        {
            get { return _outsideIndicator; }
        }

        #endregion
    }
}
