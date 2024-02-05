using System;
using System.Drawing;

namespace UIForm.Resource
{
	/// <summary>
	/// 그리드 버튼용 이미지 라이브러리입니다. 크기는 16x16 입니다.
	/// </summary>
	public class GridButtonImages
	{
		/// <summary>
		/// 다운로드
		/// </summary>
		public static Bitmap Download
		{
			get { return Properties.Resources.x16_arrow_down; }
		}

		/// <summary>
		/// 다운로드(disabled)
		/// </summary>
		public static Bitmap DownloadDisabled
		{
			get { return Properties.Resources.x16_arrow_down_gray; }
		}

		/// <summary>
		/// 파일 열기
		/// </summary>
		public static Bitmap FileOpen
		{
			get { return Properties.Resources.x16_folder_open; }
		}

		/// <summary>
		/// 파일 열기(disabled)
		/// </summary>
		public static Bitmap FileOpenDisabled
		{
			get { return Properties.Resources.x16_folder_open_gray; }
		}

		/// <summary>
		/// 읽기
		/// </summary>
		public static Bitmap Read
		{
			get { return Properties.Resources.x16_glass; }
		}

		/// <summary>
		/// 일기(disabled)
		/// </summary>
		public static Bitmap ReadDisabled
		{
			get { return Properties.Resources.x16_glass_gray; }
		}
	}
}
