using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace PB.PSA010
{ 
	/// <summary>
	/// ������ �ð����� ������ ǥ��
	/// </summary>
	public class SCH_PROG
	{

		private bool stopFlag = false;
		private ProgressBar prog;

		public ProgressBar SchProg 
		{
			set
			{
				this.prog = value;
			}
		}

		/// <summary>
		/// ���������� ���� ����
		/// </summary>
		public bool isInProc()
		{
			return !stopFlag;
		}

		/// <summary>
		/// ���������� ���� ���� ����
		/// </summary>
		public void SchProcStop() 
		{
			stopFlag = true;
		}

		/// <summary>
		/// ������ ���� ���� �˻�
		/// </summary>
		public void SchProcChk()
		{

			while(!stopFlag)  // ������� ��ȸ
			{

				String Query = QUERY.SCH_PROC_RATE();
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
				
				if(dt.Rows.Count > 0) 
				{

					// ����� ����
					string TOT_PROC_AMT  = dt.Rows[0]["TOT_PROC_AMT"].ToString();
					string CURR_PROC_AMT = dt.Rows[0]["CUR_PROC_AMT"].ToString();
					string CURR_PROC_PER = dt.Rows[0]["CUR_PROC_PER"].ToString();

					prog.Value = Convert.ToInt32(CURR_PROC_PER);

				}
				// 5�ʰ� ����Ѵ�.
				Thread.SpinWait(5000);
			}
		}

		//
		// PROGRESS ID ����
		//
		public static string GenProcId()
		{
			Random rand = new Random();
			return DateTime.Now.ToString("yyyyMMddHHmm") + rand.Next(100, 999);
		}

		//
		// ���� ���� ������ ���� ���� �Ǵ�
		//
		public static string HasDplySch()
		{
			String Query = QUERY.SCH_PROC_CHK();
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if(dt.Rows.Count > 0)
				return dt.Rows[0]["IN_ID"].ToString();
			return null;
		}

		//
		// ������ �������� ���
		//
		public static void InsSchProc(string PROC_TYPE,
									  string PROC_STATUS,
									  SqlCommand cmd)
		{

			try 
			{
				String Query = QUERY.INS_SCH_PROC(PROC_TYPE, PROC_STATUS, SystemBase.Base.gstrUserID);
				
				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// ������ �������� ����
		//
		public static void DelSchProc(SqlCommand cmd)
		{

			try 
			{
				String Query = QUERY.DEL_SCH_PROC();

				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// ������ ������ �������� ����
		//
		public static void DelResoWorkTime(SqlCommand cmd)
		{
			try 
			{
				String Query = QUERY.DEL_RESO_WORK_TIME();

				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// �������� ���� ����
		//
		public static void DelOrderProc(SqlCommand cmd)
		{
			try 
			{
				String Query =  QUERY.DEL_OREDER_PROC();

				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// SCH ������ LOG���
		//
		public static void DelLogBack()
		{
			try 
			{
				String Query = "BACKUP LOG MTMS_FT WITH NO_LOG";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// SCH ������ LOG Size 1�� ���
		//
		public static void DelLogSmall()
		{
			try 
			{
				String Query = "DBCC SHRINKFILE (MTMS_log, 1)";
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
			}
			catch(Exception f)
			{
				throw f;
			}
		}

		//
		// SCH NO ä��
		//
		public static string SchAutoNo(string SCHST_DT, string SCHST_TM, string PTF, string strSCH_MEMO, string strMEMO)
		{
			string strSchNo = "";

			String Query = " usp_PSA010 'P1'";
			Query += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
			Query += ", @pBIZ_CD ='" + SystemBase.Base.gstrBIZCD + "'";
			Query += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
			Query += ", @pIN_ID ='" + SystemBase.Base.gstrUserID + "'";
			Query += ", @pBASE_DT ='" + SCHST_DT + "'";
			Query += ", @pBASE_TM ='" + SCHST_TM + "'";
			Query += ", @pPTF_DT ='" + PTF + "'";
			Query += ", @pSCH_MEMO ='" + strSCH_MEMO + "'";
			Query += ", @pMEMO ='" + strMEMO + "'";

			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
			
			if(dt.Rows.Count > 0) 
			{
				strSchNo  = dt.Rows[0][1].ToString();
			}

			return strSchNo;
		}

	}
}
