

#region 작성정보
/*********************************************************************/
// 단위업무명: 전표 결재상신
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-02-14
// 작성내용  : 선택한 전표에 대한 상신코멘트를 입력하고 상신 처리한다.
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
    public partial class ACD001P10 : UIForm.Buttons
    {
        #region 변수선언
        string strSlipNo = "";
        string strTaskType = "";
        string strUserId = "";
        FarPoint.Win.Spread.FpSpread fpAssignGrid;
        string strCallType = "";
        string[,] strHdx = null;

        string[] returnVal = null;
        string strChgFlag = "";
        #endregion

        public ACD001P10()
        {
            InitializeComponent();
        }

        public ACD001P10(string SLIP_NO)
        {
            strSlipNo = SLIP_NO;
            strCallType = "A";

            InitializeComponent();
        }

        public ACD001P10(string SLIP_NO, string TASK_TYPE, string USER_ID, FarPoint.Win.Spread.FpSpread ASSIGN_GRID, string[,] GRID_IDX)
        {
            strSlipNo = SLIP_NO;
            strTaskType = TASK_TYPE;
            strUserId = USER_ID;
            fpAssignGrid = ASSIGN_GRID;
            strHdx = GRID_IDX;
            strCallType = "B";

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD001P10_Load(object sender, System.EventArgs e)
        {
            try
            {
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "결재상신";

                SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);

                txtSlipNo.Value = strSlipNo;

                strChgFlag = "N";

                SearchExec();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region ACD001P10_FormClosing(): 폼 종료시 저장/상신 처리 여부 체크
        private void ACD001P10_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strChgFlag == "Y")      // 상신 처리를 한 경우 폼 닫은 후 전표 조회 다시하도록 함.
            {
                RtnStr("OK");
            }
            else
            {
                RtnStr("Cancel");
            }
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_ACD001P10 @pTYPE = 'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dtpSlipDt.Value = ds.Tables[0].Rows[0]["SLIP_RES_DT"].ToString();
                    txtDeptCd.Value = ds.Tables[0].Rows[0]["DEPT_CD"].ToString();
                    txtDeptNm.Value = ds.Tables[0].Rows[0]["DEPT_NM"].ToString();
                    txtAssignNo.Value = ds.Tables[0].Rows[0]["ASSIGN_NO"].ToString();
                    cboGwStatus.SelectedValue = ds.Tables[0].Rows[0]["GW_STATUS"].ToString();

                    if (cboGwStatus.SelectedValue.ToString() == "READY" || cboGwStatus.SelectedValue.ToString() == "REJECT")
                    {
                        btnAssign.Enabled = true;
                        txtAssignComment.ReadOnly = false;
                    }
                    else
                    {
                        btnAssign.Enabled = false;
                        txtAssignComment.ReadOnly = true;
                    }
                }
                else
                {
                    btnAssign.Enabled = false;
                }
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

        public void RtnStr(string AssignNo)
        {
            returnVal = new string[2];
            returnVal[0] = strChgFlag;
        }

        #endregion

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(txtSlipNo.Text + " 전표를 상신하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (strCallType == "A")
                {
                    AssignBaseLine();       // 기본결재선을 이용한 결재상신 처리
                }
                else
                {
                    AssignGridLine();       // 그리드의 결재라인을 이용한 결재상신 처리
                }                
            }
        }

        #region AssignBaseLine(): 기본결재선을 이용한 결재상신 처리
        private void AssignBaseLine()
        {
            // 결재상신처리
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.
            string strSLIPNO = txtSlipNo.Text;

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                ERRCode = "ER";

                string strSql = " usp_ACD001_ASSIGN @pTYPE = 'I1' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text.ToUpper().Trim() + "'";
                strSql = strSql + ", @pUSR_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                strSql = strSql + ", @pASSIGN_COMMENT = '" + txtAssignComment.Text + "'";
                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);

                ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                MSGCode = ds2.Tables[0].Rows[0][1].ToString();
                strSLIPNO = txtSlipNo.Text.ToUpper().Trim();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

                Trans.Commit();
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "SY002";  //에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                strChgFlag = "Y";

                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                RtnStr(txtAssignNo.Text);
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
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region AssignGridLine(): 그리드의 결재라인을 이용한 결재상신 처리
        private void AssignGridLine()
        {
            string strNewAssignNo = "";
            string strNewStatus = "";

            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpAssignGrid, "ACD001P7", "fpSpread1", false) == true)
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
                        strSql = " usp_ACD001P7 @pTYPE = 'I2'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";
                        strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds11 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds11.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds11.Tables[0].Rows[0][1].ToString();
                        strNewAssignNo = ds11.Tables[0].Rows[0][2].ToString();      // 생성된 결재요청번호
                        strNewStatus = ds11.Tables[0].Rows[0][3].ToString();        // 전표 그룹웨어상태

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 저장 대상 전표/결재요청 데이터 체크
                        strSql_Chk = " usp_ACD001P7 @pTYPE = 'C1' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + strTaskType + "'";
                        strSql_Chk = strSql_Chk + ", @pTASK_NO = '" + strNewAssignNo + "'";
                        strSql_Chk = strSql_Chk + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";

                        DataSet ds21 = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                        ERRCode = ds21.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds21.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }

                        // 상신자 결재요청상세 생성
                        strSql = " usp_ACD001P7 @pTYPE = 'I3'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";
                        strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                        strSql = strSql + ", @pTASK_NO = '" + strNewAssignNo + "'";
                        strSql = strSql + ", @pASSIGN_ID = '" + strUserId + "'";
                        strSql = strSql + ", @pASSIGN_COMMENT = '" + txtAssignComment.Text + "'";       // 2022.02.12. hma 추가: 상신코멘트
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds23 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds23.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds23.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


                        // 입력된 결재자들 행수만큼 결재요청상세 생성
                        for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpAssignGrid.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                strSql = " usp_ACD001P7 @pTYPE = 'I1'";
                                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                                strSql = strSql + ", @pTASK_TYPE = '" + strTaskType + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "'";
                                strSql = strSql + ", @pTASK_NO = '" + strNewAssignNo + "'";
                                strSql = strSql + ", @pTASK_SEQ = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "요청SEQ")].Text + "'";
                                strSql = strSql + ", @pASSIGN_ID = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재자")].Text + "'";
                                strSql = strSql + ", @pASSIGN_SEQ = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재순서")].Text + "'";
                                strSql = strSql + ", @pASSIGN_TYPE = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재단계")].Value.ToString() + "'";
                                strSql = strSql + ", @pASSIGN_OWNER = '" + fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재구분")].Value.ToString() + "'";
                                strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                DataSet ds12 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds12.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds12.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                        // 저장된 결재선 데이터 체크
                        strSql_Chk = " usp_ACD001P7 'C2' ";
                        strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strSql_Chk = strSql_Chk + ", @pTASK_TYPE = '" + strTaskType + "'";
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

                        RtnStr(txtAssignNo.Text);
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
