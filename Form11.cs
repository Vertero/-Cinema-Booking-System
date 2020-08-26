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
    /// 排片管理
    /// </summary>
    public partial class Form11 : Form
    {
        private string aID;

        public Form11()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form11_Load);
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
        private void Form11_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 窗体刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form11_Load(object sender, EventArgs e)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //刷新datagridview界面
            string sqlSel1 = "SELECT Timing_ID," +
                    "Hall_ID," +
                    "dbo.Timing.Movie_ID," +
                    "Movie_Name," + 
                    "Timing_Price," +
                    "BeginTime," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                    " FROM dbo.Timing,dbo.Movie" +
                    " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID";
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

            //在选定了datagridview后给列命名
            dataGridView1.Columns[0].HeaderText = "排片号";
            dataGridView1.Columns[1].HeaderText = "影厅号";
            dataGridView1.Columns[2].HeaderText = "电影编号";
            dataGridView1.Columns[3].HeaderText = "电影名字";
            dataGridView1.Columns[4].HeaderText = "排片价格";
            dataGridView1.Columns[5].HeaderText = "开始时间";
            dataGridView1.Columns[6].HeaderText = "结束时间";

            //让窗体中控件可以跟随着窗体大小变化而改变大小
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());

            //根据数据内容自动调整列宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoAdding(object sender, EventArgs e)
        {
            //定义文本框读取初始值
            int HallID = 0;
            int MovieID = 0;
            decimal TimingPrice = 0;
            string BeginTime = "";

            //读取文本框的值              
            if (textBox2.Text.Trim() != "")
            {
                HallID = Convert.ToInt32(textBox2.Text.Trim());
            }
            else
            {
                HallID = 0;
            }
            if (textBox3.Text.Trim() != "")
            {
                MovieID = Convert.ToInt32(textBox3.Text.Trim());
            }
            else
            {
                MovieID = 0;
            }
            if (textBox4.Text.Trim() != "")
            {
                TimingPrice = Convert.ToDecimal(textBox4.Text.Trim());
            }
            else
            {
                TimingPrice = 0;
            }
            BeginTime = textBox5.Text.Trim();
           

            //连接数据库，尝试添加数据
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();
                string SQL_MAX = "SELECT MAX(Timing_ID)+1 FROM dbo.Timing";
                SqlCommand sqlCommand2 = new SqlCommand(SQL_MAX,sqlConnection);
                sqlCommand2.ExecuteNonQuery();
                int ID_Selected = Convert.ToInt32(sqlCommand2.ExecuteScalar());
 
                string SQL_Adding = "INSERT INTO dbo.Timing VALUES(" + ID_Selected + "," + HallID + "," + MovieID + "," + TimingPrice + ",'" + BeginTime + "',1)";
                SqlCommand sqlCommand = new SqlCommand(SQL_Adding, sqlConnection);
                sqlCommand.ExecuteNonQuery();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n输入失败", "提示");
                textBox1.Focus();
            }
            finally
            {
                sqlConnection.Close();  //关闭数据库
                sqlConnection.Dispose();  //释放资源

                //将所有的输入框内容清空
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                

                //让光标在第一个文本框
                textBox2.Focus();
            }


            //刷新datagridview界面
            string sqlSel1 = "SELECT Timing_ID," +
                    "Hall_ID," +
                    "dbo.Timing.Movie_ID," +
                    "Movie_Name," +
                    "Timing_Price," +
                    "BeginTime," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                    " FROM dbo.Timing,dbo.Movie" +
                    " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID";
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
            //打开数据库并尝试连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();
                int select_id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                int b = CheckInfoDeleting(select_id);
                if (b == 0 || b == 3)
                {
                    string SQL_Deleting = "UPDATE dbo.Timing SET Timing_isActive=0 WHERE Timing_ID=" + select_id;
                    SqlCommand sqlCommand = new SqlCommand(SQL_Deleting, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                else if (b == 1)
                {
                    MessageBox.Show("与系统数据发生冲突，修改失败！", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n请检查是否选中目标", "提示");
                textBox2.Focus();
            }
            finally
            {
                sqlConnection.Close();   //关闭数据库
                sqlConnection.Dispose();  //释放资源
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Timing_ID," +
                    "Hall_ID," +
                    "dbo.Timing.Movie_ID," +
                    "Movie_Name," +
                    "Timing_Price," +
                    "BeginTime," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                    " FROM dbo.Timing,dbo.Movie" +
                    " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID";
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
            int TimingID_Selected = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

            //选取文本框的值
            int HallID = 0;
            int MovieID = 0;
            decimal TimingPrice = 0;
            string BeginTime = textBox5.Text.Trim();

            //如果修改框中有NULL值，那么采用原数值来代替
            if (textBox2.Text.Trim() =="")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                HallID = Convert.ToInt32(test1);
            }
            else
            {
                HallID = Convert.ToInt32(textBox2.Text.Trim());
            }
            if (textBox3.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                MovieID = Convert.ToInt32(test1);
            }
            else
            {
                MovieID = Convert.ToInt32(textBox3.Text.Trim());
            }
            if (textBox4.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                TimingPrice = Convert.ToDecimal(test1);
            }
            else
            {
                TimingPrice = Convert.ToDecimal(textBox4.Text.Trim());
            }
            if (BeginTime.Trim() == "") { BeginTime = dataGridView1.SelectedRows[0].Cells[5].Value.ToString(); }

            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();
                string SQL_Updating = "UPDATE dbo.Timing SET Hall_ID=" + HallID + ",Movie_ID=" + MovieID + ",Timing_Price=" + TimingPrice + ",BeginTime ='" + BeginTime + "' WHERE Timing_ID=" + TimingID_Selected;
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
                textBox2.Focus();
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Timing_ID," +
                    "Hall_ID," +
                    "dbo.Timing.Movie_ID," +
                    "Movie_Name," +
                    "Timing_Price," +
                    "BeginTime," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                    " FROM dbo.Timing,dbo.Movie" +
                    " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();
            SqlCommand sc = new SqlCommand(sqlSel1, con);
            SqlDataAdapter sda = new SqlDataAdapter(sc);  //创建一个适配器
            DataSet ds = new DataSet();   //创建一个DataSet来接受适配器的内容
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            con.Close();
            con.Dispose();
        }


        /// <summary>
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoSearching(object sender, EventArgs e)
        {
            //读取主码进行查询
            int TimingID = 0;
            if (textBox1.Text.Trim()!="")
            {
                TimingID = Convert.ToInt32(textBox1.Text.Trim());
            }
            else
            {
                MessageBox.Show("排片编号的输入框不可为空，请重新输入", "提示");
                textBox1.Focus();
            }


            //连接数据库，尝试去搜索
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();

                int b = CheckInfoSearching(TimingID);

                if (b == 1)
                {
                    string SQL_Searching = "SELECT Timing_ID," +
                       "Hall_ID," +
                       "dbo.Timing.Movie_ID," +
                       "Movie_Name," + 
                       "Timing_Price," +
                       "BeginTime," +
                       "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                       " FROM dbo.Timing,dbo.Movie" +
                       " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID AND Timing_ID=" + TimingID;
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
            string sqlSel1 = "SELECT Timing_ID," +
                    "Hall_ID," +
                    "dbo.Timing.Movie_ID," +
                    "Movie_Name," +
                    "Timing_Price," +
                    "BeginTime," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                    " FROM dbo.Timing,dbo.Movie" +
                    " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID";
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
        /// 逻辑删除冲突检测
        /// </summary>
        /// <param name="ID_Selected"></param>
        /// <returns></returns>
        private int CheckInfoDeleting(int ID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();

                string sqlsel1 = "SELECT COUNT(*) FROM Timing,[Order]" +
                    " WHERE Timing.Timing_ID=[Order].Timing_ID" +
                    " AND Timing.Timing_ID=" + ID_Selected ;
                string sqlsel2 = "SELECT COUNT(*) FROM Timing,[Order]" +
                    " WHERE Timing.Timing_ID=[Order].Timing_ID" +
                    " AND Condition='未使用'" +
                    " AND Timing_isActive=1" +
                    " AND Timing.Timing_ID=" + ID_Selected;

                SqlCommand sqlCommand1 = new SqlCommand(sqlsel1, sqlConnection);
                sqlCommand1.ExecuteNonQuery();
                if (Convert.ToInt32(sqlCommand1.ExecuteScalar()) > 0)
                {
                    SqlCommand sqlCommand2 = new SqlCommand(sqlsel2, sqlConnection);
                    sqlCommand2.ExecuteNonQuery();

                    if (Convert.ToInt32(sqlCommand2.ExecuteScalar()) > 0)
                        return 1;
                    else
                        return 0;

                }
                else
                    return 3;

            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败！" + ex.Message);
                return 2;
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

                string SQL_Searching = "SELECT Timing_ID," +
                   "Hall_ID," +
                   "dbo.Timing.Movie_ID," +
                   "Movie_Name," + 
                   "Timing_Price," +
                   "BeginTime," +
                   "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime" +
                   " FROM dbo.Timing,dbo.Movie" +
                   " WHERE Timing_isActive<>0 AND dbo.Timing.Movie_ID=dbo.Movie.Movie_ID AND Timing_ID=" + ID_Selected;
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
