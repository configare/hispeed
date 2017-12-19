using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;

namespace GeoVis.GeoCore
{
    public class GGeoImageInfo
    {
        public LCompressFormat outFormat;

        public int MinLevel;
        public int MaxLevel;
        public int MinRow;
        public int MaxRow;
        public int MinCol;
        public int MaxCol;

        public GeoRegion Region;

    }

    class GGeoImage : IGeoImage
    {
        public LCompressFormat outFormat;

        public int MinLevel;
        public int MaxLevel;

        public int MinRow;
        public int MaxRow;
        public int MinCol;
        public int MaxCol;

        //private GeoRegion _region;
        private FileStream fs;
        private BinaryReader br;
        protected GeoRegion _region;
        private GGeoImageInfo _info;

        public GGeoImage()
        {
        }

        public virtual string Open(string path)
        {
            try
            {
                fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);

                MinLevel = br.ReadInt32();
                MaxLevel = br.ReadInt32();

                MinRow = br.ReadInt32();
                MaxRow = br.ReadInt32();
                MinCol = br.ReadInt32();
                MaxCol = br.ReadInt32();
                outFormat = (LCompressFormat)(br.ReadInt32());
                _region = new GeoRegion();
                _region.MinLon = br.ReadDouble();
                _region.MaxLon = br.ReadDouble();
                _region.MinLat = br.ReadDouble();
                _region.MaxLat = br.ReadDouble();
                this.CFormat = outFormat;
                int dt = br.ReadInt32();

                DataType = (LDataType)dt;

                _bitsPerPixel = br.ReadInt32() * 8;
                CurGeoRegion = _region;

                MinX = CurGeoRegion.MinLon;
                MaxX = CurGeoRegion.MaxLon;
                MinY = CurGeoRegion.MinLat;
                MaxY = CurGeoRegion.MaxLat;
                _info = new GGeoImageInfo();
                _info.MaxCol = MaxCol;
                _info.MinCol = MinCol;
                _info.MaxRow = MaxRow;
                _info.MinRow = MinRow;

                _info.MaxLevel = MaxLevel;
                _info.MinLevel = MinLevel;
                _info.outFormat = outFormat;
                _info.Region = CurGeoRegion;
                this.Tag = _info;
                //ResolutionX = 
            }
            catch
            {
                return "wrong";
            }

            return "";

        }

        public override unsafe int ReadTile(int band, int level, int row, int col, byte[] buffer, int offset, LCompressFormat outFormat)
        {
            if (buffer == null || offset < 0)
                return -1;
            int maxlevel = MaxLevel;
            int minlevel = MinLevel, minrow = MinRow, maxrow = MaxRow, mincol = MinCol, maxcol = MaxCol;
            int size = buffer.Length;

            for (int i = 0; i < size; i++)
                buffer[i] = 0;

            long bandoffset = 0;
            if (level > MaxLevel || level < minlevel)
                return 0;

            for (int k = maxlevel; k > level; k--)
            {
                double tileSize = (double)180 / (1 << k);

                minrow = MathEngine.GetRowFromLatitude(_region.MinLat, tileSize);
                maxrow = MathEngine.GetRowFromLatitude(_region.MaxLat, tileSize);
                mincol = MathEngine.GetColFromLongitude(_region.MinLon, tileSize);
                maxcol = MathEngine.GetColFromLongitude(_region.MaxLon, tileSize);

                bandoffset += (maxrow - minrow + 1) * (maxcol - mincol + 1) * 12;
            }

            double tilesize = (double)180 / (1 << level);
            minrow = MathEngine.GetRowFromLatitude(_region.MinLat, tilesize);
            maxrow = MathEngine.GetRowFromLatitude(_region.MaxLat, tilesize);
            mincol = MathEngine.GetColFromLongitude(_region.MinLon, tilesize);
            maxcol = MathEngine.GetColFromLongitude(_region.MaxLon, tilesize);
            if (row > maxrow || row < minrow || col > maxcol || col < mincol)
                return 0;

            br.BaseStream.Seek(1024 + bandoffset + ((row - minrow) * (maxcol - mincol + 1) + col - mincol) * 12, 0);
            long pos = br.ReadInt64();
            int posoffset = br.ReadInt32();

            if (posoffset <= 0 || br.BaseStream.Length < pos)
                return 0;

            br.BaseStream.Seek(pos, 0);
            br.Read(buffer, offset, posoffset);

            return posoffset;

        }

        public override unsafe bool WriteTile(int band, int level, int row, int col, byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }

    class HttpGGeoImage : GGeoImage
    {
        private string filepath;

        public override string Open(string path)
        {
            filepath = path;
            string url = "";
            url = System.Configuration.ConfigurationManager.AppSettings["URL"];
            url = string.Format("http://{0}/ImageServer/GetImageInfo.ashx?name={1}", url, filepath);

            GGeoImageInfo info = new GGeoImageInfo();
            try
            {
                GetInfo(url, out info);
            }
            catch (Exception)
            {
                // throw "error";
                return "wrong";
            }
            MinLevel = info.MinLevel;
            MaxLevel = info.MaxLevel;

            MinRow = info.MinRow;
            MaxRow = info.MaxRow;
            MinCol = info.MinCol;
            MaxCol = info.MaxCol;
            outFormat = info.outFormat;
            _region = info.Region;

            this.CFormat = outFormat;

            CurGeoRegion = _region;
            return "";
        }


        private void GetInfo(string url, out GGeoImageInfo info)
        {
            HttpWebRequest r = HttpWebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse result = r.GetResponse() as HttpWebResponse;
            System.IO.Stream mem = result.GetResponseStream();

            XmlDocument doc = new XmlDocument();

            doc.Load(mem);

            XmlElement elm = doc.DocumentElement;
            info = new GGeoImageInfo();
            info.MinLevel = Convert.ToInt32(elm["MinLevel"].InnerText);
            info.MaxLevel = Convert.ToInt32(elm["MaxLevel"].InnerText);
            info.MinRow = Convert.ToInt32(elm["MinRow"].InnerText);
            info.MaxRow = Convert.ToInt32(elm["MaxRow"].InnerText);
            info.MinCol = Convert.ToInt32(elm["MinCol"].InnerText);
            info.MaxCol = Convert.ToInt32(elm["MaxCol"].InnerText);

            info.outFormat = (LCompressFormat)Convert.ToInt32(elm["Format"].InnerText);

            info.Region.MinLon = Convert.ToDouble(elm["MinLon"].InnerText);
            info.Region.MaxLon = Convert.ToDouble(elm["MaxLon"].InnerText);
            info.Region.MinLat = Convert.ToDouble(elm["MinLat"].InnerText);
            info.Region.MaxLat = Convert.ToDouble(elm["MaxLat"].InnerText);

        }

        public override unsafe int ReadTile(int band, int level
            , int row, int col, byte[] buffer, int offset, LCompressFormat outFormat)
        {
            string url = "";
            url = System.Configuration.ConfigurationManager.AppSettings["URL"];
            url = string.Format("http://{0}/ImageServer/ImageServer.ashx?name={1}&l={2}&r={3}&c={4}&o=1", url, filepath,
                level, row, col);

            HttpWebRequest r = HttpWebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse result = r.GetResponse() as HttpWebResponse;
            System.IO.Stream mem = result.GetResponseStream();

            MemoryStream ms = new MemoryStream(buffer);
            byte[] _bytes = new byte[4096];
            int _i = 0;
            int len = 0;
            do
            {
                _i = mem.Read(_bytes, 0, 4096);

                ms.Write(_bytes, 0, _i);
                len += _i;

            } while (_i > 0);

            return len;
        }
    }

    
}
