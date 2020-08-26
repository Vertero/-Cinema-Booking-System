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
    public partial class Form15 : Form
    {
        public Form15()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form15_Load(object sender, EventArgs e)
        {
            textBox2.Focus();
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            int MovieID = 0;
            string MovieName = "";

            //打开数据库，尝试去搜索值
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(ConStr);
            sqlConnection.Open();

            try
            {
                if (textBox2.Text.Trim() == "" && textBox1.Text.Trim() == "")
                {
                    MessageBox.Show("输入的电影编号和电影名字不能为空", "提示");
                    textBox2.Focus();
                }

                else if (textBox2.Text.Trim() != "" && textBox1.Text.Trim() == "")
                {
                    MovieID = Convert.ToInt32(textBox2.Text.Trim());
                    string SQL = "SELECT SUM(dbo.[Order].Order_Price)"
                        + " FROM dbo.Timing, dbo.[Order] ,dbo.Movie"
                        + " WHERE dbo.Timing.Movie_ID = dbo.Movie.Movie_ID"
                        + " AND dbo.Timing.Timing_ID = dbo.[Order].Timing_ID"
                        + " and Movie.Movie_isActive=1 AND dbo.Timing.Movie_ID = "
                        + MovieID;   //统计票房的总数
                    SqlCommand sqlCommand = new SqlCommand(SQL, sqlConnection);
                    string str = sqlCommand.ExecuteScalar().ToString();
                    if (str == "")
                    {
                        MessageBox.Show("没有找到相应的数据，请重新确定电影编号是否输入正确", "提示");
                        textBox2.Focus();
                    }
                    else
                    {
                        label7.Text = str + "元";
                    }
                }

                else if (textBox2.Text.Trim() == "" && textBox1.Text.Trim() != "")
                {
                    MovieName = textBox1.Text.Trim();
                    string SQL = "SELECT SUM(dbo.[Order].Order_Price)"
                              + " FROM dbo.Timing, dbo.[Order] ,dbo.Movie"
                              + " WHERE dbo.Timing.Movie_ID = dbo.Movie.Movie_ID"
                              + " AND dbo.Timing.Timing_ID = dbo.[Order].Timing_ID"
                              + " and Movie.Movie_isActive=1"
                              + " AND dbo.Movie.Movie_Name = '"
                              + MovieName + "'";
                    SqlCommand sqlCommand = new SqlCommand(SQL, sqlConnection);
                    string str = sqlCommand.ExecuteScalar().ToString();

                    //确定SQL语句是否能在表中找到相应的数据
                    if (str == "")
                    {
                        MessageBox.Show("没有找到相应的数据，请重新确定电影编号是否输入正确", "提示");
                        textBox2.Focus();
                    }
                    else
                    {
                        label7.Text = str;
                    }
                }

                else if (textBox2.Text.Trim() != "" && textBox1.Text.Trim() != "")
                {
                    MovieID = Convert.ToInt32(textBox2.Text.Trim());
                    MovieName = textBox1.Text.Trim();
                    string SQL = "SELECT SUM(dbo.[Order].Order_Price)"
                              + " FROM dbo.Timing, dbo.[Order] ,dbo.Movie"
                              + " WHERE dbo.Timing.Movie_ID = dbo.Movie.Movie_ID"
                              + " AND dbo.Timing.Timing_ID = dbo.[Order].Timing_ID"
                              + " and Movie.Movie_isActive=1"
                              + " AND dbo.Timing.Movie_ID = " +
                               MovieID + "AND dbo.Movie.Movie_Name = '"
                              + MovieName + "'";
                    SqlCommand sqlCommand = new SqlCommand(SQL, sqlConnection);
                    string str = sqlCommand.ExecuteScalar().ToString();

                    //确定SQL语句是否能在表中找到相应的数据
                    if (str == "")
                    {
                        MessageBox.Show("没有找到相应的数据，请重新确定电影编号是否输入正确", "提示");
                        textBox2.Focus();
                    }
                    else
                    {
                        label7.Text = str;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "\n电影编号或者电影名字输入有误，请重新输入", "提示");
            }

            finally
            {
                //清空文本框的内容
                textBox1.Text = "";
                textBox2.Text = "";

                //将光标聚焦在电影编号文本框
                textBox2.Focus();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("在电影ID或者电影名字的输入框中输入你想搜索的电影，并点击“查看”按钮即可查看票房\n点击“刷新”按钮可以讲电影ID、电影名字以及票房内容置空\n祝您生活愉快", "帮助");
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
    }
}
