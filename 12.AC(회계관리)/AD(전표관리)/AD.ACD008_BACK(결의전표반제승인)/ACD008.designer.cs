namespace AD.ACD008
{
    partial class ACD008
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACD008));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSSlipNoTo = new C1.Win.C1Input.C1TextBox();
            this.btnSSlipTo = new C1.Win.C1Input.C1Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSSlipNoFr = new C1.Win.C1Input.C1TextBox();
            this.btnSSlipFr = new C1.Win.C1Input.C1Button();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.txtInEmpNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.txtDeptCd = new C1.Win.C1Input.C1TextBox();
            this.BtnDept = new C1.Win.C1Input.C1Button();
            this.txtDeptNm = new C1.Win.C1Input.C1TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpSlipDtFr = new C1.Win.C1Input.C1DateEdit();
            this.dtpSlipDtTo = new C1.Win.C1Input.C1DateEdit();
            this.c1Label12 = new C1.Win.C1Input.C1Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.optConfirm_N = new System.Windows.Forms.RadioButton();
            this.optConfirm_Y = new System.Windows.Forms.RadioButton();
            this.c1Label25 = new C1.Win.C1Input.C1Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnSlipView = new C1.Win.C1Input.C1Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.GridCommPanel2.SuspendLayout();
            this.GridCommGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
            this.GridCommPanel1.SuspendLayout();
            this.GridCommGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView1)).BeginInit();
            this.panel4.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlipNoTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlipNoFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInEmpNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label25)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridCommPanel2
            // 
            this.GridCommPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.GridCommPanel2.Location = new System.Drawing.Point(0, 0);
            this.GridCommPanel2.Size = new System.Drawing.Size(1222, 244);
            // 
            // GridCommGroupBox2
            // 
            this.GridCommGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox2.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox2.Size = new System.Drawing.Size(1222, 244);
            this.GridCommGroupBox2.Text = "전표현황";
            // 
            // fpSpread2
            // 
            this.fpSpread2.Size = new System.Drawing.Size(1712, 217);
            this.fpSpread2.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread2_SelectionChanged);
            this.fpSpread2.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread2_Change);
            this.fpSpread2.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread2_CellClick);
            this.fpSpread2.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread2_ButtonClicked);
            this.fpSpread2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread2_KeyDown);
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // 
            // GridCommPanel1
            // 
            this.GridCommPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel1.Location = new System.Drawing.Point(0, 256);
            this.GridCommPanel1.Size = new System.Drawing.Size(1222, 200);
            // 
            // GridCommGroupBox1
            // 
            this.GridCommGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox1.Size = new System.Drawing.Size(1222, 200);
            this.GridCommGroupBox1.Text = "젼표내역";
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1216, 180);
            // 
            // sheetView1
            // 
            this.sheetView1.SheetName = "Sheet1";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 159);
            this.panel4.Size = new System.Drawing.Size(1222, 496);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel2, 0);
            this.panel4.Controls.SetChildIndex(this.splitter1, 0);
            this.panel4.Controls.SetChildIndex(this.panel2, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel1, 0);
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1222, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1222, 95);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtSSlipNoTo);
            this.groupBox1.Controls.Add(this.btnSSlipTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtSSlipNoFr);
            this.groupBox1.Controls.Add(this.btnSSlipFr);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.txtInEmpNm);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.txtDeptCd);
            this.groupBox1.Controls.Add(this.BtnDept);
            this.groupBox1.Controls.Add(this.txtDeptNm);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpSlipDtFr);
            this.groupBox1.Controls.Add(this.dtpSlipDtTo);
            this.groupBox1.Controls.Add(this.c1Label12);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.c1Label25);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1222, 95);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtSSlipNoTo
            // 
            this.txtSSlipNoTo.AutoSize = false;
            this.txtSSlipNoTo.BackColor = System.Drawing.Color.White;
            this.txtSSlipNoTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSSlipNoTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSSlipNoTo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSSlipNoTo.Location = new System.Drawing.Point(282, 65);
            this.txtSSlipNoTo.Name = "txtSSlipNoTo";
            this.txtSSlipNoTo.Size = new System.Drawing.Size(104, 21);
            this.txtSSlipNoTo.TabIndex = 16;
            this.txtSSlipNoTo.Tag = ";;;";
            this.txtSSlipNoTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // btnSSlipTo
            // 
            this.btnSSlipTo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSSlipTo.BackgroundImage")));
            this.btnSSlipTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSSlipTo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSSlipTo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSSlipTo.Location = new System.Drawing.Point(386, 65);
            this.btnSSlipTo.Name = "btnSSlipTo";
            this.btnSSlipTo.Size = new System.Drawing.Size(24, 21);
            this.btnSSlipTo.TabIndex = 17;
            this.btnSSlipTo.TabStop = false;
            this.btnSSlipTo.Tag = "";
            this.btnSSlipTo.UseVisualStyleBackColor = true;
            this.btnSSlipTo.Click += new System.EventHandler(this.btnSSlipTo_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(250, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 21);
            this.label2.TabIndex = 15;
            this.label2.Text = "~";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSSlipNoFr
            // 
            this.txtSSlipNoFr.AutoSize = false;
            this.txtSSlipNoFr.BackColor = System.Drawing.Color.White;
            this.txtSSlipNoFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSSlipNoFr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSSlipNoFr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSSlipNoFr.Location = new System.Drawing.Point(116, 65);
            this.txtSSlipNoFr.Name = "txtSSlipNoFr";
            this.txtSSlipNoFr.Size = new System.Drawing.Size(104, 21);
            this.txtSSlipNoFr.TabIndex = 13;
            this.txtSSlipNoFr.Tag = ";;;";
            this.txtSSlipNoFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // btnSSlipFr
            // 
            this.btnSSlipFr.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSSlipFr.BackgroundImage")));
            this.btnSSlipFr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSSlipFr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSSlipFr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSSlipFr.Location = new System.Drawing.Point(220, 65);
            this.btnSSlipFr.Name = "btnSSlipFr";
            this.btnSSlipFr.Size = new System.Drawing.Size(24, 21);
            this.btnSSlipFr.TabIndex = 14;
            this.btnSSlipFr.TabStop = false;
            this.btnSSlipFr.Tag = "";
            this.btnSSlipFr.UseVisualStyleBackColor = true;
            this.btnSSlipFr.Click += new System.EventHandler(this.btnSSlipFr_Click);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(23, 65);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(94, 21);
            this.c1Label1.TabIndex = 12;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "결의번호";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(524, 42);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 10;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "작성자";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // txtInEmpNm
            // 
            this.txtInEmpNm.AutoSize = false;
            this.txtInEmpNm.BackColor = System.Drawing.Color.White;
            this.txtInEmpNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtInEmpNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInEmpNm.Location = new System.Drawing.Point(617, 42);
            this.txtInEmpNm.Name = "txtInEmpNm";
            this.txtInEmpNm.Size = new System.Drawing.Size(241, 21);
            this.txtInEmpNm.TabIndex = 11;
            this.txtInEmpNm.Tag = ";;;";
            this.txtInEmpNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(524, 17);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(94, 21);
            this.c1Label4.TabIndex = 4;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "부서코드";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // txtDeptCd
            // 
            this.txtDeptCd.AutoSize = false;
            this.txtDeptCd.BackColor = System.Drawing.Color.White;
            this.txtDeptCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDeptCd.Location = new System.Drawing.Point(617, 17);
            this.txtDeptCd.Name = "txtDeptCd";
            this.txtDeptCd.Size = new System.Drawing.Size(124, 21);
            this.txtDeptCd.TabIndex = 5;
            this.txtDeptCd.Tag = null;
            this.txtDeptCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtDeptCd.TextChanged += new System.EventHandler(this.txtDeptCd_TextChanged);
            // 
            // BtnDept
            // 
            this.BtnDept.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnDept.BackgroundImage")));
            this.BtnDept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnDept.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDept.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDept.Location = new System.Drawing.Point(741, 17);
            this.BtnDept.Name = "BtnDept";
            this.BtnDept.Size = new System.Drawing.Size(24, 21);
            this.BtnDept.TabIndex = 6;
            this.BtnDept.Tag = "";
            this.BtnDept.UseVisualStyleBackColor = true;
            this.BtnDept.Click += new System.EventHandler(this.btnDept_Click);
            // 
            // txtDeptNm
            // 
            this.txtDeptNm.AutoSize = false;
            this.txtDeptNm.BackColor = System.Drawing.Color.White;
            this.txtDeptNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptNm.Location = new System.Drawing.Point(765, 17);
            this.txtDeptNm.Name = "txtDeptNm";
            this.txtDeptNm.Size = new System.Drawing.Size(273, 21);
            this.txtDeptNm.TabIndex = 7;
            this.txtDeptNm.Tag = ";2;;";
            this.txtDeptNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(243, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpSlipDtFr
            // 
            this.dtpSlipDtFr.AutoSize = false;
            this.dtpSlipDtFr.BackColor = System.Drawing.Color.White;
            this.dtpSlipDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipDtFr.Calendar.DayNameLength = 1;
            this.dtpSlipDtFr.EmptyAsNull = true;
            this.dtpSlipDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSlipDtFr.Location = new System.Drawing.Point(116, 17);
            this.dtpSlipDtFr.Name = "dtpSlipDtFr";
            this.dtpSlipDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtFr.TabIndex = 1;
            this.dtpSlipDtFr.Tag = "결의일자;1;;";
            this.dtpSlipDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // dtpSlipDtTo
            // 
            this.dtpSlipDtTo.AutoSize = false;
            this.dtpSlipDtTo.BackColor = System.Drawing.Color.White;
            this.dtpSlipDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipDtTo.Calendar.DayNameLength = 1;
            this.dtpSlipDtTo.EmptyAsNull = true;
            this.dtpSlipDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSlipDtTo.Location = new System.Drawing.Point(271, 17);
            this.dtpSlipDtTo.Name = "dtpSlipDtTo";
            this.dtpSlipDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtTo.TabIndex = 3;
            this.dtpSlipDtTo.Tag = "결의일자;1;;";
            this.dtpSlipDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label12
            // 
            this.c1Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label12.Location = new System.Drawing.Point(23, 17);
            this.c1Label12.Name = "c1Label12";
            this.c1Label12.Size = new System.Drawing.Size(94, 21);
            this.c1Label12.TabIndex = 0;
            this.c1Label12.Tag = null;
            this.c1Label12.Text = "결의일자";
            this.c1Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label12.TextDetached = true;
            this.c1Label12.Value = "";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.optConfirm_N);
            this.panel3.Controls.Add(this.optConfirm_Y);
            this.panel3.Location = new System.Drawing.Point(116, 41);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(172, 21);
            this.panel3.TabIndex = 9;
            // 
            // optConfirm_N
            // 
            this.optConfirm_N.Checked = true;
            this.optConfirm_N.Location = new System.Drawing.Point(10, 2);
            this.optConfirm_N.Name = "optConfirm_N";
            this.optConfirm_N.Size = new System.Drawing.Size(64, 18);
            this.optConfirm_N.TabIndex = 0;
            this.optConfirm_N.TabStop = true;
            this.optConfirm_N.Text = "미승인";
            this.optConfirm_N.UseVisualStyleBackColor = true;
            // 
            // optConfirm_Y
            // 
            this.optConfirm_Y.Location = new System.Drawing.Point(80, 2);
            this.optConfirm_Y.Name = "optConfirm_Y";
            this.optConfirm_Y.Size = new System.Drawing.Size(69, 18);
            this.optConfirm_Y.TabIndex = 1;
            this.optConfirm_Y.Text = "승인";
            this.optConfirm_Y.UseVisualStyleBackColor = true;
            // 
            // c1Label25
            // 
            this.c1Label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label25.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label25.Location = new System.Drawing.Point(23, 41);
            this.c1Label25.Name = "c1Label25";
            this.c1Label25.Size = new System.Drawing.Size(94, 21);
            this.c1Label25.TabIndex = 8;
            this.c1Label25.Tag = null;
            this.c1Label25.Text = "진행상태";
            this.c1Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label25.TextDetached = true;
            this.c1Label25.Value = "";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 244);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1222, 12);
            this.splitter1.TabIndex = 35;
            this.splitter1.TabStop = false;
            // 
            // btnSlipView
            // 
            this.btnSlipView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSlipView.BackgroundImage")));
            this.btnSlipView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSlipView.Location = new System.Drawing.Point(12, 6);
            this.btnSlipView.Name = "btnSlipView";
            this.btnSlipView.Size = new System.Drawing.Size(86, 25);
            this.btnSlipView.TabIndex = 21;
            this.btnSlipView.Text = "전표조회";
            this.btnSlipView.UseVisualStyleBackColor = true;
            this.btnSlipView.Click += new System.EventHandler(this.btnSlipView_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSlipView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 456);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1222, 40);
            this.panel2.TabIndex = 36;
            // 
            // ACD008
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1222, 655);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACD008";
            this.Text = "결의전표 반제승인";
            this.Load += new System.EventHandler(this.ACD008_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel4, 0);
            this.GridCommPanel2.ResumeLayout(false);
            this.GridCommGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).EndInit();
            this.GridCommPanel1.ResumeLayout(false);
            this.GridCommGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView1)).EndInit();
            this.panel4.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlipNoTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlipNoFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInEmpNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label25)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label25;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton optConfirm_N;
        private System.Windows.Forms.RadioButton optConfirm_Y;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtFr;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtTo;
        private C1.Win.C1Input.C1Label c1Label12;
        private C1.Win.C1Input.C1TextBox txtInEmpNm;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1TextBox txtDeptCd;
        private C1.Win.C1Input.C1Button BtnDept;
        private C1.Win.C1Input.C1TextBox txtDeptNm;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtSSlipNoFr;
        private C1.Win.C1Input.C1Button btnSSlipFr;
        private System.Windows.Forms.Label label2;
        private C1.Win.C1Input.C1TextBox txtSSlipNoTo;
        private C1.Win.C1Input.C1Button btnSSlipTo;
    }
}