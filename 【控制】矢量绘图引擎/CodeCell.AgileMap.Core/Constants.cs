using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public static class Constants
    {
        public static string cstMapFileFilter = "IECAS Map Files(*.mcd)|*.mcd";
        public static string cstMapResourceDir = "MapResource";
        public const string cstLevelField = "DisplayLevel";

        internal static string GetMapResourceDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory + cstMapResourceDir;
        }
    }
}
