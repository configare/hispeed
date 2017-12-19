using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public static class GetObjectFromHttpStream
    {
        public static object GetObject(string url)
        {
            return GetObject(url, false);
        }

        public static object GetObjectZip(string url)
        {
            return GetObject(url, true);
        }

        public static object GetObject(string url,bool useZip)
        {
            Uri uri = new Uri(url);
            HttpWebRequest req = HttpWebRequest.CreateDefault(uri) as HttpWebRequest;
            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                bool isError = resp.StatusDescription != null && resp.StatusDescription == "ERROR";
                using (BinaryReader stReader = new BinaryReader(resp.GetResponseStream()))
                {
                    using (MemoryStream mstream = new MemoryStream())
                    {
                        {
                            byte[] buffer = new byte[256];
                            int count = stReader.Read(buffer, 0, 256);
                            while (count > 0)
                            {
                                mstream.Write(buffer, 0, count);
                                count = stReader.Read(buffer, 0, 256);
                            }
                        }
                        mstream.Position = 0;
                        if (useZip && !isError)
                            return PersistObject.StreamToObjectWithZIP(mstream);
                        else
                        {
                            return PersistObject.StreamToObject(mstream);
                        }
                    }
                }
            }
        }
    }
}
