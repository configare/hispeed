using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    public  class VectorMapServerEnironment:IDisposable,IVectorMapServerEnvironment
    {
        private static IVectorMapServerEnvironment _instance = null;
        private Dictionary<int, IServerInstance> _instances = new Dictionary<int, IServerInstance>();

        private VectorMapServerEnironment(string cnfgfile)
        {
            IServerInstance[] insts = CnfgFileParser.Parse(cnfgfile);
            if (insts != null && insts.Length > 0)
                foreach (IServerInstance i in insts)
                    _instances.Add(i.InstanceIdentify.Id, i);
        }

        public static IVectorMapServerEnvironment GetInstance(string cnfgfile)
        {
            if (_instance == null)
                _instance = new VectorMapServerEnironment(cnfgfile);
            return _instance;
        }

        public static IVectorMapServerEnvironment Instance
        {
            get { return _instance; }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_instances != null)
            {
                _instances.Clear();
                _instances = null;
            }
            _instance = null;
        }

        #endregion

        #region IVectorMapServerEnvironment Members

        public InstanceIdentify[] GetAllServerInstances()
        {
            if (_instances == null || _instances.Count == 0)
                return null;
            List<InstanceIdentify> ids = new List<InstanceIdentify>();
            foreach (IServerInstance i in _instances.Values)
            {
                ids.Add(i.InstanceIdentify);
            }
            return ids.Count > 0 ? ids.ToArray() : null;
        }

        private IServerInstance GetServerInstanceById(int id)
        {
            if (_instances.ContainsKey(id))
                return _instances[id];
            return null;
        }

        public void GetCatalogsByInstanceId(int id, out FetDatasetIdentify[] dsIds, out FetClassIdentify[] fetclassIds)
        {
            dsIds = null;
            fetclassIds = null;
            IServerInstance instance = GetServerInstanceById(id);
            if (instance == null)
                throw new Exception(string.Format(HttpCommands.cstErrorInstanceIsNotExisted, id));
            try
            {
                ICatalogProvider prd = instance.CatalogProvider;
                if (prd == null)
                    throw new Exception(string.Format(HttpCommands.cstErrorCatalogProviderNotExistdOfInstance, id));
                dsIds = prd.GetFetDatasetIdentify();
                fetclassIds = prd.GetFetClassIdentify();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(HttpCommands.cstErrorCatalogProviderNotExistdOfInstance+","+ex.Message, id));
            }
        }

        public FetClassProperty GetFetClassProperty(int instanceId, string fetclassId)
        {
            IServerInstance instance = GetServerInstanceById(instanceId);
            if (instance == null)
                throw new Exception(string.Format(HttpCommands.cstErrorInstanceIsNotExisted, instanceId));
            ICatalogProvider prd = instance.CatalogProvider;
            if (prd == null)
                throw new Exception(string.Format(HttpCommands.cstErrorCatalogProviderNotExistdOfInstance, fetclassId));
            return prd.GetFetClassProperty(fetclassId);
        }

        public void BeginRead(int instanceId, string fetclassId)
        {
        }

        public void EndRead(int instanceId, string fetclassId)
        {
        }

        public Feature[] ReadFeatures(int instanceId, string fetclassId, Envelope evp)
        {
            IServerInstance instance = GetServerInstanceById(instanceId);
            if (instance == null)
                throw new Exception(string.Format(HttpCommands.cstErrorInstanceIsNotExisted, instanceId));
            using (IFeaturesReaderService reader = instance.GetFeaturesReaderService(fetclassId))
            {
                return reader.Read(evp);
            }
        }

        #endregion
    }
}
