using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Model;
using System.Configuration;

namespace hh
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (textBox1.Text.Equals("") || textBox2.Text.Equals("") || textBox3.Text.Equals("") || textBox4.Text.Equals(""))
                {
                    MessageBox.Show("信息有空缺部分，请补充完整！", "提示");
                }
                else
                {
                    if (CheckCustomer() == true)
                    {
                        if (Customer.FindClassBycID(textBox1.Text.ToString()) == 1)
                        {
                            MessageBox.Show("您已经是会员！", "提示");
                        }
                        else
                        {
                            if (CalculateMoney() >= 2000)
                            {
                                MessageBox.Show("恭喜您，办理会员成功！(消费满2000元，免费办理)", "提示");
                                VIP();
                            }
                            else
                            {
                                MessageBox.Show("恭喜您，办理会员成功！(支付金额：200元)", "提示");
                                VIP();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("用户不存在或信息填写有误！", "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请先勾选确认阅读办理须知！", "提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals("") || textBox3.Text.Equals("") || textBox4.Text.Equals(""))
            {
                MessageBox.Show("信息有空缺部分，请补充完整！", "提示");
            }
            else
            {
                if (CheckCustomer() == true)
                {
                    if (CalculateMoney() >= 2000)
                    {
                        MessageBox.Show("您在本影院的消费总额为" + CalculateMoney()+"元，可免费办理会员", "提示");
                    }
                    else
                    {
                        MessageBox.Show("您在本影院的消费总额为" + CalculateMoney() + "元，办理会员需支付200元", "提示");
                    }
                }
                else
                {
                    MessageBox.Show("用户不存在或信息填写有误！", "提示");
                }
            }
        }


        private bool CheckCustomer()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            string cID=textBox1.Text.ToString();
            string pwd=textBox2.Text.ToString();
            string name=textBox3.Text.ToString();
            string contact=textBox4.Text.ToString();
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from Customer where Customer_ID='" + cID + "' and Customer_Password='" + pwd + "' and Customer_Name='" + name + "' and Customer_Contact='" + contact + "' and Customer_isActive=1", conn);
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
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

        
        private double CalculateMoney()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            string cID=textBox1.Text.Trim();
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sql = "select sum(Order_Price) from [Order],Timing where [Order].Timing_ID=Timing.Timing_ID and Customer_ID='" + cID + "'";
                SqlCommand sqlCommand = new SqlCommand(sql, conn);
                sqlCommand.ExecuteNonQuery();

                string accept = sqlCommand.ExecuteScalar().ToString();

                if (accept.Trim() == "")
                {
                    return 0;
                }
                else
                {
                    return Convert.ToDouble(accept);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
                return 0;
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

        private void VIP()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sql = "update Customer set Customer_Class=1 where Customer_ID='" + textBox1.Text.ToString() + "'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新错误！" + ex.Message);
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
}
