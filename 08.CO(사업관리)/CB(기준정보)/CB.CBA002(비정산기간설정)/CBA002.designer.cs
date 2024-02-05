namespace CB.CBA002
{
    partial class CBA002
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CBA002));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtProjectNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label10 = new C1.Win.C1Input.C1Label();
            this.c1Label20 = new C1.Win.C1Input.C1Label();
            this.dtpSoDtTo = new C1.Win.C1Input.C1DateEdit();
            this.dtpSoDtFr = new C1.Win.C1Input.C1DateEdit();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.c1Report1 = new C1.C1Report.C1Report();
            this.btnProjectNo = new C1.Win.C1Input.C1Button();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtFr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Report1)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 64);
            this.GridCommGroupBox.Size = new System.Drawing.Size(957, 354);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Controls.Add(this.groupBox1);
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 64);
            this.GridCommPanel.Size = new System.Drawing.Size(957, 418);
            this.GridCommPanel.TabIndex = 0;
            this.GridCommPanel.Controls.SetChildIndex(this.groupBox1, 0);
            this.GridCommPanel.Controls.SetChildIndex(this.GridCommGroupBox, 0);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(951, 334);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(957, 64);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.btnProjectNo);
            this.groupBox1.Controls.Add(this.txtProjectNo);
            this.groupBox1.Controls.Add(this.c1Label10);
            this.groupBox1.Controls.Add(this.c1Label20);
            this.groupBox1.Controls.Add(this.dtpSoDtTo);
            this.groupBox1.Controls.Add(this.dtpSoDtFr);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(957, 64);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // txtProjectNo
            // 
            this.txtProjectNo.AutoSize = false;
            this.txtProjectNo.BackColor = System.Drawing.Color.White;
            this.txtProjectNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtProjectNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProjectNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProjectNo.Location = new System.Drawing.Point(105, 27);
            this.txtProjectNo.Name = "txtProjectNo";
            this.txtProjectNo.Size = new System.Drawing.Size(90, 21);
            this.txtProjectNo.TabIndex = 1;
            this.txtProjectNo.Tag = null;
            // 
            // c1Label10
            // 
            this.c1Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label10.Location = new System.Drawing.Point(20, 27);
            this.c1Label10.Name = "c1Label10";
            this.c1Label10.Size = new System.Drawing.Size(86, 21);
            this.c1Label10.TabIndex = 0;
            this.c1Label10.Tag = null;
            this.c1Label10.Text = "프로젝트번호";
            this.c1Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label10.TextDetached = true;
            this.c1Label10.Value = "";
            // 
            // c1Label20
            // 
            this.c1Label20.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label20.Location = new System.Drawing.Point(402, 27);
            this.c1Label20.Name = "c1Label20";
            this.c1Label20.Size = new System.Drawing.Size(24, 21);
            this.c1Label20.TabIndex = 5;
            this.c1Label20.Tag = null;
            this.c1Label20.Text = "~";
            this.c1Label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label20.TextDetached = true;
            // 
            // dtpSoDtTo
            // 
            this.dtpSoDtTo.AutoSize = false;
            this.dtpSoDtTo.BackColor = System.Drawing.Color.White;
            this.dtpSoDtTo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSoDtTo.Calendar.DayNameLength = 1;
            this.dtpSoDtTo.CustomFormat = "yyyy-MM-dd";
            this.dtpSoDtTo.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpSoDtTo.Location = new System.Drawing.Point(426, 27);
            this.dtpSoDtTo.Name = "dtpSoDtTo";
            this.dtpSoDtTo.Size = new System.Drawing.Size(90, 21);
            this.dtpSoDtTo.TabIndex = 6;
            this.dtpSoDtTo.Tag = null;
            this.dtpSoDtTo.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSoDtTo.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // dtpSoDtFr
            // 
            this.dtpSoDtFr.AutoSize = false;
            this.dtpSoDtFr.BackColor = System.Drawing.Color.White;
            this.dtpSoDtFr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpSoDtFr.Calendar.DayNameLength = 1;
            this.dtpSoDtFr.CustomFormat = "yyyy-MM-dd";
            this.dtpSoDtFr.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpSoDtFr.Location = new System.Drawing.Point(312, 27);
            this.dtpSoDtFr.Name = "dtpSoDtFr";
            this.dtpSoDtFr.Size = new System.Drawing.Size(90, 21);
            this.dtpSoDtFr.TabIndex = 4;
            this.dtpSoDtFr.Tag = null;
            this.dtpSoDtFr.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpSoDtFr.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(232, 27);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(80, 21);
            this.c1Label3.TabIndex = 3;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "수주일자";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // c1Report1
            // 
            this.c1Report1.ReportDefinition = resources.GetString("c1Report1.ReportDefinition");
            this.c1Report1.ReportName = "Error Log Info";
            // 
            // btnProjectNo
            // 
            this.btnProjectNo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnProjectNo.BackgroundImage")));
            this.btnProjectNo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnProjectNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProjectNo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnProjectNo.Location = new System.Drawing.Point(194, 27);
            this.btnProjectNo.Name = "btnProjectNo";
            this.btnProjectNo.Size = new System.Drawing.Size(24, 21);
            this.btnProjectNo.TabIndex = 25;
            this.btnProjectNo.Tag = "";
            this.btnProjectNo.UseVisualStyleBackColor = true;
            this.btnProjectNo.Click += new System.EventHandler(this.btnProjectNo_Click);
            // 
            // CBA002
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(957, 482);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "CBA002";
            this.Text = "비정산기간설정";
            this.Load += new System.EventHandler(this.CBA002_Load);
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
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpSoDtFr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Report1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.C1Report.C1Report c1Report1;
        private C1.Win.C1Input.C1TextBox txtProjectNo;
        private C1.Win.C1Input.C1Label c1Label10;
        private C1.Win.C1Input.C1Label c1Label20;
        private C1.Win.C1Input.C1DateEdit dtpSoDtTo;
        private C1.Win.C1Input.C1DateEdit dtpSoDtFr;
        private C1.Win.C1Input.C1Button btnProjectNo;

    }
}