namespace MO.MIM518
{   
    partial class MIM518P6
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIM518P6));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCustCd = new C1.Win.C1Input.C1Button();
            this.txtCustNm = new C1.Win.C1Input.C1TextBox();
            this.txtCustCd = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.dtpPoDtTo = new C1.Win.C1Input.C1DateEdit();
            this.dtpPoDtFr = new C1.Win.C1Input.C1DateEdit();
            this.txtScmNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.butOk = new C1.Win.C1Input.C1Button();
            this.butCancel = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtCustNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpPoDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpPoDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScmNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Controls.Add(this.butOk);
            this.GridCommGroupBox.Controls.Add(this.butCancel);
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(689, 391);
            this.GridCommGroupBox.TabIndex = 0;
            this.GridCommGroupBox.Controls.SetChildIndex(this.fpSpread1, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.butCancel, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.butOk, 0);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 136);
            this.GridCommPanel.Size = new System.Drawing.Size(689, 391);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(683, 334);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(689, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(689, 72);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnCustCd);
            this.groupBox1.Controls.Add(this.txtCustNm);
            this.groupBox1.Controls.Add(this.txtCustCd);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.dtpPoDtTo);
            this.groupBox1.Controls.Add(this.dtpPoDtFr);
            this.groupBox1.Controls.Add(this.txtScmNo);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(689, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnCustCd
            // 
            this.btnCustCd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCustCd.BackgroundImage")));
            this.btnCustCd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnCustCd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustCd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCustCd.Location = new System.Drawing.Point(490, 15);
            this.btnCustCd.Name = "btnCustCd";
            this.btnCustCd.Size = new System.Drawing.Size(24, 21);
            this.btnCustCd.TabIndex = 4;
            this.btnCustCd.UseVisualStyleBackColor = true;
            this.btnCustCd.Click += new System.EventHandler(this.btnCustCd_Click);
            // 
            // txtCustNm
            // 
            this.txtCustNm.AutoSize = false;
            this.txtCustNm.BackColor = System.Drawing.Color.White;
            this.txtCustNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCustNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustNm.Location = new System.Drawing.Point(514, 15);
            this.txtCustNm.Name = "txtCustNm";
            this.txtCustNm.Size = new System.Drawing.Size(163, 21);
            this.txtCustNm.TabIndex = 5;
            this.txtCustNm.Tag = ";2;;";
            // 
            // txtCustCd
            // 
            this.txtCustCd.AutoSize = false;
            this.txtCustCd.BackColor = System.Drawing.Color.White;
            this.txtCustCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCustCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCustCd.Location = new System.Drawing.Point(394, 15);
            this.txtCustCd.Name = "txtCustCd";
            this.txtCustCd.Size = new System.Drawing.Size(96, 21);
            this.txtCustCd.TabIndex = 3;
            this.txtCustCd.Tag = "공급처;1;;";
            this.txtCustCd.TextChanged += new System.EventHandler(this.txtCustCd_TextChanged);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(8, 42);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(83, 21);
            this.c1Label1.TabIndex = 6;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "출고일자";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label2
            // 
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label2.Location = new System.Drawing.Point(189, 44);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(19, 20);
            this.c1Label2.TabIndex = 8;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "~";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            // 
            // dtpPoDtTo
            // 
            this.dtpPoDtTo.AutoSize = false;
            this.dtpPoDtTo.BackColor = System.Drawing.Color.White;
            this.dtpPoDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpPoDtTo.Calendar.DayNameLength = 1;
            this.dtpPoDtTo.CustomFormat = "yyyy-MM-dd";
            this.dtpPoDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpPoDtTo.Location = new System.Drawing.Point(210, 43);
            this.dtpPoDtTo.Name = "dtpPoDtTo";
            this.dtpPoDtTo.Size = new System.Drawing.Size(96, 21);
            this.dtpPoDtTo.TabIndex = 9;
            this.dtpPoDtTo.Tag = null;
            this.dtpPoDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpPoDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // dtpPoDtFr
            // 
            this.dtpPoDtFr.AutoSize = false;
            this.dtpPoDtFr.BackColor = System.Drawing.Color.White;
            this.dtpPoDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpPoDtFr.Calendar.DayNameLength = 1;
            this.dtpPoDtFr.CustomFormat = "yyyy-MM-dd";
            this.dtpPoDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpPoDtFr.Location = new System.Drawing.Point(91, 43);
            this.dtpPoDtFr.Name = "dtpPoDtFr";
            this.dtpPoDtFr.Size = new System.Drawing.Size(96, 21);
            this.dtpPoDtFr.TabIndex = 7;
            this.dtpPoDtFr.Tag = null;
            this.dtpPoDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpPoDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // txtScmNo
            // 
            this.txtScmNo.AutoSize = false;
            this.txtScmNo.BackColor = System.Drawing.Color.White;
            this.txtScmNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtScmNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScmNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtScmNo.Location = new System.Drawing.Point(90, 15);
            this.txtScmNo.Name = "txtScmNo";
            this.txtScmNo.Size = new System.Drawing.Size(120, 21);
            this.txtScmNo.TabIndex = 1;
            this.txtScmNo.Tag = null;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(312, 15);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(83, 21);
            this.c1Label4.TabIndex = 2;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "공급처";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(8, 15);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(83, 21);
            this.c1Label3.TabIndex = 0;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "SCM출고번호";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // butOk
            // 
            this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butOk.BackgroundImage")));
            this.butOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butOk.Location = new System.Drawing.Point(434, 359);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(120, 25);
            this.butOk.TabIndex = 153;
            this.butOk.Text = "확인";
            this.butOk.UseVisualStyleBackColor = true;
            this.butOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCancel.BackgroundImage")));
            this.butCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancel.Location = new System.Drawing.Point(561, 359);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(120, 25);
            this.butCancel.TabIndex = 152;
            this.butCancel.Text = "취소";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // MIM518P6
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(689, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MIM518P6";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MIM518P6";
            this.Load += new System.EventHandler(this.MIM518P6_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtCustNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpPoDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpPoDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScmNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1TextBox txtScmNo;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1DateEdit dtpPoDtTo;
        private C1.Win.C1Input.C1DateEdit dtpPoDtFr;
        private C1.Win.C1Input.C1Button btnCustCd;
        private C1.Win.C1Input.C1TextBox txtCustNm;
        private C1.Win.C1Input.C1TextBox txtCustCd;
        private C1.Win.C1Input.C1Button butOk;
        private C1.Win.C1Input.C1Button butCancel;

    }
}