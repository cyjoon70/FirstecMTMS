namespace AA.ACA006
{
    partial class ACA006
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACA006));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.txtTypeCd = new C1.Win.C1Input.C1TextBox();
            this.BtnType = new C1.Win.C1Input.C1Button();
            this.txtTypeNm = new C1.Win.C1Input.C1TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnTypeCopy = new C1.Win.C1Input.C1Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCopyTypeNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.txtCopyTypeCd = new C1.Win.C1Input.C1TextBox();
            this.BtnCopyType = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTypeCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTypeNm)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCopyTypeNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCopyTypeCd)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommPanel2
            // 
            this.GridCommPanel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.GridCommPanel2.Location = new System.Drawing.Point(0, 0);
            this.GridCommPanel2.Size = new System.Drawing.Size(1134, 541);
            // 
            // GridCommGroupBox2
            // 
            this.GridCommGroupBox2.Size = new System.Drawing.Size(1118, 522);
            this.GridCommGroupBox2.Text = "재무재표유형";
            // 
            // fpSpread2
            // 
            this.fpSpread2.Size = new System.Drawing.Size(1102, 495);
            this.fpSpread2.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread2_SelectionChanged);
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // 
            // GridCommPanel1
            // 
            this.GridCommPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel1.Location = new System.Drawing.Point(1141, 0);
            this.GridCommPanel1.Size = new System.Drawing.Size(347, 541);
            // 
            // GridCommGroupBox1
            // 
            this.GridCommGroupBox1.Size = new System.Drawing.Size(331, 522);
            this.GridCommGroupBox1.Text = "계정코드정보";
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(325, 502);
            // 
            // sheetView1
            // 
            this.sheetView1.SheetName = "Sheet1";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 112);
            this.panel4.Size = new System.Drawing.Size(1488, 588);
            this.panel4.Controls.SetChildIndex(this.panel2, 0);
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
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.txtTypeCd);
            this.groupBox1.Controls.Add(this.BtnType);
            this.groupBox1.Controls.Add(this.txtTypeNm);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1488, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(16, 17);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(94, 21);
            this.c1Label4.TabIndex = 4;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "계정분류형태";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.AutoSize = false;
            this.txtTypeCd.BackColor = System.Drawing.Color.White;
            this.txtTypeCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtTypeCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTypeCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTypeCd.Location = new System.Drawing.Point(109, 17);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.Size = new System.Drawing.Size(124, 21);
            this.txtTypeCd.TabIndex = 5;
            this.txtTypeCd.Tag = "계정분류형태;1;;;";
            this.txtTypeCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtTypeCd.TextChanged += new System.EventHandler(this.txtTypeCd_TextChanged);
            // 
            // BtnType
            // 
            this.BtnType.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnType.BackgroundImage")));
            this.BtnType.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnType.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnType.Location = new System.Drawing.Point(233, 17);
            this.BtnType.Name = "BtnType";
            this.BtnType.Size = new System.Drawing.Size(24, 21);
            this.BtnType.TabIndex = 6;
            this.BtnType.Tag = "";
            this.BtnType.UseVisualStyleBackColor = true;
            this.BtnType.Click += new System.EventHandler(this.BtnType_Click);
            // 
            // txtTypeNm
            // 
            this.txtTypeNm.AutoSize = false;
            this.txtTypeNm.BackColor = System.Drawing.Color.White;
            this.txtTypeNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtTypeNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTypeNm.Location = new System.Drawing.Point(257, 17);
            this.txtTypeNm.Name = "txtTypeNm";
            this.txtTypeNm.Size = new System.Drawing.Size(273, 21);
            this.txtTypeNm.TabIndex = 7;
            this.txtTypeNm.Tag = ";2;;";
            this.txtTypeNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(1134, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(7, 541);
            this.splitter1.TabIndex = 35;
            this.splitter1.TabStop = false;
            // 
            // btnTypeCopy
            // 
            this.btnTypeCopy.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTypeCopy.BackgroundImage")));
            this.btnTypeCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTypeCopy.Location = new System.Drawing.Point(21, 12);
            this.btnTypeCopy.Name = "btnTypeCopy";
            this.btnTypeCopy.Size = new System.Drawing.Size(86, 25);
            this.btnTypeCopy.TabIndex = 21;
            this.btnTypeCopy.Text = "복사";
            this.btnTypeCopy.UseVisualStyleBackColor = true;
            this.btnTypeCopy.Click += new System.EventHandler(this.btnTypeCopy_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 541);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1488, 47);
            this.panel2.TabIndex = 36;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.txtCopyTypeNm);
            this.groupBox2.Controls.Add(this.c1Label1);
            this.groupBox2.Controls.Add(this.btnTypeCopy);
            this.groupBox2.Controls.Add(this.txtCopyTypeCd);
            this.groupBox2.Controls.Add(this.BtnCopyType);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1488, 47);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            // 
            // txtCopyTypeNm
            // 
            this.txtCopyTypeNm.AutoSize = false;
            this.txtCopyTypeNm.BackColor = System.Drawing.Color.White;
            this.txtCopyTypeNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCopyTypeNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCopyTypeNm.Location = new System.Drawing.Point(359, 16);
            this.txtCopyTypeNm.Name = "txtCopyTypeNm";
            this.txtCopyTypeNm.Size = new System.Drawing.Size(273, 21);
            this.txtCopyTypeNm.TabIndex = 25;
            this.txtCopyTypeNm.Tag = ";2;;";
            this.txtCopyTypeNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(118, 16);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(94, 21);
            this.c1Label1.TabIndex = 22;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "계정분류형태";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // txtCopyTypeCd
            // 
            this.txtCopyTypeCd.AutoSize = false;
            this.txtCopyTypeCd.BackColor = System.Drawing.Color.White;
            this.txtCopyTypeCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCopyTypeCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCopyTypeCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCopyTypeCd.Location = new System.Drawing.Point(211, 16);
            this.txtCopyTypeCd.Name = "txtCopyTypeCd";
            this.txtCopyTypeCd.Size = new System.Drawing.Size(124, 21);
            this.txtCopyTypeCd.TabIndex = 23;
            this.txtCopyTypeCd.Tag = null;
            this.txtCopyTypeCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtCopyTypeCd.TextChanged += new System.EventHandler(this.txtCopyTypeCd_TextChanged);
            // 
            // BtnCopyType
            // 
            this.BtnCopyType.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnCopyType.BackgroundImage")));
            this.BtnCopyType.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnCopyType.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCopyType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnCopyType.Location = new System.Drawing.Point(335, 16);
            this.BtnCopyType.Name = "BtnCopyType";
            this.BtnCopyType.Size = new System.Drawing.Size(24, 21);
            this.BtnCopyType.TabIndex = 24;
            this.BtnCopyType.Tag = "";
            this.BtnCopyType.UseVisualStyleBackColor = true;
            this.BtnCopyType.Click += new System.EventHandler(this.BtnCopyType_Click);
            // 
            // ACA006
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1488, 700);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACA006";
            this.Text = "유형별계정분류";
            this.Load += new System.EventHandler(this.ACB006_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTypeCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTypeNm)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCopyTypeNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCopyTypeCd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnTypeCopy;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1TextBox txtTypeCd;
        private C1.Win.C1Input.C1Button BtnType;
        private C1.Win.C1Input.C1TextBox txtTypeNm;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtCopyTypeCd;
        private C1.Win.C1Input.C1Button BtnCopyType;
        private C1.Win.C1Input.C1TextBox txtCopyTypeNm;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}