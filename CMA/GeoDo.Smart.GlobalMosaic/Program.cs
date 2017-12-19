using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.Smart.GlobalMosaic
{
    public class Program
    {
        /// <summary>
        /// 地理数据拼接:地理坐标，投影坐标
        /// 拼接输出ldf
        /// 支持按高度角数据拼接,高度角数据为后缀为".SolarZenith.ldf"的附加文件，值为扩大了100倍的short类型的高度角值。
        /// </summary>
        /// <param name="args">
        /// "D:\20131003|*_1000M.ldf" "D:\20131002\\FY3C_VIRR_1000M_global.ldf" "1,2,3" "0.01,0.01,-180,90,36000,18000"
        /// </param>
        static void Main(string[] args)
        {
            //new ToChinaCenter().Tran();
            //return;
            if (args == null || args.Length != 4)
            {
                Console.WriteLine("输入参数错误：");
                Console.WriteLine("  指定目录[:搜索模式] 拼接后文件名 要拼接的波段(序号从1开始) 地理坐标（x分辨率,y分辨率,左上角经度,左上角纬度,宽度,高度）");
                Console.WriteLine(@"  例如：D:\20131003|*_1000M.ldf D:\20131002\FY3C_VIRR_1000M_global.ldf 1,2,3 0.01,0.01,-180,90,36000,18000");
                return;
            }
            string[] paths = args[0].Split('|');
            string dir = paths[0];
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("指定目录不存在：" + dir);
                return;
            }
            string mode = "";
            if (paths.Length == 2)
                mode =  paths[1];
            string[] files = Directory.GetFiles(dir, mode);
            string outfile = args[1];
            string[] bands = args[2].Split(',');
            string geoString = args[3];
            GeoInfo geoHeader = GeoInfo.Parse(geoString);
            if (geoHeader == null)
            {
                Console.WriteLine("输入参数错误：" + geoString);
                return;
            }
            if (geoHeader.ResX == 0 || geoHeader.ResY == 0)
            {
                Console.WriteLine("输入参数错误：" + geoString + "中分辨率解析为0");
                return;
            }
            MosaicProcess c = new MosaicProcess();
            c.Mosaic(files, outfile, geoHeader, bands,
                (p, txt) =>
                {
                    Console.WriteLine(string.Format("{0}% {1}", p, txt));
                });
        }
    }
}
