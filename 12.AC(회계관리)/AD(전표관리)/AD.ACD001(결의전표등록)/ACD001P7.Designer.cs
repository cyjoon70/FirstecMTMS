namespace AD.ACD001
{
    partial class ACD001P7
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACD001P7));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblAssignComment = new C1.Win.C1Input.C1Label();
            this.txtAssignComment = new C1.Win.C1Input.C1TextBox();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnRoutChange = new System.Windows.Forms.Button();
            this.chkMajorYn = new System.Windows.Forms.CheckBox();
            this.cboRoutNo = new C1.Win.C1List.C1Combo();
            this.lblRoutNo = new C1.Win.C1Input.C1Label();
            this.cboGwStatus = new C1.Win.C1List.C1Combo();
            this.lblGwStatus = new C1.Win.C1Input.C1Label();
            this.lblAssignNo = new C1.Win.C1Input.C1Label();
            this.txtAssignNo = new C1.Win.C1Input.C1TextBox();
            this.lblSlipNo = new C1.Win.C1Input.C1Label();
            this.txtSlipNo = new C1.Win.C1Input.C1TextBox();
            this.cboTaskType = new C1.Win.C1List.C1Combo();
            this.lblTaskType = new C1.Win.C1Input.C1Label();
            this.txtUserNm = new C1.Win.C1Input.C1TextBox();
            this.lblUser = new C1.Win.C1Input.C1Label();
            this.txtUserId = new C1.Win.C1Input.C1TextBox();
            this.btnAssign = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignComment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignComment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoutNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRoutNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGwStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGwStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSlipNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlipNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTaskType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTaskType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1099, 338);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 159);
            this.GridCommPanel.Size = new System.Drawing.Size(1099, 338);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1093, 318);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Controls.Add(this.btnAssign);
            this.panButton1.Size = new System.Drawing.Size(1099, 64);
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
            this.panButton1.Controls.SetChildIndex(this.btnAssign, 0);
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
            this.panel1.Size = new System.Drawing.Size(1099, 95);
            this.panel1.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.lblAssignComment);
            this.groupBox1.Controls.Add(this.txtAssignComment);
            this.groupBox1.Controls.Add(this.btnChange);
            this.groupBox1.Controls.Add(this.btnRoutChange);
            this.groupBox1.Controls.Add(this.chkMajorYn);
            this.groupBox1.Controls.Add(this.cboRoutNo);
            this.groupBox1.Controls.Add(this.lblRoutNo);
            this.groupBox1.Controls.Add(this.cboGwStatus);
            this.groupBox1.Controls.Add(this.lblGwStatus);
            this.groupBox1.Controls.Add(this.lblAssignNo);
            this.groupBox1.Controls.Add(this.txtAssignNo);
            this.groupBox1.Controls.Add(this.lblSlipNo);
            this.groupBox1.Controls.Add(this.txtSlipNo);
            this.groupBox1.Controls.Add(this.cboTaskType);
            this.groupBox1.Controls.Add(this.lblTaskType);
            this.groupBox1.Controls.Add(this.txtUserNm);
            this.groupBox1.Controls.Add(this.lblUser);
            this.groupBox1.Controls.Add(this.txtUserId);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1099, 95);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // lblAssignComment
            // 
            this.lblAssignComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblAssignComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblAssignComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAssignComment.Location = new System.Drawing.Point(617, 25);
            this.lblAssignComment.Name = "lblAssignComment";
            this.lblAssignComment.Size = new System.Drawing.Size(94, 21);
            this.lblAssignComment.TabIndex = 80;
            this.lblAssignComment.Tag = null;
            this.lblAssignComment.Text = "상신코멘트";
            this.lblAssignComment.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAssignComment.TextDetached = true;
            this.lblAssignComment.Value = "";
            this.lblAssignComment.Visible = false;
            // 
            // txtAssignComment
            // 
            this.txtAssignComment.AutoSize = false;
            this.txtAssignComment.BackColor = System.Drawing.Color.White;
            this.txtAssignComment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAssignComment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssignComment.EmptyAsNull = true;
            this.txtAssignComment.Location = new System.Drawing.Point(617, 45);
            this.txtAssignComment.Multiline = true;
            this.txtAssignComment.Name = "txtAssignComment";
            this.txtAssignComment.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAssignComment.Size = new System.Drawing.Size(267, 36);
            this.txtAssignComment.TabIndex = 79;
            this.txtAssignComment.Tag = null;
            this.txtAssignComment.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtAssignComment.Visible = false;
            // 
            // btnChange
            // 
            this.btnChange.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChange.Image = ((System.Drawing.Image)(resources.GetObject("btnChange.Image")));
            this.btnChange.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnChange.Location = new System.Drawing.Point(890, 43);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(152, 41);
            this.btnChange.TabIndex = 69;
            this.btnChange.Text = "결재자등록(멀티)";
            this.btnChange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnRoutChange
            // 
            this.btnRoutChange.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRoutChange.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRoutChange.Location = new System.Drawing.Point(486, 39);
            this.btnRoutChange.Name = "btnRoutChange";
            this.btnRoutChange.Size = new System.Drawing.Size(90, 23);
            this.btnRoutChange.TabIndex = 68;
            this.btnRoutChange.Text = "결재라인변경";
            this.btnRoutChange.UseVisualStyleBackColor = true;
            this.btnRoutChange.Visible = false;
            this.btnRoutChange.Click += new System.EventHandler(this.btnRoutChange_Click);
            // 
            // chkMajorYn
            // 
            this.chkMajorYn.AutoSize = true;
            this.chkMajorYn.Enabled = false;
            this.chkMajorYn.Location = new System.Drawing.Point(486, 68);
            this.chkMajorYn.Name = "chkMajorYn";
            this.chkMajorYn.Size = new System.Drawing.Size(84, 16);
            this.chkMajorYn.TabIndex = 66;
            this.chkMajorYn.Text = "기본결재선";
            this.chkMajorYn.UseVisualStyleBackColor = true;
            // 
            // cboRoutNo
            // 
            this.cboRoutNo.AddItemSeparator = ';';
            this.cboRoutNo.AutoSize = false;
            this.cboRoutNo.Caption = "";
            this.cboRoutNo.CaptionHeight = 17;
            this.cboRoutNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboRoutNo.ColumnCaptionHeight = 18;
            this.cboRoutNo.ColumnFooterHeight = 18;
            this.cboRoutNo.ContentHeight = 15;
            this.cboRoutNo.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboRoutNo.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboRoutNo.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboRoutNo.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboRoutNo.EditorHeight = 15;
            this.cboRoutNo.Images.Add(((System.Drawing.Image)(resources.GetObject("cboRoutNo.Images"))));
            this.cboRoutNo.ItemHeight = 15;
            this.cboRoutNo.Location = new System.Drawing.Point(320, 65);
            this.cboRoutNo.MatchEntryTimeout = ((long)(2000));
            this.cboRoutNo.MaxDropDownItems = ((short)(5));
            this.cboRoutNo.MaxLength = 32767;
            this.cboRoutNo.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboRoutNo.Name = "cboRoutNo";
            this.cboRoutNo.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboRoutNo.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboRoutNo.Size = new System.Drawing.Size(159, 21);
            this.cboRoutNo.TabIndex = 64;
            this.cboRoutNo.TabStop = false;
            this.cboRoutNo.Tag = "";
            this.cboRoutNo.SelectedValueChanged += new System.EventHandler(this.cboRoutNo_SelectedValueChanged);
            this.cboRoutNo.PropBag = resources.GetString("cboRoutNo.PropBag");
            // 
            // lblRoutNo
            // 
            this.lblRoutNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblRoutNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblRoutNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRoutNo.Location = new System.Drawing.Point(227, 65);
            this.lblRoutNo.Name = "lblRoutNo";
            this.lblRoutNo.Size = new System.Drawing.Size(94, 21);
            this.lblRoutNo.TabIndex = 63;
            this.lblRoutNo.Tag = null;
            this.lblRoutNo.Text = "결재선번호";
            this.lblRoutNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRoutNo.TextDetached = true;
            this.lblRoutNo.Value = "";
            // 
            // cboGwStatus
            // 
            this.cboGwStatus.AddItemSeparator = ';';
            this.cboGwStatus.AutoSize = false;
            this.cboGwStatus.Caption = "";
            this.cboGwStatus.CaptionHeight = 17;
            this.cboGwStatus.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboGwStatus.ColumnCaptionHeight = 18;
            this.cboGwStatus.ColumnFooterHeight = 18;
            this.cboGwStatus.ContentHeight = 15;
            this.cboGwStatus.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboGwStatus.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboGwStatus.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboGwStatus.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboGwStatus.EditorHeight = 15;
            this.cboGwStatus.Images.Add(((System.Drawing.Image)(resources.GetObject("cboGwStatus.Images"))));
            this.cboGwStatus.ItemHeight = 15;
            this.cboGwStatus.Location = new System.Drawing.Point(107, 65);
            this.cboGwStatus.MatchEntryTimeout = ((long)(2000));
            this.cboGwStatus.MaxDropDownItems = ((short)(5));
            this.cboGwStatus.MaxLength = 32767;
            this.cboGwStatus.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboGwStatus.Name = "cboGwStatus";
            this.cboGwStatus.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboGwStatus.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboGwStatus.Size = new System.Drawing.Size(104, 21);
            this.cboGwStatus.TabIndex = 62;
            this.cboGwStatus.TabStop = false;
            this.cboGwStatus.Tag = ";2;;";
            this.cboGwStatus.PropBag = resources.GetString("cboGwStatus.PropBag");
            // 
            // lblGwStatus
            // 
            this.lblGwStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblGwStatus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblGwStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGwStatus.Location = new System.Drawing.Point(14, 65);
            this.lblGwStatus.Name = "lblGwStatus";
            this.lblGwStatus.Size = new System.Drawing.Size(94, 21);
            this.lblGwStatus.TabIndex = 61;
            this.lblGwStatus.Tag = null;
            this.lblGwStatus.Text = "결재상태";
            this.lblGwStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblGwStatus.TextDetached = true;
            this.lblGwStatus.Value = "";
            // 
            // lblAssignNo
            // 
            this.lblAssignNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblAssignNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblAssignNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAssignNo.Location = new System.Drawing.Point(14, 40);
            this.lblAssignNo.Name = "lblAssignNo";
            this.lblAssignNo.Size = new System.Drawing.Size(94, 21);
            this.lblAssignNo.TabIndex = 18;
            this.lblAssignNo.Tag = null;
            this.lblAssignNo.Text = "결재요청번호";
            this.lblAssignNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAssignNo.TextDetached = true;
            this.lblAssignNo.Value = "";
            // 
            // txtAssignNo
            // 
            this.txtAssignNo.AutoSize = false;
            this.txtAssignNo.BackColor = System.Drawing.Color.White;
            this.txtAssignNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtAssignNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssignNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAssignNo.Location = new System.Drawing.Point(107, 40);
            this.txtAssignNo.Name = "txtAssignNo";
            this.txtAssignNo.Size = new System.Drawing.Size(104, 21);
            this.txtAssignNo.TabIndex = 19;
            this.txtAssignNo.Tag = ";2;;";
            this.txtAssignNo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblSlipNo
            // 
            this.lblSlipNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblSlipNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblSlipNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSlipNo.Location = new System.Drawing.Point(14, 15);
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
            this.txtSlipNo.Location = new System.Drawing.Point(107, 15);
            this.txtSlipNo.Name = "txtSlipNo";
            this.txtSlipNo.Size = new System.Drawing.Size(104, 21);
            this.txtSlipNo.TabIndex = 17;
            this.txtSlipNo.Tag = ";2;;";
            this.txtSlipNo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
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
            this.cboTaskType.Location = new System.Drawing.Point(320, 40);
            this.cboTaskType.MatchEntryTimeout = ((long)(2000));
            this.cboTaskType.MaxDropDownItems = ((short)(5));
            this.cboTaskType.MaxLength = 32767;
            this.cboTaskType.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboTaskType.Name = "cboTaskType";
            this.cboTaskType.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboTaskType.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboTaskType.Size = new System.Drawing.Size(159, 21);
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
            this.lblTaskType.Location = new System.Drawing.Point(227, 40);
            this.lblTaskType.Name = "lblTaskType";
            this.lblTaskType.Size = new System.Drawing.Size(94, 21);
            this.lblTaskType.TabIndex = 14;
            this.lblTaskType.Tag = null;
            this.lblTaskType.Text = "업무구분";
            this.lblTaskType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTaskType.TextDetached = true;
            this.lblTaskType.Value = "";
            // 
            // txtUserNm
            // 
            this.txtUserNm.AutoSize = false;
            this.txtUserNm.BackColor = System.Drawing.Color.White;
            this.txtUserNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUserNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserNm.Location = new System.Drawing.Point(399, 15);
            this.txtUserNm.Name = "txtUserNm";
            this.txtUserNm.Size = new System.Drawing.Size(80, 21);
            this.txtUserNm.TabIndex = 7;
            this.txtUserNm.Tag = ";2;;";
            this.txtUserNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // lblUser
            // 
            this.lblUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.lblUser.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.lblUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblUser.Location = new System.Drawing.Point(227, 15);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(94, 21);
            this.lblUser.TabIndex = 4;
            this.lblUser.Tag = null;
            this.lblUser.Text = "사용자";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUser.TextDetached = true;
            this.lblUser.Value = "";
            // 
            // txtUserId
            // 
            this.txtUserId.AutoSize = false;
            this.txtUserId.BackColor = System.Drawing.Color.White;
            this.txtUserId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUserId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserId.Location = new System.Drawing.Point(320, 15);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(80, 21);
            this.txtUserId.TabIndex = 5;
            this.txtUserId.Tag = ";2;;";
            this.txtUserId.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // btnAssign
            // 
            this.btnAssign.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAssign.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAssign.Location = new System.Drawing.Point(717, 8);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(48, 48);
            this.btnAssign.TabIndex = 8;
            this.btnAssign.Text = "결재상신";
            this.btnAssign.UseVisualStyleBackColor = true;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // ACD001P7
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1099, 497);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACD001P7";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "결재라인조회";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ACD001P7_FormClosing);
            this.Load += new System.EventHandler(this.ACD001P7_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignComment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignComment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoutNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRoutNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboGwStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGwStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblAssignNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAssignNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSlipNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSlipNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTaskType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTaskType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1TextBox txtUserNm;
        private C1.Win.C1Input.C1Label lblUser;
        private C1.Win.C1Input.C1TextBox txtUserId;
        private System.Windows.Forms.Button btnAssign;
        private C1.Win.C1List.C1Combo cboTaskType;
        private C1.Win.C1Input.C1Label lblTaskType;
        private C1.Win.C1Input.C1Label lblSlipNo;
        private C1.Win.C1Input.C1TextBox txtSlipNo;
        private C1.Win.C1Input.C1Label lblAssignNo;
        private C1.Win.C1Input.C1TextBox txtAssignNo;
        private C1.Win.C1List.C1Combo cboGwStatus;
        private C1.Win.C1Input.C1Label lblGwStatus;
        private C1.Win.C1List.C1Combo cboRoutNo;
        private C1.Win.C1Input.C1Label lblRoutNo;
        private System.Windows.Forms.CheckBox chkMajorYn;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnRoutChange;
        private C1.Win.C1Input.C1TextBox txtAssignComment;
        private C1.Win.C1Input.C1Label lblAssignComment;
    }
}