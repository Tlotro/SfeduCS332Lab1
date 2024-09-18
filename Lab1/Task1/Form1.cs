using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastBitmapF;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPEG Images|*.jpg|PNG Images|*.png";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap originalImage = new Bitmap(dialog.FileName);
                pictureBox1.Image = originalImage;

                Bitmap grayImage1 = ToGrayScale(originalImage, 1);
                Bitmap grayImage2 = ToGrayScale(originalImage, 2);

                pictureBox2.Image = grayImage1;
                pictureBox3.Image = grayImage2;

                Bitmap diffrenceImage = SubtractImages(grayImage1, grayImage2);
                pictureBox4.Image = diffrenceImage;

                Bitmap histogram1 = CreateHistogram(grayImage1);
                Bitmap histogram2 = CreateHistogram(grayImage2);

                pictureBox5.Image = histogram1;
                pictureBox6.Image = histogram2;
            }
        }

        private Bitmap ToGrayScale(Bitmap originalImage, int method)
        {
            Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);

            return originalImage.Select(color =>
            {
                byte avg;
                if (method == 1)
                    avg = (byte)((0.299*color.R + 0.587*color.G + 0.114*color.B));
                else
                    avg = (byte)((0.2126*color.R + 0.7152*color.G + 0.0722*color.B));
                return Color.FromArgb(avg, avg, avg);
            });
        }

        private Bitmap SubtractImages(Bitmap image1, Bitmap image2)
        {
            Bitmap diffrenceImage = new Bitmap(image1.Width, image1.Height);

            using (var fastImage1 = new FastBitmap(image1))
            using (var fastImage2 = new FastBitmap(image2))
            using (var fastDiffrenceImage = new FastBitmap(diffrenceImage))
            {
                for (var x = 0; x < fastImage1.Width; x++)
                {
                    for (var y = 0; y < fastImage1.Height; y++)
                    {
                        int grayValue1 = fastImage1[x, y].R;
                        int grayValue2 = fastImage2[x, y].R;
                        int deltaValue = Math.Abs(grayValue1 - grayValue2);
                        fastDiffrenceImage[x, y] = Color.FromArgb(deltaValue, deltaValue, deltaValue);
                    }
                }
            }

            return diffrenceImage;
        }

        private Bitmap CreateHistogram(Bitmap image)
        {
            int[] histogram = new int[256];

            using (var fastImage = new FastBitmap(image))
            {
                for (var x = 0; x < fastImage.Width; x++)
                {
                    for (var y = 0; y < fastImage.Height; y++)
                    {
                        int grayValue = fastImage[x, y].R;
                        histogram[grayValue]++;
                    }
                }
            }

            Bitmap histogramImage = new Bitmap(256, 256);
            using (Graphics g = Graphics.FromImage(histogramImage))
            {
                g.Clear(Color.White);
                int maxCount = histogram.Max();

                for (int i = 0; i < 256; i++)
                {
                    int height = (int)(histogram[i] * 256.0 / maxCount);
                    g.DrawLine(Pens.Black, i, 256, i, 256 - height);
                }
            }

            return histogramImage;
        }
    }

}
