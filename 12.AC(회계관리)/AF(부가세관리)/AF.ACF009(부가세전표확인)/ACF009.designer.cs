namespace AF.ACF009
{
    partial class ACF009
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACF009));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.optIoFlag_I = new System.Windows.Forms.RadioButton();
            this.optIoFlag_O = new System.Windows.Forms.RadioButton();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.cboBizAreaCd = new C1.Win.C1List.C1Combo();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.dtpIssueDtTo = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpIssueDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.GridCommPanel2.SuspendLayout();
            this.GridCommGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
            this.GridCommPanel1.SuspendLayout();
            this.GridCommGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView1)).BeginInit();
            this.GridCommPanel3.SuspendLayout();
            this.GridCommGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView2)).BeginInit();
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
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommPanel2
            // 
            this.GridCommPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel2.Location = new System.Drawing.Point(0, 386);
            this.GridCommPanel2.Size = new System.Drawing.Size(678, 305);
            // 
            // GridCommGroupBox2
            // 
            this.GridCommGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox2.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox2.Size = new System.Drawing.Size(678, 305);
            this.GridCommGroupBox2.Text = "유형별현황";
            // 
            // fpSpread2
            // 
            this.fpSpread2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread2.Location = new System.Drawing.Point(3, 17);
            this.fpSpread2.Size = new System.Drawing.Size(672, 285);
            this.fpSpread2.TabIndex = 0;
            this.fpSpread2.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread2_SelectionChanged);
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // 
            // GridCommPanel1
            // 
            this.GridCommPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.GridCommPanel1.Location = new System.Drawing.Point(0, 0);
            this.GridCommPanel1.Size = new System.Drawing.Size(685, 379);
            // 
            // GridCommGroupBox1
            // 
            this.GridCommGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox1.Size = new System.Drawing.Size(685, 379);
            this.GridCommGroupBox1.Text = "부가세집계현황";
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(679, 359);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread1_SelectionChanged);
            // 
            // sheetView1
            // 
            this.sheetView1.SheetName = "Sheet1";
            // 
            // GridCommPanel3
            // 
            this.GridCommPanel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.GridCommPanel3.Location = new System.Drawing.Point(685, 0);
            this.GridCommPanel3.Size = new System.Drawing.Size(541, 691);
            // 
            // GridCommGroupBox3
            // 
            this.GridCommGroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox3.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox3.Size = new System.Drawing.Size(541, 691);
            this.GridCommGroupBox3.Text = "거래처별상세현황";
            // 
            // fpSpread3
            // 
            this.fpSpread3.Size = new System.Drawing.Size(525, 664);
            this.fpSpread3.TabIndex = 0;
            // 
            // sheetView2
            // 
            this.sheetView2.SheetName = "Sheet1";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Controls.Add(this.splitter2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 140);
            this.panel4.Size = new System.Drawing.Size(1226, 691);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel3, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel1, 0);
            this.panel4.Controls.SetChildIndex(this.splitter2, 0);
            this.panel4.Controls.SetChildIndex(this.splitter1, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel2, 0);
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1226, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1226, 76);
            this.panel1.TabIndex = 37;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.cboBizAreaCd);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.dtpIssueDtTo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpIssueDtFr);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1226, 76);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.optIoFlag_I);
            this.panel5.Controls.Add(this.optIoFlag_O);
            this.panel5.Location = new System.Drawing.Point(554, 17);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(218, 21);
            this.panel5.TabIndex = 5;
            // 
            // optIoFlag_I
            // 
            this.optIoFlag_I.Checked = true;
            this.optIoFlag_I.Location = new System.Drawing.Point(10, 2);
            this.optIoFlag_I.Name = "optIoFlag_I";
            this.optIoFlag_I.Size = new System.Drawing.Size(57, 18);
            this.optIoFlag_I.TabIndex = 0;
            this.optIoFlag_I.TabStop = true;
            this.optIoFlag_I.Text = "매입";
            this.optIoFlag_I.UseVisualStyleBackColor = true;
            // 
            // optIoFlag_O
            // 
            this.optIoFlag_O.Location = new System.Drawing.Point(148, 2);
            this.optIoFlag_O.Name = "optIoFlag_O";
            this.optIoFlag_O.Size = new System.Drawing.Size(54, 18);
            this.optIoFlag_O.TabIndex = 1;
            this.optIoFlag_O.Text = "매출";
            this.optIoFlag_O.UseVisualStyleBackColor = true;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(461, 17);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(94, 21);
            this.c1Label4.TabIndex = 4;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "매입매출구분";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // cboBizAreaCd
            // 
            this.cboBizAreaCd.AddItemSeparator = ';';
            this.cboBizAreaCd.AutoSize = false;
            this.cboBizAreaCd.Caption = "";
            this.cboBizAreaCd.CaptionHeight = 17;
            this.cboBizAreaCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBizAreaCd.ColumnCaptionHeight = 18;
            this.cboBizAreaCd.ColumnFooterHeight = 18;
            this.cboBizAreaCd.ContentHeight = 15;
            this.cboBizAreaCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBizAreaCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBizAreaCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBizAreaCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBizAreaCd.EditorHeight = 15;
            this.cboBizAreaCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBizAreaCd.Images"))));
            this.cboBizAreaCd.ItemHeight = 15;
            this.cboBizAreaCd.Location = new System.Drawing.Point(106, 41);
            this.cboBizAreaCd.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCd.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCd.MaxLength = 32767;
            this.cboBizAreaCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCd.Name = "cboBizAreaCd";
            this.cboBizAreaCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCd.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCd.TabIndex = 7;
            this.cboBizAreaCd.Tag = "세금신고사업장;;;";
            this.cboBizAreaCd.PropBag = resources.GetString("cboBizAreaCd.PropBag");
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(12, 41);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(94, 21);
            this.c1Label2.TabIndex = 6;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "신고사업장";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
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
            this.dtpIssueDtTo.EmptyAsNull = true;
            this.dtpIssueDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpIssueDtTo.Location = new System.Drawing.Point(253, 17);
            this.dtpIssueDtTo.Name = "dtpIssueDtTo";
            this.dtpIssueDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpIssueDtTo.TabIndex = 3;
            this.dtpIssueDtTo.Tag = "발행일;1;;";
            this.dtpIssueDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(228, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.dtpIssueDtFr.EmptyAsNull = true;
            this.dtpIssueDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpIssueDtFr.Location = new System.Drawing.Point(105, 17);
            this.dtpIssueDtFr.Name = "dtpIssueDtFr";
            this.dtpIssueDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpIssueDtFr.TabIndex = 1;
            this.dtpIssueDtFr.Tag = "발행일;1;;";
            this.dtpIssueDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(12, 17);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 0;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "발행일자";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 379);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(685, 7);
            this.splitter2.TabIndex = 14;
            this.splitter2.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(678, 386);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(7, 305);
            this.splitter1.TabIndex = 15;
            this.splitter1.TabStop = false;
            // 
            // ACF009
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1226, 831);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACF009";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "부가세전표확인";
            this.Load += new System.EventHandler(this.ACF009_Load);
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
            this.GridCommPanel3.ResumeLayout(false);
            this.GridCommGroupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView2)).EndInit();
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
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton optIoFlag_I;
        private System.Windows.Forms.RadioButton optIoFlag_O;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1List.C1Combo cboBizAreaCd;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtTo;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtFr;
        private C1.Win.C1Input.C1Label c1Label3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
    }
}