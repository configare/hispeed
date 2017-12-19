using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    /// <summary>
    /// L1_20080808_0706
    /// </summary>
    public class GenericFilename
    {
        Regex _prjRegex;

        public GenericFilename()
        {
            string pattern = @"(?<level>L1)_(?<date>\d{8})_(?<time>\d{0,4})";
            _prjRegex = new Regex(pattern, RegexOptions.Compiled);
        }

        public string GenericPrjFilename(string orbitFilename, string projectionIdentify)
        {
            string filenameOnly = Path.GetFileName(orbitFilename);
            string retFilename;
            if (_prjRegex.IsMatch(filenameOnly))
            {
                Match match = _prjRegex.Match(filenameOnly);
                string str = match.Value;
                retFilename = filenameOnly.Insert(match.Index - 1, "_" + projectionIdentify);
                retFilename = _prjRegex.Replace(filenameOnly, projectionIdentify + "_" + str);
                retFilename = Path.ChangeExtension(retFilename, ".LDF");
            }
            else
                retFilename = Path.GetFileNameWithoutExtension(orbitFilename) + "_" + projectionIdentify + ".LDF";
            string dir = Path.GetDirectoryName(orbitFilename);
            return Path.Combine(dir, retFilename);
        }

        /// <summary>
        /// 
        /// -->
        ///  FY3A_VIRR_06_GLL_L1_20120909_Day3_1000M.LDF
        /// </summary>
        /// <param name="orbitFilename"></param>
        /// <param name="projectionIdentify"></param>
        /// <param name="blockName"></param>
        /// <returns></returns>
        public string PrjBlockFilename(string orbitFilename, string projectionIdentify, string blockName, string extName)
        {
            string dir = Path.GetDirectoryName(orbitFilename);
            string filenameOnly = Path.GetFileName(orbitFilename);
            string retFilename;
            if (_prjRegex.IsMatch(filenameOnly))
            {
                Match match = _prjRegex.Match(filenameOnly);
                string str = match.Value;
                retFilename = filenameOnly.Insert(match.Index - 1, "_" + projectionIdentify);
                retFilename = _prjRegex.Replace(filenameOnly, projectionIdentify + "_" + str);
                retFilename = Path.ChangeExtension(retFilename, extName);
                string[] idSplit = retFilename.Split('_');
                if (idSplit.Length > 6 && !string.IsNullOrWhiteSpace(blockName))
                {
                    retFilename = Regex.Replace(retFilename, "_" + idSplit[2], "_" + blockName);
                }
            }
            else
                retFilename = Path.GetFileNameWithoutExtension(orbitFilename) + "_" + projectionIdentify + "_" + blockName + extName;
            return Path.Combine(dir, retFilename);
        }

        public string GetL1PrjFilename(string satellite, string sensor, DateTime orbitTime, string orbitFilename, string prjIdentify, string blockName, float resolution, string extName)
        {
            if (string.IsNullOrWhiteSpace(satellite) || string.IsNullOrWhiteSpace(sensor))
                return PrjBlockFilename(orbitFilename, prjIdentify, blockName,extName);
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}{8}",
                satellite.Replace("-", ""),
                sensor.Replace("-", ""),
                blockName,
                prjIdentify,
                "L1",
                orbitTime.ToString("yyyyMMdd"),
                orbitTime.ToString("HHmm"),
                prjIdentify == "GLL" ? GLLResolutionIdentify(resolution) : ResolutionIdentify(resolution),
                extName
                );
        }

        private string ResolutionIdentify(float resolution)
        {
            if (resolution == 1000f)
                return "1000M";
            else if (resolution == 250f)
                return "0250M";
            else if (resolution == 500f)
                return "0500M";
            else if (resolution == 5000f)
                return "5000M";
            else if (resolution == 10000f)
                return "010KM";
            else if (resolution == 25000f)
                return "025KM";
            else if (resolution == 100000f)
                return "100KM";
            else
                return "00000";
        }

        /// <summary>
        /// 0250M、0500M、1000M、5000M、010KM、025KM、100KM、00000
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private string GLLResolutionIdentify(float resolution)
        {
            if (resolution == 0.01f)
                return "1000M";
            else if (resolution == 0.0025f)
                return "0250M";
            else if (resolution == 0.005f)
                return "0500M";
            else if (resolution == 0.05f)
                return "5000M";
            else if (resolution == 0.1f)
                return "010KM";
            else if (resolution == 0.25f)
                return "025KM";
            else if (resolution == 1.0f)
                return "100KM";
            else
                return "00000";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prjName">
        /// projRef.GeographicsCoordSystem.Name 
        /// projRef.ProjectionCoordSystem.Name.Name</param>
        /// <returns></returns>
        public static string GetProjectionIdentify(string prjName)
        {
            switch (prjName)
            {
                case "GCS_WGS_1984":
                case "等经纬度投影":
                case "GLL":
                    return "GLL";
                case "Polar Stereographic":
                    return "PSG";
                case "Albers Conical Equal Area":
                    return "AEA";
                case "Lambert Conformal Conic":
                    return "LBT";
                case "Mercator":
                    return "MCT";
                case "Hammer":
                    return "HAM";
                default:
                    return "PRJ";
            }
        }

    }
}
