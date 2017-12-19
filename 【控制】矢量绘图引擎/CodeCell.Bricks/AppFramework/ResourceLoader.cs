using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace CodeCell.Bricks.AppFramework
{
    public class ResourceLoader:IDisposable
    {
        private static Assembly _assembly = null;
        private static string ResourceDir = null;// "IECAS.MAS.Framework.ZResources.";

        public ResourceLoader(string assemblyUrl,string resourceFullPath)
        {
            _assembly = Assembly.Load(assemblyUrl);
            ResourceDir = resourceFullPath;
        }

        private ResourceLoader()
        {
        }

        public static Bitmap GetBitmap(string FileName)
        {
            Stream st = _assembly.GetManifestResourceStream(ResourceDir + FileName);
            if (st == null)
                return new Bitmap(16, 16);
            return new Bitmap(st);
        }

        public static Cursor GetCursor(string FileName)
        {
            Stream st = _assembly.GetManifestResourceStream(ResourceDir + FileName);
            if (st == null)
                return Cursors.Default;
            return new Cursor(st);
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_assembly != null)
            {
                _assembly = null;
            }
        }

        #endregion
    }
}
