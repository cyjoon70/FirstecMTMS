﻿namespace ME.MEA001
{
    partial class MEA001P5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MEA001P5));
            this.btnOk = new C1.Win.C1Input.C1Button();
            this.butCancel = new C1.Win.C1Input.C1Button();
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
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Controls.Add(this.butCancel);
            this.GridCommGroupBox.Controls.Add(this.btnOk);
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(1042, 518);
            this.GridCommGroupBox.TabIndex = 0;
            this.GridCommGroupBox.Controls.SetChildIndex(this.fpSpread1, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.btnOk, 0);
            this.GridCommGroupBox.Controls.SetChildIndex(this.butCancel, 0);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(0, 64);
            this.GridCommPanel.Size = new System.Drawing.Size(1042, 518);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(1036, 464);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1042, 64);
            this.panButton1.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOk.BackgroundImage")));
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.Location = new System.Drawing.Point(854, 487);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 25);
            this.btnOk.TabIndex = 112;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butCancel.BackgroundImage")));
            this.butCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancel.Location = new System.Drawing.Point(948, 487);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(88, 25);
            this.butCancel.TabIndex = 113;
            this.butCancel.Text = "취소";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // MEA001P5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 582);
            this.Name = "MEA001P5";
            this.Text = "품질증빙 일괄적용";
            this.Load += new System.EventHandler(this.MEA001P5_Load);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private C1.Win.C1Input.C1Button btnOk;
        private C1.Win.C1Input.C1Button butCancel;
    }
}