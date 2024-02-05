namespace AR.ARA001
{
    partial class ARA001
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ARA001));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCloseStatus = new C1.Win.C1Input.C1TextBox();
            this.btnAdd = new C1.Win.C1Input.C1Button();
            this.dtpCloseMonth = new C1.Win.C1Input.C1DateEdit();
            this.cboPlantCd = new C1.Win.C1List.C1Combo();
            this.btnWorkClose = new C1.Win.C1Input.C1Button();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.제조오더번호 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtCloseStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpCloseMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.제조오더번호)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(899, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(899, 442);
            this.panel1.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCloseStatus);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.dtpCloseMonth);
            this.groupBox1.Controls.Add(this.cboPlantCd);
            this.groupBox1.Controls.Add(this.btnWorkClose);
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.제조오더번호);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 442);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtCloseStatus
            // 
            this.txtCloseStatus.AutoSize = false;
            this.txtCloseStatus.BackColor = System.Drawing.Color.White;
            this.txtCloseStatus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtCloseStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCloseStatus.Location = new System.Drawing.Point(212, 19);
            this.txtCloseStatus.Name = "txtCloseStatus";
            this.txtCloseStatus.Size = new System.Drawing.Size(231, 21);
            this.txtCloseStatus.TabIndex = 29;
            this.txtCloseStatus.Tag = ";2;;";
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAdd.BackgroundImage")));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Location = new System.Drawing.Point(12, 87);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(194, 25);
            this.btnAdd.TabIndex = 26;
            this.btnAdd.Text = "원가적상";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dtpCloseMonth
            // 
            this.dtpCloseMonth.AutoSize = false;
            this.dtpCloseMonth.BackColor = System.Drawing.Color.White;
            this.dtpCloseMonth.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpCloseMonth.Calendar.DayNameLength = 1;
            this.dtpCloseMonth.CustomFormat = "yyyy-MM";
            this.dtpCloseMonth.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpCloseMonth.Location = new System.Drawing.Point(116, 19);
            this.dtpCloseMonth.Name = "dtpCloseMonth";
            this.dtpCloseMonth.Size = new System.Drawing.Size(90, 21);
            this.dtpCloseMonth.TabIndex = 25;
            this.dtpCloseMonth.Tag = ";2;;";
            this.dtpCloseMonth.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpCloseMonth.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // cboPlantCd
            // 
            this.cboPlantCd.AddItemSeparator = ';';
            this.cboPlantCd.AutoSize = false;
            this.cboPlantCd.Caption = "";
            this.cboPlantCd.CaptionHeight = 17;
            this.cboPlantCd.CaptionVisible = false;
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
            this.cboPlantCd.Location = new System.Drawing.Point(628, 17);
            this.cboPlantCd.MatchEntryTimeout = ((long)(2000));
            this.cboPlantCd.MaxDropDownItems = ((short)(5));
            this.cboPlantCd.MaxLength = 32767;
            this.cboPlantCd.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboPlantCd.Name = "cboPlantCd";
            this.cboPlantCd.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboPlantCd.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboPlantCd.Size = new System.Drawing.Size(138, 21);
            this.cboPlantCd.TabIndex = 24;
            this.cboPlantCd.Tag = "";
            this.cboPlantCd.Visible = false;
            this.cboPlantCd.SelectedValueChanged += new System.EventHandler(this.cboPlantCd_SelectedIndexChanged);
            this.cboPlantCd.PropBag = resources.GetString("cboPlantCd.PropBag");
            // 
            // btnWorkClose
            // 
            this.btnWorkClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnWorkClose.BackgroundImage")));
            this.btnWorkClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWorkClose.Location = new System.Drawing.Point(12, 56);
            this.btnWorkClose.Name = "btnWorkClose";
            this.btnWorkClose.Size = new System.Drawing.Size(194, 25);
            this.btnWorkClose.TabIndex = 22;
            this.btnWorkClose.Text = "제조오더마감";
            this.btnWorkClose.UseVisualStyleBackColor = true;
            this.btnWorkClose.Click += new System.EventHandler(this.btnWorkClose_Click);
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(12, 19);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(104, 21);
            this.c1Label4.TabIndex = 2;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "작업대상년월";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // 제조오더번호
            // 
            this.제조오더번호.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.제조오더번호.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.제조오더번호.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.제조오더번호.Location = new System.Drawing.Point(524, 17);
            this.제조오더번호.Name = "제조오더번호";
            this.제조오더번호.Size = new System.Drawing.Size(104, 21);
            this.제조오더번호.TabIndex = 0;
            this.제조오더번호.Tag = null;
            this.제조오더번호.Text = "공장";
            this.제조오더번호.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.제조오더번호.TextDetached = true;
            this.제조오더번호.Value = "";
            this.제조오더번호.Visible = false;
            // 
            // ARA001
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 506);
            this.Controls.Add(this.panel1);
            this.Name = "ARA001";
            this.Text = "실제원가마감";
            this.Load += new System.EventHandler(this.ARA001_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtCloseStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpCloseMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPlantCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.제조오더번호)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label4;
        private C1.Win.C1Input.C1Button btnWorkClose;
        private C1.Win.C1Input.C1DateEdit dtpCloseMonth;
        private C1.Win.C1Input.C1Button btnAdd;
        private C1.Win.C1List.C1Combo cboPlantCd;
        private C1.Win.C1Input.C1Label 제조오더번호;
        private C1.Win.C1Input.C1TextBox txtCloseStatus;
    }
}