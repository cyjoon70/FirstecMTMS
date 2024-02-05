#region 작성정보
/*********************************************************************/
// 단위업무명 : B/L등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-05
// 작성내용 : B/L등록 및 관리
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

namespace MB.MBL001
{
    public partial class MBL001 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strTEMP_SLIP_NO, strSLIP_NO;
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoBlNo = ""; //B/L번호
        int SearchRow = 0;
        int ShowColumn = 0;
        bool btnNew_is = true;
        string strLinkSlipNo = "";     // 2022.01.24. hma 추가: 링크전표번호
        #endregion

        #region 생성자
        public MBL001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MBL001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

            //GroupBox3입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //화폐단위

            //GroupBox4입력조건 콥보박스 세팅
            SystemBase.ComboMake.C1Combo(cboWeightUnit, "usp_B_COMMON @pType='REL', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pSPEC1 = 'WT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //순중량단위
            SystemBase.ComboMake.C1Combo(cboVolumUnit, "usp_B_COMMON @pType='REL', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pSPEC1 = 'WT' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //총용적단위

            //그리드 콤보박스 세팅
            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위

            // 2022.01.24. hma 추가(Start): 확정전표/반제전표 결재상태
            SystemBase.ComboMake.C1Combo(cboCSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboMSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 그룹웨어상태
            // 2022.01.24. hma 추가(End)

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅
            //조회조건
            dtpSBlReceiptDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
            dtpSBlReceiptDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoSCfm_All.Checked = true;

            //입력조건GroupBox3
            dtpBlReceiptDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpLoadingDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            dtpArriveDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            rdoCfm_N.Checked = true;

            panel9.Enabled = false;

            txtIvTypeCd.Value = "IIV";
            txtTaxBixCd.Value = "FS1";
            txtExchRate.Value = 1;

            btnCostCond.Enabled = false;
            btnPaymentMeth.Enabled = false;
            btnPurOrg.Enabled = false;
            btnBeneficiaryCust.Enabled = false;
            btnApplicantCust.Enabled = false;
            btnPurDuty.Enabled = false;

            NewFlg = 1;
            strAutoBlNo = "";

            c1DockingTab1.SelectedIndex = 0;

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            butSearchSlip.Enabled = false;

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            TabSetting();

            lnkJump1.Text = "확정전표상신";         // 2022.01.24. hma 추가: 화면에 보여지는 링크명
            strJumpFileName1 = "AD.ACD001.ACD001";  // 2022.01.24. hma 추가: 호출할 화면명
            lnkJump2.Text = "반제전표상신";         // 2022.01.24. hma 추가: 화면에 보여지는 링크명
            strJumpFileName2 = "AD.ACD001.ACD001";  // 2022.01.24. hma 추가: 호출할 화면명
            strLinkSlipNo = "";                     // 2022.01.24. hma 추가
        }
        #endregion

        #region TabSetting
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage3); //품목정보
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage4); //계획정보           

            this.c1DockingTab1.SelectedIndex = 0;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        { Grid_Search(false); }
        #endregion

        #region 그리드조회
        private void Grid_Search(bool Search)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strCfmYn = "";
                    if (rdoSCfm_Y.Checked == true) { strCfmYn = "Y"; }
                    else if (rdoSCfm_N.Checked == true) { strCfmYn = "N"; }

                    string strQuery = "usp_MBL001 @pTYPE = 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pBL_RECEIPT_DT_FR = '" + dtpSBlReceiptDtFr.Text + "'";
                    strQuery += ", @pBL_RECEIPT_DT_TO = '" + dtpSBlReceiptDtTo.Text + "'";
                    strQuery += ", @pIV_TYPE = '" + txtSIvTypeCd.Text + "'";
                    strQuery += ", @pPAYER_CUST = '" + txtSPayerCustCd.Text + "'";
                    strQuery += ", @pBILL_CUST = '" + txtSBillCustCd.Text + "'";
                    strQuery += ", @pPAYMENT_METH = '" + txtSPaymentMethCd.Text + "'";
                    strQuery += ", @pCOST_COND = '" + txtSCostCond.Text + "'";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";
                    strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "'";
                    strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "'";
                    strQuery += ", @pBL_NO = '" + txtSBlNo.Text + "'";
                    strQuery += ", @pINVOICE_NO = '" + txtSInvoiceNo.Text + "'";
                    strQuery += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pIV_NO = '" + txtSIvNo.Text + "' ";


                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        NewFlg = 2;

                        if (Search == true)
                        {
                            fpSpread2.Search(0, strAutoBlNo, true, true, true, true, 0, SystemBase.Base.GridHeadIndex(GHIdx2, "송장번호"), ref SearchRow, ref ShowColumn);

                            if (SearchRow < 0)
                            {
                                SearchRow = 0;
                                strAutoBlNo = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "송장번호")].Text;
                            }
                        }
                        else
                        {
                            SearchRow = 0;
                            strAutoBlNo = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "송장번호")].Text;
                        }

                        fpSpread2.Focus();
                        fpSpread2.ActiveSheet.SetActiveCell(SearchRow, 1); //Row Focus		
                        fpSpread2.ShowRow(0, SearchRow, FarPoint.Win.Spread.VerticalPosition.Center); //Focus Row 보기

                        SubSearch(strAutoBlNo);
                    }
                    else
                    {
                        NewFlg = 1;
                        strAutoBlNo = "";
                        btnNew_is = false;
                        NewExec();
                        btnNew_is = true;				
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.

            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            string strMstType = "";
            string strInUpFlag = "I";
            string strCfm = "";

            if (txtBlNo.Text != "") { strAutoBlNo = txtBlNo.Text; }
            else { strAutoBlNo = ""; }

            if (rdoCfm_Y.Checked == true) { strCfm = "Y"; }
            else { strCfm = "N"; }


            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정이 아니면
            if (strCfm == "N")
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
                            if (DelCheck() == true)
                            {
                                if (NewFlg != 0)
                                {
                                    if (NewFlg == 1) { strMstType = "I1"; }
                                    else { strMstType = "U1"; }


                                    string strSql = " usp_MBL001 '" + strMstType + "'";
                                    strSql += ", @pBL_NO = '" + txtBlNo.Text + "' ";
                                    strSql += ", @pINVOICE_NO = '" + txtInvoiceNo.Text + "' ";
                                    strSql += ", @pLOADING_DT = '" + dtpLoadingDt.Text + "'";
                                    strSql += ", @pBL_RECEIPT_DT = '" + dtpBlReceiptDt.Text + "'";
                                    strSql += ", @pARRIVE_DT = '" + dtpArriveDt.Text + "'";

                                    if (dtpSettlementDt.Text != "")
                                        strSql += ", @pSETTLEMENT_DT = '" + dtpSettlementDt.Text + "'";

                                    strSql += ", @pTRANS_METH = '" + txtTransMethCd.Text + "'";
                                    strSql += ", @pVESSEL_COMP = '" + txtVesselCompCd.Text + "'";
                                    strSql += ", @pVESSEL_NAME = '" + txtVesselName.Text + "'";
                                    strSql += ", @pVOYAGE_NO = '" + txtVoyageNo.Text + "'";
                                    strSql += ", @pSHIP_COUNTRY = '" + txtShipCountryCd.Text + "'";
                                    strSql += ", @pDISCHGE_PORT = '" + txtDischgePortCd.Text + "'";
                                    strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue + "'";

                                    if (txtExchRate.Text != "0")
                                        strSql += ", @pXCH_RATE = '" + txtExchRate.Value + "'";

                                    strSql += ", @pAPPLICANT_CUST = '" + txtApplicantCustCd.Text + "'";
                                    strSql += ", @pBENEFICIARY_CUST = '" + txtBeneficiaryCustCd.Text + "'";
                                    strSql += ", @pPAYER_CUST = '" + txtPayerCustCd.Text + "'";
                                    strSql += ", @pBILL_CUST = '" + txtBillCustCd.Text + "'";
                                    strSql += ", @pIV_TYPE = '" + txtIvTypeCd.Text + "'";
                                    strSql += ", @pCONFIRM_YN = '" + strCfm + "'";
                                    strSql += ", @pPUR_DUTY = '" + txtPurDutyCd.Text + "'";
                                    strSql += ", @pTAX_BIZ_CD = '" + txtTaxBixCd.Text + "'";
                                    strSql += ", @pPAYMENT_METH = '" + txtPaymentMethCd.Text + "'";
                                    strSql += ", @pPAYMENT_TERM = '" + txtPaymentTerm.Value + "'";
                                    strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentTermRemark.Text + "'";
                                    strSql += ", @pCOST_COND = '" + txtCostCond.Text + "'";
                                    strSql += ", @pREMARK = '" + txtRemark.Text + "'";
                                    strSql += ", @pPACK_TYPE = '" + txtPackTypeCd.Text + "'";
                                    strSql += ", @pTOT_PACKING_CNT = '" + txtTotPackingCnt.Value + "'";
                                    strSql += ", @pPACK_REMARK = '" + txtPackRemark.Text + "'";
                                    strSql += ", @pCONTAINER_CNT = '" + txtContainerCnt.Value + "'";
                                    strSql += ", @pNET_WEIGHT = '" + txtNetWeight.Value + "'";
                                    strSql += ", @pWEIGHT_UNIT = '" + cboWeightUnit.SelectedValue + "'";
                                    strSql += ", @pVOLUM_UNIT = '" + cboVolumUnit.SelectedValue + "'";
                                    strSql += ", @pFARE_PAYMENT = '" + txtFarePaymentCd.Text + "'";
                                    strSql += ", @pFARE_PAYMENT_PLACE = '" + txtFarePaymentPlace.Text + "'";
                                    strSql += ", @pFINAL_DESTINATION = '" + txtFinalDestination.Text + "'";
                                    strSql += ", @pTRANS_PLACE = '" + txtTransPlace.Text + "'";
                                    strSql += ", @pRECEIPT_PLACE = '" + txtReceiptPlace.Text + "'";
                                    strSql += ", @pLOADING_PORT = '" + txtLoadingPortCd.Text + "'";
                                    strSql += ", @pTRANSHIP_COUNTRY = '" + txtTranshipCountry.Text + "'";
                                    if (dtpTranshipDt.Text != "")
                                        strSql += ", @pTRANSHIP_DT = '" + dtpTranshipDt.Text + "'";
                                    strSql += ", @pORIGIN_CD = '" + txtOriginCd.Text + "'";
                                    strSql += ", @pORIGIN_COUNTRY = '" + txtOriginCountry.Text + "'";
                                    strSql += ", @pBL_ISSUE_PLACE = '" + txtBlIssuePlace.Text + "'";
                                    strSql += ", @pBL_ISSUE_CNT = '" + txtBlIssueCnt.Value + "'";
                                    strSql += ", @pMANUFACTURER = '" + txtManufactrerCd.Text + "'";
                                    strSql += ", @pAGENT_CUST = '" + txtAgentCustCd.Text + "'";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";


                                    DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                    ERRCode = dt.Rows[0][0].ToString();
                                    MSGCode = dt.Rows[0][1].ToString();
                                    strAutoBlNo = dt.Rows[0][2].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                            {

                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0027"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_MBL001  'D1'";
                                            strDelSql += ", @pBL_NO = '" + strAutoBlNo + "' ";
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
                                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();

                                        if (ERRCode == "OK")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                                        return;
                                    }
                                    else
                                    {
                                        Trans.Rollback();
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//작업이 취소되었습니다.
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

                                        string strSql = " usp_MBL001 '" + strGbn + "'";
                                        strSql += ", @pBL_NO = '" + strAutoBlNo + "' ";
                                        if (strGbn == "I2") strSql += ", @pBL_SEQ = 0 ";
                                        else strSql += ", @pBL_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L순번")].Value;
                                        strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
                                        strSql += ", @pPO_SEQ= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                        strSql += ", @pBL_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value + "' ";
                                        strSql += ", @pBL_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' "; ;
                                        strSql += ", @pBL_AMT1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액")].Value + "' ";
                                        strSql += ", @pBL_AMT_LOC1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                        strSql += ", @pGROSS_WEIGHT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총중량")].Value + "'";
                                        strSql += ", @pVOLUME_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "용적량")].Value + "'";
                                        strSql += ", @pREMARK1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                                        strSql += ", @pLC_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C번호")].Text + "'";

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C순번")].Text == "")
                                            strSql += ", @pLC_SEQ = NULL";
                                        else
                                            strSql += ", @pLC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "L/C순번")].Text + "'";

                                        strSql += ", @pSCM_MVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCM입고번호")].Text + "'";
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

                            string strSql1 = " usp_MBL001 'I3'";
                            strSql1 += ", @pBL_NO = '" + strAutoBlNo + "' ";
                            strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);

                            if (dt1.Rows[0][0].ToString() != "OK")
                            {
                                ERRCode = dt1.Rows[0][0].ToString();
                                MSGCode = dt1.Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
                            MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();

                        if (ERRCode == "OK")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (NewFlg == 1) { Grid_Search(true); }
                            else { SubSearch(strAutoBlNo); }
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
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0027"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strDelSql = " usp_MBL001  'D1'";
                    strDelSql += ", @pBL_NO = '" + strAutoBlNo + "' ";
                    strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region fpSpread2_LeaveCell
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (e.Row != e.NewRow)
                {
                    try
                    {
                        SearchRow = e.NewRow;
                        strAutoBlNo = fpSpread2.Sheets[0].Cells[SearchRow, SystemBase.Base.GridHeadIndex(GHIdx2, "송장번호")].Text.ToString();

                        c1DockingTab1.SelectedIndex = 0;

                        SubSearch(strAutoBlNo);
                        NewFlg = 2;
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //데이터 조회 중 오류가 발생하였습니다.				
                    }
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox3);
                SystemBase.Validation.GroupBox_Reset(groupBox4);

                // B/L MASTER 정보
                string strQuery = " usp_MBL001  'S2' ";
                strQuery += ", @pBL_NO = '" + strCode + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                txtBlNo.Value = dt.Rows[0]["BL_NO"].ToString();
                txtInvoiceNo.Value = dt.Rows[0]["INVOICE_NO"].ToString();

                dtpBlReceiptDt.Value = dt.Rows[0]["BL_RECEIPT_DT"].ToString();
                dtpLoadingDt.Value = dt.Rows[0]["LOADING_DT"].ToString();
                txtTransMethCd.Value = dt.Rows[0]["TRANS_METH"].ToString();
                dtpArriveDt.Value = dt.Rows[0]["ARRIVE_DT"].ToString();
                if (dt.Rows[0]["SETTLEMENT_DT"].ToString() != "") dtpSettlementDt.Value = dt.Rows[0]["SETTLEMENT_DT"].ToString();
                txtVesselCompCd.Value = dt.Rows[0]["VESSEL_COMP"].ToString();
                txtVesselCompNm.Value = dt.Rows[0]["VESSEL_COMP_NM"].ToString();
                txtVesselName.Value = dt.Rows[0]["VESSEL_NAME"].ToString();
                txtShipCountryCd.Value = dt.Rows[0]["SHIP_COUNTRY"].ToString();
                txtVoyageNo.Value = dt.Rows[0]["VOYAGE_NO"].ToString();
                txtTotPackingCnt.Value = dt.Rows[0]["TOT_PACKING_CNT"].ToString();
                txtDischgePortCd.Value = dt.Rows[0]["DISCHGE_PORT"].ToString();
                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                txtBlAmt.Value = dt.Rows[0]["BL_AMT"].ToString();
                txtCostCond.Value = dt.Rows[0]["COST_COND"].ToString();
                txtExchRate.Value = dt.Rows[0]["XCH_RATE"].ToString();
                txtBlAmtLoc.Value = dt.Rows[0]["BL_AMT_LOC"].ToString();
                txtPaymentMethCd.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"].ToString();

                // 확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y") { rdoCfm_Y.Checked = true; }
                    else { rdoCfm_N.Checked = true; }
                }

                txtIvTypeCd.Value = dt.Rows[0]["IV_TYPE"].ToString();
                txtPaymentTermRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                txtPayerCustCd.Value = dt.Rows[0]["PAYER_CUST"].ToString();
                txtBillCustCd.Value = dt.Rows[0]["BILL_CUST"].ToString();
                txtPurOrgCd.Value = dt.Rows[0]["PUR_ORG"].ToString();
                txtPurDutyCd.Value = dt.Rows[0]["PUR_DUTY"].ToString();
                txtBeneficiaryCustCd.Value = dt.Rows[0]["BENEFICIARY_CUST"].ToString();
                txtTaxBixCd.Value = dt.Rows[0]["TAX_BIZ_CD"].ToString();
                txtApplicantCustCd.Value = dt.Rows[0]["APPLICANT_CUST"].ToString();
                txtIvNo.Value = dt.Rows[0]["IV_NO"].ToString();
                txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                txtPackTypeCd.Value = dt.Rows[0]["PACK_TYPE"].ToString();
                txtPackRemark.Value = dt.Rows[0]["PACK_REMARK"].ToString();
                txtGrossWeight.Value = dt.Rows[0]["GROSS_WEIGHT"].ToString();
                txtContainerCnt.Value = dt.Rows[0]["CONTAINER_CNT"].ToString();
                txtFarePaymentCd.Value = dt.Rows[0]["FARE_PAYMENT"].ToString();
                txtNetWeight.Value = dt.Rows[0]["NET_WEIGHT"].ToString();
                cboWeightUnit.SelectedValue = dt.Rows[0]["WEIGHT_UNIT"].ToString();
                txtFarePaymentPlace.Value = dt.Rows[0]["FARE_PAYMENT_PLACE"];
                txtVolumeSize.Value = dt.Rows[0]["VOLUME_SIZE"].ToString();
                cboVolumUnit.SelectedValue = dt.Rows[0]["VOLUM_UNIT"];
                txtFinalDestination.Value = dt.Rows[0]["FINAL_DESTINATION"].ToString();
                txtTransPlace.Value = dt.Rows[0]["TRANS_PLACE"].ToString();
                txtReceiptPlace.Value = dt.Rows[0]["RECEIPT_PLACE"].ToString();
                txtLoadingPortCd.Value = dt.Rows[0]["LOADING_PORT"].ToString();
                txtTranshipCountry.Value = dt.Rows[0]["TRANSHIP_COUNTRY"].ToString();
                txtOriginCd.Value = dt.Rows[0]["ORIGIN_CD"].ToString();
                if (dt.Rows[0]["TRANSHIP_DT"].ToString() != "") dtpTranshipDt.Value = dt.Rows[0]["TRANSHIP_DT"].ToString();
                txtOriginCountry.Value = dt.Rows[0]["ORIGIN_COUNTRY"].ToString();
                txtBlIssueCnt.Value = dt.Rows[0]["BL_ISSUE_CNT"].ToString();
                txtBlIssuePlace.Value = dt.Rows[0]["BL_ISSUE_PLACE"].ToString();
                txtManufactrerCd.Value = dt.Rows[0]["MANUFACTURER"].ToString();
                txtAgentCustCd.Value = dt.Rows[0]["AGENT_CUST"].ToString();

                //strTEMP_SLIP_NO = dt.Rows[0]["TEMP_SLIP_NO"].ToString();
                //strSLIP_NO = dt.Rows[0]["SLIP_NO"].ToString();

                // 2022.01.24. hma 추가(Start): 결재상태 및 반제전표번호, 반제승인 추가
                txtCSlipNo.Value = dt.Rows[0]["CFM_SLIP_NO"].ToString();
                cboCSlipGwStatus.SelectedValue = dt.Rows[0]["CFM_GW_STATUS"].ToString();
                txtMinusConfirm.Value = dt.Rows[0]["MINUS_CONFIRM_YN"].ToString();
                txtMSlipNo.Value = dt.Rows[0]["MINUS_SLIP_NO"].ToString();
                cboMSlipGwStatus.SelectedValue = dt.Rows[0]["MINUS_GW_STATUS"].ToString();
                // 2022.01.24. hma 추가(End)

                fpSpread1.Sheets[0].Rows.Count = 0;

                string strQuery1 = "usp_MBL001 @PTYPE = 'S3'";
                strQuery1 += ", @pBL_NO = '" + strCode + "'";
                strQuery1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);

                //확정여부에 따른 화면 Locking
                if (rdoCfm_Y.Checked == true)	//확정
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
                    txtBlNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    txtBlNo.ReadOnly = true;

                    btnConfirmOk.Enabled = false;

                    // 2022.01.24. hma 수정(Start): 확정상태이면서 결재상태가 상신대기/반려/승인 상태이면 확정취소 버튼 활성화되게.
                    //btnConfirmCancel.Enabled = true;
                    if ((txtSlipNo.Text != "" && txtCSlipNo.Text == "") ||
                        ((txtCSlipNo.Text != "") &&
                         (cboCSlipGwStatus.SelectedValue.ToString() == "READY" || cboCSlipGwStatus.SelectedValue.ToString() == "REJECT" ||
                          (cboCSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y"))))
                        btnConfirmCancel.Enabled = true;
                    else
                        btnConfirmCancel.Enabled = false;
                    // 2022.01.24. hma 수정(End)

                    butSearchSlip.Enabled = true;
                    butScmRef.Enabled = false;
                    butPoRef.Enabled = false;
                    butLcRef.Enabled = false;

                    // Detail Locking 설정
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "총중량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "용적량") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                            );
                    }
                }
                else	//미확정
                {
                    // 2022.01.24. hma 추가(Start): 미확정 상태이지만 반제전표번호가 없거나, 반제전표번호가 있으면서 반제전표상태가 승인이고 반제승인여부가 Y인 경우
                    if ((txtMSlipNo.Text == "") ||
                       (txtMSlipNo.Text != "" &&
                        (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                    {
                    // 2022.01.24. hma 추가(End)
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                        SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
                    }
                    else
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);        // 2022.02.19. hma 추가
                        SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);        // 2022.02.19. hma 추가
                    }

                    txtBlNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    txtBlNo.ReadOnly = true;

                    // 2022.01.24. hma 수정(Start): 미확정상태이지만 반제전표 결재상태가 상신대기/반려 상태일때만 확정취소 버튼 활성화되게.
                    //                              또한 결재상태가 승인이면서 반제승인이 Y인 경우에도 확정취소 버튼 활성화.(반제처리 위해)
                    //btnConfirmOk.Enabled = true;
                    if ((txtMSlipNo.Text == "") ||
                        (txtMSlipNo.Text != "" &&
                         (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                        btnConfirmOk.Enabled = true;
                    else
                        btnConfirmOk.Enabled = false;
                    // 2022.01.24. hma 수정(End)

                    // 2022.01.24. hma 수정(Start): 미확정건이지만 반제전표가 생성된 경우에는 확정취소 버튼 비활성화 처리.
                    btnConfirmCancel.Enabled = false;     // 2022.02.12. hma 수정: 아래 부분 주석 처리하고 이 부분 주석 해제. 미확정 상태일때는 확정취소 버튼 비활성화 처리.
                    //if (txtMSlipNo.Text != "" &&
                    //     (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                    //    btnConfirmCancel.Enabled = false;
                    // 2022.01.24. hma 수정(End)

                    butSearchSlip.Enabled = false; ;
                    butScmRef.Enabled = true;
                    butPoRef.Enabled = true;
                    butLcRef.Enabled = true;
                    rdoCfm_Y.Enabled = false;
                    rdoCfm_N.Enabled = true;

                    // 2022.02.16. hma 수정(Start): 확정 버튼이 활성화 되어있을 경우에만 참조 버튼 활성화되게.
                    if (btnConfirmOk.Enabled == false)
                    {
                        butScmRef.Enabled = false;
                        butPoRef.Enabled = false;
                        butLcRef.Enabled = false;
                    }

                    // 반제전표 결재상태에 따라 반제취소 버튼 활성화 처리. 반제전표 결재상태가 상신대기, 반려이면 활성화.
                    btnMinusCancel.Enabled = false;
                    if (txtMSlipNo.Text != "" &&
                        (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                    {
                        btnMinusCancel.Enabled = true;
                    }
                    // 2022.02.16. hma 수정(End)

                    // Detail Locking해제
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        // 2022.01.24. hma 추가(Start): 미확정 상태이지만 반제전표를 생성해서 승인상태가 아니면 비활성화 처리.
                        if (dt.Rows[0]["CONFIRM_YN"].ToString() == "N" && dt.Rows[0]["MINUS_SLIP_NO"].ToString() != "" &&
                             dt.Rows[0]["MINUS_GW_STATUS"].ToString() != "APPR")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "총중량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "용적량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                );
                        }
                        else
                        {
                        // 2022.01.24. hma 추가(End))
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품명") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "총중량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "용적량") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                );
                        }
                    }
                }

                if (txtSlipNo.Text != "")
                {
                    butSearchSlip.Enabled = true;
                }
                else
                {
                    butSearchSlip.Enabled = false;
                }

                //조회내용이 없으면 리셋
                if (fpSpread1.Sheets[0].Rows.Count == 0)
                {
                    NewExec();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSBlReceiptDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0,10);
                dtpSBlReceiptDtTo.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
                rdoSCfm_All.Checked = true;
            }

            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
            fpSpread1.Sheets[0].Rows.Count = 0;


            //입력조건GroupBox3
            dtpBlReceiptDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpLoadingDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpArriveDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoCfm_N.Checked = true;

            txtIvTypeCd.Value = "IIV";
            txtTaxBixCd.Value = "FS1";
            txtExchRate.Value = 1;

            btnCostCond.Enabled = false;
            btnPaymentMeth.Enabled = false;
            btnPurOrg.Enabled = false;
            btnBeneficiaryCust.Enabled = false;
            btnApplicantCust.Enabled = false;
            btnPurDuty.Enabled = false;

            NewFlg = 1;
            strAutoBlNo = "";

            c1DockingTab1.SelectedIndex = 0;

            butScmRef.Enabled = true;
            butPoRef.Enabled = true;
            butLcRef.Enabled = true;
            rdoCfm_Y.Enabled = false;
            rdoCfm_N.Enabled = true;
            rdoCfm_N.Checked = true;

            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            butSearchSlip.Enabled = false;

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";
        }
        #endregion

        #region SCM참조 팝업
        private void butScmRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MBL001P4 myForm = new MBL001P4();

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    myForm = new MBL001P4(fpSpread1,
                        txtCostCond.Text,
                        txtPurDutyCd.Text,
                        txtBeneficiaryCustCd.Text,
                        cboCurrency.SelectedValue.ToString(),
                        txtPaymentMethCd.Text,
                        txtApplicantCustCd.Text);
                    myForm.ShowDialog();
                }
                else
                {
                    myForm = new MBL001P4(fpSpread1);

                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        if (Msgs != "")
                        {
                            string strQuery = " usp_MBL001  'S4' ";
                            strQuery += ", @pPO_NO = '" + Msgs + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                            txtTransMethCd.Value = dt.Rows[0][0].ToString();
                            txtDischgePortCd.Value = dt.Rows[0][1].ToString();
                            cboCurrency.SelectedValue = dt.Rows[0][2];
                            txtCostCond.Value = dt.Rows[0][3].ToString();
                            txtExchRate.Value = dt.Rows[0][4].ToString();
                            txtPaymentMethCd.Value = dt.Rows[0][5].ToString();
                            txtPaymentTerm.Value = dt.Rows[0][6].ToString();
                            txtPaymentTermRemark.Value = dt.Rows[0][7].ToString();
                            txtPurDutyCd.Value = dt.Rows[0][8].ToString();
                            txtPurOrgCd.Value = dt.Rows[0][9].ToString();
                            txtBeneficiaryCustCd.Value = dt.Rows[0][10].ToString();
                            txtPayerCustCd.Value = dt.Rows[0][10].ToString();
                            txtApplicantCustCd.Value = dt.Rows[0][11].ToString();
                            txtPackTypeCd.Value = dt.Rows[0][12].ToString();
                            txtTransPlace.Value = dt.Rows[0][13].ToString();
                            txtLoadingPortCd.Value = dt.Rows[0][14].ToString();
                            txtOriginCd.Value = dt.Rows[0][15].ToString();
                            txtManufactrerCd.Value = dt.Rows[0][16].ToString();
                            txtAgentCustCd.Value = dt.Rows[0][17].ToString();
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "SCM참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 발주참조 팝업
        private void butPoRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MBL001P1 myForm = new MBL001P1();

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    myForm = new MBL001P1(fpSpread1,
                                            txtCostCond.Text,
                                            txtPurDutyCd.Text,
                                            txtBeneficiaryCustCd.Text,
                                            cboCurrency.SelectedValue.ToString(),
                                            txtPaymentMethCd.Text,
                                            txtApplicantCustCd.Text);
                    myForm.ShowDialog();
                }
                else
                {
                    myForm = new MBL001P1(fpSpread1);

                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        if (Msgs != "")
                        {
                            string strQuery = " usp_MBL001  'S4' ";
                            strQuery += ", @pPO_NO = '" + Msgs + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                            txtTransMethCd.Value = dt.Rows[0][0].ToString();
                            txtDischgePortCd.Value = dt.Rows[0][1].ToString();
                            cboCurrency.SelectedValue = dt.Rows[0][2];
                            txtCostCond.Value = dt.Rows[0][3].ToString();
                            txtExchRate.Value = dt.Rows[0][4].ToString();
                            txtPaymentMethCd.Value = dt.Rows[0][5].ToString();
                            txtPaymentTerm.Value = dt.Rows[0][6].ToString();
                            txtPaymentTermRemark.Value = dt.Rows[0][7].ToString();
                            txtPurDutyCd.Value = dt.Rows[0][8].ToString();
                            txtPurOrgCd.Value = dt.Rows[0][9].ToString();
                            txtBeneficiaryCustCd.Value = dt.Rows[0][10].ToString();
                            txtPayerCustCd.Value = dt.Rows[0][10].ToString();
                            txtApplicantCustCd.Value = dt.Rows[0][11].ToString();
                            txtPackTypeCd.Value = dt.Rows[0][12].ToString();
                            txtTransPlace.Value = dt.Rows[0][13].ToString();
                            txtLoadingPortCd.Value = dt.Rows[0][14].ToString();
                            txtOriginCd.Value = dt.Rows[0][15].ToString();
                            txtManufactrerCd.Value = dt.Rows[0][16].ToString();
                            txtAgentCustCd.Value = dt.Rows[0][17].ToString();
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region L/C참조 팝업
        private void butLcRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MBL001P3 myForm = new MBL001P3();

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    myForm = new MBL001P3(fpSpread1,
                        txtCostCond.Text,
                        txtPurDutyCd.Text,
                        txtBeneficiaryCustCd.Text,
                        cboCurrency.SelectedValue.ToString(),
                        txtPaymentMethCd.Text,
                        txtApplicantCustCd.Text);
                    myForm.ShowDialog();
                }
                else
                {
                    myForm = new MBL001P3(fpSpread1);

                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        string Msgs = myForm.ReturnVal;

                        if (Msgs != "")
                        {
                            string strQuery = " usp_MBL001  'S5' ";
                            strQuery += ", @pLC_NO = '" + Msgs + "'";
                            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                            txtTransMethCd.Value = dt.Rows[0][0].ToString();
                            txtDischgePortCd.Value = dt.Rows[0][1].ToString();
                            cboCurrency.SelectedValue = dt.Rows[0][2];
                            txtCostCond.Value = dt.Rows[0][3].ToString();
                            txtExchRate.Value = dt.Rows[0][4].ToString();
                            txtPaymentMethCd.Value = dt.Rows[0][5].ToString();
                            txtPaymentTerm.Value = dt.Rows[0][6].ToString();
                            txtPaymentTermRemark.Value = dt.Rows[0][7].ToString();
                            txtPurDutyCd.Value = dt.Rows[0][8].ToString();
                            txtPurOrgCd.Value = dt.Rows[0][9].ToString();
                            txtBeneficiaryCustCd.Value = dt.Rows[0][10].ToString();
                            txtPayerCustCd.Value = dt.Rows[0][10].ToString();
                            txtApplicantCustCd.Value = dt.Rows[0][11].ToString();
                            txtPackTypeCd.Value = dt.Rows[0][12].ToString();
                            txtFarePaymentCd.Value = dt.Rows[0][13].ToString();
                            txtTransPlace.Value = dt.Rows[0][14].ToString();
                            txtLoadingPortCd.Value = dt.Rows[0][15].ToString();
                            txtOriginCd.Value = dt.Rows[0][16].ToString();
                            txtOriginCountry.Value = dt.Rows[0][17].ToString();
                            txtManufactrerCd.Value = dt.Rows[0][18].ToString();
                            txtAgentCustCd.Value = dt.Rows[0][19].ToString();
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "L/C참조 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 확정,확정취소
        private void Confirm(string strConfirmYn)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                int intErRow = 0;
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.			
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string BL_DT = "";
                DataTable blDt = SystemBase.DbOpen.NoTranDataTable("SELECT CONVERT(VARCHAR(10), BL_RECEIPT_DT, 121) FROM M_BL_MASTER(NOLOCK) WHERE BL_NO = '" + txtBlNo.Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                BL_DT = blDt.Rows[0][0].ToString();

                if (BL_DT != dtpBlReceiptDt.Text)
                {
                    this.Cursor = Cursors.Default;
                    Trans.Rollback();
                    MessageBox.Show(SystemBase.Base.MessageRtn("저장된 BL접수일 : " + BL_DT + " 과 현재 BL접수일 : " + dtpBlReceiptDt.Text + " 이 일치하지 않습니다."), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string strAutoIvNo = "";
                    string strSql10 = "";       // 2022.01.25. hma 추가

                    //확정 매입테이블에 저장
                    if (strConfirmYn == "Y")
                    {
                        /////////////////////////////////////////////// 매입 MASETR 저장 시작 /////////////////////////////////////////////////
                        //						if(txtIvNo.Text.Trim() == "")
                        //						{
                        string strSql1 = " usp_MIV001 @pTYPE = 'I1'";
                        strSql1 += ", @pIV_NO = '" + strAutoIvNo + "'";
                        strSql1 += ", @pIV_DT = '" + dtpBlReceiptDt.Text + "' ";
                        strSql1 += ", @pIV_TYPE = '" + txtIvTypeCd.Text + "' ";
                        strSql1 += ", @pCUST_CD = '" + txtBeneficiaryCustCd.Text + "' ";
                        strSql1 += ", @pPUR_DUTY= '" + txtPurDutyCd.Text + "' ";
                        strSql1 += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                        strSql1 += ", @pEXCH_RATE = '" + txtExchRate.Value + "' ";
                        strSql1 += ", @pBILL_CUST = '" + txtBillCustCd.Text + "' ";
                        strSql1 += ", @pPAYMENT_CUST = '" + txtPayerCustCd.Text + "' ";
                        if (dtpSettlementDt.Text != "")
                            strSql1 += ", @pPAYMENT_PLAN_DT = '" + dtpSettlementDt.Text + "' ";
                        strSql1 += ", @pVAT_TYPE = 'C'";
                        strSql1 += ", @pVAT_RATE = 0 ";
                        strSql1 += ", @pPAYMENT_METH = '" + txtPaymentMethCd.Text + "' ";
                        if (txtPaymentTerm.Text != "")
                            strSql1 += ", @pPAYMENT_TERM = '" + txtPaymentTerm.Text + "' ";
                        strSql1 += ", @pPAYMENT_TERM_REMARK = '' ";
                        strSql1 += ", @pTAX_BIZ_CD = '" + txtTaxBixCd.Text + "' ";
                        strSql1 += ", @pREMARK = '" + txtRemark.Text + "' ";
                        strSql1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                        ERRCode = dt1.Rows[0][0].ToString();
                        MSGCode = dt1.Rows[0][1].ToString();
                        strAutoIvNo = dt1.Rows[0][2].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        /////////////////////////////////////////////// 매입 DETAIL 저장 시작 /////////////////////////////////////////////////
                        //그리드 상단 필수 체크
                        if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
                        {
                            //행수만큼 처리
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                string strSql2 = " usp_MIV001 'I2'";
                                strSql2 += ", @pIV_NO = '" + strAutoIvNo + "'";
                                strSql2 += ", @pIV_SEQ = 0 ";
                                strSql2 += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                strSql2 += ", @pEXCH_RATE = '" + txtExchRate.Value + "' ";
                                strSql2 += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                strSql2 += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                strSql2 += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
                                strSql2 += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
                                strSql2 += ", @pBL_NO = '" + txtBlNo.Text + "' ";
                                strSql2 += ", @pBL_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L순번")].Text + "' ";
                                strSql2 += ", @pMVMT_NO = '' ";
                                strSql2 += ", @pMVMT_SEQ = NULL ";
                                strSql2 += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' ";
                                strSql2 += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                strSql2 += ", @pIV_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
                                strSql2 += ", @pIV_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value + "' ";
                                strSql2 += ", @pIV_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' ";
                                strSql2 += ", @pPRICE_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Text + "'";
                                strSql2 += ", @pIV_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액")].Value + "' ";
                                strSql2 += ", @pVAT_INC_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Text + "' ";
                                strSql2 += ", @pIV_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                strSql2 += ", @pNET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액")].Value + "' ";
                                strSql2 += ", @pNET_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                strSql2 += ", @pVAT_AMT = '0' ";
                                strSql2 += ", @pVAT_AMT_LOC = '0' ";
                                strSql2 += ", @pTOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액")].Value + "' ";
                                strSql2 += ", @pTOT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                strSql2 += ", @pREMARK  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                strSql2 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt2 = SystemBase.DbOpen.TranDataTable(strSql2, dbConn, Trans);
                                ERRCode = dt2.Rows[0][0].ToString();
                                MSGCode = dt2.Rows[0][1].ToString();

                                intErRow = i;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        else
                        {
                            Trans.Rollback();
                            this.Cursor = Cursors.Default;
                            return;
                        }

                        //금액집계 UPDATE 시작
                        string strSql3 = " usp_MIV001 'I3'";
                        strSql3 += ", @pIV_NO = '" + strAutoIvNo + "' ";
                        strSql3 += ", @pIN_UP_FLAG = 'I' ";
                        strSql3 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt3 = SystemBase.DbOpen.TranDataTable(strSql3, dbConn, Trans);
                        ERRCode = dt3.Rows[0][0].ToString();
                        MSGCode = dt3.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        //BL확정
                        string strSql5 = " usp_MBL001  'P3'";
                        strSql5 += ", @pBL_NO = '" + strAutoBlNo + "' ";
                        strSql5 += ", @pIV_NO = '" + strAutoIvNo + "'";
                        strSql5 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                        strSql5 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt5 = SystemBase.DbOpen.TranDataTable(strSql5, dbConn, Trans);
                        ERRCode = dt5.Rows[0][0].ToString();
                        MSGCode = dt5.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        //매입확정
                        string strSql8 = " usp_MIV001 'P0'";
                        strSql8 += ", @pIV_NO = '" + strAutoIvNo + "' ";
                        strSql8 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                        strSql8 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql8 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt6 = SystemBase.DbOpen.TranDataTable(strSql8, dbConn, Trans);
                        ERRCode = dt6.Rows[0][0].ToString();
                        MSGCode = dt6.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 2022.01.25. hma 추가(Start): 매입마스터의 전표번호를 BL마스터에 Update
                        strSql10 = " usp_MBL001  'U3'";
                        strSql10 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql10 += ", @pBL_NO = '" + strAutoBlNo + "' ";
                        strSql10 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";                        

                        DataTable dt10 = SystemBase.DbOpen.TranDataTable(strSql10, dbConn, Trans);
                        ERRCode = dt10.Rows[0][0].ToString();
                        MSGCode = dt10.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                        // 2022.01.25. hma 추가(End)
                    }
                    else	//확정 취소
                    {

                        //매입확정취소
                        string strSql6 = " usp_MIV001 'P0'";
                        strSql6 += ", @pIV_NO = '" + txtIvNo.Text + "' ";
                        strSql6 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                        strSql6 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql6 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt7 = SystemBase.DbOpen.TranDataTable(strSql6, dbConn, Trans);
                        ERRCode = dt7.Rows[0][0].ToString();
                        MSGCode = dt7.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 2022.01.25. hma 추가(Start): 매입마스터의 전표번호를 BL마스터에 Update. 매입마스터 데이터 삭제 전에.
                        strSql10 = " usp_MBL001  'U3'";
                        strSql10 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql10 += ", @pBL_NO = '" + strAutoBlNo + "' ";
                        strSql10 += ", @pIV_NO = '" + strAutoIvNo + "'";
                        strSql10 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";

                        DataTable dt10 = SystemBase.DbOpen.TranDataTable(strSql10, dbConn, Trans);
                        ERRCode = dt10.Rows[0][0].ToString();
                        MSGCode = dt10.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                        // 2022.01.25. hma 추가(End)

                        //매입삭제
                        string strDelSql = " usp_MIV001  'D1'";
                        strDelSql += ", @pIV_NO = '" + txtIvNo.Text + "' ";
                        strDelSql += ", @pBL_YN = 'Y' ";
                        strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt4 = SystemBase.DbOpen.TranDataTable(strDelSql, dbConn, Trans);
                        ERRCode = dt4.Rows[0][0].ToString();
                        MSGCode = dt4.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        //BL확정취소
                        string strSql9 = " usp_MBL001  'P3'";
                        strSql9 += ", @pBL_NO = '" + strAutoBlNo + "' ";
                        strSql9 += ", @pIV_NO = '" + strAutoIvNo + "'";
                        strSql9 += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                        strSql9 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt9 = SystemBase.DbOpen.TranDataTable(strSql9, dbConn, Trans);
                        ERRCode = dt9.Rows[0][0].ToString();
                        MSGCode = dt9.Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                    }

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SubSearch(strAutoBlNo);
                    this.Cursor = Cursors.Default;
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show("[" + Convert.ToString(intErRow + 1) + "행]" + SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fpSpread1.Sheets[0].AddSelection(intErRow, 0, 1, 1);
                }
                else
                {
                    MessageBox.Show("[" + Convert.ToString(intErRow + 1) + "행]" + SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fpSpread1.Sheets[0].AddSelection(intErRow, 0, 1, 1);
                }


            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Cursor = Cursors.Default;
        }

        private void btnConfirmOk_Click(object sender, System.EventArgs e)
        {
            Confirm("Y");
        }

        private void btnConfirmCancel_Click(object sender, System.EventArgs e)
        {
            Confirm("N");
        }
        #endregion

        #region 조회조건 팝업
        //매입형태
        private void btnSIvType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSIvTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSIvTypeCd.Value = Msgs[0].ToString();
                    txtSIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매입형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //지급처
        private void btnSPayerCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSPayerCustCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSPayerCustCd.Value = Msgs[1].ToString();
                    txtSPayerCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //발행처
        private void btnSBillCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtSBillCustCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSBillCustCd.Value = Msgs[1].ToString();
                    txtSBillCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발행처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //결재방법
        private void btnSPaymentMeth_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSPaymentMethCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결재방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSPaymentMethCd.Value = Msgs[0].ToString();
                    txtSPaymentMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "가격조건 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //프로젝트번호
        private void btnSProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Value = Msgs[3].ToString();
                    txtSProjectSeq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트차수
        private void btnSProjectSeq_Click(object sender, System.EventArgs e)
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
                    txtSProjectSeq.Value = Msgs[0].ToString();
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

        #region GroupBox3 입력조건 팝업
        //운송방법
        private void btnTransMeth_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S013', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTransMethCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00035", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운송방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTransMethCd.Value = Msgs[0].ToString();
                    txtTransMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "운송방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //선박회사
        private void btnVesselComp_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'CUST_CD', @pSPEC2 = 'CUST_NM', @pSPEC3 = 'B_CUST_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtVesselCompCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선박회사");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtVesselCompCd.Value = Msgs[0].ToString();
                    txtVesselCompNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "선박회사 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //선박국적
        private void btnShipCountry_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtShipCountryCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00016", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선박국적");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtShipCountryCd.Value = Msgs[0].ToString();
                    txtShipCountryNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "선박국적 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //도착항
        private void btnDischgePort_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S009', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDischgePortCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00040", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "도착항");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDischgePortCd.Value = Msgs[0].ToString();
                    txtDischgePortNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "도착항 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        //매입형태
        private void btnIvType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP1', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pSPEC4 = 'IM_YN', @pSPEC5 = 'Y', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIvTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIvTypeCd.Value = Msgs[0].ToString();
                    txtIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매입형태 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //지급처
        private void btnPayerCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtPayerCustCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPayerCustCd.Value = Msgs[1].ToString();
                    txtPayerCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "지급처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }



        //발행처
        private void btnBillCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtBillCustCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBillCustCd.Value = Msgs[1].ToString();
                    txtBillCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발행처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //세금신고사업장
        private void btnTaxBix_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S070', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBixCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금신고사업장");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBixCd.Value = Msgs[0].ToString();
                    txtTaxBixNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "세금신고사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region GroupBox4 입력조건 팝업
        //포장조건
        private void btnPackType_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S007', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPackTypeCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00017", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "포장조건");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPackTypeCd.Value = Msgs[0].ToString();
                    txtPackTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "포장조건 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //운임지불방법
        private void btnFarePayment_Click(object sender, System.EventArgs e)
        {

            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtFarePaymentCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00043", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "운임지불방법");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtFarePaymentCd.Value = Msgs[0].ToString();
                    txtFarePaymentNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "운임지불방법 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        //선적항
        private void btnLoadingPort_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S009' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtLoadingPortCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00041", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "선적항 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtLoadingPortCd.Value = Msgs[0].ToString();
                    txtLoadingPortNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //환적국가
        private void btnTranshipCountry_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='B006' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTranshipCountry.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00044", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지국가 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTranshipCountry.Value = Msgs[0].ToString();
                    txtTranshipCountryNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "환적국가 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //원산지
        private void btntxtOrigin_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S006', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtOriginCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00037", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원산지 조회");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "원산지 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //제조자
        private void btnManufactrer_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtManufactrerCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtManufactrerCd.Value = Msgs[1].ToString();
                    txtManufactrerNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        //대행자
        private void btnAgentCust_Click(object sender, System.EventArgs e)
        {

            try
            {
                WNDW002 pu = new WNDW002(txtAgentCustCd.Text, "P");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtAgentCustCd.Value = Msgs[1].ToString();
                    txtAgentCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대행자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //원산지 국가
        private void btnOriginCountry_Click(object sender, System.EventArgs e)
        {
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "원산지국가 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //매입형태
        private void txtSIvTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSIvTypeCd.Text != "")
                {
                    txtSIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtSIvTypeCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSIvTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //지급처
        private void txtSPayerCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSPayerCustCd.Text != "")
                {
                    txtSPayerCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSPayerCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSPayerCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //발행처
        private void txtSBillCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSBillCustCd.Text != "")
                {
                    txtSBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSBillCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSBillCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //결재방법
        private void txtSPaymentMethCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSPaymentMethCd.Text != "")
                {
                    txtSPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSPaymentMethCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSPaymentMethNm.Value = "";
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
                if (txtSCostCond.Text != "")
                {
                    txtSCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtSCostCond.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S005' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSCostCondNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region GroupBox3 TextChanged
        //운송방법
        private void txtTransMethCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTransMethCd.Text != "")
                {
                    txtTransMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTransMethCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S013' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTransMethNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //선박회사
        private void txtVesselCompCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtVesselCompCd.Text != "")
                {
                    txtVesselCompNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtVesselCompCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtVesselCompNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //선박국적
        private void txtShipCountryCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtShipCountryCd.Text != "")
                {
                    txtShipCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtShipCountryCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtShipCountryNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //도착항
        private void txtDischgePortCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtDischgePortCd.Text != "")
                {
                    txtDischgePortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDischgePortCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S009' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtDischgePortNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //가격조건
        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCostCond.Text != "")
                {
                    txtCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCostCond.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S005' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtCostCondNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //결재방법
        private void txtPaymentMethCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPaymentMethCd.Text != "")
                {
                    txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMethCd.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S004' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPaymentMethNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //매입형태
        private void txtIvTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIvTypeCd.Text != "")
                {
                    txtIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtIvTypeCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIvTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //지급처
        private void txtPayerCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPayerCustCd.Text != "")
                {
                    txtPayerCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtPayerCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPayerCustNm.Value = "";
                }
                if (txtBillCustNm.Text == "" && txtPayerCustNm.Text != "")
                {
                    txtBillCustCd.Text = txtPayerCustCd.Text;
                }
                else if (txtBillCustNm.Text == "" && txtPayerCustNm.Text == "")
                {
                    txtBillCustCd.Text = "";
                }                
            }
            catch
            {

            }
        }

        //구매조직
        private void txtPurOrgCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurOrgCd.Text != "")
                {
                    txtPurOrgNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPurOrgCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'M001' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurOrgNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //수출자
        private void txtBeneficiaryCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBeneficiaryCustCd.Text != "")
                {
                    txtBeneficiaryCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBeneficiaryCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBeneficiaryCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //수입자
        private void txtApplicantCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtApplicantCustCd.Text != "")
                {
                    txtApplicantCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtApplicantCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtApplicantCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //발행처
        private void txtBillCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBillCustCd.Text != "")
                {
                    txtBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBillCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtBillCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //구매담당자
        private void txtPurDutyCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPurDutyCd.Text != "")
                {
                    txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDutyCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPurDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //세금신고사업장
        private void txtTaxBixCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTaxBixCd.Text != "")
                {
                    txtTaxBixNm.Value = SystemBase.Base.CodeName("TAX_BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBixCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTaxBixNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region GroupBox4 TextChanged
        //포장조건
        private void txtPackTypeCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPackTypeCd.Text != "")
                {
                    txtPackTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPackTypeCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S007' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPackTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //운임지불방법
        private void txtFarePaymentCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtFarePaymentCd.Text != "")
                {
                    txtFarePaymentNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFarePaymentCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S028' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtFarePaymentNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //선적항
        private void txtLoadingPortCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtLoadingPortCd.Text != "")
                {
                    txtLoadingPortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtLoadingPortCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S009' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtLoadingPortNm.Value = "";
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
                if (txtOriginCd.Text != "")
                {
                    txtOriginNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtOriginCd.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'S006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtOriginNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //원산지 국가
        private void txtOriginCountry_TextChanged(object sender, System.EventArgs e)
        {
            try
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
            catch
            {

            }
        }

        //제조자
        private void txtManufactrerCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtManufactrerCd.Text != "")
                {
                    txtManufactrerNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtManufactrerCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtManufactrerNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //대행자
        private void txtAgentCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtAgentCustCd.Text != "")
                {
                    txtAgentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtAgentCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtAgentCustNm.Value = "";
                }
            }
            catch
            {

            }
        }

        //환적국가
        private void txtTranshipCountry_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTranshipCountry.Text != "")
                {
                    txtTranshipCountryNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTranshipCountry.Text, " AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND MAJOR_CD = 'B006' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTranshipCountryNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 그리드 상 Change

        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            //수량, 단가, 금액
            if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량"))
            {
                Set_Amt(Row);
            }
            else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "단가"))
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

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
            
            Amt = Price * Qty;
            LocAmt = Amt * Convert.ToDecimal(txtExchRate.Text);
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액")].Value = Amt;
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value = LocAmt;
            //			}
            Set_Sum_Amt();

        }

        private void Set_Sum_Amt()
        {
            decimal Amt = 0;
            decimal LocAmt = 0;
            int idx1 = SystemBase.Base.GridHeadIndex(GHIdx1, "B/L금액");
            int idx2 = SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액");
            //행수만큼 처리
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                Amt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, idx1].Value);
                LocAmt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, idx2].Value);
            }

            txtBlAmt.Value = Amt;
            txtBlAmtLoc.Value = LocAmt;
        }
        #endregion

        #region 환율 Leave
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

        #region 전표조회 이벤트
        private void butSearchSlip_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlipNo.Text != "")
                {
                    WNDW.WNDW026 pu = new WNDW.WNDW026(txtSlipNo.Text);
                    pu.ShowDialog();
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("S0016"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion	


        // 2022.01.24. hma 추가(Start): 확정전표번호로 결의전표등록 화면 열기
        private void lnkJump1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    strLinkSlipNo = txtCSlipNo.Text;     // 확정전표번호

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Link
        protected override void Link1Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }

        private object[] Params()
        {
            if (strLinkSlipNo == "")
                param = null;						// 파라메터를 하나도 넘기지 않을경우
            else
            {
                param = new object[1];				// 파라메터수가 1개인 경우
                param[0] = strLinkSlipNo;
            }
            return param;
        }
        #endregion
        // 2022.01.24. hma 추가(End)

        // 2022.01.24. hma 추가(Start): 반제전표상신 클릭시 처리. 반제전표번호로 결의전표등록 화면을 열어준다.
        #region lnkJump2_LinkClicked(): 반제전표상신 클릭시. 
        private void lnkJump2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strJumpFileName2.Length > 0)
                {
                    string DllName = strJumpFileName2.Substring(0, strJumpFileName2.IndexOf("."));
                    string FrmName = strJumpFileName2.Substring(strJumpFileName2.IndexOf(".") + 1, strJumpFileName2.Length - strJumpFileName2.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    strLinkSlipNo = txtMSlipNo.Text;     // 반제전표번호

                    Link2Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName2), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Link
        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        // 2022.02.16. hma 추가(Start): 반제취소 처리시 반제전표 삭제하고, 확정여부를 확정상태로 변경하게.
        #region btnMinusCancel_Click()
        private void btnMinusCancel_Click(object sender, EventArgs e)
        {
            // 반제취소 할건지 확인하고 처리하도록 함.
            DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg != DialogResult.Yes)
            {
                return;
            }

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MBL001  'D3'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pBL_NO = '" + txtBlNo.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubSearch(txtBlNo.Text);
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
        // 2022.02.16. hma 추가(End)

        #endregion

    }
}