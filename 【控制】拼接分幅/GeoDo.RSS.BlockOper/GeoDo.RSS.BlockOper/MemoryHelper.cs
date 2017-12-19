using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.BlockOper
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        public uint dwLength;
        //已用内存
        public uint dwMemoryLoad;
        //物理内存总大小 
        public uint dwTotalPhys;
        //可用物理内存 
        public uint dwAvailPhys;
        //交换页面总大小
        public uint dwTotalPageFile;
        //可交换页面大小
        public uint dwAvailPageFile;
        //虚拟内存总大小 
        public uint dwTotalVirtual;
        //可用虚拟内存 
        public uint dwAvailVirtual;
    } 
    public static class MemoryHelper
    {
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);

        public static uint GetAvalidPhyMemory()
        {
            MEMORY_INFO info = new MEMORY_INFO();
            GlobalMemoryStatus(ref info);
            return info.dwAvailPhys;
        }
    }   
}
