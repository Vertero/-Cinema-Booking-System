namespace hh
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.预订影票ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.我的订单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.会员办理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.注销ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.预订影票ToolStripMenuItem,
            this.我的订单ToolStripMenuItem,
            this.会员办理ToolStripMenuItem,
            this.注销ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1180, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 预订影票ToolStripMenuItem
            // 
            this.预订影票ToolStripMenuItem.Name = "预订影票ToolStripMenuItem";
            this.预订影票ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.预订影票ToolStripMenuItem.Text = "预订影票";
            this.预订影票ToolStripMenuItem.Click += new System.EventHandler(this.预订影票ToolStripMenuItem_Click);
            // 
            // 我的订单ToolStripMenuItem
            // 
            this.我的订单ToolStripMenuItem.Name = "我的订单ToolStripMenuItem";
            this.我的订单ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.我的订单ToolStripMenuItem.Text = "我的订单";
            this.我的订单ToolStripMenuItem.Click += new System.EventHandler(this.我的订单ToolStripMenuItem_Click);
            // 
            // 会员办理ToolStripMenuItem
            // 
            this.会员办理ToolStripMenuItem.Name = "会员办理ToolStripMenuItem";
            this.会员办理ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.会员办理ToolStripMenuItem.Text = "会员办理";
            this.会员办理ToolStripMenuItem.Click += new System.EventHandler(this.会员办理ToolStripMenuItem_Click);
            // 
            // 注销ToolStripMenuItem
            // 
            this.注销ToolStripMenuItem.Name = "注销ToolStripMenuItem";
            this.注销ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.注销ToolStripMenuItem.Text = "注销";
            this.注销ToolStripMenuItem.Click += new System.EventHandler(this.注销ToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1180, 717);
            this.panel1.TabIndex = 1;
            // 
            // skinEngine1
            // 
            this.skinEngine1.@__DrawButtonFocusRectangle = true;
            this.skinEngine1.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine1.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine1.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinFile = null;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 746);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "顾客订票菜单";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 预订影票ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 我的订单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 会员办理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 注销ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;

    }
}