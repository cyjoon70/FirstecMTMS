
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질증빙설정
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-07-17
// 작성내용 : 품목/공정 조회 후, 품질 문서 필수 여부 지정
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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TD.TDA002
{
	public partial class TDA002 : UIForm.FPCOMM2T
	{

		#region 변수 정의

		// fpSpread3 관련 변수 정의
		string[] G3Head1 = null;	// 첫번째 Head Text
		string[] G3Head2 = null;	// 두번째 Head Text
		string[] G3Head3 = null;	// 세번째 Head Text
		int[] G3Width = null;		// Cell 넓이
		string[] G3Align = null;	// Cell 데이타 정렬방식
		string[] G3Type = null;		// CellType 지정
		int[] G3Color = null;		// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
		string[] G3Etc = null;		// Mask 양식 등
		int G3HeadCnt = 0;			// Head 수
		int[] G3SEQ = null;			// 키

		// 문서 조회 키값 변수 정의
		string strKey_R = string.Empty;
		string strType_R = string.Empty;
		string strKey_I = string.Empty;
		string strType_I = string.Empty;

		UIForm.FindText frm = new UIForm.FindText();

		#endregion

		#region 생성자
		public TDA002()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load 시
		private void TDA002_Load(object sender, System.EventArgs e)
		{
			try
			{
				SystemBase.Validation.GroupBox_Setting(groupBox5);
				SystemBase.Validation.GroupBox_Setting(groupBox7);

				SetControl();

				UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

				// FORM TEMPLATE에 없는 그리드 추가
				SetFpSpread3(null);
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Setting(groupBox5);
			SystemBase.Validation.GroupBox_Setting(groupBox7);

			strKey_R = string.Empty;
			strType_R = string.Empty;
			strKey_I = string.Empty;
			strType_I = string.Empty;

			SetControl();

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

			// FORM TEMPLATE에 없는 그리드 추가
			SetFpSpread3(null);
		}
		#endregion

		#region 컨트롤 초기화 설정
		private void SetControl()
		{
			// 공장코드 설정
			SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
			SystemBase.ComboMake.C1Combo(cboPlant_P, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

			// 품목계정
			SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3);

			// 품목그룹
			//SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'TD003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3);

			// 품목구분
			SystemBase.ComboMake.C1Combo(cboItemType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'P032', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3);

			// 부서
			SystemBase.ComboMake.C1Combo(cboDept, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'TD001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3);
			cboDept.SelectedIndex = 0;

			optAll.Checked = true;
			optAll_P.Checked = true;
			optMsAll.Checked = true;
			optInAll.Checked = true;
			optUsAll.Checked = true;

			//txtItemCd.Text = "";
			//txtItemNM.Text = "";
			//txtProCD_P.Text = "";
			//txtProNM_P.Text = "";

		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{

			string strATT_YN = string.Empty;
			string strMS_YN = string.Empty;
			string strInst_YN = string.Empty;
			string strUsing_YN = string.Empty;
			string strQuery = string.Empty;

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			// 품목
			if (c1DockingTab1.SelectedTab.TabIndex == 0)
			{
			
				if (optAll.Checked == true) strATT_YN = "";
				if (optSetup.Checked == true) strATT_YN = "Y";
				if (optNotSetup.Checked == true) strATT_YN = "N";
				
				strQuery = "usp_TDA002 ";
				strQuery = strQuery + " @pTYPE = 'S1' ";
				strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pITEM_CD ='" + txtItemCd.Text + "' ";
				strQuery = strQuery + ",@pITEM_NM ='" + txtItemNM.Text + "' ";
				strQuery = strQuery + ",@pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pBOM_FLAG ='" + cboItemType.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pATT_TYPE_ITEM ='" + strATT_YN + "' ";
                strQuery = strQuery + ",@pITEM_SPEC ='" + txtITEM_SPEC.Text + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				SetRowBackColor(fpSpread1, 1);
				
			}

			// 공정
			if (c1DockingTab1.SelectedTab.TabIndex == 1)
			{

				if (optAll_P.Checked == true) strATT_YN = "";
				if (optSetup_P.Checked == true) strATT_YN = "Y";
				if (optNotSetup_P.Checked == true) strATT_YN = "N";

				if (optMsAll.Checked == true) strMS_YN = "";
				if (optMsY.Checked == true) strMS_YN = "Y";
				if (optMsN.Checked == true) strMS_YN = "N";

				if (optInAll.Checked == true) strInst_YN = "";
				if (optInY.Checked == true) strInst_YN = "Y";
				if (optInN.Checked == true) strInst_YN = "N";

				if (optUsAll.Checked == true) strUsing_YN = "";
				if (optUsY.Checked == true) strUsing_YN = "Y";
				if (optUsN.Checked == true) strUsing_YN = "N";

				strQuery = "usp_TDA002 ";
				strQuery = strQuery + " @pTYPE = 'S2' ";
				strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pJOB_CD ='" + txtProCD_P.Text + "' ";
				strQuery = strQuery + ",@pJOB_NM ='" + txtProNM_P.Text + "' ";
				strQuery = strQuery + ",@pMILESTONE_FLG ='" + strMS_YN + "' ";
				strQuery = strQuery + ",@pINSP_FLG ='" + strInst_YN + "' ";
				strQuery = strQuery + ",@pUSE_FLG ='" + strUsing_YN + "' ";
				strQuery = strQuery + ",@pATT_TYPE_JOB ='" + strATT_YN + "' ";

				SetFpSpread3(strQuery);
				SetRowBackColor(fpSpread3, 3);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;

		}
		#endregion

		#region 설정완료시 Row BackColor 강조. 
		private void SetRowBackColor(FarPoint.Win.Spread.FpSpread fpSpread, int iType)
		{

			if (fpSpread.ActiveSheet.Rows.Count > 0)
			{

				for(int i=0;i<= fpSpread.ActiveSheet.Rows.Count-1;i++)
				{
				
					if (iType == 1) // 품목
					{
						if (fpSpread.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부설정")].Text.ToString() == "설정")
						{
							fpSpread.ActiveSheet.Rows[i].BackColor = Color.LightGreen;
						}
					}
					else
					{
						if (fpSpread.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "첨부설정")].Text.ToString() == "설정")
						{
							fpSpread.ActiveSheet.Rows[i].BackColor = Color.LightGreen;
						}
					}

				}

			}
			
		}
		#endregion

		#region SaveExec() 폼에 입력된 데이타 저장 로직
		protected override void SaveExec()
		{

			int iMasterCnt = 0;

			string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.
			string strQuery = string.Empty;
			string strReq_YN = string.Empty;
			
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				if (fpSpread2.ActiveSheet.Rows.Count > 0)
				{

					// 품목
					if (c1DockingTab1.SelectedTab.TabIndex == 0)
					{

						if (fpSpread1.ActiveSheet.Rows.Count > 0)
						{

							for (int i = 0; i <= fpSpread1.ActiveSheet.Rows.Count - 1; i++)
							{
								if (fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text.ToString() == "True")
								{

									iMasterCnt++;

									if (fpSpread2.ActiveSheet.Rows.Count > 0)
									{

										for (int j = 0; j <= fpSpread2.ActiveSheet.Rows.Count - 1; j++)
										{

											strQuery = string.Empty;
											strReq_YN = string.Empty;

											if (fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "필수여부")].Text.ToString() == "True")
												strReq_YN = "Y";
											else
												strReq_YN = "N";

											strQuery = "usp_TDA002 ";
											strQuery = strQuery + " @pTYPE = 'I1' ";
											strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
											strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
											strQuery = strQuery + ",@pTARGET_KEY ='" + fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString() + "' ";
											strQuery = strQuery + ",@pDOC_CD ='" + fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "문서코드")].Text.ToString() + "' ";
											strQuery = strQuery + ",@pDOC_REQ_YN ='" + strReq_YN + "' ";
											strQuery = strQuery + ",@pTARGET_TYPE ='I' ";
											strQuery = strQuery + ",@pREG_ID ='" + SystemBase.Base.gstrUserID.ToString() + "' ";

											DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery, dbConn, Trans);
											ERRCode = dt.Rows[0][0].ToString();
											MSGCode = dt.Rows[0][1].ToString();

											if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

										}
									}

								}
							}
						}

					}

					// 공정
					iMasterCnt = 0;

					if (c1DockingTab1.SelectedTab.TabIndex == 1)
					{

						if (fpSpread3.ActiveSheet.Rows.Count > 0)
						{

							for (int i = 0; i <= fpSpread3.ActiveSheet.Rows.Count - 1; i++)
							{
								if (fpSpread3.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "선택")].Text.ToString() == "True")
								{

									iMasterCnt++;

									if (fpSpread2.ActiveSheet.Rows.Count > 0)
									{

										for (int j = 0; j <= fpSpread2.ActiveSheet.Rows.Count - 1; j++)
										{

											strQuery = string.Empty;
											strReq_YN = string.Empty;

											if (fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "필수여부")].Text.ToString() == "True")
												strReq_YN = "Y";
											else
												strReq_YN = "N";

											strQuery = "usp_TDA002 ";
											strQuery = strQuery + " @pTYPE = 'I1' ";
											strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
											strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
											strQuery = strQuery + ",@pTARGET_KEY ='" + fpSpread3.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "공정코드")].Text.ToString() + "' ";
											strQuery = strQuery + ",@pDOC_CD ='" + fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "문서코드")].Text.ToString() + "' ";
											strQuery = strQuery + ",@pDOC_REQ_YN ='" + strReq_YN + "' ";
											strQuery = strQuery + ",@pTARGET_TYPE ='R' ";
											strQuery = strQuery + ",@pREG_ID ='" + SystemBase.Base.gstrUserID.ToString() + "' ";

											DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery, dbConn, Trans);
											ERRCode = dt.Rows[0][0].ToString();
											MSGCode = dt.Rows[0][1].ToString();

											if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

										}
									}

								}
							}
						}

					}

					Trans.Commit();

					if (iMasterCnt > 0)
					{
						ERRCode = "OK";
					}
					else
					{
						ERRCode = "WR";
					}

					SearchExec();
					SearchDoc();
				}
				else 
				{
					ERRCode = "WR";
				}
				
			}
			catch (Exception e)
			{
				SystemBase.Loggers.Log(this.Name, e.ToString());
				Trans.Rollback();
				ERRCode = "ER";
				MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
			}
		Exit:
			dbConn.Close();

			if (ERRCode == "OK")
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else if (ERRCode == "ER")
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			this.Cursor = Cursors.Default; ;
				
		}
		#endregion

		#region 품목 선택
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread1.ActiveSheet.Rows.Count > 0)
			{
				strKey_I = fpSpread1.ActiveSheet.Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
				strType_I = "I";

				SearchDoc();
			}
		}
		#endregion

		#region 공정 선택
		private void fpSpread3_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread3.ActiveSheet.Rows.Count > 0)
			{
				strKey_R = fpSpread3.ActiveSheet.Cells[fpSpread3.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "공정코드")].Text.ToString();
				strType_R = "R";

				SearchDoc();
			}
		}
		#endregion

		#region 문서 조회
		private void SearchDoc()
		{
			string strQuery = string.Empty;

			if (c1DockingTab1.SelectedIndex == 0) // 품목 I
			{
				strQuery = "usp_TDA002 ";
				strQuery = strQuery + " @pTYPE = 'S3' ";
				strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pDOC_DEPT_CD ='" + cboDept.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pTARGET_kEY ='" + strKey_I + "' ";
				strQuery = strQuery + ",@pTARGET_TYPE ='" + strType_I + "' ";
			}
			else // 공정 R
			{
				strQuery = "usp_TDA002 ";
				strQuery = strQuery + " @pTYPE = 'S3' ";
				strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
				strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pDOC_DEPT_CD ='" + cboDept.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ",@pTARGET_kEY ='" + strKey_R + "' ";
				strQuery = strQuery + ",@pTARGET_TYPE ='" + strType_R + "' ";
			}

			UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
			fpSpread2.ActiveSheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
			fpSpread2.ActiveSheet.Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;

		}
		#endregion

		#region 부서필터 자동 조회
		private void cboDept_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (fpSpread2.Sheets[0].Rows.Count > 0 && (string.IsNullOrEmpty(strKey_I) == false || string.IsNullOrEmpty(strKey_R) == false))
				{
					SearchDoc();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "COMBOBOX CHANGE 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 텝 인덱스 변경시 이벤트
		private void c1DockingTab1_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				cboDept.SelectedIndex = 0;
				if (string.IsNullOrEmpty(strKey_I) == false || string.IsNullOrEmpty(strKey_R) == false)
				{
					SearchDoc();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TAB INDEX CHANGE 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region fpSpread1(품목), fpSpread2(문서), fpSpread3(공정) 기능 추가 작업

		#region  fpSpread2 CellClick
		private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			if (fpSpread2.Sheets[0].Rows.Count > 0)
			{
				int HeadCnt = 0;
				if (fpSpread2.Sheets[0].ColumnHeader.RowCount > 2)
				{
					HeadCnt = 2;
				}
				else if (fpSpread2.Sheets[0].ColumnHeader.RowCount > 1)
				{
					HeadCnt = 1;
				}

				if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).CellType != null)
				{
					if (e.ColumnHeader == true)
					{
						if (fpSpread2.Sheets[0].ColumnHeader.Cells[HeadCnt, e.Column].Text == "True")
						{
							fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = false;
							for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
								}

							}
						}
						else
						{
							fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = true;
							for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
								}
							}
						}
					}
					else 
					{
						fpSpread2_EditChange(e.Row);
					}
				}
			}
		}

		#endregion

		#region fpSpread2 데이타 수정시 U 플래그 등록
		private void fpSpread2_EditChange(int iRow)
		{
			try
			{
				UIForm.FPMake.fpChange(fpSpread2, iRow);
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region fpSpread3 그리드 설정
		private void SetFpSpread3(string strSql)
		{

			string Query3 = " usp_BAA004 'S3', @PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread3', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
			DataTable dt3 = SystemBase.DbOpen.TranDataTable(Query3);
			int G3RowCount = dt3.Rows.Count + 1;

			if (G3RowCount > 1)
			{
				G3Head1 = new string[G3RowCount];// 첫번째 Head Text
				G3Head2 = new string[G3RowCount];// 두번째 Head Text
				G3Head3 = new string[G3RowCount];// 세번째 Head Text
				G3Width = new int[G3RowCount];// Cell 넓이
				G3Align = new string[G3RowCount];// Cell 데이타 정렬방식
				G3Type = new string[G3RowCount];// CellType 지정
				G3Color = new int[G3RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
				G3Etc = new string[G3RowCount];
				G3HeadCnt = Convert.ToInt32(dt3.Rows[0][0].ToString());
				G3SEQ = new int[G3RowCount];// 키

				/********************1번째 숨김필드 정의******************/
				G3Head1[0] = "";
				if (Convert.ToInt32(dt3.Rows[0][0].ToString()) >= 1)
					G3Head2[0] = "";
				if (Convert.ToInt32(dt3.Rows[0][0].ToString()) >= 2)
					G3Head3[0] = "";
				G3Width[0] = 0;
				G3Align[0] = "";
				G3Type[0] = "";
				G3Color[0] = 0;
				G3Etc[0] = "";
				/********************1번째 숨김필드 정의******************/

				//####################그리드 Head 순번######################
				GHIdx3 = new string[G3RowCount - 1, 2];	// 그리드 Head Index 변수 길이
				//string OldHeadName2 = null;
				int OldHeadNameCount3 = 1;
				//####################그리드 Head 순번######################
				for (int i = 1; i < G3RowCount; i++)
				{
					G3Head1[i] = dt3.Rows[i - 1][1].ToString();
					if (Convert.ToInt32(dt3.Rows[i - 1][0].ToString()) >= 1)
						G3Head2[i] = dt3.Rows[i - 1][2].ToString();
					if (Convert.ToInt32(dt3.Rows[i - 1][0].ToString()) >= 2)
						G3Head3[i] = dt3.Rows[i - 1][3].ToString();

					G3Width[i] = Convert.ToInt32(dt3.Rows[i - 1][4].ToString());
					G3Align[i] = dt3.Rows[i - 1][5].ToString();
					G3Type[i] = dt3.Rows[i - 1][6].ToString();
					G3Color[i] = Convert.ToInt32(dt3.Rows[i - 1][7].ToString());
					G3Etc[i] = dt3.Rows[i - 1][8].ToString();

					G3SEQ[i] = Convert.ToInt32(dt3.Rows[i - 1][9].ToString());


					//####################그리드 Head 순번######################                            
					OldHeadNameCount3 = 1;
					GHIdx3[0, 0] = dt3.Rows[0][1].ToString().ToUpper();
					for (int k = 0; k < i - 1; k++)
					{
						if (dt3.Rows[i - 1][1].ToString().ToUpper() == GHIdx3[k, 0].ToUpper())
						{
							OldHeadNameCount3++;
						}
						else if (GHIdx3[k, 0].ToUpper().LastIndexOf("_") > 0 && dt3.Rows[i - 1][1].ToString().ToUpper() == GHIdx3[k, 0].ToUpper().Substring(0, GHIdx3[k, 0].ToUpper().LastIndexOf("_")))
						{
							OldHeadNameCount3++;
						}
					}

					if (OldHeadNameCount3 > 1)
					{
						GHIdx3[i - 1, 0] = dt3.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount3.ToString();	// 그리드 Head명
					}
					else
					{
						GHIdx3[i - 1, 0] = dt3.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
					}

					GHIdx3[i - 1, 1] = Convert.ToString(i);			    // 그리드 Head 위치
					//####################그리드 Head 순번######################
				}

				UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);

			}

		}
		#endregion

		#region  fpSpread3 CellClick
		private void fpSpread3_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			if (fpSpread3.Sheets[0].Rows.Count > 0)
			{
				int HeadCnt = 0;
				if (fpSpread3.Sheets[0].ColumnHeader.RowCount > 2)
				{
					HeadCnt = 2;
				}
				else if (fpSpread3.Sheets[0].ColumnHeader.RowCount > 1)
				{
					HeadCnt = 1;
				}

				if (fpSpread3.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).CellType != null)
				{
					if (e.ColumnHeader == true)
					{
						if (fpSpread3.Sheets[0].ColumnHeader.Cells[HeadCnt, e.Column].Text == "True")
						{
							fpSpread3.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = false;
							for (int i = 0; i < fpSpread3.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread3.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread3.Sheets[0].Cells[i, e.Column].Value = false;
								}

							}
						}
						else
						{
							fpSpread3.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = true;
							for (int i = 0; i < fpSpread3.Sheets[0].Rows.Count; i++)
							{
								if (fpSpread3.Sheets[0].Cells[i, e.Column].Locked == false)
								{
									fpSpread3.Sheets[0].Cells[i, e.Column].Value = true;
								}
							}
						}
					}
					else
					{
						//fpSpread3_EditChange(e.Row);
					}
				}
			}
		}

		#endregion

		#region fpSpread3 데이타 수정시 U 플래그 등록
		private void fpSpread3_EditChange(int iRow)
		{
			try
			{
				UIForm.FPMake.fpChange(fpSpread3, iRow);
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 마우스 오른쪽 클릭 처리

		#region 그리드 넓이 초기화
		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			//오른쪽 그리드 넓이 초기화
			if (MessageBox.Show(SystemBase.Base.MessageRtn("SY012"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{	//초기화하시겠습니까?
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				//cmd.CommandTimeout = 10000;
				try
				{
					string Query = " usp_BAA004 'S5' ";
					Query = Query + ", @pFORM_ID='" + this.Name.ToString() + "'";
					Query = Query + ", @pGRID_NAME='fpSpread3'";
					Query = Query + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
					Query = Query + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";

					cmd.CommandText = Query;
					cmd.ExecuteNonQuery();
					Trans.Commit();


				}
				catch//(Exception f)
				{
					Trans.Rollback();
					//RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
					//MessageBox.Show(RtnMsg);
				}

				try
				{
					// 초기화된 그리드 넓이 화면에 적용
					string Query2 = " usp_BAA004 'S3' ";
					Query2 = Query2 + ", @pFORM_ID='" + this.Name.ToString() + "'";
					Query2 = Query2 + ", @pGRID_NAME='fpSpread3'";
					Query2 = Query2 + ", @pIN_ID='" + SystemBase.Base.gstrUserID + "' ";
					Query2 = Query2 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
					DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query2);

					for (int i = 0; i < dt.Rows.Count; i++)
					{
						fpSpread3.Sheets[0].Columns[i + 1].Width = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
						G3Width[i + 1] = Convert.ToInt32(dt.Rows[i]["HEAD_WIDTH"].ToString());
					}
					// 초기화된 그리드 넓이 화면에 적용
				}
				catch { }

				dbConn.Close();
			}
		}
		#endregion

		#region SORT
		private void menuItem16_Click(object sender, System.EventArgs e)
		{
			if (fpSpread3.Sheets[0].GetColumnAllowAutoSort(0))
			{
				fpSpread3.Sheets[0].SetColumnAllowAutoSort(-1, false);
			}
			else
			{
				fpSpread3.Sheets[0].SetColumnAllowAutoSort(-1, true);
			}
		}
		#endregion

		#region 찾기
		private void menuItem21_Click(object sender, EventArgs e)
		{
			

			if (!frm.Created)
			{
				frm = new UIForm.FindText(fpSpread3);
				frm.ShowDialog();
			}
			else
			{
				frm.Activate();
			}
		}
		#endregion

		#region excel save
		private void menuItem19_Click(object sender, System.EventArgs e)
		{
			try
			{
				UIForm.FPMake.ExcelMake(fpSpread3, this.Text.ToString() + "_3");
				ExcelExe();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Excel 저장"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Grid Print
		private void menuItem20_Click(object sender, EventArgs e)
		{
			try
			{
				UIForm.FPMake.PrintMake(fpSpread3, this.Text);
				PrintExe();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Grid Print"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#endregion

		#region fpSpread1 - 품목조회 목록에서 체크박스 체크시 자동으로 문서목록도 조회 => 보류
		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			//try
			//{
			//    SearchDoc(fpSpread1.ActiveSheet.Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString(), "I");
			//}
			//catch (Exception f)
			//{
			//    SystemBase.Loggers.Log(this.Name, f.ToString());
			//    MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			//}
		}
		#endregion

		#endregion

	}
}
