using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.MapService
{
    public class MapImageGeneratorPool:IMapImageGeneratorPool
    {
        /*
         *               _gen = new MapImageGeneratorDefault();
                _outputdir = System.Configuration.ConfigurationManager.AppSettings["MapImageOutputDir"];
                _outputUrlBase = System.Configuration.ConfigurationManager.AppSettings["MapImageUrlBaseDir"];
                string mcd = System.Configuration.ConfigurationManager.AppSettings["MapConfigFile"];
                IMap map = MapFactory.LoadMapFrom(mcd);
                _gen.ApplyMap(map);

         */
        private List<MapImageGeneratorInstance> _instances = new List<MapImageGeneratorInstance>();

        public MapImageGeneratorPool(int maxInstanceCount,string outputDir,string outputUrlBase,string mapConfigDocument)
        {
            for (int i = 0; i < maxInstanceCount; i++)
            {
                try
                {
                    IMapImageGenerator gen = new MapImageGeneratorDefault();
                    IMap map = MapFactory.LoadMapFrom(mapConfigDocument);
                    gen.ApplyMap(map);
                    _instances.Add(new MapImageGeneratorInstance(gen));
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                }
            }
        }

        public int FreeInstanceCount
        {
            get 
            {
                lock (this)
                { 
                    int n=0;
                    foreach (IMapImageGeneratorInstance ist in _instances)
                        if (ist.IsFree)
                            n++;
                    return n;
                }
            }
        }

        public int InstanceCount
        {
            get { return _instances.Count; }
        }

        private object lockObj = new object();
        public IMapImageGeneratorInstance GetMapImageGeneratorInstance()
        {
            lock (lockObj)
            {
                foreach (IMapImageGeneratorInstance ist in _instances)
                {
                    if (ist.IsFree)
                    {
                        ist.Hold();
                        return ist;
                    }
                }
                return null;
            }
        }
    }
}