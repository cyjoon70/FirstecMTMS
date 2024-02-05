namespace UIForm
{
    partial class FPCOMM1T
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
        private void InitializeComponent()
        {
			this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
			this.ctmGrid1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
			this.GridCommGroupBox = new System.Windows.Forms.GroupBox();
			this.TabCommPanel = new System.Windows.Forms.Panel();
			this.c1TabCommon = new C1.Win.C1Command.C1DockingTab();
			this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
			this.GridCommPanel = new System.Windows.Forms.Panel();
			this.txtRowCnt = new System.Windows.Forms.TextBox();
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
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
			this.GridCommGroupBox.SuspendLayout();
			this.TabCommPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.c1TabCommon)).BeginInit();
			this.c1TabCommon.SuspendLayout();
			this.c1DockingTabPage1.SuspendLayout();
			this.GridCommPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panButton1
			// 
			this.panButton1.Size = new System.Drawing.Size(824, 64);
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
			// ctmGrid1
			// 
			this.ctmGrid1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem3,
            this.menuItem7,
            this.menuItem4,
            this.menuItem5,
            this.menuItem6});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "그리드 넓이 초기화";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "SORT";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "행추가";
			this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 3;
			this.menuItem7.Text = "찾기 ( Ctrl + F )";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 4;
			this.menuItem4.Text = "-";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 5;
			this.menuItem5.Text = "Excel Export";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 6;
			this.menuItem6.Text = "Grid Print";
			this.menuItem6.Click += new System.EventHandler(this.menuItem6_Click);
			// 
			// fpSpread1_Sheet1
			// 
			this.fpSpread1_Sheet1.Reset();
			this.fpSpread1_Sheet1.SheetName = "Sheet1";
			// 
			// GridCommGroupBox
			// 
			this.GridCommGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.GridCommGroupBox.Controls.Add(this.fpSpread1);
			this.GridCommGroupBox.Location = new System.Drawing.Point(21, 12);
			this.GridCommGroupBox.Name = "GridCommGroupBox";
			this.GridCommGroupBox.Size = new System.Drawing.Size(237, 142);
			this.GridCommGroupBox.TabIndex = 3;
			this.GridCommGroupBox.TabStop = false;
			// 
			// TabCommPanel
			// 
			this.TabCommPanel.Controls.Add(this.c1TabCommon);
			this.TabCommPanel.Location = new System.Drawing.Point(407, 86);
			this.TabCommPanel.Name = "TabCommPanel";
			this.TabCommPanel.Size = new System.Drawing.Size(355, 255);
			this.TabCommPanel.TabIndex = 4;
			// 
			// c1TabCommon
			// 
			this.c1TabCommon.BackColor = System.Drawing.Color.White;
			this.c1TabCommon.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.c1TabCommon.Controls.Add(this.c1DockingTabPage1);
			this.c1TabCommon.Location = new System.Drawing.Point(15, 15);
			this.c1TabCommon.Name = "c1TabCommon";
			this.c1TabCommon.Size = new System.Drawing.Size(327, 228);
			this.c1TabCommon.TabIndex = 0;
			this.c1TabCommon.TabsSpacing = -1;
			this.c1TabCommon.TabStyle = C1.Win.C1Command.TabStyleEnum.Rounded;
			this.c1TabCommon.VisualStyle = C1.Win.C1Command.VisualStyle.Custom;
			this.c1TabCommon.VisualStyleBase = C1.Win.C1Command.VisualStyle.OfficeXP;
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
			this.GridCommPanel.Controls.Add(this.GridCommGroupBox);
			this.GridCommPanel.Location = new System.Drawing.Point(25, 20);
			this.GridCommPanel.Name = "GridCommPanel";
			this.GridCommPanel.Size = new System.Drawing.Size(287, 168);
			this.GridCommPanel.TabIndex = 0;
			// 
			// txtRowCnt
			// 
			this.txtRowCnt.Location = new System.Drawing.Point(784, 101);
			this.txtRowCnt.Name = "txtRowCnt";
			this.txtRowCnt.Size = new System.Drawing.Size(24, 21);
			this.txtRowCnt.TabIndex = 17;
			this.txtRowCnt.Text = "textBox1";
			this.txtRowCnt.Visible = false;
			// 
			// FPCOMM1T
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(824, 365);
			this.Controls.Add(this.TabCommPanel);
			this.Controls.Add(this.txtRowCnt);
			this.Name = "FPCOMM1T";
			this.Text = "";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FPCOMM1T_Closing);
			this.Load += new System.EventHandler(this.FPCOMM1T_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FPCOMM1_KeyDown);
			this.Controls.SetChildIndex(this.panButton1, 0);
			this.Controls.SetChildIndex(this.txtRowCnt, 0);
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
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
			this.GridCommGroupBox.ResumeLayout(false);
			this.TabCommPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.c1TabCommon)).EndInit();
			this.c1TabCommon.ResumeLayout(false);
			this.c1DockingTabPage1.ResumeLayout(false);
			this.GridCommPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
    }
}