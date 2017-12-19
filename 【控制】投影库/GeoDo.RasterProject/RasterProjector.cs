using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoDo.Project;
        
namespace GeoDo.RasterProject
{
    public class RasterProjector:IRasterProjector
    {
        public const ushort InvalidValue = UInt16.MaxValue;

        public void Transform(ISpatialReference srcSpatialRef, double[] srcXs, double[] srcYs, ISpatialReference dstSpatialRef)
        {
            using (IProjectionTransform transform = ProjectionTransformFactory.GetProjectionTransform(srcSpatialRef, dstSpatialRef))
            {
                if (!srcSpatialRef.IsSame(dstSpatialRef))
                    transform.Transform(srcXs, srcYs);
            }
        }
        
        public void ComputeDstEnvelope(ISpatialReference srcSpatialRef, double[] srcXs, double[] srcYs, Size srcSize, ISpatialReference dstSpatialRef, PrjEnvelope maskEnvelope,
                                       out PrjEnvelope dstEnvelope, Action<int, string> progressCallback)
        {
            Transform(srcSpatialRef, srcXs, srcYs, dstSpatialRef);
            dstEnvelope = GetEnvelope(srcXs, srcYs, maskEnvelope, dstSpatialRef, progressCallback);
        }

        public void ComputeDstEnvelope(ISpatialReference srcSpatialRef, double[] srcXs, double[] srcYs, Size srcSize,
                                       ISpatialReference dstSpatialRef, out PrjEnvelope dstEnvelope, Action<int, string> progressCallback)
        {
            ComputeDstEnvelope(srcSpatialRef, srcXs, srcYs, srcSize, dstSpatialRef, null, out dstEnvelope, progressCallback);
        }

        public static PrjEnvelope GetEnvelope(double[] srcXs, double[] srcYs, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            return GetEnvelope(srcXs, srcYs, null, dstSpatialRef, progressCallback);
        }

        /// <summary>
        /// 这里面的去除噪声数据的处理，会影响到全球拼图处理中数据跨越+-180度数据的正确经纬度设置。
        /// </summary>
        /// <param name="srcXs"></param>
        /// <param name="srcYs"></param>
        /// <param name="maskEnvelope"></param>
        /// <param name="dstSpatialRef"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public static PrjEnvelope GetEnvelope(double[] srcXs, double[] srcYs, PrjEnvelope maskEnvelope, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = SpatialReference.GetDefault();
            bool isLatLong = (dstSpatialRef.ProjectionCoordSystem == null && dstSpatialRef.GeographicsCoordSystem != null);
            double noise = 0d;
            if (maskEnvelope == null)
            {
                noise = 0.001d;
                if (isLatLong && maskEnvelope == null)//投影目标是地理坐标
                {
                    maskEnvelope = new PrjEnvelope(-180, 180, -90, 90);
                    //maskEnvelope = new PrjEnvelope(-145, 180, -75, 75);
                }
                else
                {
                    maskEnvelope = new PrjEnvelope(double.MinValue, double.MaxValue, double.MinValue, double.MaxValue);
                    ;//...
                }
            }
            PrjEnvelope dstEnvelope = PrjEnvelope.GetEnvelope(srcXs, srcYs, maskEnvelope);
            bool spans180thMeridian = maskEnvelope.Width - dstEnvelope.Width < 0.011d;//(这里应当传入角度分辨率)
            if (spans180thMeridian)//跨越180度数据，不再去除噪声数据。
            {
                dstEnvelope.MinX = maskEnvelope.MinX;
                dstEnvelope.MaxX = maskEnvelope.MaxX;
            }
            else if (noise != 0d)//可以去除0.1%以下的噪声数据
            {
                dstEnvelope.MinX = dstEnvelope.MinX + dstEnvelope.Width * noise;
                dstEnvelope.MaxX = dstEnvelope.MaxX - dstEnvelope.Width * noise;
                dstEnvelope = PrjEnvelope.GetEnvelope(srcXs, srcYs, dstEnvelope);
            }
            return dstEnvelope;
        }

        public static PrjEnvelope GetEnvelope(float[] srcXs, float[] srcYs, ISpatialReference dstSpatialRef, Action<int, string> progressCallback)
        {
            if (dstSpatialRef == null)
                dstSpatialRef = SpatialReference.GetDefault();
            PrjEnvelope maskEnvelope = new PrjEnvelope(double.MinValue, double.MaxValue, double.MinValue, double.MaxValue);
            bool isLatLong = (dstSpatialRef.ProjectionCoordSystem == null && dstSpatialRef.GeographicsCoordSystem != null);
            if (isLatLong)//投影目标是地理坐标
            {
                //maskEnvelope = new PrjEnvelope(-180, 180, -90, 90);
                maskEnvelope = new PrjEnvelope(-145, 180, -75, 75);
            }
            else
            { ;}//其他投影范围未实现
            PrjEnvelope dstEnvelope = PrjEnvelope.GetEnvelope(srcXs, srcYs, maskEnvelope);
            return dstEnvelope;
        }

        /// <summary>
        /// 这里的srcXs，srcYs是源参考投影(srcSpatialRef)下的值。
        /// </summary>
        public void ComputeIndexMapTable(ISpatialReference srcSpatialRef, double[] srcXs, double[] srcYs, Size srcSize,
                                        ISpatialReference dstSpatialRef, Size dstSize, PrjEnvelope dstEnvelope,
                                        out UInt16[] rowMapTable, out UInt16[] colMapTable, Action<int, string> progressCallback)
        {
            PrjEnvelope maxDstEnvelope;
            ComputeDstEnvelope(srcSpatialRef, srcXs, srcYs, srcSize, dstSpatialRef, out maxDstEnvelope, progressCallback);
            ComputeIndexMapTable(srcXs, srcYs, srcSize, dstSize, dstEnvelope, maxDstEnvelope,
                out rowMapTable, out colMapTable, progressCallback);
        }
        
        /// <summary>
        /// 这里的srcToDstSpatialXs，srcToDstSpatialYs是目标参考投影(dstSpatialRef)下的值
        /// </summary>
        public void ComputeIndexMapTable(double[] srcToDstSpatialXs, double[] srcToDstSpatialYs, Size srcSize,
                                        Size dstSize, PrjEnvelope dstEnvelope, PrjEnvelope maxDstEnvelope,
                                        out UInt16[] rowMapTable, out UInt16[] colMapTable, Action<int, string> progressCallback)
        {
            if (!maxDstEnvelope.IntersectsWith(dstEnvelope))
                throw new Exception("目标坐标数据不在指定区域范围内");
            double dstMinX = dstEnvelope.MinX;
            double dstMaxX = dstEnvelope.MaxX;
            double dstMaxY = dstEnvelope.MaxY;
            double dstMinY = dstEnvelope.MinY;
            double dstResolutionsX = dstEnvelope.Width / dstSize.Width;
            double dstResolutionsY = dstEnvelope.Height / dstSize.Height;
            int srcLength = srcSize.Width * srcSize.Height;
            int dstLength = dstSize.Width * dstSize.Height;
            int dstHeight = dstSize.Height;
            int dstWidth = dstSize.Width;
            int srcHeight = srcSize.Height;
            int srcWidth = srcSize.Width;
            byte[] lootUpTableTag = new byte[dstLength];    //投影蒙板，用于图像内外标识(0：外部,1：内部)
            UInt16[] tmpRowLookUpTable = new UInt16[dstLength];
            UInt16[] tmpColLookUpTable = new UInt16[dstLength];
            if (InvalidValue != 0)
            {
                for (int i = 0; i < dstLength; i++)
                {
                    tmpRowLookUpTable[i] = InvalidValue;
                    tmpColLookUpTable[i] = InvalidValue;
                }
            }
            int srcValidLeft = 0;
            int srcValidRight = srcWidth;
            Parallel.For((int)0, srcHeight, (j) =>
            {
                int srcIndex = 0;
                int dstIndex = 0;      //目标位置
                int dstJ;       //目标行
                int dsti;       //目标列
                double curX, curY;
                for (int i = srcValidLeft; i < srcValidRight; i++)
                {
                    srcIndex = (j * srcWidth + i);
                    curX = srcToDstSpatialXs[srcIndex];
                    curY = srcToDstSpatialYs[srcIndex];
                    dstJ = (int)((dstMaxY - curY) / dstResolutionsY + 0.5);     //Y方向计算出目标行 索引从0开始所以-1
                    dsti = (int)((curX - dstMinX) / dstResolutionsX + 0.5);     //X方向计算出目标列
                    if (dstJ >= 0 && dstJ < dstHeight && dsti >= 0 && dsti < dstWidth)
                    {
                        dstIndex = dstJ * dstWidth + dsti;
                        tmpRowLookUpTable[dstIndex] = (UInt16)j;
                        tmpColLookUpTable[dstIndex] = (UInt16)i;
                        lootUpTableTag[dstIndex] = 1;
                    }
                }
            });
            if (dstSize.Width >= 2 && dstSize.Height >= 2)
            {
                Interpolation.ChazhiNear(lootUpTableTag, dstSize.Width, dstSize.Height, tmpRowLookUpTable, tmpColLookUpTable);
            }
            rowMapTable = tmpRowLookUpTable;
            colMapTable = tmpColLookUpTable;
        }

        /// <summary>
        /// 这里的srcXs，srcYs是源参考投影(srcSpatialRef)下的值。
        /// 给出的栅格坐标分辨率与栅格数据分辨率不同情况下的投影,例如数据数据分辨率是0.0025度，栅格坐标分辨率是0.01度。
        /// </summary>
        public void ComputeIndexMapTable(ISpatialReference srcSpatialRef, double[] srcXs, double[] srcYs,Size srcSize, Size srcDataSize,
                                        ISpatialReference dstSpatialRef, Size dstSize, PrjEnvelope dstEnvelope,
                                        out UInt16[] rowMapTable, out UInt16[] colMapTable, Action<int, string> progressCallback)
        {
            PrjEnvelope maxDstEnvelope;
            ComputeDstEnvelope(srcSpatialRef, srcXs, srcYs, srcSize, dstSpatialRef, out maxDstEnvelope, progressCallback);
            ComputeIndexMapTable(srcXs, srcYs, srcSize,srcDataSize, dstSize, dstEnvelope,
                out rowMapTable, out colMapTable, progressCallback);
        }

        /// <summary> 
        /// 这里的srcToDstSpatialXs，srcToDstSpatialYs是目标参考投影(dstSpatialRef)下的值
        /// 此种方法适用于给出的原始经纬度数据尺寸小于其对应栅格数据的情况下使用。
        /// 给出的栅格坐标,分辨率与栅格数据分辨率不同情况下的投影,例如数据数据分辨率是0.0025度，栅格坐标分辨率是0.01度。
        /// </summary>
        /// <param name="srcToDstProjXs">x方向经纬度值或者公里数据集</param>
        /// <param name="srcToDstProjYs">y方向经纬度值或者公里数据集</param>
        /// <param name="srcJdSize">经纬度或公里数据集大小</param>
        /// <param name="srcDataSize">使用此经纬度或公里数据集的栅格数据大小</param>
        /// <param name="dstDataSize">投影目标大小</param>
        /// <param name="dstEnvelope">投影目标范围</param>
        /// <param name="rowMapTable">行查找表</param>
        /// <param name="colMapTable">列查找表</param>
        /// <param name="progressCallback"></param>
        public void ComputeIndexMapTable(double[] srcToDstProjXs, double[] srcToDstProjYs, Size srcJdSize, Size srcDataSize,
                                        Size dstDataSize, PrjEnvelope dstEnvelope,
                                        out UInt16[] rowMapTable, out UInt16[] colMapTable, Action<int, string> progressCallback)
        {
            ComputeIndexMapTable(srcToDstProjXs, srcToDstProjYs, srcJdSize, srcDataSize,
                                        dstDataSize, dstEnvelope,
                                        out rowMapTable, out colMapTable, progressCallback, 10);
        }

        /// <summary>
        /// 通过插值经纬度数据，生成查找表
        /// 线性外插：从实际情况看，不需要执行这个线性外插，即可消除bow-tie弯弓效应。即：每个整扫描带跳过最后一行的插值。
        /// 目前局部数据看到的情况是，Fy3AMERSI的原始数据已经处理了bow-tie，FY3B和FY3C的没处理。
        /// 本插值仅适用于经纬度放大的情况，比如放大1倍，放大两倍等等。
        /// </summary>
        /// <param name="srcToDstProjXs"></param>
        /// <param name="srcToDstProjYs"></param>
        /// <param name="srcJdSize"></param>
        /// <param name="srcDataSize"></param>
        /// <param name="dstDataSize"></param>
        /// <param name="dstEnvelope"></param>
        /// <param name="rowMapTable"></param>
        /// <param name="colMapTable"></param>
        /// <param name="progressCallback"></param>
        /// <param name="scanLineWidth"></param>
        public void ComputeIndexMapTable(double[] srcToDstProjXs, double[] srcToDstProjYs, Size srcJdSize, Size srcDataSize,
                                        Size dstDataSize, PrjEnvelope dstEnvelope,
                                        out UInt16[] rowMapTable, out UInt16[] colMapTable, Action<int, string> progressCallback, int scanLineWidth)
        {
            int dstHeight = dstDataSize.Height;
            int dstWidth = dstDataSize.Width;
            int srcHeight = srcJdSize.Height;
            int srcWidth = srcJdSize.Width;
            int srcDataWidth = srcDataSize.Width;
            int srcDataHeight = srcDataSize.Height;
            int srcLength = srcWidth * srcHeight;
            int srcDataLength = srcDataWidth * srcDataHeight;
            int dstLength = dstWidth * dstHeight;
            double dstResolutionsX = dstEnvelope.Width / dstWidth;
            double dstResolutionsY = dstEnvelope.Height / dstHeight;
            int slopeX = srcDataWidth / srcWidth;   //4,2
            int slopeY = srcDataHeight / srcHeight; //4,2
            double dstMinX = dstEnvelope.MinX;
            double dstMaxX = dstEnvelope.MaxX;
            double dstMaxY = dstEnvelope.MaxY;
            double dstMinY = dstEnvelope.MinY;
            double srcValidMinX = dstEnvelope.MinX - dstResolutionsX * slopeX * 2;
            double srcValidMaxX = dstEnvelope.MaxX + dstResolutionsX * slopeX;
            double srcValidMaxY = dstEnvelope.MaxY + dstResolutionsY * slopeY * 2;
            double srcValidMinY = dstEnvelope.MinY - +dstResolutionsY * slopeY;
            float factorX = (float)srcWidth / srcDataWidth;   //1/4;1/2
            float factorY = (float)srcHeight / srcDataHeight; //1/4;1/2
            byte[] lootUpTableTag = new byte[dstLength];//
            UInt16[] tmpRowLookUpTable = new UInt16[dstLength];// 
            UInt16[] tmpColLookUpTable = new UInt16[dstLength];//
            if (InvalidValue != 0)
            {
                for (int i = 0; i < dstLength; i++)
                {
                    tmpRowLookUpTable[i] = InvalidValue;
                    tmpColLookUpTable[i] = InvalidValue;
                }
            }
            //扫描带宽度：10//用于去除
            Parallel.For(0, srcHeight, (oldRow) =>  //oldRow、oldCol插值前的行，列
            {
                if (scanLineWidth > 1 && (oldRow + 1) % scanLineWidth == 0)//每条扫描线的最后一行。
                { }
                else//线性内插
                {
                    int destRowIndex, destColIndex;
                    int destIndex;
                    double curX, curY;
                    int dstCol;
                    float oldRowF, oldColF;
                    int newRow, newCol;
                    int dstRow;
                    int rowOffset = oldRow * srcWidth;
                    int srcIndex;
                    double srcX, srcY;
                    //临时存储插后的经纬度数据集
                    double[,] m_fTempLat = new double[slopeX, slopeY];
                    double[,] m_fTempLon = new double[slopeX, slopeY];
                    double p1, p2, p3, p4;
                    int v1, v2, v3, v4;
                    double pu1;
                    int vSrcWidth;
                    int v1SrcWidth;
                    int i, j;
                    double v, u;
                    //下面用双线性差值计算出放大后的经纬度(XY)值，公式如下：
                    //f(i+u,j+v)=(1-u)(1-v)f(i,j) + (1-u)vf(i,j+1) + u(1-v)f(i+1,j) + uvf(i+1,j+1)；
                    //p1=(1-u)(1-v),p2=(1-u)v,p3=u * (1 - v),p4=u * v
                    //
                    for (int oldCol = 0; oldCol < srcWidth; oldCol++)
                    {
                        srcIndex = rowOffset + oldCol;
                        srcX = srcToDstProjXs[srcIndex];
                        srcY = srcToDstProjYs[srcIndex];
                        if (srcY <= srcValidMaxY && srcY >= srcValidMinY && srcX >= srcValidMinX && srcX <= srcValidMaxX)
                        {
                            newRow = oldRow * slopeX;//记录插值后的行列号
                            newCol = oldCol * slopeX;
                            //下面使用双线性插值，插值放大后的区域的值
                            for (int blockRow = 0; blockRow < slopeY; blockRow++)
                            {
                                dstRow = blockRow + newRow;                 //目标行
                                oldRowF = dstRow * factorY;
                                i = (int)oldRowF; if (i > oldRowF) --i;     //x = (long)Math.Floor(fx);//取最大整数
                                u = oldRowF - i;
                                pu1 = 1 - u;
                                vSrcWidth = i * srcWidth;
                                v1SrcWidth = (i + 1) * srcWidth;
                                for (int blockCol = 0; blockCol < slopeX; blockCol++)
                                {
                                    dstCol = blockCol + newCol;           //目标列
                                    oldColF = dstCol * factorX;
                                    j = (int)oldColF; if (j > oldColF) --j;
                                    v = oldColF - j;
                                    if (i >= srcHeight - 1 && j >= srcWidth - 1)//>=最后一行，并且>=最后一列
                                    {
                                        v1 = vSrcWidth + j;
                                        p1 = pu1 * (1 - v);
                                        m_fTempLon[blockRow, blockCol] = p1 * srcToDstProjXs[v1];
                                        m_fTempLat[blockRow, blockCol] = p1 * srcToDstProjYs[v1];
                                    }
                                    else if (i >= srcHeight - 1)//最后一行
                                    {
                                        p1 = pu1 * (1 - v);
                                        p2 = pu1 * v;
                                        v1 = vSrcWidth + j;
                                        v2 = vSrcWidth + j + 1;
                                        m_fTempLon[blockRow, blockCol] =
                                            p1 * srcToDstProjXs[v1] +
                                            p2 * srcToDstProjXs[v2];
                                        m_fTempLat[blockRow, blockCol] =
                                            p1 * srcToDstProjYs[v1] +
                                            p2 * srcToDstProjYs[v2];
                                    }
                                    else if (j >= srcWidth - 1)//最后一列
                                    {
                                        p1 = pu1 * (1 - v);
                                        p3 = u * (1 - v);
                                        v1 = vSrcWidth + j;
                                        v3 = v1SrcWidth + j;
                                        m_fTempLon[blockRow, blockCol] =
                                            p1 * srcToDstProjXs[v1] +
                                            p3 * srcToDstProjXs[v3];
                                        m_fTempLat[blockRow, blockCol] =
                                            p1 * srcToDstProjYs[v1] +
                                            p3 * srcToDstProjYs[v3];
                                    }
                                    else//其他位置，双线性插值。
                                    {
                                        p1 = pu1 * (1 - v);
                                        p2 = pu1 * v;
                                        p3 = u * (1 - v);
                                        p4 = u * v;
                                        v1 = vSrcWidth + j;
                                        v2 = vSrcWidth + j + 1;
                                        v3 = v1SrcWidth + j;
                                        v4 = v1SrcWidth + j + 1;
                                        m_fTempLon[blockRow, blockCol] =
                                            p1 * srcToDstProjXs[v1] +
                                            p2 * srcToDstProjXs[v2] +
                                            p3 * srcToDstProjXs[v3] +
                                            p4 * srcToDstProjXs[v4];
                                        m_fTempLat[blockRow, blockCol] =
                                            p1 * srcToDstProjYs[v1] +
                                            p2 * srcToDstProjYs[v2] +
                                            p3 * srcToDstProjYs[v3] +
                                            p4 * srcToDstProjYs[v4];
                                    }
                                }
                            }
                            //计算查找表,将上面插好值的经纬度数据集映射到查找表
                            for (int blockRow = 0; blockRow < slopeY; blockRow++)
                            {
                                for (int blockCol = 0; blockCol < slopeX; blockCol++)
                                {
                                    curX = m_fTempLon[blockRow, blockCol];
                                    curY = m_fTempLat[blockRow, blockCol];
                                    if (curY <= dstMaxY && curY >= dstMinY && curX >= dstMinX && curX <= dstMaxX)
                                    {
                                        destRowIndex = (int)((dstMaxY - curY) / dstResolutionsY);//Y方向计算出目标行 
                                        destColIndex = (int)((curX - dstMinX) / dstResolutionsX);//X方向计算出目标列
                                        destIndex = destRowIndex * dstWidth + destColIndex;
                                        if (lootUpTableTag[destIndex] == 0 &&
                                            destRowIndex < dstHeight && destRowIndex >= 0 && destColIndex < dstWidth && destColIndex >= 0)
                                        {
                                            tmpRowLookUpTable[destIndex] = (UInt16)(oldRow * slopeX + blockRow);
                                            tmpColLookUpTable[destIndex] = (UInt16)(oldCol * slopeY + blockCol);
                                            lootUpTableTag[destIndex] = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
                );
            Interpolation.ChazhiNear(lootUpTableTag, dstDataSize.Width, dstDataSize.Height, tmpRowLookUpTable, tmpColLookUpTable);
            rowMapTable = tmpRowLookUpTable;
            colMapTable = tmpColLookUpTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcRaster"></param>
        /// <param name="srcSize"></param>
        /// <param name="rowMapTable"></param>
        /// <param name="colMapTable"></param>
        /// <param name="mapSize"></param>
        /// <param name="dstRaster"></param>
        /// <param name="destFillValue"></param>
        /// <param name="mapTableInvalidValue">默认是UInt16.MaxValue</param>
        /// <param name="progressCallback"></param>
        public void Project<T>(T[] srcRaster, Size srcSize, UInt16[] rowMapTable, UInt16[] colMapTable, Size mapSize, T[] dstRaster, T destFillValue,UInt16 mapTableInvalidValue,
            Action<int, string> progressCallback)
        {
            int lutHeight = mapSize.Height;
            int lutWidth = mapSize.Width;
            int srcHeight = srcSize.Height;
            int srcWidth = srcSize.Width;
            Parallel.For(0, lutHeight, (rowIndex, parallelLoopState) =>
            {
                int destIndex;
                int srcCol, srcRow;
                int rowOffset = rowIndex * lutWidth;
                for (int j = 0; j < lutWidth; j++)
                {
                    destIndex = rowOffset + j;                     //当前目标数据的位置
                    if (colMapTable[destIndex] == mapTableInvalidValue) //LUTFillValue,查找表的默认填充值,现在设置为0
                    {
                        dstRaster[destIndex] = destFillValue;
                    }
                    else
                    {
                        srcRow = rowMapTable[destIndex];           //取出对应的原始行号
                        srcCol = colMapTable[destIndex];           //取出对应的原始列号
                        if (srcCol < 0 || srcRow < 0 || srcRow >= srcHeight || srcCol >= srcWidth)
                        {
                            dstRaster[destIndex] = destFillValue;
                        }
                        else
                        {
                            dstRaster[destIndex] = srcRaster[srcRow * srcWidth + srcCol];
                        }
                    }
                }
            });
        }

        public void Project<T>(T[] srcRaster, Size srcSize, UInt16[] rowMapTable, UInt16[] colMapTable, Size mapSize, T[] dstRaster, T destFillValue,
            Action<int, string> progressCallback)
        {
            int lutHeight = mapSize.Height;
            int lutWidth = mapSize.Width;
            int srcHeight = srcSize.Height;
            int srcWidth = srcSize.Width;
            Parallel.For(0, lutHeight, (rowIndex, parallelLoopState) =>
            {
                int destIndex;
                int srcCol, srcRow;
                int rowOffset = rowIndex * lutWidth;
                for (int j = 0; j < lutWidth; j++)
                {
                    destIndex = rowOffset + j;                     //当前目标数据的位置
                    if (colMapTable[destIndex] == InvalidValue)    //LUTFillValue,查找表的默认填充值
                    {
                        dstRaster[destIndex] = destFillValue;
                    }
                    else
                    {
                        srcRow = rowMapTable[destIndex];           //取出对应的原始行号
                        srcCol = colMapTable[destIndex];           //取出对应的原始列号
                        if (srcCol < 0 || srcRow < 0 || srcRow >= srcHeight || srcCol >= srcWidth)
                        {
                            dstRaster[destIndex] = destFillValue;
                        }
                        else
                        {
                            dstRaster[destIndex] = srcRaster[srcRow * srcWidth + srcCol];
                        }
                    }
                }
            });
        }

        public void ProjectNew<T>(T[,] srcRaster, Size srcSize, UInt16[] rowMapTable, UInt16[] colMapTable, Size mapSize, T[,] dstRaster, T destFillValue)
        {
            int lutHeight = mapSize.Height;
            int lutWidth = mapSize.Width;
            int srcHeight = srcSize.Height;
            int srcWidth = srcSize.Width;
            Parallel.For(0, lutHeight, (rowIndex, parallelLoopState) =>
            {
                int destIndex;
                int srcCol, srcRow;
                int rowOffset = rowIndex * lutWidth;
                for (int j = 0; j < lutWidth; j++)
                {
                    destIndex = rowOffset + j;                     //当前目标数据的位置
                    if (colMapTable[destIndex] == InvalidValue)    //LUTFillValue,查找表的默认填充值
                    {
                        dstRaster[rowIndex, j] = destFillValue;
                    }
                    else
                    {
                        srcRow = rowMapTable[destIndex];           //取出对应的原始行号
                        srcCol = colMapTable[destIndex];           //取出对应的原始列号
                        if (srcCol < 0 || srcRow < 0 || srcRow >= srcHeight || srcCol >= srcWidth)
                        {
                            dstRaster[rowIndex, j] = destFillValue;
                        }
                        else
                        {
                            dstRaster[rowIndex, j] = srcRaster[srcRow, srcCol];
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 根据给出的经纬度数据集或者投影坐标数据集，计算其是否在指定的范围内
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="validEnv"></param>
        /// <param name="oSpatialRef"></param>
        /// <param name="tSpatialRef"></param>
        /// <returns></returns>
        public bool HasVaildEnvelope(double[] xs, double[] ys, PrjEnvelope validEnv, ISpatialReference oSpatialRef, ISpatialReference tSpatialRef)
        {
            if (validEnv == null || validEnv.IsEmpty)
                throw new ArgumentNullException("validEnv", "参数[有效范围]不能为空");
            if (tSpatialRef == null)
                tSpatialRef = SpatialReference.GetDefault();
            if (oSpatialRef == null)
                oSpatialRef = SpatialReference.GetDefault();
            using (IProjectionTransform transform = ProjectionTransformFactory.GetProjectionTransform(oSpatialRef, tSpatialRef))
            {
                if (!oSpatialRef.IsSame(tSpatialRef))
                    transform.Transform(xs, ys);
                bool isLatLong = (tSpatialRef.ProjectionCoordSystem == null && tSpatialRef.GeographicsCoordSystem != null);
                return PrjEnvelope.ValidEnvelope(xs, ys, validEnv);
            }
        }

        /// <summary>
        /// 根据给出的经纬度数据集或者投影坐标数据集，计算其是否在指定的范围内，并且计算出有效率，以及实际输出范围
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="validEnv"></param>
        /// <param name="oSpatialRef"></param>
        /// <param name="tSpatialRef"></param>
        /// <param name="validRate"></param>
        /// <param name="outEnv"></param>
        /// <returns></returns>
        public bool VaildEnvelope(double[] xs, double[] ys, PrjEnvelope validEnv, ISpatialReference oSpatialRef, ISpatialReference tSpatialRef, out double validRate, out PrjEnvelope outEnv)
        {
            if (validEnv == null || validEnv.IsEmpty)
                throw new ArgumentNullException("validEnv", "参数[有效范围]不能为空");
            if (tSpatialRef == null)
                tSpatialRef = SpatialReference.GetDefault();
            if (oSpatialRef == null)
                oSpatialRef = SpatialReference.GetDefault();
            using (IProjectionTransform transform = ProjectionTransformFactory.GetProjectionTransform(oSpatialRef, tSpatialRef))
            {
                if (!oSpatialRef.IsSame(tSpatialRef))
                    transform.Transform(xs, ys);
                bool isLatLong = (tSpatialRef.ProjectionCoordSystem == null && tSpatialRef.GeographicsCoordSystem != null);
                return PrjEnvelope.HasValidEnvelope(xs, ys, validEnv, out validRate, out outEnv);
            }
        }

    }
}
