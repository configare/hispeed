using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GeoDo.RSS.Core.UI
{
    public static class PerformanceMonitoring
    {
        private static PerformanceCounter _ramCounter;

        //static object v;
        static PerformanceMonitoring()
        {
            try
            {
                //v = PerformanceCounterCategory.GetCategories();
                //foreach (PerformanceCounterCategory it in v as Array)
                //    Console.WriteLine(it.CategoryName);
                //
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
             
            }
            catch(Exception ex) 
            {
                Console.WriteLine();
            }
        }

        public static float GetAvailableRAM()
        {
            if (_ramCounter == null)
                return -1;
            return _ramCounter.NextValue();
        }
    }
}
