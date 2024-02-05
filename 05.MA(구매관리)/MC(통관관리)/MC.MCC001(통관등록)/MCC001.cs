#region 작성정보
/*********************************************************************/
// 단위업무명 : 통관등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-01
// 작성내용 : 통관등록 및 관리
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

namespace MC.MCC001
{
    public partial class MCC001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoCcNo = ""; //통관번호
        string strBtn = "N";
        string strSts = "";
        bool btnNew_is = true;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public MCC001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MCC001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //화폐단위

            //그리드 콤보박스 세팅
            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
            
            //기타 세팅
            //조회조건
            dtpSOpenDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSOpenDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            //GorupBox3입력조건
            dtpReportDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpArriveDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpLicenseDt.Text = "";
            dtpCarryBackDt.Text = "";

            NewFlg = 1;
            strAutoCcNo = "";

            c1DockingTab1.SelectedIndex = 0;

            butBlRef.Enabled = true;

            txtCcAmt.BackColor = SystemBase.Validation.Kind_Gainsboro;
            txtCcAmt.ReadOnly = true;

            txtCcAmtLoc.BackColor = SystemBase.Validation.Kind_Gainsboro;
            txtCcAmtLoc.ReadOnly = true;

            txtXchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
            txtXchRate.ReadOnly = true;

            txtCifAmtLoc.BackColor = SystemBase.Validation.Kind_Gainsboro;
            txtCifAmtLoc.ReadOnly = true;

            txtDischgePort.Text = "INC";
            txtTransMeth.Text = "AIR";
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSOpenDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpSOpenDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            }

            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅

            dtpReportDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpReportReqDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpArriveDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            NewFlg = 1;
            strAutoCcNo = "";

            txtDischgePort.Text = "INC";
            txtTransMeth.Text = "AIR";

            c1DockingTab1.SelectedIndex = 0;

            butBlRef.Enabled = true;

            fpSpread2.Sheets[0].Rows.Count = 0;	
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (Convert.ToInt16(strSts) >= 4) // 입고
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
                    string strSql = " usp_MCC001  'D1'";
                    strSql += ", @pCC_NO = '" + strAutoCcNo + "' ";
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

        private void Search(string strCcNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_MCC001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pREPORT_DT_FR = '" + dtpSOpenDtFr.Text + "' ";
                strQuery += ", @pREPORT_DT_TO = '" + dtpSOpenDtTo.Text + "' ";
                strQuery += ", @pID_NO = '" + txtSIdNo.Text + "' ";
                strQuery += ", @pBENEFICIARY_CUST= '" + txtSBeneficiaryCust.Text + "' ";
                strQuery += ", @pPAYMENT_METH = '" + txtSPaymentMeth.Text + "' ";
                strQuery += ", @pCOST_COND= '" + txtSCostCond.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "' ";
                strQuery += ", @pCC_NO = '" + txtSCcNo.Text + "' ";
                strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strCcNo != "")
                    {
                        fpSpread2.Search(0, strCcNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }
                    }
                    strAutoCcNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "통관번호")].Text;
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoCcNo);
                }
                else
                {
                    NewFlg = 1;
                    strAutoCcNo = "";
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
            txtIdNo.Focus();
            DialogResult dsMsg;
            string strMstType = "";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////

            //상단 그룹박스 필수 체크
            if (SystemBase.Base.GroupBoxExceptions(groupBox3))
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

                            string strSql = " usp_MCC001 '" + strMstType + "'";
                            strSql += ", @pCC_NO = '" + txtCcNo.Text.Trim() + "' ";

                            strSql += ", @pID_NO = '" + txtIdNo.Text + "' ";
                            strSql += ", @pREPORT_DT = '" + dtpReportDt.Text + "' ";
                            strSql += ", @pREPORT_REQ_DT = '" + dtpReportReqDt.Text + "' ";
                            strSql += ", @pARRIVE_DT = '" + dtpArriveDt.Text + "' ";
                            if (dtpLicenseDt.Text != "") strSql += ", @pLICENSE_DT = '" + dtpLicenseDt.Text + "' ";
                            if (dtpCarryBackDt.Text != "") strSql += ", @pCARRY_BACK_DT = '" + dtpCarryBackDt.Text + "' ";

                            strSql += ", @pCUSTOM_OFFICE = '" + txtCustomOffice.Text + "' ";
                            strSql += ", @pCOLLECT_TYPE  = '" + txtCollectType.Text + "' ";
                            strSql += ", @pLICENSE_NO = '" + txtLicenseNo.Text + "' ";
                            strSql += ", @pCARRY_BACK_NO = '" + txtCarryBackNo.Text + "' ";
                            strSql += ", @pTAX_PAYMENT_CUST = '" + txtTaxPaymentCust.Text + "' ";
                            strSql += ", @pREPORT_CUST = '" + txtReportCust.Text + "' ";
                            strSql += ", @pCC_PLAN = '" + txtCcPlan.Text + "' ";

                            strSql += ", @pREPORT_DIV = '" + txtReportDiv.Text + "' ";
                            strSql += ", @pDEAL_DIV = '" + txtDealDiv.Text + "' ";
                            strSql += ", @pIMPORT_KIND = '" + txtImportKind.Text + "' ";
                            strSql += ", @pTRANS_METH = '" + txtTransMeth.Text + "' ";
                            strSql += ", @pPACK_TYPE = '" + txtPackType.Text + "'";

                            strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue + "'";

                            if (txtTotPackingCnt.ValueIsDbNull == false)
                                strSql += ", @pTOT_PACKING_CNT = '" + txtTotPackingCnt.Value + "' ";
                            
                            if (txtXchRate.ValueIsDbNull == false)
                                strSql += ", @pEXCH_RATE = '" + txtXchRate.Value + "' ";

                            if (txtCifAmt.ValueIsDbNull == false)
                                strSql += ", @pCIF_AMT = '" + txtCifAmt.Value + "' ";

                            if (txtCifAmtLoc.ValueIsDbNull == false)
                                strSql += ", @pCIF_AMT_LOC = '" + txtCifAmtLoc.Value + "' ";

                            if (txtUsdXchRate.ValueIsDbNull == false)
                                strSql += ", @pUSD_EXCH_RATE = '" + txtUsdXchRate.Value + "' ";

                            strSql += ", @pDISCHGE_PORT = '" + txtDischgePort.Text + "' ";
                            strSql += ", @pBENEFICIARY_CUST = '" + txtBeneficiaryCust.Text + "' ";
                            strSql += ", @pAPPLICANT_CUST = '" + txtApplicantCust.Text + "' ";

                            strSql += ", @pCOST_COND = '" + txtCostCond.Text + "' ";
                            strSql += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";
                            strSql += ", @pREMARK = '" + txtRemark.Text + "' ";

                            strSql += ", @pSHIP_COUNTRY= '" + txtShipCountry.Text + "' ";
                            strSql += ", @pVESSEL_NAME = '" + txtVesselName.Text + "' ";
                            strSql += ", @pLOADING_PORT = '" + txtLoadingPort.Text + "' ";
                            strSql += ", @pPICK_OUT_COUNTRY = '" + txtPickOutCountry.Text + "' ";
                            strSql += ", @pDEVICE_NO = '" + txtDeviceNo.Text + "' ";
                            strSql += ", @pCARRY_BACK_PLACE = '" + txtCarryBackPlace.Text + "' ";
                            strSql += ", @pCHECKUP_TXT = '" + txtCheckUpTxt.Text + "' ";

                            if (dtpShippingDt.Text != "") strSql += ", @pSHIPPING_DT = '" + dtpShippingDt.Text + "' ";
                            if (dtpInspectDt.Text != "") strSql += ", @pINSPECT_DT = '" + dtpInspectDt.Text + "' ";
                            if (dtpCarrOutDt.Text != "") strSql += ", @pCARRY_OUT_DT = '" + dtpCarrOutDt.Text + "' ";
                            if (dtpExpiryDt.Text != "") strSql += ", @pEXPIRY_DT = '" + dtpExpiryDt.Text + "' ";

                            strSql += ", @pORIGIN_CD = '" + txtOriginCd.Text + "' ";
                            strSql += ", @pPACKING_NO = '" + txtPackingNo.Text + "' ";
                            strSql += ", @pORIGIN_COUNTRY = '" + txtOriginCountry.Text + "' ";
                            strSql += ", @pPAYMENT_DOC_NO = '" + txtPaymentDocNo.Text + "' ";

                            if (dtpPaymentDt.Text != "") strSql += ", @pPAYMENT_DT = '" + dtpPaymentDt.Text + "' ";

                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();
                            strAutoCcNo = dt.Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }

                        /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                        {

                            //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                            if (DelCheck() == false)
                            {
                                string msg = SystemBase.Base.MessageRtn("B0027");
                                dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (dsMsg == DialogResult.Yes)
                                {
                                    try
                                    {
                                        string strDelSql = " usp_MCC001  'D1'";
                                        strDelSql += ", @pCC_NO = '" + strAutoCcNo + "' ";
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
                                    
                                    string strSql = " usp_MCC001 '" + strGbn + "'";
                                    strSql += ", @pCC_NO = '" + strAutoCcNo + "' ";
                                    if (strGbn == "I2") strSql += ", @pCC_SEQ = 0 ";
                                    else strSql += ", @pCC_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관순번")].Value;
                                    strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue + "'";
                                    if (txtXchRate.ValueIsDbNull == false)
                                        strSql += ", @pEXCH_RATE = '" + txtXchRate.Value + "' ";
                                    strSql += ", @pBL_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장번호")].Text + "' ";
                                    strSql += ", @pBL_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "송장순번")].Text + "' ";
                                    strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                    strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                    strSql += ", @pCC_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value + "' ";
                                    strSql += ", @pCC_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' "; ;
                                    strSql += ", @pCC_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관금액")].Value + "' ";
                                    strSql += ", @pCC_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                    //										strSql += ", @pNET_WEIGHT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순중량")].Value + "' ";						
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

                        if (NewFlg == 1) Search(strAutoCcNo);
                        else SubSearch(strAutoCcNo);

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

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            DialogResult dsMsg;
            //수량, 단가, 금액
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량"))
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text == "I")
                {
                    if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value.ToString())
                        > Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value.ToString()))
                    {
                        string msg = "통관수량이 " + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Text + " 보다 크면 안됩니다!";
                        dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value;
                    }
                }
                else
                {
                    decimal sum = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value.ToString())
                                    + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value.ToString());
                    if (sum < Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value.ToString()))
                    {
                        string msg = "통관수량이 " + Convert.ToInt32(sum).ToString() + " 이하여야 됩니다!";
                        dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L잔량")].Value;
                    }
                }
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

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
            if (Price != 0 && Qty != 0 && txtXchRate.ValueIsDbNull == false)
            {
                Amt = Price * Qty;
                LocAmt = Amt * Convert.ToDecimal(txtXchRate.Value);
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "통관금액")].Value = Amt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value = LocAmt;

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
                    strAutoCcNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "통관번호")].Text.ToString();

                    c1DockingTab1.SelectedIndex = 0;
                    SubSearch(strAutoCcNo);
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

                txtCcAmt.ReadOnly = false;
                txtCcAmtLoc.ReadOnly = false;
                txtXchRate.ReadOnly = false;
                txtCifAmtLoc.ReadOnly = false;

                fpSpread1.Sheets[0].Rows.Count = 0;

                //통관Master정보
                string strSql = " usp_MCC001  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pCC_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                txtCcNo.Value = dt.Rows[0]["CC_NO"].ToString();

                txtIdNo.Value = dt.Rows[0]["ID_NO"].ToString();
                dtpReportDt.Value = dt.Rows[0]["REPORT_DT"].ToString();  //신고일자	 
                dtpReportReqDt.Value = dt.Rows[0]["REPORT_REQ_DT"].ToString(); //신고요청일자	

                dtpArriveDt.Value = dt.Rows[0]["ARRIVE_DT"].ToString();	//도착일자	
                if (dt.Rows[0]["LICENSE_DT"].ToString() != "") dtpLicenseDt.Value = dt.Rows[0]["LICENSE_DT"].ToString();
                if (dt.Rows[0]["CARRY_BACK_DT"].ToString() != "") dtpCarryBackDt.Value = dt.Rows[0]["CARRY_BACK_DT"].ToString();

                txtCustomOffice.Value = dt.Rows[0]["CUSTOM_OFFICE"].ToString();
                txtCustomOfficeNm.Value = dt.Rows[0]["CUSTOM_OFFICE_NM"].ToString();
                txtCollectType.Value = dt.Rows[0]["COLLECT_TYPE"].ToString();
                txtCollectTypeNm.Value = dt.Rows[0]["COLLECT_TYPE_NM"].ToString();
                txtLicenseNo.Value = dt.Rows[0]["LICENSE_NO"].ToString();
                txtCarryBackNo.Value = dt.Rows[0]["CARRY_BACK_NO"].ToString();
                txtTaxPaymentCust.Value = dt.Rows[0]["TAX_PAYMENT_CUST"].ToString();
                txtTaxPaymentCustNm.Value = dt.Rows[0]["TAX_PAYMENT_CUST_NM"].ToString();

                txtReportCust.Value = dt.Rows[0]["REPORT_CUST"].ToString();
                txtReportCustNm.Value = dt.Rows[0]["REPORT_CUST_NM"].ToString();
                txtCcPlan.Value = dt.Rows[0]["CC_PLAN"].ToString();
                txtCcPlanNm.Value = dt.Rows[0]["CC_PLAN_NM"].ToString();

                txtReportDiv.Value = dt.Rows[0]["REPORT_DIV"].ToString();
                txtReportDivNm.Value = dt.Rows[0]["REPORT_DIV_NM"].ToString();
                txtDealDiv.Value = dt.Rows[0]["DEAL_DIV"].ToString();
                txtDealDivNm.Value = dt.Rows[0]["DEAL_DIV_NM"].ToString();
                txtImportKind.Value = dt.Rows[0]["IMPORT_KIND"].ToString();
                txtImportKindNm.Value = dt.Rows[0]["IMPORT_KIND_NM"].ToString();
                txtTransMeth.Value = dt.Rows[0]["TRANS_METH"].ToString();
                txtTransMethNm.Value = dt.Rows[0]["TRANS_METH_NM"].ToString();
                txtPackType.Value = dt.Rows[0]["PACK_TYPE"].ToString();
                txtPackTypeNm.Value = dt.Rows[0]["PACK_TYPE_NM"].ToString();

                txtCcPlan.Value = dt.Rows[0]["CC_PLAN"].ToString();
                txtCcPlanNm.Value = dt.Rows[0]["CC_PLAN_NM"].ToString();

                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"].ToString();

                txtCcAmt.Value = dt.Rows[0]["CC_AMT"];
                txtCcAmtLoc.Value = dt.Rows[0]["CC_AMT_LOC"];
                txtCifAmt.Value = dt.Rows[0]["CIF_AMT"];
                txtCifAmtLoc.Value = dt.Rows[0]["CIF_AMT_LOC"];
                txtTotPackingCnt.Value = dt.Rows[0]["TOT_PACKING_CNT"];
                txtXchRate.Value = dt.Rows[0]["EXCH_RATE"];
                txtUsdXchRate.Value = dt.Rows[0]["USD_EXCH_RATE"];

                txtCcAmt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtCcAmt.ReadOnly = true;

                txtCcAmtLoc.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtCcAmtLoc.ReadOnly = true;

                txtXchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtXchRate.ReadOnly = true;

                txtCifAmtLoc.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtCifAmtLoc.ReadOnly = true;

                txtDischgePort.Value = dt.Rows[0]["DISCHGE_PORT"].ToString();
                txtDischgePortNm.Value = dt.Rows[0]["DISCHGE_PORT_NM"].ToString();
                txtCostCond.Value = dt.Rows[0]["COST_COND"].ToString();
                txtCostCondNm.Value = dt.Rows[0]["COST_COND_NM"].ToString();
                txtPaymentMeth.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentMethNm.Value = dt.Rows[0]["PAYMENT_METH_NM"].ToString();
                txtBeneficiaryCust.Value = dt.Rows[0]["BENEFICIARY_CUST"].ToString();
                txtBeneficiaryCustNm.Value = dt.Rows[0]["BENEFICIARY_CUST_NM"].ToString();
                txtApplicantCustNm.Value = dt.Rows[0]["APPLICANT_CUST_NM"].ToString();
                txtApplicantCust.Value = dt.Rows[0]["APPLICANT_CUST"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                txtShipCountry.Value = dt.Rows[0]["SHIP_COUNTRY"].ToString();
                txtShipCountryNm.Value = dt.Rows[0]["SHIP_COUNTRY_NM"].ToString();
                txtVesselName.Value = dt.Rows[0]["VESSEL_NAME"].ToString();
                txtLoadingPort.Value = dt.Rows[0]["LOADING_PORT"].ToString();
                txtLoadingPortNm.Value = dt.Rows[0]["LOADING_PORT_NM"].ToString();
                txtPickOutCountry.Value = dt.Rows[0]["PICK_OUT_COUNTRY"].ToString();
                txtPickOutCountryNm.Value = dt.Rows[0]["PICK_OUT_COUNTRY_NM"].ToString();
                txtDeviceNo.Value = dt.Rows[0]["DEVICE_NO"].ToString();
                txtCarryBackPlace.Value = dt.Rows[0]["CARRY_BACK_PLACE"].ToString();
                txtCheckUpTxt.Value = dt.Rows[0]["CHECKUP_TXT"].ToString();

                if (dt.Rows[0]["SHIPPING_DT"].ToString() != "") dtpShippingDt.Value = dt.Rows[0]["SHIPPING_DT"].ToString();
                if (dt.Rows[0]["INSPECT_DT"].ToString() != "") dtpInspectDt.Value = dt.Rows[0]["INSPECT_DT"].ToString();
                if (dt.Rows[0]["CARRY_OUT_DT"].ToString() != "") dtpCarrOutDt.Value = dt.Rows[0]["CARRY_OUT_DT"].ToString();
                if (dt.Rows[0]["EXPIRY_DT"].ToString() != "") dtpExpiryDt.Value = dt.Rows[0]["EXPIRY_DT"].ToString();

                txtOriginCd.Value = dt.Rows[0]["ORIGIN_CD"].ToString();
                txtOriginNm.Value = dt.Rows[0]["ORIGIN_NM"].ToString();
                txtPackingNo.Value = dt.Rows[0]["PACKING_NO"].ToString();

                txtOriginCountry.Value = dt.Rows[0]["ORIGIN_COUNTRY"].ToString();
                txtOriginCountryNm.Value = dt.Rows[0]["ORIGIN_COUNTRY_NM"].ToString();
                txtPaymentDocNo.Value = dt.Rows[0]["PAYMENT_DOC_NO"].ToString();
                if (dt.Rows[0]["PAYMENT_DT"].ToString() != "") dtpPaymentDt.Value = dt.Rows[0]["PAYMENT_DT"].ToString();

                //PO 상태
                strSts = dt.Rows[0]["PO_STATUS"].ToString();

                if (Convert.ToInt16(strSts) >= 4)  //입고
                {
                    butBlRef.Enabled = false;
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

                }
                else
                {
                    butBlRef.Enabled = true;
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
                }

                txtCcNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtCcNo.ReadOnly = true;

                //Detail그리드 정보.
                string strSql1 = " usp_MCC001  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pCC_NO ='" + strCode + "' ";
                strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 3);

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주상태")].Text) >= 4)
                        //Detail Locking설정
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|3"	
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );
                    else
                        //Detail Locking해제
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "통관수량") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|1"		
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                            );


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

        #region 조회조건 팝업
        //수출자
        private void btnSBeneficiaryCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBeneficiaryCust.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSBeneficiaryCust.Value = Msgs[1].ToString();
                    txtSBeneficiaryCustNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //결재방법
        private void btnSPaymentMeth_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결재방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSPaymentMeth.Value = Msgs[0].ToString();
                    txtSPaymentMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //가격조건
        private void btnSCostCond_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S005', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSCostCond.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
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
                string[] strSearch = new string[] { "", "" };

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
        #endregion

        #region GroupBox3 입력조건 팝업
        //세관
        private void btnCustomOffice_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S033', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCustomOffice.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00047", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세관 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCustomOffice.Value = Msgs[0].ToString();
                    txtCustomOfficeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //징수형태
        private void btnCollectType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M016', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCollectType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00069", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "징수형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCollectType.Value = Msgs[0].ToString();
                    txtCollectTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnCcPlan_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M009', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCcPlan.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00070", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "통관계획 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCcPlan.Value = Msgs[0].ToString();
                    txtCcPlanNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //신고구분
        private void btnReportDiv_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M010', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtReportDiv.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00048", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "신고구분 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtReportDiv.Value = Msgs[0].ToString();
                    txtReportDivNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //신고자
        private void btnReportCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002("P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtReportCust.Value = Msgs[1].ToString();
                    txtReportCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
            strBtn = "N";
        }

        //거래구분
        private void btnDealDiv_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDealDiv.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00049", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "거래구분 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDealDiv.Value = Msgs[0].ToString();
                    txtDealDivNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        //운송방법
        private void btnTransMeth_Click(object sender, System.EventArgs e)
        {

            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S013', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTransMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00035", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운송방법 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }

        }



        //납세의무자
        private void btnTaxPaymentCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002("P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtTaxPaymentCust.Text = Msgs[1].ToString();
                    txtTaxPaymentCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ;
            }
            strBtn = "N";
        }

        //수입종류
        private void btnImportKind_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'M012', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtImportKind.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00050", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수입종류 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtImportKind.Value = Msgs[0].ToString();
                    txtImportKindNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //포장형태
        private void btnPackType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S007', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPackType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00017", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "포장형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPackType.Value = Msgs[0].ToString();
                    txtPackTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //도착항
        private void btnDischgePort_Click(object sender, System.EventArgs e)
        {

            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S009', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDischgePort.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00040", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "도착항 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }

        }
        #endregion

        #region GroupBox4 입력조건 팝업
        //선박국적
        private void btnShipCountry_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtShipCountry.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00016", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선박국적 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtShipCountry.Value = Msgs[0].ToString();
                    txtShipCountryNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //선적항
        private void btnLoadingPort_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S009', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLoadingPort.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00041", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선적항 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //적출국가
        private void btnPickOutCountry_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPickOutCountry.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00044", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "적출국가 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPickOutCountry.Value = Msgs[0].ToString();
                    txtPickOutCountryNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //원산지
        private void btnOrigin_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOriginCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00041", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtOriginCd.Value = Msgs[0].ToString();
                    txtOriginNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //원산지국가
        private void btnOriginCountry_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOriginCountry.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00044", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지국가 조회");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); ; //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }
        #endregion

        #region 조회조건 TextChanged
        //수출자
        private void txtSBeneficiaryCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSBeneficiaryCust.Text != "")
                    {
                        txtSBeneficiaryCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSBeneficiaryCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSBeneficiaryCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //결재방법
        private void txtSPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSPaymentMeth.Text != "")
                    {
                        txtSPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSPaymentMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSPaymentMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //가격조건
        private void txtSCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSCostCond.Text != "")
                    {
                        txtSCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSCostCond.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S005' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtProjectSeq.Value = "";
        }

        #endregion

        #region GroupBox3 입력조건 TextChanged
        //세관
        private void txtCustomOffice_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCustomOffice.Text != "")
                    {
                        txtCustomOfficeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCustomOffice.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S033' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCustomOfficeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //통관계획
        private void txtCcPlan_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCcPlan.Text != "")
                    {
                        txtCcPlanNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCcPlan.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M009' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCcPlanNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //징수형태
        private void txtCollectType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCollectType.Text != "")
                    {
                        txtCollectTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCollectType.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M016' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCollectTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }

        }

        //신고구분
        private void txtReportDiv_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtReportDiv.Text != "")
                    {
                        txtReportDivNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtReportDiv.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M010' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtReportDivNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //신고자
        private void txtReportCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtReportCust.Text != "")
                    {
                        txtReportCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtReportCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtReportCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //거래구분
        private void txtDealDiv_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtDealDiv.Text != "")
                    {
                        txtDealDivNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDealDiv.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M011' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtDealDivNm.Value = "";
                    }
                }
            }
            catch
            {

            }

        }

        //운송방법
        private void txtTransMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTransMeth.Text != "")
                    {
                        txtTransMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTransMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S013' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        //납세의무자
        private void txtTaxPaymentCust_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTaxPaymentCust.Text != "")
                    {
                        txtTaxPaymentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtTaxPaymentCust.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtTaxPaymentCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //수입종류
        private void txtImportKind_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtImportKind.Text != "")
                    {
                        txtImportKindNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtImportKind.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M012' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtImportKindNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //포장형태
        private void txtPackType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPackType.Text != "")
                    {
                        txtPackTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPackType.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S007' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPackTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //도착항
        private void txtDischgePort_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtDischgePort.Text != "")
                    {
                        txtDischgePortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDischgePort.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S009' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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
        #endregion

        #region GroupBox4 입력조건 TextChanged
        //선박국적
        private void txtShipCountry_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtShipCountry.Text != "")
                    {
                        txtShipCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtShipCountry.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtShipCountryNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //선적항
        private void txtLoadingPort_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtLoadingPort.Text != "")
                    {
                        txtLoadingPortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtLoadingPort.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S009' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        //적출국가
        private void txtPickOutCountry_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPickOutCountry.Text != "")
                    {
                        txtPickOutCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPickOutCountry.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPickOutCountryNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //원산지
        private void txtOriginCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtOriginCd.Text != "")
                    {
                        txtOriginNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtOriginCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        //원산지국가
        private void txtOriginCountry_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtOriginCountry.Text != "")
                    {
                        txtOriginCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtOriginCountry.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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
        #endregion

        #region B/L참조 팝업
        private void butBlRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MCC001P1 frm1 = new MCC001P1(fpSpread1);
                frm1.ShowDialog(); ;
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
                        txtXchRate.Value = Msgs[5].ToString();

                        txtBeneficiaryCust.Value = Msgs[6].ToString();
                        txtBeneficiaryCustNm.Value = Msgs[7].ToString();

                        txtApplicantCust.Value = Msgs[8].ToString();
                        txtApplicantCustNm.Value = Msgs[9].ToString();

                        if (Msgs[10].ToString() != "")
                        {
                            txtDischgePort.Value = Msgs[10].ToString();
                            txtDischgePortNm.Value = Msgs[11].ToString();
                        }

                        txtLoadingPort.Value = Msgs[12].ToString();
                        txtLoadingPortNm.Value = Msgs[13].ToString();


                        txtOriginCd.Value = Msgs[14].ToString();
                        txtOriginNm.Value = Msgs[15].ToString();

                        txtOriginCountry.Value = Msgs[16].ToString();
                        txtOriginCountryNm.Value = Msgs[17].ToString();

                        Compute_Amt();
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

        #region 금액계산(전체)
        private void Compute_Amt()
        {
            int SumCc = 0, SumCcLoc = 0;
            int i1 = 0, i2 = 0;

            i1 = SystemBase.Base.GridHeadIndex(GHIdx1, "통관금액");
            i2 = SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액");

            //행수만큼 처리
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                Set_Amt(i);
                UIForm.FPMake.fpChange(fpSpread1, i);//수정플래그
                SumCc += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, i1].Value);
                SumCcLoc += Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, i2].Value);
            }

            txtCcAmt.ReadOnly = false;
            txtCcAmtLoc.ReadOnly = false;

            txtCcAmt.Value = SumCc;
            txtCcAmtLoc.Value = SumCcLoc;

            txtCcAmt.ReadOnly = true;
            txtCcAmtLoc.ReadOnly = true;
        }
        #endregion

        #region MCC001_Activated
        private void MCC001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSOpenDtFr.Focus();
        }

        private void MCC001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region TextBox ValueChanged

        private void txtCifAmt_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {

                if (txtCifAmt.ValueIsDbNull == false && txtUsdXchRate.ValueIsDbNull == false)
                {
                    decimal amt = Convert.ToDecimal(txtCifAmt.Value.ToString());
                    decimal xrate = Convert.ToDecimal(txtUsdXchRate.Value.ToString());
                    decimal loc = amt * xrate;

                    txtCifAmtLoc.ReadOnly = false;
                    txtCifAmtLoc.Value = Convert.ToInt32(loc);
                    txtCifAmtLoc.ReadOnly = true;
                }
                else
                {
                    txtCifAmtLoc.ReadOnly = false;
                    txtCifAmtLoc.Value = 0;
                    txtCifAmtLoc.ReadOnly = true;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUsdXchRate_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCifAmt.ValueIsDbNull == false && txtUsdXchRate.ValueIsDbNull == false)
                {
                    decimal amt = Convert.ToDecimal(txtCifAmt.Value.ToString());
                    decimal xrate = Convert.ToDecimal(txtUsdXchRate.Value.ToString());
                    decimal loc = amt * xrate;

                    txtCifAmtLoc.ReadOnly = false;
                    txtCifAmtLoc.Value = Convert.ToInt32(loc);
                    txtCifAmtLoc.ReadOnly = true;
                }
                else
                {
                    txtCifAmtLoc.ReadOnly = false;
                    txtCifAmtLoc.Value = 0;
                    txtCifAmtLoc.ReadOnly = true;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtXchRate_ValueChanged(object sender, System.EventArgs e)
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

    }
}