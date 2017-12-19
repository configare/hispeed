using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.MathAlg;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[] bs;
            double[,] x = new double[,] { { 10, 11,34,23,20,45,70 }, //Z0,...,Z6
                                          { 10, 11,34,23,20,45,70 },
                                          { 10, 11,34,23,20,45,70 },
                                          { 10, 11,34,23,20,45,70 },
                                          { 10, 11,34,23,20,45,70 },
                                          { 10, 11,34,23,20,45,70 },
                                          { 10, 11,34,23,20,45,70 }
                                         };
            //观测值
            lsfitlinear(new double[] { 4, 6, 8, 10, 11, 5, 7 }, x, out bs);
        }

        public static void lsfitlinear(double[] y, double[,] x, out double[] beta)
        {
            int nobs = y.Length;
            if (nobs != x.GetLength(0))
                throw new ArgumentException("ols: y and x must have same length");
            int nvar = x.GetLength(1);
            int info;
            alglib.lsfitreport rep;
            alglib.lsfitlinear(y, x, nobs, nvar, out info, out beta, out rep);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LinearFitterInt32 L = new LinearFitterInt32();
            LinearFitObject obj = new LinearFitObject();
            L.Fit(new int[] { 0, 1 }, new int[] { 4, 6 }, obj);
        }
    }
}
