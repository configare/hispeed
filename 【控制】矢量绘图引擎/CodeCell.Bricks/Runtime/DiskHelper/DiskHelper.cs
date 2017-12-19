using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace CodeCell.Bricks.Runtime
{
    public class DiskHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)]string rootPathName, ref int sectorsPerCluster, ref int bytesPerSector, ref int numberOfFreeClusters, ref int totalNumbeOfClusters);

        public static DiskInfo GetDiskInfo(string rootPathName)
        {
            DiskInfo diskInfo = new DiskInfo();
            int sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            GetDiskFreeSpace(rootPathName, ref sectorsPerCluster, ref bytesPerSector, ref numberOfFreeClusters, ref totalNumberOfClusters);
            diskInfo.RootPathName = rootPathName;
            diskInfo.SectorsPerCluster = sectorsPerCluster;
            diskInfo.BytesPerSector = bytesPerSector;
            diskInfo.NumberOfFreeClusters = numberOfFreeClusters;
            diskInfo.TotalNumberOfClusters = totalNumberOfClusters;
            return diskInfo;
        }
        public struct DiskInfo
        {
            public string RootPathName;
            public int SectorsPerCluster;
            public int BytesPerSector;
            public int NumberOfFreeClusters;
            public int TotalNumberOfClusters;
        }

        //Forgot to add this on my initial post, and I just noticed.
        public static int GetActualFileSize(FileInfo file)
        {
            double clusterSize = 0;
            double numberofClusters = 0;
            int actualSize = 0;
            DiskInfo diskInfo = new DiskInfo();
            diskInfo = GetDiskInfo(file.Directory.Root.FullName);

            clusterSize = (diskInfo.BytesPerSector * diskInfo.SectorsPerCluster);

            //Added this, seems if the file length and clustersize are exact it would over-report the size
            if ((file.Length % clusterSize) > 0)
                clusterSize += 1;

            numberofClusters = Math.Round(file.Length / clusterSize);

            actualSize = (int)(numberofClusters * clusterSize);
            return actualSize;
        }
    }
}
