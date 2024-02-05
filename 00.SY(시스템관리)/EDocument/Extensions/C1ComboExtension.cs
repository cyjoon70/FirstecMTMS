using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using C1.Win.C1List;

namespace EDocument.Extensions.C1ComboExtension
{
	/// <summary>
	/// C1Combo 컨트롤 확장기능입니다.
	/// </summary>
	public static class C1ComboExtension
	{
		/// <summary>
		/// Value 값과 일치하는 아이템을 삭제합니다. DataMode 속성이 AddItem 이어야 합니다.
		/// </summary>
		/// <param name="combo">콤보박스</param>
		/// <param name="value"></param>
		public static void RemoveValue(this C1Combo combo, string value)
		{
			int valueCol = -1;
			for (int col = 0; col < combo.Columns.Count; col++)
				if (combo.Columns[col].Caption == combo.ValueMember)
				{
					valueCol = col;
					break;
				}
			if (valueCol < 0) return;

			int index = combo.FindString(value, 0, valueCol);
			if (index > -1) combo.RemoveItem(index);
		}

		/// <summary>
		/// 사전의 키-값 요소들로 콤보박스 리스트 아이템을 채웁니다. 요소의 키는 콤보아이템의 value 속성에, 값은 콤보아이템의 text 속성에 치환됩니다.
		/// </summary>
		/// <param name="combo">콤보박스</param>
		/// <param name="items">사전</param>
		public static void SetItems(this C1Combo combo, Dictionary<string, string> items)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("key");
			dt.Columns.Add("value");
			foreach (string key in items.Keys)
				dt.Rows.Add(new string[] { key, items[key] });

			combo.Refresh();
			combo.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;
			combo.ColumnHeaders = false;
			combo.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
			combo.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
			combo.ValueMember = dt.Columns[0].ColumnName;
			combo.DisplayMember = dt.Columns[1].ColumnName;
			combo.DataSource = dt;
			combo.Splits[0].DisplayColumns[0].Width = 0;
			combo.Splits[0].DisplayColumns[1].Width = combo.Size.Width;
			combo.SelectedIndex = 0;
		}

		/// <summary>
		/// 쿼리 결과로 콤보박스 리스트의 아이템을 채웁니다. 콤보박스는 아이템 추가모드로 동작합니다.
		/// </summary>
		/// <param name="combo">콤보박스</param>
		/// <param name="query">쿼리</param>
		/// <param name="fieldNames">아이템 값으로 사용할 필드명들</param>
		/// <param name="valueCol">값으로 사용할 컬럼 인덱스</param>
		/// <param name="displayCol">드롭다운에 표시할 컬럼 인덱스</param>
		public static void SetItems(this C1Combo combo, string query, string[] fieldNames, int valueCol, int displayCol)
		{
			SetItems(combo, query, fieldNames, valueCol, displayCol, null);
		}

		/// <summary>
		/// 쿼리 결과로 콤보박스 리스트의 아이템을 채웁니다. 콤보박스는 아이템 추가모드로 동작합니다.
		/// </summary>
		/// <param name="combo">콤보박스</param>
		/// <param name="query">쿼리</param>
		/// <param name="fieldNames">아이템 값으로 사용할 필드명들</param>
		/// <param name="displayCol">드롭다운에 표시할 컬럼 인덱스</param>
		/// <param name="insertion">최상단에 추가할 아이템 값</param>
		public static void SetItems(this C1Combo combo, string query, string[] fieldNames, int valueCol, int displayCol, string[] insertion)
		{
			combo.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;
			combo.DataMode = DataModeEnum.AddItem;
			if (fieldNames != null && fieldNames.Length == 0) fieldNames = null;

			// 컬럼 설정
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
			int colCount;
			if (fieldNames == null)
			{
				colCount = dt.Columns.Count;
				combo.AddItemCols = colCount;
				for (int col = 0; col < colCount; col++)
					combo.Columns[col].Caption = dt.Columns[col].ColumnName;
			}
			else
			{
				colCount = fieldNames.Length;
				combo.AddItemCols = colCount;
				for (int col = 0; col < colCount; col++)
					combo.Columns[col].Caption = fieldNames[col];
			}
			combo.ValueMember = combo.Columns[valueCol].Caption; // 값으로 사용할 컬럼명 지정
			combo.DisplayMember = combo.Columns[displayCol].Caption; // 선택된 텍스트로 사용할 컬럼명 지정

			StringBuilder itemValue = new StringBuilder();

			// 추가템 삽입
			if (insertion != null)
			{
				DataRow dr = dt.NewRow();
				for (int col = 0; col < Math.Min(colCount, insertion.Length); col++)
				{
					if (itemValue.Length > 0) itemValue.Append(";");
					itemValue.Append(insertion[col]);
				}
				combo.AddItem(itemValue.ToString());
			}

			// 리스트업
			foreach (DataRow row in dt.Rows)
			{
				itemValue.Length = 0;
				for (int col = 0; col < (fieldNames == null ? dt.Columns.Count : fieldNames.Length); col++)
				{
					if (itemValue.Length > 0) itemValue.Append(";");
					itemValue.Append((fieldNames == null ? row[col] : row[fieldNames[col]]).ToString());
				}
				combo.AddItem(itemValue.ToString());
			}

			// 디스플레이 컬럼을 제외한 다른 컬럼 숨기기
			for (int col = 0; col < colCount; col++)
				combo.Splits[0].DisplayColumns[col].Width = col == displayCol ? combo.Width : 0;

			// 기타설정
			combo.AllowColMove = false;
			combo.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
			combo.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
			combo.ColumnHeaders = false;
			combo.SelectedIndex = 0;

		}
	}
}
