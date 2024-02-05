using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;

namespace PB.PSA010
{

	/// <summary>
	/// BACKWARD_U�� ���� ��� �����Դϴ�.
	/// </summary>
	public class BACKWARD
	{

		#region BACKWARD_LIMIT() ���� ������
		public static void BACKWARD_LIMIT(
			FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			, DataTable dtMPS
			, string strSCHST_DT	// ��������
			, string strSCHST_TM	// ���ؽð�
			, int j
			, string PROJECT_NO
			, string PROJECT_SEQ
			, string SCH_NO
			)
		{

			label1.Text = "������ �۾� �������� �ð�, �������� �ð��� ������Դϴ�.";

			if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
			{

				// MPS ������ ������ ����
				progressBar1.Maximum = dtMPS.Rows.Count;
				progressBar1.Value = 0;
				progressBar2.Maximum = 100;

				// PROGRESS ǥ��
				Thread schProgTh;
				SCH_PROG schProg = new SCH_PROG();
				schProg.SchProg = progressBar2;

				// PROGRESS ID
				schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));
				schProgTh.Start();

				string ERRCode = "", MSGCode = "";

				for(int i=0; i < dtMPS.Rows.Count; i++)
				{

					progressBar1.Value = i;
					progressBar2.Value = 0;

					label2.Text = "������ȣ : " + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + " " +
								  "������ID : " + fpSpread1.Sheets[0].Cells[j, 0].Text + "�� �۾��ð� ������";

					string Query = "", LogQuery = "";

					try
					{
						Query  = " usp_P_BACKWARD @pPROC_ID = " + PSA010.PROC_ID + ", ";
						Query += "                @pSCH_ID = '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
						Query += "                @pMAKEORDER_NO  = '" + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + "', ";
						Query += "                @pMAKEFINISH_DT = '" + dtMPS.Rows[i]["MAKEFINISH_DT"].ToString() + "', ";
						Query += "                @pUNLIMIT_FG    = '" + fpSpread1.Sheets[0].Cells[j, 4].Text + "', ";
						Query += "                @pUSR_ID        = '" + SystemBase.Base.gstrUserID + "', ";
                        Query += "                @pCO_CD         = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						ERRCode = ds.Tables[0].Rows[0][0].ToString();
						MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// ���� �������� ��� �����Ѵ�.
							SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
						}
					}
					catch(Exception f)
					{
						ERRCode = "ER";
						MSGCode = f.ToString();
						SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query + " \n\n " + f.ToString());
						
						// PROGRESS ����
						schProg.SchProcStop();
						schProgTh.Join();

						throw f;
					}

					//SCH LOG DETAIL ����
					LogQuery  = "usp_PSA010 'P2', @pSCH_NO = '" + SCH_NO + "'";
					LogQuery += ", @pSCH_ID = '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "'";
					LogQuery += ", @pMAKEORDER_NO = '" + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + "'";
					LogQuery += ", @pEND_TYPE = '" + ERRCode + "'";
					LogQuery += ", @pEND_MEMO = '" + MSGCode + "'";
					LogQuery += ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "' ";
                    LogQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

					DataSet Logds = SystemBase.DbOpen.TranDataSet(LogQuery, dbConn, Trans);

					string ERRCode1 = Logds.Tables[0].Rows[0][0].ToString();
					string MSGCode1	= Logds.Tables[0].Rows[0][1].ToString();

					if(ERRCode1 == "ER")  
					{
						// ���� �������� ��� �����Ѵ�.
						SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, "SCH LOG DETAIL������ ���� CODE :" + MSGCode1 + "'");
					}

					progressBar2.Value = 100;
				}

				// PROGRESS ǥ��
				schProg.SchProcStop();
				schProgTh.Join();
			}
		}
		#endregion
	}
}
