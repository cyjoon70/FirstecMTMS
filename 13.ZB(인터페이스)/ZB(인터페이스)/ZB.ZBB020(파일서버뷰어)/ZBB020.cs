

#region 작성정보
/*********************************************************************/
// 단위업무명 : 파일서버뷰어
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-05-20
// 작성내용 : 파일서버뷰어
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 : 
/*********************************************************************/
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;

using Microsoft.Win32;


using System.Collections.Generic;




namespace ZB.ZBB020
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NETRESOURCE
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        public string lpLocalName;
        public string lpRemoteName;
        public string lpComment;
        public string lpProvider;
    }

    public partial class ZBB020 : UIForm.Buttons 
    {
        string server = ""; //@"\\172.30.24.16";
        string server_id = "";
        string server_password = "";
        string server_root_drive = "";
        int Connect_Result = 1;
        private bool backCalled = false; 

        //WindowsImpersonationContext o;

        public ZBB020()
        {
            InitializeComponent();
        }

        #region API 함수 선언 공유, 공유해제
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetUseConnection(
                    IntPtr hwndOwner,
                    [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
                    string lpPassword,
                    string lpUserID,
                    uint dwFlags,
                    StringBuilder lpAccessName,
                    ref int lpBufferSize,
                    out uint lpResult);
        // API 함수 선언 (공유해제)
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Auto)]
        public static extern int WNetCancelConnection2A(string lpName, int dwFlags, int fForce);
        #endregion

        #region Form Load 시
        private void ZBB020_Load(object sender, System.EventArgs e)
        {
            try
            {
                
                if (SystemBase.Base.gstrUserID == "ADMIN")
                {
                    btnFileServerSetting.Visible = true;
                }
                else
                {
                    btnFileServerSetting.Visible = false;
                }
                if (SERVER_INFO_GET() == false)
                {
                    MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                txtPath.Enabled = false;
                int capacity = 64;
                uint resultFlags = 0;
                uint flags = 0;
                System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
                NETRESOURCE ns = new NETRESOURCE();
                ns.dwType = 1;           // 공유 디스크
                ns.lpLocalName = null;   // 로컬 드라이브 지정하지 않음
                ns.lpRemoteName = @server;
                ns.lpProvider = null;

                int Connect_Result = WNetUseConnection(IntPtr.Zero, ref ns, @server_password, server_id, flags, sb, ref capacity, out resultFlags);
                //int result = WNetUseConnection(IntPtr.Zero, ref ns, @"APP_\!#816VJ", "administrator", flags, sb, ref capacity, out resultFlags);

                if (Connect_Result != 0)
                {
                    MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                wbrPdf.IsWebBrowserContextMenuEnabled = false;

                //o = Network.LogonUser("administrator", @"APP_\!#816VJ", @"\\172.30.24.16\File Server");
                TreeNode root = tvFolders.Nodes.Add("파일서버");
                string drives = @server_root_drive;
                TreeNode node = root.Nodes.Add(drives);
                node.Nodes.Add("@%");
                root.Tag = "0";
                node.Tag = "0";
                root.Expand();
                node.Expand();

                //string[] drives = Directory.GetLogicalDrives();
                //foreach (string drive in drives)
                //{
                //    TreeNode node = root.Nodes.Add(drive);
                //    node.Nodes.Add("@%");
                //}

                /*
                string drives = @"\\172.30.24.16\File Server";

                TreeNode root = tvFolders.Nodes.Add(drives);
            
                string[] directories =
                            Directory.GetDirectories(drives);
                foreach (string directory in directories)
                {
                    TreeNode newNode = root.Nodes.Add(
                        directory.Substring(
                        directory.LastIndexOf("\\") + 1));

                    newNode.Nodes.Add("@%");
                }
                */
                //tvFolders.ExpandAll();
                

                SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
                SystemBase.Validation.GroupBox_Setting(groupBox2);//필수 적용
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                wbrPdf.Navigate("about:Tabs");
                tvFolders.Nodes.Clear();
                lvFiles.Items.Clear();

                if (Connect_Result != 0)
                {
                    MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                TreeNode root = tvFolders.Nodes.Add("파일서버");
                string drives = @server_root_drive;
                TreeNode node = root.Nodes.Add(drives);
                node.Nodes.Add("@%");
                root.Tag = "0";
                node.Tag = "0";
                root.Expand();
                node.Expand();

                SystemBase.Validation.GroupBox_Reset(groupBox1);//필수 적용
                SystemBase.Validation.GroupBox_Reset(groupBox2);//필수 적용
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 폴더 선택시
        private void tvFolders_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            try
            {
                TreeNode current = e.Node;
                Folders_Open(current, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /*
            if (current.Nodes.Count == 1 &&
                 current.Nodes[0].Text.Equals("@%"))
            {
                current.Nodes.Clear();

                String path = current.FullPath.Substring(
                     current.FullPath.IndexOf("\\") + 1);

                try // 하위 장치(목록)이 없을 경우 예외 처리
                {
                    string[] directories =
                        Directory.GetDirectories(path);
                    foreach (string directory in directories)
                    {
                        TreeNode newNode = current.Nodes.Add(
                            directory.Substring(
                            directory.LastIndexOf("\\") + 1));
                        
                        newNode.Nodes.Add("@%");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
             * */
        }

        public void Folders_Open(TreeNode current, int iOpenCnt)
        {
            try
            {

                //if (current.Nodes.Count == 1 &&
                // current.Nodes[0].Text.Equals("@%"))
                {
                    if (current.Nodes[0].Tag == null || current.Nodes[0].Tag.ToString() == "1")
                        current.Nodes.Clear();

                    String path = current.FullPath.Substring(
                         current.FullPath.IndexOf("\\") + 1);
                    if (path == "파일서버")
                    {
                        return;
                    }
                    else if(current.Nodes.Count != 0)
                    {
                        if (current.Nodes[0].Tag.ToString() == "0") return;
                        current.Nodes[0].Tag = Convert.ToString(iOpenCnt);
                    }
                    
                    string[] directories =
                        Directory.GetDirectories(path);
                    //if (directories.Length == 0) current.ExpandAll();
                    foreach (string directory in directories)
                    {
                        TreeNode newNode = current.Nodes.Add(
                            directory.Substring(
                            directory.LastIndexOf("\\") + 1));
                        newNode.Nodes.Add("@%");
                        if (iOpenCnt == 0) Folders_Open(newNode, 1);
                        newNode.Tag = Convert.ToString(iOpenCnt);
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 파일 리스트 조회
        private void tvFolders_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                TreeNode current = e.Node;
                string path = current.FullPath;

                if (path == "파일서버")
                {
                    txtPath.Enabled = true;
                    txtPath.Text = "";
                    txtPath.Enabled = false;
                    return;
                }
                txtPath.Enabled = true;
                txtPath.Text =
                    @path.Substring(path.IndexOf("\\") + 1);
                txtPath.Enabled = false;

                lvFiles.Items.Clear();

                ////디렉토리 목록 표시
                // string[] directories =
                //     Directory.GetDirectories(txtPath.Text);
                // foreach (string directory in directories)
                // {
                //     DirectoryInfo info =
                //         new DirectoryInfo(directory);
                //     ListViewItem item =
                //         new ListViewItem(new string[]
                //     {
                //         info.Name, "",
                //         "파일폴더", info.LastWriteTime.ToString()
                //     });
                //     lvFiles.Items.Add(item);
                // }


                //파일 목록 표시
                string[] files = Directory.GetFiles(@txtPath.Text);

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    ListViewItem item =
                        new ListViewItem(new string[]
                     {
                         info.Name, info.Length.ToString(),
                         info.Extension, info.LastWriteTime.ToString(),
                         info.Directory.ToString()
                     });
                    lvFiles.Items.Add(item);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 파일 더블클릭시 파일 미리보기
        private void lvFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string strFileNm = lvFiles.FocusedItem.SubItems[0].Text;
                string strPath = lvFiles.FocusedItem.SubItems[4].Text;
                string strFileFullNm = @strPath + @"\" + strFileNm;

                string tmpStr = "file:///" + strFileFullNm;

                if(Save_File_Access(strFileFullNm) == true)
                    wbrPdf.Navigate(tmpStr,false);

                //object flag = 0;
                //object targetFrameName = null;
                //object postData = null;
                //object headers = null;
                //this.axWebBrowser1.Navigate(tmpStr, ref flag, ref targetFrameName, ref postData, ref headers);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 파일접근 업데이트
        public bool Save_File_Access(string FILE_NM)
        {
            this.Cursor = Cursors.WaitCursor;

            string msg = SystemBase.Base.MessageRtn("B0027");

            //if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_ZBB020  'I1'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pUSER_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    strSql += ", @pFILE_INFO = '" + FILE_NM + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                    this.Cursor = Cursors.Default;
                    return false;
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    this.Cursor = Cursors.Default;
                    return true;
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
            return false;
        }
        #endregion

        #region 파일서버 정보 조회
        public bool SERVER_INFO_GET()
        {
            try
            {
                string strQuery = " usp_ZBB020  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    server = dt.Rows[0]["SERVER_DOMAIN"].ToString();
                    server_id = dt.Rows[0]["USER_ID"].ToString();
                    server_password = SystemBase.Base.Decode(dt.Rows[0]["PASSWORD"].ToString());
                    server_root_drive = dt.Rows[0]["ROOT_DRIVE"].ToString();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        #endregion

        #region Form Closed
        private void ZBB020_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                WNetCancelConnection2A(server, 1, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 백스페이스 막기
        private void wbrPdf_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                backCalled = true;
            }
            else
            {
                backCalled = false;
            }
        }

        private void wbrPdf_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (backCalled)
            {
                e.Cancel = true;
                backCalled = false;
            }
        }
        #endregion

        #region 파일접근이력조회
        private void btnFile_Access_Info_Click(object sender, EventArgs e)
        {
            ZBB020P1 pu = new ZBB020P1();
            pu.MaximizeBox = false;
            pu.Width = 1000;
            pu.Height = 800;
            pu.ShowDialog();
        }
        #endregion

        #region 파일서버정보 조회 및 저장
        private void btnFileServerSetting_Click(object sender, EventArgs e)
        {
            ZBB020P2 pu = new ZBB020P2();
            pu.MaximizeBox = false;
            pu.Width = 720;
            pu.Height = 240;
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
                string strServer_Change = pu.SERVER_CHANGE;
                if (strServer_Change == "Y")
                {
                    if (SERVER_INFO_GET() == false)
                    {
                        MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    txtPath.Enabled = false;
                    int capacity = 64;
                    uint resultFlags = 0;
                    uint flags = 0;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
                    NETRESOURCE ns = new NETRESOURCE();
                    ns.dwType = 1;           // 공유 디스크
                    ns.lpLocalName = null;   // 로컬 드라이브 지정하지 않음
                    ns.lpRemoteName = @server;
                    ns.lpProvider = null;

                    Connect_Result = WNetUseConnection(IntPtr.Zero, ref ns, @server_password, server_id, flags, sb, ref capacity, out resultFlags);
                    //int result = WNetUseConnection(IntPtr.Zero, ref ns, @"APP_\!#816VJ", "administrator", flags, sb, ref capacity, out resultFlags);

                    if (Connect_Result != 0)
                    {
                        MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    NewExec();
                }
            }
        }
        #endregion
    }

    //사용하지 않음..(공유 폴더 설정 다른 방법)
    class Network
    {
        [DllImport("advapi32.dll", EntryPoint = "LogonUser", SetLastError = true)]
        private static extern bool _LogonUser(string username, string domain, string password, int type, int provider, out int token);

        public static WindowsImpersonationContext LogonUser(string userName, string password, string domainName)
        {
            int token = 0;
            bool logonSuccess = _LogonUser(userName, domainName, password, 9, 0, out token);
            if (logonSuccess)
                return WindowsIdentity.Impersonate(new IntPtr(token));
            int retval = Marshal.GetLastWin32Error();
            return null;
        }
        public static void LogOutUser(WindowsImpersonationContext context)
        {
            context.Undo();
        }
    }
}
