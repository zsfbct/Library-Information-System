using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            
            InitializeComponent();
            string s = bookInfo.path;
            axAcroPDF1.LoadFile(s);// axAcroPDF1是你添加的pdf控件的默认名（name）

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 main = new Form6();
            main.Show();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
          
        }

        private void axAcroPDF1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
          
        }
    }
}
