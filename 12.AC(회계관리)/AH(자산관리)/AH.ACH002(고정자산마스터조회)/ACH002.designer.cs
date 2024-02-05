namespace AH.ACH002
{
    partial class ACH002
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACH002));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtAssetNo = new C1.Win.C1Input.C1TextBox();
            this.txtDeptNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.txtDeptCd = new C1.Win.C1Input.C1TextBox();
            this.btnDept = new C1.Win.C1Input.C1Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSlipView = new C1.Win.C1Input.C1Button();
            this.txtAcctNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.txtAcctCd = new C1.Win.C1Input.C1TextBox();
            this.btnAcct = new C1.Win.C1Input.C1Button();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.cboBizAreaCdFr = new C1.Win.C1List.C1Combo();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBizAreaCdTo = new C1.Win.C1List.C1Combo();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssetNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdTo)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1167, 355);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 137);
            this.GridCommPanel.Size = new System.Drawing.Size(1167, 355);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1161, 335);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1167, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1167, 73);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboBizAreaCdTo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtAcctNm);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.txtAcctCd);
            this.groupBox1.Controls.Add(this.btnAcct);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtAssetNo);
            this.groupBox1.Controls.Add(this.txtDeptNm);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.txtDeptCd);
            this.groupBox1.Controls.Add(this.btnDept);
            this.groupBox1.Controls.Add(this.cboBizAreaCdFr);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1167, 73);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(12, 42);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(94, 21);
            this.c1Label2.TabIndex = 18;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "자산번호";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtAssetNo
            // 
            this.txtAssetNo.AutoSize = false;
            this.txtAssetNo.BackColor = System.Drawing.Color.White;
            this.txtAssetNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAssetNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssetNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAssetNo.Location = new System.Drawing.Point(105, 42);
            this.txtAssetNo.Name = "txtAssetNo";
            this.txtAssetNo.Size = new System.Drawing.Size(148, 21);
            this.txtAssetNo.TabIndex = 19;
            this.txtAssetNo.Tag = ";;;;";
            this.txtAssetNo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // txtDeptNm
            // 
            this.txtDeptNm.AutoSize = false;
            this.txtDeptNm.BackColor = System.Drawing.Color.White;
            this.txtDeptNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptNm.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDeptNm.Location = new System.Drawing.Point(233, 17);
            this.txtDeptNm.Name = "txtDeptNm";
            this.txtDeptNm.Size = new System.Drawing.Size(207, 21);
            this.txtDeptNm.TabIndex = 7;
            this.txtDeptNm.TabStop = false;
            this.txtDeptNm.Tag = ";2;;";
            this.txtDeptNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(12, 17);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 4;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "관련부서";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // txtDeptCd
            // 
            this.txtDeptCd.AutoSize = false;
            this.txtDeptCd.BackColor = System.Drawing.Color.White;
            this.txtDeptCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDeptCd.Location = new System.Drawing.Point(105, 17);
            this.txtDeptCd.Name = "txtDeptCd";
            this.txtDeptCd.Size = new System.Drawing.Size(104, 21);
            this.txtDeptCd.TabIndex = 5;
            this.txtDeptCd.Tag = "발생부서;;;;";
            this.txtDeptCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtDeptCd.TextChanged += new System.EventHandler(this.txtDeptCd_TextChanged);
            // 
            // btnDept
            // 
            this.btnDept.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDept.BackgroundImage")));
            this.btnDept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDept.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDept.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDept.Location = new System.Drawing.Point(209, 17);
            this.btnDept.Name = "btnDept";
            this.btnDept.Size = new System.Drawing.Size(24, 21);
            this.btnDept.TabIndex = 6;
            this.btnDept.TabStop = false;
            this.btnDept.Tag = "";
            this.btnDept.UseVisualStyleBackColor = true;
            this.btnDept.Click += new System.EventHandler(this.btnDept_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 492);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1167, 51);
            this.panel2.TabIndex = 37;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.btnSlipView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1167, 51);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            // 
            // btnSlipView
            // 
            this.btnSlipView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSlipView.BackgroundImage")));
            this.btnSlipView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSlipView.Location = new System.Drawing.Point(12, 18);
            this.btnSlipView.Name = "btnSlipView";
            this.btnSlipView.Size = new System.Drawing.Size(86, 25);
            this.btnSlipView.TabIndex = 21;
            this.btnSlipView.Text = "전표조회";
            this.btnSlipView.UseVisualStyleBackColor = true;
            this.btnSlipView.Click += new System.EventHandler(this.btnSlipView_Click);
            // 
            // txtAcctNm
            // 
            this.txtAcctNm.AutoSize = false;
            this.txtAcctNm.BackColor = System.Drawing.Color.White;
            this.txtAcctNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAcctNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAcctNm.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAcctNm.Location = new System.Drawing.Point(678, 16);
            this.txtAcctNm.Name = "txtAcctNm";
            this.txtAcctNm.Size = new System.Drawing.Size(207, 21);
            this.txtAcctNm.TabIndex = 25;
            this.txtAcctNm.TabStop = false;
            this.txtAcctNm.Tag = ";2;;";
            this.txtAcctNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(457, 16);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(94, 21);
            this.c1Label5.TabIndex = 22;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "계정";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // txtAcctCd
            // 
            this.txtAcctCd.AutoSize = false;
            this.txtAcctCd.BackColor = System.Drawing.Color.White;
            this.txtAcctCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAcctCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAcctCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAcctCd.Location = new System.Drawing.Point(550, 16);
            this.txtAcctCd.Name = "txtAcctCd";
            this.txtAcctCd.Size = new System.Drawing.Size(104, 21);
            this.txtAcctCd.TabIndex = 23;
            this.txtAcctCd.Tag = "계정코드;;;;";
            this.txtAcctCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtAcctCd.TextChanged += new System.EventHandler(this.txtAcctCd_TextChanged);
            // 
            // btnAcct
            // 
            this.btnAcct.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAcct.BackgroundImage")));
            this.btnAcct.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAcct.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAcct.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAcct.Location = new System.Drawing.Point(654, 16);
            this.btnAcct.Name = "btnAcct";
            this.btnAcct.Size = new System.Drawing.Size(24, 21);
            this.btnAcct.TabIndex = 24;
            this.btnAcct.TabStop = false;
            this.btnAcct.Tag = "";
            this.btnAcct.UseVisualStyleBackColor = true;
            this.btnAcct.Click += new System.EventHandler(this.btnAcct_Click);
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(457, 40);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(94, 21);
            this.c1Label6.TabIndex = 14;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "사업장";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // cboBizAreaCdFr
            // 
            this.cboBizAreaCdFr.AddItemSeparator = ';';
            this.cboBizAreaCdFr.AutoSize = false;
            this.cboBizAreaCdFr.Caption = "";
            this.cboBizAreaCdFr.CaptionHeight = 17;
            this.cboBizAreaCdFr.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBizAreaCdFr.ColumnCaptionHeight = 18;
            this.cboBizAreaCdFr.ColumnFooterHeight = 18;
            this.cboBizAreaCdFr.ContentHeight = 15;
            this.cboBizAreaCdFr.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBizAreaCdFr.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBizAreaCdFr.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBizAreaCdFr.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBizAreaCdFr.EditorHeight = 15;
            this.cboBizAreaCdFr.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBizAreaCdFr.Images"))));
            this.cboBizAreaCdFr.ItemHeight = 15;
            this.cboBizAreaCdFr.Location = new System.Drawing.Point(550, 40);
            this.cboBizAreaCdFr.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCdFr.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCdFr.MaxLength = 32767;
            this.cboBizAreaCdFr.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCdFr.Name = "cboBizAreaCdFr";
            this.cboBizAreaCdFr.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCdFr.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCdFr.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCdFr.TabIndex = 15;
            this.cboBizAreaCdFr.Tag = ";;;";
            this.cboBizAreaCdFr.PropBag = resources.GetString("cboBizAreaCdFr.PropBag");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(704, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "~";
            // 
            // cboBizAreaCdTo
            // 
            this.cboBizAreaCdTo.AddItemSeparator = ';';
            this.cboBizAreaCdTo.AutoSize = false;
            this.cboBizAreaCdTo.Caption = "";
            this.cboBizAreaCdTo.CaptionHeight = 17;
            this.cboBizAreaCdTo.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBizAreaCdTo.ColumnCaptionHeight = 18;
            this.cboBizAreaCdTo.ColumnFooterHeight = 18;
            this.cboBizAreaCdTo.ContentHeight = 15;
            this.cboBizAreaCdTo.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBizAreaCdTo.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBizAreaCdTo.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBizAreaCdTo.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBizAreaCdTo.EditorHeight = 15;
            this.cboBizAreaCdTo.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBizAreaCdTo.Images"))));
            this.cboBizAreaCdTo.ItemHeight = 15;
            this.cboBizAreaCdTo.Location = new System.Drawing.Point(724, 40);
            this.cboBizAreaCdTo.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCdTo.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCdTo.MaxLength = 32767;
            this.cboBizAreaCdTo.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCdTo.Name = "cboBizAreaCdTo";
            this.cboBizAreaCdTo.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCdTo.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCdTo.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCdTo.TabIndex = 27;
            this.cboBizAreaCdTo.Tag = ";;;";
            this.cboBizAreaCdTo.PropBag = resources.GetString("cboBizAreaCdTo.PropBag");
            // 
            // ACH002
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1167, 543);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACH002";
            this.Text = "고정자산마스터조회";
            this.Load += new System.EventHandler(this.ACH002_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
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
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssetNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtAssetNo;
        private C1.Win.C1Input.C1TextBox txtDeptNm;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1TextBox txtDeptCd;
        private C1.Win.C1Input.C1Button btnDept;
        private C1.Win.C1List.C1Combo cboBizAreaCdTo;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1TextBox txtAcctNm;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1TextBox txtAcctCd;
        private C1.Win.C1Input.C1Button btnAcct;
        private C1.Win.C1List.C1Combo cboBizAreaCdFr;
        private C1.Win.C1Input.C1Label c1Label6;
    }
}