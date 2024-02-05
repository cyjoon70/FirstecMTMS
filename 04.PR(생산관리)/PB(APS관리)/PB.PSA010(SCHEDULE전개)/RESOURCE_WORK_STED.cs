using System;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win; 
using FarPoint.Win.Spread;

namespace PB.PSA010
{
	/// <summary>
	/// DEPLOY_TM�� ���� ��� �����Դϴ�.
	/// </summary>
	public class RESOURCE_WORK_STED
	{

		#region RESOURCE_WORK_STED()  �������
		public static void RESOURCE_WORK_MASTER(
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
			, string PROJECT_NO
			, string PROJECT_SEQ
			, string SCH_NO
			)
		{

			int SchRowCount=0;

			for(int h=0; h < fpSpread1.Sheets[0].Rows.Count; h++)
			{
				if(fpSpread1.Sheets[0].Cells[h,2].Text == "True" && fpSpread1.Sheets[0].Cells[h,3].Text.Length > 0)
					SchRowCount++;
			}

			for(int j = 0; j < fpSpread1.Sheets[0].Rows.Count; j++)
			{
				if(fpSpread1.Sheets[0].Cells[j,2].Text == "True")
				{
					if(fpSpread1.Sheets[0].Cells[j,3].Text == "������")
					{
						//########################### ����/���� ������ ###########################//
						BACKWARD.BACKWARD_LIMIT(
							fpSpread1, 
							progressBar1, 
							progressBar2,
							label1, 
							label2, 
							cmd, 
							dbConn, 
							Trans, 
							dtMPS, 
							strSCHST_DT, 
							strSCHST_TM, 
							j, 
							PROJECT_NO,
							PROJECT_SEQ,
							SCH_NO);
						//########################### ����/���� ������ ###########################//
					}
					else if(fpSpread1.Sheets[0].Cells[j,3].Text == "������")
					{
						//########################### ���� ������ ###########################//
						FORWARD.FORWARD_LIMIT(
							fpSpread1, 
							progressBar1, 
							progressBar2,
							label1, 
							label2, 
							cmd, 
							dbConn, 
							Trans, 
							dtMPS, 
							strSCHST_DT, 
							strSCHST_TM, 
							j, 
							PROJECT_NO,
							PROJECT_SEQ);
						//########################### ���� ������ ###########################//
					}
				}
			}
		}
		#endregion

	}
}
