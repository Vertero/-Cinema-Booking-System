using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicLib;
using Model;
using System.Configuration;

namespace hh
{
    public partial class Form7 : Form
    {
        public static Admin a = new Admin();   //引用Admin这个类，方便属性引用
        public Form7()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(this.LoginEnter);   //给Form7添加Enter登录事件
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }


        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form7_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Dispose();
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// Enter键登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginEnter(object sender, KeyEventArgs e)
        {
            //读取文本框的值
            string ID = textBox1.Text;
            string password = textBox2.Text;

            //输入Enter时可以登录
            if (e.KeyCode == Keys.Enter)
            {
                //判断使得登录名和密码均不为空
                if (ID.Equals("") || password.Equals(""))
                {
                    MessageBox.Show("用户名或密码不能为空！");
                }
                else
                {
                    string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                    SqlConnection sqlCon = new SqlConnection(ConStr);
                    try
                    {
                        sqlCon.Open();
                        using (SqlCommand cmd = sqlCon.CreateCommand())
                        {
                            string sql = "CheckAdminLogin";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = sql;
                            SqlParameter par1 = new SqlParameter("@username", Convert.ToInt32(textBox1.Text));
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
                                a.ID = ID;
                                Form8 f = new Form8();
                                f.Show();
                                this.Hide();
                            }
                            else if(b==-1)
                            {
                                MessageBox.Show("用户名或密码输入错误！请重新输入。", "提示");
                                textBox2.Text = null;
                                textBox2.Focus();
                            }
                            else if (b == 0)
                            {
                                MessageBox.Show("您的账号已在另外一个设备登录！！", "提示");
                                textBox2.Text = null;
                                textBox2.Focus();
                            }
                        }
                        sqlCon.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString() + "打开数据库失败", "提示");
                    }
                }
            }
        }
        
        
        /// <summary>
        /// 登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginClick(object sender, EventArgs e)
        {
            //读取文本框的值
            string ID = textBox1.Text;
            string password = textBox2.Text;

            //判断使得登录名和密码均不为空
            if (ID.Equals("") || password.Equals(""))
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            else
            {
                string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                SqlConnection sqlCon = new SqlConnection(ConStr);
                try
                {
                    sqlCon.Open();
                    using (SqlCommand cmd = sqlCon.CreateCommand())
                    {
                        string sql = "CheckAdminLogin";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = sql;
                        SqlParameter par1 = new SqlParameter("@username", Convert.ToInt32(textBox1.Text));
                        SqlParameter par2 = new SqlParameter("@password", textBox2.Text);
                        SqlParameter par3 = new SqlParameter("@result", SqlDbType.Int);
                        par3.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(par1); //将变量添加到cmd命令对象中传给参数
                        cmd.Parameters.Add(par2);
                        cmd.Parameters.Add(par3);
                        cmd.ExecuteNonQuery();
                        int b = Convert.ToInt32(par3.Value);//得到第三个参数返回来的值
                        if (b == 1)
                        {
                            MessageBox.Show("登录成功！", "提示");
                            a.ID = ID;
                            Form8 f = new Form8();
                            f.Show();
                            this.Hide();
                        }
                        else if (b == -1)
                        {
                            MessageBox.Show("用户名或密码输入错误！请重新输入。", "提示");
                            textBox2.Text = null;
                            textBox2.Focus();
                        }
                        else if (b == 0)
                        {
                            MessageBox.Show("您的账号已在另外一个设备登录！！", "提示");
                            textBox2.Text = null;
                            textBox2.Focus();
                        }
                    }
                    sqlCon.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + "打开数据库失败", "提示");
                }
            }
        }

    }
}
