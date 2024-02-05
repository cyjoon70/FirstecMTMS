namespace SS.SSC018
{
    partial class SSC018P1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SSC018P1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTaxBiz = new C1.Win.C1Input.C1Button();
            this.btnCust = new C1.Win.C1Input.C1Button();
            this.txtTaxBizNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtTaxBizCd = new C1.Win.C1Input.C1TextBox();
            this.txtCustNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.txtCustCd = new C1.Win.C1Input.C1TextBox();
            this.dtpIssueDtTo = new C1.Win.C1Input.C1DateEdit();
            this.dtpIssueDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rdoIssueN = new System.Windows.Forms.RadioButton();
            this.rdoIssueAll = new System.Windows.Forms.RadioButton();
            this.rdoIssueY = new System.Windows.Forms.RadioButton();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.butCancel1 = new C1.Win.C1Input.C1Button();
            this.btnOk = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxBizNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxBizCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(903, 347);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Controls.Add(this.groupBox2);
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 136);
            this.GridCommPanel.Size = new System.Drawing.Size(903, 391);
            this.GridCommPanel.Controls.SetChildIndex(this.groupBox2, 0);
            this.GridCommPanel.Controls.SetChildIndex(this.GridCommGroupBox, 0);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(897, 327);
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
            this.panel1.Size = new System.Drawing.Size(903, 72);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnTaxBiz);
            this.groupBox1.Controls.Add(this.btnCust);
            this.groupBox1.Controls.Add(this.txtTaxBizNm);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtTaxBizCd);
            this.groupBox1.Controls.Add(this.txtCustNm);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.txtCustCd);
            this.groupBox1.Controls.Add(this.dtpIssueDtTo);
            this.groupBox1.Controls.Add(this.dtpIssueDtFr);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(903, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnTaxBiz
            // 
            this.btnTaxBiz.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTaxBiz.BackgroundImage")));
            this.btnTaxBiz.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnTaxBiz.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTaxBiz.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTaxBiz.Location = new System.Drawing.Point(207, 42);
            this.btnTaxBiz.Name = "btnTaxBiz";
            this.btnTaxBiz.Size = new System.Drawing.Size(24, 21);
            this.btnTaxBiz.TabIndex = 10;
            this.btnTaxBiz.UseVisualStyleBackColor = true;
            this.btnTaxBiz.Click += new System.EventHandler(this.btnTaxBiz_Click);
            // 
            // btnCust
            // 
            this.btnCust.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCust.BackgroundImage")));
            this.btnCust.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCust.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCust.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCust.Location = new System.Drawing.Point(635, 16);
            this.btnCust.Name = "btnCust";
            this.btnCust.Size = new System.Drawing.Size(24, 21);
            this.btnCust.TabIndex = 6;
            this.btnCust.UseVisualStyleBackColor = true;
            this.btnCust.Click += new System.EventHandler(this.btnCust_Click);
            // 
            // txtTaxBizNm
            // 
            this.txtTaxBizNm.AutoSize = false;
            this.txtTaxBizNm.BackColor = System.Drawing.Color.White;
            this.txtTaxBizNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtTaxBizNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTaxBizNm.Location = new System.Drawing.Point(230, 42);
            this.txtTaxBizNm.Name = "txtTaxBizNm";
            this.txtTaxBizNm.Size = new System.Drawing.Size(187, 21);
            this.txtTaxBizNm.TabIndex = 11;
            this.txtTaxBizNm.Tag = ";2;;";
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(12, 42);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(100, 21);
            this.c1Label2.TabIndex = 8;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "세금신고사업장";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtTaxBizCd
            // 
            this.txtTaxBizCd.AutoSize = false;
            this.txtTaxBizCd.BackColor = System.Drawing.Color.White;
            this.txtTaxBizCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtTaxBizCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTaxBizCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxBizCd.Location = new System.Drawing.Point(111, 42);
            this.txtTaxBizCd.Name = "txtTaxBizCd";
            this.txtTaxBizCd.Size = new System.Drawing.Size(96, 21);
            this.txtTaxBizCd.TabIndex = 9;
            this.txtTaxBizCd.Tag = null;
            this.txtTaxBizCd.TextChanged += new System.EventHandler(this.txtTaxBizCd_TextChanged);
            // 
            // txtCustNm
            // 
            this.txtCustNm.AutoSize = false;
            this.txtCustNm.BackColor = System.Drawing.Color.White;
            this.txtCustNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCustNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustNm.Location = new System.Drawing.Point(658, 16);
            this.txtCustNm.Name = "txtCustNm";
            this.txtCustNm.Size = new System.Drawing.Size(187, 21);
            this.txtCustNm.TabIndex = 7;
            this.txtCustNm.Tag = ";2;;";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(440, 16);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(100, 21);
            this.c1Label3.TabIndex = 4;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "발행처";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // txtCustCd
            // 
            this.txtCustCd.AutoSize = false;
            this.txtCustCd.BackColor = System.Drawing.Color.White;
            this.txtCustCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCustCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCustCd.Location = new System.Drawing.Point(539, 16);
            this.txtCustCd.Name = "txtCustCd";
            this.txtCustCd.Size = new System.Drawing.Size(96, 21);
            this.txtCustCd.TabIndex = 5;
            this.txtCustCd.Tag = null;
            this.txtCustCd.TextChanged += new System.EventHandler(this.txtCustCd_TextChanged);
            // 
            // dtpIssueDtTo
            // 
            this.dtpIssueDtTo.AutoSize = false;
            this.dtpIssueDtTo.BackColor = System.Drawing.Color.White;
            this.dtpIssueDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpIssueDtTo.Calendar.DayNameLength = 1;
            this.dtpIssueDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpIssueDtTo.Location = new System.Drawing.Point(225, 16);
            this.dtpIssueDtTo.Name = "dtpIssueDtTo";
            this.dtpIssueDtTo.Size = new System.Drawing.Size(96, 21);
            this.dtpIssueDtTo.TabIndex = 3;
            this.dtpIssueDtTo.Tag = "수주일자;1;;";
            this.dtpIssueDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // dtpIssueDtFr
            // 
            this.dtpIssueDtFr.AutoSize = false;
            this.dtpIssueDtFr.BackColor = System.Drawing.Color.White;
            this.dtpIssueDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpIssueDtFr.Calendar.DayNameLength = 1;
            this.dtpIssueDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpIssueDtFr.Location = new System.Drawing.Point(112, 16);
            this.dtpIssueDtFr.Name = "dtpIssueDtFr";
            this.dtpIssueDtFr.Size = new System.Drawing.Size(96, 21);
            this.dtpIssueDtFr.TabIndex = 1;
            this.dtpIssueDtFr.Tag = "수주일자;1;;";
            this.dtpIssueDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(440, 42);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(100, 21);
            this.c1Label5.TabIndex = 12;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "발행여부";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.rdoIssueN);
            this.panel5.Controls.Add(this.rdoIssueAll);
            this.panel5.Controls.Add(this.rdoIssueY);
            this.panel5.Location = new System.Drawing.Point(540, 41);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(247, 21);
            this.panel5.TabIndex = 14;
            // 
            // rdoIssueN
            // 
            this.rdoIssueN.Location = new System.Drawing.Point(133, 1);
            this.rdoIssueN.Name = "rdoIssueN";
            this.rdoIssueN.Size = new System.Drawing.Size(47, 18);
            this.rdoIssueN.TabIndex = 2;
            this.rdoIssueN.Text = "등록";
            this.rdoIssueN.UseVisualStyleBackColor = true;
            // 
            // rdoIssueAll
            // 
            this.rdoIssueAll.Checked = true;
            this.rdoIssueAll.Location = new System.Drawing.Point(10, 2);
            this.rdoIssueAll.Name = "rdoIssueAll";
            this.rdoIssueAll.Size = new System.Drawing.Size(54, 18);
            this.rdoIssueAll.TabIndex = 0;
            this.rdoIssueAll.TabStop = true;
            this.rdoIssueAll.Text = "전체";
            this.rdoIssueAll.UseVisualStyleBackColor = true;
            // 
            // rdoIssueY
            // 
            this.rdoIssueY.Location = new System.Drawing.Point(65, 2);
            this.rdoIssueY.Name = "rdoIssueY";
            this.rdoIssueY.Size = new System.Drawing.Size(61, 18);
            this.rdoIssueY.TabIndex = 1;
            this.rdoIssueY.Text = "미등록";
            this.rdoIssueY.UseVisualStyleBackColor = true;
            // 
            // c1Label1
            // 
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label1.Location = new System.Drawing.Point(208, 20);
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
            this.c1Label6.Size = new System.Drawing.Size(100, 21);
            this.c1Label6.TabIndex = 0;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "발행일";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.butCancel1);
            this.groupBox2.Controls.Add(this.btnOk);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 347);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(903, 44);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // butCancel1
            // 
            this.butCancel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCancel1.BackgroundImage")));
            this.butCancel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancel1.Location = new System.Drawing.Point(814, 13);
            this.butCancel1.Name = "butCancel1";
            this.butCancel1.Size = new System.Drawing.Size(86, 25);
            this.butCancel1.TabIndex = 26;
            this.butCancel1.Text = "취소";
            this.butCancel1.UseVisualStyleBackColor = true;
            this.butCancel1.Click += new System.EventHandler(this.butCancel1_Click);
            // 
            // btnOk
            // 
            this.btnOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOk.BackgroundImage")));
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.Location = new System.Drawing.Point(722, 14);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(86, 25);
            this.btnOk.TabIndex = 25;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // SSC018P1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(903, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SSC018P1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "수주참조조회";
            this.Load += new System.EventHandler(this.SSC018P1_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxBizNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxBizCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1Label c1Label1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton rdoIssueN;
        private System.Windows.Forms.RadioButton rdoIssueAll;
        private System.Windows.Forms.RadioButton rdoIssueY;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtTo;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtFr;
        private C1.Win.C1Input.C1TextBox txtTaxBizNm;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtTaxBizCd;
        private C1.Win.C1Input.C1TextBox txtCustNm;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1TextBox txtCustCd;
        private System.Windows.Forms.GroupBox groupBox2;
        private C1.Win.C1Input.C1Button butCancel1;
        private C1.Win.C1Input.C1Button btnOk;
        private C1.Win.C1Input.C1Button btnTaxBiz;
        private C1.Win.C1Input.C1Button btnCust;
    }
}