namespace HA.HAA007
{
    partial class HAA007
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAA007));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpDate = new C1.Win.C1Input.C1DateEdit();
            this.c1Label9 = new C1.Win.C1Input.C1Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rdoNo = new System.Windows.Forms.RadioButton();
            this.rdoYes = new System.Windows.Forms.RadioButton();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.c1Label10 = new C1.Win.C1Input.C1Label();
            this.button2 = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label9)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Controls.Add(this.button2);
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(689, 231);
            this.GridCommGroupBox.TabIndex = 0;
            this.GridCommGroupBox.Controls.SetChildIndex(this.fpSpread1, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.button2, 0);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 112);
            this.GridCommPanel.Size = new System.Drawing.Size(689, 231);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Location = new System.Drawing.Point(2, 15);
            this.fpSpread1.Size = new System.Drawing.Size(684, 180);
            this.fpSpread1.TabIndex = 0;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(689, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(689, 48);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.c1Label9);
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.c1Label10);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(689, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
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
            this.dtpDate.Location = new System.Drawing.Point(104, 17);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(90, 21);
            this.dtpDate.TabIndex = 1;
            this.dtpDate.Tag = null;
            this.dtpDate.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpDate.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // c1Label9
            // 
            this.c1Label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label9.Location = new System.Drawing.Point(201, 16);
            this.c1Label9.Name = "c1Label9";
            this.c1Label9.Size = new System.Drawing.Size(96, 21);
            this.c1Label9.TabIndex = 2;
            this.c1Label9.Tag = null;
            this.c1Label9.Text = "반영여부";
            this.c1Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label9.TextDetached = true;
            this.c1Label9.Value = "";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.rdoNo);
            this.panel5.Controls.Add(this.rdoYes);
            this.panel5.Controls.Add(this.rdoAll);
            this.panel5.Location = new System.Drawing.Point(297, 16);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(223, 21);
            this.panel5.TabIndex = 3;
            // 
            // rdoNo
            // 
            this.rdoNo.Checked = true;
            this.rdoNo.Location = new System.Drawing.Point(121, 1);
            this.rdoNo.Name = "rdoNo";
            this.rdoNo.Size = new System.Drawing.Size(62, 18);
            this.rdoNo.TabIndex = 2;
            this.rdoNo.TabStop = true;
            this.rdoNo.Text = "미반영";
            this.rdoNo.UseVisualStyleBackColor = true;
            // 
            // rdoYes
            // 
            this.rdoYes.Location = new System.Drawing.Point(65, 2);
            this.rdoYes.Name = "rdoYes";
            this.rdoYes.Size = new System.Drawing.Size(50, 18);
            this.rdoYes.TabIndex = 1;
            this.rdoYes.Text = "반영";
            this.rdoYes.UseVisualStyleBackColor = true;
            // 
            // rdoAll
            // 
            this.rdoAll.Location = new System.Drawing.Point(10, 2);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(50, 18);
            this.rdoAll.TabIndex = 0;
            this.rdoAll.Text = "전체";
            this.rdoAll.UseVisualStyleBackColor = true;
            // 
            // c1Label10
            // 
            this.c1Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label10.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label10.Location = new System.Drawing.Point(8, 16);
            this.c1Label10.Name = "c1Label10";
            this.c1Label10.Size = new System.Drawing.Size(96, 21);
            this.c1Label10.TabIndex = 0;
            this.c1Label10.Tag = null;
            this.c1Label10.Text = "근태일자";
            this.c1Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label10.TextDetached = true;
            this.c1Label10.Value = "";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Location = new System.Drawing.Point(5, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 25);
            this.button2.TabIndex = 153;
            this.button2.Text = "근태방영";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // HAA007
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(689, 343);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HAA007";
            this.Text = "일근태확정";
            this.Load += new System.EventHandler(this.HAA007_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label9)).EndInit();
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label10;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton rdoYes;
        private System.Windows.Forms.RadioButton rdoAll;
        private C1.Win.C1Input.C1Label c1Label9;
        private System.Windows.Forms.RadioButton rdoNo;
        private C1.Win.C1Input.C1DateEdit dtpDate;
        private C1.Win.C1Input.C1Button button2;

    }
}