using System;
using System.Collections.Generic;
using System.Linq;
using GeoDo.HDF4;
using GeoDo.RSS.DF.HDF4.Cloudsat;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class Hdf4FileAttr
    {
        public string HDFEOSVersion { get; private set; }
        public string GridName { get; private set; }
        public int XDim { get; set; }
        public int YDim { get; set; }
        public double XMin { get; private set; }
        public double YMin { get; private set; }
        public double XMax { get; private set; }
        public double YMax { get; private set; }
        public double CellWidth { get; private set; }
        public double CellHeight { get; private set; }
        public string Projection { get; private set; }
        public string ProjParams { get; private set; }
        public DataFields DataFields { get; private set; }
        public H4File H4File { get; private set; }
        public string F4Name { get; private set; }

        public int XOffset { get; private set; }
        public int YOffset { get; private set; }

        public void Offset(int xOffset, int yOffset)
        {
            XOffset = xOffset;
            YOffset = yOffset;
        }

        /// <summary>
        /// 计算偏移
        /// </summary>
        /// <param name="parentXMin">计算拼接的父窗口x最小值</param>
        /// <param name="parentYMin">计算拼接的父窗口y最小值</param>
        /// <param name="parentXMax">计算拼接的父窗口x最大值</param>
        /// <param name="parentYMax">计算拼接的父窗口y最大值</param>
        public void Offset(double parentXMin, double parentYMin, double parentXMax, double parentYMax)
        {
            double dx = XMin - parentXMin;
            double dxcell = Math.Round(dx / CellWidth);
            XOffset = Convert.ToInt32(dxcell);

            //double dy = YMin - parentYMin;
            //double dycell = Math.Round(dy / CellHeight);
            //YOffset = Convert.ToInt32(dycell);
            double dy =  parentYMax - YMax;
            double dycell = Math.Round(dy / CellHeight);
            YOffset = Convert.ToInt32(dycell);
        }

        public Hdf4FileAttr Clone()
        {
            var hdf4FileAttr = new Hdf4FileAttr
            {
                CellHeight = CellHeight,
                CellWidth = CellWidth,
                DataFields = DataFields.Clone(),
                GridName = GridName,
                H4File = H4File,
                HDFEOSVersion = HDFEOSVersion,
                ProjParams = ProjParams,
                Projection = ProjParams,
                XDim = XDim,
                YDim = YDim,
                XMin = XMin,
                XMax = XMax,
                YMin = YMin,
                YMax = YMax
            };

            return hdf4FileAttr;
        }

        private Hdf4FileAttr()
        {
            
        }

        public Hdf4FileAttr(string f4Name)
        {
            F4Name = f4Name;
            H4File = new H4File(null, null, null, new long[] { 0, 0 });
            H4File.Load(f4Name);

            foreach (HDFAttribute attr in H4File.GlobalAttrs)
            {
                if (attr.Name == "HDFEOSVersion")
                {
                    HDFEOSVersion = attr.Value;
                }
                else if (attr.Name == "StructMetadata.0")
                {
                    string structMetadata = attr.Value;
                    Pair pair = AnalysisStructMetadata(structMetadata);
                    GetAttr(pair);
                }
            }

            GetHDFAttributeDefs();
        }



        private void GetHDFAttributeDefs()
        {
            List<HDFAttributeDef[]> lstDefses = new List<HDFAttributeDef[]>();
            //foreach (H4SDS dataset in H4File.Datasets)
            for (int i = 0; i < H4File.Datasets.Length; i++)
            {
                H4SDS dataset = H4File.Datasets[i];
                HDFAttributeDef[] attributeDef5s = new HDFAttributeDef[dataset.SDAttributes.Length];
                lstDefses.Add(attributeDef5s);

                //foreach (HDFAttribute attribute4 in dataset.SDAttributes)
                for (int j = 0; j < dataset.SDAttributes.Length; j++)
                {
                    HDFAttribute attribute4 = dataset.SDAttributes[j];
                    HDFAttributeDef attribute5 = new HDFAttributeDef();
                    attribute5.Name = attribute4.Name;
                    if (attribute4.Name == "scale_factor")
                    {
                        int sd = 9;
                        int s = sd;
                    }
                    attribute5.Size = attribute4.Count;
                    attribute5.Type = Utility.GetBaseType(attribute4.DataType);
                    attribute5.Value = attribute4.Value;
                    attributeDef5s[j] = attribute5;
                }
            }
            DatasetsAttributeDefs = lstDefses;
        }

        public List<HDFAttributeDef[]> DatasetsAttributeDefs { get; set; }

        public bool IsSameClassHdf4(Hdf4FileAttr attr)
        {
            if (HDFEOSVersion != attr.HDFEOSVersion)
                return false;
            if (GridName != attr.GridName)
                return false;
            if (Projection != attr.Projection)
                return false;
            if (ProjParams != attr.ProjParams)
                return false;
            if (XDim != attr.XDim)
                return false;
            if (YDim != attr.YDim)
                return false;
            //if (XMin - attr.XMin > double.Epsilon)
            //    return false;
            //if (YMin - attr.YMin > double.Epsilon)
            //    return false;
            //if (XMax - attr.XMax > double.Epsilon)
            //    return false;
            //if (YMax - attr.YMax > double.Epsilon)
            //    return false;
            if (CellWidth - attr.CellWidth > CellWidth/1000000)
                return false;
            if (CellHeight - attr.CellHeight > CellWidth/1000000)
                return false;
            if (DataFields!=null && !DataFields.IsSameDataFields(attr.DataFields))
                return false;
            return true;
        }

        private void GetAttr(Pair pair)
        {
            GridName = pair.GetAttrValue("GridName");
            string xdimStr = pair.GetAttrValue("XDim");

            int xdim = 0;
            if (int.TryParse(xdimStr, out xdim))
                XDim = xdim;
            string ydimStr = pair.GetAttrValue("YDim");
            int ydim = 0;
            if (int.TryParse(ydimStr, out ydim))
                YDim = ydim;

            string upperLeftPointMtrs = pair.GetAttrValue("UpperLeftPointMtrs");
            string[] strsUL = upperLeftPointMtrs.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            double xmin = 0.0;
            if (double.TryParse(strsUL[0], out xmin))
                XMin = xmin;
            double ymax = 0.0;
            if (double.TryParse(strsUL[1], out ymax))
                YMax = ymax;

            string lowerRightMtrs = pair.GetAttrValue("LowerRightMtrs");
            string[] strsLR = lowerRightMtrs.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            double xmax = 0.0;
            if (double.TryParse(strsLR[0], out xmax))
                XMax = xmax;
            double ymin = 0.0;
            if (double.TryParse(strsLR[1], out ymin))
                YMin = ymin;

            CellWidth = (XMax - XMin)/XDim;
            CellHeight = (YMax - YMin)/YDim;

            Projection = pair.GetAttrValue("Projection");
            ProjParams = pair.GetAttrValue("ProjParams");
            Pair dataFields = pair.GetGroup("DataField");
            DataFields = new DataFields(dataFields);
        }

        private Pair AnalysisStructMetadata(string structMetadata)
        {
            string[] strs = structMetadata.Split(new[] { "\n", "\t", "\0" }, StringSplitOptions.RemoveEmptyEntries);

            var pair = new Pair();
            var psPairs = new Stack<Pair>();
            foreach (string str in strs)
            {
                if (str == "END")
                    break;

                Pair curPair = null;
                string endString = null;
                if (psPairs.Count > 0)
                {
                    curPair = psPairs.Peek();
                    endString = curPair.EndString;
                }

                if (endString != null && str == endString)
                {
                    var pop = psPairs.Pop();
                    if (psPairs.Count == 0)
                        pair.Pairs.Add(pop);
                    else
                        psPairs.Peek().Pairs.Add(pop);
                    continue;
                }

                var keyValue = new KeyValue(str);
                if (keyValue.Key == "GROUP" || keyValue.Key == "OBJECT")
                {
                    Pair newPair = new Pair { BeginString = str };
                    psPairs.Push(newPair);
                }
                else if (curPair != null)
                {
                    curPair.KeyValues.Add2(keyValue);
                }
            }

            return pair;
        }
    }
}