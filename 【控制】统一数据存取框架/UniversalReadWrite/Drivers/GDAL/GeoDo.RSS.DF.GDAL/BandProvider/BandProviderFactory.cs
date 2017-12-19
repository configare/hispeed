using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using OSGeo.GDAL;
using GeoDo.HDF5;
using GeoDo.MEF;

namespace GeoDo.RSS.DF.GDAL
{
    public class BandProviderFactory
    {
        private static IBandProvider[] RegisteredBandProviders = null;
        private static bool IsLoadedBandProviders = false;

        public static IBandProvider GetBandProvider(string fname, 
            byte[] header1024, Access access, 
            IRasterDataProvider provider,Dictionary<string,string> datasetNames)
        {
            IBandProvider prd = null;
            if (!IsLoadedBandProviders)
            {
                RegisteredBandProviders = TryLoadRegisteredBandProviders();
            }
            prd = GetMatchedBandProvider(fname, header1024, RegisteredBandProviders,datasetNames);
            if (prd != null)
            {
                DataIdentify dataIdentify = prd.DataIdentify;
                prd.DataIdentify = new DataIdentify();
                prd = Activator.CreateInstance(prd.GetType()) as IBandProvider;
                prd.Init(fname, access == Access.GA_ReadOnly ? enumDataProviderAccess.ReadOnly : enumDataProviderAccess.Update, provider);
                prd.DataIdentify = dataIdentify;
            }
            return prd;
        }

        public static void PreLoading()
        {
            RegisteredBandProviders = TryLoadRegisteredBandProviders();
        }

        private static IBandProvider[] TryLoadRegisteredBandProviders()
        {
            IsLoadedBandProviders = true;
            string[] files = MefConfigParser.GetAssemblysByCatalog("波段提供者");
            using (IComponentLoader<IBandProvider> loader = new ComponentLoader<IBandProvider>())
            {
                return loader.LoadComponents(files);
            }
        }

        private static IBandProvider GetMatchedBandProvider(string fname, byte[] header1024, 
            IBandProvider[] prds,Dictionary<string,string> datasetNames)
        {
            if (prds == null || prds.Length == 0)
                return null;
            foreach (IBandProvider prd in prds)
                if (prd.IsSupport(fname, header1024,datasetNames))
                    return prd;
            return null;
        }
    }
}
