namespace PA.PBA102
{
    partial class PBA102P2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PBA102P2));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCLOSE = new C1.Win.C1Input.C1Button();
            this.cboYear = new C1.Win.C1Input.C1DateEdit();
            this.btnCreate_Cal = new C1.Win.C1Input.C1Button();
            this.c1Label21 = new C1.Win.C1Input.C1Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label21)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCLOSE);
            this.groupBox1.Controls.Add(this.cboYear);
            this.groupBox1.Controls.Add(this.btnCreate_Cal);
            this.groupBox1.Controls.Add(this.c1Label21);
            this.groupBox1.Location = new System.Drawing.Point(2, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(509, 56);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnCLOSE
            // 
            this.btnCLOSE.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCLOSE.BackgroundImage")));
            this.btnCLOSE.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCLOSE.Location = new System.Drawing.Point(424, 13);
            this.btnCLOSE.Name = "btnCLOSE";
            this.btnCLOSE.Size = new System.Drawing.Size(76, 29);
            this.btnCLOSE.TabIndex = 76;
            this.btnCLOSE.Text = "창닫기";
            this.btnCLOSE.UseVisualStyleBackColor = true;
            this.btnCLOSE.Click += new System.EventHandler(this.btnCLOSE_Click);
            // 
            // cboYear
            // 
            this.cboYear.AutoSize = false;
            this.cboYear.BackColor = System.Drawing.Color.White;
            this.cboYear.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            // 
            // 
            // 
            this.cboYear.Calendar.DayNameLength = 1;
            this.cboYear.CustomFormat = "yyyy";
            this.cboYear.DropDownFormAlign = C1.Win.C1Input.DropDownFormAlignmentEnum.Left;
            this.cboYear.FormatType = C1.Win.C1Input.FormatTypeEnum.CustomFormat;
            this.cboYear.Location = new System.Drawing.Point(93, 17);
            this.cboYear.Name = "cboYear";
            this.cboYear.Size = new System.Drawing.Size(89, 21);
            this.cboYear.TabIndex = 75;
            this.cboYear.Tag = null;
            this.cboYear.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            this.cboYear.VisibleButtons = C1.Win.C1Input.DropDownControlButtonFlags.UpDown;
            // 
            // btnCreate_Cal
            // 
            this.btnCreate_Cal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCreate_Cal.BackgroundImage")));
            this.btnCreate_Cal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreate_Cal.Location = new System.Drawing.Point(241, 13);
            this.btnCreate_Cal.Name = "btnCreate_Cal";
            this.btnCreate_Cal.Size = new System.Drawing.Size(101, 29);
            this.btnCreate_Cal.TabIndex = 74;
            this.btnCreate_Cal.Text = "카렌다 생성";
            this.btnCreate_Cal.UseVisualStyleBackColor = true;
            this.btnCreate_Cal.Click += new System.EventHandler(this.btnCreate_Cal_Click);
            // 
            // c1Label21
            // 
            this.c1Label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label21.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label21.Location = new System.Drawing.Point(10, 17);
            this.c1Label21.Name = "c1Label21";
            this.c1Label21.Size = new System.Drawing.Size(86, 21);
            this.c1Label21.TabIndex = 11;
            this.c1Label21.Tag = null;
            this.c1Label21.Text = "생성 년도";
            this.c1Label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label21.TextDetached = true;
            this.c1Label21.Value = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(2, 6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(509, 23);
            this.progressBar1.TabIndex = 75;
            // 
            // PBA102P2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(514, 97);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Name = "PBA102P2";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "기준달력생성";
            this.Load += new System.EventHandler(this.PBA102P2_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label21)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label21;
        private C1.Win.C1Input.C1Button btnCreate_Cal;
        private System.Windows.Forms.ProgressBar progressBar1;
        private C1.Win.C1Input.C1DateEdit cboYear;
        private C1.Win.C1Input.C1Button btnCLOSE;
    }
}