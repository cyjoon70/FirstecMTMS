namespace AH.ACH011
{
    partial class ACH011
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ACH011));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtYYMM = new C1.Win.C1Input.C1TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.optRun = new System.Windows.Forms.RadioButton();
            this.optCancel = new System.Windows.Forms.RadioButton();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtYYMM)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1222, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1222, 591);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtYYMM);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.c1Label3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1222, 591);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // txtYYMM
            // 
            this.txtYYMM.AutoSize = false;
            this.txtYYMM.BackColor = System.Drawing.Color.White;
            this.txtYYMM.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtYYMM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYYMM.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtYYMM.Location = new System.Drawing.Point(105, 17);
            this.txtYYMM.Name = "txtYYMM";
            this.txtYYMM.Size = new System.Drawing.Size(124, 21);
            this.txtYYMM.TabIndex = 1;
            this.txtYYMM.Tag = ";2;;";
            this.txtYYMM.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.optRun);
            this.panel3.Controls.Add(this.optCancel);
            this.panel3.Location = new System.Drawing.Point(105, 41);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(332, 21);
            this.panel3.TabIndex = 5;
            // 
            // optRun
            // 
            this.optRun.Checked = true;
            this.optRun.Location = new System.Drawing.Point(10, 2);
            this.optRun.Name = "optRun";
            this.optRun.Size = new System.Drawing.Size(137, 18);
            this.optRun.TabIndex = 0;
            this.optRun.TabStop = true;
            this.optRun.Text = "결과반영";
            this.optRun.UseVisualStyleBackColor = true;
            // 
            // optCancel
            // 
            this.optCancel.Location = new System.Drawing.Point(175, 2);
            this.optCancel.Name = "optCancel";
            this.optCancel.Size = new System.Drawing.Size(146, 18);
            this.optCancel.TabIndex = 1;
            this.optCancel.Text = "반영취소";
            this.optCancel.UseVisualStyleBackColor = true;
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(12, 17);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(94, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "기준년월";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // c1Label3
            // 
            this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label3.Location = new System.Drawing.Point(12, 41);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(94, 21);
            this.c1Label3.TabIndex = 4;
            this.c1Label3.Tag = null;
            this.c1Label3.Text = "작업구분";
            this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label3.TextDetached = true;
            this.c1Label3.Value = "";
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
            // ACH011
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1222, 655);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ACH011";
            this.Text = "감가상각결과반영";
            this.Load += new System.EventHandler(this.ACH011_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtYYMM)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button btnSlipView;
        private System.Windows.Forms.Splitter splitter1;
        private C1.Win.C1Input.C1Label c1Label3;
        private C1.Win.C1Input.C1Label c1Label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton optRun;
        private System.Windows.Forms.RadioButton optCancel;
        private C1.Win.C1Input.C1TextBox txtYYMM;
    }
}