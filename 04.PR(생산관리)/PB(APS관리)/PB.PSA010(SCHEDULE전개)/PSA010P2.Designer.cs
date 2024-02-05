namespace PB.PSA010
{
    partial class PSA010P2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PSA010P2));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClose = new C1.Win.C1Input.C1Button();
            this.btnConf = new C1.Win.C1Input.C1Button();
            this.cboSch = new C1.Win.C1List.C1Combo();
            this.c1Label6 = new C1.Win.C1Input.C1Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboSch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnConf);
            this.groupBox1.Controls.Add(this.cboSch);
            this.groupBox1.Controls.Add(this.c1Label6);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 150);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FPS 계획 확정";
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(197, 72);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 25);
            this.btnClose.TabIndex = 32;
            this.btnClose.Text = "취소";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnConf
            // 
            this.btnConf.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnConf.BackgroundImage")));
            this.btnConf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConf.Location = new System.Drawing.Point(21, 72);
            this.btnConf.Name = "btnConf";
            this.btnConf.Size = new System.Drawing.Size(96, 25);
            this.btnConf.TabIndex = 31;
            this.btnConf.Text = "확정";
            this.btnConf.UseVisualStyleBackColor = true;
            this.btnConf.Click += new System.EventHandler(this.btnConf_Click);
            // 
            // cboSch
            // 
            this.cboSch.AddItemSeparator = ';';
            this.cboSch.AutoSize = false;
            this.cboSch.Caption = "";
            this.cboSch.CaptionHeight = 17;
            this.cboSch.CaptionVisible = false;
            this.cboSch.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.cboSch.ColumnCaptionHeight = 18;
            this.cboSch.ColumnFooterHeight = 18;
            this.cboSch.ContentHeight = 15;
            this.cboSch.DeadAreaBackColor = System.Drawing.Color.Empty;
            this.cboSch.EditorBackColor = System.Drawing.SystemColors.Window;
            this.cboSch.EditorFont = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cboSch.EditorForeColor = System.Drawing.SystemColors.WindowText;
            this.cboSch.EditorHeight = 15;
            this.cboSch.Images.Add(((System.Drawing.Image)(resources.GetObject("cboSch.Images"))));
            this.cboSch.ItemHeight = 15;
            this.cboSch.Location = new System.Drawing.Point(151, 32);
            this.cboSch.MatchEntryTimeout = ((long)(2000));
            this.cboSch.MaxDropDownItems = ((short)(5));
            this.cboSch.MaxLength = 32767;
            this.cboSch.MouseCursor = System.Windows.Forms.Cursors.Default;
            this.cboSch.Name = "cboSch";
            this.cboSch.RowDivider.Style = C1.Win.C1List.LineStyleEnum.None;
            this.cboSch.RowSubDividerColor = System.Drawing.Color.DarkGray;
            this.cboSch.Size = new System.Drawing.Size(142, 21);
            this.cboSch.TabIndex = 3;
            this.cboSch.Tag = "";
            this.cboSch.PropBag = resources.GetString("cboSch.PropBag");
            // 
            // c1Label6
            // 
            this.c1Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label6.Location = new System.Drawing.Point(21, 32);
            this.c1Label6.Name = "c1Label6";
            this.c1Label6.Size = new System.Drawing.Size(131, 21);
            this.c1Label6.TabIndex = 2;
            this.c1Label6.Tag = null;
            this.c1Label6.Text = "확정 SCHEDULE";
            this.c1Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label6.TextDetached = true;
            this.c1Label6.Value = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(21, 115);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(272, 16);
            this.progressBar1.TabIndex = 0;
            // 
            // PSA010P2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 150);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PSA010P2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.PSA010P2_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboSch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private C1.Win.C1List.C1Combo cboSch;
        private C1.Win.C1Input.C1Label c1Label6;
        private C1.Win.C1Input.C1Button btnClose;
        private C1.Win.C1Input.C1Button btnConf;
    }
}