﻿namespace HA.HAA006
{
    partial class HAA006
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAA006));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkNight = new C1.Win.C1Input.C1CheckBox();
            this.txtEmpNm = new C1.Win.C1Input.C1TextBox();
            this.btnEmpNo = new C1.Win.C1Input.C1Button();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.txtEmpNo = new C1.Win.C1Input.C1TextBox();
            this.txtInternalCd = new C1.Win.C1Input.C1TextBox();
            this.dtpDate = new C1.Win.C1Input.C1DateEdit();
            this.txtDeptNm = new C1.Win.C1Input.C1TextBox();
            this.btnDept = new C1.Win.C1Input.C1Button();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtDeptCd = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnEmpCreate = new C1.Win.C1Input.C1Button();
            this.btnDeptCreate = new C1.Win.C1Input.C1Button();
            this.btnAllCreate = new C1.Win.C1Input.C1Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtEmpNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmpNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInternalCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(892, 338);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Controls.Add(this.groupBox2);
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 136);
            this.GridCommPanel.Size = new System.Drawing.Size(892, 391);
            this.GridCommPanel.Controls.SetChildIndex(this.groupBox2, 0);
            this.GridCommPanel.Controls.SetChildIndex(this.GridCommGroupBox, 0);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(886, 318);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(892, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(892, 72);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.chkNight);
            this.groupBox1.Controls.Add(this.txtEmpNm);
            this.groupBox1.Controls.Add(this.btnEmpNo);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Controls.Add(this.txtEmpNo);
            this.groupBox1.Controls.Add(this.txtInternalCd);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.txtDeptNm);
            this.groupBox1.Controls.Add(this.btnDept);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtDeptCd);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(892, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // chkNight
            // 
            this.chkNight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkNight.Location = new System.Drawing.Point(438, 43);
            this.chkNight.Name = "chkNight";
            this.chkNight.Size = new System.Drawing.Size(157, 20);
            this.chkNight.TabIndex = 10;
            this.chkNight.Tag = "";
            this.chkNight.Text = "야간조 근태 계산";
            this.chkNight.Value = null;
            // 
            // txtEmpNm
            // 
            this.txtEmpNm.AutoSize = false;
            this.txtEmpNm.BackColor = System.Drawing.Color.White;
            this.txtEmpNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtEmpNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpNm.Location = new System.Drawing.Point(213, 42);
            this.txtEmpNm.Name = "txtEmpNm";
            this.txtEmpNm.Size = new System.Drawing.Size(198, 21);
            this.txtEmpNm.TabIndex = 9;
            this.txtEmpNm.Tag = ";2;;";
            // 
            // btnEmpNo
            // 
            this.btnEmpNo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEmpNo.BackgroundImage")));
            this.btnEmpNo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnEmpNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmpNo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEmpNo.Location = new System.Drawing.Point(189, 42);
            this.btnEmpNo.Name = "btnEmpNo";
            this.btnEmpNo.Size = new System.Drawing.Size(24, 21);
            this.btnEmpNo.TabIndex = 8;
            this.btnEmpNo.UseVisualStyleBackColor = true;
            this.btnEmpNo.Click += new System.EventHandler(this.btnEmpNo_Click);
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(10, 42);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(80, 21);
            this.c1Label3.TabIndex = 6;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "사원번호";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
            // 
            // txtEmpNo
            // 
            this.txtEmpNo.AutoSize = false;
            this.txtEmpNo.BackColor = System.Drawing.Color.White;
            this.txtEmpNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtEmpNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmpNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtEmpNo.Location = new System.Drawing.Point(89, 42);
            this.txtEmpNo.Name = "txtEmpNo";
            this.txtEmpNo.Size = new System.Drawing.Size(100, 21);
            this.txtEmpNo.TabIndex = 7;
            this.txtEmpNo.Tag = null;
            this.txtEmpNo.TextChanged += new System.EventHandler(this.txtEmpNo_TextChanged);
            // 
            // txtInternalCd
            // 
            this.txtInternalCd.AutoSize = false;
            this.txtInternalCd.BackColor = System.Drawing.Color.White;
            this.txtInternalCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtInternalCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInternalCd.Location = new System.Drawing.Point(601, 15);
            this.txtInternalCd.Name = "txtInternalCd";
            this.txtInternalCd.Size = new System.Drawing.Size(92, 21);
            this.txtInternalCd.TabIndex = 17;
            this.txtInternalCd.Tag = ";2;;";
            this.txtInternalCd.Visible = false;
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
            this.dtpDate.FormatType = C1.Win.C1Input.FormatTypeEnum.ShortDate;
            this.dtpDate.Location = new System.Drawing.Point(90, 15);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(99, 21);
            this.dtpDate.TabIndex = 1;
            this.dtpDate.Tag = null;
            this.dtpDate.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpDate.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // txtDeptNm
            // 
            this.txtDeptNm.AutoSize = false;
            this.txtDeptNm.BackColor = System.Drawing.Color.White;
            this.txtDeptNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptNm.Location = new System.Drawing.Point(435, 15);
            this.txtDeptNm.Name = "txtDeptNm";
            this.txtDeptNm.Size = new System.Drawing.Size(160, 21);
            this.txtDeptNm.TabIndex = 5;
            this.txtDeptNm.Tag = ";2;;";
            // 
            // btnDept
            // 
            this.btnDept.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDept.BackgroundImage")));
            this.btnDept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDept.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDept.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDept.Location = new System.Drawing.Point(411, 15);
            this.btnDept.Name = "btnDept";
            this.btnDept.Size = new System.Drawing.Size(24, 21);
            this.btnDept.TabIndex = 4;
            this.btnDept.UseVisualStyleBackColor = true;
            this.btnDept.Click += new System.EventHandler(this.btnDept_Click);
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(232, 15);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(80, 21);
            this.c1Label2.TabIndex = 2;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "부서코드";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtDeptCd
            // 
            this.txtDeptCd.AutoSize = false;
            this.txtDeptCd.BackColor = System.Drawing.Color.White;
            this.txtDeptCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDeptCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDeptCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDeptCd.Location = new System.Drawing.Point(311, 15);
            this.txtDeptCd.Name = "txtDeptCd";
            this.txtDeptCd.Size = new System.Drawing.Size(100, 21);
            this.txtDeptCd.TabIndex = 3;
            this.txtDeptCd.Tag = null;
            this.txtDeptCd.TextChanged += new System.EventHandler(this.txtDeptCd_TextChanged);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(10, 15);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(80, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "근태일자";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnEmpCreate);
            this.groupBox2.Controls.Add(this.btnDeptCreate);
            this.groupBox2.Controls.Add(this.btnAllCreate);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 338);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(892, 53);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "근태생성(작업일보)";
            // 
            // btnEmpCreate
            // 
            this.btnEmpCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEmpCreate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEmpCreate.BackgroundImage")));
            this.btnEmpCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmpCreate.Location = new System.Drawing.Point(308, 17);
            this.btnEmpCreate.Name = "btnEmpCreate";
            this.btnEmpCreate.Size = new System.Drawing.Size(144, 25);
            this.btnEmpCreate.TabIndex = 152;
            this.btnEmpCreate.Text = "사원별 생성";
            this.btnEmpCreate.UseVisualStyleBackColor = true;
            this.btnEmpCreate.Click += new System.EventHandler(this.btnEmpCreate_Click);
            // 
            // btnDeptCreate
            // 
            this.btnDeptCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeptCreate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDeptCreate.BackgroundImage")));
            this.btnDeptCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeptCreate.Location = new System.Drawing.Point(158, 17);
            this.btnDeptCreate.Name = "btnDeptCreate";
            this.btnDeptCreate.Size = new System.Drawing.Size(144, 25);
            this.btnDeptCreate.TabIndex = 151;
            this.btnDeptCreate.Text = "부서별 생성";
            this.btnDeptCreate.UseVisualStyleBackColor = true;
            this.btnDeptCreate.Click += new System.EventHandler(this.btnDeptCreate_Click);
            // 
            // btnAllCreate
            // 
            this.btnAllCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAllCreate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAllCreate.BackgroundImage")));
            this.btnAllCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAllCreate.Location = new System.Drawing.Point(8, 17);
            this.btnAllCreate.Name = "btnAllCreate";
            this.btnAllCreate.Size = new System.Drawing.Size(144, 25);
            this.btnAllCreate.TabIndex = 150;
            this.btnAllCreate.Text = "전체 생성";
            this.btnAllCreate.UseVisualStyleBackColor = true;
            this.btnAllCreate.Click += new System.EventHandler(this.btnAllCreate_Click);
            // 
            // HAA006
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(892, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HAA006";
            this.Text = "일근태등록(을)";
            this.Load += new System.EventHandler(this.HAA006_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtEmpNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmpNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInternalCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeptCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1DateEdit dtpDate;
        private C1.Win.C1Input.C1TextBox txtDeptNm;
        private C1.Win.C1Input.C1Button btnDept;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtDeptCd;
        private C1.Win.C1Input.C1TextBox txtInternalCd;
        private System.Windows.Forms.GroupBox groupBox2;
        private C1.Win.C1Input.C1CheckBox chkNight;
        private C1.Win.C1Input.C1TextBox txtEmpNm;
        private C1.Win.C1Input.C1Button btnEmpNo;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1TextBox txtEmpNo;
        private C1.Win.C1Input.C1Button btnEmpCreate;
        private C1.Win.C1Input.C1Button btnDeptCreate;
        private C1.Win.C1Input.C1Button btnAllCreate;

    }
}