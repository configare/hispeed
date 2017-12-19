using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMap:IDisposable
    {
        string Name { get; set; }
        string Url { get; }
        Envelope GetFullEnvelope();
        MapVersion Version { get; }
        MapAuthor Author { get; }
        MapArguments MapArguments { get; }
        ConflictDefinition ConflictDefForSymbol { get; }
        ConflictDefinition ConflictDefForLabel { get; }
        ILayerContainer LayerContainer { get; }
        IMapRefresh MapRefresh { get; }
        void SaveTo(string filename, bool useRelativePath);
    }
}
