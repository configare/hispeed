using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    public interface IVectorMapServerEnvironment
    {
        InstanceIdentify[] GetAllServerInstances();
        void GetCatalogsByInstanceId(int id,out FetDatasetIdentify[] dsIds,out FetClassIdentify[] fetclassIds);
        FetClassProperty GetFetClassProperty(int instanceId, string fetclassId);
        Feature[] ReadFeatures(int instanceId, string fetclassId, Envelope evp);
        void BeginRead(int instanceId, string fetclassId);
        void EndRead(int instanceId, string fetclassId);
    }
}
