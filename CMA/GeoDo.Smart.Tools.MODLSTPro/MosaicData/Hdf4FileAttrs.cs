using System;
using System.Collections.Generic;
using System.Text;
using GeoDo.HDF4;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using GeoDo.HDF5;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class Hdf4FileAttrs : List<Hdf4FileAttr>
    {
        public Hdf4FileAttr Hdf4FileAttr { get; private set; }

        public void AddHdf4File(string f4Name)
        {
            var hdf4FileAttr = new Hdf4FileAttr(f4Name);
            Add(hdf4FileAttr);
            if (Hdf4FileAttr == null)
            {
                Hdf4FileAttr = hdf4FileAttr.Clone();
                SEnvelope envelope = Hdf4FileAttr.Envelope;
                UpperLeftPointOld = GetUpperLeftPointString(envelope);
                LowerRightPointOld = GetLowerRightPointString(envelope);
                XDimOld = string.Format("XDim={0}", Hdf4FileAttr.XDim);
                YDimOld = string.Format("YDim={0}", Hdf4FileAttr.YDim);
            }
        }

        public void Dispose()
        {
            foreach (Hdf4FileAttr hdf in this)
            {
                hdf.H4File.Dispose();
            }
            if(Hdf4FileAttr != null)
            Hdf4FileAttr.H4File.Dispose();
        }

        private string GetUpperLeftPointString(SEnvelope envelope)
        {
            return string.Format("UpperLeftPointMtrs=({0},{1})", envelope.XMin.ToString("0.000000"), envelope.YMax.ToString("0.000000"));
        }
        private string GetLowerRightPointString(SEnvelope envelope)
        {
            return string.Format("LowerRightMtrs=({0},{1})", envelope.XMax.ToString("0.000000"), envelope.YMin.ToString("0.000000"));
        }

        public void AddHdf4Files(string[] f4Names)
        {
            foreach (string f4Name in f4Names)
            {
                AddHdf4File(f4Name);
            }
        }

        private string UpperLeftPointNew { get; set; }
        private string LowerRightPointNew { get; set; }
        private string UpperLeftPointOld { get; set; }
        private string LowerRightPointOld { get; set; }
        private string XDimNew { get; set; }
        private string YDimNew { get; set; }
        private string XDimOld { get; set; }
        private string YDimOld { get; set; }

        public string GetNewAttr(string attr)
        {
            string newAttr = attr.Replace(UpperLeftPointOld, UpperLeftPointNew)
                .Replace(LowerRightPointOld, LowerRightPointNew)
                .Replace(XDimOld, XDimNew)
                .Replace(YDimOld, YDimNew);
            return newAttr;
        }

        public HDFAttributeDef[] GetHDFAttributeDefs()
        {
            HDFAttribute[] globalAttrs = Hdf4FileAttr.H4File.GlobalAttrs;
            List<HDFAttributeDef> attributeDef5s = new List<HDFAttributeDef>();
            for (int i = 0; i < globalAttrs.Length; i++)
            {
                HDFAttribute attribute4 = globalAttrs[i];
                HDFAttributeDef attribute5 = new HDFAttributeDef();
                attribute5.Name = attribute4.Name;
                attribute5.Size = attribute4.Count;
                attribute5.Type = Utility.GetAttrType(attribute4.DataType);
                if (attribute4.DataType == HDF4Helper.DataTypeDefinitions.DFNT_CHAR8)
                    attribute5.Value = GetNewAttr(attribute4.Value);
                else
                    attribute5.Value = attribute4.Value;
                attributeDef5s.Add(attribute5);
            }
            if (attributeDef5s.Count == 0)
                return null;
            else
            {
                attributeDef5s.Sort((cur, last) => cur.Name.CompareTo(last.Name));
                return attributeDef5s.ToArray();
            }
        }

        /// <summary>
        /// 验证 Hdf4 文件是否都相同，当前认为除四角范围可以不一致
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (Count < 1)
                return false;
            if (Count == 1)
                return true;

            Hdf4FileAttr hdf4FileAttr = this[0];
            for (int i = 1; i < Count; i++)
            {
                if (!hdf4FileAttr.IsSameClassHdf4(this[i]))
                    return false;
            }

            return true;
        }

        public void ResetOffset()
        {
            double XMin = double.MaxValue;
            double YMin = double.MaxValue;
            double XMax = double.MinValue;
            double YMax = double.MinValue;
            foreach (Hdf4FileAttr fileAttr in this)
            {
                SEnvelope envelope = fileAttr.Envelope;
                if (XMin > envelope.XMin) XMin = envelope.XMin;
                if (YMin > envelope.YMin) YMin = envelope.YMin;
                if (XMax < envelope.XMax) XMax = envelope.XMax;
                if (YMax < envelope.YMax) YMax = envelope.YMax;
            }

            SEnvelope envelopef4 = Hdf4FileAttr.Envelope;
            envelopef4.XMin = XMin;
            envelopef4.YMin = YMin;
            envelopef4.XMax = XMax;
            envelopef4.YMax = YMax;
            
            RefreshPointString(envelopef4);

            foreach (Hdf4FileAttr fileAttr in this)
            {
                fileAttr.Offset(envelopef4);
            }

            double dx = envelopef4.Width;
            double dxcell = Math.Round(dx / Hdf4FileAttr.CellWidth);
            Hdf4FileAttr.XDim = Convert.ToInt32(dxcell);
            XDimNew = string.Format("XDim={0}", Hdf4FileAttr.XDim);

            double dy = envelopef4.Height;
            double dycell = Math.Round(dy / Hdf4FileAttr.CellHeight);
            Hdf4FileAttr.YDim = Convert.ToInt32(dycell);
            YDimNew = string.Format("YDim={0}", Hdf4FileAttr.YDim);
        }

        public void RefreshPointString(SEnvelope envelope)
        {
            UpperLeftPointNew = GetUpperLeftPointString(envelope);
            LowerRightPointNew = GetLowerRightPointString(envelope);
        }
    }
}