using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSwepper
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public bool cancel { get; set; }
        public int width { get; set; } 
        public int height { get; set; } 
        public int mines { get; set; } 

        private void button2_Click(object sender, EventArgs e)
        {
            cancel = false;
            width = Convert.ToInt16(textBox1.Text);
            height = Convert.ToInt16(textBox2.Text);
            mines = Convert.ToInt16(textBox3.Text);
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
        }
    }
}
