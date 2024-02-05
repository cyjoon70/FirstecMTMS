namespace PA.PBA162
{
    partial class PBA162
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PBA162));
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboItemType = new C1.Win.C1List.C1Combo();
            this.c1Label7 = new C1.Win.C1Input.C1Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboBOM_NO = new C1.Win.C1List.C1Combo();
            this.dtpSTD_FROM_DT = new C1.Win.C1Input.C1DateEdit();
            this.c1Label14 = new C1.Win.C1Input.C1Label();
            this.c1Label16 = new C1.Win.C1Input.C1Label();
            this.btnPLANT_CD = new C1.Win.C1Input.C1Button();
            this.txtPLANT_NM = new C1.Win.C1Input.C1TextBox();
            this.txtPLANT_CD = new C1.Win.C1Input.C1TextBox();
            this.btnITEM_CD = new C1.Win.C1Input.C1Button();
            this.txtITEM_NM = new C1.Win.C1Input.C1TextBox();
            this.txtITEM_CD = new C1.Win.C1Input.C1TextBox();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboItemType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboBOM_NO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSTD_FROM_DT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPLANT_NM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPLANT_CD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtITEM_NM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtITEM_CD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(768, 365);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 162);
            this.GridCommPanel.Size = new System.Drawing.Size(768, 365);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(762, 345);
            this.fpSpread1.TabIndex = 0;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(768, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 98);
            this.panel1.TabIndex = 5;
            // 
            // cboItemType
            // 
            this.cboItemType.AddItemSeparator = ';';
            this.cboItemType.AutoSize = false;
            this.cboItemType.Caption = "";
            this.cboItemType.CaptionHeight = 17;
            this.cboItemType.CaptionVisible = false;
            this.cboItemType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboItemType.ColumnCaptionHeight = 18;
            this.cboItemType.ColumnFooterHeight = 18;
            this.cboItemType.ContentHeight = 15;
            this.cboItemType.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboItemType.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboItemType.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboItemType.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboItemType.EditorHeight = 15;
            this.cboItemType.Images.Add(((System.Drawing.Image)(resources.GetObject("cboItemType.Images"))));
            this.cboItemType.ItemHeight = 15;
            this.cboItemType.Location = new System.Drawing.Point(93, 69);
            this.cboItemType.MatchEntryTimeout = ((long)(2000));
            this.cboItemType.MaxDropDownItems = ((short)(5));
            this.cboItemType.MaxLength = 32767;
            this.cboItemType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboItemType.Name = "cboItemType";
            this.cboItemType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboItemType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboItemType.Size = new System.Drawing.Size(104, 21);
            this.cboItemType.TabIndex = 13;
            this.cboItemType.Tag = "";
            this.cboItemType.PropBag = resources.GetString("cboItemType.PropBag");
            // 
            // c1Label7
            // 
            this.c1Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label7.Location = new System.Drawing.Point(11, 69);
            this.c1Label7.Name = "c1Label7";
            this.c1Label7.Size = new System.Drawing.Size(83, 21);
            this.c1Label7.TabIndex = 12;
            this.c1Label7.Tag = null;
            this.c1Label7.Text = "품목구분";
            this.c1Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label7.TextDetached = true;
            this.c1Label7.Value = "";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboItemType);
            this.groupBox1.Controls.Add(this.cboBOM_NO);
            this.groupBox1.Controls.Add(this.dtpSTD_FROM_DT);
            this.groupBox1.Controls.Add(this.c1Label14);
            this.groupBox1.Controls.Add(this.c1Label16);
            this.groupBox1.Controls.Add(this.btnPLANT_CD);
            this.groupBox1.Controls.Add(this.txtPLANT_NM);
            this.groupBox1.Controls.Add(this.c1Label7);
            this.groupBox1.Controls.Add(this.txtPLANT_CD);
            this.groupBox1.Controls.Add(this.btnITEM_CD);
            this.groupBox1.Controls.Add(this.txtITEM_NM);
            this.groupBox1.Controls.Add(this.txtITEM_CD);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(768, 98);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cboBOM_NO
            // 
            this.cboBOM_NO.AddItemSeparator = ';';
            this.cboBOM_NO.AutoSize = false;
            this.cboBOM_NO.Caption = "";
            this.cboBOM_NO.CaptionHeight = 17;
            this.cboBOM_NO.CaptionVisible = false;
            this.cboBOM_NO.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBOM_NO.ColumnCaptionHeight = 18;
            this.cboBOM_NO.ColumnFooterHeight = 18;
            this.cboBOM_NO.ContentHeight = 15;
            this.cboBOM_NO.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBOM_NO.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBOM_NO.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBOM_NO.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBOM_NO.EditorHeight = 15;
            this.cboBOM_NO.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBOM_NO.Images"))));
            this.cboBOM_NO.ItemHeight = 15;
            this.cboBOM_NO.Location = new System.Drawing.Point(464, 42);
            this.cboBOM_NO.MatchEntryTimeout = ((long)(2000));
            this.cboBOM_NO.MaxDropDownItems = ((short)(5));
            this.cboBOM_NO.MaxLength = 32767;
            this.cboBOM_NO.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBOM_NO.Name = "cboBOM_NO";
            this.cboBOM_NO.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBOM_NO.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBOM_NO.Size = new System.Drawing.Size(106, 21);
            this.cboBOM_NO.TabIndex = 11;
            this.cboBOM_NO.Tag = "BOM Type;1;;";
            this.cboBOM_NO.PropBag = resources.GetString("cboBOM_NO.PropBag");
            // 
            // dtpSTD_FROM_DT
            // 
            this.dtpSTD_FROM_DT.AutoSize = false;
            this.dtpSTD_FROM_DT.BackColor = System.Drawing.Color.White;
            this.dtpSTD_FROM_DT.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSTD_FROM_DT.Calendar.DayNameLength = 1;
            this.dtpSTD_FROM_DT.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSTD_FROM_DT.Location = new System.Drawing.Point(464, 16);
            this.dtpSTD_FROM_DT.Name = "dtpSTD_FROM_DT";
            this.dtpSTD_FROM_DT.Size = new System.Drawing.Size(106, 21);
            this.dtpSTD_FROM_DT.TabIndex = 5;
            this.dtpSTD_FROM_DT.Tag = "기준일;1;;";
            this.dtpSTD_FROM_DT.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSTD_FROM_DT.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label14
            // 
            this.c1Label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label14.Location = new System.Drawing.Point(382, 42);
            this.c1Label14.Name = "c1Label14";
            this.c1Label14.Size = new System.Drawing.Size(83, 21);
            this.c1Label14.TabIndex = 10;
            this.c1Label14.Tag = null;
            this.c1Label14.Text = "BOM Type";
            this.c1Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label14.TextDetached = true;
            this.c1Label14.Value = "";
            // 
            // c1Label16
            // 
            this.c1Label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label16.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label16.Location = new System.Drawing.Point(382, 15);
            this.c1Label16.Name = "c1Label16";
            this.c1Label16.Size = new System.Drawing.Size(83, 21);
            this.c1Label16.TabIndex = 4;
            this.c1Label16.Tag = null;
            this.c1Label16.Text = "기준일";
            this.c1Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label16.TextDetached = true;
            this.c1Label16.Value = "";
            // 
            // btnPLANT_CD
            // 
            this.btnPLANT_CD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPLANT_CD.BackgroundImage")));
            this.btnPLANT_CD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPLANT_CD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPLANT_CD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPLANT_CD.Location = new System.Drawing.Point(173, 15);
            this.btnPLANT_CD.Name = "btnPLANT_CD";
            this.btnPLANT_CD.Size = new System.Drawing.Size(24, 21);
            this.btnPLANT_CD.TabIndex = 2;
            this.btnPLANT_CD.UseVisualStyleBackColor = true;
            this.btnPLANT_CD.Click += new System.EventHandler(this.btnPLANT_CD_Click);
            // 
            // txtPLANT_NM
            // 
            this.txtPLANT_NM.AutoSize = false;
            this.txtPLANT_NM.BackColor = System.Drawing.Color.White;
            this.txtPLANT_NM.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtPLANT_NM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPLANT_NM.Location = new System.Drawing.Point(196, 15);
            this.txtPLANT_NM.Name = "txtPLANT_NM";
            this.txtPLANT_NM.Size = new System.Drawing.Size(176, 21);
            this.txtPLANT_NM.TabIndex = 3;
            this.txtPLANT_NM.Tag = ";2;;";
            // 
            // txtPLANT_CD
            // 
            this.txtPLANT_CD.AutoSize = false;
            this.txtPLANT_CD.BackColor = System.Drawing.Color.White;
            this.txtPLANT_CD.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtPLANT_CD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPLANT_CD.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPLANT_CD.Location = new System.Drawing.Point(93, 15);
            this.txtPLANT_CD.Name = "txtPLANT_CD";
            this.txtPLANT_CD.Size = new System.Drawing.Size(80, 21);
            this.txtPLANT_CD.TabIndex = 1;
            this.txtPLANT_CD.Tag = "공장;1;;";
            this.txtPLANT_CD.TextChanged += new System.EventHandler(this.txtPLANT_CD_TextChanged);
            // 
            // btnITEM_CD
            // 
            this.btnITEM_CD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnITEM_CD.BackgroundImage")));
            this.btnITEM_CD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnITEM_CD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnITEM_CD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnITEM_CD.Location = new System.Drawing.Point(173, 42);
            this.btnITEM_CD.Name = "btnITEM_CD";
            this.btnITEM_CD.Size = new System.Drawing.Size(24, 21);
            this.btnITEM_CD.TabIndex = 8;
            this.btnITEM_CD.UseVisualStyleBackColor = true;
            this.btnITEM_CD.Click += new System.EventHandler(this.btnITEM_CD_Click);
            // 
            // txtITEM_NM
            // 
            this.txtITEM_NM.AutoSize = false;
            this.txtITEM_NM.BackColor = System.Drawing.Color.White;
            this.txtITEM_NM.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtITEM_NM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtITEM_NM.Location = new System.Drawing.Point(196, 42);
            this.txtITEM_NM.Name = "txtITEM_NM";
            this.txtITEM_NM.Size = new System.Drawing.Size(176, 21);
            this.txtITEM_NM.TabIndex = 9;
            this.txtITEM_NM.Tag = ";2;;";
            // 
            // txtITEM_CD
            // 
            this.txtITEM_CD.AutoSize = false;
            this.txtITEM_CD.BackColor = System.Drawing.Color.White;
            this.txtITEM_CD.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtITEM_CD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtITEM_CD.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtITEM_CD.Location = new System.Drawing.Point(93, 42);
            this.txtITEM_CD.Name = "txtITEM_CD";
            this.txtITEM_CD.Size = new System.Drawing.Size(80, 21);
            this.txtITEM_CD.TabIndex = 7;
            this.txtITEM_CD.Tag = "품목;1;;";
            this.txtITEM_CD.TextChanged += new System.EventHandler(this.txtITEM_CD_TextChanged);
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(11, 42);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(83, 21);
            this.c1Label6.TabIndex = 6;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "품목";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(11, 15);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(83, 21);
            this.c1Label5.TabIndex = 0;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "공장";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // PBA162
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(768, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PBA162";
            this.Text = "PART LIST";
            this.Load += new System.EventHandler(this.PBA162_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboItemType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboBOM_NO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSTD_FROM_DT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPLANT_NM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPLANT_CD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtITEM_NM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtITEM_CD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Label c1Label14;
        private C1.Win.C1Input.C1Label c1Label16;
        private C1.Win.C1Input.C1Button btnPLANT_CD;
        private C1.Win.C1Input.C1TextBox txtPLANT_NM;
        private C1.Win.C1Input.C1TextBox txtPLANT_CD;
        private C1.Win.C1Input.C1Button btnITEM_CD;
        private C1.Win.C1Input.C1TextBox txtITEM_NM;
        private C1.Win.C1Input.C1TextBox txtITEM_CD;
        private C1.Win.C1List.C1Combo cboBOM_NO;
        private C1.Win.C1Input.C1DateEdit dtpSTD_FROM_DT;
        private C1.Win.C1List.C1Combo cboItemType;
        private C1.Win.C1Input.C1Label c1Label7;

    }
}