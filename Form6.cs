using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace login
{
    
    public partial class Form6 : Form

    {

        readonly string constr = Class_ConStr.constr;
        public Form6()
        {
            InitializeComponent();
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            //查看所有文献信息
            SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * from literatures", conn);
            DataSet sourceDataSet1 = new DataSet();
            adapter1.Fill(sourceDataSet1);
            dataGridView1.DataSource = sourceDataSet1.Tables[0];
            conn.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string name = textBox1.Text;
            string author = textBox2.Text;
            string keyword = textBox3.Text;

            DataTable dt = (DataTable)dataGridView1.DataSource;
            dt.Rows.Clear();
            dataGridView1.DataSource = dt;
            string sql = string.Format("SELECT * FROM literatures where name like '%{0}%' and author like '%{1}%' and keyword like '%{2}%'", textBox1.Text, textBox2.Text, textBox3.Text);///改为了模糊查询，%xx%表示字符串含有xx
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            //SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM literatures WHERE name  like'" + name + "' or author = '" +author+ "' or keyword = '" + keyword + "' ;", connection);
            DataSet sourceDataSet = new DataSet();
            adapter.Fill(sourceDataSet);
            dataGridView1.DataSource = sourceDataSet.Tables[0];
            connection.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //获得当前选中的行   
            int rowindex = e.RowIndex;
            //获得当前行的第0列的值   
            textBox4.Text = dataGridView1.Rows[rowindex].Cells[0].Value.ToString();//读取ID
            textBox5.Text = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + dataGridView1.Rows[rowindex].Cells[5].Value.ToString().TrimEnd();///读取文件位置
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string literatureID = textBox4.Text;
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT content FROM literatures WHERE literatureID ='" + literatureID + "'  ;", connection);

            bookInfo.path = textBox5.Text ;///DOI
            
            this.Hide();
            Form7 main = new Form7();//////初始化主窗体
            main.Show();


        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form2 main = new Form2();
            main.Show();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

            ToolTip toolTip1 = new ToolTip();
            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.textBox4, "从表格中选择一条数据，即可自动输入");
            toolTip1.SetToolTip(this.textBox5, "从表格中选择一条数据，即可自动输入");
        }
    }
}
