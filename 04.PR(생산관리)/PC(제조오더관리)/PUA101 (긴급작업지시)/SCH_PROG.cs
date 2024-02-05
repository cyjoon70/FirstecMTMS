using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace PUA101
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
				//5�ʰ� ����Ѵ�.
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
			SqlCommand     cmd)
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
		public static void DelSchProc(SqlCommand     cmd)
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
	}
}
