using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

namespace PUA101
{

	/// <summary>
	/// BACKWARD_U�� ���� ��� �����Դϴ�.
	/// </summary>
	public class BACKWARD
	{

		#region BACKWARD_EMEG() ���ѿ�����
		public static void BACKWARD_EMEG(
			System.Windows.Forms.ProgressBar progressBar1,
			System.Windows.Forms.Label label1,
			string SCH_ID,
			SqlCommand cmd,
			SqlConnection dbConn,
			SqlTransaction Trans
			)
		{

			label1.Text = "������� �۾� �������� �ð�, �������� �ð��� ������Դϴ�.";

			string Query = QUERY.SRCH_EMG_INFO();

			// ���� ��� ��ȸ
			DataTable dtEMG = SystemBase.DbOpen.NoTranDataTable(Query);

			// MPS ������ ������ ����
			progressBar1.Maximum = 100;

			// PROGRESS ǥ��
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
						// ���� �������� ��� �����Ѵ�.
						SystemBase.Loggers.Log("�������", Query);
						throw new Exception("��ǰ������ȣ: " + dtEMG.Rows[i]["MAKEORDER_NO"].ToString() + " \n\n " + MSGCode);
					}

				}
				catch(Exception f)
				{
					SystemBase.Loggers.Log("�������", Query + " \n\n " + f.ToString());
					throw f;
				}
				progressBar1.Value = 100;

			}
			// PROGRESS ǥ��
			schProg.SchProcStop();
			schProgTh.Join();

		}
		#endregion
	}
}