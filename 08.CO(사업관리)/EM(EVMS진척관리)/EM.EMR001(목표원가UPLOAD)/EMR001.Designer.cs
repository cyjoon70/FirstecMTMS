namespace EM.EMR001
{
    partial class EMR001
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EMR001));
            this.panel1 = new System.Windows.Forms.Panel();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboSheet = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdoEqual = new System.Windows.Forms.RadioButton();
            this.rdoOver = new System.Windows.Forms.RadioButton();
            this.btnFileDownload = new C1.Win.C1Input.C1Button();
            this.txtProjNm = new C1.Win.C1Input.C1TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoSheet = new System.Windows.Forms.RadioButton();
            this.btnFileUpload = new C1.Win.C1Input.C1Button();
            this.c1Label14 = new C1.Win.C1Input.C1Label();
            this.btnFile = new C1.Win.C1Input.C1Button();
            this.txtFilePath = new C1.Win.C1Input.C1TextBox();
            this.btnProj = new C1.Win.C1Input.C1Button();
            this.제조오더번호 = new C1.Win.C1Input.C1Label();
            this.txtProjNo = new C1.Win.C1Input.C1TextBox();
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
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNm)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.제조오더번호)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(689, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fpSpread1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(689, 133);
            this.panel1.TabIndex = 8;
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "";
            this.fpSpread1.Location = new System.Drawing.Point(532, 154);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(48, 45);
            this.fpSpread1.TabIndex = 13;
            this.fpSpread1.Visible = false;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboSheet);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.btnFileDownload);
            this.groupBox1.Controls.Add(this.txtProjNm);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.btnFileUpload);
            this.groupBox1.Controls.Add(this.c1Label14);
            this.groupBox1.Controls.Add(this.btnFile);
            this.groupBox1.Controls.Add(this.txtFilePath);
            this.groupBox1.Controls.Add(this.btnProj);
            this.groupBox1.Controls.Add(this.제조오더번호);
            this.groupBox1.Controls.Add(this.txtProjNo);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(689, 133);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cboSheet
            // 
            this.cboSheet.FormattingEnabled = true;
            this.cboSheet.Location = new System.Drawing.Point(274, 71);
            this.cboSheet.Name = "cboSheet";
            this.cboSheet.Size = new System.Drawing.Size(178, 20);
            this.cboSheet.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rdoEqual);
            this.panel2.Controls.Add(this.rdoOver);
            this.panel2.Location = new System.Drawing.Point(463, 71);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(100, 21);
            this.panel2.TabIndex = 7;
            this.panel2.Visible = false;
            // 
            // rdoEqual
            // 
            this.rdoEqual.Checked = true;
            this.rdoEqual.Location = new System.Drawing.Point(10, 2);
            this.rdoEqual.Name = "rdoEqual";
            this.rdoEqual.Size = new System.Drawing.Size(34, 18);
            this.rdoEqual.TabIndex = 0;
            this.rdoEqual.TabStop = true;
            this.rdoEqual.Text = "=";
            this.rdoEqual.UseVisualStyleBackColor = true;
            // 
            // rdoOver
            // 
            this.rdoOver.Location = new System.Drawing.Point(53, 1);
            this.rdoOver.Name = "rdoOver";
            this.rdoOver.Size = new System.Drawing.Size(44, 18);
            this.rdoOver.TabIndex = 1;
            this.rdoOver.Text = ">=";
            this.rdoOver.UseVisualStyleBackColor = true;
            // 
            // btnFileDownload
            // 
            this.btnFileDownload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileDownload.BackgroundImage")));
            this.btnFileDownload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileDownload.Location = new System.Drawing.Point(141, 100);
            this.btnFileDownload.Name = "btnFileDownload";
            this.btnFileDownload.Size = new System.Drawing.Size(124, 25);
            this.btnFileDownload.TabIndex = 9;
            this.btnFileDownload.Text = "양식 DOWNLOAD";
            this.btnFileDownload.UseVisualStyleBackColor = true;
            this.btnFileDownload.Click += new System.EventHandler(this.btnFileDownload_Click);
            // 
            // txtProjNm
            // 
            this.txtProjNm.AutoSize = false;
            this.txtProjNm.BackColor = System.Drawing.Color.White;
            this.txtProjNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjNm.Location = new System.Drawing.Point(268, 17);
            this.txtProjNm.Name = "txtProjNm";
            this.txtProjNm.Size = new System.Drawing.Size(246, 21);
            this.txtProjNm.TabIndex = 0;
            this.txtProjNm.Tag = ";2;;";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rdoAll);
            this.panel3.Controls.Add(this.rdoSheet);
            this.panel3.Location = new System.Drawing.Point(116, 71);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(146, 21);
            this.panel3.TabIndex = 5;
            // 
            // rdoAll
            // 
            this.rdoAll.Checked = true;
            this.rdoAll.Location = new System.Drawing.Point(10, 2);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(50, 18);
            this.rdoAll.TabIndex = 0;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "전체";
            this.rdoAll.UseVisualStyleBackColor = true;
            this.rdoAll.CheckedChanged += new System.EventHandler(this.rdoAll_CheckedChanged);
            // 
            // rdoSheet
            // 
            this.rdoSheet.Location = new System.Drawing.Point(66, 1);
            this.rdoSheet.Name = "rdoSheet";
            this.rdoSheet.Size = new System.Drawing.Size(74, 18);
            this.rdoSheet.TabIndex = 1;
            this.rdoSheet.Text = "위크시트";
            this.rdoSheet.UseVisualStyleBackColor = true;
            this.rdoSheet.CheckedChanged += new System.EventHandler(this.rdoSheet_CheckedChanged);
            // 
            // btnFileUpload
            // 
            this.btnFileUpload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileUpload.BackgroundImage")));
            this.btnFileUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileUpload.Location = new System.Drawing.Point(11, 100);
            this.btnFileUpload.Name = "btnFileUpload";
            this.btnFileUpload.Size = new System.Drawing.Size(124, 25);
            this.btnFileUpload.TabIndex = 8;
            this.btnFileUpload.Text = "파일 UPLOAD";
            this.btnFileUpload.UseVisualStyleBackColor = true;
            this.btnFileUpload.Click += new System.EventHandler(this.btnFileUpload_Click);
            // 
            // c1Label14
            // 
            this.c1Label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label14.Location = new System.Drawing.Point(12, 71);
            this.c1Label14.Name = "c1Label14";
            this.c1Label14.Size = new System.Drawing.Size(104, 21);
            this.c1Label14.TabIndex = 4;
            this.c1Label14.Tag = null;
            this.c1Label14.Text = "구분";
            this.c1Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label14.TextDetached = true;
            this.c1Label14.Value = "";
            // 
            // btnFile
            // 
            this.btnFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFile.BackgroundImage")));
            this.btnFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFile.Location = new System.Drawing.Point(490, 44);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(24, 21);
            this.btnFile.TabIndex = 3;
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.AutoSize = false;
            this.txtFilePath.BackColor = System.Drawing.Color.White;
            this.txtFilePath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilePath.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFilePath.Location = new System.Drawing.Point(115, 44);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(375, 21);
            this.txtFilePath.TabIndex = 2;
            this.txtFilePath.Tag = "파일선택;1;;";
            // 
            // btnProj
            // 
            this.btnProj.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnProj.BackgroundImage")));
            this.btnProj.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnProj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnProj.Location = new System.Drawing.Point(244, 17);
            this.btnProj.Name = "btnProj";
            this.btnProj.Size = new System.Drawing.Size(24, 21);
            this.btnProj.TabIndex = 12;
            this.btnProj.UseVisualStyleBackColor = true;
            this.btnProj.Click += new System.EventHandler(this.btnProj_Click);
            // 
            // 제조오더번호
            // 
            this.제조오더번호.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.제조오더번호.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.제조오더번호.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.제조오더번호.Location = new System.Drawing.Point(12, 44);
            this.제조오더번호.Name = "제조오더번호";
            this.제조오더번호.Size = new System.Drawing.Size(104, 21);
            this.제조오더번호.TabIndex = 1;
            this.제조오더번호.Tag = null;
            this.제조오더번호.Text = "파일선택";
            this.제조오더번호.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.제조오더번호.TextDetached = true;
            this.제조오더번호.Value = "";
            // 
            // txtProjNo
            // 
            this.txtProjNo.AutoSize = false;
            this.txtProjNo.BackColor = System.Drawing.Color.White;
            this.txtProjNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProjNo.Location = new System.Drawing.Point(115, 17);
            this.txtProjNo.Name = "txtProjNo";
            this.txtProjNo.Size = new System.Drawing.Size(129, 21);
            this.txtProjNo.TabIndex = 11;
            this.txtProjNo.Tag = "프로젝트번호;1;;";
            this.txtProjNo.TextChanged += new System.EventHandler(this.txtProjNo_TextChanged);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(12, 17);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(104, 21);
            this.c1Label1.TabIndex = 10;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "프로젝트번호";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // EMR001
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 197);
            this.Controls.Add(this.panel1);
            this.Name = "EMR001";
            this.Text = "목표원가UPLOAD";
            this.Activated += new System.EventHandler(this.EMR001_Activated);
            this.Deactivate += new System.EventHandler(this.EMR001_Deactivate);
            this.Load += new System.EventHandler(this.EMR001_Load);
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
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNm)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.제조오더번호)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Button btnFile;
        private C1.Win.C1Input.C1TextBox txtFilePath;
        private C1.Win.C1Input.C1Button btnProj;
        private C1.Win.C1Input.C1Label 제조오더번호;
        private C1.Win.C1Input.C1TextBox txtProjNo;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Label c1Label14;
        private C1.Win.C1Input.C1Button btnFileUpload;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.RadioButton rdoSheet;
        private C1.Win.C1Input.C1TextBox txtProjNm;
        private C1.Win.C1Input.C1Button btnFileDownload;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoEqual;
        private System.Windows.Forms.RadioButton rdoOver;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.ComboBox cboSheet;
    }
}