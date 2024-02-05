namespace IN.INV118
{
    partial class INV118
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(INV118));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboPlantCd = new C1.Win.C1List.C1Combo();
            this.btnPreview = new C1.Win.C1Input.C1Button();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.cboItemAcct = new C1.Win.C1List.C1Combo();
            this.mskDT_Fr = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.mskDT_To = new C1.Win.C1Input.C1DateEdit();
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
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemAcct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_Fr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_To)).BeginInit();
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
            this.groupBox1.Controls.Add(this.mskDT_Fr);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mskDT_To);
            this.groupBox1.Controls.Add(this.cboItemAcct);
            this.groupBox1.Controls.Add(this.cboPlantCd);
            this.groupBox1.Controls.Add(this.btnPreview);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
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
            this.cboPlantCd.TabIndex = 2;
            this.cboPlantCd.Tag = ";1;;";
            this.cboPlantCd.PropBag = resources.GetString("cboPlantCd.PropBag");
            // 
            // btnPreview
            // 
            this.btnPreview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPreview.BackgroundImage")));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.Location = new System.Drawing.Point(20, 143);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(104, 25);
            this.btnPreview.TabIndex = 0;
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
            this.c1Label5.TabIndex = 3;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "수불기간";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(20, 76);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(104, 21);
            this.c1Label3.TabIndex = 7;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "품목계정";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(20, 22);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(104, 21);
            this.c1Label1.TabIndex = 1;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "공장";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // cboItemAcct
            // 
            this.cboItemAcct.AddItemSeparator = ';';
            this.cboItemAcct.AutoSize = false;
            this.cboItemAcct.Caption = "";
            this.cboItemAcct.CaptionHeight = 17;
            this.cboItemAcct.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboItemAcct.ColumnCaptionHeight = 18;
            this.cboItemAcct.ColumnFooterHeight = 18;
            this.cboItemAcct.ContentHeight = 15;
            this.cboItemAcct.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboItemAcct.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboItemAcct.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboItemAcct.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboItemAcct.EditorHeight = 15;
            this.cboItemAcct.Images.Add(((System.Drawing.Image)(resources.GetObject("cboItemAcct.Images"))));
            this.cboItemAcct.ItemHeight = 15;
            this.cboItemAcct.Location = new System.Drawing.Point(124, 76);
            this.cboItemAcct.MatchEntryTimeout = ((long)(2000));
            this.cboItemAcct.MaxDropDownItems = ((short)(5));
            this.cboItemAcct.MaxLength = 32767;
            this.cboItemAcct.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboItemAcct.Name = "cboItemAcct";
            this.cboItemAcct.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboItemAcct.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboItemAcct.Size = new System.Drawing.Size(119, 21);
            this.cboItemAcct.TabIndex = 8;
            this.cboItemAcct.Tag = "";
            this.cboItemAcct.PropBag = resources.GetString("cboItemAcct.PropBag");
            // 
            // mskDT_Fr
            // 
            this.mskDT_Fr.AutoSize = false;
            this.mskDT_Fr.BackColor = System.Drawing.Color.White;
            this.mskDT_Fr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.mskDT_Fr.Calendar.DayNameLength = 1;
            this.mskDT_Fr.EmptyAsNull = true;
            this.mskDT_Fr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.mskDT_Fr.Location = new System.Drawing.Point(124, 49);
            this.mskDT_Fr.Name = "mskDT_Fr";
            this.mskDT_Fr.Size = new System.Drawing.Size(107, 21);
            this.mskDT_Fr.TabIndex = 4;
            this.mskDT_Fr.Tag = ";1;;";
            this.mskDT_Fr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.mskDT_Fr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(229, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 22);
            this.label1.TabIndex = 5;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mskDT_To
            // 
            this.mskDT_To.AutoSize = false;
            this.mskDT_To.BackColor = System.Drawing.Color.White;
            this.mskDT_To.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.mskDT_To.Calendar.DayNameLength = 1;
            this.mskDT_To.EmptyAsNull = true;
            this.mskDT_To.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.mskDT_To.Location = new System.Drawing.Point(254, 49);
            this.mskDT_To.Name = "mskDT_To";
            this.mskDT_To.Size = new System.Drawing.Size(107, 21);
            this.mskDT_To.TabIndex = 6;
            this.mskDT_To.Tag = ";1;;";
            this.mskDT_To.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.mskDT_To.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // INV118
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "INV118";
            this.Text = "수불집계표출력(내자/외자)";
            this.Activated += new System.EventHandler(this.INV118_Activated);
            this.Deactivate += new System.EventHandler(this.INV118_Deactivate);
            this.Load += new System.EventHandler(this.INV118_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboItemAcct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_Fr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mskDT_To)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1Button btnPreview;
        private C1.Win.C1List.C1Combo cboPlantCd;
        private C1.Win.C1Input.C1DateEdit mskDT_Fr;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit mskDT_To;
        private C1.Win.C1List.C1Combo cboItemAcct;

    }
}