using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IPrjStdsMapTableParser : IDisposable
    {
        EnviPrjInfoArgDef[] EnviPrjInfoArgDefs { get; }
        EnviPrjInfoArgDef GetEnviPrjInfoArgDef(int prjId);
        Dictionary<string, string> GetDatumsItems();
        List<NameMapItem> GetPrjParameterItems();
        List<NameMapItem> GetPrjNameMapItems();
        List<CoordinateDomain> GetCoordinateDomainItems();
        string GetDatumNameByPrj4DatumName(string prj4Name);
        NameMapItem GetPrjNameItemByPrj4(string name);
        NameMapItem GetPrjNameItemByName(string name);
        NameMapItem GetPrjNameItemByWktName(string name);
        NameMapItem GetPrjNameItemByEsriName(string name);
        NameMapItem GetPrjNameItemByEPSGName(string name);
        NameMapItem GetPrjNameItemByGeoTiffName(string name);
        NameMapItem GetPrjNameItemByEnviName(string name);
        NameMapItem GetPrjParamterItemByPrj4(string name);
        NameMapItem GetPrjParamterItemByName(string name);
        NameMapItem GetPrjParamterItemByWktName(string name);
        NameMapItem GetPrjParamterItemByEsriName(string name);
        NameMapItem GetPrjParamterItemByEPSGName(string name);
        NameMapItem GetPrjParamterItemByGeoTiffName(string name);
    }
}
