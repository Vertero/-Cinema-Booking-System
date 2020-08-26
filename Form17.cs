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
    public partial class Form17 : Form
    {
        public Form17()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private bool CheckMovieTiming(int MovieID, string MovieName)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                string sql="";
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                if (MovieID != 0)
                    sql = "select count(*) from Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Movie.Movie_ID=" + MovieID + " and Movie.Movie_isActive=1";
                else if (MovieName != "")
                    sql = "select count(*) from Timing,Movie where Movie_Name='" + MovieName + "' and Timing.Movie_ID=Movie.Movie_ID and Movie.Movie_isActive=1";
                else if (MovieID != 0 && MovieName != "")
                    sql = "select count(*) from Timing,Movie where Movie_Name='" + MovieName + "' and Movie.Movie_ID=" + MovieID + " and Timing.Movie_ID=Movie.Movie_ID and Movie.Movie_isActive=1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                int i = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                if (i>0)
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
                MessageBox.Show("查询错误1！" + ex.Message);
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

        private string CalculateOccupancy(int MovieID, string MovieName)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                if (MovieID != 0)
                {
                    string sql1 = "select sum(Hall_Row*Hall_Column) from Hall,Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Hall_ID=Hall.Hall_ID and Movie.Movie_isActive=1 and Movie.Movie_ID=" + MovieID;
                    SqlCommand cmd1 = new SqlCommand(sql1, conn);
                    double i = Convert.ToDouble(cmd1.ExecuteScalar().ToString());
                    string sql2 = "select count(*) from [Order],Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Timing_ID=[Order].Timing_ID and Movie.Movie_isActive=1 and Movie.Movie_ID=" + MovieID;
                    SqlCommand cmd2 = new SqlCommand(sql2, conn);
                    double ii = Convert.ToDouble(cmd2.ExecuteScalar().ToString());
                    return Convert.ToString(ii / i*100);
                }
                else if (MovieName != "")
                {
                    string sql1 = "select sum(Hall_Row*Hall_Column) from Hall,Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Hall_ID=Hall.Hall_ID and Movie.Movie_isActive=1 and Movie_Name='" + MovieName + "'";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn);
                    double i = Convert.ToDouble(cmd1.ExecuteScalar().ToString());
                    string sql2 = "select count(*) from [Order],Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Timing_ID=[Order].Timing_ID and Movie.Movie_isActive=1 and Movie_Name='" + MovieName + "'";
                    SqlCommand cmd2 = new SqlCommand(sql2, conn);
                    double ii = Convert.ToDouble(cmd2.ExecuteScalar().ToString());
                    return Convert.ToString(ii / i*100);
                }
                else
                {
                    string sql1 = "select sum(Hall_Row*Hall_Column) from Hall,Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Hall_ID=Hall.Hall_ID and Movie.Movie_isActive=1 and Movie.Movie_ID=" + MovieID + " and Movie_Name='" + MovieName + "'";
                    SqlCommand cmd1 = new SqlCommand(sql1, conn);
                    double i = Convert.ToDouble(cmd1.ExecuteScalar().ToString());
                    string sql2 = "select count(*) from [Order],Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Timing_ID=[Order].Timing_ID and Movie.Movie_isActive=1 and Timing.Movie_ID=" + MovieID + " and Movie_Name='" + MovieName + "'";
                    SqlCommand cmd2 = new SqlCommand(sql2, conn);
                    double ii = Convert.ToDouble(cmd2.ExecuteScalar().ToString());
                    return Convert.ToString(ii / i*100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误2！" + ex.Message);
                return "";
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

        private void button1_Click(object sender, EventArgs e)
        {
            int MovieID = 0;
            string MovieName = "";
            bool b=false;
            if (textBox2.Text.Trim() != "" && textBox1.Text.Trim() == "")
                MovieID = Convert.ToInt32(textBox2.Text.Trim());
            else if(textBox2.Text.Trim() == "" && textBox1.Text.Trim() != "")
                MovieName = textBox1.Text.Trim();
            else if(textBox2.Text.Trim() != "" && textBox1.Text.Trim() != "")
            {
                MovieID = Convert.ToInt32(textBox2.Text.Trim());
                MovieName = textBox1.Text.Trim();
            }
            
            if (textBox2.Text.Trim() == "" && textBox1.Text.Trim() == "")
            {
                MessageBox.Show("输入的电影编号和电影名字不能为空", "提示");
                textBox2.Focus();
            }
            else
            {
                b = CheckMovieTiming(MovieID, MovieName);
                if (b == false)
                {
                    MessageBox.Show("输入的电影编号/电影名字有误或该电影没有排片！", "提示");
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox2.Focus();
                }
                else
                {
                    string occupancy = CalculateOccupancy(MovieID, MovieName);
                    label7.Text = occupancy + "%";
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox2.Focus();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //重置文本框的值
            textBox1.Text = "";
            textBox2.Text = "";
            label7.Text = "";

            //光标聚焦
            textBox2.Focus();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("在电影ID或者电影名字的输入框中输入你想搜索的电影，并点击“查看”按钮即可查看上座率\n点击“刷新”按钮可以讲电影ID、电影名字以及上座率内容置空\n祝您生活愉快", "帮助");
        }

        private void Form17_Load(object sender, EventArgs e)
        {
            textBox2.Focus();
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
        }
    }
}
