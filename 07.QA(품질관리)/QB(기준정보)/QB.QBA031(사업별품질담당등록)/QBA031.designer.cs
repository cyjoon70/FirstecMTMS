namespace QB.QBA031
{
    partial class QBA031
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QBA031));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboMpart = new C1.Win.C1List.C1Combo();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.c1Label9 = new C1.Win.C1Input.C1Label();
            this.txtEntNm = new C1.Win.C1Input.C1TextBox();
            this.txtEntCd = new C1.Win.C1Input.C1TextBox();
            this.cboLpart = new C1.Win.C1List.C1Combo();
            this.c1Label10 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboMpart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLpart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(840, 418);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 109);
            this.GridCommPanel.Size = new System.Drawing.Size(840, 418);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(834, 398);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(840, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(840, 45);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboMpart);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Controls.Add(this.c1Label9);
            this.groupBox1.Controls.Add(this.txtEntNm);
            this.groupBox1.Controls.Add(this.txtEntCd);
            this.groupBox1.Controls.Add(this.cboLpart);
            this.groupBox1.Controls.Add(this.c1Label10);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(840, 45);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cboMpart
            // 
            this.cboMpart.AddItemSeparator = ';';
            this.cboMpart.AutoSize = false;
            this.cboMpart.Caption = "";
            this.cboMpart.CaptionHeight = 17;
            this.cboMpart.CaptionVisible = false;
            this.cboMpart.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboMpart.ColumnCaptionHeight = 18;
            this.cboMpart.ColumnFooterHeight = 18;
            this.cboMpart.ContentHeight = 15;
            this.cboMpart.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboMpart.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboMpart.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboMpart.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboMpart.EditorHeight = 15;
            this.cboMpart.Images.Add(((System.Drawing.Image)(resources.GetObject("cboMpart.Images"))));
            this.cboMpart.ItemHeight = 15;
            this.cboMpart.Location = new System.Drawing.Point(701, 16);
            this.cboMpart.MatchEntryTimeout = ((long)(2000));
            this.cboMpart.MaxDropDownItems = ((short)(5));
            this.cboMpart.MaxLength = 32767;
            this.cboMpart.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboMpart.Name = "cboMpart";
            this.cboMpart.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboMpart.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboMpart.Size = new System.Drawing.Size(129, 21);
            this.cboMpart.TabIndex = 6;
            this.cboMpart.Tag = "공장;1;;";
            this.cboMpart.PropBag = resources.GetString("cboMpart.PropBag");
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(619, 16);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(83, 21);
            this.c1Label6.TabIndex = 5;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "중분류";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // c1Label9
            // 
            this.c1Label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label9.Location = new System.Drawing.Point(398, 16);
            this.c1Label9.Name = "c1Label9";
            this.c1Label9.Size = new System.Drawing.Size(83, 21);
            this.c1Label9.TabIndex = 3;
            this.c1Label9.Tag = null;
            this.c1Label9.Text = "대분류";
            this.c1Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label9.TextDetached = true;
            this.c1Label9.Value = "";
            // 
            // txtEntNm
            // 
            this.txtEntNm.AutoSize = false;
            this.txtEntNm.BackColor = System.Drawing.Color.White;
            this.txtEntNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtEntNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEntNm.Location = new System.Drawing.Point(194, 16);
            this.txtEntNm.Name = "txtEntNm";
            this.txtEntNm.Size = new System.Drawing.Size(195, 21);
            this.txtEntNm.TabIndex = 2;
            // 
            // txtEntCd
            // 
            this.txtEntCd.AutoSize = false;
            this.txtEntCd.BackColor = System.Drawing.Color.White;
            this.txtEntCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtEntCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEntCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtEntCd.Location = new System.Drawing.Point(90, 16);
            this.txtEntCd.Name = "txtEntCd";
            this.txtEntCd.Size = new System.Drawing.Size(105, 21);
            this.txtEntCd.TabIndex = 1;
            this.txtEntCd.Tag = null;
            // 
            // cboLpart
            // 
            this.cboLpart.AddItemSeparator = ';';
            this.cboLpart.AutoSize = false;
            this.cboLpart.Caption = "";
            this.cboLpart.CaptionHeight = 17;
            this.cboLpart.CaptionVisible = false;
            this.cboLpart.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboLpart.ColumnCaptionHeight = 18;
            this.cboLpart.ColumnFooterHeight = 18;
            this.cboLpart.ContentHeight = 15;
            this.cboLpart.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboLpart.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboLpart.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboLpart.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboLpart.EditorHeight = 15;
            this.cboLpart.Images.Add(((System.Drawing.Image)(resources.GetObject("cboLpart.Images"))));
            this.cboLpart.ItemHeight = 15;
            this.cboLpart.Location = new System.Drawing.Point(480, 16);
            this.cboLpart.MatchEntryTimeout = ((long)(2000));
            this.cboLpart.MaxDropDownItems = ((short)(5));
            this.cboLpart.MaxLength = 32767;
            this.cboLpart.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboLpart.Name = "cboLpart";
            this.cboLpart.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboLpart.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboLpart.Size = new System.Drawing.Size(129, 21);
            this.cboLpart.TabIndex = 4;
            this.cboLpart.Tag = "공장;1;;";
            this.cboLpart.PropBag = resources.GetString("cboLpart.PropBag");
            // 
            // c1Label10
            // 
            this.c1Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label10.Location = new System.Drawing.Point(8, 16);
            this.c1Label10.Name = "c1Label10";
            this.c1Label10.Size = new System.Drawing.Size(83, 21);
            this.c1Label10.TabIndex = 0;
            this.c1Label10.Tag = null;
            this.c1Label10.Text = "사업코드";
            this.c1Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label10.TextDetached = true;
            this.c1Label10.Value = "";
            // 
            // QBA031
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(840, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QBA031";
            this.Text = "사업별품질담당등록";
            this.Load += new System.EventHandler(this.PCC045_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboMpart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLpart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1List.C1Combo cboLpart;
        private C1.Win.C1Input.C1Label c1Label10;
        private C1.Win.C1Input.C1TextBox txtEntNm;
        private C1.Win.C1Input.C1TextBox txtEntCd;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1Label c1Label9;
        private C1.Win.C1List.C1Combo cboMpart;

    }
}