#region 작성정보
/*********************************************************************/
// 단위업무명 : 매출채권수금내역등록
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-27
// 작성내용 : 매출채권수금내역등록 및 조회
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
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


namespace SS.SSA002
{
    public partial class SSA002 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region 생성자
        public SSA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SSA002_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //그리드 콤보박스 세팅
            //MASTER
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "영업담당")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pType='S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//영업담당
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "매출형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'BN_TYPE', @pNAME = 'BN_TYPE_NM', @pSPEC1 = 'S_BN_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//매출형태

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//화폐단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT포함

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboBnType, "usp_S_COMMON @pTYPE = 'S050' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");//매출형태
            SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //영업담당
            SystemBase.ComboMake.C1Combo(cboCollectDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); //수금담당
            SystemBase.ComboMake.C1Combo(cboPaymentMeth, "usp_B_COMMON @pType='COMM', @pCODE = 'S004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);//결재방법
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);//화폐단위

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            dtpSBnDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpExpiryDt.Text = "2999-12-31";
            rdoAll.Checked = true;
            panel2.Enabled = false;
            panel3.Enabled = false;
            btnBnOk.Enabled = false;
            btnBnCancel.Enabled = false;
            cboCurrency.SelectedValue = "KRW"; //화폐단위
            cboPaymentMeth.SelectedValue = "CM"; //결제방법

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            if (txtBnNo.Text != "")
            {
                if (chkConfirm.Checked != true)
                {
                    UIForm.FPMake.RowInsert(fpSpread1);
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Value = "KRW";
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value = 1;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "수금액")].Value = 0;
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "수금자국금액")].Value = 0;
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //이미확정된데이터는 다른 작업을 할 수 없습니다.
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("S0005", "매출채권번호"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //매출채권 번호가 조회되지 않았습니다.
            }
        }
        #endregion

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            //마스터만 조회
            Search();
        }
        #endregion

        #region Search 조회함수
        private void Search()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    //발행처 유효성체크
                    if (txtSBillCustCd.Text != "" && txtSBillCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "발행처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 발행처 코드입니다.

                        txtSBillCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }
                    //수금처 유효성체크
                    if (txtSCollectCustCd.Text != "" && txtSCollectCustNm.Text == "")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "수금처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 수금처 코드입니다.

                        txtSCollectCustCd.Focus();
                        this.Cursor = Cursors.Default;

                        return;
                    }

                    string strCfmYn = "";
                    if (rdoNo.Checked == true) { strCfmYn = "N"; }
                    else if (rdoYes.Checked == true) { strCfmYn = "Y"; }
                    else { strCfmYn = ""; }

                    string strQuery = " usp_SSA002  @pTYPE = 'S1'";
                    strQuery += ", @pBN_DT_FR = '" + dtpSBnDtFr.Text + "' ";
                    strQuery += ", @pBN_DT_TO = '" + dtpSBnDtTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBILL_CUST = '" + txtSBillCustCd.Text + "' ";
                    strQuery += ", @pBILL_CUST_NM = '" + txtSBillCustNm.Text + "' ";
                    strQuery += ", @pCOLLECT_CUST = '" + txtSCollectCustCd.Text + "' ";
                    strQuery += ", @pCOLLECT_CUST_NM = '" + txtSCollectCustNm.Text + "' ";
                    strQuery += ", @pBN_CONFIRM_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pBN_NO = '" + txtSBnNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);

                    if (fpSpread2.Sheets[0].Rows.Count == 0)
                    {
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
                        fpSpread1.Sheets[0].Rows.Count = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
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

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    string strAutoBnNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "매출채권번호")].Text.ToString();//매출채권번호

                    SubSearch(strAutoBnNo);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 상세정보 조회
        private void SubSearch(string Code)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //매출채권Master정보
                string strSql = " usp_SSA002  'S2', @pBN_NO = '" + Code + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    bool ConfirmChk = false;

                    //확정여부
                    if (dt.Rows[0]["BN_CONFIRM_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["BN_CONFIRM_YN"].ToString() == "Y") { ConfirmChk = true; }
                        else { ConfirmChk = false; }
                    }
                    else { ConfirmChk = false; }

                    txtBnNo.Value = dt.Rows[0]["BN_NO"].ToString();
                    dtpBnDt.Value = dt.Rows[0]["BN_DT"].ToString();
                    cboBnType.SelectedValue = dt.Rows[0]["BN_TYPE"].ToString();
                    txtBillCustCd.Value = dt.Rows[0]["BILL_CUST"].ToString();
                    txtCollectCustCd.Value = dt.Rows[0]["COLLECT_CUST"].ToString();
                    cboSaleDuty.SelectedValue = dt.Rows[0]["SALE_DUTY"].ToString();
                    cboCollectDuty.SelectedValue = dt.Rows[0]["COLLECT_DUTY"].ToString();
                    chkConfirm.Checked = ConfirmChk;
                    cboPaymentMeth.SelectedValue = dt.Rows[0]["PAYMENT_METH"].ToString();
                    dtxtPaymentTerm.Value = dt.Rows[0]["PAYMENT_TERM"];
                    txtPaymentRemark.Value = dt.Rows[0]["PAYMENT_TERM_REMARK"].ToString();
                    cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"].ToString();
                    dtxtExchRate.Value = dt.Rows[0]["EXCH_RATE"];
                    dtxtBnAmt.Value = dt.Rows[0]["BN_AMT"];
                    dtxtBnAmtLoc.Value = dt.Rows[0]["BN_AMT_LOC"];
                    dtxtNetAmtLoc.Value = dt.Rows[0]["NET_AMT_LOC"];
                    dtxtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"];
                    txtTaxBizCd.Value = dt.Rows[0]["TAX_BIZ_CD"].ToString();
                    dtpExpiryDt.Value = dt.Rows[0]["EXPIRY_DT"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();

                    txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                   
                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    //매출채권Detail그리드 정보.
                    string strSql1 = " usp_SSA002  'S3' , @pBN_NO = '" + Code + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    if (txtSlipNo.Text != "")
                    {
                        btnSlip.Enabled = true;
                    }
                    else
                    {
                        btnSlip.Enabled = false;
                    }

                    //확정여부에 따른 화면 Locking
                    if (ConfirmChk == true)
                    {
                        if (Convert.ToInt32(dt.Rows[0]["BN_STATUS"].ToString()) > 0)
                        {
                            btnBnOk.Enabled = false;
                            btnBnCancel.Enabled = false;
                        }
                        else
                        {
                            btnBnOk.Enabled = false;
                            btnBnCancel.Enabled = true;
                        }

                        //Detail Locking설정
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형_2") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "수금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                                            );
                        }
                    }
                    else
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            GridSet(i);
                        }

                        btnBnOk.Enabled = true;
                        btnBnCancel.Enabled = false;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //확정상태가 아니면
            if (chkConfirm.Checked == false)
            {
                //상단 그룹박스 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                        string strBnSeq = "";

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                                {
                                //행수만큼 처리
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead.Length > 0)
                                    {
                                        switch (strHead)
                                        {
                                            case "U": strGbn = "U1"; break;
                                            case "I": strGbn = "I1"; break;
                                            case "D": strGbn = "D1"; break;
                                            default: strGbn = ""; break;
                                        }

                                        if (strGbn == "U2")
                                        {
                                            strBnSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출순번")].Value.ToString();
                                        }
                                        else
                                        {
                                            strBnSeq = "0";
                                        }

                                        string strSql = " usp_SSA002 '" + strGbn + "'";
                                        strSql += ", @pBN_NO = '" + txtBnNo.Text + "' ";
                                        strSql += ", @pCOLLECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수금순번")].Value + "' ";
                                        strSql += ", @pCURRENCY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Value + "' ";
                                        strSql += ", @pEXCH_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value + "' ";
                                        strSql += ", @pCOLLECT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text + "' ";
                                        strSql += ", @pCOLLECT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수금액")].Value + "' ";
                                        strSql += ", @pCOLLECT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수금자국금액")].Value + "' ";
                                        strSql += ", @pBANK_ACCT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text + "' ";
                                        strSql += ", @pPRRCPT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text + "' ";
                                        strSql += ", @pNOTE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text + "' ";
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
                            this.Cursor = Cursors.Default;
                            ERRCode = "ER";
                            MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        }
                    Exit:
                        dbConn.Close();

                        if (ERRCode == "OK")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Search();
                            SubSearch(txtBnNo.Text);

                            UIForm.FPMake.GridSetFocus(fpSpread2, txtBnNo.Text, SystemBase.Base.GridHeadIndex(GHIdx2, "매출채권번호"));
                            UIForm.FPMake.GridSetFocus(fpSpread1, strBnSeq, SystemBase.Base.GridHeadIndex(GHIdx1, "매출순번"));
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
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0038"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
                    }
                }
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("S0003"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //수금유형
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형_2"))
            {
                try
                {
                    string strQuery = " usp_S_COMMON 'S060', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00011", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수금유형조회");	//수금유형조회
                    pu.Width = 600;
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그

                        GridSet(Row);
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수금유형 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //계좌번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2"))
            {
                try
                {
                    SSA002P1 myForm = new SSA002P1(fpSpread1, Row, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = Msgs[2].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계좌번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //어음번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2"))
            {
                try
                {
                    SSA002P2 myForm = new SSA002P2(fpSpread1, Row, txtBillCustCd.Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "어음번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //선수금번호
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2"))
            {
                try
                {
                    SSA002P3 myForm = new SSA002P3(fpSpread1, Row, txtCollectCustCd.Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text);
                    myForm.ShowDialog();

                    if (myForm.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(myForm.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text = Msgs[0].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "선수금번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //수금유형에 따른 그리드 상태 변화
            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형명")].Text
                = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text, " And MAJOR_CD = 'S012'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");

            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형명")].Text != "")
            {
                GridSet(Row);
            }

            //환율
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "환율"))
            {
                double dblExchRate = 0;
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text != "")
                {
                    dblExchRate = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
                }

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금자국금액")].Value
                    = dblExchRate * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금액")].Value);
            }

            //수금자국금액
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수금액"))
            {
                double dblExchRate = 0;
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text != "")
                {
                    dblExchRate = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
                }

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금자국금액")].Value
                    = dblExchRate * Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금액")].Value);
            }
        }
        #endregion

        #region textBox 코드 입력시 코드명 자동입력
        //발행처
        private void txtSBillCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSBillCustCd.Text != "")
                {
                    txtSBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSBillCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSBillCustNm.Value = "";
                }
            }
            catch { }
        }
        private void txtBillCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBillCustCd.Text != "")
                {
                    // 2015.08.21. hma 수정(Start): 특정일자 기준의 거래처명을 가져오도록 함.
                    //txtBillCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBillCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    txtBillCustNm.Value = SystemBase.Base.GetCustName(dtpBnDt.Text, txtBillCustCd.Text);
                    // 2015.08.21. hma 수정(End)
                }
                else
                {
                    txtBillCustNm.Value = "";
                }
            }
            catch { }
        }
        //수금처
        private void txtSCollectCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSCollectCustCd.Text != "")
                {
                    txtSCollectCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSCollectCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSCollectCustNm.Value = "";
                }
            }
            catch { }
        }
        private void txtCollectCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtCollectCustCd.Text != "")
                {
                    // 2015.08.21. hma 수정(Start): 특정일자 기준의 거래처명을 가져오도록 함. 
                    //txtCollectCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCollectCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                    txtCollectCustNm.Value = SystemBase.Base.GetCustName(dtpBnDt.Text, txtCollectCustCd.Text);
                    // 2015.08.21. hma 수정(End)
                }
                else
                {
                    txtCollectCustNm.Value = "";
                }
            }
            catch { }
        }
        //신고사업장
        private void txtTaxBizCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTaxBizCd.Text != "")
                {
                    txtTaxBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBizCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtTaxBizNm.Value = "";
                }
            }
            catch { }
        }
        #endregion

        #region 팝업창 이벤트
        //프로젝트번호
        private void btnSProjectNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSProjectNo.Text = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //발행처
        private void btnSBillCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSBillCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSBillCustCd.Text = Msgs[1].ToString();
                    txtSBillCustNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발행처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //수금처
        private void btnSCollectCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtSCollectCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSCollectCustCd.Text = Msgs[1].ToString();
                    txtSCollectCustNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수금처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 확정, 확정취소
        private void Confirm(string strConfirmYn)
        {
            //상단 그룹박스 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_SSA002  'P1'";
                    strSql += ", @pBN_NO = '" + txtBnNo.Text + "' ";
                    strSql += ", @pBN_CONFIRM_YN = '" + strConfirmYn + "' ";
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
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SubSearch(txtBnNo.Text);
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
        //확정
        private void btnBnOk_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY068", "매출채권번호 " + txtBnNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("Y");
            }
        }
        //확정취소
        private void btnBnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY069", "매출채권번호 " + txtBnNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("N");
            }
        }
        #endregion

        #region 그리드 필수, 일반, 읽기적용 세팅
        private void GridSet(int Row)
        {
            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text == "DP")//계좌번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|1"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text == "NR")//어음번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|1"
                    );
            }
            else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수금유형")].Text == "PR")//선수금번호
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2") + "|0"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|1"
                    );
            }
            else
            {
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "은행명")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text = "";
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호")].Text = "";

                UIForm.FPMake.grdReMake(fpSpread1, Row,
                    SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "선수금번호_2") + "|3"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위") + "|1"
                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "환율") + "|1"
                    );
            }
        }
        #endregion

        #region 전표조회
        private void btnSlip_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSlipNo.Text != "")
                {
                    WNDW.WNDW026 pu = new WNDW.WNDW026(txtSlipNo.Text);
                    pu.ShowDialog();
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
