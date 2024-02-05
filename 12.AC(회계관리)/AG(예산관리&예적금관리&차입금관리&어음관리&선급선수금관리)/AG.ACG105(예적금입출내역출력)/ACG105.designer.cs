namespace AG.ACG105
{
    partial class ACG105
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACG105));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboCurCd = new C1.Win.C1List.C1Combo();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.dtpSlipDtTo = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.cboAcctPart = new C1.Win.C1List.C1Combo();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.cboBankCd = new C1.Win.C1List.C1Combo();
            this.c1Label8 = new C1.Win.C1Input.C1Label();
            this.dtpSlipDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label10 = new C1.Win.C1Input.C1Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnSlipView = new C1.Win.C1Input.C1Button();
            this.panel2 = new System.Windows.Forms.Panel();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboCurCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAcctPart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBankCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1222, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1222, 591);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboCurCd);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.dtpSlipDtTo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboAcctPart);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.cboBankCd);
            this.groupBox1.Controls.Add(this.c1Label8);
            this.groupBox1.Controls.Add(this.dtpSlipDtFr);
            this.groupBox1.Controls.Add(this.c1Label10);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1222, 591);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cboCurCd
            // 
            this.cboCurCd.AddItemSeparator = ';';
            this.cboCurCd.AutoSize = false;
            this.cboCurCd.Caption = "";
            this.cboCurCd.CaptionHeight = 17;
            this.cboCurCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboCurCd.ColumnCaptionHeight = 18;
            this.cboCurCd.ColumnFooterHeight = 18;
            this.cboCurCd.ContentHeight = 15;
            this.cboCurCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboCurCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboCurCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboCurCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboCurCd.EditorHeight = 15;
            this.cboCurCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboCurCd.Images"))));
            this.cboCurCd.ItemHeight = 15;
            this.cboCurCd.Location = new System.Drawing.Point(115, 65);
            this.cboCurCd.MatchEntryTimeout = ((long)(2000));
            this.cboCurCd.MaxDropDownItems = ((short)(5));
            this.cboCurCd.MaxLength = 32767;
            this.cboCurCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboCurCd.Name = "cboCurCd";
            this.cboCurCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboCurCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboCurCd.Size = new System.Drawing.Size(148, 21);
            this.cboCurCd.TabIndex = 7;
            this.cboCurCd.TabStop = false;
            this.cboCurCd.Tag = "통화;;;";
            this.cboCurCd.PropBag = resources.GetString("cboCurCd.PropBag");
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(22, 65);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(94, 21);
            this.c1Label2.TabIndex = 6;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "통화";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
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
            this.dtpSlipDtTo.Location = new System.Drawing.Point(266, 17);
            this.dtpSlipDtTo.Name = "dtpSlipDtTo";
            this.dtpSlipDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtTo.TabIndex = 3;
            this.dtpSlipDtTo.Tag = "입출일자;1;;";
            this.dtpSlipDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "~";
            // 
            // cboAcctPart
            // 
            this.cboAcctPart.AddItemSeparator = ';';
            this.cboAcctPart.AutoSize = false;
            this.cboAcctPart.Caption = "";
            this.cboAcctPart.CaptionHeight = 17;
            this.cboAcctPart.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboAcctPart.ColumnCaptionHeight = 18;
            this.cboAcctPart.ColumnFooterHeight = 18;
            this.cboAcctPart.ContentHeight = 15;
            this.cboAcctPart.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboAcctPart.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboAcctPart.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboAcctPart.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboAcctPart.EditorHeight = 15;
            this.cboAcctPart.Images.Add(((System.Drawing.Image)(resources.GetObject("cboAcctPart.Images"))));
            this.cboAcctPart.ItemHeight = 15;
            this.cboAcctPart.Location = new System.Drawing.Point(115, 89);
            this.cboAcctPart.MatchEntryTimeout = ((long)(2000));
            this.cboAcctPart.MaxDropDownItems = ((short)(5));
            this.cboAcctPart.MaxLength = 32767;
            this.cboAcctPart.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboAcctPart.Name = "cboAcctPart";
            this.cboAcctPart.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboAcctPart.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboAcctPart.Size = new System.Drawing.Size(148, 21);
            this.cboAcctPart.TabIndex = 9;
            this.cboAcctPart.TabStop = false;
            this.cboAcctPart.Tag = "예적금구분;;;";
            this.cboAcctPart.PropBag = resources.GetString("cboAcctPart.PropBag");
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(22, 89);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(94, 21);
            this.c1Label1.TabIndex = 8;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "예적금구분";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // cboBankCd
            // 
            this.cboBankCd.AddItemSeparator = ';';
            this.cboBankCd.AutoSize = false;
            this.cboBankCd.Caption = "";
            this.cboBankCd.CaptionHeight = 17;
            this.cboBankCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBankCd.ColumnCaptionHeight = 18;
            this.cboBankCd.ColumnFooterHeight = 18;
            this.cboBankCd.ContentHeight = 15;
            this.cboBankCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBankCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBankCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBankCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBankCd.EditorHeight = 15;
            this.cboBankCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBankCd.Images"))));
            this.cboBankCd.ItemHeight = 15;
            this.cboBankCd.Location = new System.Drawing.Point(115, 41);
            this.cboBankCd.MatchEntryTimeout = ((long)(2000));
            this.cboBankCd.MaxDropDownItems = ((short)(5));
            this.cboBankCd.MaxLength = 32767;
            this.cboBankCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBankCd.Name = "cboBankCd";
            this.cboBankCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBankCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBankCd.Size = new System.Drawing.Size(148, 21);
            this.cboBankCd.TabIndex = 5;
            this.cboBankCd.TabStop = false;
            this.cboBankCd.Tag = ";;;";
            this.cboBankCd.PropBag = resources.GetString("cboBankCd.PropBag");
            // 
            // c1Label8
            // 
            this.c1Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label8.Location = new System.Drawing.Point(22, 41);
            this.c1Label8.Name = "c1Label8";
            this.c1Label8.Size = new System.Drawing.Size(94, 21);
            this.c1Label8.TabIndex = 4;
            this.c1Label8.Tag = null;
            this.c1Label8.Text = "은행";
            this.c1Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label8.TextDetached = true;
            this.c1Label8.Value = "";
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
            this.dtpSlipDtFr.Location = new System.Drawing.Point(115, 17);
            this.dtpSlipDtFr.Name = "dtpSlipDtFr";
            this.dtpSlipDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtFr.TabIndex = 1;
            this.dtpSlipDtFr.Tag = "입출일자;1;;";
            this.dtpSlipDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label10
            // 
            this.c1Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label10.Location = new System.Drawing.Point(22, 17);
            this.c1Label10.Name = "c1Label10";
            this.c1Label10.Size = new System.Drawing.Size(94, 21);
            this.c1Label10.TabIndex = 0;
            this.c1Label10.Tag = null;
            this.c1Label10.Text = "입출일자";
            this.c1Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label10.TextDetached = true;
            this.c1Label10.Value = "";
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
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSlipView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 452);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1222, 40);
            this.panel2.TabIndex = 36;
            // 
            // ACG105
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1222, 655);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACG105";
            this.Text = "예적금입출력내역출력";
            this.Load += new System.EventHandler(this.ACG105_Load);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCurCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAcctPart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBankCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1List.C1Combo cboAcctPart;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1List.C1Combo cboBankCd;
        private C1.Win.C1Input.C1Label c1Label8;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtFr;
        private C1.Win.C1Input.C1Label c1Label10;
        private C1.Win.C1List.C1Combo cboCurCd;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtTo;
        private System.Windows.Forms.Label label1;
    }
}