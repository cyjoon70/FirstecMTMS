using System;
using System.Data;
using System.Net;

namespace SystemBase
{
	/// <summary>
	/// ComboMake에 대한 요약 설명입니다.
	/// </summary>
	public class ComboMake
	{
		#region Combo
		public static void Combo(string Query, System.Windows.Forms.ComboBox CenterName)
		{	
			DataTable dt = DbOpen.NoTranDataTable(Query.ToString());

			CenterName.DisplayMember = "cobDisplay";
			CenterName.ValueMember = "cobValue";
			CenterName.DataSource = dt;
			CenterName.SelectedIndex=0;
		}
		#endregion

		#region Combo usp_C0_COMMON의 P13에 DEF_FLAG 필드까지 가지오는 경우 콤보의 기본값 자동선택
		public static void Combo(System.Windows.Forms.ComboBox comboBox1, string Query)
		{	
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			DataView dv			= new DataView(dt);
			//dv.RowFilter		= "DEF_FLAG = 'Y'";

			comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
			comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
			comboBox1.DataSource	= dt;

			if(dv.Count > 0)
				comboBox1.SelectedValue = dv[0][0].ToString();
		}
		#endregion

        #region Combo usp_CO_COMM_CODE의 P13에 DEF_FLAG 필드까지 가지오는 경우 콤보의 기본값 자동선택
        public static void Combo(System.Windows.Forms.ComboBox comboBox1, int NullValue, string Query)
		{	
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			DataView dv			= new DataView(dt);
			//dv.RowFilter		= "DEF_FLAG = 'Y'";

			if(NullValue == 0)
			{
				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 1)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "*";
				dr[1] = "전체";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 2)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "*";
				dr[1] = "*";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 3)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "전체";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else
			{
				DataRow dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}

			if(dv.Count > 0)
				comboBox1.SelectedValue = dv[0][0].ToString();
		}
		#endregion

		#region Combo
		public static void Combo(System.Windows.Forms.ComboBox comboBox1, string Query, int NullValue)
		{	
			comboBox1.Refresh();
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(NullValue == 0)
			{
				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 1)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "*";
				dr[1] = "전체";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 2)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "*";
				dr[1] = "*";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else if(NullValue == 3)
			{
				DataRow dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "전체";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
			else
			{
				DataRow dr = dt.NewRow();
				dr[0] = "";
				dr[1] = "";
				dt.Rows.InsertAt(dr, 0);

				comboBox1.ValueMember	= dt.Columns[0].ColumnName.ToString();
				comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
				comboBox1.DataSource	= dt;
			}
		}

		#endregion

		#region DropDownString - 그리드 상단 콤보박스에 데이타 넣기
		public static string ComboOnGrid(string Query, string Where)
		{	//                           쿼리,	       Return 위치
			string Rtn;
			string RtnTmp1 = "";
			string RtnTmp2 = "";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			for(int i = 0; i < dt.Rows.Count; i++)
			{
				RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
				RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
			}
			return Rtn = Where + ":" + RtnTmp1 + "|" + RtnTmp2;
		}
		#endregion

		#region DropDownString - 그리드 상단 콤보박스에 데이타 넣기
		public static string ComboOnGrid(string Query, string Where, int NullValue)
		{	//                           쿼리,	       Return 위치
			string Rtn = null;
			string RtnTmp1 = null;
			string RtnTmp2 = null;
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			for(int i = 0; i < dt.Rows.Count; i++)
			{
				RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
				RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
			}

			if(RtnTmp1 != null)
			{
				RtnTmp1 = RtnTmp1.Substring(1,RtnTmp1.Length-1);
				RtnTmp2 = RtnTmp2.Substring(1,RtnTmp2.Length-1);
			}
			else
			{
				RtnTmp1 = "";
				RtnTmp2 = "";
			}

			if(NullValue == 0)
			{
				Rtn = RtnTmp1 + "|" + RtnTmp2;
			}
			else if(NullValue == 1)
			{
				Rtn = "#"+RtnTmp1 + "|" +"#"+ RtnTmp2;
			}
			else if(NullValue == 2)
			{
				Rtn = "*#"+RtnTmp1 + "|" +"*#"+ RtnTmp2;
			}

			return Rtn = Where + ":" + RtnTmp1 + "|" + RtnTmp2;
		}
		#endregion

		#region DropDownString - 그리드 상단 콤보박스에 데이타 넣기 2007-05-04 신규
		public static string ComboOnGrid(string Query)
		{	//                           쿼리,	       Return 위치
			string Rtn;
			string RtnTmp1 = "";
			string RtnTmp2 = "";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			for(int i = 0; i < dt.Rows.Count; i++)
			{
				RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
				RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
			}
			return Rtn = RtnTmp1 + "|" + RtnTmp2;
		}
		#endregion

		#region DropDownString - 그리드 상단 콤보박스에 데이타 넣기 2007-05-04 신규
		public static string ComboOnGrid(string Query, int NullValue)
		{	//                           쿼리,	       Return 위치
			string Rtn = null;
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			string RtnTmp1 = null;
			string RtnTmp2 = null;

			for(int i = 0; i < dt.Rows.Count; i++)
			{
				RtnTmp1 = RtnTmp1 + "#" + dt.Rows[i][0].ToString();
				RtnTmp2 = RtnTmp2 + "#" + dt.Rows[i][1].ToString();
			}
			if(RtnTmp1 != null)
			{
				RtnTmp1 = RtnTmp1.Substring(1,RtnTmp1.Length-1);
				RtnTmp2 = RtnTmp2.Substring(1,RtnTmp2.Length-1);
			}
			else
			{
				RtnTmp1 = "";
				RtnTmp2 = "";
			}

			if(NullValue == 0)
			{
				Rtn = RtnTmp1 + "|" + RtnTmp2;
			}
			else if(NullValue == 1)
			{
				Rtn = "#"+RtnTmp1 + "|" +"#"+ RtnTmp2;
			}
			else if(NullValue == 2)
			{
				Rtn = "*#"+RtnTmp1 + "|" +"*#"+ RtnTmp2;
			}

			return Rtn;
		}
		#endregion

        #region *****************   C1.Win.C1List.C1Combo   2012-10-23 추가 **************************************
        public static void C1Combo(C1.Win.C1List.C1Combo comboBox1, string Query, bool Default_Flag)
        {
            comboBox1.Refresh();
            comboBox1.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            DataView dv = new DataView(dt);
            dv.RowFilter		= "DEF_FLAG = 'Y'";

            comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
            comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
            comboBox1.DataSource = dt;

            comboBox1.AllowColMove = false;
            comboBox1.Splits[0].DisplayColumns[0].Width = 0;
            comboBox1.Splits[0].DisplayColumns[1].Width = comboBox1.Size.Width;

            if (dt.Columns.Count > 2)
            {
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    comboBox1.Splits[0].DisplayColumns[i].Width = 0;
                }
            }

            comboBox1.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
            comboBox1.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
            comboBox1.ColumnHeaders = false;

            if (dv.Count > 0)
            {
                comboBox1.SelectedValue = dv[0][0].ToString();
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        public static void C1Combo(C1.Win.C1List.C1Combo comboBox1, string Query, int NullValue, bool Default_Flag)
        {
            comboBox1.Refresh();
            comboBox1.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            DataView dv = new DataView(dt);
            dv.RowFilter = "DEF_FLAG = 'Y'";

            if (NullValue == 0)
            {
                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else if (NullValue == 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "*";
                dr[1] = "전체";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else if (NullValue == 2)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "*";
                dr[1] = "*";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else if (NullValue == 3)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "전체";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }

            comboBox1.AllowColMove = false;
            comboBox1.Splits[0].DisplayColumns[0].Width = 0;
            comboBox1.Splits[0].DisplayColumns[1].Width = comboBox1.Size.Width;
            if (dt.Columns.Count > 2)
            {
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    comboBox1.Splits[0].DisplayColumns[i].Width = 0;
                }
            }
            comboBox1.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
            comboBox1.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
            comboBox1.ColumnHeaders = false;

            if (dv.Count > 0)
            {
                comboBox1.SelectedValue = dv[0][0].ToString();
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        public static void C1Combo(C1.Win.C1List.C1Combo comboBox1, string Query)
        {
            comboBox1.Refresh();
            comboBox1.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
            DataView dv = new DataView(dt);            
 
            comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
            comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
            comboBox1.AllowColMove = false;
            comboBox1.DataSource = dt;
            
            comboBox1.AllowColMove = false;
            comboBox1.Splits[0].DisplayColumns[0].Width = 0;
            comboBox1.Splits[0].DisplayColumns[1].Width = comboBox1.Size.Width;
            if (dt.Columns.Count > 2)
            {
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    comboBox1.Splits[0].DisplayColumns[i].Width = 0;
                }
            }
            comboBox1.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
            comboBox1.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
            comboBox1.ColumnHeaders = false;
            comboBox1.SelectedIndex = 0;
            
        }

        public static void C1Combo(C1.Win.C1List.C1Combo comboBox1, string Query, int NullValue)
        {
            comboBox1.Refresh();
            comboBox1.ComboStyle = C1.Win.C1List.ComboStyleEnum.DropdownList;

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (NullValue == 0)
            {
                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else if (NullValue == 1)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "*";
                dr[1] = "전체";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;


            }
            else if (NullValue == 2)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "*";
                dr[1] = "*";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else if (NullValue == 3)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "전체";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "";
                dt.Rows.InsertAt(dr, 0);

                comboBox1.ValueMember = dt.Columns[0].ColumnName.ToString();
                comboBox1.DisplayMember = dt.Columns[1].ColumnName.ToString();
                comboBox1.DataSource = dt;
            }

            comboBox1.AllowColMove = false;
            comboBox1.Splits[0].DisplayColumns[0].Width = 0;
            comboBox1.Splits[0].DisplayColumns[1].Width = comboBox1.Size.Width;
            if (dt.Columns.Count > 2)
            {
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    comboBox1.Splits[0].DisplayColumns[i].Width = 0;
                }
            }
            comboBox1.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
            comboBox1.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
            comboBox1.ColumnHeaders = false;
            comboBox1.SelectedIndex = 0;

        }

        #endregion
    }
}
