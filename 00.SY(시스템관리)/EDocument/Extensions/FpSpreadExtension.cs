using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using FarPoint.Win.Spread.Model;

namespace EDocument.Extensions.FpSpreadExtension
{
	/// <summary>
	/// 행 모양 열거
	/// </summary>
	public enum CellAppearance
	{
		/// <summary>일반</summary>
		Normal = 0,
		/// <summary>폐기됨</summary>
		Discard,
	}

	/// <summary>
	/// 행데이터 클래스
	/// </summary>
	public class RowData
	{
		/// <summary>행 인덱스</summary>
		public int Row = -1;
		/// <summary>키 목록</summary>
		public string[] Values = null;

		#region 생성자
		/// <summary>
		/// 행 데이터 인스턴스를 생성합니다.
		/// </summary>
		public RowData() { }

		/// <summary>
		/// 행 데이터 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="row">행 번호</param>
		/// <param name="values">값 배열</param>
		public RowData(int row, string[] values)
		{
			this.Row = row;
			this.Values = values;
		}
		#endregion

		#region 속성
		/// <summary>
		/// 모든 값을 슬래시로 이어붙인 키 조합입니다.
		/// </summary>
		public string KeyCombination
		{
			get
			{
				StringBuilder s = new StringBuilder();
				for (int index = 0; index < Values.Length; index++)
				{
					if (s.Length > 0) s.Append("/");
					s.Append(Values[index]);
				}
				return s.ToString();
			}
		}
		#endregion

		#region 메소드
		/// <summary>
		/// 지정한 인덱스 범위의 값을 슬래시로 이어붙인 키 조합을 생성합니다.
		/// </summary>
		/// <param name="startValueIndex">시작값 인덱스</param>
		/// <param name="count">값 갯수</param>
		/// <returns></returns>
		public string GetKeyCombination(int startValueIndex, int count)
		{
			StringBuilder s = new StringBuilder();
			for (int index = startValueIndex; index < startValueIndex + count; index++)
			{
				if (s.Length > 0) s.Append("/");
				s.Append(Values[index]);
			}
			return s.ToString();
		}

		/// <summary>
		/// 지정한 갯수의 값을 슬래시로 이어붙인 키 조합을 생성합니다.
		/// </summary>
		/// <param name="count">값 갯수</param>
		/// <returns></returns>
		public string GetKeyCombination(int count)
		{
			return GetKeyCombination(0, count);
		}

		/// <summary>
		/// 행 데이터가 같은지 비교합니다.
		/// </summary>
		/// <param name="rdata">비교할 행 데이터</param>
		/// <returns>모든 값이 같을 경우 true. 그렇지 않으면 false.</returns>
		public bool SameWith(RowData rdata)
		{
			for (int col = 0; col < Values.Length; col++)
				if (Values[col] != rdata.Values[col])
					return false;
			return true;
		}
		#endregion
	}

	/// <summary>
	/// 행데이터 컬렉션
	/// </summary>
	public class RowDataList : List<RowData>
	{
		/// <summary>
		/// 요소를 추가합니다. 값이 중복되면 추가되지 않습니다.
		/// </summary>
		/// <param name="rdata"></param>
		public new void Add(RowData rdata)
		{
			if (!ContainsData(rdata))
				base.Add(rdata);
		}

		/// <summary>
		/// 전달된 컬렉션의 모든 요소를 추가합니다. 값이 중복되는 요소는 건너뜁니다.
		/// </summary>
		/// <param name="list"></param>
		public void Add(RowDataList list)
		{
			if (list == null) return;
			foreach (RowData rdata in list)
				Add(rdata);
		}

		/// <summary>
		/// 전달된 값으로 새 요소를 추가합니다. 값이 중복되는 요소는 건너뜁니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <param name="values">값 배열</param>
		public void Add(int row, string[] values)
		{
			RowData rdata = new RowData();
			rdata.Row = row;
			rdata.Values = values;
			Add(rdata);
		}

		/// <summary>
		/// 지정한 데이터와 모든 값이 일치하는 요소가 있는지 확인합니다.
		/// </summary>
		/// <param name="rdata">행 데이터</param>
		/// <returns></returns>
		public bool ContainsData(RowData rdata)
		{
			for (int index = 0; index < this.Count; index++)
				if (this[index].SameWith(rdata)) return true;
			return false;
		}
	}

	/// <summary>
	/// FpSpread 컨트롤 확장기능입니다.
	/// </summary>
	public static class FpSpreadExtension
	{
		#region SheetView 확장
		/// <summary>
		/// 선택된 행에서 셀을 가져옵니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="col">열 인덱스</param>
		/// <returns></returns>
		public static Cell ActiveRowCell(this SheetView sheet, int col)
		{
			if (sheet.ActiveRowIndex > -1) return sheet.Cells[sheet.ActiveRowIndex, col];
			return null;
		}

		/// <summary>
		///  시트의 내용이 변경되었는지 확인합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <returns>변경 여부</returns>
		public static bool CheckEdited(this SheetView sheet)
		{
			for (int row = 0; row < sheet.RowCount; row++)
				if (!string.IsNullOrEmpty(sheet.RowHeader.Cells[row, 0].Text)) return true;
			return false;
		}

		/// <summary>
		/// 행추가가 되었는지 확인합니다.
		/// </summary>
		/// <returns>행추가 여부</returns>
		/// <param name="sheet">시트</param>
		public static bool CheckRowInserted(this SheetView sheet)
		{
			return CheckRowHeaderText(sheet, "I");
		}

		/// <summary>
		/// 행삭제가 되었는지 확인합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <returns>행삭제 여부</returns>
		public static bool CheckRowDeleted(this SheetView sheet)
		{
			return CheckRowHeaderText(sheet, "D");
		}

		/// <summary>
		/// 행 해더 텍스트를 확인합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="text">일치를 확인할 텍스트</param>
		/// <returns>일치하는 텍스트가 있는지 여부</returns>
		public static bool CheckRowHeaderText(this SheetView sheet, string text)
		{
			return CheckRowHeaderText(sheet, text, false);
		}

		/// <summary>
		/// 행 해더 텍스트를 확인합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="text">일치를 확인할 텍스트</param>
		/// <returns>일치하는 텍스트가 있는지 여부</returns>
		public static bool CheckRowHeaderText(this SheetView sheet, string text, bool ignoreCase)
		{

			for (int row = 0; row < sheet.RowCount; row++)
				if (string.Compare(sheet.RowHeader.Cells[row, 0].Text, text, ignoreCase) == 0) return true;

			return false;
		}

		/// <summary>
		/// 선택영역과 활성영역을 초기화합니다.
		/// </summary>
		/// <param name="sheet"></param>
		public static void ResetSelection(this SheetView sheet)
		{
			sheet.ClearSelection();
			sheet.ActiveRowIndex = -1;
			sheet.ActiveColumnIndex = -1;
		}

		/// <summary>
		/// 내용이 변경되었다면 무시할 것인지 묻고 진행 여부를 반환합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <returns>진행해도 되는지 여부</returns>
		public static bool EnsureEdited(this SheetView sheet)
		{
			if (CheckEdited(sheet))
				return MessageBox.Show("내용이 변경되었습니다. 변경사항을 무시할까요?", "변경된 내용 확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK;
			return true;
		}

		/// <summary>
		/// 헤더명을 찾고 열 인덱스를 반환합니다. 대소문자는 구별하지 않습니다.
		/// </summary>
		/// <param name="spread">그리드 컨트롤</param>
		/// <param name="sheetIndex">시트 인덱스</param>
		/// <param name="name">헤더명</param>
		/// <returns></returns>
		public static int FindHeaderColumnIndex(this FpSpread spread, int sheetIndex, string name)
		{
			return FindHeaderColumnIndex(spread, sheetIndex, name, false);
		}

		/// <summary>
		/// 헤더명을 찾고 열 인덱스를 반환합니다.
		/// </summary>
		/// <param name="spread">그리드 컨트롤</param>
		/// <param name="sheetIndex">시트 인덱스</param>
		/// <param name="name">헤더명</param>
		/// <param name="caseSensitive">대소문자 구분 여부</param>
		/// <returns></returns>
		public static int FindHeaderColumnIndex(this FpSpread spread, int sheetIndex, string name, bool caseSensitive)
		{
			SheetView sheet = spread.Sheets[sheetIndex];
			int dummy = -1;
			int col = -1;
			spread.SearchHeaders(0, name, false, caseSensitive, true, false, false, true, false, false, 0, 0, sheet.ColumnHeader.Rows.Count - 1, sheet.ColumnCount - 1, ref dummy, ref col);
			return col;
		}

		/// <summary>
		/// 헤더명을 찾고 열 인덱스를 반환합니다. 대소문자는 구별하지 않습니다.
		/// </summary>
		/// <param name="sheet">그리드 시트</param>
		/// <param name="name">헤더명</param>
		/// <returns></returns>
		public static int FindHeaderColumnIndex(this SheetView sheet, string name)
		{
			return FindHeaderColumnIndex(sheet, name, false);
		}

		/// <summary>
		/// 헤더명을 찾고 열 인덱스를 반환합니다.
		/// </summary>
		/// <param name="sheet">그리드 시트</param>
		/// <param name="name">헤더명</param>
		/// <param name="caseSensitive">대소문자 구분 여부</param>
		/// <returns></returns>
		public static int FindHeaderColumnIndex(this SheetView sheet, string name, bool caseSensitive)
		{
			int dummy = -1;
			int col = -1;
			sheet.FpSpread.SearchHeaders(sheet.FpSpread.Sheets.IndexOf(sheet), name, false, caseSensitive, true, false, false, true, false, false, 0, 0, sheet.ColumnHeader.Rows.Count - 1, sheet.ColumnCount - 1, ref dummy, ref col);
			return col;
		}

		/// <summary>
		/// 행 인덱스와 헤더명으로 셀을 찾습니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="row">행 인덱스</param>
		/// <param name="headerName">해더명</param>
		/// <returns></returns>
		public static Cell FindCell(this SheetView sheet, int row, string headerName)
		{
			return sheet.Cells[row, FindHeaderColumnIndex(sheet, headerName)];
		}

		/// <summary>
		/// 지정한 열에서 텍스트 값이 일치하는 행을 찾습니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="col">열 인덱스</param>
		/// <param name="text">텍스트 값</param>
		/// <returns></returns>
		public static int FindRowIndex(this SheetView sheet, int col, string text)
		{
			for (int row = 0; row < sheet.RowCount; row++)
				if (string.Compare(sheet.Cells[row, col].Text, text, true) == 0)
					return row;
			return -1;
		}

		/// <summary>
		/// 텍스트 값이 일치하는 행을 찾아 인덱스를 반환합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="cols">검색할 행 인덱스들</param>
		/// <param name="values">비교할 필드값들</param>
		/// <returns>일치하는 행의 인덱스. 일치하지 않으면 -1.</returns>
		public static int FindText(this SheetView sheet, int[] cols, string[] values)
		{
			for (int row = 0; row < sheet.RowCount; row++)
			{
				bool same = true;
				for (int col = 0; col < cols.Length; col++)
					if (sheet.Cells[row, cols[col]].Text != values[col])
					{
						same = false;
						break;
					}
				if (same) return row;
			}

			return -1;
		}

		/// <summary>
		/// 체크값이 true인 행의 갯수를 구합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="checkCol">체크박스 열 인덱스</param>
		/// <returns></returns>
		public static int GetCheckedRowCount(this SheetView sheet, int checkCol)
		{
			int count = 0;
			for (int row = 0; row < sheet.RowCount; row++)
			{
				Cell cell = sheet.Cells[row, checkCol];
				if (cell.CellType is CheckBoxCellType && cell.Text == "True")
					count++;
			}
			return count;
		}

		/// <summary>
		/// 체크된 항목에 대한 값을 가져옵니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="colCheck">체크박스열 인덱스</param>
		/// <returns>값 목록</returns>
		public static RowDataList GetCheckedRowData(this SheetView sheet, int[] valueCols)
		{
			for (int col = 0; col < sheet.ColumnCount; col++)
				if (sheet.ColumnHeader.Cells.Get(0, col).CellType is CheckBoxCellType)
					return GetCheckedRowData(sheet, col, valueCols);
			return null;
		}

		/// <summary>
		/// 체크된 항목에 대한 값을 가져옵니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="colCheck">체크박스열 인덱스</param>
		/// <param name="valueCols">값을 추출할 열 인덱스</param>
		/// <returns>값 목록</returns>
		public static RowDataList GetCheckedRowData(this SheetView sheet, int colCheck, int[] valueCols)
		{
			RowDataList rows = new RowDataList(); // 체크된 첨부키 목록
			for (int row = 0; row < sheet.RowCount; row++)
				if (Convert.ToString(sheet.Cells[row, colCheck].Value) == "True")
				{
					RowData key = new RowData();
					key.Row = row;
					key.Values = new string[valueCols.Length];
					for (int kcol = 0; kcol < valueCols.Length; kcol++)
						key.Values[kcol] = sheet.Cells[row, valueCols[kcol]].Text;
					if (!rows.ContainsData(key)) rows.Add(key);
				}

			return rows.Count > 0 ? rows : null;
		}

		/// <summary>
		/// 체크값이 true인 행의 갯수를 구합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="checkCol">체크박스 열 인덱스</param>
		/// <returns></returns>
		public static int[] GetCheckedRowIndices(this SheetView sheet, int checkCol)
		{
			List<int> indices = new List<int>();
			if (sheet.ColumnHeader.Cells.Get(0, checkCol).CellType is CheckBoxCellType)
				for (int row = 0; row < sheet.RowCount; row++)
					if (sheet.Cells[row, checkCol].Text == "True")
						indices.Add(row);
			return indices.ToArray();
		}

		/// <summary>
		/// 삭제된 항목에 대한 값을 가져옵니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="valueCols">값을 추출할 열 인덱스</param>
		/// <returns>값 목록</returns>
		public static RowDataList GetDeletedRowData(this SheetView sheet, int[] valueCols)
		{
			RowDataList rows = new RowDataList(); // 업데이트가 필요한 첨부키 목록

			// 삭제인 경우 첨부대상 서브그리드 항목 추출(업데이트는 문서코드가 바뀌지 않으므로 문서갯수에 영향을 주지 않음)
			for (int row = 0; row < sheet.RowCount; row++)
			{
				// 삭제인 경우
				if (sheet.RowHeader.Cells[row, 0].Text == "D")
				{
					// 변경에정인 첨부문서 항목으로부터 첨부키 추출
					RowData key = new RowData();
					key.Values = new string[valueCols.Length];
					for (int kindex = 0; kindex < valueCols.Length; kindex++)
						key.Values[kindex] = sheet.Cells[row, valueCols[kindex]].Text;
					if (!rows.ContainsData(key)) rows.Add(key); // 키 추가
				}
			}

			return rows.Count > 0 ? rows : null;
		}

		/// <summary>
		/// 시트의 잠금설정을 변경합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="locked">잠금여부</param>
		public static void Lock(this SheetView sheet, bool locked)
		{
			Lock(sheet, locked, false);
		}

		/// <summary>
		/// 시트의 잠금설정을 변경합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="locked">잠금여부</param>
		/// <param name="bypassCheckbox">체크박스는 건너뛸지 여부</param>
		public static void Lock(this SheetView sheet, bool locked, bool bypassCheckbox)
		{
			for (int col = 0; col < sheet.ColumnCount; col++)
			{
				if (bypassCheckbox && sheet.ColumnHeader.Cells[0, col].CellType is CheckBoxCellType) continue;
				for (int row = 0; row < sheet.RowCount; row++)
					sheet.Cells[row, col].Locked = locked;
			}
		}

		/// <summary>
		/// 전체선택 체크박스의 체크상태를 토글합니다.
		/// </summary>
		/// <param name="sheet">시트</param>
		/// <param name="col">토글할 열 인덱스</param>
		/// <returns>토글이 성공했는지 여부</returns>
		public static bool ToggleCheckAll(this SheetView sheet, int col)
		{
			int headerRow = sheet.ColumnHeader.RowCount > 2 ? 2 : sheet.ColumnHeader.RowCount > 1 ? 1 : 0;
			Cell headerCell = sheet.ColumnHeader.Cells.Get(headerRow, col);
			if (headerCell.CellType is CheckBoxCellType)
			{
				bool newValue = headerCell.Text != "True";
				headerCell.Value = newValue;
				for (int row = 0; row < sheet.Rows.Count; row++)
				{
					FarPoint.Win.Spread.Cell cell = sheet.Cells[row, col];
					if (cell.Locked == false) cell.Value = newValue;
				}
				return true;
			}

			return false;
		}
		#endregion

		#region Row 확장
		/// <summary>
		/// 행의 모양을 지정합니다.
		/// </summary>
		/// <param name="row">행</param>
		/// <param name="appearance">모양</param>
		public static void SetApprearance(this Row row, CellAppearance appearance)
		{
			switch (appearance)
			{
				// 일반
				case CellAppearance.Normal:
					row.ForeColor = UIColors.Normal;
					row.BackColor = UIColors.NormalBackground;
					break;

				// 폐기
				case CellAppearance.Discard:
					row.ForeColor = UIColors.Discard;
					row.BackColor = UIColors.ReadonlyBackground;
					break;
			}
		}
		#endregion

		#region Cell 확장
		/// <summary>
		/// 셀의 모양을 지정합니다.
		/// </summary>
		/// <param name="cell">셀</param>
		/// <param name="appearance">모양</param>
		public static void SetApprearance(this Cell cell, CellAppearance appearance)
		{
			switch (appearance)
			{
				// 일반
				case CellAppearance.Normal:
					cell.ForeColor = UIColors.Normal;
					cell.BackColor = UIColors.NormalBackground;
					break;

				// 폐기
				case CellAppearance.Discard:
					cell.ForeColor = UIColors.Discard;
					cell.BackColor = UIColors.ReadonlyBackground;
					break;
			}
		}
		#endregion
	}
}
