#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 매입등록 및 관리
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

namespace MV.MIV001
{
    public partial class MIV001 : UIForm.FPCOMM2
    {
        #region 변수선언
        string strTEMP_SLIP_NO, strSLIP_NO;
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoIvNo = ""; //매입번호
        string strBtn = "N";
        bool btnNew_is = true;
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public MIV001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIV001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //화폐단위

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S011', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단가구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT포함구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //조회조건
            dtpSIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSIvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoSCfm_All.Checked = true;

            //입력조건
            dtpIvDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            rdoCfm_N.Checked = true;

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            butSearchSlip.Enabled = false;
            btnRef.Enabled = true;

            txtVatType.Value = "A";

           
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            strBtn = "Y";

            if (btnNew_is)
            {
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                //기타 세팅
                dtpSIvDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
                dtpSIvDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

                rdoSCfm_All.Checked = true;
            }

            SystemBase.Validation.GroupBox_Reset(groupBox2);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅

            dtpIvDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            rdoCfm_N.Checked = true;

            NewFlg = 1;
            strAutoIvNo = "";

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
            butSearchSlip.Enabled = false;
            btnRef.Enabled = true;
            strBtn = "N";

            strTEMP_SLIP_NO = "";
            strSLIP_NO = "";

            txtVatType.Value = "A";
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {

            //if (txtIvType.Text == "IIV")
            //{
            //    MessageBox.Show("해외매입은 삭제할 수 없습니다!", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_MIV001  'D1'";
                    strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";
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
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Search("", "");
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
            Search("", "");
        }

        private void Search(string strIvNo, string IvDt)
        {
            this.Cursor = Cursors.WaitCursor;

            if (strIvNo != "")
            {
                txtSIvNo.Value = strIvNo;
                dtpIvDt.Text = IvDt;
            }

            try
            {
                string strCfmYn = "";
                if (rdoSCfm_Y.Checked == true) { strCfmYn = "Y"; }
                else if (txtSCfm_N.Checked == true) { strCfmYn = "N"; }

                string strQuery = " usp_MIV001  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pIV_DT_FR = '" + dtpSIvDtFr.Text + "' ";
                strQuery += ", @pIV_DT_TO = '" + dtpSIvDtTo.Text + "' ";
                strQuery += ", @pIV_TYPE = '" + txtSIvType.Text + "' ";
                strQuery += ", @pPUR_DUTY = '" + txtSPurDuty.Text + "' ";
                strQuery += ", @pCUST_CD = '" + txtSCustCd.Text + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "' ";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                strQuery += ", @pIV_NO = '" + txtSIvNo.Text.Trim() + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);
               
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strIvNo != "")
                    {
                        fpSpread2.Search(0, strIvNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }

                    }
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    strAutoIvNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text;
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoIvNo);

                }
                else
                {
                    NewFlg = 1;
                    strAutoIvNo = "";
                    btnNew_is = false;
                    NewExec();
                    btnNew_is = true;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.; //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            txtIvNo.Focus();
            DialogResult dsMsg;
            string strMstType = "";
            string strInUpFlag = "I";
			bool bDelete = false;	// 디테일 항목에서 삭제된 행이 있는지 여부 판단

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            if (rdoCfm_Y.Checked == false)
            {
                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        this.Cursor = Cursors.WaitCursor;

                        int IvSeq = 0;

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

                                string strSql = " usp_MIV001 '" + strMstType + "'";
                                strSql += ", @pIV_NO = '" + txtIvNo.Text + "' ";

                                strSql += ", @pIV_DT = '" + dtpIvDt.Text + "' ";
                                strSql += ", @pIV_TYPE = '" + txtIvType.Text + "' ";
                                strSql += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                                strSql += ", @pPUR_DUTY= '" + txtPurDuty.Text + "' ";
                                strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                strSql += ", @pEXCH_RATE = '" + txtExchRate.Value + "' ";
                                strSql += ", @pBILL_CUST = '" + txtBillCustCd.Text + "' ";
                                strSql += ", @pPAYMENT_CUST = '" + txtPaymentCustCd.Text + "' ";
                                if (dtpPaymentPlanDt.Text != "")
                                    strSql += ", @pPAYMENT_PLAN_DT = '" + dtpPaymentPlanDt.Text + "' ";
                                strSql += ", @pVAT_TYPE = '" + txtVatType.Text + "' ";
                                strSql += ", @pVAT_RATE = '" + txtVatRate.Text + "' ";
                                strSql += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";
                                if (txtPaymentTerm.Text != "")
                                    strSql += ", @pPAYMENT_TERM = '" + txtPaymentTerm.Text + "' ";
                                strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentTermRemark.Text + "' ";
                                strSql += ", @pTAX_BIZ_CD = '" + txtTaxBizCd.Text + "' ";
                                strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoIvNo = dt.Rows[0][2].ToString();

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
                                    dsMsg = MessageBox.Show(msg, "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            string strDelSql = " usp_MIV001  'D1'";
                                            strDelSql += ", @pIV_NO = '" + strAutoIvNo + "' ";
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
                                            Search("", "");
                                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoIvNo, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호"));
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

										if (strGbn == "D2")
										{
											bDelete = true;
										}

                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량")].Text.Trim() == "")
                                        {
                                            ERRCode = "WR";
                                            MSGCode = "매입수량을 입력하세요!";
                                            Trans.Rollback();
                                            goto Exit;
                                        }
                                        string strSql = " usp_MIV001 '" + strGbn + "'";
                                        strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";

                                        if (strGbn == "I2")
                                        {
                                            IvSeq = 0;
                                        }
                                        else
                                        {
                                            IvSeq = Convert.ToInt16(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입순번")].Value);
                                        }

                                        strSql += ", @pIV_SEQ = " + IvSeq;
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "' ";
                                        strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                        strSql += ", @pEXCH_RATE = '" + txtExchRate.Value + "' ";
                                        strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
                                        strSql += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
                                        strSql += ", @pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text != "")
                                            strSql += ", @pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";

                                        strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' ";
                                        strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSql += ", @pIV_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입단위")].Text + "' ";
                                        strSql += ", @pIV_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량")].Value + "' ";
                                        strSql += ", @pIV_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입단가")].Value + "' ";
                                        strSql += ", @pPRICE_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value + "' ";
                                        strSql += ", @pIV_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Value + "' ";
                                        strSql += ", @pVAT_INC_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value + "' ";
                                        strSql += ", @pIV_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매입자국금액")].Value + "' ";

                                        strSql += ", @pNET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value + "' ";
                                        strSql += ", @pNET_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value + "' ";
                                        strSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                        strSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value + "' ";
                                        strSql += ", @pTOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value + "' ";
                                        strSql += ", @pTOT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value + "' ";

                                        strSql += ", @pREMARK  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                                    }
                                }

								// 디테일 항목 삭제가 이루어진 경우 IV_SEQ 재조정 ////////////////////////////////////////////////////////////////
								if (bDelete == true)
								{
									string strSql = " usp_MIV001 'RE'";
									strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";
									strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

									DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = ds.Tables[0].Rows[0][0].ToString();
									MSGCode = ds.Tables[0].Rows[0][1].ToString();

									if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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

                            string strSql1 = " usp_MIV001 'I3'";
                            strSql1 += ", @pIV_NO = '" + strAutoIvNo + "' ";
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
                        if (MSGCode != "")
                        {
                            if (ERRCode == "OK")
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                                if (NewFlg == 1) Search(strAutoIvNo, dtpIvDt.Text);
                                else SubSearch(strAutoIvNo);

                                UIForm.FPMake.GridSetFocus(fpSpread2, strAutoIvNo, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호"));
                                UIForm.FPMake.GridSetFocus(fpSpread1, IvSeq.ToString(), SystemBase.Base.GridHeadIndex(GHIdx2, "매입순번"));

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
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
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

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분"))
            {
                Set_Amt(Row);
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액"))
            {
                Set_Amt1(Row);
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액"))
            {
                Set_Amt2(Row);
            }
        }

        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분"))
            {
                Set_Amt(e.Row);
            }
        }
        #endregion
        
        #region 금액계산
        private void Set_Amt(int Row)
        {
            decimal IvAmt = 0, IvAmtLoc = 0;
            decimal Price = 0, Qty = 0;
            decimal VatRate = 0, xch_rate = 1;
            decimal VatAmt = 0;
            double VatAmtLoc = 0;
            decimal NetAmt = 0, NetAmtLoc = 0;
            decimal Tot = 0, TotLoc = 0;

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량")].Value);
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입단가")].Value);

            if (Price != 0 && Qty != 0)
            {
                xch_rate = Convert.ToDecimal(txtExchRate.Value);
                if (txtVatRate.Text != "")
                    VatRate = Convert.ToDecimal(txtVatRate.Text);
                else
                    VatRate = 0;
                IvAmt = Price * Qty;
                IvAmtLoc = IvAmt * xch_rate;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Value = IvAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입자국금액")].Value = IvAmtLoc;

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value.ToString() == "1")  //1포함,2별도 
                {
                    VatAmt = IvAmt - (IvAmt / ((100 + VatRate) / 100));
                    NetAmt = IvAmt - VatAmt;

                    VatAmtLoc = Math.Floor(Convert.ToDouble(Math.Round(IvAmtLoc, 0) - (Math.Round(IvAmtLoc, 0) / ((100 + VatRate) / 100))));

                    NetAmtLoc = NetAmt * xch_rate;

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = NetAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = NetAmtLoc;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = VatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = VatAmtLoc;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = IvAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = IvAmtLoc;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = IvAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = IvAmtLoc;
                    VatAmt = IvAmt * (VatRate / 100);

                    VatAmtLoc = Math.Floor(Convert.ToDouble(Math.Round(IvAmtLoc, 0) * (VatRate / 100)));

                    Tot = IvAmt + VatAmt;
                    TotLoc = IvAmtLoc + Convert.ToDecimal(VatAmtLoc);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = VatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = VatAmtLoc;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = Tot;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = TotLoc;
                }

            }

        }

        private void Set_Amt1(int Row)
        {
            decimal IvAmt = 0, IvAmtLoc = 0;
            decimal Price = 0, Qty = 0;
            decimal VatRate = 0, xch_rate = 1;
            decimal VatAmt = 0;
            double VatAmtLoc = 0;
            decimal NetAmt = 0, NetAmtLoc = 0;
            decimal Tot = 0, TotLoc = 0;

            xch_rate = Convert.ToDecimal(txtExchRate.Value);

            VatAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);

            VatAmtLoc = Math.Floor(Convert.ToDouble(VatAmt * xch_rate));


            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = VatAmtLoc;

            IvAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Value);
            IvAmtLoc = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입자국금액")].Value);

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value.ToString() == "1")  //1포함,2별도 
            {
                NetAmt = IvAmt - VatAmt;
                NetAmtLoc = NetAmt * xch_rate;

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = NetAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = NetAmtLoc;


                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = IvAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = IvAmtLoc;
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = IvAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = IvAmtLoc;

                Tot = IvAmt + VatAmt;
                TotLoc = IvAmtLoc + Convert.ToDecimal(VatAmtLoc);

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = Tot;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = TotLoc;
            }
        }

        private void Set_Amt2(int Row)
        {
            decimal IvAmt = 0, IvAmtLoc = 0;
            decimal Price = 0, Qty = 0;
            decimal VatRate = 0, xch_rate = 1;
            decimal VatAmt = 0;
            double VatAmtLoc = 0;
            decimal NetAmt = 0, NetAmtLoc = 0;
            decimal Tot = 0, TotLoc = 0;

            xch_rate = Convert.ToDecimal(txtExchRate.Value);

            VatAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);

            VatAmtLoc = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value);

            IvAmt = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액")].Value);
            IvAmtLoc = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매입자국금액")].Value);

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value.ToString() == "1")  //1포함,2별도 
            {
                NetAmt = IvAmt - VatAmt;
                NetAmtLoc = NetAmt * xch_rate;

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = NetAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = NetAmtLoc;


                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = IvAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = IvAmtLoc;
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = IvAmt;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = IvAmtLoc;

                Tot = IvAmt + VatAmt;
                TotLoc = IvAmtLoc + Convert.ToDecimal(VatAmtLoc);

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = Tot;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계자국금액")].Value = TotLoc;
            }
        }
        #endregion

        #region 금액계산(전체)
        private void Compute_Amt()
        {
            decimal SumVat = 0, SumVatLoc = 0, SumIv = 0, SumIvLoc = 0;
            int i1 = 0, i2 = 0, i3 = 0, i4 = 0;

            i1 = SystemBase.Base.GridHeadIndex(GHIdx1, "매입금액");
            i2 = SystemBase.Base.GridHeadIndex(GHIdx1, "매입자국금액");
            i3 = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액");
            i4 = SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액");

            //행수만큼 처리
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                Set_Amt(i);
                UIForm.FPMake.fpChange(fpSpread1, i);//수정플래그
                SumIv += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, i1].Value);
                SumIvLoc += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, i2].Value);
                SumVat += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, i3].Value);
                SumVatLoc += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, i4].Value);
            }

            txtIvAmt.Value = SumIv;
            txtIvAmtLoc.Value = SumIvLoc;
            txtVatAmt.Value = SumVat;
            txtVatAmtLoc.Value = SumVatLoc;

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
                    strAutoIvNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "매입번호")].Text.ToString();

                    SubSearch(strAutoIvNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다. 		
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
                string strSql = " usp_MIV001  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pIV_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                txtIvNo.Value = dt.Rows[0]["IV_NO"].ToString();

                dtpIvDt.Value = dt.Rows[0]["IV_DT"].ToString();
                txtIvType.Value = dt.Rows[0]["IV_TYPE"].ToString();
                txtIvTypeNm.Value = dt.Rows[0]["IV_TYPE_NM"].ToString();
                txtCustCd.Value = dt.Rows[0]["CUST_CD"].ToString();
                txtCustNm.Value = dt.Rows[0]["CUST_NM"].ToString();
                txtPurDuty.Value = dt.Rows[0]["PUR_DUTY"].ToString();
                txtPurDutyNm.Value = dt.Rows[0]["USR_NM"].ToString();
                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                txtExchRate.Value = dt.Rows[0]["EXCH_RATE"];

                txtIvAmt.Value = dt.Rows[0]["IV_AMT"];
                txtIvAmtLoc.Value = dt.Rows[0]["IV_AMT_LOC"];
                txtVatAmt.Value = dt.Rows[0]["VAT_AMT"];
                txtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"];

                txtBillCustCd.Value = dt.Rows[0]["BILL_CUST"].ToString();
                txtBillCustNm.Value = dt.Rows[0]["BILL_CUST_NM"].ToString();
                txtPaymentCustCd.Value = dt.Rows[0]["PAYMENT_CUST"].ToString();
                txtPaymentCustNm.Value = dt.Rows[0]["PAYMENT_CUST_NM"].ToString();

                if (dt.Rows[0]["PAYMENT_PLAN_DT"].ToString() != "")
                    dtpPaymentPlanDt.Value = dt.Rows[0]["PAYMENT_PLAN_DT"];
                else
                    dtpPaymentPlanDt.Value = null;

                txtVatType.Value = dt.Rows[0]["VAT_TYPE"].ToString();
                txtVatTypeNm.Value = dt.Rows[0]["VAT_TYPE_NM"].ToString();
                txtVatRate.Value = dt.Rows[0]["VAT_RATE"].ToString();


                txtPaymentMeth.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentMethNm.Value = dt.Rows[0]["PAYMENT_METH_NM"].ToString();
                txtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"].ToString();
                txtPaymentTermRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                txtTaxBizCd.Value = dt.Rows[0]["TAX_BIZ_CD"].ToString();
                txtTaxBizNm.Value = dt.Rows[0]["TAX_BIZ_NM"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();

                txtVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtVatType.ReadOnly = true;
                btnVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                btnVatType.Enabled = false;
               
                //확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                    {
                        rdoCfm_Y.Checked = true;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                        btnConfirmOk.Enabled = false;
                        btnConfirmCancel.Enabled = true;
                        btnRef.Enabled = false;
                        butSearchSlip.Enabled = true;
                    }
                    else
                    {
                        rdoCfm_N.Checked = true;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

                        btnConfirmOk.Enabled = true;
                        btnConfirmCancel.Enabled = false;
                        btnRef.Enabled = true;
                        butSearchSlip.Enabled = false;

                        // 2019.09.27. hma 추가(Start): 미확정건인 경우 매입형태에 따라 매입대상참조(외주) 또는 매입대상참조 버튼을 활성화/비활성 되도록 함.
                        if (txtIvType.Text == "OIV")
                        {
                            btnRef1.Enabled = true;     // 매입대상참조(외주)
                            btnRef.Enabled = false;     // 매입대상참조
                        }
                        else
                        {
                            btnRef1.Enabled = false;   // 매입대상참조(외주)
                            btnRef.Enabled = true;     // 매입대상참조
                        }
                        // 2019.09.27. hma 추가(End)
                    }
                }
                else { rdoCfm_N.Checked = true; }

                txtIvNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtIvNo.ReadOnly = true;

                if (txtSlipNo.Text != "")
                {
                    butSearchSlip.Enabled = true;
                }
                else
                {
                    butSearchSlip.Enabled = false;
                }

                //Detail그리드 정보.
                string strSql1 = " usp_MIV001  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pIV_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 4);

                //확정여부에 따른 화면 Locking
                if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y" || txtIvType.Text == "IIV")
                {
                    //Detail Locking설정
                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                        );

                }
                else
                {
                    //Detail Locking해제
                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "매입수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                        );
                }

                if (txtIvType.Text == "IIV")
                {
                    btnConfirmOk.Enabled = false;
                    btnConfirmCancel.Enabled = false;
                    btnRef.Enabled = false;
                    btnRef1.Enabled = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회조건 팝업
        //매입형태
        private void btnSIvType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1 = 'IV_TYPE', @pSPEC2 = 'IV_TYPE_NM', @pSPEC3 = 'M_IV_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSIvType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "매입형태 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSIvType.Value = Msgs[0].ToString();
                    txtSIvTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //공급처
        private void btnSCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //구매담당자
        private void btnSPurDuty_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSPurDuty.Value = Msgs[0].ToString();
                    txtSPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

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
                    txtSProjectSeq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnSProjSeq_Click(object sender, System.EventArgs e)
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

        #region 입력조건 팝업
        //지급처
        private void btnPaymentCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtPaymentCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPaymentCustCd.Value = Msgs[1].ToString();
                    txtPaymentCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";

        }

        //결재방법
        private void btnPaymentMeth_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'S004', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결재방법 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPaymentMeth.Value = Msgs[0].ToString();
                    txtPaymentMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }


        //발행처
        private void btnBillCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtBillCustCd.Text, "P");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //VAT 유형
        private void btnVatType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B040' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtVatType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00032", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "VAT유형 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtVatType.Value = Msgs[0].ToString();
                    txtVatTypeNm.Value = Msgs[1].ToString();
                    txtVatRate.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        //세금신고사업장
        private void btnTaxBiz_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_S_COMMON @pTYPE = 'S070', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금신고사업장 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBizCd.Value = Msgs[0].ToString();
                    txtTaxBizNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnPurDuty_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = "usp_M_COMMON @pTYPE = 'M011', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPurDuty.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매담당자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPurDuty.Value = Msgs[0].ToString();
                    txtPurDutyNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

            }
            strBtn = "N";
        }
        #endregion

        #region 조회조건 TextChanged
        //매입형태
        private void txtSIvType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSIvType.Text != "")
                    {
                        txtSIvTypeNm.Value = SystemBase.Base.CodeName("IV_TYPE", "IV_TYPE_NM", "M_IV_TYPE", txtSIvType.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtSIvTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //공급처
        private void txtSCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtSCustCd.Text != "")
                    {
                        txtSCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCustCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
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

        //구매담당자
        private void txtSPurDuty_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtSPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtSPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    if (temp != "")
                    {
                        if (txtSPurDuty.Text != "")
                        {
                            txtSPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtSPurDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtSPurDutyNm.Value = "";
                        }
                    }
                }
                else if (txtSPurDuty.Text.Trim() == "") txtSPurDutyNm.Value = "";                
            }
            catch
            {

            }
        }

        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N") txtSProjectSeq.Value = "";
        }
        #endregion

        #region 입력조건 TextChanged
        //지급처
        private void txtPaymentCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPaymentCustCd.Text != "")
                    {
                        txtPaymentCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtPaymentCustCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPaymentCustNm.Value = "";
                    }
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
                if (strBtn == "N")
                {
                    if (txtBillCustCd.Text != "")
                    {
                        txtBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBillCustCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtBillCustNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        //VAT유형
        private void txtVatType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    txtVatTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtVatType.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'B040'");
                    txtVatRate.Value = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", txtVatType.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'B040'");

                    if (txtVatRate.Text == "")
                        txtVatRate.Value = "0";
                }           
            }
            catch
            {

            }
        }

        private void txtPaymentMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPaymentMeth.Text != "")
                    {
                        txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, "  AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'S004' ");
                    }
                    else
                    {
                        txtPaymentMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }

        }
        //세금신고사업장
        private void txtTaxBizCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTaxBizCd.Text != "")
                    {
                        txtTaxBizNm.Value = SystemBase.Base.CodeName("TAX_BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBizCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtTaxBizNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtVatRate_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                Compute_Amt();
        }

        private void txtPurDuty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (strBtn == "N" && txtPurDuty.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtPurDuty.Text, " AND USE_YN = 'Y' AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                    if (temp != "")
                    {
                        if (txtPurDuty.Text != "")
                        {
                            txtPurDutyNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtPurDuty.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtPurDutyNm.Value = "";
                        }
                    }
                    else
                    {
                        txtPurDutyNm.Value = "";
                    }
                }
                else if (txtPurDuty.Text.Trim() == "") txtPurDutyNm.Value = "";
            }
            catch
            {

            }
        }
        #endregion

        #region 매입대상 참조 팝업
        //내자
        private void btnRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIV001P1 frm1 = new MIV001P1(fpSpread1, txtVatType.Text);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;
                    if (Msgs == null)
                        return;
                    if (NewFlg == 1)
                    {
                        txtCustCd.Value = Msgs[0].ToString();
                        txtCustNm.Value = Msgs[1].ToString();
                        txtPurDuty.Value = Msgs[2].ToString();
                        txtPurDutyNm.Value = Msgs[3].ToString();
                        txtIvType.Value = Msgs[4].ToString();
                        txtIvTypeNm.Value = Msgs[5].ToString();

                        txtPaymentCustCd.Value = Msgs[0].ToString();
                        txtBillCustCd.Value = Msgs[0].ToString();

                        txtPaymentMeth.Value = Msgs[6].ToString();
                        txtPaymentMethNm.Value = Msgs[7];

                        cboCurrency.SelectedValue = Msgs[8];
                        txtExchRate.Value = Msgs[9];
                        txtVatType.Value = Msgs[10].ToString();

                        txtVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        txtVatType.ReadOnly = true;
                        btnVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        btnVatType.Enabled = false;

                        if (txtVatType.Text == "")
                        {
                            txtVatRate.Value = "0";
                        }

                        string strSql = " usp_MIV001  'C1' ";
                        strSql = strSql + ", @pPUR_DUTY = '" + Msgs[2].ToString() + "' ";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count > 0)
                        {
                            txtTaxBizCd.Value = dt.Rows[0]["TAX_BIX_CD"].ToString();
                            txtTaxBizNm.Value = dt.Rows[0]["BIZ_NM"].ToString();
                        }
                        else
                        {
                            txtTaxBizCd.Value = "";
                            txtTaxBizNm.Value = "";
                        }

                        // 2019.09.27. hma 추가(Start): 매입대상참조 버튼을 이용하여 라인이 추가된 경우 해당 매입형태만 계속 추가하도록 하기 위해
                        //                              매입대상참조(외주) 버튼은 비활성화 처리함.
                        btnRef1.Enabled = false;
                        // 2019.09.27. hma 추가(End)
                    }
                    Compute_Amt();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //외자
        private void btnRef1_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIV001P1 frm1 = new MIV001P1(fpSpread1, true, txtVatType.Text);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = frm1.ReturnVal;
                    if (Msgs == null)
                        return;
                    if (NewFlg == 1)
                    {
                        txtCustCd.Value = Msgs[0].ToString();
                        txtCustNm.Value = Msgs[1].ToString();
                        txtPurDuty.Value = Msgs[2].ToString();
                        txtPurDutyNm.Value = Msgs[3].ToString();
                        txtIvType.Value = Msgs[4].ToString();
                        txtIvTypeNm.Value = Msgs[5].ToString();

                        txtPaymentCustCd.Value = Msgs[0].ToString();
                        txtBillCustCd.Value = Msgs[0].ToString();

                        txtPaymentMeth.Value = Msgs[6].ToString();
                        txtPaymentMethNm.Value = Msgs[7];

                        cboCurrency.SelectedValue = Msgs[8];
                        txtExchRate.Value = Msgs[9];
                        txtVatType.Value = Msgs[10].ToString();

                        txtVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        txtVatType.ReadOnly = true;
                        btnVatType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        btnVatType.Enabled = false;

                        // 
                        string strSql = " usp_MIV001  'C1' ";
                        strSql = strSql + ", @pPUR_DUTY = '" + Msgs[2].ToString() + "' ";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count > 0)
                        {
                            txtTaxBizCd.Value = dt.Rows[0]["TAX_BIX_CD"].ToString();
                            txtTaxBizNm.Value = dt.Rows[0]["BIZ_NM"].ToString();
                        }
                        else
                        {
                            txtTaxBizCd.Value = "";
                            txtTaxBizNm.Value = "";
                        }

                        // 2019.09.27. hma 추가(Start): 매입대상참조(외주) 버튼을 이용하여 라인이 추가된 경우 해당 매입형태만 계속 추가하도록 하기 위해
                        //                              매입대상참조 버튼은 비활성화 처리함.
                        btnRef.Enabled = false;
                        // 2019.09.27. hma 추가(End)
                    }
                    Compute_Amt();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 확정, 취소
        private void btnConfirmOk_Click(object sender, System.EventArgs e)
        {
            Confirm("Y");
        }

        private void btnConfirmCancel_Click(object sender, System.EventArgs e)
        {
            Confirm("N");
        }

        private void Confirm(string strConfirmYn)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MIV001  'P0'";
                strSql += ", @pIV_NO = '" + strAutoIvNo + "' ";
                strSql += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubSearch(strAutoIvNo);
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

        #region 폼 Activated & Deactivate
        private void MIV001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSIvDtFr.Focus();
        }

        private void MIV001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
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
    }
}