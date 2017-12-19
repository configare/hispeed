using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DF.HDF4.Cloudsat;

namespace TestCloudsat
{
    class Program
    {
        static void Main(string[] args)
        {
            string fullfilename = @"E:\Smart\CloudArgs\cloudsat\2007101034511_05065_CS_2B-GEOPROF_GRANULE_P_R04_E02.hdf";
            CloudsatDataProvider data = new CloudsatDataProvider(fullfilename, null, null);
            data.Test();
        }
    }
}
