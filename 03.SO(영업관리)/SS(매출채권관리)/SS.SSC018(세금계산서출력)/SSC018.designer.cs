namespace SS.SSC018
{
    partial class SSC018
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SSC018));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mskDT_Fr = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.mskDT_To = new C1.Win.C1Input.C1DateEdit();
            this.btnConfirmOk = new C1.Win.C1Input.C1Button();
            this.btnTaxNo = new C1.Win.C1Input.C1Button();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtTaxNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_Fr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_To)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(811, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 562);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.mskDT_Fr);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mskDT_To);
            this.groupBox1.Controls.Add(this.btnConfirmOk);
            this.groupBox1.Controls.Add(this.btnTaxNo);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtTaxNo);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(473, 188);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // mskDT_Fr
            // 
            this.mskDT_Fr.AutoSize = false;
            this.mskDT_Fr.BackColor = System.Drawing.Color.White;
            this.mskDT_Fr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.mskDT_Fr.Calendar.DayNameLength = 1;
            this.mskDT_Fr.EmptyAsNull = true;
            this.mskDT_Fr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.mskDT_Fr.Location = new System.Drawing.Point(112, 17);
            this.mskDT_Fr.Name = "mskDT_Fr";
            this.mskDT_Fr.Size = new System.Drawing.Size(107, 21);
            this.mskDT_Fr.TabIndex = 1;
            this.mskDT_Fr.Tag = ";1;;";
            this.mskDT_Fr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.mskDT_Fr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(217, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 22);
            this.label1.TabIndex = 2;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mskDT_To
            // 
            this.mskDT_To.AutoSize = false;
            this.mskDT_To.BackColor = System.Drawing.Color.White;
            this.mskDT_To.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.mskDT_To.Calendar.DayNameLength = 1;
            this.mskDT_To.EmptyAsNull = true;
            this.mskDT_To.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.mskDT_To.Location = new System.Drawing.Point(242, 17);
            this.mskDT_To.Name = "mskDT_To";
            this.mskDT_To.Size = new System.Drawing.Size(107, 21);
            this.mskDT_To.TabIndex = 3;
            this.mskDT_To.Tag = ";1;;";
            this.mskDT_To.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.mskDT_To.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // btnConfirmOk
            // 
            this.btnConfirmOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirmOk.BackgroundImage")));
            this.btnConfirmOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmOk.Location = new System.Drawing.Point(12, 145);
            this.btnConfirmOk.Name = "btnConfirmOk";
            this.btnConfirmOk.Size = new System.Drawing.Size(92, 25);
            this.btnConfirmOk.TabIndex = 7;
            this.btnConfirmOk.Text = "미리보기";
            this.btnConfirmOk.UseVisualStyleBackColor = true;
            this.btnConfirmOk.Click += new System.EventHandler(this.btnConfirmOk_Click);
            // 
            // btnTaxNo
            // 
            this.btnTaxNo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTaxNo.BackgroundImage")));
            this.btnTaxNo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnTaxNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTaxNo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTaxNo.Location = new System.Drawing.Point(237, 44);
            this.btnTaxNo.Name = "btnTaxNo";
            this.btnTaxNo.Size = new System.Drawing.Size(24, 21);
            this.btnTaxNo.TabIndex = 6;
            this.btnTaxNo.UseVisualStyleBackColor = true;
            this.btnTaxNo.Click += new System.EventHandler(this.btnTaxNo_Click);
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(12, 44);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(100, 21);
            this.c1Label2.TabIndex = 4;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "세금계산서번호";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtTaxNo
            // 
            this.txtTaxNo.AutoSize = false;
            this.txtTaxNo.BackColor = System.Drawing.Color.White;
            this.txtTaxNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtTaxNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTaxNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTaxNo.Location = new System.Drawing.Point(111, 44);
            this.txtTaxNo.Name = "txtTaxNo";
            this.txtTaxNo.Size = new System.Drawing.Size(127, 21);
            this.txtTaxNo.TabIndex = 5;
            this.txtTaxNo.Tag = "사업장;1;;";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(12, 17);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(100, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "발행일";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
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
            // SSC018
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(811, 626);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "SSC018";
            this.Text = "세금계산서출력";
            this.Activated += new System.EventHandler(this.SSC018_Activated);
            this.Deactivate += new System.EventHandler(this.SSC018_Deactivate);
            this.Load += new System.EventHandler(this.SSC018_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_Fr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_To)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTaxNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1Input.C1Button btnTaxNo;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtTaxNo;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1DateEdit mskDT_Fr;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit mskDT_To;
        private C1.Win.C1Input.C1Button btnConfirmOk;
    }
}