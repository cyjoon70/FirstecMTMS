using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using EDocument.Network;

namespace FilesCommon
{
    public class FtpUtil
    {
        #region Field
        /// <summary>
        /// 서버 업로드 결과 상태값
        /// </summary>
        public enum UploadResultState
        {
            Ok = 0,
            FTPError,
            DBError,
        }

        public enum DownloadResultState
        {
            Ok = 0,
            FTPError,
            DBError,
        }
                                
        string homeUrl = null;
        const string ftpAccountName = "E2MAX";
        const string ftpAccountPassword = "zemax";

        #endregion

        public FtpUtil()
        {
            string rootDir = GetServerProperty("FTPROOT");
            if (string.IsNullOrEmpty(rootDir)) rootDir = "?/";

            // *** 배포시 아래 수정 *** ======================================================
            //homeUrl = Url.Combine("ftp://" + SystemBase.Base.gstrServerNM, rootDir);
            homeUrl = Url.Combine("ftp://192.168.1.10/", rootDir);
            // ===============================================================================
        }

        #region 메소드
        /// <summary>
        /// 서버값을 가져옵니다.
        /// </summary>
        /// <param name="key">키값</param>
        /// <returns></returns>
        public string GetServerProperty(string key)
        {
            string query = "select CD_NM from B_COMM_CODE where COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' and MAJOR_CD = 'TD100' and MINOR_CD = '" + key.ToUpper() + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
            if (dt != null && dt.Rows.Count > 0)
            {
                object value = dt.Rows[0][0];
                if (value != DBNull.Value && value != null) return value.ToString();
            }
            return null;
        }

        /// <summary>
        /// 서버에 첨부문서파일을 업로드하고 첨부파일 정보를 업데이트합니다.
        /// </summary>
        /// <param name="docSeq">문서 Seq</param>
        /// <param name="docDate">문서레코드 생성일</param>
        /// <param name="filepath">업로드할 파일명(경로포함)</param>
        /// <returns>업로드 결과 상태값</returns>
        public UploadResultState UploadDocumentFile(int docSeq, DateTime docDate, string filepath)
        {
            // 파일처리 준비
            string filename = Path.GetFileName(filepath); // 파일명
            string fileext = Path.GetExtension(filename); // 확장자
            if (!string.IsNullOrEmpty(fileext)) fileext = fileext.Substring(1).ToUpper();
            string serverPath = string.Format(@"SCM/{0:0000}{1:00}", docDate.Year, docDate.Month); // 서버 FTP 경로
            string serverFilename = string.Format(@"{0}_F.{1}", docSeq, fileext); // 서버 파일명

            // 서버로 파일 복사
            string ftpPath = homeUrl + serverPath + "/";
            Ftp.CheckDirectory(ftpPath, ftpAccountName, ftpAccountPassword, true);
            
            ftpPath += serverFilename;

            string msg = "";

            if (!Ftp.UploadFile(filepath, ftpPath, ftpAccountName, ftpAccountPassword, ref msg))
            {
                MessageBox.Show(msg, "파일 업로드", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return UploadResultState.FTPError;
            }

            return UploadResultState.Ok;
        }

        public DownloadResultState DownloadDocumentFile(int docSeq, DateTime docDate, string filepath)
        {
            /*
            Ftp.DownloadFile(filename, ftppath, Server.AccountName, Server.AccountPassword, ref msg);
            */
            
            return DownloadResultState.Ok;
        }

        public void ViewDocumentFile(string filepath)
        {
            string filename = string.Empty;
			string ext = Path.GetExtension(Path.GetFileName(filepath));

			DeleteTempFiles();

			do { filename = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), GetTempFilenamePrefix() + Path.GetRandomFileName()), ext); } while (File.Exists(filename));
            bool ok = DownloadFile(filepath, filename, false);
            if (ok)
            {
                System.Diagnostics.Process ps = new System.Diagnostics.Process();
                ps.StartInfo.FileName = filename;
                ps.StartInfo.WorkingDirectory = Path.GetTempPath();
                ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                ps.Start();
            }
            else
                MessageBox.Show("서버로 부터 파일을 불러오는데 실패했습니다.", "문서열람", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
        }

		/// <summary>
		/// 열람을 위해 임시로 다운로드한 파일을 모두 삭제합니다.
		/// </summary>
		private void DeleteTempFiles()
		{
			foreach (FileInfo f in new DirectoryInfo(Path.GetTempPath()).GetFiles(GetTempFilenamePrefix() + "*.*")) // 프리픽스파일 모두 삭제
			{
				try { f.Delete(); }
				catch { }
			}
		}

		/// <summary>
		/// 임시파일명의 프리픽스로 사용할 고정된 문자열을 반환합니다.
		/// </summary>
		/// <returns></returns>
		private string GetTempFilenamePrefix()
		{
			return string.Format("{0:X}", this.GetHashCode()) + "_";
		}

		public bool DownloadFile(string filepath, string filename, bool showResultMessage)
        {

            string msg = null;
            
            string ftppath = Url.Combine(homeUrl, filepath);
            bool ok = Ftp.DownloadFile(filename, ftppath, ftpAccountName, ftpAccountPassword, ref msg);

            if (showResultMessage)
            {
                if (ok)
                    MessageBox.Show("다운로드가 완료되었습니다.", "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("다운로드에 실패했습니다: " + msg, "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ok;
        }

		public bool DeleteFile(string filepath, bool showResultMessage)
		{

			string msg = null;
			string ftppath = Url.Combine(homeUrl, filepath);
			bool ok = Ftp.DeleteFile(ftppath, ftpAccountName, ftpAccountPassword);

			if (showResultMessage)
			{
				if (ok)
					MessageBox.Show("파일삭제가 완료되었습니다.", "파일 삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show("파일삭제에 실패했습니다: " + msg, "파일 삭제", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return ok;
		}

		#endregion
	}
}
