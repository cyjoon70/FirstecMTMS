namespace CS.CSA001
{
    partial class CSA001
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSA001));
            this.panel1 = new System.Windows.Forms.Panel();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFile = new C1.Win.C1Input.C1Button();
            this.txtFilePath = new C1.Win.C1Input.C1TextBox();
            this.btnFileDownload = new C1.Win.C1Input.C1Button();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.cboContSeq = new C1.Win.C1List.C1Combo();
            this.btnFileUpload = new C1.Win.C1Input.C1Button();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.btnProj = new C1.Win.C1Input.C1Button();
            this.txtProjNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.dtpCont_App_Dt = new C1.Win.C1Input.C1DateEdit();
            this.txtProjNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboContSeq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpCont_App_Dt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(784, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fpSpread1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 463);
            this.panel1.TabIndex = 5;
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1";
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fpSpread1.AutoClipboard = false;
            this.fpSpread1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fpSpread1.Location = new System.Drawing.Point(12, 226);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(760, 225);
            this.fpSpread1.TabIndex = 3;
            this.fpSpread1.Visible = false;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnFile);
            this.groupBox1.Controls.Add(this.txtFilePath);
            this.groupBox1.Controls.Add(this.btnFileDownload);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.cboContSeq);
            this.groupBox1.Controls.Add(this.btnFileUpload);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.btnProj);
            this.groupBox1.Controls.Add(this.txtProjNo);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.dtpCont_App_Dt);
            this.groupBox1.Controls.Add(this.txtProjNm);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnFile
            // 
            this.btnFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFile.BackgroundImage")));
            this.btnFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFile.Location = new System.Drawing.Point(424, 49);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(24, 21);
            this.btnFile.TabIndex = 6;
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.AutoSize = false;
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilePath.Location = new System.Drawing.Point(123, 49);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(301, 21);
            this.txtFilePath.TabIndex = 5;
            this.txtFilePath.Tag = ";1;;";
            // 
            // btnFileDownload
            // 
            this.btnFileDownload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileDownload.BackgroundImage")));
            this.btnFileDownload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileDownload.Location = new System.Drawing.Point(169, 169);
            this.btnFileDownload.Name = "btnFileDownload";
            this.btnFileDownload.Size = new System.Drawing.Size(120, 25);
            this.btnFileDownload.TabIndex = 12;
            this.btnFileDownload.Text = "양식 DOWNLOAD";
            this.btnFileDownload.UseVisualStyleBackColor = true;
            this.btnFileDownload.Click += new System.EventHandler(this.btnFileDownload_Click);
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(234, 76);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(104, 21);
            this.c1Label2.TabIndex = 9;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "적용일자";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // cboContSeq
            // 
            this.cboContSeq.AddItemSeparator = ';';
            this.cboContSeq.AutoSize = false;
            this.cboContSeq.Caption = "";
            this.cboContSeq.CaptionHeight = 17;
            this.cboContSeq.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboContSeq.ColumnCaptionHeight = 18;
            this.cboContSeq.ColumnFooterHeight = 18;
            this.cboContSeq.ContentHeight = 15;
            this.cboContSeq.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboContSeq.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboContSeq.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboContSeq.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboContSeq.EditorHeight = 15;
            this.cboContSeq.Images.Add(((System.Drawing.Image)(resources.GetObject("cboContSeq.Images"))));
            this.cboContSeq.ItemHeight = 15;
            this.cboContSeq.Location = new System.Drawing.Point(124, 76);
            this.cboContSeq.MatchEntryTimeout = ((long)(2000));
            this.cboContSeq.MaxDropDownItems = ((short)(5));
            this.cboContSeq.MaxLength = 32767;
            this.cboContSeq.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboContSeq.Name = "cboContSeq";
            this.cboContSeq.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboContSeq.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboContSeq.Size = new System.Drawing.Size(99, 21);
            this.cboContSeq.TabIndex = 8;
            this.cboContSeq.Tag = ";1;;";
            this.cboContSeq.PropBag = resources.GetString("cboContSeq.PropBag");
            // 
            // btnFileUpload
            // 
            this.btnFileUpload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileUpload.BackgroundImage")));
            this.btnFileUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileUpload.Location = new System.Drawing.Point(20, 169);
            this.btnFileUpload.Name = "btnFileUpload";
            this.btnFileUpload.Size = new System.Drawing.Size(120, 25);
            this.btnFileUpload.TabIndex = 11;
            this.btnFileUpload.Text = "파일 UPLOAD";
            this.btnFileUpload.UseVisualStyleBackColor = true;
            this.btnFileUpload.Click += new System.EventHandler(this.btnFileUpload_Click);
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(20, 49);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(104, 21);
            this.c1Label5.TabIndex = 4;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "파일선택";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // btnProj
            // 
            this.btnProj.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnProj.BackgroundImage")));
            this.btnProj.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnProj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnProj.Location = new System.Drawing.Point(223, 22);
            this.btnProj.Name = "btnProj";
            this.btnProj.Size = new System.Drawing.Size(24, 21);
            this.btnProj.TabIndex = 2;
            this.btnProj.UseVisualStyleBackColor = true;
            this.btnProj.Click += new System.EventHandler(this.btnItem_Click_1);
            // 
            // txtProjNo
            // 
            this.txtProjNo.AutoSize = false;
            this.txtProjNo.BackColor = System.Drawing.Color.White;
            this.txtProjNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProjNo.Location = new System.Drawing.Point(123, 22);
            this.txtProjNo.Name = "txtProjNo";
            this.txtProjNo.Size = new System.Drawing.Size(100, 21);
            this.txtProjNo.TabIndex = 1;
            this.txtProjNo.Tag = ";1;;";
            this.txtProjNo.TextChanged += new System.EventHandler(this.btnProj_TextChanged);
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(20, 76);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(104, 21);
            this.c1Label3.TabIndex = 7;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "수정계약차수";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // dtpCont_App_Dt
            // 
            this.dtpCont_App_Dt.AutoSize = false;
            this.dtpCont_App_Dt.BackColor = System.Drawing.Color.White;
            this.dtpCont_App_Dt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpCont_App_Dt.Calendar.DayNameLength = 1;
            this.dtpCont_App_Dt.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpCont_App_Dt.Location = new System.Drawing.Point(338, 76);
            this.dtpCont_App_Dt.Name = "dtpCont_App_Dt";
            this.dtpCont_App_Dt.Size = new System.Drawing.Size(107, 21);
            this.dtpCont_App_Dt.TabIndex = 10;
            this.dtpCont_App_Dt.Tag = ";1;;";
            this.dtpCont_App_Dt.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpCont_App_Dt.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // txtProjNm
            // 
            this.txtProjNm.AutoSize = false;
            this.txtProjNm.BackColor = System.Drawing.Color.White;
            this.txtProjNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjNm.Location = new System.Drawing.Point(247, 22);
            this.txtProjNm.Name = "txtProjNm";
            this.txtProjNm.Size = new System.Drawing.Size(201, 21);
            this.txtProjNm.TabIndex = 3;
            this.txtProjNm.Tag = ";2;;";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(20, 22);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(104, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "프로젝트번호";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // CSA001
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CSA001";
            this.Text = "계약원가 UPLOAD";
            this.Activated += new System.EventHandler(this.CSA001_Activated);
            this.Deactivate += new System.EventHandler(this.CSA001_Deactivate);
            this.Load += new System.EventHandler(this.CSA001_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
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
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboContSeq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpCont_App_Dt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtProjNm;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Button btnProj;
        private C1.Win.C1Input.C1TextBox txtProjNo;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1DateEdit dtpCont_App_Dt;
        private C1.Win.C1Input.C1Button btnFileUpload;
        private C1.Win.C1List.C1Combo cboContSeq;
        private C1.Win.C1Input.C1Button btnFileDownload;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1Button btnFile;
        private C1.Win.C1Input.C1TextBox txtFilePath;
        public FarPoint.Win.Spread.FpSpread fpSpread1;
        public FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;

    }
}