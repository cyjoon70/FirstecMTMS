namespace AD.ACD007
{
    partial class ACD007P2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACD007P2));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboDocType = new C1.Win.C1List.C1Combo();
            this.lblDocType = new C1.Win.C1Input.C1Label();
            this.lblDocCode = new C1.Win.C1Input.C1Label();
            this.txtDocCode = new C1.Win.C1Input.C1TextBox();
            this.lblSlipNo = new C1.Win.C1Input.C1Label();
            this.txtSlipNo = new C1.Win.C1Input.C1TextBox();
            this.txtDeptNm = new C1.Win.C1Input.C1TextBox();
            this.lblDept = new C1.Win.C1Input.C1Label();
            this.txtDeptCd = new C1.Win.C1Input.C1TextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pdfViewer = new AxAcroPDFLib.AxAcroPDF();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboDocType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDocType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDocCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSlipNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlipNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(573, 613);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 133);
            this.GridCommPanel.Size = new System.Drawing.Size(573, 613);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(567, 593);
            this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1224, 64);
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
            this.panel1.Size = new System.Drawing.Size(1224, 69);
            this.panel1.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cboDocType);
            this.groupBox1.Controls.Add(this.lblDocType);
            this.groupBox1.Controls.Add(this.lblDocCode);
            this.groupBox1.Controls.Add(this.txtDocCode);
            this.groupBox1.Controls.Add(this.lblSlipNo);
            this.groupBox1.Controls.Add(this.txtSlipNo);
            this.groupBox1.Controls.Add(this.txtDeptNm);
            this.groupBox1.Controls.Add(this.lblDept);
            this.groupBox1.Controls.Add(this.txtDeptCd);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1224, 69);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // cboDocType
            // 
            this.cboDocType.AddItemSeparator = ';';
            this.cboDocType.AutoSize = false;
            this.cboDocType.Caption = "";
            this.cboDocType.CaptionHeight = 17;
            this.cboDocType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboDocType.ColumnCaptionHeight = 18;
            this.cboDocType.ColumnFooterHeight = 18;
            this.cboDocType.ContentHeight = 15;
            this.cboDocType.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboDocType.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboDocType.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboDocType.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboDocType.EditorHeight = 15;
            this.cboDocType.Images.Add(((System.Drawing.Image)(resources.GetObject("cboDocType.Images"))));
            this.cboDocType.ItemHeight = 15;
            this.cboDocType.Location = new System.Drawing.Point(325, 42);
            this.cboDocType.MatchEntryTimeout = ((long)(2000));
            this.cboDocType.MaxDropDownItems = ((short)(5));
            this.cboDocType.MaxLength = 32767;
            this.cboDocType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboDocType.Name = "cboDocType";
            this.cboDocType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboDocType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboDocType.Size = new System.Drawing.Size(90, 21);
            this.cboDocType.TabIndex = 25;
            this.cboDocType.TabStop = false;
            this.cboDocType.Tag = ";2;;";
            this.cboDocType.PropBag = resources.GetString("cboDocType.PropBag");
            // 
            // lblDocType
            // 
            this.lblDocType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblDocType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblDocType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDocType.Location = new System.Drawing.Point(232, 42);
            this.lblDocType.Name = "lblDocType";
            this.lblDocType.Size = new System.Drawing.Size(94, 21);
            this.lblDocType.TabIndex = 63;
            this.lblDocType.Tag = null;
            this.lblDocType.Text = "문서종류";
            this.lblDocType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDocType.TextDetached = true;
            this.lblDocType.Value = "";
            // 
            // lblDocCode
            // 
            this.lblDocCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblDocCode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblDocCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDocCode.Location = new System.Drawing.Point(14, 42);
            this.lblDocCode.Name = "lblDocCode";
            this.lblDocCode.Size = new System.Drawing.Size(94, 21);
            this.lblDocCode.TabIndex = 18;
            this.lblDocCode.Tag = null;
            this.lblDocCode.Text = "문서코드";
            this.lblDocCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDocCode.TextDetached = true;
            this.lblDocCode.Value = "";
            // 
            // txtDocCode
            // 
            this.txtDocCode.AutoSize = false;
            this.txtDocCode.BackColor = System.Drawing.Color.White;
            this.txtDocCode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDocCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDocCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDocCode.Location = new System.Drawing.Point(107, 42);
            this.txtDocCode.Name = "txtDocCode";
            this.txtDocCode.Size = new System.Drawing.Size(104, 21);
            this.txtDocCode.TabIndex = 20;
            this.txtDocCode.Tag = ";2;;";
            this.txtDocCode.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblSlipNo
            // 
            this.lblSlipNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblSlipNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblSlipNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSlipNo.Location = new System.Drawing.Point(14, 18);
            this.lblSlipNo.Name = "lblSlipNo";
            this.lblSlipNo.Size = new System.Drawing.Size(94, 21);
            this.lblSlipNo.TabIndex = 16;
            this.lblSlipNo.Tag = null;
            this.lblSlipNo.Text = "전표번호";
            this.lblSlipNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSlipNo.TextDetached = true;
            this.lblSlipNo.Value = "";
            // 
            // txtSlipNo
            // 
            this.txtSlipNo.AutoSize = false;
            this.txtSlipNo.BackColor = System.Drawing.Color.White;
            this.txtSlipNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSlipNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSlipNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSlipNo.Location = new System.Drawing.Point(107, 18);
            this.txtSlipNo.Name = "txtSlipNo";
            this.txtSlipNo.Size = new System.Drawing.Size(104, 21);
            this.txtSlipNo.TabIndex = 5;
            this.txtSlipNo.Tag = ";2;;";
            this.txtSlipNo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // txtDeptNm
            // 
            this.txtDeptNm.AutoSize = false;
            this.txtDeptNm.BackColor = System.Drawing.Color.White;
            this.txtDeptNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptNm.Location = new System.Drawing.Point(414, 17);
            this.txtDeptNm.Name = "txtDeptNm";
            this.txtDeptNm.Size = new System.Drawing.Size(264, 21);
            this.txtDeptNm.TabIndex = 15;
            this.txtDeptNm.Tag = ";2;;";
            this.txtDeptNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblDept
            // 
            this.lblDept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblDept.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblDept.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDept.Location = new System.Drawing.Point(232, 17);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(94, 21);
            this.lblDept.TabIndex = 4;
            this.lblDept.Tag = null;
            this.lblDept.Text = "발의부서";
            this.lblDept.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDept.TextDetached = true;
            this.lblDept.Value = "";
            // 
            // txtDeptCd
            // 
            this.txtDeptCd.AutoSize = false;
            this.txtDeptCd.BackColor = System.Drawing.Color.White;
            this.txtDeptCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDeptCd.Location = new System.Drawing.Point(325, 17);
            this.txtDeptCd.Name = "txtDeptCd";
            this.txtDeptCd.Size = new System.Drawing.Size(90, 21);
            this.txtDeptCd.TabIndex = 10;
            this.txtDeptCd.Tag = ";2;;";
            this.txtDeptCd.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(573, 133);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(8, 613);
            this.splitter2.TabIndex = 40;
            this.splitter2.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pdfViewer);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(581, 133);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(643, 613);
            this.panel2.TabIndex = 41;
            // 
            // pdfViewer
            // 
            this.pdfViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewer.Enabled = true;
            this.pdfViewer.Location = new System.Drawing.Point(0, 0);
            this.pdfViewer.Name = "pdfViewer";
            this.pdfViewer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("pdfViewer.OcxState")));
            this.pdfViewer.Size = new System.Drawing.Size(643, 613);
            this.pdfViewer.TabIndex = 1;
            // 
            // ACD001P8
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1224, 746);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACD001P8";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "전표지출증빙등록";
            this.Load += new System.EventHandler(this.ACD007P2_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.GridCommPanel, 0);
            this.Controls.SetChildIndex(this.splitter2, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboDocType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDocType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDocCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSlipNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlipNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1TextBox txtDeptNm;
        private C1.Win.C1Input.C1Label lblDept;
        private C1.Win.C1Input.C1TextBox txtDeptCd;
        private C1.Win.C1Input.C1Label lblSlipNo;
        private C1.Win.C1Input.C1TextBox txtSlipNo;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panel2;
        private AxAcroPDFLib.AxAcroPDF pdfViewer;
        private C1.Win.C1Input.C1Label lblDocCode;
        private C1.Win.C1Input.C1TextBox txtDocCode;
        private C1.Win.C1List.C1Combo cboDocType;
        private C1.Win.C1Input.C1Label lblDocType;
    }
}