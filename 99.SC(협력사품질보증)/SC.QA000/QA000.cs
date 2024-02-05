using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace SC.QA000
{
	public partial class QA000 : UIForm.FPCOMM1
	{
		#region 변수

		#endregion

		#region 생성자
		public QA000()
		{
			InitializeComponent();
		}

		#endregion

		#region Form Load
		private void QA000_Load(object sender, EventArgs e)
		{
			// 첨부파일 분류 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboType, "usp_B_COMMON @pType='COMM', @pCODE = 'SC160', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

			// 업무구분 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboAppr, "usp_SC000 @pType='C1'", 3);

			SelectExec(false);
		}
		#endregion

		#region SelectExec() 그리드 조회 로직
		private void SelectExec(bool Msg)
		{
			try
			{
				string strQuery = "";
				strQuery = " usp_SC000 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @pJOB_TYPE ='" + cboType.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @pAPPR ='" + cboAppr.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, Msg);

				fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
				fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Always);
				fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Always);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if ((fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Text == "True")  || 
                            (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "등록구분")].Text == "N"))
                    {
                        // readonly 처리
                        UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인") + "|3");
                    }
                }
            }
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region NewExec()
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
		}
		#endregion

		#region SearchExec() -- 검색
		protected override void SearchExec()
		{
			SelectExec(true);
		}
		#endregion

		#region SaveExec()
		protected override void SaveExec()
		{
			string ERRCode = "ER", MSGCode = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

					if (strHead.Length > 0)
					{
						string strSql = " usp_SC000 @pTYPE = 'U1'";
						strSql += ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Text == "True")
							strSql += ", @pAPPR = 'Y' ";
						else
							strSql += ", @pAPPR = 'N' ";

						strSql += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text + "' ";
						strSql += ", @pFILES_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일순번")].Text + "' ";
						strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

						DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
						ERRCode = ds.Tables[0].Rows[0][0].ToString();
						MSGCode = ds.Tables[0].Rows[0][1].ToString();

						if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

					}
				}

				if (ERRCode == "ER")
				{
					Trans.Rollback();
					goto Exit;  // ER 코드 Return시 점프
				}
			}
			catch (Exception ex)
			{
				Trans.Rollback();
				MessageBox.Show(ex.ToString());
				MSGCode = "P0001";
				goto Exit;  // ER 코드 Return시 점프
			}
			Trans.Commit();

		Exit:
			dbConn.Close();
			MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
			SearchExec();

		}
		#endregion

		#region 전체 선택/취소
		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{
				fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = true;
				UIForm.FPMake.fpChange(fpSpread1, i);
			}
		}

		private void btnSelectCancel_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
			{
				fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = false;
				UIForm.FPMake.fpChange(fpSpread1, i);
			}
		}
		#endregion

		#region 동일 파일번호 동시 체크 처리 및 파일 보기 및 다운로드
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{

			string strFileNo = string.Empty;

			if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일명_2"))
			{
                
				string FilePath = @"C:\temp";
				DirectoryInfo di = new DirectoryInfo(FilePath);

				if (di.Exists == false) di.Create();

				int DownCnt = 0;

				string Query = " usp_B_IMAGE @pType='S2' ";
				Query += ", @pFILES_NO = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Text + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일순번")].Text + "'";
				DataTable DT = SystemBase.DbOpen.NoTranDataTable(Query);

				if (DT.Rows.Count > 0)
				{
					byte[] MyData = null;
					MyData = (byte[])DT.Rows[0][2];
					int ArraySize = new int();
					ArraySize = MyData.GetUpperBound(0);

					FileStream fs = new FileStream(FilePath + @"\" + DT.Rows[0][0].ToString() + "." + DT.Rows[0][1].ToString(), FileMode.Create, FileAccess.Write);

					fs.Write(MyData, 0, ArraySize + 1);
					fs.Close();

					Process.Start(FilePath + @"\" + DT.Rows[0][0].ToString() + "." + DT.Rows[0][1].ToString());

					DownCnt++;
				}

				if (DownCnt > 0)
				{
					// MessageBox.Show("선택한 폴에 " + DownCnt.ToString() + "개의 파일을 다운로드 하였습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show("파일 다운로드에 실패했습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
            }
			else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "승인"))
			{
				strFileNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString();

				for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
				{
					if (strFileNo == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "파일번호")].Value.ToString())
					{
						if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value.ToString() == "True")
						{
							fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = true;
							UIForm.FPMake.fpChange(fpSpread1, i);
						}

						if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value.ToString() == "False")
						{
							fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "승인")].Value = false;
							UIForm.FPMake.fpChange(fpSpread1, i);
						}
					}
				}
			}

		}
		#endregion

	}
}
