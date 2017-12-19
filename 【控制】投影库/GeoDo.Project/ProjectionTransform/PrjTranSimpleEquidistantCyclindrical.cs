using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    /// <summary>
    /// GLL:Geographic Latitude/Longitude
    /// Equirectangular  
    /// Alias  Plate Caree  
    /// Alias  Equidistant Cylindrical  
    /// Alias  Simple Cylindrical  
    /// </summary>
    internal class PrjTranSimpleEquidistantCyclindrical : IProjectionTransform, IDisposable
    {
        private const double DEGREEStoRADIANS = Math.PI / 180;
        private const double RADIANStoDEGREES = 180 / Math.PI;
        private readonly double WGS84SEMIMAJOR = 6378137.0;
        private readonly double ONEOVERWGS84SEMIMAJOR = 0;
        private readonly float a = 0;
        private readonly float b = 0;
        private readonly float c = 0;
        private readonly float d = 0;

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
            for (int i = 0; i < xs.Length; i++)
            {
                xs[i] *= a;
                ys[i] *= b;
            }
        }

        public void InverTransform(double[] xs, double[] ys)
        {
            for (int i = 0; i < xs.Length; i++)
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
