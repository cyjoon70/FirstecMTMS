using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Globalization;


namespace E2MAXMenu
{
    public partial class Default : System.Windows.Forms.Form
    {
        public static Main frmMain = null;
        public static string MainLodeYN = "N";
        string Argument = "";
        // private System.ComponentModel.IContainer components = null;

        public static ArrayList arrProcessID = null;
        public static Excel.Application excelApp = null;
        public static Excel.Workbook excelWorkbook = null;
        public static Excel.Sheets excelSheets = null;
        public static Excel.Worksheet excelWorksheet = null;

        public Default()
        {
            InitializeComponent();
        }

        #region DllImport
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //이미실행중이면 화면 맨앞으로오게 하고
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void BringWindowToTop(IntPtr hWnd);
        //이미실행중이면 포커스(Activate)를 준다.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region Main
        //[STAThread]
        [STAThreadAttribute]
        static void Main() //dlr
        {
            try
            {        

                System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process[] proc2 = System.Diagnostics.Process.GetProcessesByName("E2MAXMenu");  //실행파일명
                int ProcessCnt = proc2.Length;  //프로세스 로드 수
                bool PSYN = true;// false인 경우 최대화 할 프로그램이 없음

                if (ProcessCnt > 1)
                {
                    for (int i = 0; i < ProcessCnt; i++)
                    {
                        if (proc2[i].Id.ToString() != currentProcess.Id.ToString())
                        {
                            SetForegroundWindow(proc2[i].MainWindowHandle);
                            PSYN = ShowWindow(proc2[i].MainWindowHandle, 3);//3- 최대화, 4-이전 크기로
                            BringWindowToTop(proc2[i].MainWindowHandle);

                            if (PSYN == false)
                            {
                                Application.Run(new Default());
                            }
                        }
                    }
                }
                else
                {
                    Application.Run(new Default());
                }
            }
            catch(Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region Default_Load
        private void Default_Load(object sender, System.EventArgs e)
        {
            try
            {
                string CmdAll = Environment.CommandLine.ToString().Trim();
                int Cnt = CmdAll.Trim().IndexOf(".exe", 0);

                if (CmdAll.Length > (Cnt + 5))
                {
                    Regex rx1 = new Regex(":");
                    string[] Parms = rx1.Split(CmdAll.Substring(Cnt + 5, CmdAll.Length - Cnt - 5).Trim().ToString());

                    Argument = Parms[0].ToString();

                    SystemBase.Base.gstrServerNM = Parms[1].ToString();//서버 ip
                    SystemBase.Base.gstrDbName = Parms[2].ToString();//접속서버정보
                    SystemBase.Base.gstrServerId = Parms[3].ToString();//사용자 ID 저장
                    SystemBase.Base.gstrServerPwd = Parms[4].ToString();//사용자 비밀번호
                }

                //아이콘 세팅 및 자동 시작프로그램 세팅
                LoadCls();

                System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("E2MAXMenu");  //실행파일명
                int ProcessCnt1 = proc.Length;  //프로세스 로드 수
                if (ProcessCnt1 > 1)
                {
                    frmMain.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    frmMain.TopMost = true;
                }
                else
                {
                    if (Argument == "ST")
                    {
                        LoginForm LForm = new LoginForm(SystemBase.Base.gstrServerNM, SystemBase.Base.gstrDbName, SystemBase.Base.gstrServerId, SystemBase.Base.gstrServerPwd);
                        LForm.ShowDialog();

                        if (LForm.DialogResult == DialogResult.OK)
                        {

                            frmMain = new Main();
                            frmMain.Show();
                            Default.MainLodeYN = "Y";

                            LForm.Close();
                        }
                    }
                }
            }
            catch (Exception f) 
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 아이콘 세팅 및 자동 시작프로그램 세팅
        public void LoadCls()
		{
            try
            {
                //SystemBase.Base.ProgramWhere = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

                //E2MaxIcon.Text = "E2Max-MTMS";
                //E2MaxIcon.ContextMenuStrip = this.ContextMenu;

                //RegistryKey Res = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                //if (Res.GetValue("MTMSMESSENGER") == null)
                //{
                //    string AppFolder2 = "\"" + SystemBase.Base.ProgramWhere + "\\E2MAXMenu.exe\"";
                //    Res.SetValue("MTMSMESSENGER", AppFolder2);
                //}

                this.Visible = false;
                this.ShowInTaskbar = false;

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
		}
		#endregion

        #region 암호화EnCode
        public static string EnCode(string Str)
        {//암호화	EnCode(textBox2.Text);
            int[] NanSuArr = new int[] { 5, 7, 0, 6, 1, 8, 3, 4, 9, 2 };
            string RtnStr = "";

            Random rnd = new Random();
            for (int i = 0; i < Str.Length; i++)
            {
                string Tmp = Str.Substring(i, 1);
                int NanSu = rnd.Next(9);
                int TmpNanSu = NanSu + 65;
                string FirstStr = Convert.ToChar(TmpNanSu).ToString();

                int SecondMod = (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29) + 65 + NanSuArr[NanSu];
                string SecondStr = Convert.ToChar(SecondMod).ToString();

                int Thirdint = ((Convert.ToInt32(Convert.ToChar(Tmp.ToString())) - (Convert.ToInt32(Convert.ToChar(Tmp.ToString())) % 29)) / 29) + 76 + NanSuArr[NanSu];
                string ThirdStr = Convert.ToChar(Thirdint).ToString();

                RtnStr = RtnStr + FirstStr.ToString() + SecondStr.ToString() + ThirdStr.ToString();
            }
            return RtnStr;
        }
        #endregion

        #region 복호화 DeCode
        public static string DeCode(string Str)
        {//복호화	DeCode(textBox1.Text);
            int[] NanSuArr = new int[] { 5, 7, 0, 6, 1, 8, 3, 4, 9, 2 };
            string RtnStr = "";
            try
            {
                //Random rnd = new Random();
                for (int i = 0; i < Str.Length / 3; i++)
                {
                    string Tmp1 = Str.Substring(((i + 1) * 3) - 3, 1);
                    int First = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp1))) - 65;

                    string Tmp2 = Str.Substring(((i + 1) * 3) - 2, 1);
                    int Secondint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp2))) - 65 - NanSuArr[First];

                    string Tmp3 = Str.Substring(((i + 1) * 3) - 1, 1);
                    int Thirdint = Convert.ToChar(Convert.ToInt32(Convert.ToChar(Tmp3))) - 76 - NanSuArr[First];

                    RtnStr = RtnStr + Convert.ToChar(Convert.ToInt32(Convert.ToChar(((Thirdint * 29) + Secondint)))).ToString();
                }
            }
            catch
            {
                RtnStr = "";
            }
            return RtnStr;
        }
        #endregion

        #region 마우스 오른쪽 클릭 메뉴
        private void e2Max실행ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmMain == null || MainLodeYN == "N" || MainLodeYN == null)
                {
                    System.Diagnostics.Process.Start(SystemBase.Base.ProgramWhere + "\\E2MAXMenu.exe");
                }
                else
                {
                    frmMain.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    frmMain.TopMost = true;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        private void 리스트ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (MessengerListLodeYN == null || MessengerListLodeYN != "Y")
            //    {
            //        frmMsgList = new MessengerList();
            //        frmMsgList.Show();
            //        frmMsgList.TopMost = true;
            //        frmMsgList.timer1.Start();
            //    }
            //    else
            //    {
            //        frmMsgList.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //        frmMsgList.TopMost = true;
            //        frmMsgList.timer1.Start();
            //    }
            //}
            //catch (Exception f)
            //{
            //    MessageBox.Show(f.ToString());
            //}
        }

        private void 환경설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //if (CHKTime.Items.Count == 0)
                //    LoadCls();

                //DataTable dtMSG = SystemBase.DbOpen.MSGMDBOpen(SystemBase.Base.ProgramWhere + "\\Messenger.mdb");
                //if (dtMSG.Rows.Count > 0)
                //{
                //    txtUSR_ID.Text = dtMSG.Rows[0][0].ToString();
                //    txtUSR_PWD.Text = SystemBase.Base.DeCode(dtMSG.Rows[0][1].ToString());
                //    CHKTime.SelectedValue = dtMSG.Rows[0][2].ToString();
                //}

                this.Visible = true;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string message = "종료하시면 MTMS와 알리미가 동시에 종료됩니다.\n\n정말로 종료하시겠습니까?";
                string caption = "[E2Max-MTMS]Question";

                // 메세지 박스 YES/NO
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // 메세지 박스 보여줌 
                result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Dispose(true);
                    Application.Exit();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 아이콘 더블 클릭시 E2Max실행
        private void E2MaxIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (frmMain == null || MainLodeYN == null || MainLodeYN == "N")
                {
                    System.Diagnostics.Process.Start(SystemBase.Base.ProgramWhere + "\\E2MAXMenu.exe");
                }
                else
                {
                    frmMain.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    frmMain.TopMost = true;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion
    }
}
