using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{
	/// <summary>
	/// SCHCONF에 대한 요약 설명입니다.
	/// </summary>
	public class SCH_CONF
	{
		public static string PROC_STATUS_CONF_SCH = "CS";

		public SCH_CONF()
		{
			//
			// TODO: 여기에 생성자 논리를 추가합니다.
			//
		}

		public string CONF_SCH(System.Windows.Forms.ProgressBar progressBar1,
							   string SCH_ID, string REASON_CD, string MEMO)
		{

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 10000;

			try 
			{

				/////////////////////// 스케쥴 프로세스 등록 //////////////////////////
				SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_CONF_SCH, cmd);
				/////////////////////// 스케쥴 프로세스 등록 /////////////////////////

				string ERRCode = "";
				string MSGCode = "";

				Thread schProgTh; // 진행바 관리
				SCH_PROG schProg = new SCH_PROG();

				// 진행율 표시
				progressBar1.Value = 0;
				progressBar1.Maximum = 100;
				schProg.SchProg = progressBar1;

				// PROGRESS ID
				schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));

				try
				{
					progressBar1.Value = 0;
					schProgTh.Start();

					string strQuery = "usp_P_SCHEDULE @pTYPE='P020' ";
					strQuery += " , @pPROC_ID = '"  + PSA010.PROC_ID + "'";
					strQuery += " , @pSCH_ID  = '"	+ SCH_ID +"' ";
					strQuery += " , @pREASON_CD = '"+ REASON_CD +"' ";
					strQuery += " , @pMEMO = '"		+ MEMO+"' ";
					strQuery += " , @pUSR_ID = '"	+ SystemBase.Base.gstrUserID +"' ";
                    strQuery += " , @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode	= ds.Tables[0].Rows[0][1].ToString();

					if(ERRCode == "ER")
					{
						Trans.Rollback();
						return MSGCode;
						//throw new Exception(MSGCode);
					}	// ER 코드 Return시 점프
					else
					{
						/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
						SCH_PROG.DelSchProc(cmd);
						/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////

						Trans.Commit();

						schProg.SchProcStop(); // PROGRESS 종료
						return MSGCode;
					}
				}
				catch(Exception f)	
				{
					Trans.Rollback();
					throw f;
				}
				finally 
				{
					if(schProgTh != null) 
					{
						// PROGRESS가 여전히 진행중일 경우
						if(schProg.isInProc()) 
							schProg.SchProcStop(); // PROGRESS 종료

						// 스레드 자원 소멸
						schProgTh.Join();
					}
				}
			}
			catch(Exception f) 
			{
				Trans.Rollback();
				throw f;
			}
			finally 
			{
				dbConn.Close();
			}
		}
	}
}
