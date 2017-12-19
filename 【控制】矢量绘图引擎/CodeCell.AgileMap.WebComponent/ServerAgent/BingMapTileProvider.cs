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

namespace CodeCell.AgileMap.WebComponent
{
    public enum enumBingMapType
    {
        Road,
        Aeria,
        Mix
    }

    public class BingMapTileProvider:IMapTileProvider
    {
        private string _urlTemplateRoad = "http://r3.tiles.ditu.live.com/tiles/r{0}.png?g=47";
        private string _urlTemplateAiral = "http://t0.tiles.virtualearth.net/tiles/a{0}.png?g=213";
        private string _urlTemplateMixEn = "http://t0.tiles.virtualearth.net/tiles/h{0}.png?g=213";
        private string _urlTemplate = null;

        public BingMapTileProvider(enumBingMapType bingMapType)
        {
            switch (bingMapType)
            {
                case enumBingMapType.Aeria:
                    _urlTemplate = _urlTemplateAiral;
                    break;
                case enumBingMapType.Road:
                    _urlTemplate = _urlTemplateRoad;
                    break;
                case enumBingMapType.Mix:
                    _urlTemplate = _urlTemplateMixEn;
                    break;
            }
        }

        public BingMapTileProvider(string urlTemplate)
        {
            _urlTemplate = urlTemplate;
        }
        //
        public string GetMapImageUrl(TileDef tileDef)
        {
            return string.Format(_urlTemplate, tileDef.Quadkey);
        }
    }
}
