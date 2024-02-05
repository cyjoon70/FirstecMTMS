namespace IN.INV113
{
    partial class INV113
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(INV113));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSpec = new C1.Win.C1Input.C1TextBox();
            this.cboPlantCd = new C1.Win.C1List.C1Combo();
            this.btnPreview = new C1.Win.C1Input.C1Button();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.btnItem = new C1.Win.C1Input.C1Button();
            this.txtItemCd = new C1.Win.C1Input.C1TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.mskDT = new C1.Win.C1Input.C1DateEdit();
            this.txtItemNm = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.txtUnit = new C1.Win.C1Input.C1TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtSpec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUnit)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(784, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(784, 463);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtUnit);
            this.groupBox1.Controls.Add(this.txtSpec);
            this.groupBox1.Controls.Add(this.cboPlantCd);
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.btnItem);
            this.groupBox1.Controls.Add(this.txtItemCd);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.mskDT);
            this.groupBox1.Controls.Add(this.txtItemNm);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtSpec
            // 
            this.txtSpec.AutoSize = false;
            this.txtSpec.BackColor = System.Drawing.Color.White;
            this.txtSpec.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtSpec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSpec.Location = new System.Drawing.Point(247, 103);
            this.txtSpec.Name = "txtSpec";
            this.txtSpec.Size = new System.Drawing.Size(150, 21);
            this.txtSpec.TabIndex = 10;
            this.txtSpec.Tag = ";2;;";
            this.txtSpec.Visible = false;
            // 
            // cboPlantCd
            // 
            this.cboPlantCd.AddItemSeparator = ';';
            this.cboPlantCd.AutoSize = false;
            this.cboPlantCd.Caption = "";
            this.cboPlantCd.CaptionHeight = 17;
            this.cboPlantCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboPlantCd.ColumnCaptionHeight = 18;
            this.cboPlantCd.ColumnFooterHeight = 18;
            this.cboPlantCd.ContentHeight = 15;
            this.cboPlantCd.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboPlantCd.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboPlantCd.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboPlantCd.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboPlantCd.EditorHeight = 15;
            this.cboPlantCd.Images.Add(((System.Drawing.Image)(resources.GetObject("cboPlantCd.Images"))));
            this.cboPlantCd.ItemHeight = 15;
            this.cboPlantCd.Location = new System.Drawing.Point(124, 22);
            this.cboPlantCd.MatchEntryTimeout = ((long)(2000));
            this.cboPlantCd.MaxDropDownItems = ((short)(5));
            this.cboPlantCd.MaxLength = 32767;
            this.cboPlantCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboPlantCd.Name = "cboPlantCd";
            this.cboPlantCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboPlantCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboPlantCd.Size = new System.Drawing.Size(119, 21);
            this.cboPlantCd.TabIndex = 1;
            this.cboPlantCd.Tag = ";1;;";
            this.cboPlantCd.PropBag = resources.GetString("cboPlantCd.PropBag");
            // 
            // btnPreview
            // 
            this.btnPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPreview.BackgroundImage")));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.Location = new System.Drawing.Point(20, 169);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(104, 25);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "미리보기";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(20, 49);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(104, 21);
            this.c1Label5.TabIndex = 2;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "기준일";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // btnItem
            // 
            this.btnItem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnItem.BackgroundImage")));
            this.btnItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnItem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnItem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnItem.Location = new System.Drawing.Point(223, 76);
            this.btnItem.Name = "btnItem";
            this.btnItem.Size = new System.Drawing.Size(24, 21);
            this.btnItem.TabIndex = 5;
            this.btnItem.UseVisualStyleBackColor = true;
            this.btnItem.Click += new System.EventHandler(this.btnItem_Click);
            // 
            // txtItemCd
            // 
            this.txtItemCd.AutoSize = false;
            this.txtItemCd.BackColor = System.Drawing.Color.White;
            this.txtItemCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtItemCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtItemCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtItemCd.Location = new System.Drawing.Point(123, 76);
            this.txtItemCd.Name = "txtItemCd";
            this.txtItemCd.Size = new System.Drawing.Size(100, 21);
            this.txtItemCd.TabIndex = 4;
            this.txtItemCd.Tag = ";1;;";
            this.txtItemCd.TextChanged += new System.EventHandler(this.txtItemCd_TextChanged);
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(20, 76);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(104, 21);
            this.c1Label3.TabIndex = 3;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "품목";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // mskDT
            // 
            this.mskDT.AutoSize = false;
            this.mskDT.BackColor = System.Drawing.Color.White;
            this.mskDT.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.mskDT.Calendar.DayNameLength = 1;
            this.mskDT.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.mskDT.Location = new System.Drawing.Point(123, 49);
            this.mskDT.Name = "mskDT";
            this.mskDT.Size = new System.Drawing.Size(107, 21);
            this.mskDT.TabIndex = 13;
            this.mskDT.Tag = ";1;;";
            this.mskDT.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.mskDT.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // txtItemNm
            // 
            this.txtItemNm.AutoSize = false;
            this.txtItemNm.BackColor = System.Drawing.Color.White;
            this.txtItemNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtItemNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtItemNm.Location = new System.Drawing.Point(247, 76);
            this.txtItemNm.Name = "txtItemNm";
            this.txtItemNm.Size = new System.Drawing.Size(150, 21);
            this.txtItemNm.TabIndex = 6;
            this.txtItemNm.Tag = ";2;;";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(20, 22);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(104, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "공장";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // txtUnit
            // 
            this.txtUnit.AutoSize = false;
            this.txtUnit.BackColor = System.Drawing.Color.White;
            this.txtUnit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUnit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUnit.Location = new System.Drawing.Point(248, 130);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(150, 21);
            this.txtUnit.TabIndex = 15;
            this.txtUnit.Tag = ";2;;";
            this.txtUnit.Visible = false;
            // 
            // INV113
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "INV113";
            this.Text = "품목별 재고이력조회";
            this.Activated += new System.EventHandler(this.INV113_Activated);
            this.Deactivate += new System.EventHandler(this.INV113_Deactivate);
            this.Load += new System.EventHandler(this.INV113_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtSpec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUnit)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtItemNm;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Button btnItem;
        private C1.Win.C1Input.C1TextBox txtItemCd;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1DateEdit mskDT;
        private C1.Win.C1Input.C1Button btnPreview;
        private C1.Win.C1List.C1Combo cboPlantCd;
        private C1.Win.C1Input.C1TextBox txtSpec;
        private C1.Win.C1Input.C1TextBox txtUnit;

    }
}