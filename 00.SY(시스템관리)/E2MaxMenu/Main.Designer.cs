namespace E2MAXMenu
{
    partial class Main
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
        #region Windows Form 디자이너에서 생성한 코드
        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel5 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel4 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.c1CommandDock1 = new C1.Win.C1Command.C1CommandDock();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabForms = new C1.Win.C1Command.C1DockingTab();
            this.c1CommandHolder1 = new C1.Win.C1Command.C1CommandHolder();
            this.c1MainMenu1 = new C1.Win.C1Command.C1MainMenu();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRefresh1 = new System.Windows.Forms.PictureBox();
            this.totBtnRefresh = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandDock1)).BeginInit();
            this.c1CommandDock1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabForms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 849);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel5,
            this.statusBarPanel4,
            this.statusBarPanel3});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(1260, 25);
            this.statusBar1.SizingGrip = false;
            this.statusBar1.TabIndex = 1;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 230;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Width = 640;
            // 
            // statusBarPanel5
            // 
            this.statusBarPanel5.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPanel5.Name = "statusBarPanel5";
            this.statusBarPanel5.Width = 150;
            // 
            // statusBarPanel4
            // 
            this.statusBarPanel4.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPanel4.Name = "statusBarPanel4";
            this.statusBarPanel4.Width = 120;
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusBarPanel3.Name = "statusBarPanel3";
            this.statusBarPanel3.Width = 120;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "step2_close.ico");
            this.imageList1.Images.SetKeyName(1, "step2_open.ico");
            this.imageList1.Images.SetKeyName(2, "step3_file.ico");
            this.imageList1.Images.SetKeyName(3, "공통기준관리.ico");
            this.imageList1.Images.SetKeyName(4, "품목관리.ico");
            this.imageList1.Images.SetKeyName(5, "영업관리.ico");
            this.imageList1.Images.SetKeyName(6, "생산관리.ico");
            this.imageList1.Images.SetKeyName(7, "구매관리.ico");
            this.imageList1.Images.SetKeyName(8, "재고관리.ico");
            this.imageList1.Images.SetKeyName(9, "품질관리.ico");
            this.imageList1.Images.SetKeyName(10, "사업관리.ico");
            this.imageList1.Images.SetKeyName(11, "EIS.ico");
            this.imageList1.Images.SetKeyName(12, "재무관리.ico");
            this.imageList1.Images.SetKeyName(13, "인사급여관리.ico");
            this.imageList1.Images.SetKeyName(14, "인터페이스.ico");
            this.imageList1.Images.SetKeyName(15, "회계관리.png");
            this.imageList1.Images.SetKeyName(16, "보세관리.ico");
            this.imageList1.Images.SetKeyName(17, "국방통합원가.ico");
            this.imageList1.Images.SetKeyName(18, "SCM.ico");
            this.imageList1.Images.SetKeyName(19, "문서관리.ICO");
            // 
            // c1CommandDock1
            // 
            this.c1CommandDock1.Controls.Add(this.c1DockingTab1);
            this.c1CommandDock1.Dock = System.Windows.Forms.DockStyle.Left;
            this.c1CommandDock1.Id = 1;
            this.c1CommandDock1.Location = new System.Drawing.Point(0, 68);
            this.c1CommandDock1.Name = "c1CommandDock1";
            this.c1CommandDock1.Size = new System.Drawing.Size(231, 781);
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.c1DockingTab1.CanAutoHide = true;
            this.c1DockingTab1.CanMoveTabs = true;
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.ShowCaption = true;
            this.c1DockingTab1.Size = new System.Drawing.Size(231, 781);
            this.c1DockingTab1.TabAreaBackColor = System.Drawing.Color.White;
            this.c1DockingTab1.TabIndex = 0;
            this.c1DockingTab1.TabSizeMode = C1.Win.C1Command.TabSizeModeEnum.Fit;
            this.c1DockingTab1.TabsSpacing = -1;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Rounded;
            this.c1DockingTab1.VisualStyle = C1.Win.C1Command.VisualStyle.Custom;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.c1DockingTabPage1.CaptionVisible = true;
            this.c1DockingTabPage1.Controls.Add(this.treeView1);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 1);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(226, 757);
            this.c1DockingTabPage1.TabBackColor = System.Drawing.Color.WhiteSmoke;
            this.c1DockingTabPage1.TabBackColorSelected = System.Drawing.Color.WhiteSmoke;
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.Text = "E2Max-MTMS Menu";
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 2;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.ItemHeight = 18;
            this.treeView1.Location = new System.Drawing.Point(0, 19);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 2;
            this.treeView1.Size = new System.Drawing.Size(226, 738);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // tabForms
            // 
            this.tabForms.BackColor = System.Drawing.SystemColors.Window;
            this.tabForms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tabForms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabForms.Location = new System.Drawing.Point(231, 68);
            this.tabForms.Name = "tabForms";
            this.tabForms.Padding = new System.Drawing.Point(10, 3);
            this.tabForms.Size = new System.Drawing.Size(1029, 29);
            this.tabForms.TabAreaBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(201)))), ((int)(((byte)(212)))));
            this.tabForms.TabIndex = 6;
            this.tabForms.TabsSpacing = -1;
            this.tabForms.TabStyle = C1.Win.C1Command.TabStyleEnum.Rounded;
            this.tabForms.Visible = false;
            this.tabForms.VisualStyle = C1.Win.C1Command.VisualStyle.Custom;
            this.tabForms.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
            this.tabForms.SelectedIndexChanged += new System.EventHandler(this.tabForms_SelectedIndexChanged);
            this.tabForms.DoubleClick += new System.EventHandler(this.tabForms_DoubleClick);
            // 
            // c1CommandHolder1
            // 
            this.c1CommandHolder1.Owner = this;
            // 
            // c1MainMenu1
            // 
            this.c1MainMenu1.AccessibleName = "Menu Bar";
            this.c1MainMenu1.BackColor = System.Drawing.SystemColors.Control;
            this.c1MainMenu1.CommandHolder = this.c1CommandHolder1;
            this.c1MainMenu1.Dock = System.Windows.Forms.DockStyle.Top;
            this.c1MainMenu1.Font = new System.Drawing.Font("돋움", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
            this.c1MainMenu1.Location = new System.Drawing.Point(0, 0);
            this.c1MainMenu1.Name = "c1MainMenu1";
            this.c1MainMenu1.Size = new System.Drawing.Size(1260, 23);
            this.c1MainMenu1.Visible = false;
            this.c1MainMenu1.VisualStyle = C1.Win.C1Command.VisualStyle.Custom;
            this.c1MainMenu1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.btnRefresh1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 45);
            this.panel1.TabIndex = 10;
            // 
            // btnRefresh1
            // 
            this.btnRefresh1.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh1.Location = new System.Drawing.Point(11, 3);
            this.btnRefresh1.Name = "btnRefresh1";
            this.btnRefresh1.Size = new System.Drawing.Size(182, 41);
            this.btnRefresh1.TabIndex = 0;
            this.btnRefresh1.TabStop = false;
            this.totBtnRefresh.SetToolTip(this.btnRefresh1, "새로고침");
            this.btnRefresh1.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1260, 874);
            this.Controls.Add(this.tabForms);
            this.Controls.Add(this.c1CommandDock1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.c1MainMenu1);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "Main";
            this.Text = "E2Max-MTMS";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MdiChildActivate += new System.EventHandler(this.Form1_MdiChildActivate);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandDock1)).EndInit();
            this.c1CommandDock1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabForms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnRefresh1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.StatusBarPanel statusBarPanel1;

        #endregion
        private System.Windows.Forms.StatusBarPanel statusBarPanel2;
        private System.Windows.Forms.StatusBarPanel statusBarPanel3;
        private System.Windows.Forms.StatusBarPanel statusBarPanel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox btnRefresh1;
        private System.Windows.Forms.ToolTip totBtnRefresh;
    }
}