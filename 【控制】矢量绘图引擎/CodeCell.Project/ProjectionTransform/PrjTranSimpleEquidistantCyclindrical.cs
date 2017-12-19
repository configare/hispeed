using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// GLL:Geographic Latitude/Longitude
    /// Equirectangular  
    /// Alias  Plate Caree  
    /// Alias  Equidistant Cylindrical  
    /// Alias  Simple Cylindrical  
    /// </summary>
    public class PrjTranSimpleEquidistantCyclindrical:IProjectionTransform,IDisposable
    {
        protected const double DEGREEStoRADIANS = Math.PI / 180;
        protected const double RADIANStoDEGREES = 180 / Math.PI;
        protected  double WGS84SEMIMAJOR = 6378137.0;
        protected  double ONEOVERWGS84SEMIMAJOR = 0;
        protected readonly float a = 0;
        protected readonly float b = 0;
        protected readonly float c = 0;
        protected readonly float d = 0;

        public PrjTranSimpleEquidistantCyclindrical()
        {
            ONEOVERWGS84SEMIMAJOR = 1.0 / WGS84SEMIMAJOR;
            a = (float)(DEGREEStoRADIANS * Math.Cos(0) * WGS84SEMIMAJOR);
            b = (float)(DEGREEStoRADIANS * WGS84SEMIMAJOR);
            //
            c = (float)(ONEOVERWGS84SEMIMAJOR / Math.Cos(0) * RADIANStoDEGREES);
            d = (float)(ONEOVERWGS84SEMIMAJOR * RADIANStoDEGREES);
        }

        public PrjTranSimpleEquidistantCyclindrical(double semimajorAxis)
        {
            WGS84SEMIMAJOR = semimajorAxis;
            ONEOVERWGS84SEMIMAJOR = 1.0 / WGS84SEMIMAJOR;
            a = (float)(DEGREEStoRADIANS * Math.Cos(0) * WGS84SEMIMAJOR);
            b = (float)(DEGREEStoRADIANS * WGS84SEMIMAJOR);
            //
            c = (float)(ONEOVERWGS84SEMIMAJOR / Math.Cos(0) * RADIANStoDEGREES);
            d = (float)(ONEOVERWGS84SEMIMAJOR * RADIANStoDEGREES);
        }

        #region IProjectionTransform Members

        public void Transform(double[] xs, double[] ys)
        { 
            int size = xs.Length ;
            for (int i = 0; i < size; i++)
            {
                xs[i] *= a;
                ys[i] *= b;
            }
        }

        public void InverTransform(double[] xs, double[] ys)
        {
            int size = xs.Length;
            for (int i = 0; i < size; i++)
            {
                xs[i] *= c;
                ys[i] *= d;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
