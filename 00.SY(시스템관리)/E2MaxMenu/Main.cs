using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using C1.Win.C1Command;

namespace E2MAXMenu
{
    public partial class Main : System.Windows.Forms.Form
    {
        private string[,] menuName;
        public static string UserID;
        public static string UserName;
        public static string ASSIGN_NO;
        public Form CForm;
        public SqlConnection dbcon;
        public SqlDataAdapter adapter;
        DataSet ds;
        DataSet dsMenu = new DataSet();

        public System.Windows.Forms.StatusBar statusBar1;
        private System.Windows.Forms.ImageList imageList1;
        private C1.Win.C1Command.C1CommandDock c1CommandDock1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.StatusBarPanel statusBarPanel5;
        private C1.Win.C1Command.C1CommandHolder c1CommandHolder1;
        private C1.Win.C1Command.C1MainMenu c1MainMenu1;
        public C1.Win.C1Command.C1DockingTab tabForms;

        //private string Pic1DownYn = "N";
        //private string Pic2DownYn = "N";
        //private string Pic3DownYn = "N";
        //private string Pic4DownYn = "N";
        //private string Pic5DownYn = "N";
        //private string Pic6DownYn = "N";

        //string Tmp = "";

        public Main()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

        }

        public Main(string MDIFORMNAME, string Assign_NO)
        {
            SystemBase.Base.RodeFormID = MDIFORMNAME;
            ASSIGN_NO = Assign_NO;

            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();  
        }


        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>


        /// <summary>
        /// 상태바 텍스트
        /// </summary>
        public string StatusBarText
        {
            get { return statusBarPanel4.Text; }
            set { statusBarPanel4.Text = value; }
        }

        #region 상단 메뉴 버튼 동적 생성
        private void MenuBarCreate()
        {
            try
            {
                //초기화
                panel1.Controls.Clear();

                //새로고침버튼
                PictureBox btnRefresh = new PictureBox();
                btnRefresh.Name = "btnRefresh";
                btnRefresh.Location = new Point(13, 3);
                btnRefresh.Size = new Size(182, 41);
                btnRefresh.BackColor = Color.Transparent;
                btnRefresh.Cursor = Cursors.Hand;
                btnRefresh.Text = "";
                totBtnRefresh.SetToolTip(btnRefresh, "새로고침");

                //이벤트발생
                btnRefresh.Click += new System.EventHandler(btnRefresh_Click);

                panel1.Controls.Add(btnRefresh);

                string Query1 = "exec usp_MAIN 'S7','" + SystemBase.Base.gstrUserID.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                DataSet ds1 = SystemBase.DbOpen.NoTranDataSet(Query1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    Button[] btn = new Button[ds1.Tables[0].Rows.Count];

                    for (int i = 0; i < btn.Length; i++)
                    {
                        btn[i] = new Button();
                        btn[i].Location = new Point(235 + (i * 87), 3);
                        btn[i].Size = new Size(81, 40);
                        btn[i].Name = "MenuBtn" + i.ToString();
                        btn[i].TabIndex = i;
                        btn[i].FlatStyle = FlatStyle.Flat;
                        btn[i].Font = new System.Drawing.Font("돋움", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(129)));
                        btn[i].BackColor = Color.Transparent;
                        btn[i].ForeColor = Color.White;
                        btn[i].Text = ds1.Tables[0].Rows[i]["MENU_NAME"].ToString();
                        btn[i].Cursor = Cursors.Hand;

                        //이벤트발생
                        btn[i].Click += new System.EventHandler(btn_Click);
                        btn[i].MouseHover += new System.EventHandler(btn_MouseHover);
                        btn[i].MouseLeave += new System.EventHandler(btn_MouseLeave);

                        panel1.Controls.Add(btn[i]);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 메뉴바 동적 버튼 이벤트
        private void btn_Click(object sender, System.EventArgs e)
        {
            Button t_sender = sender as Button;
            int iSeq = Convert.ToInt16(t_sender.Name.Replace("MenuBtn", "").ToString());

            //트리뷰 재생성
            Image_Menu_Create(iSeq);
        }

        private void btn_MouseHover(object sender, System.EventArgs e)
        {
            Button t_sender = sender as Button;
            t_sender.ForeColor = Color.Blue;
        }

        private void btn_MouseLeave(object sender, System.EventArgs e)
        {
            Button t_sender = sender as Button;
            t_sender.ForeColor = Color.White;
        }
        #endregion

        #region Main폼 로드
        private void Form1_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.Top = 0;
                this.Left = 0;
                this.SuspendLayout();
                SystemBase.Base.BaseForm = this;
                this.Text = "E2Max-MTMS";

                c1CommandHolder1.CommandClick += new CommandClickEventHandler(CommandClickHandler);
                c1CommandHolder1.ImageList = this.imageList1;
               // c1CommandHolder1.LookAndFeel = LookAndFeelEnum..Office2003;
                c1CommandHolder1.VisualStyle = VisualStyle.Office2010Blue;

                c1MainMenu1.BackColor = Color.WhiteSmoke;
                c1MainMenu1.CommandHolder = c1CommandHolder1;
                string menuType = "S6";
                CreateMenu(menuType);

                statusBarPanel1.Text = SystemBase.Base.gstrCOMNM;
                statusBarPanel2.Text = statusBarPanel2.Text = SystemBase.Base.gstrServerNM + " / " + SystemBase.Base.gstrDbName.ToUpper() + " - " + SystemBase.Base.gstrUserName + "(" + SystemBase.Base.gstrUserID + ")님이 사용중입니다.";
                statusBarPanel3.Text = SystemBase.Base.ServerTime("YYMMDD");	// 서버 시간(2007-10-10)

                string Query = "exec usp_MAIN @pTYPE = '" + menuType + "', @pUSR_ID = '" + SystemBase.Base.gstrUserID.ToString() + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
                ds = SystemBase.DbOpen.NoTranDataSet(Query);

                //	트리뷰 동적 생성
                DataView dvwData = null;
                UIForm.TreeView.CreateTreeView("*", (TreeNode)null, treeView1, ds, dvwData, 0);

                //메뉴바 동적 생성
                MenuBarCreate();

                if (SystemBase.Base.RodeFormID.Trim().Length > 0)
                {
                    //#####################MDI 화면명이 넘어온 경우############################//

                    object[] param = new object[1];
                    param[0] = ASSIGN_NO;

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + SystemBase.Base.RodeFormID.ToString() + ".dll");
                    CForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(SystemBase.Base.RodeFormID.ToString() + "." + SystemBase.Base.RodeFormID.ToString()), param);
                    CForm.MdiParent = this;
                    CForm.WindowState = FormWindowState.Maximized;
                    CForm.Show();
                    
                    //#####################MDI 화면명이 넘어온 경우############################//
                }
                else
                {
                    //#####################공지사항 로드############################//
                    MDIFORM("BB.BZG001.BZG001", "공지사항", "BZG001", "D", "P");
                    //#####################공지사항 로드############################//

                    //#####################근태등록 권한이 있으면 근태등록 로드############################//
                    string strQuery = "USP_MAIN 'S9', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "', @pUSR_ID =  '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                    DataTable Hdt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (Hdt.Rows.Count > 0)
                    {
                        MDIFORM("HA.HAA001.HAA001", "개인별출퇴근등록", "HAA001", "D", "P");
                    }
                    //#####################근태등록 로드############################//

                    //#####################개발사용자는 개발작업일보를 띄워준다..############################//
                    string strSql = "USP_MAIN 'C3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "', @pUSR_ID =  '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                    DataTable Hdt2 = SystemBase.DbOpen.NoTranDataTable(strSql);

                    if (Hdt2.Rows.Count > 0)
                    {
                        MDIFORM("PE.PEA004.PEA004", "개발작업일보등록", "PEA004", "D", "P");
                    }
                    //#####################근태등록 로드############################//
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("MDI LOAD FAIL", f.ToString());
            }
            finally
            {
                this.ResumeLayout();
            }
            this.TopMost = false;
        }
        #endregion

        private void CreateMenu(string menuType)
        {
            dsMenu = GetDataSet(menuType);

            foreach (DataRow drTemp in dsMenu.Tables[0].Select("MenuParent='*'"))
            {
                CreateMenuItem(drTemp[0].ToString());
            }
        }

        private void CreateMenuItem(string strMenu)
        {
            C1CommandMenu mFile = (C1CommandMenu)c1CommandHolder1.CreateCommand(typeof(C1CommandMenu));
            mFile.Text = GetMenuName(strMenu);
            c1MainMenu1.CommandLinks.Add(new C1CommandLink(mFile));

            if (dsMenu.Tables[0].Select("MenuParent='" + strMenu + "'").Length > 0)
            {
                foreach (DataRow drTemp in dsMenu.Tables[0].Select("MenuParent='" + strMenu + "'"))
                {
                    CreateMenuItems(mFile, drTemp[0].ToString());
                }
            }
        }

        private string CreateMenuItems(C1.Win.C1Command.C1CommandMenu mnuItem, string strMenu)
        {
            if (dsMenu.Tables[0].Select("MenuParent='" + strMenu + "'").Length > 0)
            {
                C1CommandMenu cNew = (C1CommandMenu)c1CommandHolder1.CreateCommand(typeof(C1CommandMenu));
                cNew.Text = GetMenuName(strMenu);//"&New";
                cNew.Enabled = GetMenuEnable(strMenu);

                mnuItem.CommandLinks.Add(new C1CommandLink(cNew));

                if (dsMenu.Tables[0].Select("MenuParent='" + strMenu + "'").Length > 0)
                {
                    foreach (DataRow drTemp in dsMenu.Tables[0].Select("MenuParent='" + strMenu + "'"))
                    {
                        CreateMenuItems(cNew, drTemp[0].ToString());
                    }
                }
            }
            else
            {
                C1Command cNew = c1CommandHolder1.CreateCommand();
                cNew.Text = GetMenuName(strMenu);//"&New";
                cNew.UserData = strMenu;

                cNew.Enabled = GetMenuEnable(strMenu);

                mnuItem.CommandLinks.Add(new C1CommandLink(cNew));
                return strMenu;
            }
            return strMenu;
        }


        private string GetMenuName(string strMenuID)
        {
            return dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][1].ToString();
        }
        private int ImageIndex(string strMenuID)
        {
            return int.Parse(dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][6].ToString());
        }
        private void GetShortcut(string strMenuID, C1Command cNew)
        {//단축키
            if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "A")
                cNew.Shortcut = Shortcut.CtrlA;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "B")
                cNew.Shortcut = Shortcut.CtrlB;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "C")
                cNew.Shortcut = Shortcut.CtrlC;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "D")
                cNew.Shortcut = Shortcut.CtrlD;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "E")
                cNew.Shortcut = Shortcut.CtrlE;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "F")
                cNew.Shortcut = Shortcut.CtrlF;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "G")
                cNew.Shortcut = Shortcut.CtrlG;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "H")
                cNew.Shortcut = Shortcut.CtrlH;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "I")
                cNew.Shortcut = Shortcut.CtrlI;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "J")
                cNew.Shortcut = Shortcut.CtrlJ;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "K")
                cNew.Shortcut = Shortcut.CtrlK;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "L")
                cNew.Shortcut = Shortcut.CtrlL;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "M")
                cNew.Shortcut = Shortcut.CtrlM;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "N")
                cNew.Shortcut = Shortcut.CtrlN;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "O")
                cNew.Shortcut = Shortcut.CtrlO;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "P")
                cNew.Shortcut = Shortcut.CtrlP;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "Q")
                cNew.Shortcut = Shortcut.CtrlQ;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "R")
                cNew.Shortcut = Shortcut.CtrlR;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "S")
                cNew.Shortcut = Shortcut.CtrlS;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "T")
                cNew.Shortcut = Shortcut.CtrlT;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "U")
                cNew.Shortcut = Shortcut.CtrlU;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "V")
                cNew.Shortcut = Shortcut.CtrlV;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "W")
                cNew.Shortcut = Shortcut.CtrlW;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "X")
                cNew.Shortcut = Shortcut.CtrlX;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "Y")
                cNew.Shortcut = Shortcut.CtrlY;
            else if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][7].ToString() == "Z")
                cNew.Shortcut = Shortcut.CtrlZ;
        }

        private bool GetMenuEnable(string strMenuID)
        {
            if (dsMenu.Tables[0].Select("MenuID='" + strMenuID + "'")[0][3].ToString() == "Y")
                return true;
            else
                return false;
        }

        private DataSet GetDataSet(string menuType)
        {
            DataSet ds = new DataSet("Menu");

            DataTable drTmp = ds.Tables.Add("Menu");
            drTmp.Columns.Add("MenuID", typeof(string));
            drTmp.Columns.Add("MenuName", typeof(string));
            drTmp.Columns.Add("MenuParent", typeof(string));
            drTmp.Columns.Add("MenuEnable", typeof(string));
            drTmp.Columns.Add("MDIForm", typeof(string));
            drTmp.Columns.Add("PG_KIND", typeof(string));
            drTmp.Columns.Add("SHOW_KIND", typeof(string));

            string Query = "exec usp_MAIN '" + menuType + "','" + SystemBase.Base.gstrUserID.ToString() + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() +"' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);


            menuName = new string[dt.Rows.Count, 3];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                menuName[i, 0] = row["MenuName"].ToString();
                menuName[i, 1] = row["MDIForm"].ToString();
                menuName[i, 2] = row["MenuID"].ToString();
                drTmp.Rows.Add(new object[] { row["MenuID"].ToString(), row["MenuName"].ToString(), row["MenuParent"].ToString(), row["Enable"].ToString(), row["MDIForm"].ToString(), row["PG_KIND"].ToString(), row["SHOW_KIND"].ToString() });

                i++;
            }

            return ds;
        }

        private void Menu_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i <= (menuName.Length / 3) - 1; i++)
            {
                if (menuName[i, 0] == ((MenuItem)sender).Text.ToUpper())
                {
                    MDIFORM(menuName[i, 1].ToString(), menuName[i, 0].ToString(), menuName[i, 2].ToString(), menuName[i, 5].ToString(), menuName[i, 6].ToString());
                }
            }
        }
        private void CommandClickHandler(object sender, CommandClickEventArgs e)
        {
            try
            {
                if (e.Command.Name == "cmdClose" || e.Command.Name == "cmdRestore" || e.Command.Name == "cmdMinimize")
                {
                }
                else
                {
                    MDIFORM(dsMenu.Tables[0].Select("MenuID='" + e.Command.UserData + "'")[0][4].ToString()
                        , dsMenu.Tables[0].Select("MenuID='" + e.Command.UserData + "'")[0][1].ToString()
                        , dsMenu.Tables[0].Select("MenuID='" + e.Command.UserData + "'")[0][0].ToString()
                        , dsMenu.Tables[0].Select("MenuID='" + e.Command.UserData + "'")[0]["PG_KIND"].ToString()
                        , dsMenu.Tables[0].Select("MenuID='" + e.Command.UserData + "'")[0]["SHOW_KIND"].ToString()
                        );
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "화면 구성 중"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [DllImport("user32.dll")]
        public extern static int SetParent(IntPtr child, IntPtr parent);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void MDIFORM(string MdiFM, string MenuName, string MenuID, string PG_KIND, string SHOW_KIND)
        {	// 다른 프로젝트 폼 로딩
            try
            {
                if (PG_KIND == "E")
                {
                    string fullpath = System.Windows.Forms.Application.ExecutablePath;
                    string AppFolder = Path.GetDirectoryName(fullpath);

                    System.Diagnostics.Process.Start(AppFolder + @"\" + MenuID.ToString() + ".exe", SystemBase.Base.gstrUserID.ToString());
                }
                else
                {
                    if (MdiFM.Length > 2)
                    {
                        string TMdiFM = MdiFM.ToString();
                        // 구분자 . 가 마지막뒤에있는것이 클래스명이며, 앞에것은 모두 네임스페이스로 간주한다.
                        //						string NamespaceName	= TMdiFM.Substring(0, TMdiFM.IndexOf(".",0, TMdiFM.Length-1) );
                        string NamespaceName = TMdiFM.Substring(0, TMdiFM.LastIndexOf("."));
                        //						string RodeFormName		= TMdiFM.Substring(TMdiFM.IndexOf(".",0, TMdiFM.Length)+1, TMdiFM.Length-TMdiFM.IndexOf(".",0, TMdiFM.Length)-1 );
                        string RodeFormName = TMdiFM.Substring(TMdiFM.LastIndexOf(".") + 1, TMdiFM.Length - TMdiFM.LastIndexOf(".") - 1);

                        SystemBase.Base.RodeFormName = RodeFormName;
                        SystemBase.Base.RodeFormID = MenuID.ToString();
                        SystemBase.Base.RodeFormText = SystemBase.Base.CodeName("MENU_ID", "MENU_NAME", "CO_SYS_MENU", MenuID.ToString(), "");
                        statusBarPanel4.Text = MenuID.ToString();

                        bool mdiwin = false;
                        for (int i = 0; i < MdiChildren.Length; i++)
                        {	// 폼이 이미 열려있으면 열린폼을 앞쪽으로
                            if (MdiChildren[i].Name == RodeFormName)
                            {
                                MdiChildren[i].BringToFront();
                                mdiwin = true;
                                break;
                            }
                        }

                        if (mdiwin == false)
                        {
                            Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + NamespaceName.ToString() + ".dll");
                            Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(MdiFM.ToString()));
               
                            if (SHOW_KIND.ToString() == "D")
                            {
                                myForm.ShowDialog();
                            }
                            else if (SHOW_KIND.ToString() == "S")
                            {
                                myForm.Show();
                            }
                            else if (SHOW_KIND.ToString() == "P")
                            {
                                myForm.MdiParent = this;
                                myForm.WindowState = FormWindowState.Maximized;
                                myForm.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY006"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView1_DoubleClick(object sender, System.EventArgs e)
        {
            try
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (treeView1.SelectedNode.Tag.ToString() == ds.Tables[0].Rows[i]["MENUID"].ToString() && ds.Tables[0].Rows[i]["MDIFORM"].ToString() != "*")
                    {
                        MDIFORM(ds.Tables[0].Rows[i]["MDIFORM"].ToString(), ds.Tables[0].Rows[i]["MENUNAME"].ToString(), ds.Tables[0].Rows[i]["MENUID"].ToString(), ds.Tables[0].Rows[i]["PG_KIND"].ToString(), ds.Tables[0].Rows[i]["SHOW_KIND"].ToString());
                    }
                }
            }
            catch { }
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SystemBase.Base.RodeFormName = "";
            DialogResult Rtn = MessageBox.Show(SystemBase.Base.MessageRtn("SY003"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (Rtn != DialogResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                Main.ActiveForm.Dispose();
                Application.Exit();
            }
        }

        private void Form1_MdiChildActivate(object sender, System.EventArgs e)
        {
            statusBarPanel4.Text = SystemBase.Base.RodeFormName.ToString();

            /////////////////////////탭관련////////////////////////////
            if (this.ActiveMdiChild == null)
            {
                tabForms.Visible = false;
                //TabHeight = 0;
            }
            else
            {
                if (this.ActiveMdiChild.Tag == null)
                {
                    C1.Win.C1Command.C1DockingTabPage tp = new C1DockingTabPage();
                    tp.Tag = this.ActiveMdiChild;
                    tp.Parent = tabForms;

                    tp.Text = SystemBase.Base.CodeName("MENU_ID", "MENU_NAME", "CO_SYS_MENU", this.ActiveMdiChild.Name, "");
                    tp.TabBackColor = System.Drawing.Color.FromArgb(88, 107, 137);
                    tp.TabBackColorSelected = System.Drawing.Color.FromArgb(255, 255, 255);
                    tp.TabForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    tp.TabForeColorSelected = System.Drawing.Color.FromArgb(0, 0, 0);

                    tabForms.SelectedTab = tp;
                   
                    this.ActiveMdiChild.Tag = tp;
                    this.ActiveMdiChild.Closed += new System.EventHandler(this.ActiveMdiChild_FormClosed);
                }
                else
                {
                    for (int i = 0; i < tabForms.TabPages.Count; i++)
                    {

                        if (tabForms.TabPages[i].Tag == this.ActiveMdiChild)
                            tabForms.SelectedIndex = i;
                    }
                }
                if (!tabForms.Visible) tabForms.Visible = true;
                //TabHeight = 26;
            }

            /////////////////////////탭관련////////////////////////////
        }


        /////////////////////////탭관련////////////////////////////
        private void tabForms_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((tabForms.SelectedTab != null) && (tabForms.SelectedTab.Tag != null))
                (tabForms.SelectedTab.Tag as Form).Select();
        }
       

        private void tabForms_DoubleClick(object sender, System.EventArgs e)
        {
            for (int i = 0; i < tabForms.TabPages.Count; i++)
            {
                if (tabForms.TabPages[i].Text == this.ActiveMdiChild.Text && tabForms.TabPages[i].Focused)
                {
                    this.ActiveMdiChild.Close();
                }
            }
        }

        private void ActiveMdiChild_FormClosed(object sender, System.EventArgs e)
        {
            for (int i = 0; i < tabForms.TabPages.Count; i++)
            {
                if (tabForms.TabPages[i].Text == this.ActiveMdiChild.Text)
                {
                    tabForms.TabPages[i].Dispose();
                }
            }
        }

        #region ************************************ 메뉴 이미지 이벤트 ******************************************

        //#region 공통관리
        //private void pictureBox1_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\co.png");
        //    pictureBox1.BackgroundImage = bitMap;
        //}
        //private void pictureBox1_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic1DownYn == "N")
        //    {
        //        pictureBox1.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic1DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\co.png");
        //    pictureBox1.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox2.BackgroundImage = null;
        //    pictureBox3.BackgroundImage = null;
        //    pictureBox4.BackgroundImage = null;
        //    pictureBox5.BackgroundImage = null;
        //    pictureBox6.BackgroundImage = null;
        //    Pic2DownYn = "N";
        //    Pic3DownYn = "N";
        //    Pic4DownYn = "N";
        //    Pic5DownYn = "N";
        //    Pic6DownYn = "N";

        //    Image_Menu_Create(0);

        //}
        //#endregion

        //#region 기준정보
        //private void pictureBox2_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ba.png");
        //    pictureBox2.BackgroundImage = bitMap;
        //}
        //private void pictureBox2_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic2DownYn == "N")
        //    {
        //        pictureBox2.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic2DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ba.png");
        //    pictureBox2.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox1.BackgroundImage = null;
        //    pictureBox3.BackgroundImage = null;
        //    pictureBox4.BackgroundImage = null;
        //    pictureBox5.BackgroundImage = null;
        //    pictureBox6.BackgroundImage = null;
        //    Pic1DownYn = "N";
        //    Pic3DownYn = "N";
        //    Pic4DownYn = "N";
        //    Pic5DownYn = "N";
        //    Pic6DownYn = "N";

        //    Image_Menu_Create(1);
        //}
        //#endregion

        //#region 수집관리
        //private void pictureBox3_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ga.png");
        //    pictureBox3.BackgroundImage = bitMap;
        //}

        //private void pictureBox3_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic3DownYn == "N")
        //    {
        //        pictureBox3.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic3DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ga.png");
        //    pictureBox3.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox1.BackgroundImage = null;
        //    pictureBox2.BackgroundImage = null;
        //    pictureBox4.BackgroundImage = null;
        //    pictureBox5.BackgroundImage = null;
        //    pictureBox6.BackgroundImage = null;
        //    Pic1DownYn = "N";
        //    Pic2DownYn = "N";
        //    Pic4DownYn = "N";
        //    Pic5DownYn = "N";
        //    Pic6DownYn = "N";

        //    Image_Menu_Create(2);
            
        //}
        //#endregion

        //#region 계산관리
        //private void pictureBox4_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ua.png");
        //    pictureBox4.BackgroundImage = bitMap;

        //}
        //private void pictureBox4_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic4DownYn == "N")
        //    {
        //        pictureBox4.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic4DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ua.png");
        //    pictureBox4.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox1.BackgroundImage = null;
        //    pictureBox2.BackgroundImage = null;
        //    pictureBox3.BackgroundImage = null;
        //    pictureBox5.BackgroundImage = null;
        //    pictureBox6.BackgroundImage = null;
        //    Pic1DownYn = "N";
        //    Pic2DownYn = "N";
        //    Pic3DownYn = "N";
        //    Pic5DownYn = "N";
        //    Pic6DownYn = "N";

        //    Image_Menu_Create(3);
        //}
        //#endregion

        //#region 검증관리
        //private void pictureBox5_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ve.png");
        //    pictureBox5.BackgroundImage = bitMap;
        //}
        //private void pictureBox5_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic5DownYn == "N")
        //    {
        //        pictureBox5.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic5DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\ve.png");
        //    pictureBox5.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox1.BackgroundImage = null;
        //    pictureBox2.BackgroundImage = null;
        //    pictureBox3.BackgroundImage = null;
        //    pictureBox4.BackgroundImage = null;
        //    pictureBox6.BackgroundImage = null;
        //    Pic1DownYn = "N";
        //    Pic2DownYn = "N";
        //    Pic3DownYn = "N";
        //    Pic4DownYn = "N";
        //    Pic6DownYn = "N";

        //    Image_Menu_Create(4);

        //}
        //#endregion

        //#region 전송관리
        //private void pictureBox6_MouseEnter(object sender, EventArgs e)
        //{
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\tr.png");
        //    pictureBox6.BackgroundImage = bitMap;

        //}
        //private void pictureBox6_MouseLeave(object sender, EventArgs e)
        //{
        //    if (Pic6DownYn == "N")
        //    {
        //        pictureBox6.BackgroundImage = null;
        //    }
        //}
        //private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Pic6DownYn = "Y";
        //    Bitmap bitMap = new Bitmap(SystemBase.Base.ProgramWhere + @"\images\image\tr.png");
        //    pictureBox6.BackgroundImage = bitMap;

        //    //해당 이미지외 제거
        //    pictureBox1.BackgroundImage = null;
        //    pictureBox2.BackgroundImage = null;
        //    pictureBox3.BackgroundImage = null;
        //    pictureBox4.BackgroundImage = null;
        //    pictureBox5.BackgroundImage = null;
        //    Pic1DownYn = "N";
        //    Pic2DownYn = "N";
        //    Pic3DownYn = "N";
        //    Pic4DownYn = "N";
        //    Pic5DownYn = "N";

        //    Image_Menu_Create(5);

        //}
        //#endregion

        #region 트리뷰 재생성
        private void Image_Menu_Create(int NodeNum)
        {
            string menuType = "S6";
            string Query = "exec usp_MAIN '" + menuType + "','" + SystemBase.Base.gstrUserID.ToString() + "', @pCO_CD = '"+ SystemBase.Base.gstrCOMCD.ToString() + "' ";
            ds = SystemBase.DbOpen.NoTranDataSet(Query);

            //	트리뷰 동적 생성
            treeView1.Nodes.Clear();
            DataView dvwData = null;
            UIForm.TreeView.CreateTreeView("*", (TreeNode)null, treeView1, ds, dvwData, 0);
            
            treeView1.Nodes[NodeNum].ExpandAll();

            c1DockingTab1.SlideShowPage(0);
        }

        private void Image_Menu_Create()
        {
            string menuType = "S6";
            string Query = "exec usp_MAIN '" + menuType + "','" + SystemBase.Base.gstrUserID.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'  ";
            ds = SystemBase.DbOpen.NoTranDataSet(Query);

            //	트리뷰 동적 생성
            treeView1.Nodes.Clear();
            DataView dvwData = null;
            UIForm.TreeView.CreateTreeView("*", (TreeNode)null, treeView1, ds, dvwData, 0);

            c1DockingTab1.SlideShowPage(0);
        }
        #endregion

        #region 새로고침
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Image_Menu_Create();
            MenuBarCreate();
        }
        #endregion

        #region 트리 노드 확장/확장닫기 시 이미지 On/Off
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.SelectedImageIndex == 0 || e.Node.SelectedImageIndex == 1)
            {
                if (e.Node.IsExpanded == true)
                {
                    e.Node.SelectedImageIndex = 1;
                }
                else
                {
                    e.Node.SelectedImageIndex = 0;
                }
            }
        }
        #endregion

        #endregion
    }

}
