using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace PA.PBA102
{
	/// <summary>
	/// ������ �ð����� ������ ǥ��
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
		public void CalProcChk()
		{

			while(!stopFlag)  // ������� ��ȸ
			{

				String Query = " SELECT * ";
				Query		+= " FROM   P_BOP_SCH_PROC_TEMP A (NOLOCK) ";
				Query		+= " WHERE  PROC_ID = '" +  ProcId + "'";

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
		public string GenProcId()
		{
			Random rand = new Random();
			return DateTime.Now.ToString("yyyyMMddHHmm") + rand.Next(100, 999);
		}
	}
}
