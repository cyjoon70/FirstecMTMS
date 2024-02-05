using System;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{ 
	/// <summary>
	/// SCH_MST_UP에 대한 요약 설명입니다.
	/// </summary>
	public class SCH_MST_UP
	{
		#region 스케쥴 생성정보 저장
		public static void SCH_MASTER_UP(
			FarPoint.Win.Spread.FpSpread fpSpread1
			, SqlCommand cmd
			, string strSCHST_DT
			, string strSCHST_TM
			, string strPLANT_CD
			, string StartTM
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string SCH_NO
			)
		{
			label1.Text = "SCHEDULE 결과를 저장합니다.";
			label2.Text = "SCHEDULE 결과를 저장합니다.";
			for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{
				if(fpSpread1.Sheets[0].Cells[i, 2].Text == "True")
				{

					string SCH_MST_UP = QUERY.SCH_MST_UP(
						fpSpread1.Sheets[0].Cells[i, 0].Text,
						fpSpread1.Sheets[0].Cells[i, 2].Text,
						fpSpread1.Sheets[0].Cells[i, 3].Text,
						fpSpread1.Sheets[0].Cells[i, 4].Text,
						fpSpread1.Sheets[0].Cells[i, 5].Text,
						fpSpread1.Sheets[0].Cells[i, 6].Text,
						fpSpread1.Sheets[0].Cells[i, 7].Text,
						strSCHST_DT,
						strSCHST_TM,
						StartTM,
						SystemBase.Base.ServerTime("").ToString(),
						SystemBase.Base.gstrUserID.ToString(),
						SystemBase.Base.ServerTime("").ToString(),
						strPLANT_CD
						);
					cmd.CommandText = SCH_MST_UP;
					cmd.ExecuteNonQuery();
				}
				else
				{
					string SCH_MST_UP = " UPDATE P_CAL_SCH_MST SET ";
					SCH_MST_UP = SCH_MST_UP + " ACTIVE = '"			+ fpSpread1.Sheets[0].Cells[i, 2].Text +"'";
					SCH_MST_UP = SCH_MST_UP + " WHERE SCH_ID = '"	+ fpSpread1.Sheets[0].Cells[i, 0].Text +"'";
                    SCH_MST_UP = SCH_MST_UP + "   AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
				
					cmd.CommandText = SCH_MST_UP;
					cmd.ExecuteNonQuery();
				}
			}

			//SCH CAL LOG 저장
			string SCH_CAL_LOG = "";
			SCH_CAL_LOG =  "INSERT INTO P_SCH_CAL_LOG ";
			SCH_CAL_LOG += " SELECT '" + SCH_NO + "', * ";
			SCH_CAL_LOG += " FROM P_CAL_SCH_MST";
            SCH_CAL_LOG += " WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
				
			cmd.CommandText = SCH_CAL_LOG;
			cmd.ExecuteNonQuery();

			label1.Text = "SCHEDULE 결과를 저장하였습니다.";
			label2.Text = "SCHEDULE 결과를 저장하였습니다.";

		}
		#endregion
	}
}
