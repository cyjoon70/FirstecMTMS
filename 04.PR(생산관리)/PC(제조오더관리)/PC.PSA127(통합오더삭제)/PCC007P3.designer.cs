﻿namespace PC.PSA127
{   
    partial class PCC007P3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PCC007P3));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.txtWorkOrderNo = new C1.Win.C1Input.C1TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.txtWoNoRs = new C1.Win.C1Input.C1TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrderNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWoNoRs)).BeginInit();
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.BackColor = System.Drawing.Color.White;
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(903, 418);
            this.GridCommGroupBox.TabIndex = 0;
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 109);
            this.GridCommPanel.Size = new System.Drawing.Size(903, 418);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(897, 398);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(903, 64);
            this.panButton1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 45);
            this.panel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.txtWorkOrderNo);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Controls.Add(this.txtWoNoRs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(903, 45);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(341, 14);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(95, 21);
            this.c1Label2.TabIndex = 2;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "제조오더번호";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // txtWorkOrderNo
            // 
            this.txtWorkOrderNo.AutoSize = false;
            this.txtWorkOrderNo.BackColor = System.Drawing.Color.White;
            this.txtWorkOrderNo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWorkOrderNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWorkOrderNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtWorkOrderNo.Location = new System.Drawing.Point(436, 14);
            this.txtWorkOrderNo.Name = "txtWorkOrderNo";
            this.txtWorkOrderNo.Size = new System.Drawing.Size(215, 21);
            this.txtWorkOrderNo.TabIndex = 3;
            this.txtWorkOrderNo.Tag = null;
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(11, 14);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(95, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "대표오더번호";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // txtWoNoRs
            // 
            this.txtWoNoRs.AutoSize = false;
            this.txtWoNoRs.BackColor = System.Drawing.Color.White;
            this.txtWoNoRs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtWoNoRs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWoNoRs.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtWoNoRs.Location = new System.Drawing.Point(106, 14);
            this.txtWoNoRs.Name = "txtWoNoRs";
            this.txtWoNoRs.Size = new System.Drawing.Size(215, 21);
            this.txtWoNoRs.TabIndex = 1;
            this.txtWoNoRs.Tag = ";1;;";
            // 
            // PCC007P3
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(903, 527);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PCC007P3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "통합오더삭제";
            this.Load += new System.EventHandler(this.PCC007P3_Load);
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkOrderNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWoNoRs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1TextBox txtWorkOrderNo;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1TextBox txtWoNoRs;

    }
}