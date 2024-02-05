using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDocument.Network
{
	/// <summary>
	/// 전자문서 실행환경입니다.
	/// </summary>
	public class FtpConnection
	{
		const string documentDirectoryName = "Document";
		const string sourceDirectoryName = "Source";
		const string ftpAccountName = "E2MAX";
		const string ftpAccountPassword = "zemax";
		
		#region 속성
		/// <summary>
		/// FTP 홈주소입니다.
		/// </summary>
		public static string HomeUrl
		{
			get { return "ftp://" + SystemBase.Base.gstrServerNM + "/Archive/"; }
		}

		/// <summary>
		/// FTP 문서파일 홈주소입니다.
		/// </summary>
		public static string DocumentUrl
		{
			get { return HomeUrl + documentDirectoryName + "/"; }
		}

		/// <summary>
		/// FTP 자료파일 홈주소입니다.
		/// </summary>
		public static string SourceUrl
		{
			get { return HomeUrl + sourceDirectoryName + "/"; }
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
	}
}
