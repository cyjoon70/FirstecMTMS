namespace BD.BBD003
{
    partial class BBD003
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BBD003));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCRT_RELOAD = new C1.Win.C1Input.C1Button();
            this.cboReorgNm = new C1.Win.C1List.C1Combo();
            this.cboReorgId = new C1.Win.C1List.C1Combo();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboReorgNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReorgId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(784, 398);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 129);
            this.GridCommPanel.Size = new System.Drawing.Size(784, 398);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(778, 378);
            this.fpSpread1.TabIndex = 0;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(784, 64);
            this.panButton1.TabIndex = 0;
            // 
            // BtnRowIns
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 65);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnCRT_RELOAD);
            this.groupBox1.Controls.Add(this.cboReorgNm);
            this.groupBox1.Controls.Add(this.cboReorgId);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(784, 64);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnCRT_RELOAD
            // 
            this.btnCRT_RELOAD.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCRT_RELOAD.BackgroundImage")));
            this.btnCRT_RELOAD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCRT_RELOAD.Location = new System.Drawing.Point(580, 20);
            this.btnCRT_RELOAD.Name = "btnCRT_RELOAD";
            this.btnCRT_RELOAD.Size = new System.Drawing.Size(101, 25);
            this.btnCRT_RELOAD.TabIndex = 259;
            this.btnCRT_RELOAD.Text = "불러오기";
            this.btnCRT_RELOAD.UseVisualStyleBackColor = true;
            this.btnCRT_RELOAD.Click += new System.EventHandler(this.btnCRT_RELOAD_Click);
            // 
            // cboReorgNm
            // 
            this.cboReorgNm.AddItemSeparator = ';';
            this.cboReorgNm.AutoSize = false;
            this.cboReorgNm.Caption = "";
            this.cboReorgNm.CaptionHeight = 17;
            this.cboReorgNm.CaptionVisible = false;
            this.cboReorgNm.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboReorgNm.ColumnCaptionHeight = 18;
            this.cboReorgNm.ColumnFooterHeight = 18;
            this.cboReorgNm.ContentHeight = 15;
            this.cboReorgNm.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboReorgNm.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboReorgNm.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboReorgNm.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboReorgNm.EditorHeight = 15;
            this.cboReorgNm.Images.Add(((System.Drawing.Image)(resources.GetObject("cboReorgNm.Images"))));
            this.cboReorgNm.ItemHeight = 15;
            this.cboReorgNm.Location = new System.Drawing.Point(362, 20);
            this.cboReorgNm.MatchEntryTimeout = ((long)(2000));
            this.cboReorgNm.MaxDropDownItems = ((short)(5));
            this.cboReorgNm.MaxLength = 32767;
            this.cboReorgNm.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboReorgNm.Name = "cboReorgNm";
            this.cboReorgNm.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboReorgNm.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboReorgNm.Size = new System.Drawing.Size(109, 21);
            this.cboReorgNm.TabIndex = 258;
            this.cboReorgNm.Tag = ";1;;";
            this.cboReorgNm.PropBag = resources.GetString("cboReorgNm.PropBag");
            // 
            // cboReorgId
            // 
            this.cboReorgId.AddItemSeparator = ';';
            this.cboReorgId.AutoSize = false;
            this.cboReorgId.Caption = "";
            this.cboReorgId.CaptionHeight = 17;
            this.cboReorgId.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboReorgId.ColumnCaptionHeight = 18;
            this.cboReorgId.ColumnFooterHeight = 18;
            this.cboReorgId.ContentHeight = 15;
            this.cboReorgId.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboReorgId.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboReorgId.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboReorgId.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboReorgId.EditorHeight = 15;
            this.cboReorgId.Images.Add(((System.Drawing.Image)(resources.GetObject("cboReorgId.Images"))));
            this.cboReorgId.ItemHeight = 15;
            this.cboReorgId.Location = new System.Drawing.Point(106, 21);
            this.cboReorgId.MatchEntryTimeout = ((long)(2000));
            this.cboReorgId.MaxDropDownItems = ((short)(5));
            this.cboReorgId.MaxLength = 32767;
            this.cboReorgId.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboReorgId.Name = "cboReorgId";
            this.cboReorgId.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboReorgId.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboReorgId.Size = new System.Drawing.Size(109, 21);
            this.cboReorgId.TabIndex = 257;
            this.cboReorgId.Tag = ";1;;";
            this.cboReorgId.PropBag = resources.GetString("cboReorgId.PropBag");
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(282, 20);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(80, 21);
            this.c1Label2.TabIndex = 4;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "변경후개편ID";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(26, 21);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(80, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "변경전개편ID";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // BBD003
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BBD003";
            this.Text = "부서개편 HISTORY 등록";
            this.Load += new System.EventHandler(this.BBD001_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboReorgNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboReorgId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1List.C1Combo cboReorgId;
        private C1.Win.C1List.C1Combo cboReorgNm;
        private C1.Win.C1Input.C1Button btnCRT_RELOAD;

    }
}