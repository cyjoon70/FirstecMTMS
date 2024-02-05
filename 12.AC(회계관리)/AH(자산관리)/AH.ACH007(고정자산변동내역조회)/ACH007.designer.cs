namespace AH.ACH007
{
    partial class ACH007
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACH007));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpSlipDtFr = new C1.Win.C1Input.C1DateEdit();
            this.dtpSlipDtTo = new C1.Win.C1Input.C1DateEdit();
            this.c1Label12 = new C1.Win.C1Input.C1Label();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.txtAcctCd = new C1.Win.C1Input.C1TextBox();
            this.btnAcct = new C1.Win.C1Input.C1Button();
            this.txtAcctNm = new C1.Win.C1Input.C1TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctNm)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommPanel2
            // 
            this.GridCommPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.GridCommPanel2.Location = new System.Drawing.Point(0, 0);
            this.GridCommPanel2.Size = new System.Drawing.Size(702, 588);
            // 
            // GridCommGroupBox2
            // 
            this.GridCommGroupBox2.Size = new System.Drawing.Size(686, 569);
            // 
            // fpSpread2
            // 
            this.fpSpread2.Size = new System.Drawing.Size(670, 542);
            this.fpSpread2.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread2_SelectionChanged);
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // 
            // GridCommPanel1
            // 
            this.GridCommPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel1.Location = new System.Drawing.Point(709, 0);
            this.GridCommPanel1.Size = new System.Drawing.Size(779, 588);
            // 
            // GridCommGroupBox1
            // 
            this.GridCommGroupBox1.Size = new System.Drawing.Size(763, 569);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(757, 549);
            // 
            // sheetView1
            // 
            this.sheetView1.SheetName = "Sheet1";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 112);
            this.panel4.Size = new System.Drawing.Size(1488, 588);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel2, 0);
            this.panel4.Controls.SetChildIndex(this.splitter1, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel1, 0);
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1488, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1488, 48);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpSlipDtFr);
            this.groupBox1.Controls.Add(this.dtpSlipDtTo);
            this.groupBox1.Controls.Add(this.c1Label12);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.txtAcctCd);
            this.groupBox1.Controls.Add(this.btnAcct);
            this.groupBox1.Controls.Add(this.txtAcctNm);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1488, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(232, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 21);
            this.label1.TabIndex = 10;
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
            this.dtpSlipDtFr.Location = new System.Drawing.Point(105, 17);
            this.dtpSlipDtFr.Name = "dtpSlipDtFr";
            this.dtpSlipDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtFr.TabIndex = 9;
            this.dtpSlipDtFr.Tag = "취득기간;1;;";
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
            this.dtpSlipDtTo.Location = new System.Drawing.Point(260, 17);
            this.dtpSlipDtTo.Name = "dtpSlipDtTo";
            this.dtpSlipDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtTo.TabIndex = 11;
            this.dtpSlipDtTo.Tag = "취득기간;1;;";
            this.dtpSlipDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label12
            // 
            this.c1Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label12.Location = new System.Drawing.Point(12, 17);
            this.c1Label12.Name = "c1Label12";
            this.c1Label12.Size = new System.Drawing.Size(94, 21);
            this.c1Label12.TabIndex = 8;
            this.c1Label12.Tag = null;
            this.c1Label12.Text = "취득일자";
            this.c1Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label12.TextDetached = true;
            this.c1Label12.Value = "";
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
            this.c1Label4.Text = "계정코드";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // txtAcctCd
            // 
            this.txtAcctCd.AutoSize = false;
            this.txtAcctCd.BackColor = System.Drawing.Color.White;
            this.txtAcctCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAcctCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAcctCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAcctCd.Location = new System.Drawing.Point(554, 17);
            this.txtAcctCd.Name = "txtAcctCd";
            this.txtAcctCd.Size = new System.Drawing.Size(124, 21);
            this.txtAcctCd.TabIndex = 5;
            this.txtAcctCd.Tag = null;
            this.txtAcctCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtAcctCd.TabIndexChanged += new System.EventHandler(this.txtAcctCd_TextChanged);
            // 
            // btnAcct
            // 
            this.btnAcct.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAcct.BackgroundImage")));
            this.btnAcct.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAcct.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAcct.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAcct.Location = new System.Drawing.Point(678, 17);
            this.btnAcct.Name = "btnAcct";
            this.btnAcct.Size = new System.Drawing.Size(24, 21);
            this.btnAcct.TabIndex = 6;
            this.btnAcct.Tag = "";
            this.btnAcct.UseVisualStyleBackColor = true;
            this.btnAcct.Click += new System.EventHandler(this.btnAcct_Click);
            // 
            // txtAcctNm
            // 
            this.txtAcctNm.AutoSize = false;
            this.txtAcctNm.BackColor = System.Drawing.Color.White;
            this.txtAcctNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAcctNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAcctNm.Location = new System.Drawing.Point(702, 17);
            this.txtAcctNm.Name = "txtAcctNm";
            this.txtAcctNm.Size = new System.Drawing.Size(273, 21);
            this.txtAcctNm.TabIndex = 7;
            this.txtAcctNm.Tag = ";2;;";
            this.txtAcctNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(702, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(7, 588);
            this.splitter1.TabIndex = 35;
            this.splitter1.TabStop = false;
            // 
            // ACH007
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1488, 700);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACH007";
            this.Text = "고정자산변동내역조회";
            this.Load += new System.EventHandler(this.ACH007_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcctNm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1TextBox txtAcctCd;
        private C1.Win.C1Input.C1Button btnAcct;
        private C1.Win.C1Input.C1TextBox txtAcctNm;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtFr;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtTo;
        private C1.Win.C1Input.C1Label c1Label12;
    }
}