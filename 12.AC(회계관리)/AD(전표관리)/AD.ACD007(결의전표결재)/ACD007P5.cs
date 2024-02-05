

#region 작성정보
/*********************************************************************/
// 단위업무명: 결의전표결재 > 일괄결재
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-03-18
// 작성내용  : 일괄결재 처리
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
    public partial class ACD007P5 : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpAssignGrid;
        string[,] strHdx = null;
        string strAdminYn = "";
        string strFinanceYn = "";
        string strAssignId = "";
        string strAssignNm = "";
        string[] returnVal = null;

        //string strLastAssignIdYn = "";      // 최종승인자여부(접수부서)
        string strResultType = "";
        string strResultMsg = "";

        string strChgFlag = "";
        string strCheckResult = "";

        string strCurDate = "";
        #endregion

        public ACD007P5()
        {
            InitializeComponent();
        }

        public ACD007P5(FarPoint.Win.Spread.FpSpread ASSIGN_GRID, string[,] GRID_IDX, string ADMIN_YN, string FINANCE_YN, string ASSIGN_ID, string ASSIGN_NM)
        {
            fpAssignGrid = ASSIGN_GRID;
            strHdx = GRID_IDX;
            strAdminYn = ADMIN_YN;
            strFinanceYn = FINANCE_YN;
            strAssignId = ASSIGN_ID;
            strAssignNm = ASSIGN_NM;

            InitializeComponent();
        }

        #region Form Load 시
        private void ACD007P5_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "결의전표결재 > 일괄결재";

                SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");       // 업무구분

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B091', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "라인결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B093', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                txtAdminRollYn.Value = strAdminYn;
                txtFinanceDeptYn.Value = strFinanceYn;
                txtAssignId.Value = strAssignId;
                txtAssignNm.Value = strAssignNm;

                cboTaskType.SelectedValue = "TSLIP";        // 업무구분에 결의전표로.
                strChgFlag = "N";
                
                strCurDate = SystemBase.Base.ServerTime("YYMMDD");    // 현재일자

                GridDataCheck();
                if (strCheckResult == "ER")
                    this.Close();
                else
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
                int SelectedRow;
                string strLastAssignIdYn = "";      // 2022.05.10. hma 추가: 최종승인자여부(접수부서)

                for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                {
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                    {
                        // 그리드에 행추가
                        UIForm.FPMake.RowInsert(fpSpread1);
                        SelectedRow = fpSpread1.Sheets[0].ActiveRowIndex;

                        // 선택된 행의 항목들을 행추가된 항목에 복사
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의일자")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "차변금액")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의부서")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의부서")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "생성경로")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "생성경로")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "관련번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "관련번호")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재요청번호")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상신일자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "상신일자")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀결재상태")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "재무팀결재상태")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "상신자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "상신자")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자ID")].Value = txtAssignId.Text;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명")].Value = txtAssignNm.Text;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "라인결재상태")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "라인결재상태")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "회계예정일")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종결재자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "최종결재자")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재단계")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재구분")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀여부")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "재무팀여부")].Value;
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "최종승인자여부")].Value;

                        // 재무팀인 경우 회계예정일이 없으면 결의일자로 보이도록 함.
                        // 2022.05.10. hma 수정(Start): 최종 결재자인 경우에도 회계예정일 처리
                        //if (txtFinanceDeptYn.Text == "Y")
                        strLastAssignIdYn = fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text;
                        if ((txtFinanceDeptYn.Text == "Y") || (strLastAssignIdYn == "Y"))
                        // 2022.05.10. hma 수정(End)
                        {
                            if (fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Text == "")
                                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Value = fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자")].Value;
                        }

                        fpSpread1.Sheets[0].RowHeader.Cells[SelectedRow, 0].Value = "";     // 행추가하여 만들어진 I 없애기

                        // 2022.05.11. hma 추가(Start): 해당 전표건의 최종승인자여부 데이터 가져오기
                        string Query = "usp_ACD007 @pTYPE = 'S3'";
                        Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        Query += ", @pSLIP_NO = '" + fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Value.ToString() + "'";
                        Query += ", @pASSIGN_ID = '" + txtAssignId.Text + "'";                        

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                        if (dt.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text = dt.Rows[0]["LAST_ASSIGN_YN"].ToString();
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text = "";
                        }
                        // 2022.05.11. hma 추가(End)
                    }
                }

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (txtFinanceDeptYn.Text == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1,
                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결의부서") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생성경로") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "관련번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상신일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀결재상태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상신자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자ID") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "라인결재상태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일") + "|1"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최종결재자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀여부") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부") + "|3"
                                                   );
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1,
                                                  SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결의부서") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "생성경로") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "관련번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상신일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀결재상태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상신자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자ID") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재자명") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "라인결재상태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최종결재자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재단계") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "결재구분") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀여부") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부") + "|3"
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

        #region 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string AssignNo)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                returnVal = new string[2];
                returnVal[0] = strChgFlag;
                //returnVal[1] = AssignNo;
            }
        }
        #endregion

        #region btnApprv_Click(): 승인 버튼 클릭시. 그리드의 건들을 승인 처리
        private void btnApprv_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("일괄결재 승인하시겠습니까? ", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            string strLastAssignIdYn = "";      // 2022.05.10. hma 추가: 최종승인자여부(접수부서)

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            // 접수부서 승인인 경우 회계예정일이 현재일자보다 큰지 체크
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text == "Y")
                            {
                                if (Convert.ToInt32(strCurDate.Replace("-", "")) <
                                    Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Text.Replace("-", "")))
                                {
                                    ERRCode = "ER";
                                    MSGCode = "회계예정일은 현재일자보다 클수 없습니다.";
                                    Trans.Rollback(); goto Exit;
                                }
                            }

                            // 승인하기 위한 데이터 체크
                            string strSql = " usp_ACD007P3 'C1' ";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                            strSql = strSql + ", @pASSIGN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호")].Text + "'";
                            strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

                            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                            if (dt.Rows.Count > 0)
                            {
                                ERRCode = dt.Rows[0]["RESULT_CODE"].ToString();
                                MSGCode = dt.Rows[0]["RESULT_MSG"].ToString();
                            }

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } // ER 코드 Return시 점프


                            // 결재승인 호출
                            string strSql_Appr = " usp_ACD007P3 @pTYPE = 'U1' ";
                            strSql_Appr = strSql_Appr + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql_Appr = strSql_Appr + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                            strSql_Appr = strSql_Appr + ", @pASSIGN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호")].Text + "'";
                            strSql_Appr = strSql_Appr + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                            strSql_Appr = strSql_Appr + ", @pASSIGN_COMMENT = '" + txtComment.Text + "'";

                            // 2022.05.10. hma 수정(Start): 최종 결재자인 경우에도 회계예정일 저장되게 함.
                            //if (txtFinanceDeptYn.Text == "Y")       // 재무팀인 경우 회계전표일자도 저장되게.
                            strLastAssignIdYn = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text;
                            if ((txtFinanceDeptYn.Text == "Y") || (strLastAssignIdYn == "Y"))
                            // 2022.05.10. hma 수정(End)
                                strSql_Appr = strSql_Appr + ", @pASSIGN_GL_SLIP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Text + "'";
                            else
                                strSql_Appr = strSql_Appr + ", @pASSIGN_GL_SLIP_DT = ''";

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql_Appr, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프


                            // 접수부서 승인인 경우 전표승인 처리
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "최종승인자여부")].Text == "Y")
                            {
                                strSql = " usp_ACD003 'U1'";     // 결의전표승인
                                strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                                strSql = strSql + ", @pSLIP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text + "'";
                                strSql = strSql + ", @pSLIP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계예정일")].Text + "'";
                                strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                // 2022.05.10. hma 수정(Start): 일괄처리여부 확인하기 위해 UP_IP 필드 활용
                                //strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";
                                strSql = strSql + ", @pUP_IP = 'B'";
                                // 2022.05.10. hma 수정(End)

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                            }

                            fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Value = "";     // 회계예정일 수정하면 헤더에 U가 들어가므로 없애기
                        }

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        strChgFlag = "Y";           // 변경여부를 Y로.                

                        RtnStr("");
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

        private void Data_Check()
        {
            strResultType = "";
            strResultMsg = "";

            // 승인하기 위한 데이터 체크
            string strSql = " usp_ACD007P3 'C1' ";
            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            //strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
            //strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                strResultType = dt.Rows[0]["RESULT_CODE"].ToString();
                strResultMsg = dt.Rows[0]["RESULT_MSG"].ToString();
            }
        }

        #region btnReject_Click(): 반려 버튼 클릭시. 그리드의 건들을 반려 처리
        private void btnReject_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("일괄결재 반려하시겠습니까? ", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    string ERRCode = "ER", MSGCode = "P0000";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        // 행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            // 결재반려처리
                            string strSql = " usp_ACD007P3 @pTYPE = 'U2' ";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                            strSql = strSql + ", @pASSIGN_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재요청번호")].Text + "'";
                            strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                            strSql = strSql + ", @pASSIGN_COMMENT = '" + txtComment.Text + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프
                        }

                        Trans.Commit();
                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
                    }

                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        strChgFlag = "Y";           // 변경여부를 Y로.                

                        RtnStr("");
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

        #region GridDataCheck(): 선택항목에 체크된 건수 표기 및 데이터 체크
        private void GridDataCheck()
        {
            int iChkCnt = 0;
            string strGwStatusChk = "N";
            string strLineStatusChk = "N";
            string strPlanSlipDtChk = "N";
            string strSlipNo = "";

            strCheckResult = "OK";

            for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
            {                
                if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                {
                    // 선택건수 증가
                    iChkCnt++;

                    // 전표결재상태 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "재무팀결재상태")].Value.ToString() != "START" &&
                           fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "재무팀결재상태")].Value.ToString() != "ING")
                    {
                        strGwStatusChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }

                    // 결재 라인상태 체크
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "라인결재상태")].Value.ToString() != "WAIT")
                    {
                        strLineStatusChk = "Y";
                        strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                        break;
                    }

                    //// 최종승인자인 경우 회계전표일자가 현재일자보다 크면 메시지 띄우고 처리 안되게.    // 승인 처리시에만 체크하게 하고 주석 처리.
                    //if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "최종승인자여부")].Value.ToString() == "Y")
                    //{
                    //    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "회계예정일")].Text != "")
                    //    {
                    //        if (Convert.ToInt32(strCurDate.Replace("-", "")) <
                    //                Convert.ToInt32(fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "회계예정일")].Text.Replace("-", "")))
                    //        {
                    //            strPlanSlipDtChk = "Y";
                    //            strSlipNo = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Text;
                    //            break;
                    //        }
                    //    }
                    //}
                }
            }

            // 선택건수 표기
            txtCheckedCnt.Value = iChkCnt.ToString();

            if (iChkCnt == 0)
            {
                strCheckResult = "ER";
                MessageBox.Show("선택된 건이 없으므로 처리할 수 없습니다.");
                return;
            }

            if (strGwStatusChk == "Y")
            {
                strCheckResult = "ER";
                MessageBox.Show(strSlipNo + " 전표번호의 재무팀결재상태가 상신/결재진행 상태가 아니므로 결재 할 수 없습니다.");
                return;
            }

            if (strLineStatusChk == "Y")
            {
                strCheckResult = "ER";
                MessageBox.Show(strSlipNo + " 전표번호의 라인결재상태가 대기 상태가 아니므로 결재 할 수 없습니다.");
                return;
            }

            if (strPlanSlipDtChk == "Y")
            {
                strCheckResult = "ER";
                MessageBox.Show(strSlipNo + " 전표번호의 회계예정일이 현재일자보다 미래일자이므로 결재 할 수 없습니다.");
                return;
            }
        }
        #endregion
    }
}
