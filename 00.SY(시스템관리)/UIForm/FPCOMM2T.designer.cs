namespace UIForm
{
    partial class FPCOMM2T
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
			this.ctmGrid2 = new System.Windows.Forms.ContextMenu();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.ctmGrid1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.txtRowCnt = new System.Windows.Forms.TextBox();
			this.GridCommPanel2 = new System.Windows.Forms.Panel();
			this.GridCommGroupBox2 = new System.Windows.Forms.GroupBox();
			this.fpSpread2 = new FarPoint.Win.Spread.FpSpread();
			this.fpSpread2_Sheet1 = new FarPoint.Win.Spread.SheetView();
			this.TabCommPanel = new System.Windows.Forms.Panel();
			this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
			this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
			this.GridCommPanel = new System.Windows.Forms.Panel();
			this.GridCommGroupBox1 = new System.Windows.Forms.GroupBox();
			this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
			this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
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
			this.GridCommPanel2.SuspendLayout();
			this.GridCommGroupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).BeginInit();
			this.TabCommPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
			this.c1DockingTab1.SuspendLayout();
			this.c1DockingTabPage1.SuspendLayout();
			this.GridCommPanel.SuspendLayout();
			this.GridCommGroupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
			this.SuspendLayout();
			// 
			// panButton1
			// 
			this.panButton1.Size = new System.Drawing.Size(922, 64);
			// 
			// ctmGrid2
			// 
			this.ctmGrid2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem4,
            this.menuItem6,
            this.menuItem14,
            this.menuItem7,
            this.menuItem8,
            this.menuItem12});
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "그리드 넓이 초기화";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "SORT";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 2;
			this.menuItem6.Text = "행추가";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 3;
			this.menuItem14.Text = "찾기 ( Ctrl + F )";
			this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 4;
			this.menuItem7.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 5;
			this.menuItem8.Text = "Excel Export";
			this.menuItem8.Click += new System.EventHandler(this.menuItem8_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 6;
			this.menuItem12.Text = "Grid Print";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// ctmGrid1
			// 
			this.ctmGrid1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem3,
            this.menuItem5,
            this.menuItem13,
            this.menuItem9,
            this.menuItem10,
            this.menuItem11});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "그리드 넓이 초기화";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "SORT";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "행추가";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 3;
			this.menuItem13.Text = "찾기 ( Ctrl + F )";
			this.menuItem13.Click += new System.EventHandler(this.menuItem13_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 4;
			this.menuItem9.Text = "-";
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 5;
			this.menuItem10.Text = "Excel Export";
			this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 6;
			this.menuItem11.Text = "Grid Print";
			this.menuItem11.Click += new System.EventHandler(this.menuItem11_Click);
			// 
			// txtRowCnt
			// 
			this.txtRowCnt.Location = new System.Drawing.Point(752, 91);
			this.txtRowCnt.Name = "txtRowCnt";
			this.txtRowCnt.Size = new System.Drawing.Size(24, 21);
			this.txtRowCnt.TabIndex = 17;
			this.txtRowCnt.Text = "textBox1";
			this.txtRowCnt.Visible = false;
			// 
			// GridCommPanel2
			// 
			this.GridCommPanel2.Controls.Add(this.GridCommGroupBox2);
			this.GridCommPanel2.Location = new System.Drawing.Point(80, 128);
			this.GridCommPanel2.Name = "GridCommPanel2";
			this.GridCommPanel2.Size = new System.Drawing.Size(240, 202);
			this.GridCommPanel2.TabIndex = 33;
			// 
			// GridCommGroupBox2
			// 
			this.GridCommGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.GridCommGroupBox2.Controls.Add(this.fpSpread2);
			this.GridCommGroupBox2.Location = new System.Drawing.Point(8, 9);
			this.GridCommGroupBox2.Name = "GridCommGroupBox2";
			this.GridCommGroupBox2.Size = new System.Drawing.Size(224, 183);
			this.GridCommGroupBox2.TabIndex = 3;
			this.GridCommGroupBox2.TabStop = false;
			// 
			// fpSpread2
			// 
			this.fpSpread2.AccessibleDescription = "fpSpread2";
			this.fpSpread2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fpSpread2.AutoClipboard = false;
			this.fpSpread2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fpSpread2.ContextMenu = this.ctmGrid2;
			this.fpSpread2.Location = new System.Drawing.Point(8, 18);
			this.fpSpread2.Name = "fpSpread2";
			this.fpSpread2.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread2_Sheet1});
			this.fpSpread2.Size = new System.Drawing.Size(208, 156);
			this.fpSpread2.TabIndex = 2;
			this.fpSpread2.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(this.fpSpread2_ColumnWidthChanged);
			this.fpSpread2.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpreads_CellClick);
			this.fpSpread2.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpreads_CellClick);
			this.fpSpread2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread2_KeyDown);
			// 
			// fpSpread2_Sheet1
			// 
			this.fpSpread2_Sheet1.Reset();
			this.fpSpread2_Sheet1.SheetName = "Sheet1";
			// 
			// TabCommPanel
			// 
			this.TabCommPanel.Controls.Add(this.c1DockingTab1);
			this.TabCommPanel.Location = new System.Drawing.Point(407, 118);
			this.TabCommPanel.Name = "TabCommPanel";
			this.TabCommPanel.Size = new System.Drawing.Size(355, 255);
			this.TabCommPanel.TabIndex = 34;
			// 
			// c1DockingTab1
			// 
			this.c1DockingTab1.BackColor = System.Drawing.Color.White;
			this.c1DockingTab1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
			this.c1DockingTab1.Location = new System.Drawing.Point(15, 15);
			this.c1DockingTab1.Name = "c1DockingTab1";
			this.c1DockingTab1.Size = new System.Drawing.Size(327, 228);
			this.c1DockingTab1.TabIndex = 0;
			this.c1DockingTab1.TabsSpacing = -1;
			this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Rounded;
			this.c1DockingTab1.VisualStyle = C1.Win.C1Command.VisualStyle.Custom;
			this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
			// 
			// c1DockingTabPage1
			// 
			this.c1DockingTabPage1.Controls.Add(this.GridCommPanel);
			this.c1DockingTabPage1.Location = new System.Drawing.Point(0, 23);
			this.c1DockingTabPage1.Name = "c1DockingTabPage1";
			this.c1DockingTabPage1.Size = new System.Drawing.Size(327, 205);
			this.c1DockingTabPage1.TabIndex = 0;
			this.c1DockingTabPage1.Text = "Page1";
			// 
			// GridCommPanel
			// 
			this.GridCommPanel.Controls.Add(this.GridCommGroupBox1);
			this.GridCommPanel.Location = new System.Drawing.Point(25, 20);
			this.GridCommPanel.Name = "GridCommPanel";
			this.GridCommPanel.Size = new System.Drawing.Size(287, 168);
			this.GridCommPanel.TabIndex = 0;
			// 
			// GridCommGroupBox1
			// 
			this.GridCommGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.GridCommGroupBox1.Controls.Add(this.fpSpread1);
			this.GridCommGroupBox1.Location = new System.Drawing.Point(21, 12);
			this.GridCommGroupBox1.Name = "GridCommGroupBox1";
			this.GridCommGroupBox1.Size = new System.Drawing.Size(237, 142);
			this.GridCommGroupBox1.TabIndex = 3;
			this.GridCommGroupBox1.TabStop = false;
			// 
			// fpSpread1
			// 
			this.fpSpread1.AccessibleDescription = "fpSpread1";
			this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fpSpread1.AutoClipboard = false;
			this.fpSpread1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fpSpread1.ContextMenu = this.ctmGrid1;
			this.fpSpread1.Location = new System.Drawing.Point(6, 20);
			this.fpSpread1.Name = "fpSpread1";
			this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
			this.fpSpread1.Size = new System.Drawing.Size(225, 103);
			this.fpSpread1.TabIndex = 2;
			this.fpSpread1.ColumnWidthChanged += new FarPoint.Win.Spread.ColumnWidthChangedEventHandler(this.fpSpread1_ColumnWidthChanged);
			this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
			this.fpSpread1.CellClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpreads_CellClick);
			this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpreads_CellClick);
			this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
			this.fpSpread1.ComboCloseUp += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ComboCloseUp);
			this.fpSpread1.EditChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_EditChange);
			this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
			// 
			// fpSpread1_Sheet1
			// 
			this.fpSpread1_Sheet1.Reset();
			this.fpSpread1_Sheet1.SheetName = "Sheet1";
			// 
			// FPCOMM2T
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(922, 412);
			this.Controls.Add(this.TabCommPanel);
			this.Controls.Add(this.GridCommPanel2);
			this.Controls.Add(this.txtRowCnt);
			this.Name = "FPCOMM2T";
			this.Text = "";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FPCOMM2T_Closing);
			this.Load += new System.EventHandler(this.FPCOMM2T_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FPCOMM2T_KeyDown);
			this.Controls.SetChildIndex(this.txtRowCnt, 0);
			this.Controls.SetChildIndex(this.GridCommPanel2, 0);
			this.Controls.SetChildIndex(this.panButton1, 0);
			this.Controls.SetChildIndex(this.TabCommPanel, 0);
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
			this.GridCommPanel2.ResumeLayout(false);
			this.GridCommGroupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fpSpread2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread2_Sheet1)).EndInit();
			this.TabCommPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
			this.c1DockingTab1.ResumeLayout(false);
			this.c1DockingTabPage1.ResumeLayout(false);
			this.GridCommPanel.ResumeLayout(false);
			this.GridCommGroupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem14;
    }
}