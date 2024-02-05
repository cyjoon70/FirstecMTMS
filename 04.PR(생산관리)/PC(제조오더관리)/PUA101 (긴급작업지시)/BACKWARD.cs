using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

namespace PUA101
{

	/// <summary>
	/// BACKWARD_U에 대한 요약 설명입니다.
	/// </summary>
	public class BACKWARD
	{

		#region BACKWARD_EMEG() 무한역전개
		public static void BACKWARD_EMEG(
			System.Windows.Forms.ProgressBar progressBar1,
			System.Windows.Forms.Label label1,
			string SCH_ID,
			SqlCommand cmd,
			SqlConnection dbConn,
			SqlTransaction Trans
			)
		{

			label1.Text = "긴급작지 작업 시작일자 시간, 종료일자 시간을 계산중입니다.";

			string Query = QUERY.SRCH_EMG_INFO();

			// 전개 대상 조회
			DataTable dtEMG = SystemBase.DbOpen.NoTranDataTable(Query);

			// MPS 정보로 스케쥴 전개
			progressBar1.Maximum = 100;

			// PROGRESS 표시
			Thread schProgTh;
			SCH_PROG schProg = new SCH_PROG();
			schProg.SchProg = progressBar1;

			// PROGRESS ID
			schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));
			schProgTh.Start();

			for(int i=0; i < dtEMG.Rows.Count; i++)
			{

				progressBar1.Value = 0;

				try
				{
					Query  = " usp_P_EMERG_WORKORDER @pPROC_ID = '" + PUA101.PROC_ID + "', ";
					Query += " @pSCH_ID = '" + SCH_ID + "', ";
					Query += " @pMAKEORDER_NO  = '" + dtEMG.Rows[i]["MAKEORDER_NO"].ToString() + "', ";
					Query += " @pMAKEFINISH_DT = '" + dtEMG.Rows[i]["DELIVERY_DT"].ToString().Substring(0, 10) + "' ";

					DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

					string ERRCode = ds.Tables[0].Rows[0][0].ToString();
					string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

					if(ERRCode == "ER")  {
						// 다음 스케쥴을 계속 실행한다.
						SystemBase.Loggers.Log("긴급작지", Query);
						throw new Exception("제품오더번호: " + dtEMG.Rows[i]["MAKEORDER_NO"].ToString() + " \n\n " + MSGCode);
					}

				}
				catch(Exception f)
				{
					SystemBase.Loggers.Log("긴급작지", Query + " \n\n " + f.ToString());
					throw f;
				}
				progressBar1.Value = 100;

			}
			// PROGRESS 표시
			schProg.SchProcStop();
			schProgTh.Join();

		}
		#endregion
	}
}