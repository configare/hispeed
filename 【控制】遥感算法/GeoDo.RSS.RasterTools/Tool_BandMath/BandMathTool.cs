using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public class BandMathTool : IRasterTool, IBandMathTool
    {
        public void Compute(IRasterDataProvider dataProvider, string expression, IRasterBand dstRasterBand, Action<int, string> progressTracker)
        {
            try
            {
                IBandMathExecutor exe = ClassRuntimeGenerator.GenerateBandMathExecutor(GetDataType(dataProvider.DataType), GetDataType(dstRasterBand.DataType));
                exe.Compute(dataProvider, expression, dstRasterBand, progressTracker);
            }
            catch(ArgumentOutOfRangeException) 
            {
                throw new ArgumentOutOfRangeException("波段计算公式引用的波段序号超出有效范围！");
            }
        }

        private string GetDataType(enumDataType dataType)
        {
            return DataTypeHelper.Enum2DataType(dataType).ToString();
        }

        public void Compute(IRasterDataProvider dataProvider, string expression, string outDriver, string outFile, Action<int, string> progressTracker)
        {
            if (dataProvider == null || string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(outDriver) || string.IsNullOrWhiteSpace(outFile))
                throw new ArgumentNullException();
            IGeoDataDriver driver = GeoDataDriver.GetDriverByName(outDriver);
            if (driver == null)
                throw new Exception("driver '" + outDriver + "' is not existed.");
            IRasterDataProvider dstProvider = CreateDstDataProvider(driver as IRasterDataDriver, outFile, dataProvider);
            if (dataProvider == null)
                throw new Exception("use '" + outDriver + "'create RasterDataProvider is failed.");
            try
            {
                Compute(dataProvider, expression, dstProvider.GetRasterBand(1), progressTracker);
            }
            finally
            {
                dstProvider.Dispose();
                driver.Dispose();
            }
        }

        private IRasterDataProvider CreateDstDataProvider(IRasterDataDriver driver, string outFile, IRasterDataProvider dataProvider)
        {
            ISpatialReference spatialRef = dataProvider.SpatialRef ?? new SpatialReference(new GeographicCoordSystem());
            string spRef = "SPATIALREF=" + spatialRef.ToProj4String();
            string mapInf = dataProvider.CoordEnvelope.ToMapInfoString(new Size(dataProvider.Width, dataProvider.Height));
            return driver.Create(outFile, dataProvider.Width, dataProvider.Height, 1, enumDataType.Float, spRef, mapInf, "WITHHDR=TRUE");
        }
    }
}
