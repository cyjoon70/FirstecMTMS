namespace ZB.ZBB030
{
    partial class ZBB030
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ZBB030));
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.c1Button2 = new C1.Win.C1Input.C1Button();
            this.txtDir = new C1.Win.C1Input.C1TextBox();
            this.c1Button1 = new C1.Win.C1Input.C1Button();
            this.DirTreeView = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.FileListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.chTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chMidifiedDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFileDel = new C1.Win.C1Input.C1Button();
            this.btnUpload = new C1.Win.C1Input.C1Button();
            this.txtPath = new C1.Win.C1Input.C1TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitter3 = new System.Windows.Forms.Splitter();
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDir)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panButton1
            // 
            this.panButton1.Size = new System.Drawing.Size(1122, 64);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DirTreeView);
            this.panel1.Controls.Add(this.splitter2);
            this.panel1.Controls.Add(this.tvFolders);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(377, 687);
            this.panel1.TabIndex = 0;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.Color.Beige;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 362);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(377, 5);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // tvFolders
            // 
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Top;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.imageList1;
            this.tvFolders.Location = new System.Drawing.Point(0, 44);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.Size = new System.Drawing.Size(377, 318);
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
            this.groupBox1.Controls.Add(this.c1Label4);
            this.groupBox1.Controls.Add(this.c1Button2);
            this.groupBox1.Controls.Add(this.txtDir);
            this.groupBox1.Controls.Add(this.c1Button1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 44);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // c1Label4
            // 
            this.c1Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.c1Label4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c1Label4.Location = new System.Drawing.Point(6, 14);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(52, 21);
            this.c1Label4.TabIndex = 29;
            this.c1Label4.Tag = null;
            this.c1Label4.Text = "폴더명";
            this.c1Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.c1Label4.TextDetached = true;
            this.c1Label4.Value = "";
            // 
            // c1Button2
            // 
            this.c1Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.c1Button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("c1Button2.BackgroundImage")));
            this.c1Button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.c1Button2.Location = new System.Drawing.Point(295, 12);
            this.c1Button2.Name = "c1Button2";
            this.c1Button2.Size = new System.Drawing.Size(76, 25);
            this.c1Button2.TabIndex = 28;
            this.c1Button2.Text = "폴더삭제";
            this.c1Button2.UseVisualStyleBackColor = true;
            this.c1Button2.Click += new System.EventHandler(this.btnDeleteDir_Click);
            // 
            // txtDir
            // 
            this.txtDir.AutoSize = false;
            this.txtDir.BackColor = System.Drawing.Color.White;
            this.txtDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDir.Location = new System.Drawing.Point(58, 14);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(146, 21);
            this.txtDir.TabIndex = 27;
            this.txtDir.Tag = ";;;;";
            this.txtDir.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // c1Button1
            // 
            this.c1Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.c1Button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("c1Button1.BackgroundImage")));
            this.c1Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.c1Button1.Location = new System.Drawing.Point(214, 12);
            this.c1Button1.Name = "c1Button1";
            this.c1Button1.Size = new System.Drawing.Size(76, 25);
            this.c1Button1.TabIndex = 26;
            this.c1Button1.Text = "폴더생성";
            this.c1Button1.UseVisualStyleBackColor = true;
            this.c1Button1.Click += new System.EventHandler(this.btnMakeDir_Click);
            // 
            // DirTreeView
            // 
            this.DirTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DirTreeView.ImageIndex = 0;
            this.DirTreeView.ImageList = this.imageList1;
            this.DirTreeView.Location = new System.Drawing.Point(0, 367);
            this.DirTreeView.Name = "DirTreeView";
            this.DirTreeView.SelectedImageIndex = 0;
            this.DirTreeView.Size = new System.Drawing.Size(377, 320);
            this.DirTreeView.TabIndex = 3;
            this.DirTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.DirTreeView_AfterExpand);
            this.DirTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DirTreeView_AfterSelect);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.FileListView);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 368);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(740, 319);
            this.panel3.TabIndex = 27;
            // 
            // FileListView
            // 
            this.FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.FileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileListView.Location = new System.Drawing.Point(0, 0);
            this.FileListView.Name = "FileListView";
            this.FileListView.Size = new System.Drawing.Size(740, 319);
            this.FileListView.TabIndex = 0;
            this.FileListView.UseCompatibleStateImageBehavior = false;
            this.FileListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "이름";
            this.columnHeader1.Width = 175;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "크기";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "종류";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "수정한 날짜";
            this.columnHeader4.Width = 150;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "위치";
            this.columnHeader5.Width = 300;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lvFiles);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(740, 363);
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
            this.lvFiles.Location = new System.Drawing.Point(0, 44);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(740, 319);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
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
            this.chSize.Width = 100;
            // 
            // chType
            // 
            this.chType.Text = "종류";
            // 
            // chMidifiedDate
            // 
            this.chMidifiedDate.Text = "수정한 날짜";
            this.chMidifiedDate.Width = 150;
            // 
            // chDirectory
            // 
            this.chDirectory.Text = "위치";
            this.chDirectory.Width = 300;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.btnFileDel);
            this.groupBox2.Controls.Add(this.btnUpload);
            this.groupBox2.Controls.Add(this.txtPath);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(740, 44);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            // 
            // btnFileDel
            // 
            this.btnFileDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileDel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFileDel.BackgroundImage")));
            this.btnFileDel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFileDel.Location = new System.Drawing.Point(635, 12);
            this.btnFileDel.Name = "btnFileDel";
            this.btnFileDel.Size = new System.Drawing.Size(96, 25);
            this.btnFileDel.TabIndex = 25;
            this.btnFileDel.Text = "파일삭제";
            this.btnFileDel.UseVisualStyleBackColor = true;
            this.btnFileDel.Click += new System.EventHandler(this.btnFileDel_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUpload.BackgroundImage")));
            this.btnUpload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpload.Location = new System.Drawing.Point(535, 12);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(96, 25);
            this.btnUpload.TabIndex = 23;
            this.btnUpload.Text = "파일 업로드";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // txtPath
            // 
            this.txtPath.AutoSize = false;
            this.txtPath.BackColor = System.Drawing.Color.White;
            this.txtPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPath.Location = new System.Drawing.Point(14, 14);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(513, 21);
            this.txtPath.TabIndex = 22;
            this.txtPath.Tag = ";;;;";
            this.txtPath.VerticalAlign = C1.Win.C1Input.VerticalAlignEnum.Middle;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.Beige;
            this.splitter1.Location = new System.Drawing.Point(377, 64);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 687);
            this.splitter1.TabIndex = 28;
            this.splitter1.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Controls.Add(this.splitter3);
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(382, 64);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(740, 687);
            this.panel4.TabIndex = 29;
            // 
            // splitter3
            // 
            this.splitter3.BackColor = System.Drawing.Color.Beige;
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 363);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(740, 5);
            this.splitter3.TabIndex = 28;
            this.splitter3.TabStop = false;
            // 
            // ZBB030
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1122, 751);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "ZBB030";
            this.Text = "파일등록";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ZBB030_FormClosed);
            this.Load += new System.EventHandler(this.ZBB030_Load);
            this.Controls.SetChildIndex(this.panButton1, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.splitter1, 0);
            this.Controls.SetChildIndex(this.panel4, 0);
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
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDir)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
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
        private C1.Win.C1Input.C1Button btnFileDel;
        private C1.Win.C1Input.C1Button btnUpload;
        private System.Windows.Forms.TreeView DirTreeView;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView FileListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter3;
        private C1.Win.C1Input.C1Button c1Button1;
        private C1.Win.C1Input.C1TextBox txtDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private C1.Win.C1Input.C1Button c1Button2;
        private C1.Win.C1Input.C1Label c1Label4;

    }
}