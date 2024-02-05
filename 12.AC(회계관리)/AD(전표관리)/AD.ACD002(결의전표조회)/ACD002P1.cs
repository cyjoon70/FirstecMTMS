
#region 작성정보
/*********************************************************************/
// 단위업무명: 전표 일괄결재상신
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-03-17
// 작성내용  : 선택한 전표에 대한 상신코멘트를 입력하고 일괄 상신 처리한다.
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

namespace AD.ACD002
{
    public partial class ACD002P1 : UIForm.Buttons
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpAssignGrid;
        string[,] strHdx = null;
        string strAssignType = "";

        string[] returnVal = null;
        string strChgFlag = "";
        #endregion

        public ACD002P1()
        {
            InitializeComponent();
        }

        public ACD002P1(FarPoint.Win.Spread.FpSpread ASSIGN_GRID, string[,] GRID_IDX, string ASSIGN_TYPE)
        {
            fpAssignGrid = ASSIGN_GRID;
            strHdx = GRID_IDX;
            strAssignType = ASSIGN_TYPE;

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD002P1_Load(object sender, System.EventArgs e)
        {
            try
            {
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                SystemBase.Validation.GroupBox_Reset(groupBox2);

                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "일괄 결재상신/상신취소";

                strChgFlag = "N";

                // 상신자 항목에 로그인 사용자ID, 사용자명으로.
                txtAssignId.Value = SystemBase.Base.gstrUserID;
                txtAssignNm.Value = SystemBase.Base.gstrUserName;

                // 2022.05.03. hma 수정(Start): 자동전표는 회계예정일을 입력할 필요없으므로 회계예정일 항목에 공백이 들어가게 함.
                //dtpGlSlipDt.Value = SystemBase.Base.ServerTime("YYMMDD");       // 회계전표일자에 현재일자 들어가게.
                dtpGlSlipDt.Text = "";
                // 2022.05.03. hma 수정(End)

                lblGlSlipDt.Visible = false;
                dtpGlSlipDt.Visible = false;

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SearchExec() 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_ACD002 @pTYPE = 'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                // 재무팀여부 항목 세팅
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtFinanceYn.Value = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString(); // 재무팀여부
                    btnAssign.Enabled = true;
                }
                else
                {
                    btnAssign.Enabled = false;
                }             

                // 선택건수 체크
                int iChkCnt = 0;
                for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                {
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                    {
                        iChkCnt++;
                    }
                }

                // 선택건수 표기
                txtCheckedCnt.Value = iChkCnt.ToString();

                // 상신코멘트 항목 초기화
                txtAssignComment.Text = "";

                // 상신처리인 경우 재무팀여부에 따라 상신코멘트와 회계전표일자 항목 보이게 하고, 상신취소인 경우엔 보이지 않게.
                if (strAssignType == "R")
                {
                    // 재무팀이면 회계전표일자 보이게 하고, 아니면 안보이게.
                    // 2022.05.03. hma 수정(Start): 자동전표는 회계예정일을 입력할 필요없으므로 재무팀인 경우에도 보이지 않게 함.
                    //if (txtFinanceYn.Text == "Y")
                    //{
                    //    lblGlSlipDt.Visible = true;
                    //    dtpGlSlipDt.Visible = true;
                    //}
                    //else
                    //{
                    //    lblGlSlipDt.Visible = false;
                    //    dtpGlSlipDt.Visible = false;
                    //}
                    lblAssignComment.Visible = false;
                    txtAssignComment.Visible = false;
                    // 2022.05.03. hma 수정(End)

                    lblAssignComment.Visible = true;
                    txtAssignComment.Visible = true;

                    btnAssign.Text = "일괄결재상신";
                }
                else
                {
                    lblAssignComment.Visible = false;
                    txtAssignComment.Visible = false;
                    lblGlSlipDt.Visible = false;
                    dtpGlSlipDt.Visible = false;

                    btnAssign.Text = "일괄상신취소";
                }

                // 재무팀여부 항목 안보이게
                txtFinanceYn.Visible = false;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr()
        {
            returnVal = new string[2];
            returnVal[0] = strChgFlag;
        }

        #endregion

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (strAssignType == "R")
            {
                AssignRun();
            }
            else
            {
                AssignCancel();
            }

            this.Close();
        }

        #region AssignRun(): 일괄상신
        private void AssignRun()
        {
            // 해당 사용자의 기본 결재선이 등록되어있는지 체크
            string strChkQuery = "";
            strChkQuery = " usp_ACD002 @pTYPE = 'C1' ";
            strChkQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strChkQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strChkQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["RESULT_TYPE"].ToString() == "ER")
                {
                    MessageBox.Show(ds.Tables[0].Rows[0]["RESULT_MSG"].ToString());
                    return;
                }
            }

            // 선택된 건 가운데 결재상태가 상신대기 또는 반려가 아닌 건이 있는지 체크
            string strGwStatusChk = "N";
            string strCreatePathChk = "N";
            string strDocCntChk = "N";
            string strSlipNo = "";

            for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
            {
                if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                {
                    // 결재상태 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재상태")].Value.ToString() != "READY" &&
                           fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재상태")].Value.ToString() != "REJECT")
                    {
                        strGwStatusChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }

                    // 전표생성경로가 '결의전표'인 건이 존재하는지 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "생성경로")].Value.ToString() == "TG")
                    {
                        strCreatePathChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }

                    // 증빙건수가 0인 건이 존재하는지 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "증빙건수")].Text == "")
                    {
                        strDocCntChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }
                }
            }

            if (strGwStatusChk == "Y")
            {
                MessageBox.Show(strSlipNo + " 전표번호의 결재상태가 상신대기/반려 상태가 아니므로 결재상신 할 수 없습니다.");
                return;
            }

            if (strCreatePathChk == "Y")
            {
                MessageBox.Show(strSlipNo + " 생성경로가 결의전표인 건은 일괄 결재상신 할 수 없습니다.");
                return;
            }

            string strConfirmMsg = "";

            if (strDocCntChk == "Y")
                strConfirmMsg = "지출증빙이 등록되지 않은 건이 존재합니다. 그래도 결재상신 하시겠습니까?";
            else
                strConfirmMsg = "전표를 일괄 결재상신 하시겠습니까?";

            if (MessageBox.Show(strConfirmMsg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                AssignGridLine();       // 선택된 건들 결재상신 처리
            }
        }
        #endregion

        #region AssignGridLine(): 그리드의 결재라인을 이용한 결재상신 처리
        private void AssignGridLine()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpAssignGrid, "ACD002", "fpSpread1", false) == true)
                {
                    string strSql = "";

                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 선택된 건수만큼 처리
                        for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                        {
                            if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                            {
                                strSql = " usp_ACD001_ASSIGN @pTYPE = 'I1' ";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text + "'";
                                strSql = strSql + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pASSIGN_NO = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재요청번호")].Text + "'";
                                strSql = strSql + ", @pASSIGN_COMMENT = '" + txtAssignComment.Text + "'";
                                strSql = strSql + ", @pGL_SLIP_DT = ''";        // 2022.05.03. hma 수정: 일괄상신은 자동전표만 처리하므로 회계예정일 들어가지 않게 공백으로 처리. + dtpGlSlipDt.Text + "'";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds12 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds12.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds12.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        if (ERRCode == "OK")
                        {
                            MSGCode = "결재상신되었습니다.";
                        }

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        strChgFlag = "Y";

                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RtnStr();
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
        #endregion

        #region AssignCancel(): 일괄상신취소 처리
        private void AssignCancel()
        {
            // 해당 사용자의 기본 결재선이 등록되어있는지 체크
            string strChkQuery = "";
            strChkQuery = " usp_ACD002 @pTYPE = 'C1' ";
            strChkQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strChkQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strChkQuery);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["RESULT_TYPE"].ToString() == "ER")
                {
                    MessageBox.Show(ds.Tables[0].Rows[0]["RESULT_MSG"].ToString());
                    return;
                }
            }

            // 선택된 건 가운데 결재상태가 상신이 아닌 건이 있는지 체크
            string strGwStatusChk = "N";
            string strCreatePathChk = "N";
            string strSlipNo = "";

            for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
            {
                if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                {
                    // 결재상태 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재상태")].Value.ToString() != "START")
                    {
                        strGwStatusChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }

                    // 전표생성경로가 '결의전표'인 건이 존재하는지 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "생성경로")].Value.ToString() == "TG")
                    {
                        strCreatePathChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }
                }
            }

            if (strGwStatusChk == "Y")
            {
                MessageBox.Show(strSlipNo + " 전표번호의 결재상태가 상신 상태가 아니므로 상신취소 할 수 없습니다.");
                return;
            }

            if (strCreatePathChk == "Y")
            {
                MessageBox.Show(strSlipNo + " 생성경로가 결의전표인 건은 일괄 상신취소 할 수 없습니다.");
                return;
            }

            string strConfirmMsg = "";

            strConfirmMsg = " 전표를 일괄 상신취소 하시겠습니까?";

            if (MessageBox.Show(strConfirmMsg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                AssignCancelGridLine();       // 선택된 건들 상신취소 처리
            }
        }
        #endregion

        #region AssignCancelGridLine(): 일괄상신취소
        private void AssignCancelGridLine()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpAssignGrid, "ACD002", "fpSpread1", false) == true)
                {
                    string strSql = "";

                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 선택된 건수만큼 처리
                        for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                        {
                            if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                            {
                                strSql = " usp_ACD001_ASSIGN @pTYPE = 'D1' ";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text + "'";
                                strSql = strSql + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pASSIGN_NO = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재요청번호")].Text + "'";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                DataSet ds12 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds12.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds12.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        if (ERRCode == "OK")
                        {
                            MSGCode = "상신취소되었습니다.";
                        }

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch
                    {
                        Trans.Rollback();
                        MSGCode = "P0001";
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        strChgFlag = "Y";

                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RtnStr();
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
        #endregion
    }
}
