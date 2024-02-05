#region 작성정보
/*********************************************************************/
// 단위업무명 : 매출채권등록
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-26
// 작성내용 : 매출채권등록 및 조회
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
using System.Reflection;        // 2022.01.28. hma 추가


namespace SS.SSA001
{
    public partial class SSA001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        string strLinkSlipNo = "";     // 2022.01.27. hma 추가: 링크전표번호
        #endregion

        #region 생성자
        public SSA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SSA001_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //그리드 콤보박스 세팅
            //MASTER
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "영업담당")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pType='S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//영업담당
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "매출형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'TABLE', @pCODE = 'BN_TYPE', @pNAME = 'BN_TYPE_NM', @pSPEC1 = 'S_BN_TYPE' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//매출형태

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
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
            SystemBase.ComboMake.C1Combo(cboCSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 2022.01.27. hma 추가: 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboMSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 2022.01.27. hma 추가: 그룹웨어상태

            //폼 컨트롤 초기화
            Control_Setting();

            // 2022.02.15. hma 추가(Start)
            lnkJump1.Text = "확정전표상신";
            strJumpFileName1 = "AD.ACD001.ACD001";
            lnkJump2.Text = "반제전표상신";
            strJumpFileName2 = "AD.ACD001.ACD001";
            strLinkSlipNo = "";
            // 2022.02.15. hma 추가(End)
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            dtpSBnDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSBnDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpExpiryDt.Text = "2999-12-31";
            txtTaxBizCd.Text = SystemBase.Base.CodeName("BIZ_CD", "TAX_BIZ_CD", "B_BIZ_PLACE", SystemBase.Base.gstrBIZCD, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'"); //신고사업장
            rdoAll.Checked = true;
            panel2.Enabled = false;
            panel3.Enabled = false;
            btnDnRef.Enabled = true;
            btnBnOk.Enabled = false;
            btnBnCancel.Enabled = false;
            cboCurrency.SelectedValue = "KRW"; //화폐단위
            cboPaymentMeth.SelectedValue = "CM"; //결제방법

            dtxtExchRate.Value = 1;
            dtxtExchRate.Tag = ";2;;";
            dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
            dtxtExchRate.ReadOnly = true;

            lblProcess.Visible = false;
            chkProcessCollect.Visible = false;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

            cboCSlipGwStatus.Text = "";      // 2022.02.15. hma 추가: 결재상태 초기화
            cboMSlipGwStatus.Text = "";      // 2022.02.15. hma 추가: 결재상태 초기화
            btnMinusCancel.Enabled = false;  // 2022.02.15. hma 추가: 반제취소 버튼 비활성화
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //확정상태면
            if (chkConfirm.Checked == true)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
                this.Cursor = Cursors.Default;
                return;
            }

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
                    string strSql = " usp_SSA001  'D1', @pBN_NO = '" + txtBnNo.Text + "'";
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
                    Search();
                    fpSpread1.Sheets[0].Rows.Count = 0;
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

                    string strQuery = " usp_SSA001  @pTYPE = 'S1'";
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
                string strSql = " usp_SSA001  'S2', @pBN_NO = '" + Code + "' ";
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

                    // 2022.01.27. hma 추가(Start): 결재상태 및 반제전표번호, 반제승인 추가
                    txtCSlipNo.Value = dt.Rows[0]["CFM_SLIP_NO"].ToString();
                    cboCSlipGwStatus.SelectedValue = dt.Rows[0]["CFM_GW_STATUS"].ToString();
                    txtMinusConfirm.Value = dt.Rows[0]["MINUS_CONFIRM_YN"].ToString();
                    txtMSlipNo.Value = dt.Rows[0]["MINUS_SLIP_NO"].ToString();
                    cboMSlipGwStatus.SelectedValue = dt.Rows[0]["MINUS_GW_STATUS"].ToString();
                    txtSlipConfirmYn.Value = dt.Rows[0]["SLIP_CONFIRM_YN"].ToString();                  // 전표번호 확정여부
                    txtMinusSlipConfirmYn.Value = dt.Rows[0]["MINUS_SLIP_CONFIRM_YN"].ToString();       // 반제전표 확정여부
                    // 2022.01.27. hma 추가(End)

                    if (dt.Rows[0]["VAT_UNI_FLAG"].ToString() != "")
                    {
                        if (dt.Rows[0]["VAT_UNI_FLAG"].ToString() == "1")
                        { rdoUnite.Checked = true; }
                        else
                        { rdoDutch.Checked = true; }
                    }
                    else { rdoDutch.Checked = true; }

                    if (dt.Rows[0]["VAT_INC_FLAG"].ToString() != "")
                    {
                        if (dt.Rows[0]["VAT_INC_FLAG"].ToString() == "1")
                        { rdoGroup.Checked = true;}
                        else
                        { rdoExtra.Checked = true; }
                    }
                    else { rdoExtra.Checked = true; }

                    txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                   
                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    //매출채권Detail그리드 정보.
                    string strSql1 = " usp_SSA001  'S3' , @pBN_NO = '" + Code + "' ";
                    strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //확정여부에 따른 화면 Locking
                    if (ConfirmChk == true)     // 확정상태인 경우
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                        chkProcessCollect.Enabled = false;

                        if (Convert.ToInt32(dt.Rows[0]["BN_STATUS"].ToString()) > 0)        // 매출채권진행상태가 매출채권등록 이후인 경우
                        {
                            btnBnOk.Enabled = false;
                            btnBnCancel.Enabled = false;

                            lblProcess.Visible = false;
                            chkProcessCollect.Visible = false;
                        }
                        else
                        {
                            btnBnOk.Enabled = false;

                            // 2022.01.27. hma 추가(Start): 확정상태이면서 결재상태가 상신대기/반려/승인 상태이면 확정취소 버튼 활성화되게.
                            //btnBnCancel.Enabled = true;
                            if ((txtSlipNo.Text != "" && txtCSlipNo.Text == "") ||
                                ((txtCSlipNo.Text != "") &&
                                 (cboCSlipGwStatus.SelectedValue.ToString() == "READY" || cboCSlipGwStatus.SelectedValue.ToString() == "REJECT" ||
                                  (cboCSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y"))))
                                btnBnCancel.Enabled = true;
                            else
                                btnBnCancel.Enabled = false;
                            // 2022.01.27. hma 추가(End)

                            lblProcess.Visible = false;
                            chkProcessCollect.Visible = false;
                        }

                        // Detail Locking설정
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                                            );
                        }
                    }
                    else
                    {
                        // 2022.01.27. hma 추가(Start): 미확정 상태이지만 반제전표번호가 없거나, 반제전표번호가 있으면서 반제전표상태가 승인이고 반제승인여부가 Y인 경우
                        if ((txtMSlipNo.Text == "") ||
                           (txtMSlipNo.Text != "" &&
                            (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                        {
                        // 2022.01.27. hma 추가(End)
                            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                        }
                        else
                        {
                            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);        // 2022.02.19. hma 추가
                        }

                        chkProcessCollect.Enabled = true;

                        //출고참조팝업
                        //if (dt.Rows.Count > 0) { btnDnRef.Enabled = false; }

                        btnDnRef.Enabled = true;

                        // 2022.01.27. hma 수정(Start): 미확정상태이지만 반제전표 결재상태가 상신대기/반려 상태일때만 확정취소 버튼 활성화되게.
                        //                              또한 결재상태가 승인이면서 반제승인이 Y인 경우에도 확정취소 버튼 활성화.(반제처리 위해)
                        //btnBnOk.Enabled = true;
                        if ((txtMSlipNo.Text == "") ||
                            (txtMSlipNo.Text != "" &&
                             (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                            btnBnOk.Enabled = true;
                        else
                            btnBnOk.Enabled = false;
                        // 2022.01.27. hma 수정(End)

                        // 2022.01.27. hma 수정(Start): 미확정건이지만 반제전표가 생성된 경우에는 확정취소 버튼 비활성화 처리.
                        btnBnCancel.Enabled = false;    // 2022.02.15. hma 수정: 아래 부분 주석 처리하고 이 부분 주석 해제. 미확정 상태일때는 확정취소 버튼 비활성화 처리.
                        //if (txtMSlipNo.Text != "" &&
                        //     (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        //    btnBnCancel.Enabled = false;
                        // 2022.01.27. hma 수정(End)

                        lblProcess.Visible = true;
                        chkProcessCollect.Visible = true;

                        // 2022.01.27. hma 추가(Start): 반제전표 결재상태에 따라 반제취소 버튼 활성화 처리. 반제전표 결재상태가 상신대기, 반려이면 활성화.
                        btnMinusCancel.Enabled = false;     // 2022.02.15. hma 추가
                        if (txtMSlipNo.Text != "" &&
                            (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        {
                            btnMinusCancel.Enabled = true;
                        }
                        // 2022.01.27. hma 추가(End)

                        // 2022.02.15. hma 수정(Start): 확정 버튼이 활성화 되어있을 경우에만 참조 버튼 활성화.
                        if (btnBnOk.Enabled == true)
                            btnDnRef.Enabled = true;
                        else
                            btnDnRef.Enabled = false;
                        // 2022.02.15. hma 추가(End)

                        //Detail Locking해제
                        // 2022.01.28. hma 추가(Start): 미확정상태이지만 반제전표 승인이 완료가 아닌 경우에는 비활성화 처리
                        string strSlipConfirmYn = "", strMinusSlipNo = "", strMinusGwStatus = "";
                        strSlipConfirmYn = txtSlipConfirmYn.Text;
                        strMinusSlipNo = txtMSlipNo.Text;

                        if (cboMSlipGwStatus.Text == "")
                            strMinusGwStatus = "";
                        else
                            strMinusGwStatus = cboMSlipGwStatus.SelectedValue.ToString();
                        // 2022.01.28. hma 추가(End)

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {                           
                            // 2022.01.28. hma 추가(Start): 미확정 상태이지만 반제전표를 생성해서 승인상태가 아니면 비활성화
                            if (strSlipConfirmYn != "Y" && strMinusSlipNo != "" && strMinusGwStatus != "APPR")
                            {
                                // I3: 읽기전용이면서 필수항목에서 제외
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|3"        
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액") + "|3"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3"
                                                            );
                            }
                            else
                            {
                           // 2022.01.28. hma 추가(End)
                                // I1: 필수입력
                                UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "단가") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액") + "|1"
                                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                                                            );
                            }
                            
                        }
                    }

                    if (dt.Rows[0]["CURRENCY"].ToString() == "KRW")
                    {
                        dtxtExchRate.Value = 1;
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtxtExchRate.ReadOnly = true;
                    }
                    else
                    {
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtExchRate.ReadOnly = false;
                    }

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                    }

                    if (txtSlipNo.Text != "")
                    {
                        btnSlip.Enabled = true;
                    }
                    else
                    {
                        btnSlip.Enabled = false;
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

            string strAutoBnNo = "";
            string strMstType = "";
            string strInUpFlag = "I";

            GroupBox[] gBox = null;

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            if (chkConfirm.Checked == false)
            {
                //컨트롤 체크값 초기화
                strSaveData = "";
                //컨트롤 체크 함수
                gBox = new GroupBox[] { groupBox2 };
                SystemBase.Validation.Control_Check(gBox, ref strSaveData);

                //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
                if (strSearchData == strSaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
                {
                    //변경되거나 처리할 데이터가 없습니다.
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

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
                            if (txtBnNo.Text == "") { strMstType = "I1"; }
                            else { strMstType = "U1"; strInUpFlag = "U"; }

                            string strVatUniFlag = "1", strVatIncFlag = "2"; //VAT통합구분(default :통합), VAT포함구분(default :별도)
                            if (rdoDutch.Checked == true) { strVatUniFlag = "2"; } //개별
                            if (rdoGroup.Checked == true) { strVatIncFlag = "1"; } //통합

                            string strSql = " usp_SSA001 '" + strMstType + "'";
                            strSql += ", @pBN_NO = '" + txtBnNo.Text + "' ";
                            strSql += ", @pBN_DT = '" + dtpBnDt.Text + "' ";
                            strSql += ", @pBN_TYPE = '" + cboBnType.SelectedValue.ToString() + "' ";
                            strSql += ", @pBILL_CUST = '" + txtBillCustCd.Text + "' ";
                            strSql += ", @pCOLLECT_CUST = '" + txtCollectCustCd.Text + "' ";
                            strSql += ", @pCOLLECT_DUTY = '" + Convert.ToString(cboSaleDuty.SelectedValue) + "' "; //2010-05-03 내부적으로 영업담당자와 같이 등록
                            strSql += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue.ToString() + "' ";
                            strSql += ", @pTAX_BIZ_CD = '" + txtTaxBizCd.Text + "' ";
                            strSql += ", @pPAYMENT_METH = '" + cboPaymentMeth.SelectedValue.ToString() + "' ";
                            strSql += ", @pPAYMENT_TERM = '" + dtxtPaymentTerm.Value + "' ";
                            strSql += ", @pEXPIRY_DT = '" + dtpExpiryDt.Text + "' ";
                            strSql += ", @pPAYMENT_TERM_REMARK = '" + txtPaymentRemark.Text + "' ";
                            strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                            strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Value + "' ";
                            strSql += ", @pVAT_UNI_FLAG = '" + strVatUniFlag + "' ";
                            strSql += ", @pVAT_INC_FLAG = '" + strVatIncFlag + "' ";
                            strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                            strSql += ", @pTEMP_SLIP_NO = '" + txtSlipNo.Text + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();
                            strAutoBnNo = dt.Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            /////////////////////////////////////////////// DETAIL 저장 시작 /////////////////////////////////////////////////
                            //그리드 상단 필수 체크
                            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                            {
                                //Detail정보를 모두 삭제할 경우 Master정보를 삭제할지 물어보고 아니면 취소한다.
                                if (DelCheck() == false)
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0027");
                                    DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                    if (dsMsg == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            strInUpFlag = "D2";

                                            string strDelSql = " usp_SSA001  'D1'";
                                            strDelSql += ", @pBN_NO = '" + strAutoBnNo + "' ";
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
                                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                                        }
                                    Exit1:
                                        dbConn.Close();

                                        if (ERRCode == "OK")
                                        {
                                            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            Search();

                                            //컨트롤 체크값 초기화
                                            strSearchData = "";
                                            //컨트롤 체크 함수
                                            gBox = new GroupBox[] { groupBox2 };
                                            SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                                            //그리드 셀 포커스 이동
                                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoBnNo, SystemBase.Base.GridHeadIndex(GHIdx2, "매출채권번호"));

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

                                        return;
                                    }
                                    else
                                    {
                                        Trans.Rollback();
                                        MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);//작업이 취소되었습니다.
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
                                            case "U": strGbn = "U2"; strInUpFlag = "U"; break;
                                            case "I": strGbn = "I2"; strInUpFlag = "I"; break;
                                            case "D": strGbn = "D2"; strInUpFlag = "D"; break;
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

                                        string strSubSql = " usp_SSA001 '" + strGbn + "'";
                                        strSubSql += ", @pBN_NO = '" + strAutoBnNo + "' ";
                                        strSubSql += ", @pBN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출순번")].Value + "' ";
                                        strSubSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSubSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSubSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value + "' ";
                                        strSubSql += ", @pBN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value + "' ";
                                        strSubSql += ", @pBN_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value + "' ";
                                        strSubSql += ", @pVAT_INC_FLAG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")].Value + "' ";
                                        strSubSql += ", @pBN_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' ";
                                        strSubSql += ", @pBN_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value + "' ";
                                        strSubSql += ", @pVAT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value + "' ";
                                        strSubSql += ", @pVAT_RATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value + "' ";
                                        strSubSql += ", @pBN_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value + "' ";
                                        strSubSql += ", @pDN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청번호")].Text + "' ";
                                        strSubSql += ", @pDN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Value + "' ";
                                        strSubSql += ", @pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "' ";
                                        strSubSql += ", @pSO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value + "' ";
                                        strSubSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        strSubSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                        strSubSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSubSql, dbConn, Trans);
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

                            /////////////////////////////////////////////// 금액집계 UPDATE 시작 /////////////////////////////////////////////////
                            string strSql1 = " usp_SSA001 'I3'";
                            strSql1 += ", @pBN_NO = '" + strAutoBnNo + "' ";
                            strSql1 += ", @pIN_UP_FLAG = '" + strInUpFlag + "' ";
                            strSql1 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt1 = SystemBase.DbOpen.TranDataTable(strSql1, dbConn, Trans);
                            ERRCode = dt1.Rows[0][0].ToString();
                            MSGCode = dt1.Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                            SubSearch(strAutoBnNo);

                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoBnNo, SystemBase.Base.GridHeadIndex(GHIdx2, "매출채권번호"));
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0041"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.     // 2022.01.28. hma 수정: S0003 => B0041 변경
            }

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

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            double dblQty = 0, dblPrice = 0, dblBnAmt = 0;
            dblQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
            dblPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);

            //수량
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수량"))
            {
                try
                {
                    string strQuery = "usp_SSA001 @pTYPE = 'C1' ";
                    strQuery += " , @pDN_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청번호")].Value + "' ";
                    strQuery += " , @pDN_SEQ = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Value + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        if (dblQty > Convert.ToDouble(dt.Rows[0]["BALANCE_BN_QTY"].ToString()))
                        {
                            //요청수량은 잔량(||수량 - ||수량누계) 보다 같거나 적은 범위내에서 수정 가능합니다.
                            MessageBox.Show(SystemBase.Base.MessageRtn("S0004", "출고||매출"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    dblBnAmt = dblQty * dblPrice;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value = dblBnAmt;

                    //매출자국금액, VAT금액구하기
                    VatAmt(Row);
                    
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매출금액 계산중(수량)"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
            }
            //단가
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "단가"))
            {
                try
                {
                    dblBnAmt = dblQty * dblPrice;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value = dblBnAmt;

                    //매출자국금액, VAT금액구하기
                    VatAmt(Row);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매출금액 계산중(단가)"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //VAT금액
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액"))
            {
                try
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value
                        = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value) * Convert.ToDouble(dtxtExchRate.Value);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "VAT금액 계산중"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 매출자국금액, Vat금액구하기
        private void VatAmt(int Row)
        {
            try
            {
                double dblQty = 0, dblPrice = 0, dblBnAmt = 0, dblBnAmtLoc = 0, dblVatAmt = 0;
                dblQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                dblPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);

                //금액구하기
                dblBnAmt = dblQty * dblPrice;
                dblBnAmtLoc = dblBnAmt * Convert.ToDouble(dtxtExchRate.Value);
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value = dblBnAmtLoc;

                //VAT금액구하기
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")].Value.ToString() == "2")
                {
                    dblVatAmt = Math.Floor(dblBnAmt * (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value) * 0.01));

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dblVatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dblVatAmt * Convert.ToDouble(dtxtExchRate.Value);
                }
                else
                {
                    dblVatAmt = Math.Floor(dblBnAmt - (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value) / 1.1));

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dblVatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dblVatAmt * Convert.ToDouble(dtxtExchRate.Value);
                }
            }
            catch(Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "VAT금액 계산중"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 매출자국금액, Vat금액구하기
        private void VatAmt2(int Row)
        {
            try
            {
                double dblQty = 0, dblPrice = 0, dblBnAmt = 0, dblBnAmtLoc = 0, dblVatAmt = 0;
                dblQty = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                //dblPrice = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
                dblBnAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value);

                //금액구하기
                //dblBnAmt = dblQty * dblPrice;

                dblPrice = Math.Floor(dblBnAmt / dblQty);
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = dblPrice;

                dblBnAmtLoc = dblBnAmt * Convert.ToDouble(dtxtExchRate.Value);
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value = dblBnAmtLoc;

                //VAT금액구하기
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")].Value.ToString() == "2")
                {
                    dblVatAmt = Math.Floor(dblBnAmt * (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value) * 0.01));

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dblVatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dblVatAmt * Convert.ToDouble(dtxtExchRate.Value);
                }
                else
                {
                    dblVatAmt = Math.Floor(dblBnAmt - (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value) / 1.1));

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = dblVatAmt;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dblVatAmt * Convert.ToDouble(dtxtExchRate.Value);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "VAT금액 계산중"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void btnBillCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtBillCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtBillCustCd.Text = Msgs[1].ToString();
                    txtBillCustNm.Value = Msgs[2].ToString();
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
        private void btnCollectCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCollectCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCollectCustCd.Text = Msgs[1].ToString();
                    txtCollectCustNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수금처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //신고사업장
        private void btnTaxBiz_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_S_COMMON @pTYPE ='S070', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금신고사업장조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBizCd.Text = Msgs[0].ToString();
                    txtTaxBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "세금 신고 사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 화폐단위 변경시 환율세팅
        private void cboCurrency_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            if (cboCurrency.SelectedValue.ToString() == "KRW")
            {
                dtxtExchRate.Value = 1;
                dtxtExchRate.Tag = ";2;;";
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtExchRate.ReadOnly = true;
            }
            else
            {
                dtxtExchRate.Value = 0;
                dtxtExchRate.Tag = "환율;1;;";
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtExchRate.ReadOnly = false;

            }
        }
        #endregion

        #region 환율변경시 Detail 자동 업데이트 플래그 변경
        private void dtxtExchRate_ValueChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "")
                        { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }

                        //자국금액 재계산
                        double dblBnAmt = 0, dblVatAmt = 0;

                        dblBnAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value);
                        dblVatAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value = dblBnAmt * Convert.ToDouble(dtxtExchRate.Value);
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = dblVatAmt * Convert.ToDouble(dtxtExchRate.Value);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "환율 변경 적용중"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 수주/출고참조
        private void btnDnRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strCustCd = "";

                //발행처가 있으면 출고참조 주문처에 발행처코드 입력, 발행처가 없고 수금처가 있으면 수금처 입력
                if (txtBillCustCd.Text == "")
                {
                    if (txtCollectCustCd.Text == "")
                    { strCustCd = ""; }
                    else
                    { strCustCd = txtCollectCustCd.Text; }
                }
                else
                { strCustCd = txtBillCustCd.Text; }

                DataTable PopDt = new DataTable();
                SSA001P1 myForm = new SSA001P1(txtBnNo.Text, strCustCd);
                myForm.ShowDialog();

                if (myForm.DialogResult == DialogResult.OK)
                {
                    cboBnType.SelectedValue = myForm.cboBnType.SelectedValue.ToString();
                    cboPaymentMeth.SelectedValue = myForm.cboPaymentMeth.SelectedValue.ToString();
                    cboSaleDuty.SelectedValue = myForm.cboSaleDuty.SelectedValue.ToString();
                    cboCollectDuty.SelectedValue = myForm.cboSaleDuty.SelectedValue.ToString();
                    cboCurrency.SelectedValue = myForm.cboCurrency.SelectedValue.ToString();
                    rdoDutch.Checked = myForm.rdoDutch.Checked;
                    rdoUnite.Checked = myForm.rdoUnite.Checked;
                    rdoExtra.Checked = myForm.rdoExtra.Checked;
                    rdoGroup.Checked = myForm.rdoGroup.Checked;
                    txtBillCustCd.Text = myForm.txtSoldCustCd.Text;
                    txtCollectCustCd.Text = myForm.txtSoldCustCd.Text;
                    txtTaxBizCd.Text = SystemBase.Base.CodeName("BIZ_CD", "TAX_BIZ_CD", "B_BIZ_PLACE", SystemBase.Base.gstrBIZCD, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");

                    DataTable MyFormDt = new DataTable();
                    MyFormDt = myForm.ReturnDt;

                    if (MyFormDt != null)
                    {
                        //fpSpread1.Sheets[0].Rows.Count = 0; //grid초기화

                        int row = fpSpread1.Sheets[0].RowCount;

                        for (int i = 0; i < MyFormDt.Rows.Count; i++)
                        {
                            if (MyFormDt.Rows[i]["CHK"].ToString() == "1")
                            {
                                fpSpread1.Sheets[0].ActiveRowIndex = row-1;
                                UIForm.FPMake.RowInsert(fpSpread1);//행추가

                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = MyFormDt.Rows[i]["ITEM_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text = MyFormDt.Rows[i]["ITEM_NM"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = MyFormDt.Rows[i]["ITEM_SPEC"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = MyFormDt.Rows[i]["PROJECT_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = MyFormDt.Rows[i]["PROJECT_SEQ"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value = Convert.ToDouble(MyFormDt.Rows[i]["DN_QTY"]) - Convert.ToDouble(MyFormDt.Rows[i]["BN_QTY"]);
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = MyFormDt.Rows[i]["DN_UNIT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")].Value = MyFormDt.Rows[i]["VAT_INC_FLAG"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = MyFormDt.Rows[i]["DN_PRICE"];
                                //fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value = MyFormDt.Rows[i]["DN_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value = Convert.ToDouble(MyFormDt.Rows[i]["DN_AMT"]) - Convert.ToDouble(MyFormDt.Rows[i]["BN_AMT"]);
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value = MyFormDt.Rows[i]["VAT_TYPE"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = MyFormDt.Rows[i]["VAT_RATE"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = MyFormDt.Rows[i]["VAT_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = MyFormDt.Rows[i]["VAT_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value = MyFormDt.Rows[i]["DN_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고요청번호")].Text = MyFormDt.Rows[i]["DN_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출고순번")].Value = MyFormDt.Rows[i]["DN_SEQ"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text = MyFormDt.Rows[i]["SO_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Value = MyFormDt.Rows[i]["SO_SEQ"];

                                VatAmt2(row); 

                                row++;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "출고/수주 정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string chkCollect = "N";

                    if (chkProcessCollect.Checked == true) { chkCollect = "Y"; }

                    string strSql = " usp_SSA001  'P1'";
                    strSql += ", @pBN_NO = '" + txtBnNo.Text + "' ";
                    strSql += ", @pBN_CONFIRM_YN = '" + strConfirmYn + "' ";
                    strSql += ", @pCHK_PROCESS_COLLECT = '" + chkCollect + "' ";
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

        #region 그리드상 콤보박스 변경시
        private void fpSpread1_ComboSelChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //VAT유형
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형"))
            {
                string Query = " usp_S_COMMON @pTYPE = 'S040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")].Value + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = dt.Rows[0]["REL_CD1"]; }
                else
                { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT율")].Value = 0; }

                //VAT금액구하기
                VatAmt(e.Row);
            }
        }

        // 2022.01.28. hma 추가(Start)
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
                param = new object[1];				// 파라메터수가 4개인 경우
                param[0] = strLinkSlipNo;
            }
            return param;
        }
        #endregion

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
        #endregion

        #region Link2Exec()
        protected override void Link2Exec()
        {
            param = Params();

            SystemBase.Base.RodeFormID = "ACD001";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "결의전표등록"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        #region btnMinusCancel_Click()
        private void btnMinusCancel_Click(object sender, EventArgs e)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_SSA001  'D3'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pBN_NO = '" + txtBnNo.Text + "' ";
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
        #endregion        
        // 2022.01.28. hma 추가(End)

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
