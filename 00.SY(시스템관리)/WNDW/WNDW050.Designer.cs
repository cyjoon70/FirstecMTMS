namespace WNDW
{
    partial class WNDW050
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WNDW050));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAllCancel = new System.Windows.Forms.Button();
            this.btnLineConfirm = new System.Windows.Forms.Button();
            this.btnLineDel = new System.Windows.Forms.Button();
            this.btnReference = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.GridCommGroupBox.SuspendLayout();
            this.GridCommPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.TreeCommPanel.SuspendLayout();
            this.TreeCommGroupBox.SuspendLayout();
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
            this.SuspendLayout();
            // 
            // GridCommGroupBox
            // 
            this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.GridCommGroupBox.Size = new System.Drawing.Size(549, 706);
            // 
            // GridCommPanel
            // 
            this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridCommPanel.Location = new System.Drawing.Point(427, 64);
            this.GridCommPanel.Size = new System.Drawing.Size(549, 706);
            // 
            // fpSpread1
            // 
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpSpread1.Location = new System.Drawing.Point(3, 17);
            this.fpSpread1.Size = new System.Drawing.Size(543, 686);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // 
            // TreeCommPanel
            // 
            this.TreeCommPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.TreeCommPanel.Location = new System.Drawing.Point(0, 64);
            this.TreeCommPanel.Size = new System.Drawing.Size(347, 706);
            // 
            // TreeCommGroupBox
            // 
            this.TreeCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeCommGroupBox.Location = new System.Drawing.Point(0, 0);
            this.TreeCommGroupBox.Size = new System.Drawing.Size(347, 706);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeView1.LineColor = System.Drawing.Color.Black;
            this.treeView1.Location = new System.Drawing.Point(3, 17);
            this.treeView1.Size = new System.Drawing.Size(341, 686);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(976, 64);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAllCancel);
            this.groupBox1.Controls.Add(this.btnLineConfirm);
            this.groupBox1.Controls.Add(this.btnLineDel);
            this.groupBox1.Controls.Add(this.btnReference);
            this.groupBox1.Controls.Add(this.btnConfirm);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(347, 64);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(80, 706);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            // 
            // btnAllCancel
            // 
            this.btnAllCancel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAllCancel.Image = global::WNDW.Properties.Resources.cancel;
            this.btnAllCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAllCancel.Location = new System.Drawing.Point(3, 326);
            this.btnAllCancel.Name = "btnAllCancel";
            this.btnAllCancel.Size = new System.Drawing.Size(71, 41);
            this.btnAllCancel.TabIndex = 5;
            this.btnAllCancel.Text = "취소";
            this.btnAllCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAllCancel.UseVisualStyleBackColor = true;
            this.btnAllCancel.Click += new System.EventHandler(this.btnAllCancel_Click);
            // 
            // btnLineConfirm
            // 
            this.btnLineConfirm.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLineConfirm.Image = global::WNDW.Properties.Resources.accept_button;
            this.btnLineConfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLineConfirm.Location = new System.Drawing.Point(3, 279);
            this.btnLineConfirm.Name = "btnLineConfirm";
            this.btnLineConfirm.Size = new System.Drawing.Size(71, 41);
            this.btnLineConfirm.TabIndex = 4;
            this.btnLineConfirm.Text = "확인";
            this.btnLineConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLineConfirm.UseVisualStyleBackColor = true;
            this.btnLineConfirm.Click += new System.EventHandler(this.btnLineConfirm_Click);
            // 
            // btnLineDel
            // 
            this.btnLineDel.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnLineDel.Image = global::WNDW.Properties.Resources.delete;
            this.btnLineDel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLineDel.Location = new System.Drawing.Point(3, 158);
            this.btnLineDel.Name = "btnLineDel";
            this.btnLineDel.Size = new System.Drawing.Size(71, 41);
            this.btnLineDel.TabIndex = 3;
            this.btnLineDel.Text = "삭제";
            this.btnLineDel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLineDel.UseVisualStyleBackColor = true;
            this.btnLineDel.Click += new System.EventHandler(this.btnLineDel_Click);
            // 
            // btnReference
            // 
            this.btnReference.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnReference.Image = global::WNDW.Properties.Resources.add;
            this.btnReference.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReference.Location = new System.Drawing.Point(3, 111);
            this.btnReference.Name = "btnReference";
            this.btnReference.Size = new System.Drawing.Size(71, 41);
            this.btnReference.TabIndex = 2;
            this.btnReference.Text = "참조";
            this.btnReference.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReference.UseVisualStyleBackColor = true;
            this.btnReference.Click += new System.EventHandler(this.btnReference_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnConfirm.Image = global::WNDW.Properties.Resources.add;
            this.btnConfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfirm.Location = new System.Drawing.Point(3, 64);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(71, 41);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "승인";
            this.btnConfirm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAdd.Image = global::WNDW.Properties.Resources.add;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(3, 17);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(71, 41);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "검토";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cog.png");
            this.imageList1.Images.SetKeyName(1, "folder.png");
            this.imageList1.Images.SetKeyName(2, "Emp.png");
            // 
            // WNDW050
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 770);
            this.Controls.Add(this.groupBox1);
            this.Name = "WNDW050";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WNDW050";
            this.Load += new System.EventHandler(this.WNDW050_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.TreeCommPanel, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.GridCommPanel, 0);
            this.GridCommGroupBox.ResumeLayout(false);
            this.GridCommPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.TreeCommPanel.ResumeLayout(false);
            this.TreeCommGroupBox.ResumeLayout(false);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnLineDel;
        private System.Windows.Forms.Button btnReference;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnLineConfirm;
        private System.Windows.Forms.Button btnAllCancel;
    }
}