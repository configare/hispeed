using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using CodeCell.Bricks.Runtime;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public class GeoVISIdxBuilder
    {
        private List<GrstKey> _grstDatas = null;
        private int _bands;
        private enumLDataType _dataType;
        private enumLFormat outFormat;

        public GeoVISIdxBuilder()
        {
            _grstDatas = new List<GrstKey>();
            string[] glPaths = null;
            string[] gePaths = null;
            string glIdxPath = null;
            string geIdxPath = null;
            GetArgumentFromConfig(out glPaths, out gePaths, out glIdxPath, out geIdxPath);
            try
            {
                BuildIdxs(glPaths, glIdxPath);
                BuildIdxs(gePaths, geIdxPath);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                throw new Exception("数字地球数据索引创建不成功。", ex);
            }
        }

        private void BuildIdxs(string[] glPaths, string glIdxPath)
        {
            if (BuildDataIdx(glPaths))
                WriteTableToFile(glIdxPath);
        }

        private bool BuildDataIdx(string[] fileNames)
        {
            foreach (string fname in fileNames)
            {
                if (!File.Exists(fname))
                    return false;
            }
            _grstDatas.Clear();
            if (fileNames.Length > 0)
                Open(fileNames[0]);
            string extention = string.Empty;
            string name = string.Empty;
            string[] infos = null;
            int level = int.MinValue;
            int row = 0, col = 0;
            long key = 0;
            foreach (string file in fileNames)
            {
                extention = Path.GetExtension(file);
                if (extention != ".grst")
                    continue;
                name = Path.GetFileNameWithoutExtension(file);
                infos = name.Split(new char[] { '_' });
                level = GetLevel(infos[0]);
                row = Convert.ToInt32(infos[1]);
                col = Convert.ToInt32(infos[2]);
                key = GrstKey.CalKey(level, row, col);

                _grstDatas.Add(new GrstKey(key, file));
            }
            return true;
        }

        private int GetLevel(string info)
        {
            int level = Convert.ToInt32(info);
            switch (level)
            {
                case 1:
                    return 0;
                case 2:
                    return 2;
                case 3:
                    return 6;
                case 4:
                    return 10;
                case 5:
                    return 13;
                default:
                    return level;
            }
        }

        private void Open(string fileName)
        {
            try
            {
                FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(fs);

                int MinLevel = br.ReadInt32();
                int MaxLevel = br.ReadInt32();

                br.ReadDouble();
                br.ReadDouble();
                br.ReadDouble();
                br.ReadDouble();

                _bands = br.ReadInt32();
                int dt = br.ReadInt32();
                _dataType = (enumLDataType)dt;
                outFormat = (enumLFormat)br.ReadInt32();
                enumLDataType dataType = (enumLDataType)dt;
            }
            catch
            {
                throw;
            }
        }

        private void WriteTableToFile(string indexFilePath)
        {
            if (string.IsNullOrEmpty(indexFilePath))
                return;
            string dir = Path.GetDirectoryName(indexFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (FileStream fs = File.Create(indexFilePath))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(_bands);
                    bw.Write((int)_dataType);
                    bw.Write((int)outFormat);

                    foreach (GrstKey gk in _grstDatas)
                    {
                        long key = gk.Key;
                        bw.Write(key);
                        string path = (gk).Path;
                        bw.Write(path);
                        DateTime dt = DateTime.Now;
                        bw.Write(dt.ToString());
                    }
                }
            }
        }

        private void GetArgumentFromConfig(out string[] glPaths, out string[] gePaths, out string glIdxPath, out string geIdxPath)
        {
            glPaths = null;
            gePaths = null;
            glIdxPath = null;
            geIdxPath = null;
            string pathLong = ConfigurationManager.AppSettings["GlobalImageDataPath"];
            if (!string.IsNullOrEmpty(pathLong))
                glPaths = pathLong.Split(';');
            pathLong = ConfigurationManager.AppSettings["GeoTerrainDataPath"];
            if (!string.IsNullOrEmpty(pathLong))
                gePaths = pathLong.Split(';');
            glIdxPath = ConfigurationManager.AppSettings["GlobalImageURL"];
            geIdxPath = ConfigurationManager.AppSettings["GeoTerrainPath"];
        }
    }
}
