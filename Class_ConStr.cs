using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace login
{
    class Class_ConStr
    {
        public static string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=library;Data Source=LAPTOP-2ACORCGV";///server后填入服务器名称，适用于有账户密码的情况，如果无账户，则用下面这种方式。
                                                                                                                  ///string constr = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=library;Data Source=jeremy";
        /////数据库的连接字符串，如果要运行的话，需要改成自己数据库的连接字符。一般情况下，将“library”改成你数据库系统里面的数据库名称，CHAI-PC改成你的电脑名称就可以。

    }
}
