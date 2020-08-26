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
    public partial class Form14 : Form
    {
        private string aID;

        public Form14()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form14_Load(object sender, EventArgs e)
        {
            timer1.Start();
            this.Resize += new EventHandler(FormClass.FormResize);
            FormClass.setValues(this.Width, this.Height, this);
            FormClass.setTag(this);
            FormClass.FormResize(new object(), new EventArgs());

            Form15 f = new Form15();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //在窗体刷新中显示时间
            label1.Text = "当前时间：" + DateTime.Now.ToString();
        }

        private void Form14_FormClosing(object sender, FormClosingEventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            Form15 f = new Form15();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form16 f = new Form16();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form17 f = new Form17();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form18 f = new Form18();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form8 f = new Form8();
            f.Show();
            this.Hide();
        }
    }
}
