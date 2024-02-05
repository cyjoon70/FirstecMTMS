namespace ZB.ZBB020
{
    partial class ZBB020
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZBB020));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.chTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMidifiedDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFileNm = new C1.Win.C1Input.C1TextBox();
            this.wbrPdf = new System.Windows.Forms.WebBrowser();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPath = new C1.Win.C1Input.C1TextBox();
            this.btnFile_Access_Info = new C1.Win.C1Input.C1Button();
            this.btnFileServerSetting = new C1.Win.C1Input.C1Button();
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
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtFileNm)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1122, 64);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 64);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.wbrPdf);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(1122, 687);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lvFiles);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 409);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(291, 278);
            this.panel2.TabIndex = 1;
            // 
            // lvFiles
            // 
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTitle,
            this.chSize,
            this.chType,
            this.chMidifiedDate,
            this.chDirectory});
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.Location = new System.Drawing.Point(0, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(291, 278);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.DoubleClick += new System.EventHandler(this.lvFiles_DoubleClick);
            // 
            // chTitle
            // 
            this.chTitle.Text = "이름";
            this.chTitle.Width = 175;
            // 
            // chSize
            // 
            this.chSize.Text = "크기";
            this.chSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chType
            // 
            this.chType.Text = "종류";
            // 
            // chMidifiedDate
            // 
            this.chMidifiedDate.Text = "수정한 날짜";
            this.chMidifiedDate.Width = 120;
            // 
            // chDirectory
            // 
            this.chDirectory.Text = "위치";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tvFolders);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(291, 409);
            this.panel1.TabIndex = 0;
            // 
            // tvFolders
            // 
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.imageList1;
            this.tvFolders.Location = new System.Drawing.Point(0, 51);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(291, 358);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvFolders_BeforeExpand);
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "image1.ICO");
            this.imageList1.Images.SetKeyName(1, "image2.ICO");
            this.imageList1.Images.SetKeyName(2, "image3.png");
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtFileNm);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 51);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // txtFileNm
            // 
            this.txtFileNm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileNm.AutoSize = false;
            this.txtFileNm.BackColor = System.Drawing.Color.White;
            this.txtFileNm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtFileNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFileNm.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFileNm.Location = new System.Drawing.Point(16, 20);
            this.txtFileNm.Name = "txtFileNm";
            this.txtFileNm.Size = new System.Drawing.Size(266, 21);
            this.txtFileNm.TabIndex = 21;
            this.txtFileNm.Tag = ";;;;";
            this.txtFileNm.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // wbrPdf
            // 
            this.wbrPdf.AllowWebBrowserDrop = false;
            this.wbrPdf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbrPdf.Location = new System.Drawing.Point(0, 39);
            this.wbrPdf.MinimumSize = new System.Drawing.Size(23, 18);
            this.wbrPdf.Name = "wbrPdf";
            this.wbrPdf.Size = new System.Drawing.Size(827, 648);
            this.wbrPdf.TabIndex = 26;
            this.wbrPdf.WebBrowserShortcutsEnabled = false;
            this.wbrPdf.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbrPdf_Navigating);
            this.wbrPdf.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.wbrPdf_PreviewKeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.btnFileServerSetting);
            this.groupBox2.Controls.Add(this.btnFile_Access_Info);
            this.groupBox2.Controls.Add(this.txtPath);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(827, 39);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            // 
            // txtPath
            // 
            this.txtPath.AutoSize = false;
            this.txtPath.BackColor = System.Drawing.Color.White;
            this.txtPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPath.Location = new System.Drawing.Point(14, 12);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(547, 21);
            this.txtPath.TabIndex = 22;
            this.txtPath.Tag = ";;;;";
            this.txtPath.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // btnFile_Access_Info
            // 
            this.btnFile_Access_Info.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFile_Access_Info.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFile_Access_Info.BackgroundImage")));
            this.btnFile_Access_Info.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFile_Access_Info.Location = new System.Drawing.Point(567, 8);
            this.btnFile_Access_Info.Name = "btnFile_Access_Info";
            this.btnFile_Access_Info.Size = new System.Drawing.Size(127, 25);
            this.btnFile_Access_Info.TabIndex = 23;
            this.btnFile_Access_Info.Text = "파일접근이력조회";
            this.btnFile_Access_Info.UseVisualStyleBackColor = true;
            this.btnFile_Access_Info.Click += new System.EventHandler(this.btnFile_Access_Info_Click);
            // 
            // btnFileServerSetting
            // 
            this.btnFileServerSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileServerSetting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileServerSetting.BackgroundImage")));
            this.btnFileServerSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileServerSetting.Location = new System.Drawing.Point(712, 8);
            this.btnFileServerSetting.Name = "btnFileServerSetting";
            this.btnFileServerSetting.Size = new System.Drawing.Size(103, 25);
            this.btnFileServerSetting.TabIndex = 24;
            this.btnFileServerSetting.Text = "파일서버설정";
            this.btnFileServerSetting.UseVisualStyleBackColor = true;
            this.btnFileServerSetting.Click += new System.EventHandler(this.btnFileServerSetting_Click);
            // 
            // ZBB020
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1122, 751);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ZBB020";
            this.Text = "파일서버뷰어";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ZBB020_FormClosed);
            this.Load += new System.EventHandler(this.ZBB020_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.splitContainer1, 0);
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
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtFileNm)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1TextBox txtFileNm;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.TreeView tvFolders;
        private C1.Win.C1Input.C1TextBox txtPath;
        private System.Windows.Forms.ColumnHeader chTitle;
        private System.Windows.Forms.ColumnHeader chSize;
        private System.Windows.Forms.ColumnHeader chType;
        private System.Windows.Forms.ColumnHeader chMidifiedDate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader chDirectory;
        private System.Windows.Forms.WebBrowser wbrPdf;
        private C1.Win.C1Input.C1Button btnFile_Access_Info;
        private C1.Win.C1Input.C1Button btnFileServerSetting;

    }
}