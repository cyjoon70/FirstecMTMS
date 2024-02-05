namespace AZ.ACZ003
{
    partial class ACZ003
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACZ003));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSlipYYMM_To = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpSlipYYMM_Fr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label12 = new C1.Win.C1Input.C1Label();
            this.cboCoCd = new C1.Win.C1List.C1Combo();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipYYMM_To)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipYYMM_Fr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCoCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1102, 387);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 110);
            this.GridCommPanel.Size = new System.Drawing.Size(1102, 387);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1096, 367);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1102, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1102, 46);
            this.panel1.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.dtpSlipYYMM_To);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpSlipYYMM_Fr);
            this.groupBox1.Controls.Add(this.c1Label12);
            this.groupBox1.Controls.Add(this.cboCoCd);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1102, 46);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // dtpSlipYYMM_To
            // 
            this.dtpSlipYYMM_To.AutoSize = false;
            this.dtpSlipYYMM_To.BackColor = System.Drawing.Color.White;
            this.dtpSlipYYMM_To.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipYYMM_To.Calendar.DayNameLength = 1;
            this.dtpSlipYYMM_To.CustomFormat = "yyyy-MM";
            this.dtpSlipYYMM_To.EmptyAsNull = true;
            this.dtpSlipYYMM_To.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpSlipYYMM_To.Location = new System.Drawing.Point(255, 17);
            this.dtpSlipYYMM_To.Name = "dtpSlipYYMM_To";
            this.dtpSlipYYMM_To.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipYYMM_To.TabIndex = 3;
            this.dtpSlipYYMM_To.Tag = "회계년월;1;;";
            this.dtpSlipYYMM_To.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipYYMM_To.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "~";
            // 
            // dtpSlipYYMM_Fr
            // 
            this.dtpSlipYYMM_Fr.AutoSize = false;
            this.dtpSlipYYMM_Fr.BackColor = System.Drawing.Color.White;
            this.dtpSlipYYMM_Fr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipYYMM_Fr.Calendar.DayNameLength = 1;
            this.dtpSlipYYMM_Fr.CustomFormat = "yyyy-MM";
            this.dtpSlipYYMM_Fr.EmptyAsNull = true;
            this.dtpSlipYYMM_Fr.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpSlipYYMM_Fr.Location = new System.Drawing.Point(105, 17);
            this.dtpSlipYYMM_Fr.Name = "dtpSlipYYMM_Fr";
            this.dtpSlipYYMM_Fr.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipYYMM_Fr.TabIndex = 1;
            this.dtpSlipYYMM_Fr.Tag = "회계년월;1;;";
            this.dtpSlipYYMM_Fr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipYYMM_Fr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label12
            // 
            this.c1Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label12.Location = new System.Drawing.Point(12, 17);
            this.c1Label12.Name = "c1Label12";
            this.c1Label12.Size = new System.Drawing.Size(94, 21);
            this.c1Label12.TabIndex = 0;
            this.c1Label12.Tag = null;
            this.c1Label12.Text = "회계년도";
            this.c1Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label12.TextDetached = true;
            this.c1Label12.Value = "";
            // 
            // cboCoCd
            // 
            this.cboCoCd.AddItemSeparator = ';';
            this.cboCoCd.AutoSize = false;
            this.cboCoCd.Caption = "";
            this.cboCoCd.CaptionHeight = 17;
            this.cboCoCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboCoCd.ColumnCaptionHeight = 18;
            this.cboCoCd.ColumnFooterHeight = 18;
            this.cboCoCd.ContentHeight = 15;
            this.cboCoCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboCoCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboCoCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboCoCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboCoCd.EditorHeight = 15;
            this.cboCoCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboCoCd.Images"))));
            this.cboCoCd.ItemHeight = 15;
            this.cboCoCd.Location = new System.Drawing.Point(554, 17);
            this.cboCoCd.MatchEntryTimeout = ((long)(2000));
            this.cboCoCd.MaxDropDownItems = ((short)(5));
            this.cboCoCd.MaxLength = 32767;
            this.cboCoCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboCoCd.Name = "cboCoCd";
            this.cboCoCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboCoCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboCoCd.Size = new System.Drawing.Size(148, 21);
            this.cboCoCd.TabIndex = 5;
            this.cboCoCd.Tag = ";;;";
            this.cboCoCd.PropBag = resources.GetString("cboCoCd.PropBag");
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(461, 17);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(94, 21);
            this.c1Label6.TabIndex = 4;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "법인";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // ACZ003
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1102, 497);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACZ003";
            this.Text = "통합결산수정분개";
            this.Load += new System.EventHandler(this.ACZ003_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipYYMM_To)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipYYMM_Fr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCoCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1DateEdit dtpSlipYYMM_Fr;
        private C1.Win.C1Input.C1Label c1Label12;
        private C1.Win.C1List.C1Combo cboCoCd;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1DateEdit dtpSlipYYMM_To;
        private System.Windows.Forms.Label label1;

    }
}