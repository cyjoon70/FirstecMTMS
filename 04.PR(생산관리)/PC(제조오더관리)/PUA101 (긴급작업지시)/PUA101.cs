using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Reflection;

namespace PUA101
{
	public class PUA101 : UIForm.FPCOMM1 
	{
		int NewFlg				= 0;

		string PROJECT_NO		= "";
		string PROJECT_SEQ		= "";
		string GROUP_CD			= "";

		public static string PROC_ID   = "";
		public static string PROC_TYPE = "E"; // 긴급작지

		#region 디자인

		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.Panel pnlSN;
		private System.Windows.Forms.ComboBox cboSTATUS;
		private System.Windows.Forms.Button btnPROJECT;
		private System.Windows.Forms.TextBox txtProj_Nm;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtProj_No;
		private System.Windows.Forms.GroupBox gbxITEM_MASTER;
		private System.Windows.Forms.Button btnCONF;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.DateTimePicker dtpDLV_FR_DT;
		private System.Windows.Forms.DateTimePicker dtpDLV_TO_DT;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboSCH_ID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtMakeOrderFr;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtWorkOrderFr;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtMakeOrderTo;
		private System.Windows.Forms.TextBox txtWorkOrderTo;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnItem;
		private System.Windows.Forms.TextBox txtItemNm;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtItemCd;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button btnMakeOrderFr;
		private System.Windows.Forms.Button btnWorkOrderFr;
		private System.Windows.Forms.Button btnMakeOrderTo;
		private System.Windows.Forms.Button btnWorkOrderTo;
		private System.Windows.Forms.TextBox txtProj_Seq_Fr;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtProj_Seq_To;
		private System.ComponentModel.IContainer components;
		#endregion

		#region InitializeComponent Dispose
		public PUA101()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form 디자이너에서 생성한 코드
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PUA101));
			this.pnlSN = new System.Windows.Forms.Panel();
			this.gbxITEM_MASTER = new System.Windows.Forms.GroupBox();
			this.txtProj_Seq_To = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.btnItem = new System.Windows.Forms.Button();
			this.txtItemNm = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtItemCd = new System.Windows.Forms.TextBox();
			this.btnWorkOrderTo = new System.Windows.Forms.Button();
			this.btnMakeOrderTo = new System.Windows.Forms.Button();
			this.btnWorkOrderFr = new System.Windows.Forms.Button();
			this.btnMakeOrderFr = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.txtWorkOrderTo = new System.Windows.Forms.TextBox();
			this.txtMakeOrderTo = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txtWorkOrderFr = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtMakeOrderFr = new System.Windows.Forms.TextBox();
			this.cboSCH_ID = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.dtpDLV_TO_DT = new System.Windows.Forms.DateTimePicker();
			this.dtpDLV_FR_DT = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCONF = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cboSTATUS = new System.Windows.Forms.ComboBox();
			this.btnPROJECT = new System.Windows.Forms.Button();
			this.txtProj_Seq_Fr = new System.Windows.Forms.TextBox();
			this.txtProj_Nm = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtProj_No = new System.Windows.Forms.TextBox();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
			this.pnlSN.SuspendLayout();
			this.gbxITEM_MASTER.SuspendLayout();
			this.SuspendLayout();
			// 
			// GridCommGroupBox
			// 
			this.GridCommGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GridCommGroupBox.Location = new System.Drawing.Point(0, 0);
			this.GridCommGroupBox.Name = "GridCommGroupBox";
			this.GridCommGroupBox.Size = new System.Drawing.Size(920, 309);
			// 
			// GridCommPanel
			// 
			this.GridCommPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GridCommPanel.Location = new System.Drawing.Point(0, 192);
			this.GridCommPanel.Name = "GridCommPanel";
			this.GridCommPanel.Size = new System.Drawing.Size(920, 309);
			// 
			// fpSpread1
			// 
			this.fpSpread1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fpSpread1.Location = new System.Drawing.Point(3, 17);
			this.fpSpread1.Name = "fpSpread1";
			this.fpSpread1.Size = new System.Drawing.Size(914, 289);
			this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
			this.fpSpread1.ComboSelChange += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ComboSelChange);
			this.fpSpread1_Sheet1.Reset();
			this.fpSpread1_Sheet1.SheetName = "Sheet1";
			// Formulas and custom names must be loaded with R1C1 reference style
			this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
			this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
			// 
			// BtnNew
			// 
			this.BtnNew.Name = "BtnNew";
			// 
			// BtnClose
			// 
			this.BtnClose.Name = "BtnClose";
			// 
			// BtnPrint
			// 
			this.BtnPrint.Name = "BtnPrint";
			// 
			// BtnDel
			// 
			this.BtnDel.Name = "BtnDel";
			// 
			// BtnSearch
			// 
			this.BtnSearch.Name = "BtnSearch";
			// 
			// BtnInsert
			// 
			this.BtnInsert.Name = "BtnInsert";
			// 
			// BtnExcel
			// 
			this.BtnExcel.Name = "BtnExcel";
			// 
			// BtnRowIns
			// 
			this.BtnRowIns.Name = "BtnRowIns";
			// 
			// BtnRCopy
			// 
			this.BtnRCopy.Name = "BtnRCopy";
			// 
			// BtnCancel
			// 
			this.BtnCancel.Name = "BtnCancel";
			// 
			// BtnDelete
			// 
			this.BtnDelete.Name = "BtnDelete";
			// 
			// BtnHelp
			// 
			this.BtnHelp.Name = "BtnHelp";
			// 
			// panButton2
			// 
			this.panButton2.Name = "panButton2";
			this.panButton2.Size = new System.Drawing.Size(920, 24);
			// 
			// panButton1
			// 
			this.panButton1.Name = "panButton1";
			this.panButton1.Size = new System.Drawing.Size(920, 47);
			// 
			// panButton3
			// 
			this.panButton3.Name = "panButton3";
			this.panButton3.Size = new System.Drawing.Size(920, 1);
			// 
			// panButton5
			// 
			this.panButton5.Name = "panButton5";
			this.panButton5.Size = new System.Drawing.Size(920, 72);
			// 
			// lblFormName
			// 
			this.lblFormName.Name = "lblFormName";
			// 
			// panButton4
			// 
			this.panButton4.Name = "panButton4";
			this.panButton4.Size = new System.Drawing.Size(920, 1);
			// 
			// panButton6
			// 
			this.panButton6.Name = "panButton6";
			this.panButton6.Size = new System.Drawing.Size(632, 23);
			// 
			// panButton7
			// 
			this.panButton7.Name = "panButton7";
			// 
			// lnkJump1
			// 
			this.lnkJump1.Location = new System.Drawing.Point(640, 8);
			this.lnkJump1.Name = "lnkJump1";
			// 
			// lnkJump2
			// 
			this.lnkJump2.Location = new System.Drawing.Point(568, 8);
			this.lnkJump2.Name = "lnkJump2";
			// 
			// lnkJump3
			// 
			this.lnkJump3.Location = new System.Drawing.Point(496, 8);
			this.lnkJump3.Name = "lnkJump3";
			// 
			// lnkJump4
			// 
			this.lnkJump4.Location = new System.Drawing.Point(424, 8);
			this.lnkJump4.Name = "lnkJump4";
			// 
			// lnkJump5
			// 
			this.lnkJump5.Location = new System.Drawing.Point(360, 8);
			this.lnkJump5.Name = "lnkJump5";
			// 
			// lnkJump6
			// 
			this.lnkJump6.Location = new System.Drawing.Point(288, 8);
			this.lnkJump6.Name = "lnkJump6";
			// 
			// pnlSN
			// 
			this.pnlSN.Controls.Add(this.gbxITEM_MASTER);
			this.pnlSN.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlSN.Location = new System.Drawing.Point(0, 72);
			this.pnlSN.Name = "pnlSN";
			this.pnlSN.Size = new System.Drawing.Size(920, 120);
			this.pnlSN.TabIndex = 9;
			// 
			// gbxITEM_MASTER
			// 
			this.gbxITEM_MASTER.BackColor = System.Drawing.Color.WhiteSmoke;
			this.gbxITEM_MASTER.Controls.Add(this.txtProj_Seq_To);
			this.gbxITEM_MASTER.Controls.Add(this.label11);
			this.gbxITEM_MASTER.Controls.Add(this.label10);
			this.gbxITEM_MASTER.Controls.Add(this.btnItem);
			this.gbxITEM_MASTER.Controls.Add(this.txtItemNm);
			this.gbxITEM_MASTER.Controls.Add(this.label9);
			this.gbxITEM_MASTER.Controls.Add(this.txtItemCd);
			this.gbxITEM_MASTER.Controls.Add(this.btnWorkOrderTo);
			this.gbxITEM_MASTER.Controls.Add(this.btnMakeOrderTo);
			this.gbxITEM_MASTER.Controls.Add(this.btnWorkOrderFr);
			this.gbxITEM_MASTER.Controls.Add(this.btnMakeOrderFr);
			this.gbxITEM_MASTER.Controls.Add(this.label8);
			this.gbxITEM_MASTER.Controls.Add(this.txtWorkOrderTo);
			this.gbxITEM_MASTER.Controls.Add(this.txtMakeOrderTo);
			this.gbxITEM_MASTER.Controls.Add(this.label7);
			this.gbxITEM_MASTER.Controls.Add(this.label6);
			this.gbxITEM_MASTER.Controls.Add(this.txtWorkOrderFr);
			this.gbxITEM_MASTER.Controls.Add(this.label5);
			this.gbxITEM_MASTER.Controls.Add(this.txtMakeOrderFr);
			this.gbxITEM_MASTER.Controls.Add(this.cboSCH_ID);
			this.gbxITEM_MASTER.Controls.Add(this.label4);
			this.gbxITEM_MASTER.Controls.Add(this.label34);
			this.gbxITEM_MASTER.Controls.Add(this.dtpDLV_TO_DT);
			this.gbxITEM_MASTER.Controls.Add(this.dtpDLV_FR_DT);
			this.gbxITEM_MASTER.Controls.Add(this.label1);
			this.gbxITEM_MASTER.Controls.Add(this.btnCONF);
			this.gbxITEM_MASTER.Controls.Add(this.label3);
			this.gbxITEM_MASTER.Controls.Add(this.cboSTATUS);
			this.gbxITEM_MASTER.Controls.Add(this.btnPROJECT);
			this.gbxITEM_MASTER.Controls.Add(this.txtProj_Seq_Fr);
			this.gbxITEM_MASTER.Controls.Add(this.txtProj_Nm);
			this.gbxITEM_MASTER.Controls.Add(this.label2);
			this.gbxITEM_MASTER.Controls.Add(this.txtProj_No);
			this.gbxITEM_MASTER.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbxITEM_MASTER.Location = new System.Drawing.Point(0, 0);
			this.gbxITEM_MASTER.Name = "gbxITEM_MASTER";
			this.gbxITEM_MASTER.Size = new System.Drawing.Size(920, 120);
			this.gbxITEM_MASTER.TabIndex = 0;
			this.gbxITEM_MASTER.TabStop = false;
			// 
			// txtProj_Seq_To
			// 
			this.txtProj_Seq_To.BackColor = System.Drawing.Color.White;
			this.txtProj_Seq_To.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProj_Seq_To.Location = new System.Drawing.Point(744, 16);
			this.txtProj_Seq_To.Name = "txtProj_Seq_To";
			this.txtProj_Seq_To.Size = new System.Drawing.Size(105, 21);
			this.txtProj_Seq_To.TabIndex = 288;
			this.txtProj_Seq_To.Tag = "";
			this.txtProj_Seq_To.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(728, 24);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(8, 8);
			this.label11.TabIndex = 287;
			this.label11.Tag = "0";
			this.label11.Text = "~";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.Color.Beige;
			this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label10.Location = new System.Drawing.Point(520, 16);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(96, 21);
			this.label10.TabIndex = 286;
			this.label10.Text = "생산차수";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnItem
			// 
			this.btnItem.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnItem.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnItem.Image = ((System.Drawing.Image)(resources.GetObject("btnItem.Image")));
			this.btnItem.Location = new System.Drawing.Point(239, 40);
			this.btnItem.Name = "btnItem";
			this.btnItem.Size = new System.Drawing.Size(25, 21);
			this.btnItem.TabIndex = 285;
			this.btnItem.Tag = "0";
			this.btnItem.Click += new System.EventHandler(this.btnItem_Click);
			// 
			// txtItemNm
			// 
			this.txtItemNm.BackColor = System.Drawing.Color.White;
			this.txtItemNm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtItemNm.Location = new System.Drawing.Point(263, 40);
			this.txtItemNm.Name = "txtItemNm";
			this.txtItemNm.Size = new System.Drawing.Size(241, 21);
			this.txtItemNm.TabIndex = 284;
			this.txtItemNm.TabStop = false;
			this.txtItemNm.Tag = "2";
			this.txtItemNm.Text = "";
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.Color.Beige;
			this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label9.Location = new System.Drawing.Point(8, 40);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 21);
			this.label9.TabIndex = 283;
			this.label9.Text = "품목코드";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtItemCd
			// 
			this.txtItemCd.BackColor = System.Drawing.Color.White;
			this.txtItemCd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtItemCd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtItemCd.Location = new System.Drawing.Point(103, 40);
			this.txtItemCd.Name = "txtItemCd";
			this.txtItemCd.Size = new System.Drawing.Size(137, 21);
			this.txtItemCd.TabIndex = 282;
			this.txtItemCd.Tag = "";
			this.txtItemCd.Text = "";
			this.txtItemCd.TextChanged += new System.EventHandler(this.txtItemCd_TextChanged);
			// 
			// btnWorkOrderTo
			// 
			this.btnWorkOrderTo.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnWorkOrderTo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnWorkOrderTo.Image = ((System.Drawing.Image)(resources.GetObject("btnWorkOrderTo.Image")));
			this.btnWorkOrderTo.Location = new System.Drawing.Point(424, 88);
			this.btnWorkOrderTo.Name = "btnWorkOrderTo";
			this.btnWorkOrderTo.Size = new System.Drawing.Size(25, 21);
			this.btnWorkOrderTo.TabIndex = 281;
			this.btnWorkOrderTo.Tag = "0";
			this.btnWorkOrderTo.Click += new System.EventHandler(this.btnWorkOrderTo_Click);
			// 
			// btnMakeOrderTo
			// 
			this.btnMakeOrderTo.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnMakeOrderTo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnMakeOrderTo.Image = ((System.Drawing.Image)(resources.GetObject("btnMakeOrderTo.Image")));
			this.btnMakeOrderTo.Location = new System.Drawing.Point(424, 64);
			this.btnMakeOrderTo.Name = "btnMakeOrderTo";
			this.btnMakeOrderTo.Size = new System.Drawing.Size(25, 21);
			this.btnMakeOrderTo.TabIndex = 280;
			this.btnMakeOrderTo.Tag = "0";
			this.btnMakeOrderTo.Click += new System.EventHandler(this.btnMakeOrderTo_Click);
			// 
			// btnWorkOrderFr
			// 
			this.btnWorkOrderFr.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnWorkOrderFr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnWorkOrderFr.Image = ((System.Drawing.Image)(resources.GetObject("btnWorkOrderFr.Image")));
			this.btnWorkOrderFr.Location = new System.Drawing.Point(239, 88);
			this.btnWorkOrderFr.Name = "btnWorkOrderFr";
			this.btnWorkOrderFr.Size = new System.Drawing.Size(25, 21);
			this.btnWorkOrderFr.TabIndex = 279;
			this.btnWorkOrderFr.Tag = "0";
			this.btnWorkOrderFr.Click += new System.EventHandler(this.btnWorkOrderFr_Click);
			// 
			// btnMakeOrderFr
			// 
			this.btnMakeOrderFr.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnMakeOrderFr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnMakeOrderFr.Image = ((System.Drawing.Image)(resources.GetObject("btnMakeOrderFr.Image")));
			this.btnMakeOrderFr.Location = new System.Drawing.Point(239, 64);
			this.btnMakeOrderFr.Name = "btnMakeOrderFr";
			this.btnMakeOrderFr.Size = new System.Drawing.Size(25, 21);
			this.btnMakeOrderFr.TabIndex = 278;
			this.btnMakeOrderFr.Tag = "0";
			this.btnMakeOrderFr.Click += new System.EventHandler(this.btnMakeOrderFr_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(272, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(8, 8);
			this.label8.TabIndex = 277;
			this.label8.Tag = "0";
			this.label8.Text = "~";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtWorkOrderTo
			// 
			this.txtWorkOrderTo.BackColor = System.Drawing.Color.White;
			this.txtWorkOrderTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtWorkOrderTo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtWorkOrderTo.Location = new System.Drawing.Point(288, 88);
			this.txtWorkOrderTo.Name = "txtWorkOrderTo";
			this.txtWorkOrderTo.Size = new System.Drawing.Size(137, 21);
			this.txtWorkOrderTo.TabIndex = 276;
			this.txtWorkOrderTo.Tag = "";
			this.txtWorkOrderTo.Text = "";
			// 
			// txtMakeOrderTo
			// 
			this.txtMakeOrderTo.BackColor = System.Drawing.Color.White;
			this.txtMakeOrderTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtMakeOrderTo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtMakeOrderTo.Location = new System.Drawing.Point(288, 64);
			this.txtMakeOrderTo.Name = "txtMakeOrderTo";
			this.txtMakeOrderTo.Size = new System.Drawing.Size(137, 21);
			this.txtMakeOrderTo.TabIndex = 275;
			this.txtMakeOrderTo.Tag = "";
			this.txtMakeOrderTo.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(272, 72);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(8, 8);
			this.label7.TabIndex = 274;
			this.label7.Tag = "0";
			this.label7.Text = "~";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.Beige;
			this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label6.Location = new System.Drawing.Point(8, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(96, 21);
			this.label6.TabIndex = 273;
			this.label6.Text = "제조오더번호";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtWorkOrderFr
			// 
			this.txtWorkOrderFr.BackColor = System.Drawing.Color.White;
			this.txtWorkOrderFr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtWorkOrderFr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtWorkOrderFr.Location = new System.Drawing.Point(103, 88);
			this.txtWorkOrderFr.Name = "txtWorkOrderFr";
			this.txtWorkOrderFr.Size = new System.Drawing.Size(137, 21);
			this.txtWorkOrderFr.TabIndex = 272;
			this.txtWorkOrderFr.Tag = "";
			this.txtWorkOrderFr.Text = "";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Beige;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Location = new System.Drawing.Point(8, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 21);
			this.label5.TabIndex = 271;
			this.label5.Text = "제품오더번호";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtMakeOrderFr
			// 
			this.txtMakeOrderFr.BackColor = System.Drawing.Color.White;
			this.txtMakeOrderFr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtMakeOrderFr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtMakeOrderFr.Location = new System.Drawing.Point(103, 64);
			this.txtMakeOrderFr.Name = "txtMakeOrderFr";
			this.txtMakeOrderFr.Size = new System.Drawing.Size(137, 21);
			this.txtMakeOrderFr.TabIndex = 270;
			this.txtMakeOrderFr.Tag = "";
			this.txtMakeOrderFr.Text = "";
			// 
			// cboSCH_ID
			// 
			this.cboSCH_ID.Location = new System.Drawing.Point(616, 64);
			this.cboSCH_ID.Name = "cboSCH_ID";
			this.cboSCH_ID.Size = new System.Drawing.Size(128, 20);
			this.cboSCH_ID.TabIndex = 269;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Beige;
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Location = new System.Drawing.Point(520, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 21);
			this.label4.TabIndex = 268;
			this.label4.Text = "스케쥴 ID";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label34
			// 
			this.label34.Location = new System.Drawing.Point(728, 48);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(8, 8);
			this.label34.TabIndex = 267;
			this.label34.Tag = "0";
			this.label34.Text = "~";
			this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dtpDLV_TO_DT
			// 
			this.dtpDLV_TO_DT.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpDLV_TO_DT.Location = new System.Drawing.Point(744, 40);
			this.dtpDLV_TO_DT.Name = "dtpDLV_TO_DT";
			this.dtpDLV_TO_DT.Size = new System.Drawing.Size(104, 21);
			this.dtpDLV_TO_DT.TabIndex = 266;
			// 
			// dtpDLV_FR_DT
			// 
			this.dtpDLV_FR_DT.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpDLV_FR_DT.Location = new System.Drawing.Point(616, 40);
			this.dtpDLV_FR_DT.Name = "dtpDLV_FR_DT";
			this.dtpDLV_FR_DT.Size = new System.Drawing.Size(104, 21);
			this.dtpDLV_FR_DT.TabIndex = 265;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Beige;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Location = new System.Drawing.Point(520, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 21);
			this.label1.TabIndex = 264;
			this.label1.Text = "생산완료일";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnCONF
			// 
			this.btnCONF.BackColor = System.Drawing.SystemColors.Control;
			this.btnCONF.Location = new System.Drawing.Point(760, 80);
			this.btnCONF.Name = "btnCONF";
			this.btnCONF.Size = new System.Drawing.Size(88, 32);
			this.btnCONF.TabIndex = 263;
			this.btnCONF.Text = "확정";
			this.btnCONF.Click += new System.EventHandler(this.btnCONF_Click);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Beige;
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(520, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 21);
			this.label3.TabIndex = 262;
			this.label3.Text = "처리상태";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cboSTATUS
			// 
			this.cboSTATUS.ItemHeight = 12;
			this.cboSTATUS.Location = new System.Drawing.Point(616, 88);
			this.cboSTATUS.Name = "cboSTATUS";
			this.cboSTATUS.Size = new System.Drawing.Size(128, 20);
			this.cboSTATUS.TabIndex = 261;
			this.cboSTATUS.Tag = "0";
			// 
			// btnPROJECT
			// 
			this.btnPROJECT.BackColor = System.Drawing.Color.WhiteSmoke;
			this.btnPROJECT.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPROJECT.Image = ((System.Drawing.Image)(resources.GetObject("btnPROJECT.Image")));
			this.btnPROJECT.Location = new System.Drawing.Point(239, 16);
			this.btnPROJECT.Name = "btnPROJECT";
			this.btnPROJECT.Size = new System.Drawing.Size(25, 21);
			this.btnPROJECT.TabIndex = 258;
			this.btnPROJECT.Tag = "0";
			this.btnPROJECT.Click += new System.EventHandler(this.btnPROJECT_Click);
			// 
			// txtProj_Seq_Fr
			// 
			this.txtProj_Seq_Fr.BackColor = System.Drawing.Color.White;
			this.txtProj_Seq_Fr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProj_Seq_Fr.Location = new System.Drawing.Point(615, 16);
			this.txtProj_Seq_Fr.Name = "txtProj_Seq_Fr";
			this.txtProj_Seq_Fr.Size = new System.Drawing.Size(105, 21);
			this.txtProj_Seq_Fr.TabIndex = 236;
			this.txtProj_Seq_Fr.Tag = "";
			this.txtProj_Seq_Fr.Text = "";
			// 
			// txtProj_Nm
			// 
			this.txtProj_Nm.BackColor = System.Drawing.Color.White;
			this.txtProj_Nm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProj_Nm.Location = new System.Drawing.Point(263, 16);
			this.txtProj_Nm.Name = "txtProj_Nm";
			this.txtProj_Nm.Size = new System.Drawing.Size(241, 21);
			this.txtProj_Nm.TabIndex = 239;
			this.txtProj_Nm.TabStop = false;
			this.txtProj_Nm.Tag = "2";
			this.txtProj_Nm.Text = "";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Beige;
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 21);
			this.label2.TabIndex = 238;
			this.label2.Text = "프로젝트코드";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtProj_No
			// 
			this.txtProj_No.BackColor = System.Drawing.Color.White;
			this.txtProj_No.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProj_No.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtProj_No.Location = new System.Drawing.Point(103, 16);
			this.txtProj_No.Name = "txtProj_No";
			this.txtProj_No.Size = new System.Drawing.Size(137, 21);
			this.txtProj_No.TabIndex = 235;
			this.txtProj_No.Tag = "";
			this.txtProj_No.Text = "";
			this.txtProj_No.TextChanged += new System.EventHandler(this.txtProj_No_TextChanged);
			// 
			// splitter2
			// 
			this.splitter2.BackColor = System.Drawing.Color.Beige;
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter2.Location = new System.Drawing.Point(0, 192);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(920, 8);
			this.splitter2.TabIndex = 2;
			this.splitter2.TabStop = false;
			// 
			// imageList2
			// 
			this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// PUA101
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(920, 501);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.pnlSN);
			this.Name = "PUA101";
			this.Text = "긴급작업지시";
			this.Load += new System.EventHandler(this.PUA101_Load);
			this.Controls.SetChildIndex(this.panButton5, 0);
			this.Controls.SetChildIndex(this.pnlSN, 0);
			this.Controls.SetChildIndex(this.GridCommPanel, 0);
			this.Controls.SetChildIndex(this.splitter2, 0);
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
			this.pnlSN.ResumeLayout(false);
			this.gbxITEM_MASTER.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region SearchExec() 그리드 조회
		protected override void SearchExec() 
		{			
			try 
			{
				string strSql = " usp_PUA101 @pTYPE = 'S1' ";
				strSql += " ,@pPROJECT_NO='"  + txtProj_No.Text  +"' ";
				strSql += " ,@pPROJECT_SEQ_FR ='" + txtProj_Seq_Fr.Text +"' ";
				strSql += " ,@pPROJECT_SEQ_TO ='" + txtProj_Seq_To.Text +"' ";
				strSql += " ,@pITEM_CD ='" + txtItemCd.Text +"' ";
				strSql += " ,@pSTATUS='"	  + cboSTATUS.SelectedValue.ToString() +"' ";
				strSql += " ,@pDELIVERY_FR_DT='"  + dtpDLV_FR_DT.Text + "' ";
				strSql += " ,@pDELIVERY_TO_DT='"  + dtpDLV_TO_DT.Text + "' ";
				strSql += " ,@pMAKEORDER_NO_FR ='"  + txtMakeOrderFr.Text + "' ";
				strSql += " ,@pMAKEORDER_NO_TO ='"  + txtMakeOrderTo.Text + "' ";
				strSql += " ,@pWORKORDER_NO_FR ='"  + txtWorkOrderFr.Text + "' ";
				strSql += " ,@pWORKORDER_NO_TO ='"  + txtWorkOrderTo.Text + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

				// 확정 데이터는 모두 LOCK시킨다.
				for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString() == "C")
					{
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2")].Locked = true;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Locked = true;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Locked     = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Locked = true;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Locked = true;;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Locked = true;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Locked = true;

						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"프로젝트번호_2")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"그룹코드_2")].Locked = true;
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1,"품목코드_2")].Locked = true;

					}
				}

				GridReMake();

				
			}
			catch(Exception f) 
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion

		#region PUA101_Load
		private void PUA101_Load(object sender, System.EventArgs e)
		{
			SystemBase.Base.GroupBoxLang(gbxITEM_MASTER);

			// 그리드 설정
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단계")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'P040'");
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'B029'");
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")]   = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'P027'");
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'P038'");
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")]     = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'P039'");
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")]     = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '"+SystemBase.Base.gstrLangCd+"', @pCOM_CD = 'Z005'");

			// 콤보 설정
			SystemBase.ComboMake.Combo(cboSTATUS, "usp_P_COMMON @pType='P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P039'", 0);	// 처리상태
			cboSTATUS.SelectedValue = "P";

			SystemBase.ComboMake.Combo(cboSCH_ID, "usp_P_COMMON @pType='P043', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P008'", 0);	// 처리상태
			cboSCH_ID.SelectedValue = "PB0614";
			cboSCH_ID.Enabled = false;

			dtpDLV_TO_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();

			// 프로세스 ID 설정
			PROC_ID = SCH_PROG.GenProcId();

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
		}
		#endregion

		#region 그리드 상단 팝업
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			try
			{
				// 프로젝트 조회일경우
				if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"프로젝트번호_2"))
				{
					WNDW003.WNDW003 pu = new WNDW003.WNDW003();
					pu.ShowDialog();

					if(pu.DialogResult == DialogResult.OK)
					{
						string[] Msgs = pu.ReturnVal;

						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[5].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = Msgs[6].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[6].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[7].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value
							= SystemBase.Base.CodeName("ITEM_CD", "ITEM_UNIT", "B_ITEM_INFO", Msgs[6].ToString() , "");
						
					}
				}
				else if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"품목코드_2")) // 품목코드
				{
					WNDW005.WNDW005 pu = new WNDW005.WNDW005();
					pu.ShowDialog();

					// 프로젝트 차수 클릭
					if(pu.DialogResult==DialogResult.OK)
					{
						string[] Msgs	= pu.ReturnVal;
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[8].ToString();
					}
				}
				else if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"ROUT NO_2")) // ROUT정보
				{
					if(fpSpread1.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx1,"품목코드")].Text != "")
					{
						if(fpSpread1.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx1,"단계")].Value.ToString() == "1")
						{
							string strQuery = "usp_PUA101 'P1'";
							string[] strWhere=new string[]{"@pITEM_CD"};
							string[] strSearch=new string[]{fpSpread1.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx1,"품목코드")].Text};

							UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00062", strQuery, strWhere, strSearch, new int[]{});
							pu.Width = 400;
							pu.ShowDialog();

							if(pu.DialogResult==DialogResult.OK)
							{
								Regex rx1 = new Regex("#");
								string[] Msgs	= rx1.Split(pu.ReturnVal.ToString());

								fpSpread1.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx1,"ROUT NO")].Value	= Msgs[0].ToString(); //공정명
								UIForm.FPMake.fpChange(fpSpread1, e.Row);
							}
						}
						else
						{
							//SystemBase.MessageBoxComm.Show("다단계는 라우팅을 선택할수 없습니다.");
							MessageBox.Show("다단계는 라우팅을 선택할수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
					}
					else
					{
						//SystemBase.MessageBoxComm.Show("품목코드를 입력하셔야 합니다.");
						MessageBox.Show("품목코드를 입력하셔야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}

				else if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자_2"))
				{
					string strQuery = " usp_P_COMMON @pType='P180' ";
					string[] strWhere=new string[]{};
					string[] strSearch=new string[]{};
					UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00095", strQuery, strWhere, strSearch, new int[]{2,3}, "");
					pu.ShowDialog();	
					if(pu.DialogResult==DialogResult.OK)
					{
						Regex rx1 = new Regex("#");
						string[] Msgs	= rx1.Split(pu.ReturnVal.ToString());

						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자")].Text	= Msgs[0].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "담당자명")].Text	= Msgs[1].ToString();
					}
				}
				else if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"원생산오더_2")) // 원생산오더
				{
					WNDW006.WNDW006 pu = new WNDW006.WNDW006();
					pu.ShowDialog();

					if(pu.DialogResult==DialogResult.OK)
					{
						string[] Msgs	= pu.ReturnVal;
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].Text = Msgs[1].ToString();
					}
				}
				else if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"검사의뢰번호_2")) // 검사의뢰번호
				{
					WNDW009.WNDW009 pu = new WNDW009.WNDW009();
					pu.ShowDialog();

					if(pu.DialogResult==DialogResult.OK)
					{
						string[] Msgs	= pu.ReturnVal;
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].Text = Msgs[28].ToString();
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text = Msgs[1].ToString();
					}
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region fpSpread1_Change
		protected override void fpSpread1_ChangeEvent(int Row, int Column)
		{
			string strProjectNo = "", strProjectSeq = "", strGroupCd = "", strItemCd = "";

			strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
			strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
			strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
			strGroupCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text;

			if(Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
			{
				try
				{					
					string strSql = " usp_PUA101 'M2'";
					strSql += ", @pITEM_CD= '" + strItemCd + "'";

					DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
					string ERRCode = dt.Rows[0][0].ToString();
					string MSGCode	= dt.Rows[0][1].ToString();

					if(ERRCode ==  "ER")
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = "";
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
					}
					else
					{
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text 
							= SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text , "");
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value
							= SystemBase.Base.CodeName("ITEM_CD", "ITEM_UNIT", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text , "");
					}
				}
				catch
				{
					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
				}
			}

			if(Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드"))
			{
				if(strProjectNo != "" && strProjectSeq != "" && strGroupCd != "")
				{
					try
					{
						string strSql = " usp_PUA101 'M1'";
						strSql += ", @pPROJECT_NO= '" + strProjectNo + "'";
						strSql += ", @pPROJECT_SEQ= '" + strProjectSeq + "'";
						strSql += ", @pGROUP_CD= '" + strGroupCd + "'";

						DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
						string ERRCode = dt.Rows[0][0].ToString();
						string MSGCode	= dt.Rows[0][1].ToString();

						if(ERRCode ==  "ER")
						{
							MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = "";
						}
					}
					catch
					{
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text = "";
					}
				}
			}

			if(Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자"))
			{
				fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text 
					= fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Text;
			}
		}
		#endregion

		#region MASTER 삭제
		protected override void DeleteExec() 
		{// 행 추가
			try
			{
				if (MessageBox.Show(SystemBase.Base.MessageRtn("P0003"), "Confirm", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
				{ 
					string strSql = " usp_PUA101 'D2' ";

					strSql += ", @pPROJECT_NO = '"		+ PROJECT_NO.ToString() +"'";
					strSql += ", @pPROJECT_SEQ = '"		+ PROJECT_SEQ.ToString() +"'";
					strSql += ", @pGROUP_CD = '"		+ GROUP_CD.ToString() +"'";

					DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
					MessageBox.Show(dt.Rows[0][1].ToString());
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0001"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 행추가
		protected override void RowInsExec() 
		{// 행 추가
			try
			{
				UIForm.FPMake.RowInsert(fpSpread1);
				int RowNum = fpSpread1.ActiveSheet.ActiveRowIndex;

				fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value = "1";      // 단단계
				fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = "Y"; // 재고감안
				fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value   = "N"; // 재작업여부
				fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value = "P";      // 처리상태
				//fpSpread1.Sheets[0].Cells[RowNum, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Text   = DateTime.Today.Date.ToString();
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0001"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region SaveExec() 폼에 입력된 데이타 저장 로직
		protected override void SaveExec() 
		{
			if(fpSpread1.Sheets[0].Rows.Count > 0)
			{
				if(UIForm.FPMake.FPUpCheck(fpSpread1) == true)// 그리드 상단 필수항목 체크
				{
					string ERRCode = "ER";
					string MSGCode = "P0000";
					SqlConnection dbConn = SystemBase.DbOpen.DBCON();
					SqlCommand cmd = dbConn.CreateCommand();
					SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

					try
					{
						for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
						{

							string strGbn  = "";
							string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i,0].Text;

							if(strHead.Length > 0)
							{
								switch(strHead)
								{ 
									case "D": strGbn = "D1"; break;
									case "U": strGbn = "U1"; break;
									case "I": strGbn = "I1"; break;
									default:  strGbn = "";	 break;
								}

								string strSql = " usp_PUA101 '"  + strGbn + "'";
								strSql += ", @pPROJECT_NO= '"	 + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text  + "'";
								strSql += ", @pPROJECT_SEQ= '"	 + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "'";
								strSql += ", @pGROUP_CD= '"		 + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "그룹코드")].Text + "'";
								strSql += ", @pITEM_CD='"        + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
								strSql += ", @pITEM_QTY ='"      + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산수량")].Value + "'";
								strSql += ", @pLEVEL ='"         + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value.ToString() + "'";
								strSql += ", @pROUT_NO ='"       + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text + "'";
								strSql += ", @pDELIVERY_DT ='"   + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산완료일자")].Text + "'";
								strSql += ", @pSO_DELIVERY_DT ='"   + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주납기일")].Text + "'";
								strSql += ", @pREWORK_FLG ='"    + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value.ToString() + "'";
								strSql += ", @pWORKORDER_TYPE='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() + "'";
								strSql += ", @pWORKORDER_NO='"   + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "'";
								strSql += ", @pMAKEORDER_NO='"   + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "'";

								strSql += ", @pCONF_OBJ_FLG='"   + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text=="True"?"1":"0") + "'";
								strSql += ", @pSTATUS='"         + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString() + "'";
								strSql += ", @pMF_PLAN_USER='"   + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "생산담당자")].Text + "'";
								strSql += ", @pREMARK='"         + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";

								strSql += ", @pORG_WORKORDER_NO ='"+ fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더")].Text + "'";
								strSql += ", @pORG_REQ_INSP_REQ_NO ='"+ fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호")].Text + "'";
								
								strSql += ", @pSTOCK_CONSD_FLG ='0'";
								strSql += ", @pLANG_CD='"   + SystemBase.Base.gstrLangCd + "'";
								strSql += ", @pUSR_ID= '"	+ SystemBase.Base.gstrUserID  + "'";

								DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
								ERRCode = dt.Rows[0][0].ToString();
								MSGCode	= dt.Rows[0][1].ToString();

								if(ERRCode == "ER") {Trans.Rollback();goto Exit;}	// ER 코드 Return시 점프
							}
						}
						// 결과 처리
						Trans.Commit();

						SearchExec();
						NewFlg = 0;

					}
					catch
					{
						Trans.Rollback();
						ERRCode = "ER";
						MSGCode = "P0001";
					}
				Exit:
					dbConn.Close();
					//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn(MSGCode));
					if (ERRCode == "OK")
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}

			}
			else
			{
				//MessageBox.Show(SystemBase.Base.MessageRtn("P0002"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region NewExec() 그리드 및 그룹박스 초기화
		protected override void NewExec() 
		{
			try
			{
				SystemBase.Base.GroupBoxLang(gbxITEM_MASTER);

				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);

				NewFlg = 1;
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0001"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region btnPROJECT_Click
		private void btnPROJECT_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW003.WNDW003 pu = new WNDW003.WNDW003();
				pu.ShowDialog();

				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtProj_No.Text		= Msgs[3].ToString();
					txtProj_Nm.Text		= Msgs[4].ToString();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0001"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 확정
		private void btnCONF_Click(object sender, System.EventArgs e)
		{			
			if(fpSpread1.Sheets[0].Rows.Count <= 0) // 확정할 데이터가 없을 경우
			{
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0035"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0035"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (MessageBox.Show(SystemBase.Base.MessageRtn("P0021"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
			{

				string ERRCode = "ER";
				string MSGCode = "P0000";
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try 
				{
					// 긴급작지 대상을 초기화 시킨다.
					bool hasConfData = false; // 확정데이터 존재여부
					string strSql = " usp_PUA101 'U2'";

					DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
					ERRCode = dt.Rows[0][0].ToString();
					MSGCode	= dt.Rows[0][1].ToString();

					if(ERRCode == "ER") { throw new Exception(MSGCode); }	// ER 코드 Return시 점프

					// 현재 설정값을 저장한다.
					for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text == "True")
						{

							strSql = " usp_PUA101 'U3'";
							strSql += ", @pWORKORDER_NO= '"	+ fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text  + "'";
							strSql += ", @pCONF_OBJ_FLG='"  + (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text=="True"?"1":"0") + "'";
							strSql += ", @pITEM_CD='"  + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";

							dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
							ERRCode = dt.Rows[0][0].ToString();
							MSGCode	= dt.Rows[0][1].ToString();

							if(ERRCode == "ER") 
							{ throw new Exception(); }	// ER 코드 Return시 점프

							if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대상")].Text=="True")
								hasConfData = true;
						}
					}

					if(!hasConfData) // 확정 데이터가 없을 경우
					{
						MSGCode = "P0035";
						throw new Exception();
					}

					// 결과 처리
					Trans.Commit();
				}
				catch
				{
					Trans.Rollback();
					//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn(MSGCode));
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

					return;
				}

				finally 
				{
					dbConn.Close();
				}

				// 긴급 처리
				PUA101P1 pu = new PUA101P1(cboSCH_ID.SelectedValue.ToString());
				pu.ShowDialog();

				if(pu.DialogResult==DialogResult.OK)
					SearchExec();
			}
		}
		#endregion

		#region 그리드 콤보 선택 변경
		private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			try
			{
				// 재작업 여부/작업지시구분 변경시
				if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"재작업여부")) 
					if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재작업여부")].Value.ToString() == "Y")
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = false;

				if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"작업지시구분"))
				{
					if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "3")
					{
						fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고감안여부")].Value = false;

						UIForm.FPMake.grdReMake(fpSpread1, e.Row, 
							SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
							);
					}
					else if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "4") //불량 재작업
					{
						UIForm.FPMake.grdReMake(fpSpread1, e.Row, 
							SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|1"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|1"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
							);
					}
					else if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "7") //설계변경
					{
						UIForm.FPMake.grdReMake(fpSpread1, e.Row, 
							SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|1"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
							);
					}
					else
					{
						UIForm.FPMake.grdReMake(fpSpread1, e.Row, 
							SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|0"
							+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
							);
					}
				}

				if(e.Column == SystemBase.Base.GridHeadIndex(GHIdx1,"단계"))
				{
					if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text != "")
					{

						if(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단계")].Value.ToString() == "0")
						{
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text
								= SystemBase.Base.CodeName("ITEM_CD", "ROUT_NO", "P_BOP_PROC_MASTER", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text , "AND MAJOR_FLG = 'Y'");
						}
						else
						{
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT NO")].Text = "";
						}
					}
					else
					{
						//SystemBase.MessageBoxComm.Show("품목정보를 입력하셔야 선택가능합니다.");
						MessageBox.Show(SystemBase.Base.MessageRtn("품목정보를 입력하셔야 선택가능합니다."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				//SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("P0001"));
				MessageBox.Show(SystemBase.Base.MessageRtn("P0001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region GridReMake() 그리드 재정의
		public void GridReMake() 
		{
			try
			{
				string strStatus = "";
				if(fpSpread1.Sheets[0].Rows.Count > 0)
				{
					for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						 strStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "처리상태")].Value.ToString();

						if(strStatus != "P")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i, 
								"1|3#" 	+ "2|3#" 
								+ "4|3#"	+ "5|3#"
								+ "6|3#"	+ "7|3#"
								+ "8|3#"	+ "9|3#"
								+ "10|3#"	+ "13|3#"
								+ "15|3#"	+ "17|3#"
								+ "18|3#"	+ "19|3#"  +  "21|3#"
								+ "22|3#"	+ "23|3#"
								+ "24|3#"	+ "26|3#"
								+ "27|3#"	+ "28|3#"
								+ "29|3#"	+ "30|3");
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i, 
								"1|0#" 	+ "2|0#" 
								+ "4|1#"	+ "5|0#"
								+ "6|1#"	+ "7|1#"
								+ "8|0#"	+ "9|1#"
								+ "10|0#"	+ "13|1#"
								+ "15|1#"	+ "17|0#"
								+ "18|1#"	+ "19|1#"  +  "21|1#"
								+ "22|13#"	+ "23|0#"
								+ "24|0#"	+ "26|0#"
								+ "27|0#"	+ "28|0#"
								+ "29|0#"	+ "30|0");


							if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "4") //불량 재작업
							{
								UIForm.FPMake.grdReMake(fpSpread1, i, 
									SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|1"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|1"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
									);
							}
							else if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시구분")].Value.ToString() == "7") //설계변경
							{
								UIForm.FPMake.grdReMake(fpSpread1, i, 
									SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더") + "|1"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원생산오더_2") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰번호_2") + "|0"
									);
							}
						}
					}
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050","그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region TextChanged
		private void txtProj_No_TextChanged(object sender, System.EventArgs e)
		{
			txtProj_Nm.Text = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProj_No.Text, "");
		}

		private void txtItemCd_TextChanged(object sender, System.EventArgs e)
		{
			txtItemNm.Text = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, "");
		}
		#endregion

		#region 팝업
		//품목코드
		private void btnItem_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW005.WNDW005 pu = new WNDW005.WNDW005(txtItemCd.Text,"");
				pu.ShowDialog();
				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtItemCd.Text = Msgs[2].ToString();
					txtItemNm.Text = Msgs[3].ToString();

					txtItemCd.Focus();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050","품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//제품오더fr
		private void btnMakeOrderFr_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW008.WNDW008 pu = new WNDW008.WNDW008(txtMakeOrderFr.Text);
				pu.ShowDialog();
				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;
	
					txtMakeOrderFr.Text = Msgs[1].ToString();
					txtMakeOrderFr.Focus();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050","제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//제품오더to
		private void btnMakeOrderTo_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW008.WNDW008 pu = new WNDW008.WNDW008(txtMakeOrderTo.Text);
				pu.ShowDialog();
				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;
	
					txtMakeOrderTo.Text = Msgs[1].ToString();
					txtMakeOrderTo.Focus();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050","제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//제조오더fr
		private void btnWorkOrderFr_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW006.WNDW006 pu = new WNDW006.WNDW006(txtWorkOrderFr.Text);
				pu.ShowDialog();	
				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtWorkOrderFr.Text = Msgs[1].ToString();
					txtWorkOrderFr.Focus();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//제조오더to
		private void btnWorkOrderTo_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW006.WNDW006 pu = new WNDW006.WNDW006(txtWorkOrderTo.Text);
				pu.ShowDialog();	
				if(pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtWorkOrderTo.Text = Msgs[1].ToString();
					txtWorkOrderTo.Focus();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}
}