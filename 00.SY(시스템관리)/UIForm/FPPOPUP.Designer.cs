namespace UIForm
{
    partial class FPPOPUP
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPPOPUP));
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnClose = new C1.Win.C1Input.C1Button();
			this.button1 = new C1.Win.C1Input.C1Button();
			this.txtCodeName = new C1.Win.C1Input.C1TextBox();
			this.txtCode = new C1.Win.C1Input.C1TextBox();
			this.lblCodeName = new C1.Win.C1Input.C1Label();
			this.lblCode = new C1.Win.C1Input.C1Label();
			this.lblEtc4 = new System.Windows.Forms.Label();
			this.txtEtc4 = new System.Windows.Forms.TextBox();
			this.lblEtc3 = new System.Windows.Forms.Label();
			this.txtEtc3 = new System.Windows.Forms.TextBox();
			this.lblEtc2 = new System.Windows.Forms.Label();
			this.txtEtc2 = new System.Windows.Forms.TextBox();
			this.lblEtc1 = new System.Windows.Forms.Label();
			this.txtEtc1 = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
			this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtCodeName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblCodeName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lblCode)).BeginInit();
			this.panel2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(450, 82);
			this.panel1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.btnClose);
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.txtCodeName);
			this.groupBox1.Controls.Add(this.txtCode);
			this.groupBox1.Controls.Add(this.lblCodeName);
			this.groupBox1.Controls.Add(this.lblCode);
			this.groupBox1.Controls.Add(this.lblEtc4);
			this.groupBox1.Controls.Add(this.txtEtc4);
			this.groupBox1.Controls.Add(this.lblEtc3);
			this.groupBox1.Controls.Add(this.txtEtc3);
			this.groupBox1.Controls.Add(this.lblEtc2);
			this.groupBox1.Controls.Add(this.txtEtc2);
			this.groupBox1.Controls.Add(this.lblEtc1);
			this.groupBox1.Controls.Add(this.txtEtc1);
			this.groupBox1.Location = new System.Drawing.Point(8, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(434, 73);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
			this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnClose.Location = new System.Drawing.Point(360, 14);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(64, 26);
			this.btnClose.TabIndex = 115;
			this.btnClose.Text = "닫 기";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
			this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.button1.Location = new System.Drawing.Point(292, 14);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 26);
			this.button1.TabIndex = 114;
			this.button1.Text = "조 회";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtCodeName
			// 
			this.txtCodeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCodeName.AutoSize = false;
			this.txtCodeName.BackColor = System.Drawing.Color.White;
			this.txtCodeName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtCodeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtCodeName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtCodeName.ImeMode = System.Windows.Forms.ImeMode.Hangul;
			this.txtCodeName.Location = new System.Drawing.Point(143, 44);
			this.txtCodeName.Name = "txtCodeName";
			this.txtCodeName.Size = new System.Drawing.Size(283, 21);
			this.txtCodeName.TabIndex = 2;
			this.txtCodeName.Tag = null;
			this.txtCodeName.TextDetached = true;
			this.txtCodeName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// txtCode
			// 
			this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCode.AutoSize = false;
			this.txtCode.BackColor = System.Drawing.Color.White;
			this.txtCode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtCode.ImeMode = System.Windows.Forms.ImeMode.Alpha;
			this.txtCode.Location = new System.Drawing.Point(143, 16);
			this.txtCode.Name = "txtCode";
			this.txtCode.Size = new System.Drawing.Size(137, 21);
			this.txtCode.TabIndex = 1;
			this.txtCode.Tag = null;
			this.txtCode.TextDetached = true;
			this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// lblCodeName
			// 
			this.lblCodeName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.lblCodeName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.lblCodeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblCodeName.Location = new System.Drawing.Point(8, 44);
			this.lblCodeName.Name = "lblCodeName";
			this.lblCodeName.Size = new System.Drawing.Size(136, 21);
			this.lblCodeName.TabIndex = 113;
			this.lblCodeName.Tag = null;
			this.lblCodeName.Text = "코 드 명";
			this.lblCodeName.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.lblCodeName.TextDetached = true;
			// 
			// lblCode
			// 
			this.lblCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.lblCode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.lblCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblCode.Location = new System.Drawing.Point(8, 16);
			this.lblCode.Name = "lblCode";
			this.lblCode.Size = new System.Drawing.Size(136, 21);
			this.lblCode.TabIndex = 112;
			this.lblCode.Tag = null;
			this.lblCode.Text = "코 드";
			this.lblCode.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.lblCode.TextDetached = true;
			// 
			// lblEtc4
			// 
			this.lblEtc4.BackColor = System.Drawing.Color.Beige;
			this.lblEtc4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblEtc4.Location = new System.Drawing.Point(8, 155);
			this.lblEtc4.Name = "lblEtc4";
			this.lblEtc4.Size = new System.Drawing.Size(136, 24);
			this.lblEtc4.TabIndex = 74;
			this.lblEtc4.Text = "코 드 명";
			this.lblEtc4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblEtc4.Visible = false;
			// 
			// txtEtc4
			// 
			this.txtEtc4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEtc4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEtc4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtEtc4.ImeMode = System.Windows.Forms.ImeMode.Hangul;
			this.txtEtc4.Location = new System.Drawing.Point(143, 155);
			this.txtEtc4.Name = "txtEtc4";
			this.txtEtc4.Size = new System.Drawing.Size(283, 21);
			this.txtEtc4.TabIndex = 51;
			this.txtEtc4.Visible = false;
			this.txtEtc4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// lblEtc3
			// 
			this.lblEtc3.BackColor = System.Drawing.Color.Beige;
			this.lblEtc3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblEtc3.Location = new System.Drawing.Point(8, 128);
			this.lblEtc3.Name = "lblEtc3";
			this.lblEtc3.Size = new System.Drawing.Size(136, 24);
			this.lblEtc3.TabIndex = 72;
			this.lblEtc3.Text = "코 드 명";
			this.lblEtc3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblEtc3.Visible = false;
			// 
			// txtEtc3
			// 
			this.txtEtc3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEtc3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEtc3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtEtc3.ImeMode = System.Windows.Forms.ImeMode.Hangul;
			this.txtEtc3.Location = new System.Drawing.Point(143, 128);
			this.txtEtc3.Name = "txtEtc3";
			this.txtEtc3.Size = new System.Drawing.Size(283, 21);
			this.txtEtc3.TabIndex = 41;
			this.txtEtc3.Visible = false;
			this.txtEtc3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// lblEtc2
			// 
			this.lblEtc2.BackColor = System.Drawing.Color.Beige;
			this.lblEtc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblEtc2.Location = new System.Drawing.Point(8, 101);
			this.lblEtc2.Name = "lblEtc2";
			this.lblEtc2.Size = new System.Drawing.Size(136, 24);
			this.lblEtc2.TabIndex = 70;
			this.lblEtc2.Text = "코 드 명";
			this.lblEtc2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblEtc2.Visible = false;
			// 
			// txtEtc2
			// 
			this.txtEtc2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEtc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEtc2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtEtc2.ImeMode = System.Windows.Forms.ImeMode.Hangul;
			this.txtEtc2.Location = new System.Drawing.Point(143, 101);
			this.txtEtc2.Name = "txtEtc2";
			this.txtEtc2.Size = new System.Drawing.Size(283, 21);
			this.txtEtc2.TabIndex = 31;
			this.txtEtc2.Visible = false;
			this.txtEtc2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// lblEtc1
			// 
			this.lblEtc1.BackColor = System.Drawing.Color.Beige;
			this.lblEtc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblEtc1.Location = new System.Drawing.Point(8, 73);
			this.lblEtc1.Name = "lblEtc1";
			this.lblEtc1.Size = new System.Drawing.Size(136, 24);
			this.lblEtc1.TabIndex = 68;
			this.lblEtc1.Text = "코 드 명";
			this.lblEtc1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblEtc1.Visible = false;
			// 
			// txtEtc1
			// 
			this.txtEtc1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtEtc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtEtc1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtEtc1.ImeMode = System.Windows.Forms.ImeMode.Hangul;
			this.txtEtc1.Location = new System.Drawing.Point(143, 73);
			this.txtEtc1.Name = "txtEtc1";
			this.txtEtc1.Size = new System.Drawing.Size(283, 21);
			this.txtEtc1.TabIndex = 21;
			this.txtEtc1.Visible = false;
			this.txtEtc1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Code_KeyDown);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.groupBox2);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 82);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(450, 374);
			this.panel2.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.fpSpread1);
			this.groupBox2.Location = new System.Drawing.Point(8, 9);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(434, 359);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "조회 결과";
			// 
			// fpSpread1
			// 
			this.fpSpread1.AccessibleDescription = "fpSpread1";
			this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fpSpread1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.fpSpread1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fpSpread1.Location = new System.Drawing.Point(8, 18);
			this.fpSpread1.Name = "fpSpread1";
			this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
			this.fpSpread1.Size = new System.Drawing.Size(418, 332);
			this.fpSpread1.TabIndex = 21;
			this.fpSpread1.EnterCell += new FarPoint.Win.Spread.EnterCellEventHandler(this.fpSpread1_EnterCell);
			this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellClick);
			this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
			this.fpSpread1.TopChange += new FarPoint.Win.Spread.TopChangeEventHandler(this.fpSpread1_TopChange);
			this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
			this.fpSpread1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyUp);
			// 
			// fpSpread1_Sheet1
			// 
			this.fpSpread1_Sheet1.Reset();
			this.fpSpread1_Sheet1.SheetName = "Sheet1";
			// 
			// FPPOPUP
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(450, 456);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FPPOPUP";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.PopUpSP_Load);
			this.panel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtCodeName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblCodeName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lblCode)).EndInit();
			this.panel2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
			this.ResumeLayout(false);

        }
        #endregion

        private C1.Win.C1Input.C1TextBox txtCode;
        private C1.Win.C1Input.C1Label lblCodeName;
        private C1.Win.C1Input.C1Label lblCode;
        private C1.Win.C1Input.C1TextBox txtCodeName;
        private C1.Win.C1Input.C1Button button1;
        private C1.Win.C1Input.C1Button btnClose;

    }
}