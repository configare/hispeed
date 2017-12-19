using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class LabelLocationServiceDefault:BaseLabelLocationService
    {
        public LabelLocationServiceDefault(IFeatureRenderEnvironment environment)
            : base(environment)
        { 
        }

        public override void UpdateLocations()
        {
            if (_geometry == null) 
                return;
            if (_locations == null || _locations.Length == 0)
                _locations = new LabelLocation[1] { new LabelLocation()};
            _locations[0].Angle = 0;
            _locations[0].Location = _geometry.Centroid;
        }
    }
}
