using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace PC.PUA101 
{
	/// <summary>
	/// BOP�� MBOP_TEMP ���̺� ����
	/// </summary>
	public class MBOPCOPY
	{
		#region MBOPCopy() BOP ����
		public static void MBOP_COPY(
			System.Windows.Forms.ProgressBar progressBar1,
			System.Windows.Forms.Label label1,
			string     SCH_ID,
			SqlCommand cmd,
			SqlConnection dbConn,
			SqlTransaction Trans
			)
		{
	
			// MPS ������ ������ ����
			progressBar1.Maximum = 100;
			progressBar1.Value = 0;

			Thread schProgTh;
			SCH_PROG schProg = new SCH_PROG();
			schProg.SchProg = progressBar1;

			schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));
			schProgTh.Start();

			progressBar1.Maximum = 100;

			// PROGRESS ID
			string Query = "";

			try	
			{ 

				Query  = " usp_P_CRT_MBOP_EMG @pPROC_ID = '"  + PUA101.PROC_ID + "',";
				Query += " @pSCH_ID = '"   + SCH_ID + "',";
				Query += " @pLANG_CD = '"  + SystemBase.Base.gstrLangCd   + "',";
				Query += " @pBIZ_CD    = '" + SystemBase.Base.gstrBIZCD + "', ";
				Query += " @pPLANT_CD  = '" + SystemBase.Base.gstrPLANT_CD + "', ";
				Query += " @pVALID_DT = '" + SystemBase.Base.ServerTime("YMD") + "',";
				Query += " @pUSR_ID = '" + SystemBase.Base.gstrUserID + "',";
                Query += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

				DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

				string ERRCode = ds.Tables[0].Rows[0][0].ToString();
				string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

				if(ERRCode == "ER")  
				{
					// ���� �������� ��� �����Ѵ�.
					SystemBase.Loggers.Log("������� ", Query);
					throw new Exception(MSGCode);
				}

			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log("�������", Query + " \n\n " + f.ToString());
			}
			progressBar1.Value = 100;

			schProg.SchProcStop();
			schProgTh.Join();
			
		}
		#endregion
	}
}