using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal abstract class BaseLabelLocationService:ILabelLocationService
    {
        internal IFeatureRenderEnvironment _environment = null;
        protected Shape _geometry = null;
        protected LabelLocation[] _locations = null;

        public BaseLabelLocationService(IFeatureRenderEnvironment environment)
        {
            _environment = environment;
        }

        #region ILabelLocationService Members

        public LabelLocation[] LabelLocations
        {
            get 
            {
                return _locations;
            }
        }

        public void Update(Shape geometry)
        {
            _geometry = geometry;
            UpdateLocations();
        }

        public abstract void UpdateLocations();

        #endregion
    }
}
