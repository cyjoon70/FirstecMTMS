using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace PUA101
{
	/// <summary>
	/// 스케쥴 시간전개 진해율 표시
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
		/// 스케쥴전개 진행 상태
		/// </summary>
		public bool isInProc()
		{
			return !stopFlag;
		}

		/// <summary>
		/// 스케쥴전개 진행 상태 종료
		/// </summary>
		public void SchProcStop() 
		{
			stopFlag = true;
		}

		/// <summary>
		/// 스케쥴 진행 상태 검사
		/// </summary>
		public void SchProcChk()
		{

			while(!stopFlag)  // 진행상태 조회
			{

				String Query = QUERY.SCH_PROC_RATE();
				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
				
				if(dt.Rows.Count > 0) 
				{

					// 진행률 정보
					string TOT_PROC_AMT  = dt.Rows[0]["TOT_PROC_AMT"].ToString();
					string CURR_PROC_AMT = dt.Rows[0]["CUR_PROC_AMT"].ToString();
					string CURR_PROC_PER = dt.Rows[0]["CUR_PROC_PER"].ToString();

					prog.Value = Convert.ToInt32(CURR_PROC_PER);

				}
				//5초간 대기한다.
				Thread.SpinWait(5000);
			}
		}

		//
		// PROGRESS ID 생성
		//
		public static string GenProcId()
		{
			Random rand = new Random();
			return DateTime.Now.ToString("yyyyMMddHHmm") + rand.Next(100, 999);
		}

		//
		// 전개 중인 스케쥴 존재 여부 판단
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
		// 스케쥴 전개상태 등록
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
		// 스케쥴 전개상태 삭제
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
		// 스케쥴 전개시 배정정보 삭제
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
