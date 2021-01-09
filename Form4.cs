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
using System.Text.RegularExpressions;
using System.Globalization;

namespace login
{
    public partial class Form4 : Form
    {
        readonly string constr = Class_ConStr.constr;
        public Form4(string str)
        
        {
            InitializeComponent();
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            //核查信用分
            SqlDataAdapter da_get_all_users = new SqlDataAdapter("select ID from users;",conn);
            DataSet ds_all_users = new DataSet();
            da_get_all_users.Fill(ds_all_users,"all_users");
            DataTable dt_all_users = ds_all_users.Tables["all_users"];
            for (int i=0;i<dt_all_users.Rows.Count;i++)
            {
                string get_users_records = string.Format("select borrowdate,renew,returndate from borrowrecords where userID={0} and returndate is not null;",dt_all_users.Rows[i][0].ToString().Trim());
                SqlDataAdapter da_records = new SqlDataAdapter(get_users_records, conn);
                DataSet ds_records = new DataSet();
                da_records.Fill(ds_records,"records");
                DataTable dt_records = ds_records.Tables["records"];
                int n = dt_records.Rows.Count;
                int credit = 0;
                if (n == 0)
                    credit = 80;
                else
                {
                    int[] credits = new int[n];
                    for (int j=0;j<n;j++)
                    {
                        TimeSpan timeSpan = Convert.ToDateTime(dt_records.Rows[j][2].ToString().Trim()) - Convert.ToDateTime(dt_records.Rows[j][0].ToString().Trim());
                        int days = 0;
                        if (dt_records.Rows[j][1].ToString().Trim() == "是")
                            days = timeSpan.Days - 60;
                        else
                            days = timeSpan.Days - 30;
                        if (days <= 0)
                            credits[j] = 100;
                        else if (days <= 15)
                            credits[j] = 80;
                        else if (days <= 30)
                            credits[j] = 60;
                        else if (days <= 60)
                            credits[j] = 40;
                        else
                            credits[j] = 0;
                    }
                    if (n<=3)
                    {
                        int sum = 0;
                        for (int j = 0; j < n; j++)
                            sum += credits[j];
                        credit = sum / n;
                    }
                    else
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        for (int j = 0; j < n - 3; j++)
                            sum1 += credits[j];
                        for (int j = n - 3; j < n; j++)
                            sum2 += credits[j];
                        credit = Convert.ToInt32(sum1 / (n - 3) * 0.3 + sum2 / 3 * 0.7);
                    }
                }
                string update_credit = string.Format("update users set credit={0} where ID={1}",credit, dt_all_users.Rows[i][0].ToString().Trim());
                SqlCommand cmd_update_credit = new SqlCommand(update_credit, conn);
                cmd_update_credit.ExecuteNonQuery();
            }
            //查看所有读者信息
            SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT ID as '读者编号',username as '读者名',userpwd as '读者密码',age as '年龄',idcardnumber as '身份证号',phonenumber as '电话号码', date as '注册日期',credit as '信用评分' FROM users", conn);
            DataSet sourceDataSet1 = new DataSet();
            adapter1.Fill(sourceDataSet1);
            dataGridView1.DataSource = sourceDataSet1.Tables[0];
            //查看所有书籍信息
            SqlDataAdapter adapter2 = new SqlDataAdapter("SELECT books.ID as '书籍编号',books.location as '所在分馆',books.bookname as '书籍名称',books.edition as '书籍版本',books.available as '在架上', books.type as '书籍类别',books.abstract as '书籍简介',books.author as '作者',publishers.ID as '出版社ID',publishers.publishername as '出版社',books.booking as '是否被预约',books.ISBN as 'ISBN' FROM books, publishers where publishers.ID = books.publisherID", conn);
            DataSet sourceDataSet2 = new DataSet();
            adapter2.Fill(sourceDataSet2);
            dataGridView2.DataSource = sourceDataSet2.Tables[0];
            //查看所有借阅信息
            SqlDataAdapter adapter3 = new SqlDataAdapter("SELECT borrowrecords.ID as'借阅编号',borrowrecords.borrowdate as '借阅日期',borrowrecords.renew as'是否续约',borrowrecords.userID as '读者编号',users.username as '读者姓名',books.ID as '书籍编号',books.bookname as '书籍名称',borrowrecords.returndate as '归还日期'FROM borrowrecords, users, books where users.ID = borrowrecords.userID and books.ID = borrowrecords.bookID", conn);
            DataSet sourceDataSet3 = new DataSet();
            adapter3.Fill(sourceDataSet3);
            dataGridView3.DataSource = sourceDataSet3.Tables[0];
            //查看所有购买信息
            SqlDataAdapter adapter4 = new SqlDataAdapter("select purchase.purchaseID as '购书编号',purchase.date as '购书日期',books.ID as '书籍编号',books.bookname as '书籍名称',purchase.price as '书籍单价',libraries.libraryID as '所属分馆编号',libraries.name as '所属分馆名称' from books, purchase, libraries where books.ID = purchase.bookID and libraries.libraryID = purchase.libraryID", conn);
            DataSet sourceDataSet4 = new DataSet();
            adapter4.Fill(sourceDataSet4);
            dataGridView4.DataSource = sourceDataSet4.Tables[0];
            dataGridView4.ForeColor = Color.Black;
            //查看所有预定信息
            SqlDataAdapter adapter5 = new SqlDataAdapter("SELECT booking.bookingID as '预约编号',booking.userID as '读者编号',users.username as '读者名字',books.bookname as '书籍名称', booking.bookID as '书籍编号', booking.date as '预约时间',booking.finished as '是否已完成' FROM booking, users, books where booking.bookID = books.ID and users.ID = booking.userID", conn);
            DataSet sourceDataSet5 = new DataSet();
            adapter5.Fill(sourceDataSet5);
            dataGridView5.DataSource = sourceDataSet5.Tables[0];
            dataGridView5.ForeColor = Color.Black;
            //查看所有文献信息
            SqlDataAdapter adapter6 = new SqlDataAdapter("SELECT literatureID as '文献编号',name as '文献名字', keyword as '关键字',abstract as '简介',author as '作者',content as '地址',cited as '被引量',year as '刊登年份',journal as '所属期刊', DOI FROM literatures", conn);
            DataSet sourceDataSet6 = new DataSet();
            adapter6.Fill(sourceDataSet6);
            dataGridView6.DataSource = sourceDataSet6.Tables[0];
            dataGridView6.ForeColor = Color.Black;
            //查看所有出版社信息
            SqlDataAdapter adapter7 = new SqlDataAdapter("SELECT ID as '出版社编号',publishername as '出版社名',location as '出版社地址',phonenumber as '出版社电话' FROM publishers", conn);
            DataSet sourceDataSet7 = new DataSet();
            adapter7.Fill(sourceDataSet7);
            dataGridView7.DataSource = sourceDataSet7.Tables[0];
            dataGridView7.ForeColor = Color.Black;
            //查看所有文献阅览信息
            SqlDataAdapter adapter8 = new SqlDataAdapter("SELECT literatures.literatureID as '文献编号',literatures.name as '文献名',readings.userID as '读者编号',users.username as '读者名字',readings.time as '阅览时间' FROM readings, users, literatures where readings.literatureID = literatures.literatureID and readings.userID = users.ID", conn);
            DataSet sourceDataSet8 = new DataSet();
            adapter8.Fill(sourceDataSet8);
            dataGridView8.DataSource = sourceDataSet8.Tables[0];
            dataGridView8.ForeColor = Color.Black;

            conn.Close();
        }

        private void Form4_Load(object sender, EventArgs e) //初始将整个用户表/图书表/借阅表/购书表显示
        {
      
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) //获取表中的所选行，将用户信息显示在右侧文字框中
        {

            //获得当前选中的行   
            int rowindex = e.RowIndex;

            //获得当前行的第0列的值   
            textBoxID.Text = dataGridView1.Rows[rowindex].Cells[0].Value.ToString().Trim();
            //获得当前行的第一列的值  
            textBoxusername.Text = dataGridView1.Rows[rowindex].Cells[1].Value.ToString().Trim();
            //获得当前行的第二列的值   
            textBoxuserpwd.Text = dataGridView1.Rows[rowindex].Cells[2].Value.ToString().Trim();
            textBoxage.Text = dataGridView1.Rows[rowindex].Cells[3].Value.ToString().Trim();
            textBoxidcardnumber.Text = dataGridView1.Rows[rowindex].Cells[4].Value.ToString().Trim();
            textBoxphonenumber.Text = dataGridView1.Rows[rowindex].Cells[5].Value.ToString().Trim();

        }
        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBoxusername.Text="";
            textBoxidcardnumber.Text = "";
            textBoxphonenumber.Text = "";
            textBoxID.Text = "";
            textBoxuserpwd.Text = "";
            textBoxage.Text = "";


        }
        private void button2_Click(object sender, EventArgs e)//添加用户
        {
            int k = 0;
            string tips1 = "请输入：";///生成提示
          
            if (textBoxuserpwd.Text == "")
            {
                tips1 += "密码";
                k = 1;
            }

           

            if (textBoxusername.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "姓名";
                }
                else { tips1 += "、姓名"; }
                k = 1;
            }



            if (textBoxage.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "年龄";
                }
                else { tips1 += "、年龄"; }
                k = 1;
            }

            if (textBoxidcardnumber.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "身份证号";
                }
                else { tips1 += "、身份证号"; }
                k = 1;
            }
            if (textBoxphonenumber.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "电话号码";
                }
                else { tips1 += "、电话号码"; }
                k = 1;
            }
            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            string year, month, day;
            year = DateTime.Now.Year.ToString(); ///获取年份   // 2018
            month = DateTime.Now.Month.ToString("00"); ///获取月份,"00"代表至少两位数   // 12
            day = DateTime.Now.Day.ToString("00"); ///获取该月中的第几天，"00"代表至少两位数   // 03

            int num = 0;///表示注册的馆的序号
            if ((Convert.ToInt32(day)%2)==1)
            {
                num = 1;
            }
            else
            {
                num = 2; 
            }
          

            int k2 = 0;
            string tips2 = null;
            Regex rx = new Regex("^[0-9]*$");
            if (rx.IsMatch(textBoxage.Text.TrimEnd())) { }//判断年龄是否为数字
            else
            {
                k2 = 1;
                tips2 += "年龄";
                textBoxage.Text = null;
            }
            if (rx.IsMatch(textBoxidcardnumber.Text.TrimEnd())) { }//判断身份证号是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "身份证号";
                }
                else { tips2 += "、身份证号"; }
                textBoxidcardnumber.Text = null;
            }
            if (rx.IsMatch(textBoxphonenumber.Text.TrimEnd())) { }//判断电话号码是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "电话号码";
                }
                else { tips2 += "、电话号码"; }
                textBoxphonenumber.Text = null;
            }
            if (k2 == 1)
            {
                tips2 += "必须为数字，请重新输入";
                MessageBox.Show(tips2, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

           

            string Userpwd = Convert.ToString(textBoxuserpwd.Text);///把“密码”转换为字符串
            string Username = Convert.ToString(textBoxusername.Text);///把“姓名”转换为字符串
            string Age = Convert.ToString(textBoxage.Text);///把“年龄”转换为字符串            
            string Idcardnumber = Convert.ToString(textBoxidcardnumber.Text);///把“身份证号”转换为字符串
            string Phonenumber = Convert.ToString(textBoxphonenumber.Text);///把“电话号码”转换为字符串

            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string sql1 = "select top 1 ID from library.dbo.users order by date desc";
            SqlCommand command1 = new SqlCommand(sql1, connection);
            string lastID = Convert.ToString(command1.ExecuteScalar());///把最后一次输入的ID存入lastID

            string sqlYear = lastID.Substring(1, 2);///获取最后一次操作的年份//20
            string sqlMonth = lastID.Substring(3, 2);///获取最后一次操作的月份//06
            string sqlDay = lastID.Substring(5, 2);///获取最后一次操作的日期//08
            connection.Close();
            if (k == 0 && k2 == 0 && num != 0)
            { ///如果填写完整且正确
                string ID;///生成新账号
                if (year.Substring(2, 2) != sqlYear || month != sqlMonth || day != sqlDay)
                {
                    ID = num.ToString() + year.Substring(2, 2) + month + day + 1.ToString("00");///注册馆编号+年的后两位+月+日期+‘01‘
                }
                else
                {
                    int sqlList0 = Convert.ToInt32(lastID.Substring(7, 2)) + 1;
                    string sqlList = sqlList0.ToString("00");
                    ID = num.ToString() + year.Substring(2, 2) + month + day + sqlList;
                }

                textBoxbookname.Text = ID;
                SqlConnection connection2 = new SqlConnection(constr);
                connection2.Open();

                string sql2 = @"INSERT INTO [users](ID,username,userpwd,age,idcardnumber,phonenumber,date)" + "VALUES ('" + ID + "','" + Username + "','" + Userpwd + "','" + Age + "','" + Idcardnumber + "','" + Phonenumber + "','" + DateTime.Now + "')";
                SqlCommand command2 = new SqlCommand(sql2, connection2);
                int count = command2.ExecuteNonQuery();
                connection2.Close();

                string mes = "请记住您的账号:";

                if (count > 0)
                {
                    mes = "注册成功!" + mes + ID;
                    MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                    SqlConnection conn = new SqlConnection(constr);
                    conn.Open();
                    SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT ID as '读者编号',username as '读者名',userpwd as '读者密码',age as '年龄',idcardnumber as '身份证号',phonenumber as '电话号码', date as '注册日期',credit as '信用评分' FROM users", conn);
                    DataSet sourceDataSet1 = new DataSet();
                    adapter1.Fill(sourceDataSet1);
                    dataGridView1.DataSource = sourceDataSet1.Tables[0];
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("注册失败，请重试", "提示", MessageBoxButtons.OKCancel);
                }
            }
        }
        
        private void button1_Click(object sender, EventArgs e)//查询用户（输入ID，姓名，电话，身份证号码都可以）
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string username = textBoxusername.Text;
            string idcardnumber = textBoxidcardnumber.Text;
            string phonenumber = textBoxphonenumber.Text;
            string ID = textBoxID.Text;
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM users WHERE username  ='" + username + "' or ID = '" + ID + "' or idcardnumber = '" + idcardnumber + "' or phonenumber = '" + phonenumber + "';", connection);
            DataSet sourceDataSet = new DataSet();
            adapter.Fill(sourceDataSet);
            dataGridView1.DataSource = sourceDataSet.Tables[0];
            connection.Close();

        }

        private void button4_Click(object sender, EventArgs e)//修改用户信息
        {
            int k = 0;
            string tips1 = "请输入：";///生成提示

            if (textBoxuserpwd.Text == "")
            {
                tips1 += "密码";
                k = 1;
            }



            if (textBoxusername.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "姓名";
                }
                else { tips1 += "、姓名"; }
                k = 1;
            }



            if (textBoxage.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "年龄";
                }
                else { tips1 += "、年龄"; }
                k = 1;
            }

            if (textBoxidcardnumber.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "身份证号";
                }
                else { tips1 += "、身份证号"; }
                k = 1;
            }
            if (textBoxphonenumber.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "电话号码";
                }
                else { tips1 += "、电话号码"; }
                k = 1;
            }
            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
      
            int k2 = 0;
            string tips2 = null;
            Regex rx = new Regex("^[0-9]*$");
            if (rx.IsMatch(textBoxage.Text.TrimEnd())) { }//判断年龄是否为数字
            else
            {
                k2 = 1;
                tips2 += "年龄";
                textBoxage.Text = null;
            }
            if (rx.IsMatch(textBoxidcardnumber.Text.TrimEnd())) { }//判断身份证号是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "身份证号";
                }
                else { tips2 += "、身份证号"; }
                textBoxidcardnumber.Text = null;
            }
            if (rx.IsMatch(textBoxphonenumber.Text.TrimEnd())) { }//判断电话号码是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "电话号码";
                }
                else { tips2 += "、电话号码"; }
                textBoxphonenumber.Text = null;
            }
            if (k2 == 1)
            {
                tips2 += "必须为数字，请重新输入";
                MessageBox.Show(tips2, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }



            string Userpwd = Convert.ToString(textBoxuserpwd.Text);///把“密码”转换为字符串
            string Username = Convert.ToString(textBoxusername.Text);///把“姓名”转换为字符串
            string Age = Convert.ToString(textBoxage.Text);///把“年龄”转换为字符串            
            string Idcardnumber = Convert.ToString(textBoxidcardnumber.Text);///把“身份证号”转换为字符串
            string Phonenumber = Convert.ToString(textBoxphonenumber.Text);///把“电话号码”转换为字符串
            string ID= Convert.ToString(textBoxID.Text);
                SqlConnection connection2 = new SqlConnection(constr);
                connection2.Open();

                string sql2 = "update users set username='" + Username + "',userpwd='" + Userpwd + "',age='" + Age + "', idcardnumber='" + Idcardnumber + "',phonenumber='" + Phonenumber + "' where ID='" + ID + "'"; ;
                SqlCommand command2 = new SqlCommand(sql2, connection2);
                int count = command2.ExecuteNonQuery();
                connection2.Close();

                string mes = "请记住您的账号:";

                if (count > 0)
                {
                    mes = "修改成功!" ;
                    MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT ID as '读者编号',username as '读者名',userpwd as '读者密码',age as '年龄',idcardnumber as '身份证号',phonenumber as '电话号码', date as '注册日期',credit as '信用评分' FROM users", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView1.DataSource = sourceDataSet1.Tables[0];
                conn.Close();
            }
                else
                {
                    MessageBox.Show("修改失败，请重试", "提示", MessageBoxButtons.OKCancel);
                }
            }

        private void button3_Click(object sender, EventArgs e)//删除用户
        {
            string ID = textBoxID.Text;
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();

            string sql2 = "DELETE FROM users  WHERE ID = '"+ID+"';";
            SqlCommand command2 = new SqlCommand(sql2, connection2);
           
            int count = command2.ExecuteNonQuery();

            connection2.Close();
            string mes = "";

            if (count > 0)
            {
                mes = "删除成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT ID as '读者编号',username as '读者名',userpwd as '读者密码',age as '年龄',idcardnumber as '身份证号',phonenumber as '电话号码', date as '注册日期',credit as '信用评分' FROM users", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView1.DataSource = sourceDataSet1.Tables[0];
                conn.Close();

            }
            else
            {
                MessageBox.Show("删除失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)//获取第二张表所选取的值
        {
            //获得当前选中的行   
            int rowindex = e.RowIndex;

            //获得当前行的第0列的值   
            textBoxbookID.Text = dataGridView2.Rows[rowindex].Cells[0].Value.ToString().Trim();
            //获得当前行的第一列的值  
            textBoxtype.Text = dataGridView2.Rows[rowindex].Cells[5].Value.ToString().Trim();
            //获得当前行的第二列的值   
            textBoxedition.Text = dataGridView2.Rows[rowindex].Cells[3].Value.ToString().Trim();
            textBoxlocation.Text = dataGridView2.Rows[rowindex].Cells[1].Value.ToString().Trim();
            textBoxbookname.Text = dataGridView2.Rows[rowindex].Cells[2].Value.ToString().Trim();
            textBoxpublisherID.Text = dataGridView2.Rows[rowindex].Cells[8].Value.ToString().Trim();
            textBoxauthor.Text = dataGridView2.Rows[rowindex].Cells[7].Value.ToString().Trim();
            textBoxabstract.Text = dataGridView2.Rows[rowindex].Cells[6].Value.ToString().Trim();
            textBoxavailable.Text = dataGridView2.Rows[rowindex].Cells[4].Value.ToString().Trim();
            textBoxISBN.Text=dataGridView2.Rows[rowindex].Cells[11].Value.ToString().Trim();
        }

        private void button8_Click(object sender, EventArgs e)//管理员查询书籍
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string bookname = Convert.ToString(textBoxbookname.Text);
            string ID = Convert.ToString(textBoxbookID.Text);
            string publisherID = Convert.ToString(textBoxpublisherID.Text);
            string author = Convert.ToString(textBoxauthor.Text);
            string location = Convert.ToString(textBoxlocation.Text);
            string type = Convert.ToString(textBoxtype.Text);
            string ISBN = Convert.ToString(textBoxISBN.Text);

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM books WHERE ID  ='" + ID + "' or bookname = '" + bookname
                 + "' or publisherID = '" + publisherID + "' or author = '" + author + "'or location = '" + location + "'or type = '" + type + "'or ISBN = '" + ISBN + "';", connection);
            DataSet sourceDataSet = new DataSet();
            adapter.Fill(sourceDataSet);
            dataGridView2.DataSource = sourceDataSet.Tables[0];
            connection.Close();
        }

        private void button6_Click(object sender, EventArgs e)//重置填写内容
        {
            textBoxbookID.Text = "";
            textBoxtype.Text = "";
            textBoxedition.Text = "";
            textBoxlocation.Text = "";
            textBoxbookname.Text = "";
            textBoxpublisherID.Text = "";
            textBoxauthor.Text = "";
            textBoxavailable.Text = "";
            textBoxabstract.Text = "";
            textBoxISBN.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)//修改书籍信息
        {
            string ID = Convert.ToString(textBoxbookID.Text);
            string bookname = Convert.ToString(textBoxbookname.Text);
            string publisherID = Convert.ToString(textBoxpublisherID.Text);
            string author = Convert.ToString(textBoxauthor.Text);
            string location = Convert.ToString(textBoxlocation.Text);
            string type = Convert.ToString(textBoxtype.Text);
            string edition = Convert.ToString(textBoxedition.Text);
            string ab = Convert.ToString(textBoxabstract.Text);
            string available = Convert.ToString(textBoxavailable.Text);
            string ISBN = Convert.ToString(textBoxISBN.Text);
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();
            string sql2 = "update books set location='"+location+ "',bookname='" + bookname + "',publisherID='" + publisherID + "', author='" + author + "',type='" + type + "',edition='" + edition + "',abstract='" + ab + "',available='" + available + "',ISBN='" + ISBN + "' where ID='" + ID+"'";
            SqlCommand command2 = new SqlCommand(sql2, connection2);
            int count = command2.ExecuteNonQuery();
            connection2.Close();


            if (count > 0)
            {
                string mes = "修改成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT books.ID as '书籍编号',books.location as '所在分馆',books.bookname as '书籍名称',books.edition as '书籍版本',books.available as '在架上', books.type as '书籍类别',books.abstract as '书籍简介',books.author as '作者',publishers.ID as '出版社ID',publishers.publishername as '出版社',books.booking as '是否被预约',books.ISBN as 'ISBN' FROM books, publishers where publishers.ID = books.publisherID", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView2.DataSource = sourceDataSet1.Tables[0];
                conn.Close();

            }
            else
            {
                MessageBox.Show("修改失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }

        }

        private void button5_Click_1(object sender, EventArgs e)//删除书籍
        {
            string ID = textBoxbookID.Text;
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();
            string sql2 = "DELETE FROM books WHERE ID = '"+ID+"'; ";

            SqlCommand command2 = new SqlCommand(sql2, connection2);
            int count = command2.ExecuteNonQuery();
            

            connection2.Close();
            string mes = "";

            if (count > 0)
            {
                mes = "删除成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT books.ID as '书籍编号',books.location as '所在分馆',books.bookname as '书籍名称',books.edition as '书籍版本',books.available as '在架上', books.type as '书籍类别',books.abstract as '书籍简介',books.author as '作者',publishers.ID as '出版社ID',publishers.publishername as '出版社',books.booking as '是否被预约',books.ISBN as 'ISBN' FROM books, publishers where publishers.ID = books.publisherID", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView2.DataSource = sourceDataSet1.Tables[0];
                conn.Close();

            }
            else
            {
                MessageBox.Show("删除失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }

        }

        private void button23_Click(object sender, EventArgs e)//增加书籍
        {
            int k = 0;//用于记录是否缺少必要信息
            string tips1 = "请输入：";///生成提示

            if (textBoxbookname.Text == "")
            {
                tips1 += "书名";
                k = 1;
            }

            if (textBoxbookID.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "索书号";
                }
                else { tips1 += "、索书号"; }
                k = 1;
            }

            if (textBoxauthor.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "作者";
                }
                else { tips1 += "、作者"; }
                k = 1;
            }
            if (textBoxtype.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "类型";
                }
                else { tips1 += "、类型"; }
                k = 1;
            }

            if (textBoxpublisherID.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "出版社";
                }
                else { tips1 += "、出版社"; }
                k = 1;
            }
            

            if (textBoxlocation.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "位置";
                }
                else { tips1 += "、位置"; }
                k = 1;
            }


            if (textBoxedition.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "版本";
                }
                else { tips1 += "、版本"; }
                k = 1;
            }

            if (textBoxISBN.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "ISBN";
                }
                else { tips1 += "、ISBN"; }
                k = 1;
            }
            
            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            int k2 = 0;
            if (textBoxavailable.Text!="" )
            {
                MessageBox.Show("请勿输入“在架上”行", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBoxavailable.Text = "";
                k2 = 1;
            }
            string ID = Convert.ToString(textBoxbookID.Text);
            string bookname = Convert.ToString(textBoxbookname.Text);
            string publisherID = Convert.ToString(textBoxpublisherID.Text);
            string author = Convert.ToString(textBoxauthor.Text);
            string location = Convert.ToString(textBoxlocation.Text);
            string type = Convert.ToString(textBoxtype.Text);
            string edition = Convert.ToString(textBoxedition.Text);
            string ab = Convert.ToString(textBoxabstract.Text);
            string available ="是";//新书上架默认在架上;
            string ISBN = Convert.ToString(textBoxISBN.Text);
            string booking = "否";//新书上架默认未被预约

            int k3 = 0;
            if(textBoxlocation.Text.TrimEnd()!="东川路图书馆"&& textBoxlocation.Text.TrimEnd() != "江川路图书馆")
            {
                MessageBox.Show("位置输入错误，应为“东川路图书馆”或“江川路图书馆”", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBoxlocation.Text = "";
                k3 = 1;
            }
            

            if (k == 0 && k2 == 0&&k3==0)
            { ///如果填写完整且正确,且未填写available，且地址选择未超出范围
                SqlConnection connection2 = new SqlConnection(constr);
                connection2.Open();
                string sql3 = @"INSERT INTO library.dbo.books(ID,location,bookname,edition,available,type,abstract,author,publisherID,booking,ISBN)" + "VALUES ('" + ID + "','" + location + "','" + bookname + "','" + edition + "','" + available + "','" + type + "','" + ab + "','" + author + "','" + publisherID + "','" + booking + "','" + ISBN + "')";
                try
                {
                    SqlCommand command3 = new SqlCommand(sql3, connection2);
                    int count = command3.ExecuteNonQuery();
                    if (count > 0)
                    {
                        MessageBox.Show( "添加成功!", "提示", MessageBoxButtons.OKCancel);
                        SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT books.ID as '书籍编号',books.location as '所在分馆',books.bookname as '书籍名称',books.edition as '书籍版本',books.available as '在架上', books.type as '书籍类别',books.abstract as '书籍简介',books.author as '作者',publishers.ID as '出版社ID',publishers.publishername as '出版社',books.booking as '是否被预约',books.ISBN as 'ISBN' FROM books, publishers where publishers.ID = books.publisherID", connection2);
                        DataSet sourceDataSet1 = new DataSet();
                        adapter1.Fill(sourceDataSet1);
                        dataGridView2.DataSource = sourceDataSet1.Tables[0];///刷新表格数据
                    }
                    else
                    {
                        MessageBox.Show("添加失败，请重试！", "提示", MessageBoxButtons.OKCancel);
                    }
                }
                catch
                {
                    MessageBox.Show("出版社ID不存在，请重新输入！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                }
                connection2.Close();
            }
        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();//////登陆窗体隐藏
            Form1 main = new Form1();//////初始化主窗体
            main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到注册页面
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //获得当前选中的行   
            int rowindex = e.RowIndex;
            //获得当前行的第0列的值，purchaseID   
            textBox11.Text = dataGridView4.Rows[rowindex].Cells[0].Value.ToString().Trim();
            //获得当前行的第一列的值，date
            textBox12.Text = dataGridView4.Rows[rowindex].Cells[1].Value.ToString().Trim();
            //获得当前行的第二列的值，price
            textBox13.Text = dataGridView4.Rows[rowindex].Cells[4].Value.ToString().Trim();
            //获得当前行的第三列的值，bookID
            textBox14.Text = dataGridView4.Rows[rowindex].Cells[2].Value.ToString().Trim();
            //获得当前行的第四列的值，libraryID
            textBox15.Text = dataGridView4.Rows[rowindex].Cells[5].Value.ToString().Trim();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGridView5.DataSource;
            dt.Rows.Clear();
            dataGridView5.DataSource = dt;
            //查看进行中的预定信息
            //InitializeComponent();
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlDataAdapter adapter5 = new SqlDataAdapter("SELECT booking.bookingID as '预约编号',booking.userID as '读者编号',users.username as '读者名字',books.bookname as '书籍名称', booking.bookID as '书籍编号', booking.date as '预约时间',booking.finished as '是否已完成' FROM booking, users, books where booking.bookID = books.ID and users.ID = booking.userID and booking.finished = '否'", conn);
            DataSet sourceDataSet5 = new DataSet();
            adapter5.Fill(sourceDataSet5);
            dataGridView5.DataSource = sourceDataSet5.Tables[0];
            dataGridView5.ForeColor = Color.Black;
            
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)///文献管理中的查询
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();

            string ID = Convert.ToString(textBox9.Text);//文献编号ID
            string name = Convert.ToString(textBox7.Text);//文献名称
            string keyword = Convert.ToString(textBox8.Text);//文献关键词
            string author = Convert.ToString(textBox6.Text);//文献作者
            string content = Convert.ToString(textBox5.Text);//文献相对位置
            string cited = Convert.ToString(textBox4.Text);//文献被引量///不按照被引量查询
            string year = Convert.ToString(textBox3.Text);//文献发表年份
            string journal = Convert.ToString(textBox2.Text);//文献所属期刊
            string DOI = Convert.ToString(textBox1.Text);//文献的DOI码
            string Abstract = Convert.ToString(textBox10.Text);//文献的简介

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM literatures WHERE literatureID  ='" + ID + "' or name = '" + name
                 + "' or keyword = '" + keyword + "' or author = '" + author + "'or content = '" + content + "'or year = '" + year + "'or journal = '" + journal + "'or DOI = '" + DOI + "';", connection);
            DataSet sourceDataSet = new DataSet();
            adapter.Fill(sourceDataSet);
            dataGridView6.DataSource = sourceDataSet.Tables[0];
            connection.Close();
        }

        private void textBoxbookname_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //获得当前选中的行   
            int rowindex = e.RowIndex;

            //获得当前行的第0列的值，ID   
            textBox9.Text = dataGridView6.Rows[rowindex].Cells[0].Value.ToString().Trim();
            //获得当前行的第一列的值，name
            textBox7.Text = dataGridView6.Rows[rowindex].Cells[1].Value.ToString().Trim();
            //获得当前行的第二列的值，keyword
            textBox8.Text = dataGridView6.Rows[rowindex].Cells[2].Value.ToString().Trim();
            //获得当前行的第三列的值，abstract
            textBox10.Text = dataGridView6.Rows[rowindex].Cells[3].Value.ToString().Trim();
            //获得当前行的第四列的值，author
            textBox6.Text = dataGridView6.Rows[rowindex].Cells[4].Value.ToString().Trim();
            //获取当前行的第五列的值，content相对位置
            textBox5.Text = dataGridView6.Rows[rowindex].Cells[5].Value.ToString().Trim();
            //获取当前行的第六列的值，cited被引量
            textBox4.Text = dataGridView6.Rows[rowindex].Cells[6].Value.ToString().Trim();
            //获取当前行的第七列的值，year发表年份
            textBox3.Text = dataGridView6.Rows[rowindex].Cells[7].Value.ToString().Trim();
            //获取当前行的第八列的值，journal所属期刊
            textBox2.Text = dataGridView6.Rows[rowindex].Cells[8].Value.ToString().Trim();
            //获取当前行的第九列的值，DOI
            textBox1.Text = dataGridView6.Rows[rowindex].Cells[9].Value.ToString().Trim();
        }

        private void button13_Click(object sender, EventArgs e)//重置文献管理输入框
        {
            textBox9.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox3.Text = "";
            textBox2.Text = "";
            textBox1.Text = "";
            textBox10.Text = "";
        }

        private void button12_Click(object sender, EventArgs e)//删除文献
        {
            string ID = textBox9.Text;
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();
            string sql2 = "DELETE FROM literatures WHERE literatureID = '" + ID + "'; ";

            SqlCommand command2 = new SqlCommand(sql2, connection2);
            int count = command2.ExecuteNonQuery();
            connection2.Close();
            string mes = "";

            if (count > 0)
            {
                mes = "删除成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM literatures", conn);//以下四行用于刷新表格数据
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView6.DataSource = sourceDataSet1.Tables[0];
                conn.Close();//断开与数据库的连接
            }
            else
            {
                MessageBox.Show("删除失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }
        }

        private void button14_Click(object sender, EventArgs e)//修改文献信息
        {
            string ID = Convert.ToString(textBox9.Text);//文献编号ID
            string name = Convert.ToString(textBox7.Text);//文献名称
            string keyword = Convert.ToString(textBox8.Text);//文献关键词
            string author = Convert.ToString(textBox6.Text);//文献作者
            string content = Convert.ToString(textBox5.Text);//文献相对位置
            string cited = Convert.ToString(textBox4.Text);//文献被引量///不按照被引量查询
            string year = Convert.ToString(textBox3.Text);//文献发表年份
            string journal = Convert.ToString(textBox2.Text);//文献所属期刊
            string DOI = Convert.ToString(textBox1.Text);//文献的DOI码
            string Abstract = Convert.ToString (textBox10.Text);//文献的简介
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();
            string sql2 = "update literatures set DOI='" + DOI + "',name='" + name + "',keyword='" + keyword + "', author='" + author + "',content='" + content + "',cited='" + cited + "',year='" + year + "',journal='" + journal + "',abstract='" + Abstract + "' where literatureID='" + ID + "'";
            SqlCommand command2 = new SqlCommand(sql2, connection2);
            int count = command2.ExecuteNonQuery();
            connection2.Close();

            string mes = "请记住您的账号:";

            if (count > 0)
            {
                mes = "修改成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM books", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView6.DataSource = sourceDataSet1.Tables[0];
                conn.Close();

            }
            else
            {
                MessageBox.Show("修改失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }
        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            int k = 0;
            string tips1 = "请输入：";///生成提示

            if (textBox7.Text == "")
            {
                tips1 += "文献名";
            }

            if (textBox8.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "关键词";
                }
                else { tips1 += "、关键词"; }
                k = 1;
            }

            if (textBox10.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "摘要";
                }
                else { tips1 += "、摘要"; }
                k = 1;
            }

            if (textBox6.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "作者";
                }
                else { tips1 += "、作者"; }
                k = 1;
            }

            if (textBox3.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "年份";
                }
                else { tips1 += "、年份"; }
                k = 1;
            }

            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            
            Regex rx = new Regex("^[0-9]*$");
            int k2 = 0;
            if (rx.IsMatch(textBox3.Text.TrimEnd())) { }//判断年份是否为数字
            else
            {
                MessageBox.Show("年份必须为数字，请重新输入", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBox3.Text = null;
            }

            int k3 = 0;
            if(textBox9.Text!="")
            {
                MessageBox.Show("请勿输入文献ID，系统会自动生成", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            string name = Convert.ToString(textBox7.Text);//文献名称
            string keyword = Convert.ToString(textBox8.Text);//文献关键词
            string author = Convert.ToString(textBox6.Text);//文献作者
            string content = Convert.ToString(textBox5.Text);//文献相对位置
            string cited = Convert.ToString(textBox4.Text);//文献被引量///不按照被引量查询
            string year = Convert.ToString(textBox3.Text);//文献发表年份
            string journal = Convert.ToString(textBox2.Text);//文献所属期刊
            string DOI = Convert.ToString(textBox1.Text);//文献的DOI码
            string Abstract = Convert.ToString(textBox10.Text);//文献的简介

            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string sql1 = "select top 1 literatureID from library.dbo.literatures order by literatureID desc";
            SqlCommand command1 = new SqlCommand(sql1, connection);
            string lastID = Convert.ToString(command1.ExecuteScalar());///把最后一次输入的ID存入lastID

            connection.Close();
            if (k == 0 && k2 == 0&&k3==0)
            { ///如果填写完整且正确，且未输入ID
                int sqlList0 = Convert.ToInt32(lastID) + 1;
                string ID = sqlList0.ToString("0000");///生成新账号,要求是4位数
                textBox9.Text = ID;
                SqlConnection connection2 = new SqlConnection(constr);
                connection2.Open();
                string sql2 = @"insert into library.dbo.literatures(literatureID,name,keyword,abstract,author,content,cited,year,journal,DOI)" + "VALUES ('" + ID + "','" + name + "','" + keyword + "','" + Abstract + "','" + author + "','" + content + "','" + cited + "','" + year + "','" + journal + "','" + DOI + "')";
                SqlCommand command2 = new SqlCommand(sql2, connection2);
                int count = command2.ExecuteNonQuery();
                connection2.Close();

                string mes = "该文献的编号是：";

                if (count > 0)
                {
                    mes = "添加文献成功!" + mes + ID;
                    MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                    SqlConnection conn = new SqlConnection(constr);
                    conn.Open();
                    SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM library.dbo.literatures", conn);
                    DataSet sourceDataSet1 = new DataSet();
                    adapter1.Fill(sourceDataSet1);
                    dataGridView6.DataSource = sourceDataSet1.Tables[0];
                    conn.Close();
                }
                else
                {
                    MessageBox.Show("添加文献失败，请重试", "提示", MessageBoxButtons.OKCancel);
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGridView5.DataSource;
            dt.Rows.Clear();
            dataGridView5.DataSource = dt;
            //查看进行中的预定信息
            //InitializeComponent();
            SqlConnection conn = new SqlConnection(constr);
            conn.Open();
            SqlDataAdapter adapter5 = new SqlDataAdapter("SELECT booking.bookingID as '预约编号',booking.userID as '读者编号',users.username as '读者名字',books.bookname as '书籍名称', booking.bookID as '书籍编号', booking.date as '预约时间',booking.finished as '是否已完成' FROM booking, users, books where booking.bookID = books.ID and users.ID = booking.userID", conn);
            DataSet sourceDataSet5 = new DataSet();
            adapter5.Fill(sourceDataSet5);
            dataGridView5.DataSource = sourceDataSet5.Tables[0];
            dataGridView5.ForeColor = Color.Black;
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)//查询购书记录
        {
            SqlConnection connection = new SqlConnection(constr);
            connection.Open();

            string purchaseID = Convert.ToString(textBox11.Text);//购书记录编号ID
            string date = Convert.ToString(textBox12.Text);//购书日期
            string price = Convert.ToString(textBox13.Text);//购书单价
            string bookID = Convert.ToString(textBox14.Text);//书籍ID
            string libraryID = Convert.ToString(textBox15.Text);//图书馆ID

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM purchase WHERE purchaseID  ='" + purchaseID + "' or date = '" + date
                 + "' or price = '" + price + "' or bookID = '" + bookID + "'or libraryID = '" + libraryID + "';", connection);
            DataSet sourceDataSet = new DataSet();
            adapter.Fill(sourceDataSet);
            dataGridView4.DataSource = sourceDataSet.Tables[0];
            connection.Close();
        }

        private void button19_Click(object sender, EventArgs e)//增加购书记录
        {
            int k = 0;//记录有无空缺（除了pruchaseID）
            string tips1 = "请输入：";///生成提示

            if (textBox13.Text == "")
            {
                if (k == 0)
                tips1 += "单价";
                k = 1;
            }

            if (textBox14.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "书籍编号ID";
                }
                else { tips1 += "、书籍编号ID"; }
                k = 1;
            }
            if (textBox15.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "图书馆编号ID";
                }
                else { tips1 += "、图书馆编号ID"; }
                k = 1;
            }
            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            int k3 = 0;//记录是否输入了购买记录编号ID与日期，输入了则报错
            if (textBox11.Text != "")
            {
                MessageBox.Show("请勿输入购买记录编号ID","提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBox11.Text = "";
                k3 = 1;
            }
            if (textBox12.Text != "")
            {
                MessageBox.Show("请勿输入购买日期，系统会自动录入", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBox12.Text = "";
                k3 = 1;
            }

            int k2 = 0;//记录输入格式是否正确
            string tips2 = null;
            Regex rx = new Regex("^[0-9]*$");
            if (rx.IsMatch(textBox13.Text.TrimEnd())) { }//判断价格是否为数字
            else
            {
                k2 = 1;
                tips2 += "价格";
                textBox13.Text = null;
            }
            if (rx.IsMatch(textBox15.Text.TrimEnd())) { }//判断图书馆编号ID是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "书籍编号ID";
                }
                else { tips2 += "、书籍编号ID"; }
                textBox15.Text = null;
            }
            
            if (k2 == 1)
            {
                tips2 += "必须为数字，请重新输入";
                MessageBox.Show(tips2, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            ///string purchaseID = Convert.ToString(textBox11.Text);//购书记录编号ID
            ///string date = Convert.ToString(textBox12.Text);//购书日期，
            string price = Convert.ToString(textBox13.Text);//购书单价
            string bookID = Convert.ToString(textBox14.Text);//书籍ID
            string libraryID = Convert.ToString(textBox15.Text);//图书馆ID

            int k4 = 0;
            if (libraryID == "01" || libraryID == "02") { }
            else
            {
                MessageBox.Show("图书馆ID仅有01和02，请重新输入", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBox15.Text = "";
                k4 = 1;
            }

            string year, month, day;///以操作时刻为购书日期
            year = DateTime.Now.Year.ToString("0000"); ///获取年份,"0000"代表至少四位数   // 2018
            month = DateTime.Now.Month.ToString("00"); ///获取月份,"00"代表至少两位数   // 12
            day = DateTime.Now.Day.ToString("00"); ///获取该月中的第几天，"00"代表至少两位数   // 03
            string date = year + month + day;

            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string sql1 = "select top 1 purchaseID from library.dbo.purchase order by date desc";//取出最近一次购买记录ID
            SqlCommand command1 = new SqlCommand(sql1, connection);
            string lastID = Convert.ToString(command1.ExecuteScalar());///把最后一次输入的ID存入lastID

            string sqlYear = lastID.Substring(1, 4);///获取最后一次操作的年份//2018
            string sqlMonth = lastID.Substring(5, 2);///获取最后一次操作的月份//06
            string sqlDay = lastID.Substring(7, 2);///获取最后一次操作的日期//08
            connection.Close();
            if (k == 0 && k2 == 0 && k3 == 0&&k4==0)
            { ///如果填写完整且正确,且未填写购买记录ID
                string ID;///生成新账号
                if (year!= sqlYear || month != sqlMonth || day != sqlDay)
                {
                    ID = libraryID.Substring(1,1) + year + month + day + 1.ToString("000");///馆编号+年+月+日期+‘001‘
                }
                else
                {
                    int sqlList0 = Convert.ToInt32(lastID.Substring(9, 3)) + 1;
                    string sqlList = sqlList0.ToString("000");
                    
                    ID = libraryID.Substring(1, 1) + year + month + day + sqlList;////馆编号+年+月+日期+当天购书序号
                }

                SqlConnection connection2 = new SqlConnection(constr);
                connection2.Open();
                string sql3 = @"INSERT INTO library.dbo.purchase(purchaseID,date,price,bookID,libraryID)" + "VALUES ('" + ID + "','" + date + "','" + price + "','" + bookID + "','" + libraryID + "')";
                ///string sql4 = string.Format("where exists ( select * from books where ID = '{0}') and exists(select * from libraries where libraryID ='{1}') ", bookID, libraryID);
                try{
                    SqlCommand command3 = new SqlCommand(sql3, connection2);
                    int count = command3.ExecuteNonQuery();
                    if(count>0)
                        {
                            string mes = "该购买记录编号是:";
                            mes = "添加成功!" + mes + ID;
                            MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                            textBox11.Text = ID;//显示ID
                            textBox12.Text = DateTime.Now.ToString();//显示购书日期时刻
                            SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM purchase", connection2);
                            DataSet sourceDataSet1 = new DataSet();
                            adapter1.Fill(sourceDataSet1);
                            dataGridView4.DataSource = sourceDataSet1.Tables[0];///刷新表格数据
                    }
                    else
                    {
                        MessageBox.Show("添加失败，请重试！", "提示", MessageBoxButtons.OKCancel);
                    }}
                catch
                {
                    MessageBox.Show("书籍编号ID或图书馆编号ID不存在，请重新输入！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                }
                connection2.Close();
            }
        }

        private void button20_Click(object sender, EventArgs e)///重置购买界面
        {
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
            textBox15.Text = "";
        }

        private void button21_Click(object sender, EventArgs e)//删除购买记录
        {
            string purchaseID = textBox11.Text.TrimEnd();
            SqlConnection connection2 = new SqlConnection(constr);
            connection2.Open();
            string sql2 = "DELETE FROM purchase  WHERE purchaseID = '" + purchaseID + "';";
            SqlCommand command2 = new SqlCommand(sql2, connection2);
            int count = command2.ExecuteNonQuery();
            connection2.Close();
            string mes = "";

            if (count > 0)//如果影响行数大于0，则删除成功
            {
                mes = "删除成功!";
                MessageBox.Show(mes, "提示", MessageBoxButtons.OKCancel);
                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM purchase", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView4.DataSource = sourceDataSet1.Tables[0];
                conn.Close();
            }
            else
            {
                MessageBox.Show("删除失败，请重试", "提示", MessageBoxButtons.OKCancel);
            }
        }

        private void button22_Click(object sender, EventArgs e)//修改购买记录，日期为系统自动生成，不可修改
        {
            int k = 0;
            string tips1 = "请输入：";///生成提示

            if (textBox11.Text == "")
            {
                tips1 += "购买记录编号ID";
                k = 1;
            }

            if (textBox13.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "单价";
                }
                else { tips1 += "、单价"; }
                k = 1;
            }

            if (textBox14.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "书籍编号ID";
                }
                else { tips1 += "、书籍编号ID"; }
                k = 1;
            }
            if (textBox15.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "图书馆编号ID";
                }
                else { tips1 += "、图书馆编号ID"; }
                k = 1;
            }
            if (k == 1)///若存在空缺，则统一输出提示
            {
                MessageBox.Show(tips1, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            int k2 = 0;
            string tips2 = null;
            Regex rx = new Regex("^[0-9]*$");
            if (rx.IsMatch(textBox11.Text.TrimEnd())) { }//判断购买记录编号ID是否为数字
            else
            {
                k2 = 1;
                tips2 += "购买记录编号ID";
                textBox11.Text = null;
            }
            if (rx.IsMatch(textBox13.Text.TrimEnd())) { }//判断单价是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "单价";
                }
                else { tips2 += "、单价"; }
                textBox13.Text = null;
            }
            if (rx.IsMatch(textBox15.Text.TrimEnd())) { }//判断图书馆编号ID是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "图书馆编号ID";
                }
                else { tips2 += "、图书馆编号ID"; }
                textBox15.Text = null;
            }
            if (k2 == 1)
            {
                tips2 += "必须为数字，请重新输入";
                MessageBox.Show(tips2, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            int k3 = 0;
            if (textBox12.Text != "")
            {
                MessageBox.Show("请勿输入购买日期，购买日期不可修改", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                textBox12.Text = "";
                k3 = 1;
            }

            string purchaseID = Convert.ToString(textBox11.Text.TrimEnd());//购书记录编号ID
            ///string date = Convert.ToString(textBox12.Text.TrimEnd());//购书日期，
            string price = Convert.ToString(textBox13.Text.TrimEnd());//购书单价
            string bookID = Convert.ToString(textBox14.Text.TrimEnd());//书籍ID
            string libraryID = Convert.ToString(textBox15.Text.TrimEnd());//图书馆ID

            SqlConnection connection2 = new SqlConnection(constr);
            ///connection2.Open();
            //检验bookID和libraryID是否已经存在，以便输出提示（所以购书流程需要先录入书籍信息，再录入购买信息）
            ///string sql2 = string.Format("if exists ( select * from books where ID = {0}) and exists(select * from libraries where libraryID ={1})  print 1 else print 0)", bookID, libraryID);
            ///SqlCommand command2 = new SqlCommand(sql2, connection2);
            ///string num = Convert.ToString(command2);///把sql的输出，存入num。但是没实现
            ///connection2.Close();

            if (k==0&&k2==0&&k3==0)///如果输入正确，则修改数据库
            {
                SqlConnection connection3 = new SqlConnection(constr);
                connection3.Open();
                string sql3 = "update purchase set price='" + price + "', bookID='" + bookID + "',libraryID='" + libraryID + "' where purchaseID='" + purchaseID + "'";
                string sql4 = string.Format("and exists ( select * from books where ID = '{0}') and exists(select * from libraries where libraryID ='{1}') ", bookID, libraryID);
                SqlCommand command3 = new SqlCommand(sql3+sql4, connection3);
                int count = command3.ExecuteNonQuery();///影响行数
                connection3.Close();

                if (count > 0)//影响行数大于0
                {
                    MessageBox.Show("修改成功！", "提示", MessageBoxButtons.OKCancel);
                }
                else
                {
                    MessageBox.Show("修改失败，请重试！", "提示", MessageBoxButtons.OKCancel);
                }

                SqlConnection conn = new SqlConnection(constr);
                conn.Open();
                SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT * FROM purchase", conn);
                DataSet sourceDataSet1 = new DataSet();
                adapter1.Fill(sourceDataSet1);
                dataGridView4.DataSource = sourceDataSet1.Tables[0];///刷新表格数据
                conn.Close();
            }
            else
            {
                ///MessageBox.Show("数据库中不存在该书籍ID或图书馆ID（注意图书馆ID为两位数），请重新输入", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
        }

        private void Form4_Load_1(object sender, EventArgs e)
        {

        }
    }
 }

