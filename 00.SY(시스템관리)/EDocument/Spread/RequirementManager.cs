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
	/// 필수첨부표시 관리자입니다.
	/// </summary>
	public class RequirementManager
	{
		#region 필드
		const int defaultColWidth = 115;

		SheetView sheet;
		Dictionary<string, DocumentColumnInformation> docColumns = null;
		int colFirstDoc = -1;
		int colDocReqCds = -1;
		string docCtgCd = null;
		bool hideEmptyColumns = false;
		#endregion

		#region 속성
		/// <summary>
		/// 문서코드 카테고리
		/// </summary>
		public string DocCtgCd
		{
			get { return docCtgCd; }
			set { docCtgCd = value; }
		}

		/// <summary>
		/// 필수문서코드 문자열 컬럼
		/// </summary>
		public int DocReqCdsColumn
		{
			get { return colDocReqCds; }
			set { colDocReqCds = value; }
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
		/// 필수문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">표시할 문서코드 카테고리(빈 문자열은 전체)</param>
		/// <param name="docReqCdsCol"></param>
		public RequirementManager(SheetView sheet, string docCtgCd, int docReqCdsCol)
		{
			this.sheet = sheet;
			this.docCtgCd = docCtgCd;
			this.colDocReqCds = docReqCdsCol;
			string query = "usp_T_DOC_CODE 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
			if (!string.IsNullOrEmpty(docCtgCd)) query += ", @pDOC_CTG_CD = '" + docCtgCd + "'";
			Dictionary<string, string> docTypes = SystemBase.Base.CreateDictionary(query);
			docColumns = new Dictionary<string, DocumentColumnInformation>();
			foreach (string code in docTypes.Keys)
				docColumns.Add(code, new DocumentColumnInformation(docTypes[code]));
			colFirstDoc = sheet.Columns.Count;
		}

		/// <summary>
		/// 필수문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="displayDocCtgCd">문서카테고리 코드</param>
		/// <param name="targetType">대상형식</param>
		/// <param name="targetKeyColName">첨부문서 코드문자열 컬럼 헤더명</param>
		public RequirementManager(SheetView sheet, string docCtgCd, string docReqCdsColName)
			: this(sheet, docCtgCd, -1)
		{
			colDocReqCds = sheet.FindHeaderColumnIndex(docReqCdsColName);
			if (colDocReqCds < 0)
				throw new Exception(string.Format("헤더명 '{0}' 을 찾을 수 없습니다.", docReqCdsColName));
		}

		/// <summary>
		/// 필수문서표시 관리자 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="targetType">대상형식</param>
		/// <param name="targetKeyColName">첨부문서 코드문자열 컬럼 헤더명</param>
		public RequirementManager(SheetView sheet, string docReqCdsColName)
			: this(sheet, "", docReqCdsColName)
		{ }
		#endregion

		#region 메소드
		/// <summary>
		/// 스프레드에 필수첨부표시 컬럼들을 추가하고 값을 표시합니다.
		/// </summary>
		public void AppendColumns()
		{
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

			// 품목별 필수첨부상태 표시
			for (int row = 0; row < sheet.Rows.Count; row++)
				UpdateColumns(row);

			UpdateColumnVisible();
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
		/// 지정한 행의 필수문서표시를 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		public void UpdateColumns(int row)
		{
			string reqCodeString =sheet.Cells[row, colDocReqCds].Text; // 필수첨부문서 코드문자열
			int colIndex = colFirstDoc;
			foreach (string docCode in docColumns.Keys)
			{
				Cell cell = sheet.Cells[row, colIndex];
				bool required = reqCodeString.Contains("," + docCode + ",");
				cell.Text = required ? "○" : "";
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
