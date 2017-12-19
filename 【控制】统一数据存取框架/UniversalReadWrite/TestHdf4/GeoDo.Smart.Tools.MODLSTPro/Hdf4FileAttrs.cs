using System;
using System.Collections.Generic;
using System.Text;
using GeoDo.HDF4;
using GeoDo.RSS.DF.HDF4.Cloudsat;

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
                UpperLeftPointOld = string.Format("UpperLeftPointMtrs=({0},{1})", Hdf4FileAttr.XMin, Hdf4FileAttr.YMax);
                LowerRightPointOld = string.Format("LowerRightMtrs=({0},{1})", Hdf4FileAttr.XMax, Hdf4FileAttr.YMin);
                XDimOld = string.Format("XDim={0}", Hdf4FileAttr.XDim);
                YDimOld = string.Format("YDim={0}", Hdf4FileAttr.YDim);
            }
        }

        public void AddHdf4Files(string[] f4Names)
        {
            foreach (string f4Name in f4Names)
            {
                AddHdf4File(f4Name);
            }
        }

        public string UpperLeftPointNew { get; set; }
        public string LowerRightPointNew { get; set; }
        public string UpperLeftPointOld { get; set; }
        public string LowerRightPointOld { get; set; }
        public string XDimNew { get; set; }
        public string YDimNew { get; set; }
        public string XDimOld { get; set; }
        public string YDimOld { get; set; }

        public string GetNewAttr(string attr)
        {
            string newAttr = attr.Replace(UpperLeftPointOld, UpperLeftPointNew)
                .Replace(LowerRightPointOld, LowerRightPointNew)
                .Replace(XDimOld, XDimNew)
                .Replace(YDimOld, YDimNew);
            return newAttr;//.ToCharArray();
        }
        //public byte[] GetNewAttr(string attr)
        //{
        //    string newAttr = attr.Replace(UpperLeftPointOld, UpperLeftPointNew)
        //        .Replace(LowerRightPointOld, LowerRightPointNew)
        //        .Replace(XDimOld, XDimNew)
        //        .Replace(YDimOld, YDimNew);
        //    //return newAttr;//.ToCharArray();
        //    var stringLength = newAttr.Length;
        //    byte[] bs = Encoding.Default.GetBytes(newAttr);
        //    return bs;
        //}

        public HDFAttributeDef[] GetHDFAttributeDefs()
        {
            HDFAttribute[] globalAttrs = Hdf4FileAttr.H4File.GlobalAttrs;
            HDFAttributeDef[] attributeDef5s = new HDFAttributeDef[globalAttrs.Length];
            for (int i = 0; i < globalAttrs.Length; i++)
            {
                HDFAttribute attribute4 = globalAttrs[i];
                HDFAttributeDef attribute5 = new HDFAttributeDef();
                attribute5.Name = attribute4.Name;
                attribute5.Size = attribute4.Count;
                attribute5.Type = Utility.GetBaseType(attribute4.DataType);
                if (attribute4.DataType == HDF4Helper.DataTypeDefinitions.DFNT_CHAR8)
                    attribute5.Value = GetNewAttr(attribute4.Value);
                else
                    attribute5.Value = attribute4.Value;
                attributeDef5s[i] = attribute5;
            }
            return attributeDef5s;
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
                if (XMin > fileAttr.XMin) XMin = fileAttr.XMin;
                if (YMin > fileAttr.YMin) YMin = fileAttr.YMin;
                if (XMax < fileAttr.XMax) XMax = fileAttr.XMax;
                if (YMax < fileAttr.YMax) YMax = fileAttr.YMax;
            }

            UpperLeftPointNew = string.Format("UpperLeftPointMtrs=({0},{1})", XMin, YMax);
            LowerRightPointNew = string.Format("LowerRightMtrs=({0},{1})", XMax, YMin);

            foreach (Hdf4FileAttr fileAttr in this)
            {
                fileAttr.Offset(XMin, YMin, XMax, YMax);
            }

            double dx = XMax - XMin;
            double dxcell = Math.Round(dx / Hdf4FileAttr.CellWidth);
            Hdf4FileAttr.XDim = Convert.ToInt32(dxcell);
            XDimNew = string.Format("XDim={0}", Hdf4FileAttr.XDim);

            double dy = YMax - YMin;
            double dycell = Math.Round(dy / Hdf4FileAttr.CellHeight);
            Hdf4FileAttr.YDim = Convert.ToInt32(dycell);
            YDimNew = string.Format("YDim={0}", Hdf4FileAttr.YDim);
        }
    }
}