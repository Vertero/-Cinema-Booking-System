using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PublicLib;
using System.Configuration;

namespace hh
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public partial class Form13 : Form
    {
        private string aID;

        public Form13()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form13_Load);
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }


        /// <summary>
        /// 返回键的按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurningBack(object sender, EventArgs e)
        {
            Form8 f = new Form8();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoAdding(object sender, EventArgs e)
        {
            //读取文本框的值
            string ID = "";
            string Password = "";
            string Name = "";
            string Contact = "";
            int Class = 0;  //如果使用string类型，那么SQL语句会报错


            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "" && textBox3.Text.Trim() != "" && textBox4.Text.Trim() != "" && (Convert.ToInt32(textBox5.Text.Trim()) == 0 || Convert.ToInt32(textBox5.Text.Trim()) == 1))
            {
                //读取文本框的值
                ID = textBox1.Text.Trim();
                Password = textBox2.Text.Trim();
                Name = textBox3.Text.Trim();
                Contact = textBox4.Text.Trim();
                Class = Convert.ToInt32(textBox5.Text.Trim());  //如果使用string类型，那么SQL语句会报错
            }
            else
            {
                MessageBox.Show("输入框中不能有空数值");
            }

            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(ConStr);
            try
            {
                sqlCon.Open();
                string sqlSel = "INSERT INTO dbo.Customer VALUES ('" + ID + "','" + Password + "','" + Name + "','" + Contact + "'," + Class + ",1,0)";
                SqlCommand cmd = new SqlCommand(sqlSel, sqlCon);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n插入失败", "提示");
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

                //让光标在第一个文本框
                textBox1.Focus();
            }


            //刷新datagridview界面
            string sqlSell = "SELECT Customer_ID," +
                "Customer_Password," +
                "Customer_Name," +
                "Customer_Contact," +
                "Customer_Class," +
                "Customer_LoginState" +
                " FROM dbo.Customer WHERE Customer_isActive = 1";
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();  //打开数据库进行连接
            SqlCommand sc = new SqlCommand(sqlSell, con);
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
            SqlConnection conn = new SqlConnection(ConStr);
            try
            {
                conn.Open();
                //选择的当前行第一列的值，也就是ID
                string select_id = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                int b = LoginStateJudgement(select_id);

                if (b == 2)
                {
                    //表示SQL中的删除语句，采用逻辑删除
                    string delete_SQL_id = "UPDATE dbo.Customer SET Customer_isActive=0 WHERE Customer_ID='" + select_id + "'";
                    SqlCommand cmd = new SqlCommand(delete_SQL_id, conn);
                    cmd.ExecuteNonQuery();
                }
                else if (b == 1) 
                {
                    MessageBox.Show("登录状态下不可随意删除账户", "提示");
                }
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "请检查是否选中目标");
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            SqlConnection connn = new SqlConnection(ConStr);
            connn.Open();
            string sqlSel = "SELECT Customer_ID," +
                "Customer_Password," +
                "Customer_Name," +
                "Customer_Contact," +
                "Customer_Class," +
                "Customer_LoginState" +
                " FROM dbo.Customer WHERE Customer_isActive = 1"; 
            SqlCommand sc = new SqlCommand(sqlSel, connn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            connn.Close();
            connn.Dispose();

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
            //连接数据库
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlCon = new SqlConnection(ConStr);
            //选定原来的值
            string ID_Selected = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

            //读取文本框的值
            string Password = textBox2.Text.Trim();
            string Name = textBox3.Text.Trim();
            string Contact = textBox4.Text.Trim();
            int Class = 0;  //如果使用string类型，那么SQL语句会报错

            if (textBox5.Text.Trim() == "")
            {
                string test1 = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                Class = Convert.ToInt32(test1);
            }
            else
            {
                Class = Convert.ToInt32(textBox5.Text.Trim());
            }

            //在修改前先验证是否是我们需要的修改值
            if (Password.Trim() == "") { Password = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(); }
            if (Name.Trim() == "") { Name = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(); }
            if (Contact.Trim() == "") { Contact = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(); }

            try
            {
                sqlCon.Open();
                int b = LoginStateJudgement(ID_Selected);

                if (b==2)
                {
                    string SQL_Update = "UPDATE dbo.Customer SET " +
                    "Customer_Password='" + Password +
                    "',Customer_Name='" + Name +
                    "',Customer_Contact='" + Contact +
                    "', Customer_Class=" + Class +
                    " WHERE Customer_ID='" + ID_Selected + "'";

                    SqlCommand cmd = new SqlCommand(SQL_Update, sqlCon);
                    cmd.ExecuteNonQuery();
                }
                else if (b==1)
                {
                    MessageBox.Show("不可在登录状态下随意修改用户信息", "提示");
                }               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n请检查输入是否有误");
            }
            finally
            {
                sqlCon.Close();   //将数据库关闭
                sqlCon.Dispose();   //释放资源

                //清理文本框中的内容
                textBox2.Text = null;
                textBox3.Text = null;
                textBox4.Text = null;
                textBox5.Text = null;
                textBox2.Focus();
            }

            SqlConnection conn = new SqlConnection(ConStr);
            conn.Open();
            string sqlSel = "SELECT Customer_ID," +
                "Customer_Password," +
                "Customer_Name," +
                "Customer_Contact," +
                "Customer_Class," +
                "Customer_LoginState" +
                " FROM dbo.Customer WHERE Customer_isActive = 1";
            SqlCommand sc = new SqlCommand(sqlSel, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
            conn.Dispose();

            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;
        }


        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoSearching(object sender, EventArgs e)
        {
            string ID = textBox1.Text.Trim();
            if (ID == "")
            {
                MessageBox.Show("用户名不能为空，请重新输入","提示");
            }
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

            //连接数据库，尝试去搜索
            SqlConnection conSql = new SqlConnection(ConStr);
            try
            {
                conSql.Open();
                int b = CheckInfoSearching(ID);

                if (b == 1)
                {
                    string SQL_Search = "SELECT Customer_ID," +
                                         "Customer_Password," +
                                             "Customer_Name," +
                                          "Customer_Contact," +
                                            "Customer_Class," +
                                        "Customer_LoginState" +
                                         " FROM dbo.Customer" +
                                        " WHERE Customer_isActive = 1 AND Customer_ID='" + ID + "'";
                    SqlCommand cmd = new SqlCommand(SQL_Search, conSql);
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();  //通过SQLDataReader来Binding找到的数值
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
                MessageBox.Show(ex.Message.ToString() + "\n请查看查询的输入是否有错");
            }
            finally
            {
                conSql.Close();   //关闭数据库
                conSql.Dispose();   //释放资源

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

            SqlConnection conn = new SqlConnection(ConStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "打开数据库失败");
            }
            string sqlSel = "SELECT Customer_ID," +
                "Customer_Password," +
                "Customer_Name," +
                "Customer_Contact," +
                "Customer_Class," +
                "Customer_LoginState" +
                " FROM dbo.Customer WHERE Customer_isActive = 1";
            SqlCommand sc = new SqlCommand(sqlSel, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
            conn.Dispose();

            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;
        }


        /// <summary>
        /// 滚动刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            dataGridView1.Invalidate();
        }


        /// <summary>
        /// 窗体加载函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form13_Load(object sender, EventArgs e)
        {
            //用于最开始打开数据库就可以获取数据库的信息
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConStr);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "打开数据库失败");
            }
            string sqlSel = "SELECT Customer_ID," +
                "Customer_Password," +
                "Customer_Name," +
                "Customer_Contact," +
                "Customer_Class," +
                "Customer_LoginState" +
                " FROM dbo.Customer WHERE Customer_isActive = 1";   //找出除了Customer_isActive外的所有属性的元组
            SqlCommand sc = new SqlCommand(sqlSel, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            this.dataGridView1.DataSource = ds.Tables[0].DefaultView;
            conn.Close();
            conn.Dispose();

            //在加载出datagridview后之后可以自动选定第一行
            this.dataGridView1.Rows[0].Selected = true;

            //在形成datagridview之后给生成的列进行编号
            dataGridView1.Columns[0].HeaderText = "用户ID";
            dataGridView1.Columns[1].HeaderText = "密码";
            dataGridView1.Columns[2].HeaderText = "姓名";
            dataGridView1.Columns[3].HeaderText = "联系方式";
            dataGridView1.Columns[4].HeaderText = "VIP等级";
            dataGridView1.Columns[5].HeaderText = "登录状态";

            //让窗体中控件可以跟随着窗体大小变化而改变大小
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());

            //根据数据内容自动调整列宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }


        /// <summary>
        /// 退出提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form13_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 登录状态判定
        /// </summary>
        /// <param name="ID_Selected"></param>
        /// <returns></returns>
        private int LoginStateJudgement (string ID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();

                string sqlsel = "SELECT COUNT(*) FROM dbo.Customer" +
                    " WHERE Customer_LoginState=1" +
                    " AND Customer_ID='" + ID_Selected + "'";
                SqlCommand sqlCommand = new SqlCommand(sqlsel, sqlConnection);
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


        /// <summary>
        /// 查询的错误判断
        /// </summary>
        /// <param name="ID_Selected"></param>
        /// <returns></returns>
        private int CheckInfoSearching (string ID_Selected)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(ConStr);
                //打开数据库
                sqlConnection.Open();

                string sqlsel = "SELECT COUNT(*) FROM dbo.Customer" +
                    " WHERE Customer_isActive=1" +
                    " AND Customer_ID='" + ID_Selected + "'";
                SqlCommand sqlCommand = new SqlCommand(sqlsel, sqlConnection);
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

