

#region 작성정보
/*********************************************************************/
// 단위업무명: 결재라인
// 작 성 자  : 한 미 애
// 작 성 일  : 2021-12-02
// 작성내용  : 결재라인조회
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

namespace AD.ACD001
{
    public partial class ACD001P7 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strSlipNo = "";
        string strUserId = "";
        string strUserNm = "";
        string strTaskType = "";
        string strDefaultTaskType = "TSLIP";
        string strDefaultAssignOwner = "MAKE";      // 2022.02.12. hma 추가: 기본 결재구분을 MAKE(발의)로 처리되게 하기 위해 선언.
        string[] returnVal = null;
        string strChgFlag = "";
        #endregion

        public ACD001P7()
        {
            InitializeComponent();
        }

        public ACD001P7(string SLIP_NO, string TASK_TYPE, string USER_ID, string USER_NM)
        {
            strSlipNo = SLIP_NO;
            strTaskType = TASK_TYPE;
            strUserId = USER_ID;
            strUserNm = USER_NM;

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD001P7_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("011111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "결재라인조회";

                txtSlipNo.Value = strSlipNo;
                txtUserId.Value = strUserId;
                txtUserNm.Value = strUserNm;

                SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");    // 업무구분
                SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);  // 결재상태
                SystemBase.ComboMake.C1Combo(cboRoutNo, "usp_ACD001P7 @pTYPE = 'S3', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pTASK_TYPE = '" + strDefaultTaskType + "', @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'", 0);   // 결재선번호

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B096', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B093', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                //UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                
                cboTaskType.SelectedValue = strTaskType;

                strChgFlag = "N";

                SearchHdr();

                // 상신대기 상태인 경우에만 결재선번호 선택할 수 있게.
                if (cboGwStatus.SelectedValue.ToString() == "READY")        // 상신대기
                {
                    cboRoutNo.Enabled = true;
                    cboRoutNo.SelectedIndex = 0;
                    btnRoutChange.Enabled = true;      // 결재선변경 버튼 비활성화
                    btnAssign.Enabled = true;
                }
                else
                {
                    cboRoutNo.Enabled = false;
                    btnRoutChange.Enabled = false;      // 결재선변경 버튼 비활성화
                    btnAssign.Enabled = false;
                }

                // 승인/반려된 건인 경우 결재자등록(멀티) 및 결재상신 버튼 비활성화 처리
                if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                {
                    btnChange.Enabled = true;
                }

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ACD001P7_FormClosing(): 폼 종료시 저장/상신 처리 여부 체크
        private void ACD001P7_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strChgFlag == "Y")      // 저장이나 상신 처리를 한 경우 폼 닫은 후 결재선 다시 조회하도록 
            {
                RtnStr("", "OK");
            }
            else
            {
                RtnStr("", "Cancel");
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
                string strQuery = " usp_ACD001P7 @pTYPE = 'S2' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";
                strQuery += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";                
                strQuery += ", @pUSR_ID = '" + txtUserId.Text.Trim() + "'";
                strQuery += ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "' ";     // 선택된 결재선번호.
                strQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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

                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    // 결재라인정보에서 데이터를 가져온 경우 행추가 표시
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "데이터구분")].Text == "ROUT")
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "I";
                    }
                    else 
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

                //fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = txtUserId.Text;
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Value = cboTaskType.SelectedValue;
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = strDefaultAssignOwner;
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
                    if ((fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Text != "") &&
                         (fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "COMPLETE" ||
                            fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")].Value.ToString() == "REJECT"))
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
                        strSql_Chk = " usp_ACD001P7 'C1' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + txtAssignNo.Text + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds21 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds21.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds21.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK")
                        {
                            MSGCode = strResultMsg;
                            goto Exit;
                        }

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

                                string strSql = " usp_ACD001P7 '" + strGbn + "'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pDEPT_CD = '" + txtUserId.Text.Trim() + "'";
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
                        strSql_Chk = " usp_ACD001P7 'C2' ";
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
                    catch (Exception)
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

                        strChgFlag = "Y";       // 변경여부를 Y로
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
            // 상신 처리된 건인지 체크
            if ((txtAssignNo.Text != "") &&
                (cboGwStatus.SelectedValue.ToString() != "READY" && cboGwStatus.SelectedValue.ToString() != "REJECT"))
            {
                MessageBox.Show("이미 상신 처리되었습니다.");
                return;
            }

            // 그리드 행수 체크. 결재라인 갯수가 0이면 리턴
            if (fpSpread1.Sheets[0].Rows.Count == 0)
            {
                MessageBox.Show("결재를 진행할 입력된 결재라인 데이터가 없습니다.");
                return;
            }

            //string strNewAssignNo = "";
            //string strNewStatus = "";

            //this.Cursor = Cursors.WaitCursor;

            //if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            //{
            //    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            //    {
            //        string strSql = "";
            //        string strSql_Chk = "";

            //        string ERRCode = "ER", MSGCode = "P0000";
            //        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            //        SqlCommand cmd = dbConn.CreateCommand();
            //        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            //        try
            //        {
            //            // 결재요청마스터 생성
            //            strSql = " usp_ACD001P7 @pTYPE = 'I2'";
            //            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //            strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
            //            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

            //            DataSet ds11 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            //            ERRCode = ds11.Tables[0].Rows[0][0].ToString();
            //            MSGCode = ds11.Tables[0].Rows[0][1].ToString();
            //            strNewAssignNo = ds11.Tables[0].Rows[0][2].ToString();      // 생성된 결재요청번호
            //            strNewStatus = ds11.Tables[0].Rows[0][3].ToString();        // 전표 그룹웨어상태

            //            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

            //            // 저장 대상 전표/결재요청 데이터 체크
            //            strSql_Chk = " usp_ACD001P7 @pTYPE = 'C1' ";
            //            strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //            strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //            strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + strNewAssignNo + "'";
            //            strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

            //            DataSet ds21 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
            //            ERRCode = ds21.Tables[0].Rows[0][0].ToString();
            //            MSGCode = ds21.Tables[0].Rows[0][1].ToString();

            //            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }

            //            // 상신자 결재요청상세 생성
            //            strSql = " usp_ACD001P7 @pTYPE = 'I3'";
            //            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //            strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
            //            strSql = strSql + ", @pTASK_NO = '" + strNewAssignNo + "'";
            //            strSql = strSql + ", @pASSIGN_ID = '" + txtUserId.Text + "'";
            //            strSql = strSql + ", @pASSIGN_COMMENT = '" + txtAssignComment.Text + "'";       // 2022.02.12. hma 추가: 상신코멘트
            //            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

            //            DataSet ds23 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            //            ERRCode = ds23.Tables[0].Rows[0][0].ToString();
            //            MSGCode = ds23.Tables[0].Rows[0][1].ToString();

            //            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


            //            // 입력된 결재자들 행수만큼 결재요청상세 생성
            //            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            //            {
            //                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

            //                if (strHead.Length > 0)
            //                {
            //                    strSql = " usp_ACD001P7 @pTYPE = 'I1'";
            //                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //                    strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //                    strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
            //                    strSql = strSql + ", @pTASK_NO = '" + strNewAssignNo + "'";
            //                    strSql = strSql + ", @pTASK_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "요청SEQ")].Text + "'";
            //                    strSql = strSql + ", @pASSIGN_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text + "'";
            //                    strSql = strSql + ", @pASSIGN_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재순서")].Text + "'";
            //                    strSql = strSql + ", @pASSIGN_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value.ToString() + "'";
            //                    strSql = strSql + ", @pASSIGN_OWNER = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value.ToString() + "'";
            //                    strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

            //                    DataSet ds12 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
            //                    ERRCode = ds12.Tables[0].Rows[0][0].ToString();
            //                    MSGCode = ds12.Tables[0].Rows[0][1].ToString();

            //                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
            //                }
            //            }

            //            // 저장된 결재선 데이터 체크
            //            strSql_Chk = " usp_ACD001P7 'C2' ";
            //            strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            //            strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //            strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + strNewAssignNo + "'";
            //            strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

            //            DataSet ds22 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
            //            ERRCode = ds22.Tables[0].Rows[0][0].ToString();
            //            MSGCode = ds22.Tables[0].Rows[0][1].ToString();

            //            if (ERRCode == "OK")
            //            {
            //                MSGCode = "상신되었습니다.";
            //            }

            //            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

            //            Trans.Commit();
            //        }
            //        catch
            //        {
            //            Trans.Rollback();
            //            MSGCode = "P0001";
            //        }

            //    Exit:
            //        dbConn.Close();

            //        if (ERRCode == "OK")
            //        {
            //            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            //            SearchExec();
            //            strChgFlag = "Y";       // 변경여부를 Y로

            //            RtnStr(strNewAssignNo, strNewStatus);
            //            this.DialogResult = DialogResult.OK;
            //            this.Close();
            //        }
            //        else if (ERRCode == "ER")
            //        {
            //            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        }
            //    }
            //}

            //this.Cursor = Cursors.Default;

            try
            {
                // 상신처리 팝업 띄움. 
                ACD001P10 pu = new ACD001P10(txtSlipNo.Text, cboTaskType.SelectedValue.ToString(), txtUserId.Text, fpSpread1, GHIdx1);
                pu.ShowDialog();

                string[] Msgs = pu.ReturnVal;
                if (Msgs != null && Msgs[0] == "Y")
                {
                    SearchExec();
                    strChgFlag = "Y";       // 변경여부를 Y로;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        #region btnChange_Click(): 조직도 형태의 사용자정보 팝업 띄우고 결재자를 복수 라인으로 선태하도록 함.
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

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text = txtUserId.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = txtUserNm.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선번호")].Value = cboRoutNo.SelectedValue.ToString();
                        //fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재선명세")].Text = txtRoutNm.Text;

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무코드")].Value = cboTaskType.SelectedValue.ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "업무구분")].Text = cboTaskType.Text;
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "기본결재선")].Text = (chkMajorYn.Checked ? "Y" : "N");

                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text = FormDt.Rows[i]["결재자"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Text = FormDt.Rows[i]["결재자명"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = FormDt.Rows[i]["결재단계"].ToString();
                        fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = strDefaultAssignType;
                    }
                }
            }
        }
        #endregion

        #region btnRoutChange_Click(): 선택한 결재선번호로 그리드의 결재라인을 변경한다.
        private void btnRoutChange_Click(object sender, EventArgs e)
        {
            // 결재상태가 상신대기인지 체크. 상신을 한 경우에는 이미 전표번호에 대한 결재라인이 있으므로 변경 안되게
            if (cboGwStatus.SelectedValue.ToString() != "READY")
            {
                MessageBox.Show("그룹웨어 상태가 상신대기가 아니므로 결재선을 변경할 수 없습니다.");
                return;
            }

            fpSpread1.ActiveSheet.Rows.Count = 0;       // 그리드 라인수 0으로.
            SearchExec();

            // 선택한 결재선번호에 대한 결재선 데이터 가져온다.
            // 선택된 결재선번호의 결재라인이 없으면 해당 결재선존재하는 경우 그리드의 행을 모두 삭제하고, 행추가 처리.
        }
        #endregion

        #region cboRoutNo_SelectedValueChanged(): 선택된 결재선번호에 대한 명세와 기본결재선여부를 가져와서 보여준다.
        private void cboRoutNo_SelectedValueChanged(object sender, EventArgs e)
        {
            AssignDescr_Search();
        }
        #endregion

        #region AssignDescr_Search(): 선택된 결재선번호에 대한 명세와 기본결재선여부를 가져와서 보여준다.
        private void AssignDescr_Search()
        {
            string strQuery = " usp_ACD001P7 @pTYPE = 'S4'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";
            strQuery += ", @pUSR_ID = '" + txtUserId.Text.Trim() + "' ";
            strQuery += ", @pROUT_NO = '" + cboRoutNo.SelectedValue.ToString() + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            chkMajorYn.Enabled = true;

            if (ds.Tables[0].Rows.Count > 0)
            {
                //txtRoutNm.Value = ds.Tables[0].Rows[0]["ROUT_NM"].ToString();
                chkMajorYn.Checked = (ds.Tables[0].Rows[0]["MAJOR_YN"].ToString() == "Y" ? true : false);
            }
            else
            {
                //txtRoutNm.Text = "";
                chkMajorYn.Checked = false;
            }

            chkMajorYn.Enabled = false;
        }
        #endregion


        private void SearchHdr()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQueryM = " usp_ACD001P7 @pTYPE = 'S1'";
                strQueryM += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQueryM += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "' ";
                strQueryM += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQueryM);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtUserId.Value = ds.Tables[0].Rows[0]["USR_ID"].ToString();
                    txtUserNm.Value = ds.Tables[0].Rows[0]["USR_NM"].ToString();
                    txtAssignNo.Value = ds.Tables[0].Rows[0]["ASSIGN_NO"].ToString();
                    cboTaskType.SelectedValue = ds.Tables[0].Rows[0]["TASK_TYPE"].ToString();
                    cboGwStatus.SelectedValue = ds.Tables[0].Rows[0]["GW_STATUS"].ToString();

                    // 2022.02.14. hma 수정: 상신자코멘트는 팝업에서 입력하게 하고 주석 처리함.
                    // 2022.02.12. hma 추가(Start): 상신자코멘트 항목을 상신대기/반려일 경우에만 수정하게.
                    //txtAssignComment.Text = ds.Tables[0].Rows[0]["ASSIGN_COMMENT_0"].ToString();
                    //if ((cboGwStatus.SelectedValue.ToString() != "READY") && (cboGwStatus.SelectedValue.ToString() != "REJECT"))
                    //    txtAssignComment.Enabled = false;
                    // 2022.02.12. hma 추가(End)

                    //// 그룹웨어상태가 상신대기도 아니고 반려도 아니면 상신 버튼 비활성화
                    //if (cboGwStatus.SelectedValue.ToString() != "READY" && cboGwStatus.SelectedValue.ToString() != "REJECT")
                    //{
                    //    btnAssign.Enabled = false;
                    //}
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        
    }
}
