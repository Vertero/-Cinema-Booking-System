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
    /// 影厅管理
    /// </summary>
    public partial class Form9 : Form
    {
        private string aID;

        public Form9()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form9_Load);
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
        private void Form9_FormClosing(object sender, FormClosingEventArgs e)
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
            //设定初值
            int Row = 0;
            int Column = 0;

            if (textBox2.Text.Trim()!="" && textBox3.Text.Trim()!="" )
            {
                //令设定的初值等于文本框的值
                Row = Convert.ToInt32(textBox2.Text.Trim());
                Column = Convert.ToInt32(textBox3.Text.Trim());
            }
            else
            {
                MessageBox.Show("输入框中不能有空数值");
            }

            //打开数据库，尝试添加数据
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();

                //实现SQL语句中ID是按照顺序增加的操作
                string SQL_Adding = "INSERT INTO dbo.Hall VALUES( (SELECT MAX(Hall_ID)+1 FROM dbo.Hall)," + 
                    Row + "," + 
                    Column  + ",1)";
                SqlCommand sqlCommand = new SqlCommand(SQL_Adding, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n输入失败", "提示");
            }
            finally
            {
                sqlConnection.Close();   //关闭数据库
                sqlConnection.Dispose();   //释放资源

                //将所有的输入框内容清空
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;

                //让光标在第一个文本框
                textBox2.Focus();
            }


            //刷新datagridview界面
            string sqlSel1 = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1";
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
                bool b = CheckInfoDeleting(select_id);
                if(b==true)
                {
                    string SQL_Deleting = "UPDATE dbo.Hall SET Hall_isActive=0 WHERE Hall_ID=" + select_id;  //语句中采用逻辑删除
                    SqlCommand sqlCommand = new SqlCommand(SQL_Deleting, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("与系统数据发生冲突，删除失败！", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n请检查是否选中目标", "提示");
            }
            finally
            {
                sqlConnection.Close();   //关闭数据库
                sqlConnection.Dispose();  //释放资源
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1";
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
            int ID_Selected = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

            //选取文本框的值
            int Row = 0;
            int Column = 0;

            //如果修改框中有NULL值，那么采用原数值来代替
            if (textBox2.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                Row = Convert.ToInt32(test1);
            }
            else
            {
                Row = Convert.ToInt32(textBox2.Text.Trim());
            }
            if (textBox3.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                Column = Convert.ToInt32(test1);
            }
            else
            {
                Column = Convert.ToInt32(textBox3.Text.Trim());
            }

            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                int Judge = CheckInfoUpdating(ID_Selected);
                if (Judge == 0 || Judge == 3) 
                {
                    sqlConnection.Open();

                    string SQL_Updating = "UPDATE dbo.Hall SET Hall_Row=" + Row 
                        + ",Hall_Column=" + Column 
                        + "WHERE Hall_ID=" + ID_Selected;

                    SqlCommand sqlCommand = new SqlCommand(SQL_Updating, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                else if (Judge == 1)
                {
                    MessageBox.Show("与系统数据发生冲突，修改失败！", "提示");
                    
                    //清理文本框中的内容
                    textBox2.Text = null;
                    textBox3.Text = null;
                    textBox2.Focus();
                }
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
                textBox2.Focus();
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1";
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
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoSearching(object sender, EventArgs e)
        {
            //读取主码进行查询
            int ID = 0;
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("影厅号不能为空","提示");
            }
            else
            {
                ID = Convert.ToInt32(textBox1.Text.Trim());
            }
            

            //连接数据库，尝试去搜索
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();

                int b = CheckInfoSearching(ID);

                if (b == 1)
                {
                    string SQL_Searching = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1" +
                    " AND Hall_ID=" + ID;
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
                MessageBox.Show(ex.Message.ToString() + "\n请查看查询输入是否有错", "提示");
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
            string sqlSel1 = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1";
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
        private void Form9_Load(object sender, EventArgs e)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //刷新datagridview界面
            string sqlSel1 = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1";
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
            
            //给形成的datagridview中的列进行命名
            dataGridView1.Columns[0].HeaderText = "影厅编号";
            dataGridView1.Columns[1].HeaderText = "影厅行号";
            dataGridView1.Columns[2].HeaderText = "影厅列号";

            //让窗体中控件可以跟随着窗体大小变化而改变大小
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());

            //根据数据内容自动调整列宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }


        /// <summary>
        /// 逻辑删除冲突检测
        /// </summary>
        /// <param name="select_id"></param>
        /// <returns></returns>
        private bool CheckInfoDeleting(int select_id)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();

                string sql = "Select count(*) from Timing,Movie where Hall_ID=" +
                    select_id + 
                    " and Timing.Movie_ID=Movie.Movie_ID and convert (varchar, dateadd(minute,Movie_Time,BeginTime), 120)>='" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                if (Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败！" + ex.Message);
                return true;
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
        /// 修改冲突的检测
        /// </summary>
        /// <param name="ID_Selected"></param>
        /// <returns></returns>
        private int CheckInfoUpdating(int ID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);

            //打开数据库，进行验证连接
            try
            {
                string sqlsel1 = "SELECT COUNT(*) FROM Timing,[Order],Hall" +
                    " WHERE Timing.Hall_ID=Hall.Hall_ID AND" +
                    " Timing.Timing_ID=[Order].Timing_ID AND" +
                    " Condition='未使用' AND" +
                    " Timing_isActive=1 AND" +
                    " Hall.Hall_ID=" + ID_Selected;
                string sqlsel2 = "SELECT COUNT(*) FROM Timing,[Order],Hall" +
                    " WHERE Timing.Hall_ID=Hall.Hall_ID AND" +
                    " Timing.Timing_ID=[Order].Timing_ID AND" +
                     " Hall.Hall_ID=" + ID_Selected;
                sqlConnection.Open();
                SqlCommand sqlCommand2 = new SqlCommand(sqlsel2, sqlConnection);
                sqlCommand2.ExecuteNonQuery();

                if (Convert.ToInt32(sqlCommand2.ExecuteScalar())>0)
                {
                    SqlCommand sqlCommand1 = new SqlCommand(sqlsel1, sqlConnection);
                    sqlCommand1.ExecuteNonQuery();

                    //如果执行成功，则返回1，表示可以修改；否则返回0，表示修改失败
                    if (Convert.ToInt32(sqlCommand1.ExecuteScalar()) > 0)
                        return 1;
                    else
                        return 0;
                }
                //表示并没有排片
                else
                {
                    return 3;
                }
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

                string SQL_Searching = "SELECT Hall_ID,Hall_Row,Hall_Column FROM dbo.Hall WHERE Hall_isActive=1" +
                     " AND Hall_ID=" + ID_Selected;
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
