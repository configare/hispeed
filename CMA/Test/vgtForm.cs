using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using GeoDo.RSS.MIF.Prds.MWS;

namespace Test
{
    public partial class vgtForm : Form
    {
        private static Object _obj = new Object();
        double de2rad = 1.0d / 180.0d * Math.PI;
        double rad2de = 180.0d / Math.PI;

        public vgtForm()
        {
            InitializeComponent();
            //this.ucTimeSeries1.SetChangeHandler(DoNothing);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TSANRetrieval ts = new TSANRetrieval(_obj as Dictionary<string, object>);
            //ts.DoRetrievalAndCompute();
            float[] alpha = new float[6] { 44.3f,43.63f,46.65f,44.38f,45.34f,44.67f};
            float[] omega = new float[6] { 1.86f, 1.15f, 0.69f, 0.56f,0.35f, 0.16f };
            double re = 6371230;
            double h = 786210;
            double reh = re + h;
            double[] r = new double[6], gamma = new double[6], theta = new double[6],  Ly= new double[6], Lx = new double[6];
            double halfLyout,halfLyin,rout,rin;
            for (int i = 0; i < 6;i++ )
            {
                r[i] = GetDFromAngleAndOtherD(re, reh, alpha[i]);
                gamma[i] = GetAngleFromD(re, reh, r[i]);
                theta[i] = alpha[i] + gamma[i];
                rout = GetDFromAngleAndOtherD(re, reh, alpha[i] + omega[i]/2);
                halfLyout = GetDFromAngleAndD(r[i], rout, omega[i] / 2);
                rin = GetDFromAngleAndOtherD(re, reh, alpha[i] - omega[i] / 2);
                halfLyin = GetDFromAngleAndD(r[i], rin, omega[i] / 2);
                Ly[i] = halfLyout + halfLyin;
                Lx[i] = omega[i] * de2rad * r[i];
                this.textBox1.Text += "theta:"+ theta[i].ToString("f4")+"\t";
                this.textBox1.Text += "R:" + r[i].ToString("f4") + "\t";
                this.textBox1.Text += "Rin:" + rin.ToString("f4") + "\t";
                this.textBox1.Text += "Rout:" + rout.ToString("f4") + "\t";
                this.textBox1.Text += "halfLyout:" + halfLyout.ToString("f4") + "\t";
                this.textBox1.Text += "halfLyin:" + halfLyin.ToString("f4") + "\t";
                this.textBox1.Text += "Ly:" + Ly[i].ToString("f4") + "\t";
                this.textBox1.Text += "Lx:" + Lx[i].ToString("f4") + "\n";
            }
        }

        private double GetAngleFromD(double a,double b,double c)
        {
            double cosC = (a * a + b * b - c * c)/(2*a*b);
            return Math.Acos(cosC) * rad2de;
        }


        private double GetDFromAngleAndOtherD(double Da, double Db, double angleA)
        {
            //C*C-2*Db*cosA*C+Db*Db-Da*Da=0
            double cosA = Math.Acos(angleA * de2rad);
            double a = 1, b = -2 * Db * cosA, c = Db * Db - Da * Da;
            double Dc1 = (-b + Math.Sqrt(b * b - 4 * a * c))/2/a;
            double Dc2 = (-b - Math.Sqrt(b * b - 4 * a * c))/2/a;
            return (Dc1< Dc2)?Dc1:Dc2;
        }

        private double GetDFromAngleAndD(double Da, double Db, double angleC)
        {
            //Dc*Dc=Db*Db+Da*Da-2*Da*Db*cosC
            return Math.Sqrt(Db * Db + Da * Da - 2 * Da * Db * Math.Cos(angleC * de2rad));
        }


        private void DoNothing(object obj)
        {
            _obj = obj;
        }
    }
}
