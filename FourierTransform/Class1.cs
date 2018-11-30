using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourierTransform
{
    public class Fourier
    {
        int l;
        int n;
        public double[] an;
        public double[] bn;
        public int Length
        {
            get
            {
                return an.Length;
            }
        }

        public static Fourier GetCoefficient(double[] data, int n)
        {
            int t = data.Length;
            int l = t / 2;

            double[] an = new double[n];
            double[] bn = new double[n];

            for (int i = 0; i < n; i++)
            {
                for (int j = -l; j < l; j++)
                {
                    an[i] += data[j + l] * Math.Cos(i * j * Math.PI / l);
                    bn[i] += data[j + l] * Math.Sin(i * j * Math.PI / l);
                }
                an[i] /= l;
                bn[i] /= l;
            }
            return new Fourier
            {
                l = l,
                n = n,
                an = an,
                bn = bn
            };
        }

        public double RecoverData(double pos)
        {
            double output = -an[0] / 2;
            for (int i = 0; i < n; i++)
            {
                output += an[i] * Math.Cos(pos * i * Math.PI / l)
                        + bn[i] * Math.Sin(pos * i * Math.PI / l);
            }
            return output;
        }

        public double[] GetSpectrum()
        {
            double[] output = new double[an.Length];
            for (int i = 0; i < an.Length; i++)
            {
                output[i] = Math.Sqrt(Math.Pow(an[i], 2) + Math.Pow(bn[i], 2));
            }
            return output;
        }

        public double[] GetLogSpectrum()
        {
            double[] output = new double[an.Length];
            for (int i = 0; i < an.Length; i++)
            {
                output[i] = Math.Log10(Math.Sqrt(Math.Pow(an[i], 2) + Math.Pow(bn[i], 2)));
            }
            return output;
        }

        public string GetFunctionString(string variable = "t", int d = 3)
        {
            string output = "";
            for (int i = 0; i < n; i++)
            {
                double a = SetDecimal(an[i], d);
                double b = SetDecimal(bn[i], d);
                if (a != 0)
                {
                    if (a > 0)
                    {
                        output += "+";
                    }
                    output += a + "Sin(" + i + "π" + variable + "/" + l + ")";
                }
                if (b != 0)
                {
                    if (b >= 0)
                    {
                        output += "+";
                    }
                    output += b + "Cos(" + i + "π" + variable + "/" + l + ")";
                }
            }
            return output;
        }

        private static double SetDecimal(double num, int d)
        {
            double d_ = Math.Pow(10, d);
            return (int)(num * d) / d_;
        }
    }
}
