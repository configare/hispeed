using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace test
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Application.Run(new Form2());
        }
    }

    public class ImageProcess
    {
        // RGB2CMYK
        public void RGB2CMYK(byte r, byte g, byte b, ref byte c, ref byte m, ref byte y, ref byte k)
        {
            double R, G, B;
            R = (double)r;
            G = (double)g;
            B = (double)b;

            R = 1.0 - (R / 255.0);
            G = 1.0 - (G / 255.0);
            B = 1.0 - (B / 255.0);

            double C, M, Y, K;
            if (R < G)
                K = R;
            else
                K = G;
            if (B < K)
                K = B;

            C = (R - K) / (1.0 - K);
            M = (G - K) / (1.0 - K);
            Y = (B - K) / (1.0 - K);

            C = (C * 100) + 0.5;
            M = (M * 100) + 0.5;
            Y = (Y * 100) + 0.5;
            K = (K * 100) + 0.5;

            c = (byte)C;
            m = (byte)M;
            y = (byte)Y;
            k = (byte)K;
        }
        
        // CMYK2RGB
        private Color CMYK2RGB(byte c, byte m, byte y, byte k)
        {
            byte r, g, b;
            Color rgb;

            double R, G, B;
            double C, M, Y, K;

            C = (double)c;
            M = (double)m;
            Y = (double)y;
            K = (double)k;

            C = C / 255.0;
            M = M / 255.0;
            Y = Y / 255.0;
            K = K / 255.0;

            R = C * (1.0 - K) + K;
            G = M * (1.0 - K) + K;
            B = Y * (1.0 - K) + K;

            R = (1.0 - R) * 255.0 + 0.5;
            G = (1.0 - G) * 255.0 + 0.5;
            B = (1.0 - B) * 255.0 + 0.5;

            r = (byte)R;
            g = (byte)G;
            b = (byte)B;

            rgb = Color.FromArgb(r, g, b);

            return rgb;
        }
    }
}
