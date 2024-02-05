namespace PE.PEA007
{
    partial class PEA007
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PEA007));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClose = new C1.Win.C1Input.C1Button();
            this.btnTouchProc = new C1.Win.C1Input.C1Button();
            this.dtpWorkDt = new C1.Win.C1Input.C1DateEdit();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtWORKORDER_NO = new C1.Win.C1Input.C1TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.btnWc = new C1.Win.C1Input.C1Button();
            this.txtWc_Nm = new C1.Win.C1Input.C1TextBox();
            this.txtWc_Cd = new C1.Win.C1Input.C1TextBox();
            this.btnWorkDuty = new C1.Win.C1Input.C1Button();
            this.txtWorkDutyNm = new C1.Win.C1Input.C1TextBox();
            this.txtWorkDutyId = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label7 = new C1.Win.C1Input.C1Label();
            this.btnClose1 = new C1.Win.C1Input.C1Button();
            this.btnWorkClose = new C1.Win.C1Input.C1Button();
            this.TXTINSPREQ_NO = new C1.Win.C1Input.C1TextBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpWorkDt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWORKORDER_NO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWc_Nm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWc_Cd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkDutyNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkDutyId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TXTINSPREQ_NO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnTouchProc);
            this.groupBox1.Controls.Add(this.dtpWorkDt);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Location = new System.Drawing.Point(9, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(611, 56);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "일마감전개";
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(505, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 25);
            this.btnClose.TabIndex = 73;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnTouchProc
            // 
            this.btnTouchProc.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTouchProc.BackgroundImage")));
            this.btnTouchProc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTouchProc.Location = new System.Drawing.Point(407, 20);
            this.btnTouchProc.Name = "btnTouchProc";
            this.btnTouchProc.Size = new System.Drawing.Size(92, 25);
            this.btnTouchProc.TabIndex = 72;
            this.btnTouchProc.Text = "<마감전개>";
            this.btnTouchProc.UseVisualStyleBackColor = true;
            this.btnTouchProc.Click += new System.EventHandler(this.btnTouchProc_Click);
            // 
            // dtpWorkDt
            // 
            this.dtpWorkDt.AutoSize = false;
            this.dtpWorkDt.BackColor = System.Drawing.Color.White;
            this.dtpWorkDt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpWorkDt.Calendar.DayNameLength = 1;
            this.dtpWorkDt.EmptyAsNull = true;
            this.dtpWorkDt.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpWorkDt.Location = new System.Drawing.Point(80, 23);
            this.dtpWorkDt.Name = "dtpWorkDt";
            this.dtpWorkDt.Size = new System.Drawing.Size(107, 21);
            this.dtpWorkDt.TabIndex = 3;
            this.dtpWorkDt.Tag = null;
            this.dtpWorkDt.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpWorkDt.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(10, 23);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(71, 21);
            this.c1Label2.TabIndex = 2;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "작업일자";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.listView1);
            this.groupBox2.Location = new System.Drawing.Point(9, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(611, 19);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "진행상태";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(6, 20);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(599, 0);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.TXTINSPREQ_NO);
            this.groupBox3.Controls.Add(this.c1Label4);
            this.groupBox3.Controls.Add(this.txtWORKORDER_NO);
            this.groupBox3.Controls.Add(this.c1Label3);
            this.groupBox3.Controls.Add(this.btnWc);
            this.groupBox3.Controls.Add(this.txtWc_Nm);
            this.groupBox3.Controls.Add(this.txtWc_Cd);
            this.groupBox3.Controls.Add(this.btnWorkDuty);
            this.groupBox3.Controls.Add(this.txtWorkDutyNm);
            this.groupBox3.Controls.Add(this.txtWorkDutyId);
            this.groupBox3.Controls.Add(this.c1Label1);
            this.groupBox3.Controls.Add(this.c1Label7);
            this.groupBox3.Controls.Add(this.btnClose1);
            this.groupBox3.Controls.Add(this.btnWorkClose);
            this.groupBox3.Location = new System.Drawing.Point(9, 69);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(611, 96);
            this.groupBox3.TabIndex = 74;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "TOUCH 강제 초기화";
            // 
            // txtWORKORDER_NO
            // 
            this.txtWORKORDER_NO.AutoSize = false;
            this.txtWORKORDER_NO.BackColor = System.Drawing.Color.White;
            this.txtWORKORDER_NO.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWORKORDER_NO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWORKORDER_NO.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtWORKORDER_NO.Location = new System.Drawing.Point(92, 68);
            this.txtWORKORDER_NO.Name = "txtWORKORDER_NO";
            this.txtWORKORDER_NO.Size = new System.Drawing.Size(96, 21);
            this.txtWORKORDER_NO.TabIndex = 83;
            this.txtWORKORDER_NO.Tag = null;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(10, 68);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(83, 21);
            this.c1Label3.TabIndex = 82;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "제조오더번호";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // btnWc
            // 
            this.btnWc.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnWc.BackgroundImage")));
            this.btnWc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnWc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnWc.Location = new System.Drawing.Point(188, 20);
            this.btnWc.Name = "btnWc";
            this.btnWc.Size = new System.Drawing.Size(24, 21);
            this.btnWc.TabIndex = 76;
            this.btnWc.UseVisualStyleBackColor = true;
            this.btnWc.Click += new System.EventHandler(this.btnWc_Click);
            // 
            // txtWc_Nm
            // 
            this.txtWc_Nm.AutoSize = false;
            this.txtWc_Nm.BackColor = System.Drawing.Color.LightGray;
            this.txtWc_Nm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWc_Nm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWc_Nm.Location = new System.Drawing.Point(211, 20);
            this.txtWc_Nm.Name = "txtWc_Nm";
            this.txtWc_Nm.Size = new System.Drawing.Size(182, 21);
            this.txtWc_Nm.TabIndex = 77;
            this.txtWc_Nm.Tag = ";2;;";
            // 
            // txtWc_Cd
            // 
            this.txtWc_Cd.AutoSize = false;
            this.txtWc_Cd.BackColor = System.Drawing.Color.SkyBlue;
            this.txtWc_Cd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWc_Cd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWc_Cd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtWc_Cd.Location = new System.Drawing.Point(92, 20);
            this.txtWc_Cd.Name = "txtWc_Cd";
            this.txtWc_Cd.Size = new System.Drawing.Size(96, 21);
            this.txtWc_Cd.TabIndex = 75;
            this.txtWc_Cd.Tag = "작업장;1;;";
            this.txtWc_Cd.TextChanged += new System.EventHandler(this.txtWc_Cd_TextChanged);
            // 
            // btnWorkDuty
            // 
            this.btnWorkDuty.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnWorkDuty.BackgroundImage")));
            this.btnWorkDuty.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnWorkDuty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWorkDuty.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnWorkDuty.Location = new System.Drawing.Point(188, 44);
            this.btnWorkDuty.Name = "btnWorkDuty";
            this.btnWorkDuty.Size = new System.Drawing.Size(24, 21);
            this.btnWorkDuty.TabIndex = 80;
            this.btnWorkDuty.UseVisualStyleBackColor = true;
            this.btnWorkDuty.Click += new System.EventHandler(this.btnWorkDuty_Click);
            // 
            // txtWorkDutyNm
            // 
            this.txtWorkDutyNm.AutoSize = false;
            this.txtWorkDutyNm.BackColor = System.Drawing.Color.LightGray;
            this.txtWorkDutyNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWorkDutyNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWorkDutyNm.Location = new System.Drawing.Point(211, 44);
            this.txtWorkDutyNm.Name = "txtWorkDutyNm";
            this.txtWorkDutyNm.Size = new System.Drawing.Size(96, 21);
            this.txtWorkDutyNm.TabIndex = 81;
            this.txtWorkDutyNm.Tag = ";2;;";
            // 
            // txtWorkDutyId
            // 
            this.txtWorkDutyId.AutoSize = false;
            this.txtWorkDutyId.BackColor = System.Drawing.Color.SkyBlue;
            this.txtWorkDutyId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWorkDutyId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWorkDutyId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtWorkDutyId.Location = new System.Drawing.Point(92, 44);
            this.txtWorkDutyId.Name = "txtWorkDutyId";
            this.txtWorkDutyId.Size = new System.Drawing.Size(96, 21);
            this.txtWorkDutyId.TabIndex = 79;
            this.txtWorkDutyId.Tag = "작업일자;1;;";
            this.txtWorkDutyId.TextChanged += new System.EventHandler(this.txtWorkDutyId_TextChanged);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(10, 44);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(83, 21);
            this.c1Label1.TabIndex = 78;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "작업자";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label7
            // 
            this.c1Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label7.Location = new System.Drawing.Point(10, 20);
            this.c1Label7.Name = "c1Label7";
            this.c1Label7.Size = new System.Drawing.Size(83, 21);
            this.c1Label7.TabIndex = 74;
            this.c1Label7.Tag = null;
            this.c1Label7.Text = "작업장";
            this.c1Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label7.TextDetached = true;
            this.c1Label7.Value = "";
            // 
            // btnClose1
            // 
            this.btnClose1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose1.BackgroundImage")));
            this.btnClose1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose1.Location = new System.Drawing.Point(505, 16);
            this.btnClose1.Name = "btnClose1";
            this.btnClose1.Size = new System.Drawing.Size(92, 25);
            this.btnClose1.TabIndex = 73;
            this.btnClose1.Text = "닫기";
            this.btnClose1.UseVisualStyleBackColor = true;
            this.btnClose1.Click += new System.EventHandler(this.btnClose1_Click);
            // 
            // btnWorkClose
            // 
            this.btnWorkClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnWorkClose.BackgroundImage")));
            this.btnWorkClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWorkClose.Location = new System.Drawing.Point(407, 16);
            this.btnWorkClose.Name = "btnWorkClose";
            this.btnWorkClose.Size = new System.Drawing.Size(92, 25);
            this.btnWorkClose.TabIndex = 72;
            this.btnWorkClose.Text = "강제 마감";
            this.btnWorkClose.UseVisualStyleBackColor = true;
            this.btnWorkClose.Click += new System.EventHandler(this.btnWorkClose_Click);
            // 
            // TXTINSPREQ_NO
            // 
            this.TXTINSPREQ_NO.AutoSize = false;
            this.TXTINSPREQ_NO.BackColor = System.Drawing.Color.White;
            this.TXTINSPREQ_NO.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.TXTINSPREQ_NO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TXTINSPREQ_NO.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TXTINSPREQ_NO.Location = new System.Drawing.Point(293, 67);
            this.TXTINSPREQ_NO.Name = "TXTINSPREQ_NO";
            this.TXTINSPREQ_NO.Size = new System.Drawing.Size(96, 21);
            this.TXTINSPREQ_NO.TabIndex = 85;
            this.TXTINSPREQ_NO.Tag = null;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(211, 67);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(83, 21);
            this.c1Label4.TabIndex = 84;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "검사의뢰번호";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // PEA007
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(632, 169);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PEA007";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TOUCH일마감(수동)";
            this.Activated += new System.EventHandler(this.PEA007_Activated);
            this.Load += new System.EventHandler(this.PEA007_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtpWorkDt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtWORKORDER_NO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWc_Nm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWc_Cd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkDutyNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkDutyId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TXTINSPREQ_NO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Button btnClose;
        private C1.Win.C1Input.C1Button btnTouchProc;
        private C1.Win.C1Input.C1DateEdit dtpWorkDt;
        private C1.Win.C1Input.C1Label c1Label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.GroupBox groupBox3;
        private C1.Win.C1Input.C1Button btnWc;
        private C1.Win.C1Input.C1TextBox txtWc_Nm;
        private C1.Win.C1Input.C1TextBox txtWc_Cd;
        private C1.Win.C1Input.C1Button btnWorkDuty;
        private C1.Win.C1Input.C1TextBox txtWorkDutyNm;
        private C1.Win.C1Input.C1TextBox txtWorkDutyId;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Label c1Label7;
        private C1.Win.C1Input.C1Button btnClose1;
        private C1.Win.C1Input.C1Button btnWorkClose;
        private C1.Win.C1Input.C1TextBox txtWORKORDER_NO;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1TextBox TXTINSPREQ_NO;
        private C1.Win.C1Input.C1Label c1Label4;
    }
}