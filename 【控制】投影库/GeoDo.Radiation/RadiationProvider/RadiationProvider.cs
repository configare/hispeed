using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.MEF;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Radiation
{
    /// <summary>
    /// 亮温转换提供者
    /// </summary>
    public abstract class RadiationProvider : IRadiationProvider
    {
        protected string _name;

        public string Name
        {
            get { return _name; }
        }

        public abstract void InitRadiationArgs(IRasterDataProvider srcRaster, bool isSolarZenith);

        public abstract void DoRadiation(ushort[] srcBandData, int xOffset, int yOffset, int srcWidth, int srcHeight,
            int bandIndex, bool isRadiation, bool isSolarZenith);
        
        #region STATIC

        public static IRadiationProvider GetFileProjectByName(string name)
        {
            if (_loadedRadiationProviders == null)
                _loadedRadiationProviders = LoadAllRadiationProviders();
            if (_loadedRadiationProviders == null || _loadedRadiationProviders.Length == 0)
                return null;
            foreach (IRadiationProvider rad in _loadedRadiationProviders)
                if (rad.Name == name)
                    return rad;
            return null;
        }

        private static IRadiationProvider[] _loadedRadiationProviders = null;
        public static IRadiationProvider[] LoadAllRadiationProviders()
        {
            string fileCompon = AppDomain.CurrentDomain.BaseDirectory;
            if (_loadedRadiationProviders == null)
                using (IComponentLoader<IRadiationProvider> loader = new ComponentLoader<IRadiationProvider>())
                    _loadedRadiationProviders = loader.LoadComponents(fileCompon);
            return _loadedRadiationProviders;
        }

        #endregion
    }
}
