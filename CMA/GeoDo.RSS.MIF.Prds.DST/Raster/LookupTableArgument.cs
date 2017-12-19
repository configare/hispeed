using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class LookupTableArgument
    {
        float[,] cbtd3132 = new float[50, 29];
        float[,] cbtd2931 = new float[50, 29];
        float[,] cbt31 = new float[50, 29];
        float[] dopt = new float[50];
        float[] dref = new float[29];
        float[] ext55 = new float[29];

        public LookupTableArgument()
        {
            string argumentFile1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\Cbtbtd.txt");
            string argumentFile2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\Cbtdbtd.txt");
            string extFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\Ext.txt");
            if (!File.Exists(argumentFile1)||!File.Exists(argumentFile2)||!File.Exists(extFile))
                return;
            //read LUT1 cbt31
            string[] lines = File.ReadAllLines(argumentFile1, Encoding.Default);
            if(lines.Length<1)
                return ;
            int p = 0;
            string[] parts = null;
            List<float> cbt31List=new List<float>();
            for (int i = 0; i < lines.Length;i++ )
            {
                float arg;
                parts = lines[i].Split(new char[] { '\t', ' ' },StringSplitOptions.RemoveEmptyEntries);
                if (parts != null && parts.Length > 3)
                {
                    if(i%29==0)
                    {
                        if (float.TryParse(parts[0], out arg))
                        {
                            dopt[p] = arg;
                            p++;
                        }
                    }
                    if (i < 29)
                    {
                        if (float.TryParse(parts[1], out arg))
                            dref[i] = arg;
                    }
                    if(float.TryParse(parts[2],out arg))
                        cbt31List.Add(arg);
                }
            }
            int k = 0;
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 29; j++)
                {
                    cbt31[i, j] = cbt31List[k];
                    k++;
                }
            }
            //read LUT2
            lines = File.ReadAllLines(argumentFile2, Encoding.Default);
            parts = null;
            List<float> cbtd2931List = new List<float>();
            List<float> cbtd3132List = new List<float>();
            for (int i = 0; i < lines.Length; i++)
            {
                float arg;
                parts = lines[i].Split(new char[] { '\t', ' ' },StringSplitOptions.RemoveEmptyEntries);
                if (parts != null && parts.Length > 3)
                {
                    if (float.TryParse(parts[2], out arg))
                        cbtd2931List.Add(arg);
                    if (float.TryParse(parts[3], out arg))
                        cbtd3132List.Add(arg);
                }
            }
            k = 0;
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 29; j++)
                {
                    cbtd2931[i, j] = cbtd2931List[k];
                    cbtd3132[i, j] = cbtd3132List[k];
                    k++;
                }
            }
            //read ext55
            lines = File.ReadAllLines(extFile, Encoding.Default);
            parts = null;
            for (int i = 0; i < lines.Length; i++)
            {
                float arg;
                parts = lines[i].Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts != null)
                {
                    if (float.TryParse(parts[0], out arg))
                        ext55[i] = arg;
                }
            }
        }

        public float[,] Cbtd3132
        {
            get
            {
                return cbtd3132;
            }
        }

        public float[,] Cbtd2931
        {
            get
            {
                return cbtd2931;
            }
        }

        public float[,] Cbt31
        {
            get
            {
                return cbt31;
            }
        }

        public float[] Dopt
        {
            get
            {
                return dopt;
            }
        }

        public float[] Dref
        {
            get
            {
                return dref;
            }
        }

        public float[] Ext55
        {
            get
            {
                return ext55;
            }
        }
    }
}
