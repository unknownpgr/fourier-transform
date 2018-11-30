using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FourierTransform;

namespace FourierTest
{
    public partial class Form1 : Form
    {
        int smoothness = 10;
        int sampling = 40;
        int mouseSampling = 5;
        int samplingCount = 0;

        bool click = false;
        List<int> xs, ys;
        Fourier cex, cey;

        public Form1()
        {
            xs = new List<int>(); ys = new List<int>();
            InitializeComponent();

            textBox_drw_smt.Text = smoothness + "";
            textBox_sam_fun.Text = sampling + "";
            textBox_sam_mus.Text = mouseSampling + "";

            string text = 
                " Made by UnknownPgr \r\n" +
                " Email : UnknownPgr@gmail.com \r\n" +
                "\r\n" +
                "Maybe, for series terms, half of node count would be proper.";
            textBox2.Text = text;
        }

        //===================================
        //  Mouse
        //===================================

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            samplingCount++;
            if (click && samplingCount >= mouseSampling)
            {
                xs.Add(e.X);
                ys.Add(e.Y);
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                samplingCount = 0;
                label4.Text = "Node count : " + xs.Count;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            click = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
        }

        //===================================
        //  Buttons
        //===================================

        //Set image
        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.DefaultExt = ".jpg";
            fd.ShowDialog();
            try
            {
                Image image = Image.FromFile(fd.FileName);
                double rx = pictureBox1.Width * 1.0 / image.Width;
                double ry = pictureBox1.Height * 1.0 / image.Height;
                double r = Math.Min(rx, ry);
                image = new Bitmap(image, (int)(image.Width * r), (int)(image.Height * r));
                pictureBox1.Image = image;
            }
            catch
            {

            }
        }

        //Copy function text
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 1)
            {
                Clipboard.SetText(textBox1.Text);
                MessageBox.Show("Function text was copied to your clipboard.");
            }
        }

        //Clear
        private void button3_Click(object sender, EventArgs e)
        {
            xs.Clear();
            ys.Clear();
            textBox1.Clear();
            pictureBox1.Refresh();
            pictureBox2.Refresh();
            label4.Text = "Node count : " + xs.Count;
        }

        //===================================
        //  Painting
        //===================================

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (xs.Count > 1)
            {
                Graphics g = e.Graphics;
                Point[] points = new Point[xs.Count];
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Point(xs[i], ys[i]);
                    g.DrawEllipse(Pens.DarkGray, xs[i] - 2, ys[i] - 2, 4, 4);
                }
                g.DrawEllipse(Pens.Red, xs[0] - 2, ys[0] - 2, 4, 4);
                g.DrawEllipse(Pens.Green, xs.Last() - 2, ys.Last() - 2, 4, 4);
                g.DrawLines(Pens.Black, points);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                smoothness = Int32.Parse(textBox_drw_smt.Text);
                sampling = Int32.Parse(textBox_sam_fun.Text);
                mouseSampling = Int32.Parse(textBox_sam_mus.Text);
            }
            catch
            {
                textBox_drw_smt.Text = smoothness + "";
                textBox_sam_fun.Text = sampling + "";
                textBox_sam_mus.Text = mouseSampling + "";
            }

            pictureBox1.Refresh();
            pictureBox2.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox2.Paint += pictureBox1_Paint;
            pictureBox2.Refresh();
            pictureBox2.Paint -= pictureBox1_Paint;
        }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            if (cex == null)
            {
                return;
            }
            Graphics g = e.Graphics;

            int width = pictureBox3.Width;
            int height = pictureBox1.Height;
            int length = cex.Length;
            double[] spx = cex.GetLogSpectrum();
            double[] spy = cey.GetLogSpectrum();

            double max = -256;
            double min = 256;

            for (int i = 0; i < length; i++)
            {
                max = Math.Max(spx[i], max);
                max = Math.Max(spy[i], max);

                min = Math.Min(spx[i], min);
                min = Math.Min(spy[i], min);
            }

            double range = (max - min);

            Point[] psx = new Point[length];
            Point[] psy = new Point[length];
            for (int i = 0; i < length; i++)
            {
                psx[i] = new Point(width * i / length,(int)(height*(1 - (spx[i] - min) / range) / 2.5));
                psy[i] = new Point(width * i / length,(int)(height*(1 - ( spy[i] - min) / range) / 2.5) );
            }

            g.DrawLines(Pens.Red, psx);
            g.DrawLines(Pens.Blue, psy);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (xs.Count > 1)
            {

                double[] nxs = new double[xs.Count];
                double[] nys = new double[ys.Count];

                for (int i = 0; i < nxs.Length; i++)
                {
                    nxs[i] = xs[i];
                    nys[i] = ys[i];
                }

                cex = Fourier.GetCoefficient(nxs, sampling);
                cey = Fourier.GetCoefficient(nys, sampling);

                textBox1.Text = "x(t) = " + cex.GetFunctionString() + "\r\n\r\ny(t) = " + cey.GetFunctionString();

                Point[] points = new Point[xs.Count * smoothness];
                for (int i = 0; i < points.Length; i++)
                {
                    double pos = 1.0 * i / smoothness;
                    double x = cex.RecoverData(pos);
                    double y = cey.RecoverData(pos);
                    points[i] = new Point((int)x, (int)y);
                }
                Graphics g = e.Graphics;
                g.DrawLines(Pens.Black, points);
                pictureBox3.Refresh();
            }
        }
    }
}
