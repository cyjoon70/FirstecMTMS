namespace AH.ACH016
{
    partial class ACH016
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACH016));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSlipDtTo = new C1.Win.C1Input.C1DateEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpSlipDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.cboBizAreaCdTo = new C1.Win.C1List.C1Combo();
            this.label4 = new System.Windows.Forms.Label();
            this.cboBizAreaCdFrom = new C1.Win.C1List.C1Combo();
            this.c1Label7 = new C1.Win.C1Input.C1Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnSlipView = new C1.Win.C1Input.C1Button();
            this.panel2 = new System.Windows.Forms.Panel();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1222, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1222, 591);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.dtpSlipDtTo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpSlipDtFr);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.cboBizAreaCdTo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboBizAreaCdFrom);
            this.groupBox1.Controls.Add(this.c1Label7);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1222, 591);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // dtpSlipDtTo
            // 
            this.dtpSlipDtTo.AutoSize = false;
            this.dtpSlipDtTo.BackColor = System.Drawing.Color.White;
            this.dtpSlipDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipDtTo.Calendar.DayNameLength = 1;
            this.dtpSlipDtTo.EmptyAsNull = true;
            this.dtpSlipDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSlipDtTo.Location = new System.Drawing.Point(253, 41);
            this.dtpSlipDtTo.Name = "dtpSlipDtTo";
            this.dtpSlipDtTo.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtTo.TabIndex = 7;
            this.dtpSlipDtTo.Tag = "취득일;1;;";
            this.dtpSlipDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(228, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 21);
            this.label1.TabIndex = 6;
            this.label1.Text = "~";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtpSlipDtFr
            // 
            this.dtpSlipDtFr.AutoSize = false;
            this.dtpSlipDtFr.BackColor = System.Drawing.Color.White;
            this.dtpSlipDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSlipDtFr.Calendar.DayNameLength = 1;
            this.dtpSlipDtFr.EmptyAsNull = true;
            this.dtpSlipDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpSlipDtFr.Location = new System.Drawing.Point(105, 41);
            this.dtpSlipDtFr.Name = "dtpSlipDtFr";
            this.dtpSlipDtFr.Size = new System.Drawing.Size(125, 21);
            this.dtpSlipDtFr.TabIndex = 5;
            this.dtpSlipDtFr.Tag = "취득일;1;;";
            this.dtpSlipDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSlipDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(12, 41);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 4;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "취득일";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // cboBizAreaCdTo
            // 
            this.cboBizAreaCdTo.AddItemSeparator = ';';
            this.cboBizAreaCdTo.AutoSize = false;
            this.cboBizAreaCdTo.Caption = "";
            this.cboBizAreaCdTo.CaptionHeight = 17;
            this.cboBizAreaCdTo.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBizAreaCdTo.ColumnCaptionHeight = 18;
            this.cboBizAreaCdTo.ColumnFooterHeight = 18;
            this.cboBizAreaCdTo.ContentHeight = 15;
            this.cboBizAreaCdTo.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBizAreaCdTo.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBizAreaCdTo.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBizAreaCdTo.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBizAreaCdTo.EditorHeight = 15;
            this.cboBizAreaCdTo.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBizAreaCdTo.Images"))));
            this.cboBizAreaCdTo.ItemHeight = 15;
            this.cboBizAreaCdTo.Location = new System.Drawing.Point(280, 17);
            this.cboBizAreaCdTo.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCdTo.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCdTo.MaxLength = 32767;
            this.cboBizAreaCdTo.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCdTo.Name = "cboBizAreaCdTo";
            this.cboBizAreaCdTo.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCdTo.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCdTo.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCdTo.TabIndex = 3;
            this.cboBizAreaCdTo.Tag = ";;;";
            this.cboBizAreaCdTo.PropBag = resources.GetString("cboBizAreaCdTo.PropBag");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "~";
            // 
            // cboBizAreaCdFrom
            // 
            this.cboBizAreaCdFrom.AddItemSeparator = ';';
            this.cboBizAreaCdFrom.AutoSize = false;
            this.cboBizAreaCdFrom.Caption = "";
            this.cboBizAreaCdFrom.CaptionHeight = 17;
            this.cboBizAreaCdFrom.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboBizAreaCdFrom.ColumnCaptionHeight = 18;
            this.cboBizAreaCdFrom.ColumnFooterHeight = 18;
            this.cboBizAreaCdFrom.ContentHeight = 15;
            this.cboBizAreaCdFrom.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboBizAreaCdFrom.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboBizAreaCdFrom.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboBizAreaCdFrom.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboBizAreaCdFrom.EditorHeight = 15;
            this.cboBizAreaCdFrom.Images.Add(((System.Drawing.Image)(resources.GetObject("cboBizAreaCdFrom.Images"))));
            this.cboBizAreaCdFrom.ItemHeight = 15;
            this.cboBizAreaCdFrom.Location = new System.Drawing.Point(106, 17);
            this.cboBizAreaCdFrom.MatchEntryTimeout = ((long)(2000));
            this.cboBizAreaCdFrom.MaxDropDownItems = ((short)(5));
            this.cboBizAreaCdFrom.MaxLength = 32767;
            this.cboBizAreaCdFrom.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboBizAreaCdFrom.Name = "cboBizAreaCdFrom";
            this.cboBizAreaCdFrom.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboBizAreaCdFrom.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboBizAreaCdFrom.Size = new System.Drawing.Size(148, 21);
            this.cboBizAreaCdFrom.TabIndex = 1;
            this.cboBizAreaCdFrom.Tag = ";;;";
            this.cboBizAreaCdFrom.PropBag = resources.GetString("cboBizAreaCdFrom.PropBag");
            // 
            // c1Label7
            // 
            this.c1Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label7.Location = new System.Drawing.Point(12, 17);
            this.c1Label7.Name = "c1Label7";
            this.c1Label7.Size = new System.Drawing.Size(94, 21);
            this.c1Label7.TabIndex = 0;
            this.c1Label7.Tag = null;
            this.c1Label7.Text = "사업장";
            this.c1Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label7.TextDetached = true;
            this.c1Label7.Value = "";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 244);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1222, 12);
            this.splitter1.TabIndex = 35;
            this.splitter1.TabStop = false;
            // 
            // btnSlipView
            // 
            this.btnSlipView.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSlipView.BackgroundImage")));
            this.btnSlipView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSlipView.Location = new System.Drawing.Point(12, 6);
            this.btnSlipView.Name = "btnSlipView";
            this.btnSlipView.Size = new System.Drawing.Size(86, 25);
            this.btnSlipView.TabIndex = 21;
            this.btnSlipView.Text = "전표조회";
            this.btnSlipView.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSlipView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 452);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1222, 40);
            this.panel2.TabIndex = 36;
            // 
            // ACH016
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1222, 655);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACH016";
            this.Text = "고정자산취득CheckList";
            this.Load += new System.EventHandler(this.ACH016_Load);
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
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSlipDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBizAreaCdFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label7)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1List.C1Combo cboBizAreaCdTo;
        private System.Windows.Forms.Label label4;
        private C1.Win.C1List.C1Combo cboBizAreaCdFrom;
        private C1.Win.C1Input.C1Label c1Label7;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtTo;
        private System.Windows.Forms.Label label1;
        private C1.Win.C1Input.C1DateEdit dtpSlipDtFr;
        private C1.Win.C1Input.C1Label c1Label3;
    }
}