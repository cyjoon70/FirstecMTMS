#region 작성정보
/*********************************************************************/
// 단위업무명 : 세금계산서등록
// 작 성 자 : 조  홍  태
// 작 성 일 : 2013-02-28
// 작성내용 : 세금계산서등록 및 조회
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


namespace SS.SSB001
{
    public partial class SSB001 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public SSB001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SSB001_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //그리드 콤보박스 세팅
            //MASTER
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "영업담당")] = SystemBase.ComboMake.ComboOnGrid("usp_S_COMMON @pType='S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//영업담당
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "부가세형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM',@pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//매출형태

            //DETAIL
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "단위")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Z005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//단위
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT유형")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT유형
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "VAT포함")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'S019', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//VAT포함
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //GropBox1 조회조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //영업담당

            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);//VAT유형
            SystemBase.ComboMake.C1Combo(cboCurrency, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0, true);//화폐단위

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            //기타 세팅
            dtpSIssueDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpSIssueDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpIssueDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoAll.Checked = true;
            panel2.Enabled = false;
            panel3.Enabled = false;
            btnBnRef.Enabled = true;
            btnIssueOk.Enabled = false;
            btnIssueCancel.Enabled = false;
            dtxtVatRate.Value = 10;
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
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            this.Cursor = Cursors.WaitCursor;

            //발행상태면
            if (chkIssueYn.Checked == true)
            {
                //||확정된 데이터는 다른 작업을 할 수 없습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("SY070", "발행"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//확정된 데이터는 다른 작업을 할 수 없습니다.
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
                    string strSql = " usp_SSB001  'D1', @pTAX_NO = '" + txtTaxNo.Text + "'";
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

                    string strCfmYn = "";
                    if (rdoNo.Checked == true) { strCfmYn = "N"; }
                    else if (rdoYes.Checked == true) { strCfmYn = "Y"; }
                    else { strCfmYn = ""; }

                    string strQuery = " usp_SSB001  @pTYPE = 'S1'";
                    strQuery += ", @pISSUE_DT_FR = '" + dtpSIssueDtFr.Text + "' ";
                    strQuery += ", @pISSUE_DT_TO = '" + dtpSIssueDtTo.Text + "' ";
                    strQuery += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "' ";
                    strQuery += ", @pSALE_DUTY = '" + cboSSaleDuty.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBILL_CUST = '" + txtSBillCustCd.Text + "' ";
                    strQuery += ", @pBILL_CUST_NM = '" + txtSBillCustNm.Text + "' ";
                    strQuery += ", @pISSUE_YN = '" + strCfmYn + "' ";
                    strQuery += ", @pTAX_NO = '" + txtSTaxNo.Text + "' ";
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

                    string strAutoTaxNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "계산서번호")].Text.ToString();//세금계산서번호

                    SubSearch(strAutoTaxNo);
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

                //세금계산서Master정보
                string strSql = " usp_SSB001  'S2', @pTAX_NO = '" + Code + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    bool ConfirmChk = false;

                    //발행여부
                    if (dt.Rows[0]["ISSUE_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["ISSUE_YN"].ToString() == "Y") { ConfirmChk = true; }
                        else { ConfirmChk = false; }
                    }
                    else { ConfirmChk = false; }

                    txtTaxNo.Value = dt.Rows[0]["TAX_NO"].ToString();
                    if (dt.Rows[0]["TAX_TYPE"].ToString() == "D") { rdoTaxType1.Checked = true; }
                    else { rdoTaxType2.Checked = true; }
                    dtpIssueDt.Value = dt.Rows[0]["ISSUE_DT"].ToString();       // 2015.08.21. hma 수정: ISSUE_DT와 BILL_CUST 위치 바꿔줌(발행일자에 해당하는 발행처를 가져오기 위해)
                    txtBillCustCd.Value = dt.Rows[0]["BILL_CUST"].ToString();
                    chkIssueYn.Checked = ConfirmChk;
                    if (dt.Rows[0]["VAT_UNI_FLAG"].ToString() == "1") { rdoDutch.Checked = true; }
                    else { rdoUnite.Checked = true; }
                    if (dt.Rows[0]["VAT_INC_FLAG"].ToString() == "1") { rdoExtra.Checked = true; }
                    else { rdoGroup.Checked = true; }
                    cboVatType.SelectedValue = dt.Rows[0]["VAT_TYPE"].ToString();
                    txtTaxBizCd.Value = dt.Rows[0]["TAX_BIZ_CD"].ToString();
                    dtxtVatRate.Value = dt.Rows[0]["VAT_RATE"];
                    cboCurrency.SelectedValue = dt.Rows[0]["CURRENCY"];
                    dtxtNetAmt.Value = dt.Rows[0]["NET_AMT"];
                    dtxtNetAmtLoc.Value = dt.Rows[0]["NET_AMT_LOC"];
                    dtxtVatAmt.Value = dt.Rows[0]["VAT_AMT"];
                    dtxtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"];
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                   
                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    //세금계산서Detail그리드 정보.
                    string strSql1 = " usp_SSB001  'S3' , @pTAX_NO = '" + Code + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //발행여부에 따른 화면 Locking
                    if (ConfirmChk == true)
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
                        btnBnRef.Enabled = false;
                        panel3.Enabled = false;

                        if (dt.Rows[0]["TAX_REPORT_YN"].ToString() == "Y")
                        {
                            btnIssueOk.Enabled = false;
                            btnIssueCancel.Enabled = false;
                        }
                        else
                        {
                            btnIssueOk.Enabled = false;
                            btnIssueCancel.Enabled = true;
                        }

                        //Detail Locking설정
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|3");
                        }
                    }
                    else
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                        btnBnRef.Enabled = true;
                        panel3.Enabled = true;

                        btnIssueOk.Enabled = true;
                        btnIssueCancel.Enabled = false;

                        //Detail Locking해제
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0");
                        }
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

            string strAutoTaxNo = "";
            string strMstType = "";
            string strInUpFlag = "I";

            GroupBox[] gBox = null;

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //발행상태가 아니면
            if (chkIssueYn.Checked == false)
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
                        string strTaxSeq = "";

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            if (txtTaxNo.Text == "") { strMstType = "I1"; }
                            else { strMstType = "U1"; strInUpFlag = "U"; }

                            string strVatUniFlag = "2", strVatIncFlag = "2"; //VAT통합구분(default :통합), VAT포함구분(default :포함)
                            if (rdoDutch.Checked == true) { strVatUniFlag = "1"; } //개별
                            if (rdoExtra.Checked == true) { strVatIncFlag = "1"; } //별도

                            string strTaxType = "D"; //청구
                            if (rdoTaxType2.Checked == true) { strTaxType = "R"; } //영수

                            double dblBnAmt = 0, dblBnAmtLoc = 0, dblNetAmt = 0, dblNetAmtLoc = 0;
                            double dblVatAmt = 0, dblVatAmtLoc = 0, dblTotAmt = 0, dblTotAmtLoc = 0;

                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                dblBnAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value);
                                dblBnAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value);
                                dblVatAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value);
                                dblVatAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value);
                                dblNetAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value);
                                dblNetAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value);
                                dblTotAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT금액")].Value);
                                dblTotAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT자국금액")].Value);
                            }

                            string strSql = " usp_SSB001 '" + strMstType + "'";

                            strSql += ", @pTAX_NO = '" + txtTaxNo.Text + "' ";
                            strSql += ", @pTAX_TYPE = '" + strTaxType + "' ";
                            strSql += ", @pBILL_CUST = '" + txtBillCustCd.Text + "' ";
                            strSql += ", @pISSUE_DT = '" + dtpIssueDt.Text + "' ";
                            strSql += ", @pVAT_TYPE = '" + cboVatType.SelectedValue.ToString() + "' ";
                            strSql += ", @pTAX_BIZ_CD = '" + txtTaxBizCd.Text + "' ";
                            strSql += ", @pVAT_RATE = '" + dtxtVatRate.Value + "' ";
                            strSql += ", @pBN_AMT = '" + dblBnAmt + "' ";
                            strSql += ", @pBN_AMT_LOC = '" + dblBnAmtLoc + "' ";
                            strSql += ", @pTOT_AMT = '" + dblTotAmt + "' ";
                            strSql += ", @pTOT_AMT_LOC = '" + dblTotAmtLoc + "' ";
                            strSql += ", @pNET_AMT = '" + dblNetAmt + "' ";
                            strSql += ", @pNET_AMT_LOC = '" + dblNetAmtLoc + "' ";
                            strSql += ", @pVAT_AMT = '" + dblVatAmt + "' ";
                            strSql += ", @pVAT_AMT_LOC = '" + dblVatAmtLoc + "' ";
                            strSql += ", @pCURRENCY = '" + cboCurrency.SelectedValue.ToString() + "' ";
                            strSql += ", @pVAT_UNI_FLAG = '" + strVatUniFlag + "' ";
                            strSql += ", @pVAT_INC_FLAG = '" + strVatIncFlag + "' ";
                            strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                            ERRCode = dt.Rows[0][0].ToString();
                            MSGCode = dt.Rows[0][1].ToString();
                            strAutoTaxNo = dt.Rows[0][2].ToString();

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

                                            string strDelSql = " usp_SSB001  'D1'";
                                            strDelSql += ", @pTAX_NO = '" + txtTaxNo.Text + "' ";
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
                                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoTaxNo, SystemBase.Base.GridHeadIndex(GHIdx2, "계산서번호"));
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
                                            strTaxSeq = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서순번")].Value.ToString();
                                        }
                                        else
                                        {
                                            strTaxSeq = "0";
                                        }

                                        string strSubSql = " usp_SSB001 '" + strGbn + "'";
                                        strSubSql += ", @pTAX_NO = '" + strAutoTaxNo + "' ";
                                        strSubSql += ", @pTAX_SEQ = '" + strTaxSeq + "' ";
                                        strSubSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
                                        strSubSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
                                        strSubSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Value + "' ";
                                        strSubSql += ", @pBN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "채권번호")].Text + "' ";
                                        strSubSql += ", @pBN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Value + "' ";
                                        strSubSql += ", @pBN_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value + "' ";
                                        strSubSql += ", @pBN_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value + "' ";
                                        strSubSql += ", @pBN_PRICE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value + "' ";
                                        strSubSql += ", @pBN_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value + "' ";
                                        strSubSql += ", @pBN_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value + "' ";
                                        strSubSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value + "' ";
                                        strSubSql += ", @pNET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value + "' ";
                                        strSubSql += ", @pNET_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value + "' ";
                                        strSubSql += ", @pTOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT금액")].Value + "' ";
                                        strSubSql += ", @pTOT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT자국금액")].Value + "' ";
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
                            SubSearch(strAutoTaxNo);

                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoTaxNo, SystemBase.Base.GridHeadIndex(GHIdx2, "계산서번호"));
                            UIForm.FPMake.GridSetFocus(fpSpread1, strTaxSeq, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서순번"));
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
                    txtBillCustNm.Value = SystemBase.Base.GetCustName(dtpIssueDt.Text, txtBillCustCd.Text);
                    // 2015.08.21. hma 수정(End)
                }
                else
                {
                    txtBillCustNm.Value = "";
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
                    txtSBillCustCd.Focus();
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
                    txtBillCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발행처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //신고사업장
        private void btnTaxBiz_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BIZ_CD', @pSPEC2 = 'BIZ_NM', @pSPEC3 = 'B_BIZ_PLACE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "세금 신고 사업장 조회");
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "세금신고사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 매출채권참조
        private void btnBnRef_Click(object sender, System.EventArgs e)
        {
            try
            {
                DataTable PopDt = new DataTable();
                SSB001P1 myForm = new SSB001P1();
                myForm.ShowDialog();

                if (myForm.DialogResult == DialogResult.OK)
                {
                    txtBillCustCd.Text = myForm.txtBillCustCd.Text;
                    cboVatType.SelectedValue = myForm.cboVatType.SelectedValue.ToString();
                    txtTaxBizCd.Text = myForm.txtTaxBizCd.Text;
                    cboCurrency.SelectedValue = myForm.cboCurrency.SelectedValue.ToString();
                    rdoDutch.Checked = myForm.rdoDutch.Checked;
                    rdoUnite.Checked = myForm.rdoUnite.Checked;
                    rdoExtra.Checked = myForm.rdoExtra.Checked;
                    rdoGroup.Checked = myForm.rdoGroup.Checked;

                    DataTable MyFormDt = new DataTable();
                    MyFormDt = myForm.ReturnDt;

                    if (MyFormDt != null)
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0; //grid초기화

                        int row = 0;

                        for (int i = 0; i < MyFormDt.Rows.Count; i++)
                        {
                            if (MyFormDt.Rows[i]["CHK"].ToString() == "1")
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);//행추가

                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = MyFormDt.Rows[i]["ITEM_CD"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = MyFormDt.Rows[i]["ITEM_NM"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = MyFormDt.Rows[i]["ITEM_SPEC"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value = MyFormDt.Rows[i]["BN_QTY"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Value = MyFormDt.Rows[i]["BN_UNIT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value = MyFormDt.Rows[i]["BN_PRICE"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value = MyFormDt.Rows[i]["NET_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT금액")].Value = MyFormDt.Rows[i]["VAT_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "공급자국금액")].Value = MyFormDt.Rows[i]["NET_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "VAT자국금액")].Value = MyFormDt.Rows[i]["VAT_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "채권번호")].Text = MyFormDt.Rows[i]["BN_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Value = MyFormDt.Rows[i]["BN_SEQ"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text = MyFormDt.Rows[i]["REMARK"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출금액")].Value = MyFormDt.Rows[i]["BN_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "매출자국금액")].Value = MyFormDt.Rows[i]["BN_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT금액")].Value = MyFormDt.Rows[i]["TOT_AMT"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "TOT자국금액")].Value = MyFormDt.Rows[i]["TOT_AMT_LOC"];
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = MyFormDt.Rows[i]["PROJECT_NO"].ToString();
                                fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text = MyFormDt.Rows[i]["PROJECT_SEQ"].ToString();

                                row++;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "매출채권 정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 발행, 발행취소
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
                    string strSql = " usp_SSB001  'P1', @pTAX_NO = '" + txtTaxNo.Text + "', @pISSUE_YN = '" + strConfirmYn + "' ";
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
                    SubSearch(txtTaxNo.Text);
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
        //발행
        private void btnIssueOk_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY071", "세금계산서번호 " + txtTaxNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("Y");
            }
        }
        //발행취소
        private void btnIssueCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY072", "세금계산서번호 " + txtTaxNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("N");
            }
        }
        #endregion

    }
}
