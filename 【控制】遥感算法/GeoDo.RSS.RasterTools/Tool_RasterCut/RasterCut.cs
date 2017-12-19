using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.IO;

namespace GeoDo.RSS.RasterTools
{
    public class RasterCut : IRasterCut
    {
        public class BlockItem
        {
            public string Name;
            public int Left;
            public int Top;
            public int Width;
            public int Height;

            public BlockItem() { }

            public BlockItem(Rectangle rect)
            {
                Left = rect.Left;
                Top = rect.Top;
                Width = rect.Width;
                Height = rect.Height;
            }

            public BlockItem(int left, int top, int width, int height)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
            }

            public override string ToString()
            {
                return Name ?? "_" + Left.ToString() + "_" + Top.ToString();
            }
        }

        public class CutArgument
        {
            public int[] BandNos;
            public BlockItem[] Items;
            public string Driver = "LDF";
            public string DriverOptions;
            public string OutFileName;

            public bool IsOK(out string error)
            {
                error = null;
                if (Items == null || Items.Length == 0)
                {
                    error = "分幅块为空,无法进行裁切操作!";
                    return false;
                }
                if (string.IsNullOrEmpty(OutFileName))
                {
                    error = "输出文件名未指定,无法进行裁切操作!";
                    return false;
                }
                if (string.IsNullOrEmpty(Driver))
                    Driver = "LDF";
                return true;
            }
        }

        public RasterCut()
        {
        }

        public void Cut(IRasterDataProvider dataProvider, RasterCut.CutArgument args, Action<int, string> progressTracker)
        {
            CheckArgIsOK(dataProvider, args);
            IRasterDataProvider dstDataProvider = null;
            int count = args.Items.Length;
            float step = 100 / (float)count;
            float crtStep = 0;
            foreach (RasterCut.BlockItem item in args.Items)
            {
                if (progressTracker != null)
                    progressTracker((int)crtStep, null);
                using (dstDataProvider = CreateDstDataProvider(dataProvider, args, item))
                {
                    Cut(dataProvider, dstDataProvider, args, item, progressTracker);
                }
                crtStep += step;
            }
            if (progressTracker != null)
                progressTracker(100, null);
        }

        byte[] _buffer;
        private void Cut(IRasterDataProvider srcDataProvider, IRasterDataProvider dstDataProvider, CutArgument args, BlockItem item, Action<int, string> progressTracker)
        {
            int rowsOfBlock;
            byte[] buffer = TryBuildBlockBuffer(dstDataProvider, out rowsOfBlock);
            int bRow = 0;
            int eRow = rowsOfBlock;
            int actualRows = 0;
            GCHandle handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            int srcX = item.Left;
            int srcY = item.Top;
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                do
                {
                    actualRows = eRow - bRow;
                    //
                    foreach (int bNo in args.BandNos)
                    {
                        srcDataProvider.GetRasterBand(bNo).Read(srcX, srcY + bRow, item.Width,
                            actualRows, ptr, srcDataProvider.DataType, dstDataProvider.Width, actualRows);
                        dstDataProvider.GetRasterBand(bNo).Write(0, bRow, dstDataProvider.Width, actualRows, ptr, dstDataProvider.DataType, dstDataProvider.Width, actualRows);
                    }
                    //
                    bRow += rowsOfBlock;
                    eRow = Math.Min(bRow + rowsOfBlock, srcDataProvider.Height);
                }
                while (eRow < dstDataProvider.Height);
            }
            finally
            {
                handle.Free();
            }
        }

        private byte[] TryBuildBlockBuffer(IRasterDataProvider srcDataProvider, out int rowsOfBlock)
        {
            int rowSize = srcDataProvider.Width * DataTypeHelper.SizeOf(srcDataProvider.DataType);
            rowsOfBlock = srcDataProvider.Height;
        cntTry:
            try
            {
                if (_buffer == null || _buffer.Length != srcDataProvider.Width * srcDataProvider.Height * DataTypeHelper.SizeOf(srcDataProvider.DataType))
                    _buffer = new byte[rowSize * rowsOfBlock];
            }
            catch (OutOfMemoryException)
            {
                rowsOfBlock /= 2;
                goto cntTry;
            }
            return _buffer;
        }

        private IRasterDataProvider CreateDstDataProvider(IRasterDataProvider srcDataProvider, RasterCut.CutArgument args, RasterCut.BlockItem item)
        {
            string extName;
            CoordEnvelope evp = GetDstEnvelope(srcDataProvider, item);
            object[] options = GetOptions(srcDataProvider, args, item, out extName, evp);
            IRasterDataDriver drv = GeoDataDriver.GetDriverByName(args.Driver, args.DriverOptions) as IRasterDataDriver;
            string fname = GetOutFileName(args.OutFileName, item, extName);
            return drv.Create(fname, item.Width, item.Height, args.BandNos.Length, srcDataProvider.DataType,
                options);
        }

        private string GetOutFileName(string fname, BlockItem item, string extName)
        {
            if (fname.Contains("{0}"))
                return string.Format(fname, item.ToString() + extName);
            return Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname) + item.ToString() + extName);
        }

        private object[] GetOptions(IRasterDataProvider dataProvider, CutArgument args, BlockItem item, out string extName, CoordEnvelope evp)
        {
            extName = null;
            List<object> options = new List<object>();
            if (args.Driver == "LDF")
            {
                extName = ".ldf";
                if (dataProvider.SpatialRef != null)
                {
                    string spRef = "SPATIALREF=" + dataProvider.SpatialRef.ToProj4String();
                    options.Add(spRef);
                }
                string mapInf = evp.ToMapInfoString(new Size(item.Width, item.Height));
                options.Add(mapInf);
            }
            return options.Count > 0 ? options.ToArray() : null;
        }

        private CoordEnvelope GetDstEnvelope(IRasterDataProvider srcDataProvider, BlockItem item)
        {
            return srcDataProvider.ToCoordEnvelope(new Rectangle(item.Left, item.Top, item.Width, item.Height));
        }

        private void CheckArgIsOK(IRasterDataProvider dataProvider, CutArgument args)
        {
            if (dataProvider == null)
                throw new ArgumentNullException("dataProvider");
            if (args == null)
                throw new ArgumentNullException("args");
            string error = null;
            if (!args.IsOK(out error))
                throw new ArgumentException(error);
            if (args.BandNos == null)
            {
                args.BandNos = new int[dataProvider.BandCount];
                for (int b = 1; b <= dataProvider.BandCount; b++)
                    args.BandNos[b - 1] = b;
            }
            else
            {
                foreach (int bNo in args.BandNos)
                    if (bNo < 1 || bNo > dataProvider.BandCount)
                        throw new ArgumentOutOfRangeException("BandNos");
            }
        }
    }
}
