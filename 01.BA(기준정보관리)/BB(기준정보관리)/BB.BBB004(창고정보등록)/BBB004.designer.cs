namespace BB.BBB004
{
    partial class BBB004
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BBB004));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSSlNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.cboSPlantCd = new C1.Win.C1List.C1Combo();
            this.txtSSlCd = new C1.Win.C1Input.C1TextBox();
            this.c1Label24 = new C1.Win.C1Input.C1Label();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkGbn = new C1.Win.C1Input.C1CheckBox();
            this.cboPlantCd = new C1.Win.C1List.C1Combo();
            this.cboSlType = new C1.Win.C1List.C1Combo();
            this.c1Label36 = new C1.Win.C1Input.C1Label();
            this.c1Label32 = new C1.Win.C1Input.C1Label();
            this.c1Label26 = new C1.Win.C1Input.C1Label();
            this.txtSlNm = new C1.Win.C1Input.C1TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.c1Label22 = new C1.Win.C1Input.C1Label();
            this.txtSlCd = new C1.Win.C1Input.C1TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSPlantCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSlType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label36)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlNm)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlCd)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 74);
            this.GridCommGroupBox.Size = new System.Drawing.Size(480, 467);
            this.GridCommGroupBox.TabIndex = 1;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Controls.Add(this.groupBox1);
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 64);
            this.GridCommPanel.Size = new System.Drawing.Size(480, 541);
            this.GridCommPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.GridCommPanel.Controls.SetChildIndex(this.GridCommGroupBox, 0);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(474, 447);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread1_SelectionChanged);
            this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(925, 64);
            this.panButton1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSSlNm);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.cboSPlantCd);
            this.groupBox1.Controls.Add(this.txtSSlCd);
            this.groupBox1.Controls.Add(this.c1Label24);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 74);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtSSlNm
            // 
            this.txtSSlNm.AutoSize = false;
            this.txtSSlNm.BackColor = System.Drawing.Color.White;
            this.txtSSlNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSSlNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSSlNm.Location = new System.Drawing.Point(259, 44);
            this.txtSSlNm.Name = "txtSSlNm";
            this.txtSSlNm.Size = new System.Drawing.Size(210, 21);
            this.txtSSlNm.TabIndex = 33;
            this.txtSSlNm.Tag = null;
            this.txtSSlNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(168, 44);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(92, 21);
            this.c1Label1.TabIndex = 32;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "창고명";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // cboSPlantCd
            // 
            this.cboSPlantCd.AddItemSeparator = ';';
            this.cboSPlantCd.AutoSize = false;
            this.cboSPlantCd.Caption = "";
            this.cboSPlantCd.CaptionHeight = 17;
            this.cboSPlantCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboSPlantCd.ColumnCaptionHeight = 18;
            this.cboSPlantCd.ColumnFooterHeight = 18;
            this.cboSPlantCd.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;
            this.cboSPlantCd.ContentHeight = 15;
            this.cboSPlantCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboSPlantCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboSPlantCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSPlantCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboSPlantCd.EditorHeight = 15;
            this.cboSPlantCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboSPlantCd.Images"))));
            this.cboSPlantCd.ItemHeight = 15;
            this.cboSPlantCd.Location = new System.Drawing.Point(99, 15);
            this.cboSPlantCd.MatchEntryTimeout = ((long)(2000));
            this.cboSPlantCd.MaxDropDownItems = ((short)(5));
            this.cboSPlantCd.MaxLength = 32767;
            this.cboSPlantCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboSPlantCd.Name = "cboSPlantCd";
            this.cboSPlantCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboSPlantCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboSPlantCd.Size = new System.Drawing.Size(148, 21);
            this.cboSPlantCd.TabIndex = 31;
            this.cboSPlantCd.Tag = ";1;;";
            this.cboSPlantCd.PropBag = resources.GetString("cboSPlantCd.PropBag");
            // 
            // txtSSlCd
            // 
            this.txtSSlCd.AutoSize = false;
            this.txtSSlCd.BackColor = System.Drawing.Color.White;
            this.txtSSlCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSSlCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSSlCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSSlCd.Location = new System.Drawing.Point(98, 44);
            this.txtSSlCd.Name = "txtSSlCd";
            this.txtSSlCd.Size = new System.Drawing.Size(62, 21);
            this.txtSSlCd.TabIndex = 3;
            this.txtSSlCd.Tag = null;
            this.txtSSlCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label24
            // 
            this.c1Label24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label24.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label24.Location = new System.Drawing.Point(7, 44);
            this.c1Label24.Name = "c1Label24";
            this.c1Label24.Size = new System.Drawing.Size(92, 21);
            this.c1Label24.TabIndex = 2;
            this.c1Label24.Tag = null;
            this.c1Label24.Text = "창고코드";
            this.c1Label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label24.TextDetached = true;
            this.c1Label24.Value = "";
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(7, 15);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(92, 21);
            this.c1Label5.TabIndex = 0;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "공장코드";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(480, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(445, 541);
            this.panel1.TabIndex = 18;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkGbn);
            this.groupBox3.Controls.Add(this.cboPlantCd);
            this.groupBox3.Controls.Add(this.cboSlType);
            this.groupBox3.Controls.Add(this.c1Label36);
            this.groupBox3.Controls.Add(this.c1Label32);
            this.groupBox3.Controls.Add(this.c1Label26);
            this.groupBox3.Controls.Add(this.txtSlNm);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 74);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 467);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // chkGbn
            // 
            this.chkGbn.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkGbn.Location = new System.Drawing.Point(12, 114);
            this.chkGbn.Name = "chkGbn";
            this.chkGbn.Size = new System.Drawing.Size(132, 20);
            this.chkGbn.TabIndex = 39;
            this.chkGbn.Text = "MRP재고감안여부";
            this.chkGbn.Value = null;
            // 
            // cboPlantCd
            // 
            this.cboPlantCd.AddItemSeparator = ';';
            this.cboPlantCd.AutoSize = false;
            this.cboPlantCd.Caption = "";
            this.cboPlantCd.CaptionHeight = 17;
            this.cboPlantCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboPlantCd.ColumnCaptionHeight = 18;
            this.cboPlantCd.ColumnFooterHeight = 18;
            this.cboPlantCd.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;
            this.cboPlantCd.ContentHeight = 15;
            this.cboPlantCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboPlantCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboPlantCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPlantCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboPlantCd.EditorHeight = 15;
            this.cboPlantCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboPlantCd.Images"))));
            this.cboPlantCd.ItemHeight = 15;
            this.cboPlantCd.Location = new System.Drawing.Point(104, 85);
            this.cboPlantCd.MatchEntryTimeout = ((long)(2000));
            this.cboPlantCd.MaxDropDownItems = ((short)(5));
            this.cboPlantCd.MaxLength = 32767;
            this.cboPlantCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboPlantCd.Name = "cboPlantCd";
            this.cboPlantCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboPlantCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboPlantCd.Size = new System.Drawing.Size(160, 21);
            this.cboPlantCd.TabIndex = 30;
            this.cboPlantCd.Tag = ";1;;";
            this.cboPlantCd.PropBag = resources.GetString("cboPlantCd.PropBag");
            // 
            // cboSlType
            // 
            this.cboSlType.AddItemSeparator = ';';
            this.cboSlType.AutoSize = false;
            this.cboSlType.Caption = "";
            this.cboSlType.CaptionHeight = 17;
            this.cboSlType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboSlType.ColumnCaptionHeight = 18;
            this.cboSlType.ColumnFooterHeight = 18;
            this.cboSlType.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;
            this.cboSlType.ContentHeight = 15;
            this.cboSlType.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboSlType.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboSlType.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSlType.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboSlType.EditorHeight = 15;
            this.cboSlType.Images.Add(((System.Drawing.Image)(resources.GetObject("cboSlType.Images"))));
            this.cboSlType.ItemHeight = 15;
            this.cboSlType.Location = new System.Drawing.Point(104, 53);
            this.cboSlType.MatchEntryTimeout = ((long)(2000));
            this.cboSlType.MaxDropDownItems = ((short)(5));
            this.cboSlType.MaxLength = 32767;
            this.cboSlType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboSlType.Name = "cboSlType";
            this.cboSlType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboSlType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboSlType.Size = new System.Drawing.Size(160, 21);
            this.cboSlType.TabIndex = 29;
            this.cboSlType.Tag = ";1;;";
            this.cboSlType.PropBag = resources.GetString("cboSlType.PropBag");
            // 
            // c1Label36
            // 
            this.c1Label36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label36.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label36.Location = new System.Drawing.Point(12, 85);
            this.c1Label36.Name = "c1Label36";
            this.c1Label36.Size = new System.Drawing.Size(92, 21);
            this.c1Label36.TabIndex = 8;
            this.c1Label36.Tag = null;
            this.c1Label36.Text = "공장코드";
            this.c1Label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label36.TextDetached = true;
            this.c1Label36.Value = "";
            // 
            // c1Label32
            // 
            this.c1Label32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label32.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label32.Location = new System.Drawing.Point(12, 53);
            this.c1Label32.Name = "c1Label32";
            this.c1Label32.Size = new System.Drawing.Size(92, 21);
            this.c1Label32.TabIndex = 4;
            this.c1Label32.Tag = null;
            this.c1Label32.Text = "창고구분";
            this.c1Label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label32.TextDetached = true;
            this.c1Label32.Value = "";
            // 
            // c1Label26
            // 
            this.c1Label26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label26.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label26.Location = new System.Drawing.Point(12, 22);
            this.c1Label26.Name = "c1Label26";
            this.c1Label26.Size = new System.Drawing.Size(92, 21);
            this.c1Label26.TabIndex = 0;
            this.c1Label26.Tag = null;
            this.c1Label26.Text = "창고명";
            this.c1Label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label26.TextDetached = true;
            this.c1Label26.Value = "";
            // 
            // txtSlNm
            // 
            this.txtSlNm.AutoSize = false;
            this.txtSlNm.BackColor = System.Drawing.Color.White;
            this.txtSlNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSlNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSlNm.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSlNm.Location = new System.Drawing.Point(103, 22);
            this.txtSlNm.Name = "txtSlNm";
            this.txtSlNm.Size = new System.Drawing.Size(160, 21);
            this.txtSlNm.TabIndex = 1;
            this.txtSlNm.Tag = "공장명;1;;";
            this.txtSlNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.c1Label22);
            this.groupBox2.Controls.Add(this.txtSlCd);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(445, 74);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // c1Label22
            // 
            this.c1Label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label22.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label22.Location = new System.Drawing.Point(12, 28);
            this.c1Label22.Name = "c1Label22";
            this.c1Label22.Size = new System.Drawing.Size(92, 21);
            this.c1Label22.TabIndex = 0;
            this.c1Label22.Tag = null;
            this.c1Label22.Text = "창고코드";
            this.c1Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label22.TextDetached = true;
            this.c1Label22.Value = "";
            // 
            // txtSlCd
            // 
            this.txtSlCd.AutoSize = false;
            this.txtSlCd.BackColor = System.Drawing.Color.White;
            this.txtSlCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSlCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSlCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSlCd.Location = new System.Drawing.Point(103, 28);
            this.txtSlCd.Name = "txtSlCd";
            this.txtSlCd.Size = new System.Drawing.Size(160, 21);
            this.txtSlCd.TabIndex = 1;
            this.txtSlCd.Tag = "거래처코드;1;;";
            this.txtSlCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.splitter1.Location = new System.Drawing.Point(480, 64);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(6, 541);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // BBB004
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(925, 605);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BBB004";
            this.Text = "공장정보등록";
            this.Load += new System.EventHandler(this.BBB004_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.GridCommPanel, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.splitter1, 0);
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
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSPlantCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSSlCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSlType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label36)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlNm)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlCd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private C1.Win.C1Input.C1Label c1Label24;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1TextBox txtSSlCd;
        private C1.Win.C1Input.C1Label c1Label22;
        private C1.Win.C1Input.C1TextBox txtSlCd;
        private C1.Win.C1Input.C1Label c1Label26;
        private C1.Win.C1Input.C1TextBox txtSlNm;
        private C1.Win.C1Input.C1Label c1Label36;
        private C1.Win.C1Input.C1Label c1Label32;
        private C1.Win.C1List.C1Combo cboPlantCd;
        private C1.Win.C1List.C1Combo cboSlType;
        private C1.Win.C1Input.C1TextBox txtSSlNm;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1List.C1Combo cboSPlantCd;
        private C1.Win.C1Input.C1CheckBox chkGbn;


    }
}