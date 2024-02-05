using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;

namespace PB.PSA010
{
	/// <summary>
	/// BOP를 MBOP_TEMP 테이블 생성
	/// </summary>
	public class MBOPCOPY
	{
		#region MBOPCopy() BOP 복사
		public static void MBOP_COPY(FarPoint.Win.Spread.FpSpread fpSpread1
			, System.Windows.Forms.ProgressBar progressBar1
			, System.Windows.Forms.ProgressBar progressBar2
			, System.Windows.Forms.Label label1
			, System.Windows.Forms.Label label2
			, string strPTF
			, string strPLANT_CD
			, DataTable dtMPS
			, SqlCommand cmd
			, SqlConnection dbConn
			, SqlTransaction Trans
			, string PROJECT_NO
			, string PROJECT_SEQ
			)
		{
	
			// MPS 정보로 스케쥴 전개
			progressBar1.Maximum = fpSpread1.Sheets[0].Rows.Count;
			progressBar1.Value = 0;

            Thread schProgTh;

			SCH_PROG schProg = new SCH_PROG();
			schProg.SchProg = progressBar2;

			schProgTh = new Thread(new ThreadStart(schProg.SchProcChk));

			schProgTh.Start();

			for(int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{

				if(fpSpread1.Sheets[0].Cells[i,2].Text == "True")
				{
					progressBar2.Maximum = 100;

					string Query = "";

					try	
					{
						Query  = " usp_P_CRT_MBOP_TMP @pPROC_ID = '"  + PSA010.PROC_ID + "',";
						Query += "@pSCH_ID= '" + fpSpread1.Sheets[0].Cells[i, 0].Text + "', ";
						Query += "@pPLANT_CD= '" + strPLANT_CD + "', ";
						Query += "@pBIZ_CD= '" + SystemBase.Base.gstrBIZCD + "', ";
						Query += "@pMAKEFINISH_DT= '" + strPTF + "', ";
						Query += "@pPROJECT_NO= '" + PROJECT_NO  + "', ";
						Query += "@pPROJECT_SEQ= '" + PROJECT_SEQ + "', ";
						Query += "@pDEPLOY= '" + (fpSpread1.Sheets[0].Cells[i,3].Text == "역전개"?"BWD":"FWD") + "', ";
						Query += "@pSTOCK_CONSD_YN='" + (fpSpread1.Sheets[0].Cells[i,5].Text == "True"?"Y":"N") + "', ";
						Query += "@pAVAIL_STOCK_CONSD_YN= '" + (fpSpread1.Sheets[0].Cells[i,6].Text == "True"?"Y":"N") + "', ";
						Query += "@pUSR_ID= '" + SystemBase.Base.gstrUserID + "',";
                        Query += "@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

						DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

						string ERRCode = ds.Tables[0].Rows[0][0].ToString();
						string MSGCode	= ds.Tables[0].Rows[0][1].ToString();

						if(ERRCode == "ER")  
						{
							// 다음 스케쥴을 계속 실행한다.
							SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[i, 1].Text, Query);
						}
					}
					catch(Exception f)
					{
						SystemBase.Loggers.Log("스케쥴 ID " + fpSpread1.Sheets[0].Cells[i, 1].Text, Query + " \n\n " + f.ToString());
					}
				}
				progressBar1.Value = i + 1;
			}
			schProg.SchProcStop();
			schProgTh.Join();
			
		}
		#endregion
	}
}