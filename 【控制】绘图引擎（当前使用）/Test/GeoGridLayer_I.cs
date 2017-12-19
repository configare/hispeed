using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoDo.RSS.Core.DrawEngine;

namespace Test
{
    public class GeoGridLayer_I:ILayer
    {
        public XElement ToXml()
        {
            return null;
        }

        public static GeoGridLayer_I FromXml(XElement ele)
        {
            return null;
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Alias
        {
            get { throw new NotImplementedException(); }
        }

        public bool Enabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
