using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public delegate void OnAddLayerHandler(object sender,ILayer featureLayer);
    public delegate void OnRemoveLayerHandler(object sender,ILayer featureLayer);

    public interface ILayerContainer:IDisposable
    {
        bool LayerIsChanged { get; }
        bool IsEmpty();
        ILayer[] Layers { get; }
        IFeatureLayer[] FeatureLayers { get; }
        void SetSelectableLayers(string[] layerNames);
        bool IsSelectable(string layerName);
        void Insert(int index, ILayer featureLayer);
        void Append(ILayer featureLayer);
        void Remove(ILayer featureLayer);
        void RemoveAt(int index);
        void Clear(bool needDisposeLayers);
        void AdjustOrder(ILayer mouseStartLayer, ILayer mouseStopLayer);
        OnAddLayerHandler OnAddFeatureLayer { get; set; }
        OnRemoveLayerHandler OnRemoveFeatureLayer { get; set; }
        ILayer GetLayerByName(string name);
        ILayer GetLayerById(string id);
    }
}
