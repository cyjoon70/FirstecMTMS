namespace TD.TDA009
{
	partial class DeleteDialog
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
			this.chkDocFile = new System.Windows.Forms.CheckBox();
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkSourceFile = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkDocFile
			// 
			this.chkDocFile.AutoSize = true;
			this.chkDocFile.Location = new System.Drawing.Point(25, 30);
			this.chkDocFile.Name = "chkDocFile";
			this.chkDocFile.Size = new System.Drawing.Size(72, 16);
			this.chkDocFile.TabIndex = 0;
			this.chkDocFile.Text = "문서파일";
			this.chkDocFile.UseVisualStyleBackColor = true;
			this.chkDocFile.CheckedChanged += new System.EventHandler(this.InputControls_ConditionChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.chkSourceFile);
			this.groupBox1.Controls.Add(this.chkDocFile);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(410, 65);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "정리대상";
			// 
			// chkSourceFile
			// 
			this.chkSourceFile.AutoSize = true;
			this.chkSourceFile.Location = new System.Drawing.Point(115, 30);
			this.chkSourceFile.Name = "chkSourceFile";
			this.chkSourceFile.Size = new System.Drawing.Size(96, 16);
			this.chkSourceFile.TabIndex = 1;
			this.chkSourceFile.Text = "기술자료파일";
			this.chkSourceFile.UseVisualStyleBackColor = true;
			this.chkSourceFile.CheckedChanged += new System.EventHandler(this.InputControls_ConditionChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.txtPassword);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(12, 90);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(410, 69);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "비밀번호";
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(270, 26);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(120, 21);
			this.txtPassword.TabIndex = 0;
			this.txtPassword.TextChanged += new System.EventHandler(this.InputControls_ConditionChanged);
			this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(241, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "확인을 위해 계정 비밀번호를 입력하십시오:";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(332, 175);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 30);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "취소";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(12, 175);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(163, 30);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "모든 삭제파일 영구제거";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// DeleteDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(434, 221);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeleteDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "삭제파일정리";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox chkDocFile;
		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox chkSourceFile;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
	}
}