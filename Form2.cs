using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PublicLib;
using Model;
using System.Configuration;

namespace hh
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SkinFile = Application.StartupPath + "//" + ConfigurationManager.AppSettings["skin"] + ".ssk";

            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.DialogResult != DialogResult.OK)
            {
                DialogResult result = MessageBox.Show("确定要退出吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Customer.CancelLogin(Form1.c.ID);

                    Dispose();
                    System.Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
           
        }

        private void 预订影票ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void 我的订单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form5 f = new Form5();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void 会员办理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 f = new Form6();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(f);
            //panel控件内显示窗体
            f.Show();
        }

        private void 注销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要注销登录吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
                Customer.CancelLogin(Form1.c.ID);
                Form1 f = new Form1();
                f.Show();
                this.Close();
            }
        }
    }
}
