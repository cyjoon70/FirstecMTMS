using System;

namespace EDocument
{
	/// <summary>
	/// 문서컬럼 정보
	/// </summary>
	public class DocumentColumnInformation
	{
		/// <summary>문서명</summary>
		public string Name = string.Empty;
		/// <summary>컬럼 표시 여부</summary>
		public bool Visible = true;

		/// <summary>
		/// 문서컬럼 정보를 생성합니다.
		/// </summary>
		/// <param name="name">문서명</param>
		public DocumentColumnInformation(string name)
		{
			this.Name = name;
			this.Visible = true;
		}

		/// <summary>
		/// 문서컬럼 정보를 생성합니다.
		/// </summary>
		/// <param name="name">문서명</param>
		/// <param name="visible">표시 여부</param>
		public DocumentColumnInformation(string name, bool visible)
		{
			this.Name = name;
			this.Visible = visible;
		}
	}
}