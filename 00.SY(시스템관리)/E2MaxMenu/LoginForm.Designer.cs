using System;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace E2MAXMenu
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.chkSaveId = new C1.Win.C1Input.C1CheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.cboCompCd = new C1.Win.C1List.C1Combo();
            this.txtPW = new C1.Win.C1Input.C1TextBox();
            this.txtID = new C1.Win.C1Input.C1TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCompCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(1014, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 878);
            this.label3.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(0, 866);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1016, 12);
            this.label4.TabIndex = 39;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Window;
            this.label5.Location = new System.Drawing.Point(0, -3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(8, 868);
            this.label5.TabIndex = 39;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Window;
            this.label6.Location = new System.Drawing.Point(-3, -1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(1016, 11);
            this.label6.TabIndex = 40;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(800, 552);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox5);
            this.panel1.Controls.Add(this.pictureBox4);
            this.panel1.Controls.Add(this.chkSaveId);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.cboCompCd);
            this.panel1.Controls.Add(this.txtPW);
            this.panel1.Controls.Add(this.txtID);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 548);
            this.panel1.TabIndex = 37;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Location = new System.Drawing.Point(708, 505);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(80, 36);
            this.pictureBox2.TabIndex = 34;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.White;
            this.pictureBox5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox5.BackgroundImage")));
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox5.Location = new System.Drawing.Point(7, 59);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(786, 192);
            this.pictureBox5.TabIndex = 46;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.White;
            this.pictureBox4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox4.BackgroundImage")));
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox4.Location = new System.Drawing.Point(584, 16);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(207, 35);
            this.pictureBox4.TabIndex = 45;
            this.pictureBox4.TabStop = false;
            // 
            // chkSaveId
            // 
            this.chkSaveId.BackColor = System.Drawing.Color.Transparent;
            this.chkSaveId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkSaveId.Font = new System.Drawing.Font("돋움", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkSaveId.ForeColor = System.Drawing.Color.White;
            this.chkSaveId.Location = new System.Drawing.Point(472, 356);
            this.chkSaveId.Name = "chkSaveId";
            this.chkSaveId.Size = new System.Drawing.Size(83, 21);
            this.chkSaveId.TabIndex = 42;
            this.chkSaveId.Text = "ID 저장";
            this.chkSaveId.UseVisualStyleBackColor = false;
            this.chkSaveId.Value = null;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox3.BackgroundImage")));
            this.pictureBox3.Location = new System.Drawing.Point(6, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(788, 43);
            this.pictureBox3.TabIndex = 38;
            this.pictureBox3.TabStop = false;
            // 
            // cboCompCd
            // 
            this.cboCompCd.AddItemSeparator = ';';
            this.cboCompCd.AutoSize = false;
            this.cboCompCd.Caption = "";
            this.cboCompCd.CaptionHeight = 17;
            this.cboCompCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboCompCd.ColumnCaptionHeight = 18;
            this.cboCompCd.ColumnFooterHeight = 18;
            this.cboCompCd.ColumnHeaders = false;
            this.cboCompCd.ContentHeight = 19;
            this.cboCompCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboCompCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboCompCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboCompCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboCompCd.EditorHeight = 19;
            this.cboCompCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboCompCd.Images"))));
            this.cboCompCd.ItemHeight = 15;
            this.cboCompCd.Location = new System.Drawing.Point(304, 353);
            this.cboCompCd.MatchEntryTimeout = ((long)(2000));
            this.cboCompCd.MaxDropDownItems = ((short)(5));
            this.cboCompCd.MaxLength = 32767;
            this.cboCompCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboCompCd.Name = "cboCompCd";
            this.cboCompCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboCompCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboCompCd.Size = new System.Drawing.Size(153, 25);
            this.cboCompCd.TabIndex = 35;
            this.cboCompCd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCompCd_KeyDown);
            this.cboCompCd.PropBag = resources.GetString("cboCompCd.PropBag");
            // 
            // txtPW
            // 
            this.txtPW.AutoSize = false;
            this.txtPW.Location = new System.Drawing.Point(304, 414);
            this.txtPW.Name = "txtPW";
            this.txtPW.PasswordChar = '*';
            this.txtPW.Size = new System.Drawing.Size(153, 25);
            this.txtPW.TabIndex = 0;
            this.txtPW.Tag = null;
            this.txtPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPW_KeyDown);
            // 
            // txtID
            // 
            this.txtID.AutoSize = false;
            this.txtID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtID.Location = new System.Drawing.Point(304, 384);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(153, 25);
            this.txtID.TabIndex = 36;
            this.txtID.Tag = null;
            this.txtID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtID_KeyDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(463, 384);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(92, 56);
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 552);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCompCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Input.C1TextBox txtID;
        private C1.Win.C1Input.C1TextBox txtPW;
        private C1.Win.C1List.C1Combo cboCompCd;
        private System.Windows.Forms.PictureBox pictureBox3;
        private C1.Win.C1Input.C1CheckBox chkSaveId;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}