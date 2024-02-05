using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace EDocument.Network
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

		#region 메소드
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
                request.UsePassive = false;     // 2018.02.27. hma 추가: FTP 연결시 Passive 연결이 되지 않도록 하기 위해.

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
		/// FTP 서버 경로의 파일을 삭제합니다.
		/// </summary>
		/// <param name="ftpFilename">삭제할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultCode">다운로드 결과 코드</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>삭제 성공 여부</returns>
		public static bool DeleteFile(string ftpFilename, string ftpId, string ftpPw, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Credentials = new NetworkCredential(ftpId, ftpPw);
			request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.UsePassive = false;     // 2018.02.28. hma 추가: Passive 모드로 처리되지 않도록 함.
        
            FtpWebResponse response = null;
			try
			{
				response = (FtpWebResponse)request.GetResponse();
			}
			catch (Exception e)
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = e.Message;
				return false;
			}

			resultCode = response.StatusCode;
			resultMessage = response.StatusDescription;
			response.Close();
			return resultCode == FtpStatusCode.FileActionOK;
		}

		/// <summary>
		/// FTP 서버 경로의 파일을 삭제합니다.
		/// </summary>
		/// <param name="ftpFilename">삭제할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <returns>삭제 성공 여부</returns>
		public static bool DeleteFile(string ftpFilename, string ftpId, string ftpPw)
		{
			string resultMessage = null;
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DeleteFile(ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <param name="resultCode">다운로드 결과 코드</param>
		/// <param name="resultMessage">다운로드 결과 메시지</param>
		/// <returns>다운로드한 컨텐츠의 메모리 스트림</returns>
		public static MemoryStream DownloadFileToStream(string ftpFilename, string ftpId, string ftpPw, ref FtpStatusCode resultCode, ref string resultMessage)
		{
			// FTP 연결준비
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Credentials = new NetworkCredential(ftpId, ftpPw);
			request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = false;         // 2018.02.28. hma 추가: Passive 모드로 처리되지 않도록 함.

            FtpWebResponse response = null;
			try
			{
				response = (FtpWebResponse)request.GetResponse();
			}
			catch
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버에 연결할 수 없습니다. 경로가 잘못됐거나 권한문제일 수 있습니다.";
				return null;
			}

			// 파일 요청
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

			// 파일 전송
			MemoryStream mem = new MemoryStream(); ;
			const int bufferSize = 10000000;
			int len = 0;
			byte[] buffer = new byte[bufferSize];
			try
			{
				do
				{
					len = r.Read(buffer, 0, bufferSize);
					if (len > 0) mem.Write(buffer, 0, len);
				} while (len > 0);
				mem.Flush();
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
				if (response.StatusCode == FtpStatusCode.ClosingData)
					return mem;
				else return null;
			}
			else
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버로 부터 응답을 받지 못했습니다.";
				return null;
			}
		}

		/// <summary>
		/// FTP 서버로 부터 파일을 다운로드합니다.
		/// </summary>
		/// <param name="ftpFilename">다운로드할 FTP 파일 주소(예: 'ftp://your.host.name/folder/filename.ext')</param>
		/// <param name="ftpId">FTP 접속 아이디</param>
		/// <param name="ftpPw">FTP 접속 비밀번호</param>
		/// <returns>다운로드 성공 여부</returns>
		public static MemoryStream DownloadFileToStream(string ftpFilename, string ftpId, string ftpPw)
		{
			string resultMessage = null;
			FtpStatusCode resultCode = FtpStatusCode.Undefined;
			return DownloadFileToStream(ftpFilename, ftpId, ftpPw, ref resultCode, ref resultMessage);
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
		public static bool DownloadFile(string localFilename, string ftpFilename, string ftpId, string ftpPw, ref string resultMessage)
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
			// DownloadFileToStream()을 이용해 중복구현을 피할 수 있지만 파일을 메모리에 모두 로드해야 함
			// 버퍼를 이용한 파일 다운로드는 메모리를 조금만 사용하므로 별도로 중복구현함

			// FTP 연결준비
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Credentials = new NetworkCredential(ftpId, ftpPw);
			request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = false;     // 2018.02.28. hma 추가: Passive 모드로 처리되지 않도록 함.

            FtpWebResponse response = null;
			try
			{
				response = (FtpWebResponse)request.GetResponse();
			}
			catch
			{
				resultCode = FtpStatusCode.Undefined;
				resultMessage = "서버에 연결할 수 없습니다. 경로가 잘못됐거나 권한문제일 수 있습니다.";
				return false;
			}

			// 파일 요청
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

			// 파일 전송
			FileStream w = null;
			const int bufferSize = 10000000;
			int len = 0;
			byte[] buffer = new byte[bufferSize];
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
			request.Credentials = new NetworkCredential(ftpId, ftpPw);
			request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UsePassive = false;     // 2018.02.28. hma 추가: Passive 모드로 처리되지 않도록 함.

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
		#endregion
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
