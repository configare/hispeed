using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class Datasets2Prds
    {
        public static string[] _setgroups = new string[] { "Box identification", "Cloud amount (CA)", "Mean cloud top pressure (PC)", "Mean cloud top temperature (TC)", "Mean cloud optical thickness (TAU)", "Mean cloud water path (WP)", "IR cloud types", "LOW cloud types (vis-adjusted TC)", "MIDDLE cloud types (VIS-adjusted TC)", "HIGH cloud types (VIS-adjusted TC)", "Mean surface temperature (TS)", "Mean surface reflectance (RS)", "Snow Ice cover", "TOVS atmospheric information" };
        public static string[] _prds = new string[] { "00", "CA", "PC", "TC", "TAU", "WP", "IRCT", "LOWCT", "MIDCT", "HIGHCT", "TS", "RS", "SIC", "TOVS" };
        
        public  Datasets2Prds()
        {
        }

        /// <summary>
        /// 数据集与子产品标识之间的对应关系
        /// </summary>
        /// <param name="sets2subPrds"></param>
        public static void DataSets2subPrds(out Dictionary<String, String> sets2subPrds)
        {
            sets2subPrds = new Dictionary<String, String>();
            //sets2subPrds.Add("Latitude", 1);
            //sets2subPrds.Add("Longitude index (equal-area)", 2);
            //sets2subPrds.Add("Western-most longitude index (equal-angle)", 3);
            //sets2subPrds.Add("Eastern-most longitude index (equal-angle)", 4);
            sets2subPrds.Add("Land water coast code", "COVT");//5
            sets2subPrds.Add("Number of observations", "NO");//6
            sets2subPrds.Add("Number of daytime observations", "DTNO");
            sets2subPrds.Add("Mean cloud amount", "MCA");//8
            sets2subPrds.Add("Mean IR-marginal cloud amount", "MIRCA");
            sets2subPrds.Add("Frequency of mean cloud amount = 0-10%", "MCAF1");
            sets2subPrds.Add("Frequency of mean cloud amount = 10-20%", "MCAF2");
            sets2subPrds.Add("Frequency of mean cloud amount = 20-30%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 30-40%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 40-50%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 50-60%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 60-70%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 70-80%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 80-90%", "MCAF");
            sets2subPrds.Add("Frequency of mean cloud amount = 90-100%", "MCAF");
            sets2subPrds.Add("Mean cloud top pressure(PC)", "MPC");
            sets2subPrds.Add("Standard deviation of spatial mean over time(PC)", "SDT");
            sets2subPrds.Add("Time mean of standard deviation over space(PC)", "MSDS");//23
            sets2subPrds.Add("Cloud temperature(TC)", "MTC");
            sets2subPrds.Add("Standard deviation of spatial mean over time(TC)", "SDT");
            sets2subPrds.Add("Time mean of standard deviation over space(TC)", "MSDS");
            sets2subPrds.Add("Mean cloud optical thickness(TAU)", "MTAU");//26
            sets2subPrds.Add("Standard deviation of spatial mean over time(TAU)", "SDT");
            sets2subPrds.Add("Time mean of standard deviation over space(TAU)", "MSDS");
            sets2subPrds.Add("Mean cloud water path(WP)", "MWP");//29
            sets2subPrds.Add("Standard deviation of spatial mean over time(WP)", "SDT");//20
            sets2subPrds.Add("Time mean of standard deviation over space(WP)", "MSDS");
            sets2subPrds.Add("Mean CA for low-level clouds", "MCAL");
            sets2subPrds.Add("Mean PC for low-level clouds", "MPCL");
            sets2subPrds.Add("Mean TC for low-level clouds", "MTCL");
            sets2subPrds.Add("Mean CA for middle-level clouds", "MCAM");
            sets2subPrds.Add("Mean PC for middle-level clouds", "MPCM");
            sets2subPrds.Add("Mean TC for middle-level clouds", "MTCM");
            sets2subPrds.Add("Mean CA for high-level clouds", "MCAH");
            sets2subPrds.Add("Mean PC for high-level clouds", "MPCH");
            sets2subPrds.Add("Mean TC for high-level clouds", "MTCH");

            sets2subPrds.Add("Mean CA for cloud type 1 = Cumulus, liquid", "MCA1");
            sets2subPrds.Add("Mean PC for cloud type 1 = Cumulus, liquid", "MPC1");
            sets2subPrds.Add("Mean TC for cloud type 1 = Cumulus, liquid", "MTC1");
            sets2subPrds.Add("Mean TAU for cloud type 1 = Cumulus, liquid", "MTAU1");
            sets2subPrds.Add("Mean WP for cloud type 1 = Cumulus, liquid", "MWP1");

            sets2subPrds.Add("Mean CA for cloud type 2 = Stratocumulus, liquid", "MCA2");
            sets2subPrds.Add("Mean PC for cloud type 2 = Stratocumulus, liquid", "MPC2");
            sets2subPrds.Add("Mean TC for cloud type 2 = Stratocumulus, liquid", "MTC2");
            sets2subPrds.Add("Mean TAU for cloud type 2 = Stratocumulus, liquid", "MTAU2");
            sets2subPrds.Add("Mean WP for cloud type 2 = Stratocumulus, liquid", "MWP2");

            sets2subPrds.Add("Mean CA for cloud type 3 = Stratus, liquid", "MCA3");
            sets2subPrds.Add("Mean PC for cloud type 3 = Stratus, liquid", "MPC3");
            sets2subPrds.Add("Mean TC for cloud type 3 = Stratus, liquid", "MTC3");
            sets2subPrds.Add("Mean TAU for cloud type 3 = Stratus, liquid", "MTAU3");
            sets2subPrds.Add("Mean WP for cloud type 3 = Stratus, liquid", "MWP3");

            sets2subPrds.Add("Mean CA for cloud type 4 = Cumulus, ice", "MCA4");
            sets2subPrds.Add("Mean PC for cloud type 4 = Cumulus, ice", "MPC4");
            sets2subPrds.Add("Mean TC for cloud type 4 = Cumulus, ice", "MTC4");
            sets2subPrds.Add("Mean TAU for cloud type 4 = Cumulus, ice", "MTAU4");
            sets2subPrds.Add("Mean WP for cloud type 4 = Cumulus, ice", "MWP4");

            sets2subPrds.Add("Mean CA for cloud type 5 = Stratocumulus, ice", "MCA5");
            sets2subPrds.Add("Mean PC for cloud type 5 = Stratocumulus, ice", "MPC5");
            sets2subPrds.Add("Mean TC for cloud type 5 = Stratocumulus, ice", "MTC5");
            sets2subPrds.Add("Mean TAU for cloud type 5 = Stratocumulus, ice", "MTAU5");
            sets2subPrds.Add("Mean WP for cloud type 5 = Stratocumulus, ice", "MWP5");

            sets2subPrds.Add("Mean CA for cloud type 6 = Stratus, ice", "MCA6");
            sets2subPrds.Add("Mean PC for cloud type 6 = Stratus, ice", "MPC6");
            sets2subPrds.Add("Mean TC for cloud type 6 = Stratus, ice", "MTC6");
            sets2subPrds.Add("Mean TAU for cloud type 6 = Stratus, ice", "MTAU6");
            sets2subPrds.Add("Mean WP for cloud type 6 = Stratus, ice", "MWP6");

            sets2subPrds.Add("Mean CA for cloud type 7 = Altocumulus, liquid", "MCA7");
            sets2subPrds.Add("Mean PC for cloud type 7 = Altocumulus, liquid", "MPC7");
            sets2subPrds.Add("Mean TC for cloud type 7 = Altocumulus, liquid", "MTC7");
            sets2subPrds.Add("Mean TAU for cloud type 7 = Altocumulus, liquid", "MTAU7");
            sets2subPrds.Add("Mean WP for cloud type 7 = Altocumulus, liquid", "MWP7");

            sets2subPrds.Add("Mean CA for cloud type 8 = Altostratus, liquid", "MCA8");
            sets2subPrds.Add("Mean PC for cloud type 8 = Altostratus, liquid", "MPC8");
            sets2subPrds.Add("Mean TC for cloud type 8 = Altostratus, liquid", "MTC8");
            sets2subPrds.Add("Mean TAU for cloud type 8 = Altostratus, liquid", "MTAU8");
            sets2subPrds.Add("Mean WP for cloud type 8 = Altostratus, liquid", "MWP8");

            sets2subPrds.Add("Mean CA for cloud type 9 = Nimbostratus, liquid", "MCA9");
            sets2subPrds.Add("Mean PC for cloud type 9 = Nimbostratus, liquid", "MPC9");
            sets2subPrds.Add("Mean TC for cloud type 9 = Nimbostratus, liquid", "MTC9");
            sets2subPrds.Add("Mean TAU for cloud type 9 = Nimbostratus, liquid", "MTAU9");
            sets2subPrds.Add("Mean WP for cloud type 9 = Nimbostratus, liquid", "MWP9");

            sets2subPrds.Add("Mean CA for cloud type 10 = Altocumulus, ice", "MCA10");
            sets2subPrds.Add("Mean PC for cloud type 10 = Altocumulus, ice", "MPC10");
            sets2subPrds.Add("Mean TC for cloud type 10 = Altocumulus, ice", "MTC10");
            sets2subPrds.Add("Mean TAU for cloud type 10 = Altocumulus, ice", "MTAU10");
            sets2subPrds.Add("Mean WP for cloud type 10 = Altocumulus, ice", "MWP10");

            sets2subPrds.Add("Mean CA for cloud type 11 = Altostratus, ice", "MCA11");
            sets2subPrds.Add("Mean PC for cloud type 11 = Altostratus, ice", "MPC11");
            sets2subPrds.Add("Mean TC for cloud type 11 = Altostratus, ice", "MTC11");
            sets2subPrds.Add("Mean TAU for cloud type 11 = Altostratus, ice", "MTAU11");
            sets2subPrds.Add("Mean WP for cloud type 11 = Altostratus, ice", "MWP11");

            sets2subPrds.Add("Mean CA for cloud type 12 = Nimbostratus, ice", "MCA12");
            sets2subPrds.Add("Mean PC for cloud type 12 = Nimbostratus, ice", "MPC12");
            sets2subPrds.Add("Mean TC for cloud type 12 = Nimbostratus, ice", "MTC12");
            sets2subPrds.Add("Mean TAU for cloud type 12 = Nimbostratus, ice", "MTAU12");
            sets2subPrds.Add("Mean WP for cloud type 12 = Nimbostratus, ice", "MWP12");

            sets2subPrds.Add("Mean CA for cloud type 13 = Cirrus", "MCA13");
            sets2subPrds.Add("Mean PC for cloud type 13 = Cirrus", "MPC13");
            sets2subPrds.Add("Mean TC for cloud type 13 = Cirrus", "MTC13");
            sets2subPrds.Add("Mean TAU for cloud type 13 = Cirrus", "MTAU13");
            sets2subPrds.Add("Mean WP for cloud type 13 = Cirrus", "MWP13");

            sets2subPrds.Add("Mean CA for cloud type 14 = Cirrostratus", "MCA14");
            sets2subPrds.Add("Mean PC for cloud type 14 = Cirrostratus", "MPC14");
            sets2subPrds.Add("Mean TC for cloud type 14 = Cirrostratus", "MTC14");
            sets2subPrds.Add("Mean TAU for cloud type 14 = Cirrostratus", "MTAU14");
            sets2subPrds.Add("Mean WP for cloud type 14 = Cirrostratus", "MWP14");

            sets2subPrds.Add("Mean CA for cloud type 15 = Deep convective", "MCA15");
            sets2subPrds.Add("Mean PC for cloud type 15 = Deep convective", "MPC15");
            sets2subPrds.Add("Mean TC for cloud type 15 = Deep convective", "MTC15");
            sets2subPrds.Add("Mean TAU for cloud type 15 = Deep convective", "MTAU15");
            sets2subPrds.Add("Mean WP for cloud type 15 = Deep convective", "MWP15");

            sets2subPrds.Add("Mean TS from clear sky composite(TS)", "MTS");//116
            sets2subPrds.Add("Time mean of standard deviation over space(TS)", "MSDS");
            sets2subPrds.Add("Mean RS from clear sky composite(RS)", "MRS");//118
            sets2subPrds.Add("Mean ice snow cover", "MSIA");//119
            sets2subPrds.Add("Mean Surface pressure (PS)", "MSP");
            sets2subPrds.Add("Mean Near-surface air temperature (TSA)", "MST");
            sets2subPrds.Add("Mean Temperature at 740 mb (T)", "MT740");
            sets2subPrds.Add("Mean Temperature at 500 mb (T)", "MT500");
            sets2subPrds.Add("Mean Temperature at 375 mb (T)", "MT375");
            sets2subPrds.Add("Mean Tropopause pressure (PT)", "MTP");
            sets2subPrds.Add("Mean Tropopause temperature (TT)", "MTT");
            sets2subPrds.Add("Mean Stratosphere temperature at 50 mb (T)", "MSST");
            sets2subPrds.Add("Mean Precipitable water for 1000-680 mb (PW)", "MPW1");
            sets2subPrds.Add("Mean Precipitable water for 680-310 mb (PW)", "MPW2");
            sets2subPrds.Add("Mean Ozone column abundance (O3)", "MOA");
        }

        /// <summary>
        /// 数据集分组与产品之间的对应关系
        /// </summary>
        public static void Group2Prds(out Dictionary<string, string> group2prd)
        {
            group2prd = new Dictionary<string, string>();
            for (int i = 0; i < _setgroups.Length; i++)
            {
                group2prd.Add(_setgroups[i],_prds[i]);
            }
        }

        /// <summary>
        /// 将D2数据中的数据集进行树状显示
        /// </summary>
        public static void AddAllD2DataSets(out Dictionary<string, Dictionary<String, Int16>> allDatasets)
        {
            allDatasets = new Dictionary<string, Dictionary<string, short>>();
            Dictionary<String, Int16> boxIdenti = new Dictionary<String, Int16>();
            //boxIdenti.Add("Latitude", 1);
            //boxIdenti.Add("Longitude index (equal-area)", 2);
            //boxIdenti.Add("Western-most longitude index (equal-angle)", 3);
            //boxIdenti.Add("Eastern-most longitude index (equal-angle)", 4);
            boxIdenti.Add("Land water coast code", 5);//5
            boxIdenti.Add("Number of observations", 6);//6
            boxIdenti.Add("Number of daytime observations", 7);
            allDatasets.Add("Box identification", boxIdenti);
            Dictionary<String, Int16> CA = new Dictionary<String, Int16>();
            CA.Add("Mean cloud amount", 8);//8
            CA.Add("Mean IR-marginal cloud amount", 9);
            CA.Add("Frequency of mean cloud amount = 0-10%", 10);
            CA.Add("Frequency of mean cloud amount = 10-20%", 11);
            CA.Add("Frequency of mean cloud amount = 20-30%", 12);
            CA.Add("Frequency of mean cloud amount = 30-40%", 13);
            CA.Add("Frequency of mean cloud amount = 40-50%", 14);
            CA.Add("Frequency of mean cloud amount = 50-60%", 15);
            CA.Add("Frequency of mean cloud amount = 60-70%", 16);
            CA.Add("Frequency of mean cloud amount = 70-80%", 17);
            CA.Add("Frequency of mean cloud amount = 80-90%", 18);
            CA.Add("Frequency of mean cloud amount = 90-100%", 19);
            allDatasets.Add("Cloud amount (CA)", CA);
            Dictionary<String, Int16> meanPC = new Dictionary<String, Int16>();
            meanPC.Add("Mean cloud top pressure(PC)", 20);
            meanPC.Add("Standard deviation of spatial mean over time(PC)", 21);
            meanPC.Add("Time mean of standard deviation over space(PC)", 22);
            allDatasets.Add("Mean cloud top pressure (PC)", meanPC);
            Dictionary<String, Int16> meanTC = new Dictionary<String, Int16>();
            meanTC.Add("Cloud temperature(TC)", 23);
            meanTC.Add("Standard deviation of spatial mean over time(TC)", 24);
            meanTC.Add("Time mean of standard deviation over space(TC)", 25);
            allDatasets.Add("Mean cloud top temperature (TC)", meanTC);
            Dictionary<String, Int16> meanCloudTAU = new Dictionary<String, Int16>();
            meanCloudTAU.Add("Mean cloud optical thickness(TAU)", 26);//26
            meanCloudTAU.Add("Standard deviation of spatial mean over time(TAU)", 27);
            meanCloudTAU.Add("Time mean of standard deviation over space(TAU)", 28);
            allDatasets.Add("Mean cloud optical thickness (TAU)", meanCloudTAU);
            Dictionary<String, Int16> meanCloudWP = new Dictionary<String, Int16>();
            meanCloudWP.Add("Mean cloud water path(WP)", 29);//29
            meanCloudWP.Add("Standard deviation of spatial mean over time(WP)", 30);//20
            meanCloudWP.Add("Time mean of standard deviation over space(WP)", 31);
            allDatasets.Add("Mean cloud water path (WP)", meanCloudWP);
            Dictionary<String, Int16> IRCloudTypes = new Dictionary<String, Int16>();
            IRCloudTypes.Add("Mean CA for low-level clouds", 32);
            IRCloudTypes.Add("Mean PC for low-level clouds", 33);
            IRCloudTypes.Add("Mean TC for low-level clouds", 34);
            IRCloudTypes.Add("Mean CA for middle-level clouds", 35);
            IRCloudTypes.Add("Mean PC for middle-level clouds", 36);
            IRCloudTypes.Add("Mean TC for middle-level clouds", 37);
            IRCloudTypes.Add("Mean CA for high-level clouds", 38);
            IRCloudTypes.Add("Mean PC for high-level clouds", 39);
            IRCloudTypes.Add("Mean TC for high-level clouds", 40);
            allDatasets.Add("IR cloud types", IRCloudTypes);
            Dictionary<String, Int16> lowCloudTypes = new Dictionary<String, Int16>();
            lowCloudTypes.Add("Mean CA for cloud type 1 = Cumulus, liquid", 41);
            lowCloudTypes.Add("Mean PC for cloud type 1 = Cumulus, liquid", 42);
            lowCloudTypes.Add("Mean TC for cloud type 1 = Cumulus, liquid", 43);
            lowCloudTypes.Add("Mean TAU for cloud type 1 = Cumulus, liquid", 44);
            lowCloudTypes.Add("Mean WP for cloud type 1 = Cumulus, liquid", 45);
            lowCloudTypes.Add("Mean CA for cloud type 2 = Stratocumulus, liquid", 46);
            lowCloudTypes.Add("Mean PC for cloud type 2 = Stratocumulus, liquid", 47);
            lowCloudTypes.Add("Mean TC for cloud type 2 = Stratocumulus, liquid", 48);
            lowCloudTypes.Add("Mean TAU for cloud type 2 = Stratocumulus, liquid", 49);
            lowCloudTypes.Add("Mean WP for cloud type 2 = Stratocumulus, liquid", 50);
            lowCloudTypes.Add("Mean CA for cloud type 3 = Stratus, liquid", 51);
            lowCloudTypes.Add("Mean PC for cloud type 3 = Stratus, liquid", 52);
            lowCloudTypes.Add("Mean TC for cloud type 3 = Stratus, liquid", 53);
            lowCloudTypes.Add("Mean TAU for cloud type 3 = Stratus, liquid", 54);
            lowCloudTypes.Add("Mean WP for cloud type 3 = Stratus, liquid", 55);
            lowCloudTypes.Add("Mean CA for cloud type 4 = Cumulus, ice", 56);
            lowCloudTypes.Add("Mean PC for cloud type 4 = Cumulus, ice", 57);
            lowCloudTypes.Add("Mean TC for cloud type 4 = Cumulus, ice", 58);
            lowCloudTypes.Add("Mean TAU for cloud type 4 = Cumulus, ice", 59);
            lowCloudTypes.Add("Mean WP for cloud type 4 = Cumulus, ice", 60);
            lowCloudTypes.Add("Mean CA for cloud type 5 = Stratocumulus, ice", 61);
            lowCloudTypes.Add("Mean PC for cloud type 5 = Stratocumulus, ice", 62);
            lowCloudTypes.Add("Mean TC for cloud type 5 = Stratocumulus, ice", 63);
            lowCloudTypes.Add("Mean TAU for cloud type 5 = Stratocumulus, ice", 64);
            lowCloudTypes.Add("Mean WP for cloud type 5 = Stratocumulus, ice", 65);
            lowCloudTypes.Add("Mean CA for cloud type 6 = Stratus, ice", 66);
            lowCloudTypes.Add("Mean PC for cloud type 6 = Stratus, ice", 67);
            lowCloudTypes.Add("Mean TC for cloud type 6 = Stratus, ice", 68);
            lowCloudTypes.Add("Mean TAU for cloud type 6 = Stratus, ice", 69);
            lowCloudTypes.Add("Mean WP for cloud type 6 = Stratus, ice", 70);
            lowCloudTypes.Add("Mean CA for cloud type 7 = Altocumulus, liquid", 71);
            lowCloudTypes.Add("Mean PC for cloud type 7 = Altocumulus, liquid", 72);
            lowCloudTypes.Add("Mean TC for cloud type 7 = Altocumulus, liquid", 73);
            lowCloudTypes.Add("Mean TAU for cloud type 7 = Altocumulus, liquid", 74);
            lowCloudTypes.Add("Mean WP for cloud type 7 = Altocumulus, liquid", 75);
            allDatasets.Add("LOW cloud types (vis-adjusted TC)", lowCloudTypes);
            Dictionary<String, Int16> midCloudTypes = new Dictionary<String, Int16>();
            midCloudTypes.Add("Mean CA for cloud type 8 = Altostratus, liquid", 76);
            midCloudTypes.Add("Mean PC for cloud type 8 = Altostratus, liquid", 77);
            midCloudTypes.Add("Mean TC for cloud type 8 = Altostratus, liquid", 78);
            midCloudTypes.Add("Mean TAU for cloud type 8 = Altostratus, liquid", 79);
            midCloudTypes.Add("Mean WP for cloud type 8 = Altostratus, liquid", 80);
            midCloudTypes.Add("Mean CA for cloud type 9 = Nimbostratus, liquid", 81);
            midCloudTypes.Add("Mean PC for cloud type 9 = Nimbostratus, liquid", 82);
            midCloudTypes.Add("Mean TC for cloud type 9 = Nimbostratus, liquid", 83);
            midCloudTypes.Add("Mean TAU for cloud type 9 = Nimbostratus, liquid", 84);
            midCloudTypes.Add("Mean WP for cloud type 9 = Nimbostratus, liquid", 85);
            midCloudTypes.Add("Mean CA for cloud type 10 = Altocumulus, ice", 86);
            midCloudTypes.Add("Mean PC for cloud type 10 = Altocumulus, ice", 87);
            midCloudTypes.Add("Mean TC for cloud type 10 = Altocumulus, ice", 88);
            midCloudTypes.Add("Mean TAU for cloud type 10 = Altocumulus, ice", 89);
            midCloudTypes.Add("Mean WP for cloud type 10 = Altocumulus, ice", 90);
            midCloudTypes.Add("Mean CA for cloud type 11 = Altostratus, ice", 91);
            midCloudTypes.Add("Mean PC for cloud type 11 = Altostratus, ice", 92);
            midCloudTypes.Add("Mean TC for cloud type 11 = Altostratus, ice", 93);
            midCloudTypes.Add("Mean TAU for cloud type 11 = Altostratus, ice", 94);
            midCloudTypes.Add("Mean WP for cloud type 11 = Altostratus, ice", 95);
            midCloudTypes.Add("Mean CA for cloud type 12 = Nimbostratus, ice", 96);
            midCloudTypes.Add("Mean PC for cloud type 12 = Nimbostratus, ice", 97);
            midCloudTypes.Add("Mean TC for cloud type 12 = Nimbostratus, ice", 98);
            midCloudTypes.Add("Mean TAU for cloud type 12 = Nimbostratus, ice", 99);
            midCloudTypes.Add("Mean WP for cloud type 12 = Nimbostratus, ice", 100);
            allDatasets.Add("MIDDLE cloud types (VIS-adjusted TC)", midCloudTypes);
            Dictionary<String, Int16> highCloudTypes = new Dictionary<String, Int16>();
            highCloudTypes.Add("Mean CA for cloud type 13 = Cirrus", 101);
            highCloudTypes.Add("Mean PC for cloud type 13 = Cirrus", 102);
            highCloudTypes.Add("Mean TC for cloud type 13 = Cirrus", 103);
            highCloudTypes.Add("Mean TAU for cloud type 13 = Cirrus", 104);
            highCloudTypes.Add("Mean WP for cloud type 13 = Cirrus", 105);
            highCloudTypes.Add("Mean CA for cloud type 14 = Cirrostratus", 106);
            highCloudTypes.Add("Mean PC for cloud type 14 = Cirrostratus", 107);
            highCloudTypes.Add("Mean TC for cloud type 14 = Cirrostratus", 108);
            highCloudTypes.Add("Mean TAU for cloud type 14 = Cirrostratus", 109);
            highCloudTypes.Add("Mean WP for cloud type 14 = Cirrostratus", 110);
            highCloudTypes.Add("Mean CA for cloud type 15 = Deep convective", 111);
            highCloudTypes.Add("Mean PC for cloud type 15 = Deep convective", 112);
            highCloudTypes.Add("Mean TC for cloud type 15 = Deep convective", 113);
            highCloudTypes.Add("Mean TAU for cloud type 15 = Deep convective", 114);
            highCloudTypes.Add("Mean WP for cloud type 15 = Deep convective", 115);
            allDatasets.Add("HIGH cloud types (VIS-adjusted TC)", highCloudTypes);
            Dictionary<String, Int16> meanTS = new Dictionary<String, Int16>();
            meanTS.Add("Mean TS from clear sky composite(TS)", 116);//116
            meanTS.Add("Time mean of standard deviation over space(TS)", 117);
            allDatasets.Add("Mean surface temperature (TS)", meanTS);
            Dictionary<String, Int16> meanRS = new Dictionary<String, Int16>();
            meanRS.Add("Mean RS from clear sky composite(RS)", 118);//118
            allDatasets.Add("Mean surface reflectance (RS)", meanRS);
            Dictionary<String, Int16> Snow_Icecover = new Dictionary<String, Int16>();
            Snow_Icecover.Add("Mean ice snow cover", 119);//119
            allDatasets.Add("Snow Ice cover", Snow_Icecover);
            Dictionary<String, Int16> TOVSatmosInfo = new Dictionary<String, Int16>();
            TOVSatmosInfo.Add("Mean Surface pressure (PS)", 120);
            TOVSatmosInfo.Add("Mean Near-surface air temperature (TSA)", 121);
            TOVSatmosInfo.Add("Mean Temperature at 740 mb (T)", 122);
            TOVSatmosInfo.Add("Mean Temperature at 500 mb (T)", 123);
            TOVSatmosInfo.Add("Mean Temperature at 375 mb (T)", 124);
            TOVSatmosInfo.Add("Mean Tropopause pressure (PT)", 125);
            TOVSatmosInfo.Add("Mean Tropopause temperature (TT)", 126);
            TOVSatmosInfo.Add("Mean Stratosphere temperature at 50 mb (T)", 127);
            TOVSatmosInfo.Add("Mean Precipitable water for 1000-680 mb (PW)", 128);
            TOVSatmosInfo.Add("Mean Precipitable water for 680-310 mb (PW)", 129);
            TOVSatmosInfo.Add("Mean Ozone column abundance (O3)", 130);
            allDatasets.Add("TOVS atmospheric information", TOVSatmosInfo);
        }
    }
}
