using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.WinForm
{
    internal static class MemoryIsEnoughChecker
    {
        public static ISmartSession _session;
        private const int HOST_MIN_MEMORY_SIZE = 200;//MB

        public static bool MemoryIsEnoughForVector(string vectorFileName)
        {
            if (CodeCell.AgileMap.Core.GlobalCacher.VectorDataGlobalCacher.GetData(vectorFileName) != null)
                return true;
            float availableSize = PerformanceMonitoring.GetAvailableRAM();
            if (availableSize < 0)
                return true;
            FileInfo fInfo = new FileInfo(vectorFileName);
            float requiredSize = 10 * fInfo.Length / 1024f / 1024f; //矢量文件大小的10倍
            if (availableSize < requiredSize )
                return false;
            return true;
        }

        public static bool MemoryIsEnoughForRaster(string rasterFileName)
        {
            float availableSize = PerformanceMonitoring.GetAvailableRAM();
            if (availableSize < 0)
                return true;
            int memoryOfTile, tileCount;
            float requiredSize = RasterDrawing.EstimateRequiredMemory(rasterFileName,out memoryOfTile,out tileCount);
            int countOfDrawing = 0;
            float memoryOfLoading = GetMemoryOfTileLoading(out countOfDrawing);
            //Console.WriteLine("AvailableSize:" + availableSize.ToString() + " , RequiredSize:" + requiredSize.ToString() + " , SizeOfLoading:" + memoryOfLoading.ToString());
            availableSize -= memoryOfLoading;
            int host_min_mem_size = HOST_MIN_MEMORY_SIZE;
            if (countOfDrawing == 0)
                host_min_mem_size = 0;
            if (availableSize < requiredSize + host_min_mem_size)//预留200MB
                return false;
            else if (availableSize < requiredSize)//内存碎片的可能性非常大，需要先尝试内存申请能否成功
            {
                for (int i = 0; i < tileCount; i++)
                {
                    try
                    {
                        byte[] tileBuffer = new byte[memoryOfTile];
                    }
                    catch 
                    {
                        Console.WriteLine("内存空间足够，但因内存碎片无法申请成功空间。");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        private static float GetMemoryOfTileLoading(out int countOfDrawing)
        {
            countOfDrawing = 0;
            ISmartWindow[] wnds = _session.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ICanvasViewer; });
            if (wnds == null || wnds.Length == 0)
                return 0;
            float memorySize = 0;
            foreach (ICanvasViewer viewer in wnds)
            {
                IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
                if (drawing == null)
                    continue;
                if(drawing.BandCount ==1)
                    memorySize += drawing.TileBitmapProvider.TileCountOfLoading * 512 * 512 * 1;
                else
                    memorySize += drawing.TileBitmapProvider.TileCountOfLoading * 512 * 512 * 3;
                //
                countOfDrawing++;
            }
            return memorySize / 1024 / 1024; //MB
        }

        public static bool MemoryIsEnouggWithMsgBoxForRaster(string rasterFileName)
        {
            bool isEnough = MemoryIsEnoughForRaster(rasterFileName);
            if (!isEnough)
            {
                MsgBox.ShowInfo("系统可用内存不足以打开以下文件，请关闭一些已打开的文件再尝试打开该文件。\n\n" + rasterFileName);
            }
            return isEnough;
        }

        public static bool MemoryIsEnouggWithMsgBoxForVector(string vectorFileName)
        {
            bool isEnough = MemoryIsEnoughForVector(vectorFileName);
            if (!isEnough)
            {
                MsgBox.ShowInfo("系统可用内存不足以打开以下文件，请关闭一些已打开的文件再尝试打开该文件。\n\n" + vectorFileName);
            }
            return isEnough;
        }
    }
}
