using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDocument
{
	/// <summary>
	/// 전자문서 실행환경입니다.
	/// </summary>
	public class Environment
	{
		/// <summary>
		/// 자료파일 FTP 루트 경로입니다.
		/// </summary>
		public static string SourceFtpDirectory
		{
			get { return "Source/"; }
		}

		/// <summary>
		/// 문서파일 루트 경로입니다.
		/// </summary>
		public static string DocumentFtpDirectory
		{
			get { return "Document/"; }
		}
	}
}
