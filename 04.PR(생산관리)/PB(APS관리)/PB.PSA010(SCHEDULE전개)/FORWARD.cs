using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace PB.PSA010
{
	/// <summary>
	/// FORWARD_U�� ���� ��� �����Դϴ�.
	/// </summary>
	public class FORWARD
	{
		#region FORWARD_LIMIT() FORWARD_LIMIT2 ���� ������
		public static void FORWARD_LIMIT(
			FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			, DataTable dtMPS
			, string strSCHST_DT
			, string strSCHST_TM
			, int j
			, string PROJECT_NO
			, string PROJECT_SEQ
			)
		{
			
			// MPS ������ ������ ����
			progressBar2.Maximum = 100;
			label1.Text = "������ �۾� �������� �ð�, �������� �ð��� ������Դϴ�.";

			// ����� ǥ��
			Thread schProgTh;
			SCH_PROG schProg = new SCH_PROG();
			schProg.SchProg = progressBar2;

			schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));
			schProgTh.Start();

			for(int i=0; i < dtMPS.Rows.Count; i++)
			{
				
				progressBar2.Value = 0;
				label2.Text = "������ȣ : " + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + " " +
							  "������ID : " + fpSpread1.Sheets[0].Cells[j, 0].Text + "�� �۾��ð� ������";

				string Query = "";

				try
				{

					Query  = " usp_P_FORWARD  @pPROC_ID = '" + PSA010.PROC_ID + "', ";
					Query += "                @pSCH_ID =  '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
					Query += "                @pMAKEORDER_NO  = '" + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + "', ";
					Query += "                @pST_DT  = '" + strSCHST_DT + "', ";
					Query += "                @pST_TM  = '" + strSCHST_TM + "', ";
                    Query += "                @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

					DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

					string ERRCode = ds.Tables[0].Rows[0][0].ToString();
					string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

					if(ERRCode == "ER")  
					{
						// ���� �������� ��� �����Ѵ�.
						SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
					}

				}
				catch(Exception f)
				{
					SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query + " \n\n " + f.ToString());
				}
				progressBar2.Value = 100;

			}

			// PROGRESS ǥ��
			schProg.SchProcStop();
			schProgTh.Join();


		}
		#endregion

	}
}
