namespace UIForm
{
    partial class Buttons
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Buttons));
            this.totBTN = new System.Windows.Forms.ToolTip(this.components);
            this.BtnDel = new System.Windows.Forms.PictureBox();
            this.BtnHelp = new System.Windows.Forms.PictureBox();
            this.BtnDelete = new System.Windows.Forms.PictureBox();
            this.BtnRowIns = new System.Windows.Forms.PictureBox();
            this.BtnRCopy = new System.Windows.Forms.PictureBox();
            this.BtnClose = new System.Windows.Forms.PictureBox();
            this.BtnPrint = new System.Windows.Forms.PictureBox();
            this.BtnCancel = new System.Windows.Forms.PictureBox();
            this.BtnSearch = new System.Windows.Forms.PictureBox();
            this.BtnInsert = new System.Windows.Forms.PictureBox();
            this.BtnNew = new System.Windows.Forms.PictureBox();
            this.BtnExcel = new System.Windows.Forms.PictureBox();
            this.panButton1 = new System.Windows.Forms.Panel();
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
            this.panButton1.SuspendLayout();
            this.SuspendLayout();
            // 
            // totBTN
            // 
            this.totBTN.AutoPopDelay = 5000;
            this.totBTN.InitialDelay = 50;
            this.totBTN.ReshowDelay = 100;
            // 
            // BtnDel
            // 
            this.BtnDel.BackgroundImage = global::UIForm.Properties.Resources.RDelete;
            this.BtnDel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDel.Location = new System.Drawing.Point(282, 8);
            this.BtnDel.Name = "BtnDel";
            this.BtnDel.Size = new System.Drawing.Size(48, 48);
            this.BtnDel.TabIndex = 20;
            this.BtnDel.TabStop = false;
            this.totBTN.SetToolTip(this.BtnDel, "행 삭제");
            this.BtnDel.Click += new System.EventHandler(this.BtnDel_Click);
            this.BtnDel.MouseEnter += new System.EventHandler(this.BtnDel_MouseEnter);
            this.BtnDel.MouseLeave += new System.EventHandler(this.BtnDel_MouseLeave);
            // 
            // BtnHelp
            // 
            this.BtnHelp.BackgroundImage = global::UIForm.Properties.Resources.Help;
            this.BtnHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnHelp.Location = new System.Drawing.Point(580, 8);
            this.BtnHelp.Name = "BtnHelp";
            this.BtnHelp.Size = new System.Drawing.Size(48, 48);
            this.BtnHelp.TabIndex = 16;
            this.BtnHelp.TabStop = false;
            this.totBTN.SetToolTip(this.BtnHelp, "도움말");
            this.BtnHelp.Click += new System.EventHandler(this.BtnHelp_Click);
            this.BtnHelp.MouseEnter += new System.EventHandler(this.BtnHelp_MouseEnter);
            this.BtnHelp.MouseLeave += new System.EventHandler(this.BtnHelp_MouseLeave);
            // 
            // BtnDelete
            // 
            this.BtnDelete.BackgroundImage = global::UIForm.Properties.Resources.Delete;
            this.BtnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDelete.Location = new System.Drawing.Point(507, 8);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(48, 48);
            this.BtnDelete.TabIndex = 15;
            this.BtnDelete.TabStop = false;
            this.totBTN.SetToolTip(this.BtnDelete, "전체 삭제");
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            this.BtnDelete.MouseEnter += new System.EventHandler(this.BtnDelete_MouseEnter);
            this.BtnDelete.MouseLeave += new System.EventHandler(this.BtnDelete_MouseLeave);
            // 
            // BtnRowIns
            // 
            this.BtnRowIns.BackgroundImage = global::UIForm.Properties.Resources.RAdd;
            this.BtnRowIns.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnRowIns.Location = new System.Drawing.Point(182, 8);
            this.BtnRowIns.Name = "BtnRowIns";
            this.BtnRowIns.Size = new System.Drawing.Size(48, 48);
            this.BtnRowIns.TabIndex = 14;
            this.BtnRowIns.TabStop = false;
            this.totBTN.SetToolTip(this.BtnRowIns, "행 추가");
            this.BtnRowIns.Click += new System.EventHandler(this.BtnRowIns_Click);
            this.BtnRowIns.MouseEnter += new System.EventHandler(this.BtnRowIns_MouseEnter);
            this.BtnRowIns.MouseLeave += new System.EventHandler(this.BtnRowIns_MouseLeave);
            // 
            // BtnRCopy
            // 
            this.BtnRCopy.BackgroundImage = global::UIForm.Properties.Resources.RCopy;
            this.BtnRCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnRCopy.Location = new System.Drawing.Point(132, 8);
            this.BtnRCopy.Name = "BtnRCopy";
            this.BtnRCopy.Size = new System.Drawing.Size(48, 48);
            this.BtnRCopy.TabIndex = 13;
            this.BtnRCopy.TabStop = false;
            this.totBTN.SetToolTip(this.BtnRCopy, "행 복사");
            this.BtnRCopy.Click += new System.EventHandler(this.BtnRCopy_Click);
            this.BtnRCopy.MouseEnter += new System.EventHandler(this.BtnRCopy_MouseEnter);
            this.BtnRCopy.MouseLeave += new System.EventHandler(this.BtnRCopy_MouseLeave);
            // 
            // BtnClose
            // 
            this.BtnClose.BackgroundImage = global::UIForm.Properties.Resources.Fcls;
            this.BtnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnClose.Location = new System.Drawing.Point(630, 8);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(48, 48);
            this.BtnClose.TabIndex = 11;
            this.BtnClose.TabStop = false;
            this.totBTN.SetToolTip(this.BtnClose, "나가기 Ctrl + E");
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            this.BtnClose.MouseEnter += new System.EventHandler(this.BtnClose_MouseEnter);
            this.BtnClose.MouseLeave += new System.EventHandler(this.BtnClose_MouseLeave);
            // 
            // BtnPrint
            // 
            this.BtnPrint.BackgroundImage = global::UIForm.Properties.Resources.Print;
            this.BtnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnPrint.Location = new System.Drawing.Point(457, 8);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.Size = new System.Drawing.Size(48, 48);
            this.BtnPrint.TabIndex = 10;
            this.BtnPrint.TabStop = false;
            this.totBTN.SetToolTip(this.BtnPrint, "출력 Ctrl + P");
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            this.BtnPrint.MouseEnter += new System.EventHandler(this.BtnPrint_MouseEnter);
            this.BtnPrint.MouseLeave += new System.EventHandler(this.BtnPrint_MouseLeave);
            // 
            // BtnCancel
            // 
            this.BtnCancel.BackgroundImage = global::UIForm.Properties.Resources.Cancel;
            this.BtnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCancel.Location = new System.Drawing.Point(232, 8);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(48, 48);
            this.BtnCancel.TabIndex = 9;
            this.BtnCancel.TabStop = false;
            this.totBTN.SetToolTip(this.BtnCancel, "행 취소");
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            this.BtnCancel.MouseEnter += new System.EventHandler(this.BtnCancel_MouseEnter);
            this.BtnCancel.MouseLeave += new System.EventHandler(this.BtnCancel_MouseLeave);
            // 
            // BtnSearch
            // 
            this.BtnSearch.BackgroundImage = global::UIForm.Properties.Resources.Search;
            this.BtnSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSearch.Location = new System.Drawing.Point(58, 8);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(48, 48);
            this.BtnSearch.TabIndex = 8;
            this.BtnSearch.TabStop = false;
            this.totBTN.SetToolTip(this.BtnSearch, "조회 Ctrl + Enter");
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            this.BtnSearch.MouseEnter += new System.EventHandler(this.BtnSearch_MouseEnter);
            this.BtnSearch.MouseLeave += new System.EventHandler(this.BtnSearch_MouseLeave);
            // 
            // BtnInsert
            // 
            this.BtnInsert.BackgroundImage = global::UIForm.Properties.Resources.Save;
            this.BtnInsert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnInsert.Location = new System.Drawing.Point(357, 8);
            this.BtnInsert.Name = "BtnInsert";
            this.BtnInsert.Size = new System.Drawing.Size(48, 48);
            this.BtnInsert.TabIndex = 7;
            this.BtnInsert.TabStop = false;
            this.totBTN.SetToolTip(this.BtnInsert, "저장 Ctrl + S");
            this.BtnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            this.BtnInsert.MouseEnter += new System.EventHandler(this.BtnInsert_MouseEnter);
            this.BtnInsert.MouseLeave += new System.EventHandler(this.BtnInsert_MouseLeave);
            // 
            // BtnNew
            // 
            this.BtnNew.AccessibleDescription = "";
            this.BtnNew.AccessibleName = "";
            this.BtnNew.BackgroundImage = global::UIForm.Properties.Resources.New;
            this.BtnNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnNew.ErrorImage = null;
            this.BtnNew.InitialImage = null;
            this.BtnNew.Location = new System.Drawing.Point(8, 8);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.Size = new System.Drawing.Size(48, 48);
            this.BtnNew.TabIndex = 6;
            this.BtnNew.TabStop = false;
            this.BtnNew.Tag = "";
            this.totBTN.SetToolTip(this.BtnNew, "초기화 Ctrl + N");
            this.BtnNew.Click += new System.EventHandler(this.BtnNew_Click);
            this.BtnNew.MouseEnter += new System.EventHandler(this.BtnNew_MouseEnter);
            this.BtnNew.MouseLeave += new System.EventHandler(this.BtnNew_MouseLeave);
            // 
            // BtnExcel
            // 
            this.BtnExcel.BackgroundImage = global::UIForm.Properties.Resources.Excel;
            this.BtnExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnExcel.Location = new System.Drawing.Point(407, 8);
            this.BtnExcel.Name = "BtnExcel";
            this.BtnExcel.Size = new System.Drawing.Size(48, 48);
            this.BtnExcel.TabIndex = 12;
            this.BtnExcel.TabStop = false;
            this.totBTN.SetToolTip(this.BtnExcel, "엑셀");
            this.BtnExcel.Click += new System.EventHandler(this.BtnExcel_Click);
            this.BtnExcel.MouseEnter += new System.EventHandler(this.BtnExcel_MouseEnter);
            this.BtnExcel.MouseLeave += new System.EventHandler(this.BtnExcel_MouseLeave);
            // 
            // panButton1
            // 
            this.panButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.panButton1.Controls.Add(this.BtnDel);
            this.panButton1.Controls.Add(this.BtnHelp);
            this.panButton1.Controls.Add(this.BtnDelete);
            this.panButton1.Controls.Add(this.BtnRowIns);
            this.panButton1.Controls.Add(this.BtnRCopy);
            this.panButton1.Controls.Add(this.BtnClose);
            this.panButton1.Controls.Add(this.BtnPrint);
            this.panButton1.Controls.Add(this.BtnCancel);
            this.panButton1.Controls.Add(this.BtnSearch);
            this.panButton1.Controls.Add(this.BtnInsert);
            this.panButton1.Controls.Add(this.BtnNew);
            this.panButton1.Controls.Add(this.BtnExcel);
            this.panButton1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panButton1.Location = new System.Drawing.Point(0, 0);
            this.panButton1.Name = "panButton1";
            this.panButton1.Size = new System.Drawing.Size(792, 64);
            this.panButton1.TabIndex = 7;
            // 
            // Buttons
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(792, 265);
            this.Controls.Add(this.panButton1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Buttons";
            this.Text = "Buttons";
            this.Activated += new System.EventHandler(this.Buttons_Activated);
            this.Load += new System.EventHandler(this.Buttons_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Buttons_KeyDown);
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
            this.panButton1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        public System.Windows.Forms.Panel panButton1;
        public System.Windows.Forms.PictureBox BtnDel;
        public System.Windows.Forms.PictureBox BtnHelp;
        public System.Windows.Forms.PictureBox BtnDelete;
        public System.Windows.Forms.PictureBox BtnRowIns;
        public System.Windows.Forms.PictureBox BtnRCopy;
        public System.Windows.Forms.PictureBox BtnClose;
        public System.Windows.Forms.PictureBox BtnPrint;
        public System.Windows.Forms.PictureBox BtnCancel;
        public System.Windows.Forms.PictureBox BtnSearch;
        public System.Windows.Forms.PictureBox BtnInsert;
        public System.Windows.Forms.PictureBox BtnNew;
        public System.Windows.Forms.PictureBox BtnExcel;

    }
}