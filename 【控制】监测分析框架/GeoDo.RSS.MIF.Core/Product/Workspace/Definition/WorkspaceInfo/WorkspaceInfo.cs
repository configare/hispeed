using System;

namespace GeoDo.RSS.MIF.Core
{
    public class WorkspaceInfo
    {
        public SDatinfos Datinfos { get; set; }
        public DateTime SaveTime { get; set; }

        public CatalogItem GetBySourceFileName(string sourceFileName)
        {
            var datinfo = Datinfos.GetBySourceFileName(sourceFileName);
            return datinfo == null ? null : new CatalogItem(datinfo);
        }

        public bool ValidateFileExists()
        {
            return Datinfos.ValidateFileExists();
        }
    }
}