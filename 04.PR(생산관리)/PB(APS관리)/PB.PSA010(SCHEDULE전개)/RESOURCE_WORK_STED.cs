using System;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win; 
using FarPoint.Win.Spread;

namespace PB.PSA010
{
	/// <summary>
	/// DEPLOY_TM에 대한 요약 설명입니다.
	/// </summary>
	public class RESOURCE_WORK_STED
	{

		#region RESOURCE_WORK_STED()  전개방법
		public static void RESOURCE_WORK_MASTER(
			  FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			, DataTable dtMPS
			, string strSCHST_DT	// 기준일자
			, string strSCHST_TM	// 기준시간
			, string PROJECT_NO
			, string PROJECT_SEQ
			, string SCH_NO
			)
		{

			int SchRowCount=0;

			for(int h=0; h < fpSpread1.Sheets[0].Rows.Count; h++)
			{
				if(fpSpread1.Sheets[0].Cells[h,2].Text == "True" && fpSpread1.Sheets[0].Cells[h,3].Text.Length > 0)
					SchRowCount++;
			}

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{
					if(fpSpread1.Sheets[0].Cells[j,3].Text == "역전개")
					{
						//########################### 유한/무한 역전개 ###########################//
						BACKWARD.BACKWARD_LIMIT(
							fpSpread1, 
							progressBar1, 
							progressBar2,
							label1, 
							label2, 
							cmd, 
							dbConn, 
							Trans, 
							dtMPS, 
							strSCHST_DT, 
							strSCHST_TM, 
							j, 
							PROJECT_NO,
							PROJECT_SEQ,
							SCH_NO);
						//########################### 유한/무한 역전개 ###########################//
					}
					else if(fpSpread1.Sheets[0].Cells[j,3].Text == "정전개")
					{
						//########################### 유한 정전개 ###########################//
						FORWARD.FORWARD_LIMIT(
							fpSpread1, 
							progressBar1, 
							progressBar2,
							label1, 
							label2, 
							cmd, 
							dbConn, 
							Trans, 
							dtMPS, 
							strSCHST_DT, 
							strSCHST_TM, 
							j, 
							PROJECT_NO,
							PROJECT_SEQ);
						//########################### 유한 정전개 ###########################//
					}
				}
			}
		}
		#endregion

	}
}
