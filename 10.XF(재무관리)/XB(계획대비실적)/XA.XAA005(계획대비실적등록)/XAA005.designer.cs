namespace XA.XAA005
{
    partial class XAA005
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XAA005));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.c1Report1 = new C1.C1Report.C1Report();
            this.btnConfirmOk = new C1.Win.C1Input.C1Button();
            this.dtpYearMon = new C1.Win.C1Input.C1DateEdit();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Report1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpYearMon)).BeginInit();
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
            this.groupBox1.Controls.Add(this.dtpYearMon);
            this.groupBox1.Controls.Add(this.btnConfirmOk);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(957, 64);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // c1Label1
            // 
            this.c1Label1.BorderColor = System.Drawing.Color.Empty;
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label1.Location = new System.Drawing.Point(214, 26);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(94, 21);
            this.c1Label1.TabIndex = 257;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "(단위 : 천원)";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(20, 26);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(80, 21);
            this.c1Label3.TabIndex = 253;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "년월";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // c1Report1
            // 
            this.c1Report1.ReportDefinition = resources.GetString("c1Report1.ReportDefinition");
            this.c1Report1.ReportName = "Error Log Info";
            // 
            // btnConfirmOk
            // 
            this.btnConfirmOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirmOk.BackgroundImage")));
            this.btnConfirmOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmOk.Location = new System.Drawing.Point(795, 24);
            this.btnConfirmOk.Name = "btnConfirmOk";
            this.btnConfirmOk.Size = new System.Drawing.Size(92, 25);
            this.btnConfirmOk.TabIndex = 258;
            this.btnConfirmOk.Text = "분석";
            this.btnConfirmOk.UseVisualStyleBackColor = true;
            this.btnConfirmOk.Click += new System.EventHandler(this.btnConfirmOk_Click);
            // 
            // dtpYearMon
            // 
            this.dtpYearMon.AutoSize = false;
            this.dtpYearMon.BackColor = System.Drawing.Color.White;
            this.dtpYearMon.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpYearMon.Calendar.DayNameLength = 1;
            this.dtpYearMon.CustomFormat = "yyyy-MM";
            this.dtpYearMon.EmptyAsNull = true;
            this.dtpYearMon.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpYearMon.Location = new System.Drawing.Point(100, 26);
            this.dtpYearMon.Name = "dtpYearMon";
            this.dtpYearMon.Size = new System.Drawing.Size(107, 21);
            this.dtpYearMon.TabIndex = 259;
            this.dtpYearMon.Tag = null;
            this.dtpYearMon.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpYearMon.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // XAA005
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(957, 482);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "XAA005";
            this.Text = "계획대비실적등록";
            this.Load += new System.EventHandler(this.XAA005_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Report1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpYearMon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.C1Report.C1Report c1Report1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1DateEdit dtpYearMon;
        private C1.Win.C1Input.C1Button btnConfirmOk;

    }
}