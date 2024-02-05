namespace HA.HAA008
{
    partial class HAA008
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HAA008));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new C1.Win.C1Input.C1Button();
            this.dtpDate = new C1.Win.C1Input.C1DateEdit();
            this.txtBizNm = new C1.Win.C1Input.C1TextBox();
            this.btnBizCd = new C1.Win.C1Input.C1Button();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtBizCd = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBizNm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBizCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(811, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 562);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.txtBizNm);
            this.groupBox1.Controls.Add(this.btnBizCd);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtBizCd);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(473, 106);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Location = new System.Drawing.Point(11, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 25);
            this.button2.TabIndex = 147;
            this.button2.Text = "근태현황";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            this.dtpDate.Location = new System.Drawing.Point(92, 17);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(99, 21);
            this.dtpDate.TabIndex = 1;
            this.dtpDate.Tag = null;
            this.dtpDate.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.dtpDate.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.DropDown;
            // 
            // txtBizNm
            // 
            this.txtBizNm.AutoSize = false;
            this.txtBizNm.BackColor = System.Drawing.Color.White;
            this.txtBizNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtBizNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBizNm.Location = new System.Drawing.Point(215, 44);
            this.txtBizNm.Name = "txtBizNm";
            this.txtBizNm.Size = new System.Drawing.Size(240, 21);
            this.txtBizNm.TabIndex = 5;
            this.txtBizNm.Tag = ";2;;";
            // 
            // btnBizCd
            // 
            this.btnBizCd.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnBizCd.BackgroundImage")));
            this.btnBizCd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnBizCd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBizCd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBizCd.Location = new System.Drawing.Point(191, 44);
            this.btnBizCd.Name = "btnBizCd";
            this.btnBizCd.Size = new System.Drawing.Size(24, 21);
            this.btnBizCd.TabIndex = 4;
            this.btnBizCd.UseVisualStyleBackColor = true;
            this.btnBizCd.Click += new System.EventHandler(this.btnBizCd_Click);
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(12, 44);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(80, 21);
            this.c1Label2.TabIndex = 2;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "사업장";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtBizCd
            // 
            this.txtBizCd.AutoSize = false;
            this.txtBizCd.BackColor = System.Drawing.Color.White;
            this.txtBizCd.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtBizCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBizCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBizCd.Location = new System.Drawing.Point(91, 44);
            this.txtBizCd.Name = "txtBizCd";
            this.txtBizCd.Size = new System.Drawing.Size(100, 21);
            this.txtBizCd.TabIndex = 3;
            this.txtBizCd.Tag = "사업장;1;;";
            this.txtBizCd.TextChanged += new System.EventHandler(this.txtBizCd_TextChanged);
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(12, 17);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(80, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "근태일자";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
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
            // HAA008
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(811, 626);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "HAA008";
            this.Text = "채권상세출력";
            this.Load += new System.EventHandler(this.HAA008_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.dtpDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBizNm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBizCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1Input.C1DateEdit dtpDate;
        private C1.Win.C1Input.C1TextBox txtBizNm;
        private C1.Win.C1Input.C1Button btnBizCd;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtBizCd;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Button button2;
    }
}