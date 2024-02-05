
#region 작성정보
/*********************************************************************/
// 단위업무명 : 파일등록
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-17
// 작성내용 : 파일등록삭제 관리
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
using System.Net;

using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.Reflection;

using Microsoft.Win32;


using System.Collections.Generic;

namespace ZB.ZBB030
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

    public partial class ZBB030 : UIForm.Buttons
    {
        #region 변수선언
        string server = ""; //@"\\172.30.24.16";
        string server_id = "";
        string server_password = "";
        string server_root_drive = "";
        int Connect_Result = 1;
        private bool backCalled = false; 

        string     ftpServerIP = "172.30.24.14";   //FTP 서버주소
        string     ftpUserID = "";     //아이디
        string     ftpPassword = "";  //패스워드
        string     ftpPort = "21";        //포트
        bool     usePassive = true;   //패시브모드 사용여부
        #endregion

        //WindowsImpersonationContext o;

        public ZBB030()
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
        private void ZBB030_Load(object sender, System.EventArgs e)
        {
            try
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

                int Connect_Result = WNetUseConnection(IntPtr.Zero, ref ns, @server_password, server_id, flags, sb, ref capacity, out resultFlags);
                //int result = WNetUseConnection(IntPtr.Zero, ref ns, @"\\_DEFCOST@!", "administrator", flags, sb, ref capacity, out resultFlags);

                if (Connect_Result != 0 && Connect_Result != 1219)
                {
                    MessageBox.Show("파일서버에 접속할 수 없습니다. 관리자에 문의하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //o = Network.LogonUser("administrator", @"APP_\!#816VJ", @"\\172.30.24.16\File Server");
                TreeNode root = tvFolders.Nodes.Add("파일서버");
                string drives = @server_root_drive;
                TreeNode node = root.Nodes.Add(drives);
                node.Nodes.Add("@%");
                root.Tag = "0";
                node.Tag = "0";
                root.Expand();
                node.Expand();

                InitiateTree(DirTreeView);

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
                    ftpUserID = dt.Rows[0]["USER_ID"].ToString();
                    server_password = SystemBase.Base.Decode(dt.Rows[0]["PASSWORD"].ToString());
                    ftpPassword = SystemBase.Base.Decode(dt.Rows[0]["PASSWORD"].ToString());
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
        private void ZBB030_FormClosed(object sender, FormClosedEventArgs e)
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

        #region 버튼클릭
        private void btnUpload_Click(object sender, EventArgs e)
        {
//			OpenFileDialog dlg = new OpenFileDialog();
            if (txtPath.Text == "")
            {
                MessageBox.Show("업로드대상 폴더가 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
//            if (dlg.ShowDialog() == DialogResult.OK)
//            {
//                string[] fileNames = dlg.FileNames;
//                Upload(txtPath.Text.Substring(15).Replace("\\","/"), fileNames[0]);
//            }
            try
            {
                string strFileNm = FileListView.FocusedItem.SubItems[0].Text;
                string strPath = FileListView.FocusedItem.SubItems[4].Text;
                string strFileFullNm = strPath + "\\" + strFileNm;
                if (strPath == "")
                {
                    MessageBox.Show("업로드대상 파일이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Upload(txtPath.Text.Substring(15).Replace("\\", "/"), strFileFullNm);
            }
            catch(Exception f)
            {
                MessageBox.Show(f.ToString());
                //MessageBox.Show("업로드대상 파일이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        private void btnFileDel_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileNm = lvFiles.FocusedItem.SubItems[0].Text;
                string strPath = lvFiles.FocusedItem.SubItems[4].Text.Substring(2).Replace("\\","/");
                string strFileFullNm = strPath + "/" + strFileNm;
                if (strPath == "")
                {
                    MessageBox.Show("삭제대상 파일이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dsMsg = MessageBox.Show("해당 파일을 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    DeleteFTP(strFileFullNm);
                }
            }
            catch
            {
                MessageBox.Show("삭제대상 파일이 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnMakeDir_Click(object sender, EventArgs e)
        {
            try
            {
                string strPath = txtPath.Text.Substring(2).Replace("\\", "/");
                if (strPath == "")
                {
                    MessageBox.Show("상위 폴더가 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtDir.Text == "")
                {
                    MessageBox.Show("폴더명이 입력되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDir.Focus();
                    return;
                }
                MakeDir(strPath + "/" + txtDir.Text);
            }
            catch
            {
                MessageBox.Show("상위 폴더가 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void btnDeleteDir_Click(object sender, EventArgs e)
        {
            try
            {
                string strPath = txtPath.Text.Substring(2).Replace("\\", "/");
                if (strPath == "")
                {
                    MessageBox.Show("삭제대상 폴더가 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult dsMsg = MessageBox.Show("해당 폴더를 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    DeleteDir(strPath);
                }
            }
            catch
            {
                MessageBox.Show("삭제대상 폴더가 선택되지 않았습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        #endregion

        //파일 업로드
         public Boolean Upload(string folder,string filename)
         {
             FileInfo fileInf = new FileInfo(filename);
             //folder = folder.Replace("File Server", "");
             string uri = "ftp://" + ftpServerIP + ":"+ftpPort+"/"+folder+"/" + fileInf.Name;
             FtpWebRequest reqFTP;
 
            // Create FtpWebRequest object from the Uri provided
             reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
 
            // Provide the WebPermission Credintials
             reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            
            // By default KeepAlive is true, where the control connection is not closed
             // after a command is executed.
             reqFTP.KeepAlive = false;
 
            // Specify the command to be executed.
             reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
 
            // Specify the data transfer type.
             reqFTP.UseBinary = true;
             reqFTP.UsePassive = usePassive;
 
            // Notify the server about the size of the uploaded file
             reqFTP.ContentLength = fileInf.Length;
 
            // The buffer size is set to 2kb
             int buffLength = 2048;
             byte[] buff = new byte[buffLength];
             int contentLen;
 
            // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
             FileStream fs = fileInf.OpenRead();
 
            try
             {
                 // Stream to which the file to be upload is written
                 Stream strm = reqFTP.GetRequestStream();
 
                // Read from the file stream 2kb at a time
                 contentLen = fs.Read(buff, 0, buffLength);
 
                // Till Stream content ends
                 while (contentLen != 0)
                 {
                     // Write Content from the file stream to the FTP Upload Stream
                     strm.Write(buff, 0, contentLen);
                     contentLen = fs.Read(buff, 0, buffLength);
                 }
 
                // Close the file stream and the Request Stream
                 strm.Close();
                 fs.Close();
                 MessageBox.Show("업로드 완료되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                 lvFiles.Items.Clear();

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
                return true;
             }
 
            catch(Exception f)
            {
                MessageBox.Show(f.ToString());
                //MessageBox.Show("FTP 전송중 문제가 발생하였습니다. 네트워크 상황 또는 접속정보를 살펴 보시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return false;
             }
             
        }

        //파일 삭제         
         public void DeleteFTP(string fileName)
         {
             try
             {
                 //fileName = fileName.Replace("File Server/", "");
                 string uri = "ftp://"+ fileName;
                 FtpWebRequest reqFTP;
                 reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                 reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                 reqFTP.KeepAlive = false;
                 reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                 string result = String.Empty;
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 long size = response.ContentLength;
                 Stream datastream = response.GetResponseStream();
                 StreamReader sr = new StreamReader(datastream);
                 result = sr.ReadToEnd();
                 sr.Close();
                 datastream.Close();
                 response.Close();
                 MessageBox.Show("해당 파일이 삭제되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                 lvFiles.Items.Clear();

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
             catch {
                MessageBox.Show("FTP 파일 삭제중 문제가 발생하였습니다. 네트워크 상황 또는 접속정보를 살펴 보시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }
         }
        
        //폴더 생성
         public void MakeDir(string dirName)
         {
             FtpWebRequest reqFTP;
             try
             {
                 // dirName = name of the directory to create.
                 //dirName = dirName.Replace("File Server/", "");
                 reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + dirName));
                 reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                 reqFTP.UseBinary = true;
                 reqFTP.UsePassive = usePassive;
                 reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 Stream ftpStream = response.GetResponseStream();

                 ftpStream.Close();
                 response.Close();
                 MessageBox.Show("폴더 생성이 완료되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                 SystemBase.Validation.GroupBox_Reset(groupBox2);

                 tvFolders.Nodes.Clear();
                 TreeNode root = tvFolders.Nodes.Add("파일서버");
                 string drives = @server_root_drive;
                 TreeNode node = root.Nodes.Add(drives);
                 node.Nodes.Add("@%");
                 root.Tag = "0";
                 node.Tag = "0";
                 root.Expand();
                 node.Expand();
             }
             catch
             {
                 MessageBox.Show("FTP 폴더 생성중 문제가 발생하였습니다. 네트워크 상황 또는 접속정보를 살펴 보시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }
         }

        //폴더 삭제
         public void DeleteDir(string dirName)
         {
             FtpWebRequest reqFTP;
             try
             {
                 // dirName = name of the directory to create.
                 //dirName = dirName.Replace("File Server/", "");
                 reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + dirName));
                 reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                 reqFTP.UseBinary = true;
                 reqFTP.UsePassive = usePassive;
                 reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 Stream ftpStream = response.GetResponseStream();

                 ftpStream.Close();
                 response.Close();
                 MessageBox.Show("폴더 삭제가 완료되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                 SystemBase.Validation.GroupBox_Reset(groupBox2);

                 tvFolders.Nodes.Clear();
                 TreeNode root = tvFolders.Nodes.Add("파일서버");
                 string drives = @server_root_drive;
                 TreeNode node = root.Nodes.Add(drives);
                 node.Nodes.Add("@%");
                 root.Tag = "0";
                 node.Tag = "0";
                 root.Expand();
                 node.Expand();
             }
             catch
             {
                 MessageBox.Show("FTP 폴더 삭제중 문제가 발생하였습니다. 네트워크 상황 또는 접속정보 및 삭제 대상 폴더 내의 데이타를 살펴 보시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }
         }

         private static void InitiateTree(TreeView DirTree)
         {
             string[] DRIVES = Directory.GetLogicalDrives();

             foreach (string drive in DRIVES)
             {
                 TreeNode root = new TreeNode(drive);
                 DirTree.Nodes.Add(root);

                 DirectoryInfo dir = new DirectoryInfo(drive);
                 GetDirectoryNodes(root, dir, false);
             }
         }

         private static void GetDirectoryNodes(TreeNode root, DirectoryInfo dirs, bool isLoop)
         {
             try
             {
                 DirectoryInfo[] DIRS = dirs.GetDirectories();

                 foreach (DirectoryInfo dir in DIRS)
                 {
                     TreeNode child = new TreeNode(dir.Name);
                     root.Nodes.Add(child);

                     if (isLoop)
                         GetDirectoryNodes(child, dir, false);

                 }
             }
             catch (Exception dirsE)
             {
                 dirsE.ToString();
             }
         }

         private void DirTreeView_AfterExpand(object sender, TreeViewEventArgs e)
         {
             DirectoryInfo dir = new DirectoryInfo(e.Node.FullPath);
             e.Node.Nodes.Clear();
             GetDirectoryNodes(e.Node, dir, true);
         }

         private void DirTreeView_AfterSelect(object sender, TreeViewEventArgs e)
         {
             //txtPath.Text = e.Node.FullPath;

             try
             {
                 FileListView.Items.Clear();

                 DirectoryInfo dir = new DirectoryInfo(e.Node.FullPath);
                 FileInfo[] FILES = dir.GetFiles();


                 foreach (FileInfo file in FILES)
                 {
                     ListViewItem item = new ListViewItem(file.Name);
/*
                     if (file.Length > 1024 * 1024 * 1024)
                         item.SubItems.Add
                             (String.Format("{0}GB", file.Length / 1024 / 1024 / 1024));
                     else if (file.Length > 1024 * 1024)
                         item.SubItems.Add
                             (String.Format("{0}MB", file.Length / 1024 / 1024));
                     else if (file.Length > 1024)
                         item.SubItems.Add
                             (String.Format("{0}KB", file.Length / 1024));
                     else
                         item.SubItems.Add
                             (String.Format("{0}BYTE", file.Length));
*/

                     item.SubItems.Add(file.Length.ToString());
                     item.SubItems.Add(String.Format("{0}", file.Extension));
                     item.SubItems.Add(file.LastWriteTime.ToString());
                     item.SubItems.Add(file.Directory.ToString());
                     item.ImageIndex = 2;

                     FileListView.Items.Add(item);
                 }
             }
             catch (Exception fileE)
             {
                 fileE.ToString();
             }
         }

         private void FileListView_ColumnClick(object sender, ColumnClickEventArgs e)
         {
             FileListView.ListViewItemSorter = new ListViewItemComparer(e.Column);
         }
    }

    class ListViewItemComparer : System.Collections.IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }
        public ListViewItemComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            return String.Compare
                (((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
        }
    }

}
