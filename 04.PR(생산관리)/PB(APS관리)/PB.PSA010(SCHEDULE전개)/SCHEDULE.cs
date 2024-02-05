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
		public static string PROC_STATUS_CAL_RESO_TIME = "CT"; // �۾��ð� ���
		
		#region BOP_DEPLOY() BOM ����
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

				/////////////////////// �������� ������ ���� �˻� ///////////////////
				string SchDplNm = SCH_PROG.HasDplySch();

				if(SchDplNm != null)
					return SystemBase.Base.MessageRtn("P0038", SchDplNm);
				/////////////////////// �������� ������ ���� �˻� ///////////////////
				
				string StartTM = SystemBase.Base.ServerTime("").ToString();

				label1.Text = "MPS ������ �ε����Դϴ�.";

				string mpsQuery = QUERY.MPS(strPLANT_CD.ToString(), strSCHST_DT.ToString(), strPTF.Substring(0,10), strPROJECT_NO, strPROJECT_SEQ);
				DataTable dtMPS = SystemBase.DbOpen.NoTranDataTable(mpsQuery);			// MPS ���� ����

				if(dtMPS.Rows.Count == 0)
				{
					RtnMsg = SystemBase.Base.MessageRtn("P0007");
					goto Exit;
				}

				label1.Text = "BOP ������ �������Դϴ�.";
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 10000;

				try
				{
					/////////////////////// �������� ������ ���� ��� ///////////////////
					SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_BOM_DEPLY, cmd);
					/////////////////////// �������� ������ ���� ��� ///////////////////
					
					/////////////////////////MBOP ����///////////////////////////////////
					MBOPCOPY.MBOP_COPY(fpSpread1, progressBar1, progressBar2, label1, label2, strPTF, strPLANT_CD, dtMPS, cmd, dbConn, Trans, strPROJECT_NO, strPROJECT_SEQ);
					/////////////////////////MBOP ����///////////////////////////////////
					
					///////////////////////// Resource �۾��ð� ��� ///////////////////////////////////
					RESOURCE_WORK_TIME.RESOURCEWORKTIME(progressBar2, label2, cmd, dtMPS, fpSpread1);
					///////////////////////// Resource �۾��ð� ��� ///////////////////////////////////

					////////////////////////// �ڿ��ְ�/OT CALENDAR ���� /////////////////////////
					//RESOURCE_WORK_TIME.RESOURCEWEEKCAL(label2, cmd, dbConn, Trans, fpSpread1);
					////////////////////////// �ڿ��ְ�/OT CALENDAR ���� /////////////////////////

					////////////////////////// ���� ������ �ð� ó�� /////////////////////////////
					//RESOURCE_WORK_TIME.INIT_RESOURCE_WORK_TIME(label2, cmd,	dbConn, Trans, fpSpread1);			
					////////////////////////// ���� ������ �ð� ó�� /////////////////////////////

					/////////////////////// �������� ������ ���� ���� ///////////////////
					SCH_PROG.DelSchProc(cmd);
					/////////////////////// �������� ������ ���� ���� ///////////////////
					
					/////////////////////////MBOP ����///////////////////////////////////
					SCH_PROG.DelOrderProc(cmd);
					/////////////////////////MBOP ����///////////////////////////////////

					progressBar1.Value = progressBar1.Maximum;
					progressBar2.Value = progressBar2.Maximum;

					label1.Text = "����Ÿ �������Դϴ�.";
					label2.Text = "����Ÿ �������Դϴ�.";

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
				label1.Text = "�Ϸ�Ǿ����ϴ�..";
				label2.Text = "�Ϸ�Ǿ����ϴ�..";

			return RtnMsg;
		}
		#endregion

		#region SCH_DEPLOY() ������ ����
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
				/////////////////////// �������� ������ ���� �˻� ///////////////////
				string SchDplNm = SCH_PROG.HasDplySch();

				if(SchDplNm != null)
					return SystemBase.Base.MessageRtn("P0038", SchDplNm);
				/////////////////////// �������� ������ ���� �˻� ///////////////////

				string StartTM = SystemBase.Base.ServerTime("").ToString();

				label1.Text = "MPS ������ �ε����Դϴ�.";

				string mpsQuery = QUERY.MPS(strPLANT_CD.ToString(), strSCHST_DT.ToString(), strPTF.Substring(0,10), strPROJECT_NO, strPROJECT_SEQ);
				DataTable dtMPS = SystemBase.DbOpen.NoTranDataTable(mpsQuery);			// MPS ���� ����

				if(dtMPS.Rows.Count == 0)
				{
					RtnMsg = SystemBase.Base.MessageRtn("P0007");
					goto Exit;
				}

				label1.Text = "������ ���� �غ����Դϴ�.";
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlCommand cmdLog = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 10000;

				try
				{
					/////////////////////// SCH_NO ���� �� SCH LOG MASTER ���� ///////////////////
					SCH_NO = SCH_PROG.SchAutoNo(strSCHST_DT, strSCHST_TM, strPTF, strSCH_MEMO, strMEMO);
					/////////////////////// �������� ������ ���� ��� ///////////////////
					SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_BOM_DEPLY, cmd);

					/////////////////////// RESO_WORK_TIME_TEMP ����(������������)///////
					SCH_PROG.DelResoWorkTime(cmd);

					/////////////////////////�۾� �������� �ð� �������� �ð� ���(������,������,����,����)///////////////////////////
					RESOURCE_WORK_STED.RESOURCE_WORK_MASTER(fpSpread1, progressBar1, progressBar2, label1, label2, cmd, dbConn, Trans, dtMPS, strSCHST_DT, strSCHST_TM, strPROJECT_NO, strPROJECT_SEQ, SCH_NO);

					/////////////////////////������ �Ϸ� �� �������� ����(P_CAL_SCH_MST)///////////////////////////
					SCH_MST_UP.SCH_MASTER_UP(fpSpread1, cmd, strSCHST_DT, strSCHST_TM, strPLANT_CD, StartTM, label1, label2, SCH_NO);

					/////////////////////////MPS Ȯ������ ������ FLG ����///////////////////////////
					MPSFG_UP.SCH_MPSFG_UP(cmd, label1, label2, strPTF, SCH_NO);

					/////////////////////////MPS Ȯ������ ������ FLG ����///////////////////////////
					MPSFG_UP.SCH_TEMP_DEL(cmd, label1, label2, SCH_NO);

					/////////////////////// �������� ������ ���� ���� ///////////////////
					SCH_PROG.DelSchProc(cmd);

					progressBar1.Value = progressBar1.Maximum;
					progressBar2.Value = progressBar2.Maximum;

					label1.Text = "����Ÿ �������Դϴ�.";
					label2.Text = "����Ÿ �������Դϴ�.";

					Trans.Commit();

					/////////////////////// LOG BACKUP �� ��� ///////////////////
					SCH_PROG.DelLogBack();
					SCH_PROG.DelLogSmall();
					/////////////////////// �������� ������ ���� ���� ///////////////////
					///
					dbConn.Close();
				}
				catch(Exception f)
				{
					Trans.Rollback();

					/////////////////////// LOG BACKUP �� ��� ///////////////////
					SCH_PROG.DelLogBack();
					SCH_PROG.DelLogSmall();
					/////////////////////// �������� ������ ���� ���� ///////////////////
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
				label1.Text = "�Ϸ�Ǿ����ϴ�..";
				label2.Text = "�Ϸ�Ǿ����ϴ�..";

			string msgQuery = QUERY.MSG();
			DataTable dtMsg = SystemBase.DbOpen.NoTranDataTable(msgQuery);			// MPS ���� ����

			if(dtMsg.Rows.Count == 0)
			{
				RtnMsg = "���������� ó���Ǿ����ϴ�.";
			}
			else
			{
				RtnMsg = "��¥�� �Էµ��� ���� ��ǰ�� �����մϴ�.";
			}

			return RtnMsg;
		}
		#endregion
	}
}
