using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EDocument.Network
{
	/// <summary>
	/// 전자문서 실행환경입니다.
	/// </summary>
	public static class Server
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

		static string homeUrl = null;
		const string documentDirectoryName = "Document";
		const string sourceDirectoryName = "Source";
		const string ftpAccountName = "E2MAX";
		const string ftpAccountPassword = "zemax";
		#endregion

		static Server()
		{
			string rootDir = GetServerProperty("FTPROOT");
			if (string.IsNullOrEmpty(rootDir)) rootDir = "?/";
			homeUrl = Url.Combine("ftp://" + SystemBase.Base.gstrServerNM, rootDir);
		}

		#region 속성
		/// <summary>
		/// FTP 홈주소입니다.
		/// </summary>
		public static string HomeUrl
		{
			get { return homeUrl; }
		}

		/// <summary>
		/// FTP 문서파일 홈주소입니다.
		/// </summary>
		public static string DocumentUrl
		{
			get { return Url.Combine(HomeUrl, documentDirectoryName + "/"); }
		}

		/// <summary>
		/// FTP 자료파일 홈주소입니다.
		/// </summary>
		public static string SourceUrl
		{
			get { return Url.Combine(HomeUrl, sourceDirectoryName + "/"); }
		}

		/// <summary>
		/// FTP 접속계정 이름입니다.
		/// </summary>
		public static string AccountName
		{
			get { return ftpAccountName; }
		}

		/// <summary>
		/// FTP 접속계정 비밀번호입니다.
		/// </summary>
		public static string AccountPassword
		{
			get { return ftpAccountPassword; }
		}

		/// <summary>
		/// 문서파일 루트의 폴더명입니다.
		/// </summary>
		public static string DocumentDirectoryName
		{
			get { return documentDirectoryName; }
		}

		/// <summary>
		/// 자료파일 루트의 폴더명입니다.
		/// </summary>
		public static string SourceDirectoryName
		{
			get { return sourceDirectoryName; }
		}
		#endregion

		#region 메소드
		/// <summary>
		/// 서버값을 가져옵니다.
		/// </summary>
		/// <param name="key">키값</param>
		/// <returns></returns>
		public static string GetServerProperty(string key)
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
		/// <param name="docCategoryCode">문서카테고리 코드</param>
		/// <param name="docCode">문서 코드</param>
		/// <param name="docId">문서ID</param>
		/// <param name="docDate">문서레코드 생성일</param>
		/// <param name="filepath">업로드할 파일명(경로포함)</param>
		/// <param name="dbConn">DB 연결</param>
		/// <param name="dbTran">DB 트랜젝션</param>
		/// <returns>업로드 결과 상태값</returns>
		public static UploadResultState UploadDocumentFile(string docCategoryCode, string docCode, int docId, DateTime docDate, string filepath, SqlConnection dbConn, SqlTransaction dbTran)
		{
			const string messageBoxTitle = "파일 업로드";

			// 파일처리 준비
			string filename = Path.GetFileName(filepath); // 파일명
			string fileext = Path.GetExtension(filename); // 확장자
			if (!string.IsNullOrEmpty(fileext)) fileext = fileext.Substring(1).ToUpper();
			string serverPath = string.Format(@"{0}/{1:0000}/{2:00}", docCode, docDate.Year, docDate.Month); // 서버 FTP 경로
			string serverFilename = string.Format(@"DF{0}_{1}_{2}_{3:0000}-{4:00}-{5:00}.{6}", docId, docCategoryCode, docCode, docDate.Year, docDate.Month, docDate.Day, fileext); // 서버 파일명

			// 서버로 파일 복사
			string ftpPath = Server.DocumentUrl + serverPath + "/";
			Ftp.CheckDirectory(ftpPath, Server.AccountName, Server.AccountPassword, true);
			ftpPath += serverFilename;
			string msg = "";
			if (!Ftp.UploadFile(filepath, ftpPath, Server.AccountName, Server.AccountPassword, ref msg))
			{
				MessageBox.Show(msg, "파일 업로드", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return UploadResultState.FTPError;
			}

			if (dbConn != null && dbTran != null)
			{
				// 문서레코드에 파일정보 저장
				string strFileSql = "usp_T_DOC 'U_FILE'"
					+ ", @pDOC_ID = " + docId
					+ ", @pSVR_PATH = '" + serverPath + "' "
					+ ", @pSVR_FNM = '" + serverFilename + "' "
					+ ", @pORG_FNM = '" + filename + "' "
					+ ", @pFILE_EXT = '" + fileext + "' ";
				DataSet dsf = SystemBase.DbOpen.TranDataSet(strFileSql, dbConn, dbTran);
				string resultCode = dsf.Tables[0].Rows[0][0].ToString();
				string resultMessage = dsf.Tables[0].Rows[0][1].ToString();

				if (resultCode != "OK")
				{
					MessageBox.Show(resultMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return UploadResultState.DBError;
				}
			}

			return UploadResultState.Ok;
		}

		/// <summary>
		/// 서버에 자료파일을 업로드하고 첨부파일 정보를 업데이트합니다.
		/// </summary>
		/// <param name="docCode">문서 코드</param>
		/// <param name="srcfId">자료파일ID</param>
		/// <param name="docDate">문서레코드 생성일</param>
		/// <param name="filepath">업로드할 파일명(경로포함)</param>
		/// <param name="dbConn">DB 연결</param>
		/// <param name="dbTran">DB 트랜젝션</param>
		/// <returns>업로드 결과 상태값</returns>
		public static UploadResultState UploadSourceFile(string docCode, int srcfId, DateTime docDate, string filepath, SqlConnection dbConn, SqlTransaction dbTran)
		{
			const string messageBoxTitle = "파일 업로드";

			// 파일처리 준비
			string filename = Path.GetFileName(filepath); // 파일명
			string fileext = Path.GetExtension(filename); // 확장자
			if (!string.IsNullOrEmpty(fileext)) fileext = fileext.Substring(1).ToUpper();
			string serverPath = string.Format(@"{0}/{1:0000}/{2:00}", docCode, docDate.Year, docDate.Month); // 서버 FTP 경로
			string serverFilename = string.Format(@"SF{0}_{1}_{2:0000}-{3:00}-{4:00}.{5}", srcfId, docCode, docDate.Year, docDate.Month, docDate.Day, fileext); // 서버 파일명

			// 서버로 파일 복사
			string ftpPath = Server.SourceUrl + serverPath + "/";
			Ftp.CheckDirectory(ftpPath, Server.AccountName, Server.AccountPassword, true);
			ftpPath += serverFilename;
			string msg = "";
			if (!Ftp.UploadFile(filepath, ftpPath, Server.AccountName, Server.AccountPassword, ref msg))
			{
				MessageBox.Show(msg, "파일 업로드", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return UploadResultState.FTPError;
			}

			if (dbConn != null && dbTran != null)
			{
				// 문서레코드에 파일정보 저장
				string strFileSql = "usp_TDT001 'U3'"
					+ ", @pSRCF_ID = " + srcfId
					+ ", @pSVR_PATH = '" + serverPath + "' "
					+ ", @pSVR_FNM = '" + serverFilename + "' "
					+ ", @pORG_FNM = '" + filename + "' "
					+ ", @pFILE_EXT = '" + fileext + "' ";
				DataSet dsf = SystemBase.DbOpen.TranDataSet(strFileSql, dbConn, dbTran);
				string resultCode = dsf.Tables[0].Rows[0][0].ToString();
				string resultMessage = dsf.Tables[0].Rows[0][1].ToString();

				if (resultCode != "OK")
				{
					MessageBox.Show(resultMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return UploadResultState.DBError;
				}
			}

			return UploadResultState.Ok;
		}

		#endregion
	}
}
