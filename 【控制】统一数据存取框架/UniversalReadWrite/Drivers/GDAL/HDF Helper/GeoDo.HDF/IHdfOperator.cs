using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.HDF
{
    public interface IHdfOperator:IDisposable
    {
        string GetAttributeValue(string attributeName);
        string GetAttributeValue(string datasetName, string attributeName);
        string[] GetDatasetNames { get; }
        Dictionary<string, string> GetAttributes();
        Dictionary<string, string> GetAttributes(string datasetName);
    }
}
