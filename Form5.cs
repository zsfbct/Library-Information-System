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
    public partial class Form5 : Form
    {
        readonly string constr = Class_ConStr.constr;
        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            string bookID = bookInfo.bookID;
            SqlConnection conn = new SqlConnection(constr);
            string sql = string.Format("select books.ID,books.location,books.bookname,books.edition,books.available,books.type,books.abstract,books.author,publishers.publishername from books inner join publishers on books.publisherID=publishers.ID where books.ID='{0}';", bookID);
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Details");
            DataTable dt = ds.Tables["Details"];
            DataRow row = dt.Rows[0];
            label7.Text = row[2].ToString();
            label8.Text = row[0].ToString();
            label9.Text = row[3].ToString();
            label10.Text = row[8].ToString();
            label11.Text = row[1].ToString();
            textBox1.Text = row[6].ToString();
            label12.Text = row[5].ToString();
            label15.Text = row[7].ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
