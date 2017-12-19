using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal interface IFeaturesReaderService:IDisposable
    {
        void BeginRead();
        void EndRead();
        Feature[] Read(Envelope envelope);
    }
}
