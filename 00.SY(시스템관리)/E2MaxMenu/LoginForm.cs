using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Management;
using System.Net;
using SystemBase;
using System.Globalization;
using BB.BBB006;

namespace E2MAXMenu
{
    public partial class LoginForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// 비밀번호 입력오류처리
        /// </summary>
        public enum PWErrorYn
        {
            /// <summary>OK</summary>
            OK,
            /// <summary>Error</summary>
            Error
        }
        PWErrorYn ErrMode = PWErrorYn.OK;


        protected Bitmap BackgroundBitmap = null;
        string AppFolder = "";
        string SaveId = "N";

        string strIp = "", strDbName = "", strId = "", strPwd = "";
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;

        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        //private System.ComponentModel.Container components = null;


        public LoginForm(string Ip, string DbName, string Id, string Pwd)
        {
            strIp = Ip;
            strDbName = DbName;
            strId = Id;
            strPwd = Pwd;

            InitializeComponent();

            try
            {
                AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            }
            catch { }
        }

        public LoginForm()
        {

            InitializeComponent();

            try
            {
                AppFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            }
            catch { }
        }

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>

        public void Login()
        {
            try
            {
                int CK_ID = txtID.Text.LastIndexOf("'");
                if (CK_ID > -1)
                {
                    MessageBox.Show(@"(')는 ID로 사용할수 없는 문자열입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string Query = "EXEC usp_USERLOGIN @pType='S1', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
                    
                    if (dt.Rows.Count > 0)
                    {
                        if (!PWErrorSkipYn("N")) return;  // 비밀번호 5회 이상 오류시 로그인 못함... 관리자에게 문의처리

                        string Encode = SystemBase.Base.DeCode(dt.Rows[0][0].ToString());


						if (Encode == txtPW.Text)
                        {
                            //////////////////////////보안관련/////////////////////////
                            //ObjectQuery oq = new System.Management.ObjectQuery("SELECT MACAddress, AdapterTypeID FROM Win32_NetworkAdapter");
                            //ManagementObjectSearcher query1 = new ManagementObjectSearcher(oq);

                            //foreach (ManagementObject mo in query1.Get())
                            //{
                            //    if ((mo["MACAddress"] != null) && (mo["AdapterTypeID"].ToString() == "0"))
                            //    {
                            //        SystemBase.Base.gstrMacAddress = mo["MACAddress"].ToString();
                            //        break;
                            //    }
                            //}

                            ////IPHostEntry ipHostEntry = Dns.GetHostByName(Dns.GetHostName());
                            //IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                            //IPAddress[] ipAddress = ipHostEntry.AddressList;

                            //for (int i = 0; i < ipAddress.Length; i++)
                            //{
                            //    SystemBase.Base.gstrUserIp = ipAddress[i].ToString();
                            //}

                            //string CKQuery = "usp_USERLOGIN 'S4', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
                            //DataTable CKdt = SystemBase.DbOpen.NoTranDataTable(CKQuery);	// Update IP ADDRESS, Mac Address

                            //if (CKdt.Rows[0]["IP_FLAG"].ToString() == "True" && CKdt.Rows[0]["IP_ADDRESS"].ToString() != SystemBase.Base.gstrUserIp)
                            //{
                            //    MessageBox.Show(SystemBase.Base.MessageRtn("SY004"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//등록된 IP가 아니므로 접속하실 수 없습니다.
                            //    return;
                            //}

                            //if (CKdt.Rows[0]["MAC_FLAG"].ToString() == "True" && CKdt.Rows[0]["MAC_ADDRESS"].ToString() != SystemBase.Base.gstrMacAddress)
                            //{
                            //    MessageBox.Show(SystemBase.Base.MessageRtn("SY005"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//등록된 컴퓨터가 아니므로 접속하실 수 없습니다.
                            //    return;
                            //}
                            ////////////////////////보안관련/////////////////////////
                            SystemBase.Base.ProgramWhere = AppFolder;			        //프로그램 위치
                            E2MAXMenu.Main.UserID = txtID.Text;					        //유저ID
                            E2MAXMenu.Main.UserName = dt.Rows[0][1].ToString();	        //유저명
                            SystemBase.Base.gstrServerNM = strIp.Trim();				//서버IP
                            SystemBase.Base.gstrUserID = txtID.Text;					//유저ID
                            SystemBase.Base.gstrUserName = dt.Rows[0][1].ToString();			//사용자명

                            SystemBase.Base.gstrCOMCD = "";			//법인코드
                            SystemBase.Base.gstrBIZCD = "";			//사업장코드
                            SystemBase.Base.gstrBIZNM = "";			//사업장명
                            SystemBase.Base.gstrPLANT_CD = "";			//공장코드
                            SystemBase.Base.gstrREORG_ID = "";			//부서개편ID
                            SystemBase.Base.gstrDEPT = "";			//부서코드
                            SystemBase.Base.gstrDEPTNM = "";			//부서명

                            SystemBase.Base.gstrCOMCD = cboCompCd.SelectedValue.ToString();			//법인코드
                            SystemBase.Base.gstrCOMNM = cboCompCd.SelectedText;			//법인명
                            SystemBase.Base.gstrBIZCD = dt.Rows[0][4].ToString();			//사업장코드
                            SystemBase.Base.gstrBIZNM = dt.Rows[0][5].ToString();			//사업장명
                            SystemBase.Base.gstrPLANT_CD = dt.Rows[0][9].ToString();			//공장코드
                            SystemBase.Base.gstrREORG_ID = dt.Rows[0][6].ToString();			//부서개편ID
                            SystemBase.Base.gstrDEPT = dt.Rows[0][7].ToString();			//부서코드
                            SystemBase.Base.gstrDEPTNM = dt.Rows[0][8].ToString();			//부서명

                            //string LoginQuery = "usp_USERLOGIN 'U1', @pUSR_ID='" + SystemBase.Base.gstrUserID + "', @pMAXADDRESS='" + SystemBase.Base.gstrMacAddress + "', @pUSERIP='" + SystemBase.Base.gstrUserIp + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "'";
                            //SystemBase.DbOpen.NoTranNonQuery(LoginQuery);	// Update IP ADDRESS, Mac Address

                            if (chkSaveId.Checked == true)
                            {
                                SetIniValue("DATABASE", "SaveId", "Y", SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
                                SetIniValue("DATABASE", "UserId", SystemBase.Base.gstrUserID, SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
                            }
                            else
                            {
                                SetIniValue("DATABASE", "SaveId", "N", SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
                                SetIniValue("DATABASE", "UserId", "", SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
                            }

                            SetIniValue("DATABASE", "ComCd", SystemBase.Base.gstrCOMCD, SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");


                            ////////////////////////////////////////////////////////////////
                            // 데이타베이스명 임시 MTMS_FT_TEST => MTMS_FT로 강제 변경
                            ////////////////////////////////////////////////////////////////
                            //if (txtID.Text != "ADMIN")            // 2021.09.29. hma 수정: 테스트 위해 주석 처리함!!! 운영 적용시 주석 해제 필요!!!
                            //{
                            //    SystemBase.Base.gstrDbName = "MTMS_FT";
                            //    strDbName = "MTMS_FT";
                            //    SetIniValue("DATABASE", "Database", SystemBase.Base.gstrDbName, SystemBase.Base.ProgramWhere + "\\E2MAX_FTP.ini");
                            //    SystemBase.Base.gstrDbConn = "server=" + strIp.Trim() + ";uid=" + strId.Trim() + ";pwd=" + strPwd.Trim() + ";database=" + strDbName.Trim() + " ";
                            //}
                            ////////////////////////////////////////////////////////////////


                            // 비밀번호 변경
                            if (!PWChange()) return;
                            ErrMode = PWErrorYn.OK;
                            PWErrorCount();

                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("비밀번호가 일치하지 않습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ErrMode = PWErrorYn.Error;
                            PWErrorCount();
                            txtPW.Text = "";
                        }
                    }
                    else
                    {
                        MessageBox.Show("등록된 ID가 없습니다.\n\nID를 다시한번 확인해 보세요.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("데이타베이스 접속실패입니다. 서버 접속정보를 확인해 보세요.", "[MTMS]Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log("로그인 실패", e.ToString());
                MessageBox.Show(e.ToString());
            }
        }

        private void cboCompCd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtID.Focus();
            }
        }

        private void txtID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPW.Focus();
            }
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
            }
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            Login();
        }

        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void pictureBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        private void pictureBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        }

        private void pictureBox2_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           // this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }


        #region 이미지 위치
        public void SetBackgroundBitmap(string strFilename, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(strFilename);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        public void SetBackgroundBitmap(Image image, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(image);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int height = bitmap.Height;
            int width = bitmap.Width;

            GraphicsPath path = new GraphicsPath();

            for (int j = 7; j < height; j++)
                for (int i = 5; i < width; i++)
                {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;

                    int x0 = i;

                    while ((i < width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;

                    path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }

            Region region = new Region(path);
            path.Dispose();
            return region;
        }
        #endregion

        private void pictureBox1_MouseLeave(object sender, System.EventArgs e)
        {
            this.pictureBox1.BackgroundImage = null;
        }

        private void pictureBox2_MouseLeave(object sender, System.EventArgs e)
        {
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;

        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            Bitmap bitMap = new Bitmap(AppFolder+ @"\images\image\login_btn_over.gif");
            pictureBox1.BackgroundImage = bitMap;
        }

        private void LoginForm_Load(object sender, System.EventArgs e)
        {
            Bitmap bitMap = new Bitmap(AppFolder + @"\images\image\login(퍼스텍).jpg");
            panel1.BackgroundImage = bitMap;

            //임시
            //txtID.Text = "ADMIN";
            //txtPW.Text = "gksaldo";

            SystemBase.Base.gstrDbConn = "server=" + strIp.Trim() + ";uid=" + strId.Trim() + ";pwd=" + strPwd.Trim() + ";database=" + strDbName.Trim() + " ";

            string Query = "SELECT CO_CD, CO_NM FROM B_COMP_INFO(NOLOCK)";
            SystemBase.ComboMake.C1Combo(cboCompCd, Query);

            cboCompCd.Splits[0].DisplayColumns[0].Width = 0;

            ReadINI();

            if (SaveId == "Y")
            {
                chkSaveId.Checked = true;
                txtID.Text = SystemBase.Base.gstrUserID.ToString();
            }
            else
            {
                chkSaveId.Checked = false;
                txtID.Text = "";
            }

            cboCompCd.SelectedValue = SystemBase.Base.gstrCOMCD.ToString();

            txtPW.Focus();
        }

        #region INI 값 읽기, 설정
        // INI 값 읽기, 설정 
        public void SetIniValue(String Section, String Key, String Value, String iniPath)
        {
            SystemBase.Base.WritePrivateProfileString(Section, Key, Value, iniPath);
        }

        public void ReadINI()
        {
            StreamReader objReader = new StreamReader(AppFolder + "\\E2MAX_FTP.ini");
            string sLine = "";
            ArrayList arrText = new ArrayList();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    arrText.Add(sLine);

                    if (sLine.Length > 6 && sLine.Substring(0, 6).ToString() == "SaveId")
                    {
                        string[] strTemp = sLine.Split('=');
                        SaveId = strTemp[1].Trim();
                    }

                    if (sLine.Length > 5 && sLine.Substring(0, 5).ToString() == "ComCd")
                    {
                        string[] strTemp = sLine.Split('=');
                        SystemBase.Base.gstrCOMCD = strTemp[1].Trim();
                    }

                    if (sLine.Length > 6 && sLine.Substring(0, 6).ToString() == "UserId")
                    {
                        string[] strTemp = sLine.Split('=');
                        SystemBase.Base.gstrUserID = strTemp[1].Trim();
                    }
                }
            }
            objReader.Close();
        }
        #endregion



        #region 비밀번호 변경처리 / 오류횟수 관리
        /// <summary>
        /// 비밀번호 변경 처리
        /// </summary>
        bool PWChange()
        {
            // 기존 비밀번호를 입력해서 일치했을 경우
            // 비밀번호 변경유효일수 체크 (90일 )
            string PwQuery = "EXEC usp_USERLOGIN @pType='S5', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
            DataTable Pwdt = SystemBase.DbOpen.NoTranDataTable(PwQuery);
            int ChangeDays = Convert.ToInt32(Pwdt.Rows[0][0].ToString());
            if (ChangeDays > 90)
            {
                MessageBox.Show("비밀번호 변경기간이(90일) 지났습니다. 변경 후 로그인 하세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                BBB006 PWDialog = new BBB006(txtID.Text);
                PWDialog.ShowDialog();
                if (PWDialog.DialogResult != DialogResult.OK) return false;

                MessageBox.Show("변경된 비밀번호로 다시 로그인 바랍니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 비밀번호 오류횟수 관리
        /// </summary>
        void PWErrorCount()
        {
            string Type = null;
            switch (ErrMode)
            {
                case PWErrorYn.OK:  // 비밀번호 입력오류 횟수 초기화
                    Type = "U4";
                    break;

                case PWErrorYn.Error: // 비밀번호 입력오류 ++
                    Type = "U3";
                    break;
            }

            string PwQuery = "EXEC usp_USERLOGIN @pType='" + Type + "', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
            DataTable Pwdt = SystemBase.DbOpen.NoTranDataTable(PwQuery);

            PWErrorSkipYn("Y");
        }

        /// <summary>
        /// 비밀번호 오류횟수에 의한 Skip 여부
        /// </summary>
        bool PWErrorSkipYn(string SkipYn)
        {
            // 오류횟수 메시지 처리
            string PwQuery = "EXEC usp_USERLOGIN @pType='S6', @pUSR_ID='" + txtID.Text + "', @pCO_CD = '" + cboCompCd.SelectedValue.ToString() + "' ";
            DataTable PwErrordt = SystemBase.DbOpen.NoTranDataTable(PwQuery);
            int Errorcnt = Convert.ToInt32(PwErrordt.Rows[0][0].ToString());

            // 5회 미만일 경우
            if (Errorcnt > 0  && SkipYn.Equals("Y") )
            {
                MessageBox.Show($"비밀번호 입력오류 ({Errorcnt}) 회입니다. 5회 이상 오류시 관리자에게 문의바랍니다.", "오류횟수", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }

            //5회 이상일 경우 로그인 못함.
            if (Errorcnt >= 5 && SkipYn.Equals("N"))
            {
                MessageBox.Show($"비밀번호 입력오류 ({Errorcnt}) 회입니다. 관리자에게 문의바랍니다.", "오류횟수", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        #endregion
    }
}
