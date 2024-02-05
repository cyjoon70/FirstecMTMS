using System;
using System.Text;
using System.Windows.Forms;

namespace EDocument.Forms
{
	/// <summary>
	/// PDF 파일 열람 팝업입니다.
	/// </summary>
	public partial class FileViewer : Form
	{
		#region 변수 선어
		string filename;
		#endregion

		/// <summary>
		/// PDF 파일 열람 팝업의 인스턴스를 생성합니다.
		/// </summary>
		public FileViewer(string filename)
		{
			InitializeComponent();
			panViewer.Enabled = true;
			pdfViewer.setShowToolbar(true);

			this.Filename = filename;
			this.DisplayFilename = "";
			this.DocTypeName = "";
			this.DocRevision = "";
			this.DocNumber = "";
		}

		#region 속성
		/// <summary>
		/// 열람할 전체파일명입니다. 확장자가 PDF 형식이어야 합니다.
		/// </summary>
		public string Filename
		{
			get { return filename; }
			set { filename = value; }
		}

		/// <summary>
		/// 창에 표시할 파일명입니다.
		/// </summary>
		public string DisplayFilename
		{
			get { return txtFilename.Text; }
			set { txtFilename.Value = value; }
		}

		/// <summary>
		/// 파일의 종류입니다.
		/// </summary>
		public string DocTypeName
		{
			get { return txtDocTypeName.Text; }
			set { txtDocTypeName.Value = value; }
		}

		/// <summary>
		/// 파일의 개정번호입니다.
		/// </summary>
		public string DocRevision
		{
			get { return txtDocRevision.Text; }
			set { txtDocRevision.Value = value; }
		}

		/// <summary>
		/// 문서번호입니다.
		/// </summary>
		public string DocNumber
		{
			get { return txtDocNumber.Text; }
			set { txtDocNumber.Value = value; }
		}
		#endregion

		private void WNDW035_Load(object sender, EventArgs e)
		{
			try
			{
				pdfViewer.LoadFile(filename);
			}
			catch
			{
				MessageBox.Show("파일을 여는데 실패했습니다.", "PDF 파일 불러오기", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.Close();
			}
		}

		private void WNDW035_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.PageUp:
				case Keys.Up:
					btnPrevPage_Click(sender, e);
					break;

				case Keys.PageDown:
				case Keys.Down:
					btnNextPage_Click(sender, e);
					break;
			}
		}

		private void btnPrevPage_Click(object sender, EventArgs e)
		{
			pdfViewer.gotoPreviousPage();
		}

		private void btnNextPage_Click(object sender, EventArgs e)
		{
			pdfViewer.gotoNextPage();
		}
	}
}
