#region 작성정보
/*********************************************************************/
// 단위업무명 : 부품출고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-22
// 작성내용 : 부품출고등록 관리
// 수 정 일 : 2014-08-06
// 수 정 자 : 최 용 준
// 수정내용 : 추적관리 관련 내용 추가
// 비    고 : 
/*********************************************************************/
#endregion


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;

namespace PC.PCC002
{
    public partial class PCC002 : UIForm.FPCOMM1
    {
		
        #region 변수선언

		public DataTable dtBRout = new DataTable();	// 바코드 출고 데이터 테이블
		string strBtn = "N";
        string strMQuery;
        bool SaveChk = false;
		bool bAutoOut = false;	// 일괄 출고 처리 여부. lot 추적 품목이고, true 이면 바코드출고/개별lot출고 안됨.
		DataTable dt = new DataTable();			// lot 분할 팝업 그리드 정보 데이터 테이블
		DataTable dtPrint = new DataTable();	// 바코드 인쇄용 데이터 테이블
		
		// 바코드 출력
		bool bPrintAll = false;

		// 정렬 칼럼
		int iSortIdx = 0;

		// 저장 작업한 row index
		int iCurrIdx = 0;

		// 정렬
		bool bAsc = true;

        #endregion

        #region 생성자
        public PCC002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load
        private void PCC002_Load(object sender, System.EventArgs e)
        {
            // 필수 확인
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'P002', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//작업장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "창고")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B032', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//창고
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "LOCATION")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='LOC', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//LOCATION
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            // 기본정보 바인딩
            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(6).ToShortDateString().Substring(0,10);
            dtpOutDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportDtFr.Text = "";
            dtpReportDtTo.Text = "";
			dtpOutDtFR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(0).ToShortDateString().Substring(0, 10);
			dtpOutDtTO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(0).ToShortDateString().Substring(0, 10);

			// DataTable 칼럼 디자인을 하지 않기위해 구조만 가져옴.
			dtBRout = SystemBase.DbOpen.NoTranDataTable("SELECT BAR_CODE,MVMT_NO,MVMT_SEQ,OUT_TRAN_NO,OUT_TRAN_SEQ,LOT_NO,OUT_QTY,PROC_SEQ FROM T_OUT_INFO WHERE BAR_CODE = ''");

			// 출고처리자 기본 설정
			txtPurDutyID.Value = SystemBase.Base.gstrUserID;
			txtPurDutyNM.Value = SystemBase.Base.gstrUserName;
			
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
			iSortIdx = 0;
			iCurrIdx = 0;
			bAsc = true;

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD;
            dtpSTART_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-6).ToShortDateString().Substring(0,10);
            dtpEND_DT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(6).ToShortDateString().Substring(0,10);
            dtpOutDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportDtFr.Text = "";
            dtpReportDtTo.Text = "";
            rdoNo.Checked = true;
            rdoIssueNo.Checked = true;
			dtpOutDtFR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(0).ToShortDateString().Substring(0, 10);
			dtpOutDtTO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(0).ToShortDateString().Substring(0, 10);

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

			// 출고처리자 기본 설정
			txtPurDutyID.Value = SystemBase.Base.gstrUserID;
			txtPurDutyNM.Value = SystemBase.Base.gstrUserName;

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
			bPrintAll = false;
			bAutoOut = false;
			dt.Clear();
			dtBRout.Clear();
			dtPrint.Clear();
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            DialogResult dsMsg;

            try
            {
				dt.Clear();
				dtBRout.Clear();
				bAutoOut = false;

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string Chk = "N";
                    if (rdoYes.Checked == true)
                        Chk = "Y";
                    else if (rdoAll.Checked == true)
                        Chk = "";

                    string IssueChk = "";
                    if (rdoIssueYes.Checked == true)
                        IssueChk = "A";
                    else if (rdoIssueNo.Checked == true)
                        IssueChk = "M";

                    string ClChk = "";
                    if (rdoClyes.Checked == true)
                    {
                        ClChk = "Y";
                    }
                    else if (rdoClno.Checked == true)
                    {
                        ClChk = "N";
                    }

                    string ReportChk = "N";
                    if (chkReport.Checked == true)
                    {
                        ReportChk = "Y";
                    }

                    strMQuery = " usp_PCC002 'S1'";
                    strMQuery += ", @pPLANT_CD='" + txtPlant_CD.Text + "'";
                    strMQuery += ", @pSTART_DT='" + dtpSTART_DT.Text.ToString() + "'";
                    strMQuery += ", @pEND_DT='" + dtpEND_DT.Text.ToString() + "'";
                    strMQuery += ", @pITEM_CD='" + txtITEM_CD.Text.Trim() + "'";
                    strMQuery += ", @pWORKORDER_NO_FR ='" + txtWoNoFr.Text + "'";
                    strMQuery += ", @pWORKORDER_NO_TO ='" + txtWoNoTo.Text + "'";
                    strMQuery += ", @pSL_CD='" + txtSSL_CD.Text + "'";
                    strMQuery += ", @pPROJECT_NO='" + txtProject_No.Text + "'";
                    strMQuery += ", @pPROJECT_SEQ ='" + txtProject_Seq.Text + "'";
                    strMQuery += ", @pGROUP_CD='" + txtGroup_CD.Text + "'";
                    strMQuery += ", @pWC_CD='" + txtSWc_CD.Text.Trim() + "'";
                    strMQuery += ", @pISSUED_FLAG ='" + Chk + "'";
                    strMQuery += ", @pBIZ_CD ='" + SystemBase.Base.gstrBIZCD + "'";
                    strMQuery += ", @pISSUED_MTHD ='" + IssueChk + "'";
                    strMQuery += ", @pCLOSE_YN ='" + ClChk + "'";
                    strMQuery += ", @pREPORT_YN = '" + ReportChk + "' ";
                    strMQuery += ", @pREPORT_DT_FR ='" + dtpReportDtFr.Text.ToString() + "'";
                    strMQuery += ", @pREPORT_DT_TO ='" + dtpReportDtTo.Text.ToString() + "'";
                    strMQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strMQuery += ", @pPO_NO_FR ='" + txtPoNoFr.Text + "'";
                    strMQuery += ", @pPO_NO_TO ='" + txtPoNoTo.Text + "'";
                    strMQuery += ", @pPRNT_ITEM_CD = '" + txtPrntItemCd.Text + "'";
                    strMQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strMQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 3);

					// 기존 정렬 정보 유지
					if (iSortIdx != 0 && fpSpread1.Sheets[0].Rows.Count > 0)
					{
						if (fpSpread1.Sheets[0].GetColumnAllowAutoSort(0))
						{
							fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, false);
						}
						else
						{
							fpSpread1.Sheets[0].SetColumnAllowAutoSort(-1, true);
						}

						fpSpread1.Sheets[0].SortRows(iSortIdx, bAsc, true);

						if (iCurrIdx >= fpSpread1.Sheets[0].Rows.Count)
						{
							iCurrIdx = fpSpread1.Sheets[0].Rows.Count;
						}

						if (iCurrIdx <= 1)
						{
							fpSpread1.SetViewportTopRow(0, iCurrIdx);
						}
						else
						{
							fpSpread1.SetViewportTopRow(0, iCurrIdx - 1);
						}

						SetMultiSort();						
					}

                    for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
                    {

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
								);
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|0"
									);
						}

                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value) <= 0)
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3"
								);
						}
						else
                        {
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text == "자동")
							{
								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
										SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|0"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|0"
										);
								}
								else 
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3"
									);
								}
							}
							else
							{
								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
										SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|0"
										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|0"
										);
								}
								else
								{
									UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3"
									);
								}
							}

                        }

                        if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value) > Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value))
                        {
                            fpSpread1.Sheets[0].Rows[i].ForeColor = Color.Red;

							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
																SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|0"
																+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
																+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3");
							}
							else
							{
								UIForm.FPMake.grdReMake(fpSpread1, i,
									SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|0"
									+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3");
							}
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "오더상태")].Text == "CL")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "선택") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "LOT NO_2") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고") + "|3");
                        }

						if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text) == false &&
							fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0");
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3");
						}

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

			iCurrIdx = fpSpread1.Sheets[0].ActiveRowIndex;

            fpSpread1.Focus();

            if (SaveChk == true)
            {
                return;
            }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                //그리드 상단 필수 체크
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))
                {        
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
					string TranNo = string.Empty, MovTranNo = string.Empty, strSql = string.Empty;
					int TranSeq = 0;

                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    case "D": strGbn = "D1"; break;
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text) &&
									fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True" &&
									bAutoOut == false)
                                {
                                    Trans.Rollback();
									TranNo = ""; 
									MovTranNo = ""; 
									TranSeq = 0;
                                    dbConn.Close();
                                    continue;
                                }

								if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text == ""
									|| fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value.ToString() == "0")
								{
									Trans.Rollback();
									TranNo = "";
									MovTranNo = "";
									TranSeq = 0;
									dbConn.Close();
									continue;
								}

                                strSql = " usp_PCC002 @pTYPE = '" + strGbn + "'";
                                strSql += ", @pCHILD_ITEM_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목순서")].Text + "'";
                                strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "'";
                                strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text + "'";
                                strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "'";
                                strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + "'";
                                strSql += ", @pREQ_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "필요수량")].Value + "'";
                                strSql += ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                                strSql += ", @pOUT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value + "'";
                                strSql += ", @pISSUED_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기출고수량")].Value + "'";
                                strSql += ", @pOUT_DT = '" + dtpOutDt.Text + "'";
                                strSql += ", @pSL_CD ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Value + "'";
                                strSql += ", @pWC_CD ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업장")].Value + "'";
                                strSql += ", @pPROJECT_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'";
                                strSql += ", @pPROJECT_SEQ ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'";
                                strSql += ", @pTRAN_NO = '" + TranNo + "'";
                                strSql += ", @pMOV_TRAN_NO = '" + MovTranNo + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pDEPT_CD = '" + SystemBase.Base.gstrDEPT + "'";
                                strSql += ", @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "'";
                                strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
								strSql += ", @pBAR_CODE ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Value + "'";
								strSql += ", @pMVMT_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value + "'";
								strSql += ", @pMVMT_SEQ ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value + "'";
								strSql += ", @pLOT_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value + "'";
								strSql += ", @pREMARK ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 출고 비고")].Value + "'";
								
								// 일괄 자동 lot 출고 여부
								if (bAutoOut)
									strSql += ", @pAUTO_LOT_SAVE ='Y'";
								else
									strSql += ", @pAUTO_LOT_SAVE ='N'";


                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
								MSGCode = ds.Tables[0].Rows[0][1].ToString();
								
								if (string.Compare(ERRCode, "OK", true) == 0)
                                {
									int iVal = 0;
									if (string.IsNullOrEmpty(ds.Tables[0].Rows[0][2].ToString()) == true || ds.Tables[0].Rows[0][4].ToString() == "0" || int.TryParse(ds.Tables[0].Rows[0][4].ToString(), out iVal) == false)
									{
										Trans.Rollback();
										TranNo = ""; 
										MovTranNo = ""; 
										TranSeq = 0;
										goto Exit;
									}

                                    TranNo = ds.Tables[0].Rows[0][2].ToString();
                                    MovTranNo = ds.Tables[0].Rows[0][3].ToString();
									TranSeq = Convert.ToInt32(ds.Tables[0].Rows[0][4]);

									//------------------------------------------------------------------------------------------------------------------------------
									// Lot 출고 처리 : 같은 출고건에 대하여 바코드 출고 및 개별 수동 출고 데이터가 동시에 존재하면 개별 수동 출고 데이터 우선 처리
									//------------------------------------------------------------------------------------------------------------------------------
									
									// 개별 수동 출고
									if (dt.Rows.Count > 0) // 신규등록
									{
										for (int j = 0; j <= dt.Rows.Count - 1; j++)
										{

											if (Convert.ToDecimal(dt.Rows[j]["OUT_QTY"]) > 0)
											{
												strSql = "usp_T_OUT_INFO_CUDR ";
												strSql += "  @pTYPE        = 'I1'";
												strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
												strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
												strSql += ", @pBAR_CODE    = '" + dt.Rows[j]["BAR_CODE"].ToString() + "' ";
												strSql += ", @pMVMT_NO     = '" + dt.Rows[j]["MVMT_NO"].ToString() + "' ";
												strSql += ", @pMVMT_SEQ    = '" + dt.Rows[j]["MVMT_SEQ"].ToString() + "' ";

												if (string.IsNullOrEmpty(dt.Rows[j]["OUT_TRAN_NO"].ToString()))
												{
													strSql += ", @pOUT_TRAN_NO = '" + TranNo + "' ";
													strSql += ", @pOUT_TRAN_SEQ= '" + TranSeq + "' ";
												}
												else
												{
													strSql += ", @pOUT_TRAN_NO = '" + dt.Rows[j]["OUT_TRAN_NO"].ToString() + "' ";
													strSql += ", @pOUT_TRAN_SEQ= '" + dt.Rows[j]["OUT_TRAN_SEQ"].ToString() + "' ";
												}

												strSql += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + "' ";
												strSql += ", @pTR_TYPE     = 'O' ";
												strSql += ", @pOUT_DATE    = NULL ";
												strSql += ", @pLOT_NO      = '" + dt.Rows[j]["LOT_NO"].ToString() + "' ";
												strSql += ", @pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
												strSql += ", @pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
												strSql += ", @pOUT_QTY     = '" + dt.Rows[j]["OUT_QTY"].ToString() + "' ";
												strSql += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
												strSql += ", @pREMARK      = '' ";
												strSql += ", @pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pPROC_SEQ	   ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "'";
												strSql += ", @pOUT_WORKORDER_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "'";

												DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
												ERRCode = ds4.Tables[0].Rows[0][0].ToString();
												MSGCode = ds4.Tables[0].Rows[0][1].ToString();
												if (ERRCode != "OK") { Trans.Rollback(); TranNo = ""; MovTranNo = ""; TranSeq = 0; goto Exit; }	// ER 코드 Return시 점프
											}
										}
									}

									// 바코드 출고
									if (dtBRout.Rows.Count > 0) // 신규등록
									{
										for (int j = 0; j <= dtBRout.Rows.Count - 1; j++)
										{

											if (
												Convert.ToDecimal(dtBRout.Rows[j]["OUT_QTY"]) > 0 &&
												string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text, dtBRout.Rows[j]["WORKORDER_NO"].ToString(), true) == 0 &&
												string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, " 부품")].Text, dtBRout.Rows[j]["ITEM_CD"].ToString(), true) == 0
											   )
											{
												strSql = "usp_T_OUT_INFO_CUDR ";
												strSql += "  @pTYPE        = 'I1'";
												strSql += ", @pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
												strSql += ", @pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
												strSql += ", @pBAR_CODE    = '" + dtBRout.Rows[j]["BAR_CODE"].ToString() + "' ";
												strSql += ", @pMVMT_NO     = '" + dtBRout.Rows[j]["MVMT_NO"].ToString() + "' ";
												strSql += ", @pMVMT_SEQ    = '" + dtBRout.Rows[j]["MVMT_SEQ"].ToString() + "' ";

												if (string.IsNullOrEmpty(dtBRout.Rows[j]["OUT_TRAN_NO"].ToString()))
												{
													strSql += ", @pOUT_TRAN_NO = '" + TranNo + "' ";
													strSql += ", @pOUT_TRAN_SEQ= '" + TranSeq + "' ";
												}
												else
												{
													strSql += ", @pOUT_TRAN_NO = '" + dtBRout.Rows[j]["OUT_TRAN_NO"].ToString() + "' ";
													strSql += ", @pOUT_TRAN_SEQ= '" + dtBRout.Rows[j]["OUT_TRAN_SEQ"].ToString() + "' ";
												}

												strSql += ", @pITEM_CD     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + "' ";
												strSql += ", @pTR_TYPE     = 'O' ";
												strSql += ", @pOUT_DATE    = NULL ";
												strSql += ", @pLOT_NO      = '" + dtBRout.Rows[j]["LOT_NO"].ToString() + "' ";
												strSql += ", @pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
												strSql += ", @pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
												strSql += ", @pOUT_QTY     = '" + dtBRout.Rows[j]["OUT_QTY"].ToString() + "' ";
												strSql += ", @pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
												strSql += ", @pREMARK      = '' ";
												strSql += ", @pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
												strSql += ", @pPROC_SEQ	   ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정")].Text + "'";
												strSql += ", @pOUT_WORKORDER_NO ='" + dtBRout.Rows[j]["WORKORDER_NO"].ToString() + "'";

												DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
												ERRCode = ds3.Tables[0].Rows[0][0].ToString();
												MSGCode = ds3.Tables[0].Rows[0][1].ToString();
												if (ERRCode != "OK") { Trans.Rollback(); TranNo = ""; MovTranNo = ""; TranSeq = 0; goto Exit; }	// ER 코드 Return시 점프
											}
										}
									}
									
									//----------------------------------------------------------------------------------------------------------------

                                }
                                else
                                {
                                    TranNo = "";
                                    MovTranNo = "";
									TranSeq = 0;
                                }

								if (ERRCode != "OK") { Trans.Rollback(); TranNo = ""; MovTranNo = ""; TranSeq = 0; goto Exit; }	// ER 코드 Return시 점프

                            }
                            else
                            {
                                Trans.Rollback();
								TranNo = ""; 
								MovTranNo = ""; 
								TranSeq = 0;
                                dbConn.Close();
                                continue;
                            }

                            Trans.Commit();
							dt.Clear();
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
							TranNo = ""; 
							MovTranNo = ""; 
							TranSeq = 0;
                            ERRCode = "ER";
                            MSGCode = e.Message;
                        }
                    Exit:

						if (ERRCode == "ER" || ERRCode == "WR")
                        {
							if (string.IsNullOrEmpty(MSGCode) == true)
							{
								MSGCode = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " / " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + " : 출고처리중 문제가 발생했습니다.";
							}

							SystemBase.Loggers.Log(this.Name, "부품출고 오류 : " + MSGCode);

							//if (MSGCode.IndexOf("재고") > 0 && MSGCode.IndexOf("부족") > 0)
							//{
								
							//}
							//else 
							//{
							//    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
							//}

							//if (MSGCode.IndexOf("트랜잭션") > 0)
							//{
							//    MessageBox.Show( fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " / " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + " : " + MSGCode);
							//}
                        }
						//else if (ERRCode == "WR")
						//{
						//    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
						//}

						dbConn.Close();
												
                    }

                    if (ERRCode == "OK")
                    {
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    SearchExec();

                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        #region fpSpread1_Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            DialogResult dsMsg;
            //출고수량 check
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량"))
            {
                decimal out_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);
                decimal rest_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value);
                decimal on_qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value);

                if (out_qty > on_qty)
                {
                    dsMsg = MessageBox.Show(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text
                                            + " - 출고수량은 양품재고수량보다 많을 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Column);
                    SaveChk = true;
                }
                else if (out_qty > rest_qty)
                {
                    dsMsg = MessageBox.Show(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + " : "
                                            + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text
                                            + " - 출고수량은 출고잔량보다 많을 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.ActiveSheet.SetActiveCell(Row, Column);
                    SaveChk = true;
                }
                else
                {
                    SaveChk = false;
                }

            }
        }
        #endregion

        #region 버튼 Click
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";								// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };	// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlant_CD.Text = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                    txtITEM_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrntItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPrntItemCd.Text = Msgs[2].ToString();
                    txtPrntItemNm.Value = Msgs[3].ToString();
                    txtPrntItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGroupCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtGroup_CD.Text = Msgs[2].ToString();
                    txtGROUP_NM.Value = Msgs[3].ToString();
                    txtGroup_CD.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "C");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Name.Value = Msgs[4].ToString();
                    txtProject_Seq.Text = Msgs[5].ToString();
                    txtGroup_CD.Text = Msgs[6].ToString();
                    txtGROUP_NM.Value = Msgs[7].ToString();

					txtCProjectNo.Value = txtProject_No.Value;
					txtCProjectNM.Value = txtProject_Name.Value;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtSWc_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", true);
                pu.Width = 500;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSWc_CD.Text = Msgs[0].ToString();
                    txtSWc_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSSL_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='B035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + txtPlant_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSSL_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSSL_CD.Text = Msgs[0].ToString();
                    txtSSL_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWoNoFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoFr.Text = Msgs[1].ToString();
                    txtWoNoFr.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWoNoTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWoNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWoNoTo.Text = Msgs[1].ToString();
                    txtWoNoTo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSL_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='B035', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = '" + txtPlant_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSL_CD.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSL_CD.Text = Msgs[0].ToString();
                    txtSL_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();

                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "외주거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        // 작업장
        private void txtSWc_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSWc_CD.Text != "")
                {
                    txtSWc_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSWc_CD.Text, "AND MAJOR_CD = 'P002' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSWc_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 부품
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 공장
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlant_CD.Text != "")
                {
                    txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlant_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 창고
        private void txtSSL_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSSL_CD.Text != "")
                {
                    txtSSL_NM.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSSL_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSSL_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        // 제품코드
        private void txtGroup_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtGroup_CD.Text != "")
                {
                    txtGROUP_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtGroup_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtGROUP_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtSL_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSL_CD.Text != "")
                {
                    txtSL_NM.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSL_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtSL_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtPrntItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPrntItemCd.Text != "")
                {
                    txtPrntItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtPrntItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPrntItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCustCd.Text != "")
                {
                    txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtCustNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 일괄선택 & 일괄취소
        private void btnSelectAll_Click(object sender, System.EventArgs e)
        {

			if (fpSpread1.Sheets[0].Rows.Count == 0)
			{
				return;
			}


			DialogResult result = MessageBox.Show("일괄선택 출고시에는 바코드출고로 지정된 데이터는 취소됩니다. \r\n일괄선택 출고를 진행하시겠습니까?", "확인", MessageBoxButtons.YesNoCancel);

			if (result == DialogResult.Yes)
			{
				bAutoOut = true;
				dtBRout.Clear();
			}
			else 
			{
				return;
			}

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value.ToString()) > 0
                    && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value.ToString()) > 0)
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value.ToString())
                        >= Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value.ToString()))
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value
                            = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value;
                    else
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value
                            = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "양품재고수량")].Value;

                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고방법")].Text == "수동")
                    {
                        UIForm.FPMake.fpChange(fpSpread1, i);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text = "True";
                    }

					//if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text == "True")
					//{
					//    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;
					//    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text = "False";
					//}
                }
            }
        }

        private void btnSelectCancel_Click(object sender, System.EventArgs e)
        {
			bAutoOut = false;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = 0;
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text = "False";
            }
        }
        #endregion

		#region 출고확정서
		private void btnConfirmReport_Click(object sender, EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				string RptName = "";
				string[] RptParmValue = new string[7];   // SP PARAMETER

				string strOrderBy = string.Empty;
				if (rdoObyItem.Checked == true)
				{
					strOrderBy = "I";
				}
				else
				{
					strOrderBy = "W";
				}

				RptName = SystemBase.Base.ProgramWhere + @"\Report\PCC002_RPT.rpt";    // 레포트경로+레포트명

				RptParmValue[0] = "S1";
				RptParmValue[1] = SystemBase.Base.gstrCOMCD;
				RptParmValue[2] = txtCProjectNo.Text;					
				RptParmValue[3] = dtpOutDtFR.Text;
				RptParmValue[4] = dtpOutDtTO.Text; 
				RptParmValue[5] = txtPurDutyID.Text;
				RptParmValue[6] = strOrderBy;

				UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue); //공통크리스탈 10버전				
				frm.ShowDialog();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY081"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
			
		}
		#endregion

		#region 바코드 일괄출력
		private void btnPrintAll_Click(object sender, EventArgs e)
		{
			try
			{

				if (cboPort.SelectedText == "선택")
				{
					MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				DialogResult result = MessageBox.Show("바코드 일괄 출력을 진행하시겠습니까?\r\n이 작업에는 많은 시간이 소요될 수 있습니다.", "확인",  MessageBoxButtons.YesNoCancel);
				
				if (result == DialogResult.Yes)
				{
					bPrintAll = true;
					PrintBarCode(-1);
				}
				
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY082"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Grid Button Click Event
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{

			decimal dSum = 0;
			decimal dOutQty = 0;
			string strLotNo = string.Empty;
			int iLotCount = 0;
			
			try
			{
				
				if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2"))
				{

					if (bAutoOut == true)
					{
						MessageBox.Show("일괄선택 출고시에는 개별출고를 할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					PCC002P1 pu = new PCC002P1();

					pu.dtBarCD = dtBRout;

					pu.bReadOnlyYN = true;
					pu.strPLANT_CD = SystemBase.Base.gstrPLANT_CD;
					pu.strPROJECT_NO = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Value.ToString();
					pu.strPROJECT_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value.ToString();
					pu.strMVMT_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value.ToString();
					pu.strITEM_CD = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Value.ToString();
					pu.strITEM_NM = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품명")].Value.ToString();
					pu.strITEM_SPEC = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품규격")].Value.ToString();
					pu.strREM_QTY = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고잔량")].Value.ToString();
					pu.strTRAN_NO = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고번호")].Value.ToString();
					pu.strTRAN_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Value.ToString();
					pu.strWORKORDER_NO = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Value.ToString();
					pu.strPROC_SEQ = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Value.ToString();
					pu.strOUT_QTY = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value.ToString();

					pu.ShowDialog();

					if (pu.DialogResult == DialogResult.OK)
					{

						if (pu.dt != null && pu.dt.Rows.Count > 0)
						{
							dt.Clear();
							dt = pu.dt;

							// 단일 lot no 구분
							for (int i = 0; i <= pu.dt.Rows.Count - 1; i++)
							{
								if (pu.dt.Rows[0]["OUT_QTY"] == DBNull.Value) { pu.dt.Rows[0]["OUT_QTY"] = 0; }
								dSum += Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]);

								if (Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]) > 0)
								{
									iLotCount++;
									dOutQty = Convert.ToDecimal(pu.dt.Rows[i]["OUT_QTY"]);
									strLotNo = pu.dt.Rows[i]["LOT_NO"].ToString();
								}
							}

							if (iLotCount == 1)
							{
								fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = strLotNo;
								fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dOutQty;
							}
							else
							{
								fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할";
								fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = dSum;
							}
						}

					}

					// 팝업화면에서 변경사항이 적용되면 Parent Form Reload
					if (pu.strSaveYN == "Y")
					{

						// LOT 수량과 기출고수량 비교
						if (pu.dLotSum == Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 출고수량")].Value))
						{
						
						}
						else
						{
							fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = pu.dLotSum;
							fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";
							fpSpread1.Sheets[0].Cells[e.Row, 1].Value = "True";

							for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
							{
								if (i != e.Row)
								{
									fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
									fpSpread1.Sheets[0].Cells[i, 1].Value = "False";
								}
							}

							SetBarCodeDT();
							SaveExec();
						}

					}

					//if (iLotCount > 1 && pu.strSaveYN == "Y") { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "Lot 분할"; }

					pu.strSaveYN = string.Empty;
					pu.dLotSum = 0;

				}
				else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력"))
				{
					if (cboPort.SelectedText == "선택")
					{
						MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					bPrintAll = false;
					PrintBarCode(e.Row);
					fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "Lot 정보조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 팝업창에서 출고 데이터가 조정되면 바코드 처리 데이터테이블에 있는 동일한 건을 삭제한다.
		private void SetBarCodeDT()
		{
			if (dtBRout.Rows.Count > 0 && dt.Rows.Count > 0)
			{
				for (int i = 0; i <= dt.Rows.Count - 1; i++)
				{
					for (int j = dtBRout.Rows.Count - 1; j >= 0; j--)
					{
						if (
							string.Compare(dt.Rows[i]["BAR_CODE"].ToString(), dtBRout.Rows[j]["BAR_CODE"].ToString(), true) == 0 &&
							string.Compare(dt.Rows[i]["LOT_NO"].ToString(), dtBRout.Rows[j]["LOT_NO"].ToString(), true) == 0 &&
							string.Compare(dt.Rows[i]["MVMT_SEQ"].ToString(), dtBRout.Rows[j]["MVMT_SEQ"].ToString(), true) == 0 &&
							Convert.ToDecimal(dt.Rows[i]["OUT_QTY"]) > 0
						   )
						{
							dtBRout.Rows.RemoveAt(j);
						}
					}
				}
			}
		}
		#endregion

		#region 예상 재고수량 자동 계산
		private void fpSpread1_EditModeOff(object sender, EventArgs e)
		{

			DialogResult dsMsg;

			try
			{
				if (fpSpread1.Sheets[0].ActiveColumnIndex == SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량") &&
					string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
				{
					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Text) == false &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT 재고수량")].Text) == false)
					{

						fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "예상재고수량")].Value =
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT 재고수량")].Value) -
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value);

						if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT 재고수량")].Value) -
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value) <= 0)
						{
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Value = 0;
						}

						if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT 재고수량")].Value) -
							Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value) < 0)
						{
							dsMsg = MessageBox.Show(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + " : "
											+ fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + " : "
											+ fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text
											+ " - 출고수량은 LOT 재고수량보다 많을 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

							fpSpread1.ActiveSheet.Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = DBNull.Value;
							fpSpread1.ActiveSheet.Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "예상재고수량")].Value =
								fpSpread1.ActiveSheet.Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "LOT 재고수량")].Value;
							SaveChk = true;

						}
						else
						{
							SaveChk = false;
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 바코드 인쇄
		private void PrintBarCode(int row)
		{
			string strZPL = string.Empty;

			int X = 15;
			int Y = 5;

			GetPrintData(row);

			if (dtPrint.Rows.Count > 0)
			{

				for (int i = 0; i <= dtPrint.Rows.Count - 1; i++)
				{

					strZPL = "";

					strZPL += "^XA";					// start format

					strZPL += "^LL176";					// label hight
					strZPL += "^PW560";					// print length

					strZPL += "^LS0";					// print length
					strZPL += "^LH5,5";					// label home location - 최초 시작 위치(x, y)

					strZPL += "^FO" + (X + 5) + "," + (Y + 10) + "^BY1.3,0.5,110^BCN,110,Y,N,N^FD" + dtPrint.Rows[i]["BAR_CODE"].ToString() + "^FS";	//^BC:Code 128(USD-6)체계
					strZPL += "^FO" + (X + 250) + "," + (Y + 10) + "^AC,14,14^FDQ'ty^FS" + "^FO" + (X + 332) + "," + (Y + 10) + "^AC,14,14^FD:" + SetConvert(Convert.ToDecimal(dtPrint.Rows[i]["OUT_QTY"].ToString())) + " "
																																				+ dtPrint.Rows[i]["STOCK_UNIT"].ToString() + "^FS";
					strZPL += "^FO" + (X + 250) + "," + (Y + 35) + "^AC,14,14^FDLot^FS" + "^FO" + (X + 295) + "," + (Y + 35) + "^AC,14,14^FD No:" + dtPrint.Rows[i]["LOT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 250) + "," + (Y + 60) + "^AC,14,14^FDCode^FS" + "^FO" + (X + 295) + "," + (Y + 60) + "^AC,14,14^FD No:" + dtPrint.Rows[i]["ITEM_CD"].ToString() + "^FS";
					strZPL += "^FO" + (X + 250) + "," + (Y + 85) + "^AC,14,14^FDW/O^FS" + "^FO" + (X + 295) + "," + (Y + 85) + "^AC,14,14^FD No:" + dtPrint.Rows[i]["WORKORDER_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 250) + "," + (Y + 110) + "^AC,14,14^FDUser^FS" + "^FO" + (X + 295) + "," + (Y + 110) + "^AC,14,14^FD ID:" + SystemBase.Base.gstrUserID + "^FS";

					if (row == -1) // 일괄
					{
						strZPL += "^PQ1^FS";	// 라벨 인쇄 매수
					}
					else
					{
						if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text) == true)
						{
							strZPL += "^PQ1^FS";	// 라벨 인쇄 매수
						}
						else
						{
							strZPL += "^PQ" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text + "^FS";	// 개별
						}
					}

					
					strZPL += "^XZ";		// end format


					if (string.Compare(cboPort.SelectedText.Substring(0, 3), "LPT", true) == 0)
					{
						if (SystemBase.RawPrinterHelper.SendStringToPrinter("LPT1", strZPL) == false)
						{
							throw new Exception("바코드 발행 중 오류가 발생했습니다.");
						}
					}
					else
					{
						if (SystemBase.RawPrinterHelper.PrintZPL(cboPort.SelectedText, strZPL) == false)
						{
							throw new Exception("바코드 발행 중 오류가 발생했습니다.");
						}
					}

				}
			}
		}
		#endregion

		#region 바코드 정보 조회
		private void GetPrintData(int row)
		{
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();

			dtPrint.Clear();

			/*
			바코드, 출고수량, LOT NO, 품목코드, 제조오더번호  
			*/

			string strSql = string.Empty;

			if (row == -1) // 일괄
			{
				strSql = " usp_T_IN_INFO_CUDR ";
				strSql += "  @pTYPE			= 'P2'";
				strSql += ", @pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql += ", @pPLANT_CD		= '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strSql += ", @pPROJECT_NO	= '" + txtCProjectNo.Text + "' ";
				strSql += ", @pTRAN_DT_FR	= '" + dtpOutDtFR.Text + "' ";
				strSql += ", @pTRAN_DT_TO	= '" + dtpOutDtTO.Text + "' ";
				strSql += ", @pUP_ID		= '" + txtPurDutyID.Text + "' ";
				strSql += ", @pALL_ONE		= 'A' ";
			}
			else // 개별
			{
				strSql = " usp_T_IN_INFO_CUDR ";
				strSql += "  @pTYPE			= 'P2'";
				strSql += ", @pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql += ", @pPLANT_CD		= '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strSql += ", @pWORKORDER_NO	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
				strSql += ", @pPROC_SEQ	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
				strSql += ", @pITEM_CD	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "부품")].Text + "' ";
				strSql += ", @pALL_ONE		= 'E' ";
			}

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strSql);
		}
		#endregion

		#region 수량 형식 변경
		private string SetConvert(decimal dNumber)
		{
			string strReturn = string.Empty;

			strReturn = double.Parse(dNumber.ToString()).ToString();

			return strReturn;
		}
		#endregion

		#region 바코드 출고
		private void btnBarCodeOut_Click(object sender, EventArgs e)
		{
			try
			{

				if (rdoNo.Checked == false)
				{
					MessageBox.Show("바코드 출고는 출고완료여부 값이 '미출고'여야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				if (bAutoOut == true)
				{
					MessageBox.Show("일괄선택 출고시에는 \r\n바코드출고를 할 수 없습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					PCC002P2 pu = new PCC002P2(fpSpread1);

					pu.dtBarCD = dtBRout;

					pu.ShowDialog();

					if (pu.DialogResult == DialogResult.OK)
					{

						if (dtBRout.Rows.Count > 0)
						{
							for (int i = 0; i <= pu.dtOutInfo.Rows.Count - 1; i++)
							{
								DataRow dr = pu.dtOutInfo.Rows[i];
								dtBRout.Rows.Add(dr.ItemArray);
							}
						}
						else
						{
							dtBRout = pu.dtOutInfo;
						}

					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Logger.StaticLog(f.Message);
				MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 프로젝트명 자동 조회
		private void txtProject_No_TextChanged(object sender, EventArgs e)
		{
			if (txtProject_No.Text != "")
			{
				if (txtProject_No.Text != "")
				{
					txtProject_Name.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
				}
				else
				{
					txtProject_Name.Value = "";
				}
			}
			else
			{
				txtProject_Name.Value = "";
				txtProject_Seq.Text = "";
			}
		}
		#endregion

		#region 프린터 포트 저장
		private void cboPort_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(cboPort.SelectedText) == false && cboPort.SelectedText != "선택")
				{
					SystemBase.RawPrinterHelper.SavePrinterPort(cboPort.SelectedText);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 출고처리자 조회
		private void btnPurDuty_Click(object sender, EventArgs e)
		{
			strBtn = "Y";
			try
			{
				string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtPurDutyID.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtPurDutyID.Text = Msgs[0].ToString();
					txtPurDutyNM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
			strBtn = "N";
		}

		private void txtPurDutyID_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (strBtn == "N" && txtPurDutyID.Text.Trim() != "")
				{
					string temp = "";
					temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDutyID.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
					if (temp != "")
					{
						if (txtPurDutyID.Text != "")
						{
							txtPurDutyNM.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDutyID.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
						}
						else
						{
							txtPurDutyNM.Value = "";
						}
					}
				}
				else if (txtPurDutyID.Text.Trim() == "") txtPurDutyNM.Value = "";
			}
			catch(Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 프로젝트 조회
		private void btnCProject_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW003 pu = new WNDW003(txtCProjectNo.Text, "S1", "C");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtCProjectNo.Text = Msgs[3].ToString();
					txtCProjectNM.Value = Msgs[4].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtCProjectNo_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (txtProject_No.Text != "")
				{
					if (txtProject_No.Text != "")
					{
						txtProject_Name.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
					}
					else
					{
						txtProject_Name.Value = "";
					}
				}
				else
				{
					txtProject_Name.Value = "";
					txtProject_Seq.Text = "";
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 출고확정서 프로젝트 자동 입력
		private void txtProject_No_Leave(object sender, EventArgs e)
		{
			try
			{
				txtCProjectNo.Value = txtProject_No.Text;
				txtCProjectNM.Value = txtProject_Name.Text;
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show("프로젝트 선택중 오류가 발생헸습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 정렬 정보 조회
		private void fpSpread1_AutoSortingColumn(object sender, FarPoint.Win.Spread.AutoSortingColumnEventArgs e)
		{
			iSortIdx = e.Column;

			if (e.Ascending)
			{
				bAsc = true;
			}
			else
			{
				bAsc = false;
			}
			
		}
		#endregion

		#region 정렬을 못하게 하기 위한 설정
		private void fpSpread1_MouseDown(object sender, MouseEventArgs e)
		{
			//fpSpread1.ActiveSheet.SetColumnAllowAutoSort(0, true);
		}
		#endregion

		#region 제조오더번호 멀티 정렬
		private void SetMultiSort()
		{
			FarPoint.Win.Spread.SortInfo[] si = new FarPoint.Win.Spread.SortInfo[2];

			si[0] = new FarPoint.Win.Spread.SortInfo(iSortIdx, bAsc);
			si[1] = new FarPoint.Win.Spread.SortInfo(4, true);
			
			fpSpread1.ActiveSheet.SortRows(0, fpSpread1.ActiveSheet.RowCount, si); 
		}
		
		private void fpSpread1_AutoSortedColumn(object sender, FarPoint.Win.Spread.AutoSortedColumnEventArgs e)
		{
			SetMultiSort();
		}
		#endregion

	}
}
