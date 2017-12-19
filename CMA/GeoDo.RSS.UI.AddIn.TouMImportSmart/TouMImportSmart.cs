using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;
using GeoDo.Project;
using System.Xml.Linq;

namespace GeoDo.RSS.UI.AddIn.TouMImportSmart
{
    /// <summary>
    /// 雾霾数据转换，将FY-3A、FOU的txt数据转为dat。
    /// Class1 s = new Class1();
    /// s.ConvertTextToDat(args[0]);
    /// </summary>
    public class FY3TouImportSMART
    {
        private IRasterDataProvider _outputDataProvider;
        private IRasterBand _outputRasterBand;
        private Action<int, string> _prograssTracker = null;
        private int _width = -1;
        private int _height = -1;
        private float _zoom = -1;
        private float _xResolution = -1f;
        private float _yResolution = -1f;
        private CoordEnvelope _coordEnvelope = null;
        private ISpatialReference _spatialRef = SpatialReference.GetDefault();
        private float _inVaildValue = 0f;

        public FY3TouImportSMART(string configerFile, Action<string, int> prograssTracker)
        {
            InitEvnByConfiger(configerFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orbitSize">原数据大小</param>
        /// <param name="orbitEnvelope">原数据经纬度范围</param>
        /// <param name="zoom">结果放大倍数</param>
        /// <param name="prograssTracker">进度条</param>
        public FY3TouImportSMART(Size orbitSize, CoordEnvelope orbitEnvelope, float zoom, float invaildValue, Action<int, string> prograssTracker)
        {
            _prograssTracker = prograssTracker;
            _width = orbitSize.Width;
            _height = orbitSize.Height;
            _coordEnvelope = orbitEnvelope;
            _zoom = zoom;
            _inVaildValue = invaildValue;
        }

        private void InitEvnByConfiger(string configerFile)
        {
            if (string.IsNullOrEmpty(configerFile) || !File.Exists(configerFile))
                return;
            XElement root = XElement.Load(configerFile);
            if (root == null)
                return;
            XElement FogTou = root.Element("FOGTxtImportSMART");
            if (FogTou == null)
                return;
            _zoom = GetIntAtrribut(FogTou, "zoom");
            _width = GetIntAtrribut(FogTou, "Size", "width");
            _height = GetIntAtrribut(FogTou, "Size", "height");
            float minX = GetFloatAtrribut(FogTou, "Envelope", "minX");
            float maxX = GetFloatAtrribut(FogTou, "Envelope", "maxX");
            float minY = GetFloatAtrribut(FogTou, "Envelope", "minY");
            float maxY = GetFloatAtrribut(FogTou, "Envelope", "maxY");
            if (minX == float.MinValue || maxX == float.MinValue || minY == float.MinValue || maxY == float.MinValue)
                _coordEnvelope = null;
            else
                _coordEnvelope = new CoordEnvelope(minX, maxX, minY, maxY);
        }

        private int GetIntAtrribut(XElement node, string atrributName)
        {
            int result = -1;
            string atrrValue = node.Attribute(atrributName).Value;
            if (string.IsNullOrEmpty(atrrValue))
                return result;
            int.TryParse(atrrValue, out result);
            return result;
        }

        private int GetIntAtrribut(XElement pNode, string sNodeName, string atrributName)
        {
            int result = -1;
            XElement sNode = pNode.Element(sNodeName);
            if (sNode == null)
                return result;
            string atrrValue = sNode.Attribute(atrributName).Value;
            if (string.IsNullOrEmpty(atrrValue))
                return result;
            int.TryParse(atrrValue, out result);
            return result;
        }

        private float GetFloatAtrribut(XElement pNode, string sNodeName, string atrributName)
        {
            float result = float.MinValue;
            XElement sNode = pNode.Element(sNodeName);
            if (sNode == null)
                return result;
            string atrrValue = sNode.Attribute(atrributName).Value;
            if (string.IsNullOrEmpty(atrrValue))
                return result;
            float.TryParse(atrrValue, out result);
            return result;
        }

        public bool ConvertTextToDat(string touTxtFile, string dstname, out string error)
        {
            return ConvertTextToDat(touTxtFile, dstname, _coordEnvelope, out error);
        }

        public bool ConvertTextToDat(string touTxtFile, string dstname, CoordEnvelope outEnvelope, out string error)
        {
            error = null;
            try
            {
                if (!CheckEnv(touTxtFile, outEnvelope, out error))
                    return false;
                _xResolution = (float)_coordEnvelope.Height / _width;
                _yResolution = (float)_coordEnvelope.Width / _height;
                IEnumerable<string> lines = File.ReadAllLines(touTxtFile);
                int lineCount = lines.Count();
                List<Int16[]> vData = new List<Int16[]>();
                for (int i = 0; i < lineCount; i++)
                {
                    string lineString = lines.ElementAt(i);
                    if (string.IsNullOrWhiteSpace(lineString))
                        continue;
                    string[] lineDataStrings = lineString.Split(' ');
                    Int16[] lineDat = GetLineData(lineDataStrings);
                    vData.Add(lineDat);

                }
                int startRow, endRow, startCol, endCol;
                if (!GetOutRowCol(ref outEnvelope, out startRow, out endRow, out startCol, out endCol))
                {
                    error = "TouTxtImportSMART错误:设定的输出范围超出原始数据范围";
                    return false;
                }
                BuildInternalBuffer(dstname, outEnvelope, endRow - startRow, endCol - startCol);
                Int16[][] datas = vData.ToArray();
                Int16[] lineData = null;
                int writeLine = 0;
                int wirteCol = 0;
                for (int col = endCol - 1; col >= startCol; col--)
                {
                    lineData = new Int16[endRow - startRow];
                    for (int row = startRow; row < endRow; row++)
                    {
                        lineData[wirteCol++] = datas[row][col];
                    }
                    WriteLine<Int16>(_outputRasterBand, writeLine++, lineData, endRow - startRow);
                    wirteCol = 0;
                }
            }
            finally
            {
                Dispose();
            }
            return true;
        }

        private bool GetOutRowCol(ref CoordEnvelope outEnvelope, out int startRow, out int endRow, out int startCol, out int endCol)
        {
            startRow = startCol = 0;
            endRow = _height;
            endCol = _width;
            if (outEnvelope.MinX == _coordEnvelope.MinX && outEnvelope.MaxX == _coordEnvelope.MaxX &&
                outEnvelope.MinY == _coordEnvelope.MinY && outEnvelope.MaxY == _coordEnvelope.MaxY)
                return true;
            float resolution = _yResolution > _xResolution ? _yResolution : _xResolution;
            int yUpdate = _width - (int)((_coordEnvelope.MaxY - _coordEnvelope.MinY) / resolution);
            int xUpdate = _height - (int)((_coordEnvelope.MaxX - _coordEnvelope.MinX) / resolution);
            startCol = (int)((outEnvelope.MinY - _coordEnvelope.MinY) / resolution) + yUpdate;
            endCol = (int)((outEnvelope.MaxY - _coordEnvelope.MinY) / resolution) + yUpdate;
            startRow = (int)((outEnvelope.MinX - _coordEnvelope.MinX) / resolution) + xUpdate;
            endRow = (int)((outEnvelope.MaxX - _coordEnvelope.MinX) / resolution) + xUpdate;
            if (startCol < 0 || endCol < 0 || startRow < 0 || endRow < 0)
                return false;
            outEnvelope = new CoordEnvelope(_coordEnvelope.MinX + resolution * (startRow + (xUpdate + 1) / 2f), _coordEnvelope.MinX + resolution * (endRow + (xUpdate + 1) / 2f),
                _coordEnvelope.MinY + resolution * (startCol - (yUpdate + 1) / 2f), _coordEnvelope.MinY + resolution * (endCol - (yUpdate + 1) / 2f));
            return true;
        }

        private bool CheckEnv(string touTxtFile, CoordEnvelope outEnvelope, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(touTxtFile) || !File.Exists(touTxtFile))
            {
                error = "TouTxtImportSMART错误:待转换的文件不存在![" + touTxtFile + "]";
                return false;
            }
            if (outEnvelope == null || _width == -1 || _height == -1)
            {
                error = "TouTxtImportSMART错误:待转换的文件中的输出范围为空或原始文件大小未设定";
                return false;
            }
            return true;
        }

        private Int16[] GetLineData(string[] lineDataStrings)
        {
            List<Int16> datas = new List<Int16>();
            foreach (string dataStr in lineDataStrings)
            {
                if (string.IsNullOrWhiteSpace(dataStr))
                    continue;
                datas.Add(ToShort(dataStr));
            }
            return datas.ToArray();
        }

        private Int16 ToShort(string dataStr)
        {
            float va = 0;
            float.TryParse(dataStr, out va);
            if (va == _inVaildValue)
                return 0;
            return (Int16)(va * _zoom);
        }

        private void BuildInternalBuffer(string fileName, CoordEnvelope outputEnvelope, int width, int height)
        {
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            _outputDataProvider = drv.Create(fileName, width, height, 1, enumDataType.Int16, GetOptions(outputEnvelope, width, height));
            _outputRasterBand = _outputDataProvider.GetRasterBand(1);
        }

        private void WriteLine<T>(IRasterBand outputRasterBand, int line, T[] lineData, int width)
        {
            GCHandle handle = GCHandle.Alloc(lineData, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                outputRasterBand.Write(0, line, width, 1, ptr, enumDataType.Int16, width, 1);
            }
            finally
            {
                handle.Free();
            }
        }

        private object[] GetOptions(CoordEnvelope outputEnvelope, int width, int height)
        {
            List<string> ops = new List<string>();
            if (_coordEnvelope != null)
                ops.Add(outputEnvelope.ToMapInfoString(new Size(width, height)));
            if (_spatialRef != null)
            {
                try
                {
                    string spref = _spatialRef.ToProj4String();
                    ops.Add("SPATIALREF=" + spref);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            return ops.Count > 0 ? ops.ToArray() : null;
        }

        public void Dispose()
        {
            if (_outputDataProvider != null)
                _outputDataProvider.Dispose();
            if (_outputRasterBand != null)
                _outputRasterBand.Dispose();
        }
    }
}
