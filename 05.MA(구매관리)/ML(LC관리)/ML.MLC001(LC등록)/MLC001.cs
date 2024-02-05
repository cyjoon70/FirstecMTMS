#region 작성정보
/*********************************************************************/
// 단위업무명 : L/C등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-25
// 작성내용 : L/C등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
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

namespace ML.MLC001
{
    public partial class MLC001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoLcNo = ""; //L/C번호
        string strBtn = "N";
        string strAmend = "N";
        string strSts = "";
        bool btnNew_is = true;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public MLC001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MLC001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);
            SystemBase.Validation.GroupBox_Setting(groupBox5);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//화폐단위

            //기타 세팅
            dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);

            c1DockingTab1.SelectedIndex = 0;

            dtpReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            dtpExpiryDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            dtpLatestShipDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

            rdoChargeA.Checked = true;
            rdoPartialShipY.Checked = true;
            rdoTranshipY.Checked = true;
            rdoTransferY.Checked = true;
            rdoCertOriginY.Checked = true;
            rdoBlTypeBl.Checked = true;
            txtExchRate.Value = 1;

            NewFlg = 1;
            strAutoLcNo = "";



            butPoRef.Enabled = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpReqDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            }

            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);
            SystemBase.Validation.GroupBox_Reset(groupBox5);

            SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox5, false);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅

            c1DockingTab1.SelectedIndex = 0;

            dtpReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            dtpExpiryDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
            dtpLatestShipDt.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);

            rdoChargeA.Checked = true;
            rdoPartialShipY.Checked = true;
            rdoTranshipY.Checked = true;
            rdoTransferY.Checked = true;
            rdoCertOriginY.Checked = true;
            rdoBlTypeBl.Checked = true;
            txtExchRate.Value = 1;

            NewFlg = 1;
            strAutoLcNo = "";

            butPoRef.Enabled = true;
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (strAmend == "Y" || Convert.ToInt16(strSts) >= 2) //L/C Amend정보 또는 B/L정보
            {
                MessageBox.Show("다음단계에서 참조하여 삭제할 수 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_MLC001  'D1'";
                    strSql += ", @pLC_NO = '" + strAutoLcNo + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search("");
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
        }
        #endregion
        
        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            Search("");
        }

        private void Search(string strLcNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MLC001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "' ";
                strQuery += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtSCustCd.Text.Trim() + "' ";
                strQuery += ", @pPUR_DUTY = '" + txtSUserId.Text.Trim() + "' ";
                strQuery += ", @pCOST_COND= '" + txtSCostCond.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text.Trim() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strLcNo != "")
                    {
                        fpSpread2.Search(0, strLcNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }
                    }
                    strAutoLcNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "L/C번호")].Text;
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoLcNo);
                }
                else
                {
                    NewFlg = 1;
                    strAutoLcNo = "";
                    btnNew_is = false;
                    NewExec();
                    btnNew_is = true;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002")); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            txtUserId.Focus();
            DialogResult dsMsg;
            string strMstType = "";
            string strInUpFlag = "I";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //AMEND정보가 없으면
            if (strAmend == "N")
            {
                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
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
                            if (NewFlg != 0)
                            {
                                if (NewFlg == 1) { strMstType = "I1"; }
                                else { strMstType = "U1"; }

                                string strSql = " usp_MLC001 '" + strMstType + "'";
                                strSql += ", @pLC_NO = '" + txtLcNo.Text + "' ";
                                strSql += ", @pREQ_DT = '" + dtpReqDt.Text + "' ";
                                if (dtpAdvDt.Text != "") strSql += ", @pADV_DT = '" + dtpAdvDt.Text + "' ";
                                if (dtpOpenDt.Text != "") strSql += ", @pOPEN_DT = '" + dtpOpenDt.Text + "' ";

                                strSql += ", @pEXPIRY_DT = '" + dtpExpiryDt.Text + "' ";
                                strSql += ", @pMAKER_CUST = '" + txtMakerCust.Text + "' ";
                                strSql += ", @pAGENT_CUST = '" + txtAgentCust.Text + "' ";
                                strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                strSql += ", @pXCH_RATE = '" + txtExchRate.Value + "' ";
                                strSql += ", @pBANK_TXT = '" + txtBankTxt.Text + "' ";
                                strSql += ", @pCOST_COND = '" + txtCostCond.Text + "' ";
                                strSql += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";

                                if (txtPaymentTerm.Value.ToString() != "")
                                    strSql += ", @pPAYMENT_TERM = '" + txtPaymentTerm.Value + "' ";

                                strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentTermRemark.Text + "' ";

                                if (rdoPartialShipY.Checked == true) strSql += ", @pPARTIAL_SHIP_YN = 'Y' ";
                                else strSql += ", @pPARTIAL_SHIP_YN = 'N' ";

                                strSql += ", @pLATEST_SHIP_DT = '" + dtpLatestShipDt.Text + "' ";
                                strSql += ", @pSHIPMENT_REMARK= '" + txtShipmentRemark.Text + "' ";

                                if (txtFileDd.Value.ToString() != "")
                                    strSql += ", @pFILE_DD = '" + txtFileDd.Value + "' ";

                                strSql += ", @pFILE_DD_TXT = '" + txtFileDdTxt.Text + "' ";
                                strSql += ", @pATTACH_DOC1 = '" + txtAttachDoc1.Text + "' ";
                                strSql += ", @pATTACH_DOC2 = '" + txtAttachDoc2.Text + "' ";
                                strSql += ", @pATTACH_DOC3 = '" + txtAttachDoc3.Text + "' ";
                                strSql += ", @pATTACH_DOC4 = '" + txtAttachDoc4.Text + "' ";
                                strSql += ", @pATTACH_DOC5 = '" + txtAttachDoc5.Text + "' ";

                                strSql += ", @pETC_REMARK = '" + txtEtcRemark.Text + "' ";
                                strSql += ", @pOPEN_BANK = '" + txtOpenBank.Text + "' ";
                                strSql += ", @pADVICE_BANK = '" + txtAdviceBank.Text + "' ";
                                strSql += ", @pAPPLICANT_CUST = '" + txtApplicantCust.Text + "' ";
                                strSql += ", @pBENEFICIARY_CUST = '" + txtBeneficiaryCust.Text + "' ";
                                strSql += ", @pPUR_DUTY = '" + txtUserId.Text + "' ";
                                strSql += ", @pLC_TYPE = '" + txtLcType.Text + "' ";


                                if (rdoTranshipY.Checked == true) strSql += ", @pTRANSHIPMENT_YN = 'Y' ";
                                else strSql += ", @pTRANSHIPMENT_YN = 'N' ";

                                if (rdoTransferY.Checked == true) strSql += ", @pTRANSFER_YN = 'Y' ";
                                else strSql += ", @pTRANSFER_YN = 'N' ";

                                strSql += ", @pTRANS_PLACE = '" + txtTransPlace.Text + "' ";


                                strSql += ", @pTOLERANCE_RATE  = '" + txtToleranceRate.Value + "' ";

                                strSql += ", @pDISCHGE_PORT = '" + txtDischgePort.Text + "' ";
                                strSql += ", @pLOADING_PORT = '" + txtLoadingPort.Text + "' ";
                                strSql += ", @pTRANS_METH  = '" + txtTransMeth.Text + "' ";
                                strSql += ", @pTRANS_COMP = '" + txtTransMeth.Text + "' ";
                                strSql += ", @pORIGIN_CD = '" + txtOrigin.Text + "' ";
                                strSql += ", @pORIGIN_COUNTRY = '" + txtOriginCountry.Text + "' ";

                                if (rdoCertOriginY.Checked == true) strSql += ", @pCERT_ORIGIN_YN = 'Y' ";
                                else strSql += ", @pCERT_ORIGIN_YN = 'N' ";

                                strSql += ", @pCHARGE_REMARK = '" + txtChargeRemark.Text + "' ";

                                if (rdoChargeA.Checked == true) strSql += ", @pCHARGE_CD = 'A' ";
                                else strSql += ", @pCHARGE_CD = 'B' ";

                                strSql += ", @pCREDIT_CORE = '" + txtCreditCore.Text + "' ";
                                strSql += ", @pFUND_TYPE = '" + txtFundType.Text + "' ";
                                if (txtLmtXchRate.Value.ToString() != "")
                                    strSql += ", @pLMT_XCH_RATE = '" + txtLmtXchRate.Value + "' ";

                                if (txtLmtAmt.Value.ToString() != "")
                                    strSql += ", @pLMT_AMT = '" + txtLmtAmt.Value + "' ";

                                if (txtInvCnt.Value.ToString() != "")
                                    strSql += ", @pINV_CNT = '" + txtInvCnt.Value + "' ";

                                if (rdoBlTypeBl.Checked == true) strSql += ", @pBL_TYPE = 'BL' ";
                                else strSql += ", @pBL_TYPE = 'AW' ";

                                strSql += ", @pFIRE_PAYMENT = '" + txtFirePayment.Text + "' ";
                                strSql += ", @pNOTIFY_CUST = '" + txtNotifyCust.Text + "' ";
                                strSql += ", @pCONSIGNEE = '" + txtConsignee.Text + "' ";
                                strSql += ", @pINSUR_POLICY = '" + txtInsurPolicy.Text + "' ";

                                if (txtPackListCnt.Value.ToString() != "")
                                    strSql += ", @pPACK_LIST_CNT = '" + txtPackListCnt.Value + "' ";

                                strSql += ", @pRENEGO_BANK = '" + txtRenegoBank.Text + "' ";
                                strSql += ", @pPAYMENT_BANK = '" + txtPaymentBank.Text + "' ";
                                strSql += ", @pRETURN_BANK = '" + txtReturnBank.Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoLcNo = dt.Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                            {

                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0027");
                                    dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_MLC001  'D1'";
                                            strDelSql += ", @pLC_NO = '" + strAutoLcNo + "' ";
                                            strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit1; }	// ER 코드 Return시 점프

                                            Trans.Commit();
                                        }
                                        catch (Exception f)
                                        {
                                            SystemBase.Loggers.Log(this.Name, f.ToString());
                                            Trans.Rollback();
                                            ERRCode = "ER";
                                            MSGCode = f.Message;
                                            //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();
                                        if (ERRCode == "OK")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Search("");
                                        }
                                        else if (ERRCode == "ER")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        else
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }

                                        return;
                                    }
                                    else
                                    {
                                        Trans.Rollback();
                                        dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        //작업이 취소되었습니다.
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                }

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
                                            case "I": strGbn = "I2"; break;
                                            case "D": strGbn = "D2"; break;
                                            default: strGbn = ""; break;
                                        }

                                        string strSql = " usp_MLC001 '" + strGbn + "'";
                                        strSql += ", @pLC_NO = '" + strAutoLcNo + "' ";
                                        if (strGbn == "I2") strSql += ", @pLC_SEQ = 0 ";
                                        else strSql += ", @pLC_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C순번")].Value;
                                        strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                        strSql += ", @pXCH_RATE = '" + txtExchRate.Value + "' ";
                                        strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
                                        strSql += ", @pPO_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                        strSql += ", @pLC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량")].Value + "' ";
                                        strSql += ", @pLC_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' "; ;
                                        strSql += ", @pLC_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C금액")].Value + "' ";
                                        strSql += ", @pLC_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C자국금액")].Value + "' ";
                                        strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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

                            /////////////////////////////////////////////// 금액집계 UPDATE 시작 /////////////////////////////////////////////////
                            if (NewFlg == 1) { strInUpFlag = "I"; }
                            else { strInUpFlag = "U"; }

                            string strSql1 = " usp_MLC001 'I3'";
                            strSql1 += ", @pLC_NO = '" + strAutoLcNo + "' ";
                            strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                            ERRCode = dt1.Rows[0][0].ToString();
                            if (ERRCode == "ER")
                                MSGCode = dt1.Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                            if (NewFlg == 1) Search(strAutoLcNo);
                            else SubSearch(strAutoLcNo);
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
            else
            {
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0006"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //L/C AMEND 정보가 등록되었으면 다른작업을 할 수 없습니다.				
            }
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //수량, 단가, 금액
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단가"))
            {
                Set_Amt(Row);
            }
        }
        #endregion

        #region 금액계산
        private void Set_Amt(int Row)
        {
            decimal Amt = 0;
            decimal LocAmt = 0;
            decimal Price = 0;
            decimal Qty = 0;

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
            if (Price != 0 && Qty != 0)
            {
                Amt = Price * Qty;
                LocAmt = Amt * Convert.ToDecimal(txtExchRate.Value);
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C금액")].Value = Amt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C자국금액")].Value = LocAmt;

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
                    strAutoLcNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "L/C번호")].Text.ToString();

                    c1DockingTab1.SelectedIndex = 0;
                    SubSearch(strAutoLcNo);
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

                SystemBase.Validation.GroupBox_Reset(groupBox3);
                SystemBase.Validation.GroupBox_Reset(groupBox4);
                SystemBase.Validation.GroupBox_Reset(groupBox5);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_MLC001  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pLC_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                //AMEND 등록 여부
                strAmend = dt.Rows[0]["AMEND_IS"].ToString();
                //PO 상태
                strSts = dt.Rows[0]["PO_STATUS"].ToString();

                txtLcNo.Text = dt.Rows[0]["LC_NO"].ToString();

                dtpReqDt.Value = dt.Rows[0]["REQ_DT"].ToString();

                if (dt.Rows[0]["OPEN_DT"].ToString() != "") dtpOpenDt.Value = dt.Rows[0]["OPEN_DT"].ToString();

                dtpExpiryDt.Value = dt.Rows[0]["EXPIRY_DT"].ToString();
                txtMakerCust.Value = dt.Rows[0]["MAKER_CUST"].ToString();
                txtMakerCustNm.Value = dt.Rows[0]["MAKER_CUST_NM"].ToString();
                txtAgentCust.Value = dt.Rows[0]["AGENT_CUST"].ToString();
                txtAgentCustNm.Value = dt.Rows[0]["AGENT_CUST_NM"].ToString();
                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                txtExchRate.Value = dt.Rows[0]["XCH_RATE"].ToString();

                txtLcAmt.Value = dt.Rows[0]["LC_AMT"].ToString();
                txtLcAmtLoc.Value = dt.Rows[0]["LC_AMT_LOC"].ToString();

                txtBankTxt.Value = dt.Rows[0]["BANK_TXT"].ToString();
                txtCostCond.Value = dt.Rows[0]["COST_COND"].ToString();
                txtCostCondNm.Value = dt.Rows[0]["COST_COND_NM"].ToString();
                txtPaymentMeth.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentMethNm.Value = dt.Rows[0]["PAYMENT_METH_NM"].ToString();
                txtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"].ToString();
                txtPaymentTermRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();

                if (rdoPartialShipY.Checked == true)

                    dtpLatestShipDt.Value = dt.Rows[0]["LATEST_SHIP_DT"].ToString();

                txtShipmentRemark.Value = dt.Rows[0]["SHIPMENT_REMARK"].ToString();

                c1DockingTab1.SelectedIndex = 1;

                txtFileDd.Value = dt.Rows[0]["FILE_DD"];
                txtFileDdTxt.Value = dt.Rows[0]["FILE_DD_TXT"].ToString();
                txtAttachDoc1.Value = dt.Rows[0]["ATTACH_DOC1"].ToString();
                txtAttachDoc2.Value = dt.Rows[0]["ATTACH_DOC2"].ToString();
                txtAttachDoc3.Value = dt.Rows[0]["ATTACH_DOC3"].ToString();
                txtAttachDoc4.Value = dt.Rows[0]["ATTACH_DOC4"].ToString();
                txtAttachDoc5.Value = dt.Rows[0]["ATTACH_DOC5"].ToString();

                txtEtcRemark.Value = dt.Rows[0]["ETC_REMARK"].ToString();
                txtOpenBank.Value = dt.Rows[0]["OPEN_BANK"].ToString();
                txtOpenBankNm.Value = dt.Rows[0]["OPEN_BANK_NM"].ToString();
                txtAdviceBank.Value = dt.Rows[0]["ADVICE_BANK"].ToString();
                txtAdviceBankNm.Value = dt.Rows[0]["ADVICE_BANK_NM"].ToString();
                txtApplicantCust.Value = dt.Rows[0]["APPLICANT_CUST"].ToString();
                txtApplicantCustNm.Value = dt.Rows[0]["APPLICANT_CUST_NM"].ToString();
                txtBeneficiaryCust.Value = dt.Rows[0]["BENEFICIARY_CUST"].ToString();
                txtBeneficiaryCustNm.Value = dt.Rows[0]["BENEFICIARY_CUST_NM"].ToString();
                txtPurOrg.Value = dt.Rows[0]["PUR_ORG"].ToString();
                txtPurOrgNm.Value = dt.Rows[0]["PUR_ORG_NM"].ToString();
                txtUserId.Value = dt.Rows[0]["PUR_DUTY"].ToString();
                txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                txtLcType.Value = dt.Rows[0]["LC_TYPE"].ToString();
                txtLcTypeNm.Value = dt.Rows[0]["LC_TYPE_NM"].ToString();

                if (dt.Rows[0]["TRANSHIPMENT_YN"].ToString() == "Y") rdoTranshipY.Checked = true;
                else rdoTranshipN.Checked = true;

                if (dt.Rows[0]["TRANSFER_YN"].ToString() == "Y") rdoTransferY.Checked = true;
                else rdoTransferN.Checked = true;

                txtTransPlace.Value = dt.Rows[0]["TRANS_PLACE"].ToString();
                txtToleranceRate.Value = dt.Rows[0]["TOLERANCE_RATE"];
                txtDischgePort.Value = dt.Rows[0]["DISCHGE_PORT"].ToString();
                txtDischgePortNm.Value = dt.Rows[0]["DISCHGE_PORT_NM"].ToString();
                txtLoadingPort.Value = dt.Rows[0]["LOADING_PORT"].ToString();
                txtLoadingPortNm.Value = dt.Rows[0]["LOADING_PORT_NM"].ToString();
                txtTransMeth.Value = dt.Rows[0]["TRANS_METH"].ToString();
                txtTransMethNm.Value = dt.Rows[0]["TRANS_METH_NM"].ToString();
                txtOrigin.Value = dt.Rows[0]["ORIGIN_CD"].ToString();
                txtOriginNm.Value = dt.Rows[0]["ORIGIN_NM"].ToString();
                txtOriginCountry.Value = dt.Rows[0]["ORIGIN_NM"].ToString();

                if (dt.Rows[0]["CERT_ORIGIN_YN"].ToString() == "Y") rdoCertOriginY.Checked = true;
                else rdoCertOriginN.Checked = true;

                txtChargeRemark.Value = dt.Rows[0]["CHARGE_REMARK"].ToString();

                if (dt.Rows[0]["CHARGE_CD"].ToString() == "A") rdoChargeA.Checked = true;
                else rdoChargeB.Checked = true;

                txtCreditCore.Value = dt.Rows[0]["CREDIT_CORE"].ToString();
                txtCreditCoreNm.Value = dt.Rows[0]["CREDIT_CORE_NM"].ToString();
                txtFundType.Value = dt.Rows[0]["FUND_TYPE"].ToString();
                txtFundTypeNm.Value = dt.Rows[0]["FUND_TYPE_NM"].ToString();
                txtLmtXchRate.Value = dt.Rows[0]["LMT_XCH_RATE"];
                txtLmtAmt.Value = dt.Rows[0]["LMT_AMT"];
                txtInvCnt.Value = dt.Rows[0]["INV_CNT"];

                if (dt.Rows[0]["BL_TYPE"].ToString() == "B") rdoBlTypeBl.Checked = true;
                else rdoBlTypeAw.Checked = true;

                c1DockingTab1.SelectedIndex = 2;

                txtFirePayment.Value = dt.Rows[0]["FIRE_PAYMENT"].ToString();
                txtFirePaymentNm.Value = dt.Rows[0]["FIRE_PAYMENT_NM"].ToString();
                txtNotifyCust.Value = dt.Rows[0]["NOTIFY_CUST"].ToString();
                txtNotifyCustNm.Value = dt.Rows[0]["NOTIFY_CUST_NM"].ToString();
                txtConsignee.Value = dt.Rows[0]["CONSIGNEE"].ToString();
                txtInsurPolicy.Value = dt.Rows[0]["INSUR_POLICY"].ToString();
                txtPackListCnt.Value = dt.Rows[0]["PACK_LIST_CNT"];
                txtRenegoBank.Value = dt.Rows[0]["RENEGO_BANK"].ToString();
                txtRenegoBankNm.Value = dt.Rows[0]["RENEGO_BANK_NM"].ToString();
                txtPaymentBank.Value = dt.Rows[0]["PAYMENT_BANK"].ToString();
                txtPaymentBankNm.Value = dt.Rows[0]["PAYMENT_BANK_NM"].ToString();
                txtReturnBank.Value = dt.Rows[0]["RETURN_BANK"].ToString();
                txtReturnBankNm.Value = dt.Rows[0]["RETURN_BANK_NM"].ToString();

                if (dt.Rows[0]["ADV_DT"].ToString() != "") dtpAdvDt.Value = dt.Rows[0]["ADV_DT"].ToString();
                if (dt.Rows[0]["AMEND_DT"].ToString() != "") dtpAmendDt.Value = dt.Rows[0]["AMEND_DT"].ToString();

                if (strAmend == "Y" || Convert.ToInt16(strSts) >= 2) //BL
                {
                    butPoRef.Enabled = false;
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
                }
                else
                {
                    butPoRef.Enabled = true;
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox5, false);
                }

                txtLcNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtLcNo.ReadOnly = true;

                c1DockingTab1.SelectedIndex = 0;

                //Detail그리드 정보.
                string strSql1 = " usp_MLC001  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pLC_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);


                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    //AMEND 여부에 따른 화면 Locking
                    if (strAmend == "Y" || Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주상태")].Text) >= 2)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/C금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/C자국금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );
                    }
                    else
                    {
                        //Detail Locking해제							
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "L/C수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/C금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "L/C자국금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
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
            strBtn = "N";
            this.Cursor = Cursors.Default;
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

        #region 버튼 Click
        private void butSUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSUserId.Value = Msgs[0].ToString();
                    txtSUserNm.Value = Msgs[1].ToString();
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


        private void butSCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtSCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCustCd.Value = Msgs[1].ToString();
                    txtSCustNm.Value = Msgs[2].ToString();
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

        private void btnSCostCond_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S005' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSCostCond.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSCostCond.Value = Msgs[0].ToString();
                    txtSCostCondNm.Value = Msgs[1].ToString();
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
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Value = Msgs[3].ToString();
                    txtProjectSeq.Value = "";
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
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
                    txtProjectSeq.Value = Msgs[0].ToString();
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
        private void butTransMeth_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S013' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTransMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00035", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운송방법 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTransMeth.Value = Msgs[0].ToString();
                    txtTransMethNm.Value = Msgs[1].ToString();
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


        private void btnNotifyCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtNotifyCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtNotifyCust.Value = Msgs[1].ToString();
                    txtNotifyCustNm.Value = Msgs[2].ToString();
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

        private void btnAgentCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtAgentCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtAgentCust.Value = Msgs[1].ToString();
                    txtAgentCustNm.Value = Msgs[2].ToString();
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

        private void butMaker_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtMakerCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakerCust.Value = Msgs[1].ToString();
                    txtMakerCustNm.Value = Msgs[2].ToString();
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



        private void butOrigin_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S006' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOrigin.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00037", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtOrigin.Value = Msgs[0].ToString();
                    txtOriginNm.Value = Msgs[1].ToString();
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

        private void btnDischgePort_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S009' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDischgePort.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00040", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "도착항 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDischgePort.Value = Msgs[0].ToString();
                    txtDischgePortNm.Value = Msgs[1].ToString();
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

        private void butLoadingPort_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S009' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLoadingPort.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00041", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선적항 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtLoadingPort.Value = Msgs[0].ToString();
                    txtLoadingPortNm.Value = Msgs[1].ToString();
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



        private void btnAdviceBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAdviceBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtAdviceBank.Value = Msgs[0].ToString();
                    txtAdviceBankNm.Value = Msgs[1].ToString();
                    txtOpenBank.Value = Msgs[0].ToString();
                    txtOpenBankNm.Value = Msgs[1].ToString();

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

        private void btnOpenBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOpenBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtOpenBank.Value = Msgs[0].ToString();
                    txtOpenBankNm.Value = Msgs[1].ToString();
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

        private void btnPaymentBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPaymentBank.Value = Msgs[0].ToString();
                    txtPaymentBankNm.Value = Msgs[1].ToString();
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

        private void btnRenegoBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtRenegoBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtRenegoBank.Value = Msgs[0].ToString();
                    txtRenegoBankNm.Value = Msgs[1].ToString();
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

        private void btnReturnBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReturnBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReturnBank.Value = Msgs[0].ToString();
                    txtReturnBankNm.Value = Msgs[1].ToString();
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


        private void btnLcType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S032' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLcType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00042", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "L/C유형 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtLcType.Value = Msgs[0].ToString();
                    txtLcTypeNm.Value = Msgs[1].ToString();
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

        private void btnFirePayment_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S028' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtFirePayment.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00043", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운임지불형태 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtFirePayment.Value = Msgs[0].ToString();
                    txtFirePaymentNm.Value = Msgs[1].ToString();
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

        private void btnOriginCountry_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='B006' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOriginCountry.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00044", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지국가 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtOriginCountry.Value = Msgs[0].ToString();
                    txtOriginCountryNm.Value = Msgs[1].ToString();
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

        private void btnFundType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S027' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtFundType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00045", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "자금종류 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtFundType.Value = Msgs[0].ToString();
                    txtFundTypeNm.Value = Msgs[1].ToString();
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

        private void btnCreditCore_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S026' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCreditCore.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00046", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "신공여주체 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCreditCore.Value = Msgs[0].ToString();
                    txtCreditCoreNm.Value = Msgs[1].ToString();
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

        #region 발주참조버튼
        private void butPoRef_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                MLC001P1 frm1 = new MLC001P1(fpSpread1);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;

                    if (Msgs != null)
                    {
                        txtCostCond.Value = Msgs[0].ToString();
                        txtCostCondNm.Value = Msgs[1].ToString();
                        txtPaymentMeth.Value = Msgs[2].ToString();
                        txtPaymentMethNm.Value = Msgs[3].ToString();
                        cboCurrency.SelectedValue = Msgs[4].ToString();
                        txtBeneficiaryCust.Value = Msgs[5].ToString();
                        txtBeneficiaryCustNm.Value = Msgs[6].ToString();
                        txtApplicantCust.Value = Msgs[7].ToString();
                        txtApplicantCustNm.Value = Msgs[8].ToString();
                        txtUserId.Value = Msgs[9].ToString();
                        txtUserNm.Value = Msgs[10].ToString();
                        txtPurOrg.Value = Msgs[11].ToString();
                        txtPurOrgNm.Value = Msgs[12].ToString();

                        txtTransMeth.Value = Msgs[13].ToString();
                        txtTransMethNm.Value = Msgs[14].ToString();
                        txtDischgePort.Value = Msgs[15].ToString();
                        txtDischgePortNm.Value = Msgs[16].ToString();
                        txtLoadingPort.Value = Msgs[17].ToString();
                        txtLoadingPortNm.Value = Msgs[18].ToString();
                        txtOrigin.Value = Msgs[19].ToString();
                        txtOriginNm.Value = Msgs[20].ToString();
                        txtExchRate.Value = Msgs[21];
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }
        #endregion

        #region TextChanged
        private void txtSCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {                
                if (strBtn == "N")
                {
                    if (txtSCustCd.Text != "")
                    {
                        txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSCostCond.Text != "")
                    {
                        txtSCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSCostCond.Text, " AND MAJOR_CD = 'S005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSCostCondNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtTransMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTransMeth.Text != "")
                    {
                        txtTransMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTransMeth.Text, " AND MAJOR_CD = 'S013' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtTransMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtProjectSeq.Value = "";
        }
        private void txtAdviceBank_TextChanged(object sender, System.EventArgs e)
        {
            
            try
            {
                if (strBtn == "N")
                {
                    if (txtAdviceBank.Text != "")
                    {
                        txtAdviceBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtAdviceBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtAdviceBankNm.Value = "";
                    }
                    if (txtOpenBank.Text.Trim() == "")
                    {
                        txtOpenBank.Text = txtAdviceBank.Text;
                        txtOpenBankNm.Text = txtAdviceBankNm.Text;
                    }
                }                
            }
            catch
            {

            }
        }

        private void txtOpenBank_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtOpenBank.Text != "")
                    {
                        txtOpenBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtOpenBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtOpenBankNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtPaymentBank_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtPaymentBank.Text != "")
                    {
                        txtPaymentBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtPaymentBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPaymentBankNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtRenegoBank_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtRenegoBank.Text != "")
                    {
                        txtRenegoBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtRenegoBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtRenegoBankNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtReturnBank_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtReturnBank.Text != "")
                    {
                        txtReturnBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtReturnBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtReturnBankNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtNotifyCust_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtNotifyCust.Text != "")
                    {
                        txtNotifyCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtNotifyCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtNotifyCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtMakerCust_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtMakerCust.Text != "")
                    {
                        txtMakerCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtMakerCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtMakerCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtAgentCust_TextChanged(object sender, System.EventArgs e)
        {           
            try
            {
                if (strBtn == "N")
                {
                    if (txtAgentCust.Text != "")
                    {
                        txtAgentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtAgentCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtAgentCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtOrigin_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtOrigin.Text != "")
                    {
                        txtOriginNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtOrigin.Text, " AND MAJOR_CD = 'S006' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtOriginNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtDischgePort_TextChanged(object sender, System.EventArgs e)
        {           
            try
            {
                if (strBtn == "N")
                {
                    if (txtDischgePort.Text != "")
                    {
                        txtDischgePortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDischgePort.Text, " AND MAJOR_CD = 'S009' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtDischgePortNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtLoadingPort_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtLoadingPort.Text != "")
                    {
                        txtLoadingPortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtLoadingPort.Text, " AND MAJOR_CD = 'S009' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtLoadingPortNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtLcType_TextChanged(object sender, System.EventArgs e)
        {           
            try
            {
                if (strBtn == "N")
                {
                    if (txtLcType.Text != "")
                    {
                        txtLcTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtLcType.Text, " AND MAJOR_CD = 'S032' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtLcTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtOriginCountry_TextChanged(object sender, System.EventArgs e)
        {           
            try
            {
                if (strBtn == "N")
                {
                    if (txtOriginCountry.Text != "")
                    {
                        txtOriginCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtOriginCountry.Text, " AND MAJOR_CD = 'B006' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtOriginCountryNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtFirePayment_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtFirePayment.Text != "")
                    {
                        txtFirePaymentNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFirePayment.Text, " AND MAJOR_CD = 'S028' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtFirePaymentNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtCreditCore_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtCreditCore.Text != "")
                    {
                        txtCreditCoreNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCreditCore.Text, " AND MAJOR_CD = 'S026' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCreditCoreNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtFundType_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N")
                {
                    if (txtFundType.Text != "")
                    {
                        txtFundTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFundType.Text, " AND MAJOR_CD = 'S027' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtFundTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtSUserId_TextChanged(object sender, System.EventArgs e)
        {            
            try
            {
                if (strBtn == "N" && txtSUserId.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtSUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtSUserId.Text != "")
                        {
                            txtSUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtSUserNm.Value = "";
                        }
                    }
                }
                else if (txtSUserId.Text.Trim() == "") txtSUserNm.Value = "";               
            }
            catch
            {

            }
        }

        #endregion

        #region Text Leave
        private void txtExchRate_Leave(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                if (strHead == "")
                { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
                Set_Amt(i);

            }
        }
        #endregion

        #region MLC001_Activated
        private void MLC001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpReqDtFr.Focus();
        }

        private void MLC001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
        
    }
}