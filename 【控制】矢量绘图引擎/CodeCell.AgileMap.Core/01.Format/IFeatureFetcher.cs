using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureFetcher
    {
        Feature FetchFirstFeature();
        Feature FetchFeature(Func<Feature, bool> where);
        Feature[] FetchFeatures(Func<Feature, bool> where);
        Feature[] FetchFeatures();
    }
}
