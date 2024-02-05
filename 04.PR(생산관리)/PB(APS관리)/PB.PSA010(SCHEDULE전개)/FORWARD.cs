using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace PB.PSA010
{
	/// <summary>
	/// FORWARD_U에 대한 요약 설명입니다.
	/// </summary>
	public class FORWARD
	{
		#region FORWARD_LIMIT() FORWARD_LIMIT2 유한 정전개
		public static void FORWARD_LIMIT(
			FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			, DataTable dtMPS
			, string strSCHST_DT
			, string strSCHST_TM
			, int j
			, string PROJECT_NO
			, string PROJECT_SEQ
			)
		{
			
			// MPS 정보로 스케쥴 전개
			progressBar2.Maximum = 100;
			label1.Text = "정전개 작업 시작일자 시간, 종료일자 시간을 계산중입니다.";

			// 진행률 표시
			Thread schProgTh;
			SCH_PROG schProg = new SCH_PROG();
			schProg.SchProg = progressBar2;

			schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));
			schProgTh.Start();

			for(int i=0; i < dtMPS.Rows.Count; i++)
			{
				
				progressBar2.Value = 0;
				label2.Text = "제조번호 : " + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + " " +
							  "스케쥴ID : " + fpSpread1.Sheets[0].Cells[j, 0].Text + "의 작업시간 전개중";

				string Query = "";

				try
				{

					Query  = " usp_P_FORWARD  @pPROC_ID = '" + PSA010.PROC_ID + "', ";
					Query += "                @pSCH_ID =  '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
					Query += "                @pMAKEORDER_NO  = '" + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + "', ";
					Query += "                @pST_DT  = '" + strSCHST_DT + "', ";
					Query += "                @pST_TM  = '" + strSCHST_TM + "', ";
                    Query += "                @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

					DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

					string ERRCode = ds.Tables[0].Rows[0][0].ToString();
					string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

					if(ERRCode == "ER")  
					{
						// 다음 스케쥴을 계속 실행한다.
						SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
					}

				}
				catch(Exception f)
				{
					SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query + " \n\n " + f.ToString());
				}
				progressBar2.Value = 100;

			}

			// PROGRESS 표시
			schProg.SchProcStop();
			schProgTh.Join();


		}
		#endregion

	}
}
