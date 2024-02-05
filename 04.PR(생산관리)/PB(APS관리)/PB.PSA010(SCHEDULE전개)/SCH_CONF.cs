using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{
	/// <summary>
	/// SCHCONF�� ���� ��� �����Դϴ�.
	/// </summary>
	public class SCH_CONF
	{
		public static string PROC_STATUS_CONF_SCH = "CS";

		public SCH_CONF()
		{
			//
			// TODO: ���⿡ ������ ���� �߰��մϴ�.
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

				/////////////////////// ������ ���μ��� ��� //////////////////////////
				SCH_PROG.InsSchProc(PSA010.PROC_TYPE, PROC_STATUS_CONF_SCH, cmd);
				/////////////////////// ������ ���μ��� ��� /////////////////////////

				string ERRCode = "";
				string MSGCode = "";

				Thread schProgTh; // ����� ����
				SCH_PROG schProg = new SCH_PROG();

				// ������ ǥ��
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
					}	// ER �ڵ� Return�� ����
					else
					{
						/////////////////////// �������� ������ ���� ���� ///////////////////
						SCH_PROG.DelSchProc(cmd);
						/////////////////////// �������� ������ ���� ���� ///////////////////

						Trans.Commit();

						schProg.SchProcStop(); // PROGRESS ����
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
						// PROGRESS�� ������ �������� ���
						if(schProg.isInProc()) 
							schProg.SchProcStop(); // PROGRESS ����

						// ������ �ڿ� �Ҹ�
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
