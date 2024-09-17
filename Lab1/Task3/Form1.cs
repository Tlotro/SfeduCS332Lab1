using FastBitmap;
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

namespace Task3
{
    public partial class Form1 : Form
    {
        private Bitmap bm;
        private (double, float, float)[,] HSV;
        public Form1()
        {
            InitializeComponent();
        }
        private void trackBarV_Scroll(object sender, EventArgs e)
        {
            labelV.Text = (trackBarV.Value>0?"+":"")+trackBarV.Value.ToString();
        }

        private void trackBarH_Scroll(object sender, EventArgs e)
        {
            labelH.Text = (trackBarH.Value > 0 ? "+" : "") + trackBarH.Value.ToString();
        }

        private void trackBarS_Scroll(object sender, EventArgs e)
        {
            labelS.Text = (trackBarS.Value > 0 ? "+" : "") + trackBarS.Value.ToString();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                Message.Text = "Please, enter the file path";
            else if (!File.Exists(textBox1.Text))
                Message.Text = "File does not exist";
            else
            {
                Message.Text = "Warning, file already exist";
                bm = new Bitmap(textBox1.Text);
                

                pictureBox1.Image = bm;
                SaveButton.Enabled = true;
                UpdateButton.Enabled = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                Message.Text = "Confirm override";
                OverrideButton.Enabled = true;
            }
            else
                pictureBox1.Image.Save(textBox1.Text);
        }

        private void OverrideButton_Click(object sender, EventArgs e)
        {
            Message.Text = "Warning, file already exist";
            OverrideButton.Enabled = false;
            pictureBox1.Image.Save(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                LoadButton.Enabled = false;
                SaveButton.Enabled = false;
            }
            else
            {
                if (pictureBox1.Image != null)
                {
                    SaveButton.Enabled = true;
                    if (File.Exists(textBox1.Text))
                        Message.Text = "Warning, file already exist";
                    else
                        Message.Text = "";
                }
                LoadButton.Enabled = true;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            HSV = new (double, float, float)[bm.Width, bm.Height];
            using (var fastBitmap = new FastBitmap.FastBitmap(bm))
            {
                for (var x = 0; x < fastBitmap.Width; x++)
                    for (var y = 0; y < fastBitmap.Height; y++)
                    {
                        var color = fastBitmap[x, y];

                        int M = Math.Max(color.R, Math.Max(color.G, color.B));
                        int m = Math.Min(color.R, Math.Min(color.G, color.B));
                        double H;
                        if (M == m)
                            H = 0;
                        else if ((M == color.R) && (color.G >= color.B))
                            H = 60 * (double)(color.G - color.B) / (M - m);
                        else if ((M == color.R) && (color.G < color.B))
                            H = (60 * (double)(color.G - color.B) / (M - m)) + 360;
                        else if (M == color.G)
                            H = (60 * (double)(color.B - color.R) / (M - m)) + 120;
                        else H = (60 * (double)(color.R - color.G) / (M - m)) + 240;

                        float S = M == 0 ? 0 : (1 - (m / M));
                        float V = M / 255f;
                        HSV[x, y] = (H, S, V);
                    }
            }

            if (HSV != null)
            {
                for (var x = 0; x < HSV.GetLength(0); x++)
                    for (var y = 0; y < HSV.GetLength(1); y++)
                    {
                        float S = trackBarS.Value / 100f;
                        float V = trackBarV.Value / 100f;
                        HSV[x, y].Item1 = (HSV[x, y].Item1 + trackBarH.Value) % 360;
                        HSV[x, y].Item2 = (HSV[x, y].Item2 * (Math.Min(1, S + 1) - Math.Max(0, S)) + Math.Max(0, S));
                        HSV[x, y].Item3 = (HSV[x, y].Item3 * (Math.Min(1, V + 1) - Math.Max(0, V)) + Math.Max(0, V));
                    }

                Bitmap bmn = new Bitmap(bm.Width, bm.Height);
                for (var x = 0; x < HSV.GetLength(0); x++)
                    for (var y = 0; y < HSV.GetLength(1); y++)
                    {
                        int hi = Convert.ToInt32(Math.Floor(HSV[x, y].Item1 / 60)) % 6;
                        double f = HSV[x, y].Item1 / 60 - Math.Floor(HSV[x, y].Item1 / 60);

                        int value = (int)(HSV[x, y].Item3 * 255);
                        int v = Convert.ToInt32(value);
                        int p = Convert.ToInt32(value * (1 - HSV[x, y].Item2));
                        int q = Convert.ToInt32(value * (1 - f * HSV[x, y].Item2));
                        int t = Convert.ToInt32(value * (1 - (1 - f) * HSV[x, y].Item2));

                        if (hi == 0)
                            bmn.SetPixel(x, y, Color.FromArgb(255, v, t, p));
                        else if (hi == 1)
                            bmn.SetPixel(x, y, Color.FromArgb(255, q, v, p));
                        else if (hi == 2)
                            bmn.SetPixel(x, y, Color.FromArgb(255, p, v, t));
                        else if (hi == 3)
                            bmn.SetPixel(x, y, Color.FromArgb(255, p, q, v));
                        else if (hi == 4)
                            bmn.SetPixel(x, y, Color.FromArgb(255, t, p, v));
                        else
                            bmn.SetPixel(x, y, Color.FromArgb(255, v, p, q));
                    }
                pictureBox1.Image = bmn;
            }
        }
    }
}
