using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using FarPoint.Win.Spread;
using UIForm.Extensions.FpSpreadExtension;

namespace EDocument.Spread
{
	/// <summary>
	/// 스프레드상의 문서첨부표시 관리자입니다.
	/// </summary>
	public class AttachmentManager
	{
		#region 변수 선언
		SheetView sheet;
		Dictionary<string, string> docCodes = null;
		int colFirstAttDocCd = -1;
		int colAttDocCds = -1;
		#endregion

		/// <summary>
		/// 클래스 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서카테고리 코드</param>
		/// <param name="attDocCdsCol">첨부문서 코드문자열 컬럼 인덱스</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, int attDocCdsCol)
		{
			this.sheet = sheet;
			colAttDocCds = attDocCdsCol;
			docCodes = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pDOC_CTG_CD = '" + docCtgCd + "'");
			colFirstAttDocCd = sheet.Columns.Count;
		}

		/// <summary>
		/// 클래스 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="sheet">스프레드 시트</param>
		/// <param name="docCtgCd">문서카테고리 코드</param>
		/// <param name="attDocCodeColName">첨부문서 코드문자열 컬럼 헤더명</param>
		public AttachmentManager(SheetView sheet, string docCtgCd, string attDocCodeColName)
			: this(sheet, docCtgCd, -1)
		{
			colAttDocCds = sheet.FindHeaderColumnIndex(attDocCodeColName);
			if (colAttDocCds < 0)
				throw new Exception(string.Format("헤더명 '{0}' 을 찾을 수 없습니다.", attDocCodeColName));
		}

		/// <summary>
		/// 스프레드에 문서첨부표시 컬럼들을 추가하고 값을 표시합니다.
		/// </summary>
		public void AppendDocumentAttachmentColumns()
		{
			// 컬럼 추가 및 헤더 표시
			int colIndex = colFirstAttDocCd;
			foreach (string docCode in docCodes.Keys)
			{
				sheet.Columns.Add(colIndex, 1);
				sheet.ColumnHeader.Cells[0, colIndex].Text = docCodes[docCode];
				Column column = sheet.Columns[colIndex];
				column.Width = 100;
				column.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
				column.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
				column.BackColor = Color.FromArgb(245, 245, 245);
				colIndex++;
			}

			// 품목별 첨부상태 표시
			for (int row = 0; row < sheet.Rows.Count; row++)
				UpdateDocumentAttachmentStates(row);
		}

		/// <summary>
		/// 스프레드 행의 문서첨부표시를 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		public void UpdateDocumentAttachmentStates(int row)
		{
			int colIndex = colFirstAttDocCd;
			string codeString = sheet.Cells[row, colAttDocCds].Text;
			foreach (string docCode in docCodes.Keys)
			{
				sheet.Cells[row, colIndex].Text = codeString.Contains(docCode) ? "○" : "";
				colIndex++;
			}
		}
	}
}
