using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GeoDo.FileProject
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

        /// <summary>
        /// 可用物理内存
        /// </summary>
        /// <returns></returns>
        public static uint GetAvalidPhyMemory()
        {
            MEMORY_INFO info = new MEMORY_INFO();
            GlobalMemoryStatus(ref info);
            return info.dwAvailPhys;
        }

        public static long WorkingSet64()
        {
            Process process = Process.GetCurrentProcess();
            return process.WorkingSet64;
        }
        /// <summary>
        /// 限制当前进程内存，
        /// 剩余内存限制（系统可用内存最低限制）
        /// 已使用内存限制（系统已用内存最高限制）
        /// </summary>
        /// <param name="avalidPhyMemoryMin">（当前操作系统）剩余内存限制（MB）</param>
        /// <param name="usedMemMax">（当前应用程序）已使用内存限制（MB）</param>
        public static void MemoryNeed(int avalidPhyMemoryMin, int usedMemMax)
        {
            uint avalidPhyMemory = MemoryHelper.GetAvalidPhyMemory();
            long usedMem = MemoryHelper.WorkingSet64();
            if (avalidPhyMemory < avalidPhyMemoryMin * 1024 * 1024 ||
                usedMem > usedMemMax * 1024 * 1024)
                throw new Exception(string.Format("当前系统资源不足以完成该操作，请释放部分资源后再试，剩余{0}<{1}，已使用{2}<{3}。",
                    avalidPhyMemory / (1024f * 1024), avalidPhyMemoryMin, usedMem / (1024f * 1024), usedMemMax));
        }

        /// <summary>
        ///  当前可以申请的最大byte数组的大小
        /// </summary>
        /// <returns></returns>
        public static long GetMaxArray()
        {
            long memLong = System.GC.GetTotalMemory(true);
            bool sessory = false;
            memLong = int.MaxValue / 2;
            int count = 0;
            double s = -1;
            while (!sessory)
            {
                try
                {
                    byte[] mem = new byte[memLong];
                    //double[] mem2 = new double[memLong];
                    //double[] mem3 = new double[memLong];

                    memLong = mem.LongLength;
                    s = mem[memLong - 1];
                    //s = mem2[memLong - 1];
                    //s = mem3[memLong - 1];
                    sessory = true;
                }
                catch
                {
                    memLong = memLong * 4 / 5;
                    count++;
                    sessory = false;
                }
            }
            Console.WriteLine(s);
            return memLong;
        }

        /// <summary>
        /// 当前可以申请的最大byte数组的大小
        /// </summary>
        /// <param name="arrayCount">byte数组个数</param>
        /// <returns></returns>
        public static long GetMaxArrayLength<T>(int arrayCount)
        {
            //by chennan 20140810 封装后的投影FY3C MERSI 250米投影为黑色，原因内存计算错误（不止托管内存可用）
            long memLong = Process.GetCurrentProcess().PrivateMemorySize;
            //long memLong = System.GC.GetTotalMemory(true);
            bool sessory = false;
            memLong = int.MaxValue / 2 > memLong ? memLong : int.MaxValue / 2;
            int count = 0;
            double s = -1;
            List<GCHandle> hs = new List<GCHandle>();
            while (!sessory)
            {
                try
                {
                    T[][] mems = new T[arrayCount][];
                    for (int i = 0; i < arrayCount; i++)
                    {
                        mems[i] = new T[memLong];
                        GCHandle h = GCHandle.Alloc(mems[i], GCHandleType.Pinned);
                        hs.Add(h);
                    }
                    sessory = true;
                }
                catch (OutOfMemoryException ex)
                {
                    memLong = memLong * 3 / 4;
                    count++;
                    sessory = false;
                }
                finally
                {
                    foreach (GCHandle h in hs)
                    {
                        h.Free();
                    }
                    hs.Clear();
                }
            }
#if test
            Console.WriteLine(string.Format("Can Use Memory {0}*{1}byte,{2}MB", arrayCount, memLong, arrayCount * memLong / 1048576f));
#endif
            return memLong;
        }
    }
}
