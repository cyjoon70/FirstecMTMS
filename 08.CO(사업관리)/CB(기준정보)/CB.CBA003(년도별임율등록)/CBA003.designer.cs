namespace CB.CBA003
{
    partial class CBA003
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CBA003));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnConfirmOk = new C1.Win.C1Input.C1Button();
            this.btnConfirmCancel = new C1.Win.C1Input.C1Button();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.dtpYyyy = new C1.Win.C1Input.C1DateEdit();
            this.c1Report1 = new C1.C1Report.C1Report();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpYyyy)).BeginInit();
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
            this.groupBox1.Controls.Add(this.btnConfirmOk);
            this.groupBox1.Controls.Add(this.btnConfirmCancel);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.dtpYyyy);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(957, 64);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnConfirmOk
            // 
            this.btnConfirmOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfirmOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirmOk.BackgroundImage")));
            this.btnConfirmOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmOk.Location = new System.Drawing.Point(310, 24);
            this.btnConfirmOk.Name = "btnConfirmOk";
            this.btnConfirmOk.Size = new System.Drawing.Size(92, 25);
            this.btnConfirmOk.TabIndex = 2;
            this.btnConfirmOk.Text = "확정";
            this.btnConfirmOk.UseVisualStyleBackColor = true;
            this.btnConfirmOk.Click += new System.EventHandler(this.btnConfirmOk_Click);
            // 
            // btnConfirmCancel
            // 
            this.btnConfirmCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfirmCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConfirmCancel.BackgroundImage")));
            this.btnConfirmCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmCancel.Location = new System.Drawing.Point(410, 24);
            this.btnConfirmCancel.Name = "btnConfirmCancel";
            this.btnConfirmCancel.Size = new System.Drawing.Size(92, 25);
            this.btnConfirmCancel.TabIndex = 3;
            this.btnConfirmCancel.Text = "확정취소";
            this.btnConfirmCancel.UseVisualStyleBackColor = true;
            this.btnConfirmCancel.Click += new System.EventHandler(this.btnConfirmCancel_Click);
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(20, 27);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(80, 21);
            this.c1Label3.TabIndex = 0;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "년도";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // dtpYyyy
            // 
            this.dtpYyyy.AutoSize = false;
            this.dtpYyyy.BackColor = System.Drawing.Color.White;
            this.dtpYyyy.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpYyyy.Calendar.DayNameLength = 1;
            this.dtpYyyy.CustomFormat = "yyyy";
            this.dtpYyyy.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpYyyy.Location = new System.Drawing.Point(99, 27);
            this.dtpYyyy.Name = "dtpYyyy";
            this.dtpYyyy.Size = new System.Drawing.Size(90, 21);
            this.dtpYyyy.TabIndex = 1;
            this.dtpYyyy.Tag = null;
            this.dtpYyyy.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.UpDown;
            // 
            // c1Report1
            // 
            this.c1Report1.ReportDefinition = resources.GetString("c1Report1.ReportDefinition");
            this.c1Report1.ReportName = "Error Log Info";
            // 
            // CBA003
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(957, 482);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "CBA003";
            this.Text = "년도별임율등록";
            this.Load += new System.EventHandler(this.CBA003_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpYyyy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Report1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1DateEdit dtpYyyy;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.C1Report.C1Report c1Report1;
        private C1.Win.C1Input.C1Button btnConfirmOk;
        private C1.Win.C1Input.C1Button btnConfirmCancel;

    }
}