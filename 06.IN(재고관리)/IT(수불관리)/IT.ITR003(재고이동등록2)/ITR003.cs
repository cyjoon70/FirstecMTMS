#region 작성정보
/*********************************************************************/
// 단위업무명 : 재고이동등록2
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 재고이동등록2 및 관리
// 수 정 일 : 2014-08-19
// 수 정 자 : 최 용 준
// 수정내용 : 추적관리 관련 내용 추가
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace IT.ITR003
{
    public partial class ITR003 : UIForm.FPCOMM2
    {

        #region 변수선언
        int NewFlg = 1;	//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoTranNo = string.Empty;	// 수불번호
		string strAutoTranSeq = "0";
        string strBtn = "N";
        bool btnNew_is = true;
        bool form_act_chk = false;
		bool bSave = true;

		// 바코드 출력 관련
		bool bPrintAll = false;
		DataTable dtPrint = new DataTable();	// 바코드 인쇄용 데이터 테이블
        #endregion

        #region 생성자
        public ITR003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ITR003_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

            G1Etc[13] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            G1Etc[31] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//재고단위
            
            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타세팅
            dtpSTranDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpSTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            txtProjectNo2.ReadOnly = true;          // 2015.03.23. hma 추가: 사용자가 키보드 입력 못하도록. 팝업으로만 선택하도록 하기 위함임.
            txtProjectSeq2.ReadOnly = true;         // 2015.03.23. hma 추가: 사용자가 키보드 입력 못하도록. 팝업으로만 선택하도록 하기 위함임.

            NewFlg = 1;
            strAutoTranNo = "";

            txtLocation2.Value = "*";
            txtLocationNm2.Value = "*";

        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            strBtn = "Y";

            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSTranDtFr.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
                dtpSTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }

            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);

            txtProjectNo2.ReadOnly = true;          // 2015.03.23. hma 추가: 사용자가 키보드 입력 못하도록. 팝업으로만 선택하도록 하기 위함임. 
            txtProjectSeq2.ReadOnly = true;         // 2015.03.23. hma 추가: 사용자가 키보드 입력 못하도록. 팝업으로만 선택하도록 하기 위함임.

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅

            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            NewFlg = 1;
            strAutoTranNo = "";

            strBtn = "N";

            txtLocation2.Value = "*";
            txtLocationNm2.Value = "*";

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

			bSave = true;
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            if (txtTranNo.Text.Trim() == "")
            {
                if (txtMoveType.Text.Trim() == "")
                {
                    MessageBox.Show("수불유형을 먼저 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMoveType.Focus();
                    return;
                }
                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;//0

                if (txtMoveType.Text == "T61" || txtMoveType.Text == "T65")
                {
                    UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex, 
                        SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")+"|1#"+
                        SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드_2")+"|0");
                }

				UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
					SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#" +
					SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3");

            }
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
			bSave = true;
            Search("");
        }

        private void Search(string strTranNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_ITR003  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pTRAN_DT_FR= '" + dtpSTranDtFr.Text + "' ";
                strQuery += ", @pTRAN_DT_TO = '" + dtpSTranDtTo.Text + "' ";
                strQuery += ", @pMOVE_TYPE = '" + txtSMoveType.Text + "' ";
                strQuery += ", @pTRAN_DUTY = '" + txtSTranDuty.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "' ";
                strQuery += ", @pTRAN_NO = '" + txtSTranNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					int x = 0, y = 0;

					if (strTranNo != "")
					{
						fpSpread2.Search(0, strTranNo, false, false, false, false, 0, 0, ref x, ref y);

						if (x > 0)
						{
							fpSpread2.Sheets[0].SetActiveCell(x, y);
						}
						else
						{
							x = 0;
						}
					}
					strAutoTranNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "수불번호")].Text;
					fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
					NewFlg = 2;

					//상세정보조회
					SubSearch(strAutoTranNo);
				}
				else
				{
					NewFlg = 1;
					strAutoTranNo = "";
					btnNew_is = false;
					NewExec();
					btnNew_is = true;
				}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            bool All_del = false;
            string strAutoTranNo2 = "";
			string strAutoTranSeq2 = "0";
            DialogResult dsMsg;
            txtMoveType.Focus();
			strAutoTranNo = "";
			strAutoTranNo2 = "";

			if (bSave == false) { bSave = true; return; }

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        if (NewFlg == 2)
                        {
                            string strSql = " usp_ITR003 'U1'";
                            strSql += ", @pTRAN_NO = '" + txtTranNo.Text + "' ";
                            strSql += ", @pREMARK1 = '" + txtRemark.Text + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();

                            if (ERRCode != "OK")
                            {
                                Trans.Rollback();
                                strAutoTranNo = "";
								strAutoTranNo2 = "";
                                goto Exit;
                            } 	// ER 코드 Return시 점프
                            else
                            {
                                strAutoTranNo = dt.Rows[0][2].ToString();
                            }
                        }

                        /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                        {
                            if (DelCheck() == false) All_del = true;

                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                string strGbn = "";

                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U2"; break;
                                        case "I": strGbn = "I1"; break;
                                        case "D": strGbn = "D1"; break;
                                        default: strGbn = ""; break;
                                    }

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text
                                            == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text == txtProjectNo2.Text
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text == txtProjectSeq2.Text
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text == txtSlCd2.Text
                                        && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text == txtLocation2.Text
                                        )
                                    {

                                        ERRCode = "ER"; MSGCode = "같은 품목, 프로젝트번호, 프로젝트차수, 창고, 위치로는 재고이동을 하지 않습니다!";
										Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit;
                                    }


                                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value)
                                        > Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value)
                                        && Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value) != 0)
                                    {
                                        ERRCode = "ER";

                                        string msg = "출고수량이 ";
                                        msg += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value).ToString("###,###,###,###,##0.00");
                                        msg += " 보다 크면 안됩니다!";

                                        MSGCode = msg;
                                        fpSpread1.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량"));
										Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit;
                                    }

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value.ToString() == "0")
                                    {
                                        ERRCode = "ER"; MSGCode = "이동수량이 0이면 안됩니다.";
										Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit;
                                    }

									if (string.Compare(strHead, "D", true) != 0 && GetValidLotNo() == false)
									{
										ERRCode = "ER"; MSGCode = "Lot 추적 품목은 반드시 \r\nLot를 지정한 후 재고이동해야 합니다.";
										Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit;
									}

                                    string strSql = " usp_ITR003 '" + strGbn + "'";
                                    strSql += ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    strSql += ", @pTRAN_NO = '" + strAutoTranNo + "' ";
                                    strSql += ", @pTRAN_DT = '" + dtpTranDt.Text + "' ";
                                    strSql += ", @pMOVE_TYPE = '" + txtMoveType.Text + "' ";
                                    strSql += ", @pTRAN_DUTY = '" + txtTranDuty.Text + "' ";
                                    strSql += ", @pCOST_DEPT_CD = '" + txtCostDeptCd.Text + "' ";
                                    strSql += ", @pREMARK1 = '" + txtRemark.Text + "' ";

                                    if (strGbn != "I1")
                                        strSql += ", @pTRAN_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text;

                                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                    strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                    strSql += ", @pINV_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
                                    strSql += ", @pTRAN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value + "' ";
                                    strSql += ", @pTRAN_PRICE  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value + "' ";
                                    strSql += ", @pTRAN_AMT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value + "' ";
                                    strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' ";
                                    strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text + "' ";

                                    strSql += ", @pMOV_TRAN_NO = '" + strAutoTranNo2 + "' ";
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text.Trim() == "")
                                        strSql += ", @pMOV_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                    else
                                        strSql += ", @pMOV_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text + "' ";
                                    strSql += ", @pMOV_PROJECT_NO = '" + txtProjectNo2.Text + "' ";
                                    strSql += ", @pMOV_PROJECT_SEQ = '" + txtProjectSeq2.Text + "' ";
                                    strSql += ", @pMOV_SL_CD = '" + txtSlCd2.Text + "' ";
                                    strSql += ", @pMOV_LOCATION_CD = '" + txtLocation2.Text + "' ";
                                    strSql += ", @pMOV_INV_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
                                    strSql += ", @pMOV_TRAN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value + "' ";
                                    strSql += ", @pREMARK2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                    // 2022.06.14. hma 수정(Start): 발주감안참조 사용 안하므로 주석 처리함. S/P에 매개변수도 없음.
                                    //strSql += ", @pREQ_NO = '"  + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text + "' ";      //요청번호 추가  2017.03.16
                                    //strSql += ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Value + "' ";     //요청순번 추가  2017.03.16
                                    //strSql += ", @pTRAN_NO_B = '"  + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주감안입고번호")].Text + "' ";   //수불번호 추가  2017.03.16
                                    //strSql += ", @pTRAN_SEQ_B = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주감안입고순번")].Value + "' ";  //수불순번 추가  2017.03.16
                                    // 2022.06.14. hma 수정(End)

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
									if (ERRCode != "OK") { Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit; } 	// ER 코드 Return시 점프

									#region Lot 추적관리
									
									if (
										ERRCode == "OK" && 
										string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0
									   )
                                    {
										strAutoTranNo = ds.Tables[0].Rows[0][2].ToString();		// T_OUT_INFO.OUT_TRAN_NO
										strAutoTranSeq = ds.Tables[0].Rows[0][4].ToString();	// T_OUT_INFO.OUT_TRAN_SEQ
									
										if (strGbn == "I1") // 입고 후 저장번호/순번
										{
											strAutoTranNo2 = ds.Tables[0].Rows[0][3].ToString();	// T_IN_INFO.IN_TRAN_NO,	T_IN_INFO.MVMT_NO
											strAutoTranSeq2 = ds.Tables[0].Rows[0][5].ToString();	// T_IN_INFO.IN_TRAN_SEQ,	T_IN_INFO.MVMT_SEQ
										}

										// 1. LOT 출고처리 - strAutoTranNo, strAutoTranSeq
										strSql = "  usp_T_OUT_INFO_CUDR ";
										strSql += " @pTYPE            = '" + strGbn + "' ";
										strSql += ",@pCO_CD           = '" + SystemBase.Base.gstrCOMCD + "' ";
										strSql += ",@pPLANT_CD        = '" + SystemBase.Base.gstrPLANT_CD + "' ";
										strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
										strSql += ",@pMVMT_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
										strSql += ",@pMVMT_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
										strSql += ",@pOUT_TRAN_NO     = '" + strAutoTranNo + "' ";
										strSql += ",@pOUT_TRAN_SEQ    = '" + strAutoTranSeq + "' ";
										strSql += ",@pITEM_CD         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
										strSql += ",@pTR_TYPE         = 'M' ";
										strSql += ",@pOUT_DATE        = '" + dtpTranDt.Text + "' ";
										strSql += ",@pLOT_NO          = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
										strSql += ",@pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
										strSql += ",@pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
										strSql += ",@pOUT_QTY         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value + "' ";
										strSql += ",@pSTOCK_UNIT      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
										strSql += ",@pREMARK          = '" + strAutoTranNo2 + "/" + strAutoTranSeq2 + "' ";
										strSql += ",@pIN_ID           = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pUP_ID           = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pORG_MVMT_NO	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고번호")].Text + "' ";
										strSql += ",@pORG_MVMT_SEQ	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고순번")].Text + "' ";

										DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds2.Tables[0].Rows[0][0].ToString();
										MSGCode = ds2.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit; }	// ER 코드 Return시 점프


										// 2. LOT 입고처리 - strAutoTranNo2, strAutoTranSeq2
										strSql = "  usp_T_IN_INFO_CUDR ";
										strSql += " @pTYPE        = '" + strGbn + "' ";
										strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
										strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";
										
										
										// 입고 삭제의 경우 이동후 수불번호(MOV_TRAN_NO)가 기준이 되므로 이에 대한 처리를 달리해줘야 함
										if (string.Compare(strGbn, "D1", true) == 0)
										{
											strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 바코드")].Text + "' ";
											strSql += ",@pMVMT_NO     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고번호")].Text + "' ";
											strSql += ",@pMVMT_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고순번")].Text + "' ";
										}
										else
										{
											strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
											strSql += ",@pMVMT_NO     = '" + strAutoTranNo2 + "' ";
											strSql += ",@pMVMT_SEQ    = '" + strAutoTranSeq2 + "' ";
										}

										if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text.Trim() == "")
											strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
										else
											strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text + "' ";
										
										strSql += ",@pTR_TYPE      = 'M' ";
										strSql += ",@pIN_DATE      = '" + dtpTranDt.Text + "' ";
										strSql += ",@pLOT_NO       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
										strSql += ",@pPROJECT_NO   = '" + txtProjectNo2.Text + "' ";
										strSql += ",@pPROJECT_SEQ  = '" + txtProjectSeq2.Text + "' ";
										strSql += ",@pRCPT_QTY     = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value + "' ";
										strSql += ",@pIN_TRAN_NO   = '" + strAutoTranNo2 + "' ";
										strSql += ",@pIN_TRAN_SEQ  = '" + strAutoTranSeq2 + "' ";
										strSql += ",@pIN_TRAN_QTY  = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value + " ";
										strSql += ",@pSTOCK_QTY    = 0 ";
										strSql += ",@pSTOCK_UNIT   = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
										strSql += ",@pEND_YN       = 'N' ";
										strSql += ",@pREMARK       = '" + strAutoTranNo + "/" + strAutoTranSeq + "' ";
										strSql += ",@pIN_ID        = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pUP_ID        = '" + SystemBase.Base.gstrUserID + "' ";
										strSql += ",@pORG_MVMT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고번호")].Text + "' ";
										strSql += ",@pORG_MVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고순번")].Text + "' ";

										DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds3.Tables[0].Rows[0][0].ToString();
										MSGCode = ds3.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); strAutoTranNo = ""; strAutoTranNo2 = ""; goto Exit; }	// ER 코드 Return시 점프

									}

									#endregion

								}
                            }
                        }
                        else
                        {
                            Trans.Rollback();
							strAutoTranNo = "";
							strAutoTranNo2 = "";
                            this.Cursor = Cursors.Default;
                            return;
                        }
                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = e.Message;
						strAutoTranNo = ""; 
						strAutoTranNo2 = "";
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();
                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (All_del) { Search(""); NewExec(); }
                        else if (NewFlg == 1) Search(strAutoTranNo);
                        else SubSearch(strAutoTranNo);

                    }
                    else if (ERRCode == "ER")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.Cursor = Cursors.Default;
                }
                else
                {
                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
                }
            }

        }
        #endregion

		#region Lot 추적품목은 반드시 Lot를 지정해야 함
		private bool GetValidLotNo()
		{
			bool bReturn = true;

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (
						string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0 &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text) == true
					   )
					{
						bReturn = false;
						break;
					}
				}
			}

			return bReturn;
		}
		#endregion

		#region 삭제Row Count 체크
		private bool DelCheck()
        {
            bool delChk = true;
            int delCount = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                {
                    delCount++;
                }
            }

            if (delCount == fpSpread1.Sheets[0].Rows.Count)
            { delChk = false; }

            return delChk;
        }
        #endregion

        #region Grid Button Click
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";

            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    //ITR003P1 pu = new ITR003P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                    //pu.ShowDialog();

                    //if (pu.DialogResult == DialogResult.OK)
                    //{
                    //    string[] Msgs = pu.ReturnVal;

                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[1].ToString();		//품목코드
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs[2].ToString();			//품명
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[3].ToString();			//규격
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text = "";
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[4].ToString();	//프로젝트번호
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[6].ToString();	//프로젝트차수
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[7].ToString();			//창고
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[8].ToString();			//창고명
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = Msgs[9].ToString();			//창고위치			
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[10].ToString();			//위치명

                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[11].ToString();			//재고단위
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = Msgs[12].ToString();		//수불수량	

                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = Msgs[13].ToString();		//단가	
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = Convert.ToDecimal(Msgs[13].ToString()) * Convert.ToDecimal(Msgs[12].ToString());
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = Convert.ToDecimal(Msgs[14].ToString());

                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Value = Msgs[15].ToString();		//바코드
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value = Msgs[16].ToString();		//입고번호
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value = Msgs[17].ToString();		//입고순번
                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = Msgs[18].ToString();		//Lot No
                    //    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value = Msgs[14].ToString();		//Lot 추적 여부
                        
                    //    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                    //    SetGrd(Row, true);

                    //    //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = true;

                    //    if (string.Compare(Msgs[14].ToString(), "True", true) == 0)
                    //    {
                    //        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0#"
                    //                                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|0"
                    //                                                );
                    //    }
                    //    else
                    //    {
                    //        UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3#"
                    //                                                + SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|3"
                    //                                                );
                    //    }

                    //}

                    ITR003P4 pu = new ITR003P4(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[1].ToString();		//품목코드
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs[2].ToString();			//품명
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[3].ToString();			//규격
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[4].ToString();	//프로젝트번호
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = Msgs[5].ToString();	    // 2022.06.13. hma 추가: 프로젝트명
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[6].ToString();	//프로젝트차수
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[7].ToString();			//창고
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[8].ToString();			//창고명
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = Msgs[9].ToString();			//창고위치			
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[10].ToString();			//위치명

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = Msgs[11].ToString();			//재고단위
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = Msgs[12].ToString();		//수불수량

                        decimal dcmSTOCK_QTY = 0;
                        decimal dcmLOT_QTY = 0;
                        decimal dcmMOVE_QTY = 0;

                        dcmSTOCK_QTY = Convert.ToDecimal(Msgs[12].ToString()); //이동수량 (차수별 재고)
                        dcmLOT_QTY = Convert.ToDecimal(Msgs[18].ToString()); //이동수량 (LOT 재고)

                        //Lot 관리 품목이면 둘중에 작은게 이동수량이 된다
                        if (dcmSTOCK_QTY <= dcmLOT_QTY)
                        {
                            dcmMOVE_QTY = dcmSTOCK_QTY;
                        }
                        else
                        {
                            dcmMOVE_QTY = dcmLOT_QTY;
                        }

                        // 2015.09.30. hma 추가: 재고수량 항목에 현재고수량이 들어가도록 함.
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = dcmSTOCK_QTY;

                        if (string.Compare(Msgs[15].ToString(), "True", true) == 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = dcmMOVE_QTY; //이동수량 (LOT 재고)
                            //dcmMOVE_QTY = Convert.ToDecimal(Msgs[18].ToString());
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = dcmSTOCK_QTY;  //이동수량
                            //dcmMOVE_QTY = Convert.ToDecimal(Msgs[12].ToString());
                        }

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Value = Msgs[17].ToString();		//바코드
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Value = Msgs[13].ToString();		//입고번호
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Value = Msgs[14].ToString();		//입고순번
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = Msgs[16].ToString();		//Lot No
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Value = Msgs[15].ToString();		//Lot 추적 여부

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = Msgs[19].ToString();		//단가

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = Convert.ToDecimal(Msgs[19].ToString()) * dcmMOVE_QTY;
                        
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                        SetGrd(Row, true);

                        //fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = true;

                        if (string.Compare(Msgs[15].ToString(), "True", true) == 0)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0#"
                                                                    + SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|0"
                                                                    );
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3#"
                                                                    + SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|3"
                                                                    );
                        }

                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //변경품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드")].Text = Msgs[2].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //프로젝트번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {
                    // 2016.09.08. hma 수정: 프로젝트정보 팝업에서 마감여부 기본값이 '전체'가 되도록 파라메터 "A" 추가함.
                    WNDW007 pu = new WNDW007(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "A");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text = Msgs[4].ToString();       // 2022.06.13. hma 추가
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //프로젝트차수
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[0].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //창고
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2"))
            {
                try
                {
                    string strQuery = "usp_I_COMMON @pTYPE ='I010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pSPEC2 = '" + SystemBase.Base.gstrPLANT_CD + "' , @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }

            //위치
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "위치_2"))
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text.Trim() == ""
                        && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text != "*")
                    {
                        MessageBox.Show("창고 먼저 선택하세요!");
                        fpSpread1.Sheets[0].SetActiveCell(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고"));
                        return;
                    }
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }

			//LOT NO
			if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") &&
				string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text) == false)
			{
				try
				{
					ITR003P2 pu = new ITR003P2();

					pu.strCO_CD = SystemBase.Base.gstrCOMCD;
					pu.strPLANT_CD = SystemBase.Base.gstrPLANT_CD;
					pu.strITEM_CD = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
					pu.strPROJECT_NO = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;

					pu.ShowDialog();

					if (pu.DialogResult == DialogResult.OK)
					{
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = pu.ReturnVal[6];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text = pu.ReturnVal[8];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text = pu.ReturnVal[12];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text = pu.ReturnVal[13];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text = pu.ReturnVal[14];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고번호")].Text = pu.ReturnVal[15];
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "원본 입고순번")].Text = pu.ReturnVal[16];
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					//데이터 조회 중 오류가 발생하였습니다.
				}
			}

			// 바코드 출력
			if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력"))
			{
				if (cboPort.SelectedText == "선택")
				{
					MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bPrintAll = false;
				GetPrintData(Row, "E");
			}

            strBtn = "N";
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                //수량, 단가, 발주금액
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량"))
                {
                    if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text) == false
                        && string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Text) == false)
                    {
						if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value)
							> Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value))
						{
							string msg = "이동수량이 ";
							msg += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value).ToString("###,###,###,###,##0.00");
							msg += " 보다 크면 안됩니다!";

							MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value;
							bSave = false;
						}
						else
						{
							bSave = true;
							Set_Amt(Row); 
						}
                    }
                }

                //품목코드
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                {
                    string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                    strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                    strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                    strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                    strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

					string strQuery = "SELECT A.ITEM_NM, A.ITEM_SPEC, A.ITEM_UNIT, ISNULL((SELECT LOT_YN FROM B_PLANT_ITEM_INFO WHERE CO_CD = A.CO_CD AND ITEM_CD = A.ITEM_CD), 'N') AS LOT_YN ";
                    strQuery += "       FROM B_ITEM_INFO A(NOLOCK) ";
					strQuery += "	   WHERE A.ITEM_CD = '" + strItemCd + "' AND A.CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

					if (dt != null && dt.Rows.Count > 0 && string.Compare(dt.Rows[0][0].ToString(), "ER", true) != 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = dt.Rows[0]["ITEM_UNIT"].ToString();

						if (string.Compare(dt.Rows[0]["LOT_YN"].ToString(), "Y", true) == 0)
						{
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = "True";
						}
						else
						{
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = "False";
						}

                        //재고정보 가져오기
                        if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                        {
                            DataInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                        }

						// lot 추적 품목만 lot 팝업 호출가능
						if (string.Compare(dt.Rows[0]["LOT_YN"].ToString(), "Y", true) == 0)
						{
							UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0");
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = true;
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3");
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = false;
						}
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";//프로젝트차수
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = "";		//창고
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";		//창고명
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = "*";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "*";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                    }

                    SetGrd(Row, false);
                }

                //프로젝트번호
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
                {
                    string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                    strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                    strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                    strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                    strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                    // 2022.06.13. hma 추가(Start)
                    string strQuery = " SELECT MAX(PROJECT_NM) AS PROJECT_NM FROM S_SO_MASTER A(NOLOCK) ";
                    strQuery += " WHERE A.CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND PROJECT_NO = '" + strProjectNo + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt != null && dt.Rows.Count > 0 && string.Compare(dt.Rows[0][0].ToString(), "ER", true) != 0)
                    {
                    // 2022.06.13. hma 추가(End)
                        if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                        {
                            DataInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                        }
                    }
                }

                //프로젝트차수
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
                {
                    string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                    strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                    strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                    strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                    strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                    if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                    {
                        DataInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                    }
                }

                //창고
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고"))
                {
                    string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                    strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                    strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                    strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                    strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                    if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                    {
                        DataInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                    }
                }

                //위치
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "위치"))
                {
                    string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                    strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                    strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                    strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                    strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                    strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                    if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                    {
                        DataInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;		//수불수량
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;		//단가	
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DataInfo
        private void DataInfo(int Row, string strItemCd, string strProjectNo, string strProjectSeq, string strSlCd, string strLocationCd)
        {
            DataTable ItemAcctDt = SystemBase.DbOpen.NoTranDataTable("SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + strItemCd + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (ItemAcctDt.Rows.Count > 0)
            {
                string Query = "usp_ITR003 'S6', @pITEM_CD = '" + strItemCd + "'";
                Query += ", @pITEM_ACCT = '" + ItemAcctDt.Rows[0]["ITEM_ACCT"].ToString() + "'";
                Query += ", @pPROJECT_NO = '" + strProjectNo + "'";
                Query += ", @pPROJECT_SEQ = '" + strProjectSeq + "'";
                Query += ", @pSL_CD = '" + strSlCd + "'";
                Query += ", @pLOCATION_CD = '" + strLocationCd + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable itemDt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (itemDt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = itemDt.Rows[0][4].ToString();	//프로젝트번호
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = itemDt.Rows[0][5].ToString();	//프로젝트차수
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = itemDt.Rows[0][6].ToString();          //창고
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = itemDt.Rows[0][7].ToString();		//창고명
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = itemDt.Rows[0][8].ToString();          //창고위치			
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = itemDt.Rows[0][9].ToString();		//위치명
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = itemDt.Rows[0][10].ToString();		//재고단위
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = itemDt.Rows[0][11].ToString();    //재고수량	
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = itemDt.Rows[0][12].ToString();    //단가	
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = itemDt.Rows[0][13].ToString();    //금액
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = itemDt.Rows[0][11].ToString();    // 2015.09.30. hma 추가
                }
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";	//프로젝트차수
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = "";		//창고
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";		//창고명
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text = "*";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "*";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;    //수불수량
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value = 0;    //단가	
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = 0;	//금액
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;    // 2015.09.30. hma 추가
            }
        }
        #endregion

        #region 그리드 제정의
        private void SetGrd(int iRow, bool div)
        {
            if (div == true)
            {
                UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "위치") + "|3#"
                                                       + SystemBase.Base.GridHeadIndex(GHIdx1, "위치_2") + "|3");
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "위치") + "|1#"
                                                        + SystemBase.Base.GridHeadIndex(GHIdx1, "위치_2") + "|1");
            }

        }
        #endregion

        #region 금액계산
        private void Set_Amt(int Row)
        {
            decimal Amt = 0;
            decimal Price = 0;
            decimal Qty = 0;


            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text != "0"
                && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value);

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Text != "0"
                && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단가")].Value);

            if (Price != 0 && Qty != 0)
            {
                Amt = Price * Qty;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = Amt;
            }


        }
        #endregion

        #region Master그리드 선택시 상세정보 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                    strAutoTranNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "수불번호")].Text.ToString();

                    SubSearch(strAutoTranNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.			
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
            strBtn = "Y";
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_ITR003  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pTRAN_NO ='" + strCode + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                dtpTranDt.ReadOnly = false;

                txtTranNo.Value = dt.Rows[0]["TRAN_NO"].ToString();
                dtpTranDt.Value = dt.Rows[0]["TRAN_DT"].ToString();
                txtMoveType.Value = dt.Rows[0]["MOVE_TYPE"].ToString();
                txtMoveTypeNm.Value = dt.Rows[0]["MOVE_TYPE_NM"].ToString();
                txtTranDuty.Value = dt.Rows[0]["TRAN_DUTY"].ToString();
                txtTranDutyNm.Value = dt.Rows[0]["USR_NM"].ToString();
                txtCostDeptCd.Value = dt.Rows[0]["COST_DEPT_CD"].ToString();
                txtCostDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                //Detail그리드 정보.
                string strSql1 = " usp_ITR003  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pTRAN_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);


                //Detail Locking설정
                UIForm.FPMake.grdReMake(fpSpread1, 19, 3);

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
					{
						if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0");
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3");
						}
					}
				}

                //Detail그리드 정보.
                strSql1 = " usp_ITR003  'S5' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pTRAN_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strSql1);
                if (dt1.Rows.Count > 0)
                {
                    txtProjectNo2.Value = dt1.Rows[0]["MOV_PROJECT_NO"].ToString();
                    txtProjectSeq2.Value = dt1.Rows[0]["MOV_PROJECT_SEQ"].ToString();
                    txtSlCd2.Value = dt1.Rows[0]["MOV_SL_CD"].ToString();
                    txtSlNm2.Value = dt1.Rows[0]["MOV_SL_NM"].ToString();
                    txtLocation2.Value = dt1.Rows[0]["MOV_LOCATION_CD"].ToString();
                    txtLocationNm2.Value = dt1.Rows[0]["MOV_LOCATION_NM"].ToString();
                }

				// 프린터 포트 ComboBox 설정
				cboPort.Enabled = true;
				SystemBase.RawPrinterHelper.SetPortCombo(cboPort);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //수불유형
        private void btnSMoveType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE ='I014', @pSPEC1= 'ST' , @pSPEC2= '-', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00065", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSMoveType.Value = Msgs[0].ToString();
                    txtSMoveTypeNm.Value = Msgs[1].ToString();
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

        //수불담당자
        private void btnSTranDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSTranDuty.Value = Msgs[0].ToString();
                    txtSTranDutyNm.Value = Msgs[1].ToString();
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

        private void btnSProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Value = Msgs[3].ToString();
                    if (txtSProjectSeq.Text == "*") txtSProjectSeq.Value = "";
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

        private void btnSProjSeq_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtSProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtSProjectSeq.Value = Msgs[0].ToString();
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


        #endregion

        #region 조회조건 TextChanged
        //수불유형
        private void txtSMoveType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSMoveType.Text != "")
                    {
                        txtSMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtSMoveType.Text, " AND TRAN_TYPE = 'ST' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSMoveTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //수불담당자
        private void txtSTranDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSTranDuty.Text != "")
                    {
                        txtSTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSTranDuty.Text, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSTranDutyNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            txtSProjectSeq.Value = "";
        }

        #endregion

        #region 입력조건 팝업
        //수불담당자
        private void btnTranDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranDuty.Value = Msgs[0].ToString();
                    txtTranDutyNm.Value = Msgs[1].ToString();
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

        //수불유형
        private void btnMoveType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE ='I015', @pSPEC1= 'ST' , @pSPEC2= '-', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00065", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveType.Value = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();

                    if (txtMoveType.Text == "T61" || txtMoveType.Text == "T65")
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            UIForm.FPMake.grdReMake(fpSpread1, "8|1#9|0");
                    }
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

        //비용발생부서
        private void btnCostDeptCd_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE= 'D010', @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCostDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "비용발생부서 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCostDeptCd.Value = Msgs[0].ToString();
                    txtCostDeptNm.Value = Msgs[1].ToString();
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

        private void btnSl2_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE ='I010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pSPEC2 = '" + SystemBase.Base.gstrPLANT_CD + "' , @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlCd2.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00056", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlCd2.Value = Msgs[0].ToString();
                    txtSlNm2.Value = Msgs[1].ToString();
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

        private void btnLocation2_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                if (txtSlCd2.Text.Trim() == "" && txtLocation2.Text != "*")
                {
                    MessageBox.Show("창고 먼저 선택하세요!");
                    txtSlCd2.Focus();
                    return;
                }
                string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + txtSlCd2.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLocation2.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtLocation2.Value = Msgs[0].ToString();
                    txtLocationNm2.Value = Msgs[1].ToString();
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

        private void btnProj2_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                // 2016.09.08. hma 수정: 프로젝트정보 팝업에서 마감여부 기본값이 '전체'가 되도록 파라메터 "A" 추가함.
                WNDW007 pu = new WNDW007(txtProjectNo2.Text, "A");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo2.Value = Msgs[3].ToString();
                    if (txtProjectSeq2.Text != "*") txtProjectSeq2.Value = "";
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

        private void btnProjSeq2_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo2.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProjectSeq2.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region 입력조건 TextChanged
        //수불담당자
        private void txtTranDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTranDuty.Text != "")
                    {
                        txtTranDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtTranDuty.Text, " AND TRAN_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtTranDutyNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //수불유형
        private void txtMoveType_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                //수불구분
                if (strBtn == "N")
                {
                    if (txtMoveType.Text != "")
                    {
                        txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveType.Text, " AND TRAN_TYPE = 'ST' AND INV_DCR_FLAG = '-' AND MOVE_TYPE <> 'T00' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    }
                    else
                    {
                        txtMoveTypeNm.Value = "";
                    }
                    if (txtMoveType.Text == "T61" || txtMoveType.Text == "T65")
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            UIForm.FPMake.grdReMake(fpSpread1, "8|1#9|0");
                    }
                }                
            }
            catch
            {

            }
        }

        //비용발생부서
        private void txtCostDeptCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCostDeptCd.Text != "")
                    {
                        txtCostDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtCostDeptCd.Text, " AND REORG_ID = '" + SystemBase.Base.gstrREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCostDeptNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSlCd2_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSlCd2.Text != "")
                    {
                        txtSlNm2.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlCd2.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSlNm2.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtLocation2_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtSlCd2.Text.Trim() == "" && txtLocation2.Text != "*")
                    {
                        DialogResult dsMsg = MessageBox.Show("창고 먼저 선택하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSlCd2.Focus();
                        txtLocation2.Value = "";
                        txtLocationNm2.Value = "";
                    }
                    else
                    {
                        if (txtLocation2.Text != "")
                        {
                            txtLocationNm2.Value = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", txtLocation2.Text, " AND SL_CD ='" + txtSlCd2.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtLocationNm2.Value = "";
                        }
                    }
                }                
            }
            catch
            {

            }
        }

        private void txtProjectNo2_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtProjectSeq2.Text != "*")
                txtProjectSeq2.Value = "";
        }


        private void txtProjectSeq2_Leave(object sender, System.EventArgs e)
        {            
            try
            {
                if (txtProjectSeq2.Text != "")
                {
                    if (strBtn == "N" && txtProjectSeq2.Text != "*")
                    {
                        string seq = SystemBase.Base.CodeName("PROJECT_NO", "MAX(PROJECT_SEQ)", "S_SO_DETAIL", txtProjectNo2.Text, " AND PROJECT_SEQ = '" + txtProjectSeq2.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        
                        if (seq == "")
                        {	//"프로젝트차수가 잘못 입력되었습니다!"
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0054"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtProjectSeq2.Value = "";
                            txtProjectSeq2.Focus();
                        }
                        else
                        {
                            txtProjectSeq2.Value = seq;
                        }
                    }
                }                
            }
            catch
            {

            }
        }
        #endregion

        #region  Activated
        private void ITR003_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSTranDtFr.Focus();
        }

        private void ITR003_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion		

		#region 바코드 정보 조회
		private void GetPrintData(int row, string flag)
		{
			string strSql = string.Empty;
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();

			dtPrint.Clear();

			strSql = " usp_T_IN_INFO_CUDR ";
			strSql += "  @pTYPE         = 'P1'";
			strSql += ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "' ";
			strSql += ", @pPLANT_CD     = '" + SystemBase.Base.gstrPLANT_CD + "' ";
			strSql += ", @pBAR_CODE     = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 바코드")].Text + "' ";
			strSql += ", @pMVMT_NO		= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고번호")].Text + "' ";
			strSql += ", @pMVMT_SEQ     = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "이동후 입고순번")].Text + "' ";

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strSql);

			if (dtPrint.Rows.Count > 0)
			{
				PrintBarCode(row, flag);
			}
			else
			{
				MessageBox.Show("검색된 데이터가 없습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		#endregion

		#region 바코드 인쇄
		private void PrintBarCode(int row, string flag)
		{
			string strZPL = string.Empty;

			int X = -30;
			int Y = 5;

			if (dtPrint.Rows.Count > 0)
			{

				for (int i = 0; i <= dtPrint.Rows.Count - 1; i++)
				{
					strZPL = "";
					strZPL += "^XA";					// start format

					strZPL += "^LL440";					// label hight
					strZPL += "^PW600";					// print length

					strZPL += "^LS0";					// print length
					strZPL += "^LH5,5";					// label home location - 최초 시작 위치(x, y)


					strZPL += "^SEE:UHANGUL.DAT^FS";	// 인코딩 지정, ^FS:field separator, ^FO:field origin
					strZPL += "^CWJ,E:KFONT3.FNT^FS";	// 폰트

					// FO : 인쇄할 항목의 인쇄 위치 지정(X,Y)
					// GB500(라인 길이),150(라인 높이),7(라인 두께),(라인 색상),5(라인 모서리 둥글기)
					//strZPL += "^FO50,0^GB550,140,7,,5^FS";	//라인 박스 그리기

					// BY2,2,80 - 바코드 속성 중 좁은 바 넓이를 2로 하고, 넓은 바는 좁은 바의 2배로 지정. 바코드 높이는 80 
					// BCN(문자회전 NORMAL, R:90도, I:180도, B:270도),80(바코드 높이),Y(바코드 밑에 문자인쇄 여부),N(바코드 위에 문자인쇄 여부),N(CHECK DIGIT 사용 여부) 
					strZPL += "^FO" + (X + 80) + "," + (Y + 10) + "^BY2,2.2,90^BCN,90,Y,N,N^FD" + dtPrint.Rows[i]["BAR_CODE"].ToString() + "^FS";	//^BC:Code 128(USD-6)체계

					strZPL += "^FO" + (X + 80) + "," + (Y + 140) + "^CI28^AJN,25,25^FDPrj No^FS" + "^FO" + (X + 180) + "," + (Y + 140) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["PROJECT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 170) + "^CI28^AJN,40,40^FDCode No : " + dtPrint.Rows[i]["ITEM_CD"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 220) + "^CI28^AJN,25,25^FDDesc^FS" + "^FO" + (X + 180) + "," + (Y + 220) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_NM"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 250) + "^CI28^AJN,25,25^FDPart No^FS" + "^FO" + (X + 180) + "," + (Y + 250) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_SPEC"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 280) + "^CI28^AJN,25,25^FDRec No^FS" + "^FO" + (X + 180) + "," + (Y + 280) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["MVMT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 310) + "^CI28^AJN,25,25^FDLot No^FS" + "^FO" + (X + 180) + "," + (Y + 310) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["LOT_NO"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 340) + "^CI28^AJN,25,25^FDVendor^FS" + "^FO" + (X + 180) + "," + (Y + 340) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["VENDOR"].ToString() + "^FS";
					strZPL += "^FO" + (X + 80) + "," + (Y + 370) + "^CI28^AJN,25,25^FDQ'ty^FS" + "^FO" + (X + 180) + "," + (Y + 370) + "^CI28^AJN,25,25^FD : " + SetConvert(Convert.ToDecimal(dtPrint.Rows[i]["STOCK_QTY"])) + " "
																							   + dtPrint.Rows[i]["STOCK_UNIT"].ToString() + "^FS"
																							   + "^FO" + (X + 370) + "," + (Y + 370) + "^CI28^AJN,25,25^FD(" + SystemBase.Base.gstrUserName + ")^FS"; 	
					strZPL += "^FO" + (X + 80) + "," + (Y + 400) + "^CI28^AJN,25,25^FDPrint^FS" + "^FO" + (X + 180) + "," + (Y + 400) + "^CI28^AJN,25,25^FD : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^FS";

					if (flag == "A")
					{
						strZPL += "^PQ" + "1" + "^FS";	// 라벨 인쇄 매수
					}
					else
					{
						strZPL += "^PQ" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text + "^FS";	// 라벨 인쇄 매수					
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

		#region 수량 형식 변경
		private string SetConvert(decimal dNumber)
		{
			string strReturn = string.Empty;

			strReturn = double.Parse(dNumber.ToString()).ToString();

			return strReturn;
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

		#region 일괄 이동
		private void btnMultiMove_Click(object sender, EventArgs e)
		{

			try
			{
				ITR003P3 pu = new ITR003P3(fpSpread1);
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						//fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Locked = false;

						if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0)
						{
							UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|0#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량") + "|3"
																	);
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "재고분할") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3#"
																	+ SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량") + "|0"
																	);
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
			finally 
			{ 
			
			}
			
		}
		#endregion

        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

        }

        #region fpSpread1_TextChanged:  2015.10.05. hma 추가: 그리드 텍스트 데이터 변경된 경우 ==> 그런데 이 이벤트를 안타네..ㅠㅠ
        private void fpSpread1_TextChanged(object sender, EventArgs e)
        {
            // 2015.10.05. hma 추가: 수정된 행과 열 저장 변수
            int Column = fpSpread1.Sheets[0].ActiveColumn.Index;
            int Row = fpSpread1.Sheets[0].ActiveRow.Index;

            //프로젝트번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
            {
                string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                {
                    StockInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
                }
            }

            //프로젝트번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
            {
                string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                {
                    StockInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
                }
            }

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "위치"))
            {
                string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                {
                    StockInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
                }
            }

            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고"))
            {
                string strItemCd = "", strProjectNo = "", strProjectSeq = "", strSlCd = "", strLocationCd = "";

                strItemCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
                strProjectNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
                strProjectSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
                strSlCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text;
                strLocationCd = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치")].Text;

                if (strItemCd != "" && strProjectNo != "" && strProjectSeq != "" && strSlCd != "" && strLocationCd != "")
                {
                    StockInfo(Row, strItemCd, strProjectNo, strProjectSeq, strSlCd, strLocationCd);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
                }
            }
        }
        #endregion


        #region StockInfo:  2015.10.05. hma 추가: 현재고 정보 가져와서 항목에 넣어줌.
        private void StockInfo(int Row, string strItemCd, string strProjectNo, string strProjectSeq, string strSlCd, string strLocationCd)
        {
            DataTable ItemAcctDt = SystemBase.DbOpen.NoTranDataTable("SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + strItemCd + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (ItemAcctDt.Rows.Count > 0)
            {
                string Query = "usp_ITR003 'S6', @pITEM_CD = '" + strItemCd + "'";
                Query += ", @pITEM_ACCT = '" + ItemAcctDt.Rows[0]["ITEM_ACCT"].ToString() + "'";
                Query += ", @pPROJECT_NO = '" + strProjectNo + "'";
                Query += ", @pPROJECT_SEQ = '" + strProjectSeq + "'";
                Query += ", @pSL_CD = '" + strSlCd + "'";
                Query += ", @pLOCATION_CD = '" + strLocationCd + "'";
                Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable itemDt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (itemDt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = itemDt.Rows[0][11].ToString();
                }
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = 0;
            }
        }
        #endregion


        #region 발주감안참조 2017.03.16
        private void btnREQ_PO_REF_Click(object sender, EventArgs e)
        {

            if (txtMoveType.Text.Trim() == "")
            {
                MessageBox.Show("수불유형을 먼저 입력하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMoveType.Focus();
                return;
            }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                try
                {
                    ITR003P5 pu = new ITR003P5(txtProjectNo2.Text, txtProjectSeq2.Text, fpSpread1);
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {

                            if (txtTranNo.Text.Trim() == "")
                            {

                                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "기존수량")].Value = 0;//0

                                if (txtMoveType.Text == "T61" || txtMoveType.Text == "T65")
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드") + "|1#" +
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "변경품목코드_2") + "|0");
                                }

                                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ActiveRowIndex,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#" +
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3");

                            }

                        }
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }
        #endregion 발주감안참조 2017.03.16



    }
}