using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class RepeatFeatureRecorder
    {
        private List<int> _renderedOIDs = new List<int>();

        public RepeatFeatureRecorder()
        { 
        }

        public void AddRenderedOID(int oid)
        {
            if (!_renderedOIDs.Contains(oid))
                _renderedOIDs.Add(oid);
        }

        public void BeginRender()
        { 
        }

        public bool IsRendered(int oid)
        {
            return _renderedOIDs.Contains(oid);
        }

        public void EndRender()
        {
            _renderedOIDs.Clear();
        }
    }
}
