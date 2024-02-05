using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;

namespace EDocument.Spread
{
	/// <summary>
	/// 첨부문서표시 관리자입니다.
	/// </summary>
	public class AttachmentManager
	{
		#region 필드
		const int defaultColWidth = 115;

		SheetView sheet;
		Dictionary<string, DocumentColumnInformation> docColumns = null;
		int colFirstDoc = -1;
		int colAttDocCds = -1;
		int colReqDocCds = -1;
		string plantCd = null;
		string docCtgCd = null;
		string displayDocCtgCd = null;
		bool hideEmptyColumns = false;
		#endregion

		#region 속성
		/// <summary>
		/// 공장코드
		/// </summary>
		public string PlantCode
		{
			get { return plantCd; }
			set { plantCd = value; }
		}

		/// <summary>
		/// 문서코드 카테고리
		/// </summary>
		public string DisplayDocCtgCd
		{
			get { return displayDocCtgCd; }
			set
			{
				if (value != null) value = value.Trim().ToUpper();
				if (docColumns != null && displayDocCtgCd == value) return;

				displayDocCtgCd = value;
				string query = "usp_T_DOC_CODE 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				if (!string.IsNullOrEmpty(displayDocCtgCd)) query += ", @pDOC_CTG_CD = '" + displayDocCtgCd + "'";
				Dictionary<string, string> docTypes = SystemBase.Base.CreateDictionary(query);
				docColumns = new Dictionary<string, DocumentColumnInformation>();
				foreach (string code in docTypes.Keys)
					docColumns.Add(code, new DocumentColumnInformation(docTypes[code]));
			}
		}

		/// <summary>
		/// 문서코드 카테고리
		/// </summary>
		public string DocCtgCd
		{
			get { return docCtgCd; }
			set { docCtgCd = value; }
		}

		/// <summary>
		/// 첨부문서코드 문자열 컬럼
		/// </summary>
		public int AttDocCdsColumn
		{
			get { return colAttDocCds; }
			set { colAttDocCds = value; }
		}

		/// <summary>
		/// 필수문서코드 문자열 컬럼
		/// </summary>
		public int ReqDocCdsColumn
		{
			get { return colReqDocCds; }
			set { colReqDocCds = value; }
		}

		/// <summary>
		/// 문서코드 사전(키는 문서코드, 값은 문서종류)
		/// </summary>
		public Dictionary<string, DocumentColumnInformation> DocumentColumns
		{
			get { return docColumns; }
		}

		/// <summary>
		/// 내용이 없는 컬럼을 숨길지 여부
		/// </summary>
		public bool HideEmptyColumns
		{
			get { return hideEmptyColumns; }
			set
			{
				hideEmptyColumns = value;
				UpdateColumnVisible();
			}
		}
		#endregion

		#region 생성자
		/// <summary>
		/// 첨부문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서첨부 카테고리</param>
		/// <param name="displayDocCtgCd">컬럼을 표시할 문서코드 카테고리(null 또는 빈문자열 지정시 문서코드 전체)</param>
		/// <param name="attDocCdsCol">첨부문서 코드문자열 컬럼 인덱스</param>
		/// <param name="reqDocCdsCol">필수문서 코드문자열 컬럼 인덱스</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string displayDocCtgCd, int attDocCdsCol, int reqDocCdsCol)
		{
			this.sheet = sheet;
			this.docCtgCd = docCtgCd;
			this.colAttDocCds = attDocCdsCol;
			this.colReqDocCds = reqDocCdsCol;
			this.colFirstDoc = sheet.Columns.Count; // 추가될 첫 번째 컬럼의 인덱스
			this.DisplayDocCtgCd = displayDocCtgCd;
		}

		/// <summary>
		/// 첨부문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서첨부 카테고리</param>
		/// <param name="displayDocCtgCd">컬럼을 표시할 문서코드 카테고리(null 또는 빈문자열 지정시 문서코드 전체)</param>
		/// <param name="attDocCdsColName">첨부문서 코드문자열 컬럼 헤더명</param>
		/// <param name="reqDocCdsCol">필수문서 코드문자열 컬럼 인덱스</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string displayDocCtgCd, string attDocCdsColName, int reqDocCdsCol)
			: this(sheet, docCtgCd, displayDocCtgCd, -1, reqDocCdsCol)
		{
			colAttDocCds = sheet.FindHeaderColumnIndex(attDocCdsColName);
			if (colAttDocCds < 0) throw new Exception(string.Format("헤더명 '{0}' 을 찾을 수 없습니다.", attDocCdsColName));
		}

		/// <summary>
		/// 첨부문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서첨부 카테고리</param>
		/// <param name="displayDocCtgCd">컬럼을 표시할 문서코드 카테고리(null 또는 빈문자열 지정시 문서코드 전체)</param>
		/// <param name="attDocCdsCol">첨부문서 코드문자열 컬럼 인덱스</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string displayDocCtgCd, int attDocCdsCol)
			: this(sheet, docCtgCd, displayDocCtgCd, attDocCdsCol, -1)
		{ }

		/// <summary>
		/// 첨부문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서첨부 카테고리</param>
		/// <param name="displayDocCtgCd">컬럼을 표시할 문서코드 카테고리(null 또는 빈문자열 지정시 문서코드 전체)</param>
		/// <param name="attDocCdsColName">첨부문서 코드문자열 컬럼 헤더명</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string displayDocCtgCd, string attDocCdsColName)
			: this(sheet, docCtgCd, displayDocCtgCd, attDocCdsColName, -1)
		{ }

		/// <summary>
		/// 첨부문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서첨부 카테고리</param>
		/// <param name="displayDocCtgCd">컬럼을 표시할 문서코드 카테고리(null 또는 빈문자열 지정시 문서코드 전체)</param>
		/// <param name="attDocCdsColName">첨부문서 코드문자열 컬럼 헤더명</param>
		/// <param name="reqDocCdsColName">필수문서 코드문자열 컬럼 헤더명</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string displayDocCtgCd, string attDocCdsColName, string reqDocCdsColName)
			: this(sheet, docCtgCd, displayDocCtgCd, attDocCdsColName, -1)
		{
			this.colReqDocCds = sheet.FindHeaderColumnIndex(reqDocCdsColName);
			if (this.colReqDocCds < 0) throw new Exception(string.Format("헤더명 '{0}' 을 찾을 수 없습니다.", reqDocCdsColName));
		}
		#endregion

		#region 메소드
		/// <summary>
		/// 스프레드에 문서첨부표시 컬럼들을 추가하고 값을 표시합니다.
		/// </summary>
		public void AppendColumns()
		{
			sheet.FpSpread.SuspendLayout(); // BeginUpdate 메소드 못찾음. SuspendLayout 사용시 20초 -> 7초로 속도 향상.

			// 컬럼 추가 및 헤더 표시
			int col = colFirstDoc;
			foreach (string docCode in docColumns.Keys)
			{
				sheet.Columns.Add(col, 1);
				sheet.ColumnHeader.Cells[0, col].Text = docColumns[docCode].Name;
				Column column = sheet.Columns[col];
				FarPoint.Win.Spread.CellType.TextCellType cellType = new FarPoint.Win.Spread.CellType.TextCellType();
				column.CellType = cellType;
				column.Width = defaultColWidth;
				column.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
				column.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
				column.BackColor = Color.FromArgb(245, 245, 245);
				column.Locked = true;
				col++;
			}

			// 품목별 첨부상태 표시
			for (int row = 0; row < sheet.Rows.Count; row++)
				UpdateColumns(row);

			UpdateColumnVisible();

			sheet.FpSpread.ResumeLayout();
		}

		/// <summary>
		/// 지정한 문서코드의 첨부셀에서 텍스트값을 읽어옵니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <param name="docCd">문서코드</param>
		/// <returns></returns>
		public string GetCellText(int row, string docCd)
		{
			int col = colFirstDoc;
			foreach (string docCode in docColumns.Keys)
			{
				if (string.Compare(docCd, docCode, true) == 0)
					return sheet.Cells[row, col].Text;
				col++;
			}
			return string.Empty;
		}

		/// <summary>
		/// 지정한 행의 첨부문서코드 문자열을 다리 로드하고 첨부표시를 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <param name="keys">첨부키 목록</param>
		public void ReloadData(int row, string[] keys)
		{
			StringBuilder query = new StringBuilder();
			query.Append("usp_T_DOC 'S_CDS'");
			query.Append(", @pDOC_CTG_CD = '" + docCtgCd + "'");
			query.Append(", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
			query.Append(", @pPLANT_CD = '" + plantCd + "' ");
			for (int kn = 0; kn < keys.Length; kn++)
				if (!string.IsNullOrEmpty(keys[kn]))
					query.AppendFormat(", @pATT_KEY{0} = '{1}'", kn + 1, keys[kn]);

			DataTable dt = SystemBase.DbOpen.NoTranDataTable(query.ToString());
			if (dt != null && dt.Rows.Count > 0)
			{
				sheet.Cells[row, colAttDocCds].Text = dt.Rows[0][0].ToString(); // 첨부문서코드 업데이트
				UpdateColumns(row); // 문서첨부표시 업데이트
			}
		}

		/// <summary>
		/// 지정한 행의 첨부문서표시를 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		public void UpdateColumns(int row)
		{
			string codeString = sheet.Cells[row, colAttDocCds].Text; // 첨부문서정보 코드문자열
			string reqCodeString = colReqDocCds > -1 ? sheet.Cells[row, colReqDocCds].Text : ""; // 필수첨부문서 코드문자열

			// 첨부표시루틴
			int colIndex = colFirstDoc;
			foreach (string docCode in docColumns.Keys)
			{
				Match match = Regex.Match(codeString, ",(" + docCode + @"):\d+,");
				Cell cell = sheet.Cells[row, colIndex];
				bool required = reqCodeString.Contains("," + docCode + ",");

				// 문서가 있는 경우
				if (match.Success)
				{
					// 문서 갯수 표시
					cell.Text = match.Value.Split(new char[] { ':', ',' })[2];
					cell.ForeColor = Color.Black;
				}
				// 문서가 없는 경우
				else
				{
					// 필수문서인 경우
					if (required)
					{
						// 빨강색 '0' 표시
						cell.Text = "0";
						cell.ForeColor = Color.Red;
					}
					// 아닌 경우 초기화
					else
					{
						cell.Text = "";
						cell.ForeColor = Color.Black;
					}
				}
				cell.BackColor = required ? EDocument.UIColors.RequiredBackground : EDocument.UIColors.ReadonlyBackground;
				colIndex++;
			}
		}

		/// <summary>
		/// 컬럼의 표시 상태를 업데이트합니다.
		/// </summary>
		public void UpdateColumnVisible()
		{
			if (colFirstDoc < 0 || colFirstDoc >= sheet.ColumnCount) return;

			int col = colFirstDoc;
			foreach (string docCode in docColumns.Keys)
			{
				bool visible = docColumns[docCode].Visible;
				// 내용(텍스트)가 있는지 확인
				if (visible && hideEmptyColumns)
				{
					bool empty = true;
					for (int row = 0; row < sheet.RowCount; row++)
						if (!string.IsNullOrEmpty(sheet.Cells[row, col].Text))
						{
							empty = false;
							break;
						}

					// 컬럼 숨김
					if (empty) visible = false; // 빈컬럼 숨김 설정인 경우 자동 숨김
				}

				sheet.Columns[col].Visible = visible;
				col++;
			}
		}
		#endregion
	}
}
