using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class AnnotationDbfReader:IAnnotationDbfReader
    {
        private DbaseReader _reader = null;
        private string _oidFieldname = "FeatureID".ToUpper();
        private int _idxOfOIDField = -1;
        private const int MinMapLevel = 0;
        private const int MaxMapLevel = 16;
        private const string cstDefaultAngleField = "Angle";
        private const string cstDefaultXField = "X";
        private const string cstDefaultYField = "Y";

        public AnnotationDbfReader(string anndbf)
        {
            _reader = new DbaseReader(anndbf);
            int i = 0;
            foreach (string fld in _reader.Fields)
            {
                if (fld.ToUpper() == _oidFieldname)
                {
                    _idxOfOIDField = i;
                    break;
                }
                i++;
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (_reader != null)
                _reader.Dispose();
        }

        #endregion

        #region IAnnotationDbfReader 成员

        public LabelLocation[] GetLabelLocation(int oid)
        {
            oid = oid - 1;
            List<string[]> locArgs = new List<string[]>();
            for (int i = 0; i < _reader.RecordCount; i++)
            {
                int id = -1;
                if (int.TryParse(_reader.Values[i][_idxOfOIDField], out id))
                {
                    if (id == oid)
                    {
                        locArgs.Add(_reader.Values[i]);
                    }
                }
            }
            if (locArgs.Count == 0)
                return null;
            return GetLabelLocation(locArgs);
        }

        private LabelLocation[] GetLabelLocation(List<string[]> locArgs)
        {
            string[] fields = _reader.Fields;
            for (int i = 0; i < fields.Length; i++)
                fields[i] = fields[i].ToUpper();
            List<LabelLocation> locs = new List<LabelLocation>();
            //
            LabelLocation[] lbs = GetLabelLocation(fields, cstDefaultXField,cstDefaultYField, cstDefaultAngleField, locArgs);
            if (lbs != null)
                locs.AddRange(lbs);
            //
            for (int i = MinMapLevel; i <= MaxMapLevel; i++)
            {
                string xfield = cstDefaultXField + "L" + i.ToString();
                string yfield = cstDefaultYField + "L" + i.ToString();
                string angleField = cstDefaultAngleField + "L" + i.ToString();
                lbs = GetLabelLocation(fields, xfield, yfield, angleField, locArgs);
                if (lbs != null)
                    locs.AddRange(lbs);
            }
            return locs.Count > 0 ? locs.ToArray() : null;
        }

        private LabelLocation[] GetLabelLocation(string[] fields,string xfield, string yfield, string angleField, List<string[]> locArgs)
        {
            int ix = Array.IndexOf(fields, xfield.ToUpper());
            int iy = Array.IndexOf(fields, yfield.ToUpper());
            int iangle = Array.IndexOf(fields, angleField.ToUpper());
            if (ix == -1 || iy == -1 || iangle == -1)
                return null;
            List<LabelLocation> locs = new List<LabelLocation>();
            foreach (string[] vs in locArgs)
            {
                double x = 0;
                double y = 0;
                float angle = 0;
                if (!double.TryParse(vs[ix], out x))
                    continue;
                if (!double.TryParse(vs[iy], out y))
                    continue;
                if (vs[iangle].Trim() == string.Empty)
                    vs[iangle] = "0";
                if (!float.TryParse(vs[iangle], out angle))
                    continue;
                if (angle < 0)
                    angle = Math.Abs(angle);
                else
                    angle = 360 - angle;
                LabelLocation l = new LabelLocation(new ShapePoint(x, y), (int)angle);
                locs.Add(l);
            }
            return locs.Count > 0 ? locs.ToArray() : null;
        }

        #endregion
    }
}
