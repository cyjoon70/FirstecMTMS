using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SystemBase.Network
{
	/// <summary>
	/// FTP 관련 라이브러리입니다.
	/// </summary>
	public class Ftp
	{
		/// <summary>
		/// 접속 계정 정보입니다.
		/// </summary>
		public struct Account
		{
			public string Username;
			public string Password;

			public Account(string username, string password)
			{
				Username = username;
				Password = password;
			}
		}

		/// <summary>
		/// FTP 경로가 존재하는지 확인합니다.
		/// </summary>
		/// <param name="url">FTP 주소</param>
		/// <param name="account">계정정보</param>
		/// <param name="e">실패시 오류 정보</param>
		/// <returns>1 = 경로가 존재함, 0 = 경로가 존재하지 않음, -1 = 접속할 수 없음</returns>
		public static bool UrlExists(string url, Account account, ref WebException e)
		{
			return UrlExists(url, account.Username, account.Password, ref e);
		}

		/// <summary>
		/// FTP 경로가 존재하는지 확인합니다.
		/// </summary>
		/// <param name="url">위치</param>
		/// <param name="username">계정명</param>
		/// <param name="password">비밀번호</param>
		/// <param name="e">실패시 오류 정보</param>
		/// <returns>1 = 경로가 존재함, 0 = 경로가 존재하지 않음, -1 = 접속할 수 없음</returns>
		public static bool UrlExists(string url, string username, string password, ref WebException e)
		{
			e = null;
			try
			{
				var request = (FtpWebRequest)WebRequest.Create(url);
				request.Credentials = new NetworkCredential(username, password);
				request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;

				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
				return true;
			}
			catch (WebException ex)
			{
				if (ex.Status != WebExceptionStatus.ProtocolError) e = ex;
			}
			return false;
		}

		/// <summary>
		/// FTP 경로가 존재하는지 확인하고 필요하다면 생성합니다.
		/// </summary>
		/// <param name="url">FTP 주소</param>
		/// <param name="account">접속 정보</param>
		/// <param name="createDirectory">디렉토리가 없을 경우 생성할지 여부</param>
		/// <returns>경로가 존재하는지(또는 생성되었는지) 여부</returns>
		public static bool CheckDirectory(string url, Account account, bool createDirectory)
		{
			return CheckDirectory(url, account.Username, account.Password, createDirectory);
		}

		/// <summary>
		/// FTP 경로가 존재하는지 확인하고 필요하다면 생성합니다.
		/// </summary>
		/// <param name="url">FTP 경로</param>
		/// <param name="username">계정명</param>
		/// <param name="password">비밀번호</param>
		/// <param name="createDirectory">디렉토리가 없을 경우 생성할지 여부</param>
		/// <returns>경로가 존재하는지(또는 생성되었는지) 여부</returns>
		public static bool CheckDirectory(string url, string username, string password, bool createDirectory)
		{
			WebException e = null;
			if (UrlExists(url, username, password, ref e))
				return true;
			else
			{
				Uri uri = new Uri(url);
				if (e != null)
					throw e;
				else if (createDirectory && uri.Segments.Length > 1)
				{
					string path = uri.Scheme + "://" + uri.Host + "/";
					for (int n = 1; n < uri.Segments.Length; n++)
					{
						path += uri.Segments[n];
						try
						{
							FtpWebRequest FTPReq = (FtpWebRequest)FtpWebRequest.Create(path);
							FTPReq.Credentials = new NetworkCredential(username, password);
							FTPReq.Method = WebRequestMethods.Ftp.MakeDirectory;
							FtpWebResponse FTPRes = (FtpWebResponse)FTPReq.GetResponse();
						}
						catch { }
					}

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">FTP 주소(예: 'ftp://my.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, Account account)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			string resultMessage = null;
			return UploadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">FTP 주소(예: 'ftp://my.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <param name="resultMessage">업로드 결과 메시지</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, Account account, ref string resultMessage)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return UploadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">FTP 주소(예: 'ftp://my.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <param name="resultCode">업로드 결과 코드</param>
		/// <param name="resultMessage">업로드 결과 메시지</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, Account account, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			return UploadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">FTP 주소(예: 'ftp://my.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			string resultMessage = null;
			return UploadFile(localFilename, ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">FTP 주소(예: 'ftp://my.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultMessage">업로드 결과 메시지</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw, ref string resultMessage)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return UploadFile(localFilename, ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// 로컬파일을 FTP 서버에 업로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 로컬 파일명</param>
		/// <param name="ftpFilename">업로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultCode">업로드 결과 코드</param>
		/// <param name="resultMessage">업로드 결과 메시지</param>
		/// <returns>업로드 성공 여부</returns>
		public static bool UploadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			// FTP 연결준비
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential(ftpId, ftpPw);

			// 파일 전송
			const int bufferSize = 10000000;
			int len = 0;
			byte[] buffer = new byte[bufferSize];
			Stream r = request.GetRequestStream();
			FileStream w = new FileStream(localFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
			do
			{
				len = w.Read(buffer, 0, bufferSize);
				if (len > 0) r.Write(buffer, 0, len);
			} while (len > 0);
			w.Close();
			r.Close();

			// 응답상태
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			resultCode = response.StatusCode;
			resultMessage = response.StatusDescription;
			response.Close();
			return response.StatusCode == FtpStatusCode.ClosingData;
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, Account account)
		{
			string resultMessage = null;
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DownloadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, Account account, ref string resultMessage)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DownloadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="account">계정정보</param>
		/// <param name="resultCode">다운로드 결과 코드</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, Account account, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			return DownloadFile(localFilename, ftpFilename, account.Username, account.Password, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw)
		{
			string resultMessage = null;
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DownloadFile(localFilename, ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, string ftpUrl, string ftpId, string ftpPw, ref string resultMessage)
		{
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DownloadFile(localFilename, ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="localFilename">경로를 포함한 다운로드 파일명</param>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultCode">다운로드 결과 코드</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>다운로드 성공 여부</returns>
		public static bool DownloadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			// FTP 연결준비
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Method = WebRequestMethods.Ftp.DownloadFile;
			request.Credentials = new NetworkCredential(ftpId, ftpPw);
			FtpWebResponse response = null;
			try
			{
				response = (FtpWebResponse)request.GetResponse();
			}
			catch
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버에 연결할 수 없습니다. 경로가 잘못된 것 같습니다.";
				return false;
			}

			// 파일 전송
			const int bufferSize = 10000000;
			int len = 0;
			byte[] buffer = new byte[bufferSize];
			Stream r = null;
			try
			{
				r = response.GetResponseStream();
			}
			catch
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버의 파일을 열 수 없습니다.";
			}
			FileStream w = null;
			try
			{
				w = new FileStream(localFilename, FileMode.Create, FileAccess.Write, FileShare.None);
			}
			catch (IOException e)
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = e.Message;
				return false;
			}

			try
			{
				do
				{
					len = r.Read(buffer, 0, bufferSize);
					if (len > 0) w.Write(buffer, 0, len);
				} while (len > 0);
				w.Close();
				r.Close();
			}
			catch (Exception e)
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = e.Message;
			}

			// 응답상태
			if (response != null)
			{
				resultCode = response.StatusCode;
				resultMessage = response.StatusDescription;
				response.Close();
				return response.StatusCode == FtpStatusCode.ClosingData;
			}
			else
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버로 부터 응답을 받지 못했습니다.";
				return false;
			}
		}
	}

	/// <summary>
	/// URL 관련 라이브러리입니다.
	/// </summary>
	public class Url
	{
		/// <summary>
		/// URL 경로를 이어 붙입니다.
		/// </summary>
		/// <param name="path1">왼쪽에 붙일 URL 경로</param>
		/// <param name="path2">오른쪽에 붙일 URL 경로</param>
		/// <returns></returns>
		public static string Combine(string path1, string path2)
		{
			if (string.IsNullOrEmpty(path1)) return path2;
			else if (string.IsNullOrEmpty(path2)) return path1;

			if (path1.EndsWith("/"))
			{
				if (path2.StartsWith("/")) return path1 + path2.Substring(1);
				else return path1 + path2;
			}
			else
			{
				if (path2.StartsWith("/")) return path1 + path2;
				else return path1 + "/" + path2;
			}
		}

	}
}
