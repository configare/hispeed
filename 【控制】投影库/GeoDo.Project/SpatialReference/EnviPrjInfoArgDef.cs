using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public class EnviPrjInfoArgDef
    {
        public int PrjId = 0;
        public string PrjName = null;
        public string[] Args = null;

        public EnviPrjInfoArgDef(int prjId, string prjName, string args)
        {
            PrjId = prjId;
            PrjName = prjName;
            if (args != null)
               Args = args.Split(',');
        }
    }
}
