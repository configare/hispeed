using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoDo.RSS.MIF.Core
{
    public class SDatinfos : List<SDatinfo>
    {
        public void AddDatinfo(SDatinfo datinfo)
        {
            if(!IsContains(datinfo))
                Add(datinfo);
        }

        public bool IsContains(SDatinfo datinfo)
        {
            var name = datinfo.SourceFileName;
            return this.Any(sDatinfo => sDatinfo.SourceFileName == name);
        }

        public SDatinfo GetBySourceFileName(string sourceFileName)
        {
            //return this.FirstOrDefault(datinfo => datinfo.SourceFileName == sourceFileName);
            foreach (SDatinfo datinfo in this)
            {
                if (datinfo.SourceFileName == sourceFileName)
                    return datinfo;
            }
            return null;
        }

        public bool ValidateFileExists()
        {
            bool isChange = false;
            for (var i = Count-1; i >=0; i--)
            {
                var cur = this[i];
                var file = cur.SourceFileName;
                if (!File.Exists(file))
                {
                    RemoveAt(i);
                    isChange = true;
                }
            }
            return isChange;
        }
    }
}