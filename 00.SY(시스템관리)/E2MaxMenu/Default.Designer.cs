using System;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;


namespace E2MAXMenu
{
    partial class Default
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Default));
            this.E2MaxIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.e2Max실행ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.리스트ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.환경설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.종료ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // E2MaxIcon
            // 
            this.E2MaxIcon.ContextMenuStrip = this.ContextMenu;
            this.E2MaxIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("E2MaxIcon.Icon")));
            this.E2MaxIcon.Text = "E2Max-MTMS";
            this.E2MaxIcon.Visible = true;
            this.E2MaxIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.E2MaxIcon_MouseDoubleClick);
            // 
            // ContextMenu
            // 
            this.ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.e2Max실행ToolStripMenuItem,
            this.리스트ToolStripMenuItem,
            this.환경설정ToolStripMenuItem,
            this.종료ToolStripMenuItem});
            this.ContextMenu.Name = "ContextMenu";
            this.ContextMenu.Size = new System.Drawing.Size(135, 92);
            // 
            // e2Max실행ToolStripMenuItem
            // 
            this.e2Max실행ToolStripMenuItem.Name = "e2Max실행ToolStripMenuItem";
            this.e2Max실행ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.e2Max실행ToolStripMenuItem.Text = "E2Max실행";
            this.e2Max실행ToolStripMenuItem.Click += new System.EventHandler(this.e2Max실행ToolStripMenuItem_Click);
            // 
            // 리스트ToolStripMenuItem
            // 
            this.리스트ToolStripMenuItem.Name = "리스트ToolStripMenuItem";
            this.리스트ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.리스트ToolStripMenuItem.Text = "리스트";
            this.리스트ToolStripMenuItem.Click += new System.EventHandler(this.리스트ToolStripMenuItem_Click);
            // 
            // 환경설정ToolStripMenuItem
            // 
            this.환경설정ToolStripMenuItem.Name = "환경설정ToolStripMenuItem";
            this.환경설정ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.환경설정ToolStripMenuItem.Text = "환경설정";
            this.환경설정ToolStripMenuItem.Click += new System.EventHandler(this.환경설정ToolStripMenuItem_Click);
            // 
            // 종료ToolStripMenuItem
            // 
            this.종료ToolStripMenuItem.Name = "종료ToolStripMenuItem";
            this.종료ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.종료ToolStripMenuItem.Text = "종료";
            this.종료ToolStripMenuItem.Click += new System.EventHandler(this.종료ToolStripMenuItem_Click);
            // 
            // Default
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(326, 170);
            this.ControlBox = false;
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Default";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Default_Load);
            this.ContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon E2MaxIcon;
        private System.Windows.Forms.ContextMenuStrip ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem e2Max실행ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 리스트ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 환경설정ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 종료ToolStripMenuItem;
    }
}