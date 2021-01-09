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
using System.Security.Cryptography.X509Certificates;

namespace login
{
   
    public partial class Form3 : Form
    {
        readonly string constr = Class_ConStr.constr;

        public string ID { get; private set; }

        
        public Form3()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
           
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0 ;
            string tips1="请输入：";///生成提示
            if (textBox2.Text!= textBox3.Text)
            {
                MessageBox.Show("两次密码不相同，请重新输入", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            if (textBox2.Text == "")
            {
                tips1 += "密码";
                k = 1;
            }

            if (textBox3.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "确认密码";
                }
                else { tips1 += "、确认密码"; }
                k = 1;
            }

            if (textBox4.Text == "")
            {
                if (k == 0)
                {
                    tips1 += "姓名";
                }
                else { tips1 += "、姓名"; }
                k = 1;
            }

            

            if (textBox5.Text == "")
            {
                if (k==0)
                {
                    tips1 += "年龄";
                }
                else { tips1 += "、年龄"; }
                k = 1;
            }

            if (textBox6.Text == "")
            {
                if (k==0)
                {
                    tips1 += "身份证号";
                }
                else { tips1 += "、身份证号"; }
                k = 1;
            }
            if (textBox7.Text == "")
            {
                if (k==0)
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

            int num=0;///表示注册的馆的序号
            if (radioButton1.Checked)
            {
                num = 1;
            }
            else if (radioButton2.Checked)
            {
                num = 2;
            }
            else
            {
                MessageBox.Show("请选择注册分馆", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            int k2 = 0;
            string tips2 = null;
            Regex rx = new Regex("^[0-9]*$");
            if (rx.IsMatch(textBox5.Text)) { }//判断年龄是否为数字
            else
            {
                k2 = 1;
                tips2 += "年龄";
                textBox5.Text = null;
            }
            if (rx.IsMatch(textBox6.Text)) { }//判断身份证号是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "身份证号";
                }
                else { tips2 += "、身份证号"; }
                textBox6.Text = null;
            }
            if (rx.IsMatch(textBox7.Text)) { }//判断电话号码是否为数字
            else
            {
                k2 = 1;
                if (tips2 == null)
                {
                    tips2 += "电话号码";
                }
                else { tips2 += "、电话号码"; }
                textBox7.Text = null;
            }
            if (k2 == 1)
            {
                tips2 += "必须为数字，请重新输入";
                MessageBox.Show(tips2, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }

            string year, month, day;
            year = DateTime.Now.Year.ToString(); ///获取年份   // 2018
            month = DateTime.Now.Month.ToString("00"); ///获取月份,"00"代表至少两位数   // 12
            day = DateTime.Now.Day.ToString("00"); ///获取该月中的第几天，"00"代表至少两位数   // 03

            string Userpwd = Convert.ToString(textBox3.Text);///把“密码”转换为字符串
            string Username = Convert.ToString(textBox4.Text);///把“姓名”转换为字符串
            string Age = Convert.ToString(textBox5.Text);///把“年龄”转换为字符串            
            string Idcardnumber = Convert.ToString(textBox6.Text);///把“身份证号”转换为字符串
            string Phonenumber = Convert.ToString(textBox7.Text);///把“电话号码”转换为字符串

            SqlConnection connection = new SqlConnection(constr);
            connection.Open();
            string sql1 = "select top 1 ID from library.dbo.users order by date desc";
            SqlCommand command1 = new SqlCommand(sql1, connection);
            string lastID= Convert.ToString(command1.ExecuteScalar());///把最后一次输入的ID存入lastID
            
            string sqlYear = lastID.Substring(1,2);///获取最后一次操作的年份//20
            string sqlMonth = lastID.Substring(3, 2);///获取最后一次操作的月份//06
            string sqlDay = lastID.Substring(5, 2);///获取最后一次操作的日期//08
            connection.Close();
            if (k == 0 && k2 == 0 && num != 0) { ///如果填写完整且正确
                string ID;///生成新账号
                if (year.Substring(2,2) != sqlYear || month != sqlMonth || day != sqlDay)
                {
                    ID = num.ToString() + year.Substring(2, 2) + month + day + 1.ToString("00");///注册馆编号+年的后两位+月+日期+‘01‘
                }
                else
                {
                    int sqlList0 = Convert.ToInt32(lastID.Substring(7, 2)) + 1;
                    string sqlList = sqlList0.ToString("00");
                    ID = num.ToString() + year.Substring(2, 2) + month + day + sqlList;
                    }
                
                textBox1.Text = ID;
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
                    MessageBox.Show(mes, "提示",MessageBoxButtons.OKCancel);
                }
                else
                {
                    MessageBox.Show("注册失败，请重试", "提示", MessageBoxButtons.OKCancel);
                }
            }


            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();//////登陆窗体隐藏
            Form1 main = new Form1();//////初始化主窗体
            main.Show();//////主窗体显示，效果上，窗体从当前的登陆窗体，跳转到注册页面
        }
    }
}
