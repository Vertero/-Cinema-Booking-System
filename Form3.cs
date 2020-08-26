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
using System.Configuration;

namespace hh
{
    public partial class Form3 : Form
    {
        public SqlConnection conn { get; set; }
        public Form3()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void PassWordHide(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.textBox2.UseSystemPasswordChar = true;
                this.textBox3.UseSystemPasswordChar = true;
            }
            else
            {
                this.textBox2.UseSystemPasswordChar = false;
                this.textBox3.UseSystemPasswordChar = false;
            }
        }

        private bool CheckIDUse()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            if (textBox1.Text == String.Empty)
            {
                MessageBox.Show("用户名不能为空！", "提示");
                return false;
            }
            else if (textBox1.Text.ToString().IndexOf(" ")>=0)
            {
                MessageBox.Show("用户名不能包含空字符！", "提示");
                return false;
            }
            else
            {
                try
                {
                    conn = new SqlConnection(ConStr);
                    conn.Open();
                    string ID = this.textBox1.Text;
                    string sqlSel = "select count(*) from Customer where Customer_ID = '" + ID + "'";
                    SqlCommand com = new SqlCommand(sqlSel, conn);
                    if (Convert.ToInt32(com.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("用户名已被使用，请重新填写！", "提示");
                        textBox1.Text = null;
                        textBox1.Focus();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询错误！" + ex.Message);
                    return false;
                }
                finally
                {
                    if (conn != null)
                    {
                        //关闭数据库连接
                        conn.Close();
                    }
                }
            }
        }

        private void CheckIDUse_Click(object sender, EventArgs e)
        {
            if (CheckIDUse() == true)
            {
                MessageBox.Show("恭喜您，用户名未被使用过！", "提示");
            }
        }

        private bool CheckPassword()

        {
            string pattern = @"^(?=.*[0-9])(?=.*[a-zA-Z])(.{6,20})$";//字母数字，6到20位
            bool result = false;
            if (!string.IsNullOrEmpty(this.textBox2.Text.Trim()))
            {
                result = System.Text.RegularExpressions.Regex.IsMatch(this.textBox2.Text, pattern);
                if (!result)
                {
                    MessageBox.Show("密码长度为6-20位，且必须包含字母和数字", "提示");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("密码不能为空！", "提示");
                return false;
            }
        }


        private void Form3_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
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

        private void AddCustomer()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                conn.Open();
                string sqlSel = "insert into dbo.Customer values ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox4.Text + "','" + textBox5.Text + "',0,1,0)";
                SqlCommand com = new SqlCommand(sqlSel, conn);
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("插入错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }


        private void Regist_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == String.Empty || textBox2.Text == String.Empty || textBox3.Text == String.Empty || textBox4.Text == String.Empty || textBox5.Text == String.Empty)
            {
                MessageBox.Show("信息有空缺部分，请补充完整！", "提示");
            }
            else if (textBox1.Text.ToString().IndexOf(" ") >= 0 || textBox2.Text.ToString().IndexOf(" ") >= 0 || textBox3.Text.ToString().IndexOf(" ") >= 0 || textBox4.Text.ToString().IndexOf(" ") >= 0 || textBox5.Text.ToString().IndexOf(" ") >= 0)
            {
                MessageBox.Show("信息中不允许包含空格！", "提示");
            }
            else if (CheckIDUse()==true)
            {
                if (CheckPassword() == true)
                {
                    if (textBox2.Text != textBox3.Text)
                    {
                        MessageBox.Show("两次密码输入不一致，请检查！", "提示");
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(textBox5.Text, @"^1[34578][0-9]\d{8}$") == false)
                    {
                        MessageBox.Show("联系方式格式有误！", "提示");
                    }
                    else
                    {
                        AddCustomer();
                        textBox1.Text = null;
                        textBox2.Text = null;
                        textBox3.Text = null;
                        textBox4.Text = null;
                        textBox5.Text = null;
                        if (MessageBox.Show("注册成功！是否返回登录界面?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Form1 f = new Form1();
                            f.Show();
                            this.Hide();
                        }
                    }
                }
            }
        }
        
        private void Return(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
        }
    }
}
