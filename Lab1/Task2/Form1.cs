using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Task2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap ExtractChannel(Bitmap orig, char channel)
        {
            Bitmap rIm = new Bitmap(orig.Width, orig.Height);
            for (int y = 0; y < orig.Height; y++)
            {
                for (int x = 0; x < orig.Width; x++)
                {
                    Color pixelColor = orig.GetPixel(x, y);
                    Color newColor;
                    switch (channel) {
                        case 'R':
                            newColor = Color.FromArgb(pixelColor.A, pixelColor.R, 0, 0);
                            break;
                        case 'G':
                            newColor = Color.FromArgb(pixelColor.A, 0, pixelColor.G, 0);
                            break;
                        case 'B':
                            newColor = Color.FromArgb(pixelColor.A, 0, 0, pixelColor.B);
                            break;
                        default:
                            newColor = Color.FromArgb(pixelColor.A, pixelColor.R, 0, 0);
                            break;
                    }
                    rIm.SetPixel(x, y, newColor);
                }
            }
            return rIm;
        }

        private static void AnalyzeBitmap(Bitmap bitmap, Chart chart, char channel)
        {
            chart.Series.Clear();
            Series series = new Series("Pixels");
            series.ChartType = SeriesChartType.Spline;

            int[] values = new int[256];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    switch (channel)
                    {
                        case 'R':
                            values[bitmap.GetPixel(x, y).R]++;
                            break;
                        case 'G':
                            values[bitmap.GetPixel(x, y).G]++;
                            break;
                        case 'B':
                            values[bitmap.GetPixel(x, y).B]++;
                            break;
                        default:
                            values[bitmap.GetPixel(x, y).R]++;
                            break;
                    }
                }
            }
            for (int x = 0;x < 256; x++)
            {
                series.Points.AddXY(x, values[x]);
                switch (channel)
                {
                    case 'R':
                        series.Points[x].Color = Color.Red;
                        break;
                    case 'G':
                        series.Points[x].Color = Color.Green;
                        break;
                    case 'B':
                        series.Points[x].Color = Color.Blue;
                        break;
                    default:
                        series.Points[x].Color = Color.Blue;
                        break;
                }
            }
            
            chart.Series.Add(series);
            chart.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPEG Images|*.jpg|PNG Images|*.png";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK) {
                
                Bitmap orig = new Bitmap(dialog.FileName);
                pictureBox1.Image = orig;
                Bitmap rIm = ExtractChannel(orig, 'R');
                pictureBox2.Image = rIm;
                Bitmap gIm = ExtractChannel(orig, 'G');
                pictureBox3.Image = gIm;
                Bitmap bIm = ExtractChannel(orig, 'B');
                pictureBox4.Image = bIm;

                AnalyzeBitmap(rIm, chart1, 'R');
                AnalyzeBitmap(gIm, chart2, 'G');
                AnalyzeBitmap(bIm, chart3, 'B');
            }
        
        }
    }
}
