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
    public partial class BuyTickets : Form
    {
        public BuyTickets()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }
        public string mName;
        public double discount;
        public string tID;
        public string begintime;
        public string endtime;
        public string hID;
        public string price;
        public string vipprice;
        public string finalprice;
        private int rows, columns;
        private string cID;

        private void BuyTickets_Load(object sender, EventArgs e)
        {
            cID = Form1.c.ID;
            int Class = Customer.FindClassBycID(cID);
            FindByHall_ID(hID, out rows, out columns);
            tabPage1.Text = hID + "号影厅";
            label3.Text = cID;
            if (Class == 0)
            {
                label5.Text = "普通顾客";
                label7.Text = "无";
                finalprice = price;
            }
            else if (Class == 1)
            {
                label5.Text = "尊享会员";
                label7.Text = (discount*10).ToString() + "折";
                finalprice = vipprice;
            }
            int RowInterval = (500 - 25 * rows) / (rows + 1);
            int ColumnInterval = (800 - 50 * columns) / (columns + 1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Label lb = new Label();
                    lb.Name="lb"+ (i + 1).ToString() + "-" + (j + 1).ToString();
                    lb.BackColor = Color.Yellow;
                    lb.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    lb.AutoSize = false;
                    lb.Size = new System.Drawing.Size(50, 25);
                    lb.Text = (i + 1).ToString() + "-" + (j + 1).ToString();
                    lb.TextAlign = ContentAlignment.MiddleCenter;
                    lb.Location = new Point(ColumnInterval + j * (ColumnInterval+50), 50 + RowInterval + i * (RowInterval+25));
                    lb.Click += lbSeat_Click;
                    lb.DoubleClick += lbSeat_DoubleClick;
                    tabPage1.Controls.Add(lb);
                }
            }
            UpdateSeat();
        }

        private void lbSeat_Click(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            if(lb.BackColor == Color.Blue)
            {
                MessageBox.Show("该座位已售出，请选择其他座位","提示");
            }
            else if(lb.BackColor == Color.Yellow)
            {
                lb.BackColor = Color.Red;
            }
        }

        private void lbSeat_DoubleClick(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            lb.BackColor = Color.Yellow;
        }

        private void FindByHall_ID(string hID, out int rows, out int columns)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                String sqlSel = "select Hall_Row,Hall_Column from Hall where Hall_ID=" + int.Parse(hID);
                SqlDataAdapter sda = new SqlDataAdapter(sqlSel, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                rows = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                columns = int.Parse(ds.Tables[0].Rows[0][1].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误2！" + ex.Message);
                rows = 0;
                columns = 0;
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

        private void UpdateSeat()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                String sqlSel = "select Seat_ID from [Order] where Timing_ID=" + int.Parse(tID);
                SqlDataAdapter sda = new SqlDataAdapter(sqlSel, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    foreach (var item in this.tabPage1.Controls)
                    {
                        if (item is Label && (item as Label).Text == ds.Tables[0].Rows[i][0].ToString())
                        {
                            (item as Label).BackColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误3！" + ex.Message);
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

        private bool CheckSeat(List<string> SeatNum)
        {
            SqlCommand cmd;
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                foreach (var seatnum in SeatNum)
                {
                    cmd = new SqlCommand("select count(*) from [Order] where Timing_ID=" + int.Parse(tID) + "and Seat_ID='" + seatnum + "'", conn);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误4！" + ex.Message);
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

        private void Buytickets(object sender, EventArgs e)
        {
            int ordernum = OrderNum();
            List<string> SeatNum=new List<string>();
            string seatnum=null;
            foreach (var item in this.tabPage1.Controls)
            {
                if (item is Label && (item as Label).BackColor == Color.Red)
                {
                    SeatNum.Add((item as Label).Text);
                }
            }

            if(SeatNum!=null && SeatNum.Count>0)
            {
                foreach(var num in SeatNum)
                {
                    seatnum=seatnum+num+" ";
                }
                if (MessageBox.Show("【订单信息】\n影片名称："+mName+"\n时间："+begintime+" - "+endtime+"\n座位："+seatnum+"\n价格："+(SeatNum.Count()*double.Parse(finalprice)).ToString()+"元\n确认购买？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DateTime et = Convert.ToDateTime(endtime);
                    if (et<DateTime.Now)
                    {
                        MessageBox.Show("您选择的电影排片已结束，请重新选择……", "提示");
                        this.Hide();
                    }
                    else if (CheckSeat(SeatNum) == false)
                    {
                        MessageBox.Show("手速慢了，有所选座位被抢先预订！请重新选择……", "提示");
                        UpdateSeat();
                    }
                    else
                    {
                        for (int i = 0; i < SeatNum.Count; i++)
                        {
                            string AddOrderSql = "insert into [Order](Order_ID,Seat_ID,Customer_ID,Order_Time,Timing_ID,Order_Price,Condition) values(" + (OrderNum() + 1) + ",'" + SeatNum[i] + "','" + cID + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + int.Parse(tID) + "," + decimal.Parse(finalprice) + ",'未使用')";
                            AddOrder(AddOrderSql);
                        }
                        MessageBox.Show("购票成功！", "提示");
                        UpdateSeat();
                    }
                }
            }
            else
            {
                MessageBox.Show("您没有选择座位！", "提示");
            }
        }

        private int OrderNum()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter("select max(Order_ID) from [Order]", conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds.Tables[0].Rows[0][0].ToString() == "")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误5！" + ex.Message);
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

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateSeat();
        }

        private void AddOrder(string sql)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加错误！" + ex.Message);
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