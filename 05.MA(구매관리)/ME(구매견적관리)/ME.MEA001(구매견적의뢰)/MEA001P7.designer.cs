namespace ME.MEA001
{
    partial class MEA001P7
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEA001P7));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtUserNm = new C1.Win.C1Input.C1TextBox();
            this.txtUserId = new C1.Win.C1Input.C1TextBox();
            this.c1Label10 = new C1.Win.C1Input.C1Label();
            this.c1Label12 = new C1.Win.C1Input.C1Label();
            this.butCancel = new C1.Win.C1Input.C1Button();
            this.btnOk = new C1.Win.C1Input.C1Button();
            this.dtpEstDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.dtpEstDtTo = new C1.Win.C1Input.C1DateEdit();
            this.btnUser = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtUserNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEstDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEstDtTo)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Controls.Add(this.btnOk);
            this.GridCommGroupBox.Controls.Add(this.butCancel);
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1042, 473);
            this.GridCommGroupBox.TabIndex = 0;
            this.GridCommGroupBox.Controls.SetChildIndex(this.fpSpread1, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.butCancel, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.btnOk, 0);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 109);
            this.GridCommPanel.Size = new System.Drawing.Size(1042, 473);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1036, 419);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1042, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1042, 45);
            this.panel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtUserNm);
            this.groupBox1.Controls.Add(this.btnUser);
            this.groupBox1.Controls.Add(this.txtUserId);
            this.groupBox1.Controls.Add(this.c1Label10);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Controls.Add(this.dtpEstDtTo);
            this.groupBox1.Controls.Add(this.dtpEstDtFr);
            this.groupBox1.Controls.Add(this.c1Label12);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1042, 45);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtUserNm
            // 
            this.txtUserNm.AutoSize = false;
            this.txtUserNm.BackColor = System.Drawing.Color.White;
            this.txtUserNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUserNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserNm.Enabled = false;
            this.txtUserNm.Location = new System.Drawing.Point(525, 15);
            this.txtUserNm.Name = "txtUserNm";
            this.txtUserNm.Size = new System.Drawing.Size(133, 21);
            this.txtUserNm.TabIndex = 111;
            this.txtUserNm.Tag = ";2;;";
            this.txtUserNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // txtUserId
            // 
            this.txtUserId.AutoSize = false;
            this.txtUserId.BackColor = System.Drawing.Color.White;
            this.txtUserId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtUserId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserId.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserId.Location = new System.Drawing.Point(426, 15);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Size = new System.Drawing.Size(75, 21);
            this.txtUserId.TabIndex = 109;
            this.txtUserId.Tag = null;
            this.txtUserId.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.txtUserId.TextChanged += new System.EventHandler(this.txtUserId_TextChanged);
            // 
            // c1Label10
            // 
            this.c1Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label10.Location = new System.Drawing.Point(332, 15);
            this.c1Label10.Name = "c1Label10";
            this.c1Label10.Size = new System.Drawing.Size(94, 21);
            this.c1Label10.TabIndex = 108;
            this.c1Label10.Tag = null;
            this.c1Label10.Text = "구매담당자";
            this.c1Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label10.TextDetached = true;
            this.c1Label10.Value = "";
            // 
            // c1Label12
            // 
            this.c1Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label12.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label12.Location = new System.Drawing.Point(8, 15);
            this.c1Label12.Name = "c1Label12";
            this.c1Label12.Size = new System.Drawing.Size(94, 21);
            this.c1Label12.TabIndex = 56;
            this.c1Label12.Tag = null;
            this.c1Label12.Text = "견적의뢰일자";
            this.c1Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label12.TextDetached = true;
            this.c1Label12.Value = "";
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCancel.BackgroundImage")));
            this.butCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancel.Location = new System.Drawing.Point(948, 442);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(88, 25);
            this.butCancel.TabIndex = 111;
            this.butCancel.Text = "취소";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOk.BackgroundImage")));
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.Location = new System.Drawing.Point(854, 442);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 25);
            this.btnOk.TabIndex = 112;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // dtpEstDtFr
            // 
            this.dtpEstDtFr.AutoSize = false;
            this.dtpEstDtFr.BackColor = System.Drawing.Color.White;
            this.dtpEstDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpEstDtFr.Calendar.DayNameLength = 1;
            this.dtpEstDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpEstDtFr.Location = new System.Drawing.Point(102, 15);
            this.dtpEstDtFr.Name = "dtpEstDtFr";
            this.dtpEstDtFr.Size = new System.Drawing.Size(100, 21);
            this.dtpEstDtFr.TabIndex = 57;
            this.dtpEstDtFr.Tag = "견적의뢰일자;1;;";
            this.dtpEstDtFr.Value = new System.DateTime(2013, 4, 9, 0, 0, 0, 0);
            this.dtpEstDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpEstDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label6
            // 
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label6.Location = new System.Drawing.Point(202, 15);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(24, 21);
            this.c1Label6.TabIndex = 59;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "~";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            // 
            // dtpEstDtTo
            // 
            this.dtpEstDtTo.AutoSize = false;
            this.dtpEstDtTo.BackColor = System.Drawing.Color.White;
            this.dtpEstDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpEstDtTo.Calendar.DayNameLength = 1;
            this.dtpEstDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpEstDtTo.Location = new System.Drawing.Point(226, 15);
            this.dtpEstDtTo.Name = "dtpEstDtTo";
            this.dtpEstDtTo.Size = new System.Drawing.Size(100, 21);
            this.dtpEstDtTo.TabIndex = 58;
            this.dtpEstDtTo.Tag = "견적의뢰일자;1;;";
            this.dtpEstDtTo.Value = new System.DateTime(2013, 4, 9, 0, 0, 0, 0);
            this.dtpEstDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpEstDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // btnUser
            // 
            this.btnUser.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUser.BackgroundImage")));
            this.btnUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnUser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUser.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUser.Location = new System.Drawing.Point(501, 15);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(24, 21);
            this.btnUser.TabIndex = 110;
            this.btnUser.Tag = ";;true;;";
            this.btnUser.UseVisualStyleBackColor = true;
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            // 
            // MEA001P7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 582);
            this.Controls.Add(this.panel1);
            this.Name = "MEA001P7";
            this.Text = "견적의뢰번호 팝업";
            this.Load += new System.EventHandler(this.MEA001P7_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtUserNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEstDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEstDtTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label12;
        private C1.Win.C1Input.C1TextBox txtUserNm;
        private C1.Win.C1Input.C1TextBox txtUserId;
        private C1.Win.C1Input.C1Label c1Label10;
        private C1.Win.C1Input.C1Button butCancel;
        private C1.Win.C1Input.C1Button btnOk;
        private C1.Win.C1Input.C1Button btnUser;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1DateEdit dtpEstDtTo;
        private C1.Win.C1Input.C1DateEdit dtpEstDtFr;
    }
}