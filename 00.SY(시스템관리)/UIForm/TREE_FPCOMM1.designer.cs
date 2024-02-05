namespace UIForm
{
    partial class TREE_FPCOMM1
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
			this.GridCommPanel = new System.Windows.Forms.Panel();
			this.txtRowCnt = new System.Windows.Forms.TextBox();
			this.TreeCommPanel = new System.Windows.Forms.Panel();
			this.TreeCommGroupBox = new System.Windows.Forms.GroupBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
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
			this.GridCommPanel.SuspendLayout();
			this.TreeCommPanel.SuspendLayout();
			this.TreeCommGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// panButton1
			// 
			this.panButton1.Size = new System.Drawing.Size(842, 64);
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
			this.fpSpread1.Location = new System.Drawing.Point(8, 18);
			this.fpSpread1.Name = "fpSpread1";
			this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
			this.fpSpread1.Size = new System.Drawing.Size(165, 117);
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
			this.GridCommGroupBox.Location = new System.Drawing.Point(8, 9);
			this.GridCommGroupBox.Name = "GridCommGroupBox";
			this.GridCommGroupBox.Size = new System.Drawing.Size(181, 144);
			this.GridCommGroupBox.TabIndex = 3;
			this.GridCommGroupBox.TabStop = false;
			// 
			// GridCommPanel
			// 
			this.GridCommPanel.Controls.Add(this.GridCommGroupBox);
			this.GridCommPanel.Location = new System.Drawing.Point(440, 176);
			this.GridCommPanel.Name = "GridCommPanel";
			this.GridCommPanel.Size = new System.Drawing.Size(197, 162);
			this.GridCommPanel.TabIndex = 4;
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
			// TreeCommPanel
			// 
			this.TreeCommPanel.Controls.Add(this.TreeCommGroupBox);
			this.TreeCommPanel.Location = new System.Drawing.Point(82, 118);
			this.TreeCommPanel.Name = "TreeCommPanel";
			this.TreeCommPanel.Size = new System.Drawing.Size(270, 241);
			this.TreeCommPanel.TabIndex = 18;
			// 
			// TreeCommGroupBox
			// 
			this.TreeCommGroupBox.Controls.Add(this.treeView1);
			this.TreeCommGroupBox.Location = new System.Drawing.Point(15, 13);
			this.TreeCommGroupBox.Name = "TreeCommGroupBox";
			this.TreeCommGroupBox.Size = new System.Drawing.Size(234, 211);
			this.TreeCommGroupBox.TabIndex = 19;
			this.TreeCommGroupBox.TabStop = false;
			// 
			// treeView1
			// 
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.treeView1.Font = new System.Drawing.Font("굴림", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.treeView1.Location = new System.Drawing.Point(12, 20);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(202, 175);
			this.treeView1.TabIndex = 0;
			// 
			// TREE_FPCOMM1
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(842, 434);
			this.Controls.Add(this.TreeCommPanel);
			this.Controls.Add(this.GridCommPanel);
			this.Controls.Add(this.txtRowCnt);
			this.Name = "TREE_FPCOMM1";
			this.Text = "";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.TREE_FPCOMM1_Closing);
			this.Load += new System.EventHandler(this.TREE_FPCOMM1_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TREE_FPCOMM1_KeyDown);
			this.Controls.SetChildIndex(this.panButton1, 0);
			this.Controls.SetChildIndex(this.txtRowCnt, 0);
			this.Controls.SetChildIndex(this.GridCommPanel, 0);
			this.Controls.SetChildIndex(this.TreeCommPanel, 0);
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
			this.GridCommPanel.ResumeLayout(false);
			this.TreeCommPanel.ResumeLayout(false);
			this.TreeCommGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        public System.Windows.Forms.Panel TreeCommPanel;
        public System.Windows.Forms.GroupBox TreeCommGroupBox;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
    }
}