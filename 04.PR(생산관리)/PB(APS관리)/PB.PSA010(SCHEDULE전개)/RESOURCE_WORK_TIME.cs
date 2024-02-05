using System;
using System.Data;
using System.Data.SqlClient;

namespace PB.PSA010
{
	/// <summary>
	/// RESOURCE_WORK_TIME�� ���� ��� �����Դϴ�.
	/// </summary>
	public class RESOURCE_WORK_TIME
	{

		#region RESOURCEWORKTIME() P_BOP_M_PROC_DETAIL_TEMP ���̺� �� �۾��ð�(�۾����ýð�) ��� �� ������Ʈ
		public static void RESOURCEWORKTIME(
			System.Windows.Forms.ProgressBar progressBar2,
			System.Windows.Forms.Label label2, 
			SqlCommand cmd,
			DataTable dtMPS,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	//	CMLT(���� L/T)�� ���ÿ� ������Ʈ
			label2.Text = "�۾��ð� ������Դϴ�.";

			// ���α׷��� �ʱ�ȭ
			progressBar2.Maximum = dtMPS.Rows.Count;
			progressBar2.Value   = 0;

			for(int i=0; i < dtMPS.Rows.Count; i++)
			{
				for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
				{	//������ id�� BOP MBOP�� ����
					if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
					{
						label2.Text = "��ǰ������ȣ " + dtMPS.Rows[i]["MAKEORDER_NO"].ToString() + 
							" ������ ID " + fpSpread1.Sheets[0].Cells[j, 0].Text + "�� �۾��ð� ������Դϴ�.";

						string RESTimeUP = QUERY.WORK_TM_UP(
							dtMPS.Rows[i]["MAKEORDER_NO"].ToString(),
							fpSpread1.Sheets[0].Cells[j, 0].Text,
							fpSpread1.Sheets[0].Cells[j,3].Text == "������"?"BWD":"FWD"
							);
						cmd.CommandText = RESTimeUP;
						cmd.ExecuteNonQuery();
					}
				}
				progressBar2.Value = i + 1;
			}
		}
		#endregion
		
		#region ���������� ���� ������ ó��
		public static void INIT_RESOURCE_WORK_TIME(
			System.Windows.Forms.Label label2, 
			SqlCommand     cmd,
			SqlConnection  dbConn,
			SqlTransaction Trans,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	
			//	CMLT(���� L/T)�� ���ÿ� ������Ʈ
			label2.Text = "���������� ó�����Դϴ�.";

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{	//������ id�� �ڿ� CALENDAR ����
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{

					string Query = "";

					try
					{
						Query += " usp_P_INIT_RESO_WORK_TIME '" + SystemBase.Base.gstrCOMCD.ToString() + "',";
                        Query += "                           '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
						Query += "                           '" + fpSpread1.Sheets[0].Cells[j, 4].Text + "' ";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						string ERRCode = ds.Tables[0].Rows[0][0].ToString();
						string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// ���� �������� ��� �����Ѵ�.
							SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
							throw new Exception(MSGCode);
						}

					}
					catch(Exception f)
					{
						SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, f.ToString());
						throw f;
					}
				}
			}
		}
		#endregion

		#region �ڿ��� �ְ�/OT CALENDAR ����
		public static void RESOURCEWEEKCAL(
			System.Windows.Forms.Label label2, 
			SqlCommand     cmd,
			SqlConnection  dbConn,
			SqlTransaction Trans,
			FarPoint.Win.Spread.FpSpread fpSpread1
			)
		{	
			label2.Text = "�ڿ� �ְ�/OT CALENDAR �������Դϴ�.";

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{	//������ id�� �ڿ� CALENDAR ����
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{

					string Query = "";

					try
					{

						Query  = " DECLARE @O_RTN_CHK AS VARCHAR(2), "; 
						Query += "         @O_MSG_CD  AS VARCHAR(50)  ";

                        Query += " EXEC usp_P_CRT_RESO_CAL '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        Query += "                         '" + fpSpread1.Sheets[0].Cells[j, 0].Text + "', ";
						Query += "                         'N',"; // (N : NEW ) ���� �����Ѵ�.
						Query += "                         'N',"; // (N : NORMAL ) �⺻ ������ ����
						Query += "                         '" + SystemBase.Base.gstrUserID + "', ";
						Query += "                         @O_RTN_CHK   OUTPUT, ";
						Query += "                         @O_MSG_CD    OUTPUT  ";
						Query += " SELECT @O_RTN_CHK, @O_MSG_CD ";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						string ERRCode = ds.Tables[0].Rows[0][0].ToString();
						string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// ���� �������� ��� �����Ѵ�.
							SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, Query);
							throw new Exception(MSGCode);
						}

					}
					catch(Exception f)
					{
						SystemBase.Loggers.Log("������ ID " + fpSpread1.Sheets[0].Cells[j, 1].Text, f.ToString());
						throw f;
					}
				}
			}
		}
		#endregion

	}
}