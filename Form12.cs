using Model;
using PublicLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hh
{
    /// <summary>
    /// 订单管理
    /// </summary>
    public partial class Form12 : Form
    {
        private string aID;

        public Form12()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form12_Load);
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        /// <summary>
        /// 窗体关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form12_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                DialogResult result = MessageBox.Show("确定要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //连接数据库，设置数据库中管理员的登录状态为0
                    aID = Form7.a.ID;
                    Admin.CancelLogin(aID);

                    Dispose();
                    System.Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }


        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoAdding(object sender, EventArgs e)
        {
            //读取文本框的值
            string SeatID = "";
            string CustomerID = "";
            string OrderTime = "";
            int TimingID = 0;
            string Condition = "";
            decimal OrderPrice = 0;

            //保证输入的每一个文本框都是非空值
            if ( textBox2.Text.Trim() != "" && textBox3.Text.Trim() != "" && textBox4.Text.Trim() != "" && textBox5.Text.Trim() != "" && textBox6.Text.Trim() != "" && comboBox1.SelectedItem.ToString().Trim() != "")
            {
                //读取文本框的值
                SeatID = textBox2.Text.Trim();
                CustomerID = textBox3.Text.Trim();
                OrderTime = textBox4.Text.Trim();
                TimingID = Convert.ToInt32(textBox5.Text.Trim());  //如果使用string类型，那么SQL语句会报错
                OrderPrice = Convert.ToDecimal(textBox6.Text.Trim());
                Condition = comboBox1.SelectedItem.ToString().Trim();
            }
            else
            {
                MessageBox.Show("输入框中不能有空值");
            }

            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(ConStr);
            try
            {
                sqlCon.Open();
            string SQL_Adding = "INSERT INTO dbo.[Order] VALUES ((SELECT MAX(Order_ID)+1 FROM dbo.[Order]),'"
                + SeatID + "','"
                + CustomerID + "','"
                + OrderTime + "',"
                + TimingID + ","
                + OrderPrice + ",'"
                + Condition + "')";
                SqlCommand cmd = new SqlCommand(SQL_Adding, sqlCon);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n输入失败", "提示");
                textBox1.Focus();
            }
            finally
            {
                sqlCon.Close();  //关闭数据库
                sqlCon.Dispose();  //释放资源

                //将所有的输入框内容清空
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                comboBox1.SelectedIndex = 3;

                //让光标在第一个文本框
                textBox1.Focus();
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();
            SqlCommand sc = new SqlCommand(sqlSel1, con);
            SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
            DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            con.Close();
            con.Dispose();


            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;
        }


        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoDeleting(object sender, EventArgs e)
        {
            //打开数据库并且尝试连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();
                int select_id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());   //选择的当前行第一列的值，也就是ID
                string SQL_Deleting = "DELETE FROM dbo.[Order] WHERE Order_ID=" + select_id;  //表示SQL中的删除语句
                SqlCommand sqlCommand = new SqlCommand(SQL_Deleting, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "请检查是否选中目标" , "提示");
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();
            SqlCommand sc = new SqlCommand(sqlSel1, con);
            SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
            DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            con.Close();
            con.Dispose();


            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;
        }


        /// <summary>
        /// 修改按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoUpdating(object sender, EventArgs e)
        {           
            //选定原来的值
            int OrderID_Selected = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

            //读取文本框的值
            string SeatID = textBox2.Text.Trim();
            string CustomerID = textBox3.Text.Trim();
            string OrderTime = textBox4.Text.Trim();
            int TimingID = 0;
            decimal OrderPrice = 0;
            string Condition = comboBox1.SelectedItem.ToString().Trim();
            

            //如果修改框中有NULL值，那么采用原函数值来代替
            if (SeatID == "")
            {
                SeatID = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            }
            if (CustomerID == "") 
            { 
                CustomerID = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(); 
            }
            if (OrderTime == "") 
            { 
                OrderTime = dataGridView1.SelectedRows[0].Cells[4].Value.ToString(); 
            }
            if (textBox5.Text.Trim() == "")
            {
                string test2 = dataGridView1.SelectedRows[0].Cells[6].Value.ToString();
                TimingID = Convert.ToInt32(test2);
            }
            else
            {
                TimingID = Convert.ToInt32(textBox5.Text.Trim());
            }
            if (textBox6.Text.Trim() == "")
            {
                string test2 = dataGridView1.SelectedRows[0].Cells[7].Value.ToString();
                OrderPrice = Convert.ToDecimal(test2);
            }
            else
            {
                OrderPrice = Convert.ToDecimal(textBox6.Text.Trim());
            }
            if (Condition.Trim() == "") 
            { 
                Condition = dataGridView1.SelectedRows[0].Cells[8].Value.ToString(); 
            }

            if (dataGridView1.SelectedRows[0].Cells[6].Value.ToString() == "已过期" || dataGridView1.SelectedRows[0].Cells[6].Value.ToString() == "已使用")
            {
                MessageBox.Show("已过期或已使用的订单不允许修改！", "提示");
            }
            else
            {
                bool b = true;
                if (comboBox1.SelectedItem.ToString().Trim() == "已过期")
                    b = CheckInfoUpdating1(OrderID_Selected);
                else if (comboBox1.SelectedItem.ToString().Trim() == "已使用")
                    b = CheckInfoUpdating2(OrderID_Selected);
                if (b == false)
                    MessageBox.Show("订单状态更新存在冲突！", "提示");
                else
                {
                    //连接数据库
                    string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                    SqlConnection sqlConnection = new SqlConnection(ConStr);
                    try
                    {
                        sqlConnection.Open();
                        string SQL_Updating = "UPDATE dbo.[Order] SET Seat_ID='" + SeatID
                            + "',Customer_ID='" + CustomerID
                            + "',Order_Time='" + OrderTime
                            + "',Timing_ID=" + TimingID
                            + ",Order_Price=" + OrderPrice
                            + ",Condition='" + Condition
                            + "' WHERE Order_ID =" + OrderID_Selected;
                        SqlCommand sqlCommand = new SqlCommand(SQL_Updating, sqlConnection);
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString() + "\n请检查输入是否有误", "提示");
                    }
                    finally
                    {
                        sqlConnection.Close();   //将数据库关闭
                        sqlConnection.Dispose();   //释放资源

                        //清理文本框中的内容
                        textBox2.Text = null;
                        textBox3.Text = null;
                        textBox4.Text = null;
                        textBox5.Text = null;
                        textBox6.Text = null;
                        comboBox1.SelectedIndex = 3;
                        textBox2.Focus();
                    }

                    //刷新datagridview界面
                    string sqlSel1 = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                        " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                        " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID";
                    SqlConnection con = new SqlConnection(ConStr);
                    con.Open();
                    SqlCommand sc = new SqlCommand(sqlSel1, con);
                    SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
                    DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
                    sda.Fill(ds);
                    this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
                    con.Close();
                    con.Dispose();

                    //在加载出datagridview后之后可以自动选定第一行
                    this.dataGridView1.Rows[0].Selected = true;
                }
            }
        }


        /// <summary>
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoSearching(object sender, EventArgs e)
        {
            //读取主码进行查询
            int OrderID = 0;
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("订单号不能为空，请重新输入", "提示");
            }
            else
            {
                OrderID = Convert.ToInt32(textBox1.Text.Trim());
            }
            

            //连接数据库，尝试去搜索
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();

                int b = CheckInfoSearching(OrderID);

                if (b == 1)
                {
                    string SQL_Searching = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID AND Order_ID =" + OrderID;
                    SqlCommand sqlCommand = new SqlCommand(SQL_Searching, sqlConnection);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();   //通过SQLDataReader来Binding找到的数值
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = sqlDataReader;
                    dataGridView1.DataSource = bindingSource;
                }
                else if (b == 2)
                {
                    MessageBox.Show("查询错误，请验证ID是否正确", "提示");
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n请查看查询的输入是否有错", "提示");
            }
            finally
            {
                sqlConnection.Close();  //关闭数据库
                sqlConnection.Dispose();   //释放资源

                textBox1.Text = "";   //重新设置数值框
                textBox1.Focus();
            }
        }


        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoRefreshing(object sender, EventArgs e)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //刷新datagridview界面
            string sqlSel1 = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();
            SqlCommand sc = new SqlCommand(sqlSel1, con);
            SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
            DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            con.Close();
            con.Dispose();


            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;
        }


        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Return(object sender, EventArgs e)
        {
            Form8 f = new Form8();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 窗体刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form12_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("未使用");
            comboBox1.Items.Add("已过期");
            comboBox1.Items.Add("已使用");
            comboBox1.Items.Add("");
            this.comboBox1.SelectedIndex = 3;

            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //刷新datagridview界面
            string sqlSel1 = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();
            SqlCommand sc = new SqlCommand(sqlSel1, con);
            SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
            DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            con.Close();
            con.Dispose();


            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;

            //在形成datagridview后给生成的列命名
            dataGridView1.Columns[0].HeaderText = "订单号";
            dataGridView1.Columns[1].HeaderText = "电影名称";
            dataGridView1.Columns[2].HeaderText = "座位号";
            dataGridView1.Columns[3].HeaderText = "用户名";
            dataGridView1.Columns[4].HeaderText = "订单时间";
            dataGridView1.Columns[5].HeaderText = "开始时间";
            dataGridView1.Columns[6].HeaderText = "排片编号";
            dataGridView1.Columns[7].HeaderText = "支付价格";
            dataGridView1.Columns[8].HeaderText = "订单状态";

            //让窗体中控件可以跟随着窗体大小变化而改变大小
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());

            //根据数据内容自动调整列宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 

        }


        /// <summary>
        /// 增加冲突逻辑验证1
        /// </summary>
        /// <param name="OrderID_Selected"></param>
        /// <returns></returns>
        private bool CheckInfoUpdating1(int OrderID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();
                string sql = "Select count(*) from Timing,[Order],Movie where Timing.Timing_ID=[Order].Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Order_ID=" + OrderID_Selected + " and convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120)>='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                SqlCommand sqlCommand1 = new SqlCommand(sql, sqlConnection);
                if (Convert.ToInt32(sqlCommand1.ExecuteScalar()) > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败！" + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    //关闭数据库连接
                    sqlConnection.Close();
                }
            }
        }


        /// <summary>
        /// 增加冲突逻辑验证2
        /// </summary>
        /// <param name="OrderID_Selected"></param>
        /// <returns></returns>
        private bool CheckInfoUpdating2(int OrderID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();
                string sqll = "Select count(*) from Timing,[Order],Movie where Timing.Timing_ID=[Order].Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Order_ID=" + OrderID_Selected + " and (convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120)<='" + DateTime.Now.ToString("yyyy - MM - dd HH: mm: ss") + "' or convert(varchar, dateadd(minute,-15,BeginTime), 120)>='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                SqlCommand sqlCommand2 = new SqlCommand(sqll, sqlConnection);
                if (Convert.ToInt32(sqlCommand2.ExecuteScalar()) > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败！" + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    //关闭数据库连接
                    sqlConnection.Close();
                }
            }
        }


        /// <summary>
        /// 搜索错误提示
        /// </summary>
        /// <param name="ID_Selected"></param>
        /// <returns></returns>
        private int CheckInfoSearching(int ID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();

                string SQL_Searching = "SELECT Order_ID,Movie_Name,Seat_ID,Customer_ID,Order_Time,BeginTime,[Order].Timing_ID,Order_Price,Condition" +
                 " FROM dbo.[Order],dbo.Timing,dbo.Movie" +
                 " WHERE [Order].Timing_ID=Timing.Timing_ID AND Timing.Movie_ID=Movie.Movie_ID AND Order_ID =" + ID_Selected;
                SqlCommand sqlCommand = new SqlCommand(SQL_Searching, sqlConnection);

                if (Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0)
                    return 1;
                else
                    return 2;

            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败！" + ex.Message);
                return 0;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    //关闭数据库连接
                    sqlConnection.Close();
                }
            }
        }
    }
}
