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
    public partial class Form2 : Form
    {
        readonly string constr = Class_ConStr.constr;
        public Form2(string str)
        {
            InitializeComponent();
        }

        public Form2()
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                int i = e.RowIndex;
                bookInfo.bookID = dataGridView1.Rows[i].Cells[1].Value.ToString();
                Form5 bookInfomation = new Form5();
                bookInfomation.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "" && textBox3.Text == "" && textBox4.Text == "")
            {
                MessageBox.Show("请输入要查询的图书信息", "提示");
            }
            else
            {
                SqlConnection conn = new SqlConnection(constr);
                string sql = string.Format("select ID,bookname,edition,type author from books where bookname like '%{0}%' and author like '%{1}%' and ID like '%{2}%' and type like '%{3}%'", textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "SearchResults");
                DataTable dt = ds.Tables["SearchResults"];
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                DataGridViewCheckBoxColumn chosenCol = new DataGridViewCheckBoxColumn();
                chosenCol.Name = "Chosen";
                chosenCol.HeaderText = "Chosen";
                dataGridView1.Columns.Add(chosenCol);
                foreach (DataColumn col in dt.Columns)
                {
                    dataGridView1.Columns.Add(col.ColumnName, col.ColumnName);
                }
                dataGridView1.Rows.Add(dt.Rows.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dt.Columns.Count; j++)
                        dataGridView1.Rows[i].Cells[j + 1].Value = dt.Rows[i][j].ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ///this.Hide();//////登陆窗体隐藏
            Form6 main = new Form6();//////初始化主窗体
            main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到注册页面
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(constr);
            string check_booked_returned = string.Format("select * from booking where userID={0} and finished='是';", UserInfo.user_id);
            SqlDataAdapter da_check_booked_returned = new SqlDataAdapter(check_booked_returned, conn);
            DataSet ds_check_booked_returned = new DataSet();
            da_check_booked_returned.Fill(ds_check_booked_returned, "booked_returned");
            if (ds_check_booked_returned.Tables["booked_returned"].Rows.Count > 0)
            {
                string notice = string.Format("您预约的{0}本图书已经可以借阅，请到服务台领取！", ds_check_booked_returned.Tables["booked_returned"].Rows.Count);
                MessageBox.Show(notice, "提示");
                conn.Open();
                for (int i = 0; i < ds_check_booked_returned.Tables["booked_returned"].Rows.Count; i++)
                {
                    string book_id = ds_check_booked_returned.Tables["booked_returned"].Rows[i][2].ToString().Trim();
                    string set_booked = string.Format("update books set booking='否' where ID='{0}';", book_id);
                    SqlCommand cmd_set_booked = new SqlCommand(set_booked, conn);
                    cmd_set_booked.ExecuteNonQuery();
                    string delete_booking = string.Format("delete from booking where bookID='{0}';", book_id);
                    SqlCommand cmd_delete_booking = new SqlCommand(delete_booking, conn);
                    cmd_delete_booking.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();//////登陆窗体隐藏
            Form1 main = new Form1();//////初始化主窗体
            main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到注册页面
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            string book_borrowed = string.Format("select * from borrowrecords where userID={0} and returndate is null;", UserInfo.user_id);
            SqlConnection conn = new SqlConnection(constr);
            SqlDataAdapter da_borrowed_num = new SqlDataAdapter(book_borrowed, conn);
            DataSet ds_borrowed_num = new DataSet();
            da_borrowed_num.Fill(ds_borrowed_num, "num");
            if (ds_borrowed_num.Tables["num"].Rows.Count > 10)
                MessageBox.Show("您借阅的图书数量已经到达上限！", "提示");
            else
            {
                string book_id = textBox21.Text;
                try
                {
                    string find_book = string.Format("select bookname,location,available from books where ID='{0}';", book_id);
                    SqlDataAdapter da_find_book = new SqlDataAdapter(find_book, conn);
                    DataSet ds_find_book = new DataSet();
                    da_find_book.Fill(ds_find_book, "found");
                    string msg_found = string.Format("请确认您要借阅的是位于 {0} 的 {1} 吗？", ds_find_book.Tables["found"].Rows[0][1].ToString().Trim(), ds_find_book.Tables["found"].Rows[0][0].ToString().Trim());
                    if (MessageBox.Show(msg_found, "确认信息", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        if (ds_find_book.Tables["found"].Rows[0][2].ToString().Trim() == "是")
                        {
                            string get_record_num = string.Format("select ID from borrowrecords where borrowdate='{0}';", DateTime.Now.ToShortDateString());
                            int max_k = 0;
                            try
                            {
                                SqlDataAdapter da_get_record_num = new SqlDataAdapter(get_record_num, conn);
                                DataSet ds_get_record_num = new DataSet();
                                da_get_record_num.Fill(ds_get_record_num, "recordID");
                                DataTable dt_record_num = ds_get_record_num.Tables["recordID"];
                                int k = 0;
                                for (int i = 0; i < dt_record_num.Rows.Count; i++)
                                {
                                    string str = dt_record_num.Rows[i][0].ToString().Trim();
                                    str = str.Substring(str.Length - 4);
                                    k = Convert.ToInt32(str);
                                    if (k > max_k)
                                    {
                                        max_k = k;
                                    }
                                }
                                max_k++;
                            }
                            catch
                            {
                                max_k = 1;
                            }
                            string borrowrecord_id = string.Format("2{0}{1}", DateTime.Now.ToString("yyMMdd"), max_k.ToString().PadLeft(4, '0'));
                            string insert_borrow_record = string.Format("insert into borrowrecords (ID,borrowdate,renew,userID,bookID) values ({0},cast('{1}' as date),'否',{2},'{3}')", borrowrecord_id, DateTime.Now.ToShortDateString(), UserInfo.user_id, book_id);
                            string not_available = string.Format("update books set available='否' where ID='{0}';", book_id);
                            conn.Open();
                            SqlCommand cmd_set_available = new SqlCommand(not_available, conn);
                            SqlCommand cmd_insert_borrow_record = new SqlCommand(insert_borrow_record, conn);
                            cmd_insert_borrow_record.ExecuteNonQuery();
                            cmd_set_available.ExecuteNonQuery();
                            conn.Close();
                            string return_date = string.Format("借阅成功！请在{0}前按时还书。", DateTime.Now.AddDays(60).ToString("yyyy年M月d日"));
                            MessageBox.Show(return_date, "提示");
                        }
                        else
                        {
                            MessageBox.Show("该书籍不可外借，请见谅！", "提示");
                        }
                    textBox21.Text = null;
                }
                catch
                {
                    MessageBox.Show("您输入的书籍编号有误，请重新输入！", "提示");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//预约书籍
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            Boolean flag = true;
            Boolean already_booked = false;
            Boolean process = true;
            SqlConnection conn = new SqlConnection(constr);
            string bookID = "";
            int count = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "True")
                {
                    flag = false;
                    bookID = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    string sql = string.Format("select booking from books where ID='{0}'", bookID);
                    SqlDataAdapter da_ifbooked = new SqlDataAdapter(sql, conn);
                    DataSet ds_ifbooked = new DataSet();
                    da_ifbooked.Fill(ds_ifbooked, "if_booked");
                    Console.WriteLine(ds_ifbooked.Tables["if_booked"].Rows[0][0].ToString());
                    if (ds_ifbooked.Tables["if_booked"].Rows[0][0].ToString().Trim() == "是")
                    {
                        already_booked = true;
                        process = false;
                        dataGridView1.Rows[i].Cells[0].Value = false;
                    }
                    else
                        count++;
                }
            }
            string sql_count = string.Format("select count(bookingID) from booking where userID={0}", UserInfo.user_id.ToString());
            DataSet ds_count = new DataSet();
            SqlDataAdapter da_count = new SqlDataAdapter(sql_count, conn);
            da_count.Fill(ds_count, "count");
            if (flag)
                MessageBox.Show("请选择要预约的书目！", "提示");
            if (already_booked)
                if (MessageBox.Show("您选中的书目中有的已经被预约，是否预约其余的书目？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    process = true;
            if (Convert.ToInt32(ds_count.Tables["count"].Rows[0][0]) + count > 3)
            {
                string notice = string.Format("按照规定，每位读者最多同时预约3本书，您已经预约{0}本书，还可以预约{1}本。请重新选择！", ds_count.Tables["count"].Rows[0][0].ToString(), (3 - Convert.ToInt32(ds_count.Tables["count"].Rows[0][0])).ToString());
                MessageBox.Show(notice, "提示");
                process = false;
            }
            if (!flag && process)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "True")
                    {
                        string sql_update = string.Format("update books set booking='是' where ID='{0}'", dataGridView1.Rows[i].Cells[1].Value.ToString());
                        string sql_select = string.Format("select bookingID from booking where date=cast('{0}' as date)",DateTime.Now.ToShortDateString());
                        SqlDataAdapter da = new SqlDataAdapter(sql_select, conn);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "ids");
                        int new_num = 0;
                        if (ds.Tables["ids"].Rows.Count==0)
                            new_num = 1;
                        else
                        {
                            int max_num = 0;
                            for (int j=0;j<ds.Tables["ids"].Rows.Count;j++)
                            {
                                string this_id = ds.Tables["ids"].Rows[j][0].ToString();
                                int id_num = Convert.ToInt32(this_id.Substring(this_id.Length - 4));
                                if (id_num > max_num)
                                    max_num = id_num;
                            }
                            new_num = max_num + 1;
                        }
                        string new_id = string.Format("2{0}{1}",DateTime.Now.ToString("yyMMdd"),new_num.ToString().PadLeft(4,'0'));
                        string date = DateTime.Now.ToString();
                        string sql_insert = string.Format("insert into booking values ({0},{1},'{2}','{3}','否');", new_id, UserInfo.user_id.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString(),date);
                        conn.Open();
                        SqlCommand cmd1 = new SqlCommand(sql_update, conn);
                        SqlCommand cmd2 = new SqlCommand(sql_insert, conn);
                        cmd1.ExecuteNonQuery();
                        cmd2.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("预约成功！请注意接收本馆的通知。", "提示");
                    }
            }
        }

        private void button8_Click(object sender, EventArgs e)//刷新借阅记录
        {
            string get_borrow_record = string.Format("select borrowdate,bookID,renew from borrowrecords where userID={0} and returndate is null", UserInfo.user_id);
            SqlConnection conn = new SqlConnection(constr);
            SqlDataAdapter da_get_borrow_record = new SqlDataAdapter(get_borrow_record, conn);
            DataSet ds_borrow_record = new DataSet();
            da_get_borrow_record.Fill(ds_borrow_record, "borrow_record");
            DataTable dt_borrow_record = ds_borrow_record.Tables["borrow_record"];
            if (dt_borrow_record.Rows.Count > 0)
            {
                dataGridView2.Columns.Clear();
                dataGridView2.Rows.Clear();
                dataGridView2.AllowUserToAddRows = false;
                DataGridViewCheckBoxColumn chosenCol = new DataGridViewCheckBoxColumn();
                chosenCol.Name = "Chosen";
                chosenCol.HeaderText = "Chosen";
                dataGridView2.Columns.Add(chosenCol);
                dataGridView2.Columns.Add("bookID", "Book ID");
                dataGridView2.Columns.Add("bookname", "Book Name");
                dataGridView2.Columns.Add("borrowdate", "Borrow Date");
                dataGridView2.Columns.Add("renew", "If Renewed");
                dataGridView2.Columns.Add("returndate", "Return Date");
                dataGridView2.Rows.Add(dt_borrow_record.Rows.Count);
                for (int i = 0; i < dt_borrow_record.Rows.Count; i++)
                {
                    string get_book_name = string.Format("select bookname from books where ID='{0}';", dt_borrow_record.Rows[i][1].ToString().Trim());
                    SqlDataAdapter da_get_book_name = new SqlDataAdapter(get_book_name, conn);
                    DataSet ds_book_name = new DataSet();
                    da_get_book_name.Fill(ds_book_name, "book_name");
                    string book_name = ds_book_name.Tables["book_name"].Rows[0][0].ToString().Trim();
                    dataGridView2.Rows[i].Cells[0].Value = false;
                    dataGridView2.Rows[i].Cells[1].Value = dt_borrow_record.Rows[i][1].ToString().Trim();
                    dataGridView2.Rows[i].Cells[2].Value = book_name;
                    dataGridView2.Rows[i].Cells[3].Value = Convert.ToDateTime(dt_borrow_record.Rows[i][0].ToString().Trim()).ToShortDateString();
                    dataGridView2.Rows[i].Cells[4].Value = dt_borrow_record.Rows[i][2].ToString().Trim();
                    int add_days = 0;
                    if (dt_borrow_record.Rows[i][2].ToString().Trim() == "是")
                        add_days = 90;
                    else
                        add_days = 60;
                    dataGridView2.Rows[i].Cells[5].Value = Convert.ToDateTime(dt_borrow_record.Rows[i][0].ToString().Trim()).AddDays(add_days).ToShortDateString();
                }
            }
            else
            {
                MessageBox.Show("您所有借阅的图书都已归还！", "提示");
                dataGridView2.Columns.Clear();
                dataGridView2.Rows.Clear();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            Boolean flag = false;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
                if (dataGridView2.Rows[i].Cells[0].Value.ToString() == "True")
                {
                    flag = true;
                    break;
                }
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            if (flag)
            {
                Boolean if_late = false;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    if (dataGridView2.Rows[i].Cells[0].Value.ToString() != "True")
                        continue;
                    string book_id = dataGridView2.Rows[i].Cells[1].Value.ToString().Trim();
                    string sql_return = string.Format("update borrowrecords set returndate=cast('{0}' as date) where bookID='{1}';", DateTime.Now.ToShortDateString(), book_id);
                    SqlCommand cmd_return = new SqlCommand(sql_return, conn);
                    cmd_return.ExecuteNonQuery();
                    string check_booked = string.Format("select userID from booking where bookID='{0}';", book_id);
                    SqlDataAdapter da_if_booked = new SqlDataAdapter(check_booked, conn);
                    DataSet ds_if_booked = new DataSet();
                    da_if_booked.Fill(ds_if_booked, "if_booked");
                    if (ds_if_booked.Tables["if_booked"].Rows.Count > 0)
                    {
                        string set_returned = string.Format("update booking set finished='是' where bookID='{0}';", book_id);
                        SqlCommand cmd_set_returned = new SqlCommand(set_returned, conn);
                        cmd_set_returned.ExecuteNonQuery();
                    }
                    else
                    {
                        string set_available = string.Format("update books set available='是' where ID='{0}';", book_id);
                        SqlCommand cmd_set_available = new SqlCommand(set_available, conn);
                        cmd_set_available.ExecuteNonQuery();
                    }
                    if (string.Compare(dataGridView2.Rows[i].Cells[4].Value.ToString(), DateTime.Now.ToShortDateString()) > 0)
                        if_late = true;
                }
                if (if_late)
                {
                    MessageBox.Show("您有图书逾期归还，请将已经归还的图书放在还书箱中，携带读者证前往前台缴纳误期费用！", "提示");
                }
                else
                {
                    MessageBox.Show("归还成功！请将已经归还的图书放在还书箱中，谢谢您的配合。", "提示");
                }
                button8_Click(sender, e);
            }
            else
            {
                MessageBox.Show("请选择要归还的书目！", "提示");
            }
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            Boolean flag = false;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
                if (dataGridView2.Rows[i].Cells[0].Value.ToString() == "True")
                {
                    flag = true;
                    break;
                }
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            if (flag)
            {
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    if (dataGridView2.Rows[i].Cells[0].Value.ToString() != "True")
                        continue;
                    string book_id = dataGridView2.Rows[i].Cells[1].Value.ToString().Trim();
                    string sql_renew = string.Format("update borrowrecords set renew='是' where bookID='{0}';", book_id);
                    SqlCommand cmd_renew = new SqlCommand(sql_renew, conn);
                    cmd_renew.ExecuteNonQuery();
                }
                MessageBox.Show("续借成功！", "提示");
                button8_Click(sender, e);
            }
            else
            {
                MessageBox.Show("请选择要续借的书目！", "提示");
            }
            conn.Close();
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
