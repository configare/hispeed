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

namespace CodeCell.AgileMap.WebComponent
{
    internal class QueryStatus
    {
        public IQueryResultContainer QueryResultContainer = null;
        public object QueryTarget = null;

        public QueryStatus(IQueryResultContainer qrc, object queryTarget)
        {
            QueryResultContainer = qrc;
            QueryTarget = queryTarget;
        }
    }

    public static class QueryExecutor
    {
        private static bool IsAttachedEvent = false;

        public static void Query(this FeatureLayer layer, IQueryResultContainer queryResultContainer, string geometry, string keywords)
        {
            if (!IsAttachedEvent)
            {
                layer.Map.ServerAgent.MapServiceClient.QueryCompleted += new EventHandler<AgileMapServiceProxy.QueryCompletedEventArgs>(MapServiceClient_QueryCompleted);
                IsAttachedEvent = true;
            }
            layer.Map.ServerAgent.MapServiceClient.QueryAsync(layer.Id, geometry, keywords, new QueryStatus(queryResultContainer, layer));
        }

        public static void Query(this Map map, IQueryResultContainer queryResultContainer,string geometry, string keywords)
        {
            if (map.Layers == null || map.Layers.Count == 0)
                return;
            if (!IsAttachedEvent)
            {
                map.ServerAgent.MapServiceClient.QueryCompleted += new EventHandler<AgileMapServiceProxy.QueryCompletedEventArgs>(MapServiceClient_QueryCompleted);
                IsAttachedEvent = true;
            }
            //
            foreach (Layer lyr in map.Layers)
            {
                FeatureLayer fetLyr = lyr as FeatureLayer;
                if (fetLyr != null)
                    map.ServerAgent.MapServiceClient.QueryAsync(fetLyr.Id, geometry, keywords, new QueryStatus(queryResultContainer, fetLyr));
            }
        }

        static void MapServiceClient_QueryCompleted(object sender, AgileMapServiceProxy.QueryCompletedEventArgs e)
        {
            QueryStatus qs = e.UserState as QueryStatus;
            if (e.Result != null)
            {
                foreach (FeatureInfo fetInfo in e.Result)
                {
                    //Feature fet = new Feature(fetInfo.OID);
                    //if (fetInfo.Properties != null)
                    //    foreach (string pkey in fetInfo.Properties.Keys)
                    //        fet.Properties.Add(pkey, fetInfo.Properties[pkey]);
                    Feature fet = Feature.FromFeatureInfo(fetInfo);
                    qs.QueryResultContainer.Add(fet);
                }
            }
        }
    }
}
