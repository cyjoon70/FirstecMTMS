namespace AF.ACF002
{
    partial class ACF002
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACF002));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpIssueDtTo = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpIssueDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.cboBizAreaCd = new C1.Win.C1List.C1Combo();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
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
            this.groupBox1.Controls.Add(this.dtpIssueDtTo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpIssueDtFr);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.cboBizAreaCd);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1222, 591);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
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
            this.dtpIssueDtTo.Location = new System.Drawing.Point(263, 41);
            this.dtpIssueDtTo.Name = "dtpIssueDtTo";
            this.dtpIssueDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpIssueDtTo.TabIndex = 5;
            this.dtpIssueDtTo.Tag = "발행일;1;;";
            this.dtpIssueDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(238, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 21);
            this.label1.TabIndex = 4;
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
            this.dtpIssueDtFr.Location = new System.Drawing.Point(115, 41);
            this.dtpIssueDtFr.Name = "dtpIssueDtFr";
            this.dtpIssueDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpIssueDtFr.TabIndex = 3;
            this.dtpIssueDtFr.Tag = "발행일;1;;";
            this.dtpIssueDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpIssueDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(22, 41);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 2;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "발행일";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
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
            this.cboBizAreaCd.Location = new System.Drawing.Point(116, 17);
            this.cboBizAreaCd.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCd.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCd.MaxLength = 32767;
            this.cboBizAreaCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCd.Name = "cboBizAreaCd";
            this.cboBizAreaCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCd.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCd.TabIndex = 1;
            this.cboBizAreaCd.Tag = ";;;";
            this.cboBizAreaCd.PropBag = resources.GetString("cboBizAreaCd.PropBag");
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(22, 17);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(94, 21);
            this.c1Label2.TabIndex = 0;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "세금신고사업장";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
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
            // ACF002
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1222, 655);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACF002";
            this.Text = "부가세집계표출력";
            this.Load += new System.EventHandler(this.ACF002_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpIssueDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1List.C1Combo cboBizAreaCd;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtTo;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit dtpIssueDtFr;
        private C1.Win.C1Input.C1Label c1Label3;
    }
}