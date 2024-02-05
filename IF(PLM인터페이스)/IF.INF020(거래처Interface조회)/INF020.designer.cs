namespace IF.INF020
{
    partial class INF020
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoCfmN = new System.Windows.Forms.RadioButton();
			this.rdoCfmY = new System.Windows.Forms.RadioButton();
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
			this.SuspendLayout();
			// 
			// GridCommGroupBox
			// 
			this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
			this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
			this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
			this.GridCommGroupBox.Size = new System.Drawing.Size(992, 281);
			// 
			// GridCommPanel
			// 
			this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GridCommPanel.Location = new System.Drawing.Point(0, 118);
			this.GridCommPanel.Size = new System.Drawing.Size(992, 281);
			// 
			// fpSpread1
			// 
			this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
			this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fpSpread1.Location = new System.Drawing.Point(3, 17);
			this.fpSpread1.Size = new System.Drawing.Size(986, 261);
			// 
			// fpSpread1_Sheet1
			// 
			this.fpSpread1_Sheet1.SheetName = "Sheet1";
			// 
			// panButton1
			// 
			this.panButton1.Size = new System.Drawing.Size(992, 64);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 64);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(992, 54);
			this.panel1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.rdoCfmN);
			this.groupBox1.Controls.Add(this.rdoCfmY);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(992, 54);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// rdoCfmN
			// 
			this.rdoCfmN.AutoSize = true;
			this.rdoCfmN.Location = new System.Drawing.Point(110, 20);
			this.rdoCfmN.Name = "rdoCfmN";
			this.rdoCfmN.Size = new System.Drawing.Size(59, 16);
			this.rdoCfmN.TabIndex = 3;
			this.rdoCfmN.Text = "확정후";
			this.rdoCfmN.UseVisualStyleBackColor = true;
			// 
			// rdoCfmY
			// 
			this.rdoCfmY.AutoSize = true;
			this.rdoCfmY.Checked = true;
			this.rdoCfmY.Location = new System.Drawing.Point(12, 20);
			this.rdoCfmY.Name = "rdoCfmY";
			this.rdoCfmY.Size = new System.Drawing.Size(59, 16);
			this.rdoCfmY.TabIndex = 2;
			this.rdoCfmY.TabStop = true;
			this.rdoCfmY.Text = "확정전";
			this.rdoCfmY.UseVisualStyleBackColor = true;
			// 
			// INF020
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(992, 399);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Name = "INF020";
			this.Text = "품목정보등록(멀티)";
			this.Load += new System.EventHandler(this.INF020_Load);
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
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdoCfmN;
		private System.Windows.Forms.RadioButton rdoCfmY;
	}
}