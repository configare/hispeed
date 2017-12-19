using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace GeoDo.RSS.Core.CA
{
    public delegate void OnStackChangedHandler(object sender,bool processStackIsEmpty,bool unProcessStackIsEmpty);

    public interface IRgbProcessorStack
    {
        int Count { get; }
        IEnumerable<IRgbProcessor> Processors { get; }
        OnStackChangedHandler OnStackChanged { get; set; }
        void Process(IRgbProcessor processor);
        void ReProcess();
        void UnProcess();
        void RemoveLast();
        void Apply(int[][] indexesOfAOIs,Bitmap bitmap);
        void Clear();
        void SaveTo(string filename);
        XmlElement ToXML();
        void ReadXmlElement(string filename);
    }
}
