using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.DF.GeoEye
{
    /// <summary>
    /// args[0] == "ComponentID=000000"
    /// </summary>
    [Export(typeof(IGeoDataDriver)), ExportMetadata("VERSION", "1")]
    public class GeoEyeDriver : RasterDataDriver
    {
        protected override IGeoDataProvider BuildDataProvider(string fileName, byte[] header1024, enumDataProviderAccess access, params object[] args)
        {
            string id;
            string[] fnames;
            if (!IsOK(fileName, args, out id, out fnames))
                return null;
            foreach (string f in fnames)
                if (!File.Exists(f))
                    throw new FileNotFoundException(f);
            return new LogicalRasterDataProvider(fileName, fnames, "ComponentID=" + id);
        }

        protected override bool IsCompatible(string fileName, byte[] header1024, params object[] args)
        {
            string id;
            string[] fnames;
            if (IsOK(fileName, args, out id, out fnames))
                return true;
            return false;
        }

        private bool IsOK(string fileName, object[] args, out string id, out string[] fnames)
        {
            id = null;
            fnames = null;
            string extName = Path.GetExtension(fileName).ToUpper();
            if (extName != ".TXT")
                return false;
            if (args == null || args.Length == 0)
                return false;
            if (!args[0].ToString().Contains("ComponentID="))
                return false;
            string[] lines = File.ReadAllLines(fileName);
            if (lines.Length == 0)
                return false;
            string cid = args[0].ToString().Split('=')[1].Trim(); //ComponentID = 000000
            id = null;
            int lnIdx = 0;
            foreach (string ln in lines)
            {
                if (ln.Contains("Component ID:"))
                {
                    id = ln.Split(':')[1].Trim();
                    if (id == cid)
                    {
                        break;
                    }
                }
                lnIdx++;
            }
            if (cid == id)
            {
                lnIdx += 2;
                if (!lines[lnIdx].Contains("Component File Name:"))
                    return false;
                string f = lines[lnIdx].Split(':')[1].Trim();
                fnames = f.Split(' ');
                List<string> fs = new List<string>();
                foreach (string s in fnames)
                    if (!s.ToUpper().Contains("_PAN_"))
                        fs.Add(Path.Combine(Path.GetDirectoryName(fileName) ,s));
                fnames = fs.ToArray();
                return true;
            }
            return false;
        }


        public override void Delete(string fileName)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options)
        {
            throw new NotImplementedException();
        }

        public override IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options)
        {
            throw new NotImplementedException();
        }

        protected override IGeoDataProvider BuildDataProvider(string fileName,enumDataProviderAccess access, params object[] args)
        {
            string id;
            string[] fnames;
            if (!IsOK(fileName, args, out id, out fnames))
                return null;
            return new LogicalRasterDataProvider(fileName, fnames, "ComponentID=" + id);
        }
    }
}
