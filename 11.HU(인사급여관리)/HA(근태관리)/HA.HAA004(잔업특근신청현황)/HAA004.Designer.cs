namespace HA.HAA004
{
    partial class HAA004
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAA004));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new C1.Win.C1Input.C1Button();
            this.dtpDate = new C1.Win.C1Input.C1DateEdit();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.GridCommPanel2.SuspendLayout();
            this.GridCommGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
            this.GridCommPanel1.SuspendLayout();
            this.GridCommGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView1)).BeginInit();
            this.panel4.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommPanel2
            // 
            this.GridCommPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.GridCommPanel2.Location = new System.Drawing.Point(0, 48);
            this.GridCommPanel2.Size = new System.Drawing.Size(922, 194);
            // 
            // GridCommGroupBox2
            // 
            this.GridCommGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox2.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox2.Size = new System.Drawing.Size(922, 194);
            this.GridCommGroupBox2.TabIndex = 0;
            this.GridCommGroupBox2.Text = "[집계현황]";
            // 
            // fpSpread2
            // 
            this.fpSpread2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread2.Location = new System.Drawing.Point(3, 17);
            this.fpSpread2.Size = new System.Drawing.Size(916, 174);
            this.fpSpread2.TabIndex = 0;
            this.fpSpread2.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread2_CellClick);
            // 
            // fpSpread2_Sheet1
            // 
            this.fpSpread2_Sheet1.SheetName = "Sheet1";
            // 
            // GridCommPanel1
            // 
            this.GridCommPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel1.Location = new System.Drawing.Point(0, 242);
            this.GridCommPanel1.Size = new System.Drawing.Size(922, 268);
            // 
            // GridCommGroupBox1
            // 
            this.GridCommGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox1.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox1.Size = new System.Drawing.Size(922, 268);
            this.GridCommGroupBox1.TabIndex = 0;
            this.GridCommGroupBox1.Text = "[부서별세부현황]";
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(916, 248);
            this.fpSpread1.TabIndex = 0;
            // 
            // sheetView1
            // 
            this.sheetView1.SheetName = "Sheet1";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Controls.Add(this.splitter1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 64);
            this.panel4.Size = new System.Drawing.Size(922, 520);
            this.panel4.Controls.SetChildIndex(this.splitter1, 0);
            this.panel4.Controls.SetChildIndex(this.panel1, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel2, 0);
            this.panel4.Controls.SetChildIndex(this.GridCommPanel1, 0);
            // 
            // panButton1
            // 
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(922, 48);
            this.panel1.TabIndex = 34;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(922, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Location = new System.Drawing.Point(317, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 25);
            this.button1.TabIndex = 27;
            this.button1.Text = "잔업식권집계표";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dtpDate
            // 
            this.dtpDate.AutoSize = false;
            this.dtpDate.BackColor = System.Drawing.Color.White;
            this.dtpDate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.dtpDate.Calendar.DayNameLength = 1;
            this.dtpDate.CustomFormat = "yyyy-MM-dd";
            this.dtpDate.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.dtpDate.Location = new System.Drawing.Point(91, 18);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(89, 21);
            this.dtpDate.TabIndex = 53;
            this.dtpDate.Tag = null;
            this.dtpDate.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpDate.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(8, 17);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(83, 21);
            this.c1Label2.TabIndex = 52;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "근태일자";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 510);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(922, 10);
            this.splitter1.TabIndex = 37;
            this.splitter1.TabStop = false;
            // 
            // HAA004
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 584);
            this.Name = "HAA004";
            this.Text = "잔업특근신청현황";
            this.Load += new System.EventHandler(this.HAA004_Load);
            this.GridCommPanel2.ResumeLayout(false);
            this.GridCommGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).EndInit();
            this.GridCommPanel1.ResumeLayout(false);
            this.GridCommGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sheetView1)).EndInit();
            this.panel4.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1DateEdit dtpDate;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1Button button1;
    }
}