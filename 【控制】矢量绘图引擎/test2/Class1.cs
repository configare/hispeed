using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace test2
{
    class Class1
    {
        public static string GetRelativePath(string refdir, string fname)
        {
            string[] startPathParts = Path.GetFullPath(fname).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] dstPathParts = refdir.Split(Path.DirectorySeparatorChar);
            int sameCounter = 0;
            while ((sameCounter < startPathParts.Length) && (sameCounter < dstPathParts.Length) && startPathParts[sameCounter].Equals(dstPathParts[sameCounter], StringComparison.InvariantCultureIgnoreCase))
            {
                sameCounter++;
            }
            if (sameCounter == 0)
            {
                return fname;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = sameCounter; i < startPathParts.Length; i++)
            {
                sb.Append(".." + Path.DirectorySeparatorChar);
            }
            for (int i = sameCounter; i < dstPathParts.Length; i++)
            {
                sb.Append(dstPathParts[i] + Path.DirectorySeparatorChar);
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
