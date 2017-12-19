using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace GeoDo.Smart.TrueColorImageWorldFile
{
    public class TCode
    {
        //#分块号	左上角点纬度	左上角点经度	右下角点纬度	右下角点经度
        public static SortedDictionary<string, Rectangle> GetTCode()
        {
            SortedDictionary<string, Rectangle> tCode = new SortedDictionary<string, Rectangle>();
            string codeRes = "GeoDo.Smart.TrueColorImageWorldFile.TCode.txt";
            Assembly asm = Assembly.GetExecutingAssembly();//读取嵌入式资源            
            using (Stream obj = asm.GetManifestResourceStream(codeRes))
            {
                using (StreamReader sr = new StreamReader(obj))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                            continue;
                        string[] lines = line.Split('\t');
                        if (lines.Length == 5)
                        {
                            string code = lines[0];
                            int minx, maxx, miny, maxy;
                            if (int.TryParse(lines[1], out maxy)
                            && int.TryParse(lines[2], out minx)
                            && int.TryParse(lines[3], out miny)
                            && int.TryParse(lines[4], out maxx))
                            {
                                tCode.Add(code, new Rectangle(minx, miny, maxx - minx, maxy - miny));
                            }
                        }
                    }
                }
            }
            return tCode;
        }
    }
}
