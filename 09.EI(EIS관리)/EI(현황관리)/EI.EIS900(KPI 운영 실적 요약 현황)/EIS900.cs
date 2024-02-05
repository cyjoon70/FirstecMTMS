#region 작성정보
/*********************************************************************/
// 단위업무명 : KPI 운영실적현황
// 작 성 자 : 최용준
// 작 성 일 : 2017-06-02
// 작성내용 : 
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using WNDW;
using System.Runtime.InteropServices;

namespace EI.EIS900
{
	public partial class EIS900 : UIForm.FPCOMM1
	{

		#region 생성자
		public EIS900()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void EIS900_Load(object sender, EventArgs e)
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
			dtpInsTranDt.Text = "";
		}
		#endregion

		#region 화면 초기화
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			fpSpread1.Sheets[0].Rows.Count = 0;

			//기타 세팅
			dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
			dtpInsTranDt.Text = "";
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
				{
					string strQuery = " usp_EIS900 'S1'";
					strQuery += ", @pBasicYY ='" + dtpTranDt.Text.Substring(0,4) + "'";
					strQuery += ", @pBasicMM ='" + dtpTranDt.Text.Substring(5,2) + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
					fpSpread1.Sheets[0].Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
					fpSpread1.Sheets[0].Columns[18].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
                    fpSpread1.Sheets[0].Columns[19].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Restricted;     // 2017.06.19. hma 추가: 클리닉대상 항목 병합 처리
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

					// 7,8   11, 12
					fpSpread1.Sheets[0].Columns[7].Label = (Convert.ToInt32(dtpTranDt.Text.Substring(0, 4)) - 1).ToString() + " 실적";
					fpSpread1.Sheets[0].Columns[8].Label = dtpTranDt.Text.Substring(0, 4) + " 목표";
					fpSpread1.Sheets[0].Columns[11].Label = Convert.ToInt16(dtpTranDt.Text.Substring(5, 2)).ToString() + "월 계획";
					fpSpread1.Sheets[0].Columns[12].Label = Convert.ToInt16(dtpTranDt.Text.Substring(5, 2)).ToString() + "월 실적";

				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}			
		}
		#endregion

		#region 엑셀UPLOAD
		private void btnFileUpload_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;

			// 부서,팀명
			string strOrgDept = string.Empty;
			string strDtDept = string.Empty;

			// 종합평가
			decimal dOrgLastPoint = 0;
			decimal dDtLastPoint = 0;

            // 2017.06.19. hma 추가(Start): 클리닉대상
            string strOrgClinic = string.Empty;
            string strClinic = string.Empty;
            // 2017.06.19. hma 추가(End)

            string ERRCode = string.Empty;
			string MSGCode = string.Empty;

			string strSql = string.Empty;

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd1 = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				if (string.IsNullOrEmpty(txtFilePath.Text) || string.IsNullOrEmpty(dtpInsTranDt.Text))
				{
					MessageBox.Show("등록기준년월 및 업로드할 파일명은 필수 입니다.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=no;""", txtFilePath.Text);

				// xlsx 확장자 버전 처리. 오류가 발생하므로 사용안함.
				//string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Mode=ReadWrite|Share Deny None;Extended Properties='Excel 12.0; HDR=no;IMEX=1';Persist Security Info=False;", txtFilePath.Text);

				OleDbConnection conn = new OleDbConnection(connectionString);
				conn.Open();

				DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

				string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[0]["TABLE_NAME"]);
				OleDbCommand cmd = new OleDbCommand(commandString, conn);

				OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
				DataSet ds = new DataSet();

				dapt.Fill(ds);
				conn.Close();

				if (ds != null)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{

						#region 삭제
						strSql = string.Empty;
						strSql = " usp_EIS900 ";
						strSql += " @pTYPE			 = 'D1' ";
						strSql += ",@pBasicYY		 = '" + dtpInsTranDt.Text.Substring(0, 4) + "'";
						strSql += ",@pBasicMM		 = '" + dtpInsTranDt.Text.Substring(5, 2) + "'";

						DataSet dsDel = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
						ERRCode = dsDel.Tables[0].Rows[0][0].ToString();
						MSGCode = dsDel.Tables[0].Rows[0][1].ToString();

						if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프 
						#endregion

						for (int i = 6; i <= ds.Tables[0].Rows.Count-1; i++)
						{

							//MessageBox.Show(ds.Tables[0].Rows[i][0].ToString());

                            // 부서/팀명 항목
							if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][0].ToString()))
							{
								strDtDept = ds.Tables[0].Rows[i][0].ToString();
							}
							else
							{
								strDtDept = strOrgDept;
							}

                            // 종합평가 항목
							if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][15].ToString()))
							{
								dDtLastPoint = Convert.ToDecimal(ds.Tables[0].Rows[i][15].ToString().Replace(",",""));
                            }
							else
							{
								dDtLastPoint = dOrgLastPoint;
							}

                            // 2017.06.19. hma 추가(Start): 클리닉대상 항목
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][16].ToString()))
                            {
                                strClinic = ds.Tables[0].Rows[i][16].ToString();
                            }
                            else
                            {
                                strClinic = strOrgClinic;
                            }
                            // 2017.06.19. hma 추가(End)

                            if (string.Compare(strOrgDept, strDtDept, true) != 0)
							{
								ds.Tables[0].Rows[i][0] = strDtDept;
								ds.Tables[0].Rows[i][15] = dDtLastPoint.ToString();
                                ds.Tables[0].Rows[i][16] = strClinic;       // 2017.06.19. hma 추가: 클리닉대상
                                strOrgDept = strDtDept;
								dOrgLastPoint = dDtLastPoint;
                                strOrgClinic = strClinic;                   // 2017.06.19. hma 추가: 클리닉대상
                            }
							else
							{
								ds.Tables[0].Rows[i][0] = strOrgDept;
								ds.Tables[0].Rows[i][15] = dOrgLastPoint.ToString();
                                ds.Tables[0].Rows[i][16] = strOrgClinic;        // 2017.06.19. hma 추가: 클리닉대상
                            }

							#region DB Insert
							strSql = string.Empty;
							strSql = " usp_EIS900 ";
							strSql += " @pTYPE			 = 'I1' ";
							strSql += ",@pBasicYY		 = '" + dtpInsTranDt.Text.Substring(0,4) + "'";
							strSql += ",@pBasicMM		 = '" + dtpInsTranDt.Text.Substring(5,2) + "'";
							strSql += ",@pDept			 = '" + ds.Tables[0].Rows[i][0].ToString() + "'";   // 부서, 팀명
							strSql += ",@pNo			 = '" + ds.Tables[0].Rows[i][1].ToString() + "'";   // No.
							strSql += ",@pKPI			 = '" + ds.Tables[0].Rows[i][2].ToString() + "'";   // 성과지표(KPI)
							strSql += ",@pFormula		 = '" + ds.Tables[0].Rows[i][3].ToString() + "'";   // 산출식

							if (ds.Tables[0].Rows[i][4] == null || ds.Tables[0].Rows[i][4].ToString() == "" || ds.Tables[0].Rows[i][4].ToString() == "-")
							{
								strSql += ",@pPrevYearActual	 = 0 ";    // 전년실적
							}
							else
							{
								strSql += ",@pPrevYearActual	 = " + ds.Tables[0].Rows[i][4].ToString().Replace(",","") + "";    // 전년실적
							}

							if (ds.Tables[0].Rows[i][5] == null || ds.Tables[0].Rows[i][5].ToString() == "" || ds.Tables[0].Rows[i][5].ToString() == "-")
							{
								strSql += ",@pCurrYearPlan	 = 0 ";    // 당해목표
							}
							else
							{
								strSql += ",@pCurrYearPlan	 = " + ds.Tables[0].Rows[i][5].ToString().Replace(",", "") + "";    // 당해목표
							}

							strSql += ",@pUnit			 = '" + ds.Tables[0].Rows[i][6].ToString() + "'";   // 단위

							if (ds.Tables[0].Rows[i][7] == null || ds.Tables[0].Rows[i][7].ToString() == "" || ds.Tables[0].Rows[i][7].ToString() == "-")
							{
								strSql += ",@pWeight	 = 0 ";    // 가중치
							}
							else
							{
								strSql += ",@pWeight	 = " + ds.Tables[0].Rows[i][7].ToString().Replace(",", "") + "";    // 가중치
							}

							if (ds.Tables[0].Rows[i][8] == null || ds.Tables[0].Rows[i][8].ToString() == "" || ds.Tables[0].Rows[i][8].ToString() == "-")
							{
								strSql += ",@pPlanMM	 = 0 ";    // 월계획
							}
							else
							{
								strSql += ",@pPlanMM	 = " + ds.Tables[0].Rows[i][8].ToString().Replace(",", "") + "";    // 월계획
							}

							if (ds.Tables[0].Rows[i][9] == null || ds.Tables[0].Rows[i][9].ToString() == "" || ds.Tables[0].Rows[i][9].ToString() == "-")
							{
								strSql += ",@pActualMM	 = 0 ";    // 월실적
							}
							else
							{
								strSql += ",@pActualMM	 = " + ds.Tables[0].Rows[i][9].ToString().Replace(",", "") + "";    // 월실적
							}

							strSql += ",@pIncDec		 = '" + ds.Tables[0].Rows[i][10].ToString() + "'";   // 증감

							if (ds.Tables[0].Rows[i][11] == null || ds.Tables[0].Rows[i][11].ToString() == "" || ds.Tables[0].Rows[i][11].ToString() == "-")
							{
								strSql += ",@pGap	 = 0 ";    // 차이
							}
							else
							{
								strSql += ",@pGap	 = " + ds.Tables[0].Rows[i][11].ToString().Replace(",", "") + "";    // 차이
							}

							strSql += ",@pCurrTotal		 = '" + ds.Tables[0].Rows[i][12].ToString() + "'";  // 누적비교
							strSql += ",@pGrade			 = '" + ds.Tables[0].Rows[i][13].ToString() + "'";  // 지표평가등급

							if (ds.Tables[0].Rows[i][14] == null || ds.Tables[0].Rows[i][14].ToString() == "" || ds.Tables[0].Rows[i][14].ToString() == "-")
							{
								strSql += ",@pPoint	 = 0 ";    // 가중치환산점수
							}
							else
							{
								strSql += ",@pPoint	 = " + ds.Tables[0].Rows[i][14].ToString().Replace(",", "") + "";    // 가중치환산점수
							}

							if (ds.Tables[0].Rows[i][15] == null || ds.Tables[0].Rows[i][15].ToString() == "" || ds.Tables[0].Rows[i][15].ToString() == "-")
							{
								strSql += ",@pEvaluation	 = 0 ";    // 종합평가
							}
							else
							{
								strSql += ",@pEvaluation	 = " + ds.Tables[0].Rows[i][15].ToString().Replace(",", "") + "";    // 종합평가
							}

                            // 2017.06.19. hma 추가(Start): 클리닉대상
                            if (ds.Tables[0].Rows[i][16].ToString() == "")
                                strSql += ",@pClinicTarget			 = ' '";        // 값이 없으면 Merge가 안되어 공백1개를 강제로 넣어줌.
                            else
                                strSql += ",@pClinicTarget			 = '" + ds.Tables[0].Rows[i][16].ToString() + "'";
                            // 2017.06.19. hma 추가(End)

                            strSql += ",@pInUpId		 = '" + SystemBase.Base.gstrUserID + "'";

							DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
							ERRCode = ds1.Tables[0].Rows[0][0].ToString();
							MSGCode = ds1.Tables[0].Rows[0][1].ToString();

							if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프 
							#endregion

						}

						Trans.Commit();

					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				Trans.Rollback();
				ERRCode = "ER";
				MSGCode = f.Message;
			}
			finally
			{
				dbConn.Close();
				this.Cursor = Cursors.Default;
			}

			Exit:
			
			if (ERRCode == "OK")
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				dtpTranDt.Value = dtpInsTranDt.Value;
				SearchExec();
			}
			else if (ERRCode == "ER")
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

		}
		#endregion

		#region Upload 파일 선택
		private void btnFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtFilePath.Value = dlg.FileName;
			}
		}

		#endregion

		#region 양식 파일 Download
		private void btnFileDownload_Click(object sender, EventArgs e)
		{
			string updndl = string.Empty;

			if (SystemBase.Base.gstrUserID == "ADMIN")
				updndl = "Y#Y#Y";
			else
				updndl = "N#Y#N";

			UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
			form1.ShowDialog();
		}
		#endregion

	}
}
