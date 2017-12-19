using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace GeoVis.GeoCore
{
    public enum GeometryType
      : ushort
    {
        Point,
        Arc,
        LineString,
        Beziers,

        Rectangle,
        Ellipse,
        Polygon,

        Group,
        Figure,

        TexRectangle,
    }

    public class Envelope
    {
        public double MinX;
        public double MinY;
        public double MinZ;

        public double MaxX;
        public double MaxY;
        public double MaxZ;

        public static Envelope Empty = new Envelope(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

        public Envelope()
        { }

        public Envelope(double minx,double miny,double maxx,double maxy)
        {
            MinX = minx;
            MinY = miny;
            MaxX = maxx;
            MaxY = maxy;
            MinZ = MaxZ = 0;
        }

        public Envelope(double minx, double miny, double minz, double maxx, double maxy,double maxz)
        {
            MinX = minx;
            MinY = miny;
            MaxX = maxx;
            MaxY = maxy;
            MinZ = minz;
            MaxZ = maxz;
        }

        public Envelope(IEnumerable<GPoint> pts)
        {
            MinX = double.MaxValue;
            MinY = double.MaxValue;
            MinZ = double.MaxValue;
            MaxX = double.MinValue;
            MaxY = double.MinValue;
            MaxZ = double.MinValue;
            
            Join(pts);
        }

        public Envelope Clone()
        {
            return new Envelope(MinX, MinY, MinZ, MaxX, MaxY, MaxZ);
        }

        public void Join(IEnumerable<GPoint> pts)
        {
            foreach (GPoint pt in pts)
            {
                if (MinX > pt.X)
                    MinX = pt.X;
                if (MinY > pt.Y)
                    MinY = pt.Y;
                if (MinZ > pt.Z)
                    MinZ = pt.Z;
                if (MaxX < pt.X)
                    MaxX = pt.X;
                if (MaxY < pt.Y)
                    MaxY = pt.Y;
                if (MaxZ < pt.Z)
                    MaxZ = pt.Z;
            }
        }

        public void Join(GPoint pt)
        {
            if (MinX > pt.X)
                MinX = pt.X;
            if (MinY > pt.Y)
                MinY = pt.Y;
            if (MaxX < pt.X)
                MaxX = pt.X;
            if (MaxY < pt.Y)
                MaxY = pt.Y;
        }

        public void Join(Envelope e)
        {
            Join(new GPoint(e.MinX, e.MinY, e.MinZ));
            Join(new GPoint(e.MaxX, e.MaxY, e.MaxZ));
        }

    }

    public abstract class Geometry
    {
        private GeometryType _type;

        public Geometry()
        { }

        protected Geometry(GeometryType type)            
        {
            _type = type;
        }

        public GeometryType Type { get { return _type; } }

        public abstract Envelope Envelope { get; }
        public abstract Geometry Clone();
        public abstract string ToString(string format);

        public static Geometry Parse(string data,string format)
        {
            return null;
        }
    }

    public class GPoint : Geometry
    {
        public double X=0;
        public double Y=0;
        public double Z=0;

        public GPoint()
            : this(0,0,0)
        {
        }

        public GPoint(double x, double y)
            :this(x,y,0)
        {
        }

        public GPoint(double x,double y,double z)
            : base(GeometryType.Point)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override Geometry Clone()
        {
            return new GPoint(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            GPoint p = obj as GPoint;
            if (p == null)
                return false;
            return p.X == X && p.Y == Y && p.Z == Z;
        }

        public override int GetHashCode()
        {
            return (int)(X*X + Y * Y + Z*Z);
        }

        public string ExportToWKT()
        {
            if (Z <= 0)
                return string.Format("POINT({0} {1})", X, Y);
            else
                return string.Format("POINT({0} {1} {2})", X, Y, Z);
        }

        public  void LoadFromWKT(string ppszDstText)
        {
            int start = ppszDstText.IndexOf('(') + 1;
            int end = ppszDstText.IndexOf(')');
            string subs = ppszDstText.Substring(start, end - start);

            string[] ss = subs.Split(new char[] { ' ' });
            if (ss.Length > 2)
            {
                X = Convert.ToDouble(ss[0]);
                Y = Convert.ToDouble(ss[1]);
                Z = Convert.ToDouble(ss[2]);
            }
            else
            {
                X = Convert.ToDouble(ss[0]);
                Y = Convert.ToDouble(ss[1]);
                Z = 6378137.0;
            }

        }


        public override Envelope Envelope
        {
            get
            {
                Envelope lop = new Envelope(new GPoint[] { this });
                return lop;
               
            }
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented."); 
        }
    }

    public class GLineString : Geometry
    {
        protected List<GPoint> _vertexs = new List<GPoint>();

        public GLineString()
            :base(GeometryType.LineString)
        {
            
        }

        public void AddPoint(double x, double y)
        {
            _vertexs.Add(new GPoint(x, y));
        }

        public void AddPoint(double x, double y,double z)
        {
            _vertexs.Add(new GPoint(x, y,z));
        }

        public void AddPoint(GPoint p)
        {
            _vertexs.Add(p);
        }

        public int PointsCount
        {
            get { return _vertexs.Count; }
        }

        public void InsertPoint(GPoint p, int index)
        {
            _vertexs.Insert(index, p);
        }

        public void SetPoints(params GPoint[] pts)
        {
            _vertexs.Clear();
            _vertexs.AddRange(pts);
        }

        public void SetPoint(int index, GPoint p)
        {
            if (index < 0 || index > _vertexs.Count - 1)
                return ;
            _vertexs[index] = p;
        }

        public GPoint GetPoint(int index)
        {
            if (index < 0 || index > _vertexs.Count - 1)
                return null;
            return _vertexs[index];
        }

        public void RemovePoint(GPoint p)
        {
            if (_vertexs.Contains(p))
            {
                _vertexs.Remove(p);
            }
        }

        public void RemovePointAt(int index)
        {
            if (index < 0 || index > _vertexs.Count - 1)
                return ;
            _vertexs.RemoveAt(index);
 
        }

        public GPoint[] ForceToArray()
        {
            GPoint[] pts = new GPoint[PointsCount];
            _vertexs.CopyTo(pts);
            return pts;
        }

        public  string ExportToWKT()
        {
            string s = string.Empty;
            if (this._vertexs.Count > 0)
            {
                s = s + "(";
                foreach (GPoint gp in _vertexs)
                {
                    s = s + gp.X.ToString() + " ";
                    s = s + gp.Y.ToString() + ",";
                }
                if (_vertexs.Count > 0)
                    s = s + _vertexs[0].X.ToString() + " " + _vertexs[0].Y.ToString();
                if (s.EndsWith(","))
                    s = s.Substring(0, s.Length - 1);
                s = s + ")";
            }
            return s;
        }

        public  void LoadFromWKT(string ppszDstText)
        {
            _vertexs.Clear();

            if (string.IsNullOrEmpty(ppszDstText))
                return;

            int start = ppszDstText.IndexOf('(') + 1;
            int end = ppszDstText.IndexOf(')');
            string subs = ppszDstText.Substring(start, end - start);
            string[] ss = subs.Split(new char[] { ',' });
            foreach (string s in ss)
            {
                string temp = s.Trim();
                string[] pieces = temp.Split(' ');
                if (pieces.Length >= 2)
                {
                    GPoint gp = new GPoint();
                    gp.X = double.Parse(pieces[0]);
                    gp.Y = double.Parse(pieces[1]);
                    gp.Z = 6378137.0;
                    AddPoint(gp);
                }
            }
        }

        public override Envelope Envelope
        {
            get
            {
                if (_vertexs.Count < 2)
                    return null;

                return new Envelope(_vertexs);
            }
        }

        public override Geometry Clone()
        {
            GLineString ls = new GLineString();
            foreach (GPoint p in _vertexs)
                ls.AddPoint(p.Clone() as GPoint);
            
            return ls;
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class GLinearRing : GLineString
    {
        public GLinearRing()
            : base()
        {
            
        }

        public void CloseRing()
        {
            int n = PointsCount;
            if (n < 2)
                return;

            if (!GetPoint(0).Equals(GetPoint(n  - 1)))
            {
                this.AddPoint(GetPoint(0));
            }
        }

        public override Geometry Clone()
        {
            GLinearRing ring = new GLinearRing();
            foreach (GPoint p in _vertexs)
                ring.AddPoint(p.Clone() as GPoint);
            return ring;
        }
    }

    public class GPolygon : Geometry
    {
        List<GLinearRing> _rings = new List<GLinearRing>();

        public GPolygon()
            :base(GeometryType.Polygon)
        {
           
        }

        public void AddLineRing(GLinearRing r)
        {
            _rings.Add(r);
        }

        public GLinearRing GetExternalRing()
        {
            if (_rings.Count > 0)
                return _rings[0];
            return null;
        }

        public GLinearRing GetInternalRing(int index)
        {
          
            if (index < 1 || index > _rings.Count - 1)
                return null;
            return _rings[index];
        }

        public int GetInternalRingNums()
        {
            if (_rings.Count < 2)
                return 0;
            return _rings.Count - 1;
        }

        public override Geometry Clone()
        {
            GPolygon gp = new GPolygon();
            foreach (GLinearRing r in _rings)
                gp.AddLineRing(r.Clone() as GLinearRing);
            return gp;
        }

        public  string ExportToWKT()
        {
            string s = "POLYGON(";
            foreach (GLinearRing r in _rings)
            {
                s += r.ExportToWKT() + ",";
            }
            if(_rings.Count>0)
            if (s.EndsWith(","))
                s = s.Substring(0, s.Length - 1);
            s += ")";
            return s;
        }

        public  void LoadFromWKT(string ppszDstText,string format)
        {
            //string s = (string)ppszDstText.Clone();
            //s = s.Remove(0, 1);
            //s = s.Remove(s.Length - 2, 2);
            //string[] pts = s.Split(new char[] { ';' });
            //for (int i = 0; i < pts.Length; i++)
            //{
            //    GLinearRing gp = new GLinearRing();
            //    if (pts[i] != "")
            //    {
            //        gp.LoadFromWKT(pts[i]);
            //        AddLineRing(gp);
            //    }
            //}
            _rings.Clear();
            if (string.IsNullOrEmpty(ppszDstText))
                return;
            int start = ppszDstText.IndexOf('(') + 1;
            int end = ppszDstText.LastIndexOf(')');
            string subs = ppszDstText.Substring(start, end - start);
            int left = 1;
            while (left != 0)
            {
                start = subs.IndexOf('(');
                end = subs.IndexOf(')');
                left = subs.Length - end - 1;
                GLinearRing gr = new GLinearRing();
                gr.LoadFromWKT(subs.Substring(start, end - start+1));
                AddLineRing(gr);
                if (left > 0)
                    subs = subs.Substring(end + 1, subs.Length - 1);
            }
        }

        public override Envelope Envelope
        {
            get
            {
                if (_rings.Count < 1)
                    return null;
                Envelope e = _rings[0].Envelope.Clone();
                foreach (GLinearRing r in _rings)
                {
                    Envelope ee = r.Envelope;
                    e.Join(ee);
                }
                return e;
            }
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class GRectangle : Geometry
    {
        private double _x; 
        private double _y;
        private double _width; 
        private double _height;

        public enum VertexType
        {
            TwoVertex,
            FourVertex,
            EightVertex,
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
       
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public double Left
        {
            get { return X; }
        }

        public double Right
        {
            get { return _x + _width; }
        }

        public double Bottom
        {
            get { return _y - Height; }//abby
        }

        public double Top
        {
            get { return _y; }
        }


        public GRectangle()
            :base(GeometryType.Rectangle)
        {
           
        }
        public GRectangle(GeometryType type)
            : base(type)
        {

        }

        public GRectangle(double x, double y, double w, double h)
            :base(GeometryType.Rectangle)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public static GRectangle FromLRTB(double left, double right, double top, double bottom)
        {
            GRectangle rect = new GRectangle();
            rect.X = left;
            rect.Y = top;
            rect.Width = right - left;
            rect.Height = bottom - top;
            return rect;
        }


        public override   Geometry Clone()
        {
            return new GRectangle(X, Y, Width, Height);
        }
        public override Envelope Envelope
        {
            get { return new Envelope(X, Y, X + Width, Y + Height); }
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class GeometryGroup : Geometry
    {
        private List<Geometry> geometryList = new List<Geometry>();

        public GeometryGroup()
            : base(GeometryType.Group)
        { }

        public void AddGeometry(Geometry m)
        {
            geometryList.Add(m);
        }

        public void RemoveGeometry(Geometry m)
        {
            geometryList.Remove(m);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > geometryList.Count - 1)
                return;
            geometryList.RemoveAt(index);
        }

        public void ClearAll()
        {
            geometryList.Clear();
        }

        public bool Contains(Geometry m)
        {
            return geometryList.Contains(m);
        }

        public Geometry GetSubGeometry(int index)
        {
            if (index < 0 || index > geometryList.Count - 1)
                return null;
            return geometryList[index];
        }

        public void SetSubGeometry(Geometry gem, int index)
        {
            if (index < 0 || index > geometryList.Count - 1)
                return ;

            geometryList[index] = gem;
        }

        public void InsertSubGeometry(Geometry geom, int index)
        {
            geometryList.Insert(index, geom);
        }

        public int GetSubGeomCount
        {
            get
            {
                return geometryList.Count;
            }
        }

        public override Envelope Envelope
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override Geometry Clone()
        {
            GeometryGroup result = new GeometryGroup();
            foreach (Geometry g in this.geometryList)
            {
                result.AddGeometry(g.Clone());
            }
            return result;
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class GFigure
        : GeometryGroup
    {
        bool _isClosed = false;
        bool IsClosed 
        {
            get{return _isClosed;}
            set { _isClosed = true; } 
        }
    }

    public class GEllipse : Geometry
    {
        public GPoint Center;
        public double A;
        public double B;

        public GEllipse()
            :base(GeometryType.Ellipse)
        {
            
        }

        public GEllipse(GeometryType type)
            : base(type)
        {

        }

        public GEllipse(GPoint c,double a,double b)
            :base(GeometryType.Ellipse)
        {
            Center = c;
            A = a;
            B = b;
        }

        public override Envelope Envelope
        {
            get
            {
                return new Envelope(Center.X - A,Center.Y - B,
                    Center.X + A,Center.Y + B);
            }
        }


        public override string ToString(string format)
        {
           
            return "";
        }

        public override Geometry Clone()
        {
            return new GEllipse(this.Center,A,B);
        }
        
    }

    public class GArc : GEllipse
    {
        public double StartAngle;
        public double SweepAngle;

        public GArc()
            :base(GeometryType.Arc)
        { }


        public override Envelope Envelope
        {
            get
            {
                return base.Envelope;
            }
        }

        public override string ToString(string format)
        {
            return base.ToString(format);
        }

        public override Geometry Clone()
        {
            return new GEllipse();
        }
    }

    public class GBeziers : Geometry
    {
        private List<GPoint> _controlPts = new List<GPoint>();


        public GBeziers()
            :base(GeometryType.Beziers)
        {
            
        }

        public GPoint this[int i]
        {
            get { return _controlPts[i]; }
            set 
            {
                if (i < 0 || i > _controlPts.Count - 1)
                    return;
                _controlPts[i] = value; 
            }
        }

        public void AddControlPt(GPoint pt)
        {
            _controlPts.Add(pt);
        }

        public void RemoveControlPt(GPoint pt)
        {
            _controlPts.Remove(pt);
        }


        public GeometryGroup ToPoints()
        {
            return null;
        }

        public override Envelope Envelope
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override Geometry Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ToString(string format)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class GTexRectangle : GRectangle
    {
        public Image image;

        public GTexRectangle()
            :base(GeometryType.TexRectangle)
        {
        }
    }
}
