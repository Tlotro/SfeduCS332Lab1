using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task3
{
    public partial class Form1 : Form
    {
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
    }
}
