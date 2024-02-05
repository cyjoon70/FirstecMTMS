namespace BB.BBB006
{
    partial class BBB006
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BBB006));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdCancel = new C1.Win.C1Input.C1Button();
            this.cmdChange = new C1.Win.C1Input.C1Button();
            this.txtChPWCf = new C1.Win.C1Input.C1TextBox();
            this.txtChPW = new C1.Win.C1Input.C1TextBox();
            this.txtNowPW = new C1.Win.C1Input.C1TextBox();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChPWCf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChPW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNowPW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.cmdCancel);
            this.groupBox1.Controls.Add(this.cmdChange);
            this.groupBox1.Controls.Add(this.txtChPWCf);
            this.groupBox1.Controls.Add(this.txtChPW);
            this.groupBox1.Controls.Add(this.txtNowPW);
            this.groupBox1.Controls.Add(this.c1Label2);
            this.groupBox1.Controls.Add(this.c1Label5);
            this.groupBox1.Controls.Add(this.c1Label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 153);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "비밀번호 변경";
            // 
            // cmdCancel
            // 
            this.cmdCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdCancel.BackgroundImage")));
            this.cmdCancel.Location = new System.Drawing.Point(192, 113);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(72, 25);
            this.cmdCancel.TabIndex = 42;
            this.cmdCancel.Text = "취소";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdChange
            // 
            this.cmdChange.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdChange.BackgroundImage")));
            this.cmdChange.Location = new System.Drawing.Point(102, 113);
            this.cmdChange.Name = "cmdChange";
            this.cmdChange.Size = new System.Drawing.Size(72, 25);
            this.cmdChange.TabIndex = 41;
            this.cmdChange.Text = "변경";
            this.cmdChange.UseVisualStyleBackColor = true;
            this.cmdChange.Click += new System.EventHandler(this.cmdChange_Click);
            // 
            // txtChPWCf
            // 
            this.txtChPWCf.AutoSize = false;
            this.txtChPWCf.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChPWCf.Location = new System.Drawing.Point(137, 81);
            this.txtChPWCf.Name = "txtChPWCf";
            this.txtChPWCf.PasswordChar = '*';
            this.txtChPWCf.Size = new System.Drawing.Size(181, 21);
            this.txtChPWCf.TabIndex = 40;
            this.txtChPWCf.Tag = null;
            this.txtChPWCf.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChPWCf_KeyDown);
            // 
            // txtChPW
            // 
            this.txtChPW.AutoSize = false;
            this.txtChPW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChPW.Location = new System.Drawing.Point(137, 53);
            this.txtChPW.Name = "txtChPW";
            this.txtChPW.PasswordChar = '*';
            this.txtChPW.Size = new System.Drawing.Size(181, 21);
            this.txtChPW.TabIndex = 39;
            this.txtChPW.Tag = null;
            // 
            // txtNowPW
            // 
            this.txtNowPW.AutoSize = false;
            this.txtNowPW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNowPW.Location = new System.Drawing.Point(137, 26);
            this.txtNowPW.Name = "txtNowPW";
            this.txtNowPW.PasswordChar = '*';
            this.txtNowPW.Size = new System.Drawing.Size(181, 21);
            this.txtNowPW.TabIndex = 38;
            this.txtNowPW.Tag = null;
            this.txtNowPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNowPW_KeyDown);
            // 
            // c1Label2
            // 
            this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label2.Location = new System.Drawing.Point(38, 81);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(100, 21);
            this.c1Label2.TabIndex = 6;
            this.c1Label2.Tag = null;
            this.c1Label2.Text = "비밀번호 확인";
            this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label2.TextDetached = true;
            this.c1Label2.Value = "";
            // 
            // c1Label5
            // 
            this.c1Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label5.Location = new System.Drawing.Point(38, 53);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(100, 21);
            this.c1Label5.TabIndex = 4;
            this.c1Label5.Tag = null;
            this.c1Label5.Text = "변경 비밀번호";
            this.c1Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label5.TextDetached = true;
            this.c1Label5.Value = "";
            // 
            // c1Label1
            // 
            this.c1Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label1.Location = new System.Drawing.Point(38, 26);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(100, 21);
            this.c1Label1.TabIndex = 0;
            this.c1Label1.Tag = null;
            this.c1Label1.Text = "기존 비밀번호";
            this.c1Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label1.TextDetached = true;
            this.c1Label1.Value = "";
            // 
            // BBB006
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(387, 179);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BBB006";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BBB006";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtChPWCf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChPW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNowPW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1Label c1Label5;
        private C1.Win.C1Input.C1Label c1Label1;
        private C1.Win.C1Input.C1Button cmdCancel;
        private C1.Win.C1Input.C1Button cmdChange;
        private C1.Win.C1Input.C1TextBox txtChPWCf;
        private C1.Win.C1Input.C1TextBox txtChPW;
        private C1.Win.C1Input.C1TextBox txtNowPW;
    }
}