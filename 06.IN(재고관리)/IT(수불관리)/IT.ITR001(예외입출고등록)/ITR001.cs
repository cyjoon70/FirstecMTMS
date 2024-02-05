#region 작성정보
/*********************************************************************/
// 단위업무명 : 예외입/출고등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 예외입/출고등록 및 관리
// 수 정 일 : 2014-08-26
// 수 정 자 : 최 용 준
// 수정내용 : 추적관리 관련 기능 추가
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
using System.Text.RegularExpressions;
using WNDW;

namespace IT.ITR001
{
    public partial class ITR001 : UIForm.FPCOMM2
	{

		#region 변수선언
		int NewFlg = 1;	//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoTranNo = string.Empty;	//수불번호
		string strAutoTranSeq = string.Empty;
        string strBtn = "N";
        bool btnNew_is = true;
        bool form_act_chk = false;
        bool All_del = false;

		// 바코드 출력 관련
		bool bPrintAll = false;
		DataTable dtPrint = new DataTable();	// 바코드 인쇄용 데이터 테이블
		#endregion

		#region 생성자
		public ITR001()
        {
            InitializeComponent();
        }
		#endregion

		#region Form Load 시
		private void ITR001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수체크

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

            //그리드 콤보박스 세팅
            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//재고단위


            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 9, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타세팅
			SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정
			cboItemAcct.SelectedIndex = 3;

			txtClearQty.BackColor = SystemBase.Validation.Kind_LightCyan;

			dtpSTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
			dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            rdoSTranType_Oi.Checked = true;
            rdoTranType_Oi.Checked = true;

            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = false;
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = false;

            NewFlg = 1;
            strAutoTranNo = "";

			if (string.IsNullOrEmpty(strAutoTranNo) == true)
			{
				btnPrintAll.Enabled = false;
			}
			else
			{
				btnPrintAll.Enabled = true;
			}

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpSTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

                rdoSTranType_Oi.Checked = true;
            }

            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
            fpSpread1.Sheets[0].Rows.Count = 0;
			fpSpread2.Sheets[0].Rows.Count = 0;

			SystemBase.Validation.GroupBox_Reset(groupBox3);

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

            //기타 세팅
			cboItemAcct.SelectedIndex = 3;
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            rdoTranType_Oi.Checked = true;

            rdoTranType_Oi.Enabled = true;
            rdoTranType_Or.Enabled = true;
            panel9.Enabled = true;

            NewFlg = 1;
            strAutoTranNo = "";

            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = false;
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = false;
            UIForm.Buttons.ReButton("111111011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            All_del = false;

			if (string.IsNullOrEmpty(strAutoTranNo) == true)
			{
				btnPrintAll.Enabled = false;
			}
			else
			{
				btnPrintAll.Enabled = true;
			}
        }
        #endregion

        #region 행추가 버튼
        protected override void RowInsExe()
        {
            if (rdoTranType_Oi.Checked == true)
            {
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|0");
            }
            else
            {
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|1");
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액") + "|1");
            }

			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Locked = true;
			fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력")].Locked = true;
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직

        protected override void SearchExec()
        {
			SystemBase.Validation.GroupBox_Reset(groupBox3);
			cboItemAcct.SelectedIndex = 3;
			Search("");
        }

        private void Search(string strTranNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strType = "";
                if (rdoSTranType_Oi.Checked == true) { strType = "OI"; }
                else if (rdoSTranType_Or.Checked == true) { strType = "OR"; }

                string strQuery = " usp_ITR001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pTRAN_DT_FR= '" + dtpSTranDtFr.Text + "' ";
                strQuery += ", @pTRAN_DT_TO = '" + dtpSTranDtTo.Text + "' ";
                strQuery += ", @pTRAN_TYPE = '" + strType + "' ";
                strQuery += ", @pMOVE_TYPE = '" + txtSMoveType.Text + "' ";
                strQuery += ", @pTRAN_DUTY = '" + txtSTranDuty.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "' ";
                strQuery += ", @pTRAN_NO = '" + txtSTranNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
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
            string strType = "";
            DialogResult dsMsg;
            txtMoveType.Focus();

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////

            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    if (rdoTranType_Oi.Checked == true) rdoSTranType_Oi.Checked = true;
                    else rdoSTranType_Or.Checked = true;

                    try
                    {
                        if (NewFlg == 2)
                        {
                            string strSql = " usp_ITR001 'U1'";
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
                                goto Exit; // ER 코드 Return시 점프
                            }
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
								string strWO_NO = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text;

                                if (strHead.Length > 0)
                                {
                                    switch (strHead)
                                    {
                                        case "U": strGbn = "U2"; break;
                                        case "I": strGbn = "I1"; break;
                                        case "D": strGbn = "D1"; break;
                                        default: strGbn = ""; break;
                                    }

									if (string.Compare(strGbn, "D1", true) != 0 && GetValidLotNo() == false)
									{
										ERRCode = "ER"; MSGCode = "Lot 추적 품목은 반드시 \r\nLot를 지정한 후 예외처리를 해야 합니다.";
										Trans.Rollback(); goto Exit;
									}

                                    string strSql = " usp_ITR001 '" + strGbn + "'";
                                    strSql += ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "' ";
                                    strSql += ", @pTRAN_NO = '" + strAutoTranNo + "' ";
                                    strSql += ", @pTRAN_DT = '" + dtpTranDt.Text + "' ";

                                    if (rdoTranType_Oi.Checked == true) strType = "OI";
                                    else strType = "OR";

                                    strSql += ", @pTRAN_TYPE = '" + strType + "' ";

                                    strSql += ", @pMOVE_TYPE = '" + txtMoveType.Text + "' ";
                                    strSql += ", @pTRAN_DUTY = '" + txtTranDuty.Text + "' ";
                                    strSql += ", @pCOST_DEPT_CD = '" + txtCostDeptCd.Text + "' ";
                                    strSql += ", @pREMARK1 = '" + txtRemark.Text + "' ";

                                    if (strGbn != "I1")
                                        strSql += ", @pTRAN_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text;

                                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                    strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                    strSql += ", @pINV_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
                                    strSql += ", @pTRAN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value + "' ";
                                    if (rdoTranType_Oi.Checked == true)
                                    {
                                        strSql += ", @pTRAN_PRICE  = 0 ";
                                        strSql += ", @pTRAN_AMT  = 0 ";
                                    }
                                    else
                                    {
                                        strSql += ", @pTRAN_PRICE  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Value + "' ";
                                        strSql += ", @pTRAN_AMT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Value + "' ";
                                    }
                                    strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' ";
                                    strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text + "' ";

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text.Trim() != "")
                                        strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text + "' ";
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text.Trim() != "")
                                        strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";

                                    strSql += ", @pREMARK2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK")
                                    {
                                        Trans.Rollback();
                                        strAutoTranNo = "";
                                        goto Exit; 	// ER 코드 Return시 점프
                                    }
                                    else
                                    {
                                        strAutoTranNo = ds.Tables[0].Rows[0][2].ToString();
										strAutoTranSeq = ds.Tables[0].Rows[0][3].ToString();
                                    }


									#region Lot 추적관리
									if (
									   ERRCode == "OK" &&
									   string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "True", true) == 0
									  )
									{
										// strType = "OI" : 예외출고 => Lot 출고
										if (string.Compare(strType, "OI", true) == 0)
										{

											// 예외 출고 삭제는 수불번호/순번만으로 처리
											if (string.Compare(strGbn, "D1", true) == 0)
											{
												strGbn = "D3";
											}

											strSql = "  usp_T_OUT_INFO_CUDR ";
											strSql += " @pTYPE            = '" + strGbn + "' ";
											strSql += ",@pCO_CD           = '" + SystemBase.Base.gstrCOMCD + "' ";
											strSql += ",@pPLANT_CD        = '" + SystemBase.Base.gstrPLANT_CD + "' ";
											strSql += ",@pBAR_CODE		  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
											strSql += ",@pMVMT_NO		  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
											strSql += ",@pMVMT_SEQ		  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
											strSql += ",@pOUT_TRAN_NO     = '" + strAutoTranNo + "' ";
											strSql += ",@pOUT_TRAN_SEQ    = '" + strAutoTranSeq + "' ";
											strSql += ",@pITEM_CD         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
											strSql += ",@pTR_TYPE         = 'E' ";
											strSql += ",@pOUT_DATE        = '" + dtpTranDt.Text + "' ";
											strSql += ",@pLOT_NO          = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
											strSql += ",@pOUT_PROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
											strSql += ",@pOUT_PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
											strSql += ",@pOUT_QTY         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value + "' ";
											strSql += ",@pSTOCK_UNIT      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
											strSql += ",@pREMARK          = '' ";
											strSql += ",@pIN_ID           = '" + SystemBase.Base.gstrUserID + "' ";
											strSql += ",@pUP_ID           = '" + SystemBase.Base.gstrUserID + "' ";

											strSql += ",@pOUT_WORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text + "' ";
											strSql += ",@pPROC_SEQ         = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Value + "' ";
										}

										// strType = "OR" : 예외입고 => Lot 입고
										if (string.Compare(strType, "OR", true) == 0)
										{
											strSql = "  usp_T_IN_INFO_CUDR ";
											strSql += " @pTYPE        = '" + strGbn + "' ";
											strSql += ",@pCO_CD       = '" + SystemBase.Base.gstrCOMCD + "' ";
											strSql += ",@pPLANT_CD    = '" + SystemBase.Base.gstrPLANT_CD + "' ";

											if (string.IsNullOrEmpty(strWO_NO))
											{
												strSql += ",@pBAR_CODE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
												strSql += ",@pMVMT_NO     = '" + strAutoTranNo + "' ";
												strSql += ",@pMVMT_SEQ    = '" + strAutoTranSeq + "' ";
												strSql += ",@pLOT_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
											}
											else
											{
												strSql += ",@pBAR_CODE    = '" + strWO_NO + "' ";
												strSql += ",@pMVMT_NO     = '" + strWO_NO + "' ";
												strSql += ",@pMVMT_SEQ    = '" + i.ToString() + "' ";
												strSql += ",@pLOT_NO      = '" + strWO_NO + "' ";
											}
											
											strSql += ",@pITEM_CD	  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
											strSql += ",@pTR_TYPE     = 'E' ";
											strSql += ",@pIN_DATE     = '" + dtpTranDt.Text + "' ";
											strSql += ",@pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
											strSql += ",@pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
											strSql += ",@pRCPT_QTY    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value + "' ";
											strSql += ",@pIN_TRAN_NO  = '" + strAutoTranNo + "' ";
											strSql += ",@pIN_TRAN_SEQ = '" + strAutoTranSeq + "' ";
											strSql += ",@pIN_TRAN_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value + " ";
											strSql += ",@pSTOCK_QTY   = 0 ";
											strSql += ",@pSTOCK_UNIT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text + "' ";
											strSql += ",@pEND_YN      = 'N' ";
											strSql += ",@pREMARK      = '' ";
											strSql += ",@pIN_ID       = '" + SystemBase.Base.gstrUserID + "' ";
											strSql += ",@pUP_ID       = '" + SystemBase.Base.gstrUserID + "' ";
											strSql += ",@pPROC_SEQ    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Value + "' ";
										}

										
										DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
										ERRCode = ds3.Tables[0].Rows[0][0].ToString();
										MSGCode = ds3.Tables[0].Rows[0][1].ToString();
										if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

									}
									#endregion

								}
                            }
                        }
                        else
                        {
                            Trans.Rollback();
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
                        //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (All_del) Search("");
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
                    dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text) == true &&
						rdoTranType_Oi.Checked == true
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

        #region Grid Button Click Event
        protected override void fpButtonClick(int Row, int Column)
        {

			strBtn = "Y";
			
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                
					if (rdoTranType_Oi.Checked == true)
                    {
                     
						ITR001P1 pu = new ITR001P1(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);

						// 테스트 품목
						//pu.strItemCd = "PBBB0081";
                        
						pu.ShowDialog();
                        
						if (pu.DialogResult == DialogResult.OK)
                        {

							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = 0;

                            string[] Msgs = pu.ReturnVal;

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[1].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs[2].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[3].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[4].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[5].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[6].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[7].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[8].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[9].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text = Msgs[10].ToString();
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = Msgs[13].ToString();

                            string Query1 = "SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                            DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                            UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

							// 추적관리 관련 설정
							if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
							{
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = false;
							}
							else
							{
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
								fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
							}
                        }
                    }
                    else
                    {
                        WNDW005 pu1 = new WNDW005(SystemBase.Base.gstrPLANT_CD, true, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                        pu1.ShowDialog();
                        if (pu1.DialogResult == DialogResult.OK)
                        {

							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = 0;

                            string[] Msgs1 = pu1.ReturnVal;

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs1[2].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = Msgs1[3].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs1[7].ToString();
                            if (rdoTranType_Oi.Checked == true) //출고
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs1[18].ToString();
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs1[19].ToString();

                                string Query1 = "SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);
                            }
                            else //입고
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs1[16].ToString();
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs1[17].ToString();
                            }

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                                = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text = Msgs1[8].ToString();
                            UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

							string Query2 = "SELECT CASE WHEN LOT_YN = 'Y' THEN 'True' ELSE 'False' END FROM B_PLANT_ITEM_INFO WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = SystemBase.DbOpen.NoTranScalar(Query2).ToString();
                        }

						// 추적관리 관련 설정
						if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
						{

							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = false;
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = Color.White;
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = false;
						}
						else
						{
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
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
            //프로젝트번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {

                    WNDW007 pu = new WNDW007(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, "N");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {

						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = 0;

                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            // 프로젝트차수
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2"))
            {
                try
                {

                    string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {

						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Value = "";
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = 0;

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
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + SystemBase.Base.gstrPLANT_CD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
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
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Value + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00030", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고위치팝업");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
			//작업지시번호
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호_2"))
            {
                try
                {
                    string strQuery = " usp_P_COMMON 'P260', @pETC = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pETC2 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P06013", strQuery, strWhere, strSearch, new int[] { 5, 3 }, "작업지시번호");
                    pu.Width = 1000;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text = Msgs[5].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);	// 수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
			// Lot 정보
			else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2"))
			{
				try
				{
					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text) == true &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text))
					{
						MessageBox.Show("품목 또는 프로젝트번호를 선택해주세요. ", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					else
					{
						WNDW.WNDW032 pu = new WNDW032();

						pu.strPLANT_CD = SystemBase.Base.gstrPLANT_CD;
						pu.strPROJECT_NO = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
						pu.strPROJECT_NM = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text;
						pu.strPROJECT_SEQ = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text;
						pu.strITEM_CD = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text;
						pu.strITEM_NM = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;
						pu.strLOT_NO = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text;

						pu.ShowDialog();

						if (pu.DialogResult == DialogResult.OK)
						{
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = pu.ReturnVal[4];
							//fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = pu.ReturnVal[5];  //기존차수 유지 2014-12-29 BY KCJ
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = pu.ReturnVal[9];
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Text = pu.ReturnVal[10];
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text = pu.ReturnVal[11];
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text = pu.ReturnVal[12];
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text = pu.ReturnVal[13];
						}
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			// 바코드 출력
			else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력"))
			{
				try
				{
					if (cboPort.SelectedText == "선택")
					{
						MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					bPrintAll = false;
					// 예외출고
					if (rdoTranType_Oi.Checked == true)
					{
						GetPrintDataOut(Row, "E");
					}
					else // 예외입고
					{
						GetPrintData(Row, "E");
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

            strBtn = "N";
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            try
            {
                if (strBtn == "N")
                {
                    //품목코드
                    if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                    {
                        string Query = " usp_M_COMMON @pTYPE = 'M012', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pNAME = '" + SystemBase.Base.gstrPLANT_CD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
							fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text = dt.Rows[0]["LOT_YN"].ToString();

                            if (rdoTranType_Oi.Checked == true) //출고
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = dt.Rows[0]["ISSUED_SL_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = dt.Rows[0]["ISSUED_LOCATION_CD"].ToString();

                                string Query1 = "SELECT ITEM_ACCT FROM B_ITEM_INFO(NOLOCK) WHERE ITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

								// 추적관리 관련 설정
								if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
								{
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = false;
								}
								else
								{
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
								}
                            }
                            else //입고
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = dt.Rows[0]["RCPT_SL_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = dt.Rows[0]["RCPT_LOCATION_CD"].ToString();

								// 추적관리 관련 설정
								if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
								{

									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = false;
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = Color.White;
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = false;
								}
								else
								{
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
									fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
								}
                            }

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                                = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Text = dt.Rows[0]["ITEM_UNIT"].ToString();
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "재고단위")].Value = "";
                        }
                    }
                    //프로젝트번호
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
                    {
                       if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*")
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";

                    }
                    // 프로젝트차수
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*")
                        {
                            string seq = SystemBase.Base.CodeName("PROJECT_NO", "MAX(PROJECT_SEQ)", "S_SO_DETAIL", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, " AND PROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            if (seq == "")
                            {	//"프로젝트차수가 잘못 입력되었습니다!"
                                MessageBox.Show(SystemBase.Base.MessageRtn("B0054"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = seq;
                            }
                        }
                    }
                    //수불 수량, 단가, 금액
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가"))
                    {

						if (string.Compare(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0 &&
							string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Text) == false)
						{
							if (
								Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Value) <
								Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value)
							   )
							{
								MessageBox.Show("수불 수량은 Lot 재고수량(" + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Value).ToString() + ")을 초과할 수 없습니다."
												, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

								fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = 0;
							}
						}

                        Set_Amt(Row);
                    }
                    // 창고 
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    //위치
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text.Trim() == "")
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                        else
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                    }
					
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 금액계산
        private void Set_Amt(int Row)
        {
            decimal Amt = 0;
            decimal Price = 0;
            decimal Qty = 0;


            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Value);

            Amt = Price * Qty;
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Value = Amt;


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
            if (All_del == false)
            {
                this.Cursor = Cursors.WaitCursor;
                strBtn = "Y";

                try
                {

                    SystemBase.Validation.GroupBox_Reset(groupBox2);

                    fpSpread1.Sheets[0].Rows.Count = 0;

					SystemBase.Validation.GroupBox_Reset(groupBox3);
					cboItemAcct.SelectedIndex = 3;

                    //수주Master정보
                    string strSql = " usp_ITR001  'S2' ";
                    strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strSql = strSql + ", @pTRAN_NO ='" + strCode + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                    if (dt.Rows[0]["TRAN_TYPE"].ToString() == "OI") rdoTranType_Oi.Checked = true;
                    else rdoTranType_Or.Checked = true;

                    dtpTranDt.ReadOnly = false;

                    txtTranNo.Value = dt.Rows[0]["TRAN_NO"].ToString();
                    dtpTranDt.Text = dt.Rows[0]["TRAN_DT"].ToString();
                    txtMoveType.Value = dt.Rows[0]["MOVE_TYPE"].ToString();
                    txtMoveTypeNm.Value = dt.Rows[0]["MOVE_TYPE_NM"].ToString();
                    txtTranDuty.Value = dt.Rows[0]["TRAN_DUTY"].ToString();
                    txtTranDutyNm.Value = dt.Rows[0]["USR_NM"].ToString();
                    txtCostDeptCd.Value = dt.Rows[0]["COST_DEPT_CD"].ToString();
                    txtCostDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                    //Detail그리드 정보.
                    string strSql1 = "usp_ITR001 ";
					strSql1 += " @pTYPE = 'S3' ";
                    strSql1 += ",@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strSql1 += ",@pTRAN_NO = '" + strCode + "' ";
                    strSql1 += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
					strSql1 += ",@pTRAN_TYPE = '" + dt.Rows[0]["TRAN_TYPE"].ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 9);

                    SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                    rdoTranType_Oi.Enabled = false;
                    rdoTranType_Or.Enabled = false;
                    panel9.Enabled = false;
					btnPrintAll.Enabled = true;

                    //Detail Locking설정
                    UIForm.FPMake.grdReMake(fpSpread1, 
                        SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|3#" +
						SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적") + "|3#" +
						SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3#" +
						SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3#" +
						SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#" +
						SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호_2") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서") + "|3#" +
                        SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3");

					if (string.Compare(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex,SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
					{
						if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text))
						{
							UIForm.FPMake.grdReMake(fpSpread1,
								SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|0#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|0#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|3#" +
								SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|3");
						}
						else
						{
							UIForm.FPMake.grdReMake(fpSpread1,
									SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No") + "|3#" +
									SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2") + "|3#" +
									SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수") + "|0#" +
									SystemBase.Base.GridHeadIndex(GHIdx1, "바코드출력") + "|0");
						}
					}

					UIForm.Buttons.ReButton("110001011001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

					// 프린터 포트 ComboBox 설정
					btnPrintAll.Enabled = true;
					cboPort.Enabled = true;
					SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
                strBtn = "N";
            }
            else
            {
                NewExec();
            }

        }
        #endregion

        #region 조회조건 팝업
        //수불유형
        private void btnSMoveType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                //수불구분
                string strTranType = "";
                if (rdoSTranType_Oi.Checked == true) { strTranType = "OI"; } //출고
                else { strTranType = "OR"; } //입고

                string strQuery = "usp_I_COMMON @pTYPE ='I011', @pSPEC1= '" + strTranType + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00054", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSMoveType.Text = Msgs[0].ToString();
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
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSTranDuty.Text = Msgs[0].ToString();
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

        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtSProjectNo.Text = Msgs[3].ToString();
                    txtSProjectSeq.Text = "";
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

        private void btnProjSeq_Click(object sender, System.EventArgs e)
        {
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

                    txtSProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 조회조건 TextChanged
        //수불유형
        private void txtSMoveType_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                //수불구분
                string strTranType = "";
                if (rdoSTranType_Oi.Checked == true) { strTranType = "OI"; } //출고
                else { strTranType = "OR"; } //입고

                if (strBtn == "N")
                {
                    if (txtSMoveType.Text != "")
                    {
                        txtSMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtSMoveType.Text, " AND TRAN_TYPE = '" + strTranType + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
            if (strBtn == "N") txtSProjectSeq.Text = "";
        }

        #endregion

        #region 입력조건 팝업
        //수불담당자
        private void btnTranDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_I_COMMON @pTYPE= 'I012', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranDuty.Text = Msgs[0].ToString();
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
                //수불구분
                string strTranType = "";
                if (rdoTranType_Oi.Checked == true) { strTranType = "OI"; } //출고
                else { strTranType = "OR"; } //입고

                string strQuery = "usp_I_COMMON @pTYPE ='I011', @pSPEC1= '" + strTranType + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00054", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveType.Text = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();
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
                string strQuery = "usp_B_COMMON @pTYPE= 'D010', @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCostDeptCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "비용발생부서 조회");
                pu.Width = 550;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCostDeptCd.Text = Msgs[0].ToString();
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
        #endregion

        #region radio CheckedChanged
        private void rdoTranType_Oi_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoTranType_Oi.Checked == true)
            {
                if (txtMoveType.Text != "") txtMoveType.Text = "";
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = false;
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = false;
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")+"|0");
            }
            else
            {
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = true;
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = true;
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|1");
            }
        }

        private void rdoTranType_Or_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoTranType_Or.Checked == true)
            {
                if (rdoTranType_Or.Checked == true)
                {
                    if (txtMoveType.Text != "") txtMoveType.Text = "";
                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = true;
                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = true;
                    UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|1");
                }
                else
                {
                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가")].Visible = false;
                    fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "수불금액")].Visible = false;
                    UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "수불단가") + "|0");
                }
            }
        }
        #endregion

        #region Form Activated & Deactivate

        private void ITR001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSTranDtFr.Focus();
        }

        private void ITR001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

        #endregion

		#region 바코드 일괄 출력
		private void btnPrintAll_Click(object sender, EventArgs e)
		{
			try
			{

				if (cboPort.SelectedText == "선택")
				{
					MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				DialogResult result = MessageBox.Show("바코드 일괄 출력을 진행하시겠습니까?\r\n이 작업에는 많은 시간이 소요될 수 있습니다.", "확인", MessageBoxButtons.YesNoCancel);

				if (result == DialogResult.Yes)
				{
					bPrintAll = true;

					// 예외출고
					if (rdoTranType_Oi.Checked == true)
					{
						GetPrintDataOut(0, "A");
					}
					else // 예외입고
					{
						GetPrintData(0, "A");
					}
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY082"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 입고 바코드

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

		#region 바코드 정보 조회
		private void GetPrintData(int row, string flag)
		{
			string strQuery = string.Empty;
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();

			dtPrint.Clear();

			/*
			바코드, 출고수량, LOT NO, 품목코드, 제조오더번호  
			*/

			if (flag == "A")
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += "  @pTYPE		= 'P1' ";
				strQuery += ", @pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ", @pPLANT_CD	= '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ", @pGUBUN		= 'A' "; // 인쇄구분(A:일괄, E:각각)
				strQuery += ", @pBAR_CODE	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
				strQuery += ", @pMVMT_NO	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
			}
			else
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += "  @pTYPE		= 'P1' ";
				strQuery += ", @pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ", @pPLANT_CD	= '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ", @pGUBUN		= 'E' "; // 인쇄구분(A:일괄, E:각각)
				strQuery += ", @pBAR_CODE	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text + "' ";
				strQuery += ", @pMVMT_NO	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
				strQuery += ", @pMVMT_SEQ	= '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
			}

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strQuery);

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

		#endregion

		#region 출고 바코드

		#region 바코드 인쇄
		private void PrintBarCodeOut(int row)
		{
			string strZPL = string.Empty;

			int X = 15;
			int Y = 5;

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


				if (row == 0)
					strZPL += "^PQ1^FS";	// 라벨 인쇄 매수
				else
					strZPL += "^PQ" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text + "^FS";	// 라벨 인쇄 매수


				strZPL += "^XZ";			// end format


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
		#endregion

		#region 바코드 정보 조회
		private void GetPrintDataOut(int row, string flag)
		{
			string strSql = string.Empty;
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();

			dtPrint.Clear();

			if (string.Compare(flag, "A", true) == 0)
			{
				strSql = " usp_T_IN_INFO_CUDR ";
				strSql += "  @pTYPE         = 'P4'";
				strSql += ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql += ", @pPLANT_CD     = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strSql += ", @pITEM_CD      = '" + fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
				strSql += ", @pOUT_TRAN_NO	= '" + txtTranNo.Text + "' ";
				strSql += ", @pGUBUN		= 'A'";
			}
			else
			{
				strSql = " usp_T_IN_INFO_CUDR ";
				strSql += "  @pTYPE         = 'P4'";
				strSql += ", @pCO_CD        = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql += ", @pPLANT_CD     = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strSql += ", @pITEM_CD      = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
				strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업지시번호")].Text + "' ";
				strSql += ", @pPROC_SEQ     = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
				strSql += ", @pOUT_TRAN_NO	= '" + txtTranNo.Text + "' ";
				strSql += ", @pOUT_TRAN_SEQ	= '" + fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "수불순번")].Text + "' ";
				strSql += ", @pGUBUN		= 'E'";
			}

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strSql);

			if (dtPrint != null && dtPrint.Rows.Count > 0)
			{
				if (string.Compare(flag, "A", true) == 0)
				{
					PrintBarCodeOut(0);
				}
				else
				{
					PrintBarCodeOut(row);
				}
			}
			else
			{
				MessageBox.Show("검색된 데이터가 없습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			
		}
		#endregion

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

		#region btnITEM_Click
		private void btnITEM_Click(object sender, System.EventArgs e)
		{
			WNDW005 pu1 = new WNDW005(SystemBase.Base.gstrPLANT_CD, cboItemAcct.SelectedValue.ToString(), txtITEM_CD.Text);
			pu1.ShowDialog();
			if (pu1.DialogResult == DialogResult.OK)
			{
				string[] Msgs1 = pu1.ReturnVal;

				txtITEM_CD.Value = Msgs1[2].ToString();
				txtITEM_NM.Value = Msgs1[3].ToString();
			}
		}

		private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (txtITEM_CD.Text != "")
				{
					txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
				}
				else
				{
					txtITEM_NM.Value = "";
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

		#region 짜투리 정리 조회
		private void btnClear_Click(object sender, EventArgs e)
		{
			try
			{

				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
				{

					if (Convert.ToDecimal(txtClearQty.Value) == 0)
					{
						MessageBox.Show("재고수량은 0보다 커야 합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					SystemBase.Validation.GroupBox_Reset(groupBox2);
					SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
					fpSpread1.Sheets[0].Rows.Count = 0;
					dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
					NewFlg = 1;
					strAutoTranNo = "";

					if (rdoTranType_Oi.Checked == false)
					{
						MessageBox.Show("예외출고를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					SubSearchNew();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void SubSearchNew()
		{

			this.Cursor = Cursors.WaitCursor;

			string strQuery = string.Empty;

			strQuery  = " usp_ITR001 ";
			strQuery += " @pTYPE		= 'S4' ";
			strQuery += ",@pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
			strQuery += ",@pPLANT_CD	= '" + SystemBase.Base.gstrPLANT_CD + "' ";
			strQuery += ",@pITEM_ACCT	= '" + cboItemAcct.SelectedValue + "' ";
			strQuery += ",@pITEM_CD		= '" + txtITEM_CD.Text.Trim() + "' ";
			strQuery += ",@pCLEAR_QTY	= '" + txtClearQty.Value + "' ";

			UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 9);

			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{

					fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";

					// 추적관리 관련 설정
					if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 추적")].Text, "true", true) == 0)
					{
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Locked = true;
					}
					else
					{
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text = "";
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Locked = true;
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
					    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No_2")].Locked = true;
					}

					//#region single lot 자동 입력 처리

					//if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot NO")].Text) == false)
					//{
					//    strQuery = string.Empty;

					//    strQuery = " usp_ITR001 ";
					//    strQuery += " @pTYPE		= 'S5' ";
					//    strQuery += ",@pCO_CD		= '" + SystemBase.Base.gstrCOMCD + "' ";
					//    strQuery += ",@pPLANT_CD	= '" + SystemBase.Base.gstrPLANT_CD + "' ";
					//    strQuery += ",@pPROJECT_NO	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
					//    strQuery += ",@pITEM_CD	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
					//    strQuery += ",@pLOT_NO	= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";

					//    DataTable dtLot = SystemBase.DbOpen.NoTranDataTable(strQuery);

					//    if (dtLot.Rows.Count > 0)
					//    {

					//        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot 재고")].Text = dtLot.Rows[0]["STOCK_QTY"].ToString();
					//        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "바코드")].Text = dtLot.Rows[0]["BAR_CODE"].ToString();
					//        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text = dtLot.Rows[0]["MVMT_NO"].ToString();
					//        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text = dtLot.Rows[0]["MVMT_SEQ"].ToString();
					//    }
					//}

					//#endregion

					fpSpread1.SetViewportTopRow(0, 0);
				}
			}

			this.Cursor = Cursors.Default;

		}
		#endregion

		#region 수량 입력시 전체 선택
		private void txtClearQty_Click(object sender, EventArgs e)
		{
			txtClearQty.Value = "";
		}
		#endregion

		#region 프로젝트/차수 변경되면 짜투리 출고 데이터는 일반 출고 데이터로 설정
		private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{
			if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") || e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
			{ 
				fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1,"Lot No")].Value = "";
				fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수불수량")].Value = "";
			}
		}
		#endregion

	}
}