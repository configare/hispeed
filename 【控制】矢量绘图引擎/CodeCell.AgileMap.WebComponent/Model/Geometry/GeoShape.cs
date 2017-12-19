using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CodeCell.AgileMap.WebComponent
{
    public abstract class GeoShape:IProjectableArguments
    {
        private enumCoordTypes _coordType = enumCoordTypes.Geographics;

        public GeoShape()
        { 
        }

        public enumCoordTypes CoordType
        {
            get { return _coordType; }
            set { _coordType = value; }
        }

        public virtual Geometry ToGeometry()
        {
            return null;
        }

        public virtual double[] SingleValues
        {
            get { return null; }
        }

        public virtual GeoPoint[] Points
        {
            get { return null; }
        }
    }
}
