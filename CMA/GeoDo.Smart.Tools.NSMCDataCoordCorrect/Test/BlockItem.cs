using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    public class BlockItem
    {
        private string _name = null;
        private float _leftUpX = 0;
        private float _leftUpY = 0;
        private float _spanX = 0;  //跨度,非宽度
        private float _spanY = 0;
        private string _tag = null;
        private string _orbitFile = null;
        private string _quickImageFile = null;
        private string _blockIdentity = null;
        private PrjEnvelope _envelope = null;
        private masBlockTypes _blockTypes = masBlockTypes.DXX;

        public BlockItem(string name, float leftUpX, float leftUpY, float span)
        {
            
            _name = name;
            _leftUpX = leftUpX;
            _leftUpY = leftUpY;
            _spanX = GetSpan(span);
            _spanY = GetSpan(span);
        }

        public BlockItem(string name, float leftUpX, float leftUpY, float spanX, float spanY)
        {
            _name = name;
            _leftUpX = leftUpX;
            _leftUpY = leftUpY;
            _spanX = GetSpan(spanX);
            _spanY = GetSpan(spanY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="centerXresolution"></param>
        /// <param name="centerYresolution"></param>
        /// <param name="xPiexs"></param>
        /// <param name="yPiexs"></param>
        public BlockItem(string name, float centerX, float centerY, float centerXresolution, float centerYresolution, int xPixelNumber, int yPixelNumber)
        {
            _name = name;
            _leftUpX = (float)Math.Round(centerX - centerXresolution * xPixelNumber / 2, 4);
            _leftUpY = (float)Math.Round(centerY + centerYresolution * yPixelNumber / 2, 4);
            _spanX = centerXresolution * xPixelNumber;
            _spanY = centerYresolution * yPixelNumber;
        }

        public BlockItem(string name, float leftX, float rightX, float topY, float buttomY, object nullValue)
        {
            _name = name;
            _leftUpX = leftX;
            _leftUpY = topY;
            _spanX = Math.Abs(rightX - leftX);
            _spanY = Math.Abs(topY - buttomY);
        }

        public masBlockTypes BlockTypes
        {
            get { return _blockTypes; }
            set { _blockTypes = value; }
        }

        public PrjEnvelope ToEnvelope()
        {
            if (_envelope == null)
                _envelope = new PrjEnvelope(MinX, MinY, MaxX, MaxY);
            return _envelope;
        }

        //public static int oid = 990000;
        //public VectorFeature ToVectorFeature()
        //{
        //    Envelope evp = ToEnvelope();
        //    string[] flds = new string[] { "分幅编号","最小经度","最大经度","最小纬度","最大纬度"};
        //    string[] vals = new string[] { _name,evp.MinX.ToString(),evp.MaxX.ToString(),MinY.ToString(),MaxY.ToString()};
        //    return new VectorFeature(oid--, evp.ToPolygon(),flds,vals);
        //}

        private float GetSpan(float span)
        {
            //int v = (int)Math.Floor(span);
            //return (float)Math.Round(span, 2);
            return span;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string QuickImageFile
        {
            get { return _quickImageFile; }
            set { _quickImageFile = value; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string OrbitFilename
        {
            get { return _orbitFile; }
            set { _orbitFile = value; }
        }

        public string BlockIdentity
        {
            get { return _blockIdentity; }
            set { _blockIdentity = value; }
        }

        public float MinX { get { return _leftUpX; } }
        public float MaxX { get { return _leftUpX + _spanX; } }
        public float MinY { get { return (float)Math.Round(_leftUpY - _spanY, 5); } }
        public float MaxY { get { return _leftUpY; } }
        public float CenterX { get { return (float)Math.Round(_leftUpX + _spanX / 2, 6); } }
        public float CenterY { get { return (float)Math.Round(_leftUpY - _spanY / 2, 6); } }

        public float Width
        {
            get { return (float)Math.Round(MaxX - MinX, 5); }
        }

        public float Height
        {
            get { return (float)Math.Round(MaxY - MinY, 5); }
        }

        public bool IsContains(double longitude, double latitude)
        {
            int x = (int)longitude;
            int y = (int)latitude;
            return (x >= _leftUpX) &&
                      (x < _leftUpX + _spanX) &&
                      (y >= _leftUpY - _spanY) &&
                      (y < _leftUpY);
        }

        public override string ToString()
        {
            PrjEnvelope evp = ToEnvelope();
            return "{"+string.Format("Name:{0},MinLon:{1},MaxLon:{2},MinLat:{3},MaxLat:{4}",
                                            Name,
                                            evp.MinX.ToString("0.####"),
                                            evp.MaxX.ToString("0.####"),
                                            evp.MinY.ToString("0.####"),
                                            evp.MaxY.ToString("0.####")+"}");
        }

        public static bool TryParse(string text, out BlockItem blockItem)
        {
            blockItem = null;
            if (string.IsNullOrEmpty(text))
                return false;
            string exp = @"^{Name:(?<Name>\S*),MinLon:(?<MinLon>\d+(\.\d+)?),MaxLon:(?<MaxLon>\d+(\.\d+)?),MinLat:(?<MinLat>\d+(\.\d+)?),MaxLat:(?<MaxLat>\d+(\.\d+)?)}$";
            Match m = Regex.Match(text, exp);
            if (!m.Success)
                return false;
            blockItem = new BlockItem(m.Groups["Name"].Value,
                                                       float.Parse(m.Groups["MinLon"].Value),
                                                       float.Parse(m.Groups["MaxLat"].Value),
                                                       float.Parse(m.Groups["MaxLon"].Value) - float.Parse(m.Groups["MinLon"].Value),
                                                       float.Parse(m.Groups["MaxLat"].Value) - float.Parse(m.Groups["MinLat"].Value));
            return true;
        }
    }
}
