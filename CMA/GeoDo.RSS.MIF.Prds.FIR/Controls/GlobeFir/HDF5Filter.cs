using System;
using System.Collections.Generic;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using FYTools;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class HDF5Filter
    {
        public static void SaveAsNewHDF5ByFeatures(string src, string dst, Feature[] features, Action<int, string> progress)
        {
            int width = features[0].FieldValues.Length;
            int height = features.Length;
            using (HDF5Helper helperSrc = new HDF5Helper(src, false))
            {
                using (HDF5Helper helperDst = new HDF5Helper(dst, true))
                {
                    //写文件属性
                    Dictionary<string, string> fileAttributes = helperSrc.GetFileAttributes();
                    foreach (KeyValuePair<string, string> fileAttribute in fileAttributes)
                    {
                        helperDst.WriteFileAttribute(fileAttribute.Key, fileAttribute.Value);
                    }
                    List<string> datasetNames = helperSrc.DatasetNames;
                    foreach (string datasetName in datasetNames)
                    {
                        //写数据集
                        string datasetType = helperSrc.GetDatasetType(datasetName);
                        int bandN = 0, bandH = 0, bandW = 0;
                        switch (datasetType)
                        {
                            case "FLOAT":
                                {
                                    float[] data = new float[width * height];
                                    helperSrc.ReadDataArray<float>(datasetName, ref bandN, ref bandH, ref bandW);
                                    int index = 0;
                                    int pct = 0;
                                    foreach (Feature f in features)
                                    {
                                        if (progress != null)
                                            progress(pct++, null);
                                        foreach (string s in f.FieldValues)
                                        {
                                            data[index++] = Convert.ToSingle(s);
                                        }
                                    }
                                    //helperDst.WriteDataArray<float>(datasetName, data, bandN, bandH, bandW);
                                    helperDst.WriteDataArray<float>(datasetName, data, bandN, height, width);//zyb,20140424
                                    break;
                                }
                        }
                        //写数据集属性
                        Dictionary<string, string> datasetAttributes = helperSrc.GetDatasetAttributes(datasetName);
                        foreach (KeyValuePair<string, string> datasetAttribute in datasetAttributes)
                        {
                            helperDst.WriteDatasetAttribute(datasetName, datasetAttribute.Key, datasetAttribute.Value);
                        }
                    }
                }
            }
        }

        public static void GetDataCoordEnvelope(string hdffname, out CoordEnvelope env)
        {
            env = null;
            try
            {
                double lulat = 0, lulon = 0, rdlat = 0, rdlon = 0;
                double ldlat = 0, ldlon = 0, rulat = 0, rulon = 0;
                double lulatorbit=0,lulonorbit=0,rdlatorbit=0,rdlonorbit=0,ldlatorbit=0,ldlonorbit=0,rulatorbit=0,rulonorbit=0;
                string lulatstr = "Left-Top Latitude", lulonstr = "Left-Top Longitude", rdlatstr = "Right-Bottom Latitude", rdlonstr = "Right-Bottom Longitude";
                string ldlatstr = "Left-Bottom Latitude", ldlonstr = "Left-Bottom Longitude", rulatstr = "Right-Top Latitude", rulonstr = "Right-Top Longitude";
                string lulatstrOrbit = "Left-Top X", lulonstrOrbit = "Left-Top Y", rdlatstrOrbit = "Right-Bottom X", rdlonstrOrbit = "Right-Bottom Y";
                string ldlatstrOrbit = "Left-Bottom X", ldlonstrOrbit = "Left-Bottom Y", rulatstrOrbit = "Right-Top X", rulonstrOrbit = "Right-Top Y";
                HDF5Helper helperSrc = new HDF5Helper(hdffname, false);
                Dictionary<string, string> fileAttributes = helperSrc.GetFileAttributes();
                foreach (KeyValuePair<string, string> fileAttribute in fileAttributes)
                {
                    if (fileAttribute.Key == lulatstr)
                        lulat = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == lulonstr)
                        lulon = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rdlatstr )
                        rdlat = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rdlonstr)
                        rdlon = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == ldlonstr)
                        ldlon = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == ldlatstr)
                        ldlat = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rulonstr)
                        rulon = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rulatstr)
                        rulat = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == lulatstrOrbit)
                        lulatorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == lulonstrOrbit)
                        lulonorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == ldlatstrOrbit)
                        ldlatorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == ldlonstrOrbit)
                        ldlonorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rdlatstrOrbit)
                        rdlatorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rdlonstrOrbit)
                        rdlonorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rulatstrOrbit)
                        rulatorbit = double.Parse(fileAttribute.Value);
                    else if (fileAttribute.Key == rulonstrOrbit)
                        rulonorbit = double.Parse(fileAttribute.Value);
                }
                double minlon, maxlon, minlat, maxlat;
                bool lonreverse = false, latreverse = false;
                if (lulon != 0 || rdlon != 0 || rdlat != 0 || lulat != 0 || ldlat != 0 || ldlon != 0 || rulat != 0 || rulon!= 0)
                {
                    if (lulon > rdlon)
                        lonreverse = true;
                    if (rdlat > lulat)
                        latreverse = true;
                    if (!lonreverse)
                    {
                        lulon = Math.Min(lulon, ldlon);
                        rdlon = Math.Max(rdlon, rulon);
                    }
                    else
                    {
                        lulon = Math.Max(lulon, ldlon);
                        rdlon = Math.Min(rdlon, rulon);
                    }
                    if (!latreverse)
                    {
                        lulat = Math.Max(lulat, rulat);
                        rdlat = Math.Min(ldlat, rdlat);
                    }
                    else
                    {
                        lulat = Math.Min(lulat, rulat);
                        rdlat = Math.Max(rdlat, ldlat);
                    }
                    minlon = !lonreverse ? lulon : rdlon;
                    maxlon = lonreverse ? lulon : rdlon;
                    minlat =!latreverse?rdlat:lulat;
                    maxlat = latreverse ? rdlat : lulat;
                }
                else
                {
                    lulon = lulonorbit < ldlonorbit ? lulonorbit : ldlonorbit;//左侧经度取小的
                    lulat = lulatorbit > rulatorbit ? lulatorbit : rulatorbit;//上侧纬度取大的
                    rdlon = rulonorbit > rdlonorbit ? rulonorbit : rdlonorbit;//右侧经度取大的
                    rdlat = ldlatorbit < rdlatorbit ? ldlatorbit : rdlatorbit;//下侧纬度取小的
                    minlon = lulon < rdlon ? lulon : rdlon;
                    maxlon = lulon > rdlon ? lulon : rdlon;
                    minlat = rdlat < lulat ? rdlat : lulat;
                    maxlat = rdlat > lulat ? rdlat : lulat;
                }
                env = new CoordEnvelope(minlon, maxlon, minlat, maxlat);
            }
            catch (System.Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

    }
}
