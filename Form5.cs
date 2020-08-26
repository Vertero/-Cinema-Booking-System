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
using PublicLib;

namespace hh
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }
        private string cID = Form1.c.ID;
        private void Form5_Load(object sender, EventArgs e)
        {
            int Customer_Class = Customer.FindClassBycID(cID);
            label3.Text = cID;
            if (Customer_Class == 0)
            {
                label5.Text = "普通顾客";
            }
            else if (Customer_Class == 1)
            {
                label5.Text = "尊享会员";
            }
            SetExpiredOrder();
            DataGridViewCheckBoxColumn cbc = new DataGridViewCheckBoxColumn();
            cbc.Name = "IsChecked";
            cbc.TrueValue = true;
            cbc.FalseValue = false;
            cbc.DataPropertyName = "IsChecked";
            dataGridView1.Columns.Add(cbc);
            dataGridView1.Columns[0].HeaderText = "选择";
            dataGridView1.Columns[0].ReadOnly = false;
            ShowOrder(tabPage1, "select Order_ID,Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Condition='未使用' and Customer_ID='" + cID + "' order by BeginTime desc");
            
        }

        private void SetExpiredOrder()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sql = "update [Order] set Condition = '已过期' where Condition = '未使用' and (select convert (varchar, dateadd(minute,Movie_Time,BeginTime), 120) from Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Timing_ID=[Order].Timing_ID)<='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"'";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败！" + ex.Message);
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

        private void ShowOrder(TabPage tp, string sqlSel)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sqlSel, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[1].HeaderText = "订单号";
                dataGridView1.Columns[2].HeaderText = "订单时间";
                dataGridView1.Columns[3].HeaderText = "影片名称";
                dataGridView1.Columns[4].HeaderText = "开始时间";
                dataGridView1.Columns[5].HeaderText = "结束时间";
                dataGridView1.Columns[6].HeaderText = "影厅";
                dataGridView1.Columns[7].HeaderText = "支付价格";
                dataGridView1.Columns[8].HeaderText = "订单状态";
                for (int i = 1; i < 9; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                tp.Controls.Add(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！！" + ex.Message);
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
	        {
                ShowOrder(tabPage1, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Condition='未使用' and Customer_ID='" + cID + "' order by BeginTime desc");
	        }
            else if (tabControl1.SelectedIndex == 1)
            {
                ShowOrder(tabPage2, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Customer_ID='" + cID + "' and (Condition='已过期' or Condition='已使用') order by BeginTime desc");
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                ShowOrder(tabPage3, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Customer_ID='" + cID + "' order by BeginTime desc");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int count = 0;
                bool judge=true;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string selectValue = dataGridView1.Rows[i].Cells[0].EditedFormattedValue.ToString();
                    if (selectValue == "True")
                    {
                        count++;
                    }
                    if (selectValue == "True" && (dataGridView1.Rows[i].Cells[8].Value.ToString() == "已使用" || dataGridView1.Rows[i].Cells[8].Value.ToString() == "已过期"))
                    {
                        judge=false;
                    }
                }
                if (count == 0)
                {
                    MessageBox.Show("请至少选择一条订单记录！", "提示");
                }
                else
                {
                    if (judge == false)
                    {
                        MessageBox.Show("已使用或已过期的订单不可退票！", "提示");
                    }
                    else
                    {
                        if (MessageBox.Show(this, "共选择 " + count + " 条订单，确定退票？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information).ToString() == "Yes")
                        {
                            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                            SqlConnection conn = null;
                            try
                            {
                                conn = new SqlConnection(ConStr);
                                //打开数据库
                                conn.Open();
                                string str = "(";
                                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                                {
                                    string selectValue = dataGridView1.Rows[i].Cells[0].EditedFormattedValue.ToString();
                                    if (selectValue == "True")
                                    {
                                        str += dataGridView1.Rows[i].Cells[1].Value.ToString() + ",";
                                    }
                                }
                                if (str.Length > 0)
                                {
                                    str = str.Substring(0, str.Length - 1);
                                    str += ")";
                                }
                                string sql = "delete from [Order] where Order_ID in " + str;
                                SqlCommand cmd = new SqlCommand(sql, conn);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("退票成功！", "提示");
                                //刷新“未使用订单”或“全部订单”页面的状态
                                if (tabControl1.SelectedIndex == 0)
                                {
                                    ShowOrder(tabPage1, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Condition='未使用' and Customer_ID='" + cID + "' order by BeginTime desc");
                                }
                                else if (tabControl1.SelectedIndex == 2)
                                {
                                    ShowOrder(tabPage3, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Customer_ID='" + cID + "' order by BeginTime desc");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("查询错误！" + ex.Message);
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
            }
            else
            {
                MessageBox.Show("请至少选择一条订单记录！", "提示");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                ShowOrder(tabPage1, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Condition='未使用' and Customer_ID='" + cID + "' order by BeginTime desc");
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                ShowOrder(tabPage2, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Customer_ID='" + cID + "' and (Condition='已过期' or Condition='已使用') order by BeginTime desc");
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                ShowOrder(tabPage3, "select Order_ID, Order_Time, Movie_Name, BeginTime, convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime, Hall_ID, Order_Price, Condition from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Customer_ID='" + cID + "' order by BeginTime desc");
            }
        }
     }
 } 