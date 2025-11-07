using CleanPlanetApp.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clean
{
    public partial class Managers : Form
    {
        public Managers()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UprPart uprPart = new UprPart();
            this.Close();
            uprPart.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServiceCostForm costForm = new ServiceCostForm();
            this.Close();
            costForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            this.Close();    
            form1.ShowDialog();
        }
    }
}
