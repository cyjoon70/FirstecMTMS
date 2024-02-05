using System;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win; 
using FarPoint.Win.Spread;

namespace PB.PSA010
{
	public class SCHEDULE
	{
		public static string SCH_NO = "";
		public static string PROC_STATUS_BOM_DEPLY     = "BD";
		public static string PROC_STATUS_CAL_RESO_TIME = "CT"; // 작업시간 계산
		
		#region BOP_DEPLOY() BOM 전개
		public static string BOM_DEPLOY(FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string strSCHST_DT
			, string strSCHST_TM
			, string strPLANT_CD
			, string strPTF
			, string strPROJECT_NO
			, string strPROJECT_SEQ
			)
		{

			string RtnMsg = SystemBase.Base.MessageRtn("P0010");

			try
			{

				/////////////////////// 진행중인 스케쥴 정보 검사 ///////////////////
				string SchDplNm = SCH_PROG.HasDplySch();

				if(SchDplNm != null)
					return SystemBase.Base.MessageRtn("P0038", SchDplNm);
				/////////////////////// 진행중인 스케쥴 정보 검사 ///////////////////
				
				string StartTM = SystemBase.Base.ServerTime("").ToString();

				label1.Text = "MPS 정보를 로드중입니다.";

				string mpsQuery = QUERY.MPS(strPLANT_CD.ToString(), strSCHST_DT.ToString(), strPTF.Substring(0,10), strPROJECT_NO, strPROJECT_SEQ);
				DataTable dtMPS = SystemBase.DbOpen.NoTranDataTable(mpsQuery);			// MPS 정보 저장

				if(dtMPS.Rows.Count == 0)
				{
					RtnMsg = SystemBase.Base.MessageRtn("P0007");
					goto Exit;
				}

				label1.Text = "BOP 정보를 저장중입니다.";
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 10000;

				try
				{
					/////////////////////// 진행중인 스케쥴 정보 등록 ///////////////////
					SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_BOM_DEPLY, cmd);
					/////////////////////// 진행중인 스케쥴 정보 등록 ///////////////////
					
					/////////////////////////MBOP 생성///////////////////////////////////
					MBOPCOPY.MBOP_COPY(fpSpread1, progressBar1, progressBar2, label1, label2, strPTF, strPLANT_CD, dtMPS, cmd, dbConn, Trans, strPROJECT_NO, strPROJECT_SEQ);
					/////////////////////////MBOP 생성///////////////////////////////////
					
					///////////////////////// Resource 작업시간 계산 ///////////////////////////////////
					RESOURCE_WORK_TIME.RESOURCEWORKTIME(progressBar2, label2, cmd, dtMPS, fpSpread1);
					///////////////////////// Resource 작업시간 계산 ///////////////////////////////////

					////////////////////////// 자원주간/OT CALENDAR 생성 /////////////////////////
					//RESOURCE_WORK_TIME.RESOURCEWEEKCAL(label2, cmd, dbConn, Trans, fpSpread1);
					////////////////////////// 자원주간/OT CALENDAR 생성 /////////////////////////

					////////////////////////// 고정 스케쥴 시간 처리 /////////////////////////////
					//RESOURCE_WORK_TIME.INIT_RESOURCE_WORK_TIME(label2, cmd,	dbConn, Trans, fpSpread1);			
					////////////////////////// 고정 스케쥴 시간 처리 /////////////////////////////

					/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
					SCH_PROG.DelSchProc(cmd);
					/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
					
					/////////////////////////MBOP 생성///////////////////////////////////
					SCH_PROG.DelOrderProc(cmd);
					/////////////////////////MBOP 생성///////////////////////////////////

					progressBar1.Value = progressBar1.Maximum;
					progressBar2.Value = progressBar2.Maximum;

					label1.Text = "데이타 저장중입니다.";
					label2.Text = "데이타 저장중입니다.";

					Trans.Commit();
				}
				catch(Exception f)
				{
					Trans.Rollback();					
					
					SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
					RtnMsg = SystemBase.Base.MessageRtn("P0001");
				}
				finally
				{
					dbConn.Close();
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
				RtnMsg = SystemBase.Base.MessageRtn("P0001");
			}

			Exit:
				label1.Text = "완료되었습니다..";
				label2.Text = "완료되었습니다..";

			return RtnMsg;
		}
		#endregion

		#region SCH_DEPLOY() 스케쥴 전개
		public static string SCH_DEPLOY(FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string strSCHST_DT
			, string strSCHST_TM
			, string strPLANT_CD
			, string strPTF
			, string strPROJECT_NO
			, string strPROJECT_SEQ
			, string strSCH_MEMO
			, string strMEMO
			)
		{
			string RtnMsg = SystemBase.Base.MessageRtn("P0010");

			try
			{
				/////////////////////// 진행중인 스케쥴 정보 검사 ///////////////////
				string SchDplNm = SCH_PROG.HasDplySch();

				if(SchDplNm != null)
					return SystemBase.Base.MessageRtn("P0038", SchDplNm);
				/////////////////////// 진행중인 스케쥴 정보 검사 ///////////////////

				string StartTM = SystemBase.Base.ServerTime("").ToString();

				label1.Text = "MPS 정보를 로드중입니다.";

				string mpsQuery = QUERY.MPS(strPLANT_CD.ToString(), strSCHST_DT.ToString(), strPTF.Substring(0,10), strPROJECT_NO, strPROJECT_SEQ);
				DataTable dtMPS = SystemBase.DbOpen.NoTranDataTable(mpsQuery);			// MPS 정보 저장

				if(dtMPS.Rows.Count == 0)
				{
					RtnMsg = SystemBase.Base.MessageRtn("P0007");
					goto Exit;
				}

				label1.Text = "스케쥴 전개 준비중입니다.";
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlCommand cmdLog = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 10000;

				try
				{
					/////////////////////// SCH_NO 추출 및 SCH LOG MASTER 저장 ///////////////////
					SCH_NO = SCH_PROG.SchAutoNo(strSCHST_DT, strSCHST_TM, strPTF, strSCH_MEMO, strMEMO);
					/////////////////////// 진행중인 스케쥴 정보 등록 ///////////////////
					SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_BOM_DEPLY, cmd);

					/////////////////////// RESO_WORK_TIME_TEMP 삭제(배정정보삭제)///////
					SCH_PROG.DelResoWorkTime(cmd);

					/////////////////////////작업 시작일자 시간 종료일자 시간 계산(정전개,역전개,무한,유한)///////////////////////////
					RESOURCE_WORK_STED.RESOURCE_WORK_MASTER(fpSpread1, progressBar1, progressBar2, label1, label2, cmd, dbConn, Trans, dtMPS, strSCHST_DT, strSCHST_TM, strPROJECT_NO, strPROJECT_SEQ, SCH_NO);

					/////////////////////////스케쥴 완료 후 생성정보 저장(P_CAL_SCH_MST)///////////////////////////
					SCH_MST_UP.SCH_MASTER_UP(fpSpread1, cmd, strSCHST_DT, strSCHST_TM, strPLANT_CD, StartTM, label1, label2, SCH_NO);

					/////////////////////////MPS 확정에서 전개로 FLG 변경///////////////////////////
					MPSFG_UP.SCH_MPSFG_UP(cmd, label1, label2, strPTF, SCH_NO);

					/////////////////////////MPS 확정에서 전개로 FLG 변경///////////////////////////
					MPSFG_UP.SCH_TEMP_DEL(cmd, label1, label2, SCH_NO);

					/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
					SCH_PROG.DelSchProc(cmd);

					progressBar1.Value = progressBar1.Maximum;
					progressBar2.Value = progressBar2.Maximum;

					label1.Text = "데이타 저장중입니다.";
					label2.Text = "데이타 저장중입니다.";

					Trans.Commit();

					/////////////////////// LOG BACKUP 및 축소 ///////////////////
					SCH_PROG.DelLogBack();
					SCH_PROG.DelLogSmall();
					/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
					///
					dbConn.Close();
				}
				catch(Exception f)
				{
					Trans.Rollback();

					/////////////////////// LOG BACKUP 및 축소 ///////////////////
					SCH_PROG.DelLogBack();
					SCH_PROG.DelLogSmall();
					/////////////////////// 진행중인 스케쥴 정보 삭제 ///////////////////
					///
					dbConn.Close();

					SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
                    RtnMsg = SystemBase.Base.MessageRtn(f.ToString());
				}
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
                RtnMsg = SystemBase.Base.MessageRtn(f.ToString());
			}
			Exit:
				label1.Text = "완료되었습니다..";
				label2.Text = "완료되었습니다..";

			string msgQuery = QUERY.MSG();
			DataTable dtMsg = SystemBase.DbOpen.NoTranDataTable(msgQuery);			// MPS 정보 저장

			if(dtMsg.Rows.Count == 0)
			{
				RtnMsg = "정상적으로 처리되었습니다.";
			}
			else
			{
				RtnMsg = "날짜가 입력되지 않은 제품이 존재합니다.";
			}

			return RtnMsg;
		}
		#endregion
	}
}
