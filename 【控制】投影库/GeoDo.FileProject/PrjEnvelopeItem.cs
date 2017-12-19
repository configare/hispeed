using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RasterProject;

namespace GeoDo.FileProject
{
    public class PrjEnvelopeItem
    {
        private string _name;
        private string _identify;
        private PrjEnvelope _prjEnvelope;

        public PrjEnvelopeItem(string name, PrjEnvelope prjEnvelope)
        {
            _name = name;
            _prjEnvelope = prjEnvelope;
        }

        public PrjEnvelopeItem(string name, PrjEnvelope prjEnvelope, string identify)
            : this(name, prjEnvelope)
        {
            _identify = identify;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public PrjEnvelope PrjEnvelope
        {
            get { return _prjEnvelope; }
            set { _prjEnvelope = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Name, Identify);
        }
    }
}
