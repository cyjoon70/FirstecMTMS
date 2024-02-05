namespace PA.SBA010
{ 
    partial class SBA010P1
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

        #region Windows Form 디자이너에서 생성한 코드
        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SBA010P1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSoDtTo = new C1.Win.C1Input.C1DateEdit();
            this.dtpSoDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.txtSoNo = new C1.Win.C1Input.C1TextBox();
            this.txtShipNm = new C1.Win.C1Input.C1TextBox();
            this.btnShip = new C1.Win.C1Input.C1Button();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.txtShipCd = new C1.Win.C1Input.C1TextBox();
            this.txtProjectNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtProjectNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.GridCommGroupBox.SuspendLayout();
            this.GridCommPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.panButton1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnHelp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRowIns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnExcel)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSoNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(903, 389);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 138);
            this.GridCommPanel.Size = new System.Drawing.Size(903, 389);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(897, 369);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(903, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 74);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.dtpSoDtTo);
            this.groupBox1.Controls.Add(this.dtpSoDtFr);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.txtSoNo);
            this.groupBox1.Controls.Add(this.txtShipNm);
            this.groupBox1.Controls.Add(this.btnShip);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.txtShipCd);
            this.groupBox1.Controls.Add(this.txtProjectNm);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtProjectNo);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(903, 74);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // dtpSoDtTo
            // 
            this.dtpSoDtTo.AutoSize = false;
            this.dtpSoDtTo.BackColor = System.Drawing.Color.White;
            this.dtpSoDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSoDtTo.Calendar.DayNameLength = 1;
            this.dtpSoDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSoDtTo.Location = new System.Drawing.Point(214, 16);
            this.dtpSoDtTo.Name = "dtpSoDtTo";
            this.dtpSoDtTo.Size = new System.Drawing.Size(96, 21);
            this.dtpSoDtTo.TabIndex = 3;
            this.dtpSoDtTo.Tag = "수주일자;1;;";
            this.dtpSoDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSoDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // dtpSoDtFr
            // 
            this.dtpSoDtFr.AutoSize = false;
            this.dtpSoDtFr.BackColor = System.Drawing.Color.White;
            this.dtpSoDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSoDtFr.Calendar.DayNameLength = 1;
            this.dtpSoDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSoDtFr.Location = new System.Drawing.Point(101, 16);
            this.dtpSoDtFr.Name = "dtpSoDtFr";
            this.dtpSoDtFr.Size = new System.Drawing.Size(96, 21);
            this.dtpSoDtFr.TabIndex = 1;
            this.dtpSoDtFr.Tag = "수주일자;1;;";
            this.dtpSoDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSoDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(12, 44);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(90, 21);
            this.c1Label4.TabIndex = 7;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "수주번호";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // txtSoNo
            // 
            this.txtSoNo.AutoSize = false;
            this.txtSoNo.BackColor = System.Drawing.Color.White;
            this.txtSoNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSoNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSoNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSoNo.Location = new System.Drawing.Point(101, 44);
            this.txtSoNo.Name = "txtSoNo";
            this.txtSoNo.Size = new System.Drawing.Size(96, 21);
            this.txtSoNo.TabIndex = 8;
            this.txtSoNo.Tag = null;
            // 
            // txtShipNm
            // 
            this.txtShipNm.AutoSize = false;
            this.txtShipNm.BackColor = System.Drawing.Color.White;
            this.txtShipNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtShipNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShipNm.Location = new System.Drawing.Point(585, 44);
            this.txtShipNm.Name = "txtShipNm";
            this.txtShipNm.Size = new System.Drawing.Size(187, 21);
            this.txtShipNm.TabIndex = 12;
            this.txtShipNm.Tag = ";2;;";
            // 
            // btnShip
            // 
            this.btnShip.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnShip.BackgroundImage")));
            this.btnShip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnShip.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShip.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShip.Location = new System.Drawing.Point(561, 44);
            this.btnShip.Name = "btnShip";
            this.btnShip.Size = new System.Drawing.Size(24, 21);
            this.btnShip.TabIndex = 11;
            this.btnShip.UseVisualStyleBackColor = true;
            this.btnShip.Click += new System.EventHandler(this.btnShip_Click);
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(356, 44);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(110, 21);
            this.c1Label3.TabIndex = 9;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "납품처";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // txtShipCd
            // 
            this.txtShipCd.AutoSize = false;
            this.txtShipCd.BackColor = System.Drawing.Color.White;
            this.txtShipCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtShipCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtShipCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtShipCd.Location = new System.Drawing.Point(465, 44);
            this.txtShipCd.Name = "txtShipCd";
            this.txtShipCd.Size = new System.Drawing.Size(96, 21);
            this.txtShipCd.TabIndex = 10;
            this.txtShipCd.Tag = null;
            // 
            // txtProjectNm
            // 
            this.txtProjectNm.AutoSize = false;
            this.txtProjectNm.BackColor = System.Drawing.Color.White;
            this.txtProjectNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjectNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjectNm.Location = new System.Drawing.Point(560, 16);
            this.txtProjectNm.Name = "txtProjectNm";
            this.txtProjectNm.Size = new System.Drawing.Size(211, 21);
            this.txtProjectNm.TabIndex = 6;
            this.txtProjectNm.Tag = null;
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(356, 16);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(110, 21);
            this.c1Label2.TabIndex = 4;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "프로젝트번호";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtProjectNo
            // 
            this.txtProjectNo.AutoSize = false;
            this.txtProjectNo.BackColor = System.Drawing.Color.White;
            this.txtProjectNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjectNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjectNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProjectNo.Location = new System.Drawing.Point(465, 16);
            this.txtProjectNo.Name = "txtProjectNo";
            this.txtProjectNo.Size = new System.Drawing.Size(96, 21);
            this.txtProjectNo.TabIndex = 5;
            this.txtProjectNo.Tag = null;
            // 
            // c1Label1
            // 
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label1.Location = new System.Drawing.Point(197, 20);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(16, 21);
            this.c1Label1.TabIndex = 2;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "~";
            this.c1Label1.TextDetached = true;
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(12, 16);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(90, 21);
            this.c1Label6.TabIndex = 0;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "수주일자";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // SBA010P1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(903, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SBA010P1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "수주참조조회";
            this.Load += new System.EventHandler(this.SBA010P1_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.GridCommPanel, 0);
            this.GridCommGroupBox.ResumeLayout(false);
            this.GridCommPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.panButton1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BtnDel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnHelp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRowIns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnRCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BtnExcel)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSoNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtShipCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtProjectNm;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtProjectNo;
        private C1.Win.C1Input.C1TextBox txtShipNm;
        private C1.Win.C1Input.C1Button btnShip;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1TextBox txtShipCd;
        private C1.Win.C1Input.C1TextBox txtSoNo;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1DateEdit dtpSoDtTo;
        private C1.Win.C1Input.C1DateEdit dtpSoDtFr;

    }
}