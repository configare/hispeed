using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HJ
{
    public class HJSatelliteDef
    {
        public static string[] HJSatelliteAttList()
        {
            List<string> attList = new List<string>();
            attList.Add("productId");
            attList.Add("sceneId");
            attList.Add("satelliteId");
            attList.Add("sensorId");
            attList.Add("recStationId");
            attList.Add("productDate");
            attList.Add("productLevel");
            attList.Add("pixelSpacing");
            attList.Add("productType");
            attList.Add("sceneCount");
            attList.Add("sceneShift");
            attList.Add("overallQuality");
            attList.Add("satPath");
            attList.Add("satRow");
            attList.Add("satPathbias");
            attList.Add("satRowbias");
            attList.Add("scenePath");
            attList.Add("sceneRow");
            attList.Add("scenePathbias");
            attList.Add("sceneRowbias");
            attList.Add("direction");
            attList.Add("sunElevation");
            attList.Add("sunAzimuthElevation");
            attList.Add("recStationID");
            attList.Add("sceneDate");
            attList.Add("sceneTime");
            attList.Add("instrumentMode");
            attList.Add("imagingStartTime");
            attList.Add("imagingStopTime");
            attList.Add("gain");
            attList.Add("satOffNadir");
            attList.Add("mirrorOffNadir");
            attList.Add("bands");
            attList.Add("absCalibType");
            attList.Add("mtfcProMode");
            attList.Add("radioMatricMethod");
            attList.Add("addWindow");
            attList.Add("correctPhase");
            attList.Add("reconstructProcess");
            attList.Add("earthModel");
            attList.Add("mapProjection");
            attList.Add("resampleTechnique");
            attList.Add("productOrientation");
            attList.Add("ephemerisData");
            attList.Add("attitudeData");
            attList.Add("sceneCenterLat");
            attList.Add("sceneCenterLong");
            attList.Add("dataUpperLeftLat");
            attList.Add("dataUpperLeftLong");
            attList.Add("dataUpperRightLat");
            attList.Add("dataUpperRightLong");
            attList.Add("dataLowerLeftLat");
            attList.Add("dataLowerLeftLong");
            attList.Add("dataLowerRightLat");
            attList.Add("dataLowerRightLong");
            attList.Add("productUpperLeftLat");
            attList.Add("productUpperLeftLong");
            attList.Add("productUpperRightLat");
            attList.Add("productUpperRightLong");
            attList.Add("productLowerLeftLat");
            attList.Add("productLowerLeftLong");
            attList.Add("productLowerRightLat");
            attList.Add("productLowerRightLong");
            attList.Add("dataUpperLeftX");
            attList.Add("dataUpperLeftY");
            attList.Add("dataUpperRightX");
            attList.Add("dataUpperRightY");
            attList.Add("dataLowerLeftX");
            attList.Add("dataLowerLeftY");
            attList.Add("dataLowerRightX");
            attList.Add("dataLowerRightY");
            attList.Add("productUpperLeftX");
            attList.Add("productUpperLeftY");
            attList.Add("productUpperRightX");
            attList.Add("productUpperRightY");
            attList.Add("productLowerLeftX");
            attList.Add("productLowerLeftY");
            attList.Add("productLowerRightX");
            attList.Add("productLowerRightY");
            attList.Add("isSimulateData");
            attList.Add("dataFormatDes");
            attList.Add("delStatus");
            attList.Add("dataTempDir");
            attList.Add("dataArchiveDir");
            attList.Add("browseArchiveDir");
            attList.Add("browseDirectory");
            attList.Add("browseFileLocation");
            return attList.ToArray();
        }
    }
}
