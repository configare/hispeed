using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class AOIDef
    {
        public string Name;
        public string Provider;
        public AOIDef[] SubAOIs;

        public AOIDef()
        { 
        }

        public AOIDef(string name, string provider)
        {
            Name = name;
            Provider = provider;
        }

        public AOIDef(string name, string provider, AOIDef[] subAOIs)
        {
            Name = name;
            Provider = provider;
            SubAOIs = subAOIs;
        }
    }
}
