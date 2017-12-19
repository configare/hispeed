using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public enum LFRWFlag
    {
        /*! Read data */
        LF_Read = 0,
        /*! Write data */
        LF_Write = 1
    }

    public enum LDataType
    {
        L_Unknown = 0,
        /*! Eight bit unsigned integer */
        L_Byte = 1,
        /*! Sixteen bit unsigned integer */
        L_UInt16 = 2,
        /*! Sixteen bit signed integer */
        L_Int16 = 3,
        /*! Thirty two bit unsigned integer */
        L_UInt32 = 4,
        /*! Thirty two bit signed integer */
        L_Int32 = 5,
        /*! Thirty two bit floating point */
        L_Float32 = 6,
        /*! Sixty four bit floating point */
        L_Float64 = 7,
        /*! Complex Int16 */
        L_CInt16 = 8,
        /*! Complex Int32 */
        L_CInt32 = 9,
        /*! Complex Float32 */
        L_CFloat32 = 10,
        /*! Complex Float64 */
        L_CFloat64 = 11,
        L_TypeCount = 12,         /* maximum type # + 1 */
        L_RGB = 13,                /* 全色图像 */
        L_ARGB = 14                /* 带有透明度的全色图像 */
    }


    public enum CPLErr
    {
        CE_None = 0,
        CE_Log = 1,
        CE_Warning = 2,
        CE_Failure = 3,
        CE_Fatal = 4
    }

    public enum GDALGridAlgorithm
    {
        /*! Inverse distance to a power */
        GGA_InverseDistanceToAPower = 1,
        /*! Moving Average */
        GGA_MovingAverage = 2,
        /*! Nearest Neighbor */
        GGA_NearestNeighbor = 3,
        /*! Minimum Value (Data Metric) */
        GGA_MetricMinimum = 4,
        /*! Maximum Value (Data Metric) */
        GGA_MetricMaximum = 5,
        /*! Data Range (Data Metric) */
        GGA_MetricRange = 6,
        /*! Number of Points (Data Metric) */
        GGA_MetricCount = 7,
        /*! Average Distance (Data Metric) */
        GGA_MetricAverageDistance = 8,
        /*! Average Distance Between Data Points (Data Metric) */
        GGA_MetricAverageDistancePts = 9
    }

    /** Inverse distance to a power method control options */
    public struct GDALGridInverseDistanceToAPowerOptions
    {
        /*! Weighting power. */
        double dfPower;
        /*! Smoothing parameter. */
        double dfSmoothing;
        /*! Reserved for future use. */
        double dfAnisotropyRatio;
        /*! Reserved for future use. */
        double dfAnisotropyAngle;
        /*! The first radius (X axis if rotation angle is 0) of search ellipse. */
        double dfRadius1;
        /*! The second radius (Y axis if rotation angle is 0) of search ellipse. */
        double dfRadius2;
        /*! Angle of ellipse rotation in degrees.
         *
         * Ellipse rotated counter clockwise.
         */
        double dfAngle;
        /*! Maximum number of data points to use.
         *
         * Do not search for more points than this number.
         * If less amount of points found the grid node considered empty and will
         * be filled with NODATA marker.
         */
        UInt32 nMaxPoints;
        /*! Minimum number of data points to use.
         *
         * If less amount of points found the grid node considered empty and will
         * be filled with NODATA marker.
         */
        UInt32 nMinPoints;
        /*! No data marker to fill empty points. */
        double dfNoDataValue;
    } ;

    /** Moving average method control options */
    public struct GDALGridMovingAverageOptions
    {
        /*! The first radius (X axis if rotation angle is 0) of search ellipse. */
        double dfRadius1;
        /*! The second radius (Y axis if rotation angle is 0) of search ellipse. */
        double dfRadius2;
        /*! Angle of ellipse rotation in degrees.
         *
         * Ellipse rotated counter clockwise.
         */
        double dfAngle;
        /*! Minimum number of data points to average.
         *
         * If less amount of points found the grid node considered empty and will
         * be filled with NODATA marker.
         */
        UInt32 nMinPoints;
        /*! No data marker to fill empty points. */
        double dfNoDataValue;
    } ;

    /** Nearest neighbor method control options */
    public struct GDALGridNearestNeighborOptions
    {
        /*! The first radius (X axis if rotation angle is 0) of search ellipse. */
        public double dfRadius1;
        /*! The second radius (Y axis if rotation angle is 0) of search ellipse. */
        public double dfRadius2;
        /*! Angle of ellipse rotation in degrees.
         *
         * Ellipse rotated counter clockwise.
         */
        public double dfAngle;
        /*! No data marker to fill empty points. */
        public double dfNoDataValue;
    } ;

    /** Data metrics method control options */
    public struct GDALGridDataMetricsOptions
    {
        /*! The first radius (X axis if rotation angle is 0) of search ellipse. */
        double dfRadius1;
        /*! The second radius (Y axis if rotation angle is 0) of search ellipse. */
        double dfRadius2;
        /*! Angle of ellipse rotation in degrees.
         *
         * Ellipse rotated counter clockwise.
         */
        double dfAngle;
        /*! Minimum number of data points to average.
         *
         * If less amount of points found the grid node considered empty and will
         * be filled with NODATA marker.
         */
        UInt32 nMinPoints;
        /*! No data marker to fill empty points. */
        double dfNoDataValue;
    }

   

    public delegate int GDALProgressFunc(double dfComplete, string pszMessage, IntPtr pProgressArg);

}
