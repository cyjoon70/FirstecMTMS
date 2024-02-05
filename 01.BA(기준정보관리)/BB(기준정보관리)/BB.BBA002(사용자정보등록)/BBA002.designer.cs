namespace BB.BBA002
{
    partial class BBA002
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BBA002));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnHistory = new C1.Win.C1Input.C1Button();
            this.cboUSE_FLAG = new C1.Win.C1List.C1Combo();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.txtUsernm = new C1.Win.C1Input.C1TextBox();
            this.txtUser = new C1.Win.C1Input.C1TextBox();
            this.lblInfo = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboUSE_FLAG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsernm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1244, 403);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 123);
            this.GridCommPanel.Size = new System.Drawing.Size(1244, 403);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1238, 383);
            this.fpSpread1.SelectionChanged += new FarPoint.Win.Spread.SelectionChangedEventHandler(this.fpSpread1_SelectionChanged);
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            this.fpSpread1.EditChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_EditChange);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1244, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1244, 59);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.lblInfo);
            this.groupBox1.Controls.Add(this.btnHistory);
            this.groupBox1.Controls.Add(this.cboUSE_FLAG);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.txtUsernm);
            this.groupBox1.Controls.Add(this.txtUser);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1244, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnHistory
            // 
            this.btnHistory.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnHistory.BackgroundImage")));
            this.btnHistory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistory.Location = new System.Drawing.Point(743, 19);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(81, 27);
            this.btnHistory.TabIndex = 246;
            this.btnHistory.Text = "이력보기";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // cboUSE_FLAG
            // 
            this.cboUSE_FLAG.AddItemSeparator = ';';
            this.cboUSE_FLAG.AutoSize = false;
            this.cboUSE_FLAG.Caption = "";
            this.cboUSE_FLAG.CaptionHeight = 17;
            this.cboUSE_FLAG.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboUSE_FLAG.ColumnCaptionHeight = 18;
            this.cboUSE_FLAG.ColumnFooterHeight = 18;
            this.cboUSE_FLAG.ContentHeight = 15;
            this.cboUSE_FLAG.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboUSE_FLAG.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboUSE_FLAG.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboUSE_FLAG.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboUSE_FLAG.EditorHeight = 15;
            this.cboUSE_FLAG.Images.Add(((System.Drawing.Image)(resources.GetObject("cboUSE_FLAG.Images"))));
            this.cboUSE_FLAG.ItemHeight = 15;
            this.cboUSE_FLAG.Location = new System.Drawing.Point(455, 22);
            this.cboUSE_FLAG.MatchEntryTimeout = ((long)(2000));
            this.cboUSE_FLAG.MaxDropDownItems = ((short)(5));
            this.cboUSE_FLAG.MaxLength = 32767;
            this.cboUSE_FLAG.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboUSE_FLAG.Name = "cboUSE_FLAG";
            this.cboUSE_FLAG.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboUSE_FLAG.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboUSE_FLAG.Size = new System.Drawing.Size(204, 21);
            this.cboUSE_FLAG.TabIndex = 245;
            this.cboUSE_FLAG.Tag = ";1;;";
            this.cboUSE_FLAG.PropBag = resources.GetString("cboUSE_FLAG.PropBag");
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(376, 22);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(80, 21);
            this.c1Label1.TabIndex = 244;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "사용여부";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(16, 21);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(80, 21);
            this.c1Label5.TabIndex = 242;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "사용자";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // txtUsernm
            // 
            this.txtUsernm.AutoSize = false;
            this.txtUsernm.BackColor = System.Drawing.Color.White;
            this.txtUsernm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUsernm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsernm.Location = new System.Drawing.Point(202, 21);
            this.txtUsernm.Name = "txtUsernm";
            this.txtUsernm.Size = new System.Drawing.Size(154, 21);
            this.txtUsernm.TabIndex = 2;
            this.txtUsernm.Tag = null;
            // 
            // txtUser
            // 
            this.txtUser.AutoSize = false;
            this.txtUser.BackColor = System.Drawing.Color.White;
            this.txtUser.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUser.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUser.Location = new System.Drawing.Point(95, 21);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(105, 21);
            this.txtUser.TabIndex = 1;
            this.txtUser.Tag = null;
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_TextChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblInfo.Location = new System.Drawing.Point(888, 30);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(248, 12);
            this.lblInfo.TabIndex = 247;
            this.lblInfo.Text = "* 사용자ID/사용자명 더블클릭: 변경이력팝업";
            // 
            // BBA002
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1244, 526);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BBA002";
            this.Text = "사용자정보등록";
            this.Load += new System.EventHandler(this.BBA002_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboUSE_FLAG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsernm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1TextBox txtUser;
        private C1.Win.C1Input.C1TextBox txtUsernm;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1List.C1Combo cboUSE_FLAG;
        private C1.Win.C1Input.C1Button btnHistory;
        private System.Windows.Forms.Label lblInfo;
    }
}