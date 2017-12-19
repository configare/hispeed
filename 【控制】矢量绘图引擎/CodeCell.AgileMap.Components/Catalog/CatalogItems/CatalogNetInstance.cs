using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;



namespace CodeCell.AgileMap.Components
{
    public class CatalogNetInstance : CatalogItem
    {
        public CatalogNetInstance()
            :base()
        { 
        }

        public CatalogNetInstance(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogNetInstance(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogNetInstance(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogNetInstance.png");
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                LoadDatasetAndFetclass();
                Refresh();
                _isLoaded = true;
            }
        }

        private void LoadDatasetAndFetclass()
        {
            InstanceIdentify id = _tag as InstanceIdentify;
            try
            {
                string url = _parent.Tag.ToString();
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdGetCatalogList);
                args.Add(HttpCommands.cstArgInstanceId, id.Id.ToString());
                url = url + HttpCommands.GetCatalogUrl(HttpCommands.cstCatalogPage, args);
                object[] objs = GetObjectFromHttpStream.GetObject(url) as object[];
                if (objs != null && objs.Length > 0)
                {
                    FetDatasetIdentify[] dsIds = objs[0] as FetDatasetIdentify[];
                    FetClassIdentify[] fetcIds = objs[1] as FetClassIdentify[];
                    //
                    if (dsIds != null && dsIds.Length > 0)
                    {
                        foreach (FetDatasetIdentify did in dsIds)
                        {
                            ICatalogItem cit = new CatalogNetFeatureDataset(did.Name, did.Id, did.Description);
                            AddChild(cit);
                            //
                            if (did.FetClassIds != null && did.FetClassIds.Length > 0)
                            {
                                foreach (FetClassIdentify fetcId in did.FetClassIds)
                                {
                                    ICatalogItem fcit = new CatalogNetFeatureClass(fetcId.Name, fetcId, fetcId.Description);
                                    cit.AddChild(fcit);
                                }
                            }
                        }
                    }
                    //
                    if (fetcIds != null && fetcIds.Length > 0)
                    {
                        foreach (FetClassIdentify fetcId in fetcIds)
                        {
                            ICatalogItem fcit = new CatalogNetFeatureClass(fetcId.Name, fetcId, fetcId.Description);
                            AddChild(fcit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowError(ex.Message);
            }
        }
    }
}
