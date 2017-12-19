using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public class ImporterByCopyFile:IFetClassImporter,IDisposable
    {
        public ImporterByCopyFile()
        { 
        }

        #region IFetClassImporter Members

        public void Import(ICatalogItem fetClassItem, ICatalogItem locationItem, IProgressTracker tracker,string name,string displayName,string description)
        {
            CatalogFile cfile = fetClassItem as CatalogFile;
            CatalogLocal cloc = locationItem as CatalogLocal;
            string file = cfile.Tag.ToString();
            string dstdir = cloc.Tag.ToString();
            CopyFile(file, name,displayName, dstdir,tracker);
        }

        private void CopyFile(string file, string name,string displayName,string dstdir,IProgressTracker tracker)
        {
            if (file.ToUpper().EndsWith(".SHP"))
                CopyShapeFiles(file, name,dstdir,tracker);
            else
                throw new NotSupportedException("暂不支持类型为\""+Path.GetExtension(file)+"\"的文件数据源。");
        }

        private void CopyShapeFiles(string file,string name, string dstdir, IProgressTracker tracker)
        {
            string[] exts = new string[] { ".dbf",".shp",".prj",".shx",".sbn",".shp.xml",".sidx"};
            if (tracker != null)
                tracker.StartTracking("开始拷贝文件\"" + Path.GetFileNameWithoutExtension(file) + "\"...", 7);
            int i = 0;
            foreach (string ext in exts)
            {
                string f = Path.ChangeExtension(file, ext);
                if (tracker != null)
                    tracker.Tracking("正在拷贝文件\"" + Path.GetFileName(f), i);
                if(File.Exists(f))
                    File.Copy(f, Path.Combine(dstdir, name + ext));
                i++;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
