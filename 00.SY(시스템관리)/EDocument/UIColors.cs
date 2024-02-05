using System;
using System.Text;
using System.Drawing;

namespace EDocument
{
	/// <summary>
	/// 색 정의 클래스입니다.
	/// </summary>
	public class UIColors
	{
		/// <summary>폐기됨 전경색</summary>
		public static Color Discard = Color.Red;

		/// <summary>기본 전경색</summary>
		public static Color Normal = Color.Black;

		/// <summary>기본 배경색</summary>
		public static Color NormalBackground = Color.White;

		/// <summary>읽기전용 배경색</summary>
		public static Color ReadonlyBackground = Color.FromArgb(239, 239, 239);

		/// <summary>필수입력 배경색</summary>
		public static Color RequiredBackground = Color.LightSkyBlue;
	}
}
