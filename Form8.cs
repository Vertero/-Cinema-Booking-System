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
using Model;

namespace hh
{
    /// <summary>
    /// 管理员管理界面
    /// </summary>
    public partial class Form8 : Form
    {
        private string aID;

        public Form8()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }


        /// <summary>
        /// 显示当前时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString();
        }


        /// <summary>
        /// 在窗体8中加载时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }


        /// <summary>
        /// 影厅管理跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Form9 f = new Form9();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 影片管理跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form10 f = new Form10();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 订单管理跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Form12 f = new Form12();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 排片管理跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Form11 f = new Form11();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 用户管理跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Form13 f = new Form13();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 统计信息跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            Form14 f = new Form14();
            f.Show();
            this.Hide();
        }


        /// <summary>
        /// 注销管理员用户按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要注销登录吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogResult = DialogResult.OK;

                //连接数据库，设置数据库中管理员的登录状态为0
                aID = Form7.a.ID;
                Admin.CancelLogin(aID);
                

                Form7 f = new Form7();
                this.Close();
                f.Show();
            }
        }


        /// <summary>
        /// 窗体退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form8_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
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


    }
}
