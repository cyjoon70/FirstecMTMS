using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument.Network;

namespace EDocument.Spread
{
	/// <summary>
	/// 스프레드의 버튼을 관리합니다.
	/// </summary>
	public class FileButtonManager
	{
		#region 필드
		const string AttChar = "?";

		/// <summary>업로드 파일 형식</summary>
		public enum ServerFileType
		{
			DocumentFile,
			SourceFile,
		}

		/// <summary>관리대상시트</summary>
		SheetView sheet;

		// 서버정보
		/// <summary>서버파일 소속영역</summary>
		ServerFileType serverFileDomain;

		// 컬럼 인덱스들
		/// <summary>파일명 컬럼</summary>
		int colFilename = -1;
		/// <summary>서버경로 컬럼</summary>
		int colServerPath = -1;
		/// <summary>서버파일명 컬럼</summary>
		int colServerFilename = -1;
		/// <summary>파일선택 버튼 컬럼</summary>
		int colFileSelect = -1;
		/// <summary>파일보기 버튼 컬럼</summary>
		int colFileView = -1;
		/// <summary>파일다운로드 버튼 컬럼</summary>
		int colFileDownload = -1;
		/// <summary>문서형식 컬럼</summary>
		int colDocTypeName = -1;
		/// <summary>개정번호 컬럼</summary>
		int colDocRevision = -1;
		/// <summary>문서번호 컬럼</summary>
		int colDocNumber = -1;

		// 버튼
		ButtonCellType bcFileSelect, bcDisabledFileSelect;
		ButtonCellType bcFileView, bcDisabledFileView;
		ButtonCellType bcFileDownload, bcDisabledFileDownload;
		bool fileSelectButtonEnabled = true;

		// 파일
		long maxUploadFileLength = 200000000;
		string openFileFilter = "모든 문서파일|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.xls;*.xlsx;*.hwp|모든 파일|*.*";
		bool useInternalViewer = false;
		#endregion

		#region 속성
		/// <summary>
		/// 서버파일 루트경로
		/// </summary>
		public string ServerFileRoot
		{
			get
			{
				switch (serverFileDomain)
				{
					case ServerFileType.DocumentFile: return Server.DocumentUrl;
					case ServerFileType.SourceFile: return Server.SourceUrl;
					default: return null;
				}
			}
		}

		/// <summary>
		/// 서버파일 소속영역
		/// </summary>
		public ServerFileType ServerFileDomain
		{
			get { return serverFileDomain; }
			set { serverFileDomain = value; }
		}

		/// <summary>
		/// 파일선택버튼을 활성화할 것인지 여부
		/// </summary>
		public bool FileSelectButtonEnabled
		{
			get { return fileSelectButtonEnabled; }
			set { fileSelectButtonEnabled = value; }
		}

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
		///  문서형식명 컬럼의 인덱스
		/// </summary>
		public int DocTypeNameColumnIndex
		{
			get { return colDocTypeName; }
			set { colDocTypeName = value; }
		}

		/// <summary>
		///  문서개정번호 컬럼의 인덱스
		/// </summary>
		public int DocRevisionColumnIndex
		{
			get { return colDocRevision; }
			set { colDocRevision = value; }
		}

		/// <summary>
		///  문서번호 컬럼의 인덱스
		/// </summary>
		public int DocNumberColumnIndex
		{
			get { return colDocNumber; }
			set { colDocNumber = value; }
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
		#endregion

		#region 생성자
		/// <summary>
		/// 디펄트 버튼 이미지로 클래스를 초기화합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		public FileButtonManager(SheetView sheet, ServerFileType sftype)
		{
			this.sheet = sheet;
			sheet.FpSpread.ButtonClicked += fpSpread_ButtonClicked;
			sheet.FpSpread.CellDoubleClick += fpSpread_CellDoubleClick;
			serverFileDomain = sftype;

			FileSelectImage = EDocument.Resource.GridButtonImages.FileOpen;
			FileViewImage = EDocument.Resource.GridButtonImages.Read;
			FileDownloadImage = EDocument.Resource.GridButtonImages.Download;
			DisabledFileSelectImage = EDocument.Resource.GridButtonImages.FileOpenDisabled;
			DisabledFileViewImage = EDocument.Resource.GridButtonImages.ReadDisabled;
			DisabledFileDownloadImage = EDocument.Resource.GridButtonImages.DownloadDisabled;
		}

		~FileButtonManager()
		{
			DeleteTempFiles();
		}
		#endregion

		#region 메소드
		/// <summary>
		/// 열람을 위해 임시로 다운로드한 파일을 모두 삭제합니다.
		/// </summary>
		void DeleteTempFiles()
		{
			foreach (FileInfo f in new DirectoryInfo(Path.GetTempPath()).GetFiles(GetTempFilenamePrefix() + "*.*")) // 프리픽스파일 모두 삭제
			{
				try { f.Delete(); }
				catch { }
			}
		}

		/// <summary>
		/// 파일을 다운로드합니다.
		/// </summary>
		/// <param name="row">다운로드 파일의 행 인덱스</param>
		/// <param name="filename">다운로드할 로컬 파일명</param>
		/// <param name="showResultMessage">성공여부 메시지를 표시할 지 여부</param>
		/// <returns>성공여부</returns>
		bool DownloadFile(int row, string filename, bool showResultMessage)
		{
			string ftppath = Url.Combine(this.ServerFileRoot, sheet.Cells[row, colServerPath].Text + "/" + sheet.Cells[row, colServerFilename].Text);

			sheet.FpSpread.TopLevelControl.Cursor = Cursors.WaitCursor;
			string msg = null;
			bool ok = Ftp.DownloadFile(filename, ftppath, Server.AccountName, Server.AccountPassword, ref msg);
			sheet.FpSpread.TopLevelControl.Cursor = Cursors.Default;

			if (showResultMessage)
			{
				if (ok)
					MessageBox.Show("다운로드가 완료되었습니다.", "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else
					MessageBox.Show("다운로드에 실패했습니다: " + msg, "파일 다운로드", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return ok;
		}

		/// <summary>
		/// 임시파일명의 프리픽스로 사용할 고정된 문자열을 반환합니다.
		/// </summary>
		/// <returns></returns>
		string GetTempFilenamePrefix()
		{
			return string.Format("{0:X}", this.GetHashCode()) + "_";
		}

		/// <summary>
		/// 사용자가 첨부한 로컬 파일명을 가져옵니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <returns></returns>
		public string GetAttachedFilename(int row)
		{
			Cell serverPathCell = sheet.Cells[row, colServerPath];
			if (!string.IsNullOrEmpty(serverPathCell.Text) && serverPathCell.Text.StartsWith(AttChar)) // 업로드된 파일이 아닌경우만 첨부파일명 반환
				return serverPathCell.Text.Substring(1);
			return string.Empty;
		}

		/// <summary>
		/// 첨부파일을 지정합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <param name="filename">경로를 포함한 전체파일명</param>
		/// <param name="overwrite">이지 첨부파일이 지정된 경우 덮어쓸지 여부</param>
		/// <returns></returns>
		public bool SetAttachedFilename(int row, string filename, bool overwrite)
		{
			Cell serverPathCell = sheet.Cells[row, colServerPath];
			if (string.IsNullOrEmpty(serverPathCell.Text) || (overwrite && serverPathCell.Text.StartsWith(AttChar))) // 업로드된 파일이 아닌경우만 첨부파일명 반환
			{
				serverPathCell.Text = AttChar + filename;
				sheet.Cells[row, colFilename].Text = Path.GetFileName(filename);
				return true;
			}
			return false;
		}
		/// <summary>
		/// 첨부파일을 지정합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <param name="filename">경로를 포함한 전체파일명</param>
		/// <returns></returns>
		public bool SetAttachedFilename(int row, string filename)
		{
			return SetAttachedFilename(row, filename, true);
		}

		/// <summary>
		/// 업로드할 파일을 선택합니다. 파일 열기 대화창이 표시되며 시트에 파일명이 입력됩니다.
		/// </summary>
		/// <param name="row">자료목록 줄번호</param>
		/// <returns>파일이 지정되었는지 여부</returns>
		bool SelectUploadFile(int row)
		{
			Cell serverPathCell = sheet.Cells[row, colServerPath];
			if (!string.IsNullOrEmpty(serverPathCell.Text) && !serverPathCell.Text.StartsWith(AttChar)) // 이미 서버경로가 지정되어 업로드된 파일이 있다면 교체 불가
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
				serverPathCell.Value = AttChar + dlg.FileName; // 임시로 서버경로셀에 전체파일명 기록(업로드시 처리)
				sheet.Cells[row, colFilename].Value = Path.GetFileName(dlg.FileName); // 원본 파일명
				return true;
			}

			return false;
		}

		/// <summary>
		/// 문서열람창을 띄워 파일을 봅니다.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		bool ShowFileView(int row)
		{
			string filename;
			string ext = Path.GetExtension(sheet.Cells[row, colFilename].Text); // 확장자
			if (!string.IsNullOrEmpty(ext)) ext = ext.Substring(1);
			DeleteTempFiles();

			do { filename = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), GetTempFilenamePrefix() + Path.GetRandomFileName()), ext); } while (File.Exists(filename));
			bool ok = DownloadFile(row, filename, false);
			if (ok)
			{
				// 뷰어 팝업
				if (useInternalViewer)
				{
					Forms.FileViewer dialog = new Forms.FileViewer(filename);
					dialog.DisplayFilename = sheet.Cells[row, colFilename].Text;
					if (colDocTypeName > -1) dialog.DocTypeName = sheet.Cells[row, colDocTypeName].Text;
					if (colDocRevision > -1) dialog.DocRevision = sheet.Cells[row, colDocRevision].Text;
					if (colDocNumber > -1) dialog.DocNumber = sheet.Cells[row, colDocNumber].Text;
					dialog.ShowDialog();
				}
				// 외부 프로그램 연결
				else
				{
					System.Diagnostics.Process ps = new System.Diagnostics.Process();
					ps.StartInfo.FileName = filename;
					ps.StartInfo.WorkingDirectory = Path.GetTempPath();
					ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
					ps.Start();
				}
			}
			else
				MessageBox.Show("서버로 부터 파일을 불러오는데 실패했습니다.", "문서열람", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			if (useInternalViewer) File.Delete(filename);

			return ok;
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
			if (colFileSelect > -1)
			{
				cell = sheet.Cells[row, colFileSelect];
				cell.Locked = true;
				if (fileSelectButtonEnabled && bcFileSelect != null && bcDisabledFileSelect != null)
					cell.Locked = serverFileExists; // 업로드한 파일은 교체 불가
				cell.CellType = !cell.Locked ? bcFileSelect : bcDisabledFileSelect;
			}

			// 파일 미리보기 버튼
			if (colFileView > -1 && bcFileView != null && bcDisabledFileView != null)
			{
				cell = sheet.Cells[row, colFileView];
				if (useInternalViewer) cell.Locked = !(serverFileExists && fileext == ".pdf"); //TODO: 권한이 있는 경우만 허가할 것
				else cell.Locked = !serverFileExists;
				cell.CellType = !cell.Locked ? bcFileView : bcDisabledFileView;
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
		#endregion

		#region 이벤트 핸들러
		void fpSpread_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			// 파일선택 버튼 클릭
			if (e.Column == colFileSelect)
				SelectUploadFile(e.Row);

			// 파일보기 버튼 클릭
			else if (e.Column == colFileView)
				ShowFileView(e.Row);

			// 파일다운로드 버튼 클릭
			else if (e.Column == colFileDownload)
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.FileName = sheet.Cells[e.Row, colFilename].Text;
				dlg.DefaultExt = Path.GetExtension(dlg.FileName);
				dlg.Filter = dlg.DefaultExt.ToUpper() + " 파일" + "|*." + dlg.DefaultExt;
				if (dlg.ShowDialog() == DialogResult.OK)
					DownloadFile(e.Row, dlg.FileName, true);
			}
		}

		private void fpSpread_CellDoubleClick(object sender, CellClickEventArgs e)
		{
			// 파일명 셀 더블클릭시 파일선택
			if (e.Column == colFilename)
			{
				if (colFileSelect > -1 && !sheet.Cells[e.Row, colFileSelect].Locked)
				{
					if (fileSelectButtonEnabled) SelectUploadFile(e.Row);
				}
				else if (colFileView > -1 && !sheet.Cells[e.Row, colFileView].Locked)
					ShowFileView(e.Row);
			}
		}
		#endregion

	}
}
