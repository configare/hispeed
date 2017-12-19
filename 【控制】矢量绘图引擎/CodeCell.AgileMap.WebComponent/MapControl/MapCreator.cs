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
using CodeCell.AgileMap.WebComponent.AgileMapServiceProxy;
using System.Collections.Generic;
using System.Threading;

namespace CodeCell.AgileMap.WebComponent
{
    internal class MapCreator : IDisposable
    {
        public Map CreateMap(IMapServerAgent agent, Canvas canvas,IMapControl mapcontrol)
        {
            Map map = new Map(string.Empty, canvas,agent,mapcontrol);
            agent.MapServiceClient.GetMapInfoCompleted += new EventHandler<GetMapInfoCompletedEventArgs>(MapServiceClient_GetMapInfoCompleted);
            agent.MapServiceClient.GetLayerInfosCompleted += new EventHandler<GetLayerInfosCompletedEventArgs>(MapServiceClient_GetLayerInfosCompleted);
            agent.MapServiceClient.GetMapInfoAsync(map);
            agent.MapServiceClient.GetLayerInfosAsync(map);
            return map;
        }

        void MapServiceClient_GetLayerInfosCompleted(object sender, GetLayerInfosCompletedEventArgs e)
        {
            Map map = e.UserState as Map;
            ServiceFeatureLayer[] fetLyrs = GetServiceFeatureLayers(e.Result);
            if (fetLyrs != null && fetLyrs.Length > 0)
            {
                foreach (ServiceFeatureLayer lyr in fetLyrs)
                    map.Layers.Add(lyr);
            }
        }

        private ServiceFeatureLayer[] GetServiceFeatureLayers(System.Collections.ObjectModel.ObservableCollection<LayerInfo> layerInfos)
        {
            if (layerInfos == null || layerInfos.Count == 0)
                return null;
            List<ServiceFeatureLayer> lyrs = new List<ServiceFeatureLayer>();
            foreach (LayerInfo info in layerInfos)
            {
                ServiceFeatureLayer fetLyr = new ServiceFeatureLayer(info.Id);
                fetLyr.Name = info.Name;
                fetLyr.Visible = true;
                lyrs.Add(fetLyr);
            }
            return lyrs.ToArray();
        }

        void MapServiceClient_GetMapInfoCompleted(object sender, GetMapInfoCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                (e.UserState as Map).Name = e.Result.Name;
                (e.UserState as Map).SpatialRef = e.Result.SpatialRef;
            }
        }

        public void Dispose()
        {
        }
    }
}
