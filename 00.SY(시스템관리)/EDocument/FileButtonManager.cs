using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using SystemBase.Network;

namespace EDocument.Spread
{
	/// <summary>
	/// 스프레드의 버튼을 관리합니다.
	/// </summary>
	public class FileButtonManager
	{
		#region 변수선언
		SheetView sheet;

		// 컬럼 인덱스들
		int colFilename = -1;
		int colServerPath = -1;
		int colServerFilename = -1;
		int colFileSelect = -1;
		int colFileView = -1;
		int colFileDownload = -1;

		// 버튼
		ButtonCellType bcFileSelect, bcDisabledFileSelect;
		ButtonCellType bcFileView, bcDisabledFileView;
		ButtonCellType bcFileDownload, bcDisabledFileDownload;

		// 파일
		long maxUploadFileLength = 200000000;
		string openFileFilter = "모든 문서파일|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.xls;*.xlsx;*.hwp|모든 파일|*.*";

		// FTP 접속
		string ftpHomeUrl = string.Empty;
		Ftp.Account ftpAccount;
		#endregion

		#region 속성
		/// <summary>
		/// 원본 파일명 컬럼의 인덱스
		/// </summary>
		public int FilenameColumnIndex
		{
			get { return colFilename; }
			set { colFilename = value; }
		}

		/// <summary>
		/// 서버 경로 컬럼의 인덱스
		/// </summary>
		public int ServerPathColumnIndex
		{
			get { return colServerPath; }
			set { colServerPath = value; }
		}

		/// <summary>
		/// 서버 파일명 컬럼의 인덱스
		/// </summary>
		public int ServerFilenameColumnIndex
		{
			get { return colServerFilename; }
			set { colServerFilename = value; }
		}

		/// <summary>
		/// 파일선택 버튼 컬럼의 인덱스
		/// </summary>
		public int FileSelectButtonColumnIndex
		{
			get { return colFileSelect; }
			set { colFileSelect = value; }
		}

		/// <summary>
		/// 파일보기 버튼 컬럼의 인덱스
		/// </summary>
		public int FileViewButtonColumnIndex
		{
			get { return colFileView; }
			set { colFileView = value; }
		}

		/// <summary>
		/// 파일다운로드 버튼 컬럼의 인덱스
		/// </summary>
		public int FileDownloadButtonColumnIndex
		{
			get { return colFileDownload; }
			set { colFileDownload = value; }
		}

		/// <summary>
		/// 파일선택 버튼 이미지
		/// </summary>
		public Image FileSelectImage
		{
			get { return bcFileSelect != null ? bcFileSelect.Picture : null; }
			set
			{
				bcFileSelect = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 파일보기 버튼 이미지
		/// </summary>
		public Image FileViewImage
		{
			get { return bcFileView != null ? bcFileView.Picture : null; }
			set
			{
				bcFileView = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 파일다운로드 버튼 이미지
		/// </summary>
		public Image FileDownloadImage
		{
			get { return bcFileDownload != null ? bcFileDownload.Picture : null; }
			set
			{
				bcFileDownload = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 파일선택 버튼 이미지(Disabled)
		/// </summary>
		public Image DisabledFileSelectImage
		{
			get { return bcDisabledFileSelect != null ? bcDisabledFileSelect.Picture : null; }
			set
			{
				bcDisabledFileSelect = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 파일보기 버튼 이미지(Disabled)
		/// </summary>
		public Image DisabledFileViewImage
		{
			get { return bcDisabledFileView != null ? bcDisabledFileView.Picture : null; }
			set
			{
				bcDisabledFileView = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 파일다운로드 버튼 이미지(Disabled)
		/// </summary>
		public Image DisabledFileDownloadImage
		{
			get { return bcDisabledFileDownload != null ? bcDisabledFileDownload.Picture : null; }
			set
			{
				bcDisabledFileDownload = new ButtonCellType()
				{
					Text = "",
					Picture = value,
				};
			}
		}

		/// <summary>
		/// 업로드할 수 있는 최대 파일 크기
		/// </summary>
		public long MaxUploadFileLength
		{
			get { return maxUploadFileLength; }
			set { maxUploadFileLength = value; }
		}

		/// <summary>
		/// 파일열기 대화창에 사용할 필터
		/// </summary>
		public string OpenFileFilter
		{
			get { return openFileFilter; }
			set { openFileFilter = value; }
		}

		/// <summary>
		/// 파일을 다운로드할 FTP 홈 주소
		/// </summary>
		public string FtpHomeUrl
		{
			get { return ftpHomeUrl; }
			set { ftpHomeUrl = value; }
		}

		/// <summary>
		/// FTP 접속 계정
		/// </summary>
		public Ftp.Account FtpAccount
		{
			get { return ftpAccount; }
			set { ftpAccount = value; }
		}

		/// <summary>
		/// FTP 접속 계정명
		/// </summary>
		public string FtpUsername
		{
			get { return ftpAccount.Username; }
			set { ftpAccount.Username = value; }
		}

		/// <summary>
		/// FTP 접속 비밀번호
		/// </summary>
		public string FtpPassword
		{
			get { return ftpAccount.Password; }
			set { ftpAccount.Password = value; }
		}
		#endregion

		/// <summary>
		/// 디펄트 버튼 이미지로 클래스를 초기화합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		public FileButtonManager(SheetView sheet)
		{
			this.sheet = sheet;
			sheet.FpSpread.ButtonClicked += fpSpread_ButtonClicked;
			sheet.FpSpread.CellDoubleClick += fpSpread_CellDoubleClick;

			FileSelectImage = UIForm.Resource.GridButtonImages.FileOpen;
			FileViewImage = UIForm.Resource.GridButtonImages.Read;
			FileDownloadImage = UIForm.Resource.GridButtonImages.Download;
			DisabledFileSelectImage = UIForm.Resource.GridButtonImages.FileOpenDisabled;
			DisabledFileViewImage = UIForm.Resource.GridButtonImages.ReadDisabled;
			DisabledFileDownloadImage = UIForm.Resource.GridButtonImages.DownloadDisabled;
		}

		/// <summary>
		/// 자료파일을 선택합니다. 파일 열기 대화창이 표시되며 자료목록에 파일명이 입력됩니다.
		/// </summary>
		/// <param name="row">자료목록 줄번호</param>
		/// <returns>파일이 지정되었는지 여부</returns>
		bool SelectSourceFile(int row)
		{
			if (!string.IsNullOrEmpty(sheet.Cells[row, colServerPath].Text)) // 이미 업로드된 파일이 있다면 교체 불가
				return false;

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = openFileFilter;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				if ((new FileInfo(dlg.FileName)).Length > maxUploadFileLength)
				{
					MessageBox.Show(string.Format("업로드 파일 크기는 {0}Bytes를 초과할 수 없습니다.", maxUploadFileLength), "업로드 파일 추가", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				// 파일명 입력
				sheet.Cells[row, colServerFilename].Value = dlg.FileName; // 임시로 서버파일명에 전체파일명 기록(업로드시 처리)
				sheet.Cells[row, colFilename].Value = Path.GetFileName(dlg.FileName); // 원본 파일명
				return true;
			}

			return false;
		}

		/// <summary>
		/// 전체 열에 대해 버튼을 업데이트합니다.
		/// </summary>
		public void UpdateButtons()
		{
			for (int row = 0; row < sheet.RowCount; row++) UpdateButtons(row);
		}

		/// <summary>
		/// 버튼을 업데이트합니다.
		/// </summary>
		/// <param name="row">대상 줄 인덱스</param>
		public void UpdateButtons(int row)
		{
			Cell cell;
			bool serverFileExists = sheet.Cells[row, colServerPath].Text != string.Empty; // 업로드한 파일이 존재하는지 여부
			string filename = sheet.Cells[row, colFilename].Text;
			string fileext = !string.IsNullOrEmpty(filename) ? Path.GetExtension(filename).ToLower() : null;

			// 파일 선택 버튼
			if (colFileSelect > -1 && bcFileSelect != null && bcDisabledFileSelect != null)
			{
				cell = sheet.Cells[row, colFileSelect];
				cell.Locked = serverFileExists; // 업로드한 파일은 교체 불가
				cell.CellType = !cell.Locked ? bcFileSelect : bcDisabledFileSelect;
			}

			// 파일 미리보기 버튼
			if (colFileView > -1 && bcFileView != null && bcDisabledFileView != null)
			{
				cell = sheet.Cells[row, colFileView];
				cell.Locked = !serverFileExists; //TODO: 권한이 있는 경우만 허가할 것
				cell.CellType = !cell.Locked && fileext == ".pdf" ? bcFileView : bcDisabledFileView;
			}

			// 파일 다운로드 버튼
			if (colFileDownload > -1 && bcFileDownload != null && bcDisabledFileDownload != null)
			{
				cell = sheet.Cells[row, colFileDownload];
				cell.Locked = !serverFileExists; //TODO: 권한이 있는 경우만 허가할 것
				cell.CellType = !cell.Locked ? bcFileDownload : bcDisabledFileDownload;
			}

			sheet.Cells[row, colFilename].Locked = true; // 파일명을 직접 입력할 수 없도록 잠금
		}

		#region 이벤트 핸들러
		void fpSpread_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			// 파일선택 버튼 클릭
			if (e.Column == colFileSelect)
				SelectSourceFile(e.Row);

			// 파일다운로드 버튼 클릭
			else if (e.Column == colFileDownload)
			{
				if (string.IsNullOrEmpty(ftpHomeUrl))
				{
					MessageBox.Show("FTP 홈 주소가 설정되어 있지 않습니다.");
					return;
				}

				string filename = sheet.Cells[e.Row, colFilename].Text;
				string ftppath = Url.Combine(Url.Combine(ftpHomeUrl, Environment.SourceFtpDirectory), sheet.Cells[e.Row, colServerPath].Text + "/" + sheet.Cells[e.Row, colServerFilename].Text);
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.DefaultExt = Path.GetExtension(filename);
				dlg.Filter = dlg.DefaultExt.ToUpper() + " 파일" + "|*." + dlg.DefaultExt;
				dlg.FileName = filename;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					sheet.FpSpread.TopLevelControl.Cursor = Cursors.WaitCursor;
					string msg = null;
					bool ok = Ftp.DownloadFile(dlg.FileName, ftppath, ftpAccount, ref msg);
					sheet.FpSpread.TopLevelControl.Cursor = Cursors.Default;
					if (ok)
						MessageBox.Show("다운로드가 완료되었습니다.", "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Information);
					else
						MessageBox.Show("다운로드에 실패했습니다: " + msg, "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void fpSpread_CellDoubleClick(object sender, CellClickEventArgs e)
		{
			// 파일명 셀 더블클릭시 파일선택
			if (e.Column == colFilename)
				SelectSourceFile(e.Row);
		}
		#endregion

	}
}
