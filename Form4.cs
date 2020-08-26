using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using PublicLib;
using Model;
using System.Configuration;

namespace hh
{
    public partial class Form4 : Form
    {
        private Movie m = new Movie();
        private Timing t = new Timing();
        private string mID, sDate;
        private double discount;
        public Form4()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            timer1.Start();
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
            this.panel1.AutoScrollMinSize = this.panel1.Size;
            string sqlSel = "select * from Movie where (DownDate is null or datediff(day,(select convert(date,getdate(),23)),DownDate)>0) and ReleaseDate is not null and datediff(day,(select convert(date,getdate(),23)),ReleaseDate)<=0 and Movie_isActive=1";
            SqlCommand command =new SqlCommand(sqlSel, conn);
            SqlDataReader dr = command.ExecuteReader();
            int i=0;
            while (dr.Read())
            {
                i++;
                m.ID = dr["Movie_ID"].ToString();
                m.Name = dr["Movie_Name"].ToString();
                m.Poster = dr["Movie_Poster"].ToString();
                m.Director = dr["Movie_Director"].ToString();
                m.Actor = dr["Movie_Actor"].ToString();
                m.Time = dr["Movie_Time"].ToString();
                m.Language = dr["Movie_Language"].ToString();
                m.Type = dr["Movie_Type"].ToString();
                m.Intro = dr["Movie_Intro"].ToString();
                m.ReleaseDate = dr["ReleaseDate"].ToString();
                if (i == 1)
                {
                    mID = m.ID;
                    label2.Text = m.Name;
                    pictureBox1.Image = Image.FromFile(m.Poster);
                    label4.Text = m.Director;
                    label6.Text = m.Type;
                    label8.Text = m.Time + "分钟";
                    label10.Text = m.Language;
                    label12.Text = m.ReleaseDate;
                    label14.Text = m.Actor;
                    label16.Text = m.Intro;
                }
                PictureBox pb = new PictureBox();
                pb.Name = m.ID;
                pb.Size = new System.Drawing.Size(120,180);
                pb.Top = 14+200*(i-1);
                pb.Left = 8;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Click += ShowMovieInfor;
                pb.Click += UpdateTimingByPb;
                pb.Image = Image.FromFile(m.Poster);
                this.panel1.Controls.Add(pb);
            }
            dr.Close();
            conn.Close();
            label18.Text = i.ToString().PadLeft(2,'0');
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.ValueChanged += UpdateTimingByDtp;
            sDate = this.dateTimePicker1.Value.ToString("yyyy/MM/dd");
            GetTiming(mID, sDate); 
        }

        private void ShowMovieInfor(object sender, EventArgs e)
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
            PictureBox pb = sender as PictureBox;
            string sqlSel = "select * from Movie where Movie_ID="+int.Parse(pb.Name);
            SqlCommand command = new SqlCommand(sqlSel, conn);
            SqlDataReader dr = command.ExecuteReader();
            dr.Read();
            label2.Text = dr["Movie_Name"].ToString();
            pictureBox1.Image = Image.FromFile(dr["Movie_Poster"].ToString());
            label4.Text = dr["Movie_Director"].ToString();
            label6.Text = dr["Movie_Type"].ToString();
            label8.Text = dr["Movie_Time"].ToString() + "分钟";
            label10.Text = dr["Movie_Language"].ToString();
            label12.Text = dr["ReleaseDate"].ToString();
            label14.Text = dr["Movie_Actor"].ToString();
            label16.Text = dr["Movie_Intro"].ToString();
            dr.Close();
            conn.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label17.Text = "当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void GetTiming(string mID,string sDate)
        {
            discount = Class.FindDiscountbyClass(1);
            Timing.UpdateTimingStatus();
            string ConStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConStr);
                //打开数据库
                conn.Open();
                String sqlSel = "select Timing_ID," +
                    "convert(varchar, BeginTime, 120)," +
                    "convert(varchar, dateadd(minute,Movie_Time,BeginTime), 120) as EndTime," +
                    "Hall_ID,Timing_Price,Timing_Price*" + discount + 
                    " as VIP_Price from Timing,Movie" +
                    " where Timing.Movie_ID=Movie.Movie_ID and " +
                    "(select convert(varchar(12),BeginTime, 111)) = '" + 
                    sDate + "' and Timing.Movie_ID = " + 
                    int.Parse(mID) + "and Timing_isActive=1 order by BeginTime";
                SqlDataAdapter sda = new SqlDataAdapter(sqlSel, conn);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[0].HeaderText = "排片编号";
                dataGridView1.Columns[1].HeaderText = "开始时间";
                dataGridView1.Columns[2].HeaderText = "结束时间";
                dataGridView1.Columns[3].HeaderText = "影厅";
                dataGridView1.Columns[4].HeaderText = "价格";
                dataGridView1.Columns[5].HeaderText = "VIP优惠价";
                //设置数据表格为只读
                dataGridView1.ReadOnly = true;
                //不允许添加行
                dataGridView1.AllowUserToAddRows = false;
                //背景为白色
                dataGridView1.BackgroundColor = Color.White;
                //只允许选中单行
                dataGridView1.MultiSelect = false;
                //整行选中
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message, "提示");
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

        private void UpdateTimingByPb(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            mID = pb.Name;
            GetTiming(mID, sDate);
        }

        private void UpdateTimingByDtp(object sender, EventArgs e)
        {
            DateTimePicker dtp = sender as DateTimePicker;
            sDate = dtp.Value.ToString("yyyy/MM/dd");
            GetTiming(mID, sDate);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("您没有选择任何放映时间！", "提示");
            }
            else
            {
                BuyTickets bt = new BuyTickets();
                bt.mName = Movie.FindNameBymID(mID);
                bt.discount = discount;
                bt.tID = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                bt.begintime = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                bt.endtime = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                bt.hID = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                bt.price = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                bt.vipprice = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
                bt.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
            string sqlSel = "select count(*) from Movie where Movie_isActive=1 and Movie_ID=" + mID;
            SqlCommand command = new SqlCommand(sqlSel, conn);
            int valid = Convert.ToInt32(command.ExecuteScalar());
            conn.Close();
            UpdateMovie(valid);
        }

        private void UpdateMovie(int valid)
        {
            this.panel1.Controls.Clear();
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
            this.panel1.AutoScrollMinSize = this.panel1.Size;
            string sqlSel = "select * from Movie where (DownDate is null or datediff(day,(select convert(date,getdate(),23)),DownDate)>0) and ReleaseDate is not null and datediff(day,(select convert(date,getdate(),23)),ReleaseDate)<=0 and Movie_isActive=1";
            SqlCommand command = new SqlCommand(sqlSel, conn);
            SqlDataReader dr = command.ExecuteReader();
            int i = 0;
            while (dr.Read())
            {
                i++;
                m.ID = dr["Movie_ID"].ToString();
                m.Name = dr["Movie_Name"].ToString();
                m.Poster = dr["Movie_Poster"].ToString();
                m.Director = dr["Movie_Director"].ToString();
                m.Actor = dr["Movie_Actor"].ToString();
                m.Time = dr["Movie_Time"].ToString();
                m.Language = dr["Movie_Language"].ToString();
                m.Type = dr["Movie_Type"].ToString();
                m.Intro = dr["Movie_Intro"].ToString();
                m.ReleaseDate = dr["ReleaseDate"].ToString();
                if(valid == 0 && i == 1)
                {
                    mID = m.ID;
                    label2.Text = m.Name;
                    pictureBox1.Image = Image.FromFile(m.Poster);
                    label4.Text = m.Director;
                    label6.Text = m.Type;
                    label8.Text = m.Time + "分钟";
                    label10.Text = m.Language;
                    label12.Text = m.ReleaseDate;
                    label14.Text = m.Actor;
                    label16.Text = m.Intro;
                }
                PictureBox pb = new PictureBox();
                pb.Name = m.ID;
                pb.Size = new System.Drawing.Size(120, 180);
                pb.Top = 14 + 200 * (i - 1);
                pb.Left = 8;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Click += ShowMovieInfor;
                pb.Click += UpdateTimingByPb;
                pb.Image = Image.FromFile(m.Poster);
                this.panel1.Controls.Add(pb);
            }
            dr.Close();
            conn.Close();
            label18.Text = i.ToString().PadLeft(2, '0');
            GetTiming(mID, sDate); 
        }
    }
}
