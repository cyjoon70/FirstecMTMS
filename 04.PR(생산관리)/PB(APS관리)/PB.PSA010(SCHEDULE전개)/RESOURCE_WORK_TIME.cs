using System;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{
	/// <summary>
	/// RESOURCE_WORK_TIME에 대한 요약 설명입니다.
	/// </summary>
	public class RESOURCE_WORK_TIME
	{

		#region RESOURCEWORKTIME() P_BOP_M_PROC_DETAIL_TEMP 테이블에 실 작업시간(작업지시시간) 계산 후 업데이트
		public static void RESOURCEWORKTIME(
			System.Windows.Forms.ProgressBar progressBar2,
			System.Windows.Forms.Label label2, 
			SqlCommand cmd,
			DataTable dtMPS,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	//	CMLT(누적 L/T)도 동시에 업데이트
			label2.Text = "작업시간 계산중입니다.";

			// 프로그래스 초기화
			progressBar2.Maximum = dtMPS.Rows.Count;
			progressBar2.Value   = 0;

			for(int i=0; i < dtMPS.Rows.Count; i++)
			{
				for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
				{	//스케쥴 id별 BOP MBOP로 복사
					if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
					{
						label2.Text = "제품오더번호 " + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + 
							" 스케줄 ID " + fpSpread1.Sheets[0].Cells[j, 0].Text + "의 작업시간 계산중입니다.";

						string RESTimeUP = QUERY.WORK_TM_UP(
							dtMPS.Rows[i]["MAKEORDER_NO"].ToString(),
							fpSpread1.Sheets[0].Cells[j, 0].Text,
							fpSpread1.Sheets[0].Cells[j,3].Text == "역전개"?"BWD":"FWD"
							);
						cmd.CommandText = RESTimeUP;
						cmd.ExecuteNonQuery();
					}
				}
				progressBar2.Value = i + 1;
			}
		}
		#endregion
		
		#region 기존배정된 고정 스케쥴 처리
		public static void INIT_RESOURCE_WORK_TIME(
			System.Windows.Forms.Label label2, 
			SqlCommand     cmd,
			SqlConnection  dbConn,
			SqlTransaction Trans,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	
			//	CMLT(누적 L/T)도 동시에 업데이트
			label2.Text = "고정스케쥴 처리중입니다.";

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{	//스케쥴 id별 자원 CALENDAR 생성
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{

					string Query = "";

					try
					{
						Query += " usp_P_INIT_RESO_WORK_TIME '" + SystemBase.Base.gstrCOMCD.ToString() + "',";
                        Query += "                           '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
						Query += "                           '" + fpSpread1.Sheets[0].Cells[j, 4].Text + "' ";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						string ERRCode = ds.Tables[0].Rows[0][0].ToString();
						string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// 다음 스케쥴을 계속 실행한다.
							SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
							throw new Exception(MSGCode);
						}

					}
					catch(Exception f)
					{
						SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, f.ToString());
						throw f;
					}
				}
			}
		}
		#endregion

		#region 자원의 주간/OT CALENDAR 생성
		public static void RESOURCEWEEKCAL(
			System.Windows.Forms.Label label2, 
			SqlCommand     cmd,
			SqlConnection  dbConn,
			SqlTransaction Trans,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	
			label2.Text = "자원 주간/OT CALENDAR 생성중입니다.";

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{	//스케쥴 id별 자원 CALENDAR 생성
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{

					string Query = "";

					try
					{

						Query  = " DECLARE @O_RTN_CHK AS VARCHAR(2), "; 
						Query += "         @O_MSG_CD  AS VARCHAR(50)  ";

                        Query += " EXEC usp_P_CRT_RESO_CAL '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        Query += "                         '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
						Query += "                         'N',"; // (N : NEW ) 새로 생성한다.
						Query += "                         'N',"; // (N : NORMAL ) 기본 스케쥴 생성
						Query += "                         '" + SystemBase.Base.gstrUserID + "', ";
						Query += "                         @O_RTN_CHK   OUTPUT, ";
						Query += "                         @O_MSG_CD    OUTPUT  ";
						Query += " SELECT @O_RTN_CHK, @O_MSG_CD ";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						string ERRCode = ds.Tables[0].Rows[0][0].ToString();
						string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// 다음 스케쥴을 계속 실행한다.
							SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
							throw new Exception(MSGCode);
						}

					}
					catch(Exception f)
					{
						SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, f.ToString());
						throw f;
					}
				}
			}
		}
		#endregion

	}
}