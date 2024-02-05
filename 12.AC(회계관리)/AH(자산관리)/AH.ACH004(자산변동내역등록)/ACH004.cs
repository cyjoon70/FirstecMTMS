#region 작성정보
/*********************************************************************/
// 단위업무명 : 자산변동내역등록
// 작 성 자 : 유재규
// 작 성 일 : 2013-03-22
// 작성내용 : 자산변동내역등록
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

namespace AH.ACH004
{
    public partial class ACH004 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string Locking = "";
        int NewFlg = 0;//마스터 데이터 수정여부 0:등록,수정X, 1:등록, 2:수정
        string strAutoChangeNo = "";
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        string strREORG_ID = "";
        string strLinkSlipNo = "";     // 2022.01.28. hma 추가: 링크전표번호
        #endregion

        #region 생성자
        public ACH004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ACH004_Load(object sender, System.EventArgs e)
        {
            //그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            NewFlg = 1;
            //GroupBox2 입력조건 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pType='COMM', @pCODE = 'Z003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");//화폐단위
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pType='COMM', @pCODE = 'B040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);//VAT유형
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            SystemBase.ComboMake.C1Combo(cboCSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); // 2022.01.28. hma 추가: 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboMSlipGwStatus, "usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); // 2022.01.28. hma 추가: 그룹웨어상태

            // 2022.02.15. hma 추가(Start)
            lnkJump1.Text = "확정전표상신";         // 화면에 보여지는 링크명
            strJumpFileName1 = "AD.ACD001.ACD001";  // 호출할 화면명
            lnkJump2.Text = "반제전표상신";         // 화면에 보여지는 링크명
            strJumpFileName2 = "AD.ACD001.ACD001";  // 호출할 화면명
            strLinkSlipNo = "";
            // 2022.02.15. hma 추가(End)

            //폼 컨트롤 초기화
            NewExec();
        }
        #endregion

        #region ControlSetting()
        private void Control_Setting()
        {
            dtpChangeDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboCurCd.SelectedValue = "KRW";
            dtxtExchRate.Value = 1;
            dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
            cboBizAreaCd.SelectedValue = SystemBase.Base.gstrBIZCD;
            cboVatType.SelectedValue = "A";
            strREORG_ID = SystemBase.Base.gstrREORG_ID;
            //dtxtExchRate.ReadOnly = true;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            NewFlg = 1;

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            //폼 컨트롤 초기화
            Control_Setting();

            //기타 세팅
            dtpChangeDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4).ToString() + "-01-01";
            dtpChangeDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            // 2022.02.15. hma 추가(Start): 항목들 기본값 세팅
            rdoCfm_N.Checked = true;        

            btnChangeOk.Enabled = false;
            btnChangeCancel.Enabled = false;
            btnSlipView.Enabled = false;

            cboCSlipGwStatus.Text = "";      // 결재상태 초기화
            cboMSlipGwStatus.Text = "";      // 결재상태 초기화
            btnMinusCancel.Enabled = false;  // 반제취소 버튼 비활성화
            // 2022.02.15. hma 추가(End)
        }
        #endregion

        #region DetailNew() 우측 디테일 초기화
        private void DetailNew()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            NewFlg = 1;

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //폼 컨트롤 초기화
            Control_Setting();
        }
        #endregion

        #region 행삭제 버튼 클릭 이벤트
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                // 2022.01.28. hma 추가(Start): 확정된 건이거나, 미확정이면서 반제전표번호 존재하면서 결재중인 경우
                if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                    rdoCfm_Y.Checked == true)
                {
                    MessageBox.Show("확정된 건이므로 삭제할 수 없습니다.");
                    return;
                }

                if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                    rdoCfm_N.Checked == true && (cboMSlipGwStatus.SelectedValue.ToString() != "APPR" || txtMinusConfirm.Text != "Y"))
                {
                    MessageBox.Show("반제전표 결재상태가 승인이 아니므로 삭제할 수 없습니다.");
                    return;
                }
                // 2022.01.28. hma 추가(End)

                UIForm.FPMake.RowRemove(fpSpread1);
                DelExe();
                SumAmt();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행삭제"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 행복사 버튼 클릭 이벤트
        protected override void RCopyExec()
        {
            // 2022.01.28. hma 추가(Start): 확정된 건이거나, 미확정이면서 반제전표번호 존재하면서 결재중인 경우
            if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                rdoCfm_Y.Checked == true)
            {
                MessageBox.Show("확정된 건이므로 수정할 수 없습니다.");
                return;
            }

            if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                rdoCfm_N.Checked == true && (cboMSlipGwStatus.SelectedValue.ToString() != "APPR" || txtMinusConfirm.Text != "Y"))
            {
                MessageBox.Show("반제전표 결재상태가 승인이 아니므로 수정할 수 없습니다.");
                return;
            }
            // 2022.01.28. hma 추가(End)

            UIForm.FPMake.RowCopy(fpSpread1);
            SumAmt();
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            // 2022.01.28. hma 추가(Start): 확정된 건이거나, 미확정이면서 반제전표번호 존재하면서 결재중인 경우
            if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                rdoCfm_Y.Checked == true)
            {
                MessageBox.Show("확정된 건이므로 삭제할 수 없습니다.");
                return;
            }

            if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                rdoCfm_N.Checked == true && (cboMSlipGwStatus.SelectedValue.ToString() != "APPR" || txtMinusConfirm.Text != "Y"))
            {
                MessageBox.Show("반제전표 결재상태가 승인이 아니므로 삭제할 수 없습니다.");
                return;
            }
            // 2022.01.28. hma 추가(End)

            this.Cursor = Cursors.WaitCursor;

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
                    string strSql = " usp_ACH004  'D1'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pCHANGE_NO = '" + strAutoChangeNo + "' ";

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
                    DetailNew();
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

        // 2022.01.28. hma 추가(Start)
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                // 2022.01.28. hma 추가(Start): 확정된 건이거나, 미확정이면서 반제전표번호 존재하면서 결재중인 경우
                if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                    rdoCfm_Y.Checked == true)
                {
                    MessageBox.Show("확정된 건이므로 수정할 수 없습니다.");
                    return;
                }

                if (txtCSlipNo.Text != "" &&        // 2022.02.15. hma 추가
                    rdoCfm_N.Checked == true && (cboMSlipGwStatus.SelectedValue.ToString() != "APPR" || txtMinusConfirm.Text != "Y"))
                {
                    MessageBox.Show("반제전표 결재상태가 승인이 아니므로 수정할 수 없습니다.");
                    return;
                }
                // 2022.01.28. hma 추가(End)

                UIForm.FPMake.RowInsert(fpSpread1);
                RowInsExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 2022.01.28. hma 추가(End)

        #region SearchExec() Master 그리드 조회 로직
        protected override void SearchExec()
        {
            //마스터만 조회
            Search();

            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //폼 컨트롤 초기화
            Control_Setting();

            //프린트 버튼 활성화여부
            UIForm.Buttons.ReButton(BtnPrint, "BtnPrint", false);

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
                    string strQuery = " usp_ACH004  @pTYPE = 'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pCHANGE_DT_FROM = '" + dtpChangeDtFr.Text + "' ";
                    strQuery += ", @pCHANGE_DT_TO = '" + dtpChangeDtTo.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0, true);
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

                    strAutoChangeNo = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2,"변동번호")].Text.ToString();//수주번호

                    SubSearch(strAutoChangeNo);
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
        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                fpSpread1.Sheets[0].Rows.Count = 0;

                //Master정보
                string strSql = " usp_ACH004  'S2' ";
                strSql = strSql + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' "; 
                strSql = strSql + ", @pCHANGE_NO ='" + strCode + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    NewFlg = 2;

                    // 2022.01.28. hma 추가(Start)
                    bool ConfirmChk = false;

                    //확정여부
                    if (dt.Rows[0]["CONFIRM_YN"].ToString() != "")
                    {
                        if (dt.Rows[0]["CONFIRM_YN"].ToString() == "Y") { ConfirmChk = true; }
                        else { ConfirmChk = false; }
                    }
                    else { ConfirmChk = false; }
                    // 2022.01.28. hma 추가(End)

                    txtChangeNo.Value = dt.Rows[0]["CHANGE_NO"].ToString();
                    strREORG_ID = dt.Rows[0]["REORG_ID"].ToString();
                    txtDeptCd.Text = dt.Rows[0]["DEPT_CD"].ToString();
                    
                    txtSlipNo.Value = dt.Rows[0]["SLIP_NO"].ToString();
                    dtpChangeDt.Value = dt.Rows[0]["CHANGE_DT"].ToString();
                    txtCustCd.Value = dt.Rows[0]["CUST_CD"].ToString();
                    optChangeDiv1.Checked = true;
                    
                    txtAcctCd.Text = dt.Rows[0]["ACCT_CD"].ToString();
                    //cboCurCd.SelectedValue = dt.Rows[0]["CUR_CD"].ToString();
                    dtxtExchRate.Value = dt.Rows[0]["EXCH_RATE"].ToString();
                    cboCurCd.SelectedValue = dt.Rows[0]["CUR_CD"].ToString();
                    cboVatType.SelectedValue = dt.Rows[0]["VAT_TYPE"].ToString();
                    dtxtChangeAmt.Value = dt.Rows[0]["CHANGE_AMT"].ToString();
                    dtxtChangeAmtLoc.Value = dt.Rows[0]["CHANGE_AMT_LOC"].ToString();
                    dtxtVatRate.Value = dt.Rows[0]["VAT_RATE"].ToString();
                    dtxtVatAmt.Value = dt.Rows[0]["VAT_AMT"].ToString();
                    dtxtVatAmtLoc.Value = dt.Rows[0]["VAT_AMT_LOC"].ToString();
                    cboBizAreaCd.SelectedValue = dt.Rows[0]["BIZ_AREA_CD"].ToString();
                    txtRemark.Text = dt.Rows[0]["REMARK"].ToString();
                    
                    if (dt.Rows[0]["CUR_CD"].ToString() == "KRW")
                    {
                        //dtxtExchRate.Value = 1;
                        dtxtExchRate.Tag = "환율;2;;";
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                        dtxtExchRate.ReadOnly = true;
                    }
                    else
                    {
                        dtxtExchRate.Tag = "환율;1;;";
                        dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                        dtxtExchRate.ReadOnly = false;
                    }

                    // 2022.01.28. hma 추가(Start)
                    // 결재상태 및 반제전표번호, 반제승인 추가
                    txtCSlipNo.Value = dt.Rows[0]["CFM_SLIP_NO"].ToString();
                    cboCSlipGwStatus.SelectedValue = dt.Rows[0]["CFM_GW_STATUS"].ToString();
                    txtMinusConfirm.Value = dt.Rows[0]["MINUS_CONFIRM_YN"].ToString();
                    txtMSlipNo.Value = dt.Rows[0]["MINUS_SLIP_NO"].ToString();
                    cboMSlipGwStatus.SelectedValue = dt.Rows[0]["MINUS_GW_STATUS"].ToString();
                    txtSlipConfirmYn.Value = dt.Rows[0]["CFM_SLIP_CONFIRM_YN"].ToString();              // 확정전표 확정여부
                    txtMinusSlipConfirmYn.Value = dt.Rows[0]["MINUS_SLIP_CONFIRM_YN"].ToString();       // 반제전표 확정여부

                    //확정여부에 따른 화면 Locking
                    if (ConfirmChk == true)     // 확정상태인 경우
                    {
                        rdoCfm_Y.Checked = true;
                        SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                        btnChangeOk.Enabled = false;

                        // 확정상태이면서 결재상태가 상신대기/반려/승인 상태이면 확정취소 버튼 활성화되게.
                        if ((txtSlipNo.Text != "" && txtCSlipNo.Text == "") ||
                            ((txtCSlipNo.Text != "") &&
                                (((cboCSlipGwStatus.SelectedValue.ToString() == "READY" || cboCSlipGwStatus.SelectedValue.ToString() == "REJECT" ) && txtSlipConfirmYn.Text == "N") ||
                                 (cboCSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y"))) ) 
                            btnChangeCancel.Enabled = true;
                        else
                            btnChangeCancel.Enabled = false;

                        btnSlipView.Enabled = true;     // 2022.02.17. hma 추가
                    }
                    else
                    {
                        rdoCfm_N.Checked = true;

                        // 미확정 상태이지만 반제전표번호가 없거나, 반제전표번호가 있으면서 반제전표결재상태가 승인이고 반제승인이 Y이면 활성화
                        if ((txtMSlipNo.Text == "") ||
                            (txtMSlipNo.Text != "" &&
                             (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                        {
                            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
                        }
                        else
                        {
                            SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);        // 2022.02.19. hma 추가
                        }

                        // 미확정상태이지만 반제전표 결재상태가 상신대기/반려 상태일때만 확정취소 버튼 활성화되게.
                        // 또한 결재상태가 승인이면서 반제승인이 Y인 경우에도 확정취소 버튼 활성화.(반제처리 위해)
                        if ((txtMSlipNo.Text == "") ||
                            (txtMSlipNo.Text != "" &&
                             (cboMSlipGwStatus.SelectedValue.ToString() == "APPR" && txtMinusConfirm.Text == "Y")))
                            btnChangeOk.Enabled = true;
                        else
                            btnChangeOk.Enabled = false;

                        // 미확정건이지만 반제전표가 생성된 경우에는 확정취소 버튼 비활성화 처리.
                        // 2022.02.16. hma 수정(Start): 미확정상태인 경우엔 확정취소 버튼 비활성화되게.
                        //if (txtMSlipNo.Text != "" &&
                        //     (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        //    btnChangeCancel.Enabled = false;
                        btnChangeCancel.Enabled = false;
                        // 2022.02.16. hma 수정(End)

                        // 반제전표 결재상태에 따라 반제취소 버튼 활성화 처리. 반제전표 결재상태가 상신대기, 반려이면 활성화.
                        if (txtMSlipNo.Text != "" &&
                            (cboMSlipGwStatus.SelectedValue.ToString() == "READY" || cboMSlipGwStatus.SelectedValue.ToString() == "REJECT"))
                        {
                            btnMinusCancel.Enabled = true;
                        }
                        else
                        {
                            btnMinusCancel.Enabled = false;
                        }

                        btnSlipView.Enabled = false;     // 2022.02.17. hma 추가
                    }
                    // 2022.01.28. hma 추가(End)

                    //현재 row값 설정
                    PreRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                    SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                    //컨트롤 체크값 초기화
                    strSearchData = "";
                    //컨트롤 체크 함수
                    GroupBox[] gBox = new GroupBox[] { groupBox2};
                    SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                    // Detail그리드 정보.
                    string strSql1 = " usp_ACH004  'S3' ";
                    strSql1 += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' "; 
                    strSql1 += ", @pCHANGE_NO ='" + strCode + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql1, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    int iLock = 0;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호") + "|2"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호_2") + "|2");
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "LOCK_YN")].Text != "0")
                        {
                            iLock++;
                        }
                    }

                    if (iLock == 0) Locking = "N";
                    else Locking = "Y";

                    // 2022.01.28. hma 추가(Start): 확정여부에 따라 화면/그리드 Lock하기 위해 확정여부 체크하여 Locking 변수 저장.
                    if ((ConfirmChk == false) &&
                        (txtMSlipNo.Text != "" && cboMSlipGwStatus.SelectedValue.ToString() == "APPR"))     // 2022.02.17. hma 추가: 미확정인 경우 반제전표 결재상태가 승인인 경우에만
                        Locking = "N";
                    else
                        Locking = "Y";
                    // 2022.01.28. hma 추가(End)

                    Set_Lock_yn(Locking); //확정,결재여부에 따른 그리드 Lock

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

        #region Set_Lock_yn 확정,결재여부에 따른 Locking
        private void Set_Lock_yn(string strLock)
        {
            // 2022.01.28. hma 추가(Start): 미확정상태이지만 반제전표 승인이 완료가 아닌 경우에는 비활성화 처리
            string strSlipConfirmYn = "", strMinusSlipNo = "", strMinusGwStatus = "";
            strSlipConfirmYn = txtSlipConfirmYn.Text;
            strMinusSlipNo = txtMSlipNo.Text;
            if (cboMSlipGwStatus.Text == "")
                strMinusGwStatus = "";
            else
                strMinusGwStatus = cboMSlipGwStatus.SelectedValue.ToString();
            // 2022.01.28. hma 추가(End)

            //확정여부에 따른 화면 Locking            
            if ((strLock == "Y")
                || (strSlipConfirmYn != "Y" && strMinusSlipNo != "" && strMinusGwStatus != "APPR")) // 2022.01.28. hma 추가: 미확정 상태이지만 반제전표를 생성해서 승인상태가 아닌 경우도 비활성화
            {
                // 2022.01.28. hma 추가(Start): 반제취소 활성여부 저장
                bool bMinusCancel;
                bMinusCancel = btnMinusCancel.Enabled;
                // 2022.01.28. hma 추가(End)

                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

                btnMinusCancel.Enabled = bMinusCancel;      // 2022.01.28. hma 추가

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    // I2: 읽기전용, 필수항목
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액") + "|2"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|2"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세") + "|2"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국") + "|2"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|2"
                            );
                }
            }
            else
            {
                //SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);     // 2022.01.28. hma 수정: 주석 처리

                //Detail Locking설정
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    // I1: 필수입력
                    UIForm.FPMake.grdReMake(fpSpread1, i,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국") + "|1"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "비고") + "|0"
                            );
                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            // 2022.01.28. hma 추가(Start): 확정된 건이거나 결재중인 건인지 체크하여 리턴
            if (rdoCfm_Y.Checked == true)
            {
                MessageBox.Show("확정된 건이므로 수정할 수 없습니다.");
                return;
            }

            // 미확정건이면서 반제전표번호가 존재하는 경우
            if ((rdoCfm_N.Checked == true) && (txtMinusConfirm.Text != ""))
            {
                if (cboMSlipGwStatus.SelectedValue.ToString() != "APPR" || txtMinusConfirm.Text != "Y")
                {
                    MessageBox.Show("반제전표 결재상태가 승인이 아니거나 반제승인이 Y가 아니므로 수정할 수 없습니다.");
                    return;
                }
            }

            this.Cursor = Cursors.WaitCursor;

            string ChkChangeNo = strAutoChangeNo;
            GroupBox[] gBox = null;

            string strMstType = "";

            /////////////////////////////////////////////// MASTER 저장 시작 /////////////////////////////////////////////////
            //확정상태가 아니면
            //if (chkConfirm.Checked == false)
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
                        //주문처 유효성 체크
                        if (txtCustCd.Text != "" && txtCustNm.Text == "")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "거래처"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                            txtCustCd.Focus();
                            this.Cursor = Cursors.Default;

                            return;
                        }
                        //부서코드 유효성 체크
                        if (txtDeptCd.Text != "" && txtDeptNm.Text == "")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "부서코드"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                            txtDeptCd.Focus();
                            this.Cursor = Cursors.Default;

                            return;
                        }

                        //수금처 유효성 체크
                        if (txtAcctCd.Text != "" && txtAcctNm.Text == "")
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("B0036", "변동계정코드"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); //존재하지 않는 주문처 코드입니다.

                            txtAcctCd.Focus();
                            this.Cursor = Cursors.Default;

                            return;
                        }

                        string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.
                        string strAssetNo = "";

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            int iChangeRow = -1;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                                if (strHead != "")
                                {
                                    iChangeRow = i;
                                }
                            }

                            if (NewFlg != 0)
                            {
                                if (NewFlg == 1) { strMstType = "I1"; }
                                else { strMstType = "U1"; }

                                string strSql = " usp_ACH004 '" + strMstType + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql += ", @pCHANGE_NO = '" + txtChangeNo.Text + "' ";
                                strSql += ", @pCHANGE_DT = '" + dtpChangeDt.Text + "' ";
                                strSql += ", @pREORG_ID = '" + strREORG_ID + "' ";
                                strSql += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                                strSql += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                                strSql += ", @pCHANGE_DIV = '01' ";
                                
                                strSql += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                                strSql += ", @pCUR_CD = '" + cboCurCd.SelectedValue.ToString() + "' ";
                                strSql += ", @pEXCH_RATE = '" + dtxtExchRate.Text.Replace(",","") + "' ";
                                strSql += ", @pCHANGE_AMT = '" + dtxtChangeAmt.Text.Replace(",","") + "' ";
                                strSql += ", @pCHANGE_AMT_LOC = '" + dtxtChangeAmtLoc.Text.Replace(",","") + "' ";
                                strSql += ", @pVAT_TYPE = '" + cboVatType.SelectedValue.ToString() + "' ";
                                strSql += ", @pVAT_RATE = '" + dtxtVatRate.Text.Replace(",","") + "' ";
                                strSql += ", @pVAT_AMT = '" + dtxtVatAmt.Text.Replace(",","") + "' ";
                                strSql += ", @pVAT_AMT_LOC = '" + dtxtVatAmtLoc.Text.Replace(",","") + "' ";
                                strSql += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                                strSql += ", @pREMARK = '" + txtRemark.Text + "' ";
                                strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";
                                if (iChangeRow == -1) strSql += ", @pEND_FLAG = 'Y' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strSql, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();
                                strAutoChangeNo = dt.Rows[0][2].ToString();
                                ChkChangeNo = dt.Rows[0][2].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }

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
                                            string strDelSql = " usp_ACH004  'D1'";
                                            strDelSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                            strDelSql += ", @pCHANGE_NO = '" + txtChangeNo.Text + "' ";

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
                                            DetailNew();
                                            //컨트롤 체크값 초기화
                                            strSearchData = "";
                                            //컨트롤 체크 함수
                                            gBox = new GroupBox[] { groupBox2 };
                                            SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                                            //그리드 셀 포커스 이동
                                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoChangeNo, SystemBase.Base.GridHeadIndex(GHIdx2, "변동번호"));
                                            
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
                                            case "U": strGbn = "U2"; break;
                                            case "I": strGbn = "I2"; break;
                                            case "D": strGbn = "D2"; break;
                                            default: strGbn = ""; break;
                                        }

                                        strAssetNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Value.ToString();

                                        string strSql = " usp_ACH004 '" + strGbn + "'";
                                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                        strSql += ", @pCHANGE_NO = '" + strAutoChangeNo + "' ";
                                        strSql += ", @pCHANGE_DT = '" + dtpChangeDt.Text + "' ";
                                        strSql += ", @pASSET_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Value + "' ";
                                        strSql += ", @pCHANGE_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value + "' ";
                                        strSql += ", @pCHANGE_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value + "' ";
                                        strSql += ", @pVAT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value + "' ";
                                        strSql += ", @pVAT_AMT_LOC = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value + "' ";
                                        strSql += ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                                        if (iChangeRow == i) strSql += ", @pEND_FLAG = 'Y' ";
                                        strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                        strSql += ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

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
                            SubSearch(strAutoChangeNo);

                            UIForm.FPMake.GridSetFocus(fpSpread2, strAutoChangeNo, SystemBase.Base.GridHeadIndex(GHIdx2, "변동번호"));
                            UIForm.FPMake.GridSetFocus(fpSpread1, strAssetNo, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호"));
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

        //자산번호 팝업 만들기..
        #region 그리드 상 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호_2"))
            {
                try
                {
                    WNDW.WNDW027 pu = new WNDW.WNDW027();
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text = Msgs[1].ToString();
                        fpSpread1_ChangeEvent(Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호"));
                        
                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자산정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호"))
            {
                string Query = " usp_ACH004 @pTYPE = 'C1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pASSET_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    double dblExch_Rate = 0;
                    if (dtxtExchRate.Text != "") dblExch_Rate = Convert.ToDouble(dtxtExchRate.Text.Replace(",", ""));
                    double dblVat_Rate = 0;
                    if(dtxtVatRate.Text != "") dblVat_Rate = Convert.ToDouble(dtxtVatRate.Text.Replace(",",""));

                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자산명")].Value = dt.Rows[0]["ASSET_NM"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Value = dt.Rows[0]["DEPT_CD"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Value = dt.Rows[0]["DEPT_NM"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value = dt.Rows[0]["ASSET_AMT"].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * (dblVat_Rate * 0.01));
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * dblExch_Rate);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * dblExch_Rate * (dblVat_Rate * 0.01));
                    SumAmt();
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자산명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Text = "";
                }
            }

            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부가세"))
            {
                double dblExch_Rate = 0;
                dblExch_Rate = Convert.ToDouble(dtxtExchRate.Value);
                double dblVat_Rate = 0;
                dblVat_Rate = Convert.ToDouble(dtxtVatRate.Value);
                //지출금액
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * (dblVat_Rate * 0.01));
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * dblExch_Rate);
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value) * dblExch_Rate * (dblVat_Rate * 0.01));
                    SumAmt();
                }
                //부가세
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부가세"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                            = Math.Floor(Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value) * (dblVat_Rate * 0.01));
                    SumAmt();
                }
            }
            else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국"))
            {
                SumAmt();
            }
        }
        #endregion

        #region 합계금액구하기
        private void SumAmt()
        {
            double dAmt = 0, dAmtLoc = 0, dVatAmt = 0, dVatAmtLoc = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text != "D")
                {
                    if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Text != "")
                        dAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value);
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Text != "") 
                        dAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Text != "") 
                        dVatAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value);
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Text != "") 
                        dVatAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value);
                }
            }
            dtxtChangeAmt.Value = dAmt;
            dtxtChangeAmtLoc.Value = dAmtLoc;
            dtxtVatAmt.Value = dVatAmt;
            dtxtVatAmtLoc.Value = dVatAmtLoc;
        }
        #endregion

        #region 화폐단위 변경시 환율세팅
        private void cboCurrency_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            if (cboCurCd.SelectedValue.ToString() == "KRW")
            {
                dtxtExchRate.Value = 1;
                dtxtExchRate.Tag = "환율;2;;";
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_Gainsboro;
                dtxtExchRate.ReadOnly = true;
            }
            else
            {
                dtxtExchRate.Tag = "환율;1;;";
                dtxtExchRate.BackColor = SystemBase.Validation.Kind_LightCyan;
                dtxtExchRate.ReadOnly = false;
            }
        }
        #endregion

        #region 환율변경시 Detail 자동 업데이트 플래그 변경
        private void dtxtExchRate_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (NewFlg != 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        double dblExch_Rate = 0;
                        if (dtxtExchRate.Text != "") dblExch_Rate = Convert.ToDouble(dtxtExchRate.Text.Replace(",", ""));
                        double dblVat_Rate = 0;
                        if (dtxtVatRate.Text != "") dblVat_Rate = Convert.ToDouble(dtxtVatRate.Text.Replace(",", ""));

                        double dAmt = 0;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Text != "")
                            dAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value
                                = Math.Floor(dAmt * dblExch_Rate);
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                                = Math.Floor(dAmt * dblExch_Rate * (dblVat_Rate * 0.01));

                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "")
                        { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
                    }
                    SumAmt();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//최소 한건 이상의 DETAIL정보가 존재하지 않으면 등록할 수 없습니다.
            }
        }
        #endregion

        #region TextChanged
        //거래처 조회조건
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch { }
        }
        //부가세유형
        private void cboVatType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dtxtVatRate.Value = SystemBase.Base.CodeName("MINOR_CD", "REL_CD1", "B_COMM_CODE", cboVatType.SelectedValue.ToString(), " AND MAJOR_CD = 'B040' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

                if (NewFlg != 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        double dblExch_Rate = 0;
                        if (dtxtExchRate.Text != "") dblExch_Rate = Convert.ToDouble(dtxtExchRate.Text.Replace(",", ""));
                        double dblVat_Rate = 0;
                        if (dtxtVatRate.Text != "") dblVat_Rate = Convert.ToDouble(dtxtVatRate.Text.Replace(",", ""));

                        double dAmt = 0;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Text != "")
                            dAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value
                                = Math.Floor(dAmt * dblVat_Rate * 0.01);
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                                = Math.Floor(dAmt * dblExch_Rate * (dblVat_Rate * 0.01));

                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "")
                        { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
                    }
                    SumAmt();
                }

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //계정코드
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y' ");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //부가세율
        private void dtxtVatRate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (NewFlg != 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        double dblExch_Rate = 0;
                        if (dtxtExchRate.Text != "") dblExch_Rate = Convert.ToDouble(dtxtExchRate.Text.Replace(",", ""));
                        double dblVat_Rate = 0;
                        if (dtxtVatRate.Text != "") dblVat_Rate = Convert.ToDouble(dtxtVatRate.Text.Replace(",", ""));

                        double dAmt = 0;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Text != "")
                            dAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "지출금액")].Value);

                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세")].Value
                                = Math.Floor(dAmt * dblVat_Rate * 0.01);
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세자국")].Value
                                = Math.Floor(dAmt * dblExch_Rate * (dblVat_Rate * 0.01));

                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                        if (strHead == "")
                        { fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U"; }
                    }
                    SumAmt();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //부서
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업창 이벤트
        //거래처
        private void btnCust_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //계정
        private void btnAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCd.Value = Msgs[0].ToString();
                    txtAcctNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //부서
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011(dtpChangeDt.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    strREORG_ID = Msgs[5].ToString();
                    txtDeptCd.Text = Msgs[1].ToString();
                    
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        // 2022.01.28. hma 추가(Start)
        #region btnChangeOk_Click(): 확정 버튼 클릭시. 확정처리.
        private void btnChangeOk_Click(object sender, EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY068", "자산변동번호 " + txtChangeNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("Y");
            }
        }
        #endregion

        #region btnChangeCancel_Click(): 확정취소 버튼 클릭시. 확정취소 처리.
        private void btnChangeCancel_Click(object sender, EventArgs e)
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY069", "자산변동번호 " + txtChangeNo.Text + " "), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                Confirm("N");
            }
        }
        #endregion

        #region Confirm(): 확정/확정취소 처리
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
                    string strSql = " usp_ACH004  'P1'";
                    strSql += ", @pCHANGE_NO = '" + txtChangeNo.Text + "' ";
                    strSql += ", @pCONFIRM_YN = '" + strConfirmYn + "' ";
                    strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
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
                    SubSearch(txtChangeNo.Text);
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

        #region lnkJump1_LinkClicked(): 확정전표상신 링크 클릭시.
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
        #endregion

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

        #region lnkJump2_LinkClicked(): 반제전표상신 링크 클릭시.
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
            // 2022.02.16. hma 추가(Start): 반제취소 버튼 클릭시 반제취소 할건지 확인하고 처리하도록 함.
            DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg != DialogResult.Yes)
            {
                return;
            }
            // 2022.02.16. hma 추가(End)

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_ACH004  'D3'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pCHANGE_NO = '" + txtChangeNo.Text + "' ";
                strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

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
                SubSearch(txtChangeNo.Text);
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
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSlipNo.Text != "")
                {
                    string strSLIP_NO = txtSlipNo.Text;

                    WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIP_NO);
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
