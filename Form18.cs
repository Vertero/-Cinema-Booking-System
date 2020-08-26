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
    public partial class Form18 : Form
    {
        public Form18()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form18_Load(object sender, EventArgs e)
        {
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());
            Update1();
        }

        private void Update1()
        {
            int n = getMovieNum();
            bool b = false;
            double d = 0;
            Dictionary<string, double> dic = new Dictionary<string, double>();
            for (int i = 1; i <= n; i++)
            {
                b = CheckMovieTiming(i);
                if (b == true)
                {
                    d = CalculateOccupancy(i);
                    dic.Add(i.ToString(),d);
                }
            }
            var dicSort = from objDic in dic orderby objDic.Value descending select objDic;
            int ii=0;
            DataGridViewTextBoxColumn dg = new DataGridViewTextBoxColumn();
            dg.HeaderText = "影片编号";
            dataGridView1.Columns.Add(dg);
            DataGridViewTextBoxColumn dg1 = new DataGridViewTextBoxColumn();
            dg1.HeaderText = "影片名称";
            dataGridView1.Columns.Add(dg1);
            DataGridViewTextBoxColumn dg2 = new DataGridViewTextBoxColumn();
            dg2.HeaderText = "上座率（%）";
            dataGridView1.Columns.Add(dg2);
            DataGridViewTextBoxColumn dg3 = new DataGridViewTextBoxColumn();
            dg3.HeaderText = "排名";
            dataGridView1.Columns.Add(dg3);
            foreach (var item in dicSort)
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dataGridView1);
                //添加的行作为第一行
                dataGridView1.Rows.Add(dr);
			    dataGridView1.Rows[ii].Cells[0].Value=item.Key;
                dataGridView1.Rows[ii].Cells[1].Value=Movie.FindNameBymID(item.Key);
                dataGridView1.Rows[ii].Cells[2].Value=item.Value;
                dataGridView1.Rows[ii].Cells[3].Value=ii+1;
                ii = ii + 1;
            }
            for (int i = 0; i < 4; i++)
            {
                dataGridView1.Columns[i].ReadOnly = true;
            }
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private int getMovieNum()
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sqlSel = "select max(Movie_ID) from Movie";
                SqlCommand cmd = new SqlCommand(sqlSel, conn);
                return Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！！" + ex.Message);
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

        private bool CheckMovieTiming(int MovieID)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sql = "select count(*) from Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Movie.Movie_ID=" + MovieID + " and Movie.Movie_isActive=1";
                SqlCommand cmd = new SqlCommand(sql, conn);
                int i = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                if (i > 0)
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

        private double CalculateOccupancy(int MovieID)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                string sql1 = "select sum(Hall_Row*Hall_Column) from Hall,Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Hall_ID=Hall.Hall_ID and Movie.Movie_isActive=1 and Movie.Movie_ID=" + MovieID;
                SqlCommand cmd1 = new SqlCommand(sql1, conn);
                double i = Convert.ToDouble(cmd1.ExecuteScalar().ToString());
                string sql2 = "select count(*) from [Order],Timing,Movie where Timing.Movie_ID=Movie.Movie_ID and Timing.Timing_ID=[Order].Timing_ID and Movie.Movie_isActive=1 and Movie.Movie_ID=" + MovieID;
                SqlCommand cmd2 = new SqlCommand(sql2, conn);
                double ii = Convert.ToDouble(cmd2.ExecuteScalar().ToString());
                return (ii / i*100);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误2！" + ex.Message);
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
    }
}
