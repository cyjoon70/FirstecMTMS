namespace EDocument.Forms
{
	partial class FileViewer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileViewer));
			this.pdfViewer = new AxAcroPDFLib.AxAcroPDF();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.c1Label9 = new C1.Win.C1Input.C1Label();
			this.txtFilename = new C1.Win.C1Input.C1TextBox();
			this.c1Label2 = new C1.Win.C1Input.C1Label();
			this.txtDocTypeName = new C1.Win.C1Input.C1TextBox();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.c1Label3 = new C1.Win.C1Input.C1Label();
			this.txtDocRevision = new C1.Win.C1Input.C1TextBox();
			this.c1Label4 = new C1.Win.C1Input.C1Label();
			this.txtDocNumber = new C1.Win.C1Input.C1TextBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnPrevPage = new System.Windows.Forms.Button();
			this.btnNextPage = new System.Windows.Forms.Button();
			this.panViewer = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.c1Label9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtFilename)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocTypeName)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocRevision)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocNumber)).BeginInit();
			this.panViewer.SuspendLayout();
			this.SuspendLayout();
			// 
			// pdfViewer
			// 
			this.pdfViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pdfViewer.Enabled = true;
			this.pdfViewer.Location = new System.Drawing.Point(0, 0);
			this.pdfViewer.Name = "pdfViewer";
			this.pdfViewer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("pdfViewer.OcxState")));
			this.pdfViewer.Size = new System.Drawing.Size(760, 468);
			this.pdfViewer.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(12, 12);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer1.Size = new System.Drawing.Size(628, 25);
			this.splitContainer1.SplitterDistance = 360;
			this.splitContainer1.TabIndex = 5;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.IsSplitterFixed = true;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.c1Label9);
			this.splitContainer3.Panel1.Controls.Add(this.txtFilename);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.c1Label2);
			this.splitContainer3.Panel2.Controls.Add(this.txtDocTypeName);
			this.splitContainer3.Size = new System.Drawing.Size(360, 25);
			this.splitContainer3.SplitterDistance = 200;
			this.splitContainer3.TabIndex = 7;
			// 
			// c1Label9
			// 
			this.c1Label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.c1Label9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.c1Label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.c1Label9.Location = new System.Drawing.Point(3, 2);
			this.c1Label9.Name = "c1Label9";
			this.c1Label9.Size = new System.Drawing.Size(80, 21);
			this.c1Label9.TabIndex = 5;
			this.c1Label9.Tag = null;
			this.c1Label9.Text = "파일명";
			this.c1Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.c1Label9.TextDetached = true;
			this.c1Label9.Value = "";
			// 
			// txtFilename
			// 
			this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFilename.AutoSize = false;
			this.txtFilename.BackColor = System.Drawing.Color.White;
			this.txtFilename.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtFilename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtFilename.Location = new System.Drawing.Point(82, 2);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.ReadOnly = true;
			this.txtFilename.Size = new System.Drawing.Size(115, 21);
			this.txtFilename.TabIndex = 10;
			this.txtFilename.Tag = null;
			this.txtFilename.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
			// 
			// c1Label2
			// 
			this.c1Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.c1Label2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.c1Label2.Location = new System.Drawing.Point(3, 2);
			this.c1Label2.Name = "c1Label2";
			this.c1Label2.Size = new System.Drawing.Size(80, 21);
			this.c1Label2.TabIndex = 7;
			this.c1Label2.Tag = null;
			this.c1Label2.Text = "종류";
			this.c1Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.c1Label2.TextDetached = true;
			this.c1Label2.Value = "";
			// 
			// txtDocTypeName
			// 
			this.txtDocTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDocTypeName.AutoSize = false;
			this.txtDocTypeName.BackColor = System.Drawing.Color.White;
			this.txtDocTypeName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtDocTypeName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDocTypeName.Location = new System.Drawing.Point(82, 2);
			this.txtDocTypeName.Name = "txtDocTypeName";
			this.txtDocTypeName.ReadOnly = true;
			this.txtDocTypeName.Size = new System.Drawing.Size(71, 21);
			this.txtDocTypeName.TabIndex = 11;
			this.txtDocTypeName.Tag = null;
			this.txtDocTypeName.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.IsSplitterFixed = true;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.c1Label3);
			this.splitContainer4.Panel1.Controls.Add(this.txtDocRevision);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.c1Label4);
			this.splitContainer4.Panel2.Controls.Add(this.txtDocNumber);
			this.splitContainer4.Size = new System.Drawing.Size(264, 25);
			this.splitContainer4.SplitterDistance = 119;
			this.splitContainer4.TabIndex = 8;
			// 
			// c1Label3
			// 
			this.c1Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.c1Label3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.c1Label3.Location = new System.Drawing.Point(3, 2);
			this.c1Label3.Name = "c1Label3";
			this.c1Label3.Size = new System.Drawing.Size(80, 21);
			this.c1Label3.TabIndex = 5;
			this.c1Label3.Tag = null;
			this.c1Label3.Text = "개정번호";
			this.c1Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.c1Label3.TextDetached = true;
			this.c1Label3.Value = "";
			// 
			// txtDocRevision
			// 
			this.txtDocRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDocRevision.AutoSize = false;
			this.txtDocRevision.BackColor = System.Drawing.Color.White;
			this.txtDocRevision.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtDocRevision.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDocRevision.Location = new System.Drawing.Point(82, 2);
			this.txtDocRevision.Name = "txtDocRevision";
			this.txtDocRevision.ReadOnly = true;
			this.txtDocRevision.Size = new System.Drawing.Size(34, 21);
			this.txtDocRevision.TabIndex = 12;
			this.txtDocRevision.Tag = null;
			this.txtDocRevision.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
			// 
			// c1Label4
			// 
			this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
			this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.c1Label4.Location = new System.Drawing.Point(3, 2);
			this.c1Label4.Name = "c1Label4";
			this.c1Label4.Size = new System.Drawing.Size(80, 21);
			this.c1Label4.TabIndex = 7;
			this.c1Label4.Tag = null;
			this.c1Label4.Text = "문서번호";
			this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.c1Label4.TextDetached = true;
			this.c1Label4.Value = "";
			// 
			// txtDocNumber
			// 
			this.txtDocNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDocNumber.AutoSize = false;
			this.txtDocNumber.BackColor = System.Drawing.Color.White;
			this.txtDocNumber.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
			this.txtDocNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtDocNumber.Location = new System.Drawing.Point(82, 2);
			this.txtDocNumber.Name = "txtDocNumber";
			this.txtDocNumber.ReadOnly = true;
			this.txtDocNumber.Size = new System.Drawing.Size(56, 21);
			this.txtDocNumber.TabIndex = 12;
			this.txtDocNumber.Tag = null;
			this.txtDocNumber.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(672, 517);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(100, 32);
			this.btnClose.TabIndex = 7;
			this.btnClose.Text = "닫기";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnPrevPage
			// 
			this.btnPrevPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPrevPage.Location = new System.Drawing.Point(646, 12);
			this.btnPrevPage.Name = "btnPrevPage";
			this.btnPrevPage.Size = new System.Drawing.Size(60, 25);
			this.btnPrevPage.TabIndex = 8;
			this.btnPrevPage.Text = "<";
			this.btnPrevPage.UseVisualStyleBackColor = true;
			this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
			// 
			// btnNextPage
			// 
			this.btnNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnNextPage.Location = new System.Drawing.Point(712, 12);
			this.btnNextPage.Name = "btnNextPage";
			this.btnNextPage.Size = new System.Drawing.Size(60, 25);
			this.btnNextPage.TabIndex = 9;
			this.btnNextPage.Text = ">";
			this.btnNextPage.UseVisualStyleBackColor = true;
			this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
			// 
			// panViewer
			// 
			this.panViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panViewer.Controls.Add(this.pdfViewer);
			this.panViewer.Location = new System.Drawing.Point(12, 43);
			this.panViewer.Name = "panViewer";
			this.panViewer.Size = new System.Drawing.Size(760, 468);
			this.panViewer.TabIndex = 10;
			// 
			// FileViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.panViewer);
			this.Controls.Add(this.btnNextPage);
			this.Controls.Add(this.btnPrevPage);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.splitContainer1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MinimizeBox = false;
			this.Name = "FileViewer";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "문서열람";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.WNDW035_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WNDW035_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pdfViewer)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.c1Label9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtFilename)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocTypeName)).EndInit();
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocRevision)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtDocNumber)).EndInit();
			this.panViewer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private AxAcroPDFLib.AxAcroPDF pdfViewer;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private C1.Win.C1Input.C1Label c1Label9;
		private C1.Win.C1Input.C1Label c1Label2;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private C1.Win.C1Input.C1Label c1Label3;
		private C1.Win.C1Input.C1Label c1Label4;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnPrevPage;
		private System.Windows.Forms.Button btnNextPage;
		private C1.Win.C1Input.C1TextBox txtFilename;
		private C1.Win.C1Input.C1TextBox txtDocTypeName;
		private C1.Win.C1Input.C1TextBox txtDocRevision;
		private C1.Win.C1Input.C1TextBox txtDocNumber;
		private System.Windows.Forms.Panel panViewer;
	}
}