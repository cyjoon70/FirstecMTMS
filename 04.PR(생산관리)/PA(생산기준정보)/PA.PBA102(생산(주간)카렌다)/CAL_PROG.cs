using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace PA.PBA102
{
	/// <summary>
	/// 스케쥴 시간전개 진해율 표시
	/// </summary>
	public class CAL_PROG
	{

		private bool stopFlag = false;
		private ProgressBar prog;
		private string ProcId;

		public ProgressBar CalProg 
		{
			set
			{
				this.prog = value;
			}
		}

		public string CalProcId
		{
			set
			{
				this.ProcId = value;
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
		public void CalProcChk()
		{

			while(!stopFlag)  // 진행상태 조회
			{

				String Query = " SELECT * ";
				Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
				Query		+= " WHERE  PROC_ID = '" +  ProcId + "'";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);
				
				if(dt.Rows.Count > 0) 
				{

					// 진행률 정보
					string TOT_PROC_AMT  = dt.Rows[0]["TOT_PROC_AMT"].ToString();
					string CURR_PROC_AMT = dt.Rows[0]["CUR_PROC_AMT"].ToString();
					string CURR_PROC_PER = dt.Rows[0]["CUR_PROC_PER"].ToString();

					prog.Value = Convert.ToInt32(CURR_PROC_PER);

				}
				// 5초간 대기한다.
				Thread.SpinWait(5000);
			}
		}

		//
		// PROGRESS ID 생성
		//
		public string GenProcId()
		{
			Random rand = new Random();
			return DateTime.Now.ToString("yyyyMMddHHmm") + rand.Next(100, 999);
		}
	}
}
