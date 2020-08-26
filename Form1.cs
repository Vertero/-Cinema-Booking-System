using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using PublicLib;
using Model;
using System.Configuration;

namespace hh
{
    public partial class Form1 : Form
    {
        public static Customer c = new Customer();
        public Form1()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Login(object sender, EventArgs e)
        {
            string ID = this.textBox1.Text;
            string password = this.textBox2.Text;
            if (ID.Equals("") || password.Equals(""))
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            else
            {
                string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                SqlConnection conn = new SqlConnection(ConStr);
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        string sql = "CheckCustomerLogin";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = sql;
                        SqlParameter par1 = new SqlParameter("@username", textBox1.Text);
                        SqlParameter par2 = new SqlParameter("@password", textBox2.Text);
                        SqlParameter par3 = new SqlParameter("@result", SqlDbType.Int);
                        par3.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(par1); //将变量添加到cmd命令对象中传给参数
                        cmd.Parameters.Add(par2);
                        cmd.Parameters.Add(par3);
                        cmd.ExecuteNonQuery();
                        int b = Convert.ToInt32(par3.Value);//得到第三个参数返回来的值
                        if (b==1)
                        {
                            MessageBox.Show("登录成功！", "提示");
                            c.ID = ID;
                            Form2 f = new Form2();
                            f.Show();
                            this.Hide();
                        }
                        else if (b == -1)
                        {
                            MessageBox.Show("用户名或密码输入错误！请重新输入。", "提示");
                            textBox2.Text = null;
                            textBox2.Focus();
                        }
                        else
                        {
                            MessageBox.Show("您的账号已在另外一个设备登录！！", "提示");
                            textBox2.Text = null;
                            textBox2.Focus();
                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + "打开数据库失败");
                }
            }
        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Customer.CancelLogin(c.ID);
                Dispose();
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void PassWordProtection(object sender, EventArgs e)
        {
            this.textBox2.UseSystemPasswordChar = true;
        }

        private void Regist(object sender, EventArgs e)
        {
            Form3 ff = new Form3();
            ff.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
        }

        private void Help_LinkLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1.已注册顾客：输入用户名与密码并点击“登录”按钮\n2.未注册顾客：点击“注册”按钮输入信息进行注册\n3.影院员工：点击“管理员登录”按钮并输入用户名和密码进行登录","提示");
        }

        private void Admin_Login(object sender, EventArgs e)
        {
            Form7 fff = new Form7();
            fff.Show();
            this.Hide();
        }
    }
}
