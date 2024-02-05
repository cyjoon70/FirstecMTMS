using System;
using System.Text;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;


namespace UIForm.Extension.FpSpread
{
	/// <summary>
	/// FpSpread의 확장기능입니다.
	/// </summary>
	public static class FpSpreadExtension
	{

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
	}
}
