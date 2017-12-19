using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CodeCell.AgileMap.WebComponent.AgileMapServiceProxy;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace CodeCell.AgileMap.WebComponent
{
    public class MapServerAgent:IMapServerAgent
    {
        private MapServiceClient _mapServiceClient = null;
        private string _url = null;
        private IMapTileProvider _mapImageProvider = null;

        public MapServerAgent(string url)
        {
            _url = url;
            ConfigClient();
            CreateDefaultMapImageProvider();
        }

        public MapServiceClient MapServiceClient
        {
            get { return _mapServiceClient; }
        }

        public IMapTileProvider MapTileProvider
        {
            get { return _mapImageProvider; }
            set { _mapImageProvider = value; }
        }

        public string Url
        {
            get { return _url; }
        }

        public bool TestConnect()
        {
            return true;
        }

        private void ConfigClient()
        {
            Binding binding = new BasicHttpBinding();
            BasicHttpBinding b = binding as BasicHttpBinding;
            //b.MaxReceivedMessageSize = long.MaxValue;
            //b.MaxBufferSize = int.MaxValue;
            EndpointAddress endpoint = new EndpointAddress(_url);
            MapServiceClient proxy = new MapServiceClient(binding, endpoint);
            _mapServiceClient = proxy;
        }

        private void CreateDefaultMapImageProvider()
        {
            _mapImageProvider = new DefaultMapTileProvider(_mapServiceClient);
        }
    }

    internal class DefaultMapTileProvider : IMapTileProvider
    {
        private MapServiceClient _mapServiceClient = null;

        public DefaultMapTileProvider(MapServiceClient mapServiceClient)
        {
            _mapServiceClient = mapServiceClient;
        }

        public string GetMapImageUrl(TileDef tileDef)
        {
            _mapServiceClient.GetMapImageByQuadkeyAsync(tileDef.Quadkey, tileDef.Rect.X, tileDef.Rect.Y, tileDef.Rect.Width, tileDef.Rect.Height, 256, 256);
            return null;
        }
    }
}
