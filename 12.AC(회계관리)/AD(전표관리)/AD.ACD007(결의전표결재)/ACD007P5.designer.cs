namespace AD.ACD007
{
    partial class ACD007P5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACD007P5));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAssignNm = new C1.Win.C1Input.C1TextBox();
            this.txtAssignId = new C1.Win.C1Input.C1TextBox();
            this.lblAssignId = new C1.Win.C1Input.C1Label();
            this.lblCntNm = new System.Windows.Forms.Label();
            this.txtCheckedCnt = new C1.Win.C1Input.C1TextBox();
            this.lblCheckCnt = new C1.Win.C1Input.C1Label();
            this.txtComment = new C1.Win.C1Input.C1TextBox();
            this.lblComment = new C1.Win.C1Input.C1Label();
            this.txtFinanceDeptYn = new C1.Win.C1Input.C1TextBox();
            this.txtAdminRollYn = new C1.Win.C1Input.C1TextBox();
            this.cboTaskType = new C1.Win.C1List.C1Combo();
            this.lblTaskType = new C1.Win.C1Input.C1Label();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnApprv = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckedCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCheckCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblComment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFinanceDeptYn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdminRollYn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTaskType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTaskType)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1087, 341);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 139);
            this.GridCommPanel.Size = new System.Drawing.Size(1087, 341);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1081, 321);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Controls.Add(this.btnReject);
            this.panButton1.Controls.Add(this.btnApprv);
            this.panButton1.Size = new System.Drawing.Size(1087, 64);
            this.panButton1.Controls.SetChildIndex(this.BtnExcel, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnNew, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnInsert, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnSearch, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnCancel, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnPrint, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnClose, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnRCopy, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnRowIns, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnDelete, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnHelp, 0);
            this.panButton1.Controls.SetChildIndex(this.BtnDel, 0);
            this.panButton1.Controls.SetChildIndex(this.btnApprv, 0);
            this.panButton1.Controls.SetChildIndex(this.btnReject, 0);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 251);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1019, 10);
            this.splitter1.TabIndex = 35;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1087, 75);
            this.panel1.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtAssignNm);
            this.groupBox1.Controls.Add(this.txtAssignId);
            this.groupBox1.Controls.Add(this.lblAssignId);
            this.groupBox1.Controls.Add(this.lblCntNm);
            this.groupBox1.Controls.Add(this.txtCheckedCnt);
            this.groupBox1.Controls.Add(this.lblCheckCnt);
            this.groupBox1.Controls.Add(this.txtComment);
            this.groupBox1.Controls.Add(this.lblComment);
            this.groupBox1.Controls.Add(this.txtFinanceDeptYn);
            this.groupBox1.Controls.Add(this.txtAdminRollYn);
            this.groupBox1.Controls.Add(this.cboTaskType);
            this.groupBox1.Controls.Add(this.lblTaskType);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1087, 75);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // txtAssignNm
            // 
            this.txtAssignNm.AutoSize = false;
            this.txtAssignNm.BackColor = System.Drawing.Color.White;
            this.txtAssignNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAssignNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssignNm.Location = new System.Drawing.Point(215, 42);
            this.txtAssignNm.Name = "txtAssignNm";
            this.txtAssignNm.Size = new System.Drawing.Size(151, 21);
            this.txtAssignNm.TabIndex = 98;
            this.txtAssignNm.Tag = ";2;;";
            // 
            // txtAssignId
            // 
            this.txtAssignId.AutoSize = false;
            this.txtAssignId.BackColor = System.Drawing.Color.White;
            this.txtAssignId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAssignId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssignId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAssignId.Location = new System.Drawing.Point(111, 42);
            this.txtAssignId.Name = "txtAssignId";
            this.txtAssignId.Size = new System.Drawing.Size(105, 21);
            this.txtAssignId.TabIndex = 96;
            this.txtAssignId.Tag = ";2;;";
            // 
            // lblAssignId
            // 
            this.lblAssignId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblAssignId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblAssignId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAssignId.Location = new System.Drawing.Point(18, 42);
            this.lblAssignId.Name = "lblAssignId";
            this.lblAssignId.Size = new System.Drawing.Size(94, 21);
            this.lblAssignId.TabIndex = 99;
            this.lblAssignId.Tag = null;
            this.lblAssignId.Text = "결재자";
            this.lblAssignId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAssignId.TextDetached = true;
            this.lblAssignId.Value = "";
            // 
            // lblCntNm
            // 
            this.lblCntNm.AutoSize = true;
            this.lblCntNm.Location = new System.Drawing.Point(571, 21);
            this.lblCntNm.Name = "lblCntNm";
            this.lblCntNm.Size = new System.Drawing.Size(17, 12);
            this.lblCntNm.TabIndex = 95;
            this.lblCntNm.Text = "건";
            // 
            // txtCheckedCnt
            // 
            this.txtCheckedCnt.AutoSize = false;
            this.txtCheckedCnt.BackColor = System.Drawing.Color.White;
            this.txtCheckedCnt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCheckedCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCheckedCnt.Location = new System.Drawing.Point(492, 16);
            this.txtCheckedCnt.Name = "txtCheckedCnt";
            this.txtCheckedCnt.Size = new System.Drawing.Size(75, 21);
            this.txtCheckedCnt.TabIndex = 94;
            this.txtCheckedCnt.Tag = ";2;;";
            this.txtCheckedCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCheckedCnt.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblCheckCnt
            // 
            this.lblCheckCnt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblCheckCnt.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblCheckCnt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCheckCnt.Location = new System.Drawing.Point(399, 16);
            this.lblCheckCnt.Name = "lblCheckCnt";
            this.lblCheckCnt.Size = new System.Drawing.Size(94, 21);
            this.lblCheckCnt.TabIndex = 93;
            this.lblCheckCnt.Tag = null;
            this.lblCheckCnt.Text = "선택건수";
            this.lblCheckCnt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCheckCnt.TextDetached = true;
            this.lblCheckCnt.Value = "";
            // 
            // txtComment
            // 
            this.txtComment.AutoSize = false;
            this.txtComment.BackColor = System.Drawing.Color.White;
            this.txtComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtComment.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtComment.Location = new System.Drawing.Point(492, 42);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(554, 21);
            this.txtComment.TabIndex = 78;
            this.txtComment.Tag = null;
            this.txtComment.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblComment
            // 
            this.lblComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblComment.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblComment.ForeColor = System.Drawing.Color.Blue;
            this.lblComment.Location = new System.Drawing.Point(399, 42);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(94, 21);
            this.lblComment.TabIndex = 77;
            this.lblComment.Tag = null;
            this.lblComment.Text = "결재코멘트";
            this.lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblComment.TextDetached = true;
            this.lblComment.Value = "";
            // 
            // txtFinanceDeptYn
            // 
            this.txtFinanceDeptYn.AutoSize = false;
            this.txtFinanceDeptYn.BackColor = System.Drawing.Color.White;
            this.txtFinanceDeptYn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtFinanceDeptYn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFinanceDeptYn.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFinanceDeptYn.Location = new System.Drawing.Point(338, 12);
            this.txtFinanceDeptYn.Name = "txtFinanceDeptYn";
            this.txtFinanceDeptYn.Size = new System.Drawing.Size(28, 21);
            this.txtFinanceDeptYn.TabIndex = 76;
            this.txtFinanceDeptYn.Tag = null;
            this.txtFinanceDeptYn.Visible = false;
            // 
            // txtAdminRollYn
            // 
            this.txtAdminRollYn.AutoSize = false;
            this.txtAdminRollYn.BackColor = System.Drawing.Color.White;
            this.txtAdminRollYn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAdminRollYn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAdminRollYn.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAdminRollYn.Location = new System.Drawing.Point(304, 12);
            this.txtAdminRollYn.Name = "txtAdminRollYn";
            this.txtAdminRollYn.Size = new System.Drawing.Size(28, 21);
            this.txtAdminRollYn.TabIndex = 75;
            this.txtAdminRollYn.Tag = null;
            this.txtAdminRollYn.Visible = false;
            // 
            // cboTaskType
            // 
            this.cboTaskType.AddItemSeparator = ';';
            this.cboTaskType.AutoSize = false;
            this.cboTaskType.Caption = "";
            this.cboTaskType.CaptionHeight = 17;
            this.cboTaskType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboTaskType.ColumnCaptionHeight = 18;
            this.cboTaskType.ColumnFooterHeight = 18;
            this.cboTaskType.ContentHeight = 15;
            this.cboTaskType.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboTaskType.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboTaskType.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboTaskType.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboTaskType.EditorHeight = 15;
            this.cboTaskType.Images.Add(((System.Drawing.Image)(resources.GetObject("cboTaskType.Images"))));
            this.cboTaskType.ItemHeight = 15;
            this.cboTaskType.Location = new System.Drawing.Point(111, 16);
            this.cboTaskType.MatchEntryTimeout = ((long)(2000));
            this.cboTaskType.MaxDropDownItems = ((short)(5));
            this.cboTaskType.MaxLength = 32767;
            this.cboTaskType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboTaskType.Name = "cboTaskType";
            this.cboTaskType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboTaskType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboTaskType.Size = new System.Drawing.Size(105, 21);
            this.cboTaskType.TabIndex = 15;
            this.cboTaskType.TabStop = false;
            this.cboTaskType.Tag = ";2;;";
            this.cboTaskType.PropBag = resources.GetString("cboTaskType.PropBag");
            // 
            // lblTaskType
            // 
            this.lblTaskType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblTaskType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblTaskType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTaskType.Location = new System.Drawing.Point(18, 16);
            this.lblTaskType.Name = "lblTaskType";
            this.lblTaskType.Size = new System.Drawing.Size(94, 21);
            this.lblTaskType.TabIndex = 14;
            this.lblTaskType.Tag = null;
            this.lblTaskType.Text = "업무구분";
            this.lblTaskType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTaskType.TextDetached = true;
            this.lblTaskType.Value = "";
            // 
            // btnReject
            // 
            this.btnReject.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnReject.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReject.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReject.Location = new System.Drawing.Point(827, 8);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(48, 48);
            this.btnReject.TabIndex = 38;
            this.btnReject.Text = "반려";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnApprv
            // 
            this.btnApprv.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnApprv.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApprv.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnApprv.Location = new System.Drawing.Point(777, 8);
            this.btnApprv.Name = "btnApprv";
            this.btnApprv.Size = new System.Drawing.Size(48, 48);
            this.btnApprv.TabIndex = 37;
            this.btnApprv.Text = "승인";
            this.btnApprv.UseVisualStyleBackColor = true;
            this.btnApprv.Click += new System.EventHandler(this.btnApprv_Click);
            // 
            // ACD007P5
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1087, 480);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACD007P5";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "결의전표결재>일괄결재";
            this.Load += new System.EventHandler(this.ACD007P5_Load);
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
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckedCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCheckCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblComment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFinanceDeptYn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdminRollYn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTaskType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTaskType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1List.C1Combo cboTaskType;
        private C1.Win.C1Input.C1Label lblTaskType;
        private C1.Win.C1Input.C1TextBox txtFinanceDeptYn;
        private C1.Win.C1Input.C1TextBox txtAdminRollYn;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnApprv;
        private System.Windows.Forms.Label lblCntNm;
        private C1.Win.C1Input.C1TextBox txtCheckedCnt;
        private C1.Win.C1Input.C1Label lblCheckCnt;
        private C1.Win.C1Input.C1TextBox txtComment;
        private C1.Win.C1Input.C1Label lblComment;
        private C1.Win.C1Input.C1TextBox txtAssignNm;
        private C1.Win.C1Input.C1TextBox txtAssignId;
        private C1.Win.C1Input.C1Label lblAssignId;
    }
}