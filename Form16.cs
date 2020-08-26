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
    public partial class Form16 : Form
    {
        public Form16()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form16_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
            Update1();
        }

        private void Update1()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sqlSel = "select Movie.Movie_ID, Movie_Name, sum(Order_Price), rank() over(order by sum(Order_Price) desc) as rank from [Order],Timing,Movie where [Order].Timing_ID=Timing.Timing_ID and Timing.Movie_ID=Movie.Movie_ID and Movie.Movie_isActive=1 group by Movie.Movie_ID,Movie.Movie_Name order by rank";
                SqlDataAdapter sda = new SqlDataAdapter(sqlSel, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[0].HeaderText = "影片编号";
                dataGridView1.Columns[1].HeaderText = "影片名称";
                dataGridView1.Columns[2].HeaderText = "票房";
                dataGridView1.Columns[3].HeaderText = "排名";
                for (int i = 0; i < 3; i++)
                {
                    dataGridView1.Columns[i].ReadOnly = true;
                }
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
    }
}
