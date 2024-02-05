

#region 작성정보
/*********************************************************************/
// 단위업무명: 결의전표결재 > 결재라인
// 작 성 자  : 한 미 애
// 작 성 일  : 2021-12-16
// 작성내용  : 결재라인조회 및 저장
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace AD.ACD007
{
    public partial class ACD007P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strSlipNo = "";
        string strGwStatus = "";
        string strTaskType = "";
        string strAssignNo = "";
        string strDeptCd = "";
        string strDeptNm = "";        
        string strUserId = "";
        string strUserNm = "";
        string strAdminYn = "";
        string strFinanceYn = "";
        string[] returnVal = null;
        #endregion

        public ACD007P1()
        {
            InitializeComponent();
        }

        public ACD007P1(string SLIP_NO, string GW_STATUS, string TASK_TYPE, string ASSIGN_NO, string DEPT_CD, string DEPT_NM, string USER_ID, string USER_NM, string ADMIN_YN, string FINANCE_YN)
        {
            strSlipNo = SLIP_NO;
            strGwStatus = GW_STATUS;
            strTaskType = TASK_TYPE;
            strAssignNo = ASSIGN_NO;
            strDeptCd = DEPT_CD;
            strDeptNm = DEPT_NM;
            strUserId = USER_ID;
            strUserNm = USER_NM;
            strAdminYn = ADMIN_YN;
            strFinanceYn = FINANCE_YN;

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD007P1_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("011111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "결의전표결재 > 결재라인조회";

                SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");       // 업무구분
                SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);  // 결재상태

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

                // 관리자권한 사용자 또는 재무팀 사용자인 경우에는 접수 결재구분도 콤보박스에 나오게 하고 일반 사용자들은 발의만 나오도록.
                if (strAdminYn == "Y" || strFinanceYn == "Y")
                    G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B096', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                else
                    G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'REL1', @pSPEC1 = 'Y', @pCODE = 'B096', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);  // 등록조회여부가 Y인 건

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B093', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                //UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                txtAdminRollYn.Value = strAdminYn;
                txtFinanceDeptYn.Value = strFinanceYn;

                txtSlipNo.Value = strSlipNo;
                cboGwStatus.SelectedValue = strGwStatus;
                cboTaskType.SelectedValue = strTaskType;
                txtAssignNo.Value = strAssignNo;
                txtDeptCd.Value = strDeptCd;
                txtDeptNm.Value = strDeptNm;
                txtUserId.Value = strUserId;
                txtUserNm.Value = strUserNm;

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_ACD007P1 @pTYPE = 'S2' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";
                strQuery += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";                
                strQuery += ", @pASSIGN_NO = '" + txtAssignNo.Text + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                // 결재 승인/반려된 건인 경우 수정 못하게 비활성화 처리
                if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                {
                    UIForm.FPMake.grdReMake(fpSpread1,
                       SystemBase.Base.GridHeadIndex(GHIdx1, "결재자") + "|3"
                           + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자_2") + "|3"
                           + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서") + "|3"
                           + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계") + "|3"
                           + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분") + "|3"
                        );

                    btnChange.Enabled = false;
                }
                else
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        // 결재상태와 완료이거나 반려이면 수정 못하게.
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Text != "" && 
                                (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "COMPLETE" ||
                                 fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "REJECT"))
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                SystemBase.Base.GridHeadIndex(GHIdx1, "결재자") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자_2") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계") + "|3"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분") + "|3"
                                );
                    }

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;            
        }
        #endregion

        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                {
                    MessageBox.Show("승인/반려된 건이므로 수정할 수 없습니다.");
                    return;
                }

                UIForm.FPMake.RowInsert(fpSpread1);

                int SelectedRow = fpSpread1.Sheets[0].ActiveRowIndex;

                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = "B";          // 행추가시 기본적으로 검토로 들어가도록 함.
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = "MAKE";       // 행추가시 기본적으로 발의로 들어가도록 함.
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                {
                    MessageBox.Show("승인/반려된 건이므로 수정할 수 없습니다.");
                    return;
                }

                UIForm.FPMake.RowCopy(fpSpread1);
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                    fpSpread1.Sheets[0].RowHeader.Cells[SelectedRow, 0].Text = "";
                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = "";
                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = "";
                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text = "";
                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Text = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                {
                    MessageBox.Show("승인/반려된 건이므로 수정할 수 없습니다.");
                    return;
                }

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int iDelRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                    // 해당 결재순서의 결재상태 항목에 값이 있으면서 완료/반려 상태가 아니면
                    if ( (fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Text != "") &&
                         (fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "COMPLETE"  ||
                            fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "REJECT") )
                    {
                        MessageBox.Show("결재완료된 건이므로 삭제할 수 없습니다.");
                        return;
                    }

                    UIForm.FPMake.RowRemove(fpSpread1);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 메인 화면으로 리턴
        protected override void SaveExec()
        {
            if (txtAssignNo.Text == "")
            {
                MessageBox.Show("상신 처리후 결재선 데이터를 저장하실 수 있습니다.");
                return;
            }

            if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
            {
                MessageBox.Show("승인/반려된 건이므로 수정할 수 없습니다.");
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    string strResultMsg = "";
                    string strSql_Chk = "";

                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 저장 대상 전표/결재요청 데이터 체크
                        strSql_Chk = " usp_ACD007P1 'C1' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + txtAssignNo.Text + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds21 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds21.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds21.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                            string strGbn = "";

                            if (strHead.Length > 0)
                            {
                                switch (strHead)
                                {
                                    case "U": strGbn = "U1"; break;
                                    case "D": strGbn = "D1"; break;
                                    case "I": strGbn = "I1"; break;
                                    default: strGbn = ""; break;
                                }

                                string strSql = " usp_ACD007P1 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                                strSql = strSql + ", @pTASK_NO = '" + txtAssignNo.Text + "'";
                                strSql = strSql + ", @pTASK_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청SEQ")].Text + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                                strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value.ToString() + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode == "OK")
                                    strResultMsg = MSGCode;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        // 저장된 결재선 데이터 체크
                        strSql_Chk = " usp_ACD007P1 'C2' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + txtAssignNo.Text + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds22 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds22.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds22.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "OK")
                            MSGCode = strResultMsg;

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();

                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
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

            this.Cursor = Cursors.Default;
        }
        #endregion

        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "결재자"))
                {

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 버튼클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "결재자_2"))
                {
                    string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명2")].Text = Msgs[3].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        // 화면 그리드의 결재라인 데이터로 결재선 저장
        private void btnAssign_Click(object sender, EventArgs e)
        {
            if ((txtAssignNo.Text != "") &&
                (cboGwStatus.SelectedValue.ToString() != "READY" && cboGwStatus.SelectedValue.ToString() != "REJECT"))
            {
                MessageBox.Show("이미 상신 처리되었습니다.");
                return;
            }

            string strNewAssignNo = "";
            string strNewStatus = "";

            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    string strSql = "";
                    string strSql_Chk = "";

                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 결재요청마스터 생성
                        strSql = " usp_ACD007P1 @pTYPE = 'I2'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds11 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds11.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds11.Tables[0].Rows[0][1].ToString();
                        strNewAssignNo = ds11.Tables[0].Rows[0][2].ToString();      // 생성된 결재요청번호
                        strNewStatus = ds11.Tables[0].Rows[0][3].ToString();        // 전표 그룹웨어상태

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 저장 대상 전표/결재요청 데이터 체크
                        strSql_Chk = " usp_ACD007P1 @pTYPE = 'C1' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + strNewAssignNo + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds21 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds21.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds21.Tables[0].Rows[0][1].ToString();                        

                        if (ERRCode != "OK")
                        {
                            goto Exit;
                        }                        

                        // 행수만큼 결재요청상세 생성
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                strSql = " usp_ACD007P1 @pTYPE = 'I1'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                                strSql = strSql + ", @pTASK_NO = '" + strNewAssignNo + "'";
                                strSql = strSql + ", @pTASK_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청SEQ")].Text + "'";
                                strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value.ToString() + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                DataSet ds12 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds12.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds12.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        // 저장된 결재선 데이터 체크
                        strSql_Chk = " usp_ACD007P1 'C2' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + strNewAssignNo + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds22 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds22.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds22.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "OK")
                        {
                            MSGCode = "상신되었습니다.";
                        }

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        SearchExec();
                        RtnStr(strNewAssignNo, strNewStatus);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
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

            this.Cursor = Cursors.Default;
        }

        #region 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string AssignNo, string GwStatus)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[2];
                returnVal[0] = GwStatus;
                returnVal[1] = AssignNo;
            }
        }
        #endregion

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserId.Text))
            {
                MessageBox.Show("사용자ID가 입력되지 않았습니다. 사용자ID를 입력해주세요.");
                return;
            }
            if (string.IsNullOrEmpty(cboTaskType.Text))
            {
                MessageBox.Show("업무구분이 선택되지 않았습니다. 업무구분을 선택해주세요.");
                return;
            }

            string strDefaultAssignType = "MAKE";       // 결재구분 항목에 발의가 들어가도록 하기 위해 지정.

            WNDW.WNDW050 Dialog = new WNDW.WNDW050();
            Dialog.ShowDialog();

            if (Dialog.DialogResult == DialogResult.OK)
            {
                DataTable FormDt = new DataTable();
                FormDt = Dialog.ReturnDt;

                if (FormDt != null)
                {
                    for (int i = 0; i < FormDt.Rows.Count; i++)
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);//행추가
                        int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무코드")].Value = cboTaskType.SelectedValue.ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Text = cboTaskType.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = FormDt.Rows[i]["결재자"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = FormDt.Rows[i]["결재자명"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = FormDt.Rows[i]["결재단계"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = strDefaultAssignType;
                    }
                }
            }
        }
    }
}
