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
    /// 影片管理
    /// </summary>
    public partial class Form10 : Form
    {
        private string aID;

        public Form10()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form10_Load);
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
        private void Form10_FormClosing(object sender, FormClosingEventArgs e)
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
            //定义文本框读取初始值

            string MovieName = "";
            int MovieTime = 0;
            string MovieActor = "";
            string MovieDirector = "";
            string MovieLanguage = "";
            string MovieType = "";
            string MovieIntro = "";
            string MoviePoster = "";
            string ReleaseDate = "";
            string DownDate = "";
            int MovieisActive = 1;
            
            MovieName = textBox2.Text.Trim();
            MovieTime = Convert.ToInt32(textBox3.Text.Trim());
            MovieActor = textBox4.Text.Trim();
            MovieDirector = textBox5.Text.Trim();
            MovieLanguage = textBox6.Text.Trim();
            MovieType = textBox7.Text.Trim();
            MovieIntro = textBox8.Text.Trim();
            MoviePoster = textBox9.Text.Trim();
            ReleaseDate = textBox10.Text.Trim();
            DownDate = textBox11.Text.Trim();

            int x = CheckInfoAdding(ReleaseDate,DownDate);
            if (x == 0)
                MessageBox.Show("上映日期不能早于当前日期！", "提示");
            else if (x == 1)
                MessageBox.Show("下架日期设置有误！若上映日期为空，不能设置下架时间；若上映日期不为空，下架日期应晚于上映日期。", "提示");
            else
            {
                //连接数据库，尝试添加数据
                string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(ConStr);
                try
                {
                    sqlConnection.Open();
                    string SQL_Adding = "INSERT INTO dbo.Movie VALUES( (SELECT MAX(Movie_ID)+1 FROM dbo.Movie),'" + MovieName + "',"
                        + MovieTime + ",'"
                        + MovieActor + "','"
                        + MovieDirector + "','"
                        + MovieLanguage + "','"
                        + MovieType + "','"
                        + MovieIntro + "','"
                        + MoviePoster + "','"
                        + ReleaseDate + "','"
                        + DownDate + "',"
                        + MovieisActive + ")";
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
                    textBox6.Text = null;
                    textBox7.Text = null;
                    textBox8.Text = null;
                    textBox9.Text = null;
                    textBox10.Text = null;
                    textBox11.Text = null;

                    //让光标在第一个文本框
                    textBox1.Focus();
                }

                //刷新datagridview界面
                string sqlSel1 = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1";
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
                if (b == true)
                {
                    string SQL_Deleting = "UPDATE dbo.Movie SET Movie_isActive=0 WHERE Movie_ID=" + select_id;  //采用逻辑删来代替物理删除
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
                textBox2.Focus();
            }
            finally
            {
                sqlConnection.Close();   //关闭数据库
                sqlConnection.Dispose();  //释放资源
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1";
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
            int MovieID_Selected = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());

            //选取文本框的值
            string MovieName = textBox2.Text.Trim();
            int MovieTime = 0;
            string MovieActor = textBox4.Text.Trim();
            string MovieDirector = textBox5.Text.Trim();
            string MovieLanguage = textBox6.Text.Trim();
            string MovieType = textBox7.Text.Trim();
            string MovieIntro = textBox8.Text.Trim();
            string MoviePoster = textBox9.Text.Trim();
            string ReleaseDate = textBox10.Text.Trim();
            string DownDate = textBox11.Text.Trim();
            int MovieisActive = 1;

            //如果修改框中有NULL值，那么采用原数值来代替
            if (textBox3.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                MovieTime = Convert.ToInt32(test1);
            }
            else
            {
                MovieTime = Convert.ToInt32(textBox3.Text.Trim());
            }
            if (MovieName.Trim() == "") { MovieName = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(); }
            if (MovieActor.Trim() == "") { MovieActor = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(); }
            if (MovieDirector.Trim() == "") { MovieDirector = dataGridView1.SelectedRows[0].Cells[4].Value.ToString(); }
            if (MovieLanguage.Trim() == "") { MovieLanguage = dataGridView1.SelectedRows[0].Cells[5].Value.ToString(); }
            if (MovieType.Trim() == "") { MovieType = dataGridView1.SelectedRows[0].Cells[6].Value.ToString(); }
            if (MovieIntro.Trim() == "") { MovieIntro = dataGridView1.SelectedRows[0].Cells[7].Value.ToString(); }
            if (MoviePoster.Trim() == "") { MoviePoster = dataGridView1.SelectedRows[0].Cells[8].Value.ToString(); }
            if (ReleaseDate.Trim() == "") { ReleaseDate = dataGridView1.SelectedRows[0].Cells[9].Value.ToString(); }
            if (DownDate.Trim() == "") { DownDate = dataGridView1.SelectedRows[0].Cells[10].Value.ToString(); }

            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();
                string SQL_Updating = "UPDATE dbo.Movie SET Movie_Name='" 
                    + MovieName + "',Movie_Time=" 
                    + MovieTime + ",Movie_Actor='" 
                    + MovieActor + "',Movie_Director='" 
                    + MovieDirector + "',Movie_Language='" 
                    + MovieLanguage + "',Movie_Type='" 
                    + MovieType + "',Movie_Intro='" 
                    + MovieIntro + "',Movie_Poster='" 
                    + MoviePoster + "',ReleaseDate='" 
                    + ReleaseDate + "',DownDate='" 
                    + DownDate + "',Movie_isActive=" 
                    + MovieisActive +" WHERE Movie_ID=" + MovieID_Selected;
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
                textBox7.Text = null;
                textBox8.Text = null;
                textBox9.Text = null;
                textBox10.Text = null;
                textBox11.Text = null;
                textBox2.Focus();
            }

            //刷新datagridview界面
            string sqlSel1 = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1";
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
            int MovieID = 0;
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("电影编号不能为空", "提示");
            }
            else
            {  MovieID = Convert.ToInt32(textBox1.Text.Trim()); }

            //连接数据库，尝试去搜索
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            try
            {
                sqlConnection.Open();

                int b = CheckInfoSearching(MovieID);

                if (b == 1)
                {
                    string SQL_Searching = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1 AND Movie_ID=" + MovieID;
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
            string sqlSel1 = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1";
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
        private void Form10_Load(object sender, EventArgs e)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //刷新datagridview界面
            string sqlSel1 = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate FROM dbo.Movie WHERE Movie_isActive=1";
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
            dataGridView1.Columns[0].HeaderText = "电影编号";
            dataGridView1.Columns[1].HeaderText = "电影名字";
            dataGridView1.Columns[2].HeaderText = "电影时长";
            dataGridView1.Columns[3].HeaderText = "电影主演";
            dataGridView1.Columns[4].HeaderText = "电影导演";
            dataGridView1.Columns[5].HeaderText = "电影语言";
            dataGridView1.Columns[6].HeaderText = "电影类型";
            dataGridView1.Columns[7].HeaderText = "电影简介";
            dataGridView1.Columns[8].HeaderText = "电影海报";
            dataGridView1.Columns[9].HeaderText = "上映时间";
            dataGridView1.Columns[10].HeaderText = "下架时间";

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
                string sql = "Select count(*) from Timing,Movie where Movie.Movie_ID=" 
                    + select_id + " and Timing.Movie_ID=Movie.Movie_ID" +
                    " and convert (varchar, dateadd(minute,Movie_Time,BeginTime), 120)>='" + 
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
        /// 增加记录冲突检测
        /// </summary>
        /// <param name="ReleaseDate"></param>
        /// <param name="DownDate"></param>
        /// <returns></returns>
        private int CheckInfoAdding(string ReleaseDate, string DownDate)
        {
            if(ReleaseDate.Trim()==""&&DownDate.Trim()=="")
                return 2;
            else if (ReleaseDate.Trim() == "" && DownDate.Trim() != "")
                return 1;
            else
            {
                string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                SqlConnection sqlConnection = null;
                try
                {
                    sqlConnection = new SqlConnection(ConStr);
                    //打开数据库
                    sqlConnection.Open();
                    string sql = "Select datediff(day,(select convert(date,getdate(),23)),convert(date,'" + ReleaseDate + "'))";
                    SqlCommand sqlCommand1 = new SqlCommand(sql, sqlConnection);
                    if (Convert.ToInt32(sqlCommand1.ExecuteScalar()) <= 0)
                        return 0;
                    else
                    {
                        if (DownDate.Trim() == "")
                            return 2;
                        else
                        {
                            string sqll = "Select datediff(day,convert(date,'" + ReleaseDate + "'),convert(date,'" + DownDate + "'))";
                            SqlCommand sqlCommand2 = new SqlCommand(sqll, sqlConnection);
                            if (Convert.ToInt32(sqlCommand2.ExecuteScalar()) > 0)
                                return 2;
                            else
                                return 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询失败！" + ex.Message);
                    return -1;
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

                string SQL_Searching = "SELECT Movie_ID, Movie_Name, Movie_Time, Movie_Actor, Movie_Director, Movie_Language, Movie_Type, Movie_Intro, Movie_Poster, ReleaseDate, DownDate" +
                    " FROM dbo.Movie WHERE Movie_isActive=1 AND Movie_ID=" + ID_Selected;
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
