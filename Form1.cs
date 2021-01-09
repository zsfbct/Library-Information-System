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
    public partial class Form1 : Form
    {
        readonly string constr = Class_ConStr.constr;
        string str1;
        string str2;
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {if (radioButton2.Checked)///如果“读者”被选中
            {
                if (textBox1.Text == "")////判断textbox1是否为空
                {
                    MessageBox.Show("请填写账号", "提示");/////messagebox为弹出框控件
                    textBox1.Focus();///////光标落在textbox1里面
                }
                else if (textBox2.Text == "")////判断textbox2是否为空
                {
                    MessageBox.Show("请填写密码", "提示");
                    textBox2.Focus();
                }
                else
                {
                    str1 = textBox1.Text.ToString();///////因为上面已经定了str1,所以这里不需要再定义，可以直接赋值；不然要写成string str1=.....形式
                    str2 = textBox2.Text.ToString();
                    SqlConnection conn = new SqlConnection(constr);//////初始化一个新的sql数据库连接conn，constr为数据库连接字符串，上面已定义
                    conn.Open();//////用conn打开数据库连接
                    string sql = string.Format("select * from users where ID ='{0}'and userpwd='{1}'", str1, str2); //////定义要执行的数据库操作，注意：因为我的电脑里面database1数据库里面有users数据表，所以，这段代码不会报错，如果在你们的电脑上运行的话，则要么改成你的数据表名，要么新建一个users数据表。我的users数据表里面，有ID， username 以及userpwd三个字段。                                                                                                              
                    SqlCommand comtext = new SqlCommand(sql, conn);///////初始化数据库操作，SqlCommand(sql, conn)中sql为要执行的数据库操作代码，conn为数据库连接
                    SqlDataReader dr;//////定义读取数据的对象
                    dr = comtext.ExecuteReader();///////给数据读取对象初始化
                    dr.Read();///////开始读取
                    if (dr.HasRows)///////如果dr读取到了数据，即：数据库的执行语句里面有返回值，说明账号与密码匹配
                    {
                        UserInfo.user_id = Convert.ToInt32(str1);
                        this.Hide();//////登陆窗体隐藏
                        Form2 main = new Form2(str1);//////初始化主窗体
                        main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到主窗体
                    }
                    else///////如果没有读取到数据，即：数据库的执行语句里面无返回值，说明账号与密码不匹配
                    {
                        MessageBox.Show("登录失败,账户或者密码错误！", "提示");///////提示登陆失败
                        textBox1.Text = "";/////将textbox内的取值清空
                        textBox2.Text = "";
                    }
                    conn.Close();///断开数据库连接
                }
            }
            else if(radioButton1.Checked) ///如果“管理员”被选中
            {
                if (textBox1.Text == "")////判断textbox1是否为空
                {
                    MessageBox.Show("请填写账号", "提示");/////messagebox为弹出框控件
                    textBox1.Focus();///////光标落在textbox1里面
                }
                else if (textBox2.Text == "")////判断textbox2是否为空
                {
                    MessageBox.Show("请填写密码", "提示");
                    textBox2.Focus();
                }
                else
                {
                    str1 = textBox1.Text.ToString();///////因为上面已经定了str1,所以这里不需要再定义，可以直接赋值；不然要写成string str1=.....形式
                    str2 = textBox2.Text.ToString();
                    SqlConnection conn = new SqlConnection(constr);//////初始化一个新的sql数据库连接conn，constr为数据库连接字符串，上面已定义
                    conn.Open();//////用conn打开数据库连接
                    string sql = string.Format("select * from managers where ID ='{0}'and managerpwd='{1}'", str1, str2); //////定义要执行的数据库操作，注意：因为我的电脑里面database1数据库里面有users数据表，所以，这段代码不会报错，如果在你们的电脑上运行的话，则要么改成你的数据表名，要么新建一个users数据表。我的users数据表里面，有ID， username 以及userpwd三个字段。                                                                                                              
                    SqlCommand comtext = new SqlCommand(sql, conn);///////初始化数据库操作，SqlCommand(sql, conn)中sql为要执行的数据库操作代码，conn为数据库连接
                    SqlDataReader dr;//////定义读取数据的对象
                    dr = comtext.ExecuteReader();///////给数据读取对象初始化
                    dr.Read();///////开始读取
                    if (dr.HasRows)///////如果dr读取到了数据，即：数据库的执行语句里面有返回值，说明账号与密码匹配
                    {
                        this.Hide();//////登陆窗体隐藏
                        Form4 main = new Form4(str1);//////初始化主窗体
                        main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到主窗体
                    }
                    else///////如果没有读取到数据，即：数据库的执行语句里面无返回值，说明账号与密码不匹配
                    {
                        MessageBox.Show("登录失败,账户或者密码错误！", "提示");///////提示登陆失败
                        textBox1.Text = "";/////将textbox内的取值清空
                        textBox2.Text = "";
                    }
                    conn.Close();///断开数据库连接
                }
            }
            else///如果radio button没有被选择
            {
                MessageBox.Show("请选择登录类型“管理员”或“读者”", "提示");///////提示登陆失败
            }
         }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();//////登陆窗体隐藏
            Form3 main = new Form3();//////初始化主窗体
            main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到注册页面
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
