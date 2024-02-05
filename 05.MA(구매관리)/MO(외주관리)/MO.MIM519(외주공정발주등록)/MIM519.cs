#region 작성정보
/*********************************************************************/
// 단위업무명 : 외주공정발주등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-08
// 작성내용 : 외주공정발주등록 및 관리
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

namespace MO.MIM519
{
    public partial class MIM519 : UIForm.FPCOMM2
    {
        #region 변수선언
        int NewFlg = 1;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정\
        string strAutoPoNo = ""; //발주번호
        string strBtn = "N";
        bool form_act_chk = false;
        string im_yn = "";		//수입여부
        #endregion

        #region 생성자
        public MIM519()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void MIM519_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboPoType, "usp_M_COMMON @pTYPE = 'M034', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//화폐단위

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "공장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT포함구분
            
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            fpSpread1.ActiveSheet.Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택")].Visible = false;
            btnQualityAll.Enabled = false;

            c1DockingTab1.SelectedIndex = 0;
            c1DockingTab1.TabPages[1].Enabled = false;

            //기타 세팅
            dtpSPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpPoDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtxtExchRate.Value = 1;

            cboCurrency.SelectedValue = "KRW";
            rdoCfmAll.Checked = true;
            rdoCfmN.Checked = true;

            panel3.Enabled = false;

            NewFlg = 1;
            strAutoPoNo = "";

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;
        }
        #endregion
        
        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            //확정상태가 아니면
            if (rdoCfmY.Checked == false)
            {
                UIForm.FPMake.RowInsert(fpSpread1);

                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value = SystemBase.Base.gstrPLANT_CD.ToString();//자기소속공장
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = "EA";//EA
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value = "T";//진단가
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가")].Value = 0;//0      // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가")].Value = 0;//0  // 2019.02.22. hma 추가
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value = "2";//별도
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value = "A";//일반세금계산서
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형명")].Text = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", "A", " AND MAJOR_CD = 'B040' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = 10;//10
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = 0;//0
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ여부")].Text = "N";
            }
            else
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            //확정상태가 아니면
            if (rdoCfmY.Checked == false)
            {
                UIForm.FPMake.RowCopy(fpSpread1);
            }
            else
            {
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //확정된 데이터는 다른 작업을 할 수 없습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);

            SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread1.ActiveSheet.Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택")].Visible = false;

            c1DockingTab1.SelectedIndex = 0;
            c1DockingTab1.TabPages[1].Enabled = false;

            //기타 세팅
            dtpSPoDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpSPoDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            dtpPoDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            cboCurrency.SelectedValue = "KRW";
            dtxtExchRate.Value = 1;
            dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
            dtxtExchRate.ReadOnly = true;

            rdoCfmAll.Checked = true;
            rdoCfmN.Checked = true;

            panel3.Enabled = false;

            NewFlg = 1;
            strAutoPoNo = "";

            //확정버튼 Disable
            btnConfirmOk.Enabled = false;
            btnConfirmCancel.Enabled = false;

            btnQualityAll.Enabled = false;
            btnReqRef.Enabled = true;
            btnEstRef.Enabled = true;
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (rdoCfmY.Checked == true)
            {
                DialogResult dsMsg1 = MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    string strSql = " usp_MIM519  'D1'";
                    strSql += ", @pPO_NO = '" + strAutoPoNo + "' ";
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

        private void Search(string strPoNo)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strCfmYn = "";
                if (rdoCfmYes.Checked == true) { strCfmYn = "Y"; }
                else if (rdoCfmNo.Checked == true) { strCfmYn = "N"; }

                string strQuery = " usp_MIM519  @pTYPE = 'S1'";
                strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strQuery += ", @pPO_DT_FR = '" + dtpSPoDtFr.Text + "' ";
                strQuery += ", @pPO_DT_TO = '" + dtpSPoDtTo.Text + "' ";
                strQuery += ", @pPUR_DUTY = '" + txtSUserId.Text.Trim() + "' ";
                strQuery += ", @pCUST_CD = '" + txtSCustCd.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text.Trim() + "' ";
                strQuery += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text.Trim() + "' ";
                strQuery += ", @pCONFIRM_YN = '" + strCfmYn + "' ";
                strQuery += ", @pWORKORDER_NO = '" + txtWorkorderNo.Text + "' ";
                strQuery += ", @pREQ_NO = '" + txtReq_No.Text + "' ";
                strQuery += ", @pPO_NO = '" + txtSPoNo.Text + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
                fpSpread2.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strPoNo != "")
                    {
                        fpSpread2.Search(0, strPoNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);                         
                        }
                        else
                        {
                            x = 0;
                        }

                    }

                    strAutoPoNo = fpSpread2.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text;
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    NewFlg = 2;

                    //상세정보조회
                    SubSearch(strAutoPoNo);
                }
                else
                {
                    NewFlg = 1;
                    strAutoPoNo = "";
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
            txtUserId.Focus();

            string strMstType = "";
            string strInUpFlag = "I";
            DialogResult dsMsg;

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            if (rdoCfmY.Checked == false)
            {
                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
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

                                if (im_yn == "Y" && cboCurrency.SelectedValue.ToString() == "KRW")
                                {
                                    MSGCode = "발주형태가 수입인 경우 화폐단위가 원화이면 저장할 수 없습니다!";
                                    goto Exit;
                                }

                                string strSql = " usp_MIM519 '" + strMstType + "'";						
                                strSql += ", @pPO_NO = '" + txtPoNo.Text.Trim() + "' ";

                                strSql += ", @pPO_DT = '" + dtpPoDt.Text + "' ";
                                strSql += ", @pPO_TYPE = '" + cboPoType.SelectedValue.ToString() + "' ";
                                strSql += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                                strSql += ", @pPUR_DUTY= '" + txtUserId.Text + "' ";
                                strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                                strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                                strSql += ", @pCUST_DUTY_NM = '" + txtCustDutyNm.Text + "' ";
                                strSql += ", @pCUST_DUTY_TEL = '" + txtCustTel.Text + "' ";
                                strSql += ", @pPAYMENT_METH = '" + txtPaymentMeth.Text + "' ";
                                strSql += ", @pPAYMENT_TERM = '" + dtxtPaymentTerm.Text + "' ";
                                strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPayRemark.Text + "' ";
                                strSql += ", @pPAYOUT_METH = '" + txtPayoutMeth.Text + "' ";
                                strSql += ", @pPAYOUT_METH_REMARK = '" + txtPayoutRemark.Text + "' ";
                                strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                                if (dtpContDt.Text.Trim() != "")
                                    strSql += ", @pCONTRACT_DT = '" + dtpContDt.Text + "' ";
                                if (dtpValidDt.Text.Trim() != "")
                                    strSql += ", @pVALID_DT = '" + dtpValidDt.Text + "' ";
                                strSql += ", @pINVOICE_NO = '" + txtInvoiceNo.Text + "' ";
                                strSql += ", @pCOST_COND = '" + txtCostCond.Text + "' ";
                                strSql += ", @pTRANS_METH  = '" + txtTransMeth.Text + "' ";
                                strSql += ", @pTRANS_BANK_CD = '" + txtTransBank.Text + "' ";
                                strSql += ", @pTRANS_PLACE  = '" + txtTransPlace.Text + "' ";
                                strSql += ", @pAPPLICANT_CUST  = '" + txtACsut.Text + "' ";
                                strSql += ", @pMAKER_CUST = '" + txtMaker.Text + "' ";
                                strSql += ", @pAGENT_CUST = '" + txtAgent.Text + "' ";
                                strSql += ", @pORIGIN_CD = '" + txtOrigin.Text + "' ";
                                strSql += ", @pPACK_TYPE = '" + txtPactType.Text + "' ";
                                strSql += ", @pINSPECT_METH = '" + txtInsMeth.Text + "' ";
                                strSql += ", @pDISCHGE_CITY = '" + txtDischCity.Text + "' ";
                                strSql += ", @pDISCHGE_PORT = '" + txtDischPort.Text + "' ";
                                strSql += ", @pLOADING_PORT = '" + txtLoadingPort.Text + "' ";
                                strSql += ", @pLOADING_TERM = '" + txtLoadingTerm.Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoPoNo = dt.Rows[0][2].ToString();

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
                                            string strDelSql = " usp_MIM519  'D1'";
                                            strDelSql += ", @pPO_NO = '" + strAutoPoNo + "' ";
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

                                        string strSql = " usp_MIM519 '" + strGbn + "'";
                                        strSql += ", @pPO_NO = '" + strAutoPoNo + "' ";

                                        if (strGbn == "I2") strSql += ", @pPO_SEQ = 0 ";
                                        else strSql += ", @pPO_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Value;
                                        strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                                        strSql += ", @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' ";
                                        strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSql += ", @pITEM_SPEC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text + "' ";
                                        strSql += ", @pSL_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' ";
                                        strSql += ", @pLOCATION_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text + "' ";
                                        strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text + "' ";
                                        strSql += ", @pREQ_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text != "")
                                            strSql += ", @pREQ_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text + "' ";
                                        strSql += ", @pEST_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적번호")].Text + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text != "")
                                            strSql += ", @pEST_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적순번")].Text + "' ";
                                        strSql += ", @pDELIVERY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text + "' ";

                                        strSql += ", @pPO_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "' ";
                                        strSql += ", @pPO_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value + "' ";
                                        strSql += ", @pPO_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' ";
                                        strSql += ", @pPRICE_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가구분")].Value + "' ";
                                        strSql += ", @pPO_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value + "' ";
                                        strSql += ", @pVAT_INC_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value + "' ";
                                        strSql += ", @pVAT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value + "' ";
                                        strSql += ", @pVAT_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value + "' ";
                                        strSql += ", @pNET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value + "' ";
                                        strSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                        strSql += ", @pTOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value + "' ";

                                        strSql += ", @pMOQ_YN   = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ여부")].Text + "' ";
                                        strSql += ", @pEST_AMT_LOC  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액")].Value + "' ";
                                        strSql += ", @pNEGO_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액")].Value + "' ";
                                        strSql += ", @pCOST_AMT_LOC  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가")].Value + "' ";       // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                                        strSql += ", @pREVIEW_PRICE  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가")].Value + "' ";   // 2019.02.22. hma 추가

                                        strSql += ", @pDRAWING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호")].Text + "' ";
                                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.")].Text != "")
                                            strSql += ", @pDRAWING_REV = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.")].Text + "' ";
                                        //strSql += ", @pQUALITY_PROOF = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙코드")].Text + "' ";

                                        strSql += ", @pREMARK1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text + "' ";
                                        strSql += ", @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text + "' ";
                                        strSql += ", @pROUT_DOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서")].Text + "' ";
                                        strSql += ", @pROUT_SIZE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격")].Text + "' ";
                                        strSql += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                                        strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";

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

                            string strSql1 = " usp_MIM519 'I3'";
                            strSql1 += ", @pPO_NO = '" + strAutoPoNo + "' ";
                            strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                            ERRCode = dt1.Rows[0][0].ToString();
                            if (ERRCode != "OK")
                                MSGCode = dt1.Rows[0][1].ToString();

                            if (ERRCode != "OK") { goto Exit; }	// ER 코드 Return시 점프

                            /////////////////////////////////////////////// 요청정보체크 없으면 생성/////////////////////////////////////////////////
                            if (NewFlg == 1)
                            {
                                string strSql2 = " usp_MIM519 'I4'";
                                strSql2 += ", @pPO_NO = '" + strAutoPoNo + "' ";
                                strSql2 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataTable dt2 = SystemBase.DbOpen.TranDataTable(strSql2, dbConn, Trans);
                                ERRCode = dt2.Rows[0][0].ToString();
                                if (ERRCode != "OK")
                                    MSGCode = dt2.Rows[0][1].ToString();

                                if (ERRCode != "OK") { goto Exit; }	// ER 코드 Return시 점프
                            }

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

                                if (NewFlg == 1) Search(strAutoPoNo);
                                else SubSearch(strAutoPoNo);
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

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            strBtn = "Y";
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
            {
                try
                {
                    WNDW005 pu = new WNDW005(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString(), true, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text);
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[2].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = Msgs[16].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", Msgs[16].ToString(), " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = Msgs[17].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                            = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", Msgs[17].ToString(), " AND SL_CD ='" + Msgs[16].ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                        if (Msgs[14].ToString() == "0") //tracking_flag = "N"
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "*";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "*";

                            UIForm.FPMake.grdReMake(fpSpread1, Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3");

                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, Row,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0");
                        }
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //단위
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2"))
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'Z005' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00029", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "단위팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = Msgs[0].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                }
            }
            //VAT유형
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형_2"))
            {
                string strQuery = " usp_B_COMMON 'COMM_POP1', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B040' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00032", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "VAT유형 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형명")].Text = Msgs[1].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Text = Msgs[2].ToString();

                    UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                }
            }
            //창고
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //위치
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'B036', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
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
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[3].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호차수
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2"))
            {
                try
                {
                    string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";	// 쿼리
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
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙"))
            {
                try
                {
                    if (txtPoNo.Text != "")
                    {
                        string strCfmYn = "";
                        if (rdoCfmY.Checked == true) strCfmYn = "Y";
                        else if (rdoCfmN.Checked == true) strCfmYn = "N";

                        WNDW031 pu = new WNDW031("PO",
                                                 txtPoNo.Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text.ToString(),
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text,
                                                 fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text,
                                                 strCfmYn);

                        pu.ShowDialog();

                        string strSql = " usp_MIM519  'P5' ";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pPO_NO ='" + txtPoNo.Text + "' ";
                        strSql = strSql + ", @pPO_SEQ ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text.ToString() + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                        if (dt.Rows.Count != 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙문서")].Value = dt.Rows[0]["Q_REQ_DOC_NM"].ToString();
                        }
                    }
                    else
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString() != "" &&
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text.ToString() != "")
                        {
                            WNDW031 pu = new WNDW031("RP",
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text.ToString(),
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString(),
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text,
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "제조오더번호")].Text,
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공정순서")].Text,
                                                     fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "작업코드")].Text,
                                                     "Y");

                            pu.ShowDialog();
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
                        string Query = " usp_M_COMMON @pTYPE = 'M012', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', @pNAME = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = dt.Rows[0]["ITEM_NM"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = dt.Rows[0]["ITEM_SPEC"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = dt.Rows[0]["RCPT_SL_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = dt.Rows[0]["RCPT_LOCATION_CD"].ToString();
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                                = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", dt.Rows[0]["RCPT_SL_CD"].ToString(), " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", dt.Rows[0]["RCPT_LOCATION_CD"].ToString(), " AND SL_CD ='" + dt.Rows[0]["RCPT_SL_CD"].ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                            if (dt.Rows[0]["TRACKING_FLAG"].ToString() == "N") //tracking_flag = "N"
                            {
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = "*";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "*";

                                UIForm.FPMake.grdReMake(fpSpread1, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3");

                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, Row,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0");
                            }

                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text = "";
                        }
                    }
                    //발주수량, 단가, 발주금액
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단가")
                        || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형"))
                    {
                        Set_Amt(Row);

                    }
                    //VAT유형
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형명")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text, " AND MAJOR_CD = 'BO40' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Text
                            = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Text, " AND MAJOR_CD = 'BO40' AND LANG_CD ='" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    // 창고 
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "창고"))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                            = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND PLANT_CD ='" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
                    //프로젝트번호
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호"))
                    {
                        //						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text 
                        //							=  SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text, " AND SO_CONFIRM_YN = 'Y' ");
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*")
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = "";

                    }
                    // 프로젝트차수
                    else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "*"
                            || fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text != "")
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
            decimal PoAmt = 0;
            decimal Price = 0;
            decimal Qty = 0;
            decimal ReqQty = 0;
            decimal VatRate = 0;
            decimal VatAmt = 0;
            decimal NetAmt = 0;
            decimal Tot = 0;

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Text.Trim() != "")
                ReqQty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "요청수량")].Value);

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text.Trim() != "")
                Qty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value);

            if (ReqQty != 0 && (ReqQty < Qty))
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "MOQ여부")].Text = "Y";

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text != "0" && fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Text.Trim() != "")
                Price = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
            if (Price != 0 && Qty != 0)
            {
                VatRate = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value);
                PoAmt = Price * Qty;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value = PoAmt;

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분")].Value.ToString() == "1")  //1포함,2별도 
                {
                    VatAmt = PoAmt - (PoAmt / ((100 + VatRate) / 100));
                    NetAmt = PoAmt - VatAmt;

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = NetAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = VatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = PoAmt;
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = PoAmt;
                    VatAmt = PoAmt * (VatRate / 100);
                    Tot = PoAmt + VatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = VatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합계금액")].Value = Tot;
                }
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
                    strAutoPoNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "발주번호")].Text.ToString();

                    c1DockingTab1.SelectedIndex = 0;
                    SubSearch(strAutoPoNo);
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
                fpSpread1.Sheets[0].Rows.Count = 0;

                //수주Master정보
                string strSql = " usp_MIM519  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pPO_NO = '" + strCode + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                txtPoNo.Value = dt.Rows[0]["PO_NO"].ToString();

                dtpPoDt.Value = dt.Rows[0]["PO_DT"].ToString();
                cboPoType.SelectedValue = dt.Rows[0]["PO_TYPE"];
                txtCustCd.Value = dt.Rows[0]["CUST_CD"].ToString();
                txtCustNm.Value = dt.Rows[0]["CUST_NM"].ToString();
                txtUserId.Value = dt.Rows[0]["PUR_DUTY"].ToString();
                txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                dtxtExchRate.Value = dt.Rows[0]["EXCH_RATE"];

                if (dt.Rows[0]["CURRENCY"].ToString() == "KRW")
                {
                    dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    dtxtExchRate.ReadOnly = true;
                }
                else
                {
                    dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                    dtxtExchRate.ReadOnly = false;
                }

                dtxtPoAmt.Value = dt.Rows[0]["PO_AMT"];
                dtxtNetAmt.Value = dt.Rows[0]["NET_AMT"];
                dtxtVatAmt.Value = dt.Rows[0]["VAT_AMT"];
                dtxtTotAmt.Value = dt.Rows[0]["TOT_AMT"];
                txtCustDutyNm.Value = dt.Rows[0]["CUST_DUTY_NM"].ToString();
                txtCustTel.Value = dt.Rows[0]["CUST_DUTY_TEL"].ToString();
                txtPaymentMeth.Value = dt.Rows[0]["PAYMENT_METH"].ToString();
                txtPaymentMethNm.Value = dt.Rows[0]["PAYMENT_METH_NM"].ToString();
                dtxtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"];
                txtPayoutMeth.Value = dt.Rows[0]["PAYOUT_METH"].ToString();
                txtPayoutMethNm.Value = dt.Rows[0]["PAYOUT_METH_NM"].ToString();
                txtPayoutRemark.Value = dt.Rows[0]["PAYOUT_METH_REMARK"].ToString();

                txtPayRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                im_yn = dt.Rows[0]["IM_YN"].ToString();

                if (dt.Rows[0]["IM_YN"].ToString() == "Y")
                {
                    c1DockingTab1.TabPages[1].Enabled = true;

                    dtpContDt.Value = dt.Rows[0]["CONTRACT_DT"].ToString();
                    if (dt.Rows[0]["VALID_DT"].ToString() != "") dtpValidDt.Value = dt.Rows[0]["VALID_DT"].ToString();
                    txtInvoiceNo.Value = dt.Rows[0]["INVOICE_NO"].ToString();
                    txtCostCond.Value = dt.Rows[0]["COST_COND"].ToString();
                    txtCostCondNm.Value = dt.Rows[0]["COST_COND_NM"].ToString();
                    txtTransMeth.Value = dt.Rows[0]["TRANS_METH"].ToString();
                    txtTransMethNm.Value = dt.Rows[0]["TRANS_METH_NM"].ToString();
                    txtTransBank.Value = dt.Rows[0]["TRANS_BANK_CD"].ToString();
                    txtTransBankNm.Value = dt.Rows[0]["TRANS_BANK_NM"].ToString();
                    txtTransPlace.Value = dt.Rows[0]["TRANS_PLACE"].ToString();
                    txtACsut.Value = dt.Rows[0]["APPLICANT_CUST"].ToString();
                    txtACsutNm.Value = dt.Rows[0]["APPLICANT_CUST_NM"].ToString();
                    txtMaker.Value = dt.Rows[0]["MAKER_CUST"].ToString();
                    txtMakerNm.Value = dt.Rows[0]["MAKER_CUST_NM"].ToString();
                    txtAgent.Value = dt.Rows[0]["AGENT_CUST"].ToString();
                    txtAgentNm.Value = dt.Rows[0]["AGENT_CUST_NM"].ToString();
                    txtOrigin.Value = dt.Rows[0]["ORIGIN_CD"].ToString();
                    txtOriginNm.Value = dt.Rows[0]["ORIGIN_NM"].ToString();
                    txtPactType.Value = dt.Rows[0]["PACK_TYPE"].ToString();
                    txtPactTypeNm.Value = dt.Rows[0]["PACK_TYPE_NM"].ToString();
                    txtInsMeth.Value = dt.Rows[0]["INSPECT_METH"].ToString();
                    txtInsMethNm.Value = dt.Rows[0]["INSPECT_METH_NM"].ToString();
                    txtDischCity.Value = dt.Rows[0]["DISCHGE_CITY"].ToString();
                    txtDischPort.Value = dt.Rows[0]["DISCHGE_PORT"].ToString();
                    txtDischPortNm.Value = dt.Rows[0]["DISCHGE_PORT_NM"].ToString();
                    txtLoadingPort.Value = dt.Rows[0]["LOADING_PORT"].ToString();
                    txtLoadingPortNm.Value = dt.Rows[0]["LOADING_PORT_NM"].ToString();
                    txtLoadingTerm.Value = dt.Rows[0]["LOADING_TERM"].ToString();
                    Set_Tab2("1", "R");
                }
                else
                {
                    Set_Tab2("2", "R");
                    c1DockingTabPage2.Enabled = false;
                }

                c1DockingTab1.SelectedIndex = 0;

                //확정여부
                if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                {
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                    {
                        rdoCfmY.Checked = true;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                        SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
                    }
                    else
                    {
                        rdoCfmN.Checked = true;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                        SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
                    }
                }
                else { rdoCfmN.Checked = true; }

                txtPoNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                txtPoNo.ReadOnly = true;

                //PO 상태
                string strSts = dt.Rows[0]["PO_STATUS"].ToString();

                if (Convert.ToInt16(strSts) > 0)
                {
                    btnConfirmOk.Enabled = false;
                    btnConfirmCancel.Enabled = false;

                    btnQualityAll.Enabled = false;
                    btnReqRef.Enabled = false;
                    btnEstRef.Enabled = false;
                }
                else if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y")
                {
                    btnConfirmOk.Enabled = false;
                    btnConfirmCancel.Enabled = true;

                    btnQualityAll.Enabled = false;
                    btnReqRef.Enabled = false;
                    btnEstRef.Enabled = false;
                }
                else
                {
                    btnConfirmOk.Enabled = true;
                    btnConfirmCancel.Enabled = false;

                    btnQualityAll.Enabled = true;
                    btnReqRef.Enabled = true;
                    btnEstRef.Enabled = true;
                }
                panel3.Enabled = false;
                //Detail그리드 정보.
                SubSearch_Detail(strCode, dt.Rows[0]["CONFIRM_YN"].ToString());

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

        private void SubSearch_Detail(string strCode, string strCfm)
        {
            try
            {
                //Detail그리드 정보.
                string strSql1 = " usp_MIM519  'S3' ";
                strSql1 = strSql1 + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                strSql1 = strSql1 + ", @pPO_NO ='" + strCode + "' ";
                strSql1 = strSql1 + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                //확정여부에 따른 화면 Locking
                if (strCfm == "Y")
                {
                    fpSpread1.ActiveSheet.Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택")].Visible = false;
                    //Detail Locking설정					

                    UIForm.FPMake.grdReMake(fpSpread1,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공장") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"

                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가") + "|3"        // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가") + "|3"    // 2019.02.22. hma 추가
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|3"
                        );

                }
                else
                {
                    fpSpread1.ActiveSheet.Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택")].Visible = true;

                    //Detail Locking해제
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실적여부")].Text == "4") //실적등록이 됬다면
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공장") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"

                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가") + "|3"        // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가") + "|3"    // 2019.02.22. hma 추가
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|3"
                                );
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")].Text == "N") //프로젝트, 차수 realonly
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공장") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|1"

                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가") + "|0"        // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가") + "|0"    // 2019.02.22. hma 추가
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|0"
                                    );
                            else
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    SystemBase.Base.GridHeadIndex(GHIdx1, "일괄선택") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공장") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "규격") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단위_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|1"

                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "견적금액") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "NEGO금액") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가단가") + "|0"        // 2019.02.22. hma 수정: 원가금액=>원가단가로 변경
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "원가검토단가") + "|0"    // 2019.02.22. hma 추가
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함구분") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면번호") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "도면REV.") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정문서") + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공정규격") + "|0"
                                    );

                        }
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

        #endregion

        #region 무역정보필수셋팅
        private void Set_Tab2(string div, string mode)
        {
            if (div == "0")
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
                if (mode == "U") SystemBase.Validation.GroupBox_Reset(groupBox4);
            }
            else
            {
                dtpContDt.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtCostCond.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtTransMeth.BackColor = SystemBase.Validation.Kind_LightCyan;
                txtACsut.BackColor = SystemBase.Validation.Kind_LightCyan;

                if (mode == "U")
                {
                    txtACsut.Value = "KB065";
                    txtACsutNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtACsut.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

        private void btnUser_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_M_COMMON 'M011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtUserId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtUserId.Value = Msgs[0].ToString();
                    txtUserNm.Value = Msgs[1].ToString();
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

        private void butInID_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInID.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInID.Value = Msgs[0].ToString();
                    txtInNm.Value = Msgs[1].ToString();
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
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtSProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
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

        private void butCust_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void butPaymentMeth_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPaymentMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00033", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "결제방법 팝업");
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
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butPayout_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='M018', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPayoutMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00083", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "지불방법 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPayoutMeth.Value = Msgs[0].ToString();
                    txtPayoutMethNm.Value = Msgs[1].ToString();
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

        private void butCostCond_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S005', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtCostCond.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00034", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "가격조건 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtCostCond.Value = Msgs[0].ToString();
                    txtCostCondNm.Value = Msgs[1].ToString();
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
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S013', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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

        private void butTransBank_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'B070', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTransBank.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTransBank.Value = Msgs[0].ToString();
                    txtTransBankNm.Value = Msgs[1].ToString();
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

        private void butACsut_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtACsut.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtACsut.Value = Msgs[1].ToString();
                    txtACsutNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void butMaker_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtMaker.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMaker.Value = Msgs[1].ToString();
                    txtMakerNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void butAgent_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW002 pu = new WNDW002(txtAgent.Text, "P");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtAgent.Value = Msgs[1].ToString();
                    txtAgentNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            strBtn = "N";
        }

        private void butOrigin_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S006', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
            }
            strBtn = "N";
        }

        private void butPactType_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S007', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtPactType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00038", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "포장방법 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtPactType.Value = Msgs[0].ToString();
                    txtPactTypeNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butInsMeth_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInsMeth.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00039", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사방법 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInsMeth.Value = Msgs[0].ToString();
                    txtInsMethNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butDischPort_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S009', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDischPort.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00040", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "도착항 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDischPort.Value = Msgs[0].ToString();
                    txtDischPortNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void butLoadingPort_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pSPEC1='S009', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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

        private void btnWorkorderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW006 pu = new WNDW006(txtWorkorderNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtWorkorderNo.Value = Msgs[1].ToString();
                    txtWorkorderNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 외주구매요청 참조
        private void btnReqRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM519P1 frm1 = new MIM519P1(fpSpread1);
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal;
                    if (Msgs == "Y")
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            if (strHead == "I")
                            {
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고명")].Text
                                    = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text, " AND PLANT_CD ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장")].Value.ToString() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "위치명")].Text
                                    = SystemBase.Base.CodeName("LOCATION_CD", "LOCATION_NM", "B_LOCATION_INFO", fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고위치")].Text, " AND SL_CD ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                                string strSql = " usp_MIM519 @pTYPE = 'S4' ";
                                strSql += "                , @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                strSql += "                , @pCUST_CD = '" + txtCustCd.Text + "' ";
                                strSql += "                , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                                if (dt.Rows.Count > 0)
                                {
                                    double po_price = 0, po_Qty = 0;
                                    po_price = Convert.ToDouble(dt.Rows[0][0].ToString());
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Text != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value.ToString() != "0")
                                    {
                                        po_Qty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주수량")].Value.ToString());
                                    }

                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = po_price;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value = po_price * po_Qty;
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = 0;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주금액")].Value = 0;
                                }

                                Set_Amt(i);

                                string strQuery = " usp_PCC009  'P2' ";
                                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strQuery = strQuery + ", @pREQ_NO ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청번호")].Text.ToString() + "' ";
                                strQuery = strQuery + ", @pREQ_SEQ ='" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청순번")].Text.ToString() + "' ";

                                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery);

                                if (dt1.Rows.Count != 0)
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품질증빙문서")].Value = dt1.Rows[0]["Q_REQ_DOC_NM"].ToString();
                                }

                            }
                        }
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEstRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                MIM519P2 frm2 = new MIM519P2(fpSpread1);
                frm2.ShowDialog();
                if (frm2.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm2.ReturnVal;
                    if (Msgs == "Y")
                    {	//거래처셋팅
                        txtCustCd.Value = frm2.ReturnStr;
                        txtCustCd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        txtCustCd.ReadOnly = true;
                        butCust.Enabled = false;
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            if (strHead == "I") Set_Amt(i);
                        }
                    }
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQualityAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (get_Check())
                {
                    if (strAutoPoNo == "")
                    {
                        MessageBox.Show("저장하고 실행하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MIM519P4 frm1 = new MIM519P4(fpSpread1, strAutoPoNo);
                    frm1.ShowDialog();
                    if (frm1.DialogResult == DialogResult.OK)
                    {
                        string Msgs = frm1.ReturnVal;
                        if (Msgs == "Y")
                        {
                            string cfm = "N";
                            if (rdoCfmY.Checked == true) cfm = "Y";
                            SubSearch_Detail(strAutoPoNo, cfm);
                        }
                    }
                }
                else
                    MessageBox.Show("일괄선택에 체크된 것이 없습니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 일괄선택 체크여부
        private bool get_Check()
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 2].Text == "True") return true;
            }
            return false;

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
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_MIM519  'P0'";
                strSql += ", @pPO_NO = '" + strAutoPoNo + "' ";
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
                this.Cursor = Cursors.Default;
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SubSearch(strAutoPoNo);
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

        private void txtSProjectNo_TextChanged(object sender, System.EventArgs e)
        {
            if (strBtn == "N")
                txtSProjectNo.Value = "";
        }

        private void txtInID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtInID.Text != "")
                    {
                        txtInNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtInID.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtInNm.Value = "";
                    }
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
                if (strBtn == "N")
                {
                    if (txtCustCd.Text != "")
                    {
                        txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCustNm.Value = "";
                    }
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
                        txtPaymentMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPaymentMeth.Text, " AND MAJOR_CD = 'S004' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
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

        private void txtPayoutMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPayoutMeth.Text != "")
                    {
                        txtPayoutMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPayoutMeth.Text, " AND MAJOR_CD = 'M018' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPayoutMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtCostCond_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtCostCond.Text != "")
                    {
                        txtCostCondNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtCostCond.Text, " AND MAJOR_CD = 'S005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtCostCondNm.Value = "";
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

        private void txtTransBank_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtTransBank.Text != "")
                    {
                        txtTransBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtTransBank.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtTransBankNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtACsut_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtACsut.Text != "")
                    {
                        txtACsutNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtACsut.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtACsutNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtMaker_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtMaker.Text != "")
                    {
                        txtMakerNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtMaker.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtMakerNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtAgent_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtAgent.Text != "")
                    {
                        txtAgentNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtAgent.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtAgentNm.Value = "";
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

        private void txtPactType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtPactType.Text != "")
                    {
                        txtPactTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtPactType.Text, " AND MAJOR_CD = 'S007' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtPactTypeNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtInsMeth_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtInsMeth.Text != "")
                    {
                        txtInsMethNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInsMeth.Text, " AND MAJOR_CD = 'S008' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtInsMethNm.Value = "";
                    }
                }
            }
            catch
            {

            }
        }

        private void txtDischPort_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtDischPort.Text != "")
                    {
                        txtDischPortNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDischPort.Text, " AND MAJOR_CD = 'S009' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");
                    }
                    else
                    {
                        txtDischPortNm.Value = "";
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

        #endregion

        #region Text Leave
        private void txtSUserId_Leave(object sender, System.EventArgs e)
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
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSUserId.Value = "";
                        txtSUserNm.Value = "";
                        txtSUserId.Focus();
                    }
                }
                
            }
            catch
            {

            }
        }

        private void txtUserId_Leave(object sender, System.EventArgs e)
        {
            if (strBtn == "N" && txtUserId.Text.Trim() != "")
            {
                string temp = "";
                temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                if (temp != "")
                    txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                else
                {
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //구매담당자가 아닙니다
                    txtUserId.Text = "";
                    txtUserNm.Value = "";
                    txtUserId.Focus();
                }
            }
            try
            {
                if (strBtn == "N" && txtUserId.Text.Trim() != "")
                {
                    string temp = "";
                    temp = SystemBase.Base.CodeName("PUR_DUTY", "PUR_DUTY", "M_PUR_DUTY", txtUserId.Text, " AND USE_YN = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
                    if (temp != "")
                    {
                        if (txtUserId.Text != "")
                        {
                            txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                        }
                        else
                        {
                            txtUserNm.Value = "";
                        }
                    }
                    else
                    {
                        DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("M0001"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //구매담당자가 아닙니다
                        txtUserId.Value = "";
                        txtUserNm.Value = "";
                        txtUserId.Focus();
                    }
                }                
            }
            catch
            {

            }
        }
        #endregion

        #region combobox SelectedIndexChanged
        private void cboPoType_SelectedIndexChanged(object sender, System.EventArgs e)
        {

        }


        private void cboCurrency_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (cboCurrency.SelectedValue.ToString() == "KRW")
            {
                dtxtExchRate.Value = 1;
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtExchRate.ReadOnly = true;
            }
            else
            {
                dtxtExchRate.Value = 0;
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtExchRate.ReadOnly = false;

            }
        }
        #endregion

        #region 환율변경시 Detail 자동 업데이트 플래그 변경
        private void dtxtExchRate_TextChanged(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                if (strHead == "")
                { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
            }
        }
        #endregion

        #region MIM519_Activated
        private void MIM519_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpSPoDtFr.Focus();
        }

        private void MIM519_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region c1DockingTab1_Click
        private void c1DockingTab1_Click(object sender, System.EventArgs e)
        {
            if (im_yn == "N")
            {
                c1DockingTab1.SelectedIndex = 0;

            }
        }
        #endregion

    }
}