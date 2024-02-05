#region 작성정보
/*********************************************************************/
// 단위업무명: 결의전표결재
// 작 성 자  : 한 미 애
// 작 성 일  : 2021-12-16
// 작성내용  : 결의전표결재
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
    public partial class ACD007 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        string strAdminYn = "N";
        string strFinanceYn = "N";

        public ACD007()
        {
            InitializeComponent();
        }

        #region ACD007_Load(): Form Load 시
        private void ACD007_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //발생경로
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE = 'BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE = 'COMM3', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);    // 2022.05.03. hma 추가: 그룹웨어상태(재무팀결재상태)

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "재무팀결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            // 2022.05.11. hma 수정(Start): 결의일자 기간이 이전년월 1일 ~ 해당년월 말일로 지정되도록
            //string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            //dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            //dtpSlipDtTo.Text = YYMMDD;
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = Convert.ToDateTime(dtpSlipDtFr.Text).AddMonths(2).AddDays(-1).ToShortDateString().Substring(0, 10);
            // 2022.05.11. hma 수정(End)

            txtSlipAmtFr.Text = "";
            txtSlipAmtTo.Text = "";
            rdoGwStatusWait.Checked = true;     // 기본적으로 미결 상태가 조회되도록 함.

            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            txtAssignId.Value = SystemBase.Base.gstrUserID;
            Check_RollGroup();          // 관리자권한여부 체크

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec(): New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            // 2022.04.29. hma 추가(Start): 초기화시 결재자ID를 다시 세팅하도록 함.
            txtAssignId.Value = SystemBase.Base.gstrUserID;
            Check_RollGroup();          // 관리자권한여부 체크
            // 2022.04.29. hma 추가(End)

            txtSlipAmtFr.Text = "";
            txtSlipAmtTo.Text = "";

            // 2022.05.11. hma 수정(Start): 결의일자 기간이 이전년월 1일 ~ 해당년월 말일로 지정되도록
            //string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            //dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            //dtpSlipDtTo.Text = YYMMDD;
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = Convert.ToDateTime(dtpSlipDtFr.Text).AddMonths(2).AddDays(-1).ToShortDateString().Substring(0, 10);
            // 2022.05.11. hma 수정(End)
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec(): 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strGwStaus = "";
                    if (rdoGwStatusWait.Checked == true)        // 대기
                        strGwStaus = "WAIT";
                    else if (rdoGwStatusNotCmpl.Checked == true)       // 2022.02.10. hma 추가: 미결
                        strGwStaus = "NOT_CMPL";
                    else if (rdoGwStatusCmpl.Checked == true)
                        strGwStaus = "COMPLETE";
                    else if (rdoGwStatusReject.Checked == true)        // 2022.02.10. hma 추가: 반려
                        strGwStaus = "REJECT";

                    string strQuery = " usp_ACD007  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    strQuery += ", @pCREATE_PATH = '" + cboCreathPath.SelectedValue.ToString() + "' ";
                    if(txtSlipAmtFr.Text != "")
                        strQuery += ", @pSLIP_AMT_FROM = '" + txtSlipAmtFr.Text.Replace(",","") + "' ";
                    if (txtSlipAmtTo.Text != "") 
                        strQuery += ", @pSLIP_AMT_TO = '" + txtSlipAmtTo.Text.Replace(",", "") + "' ";
                    strQuery += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pREF_NO = '" + txtRefNo.Text + "' ";
                    strQuery += ", @pASSIGN_ID = '" + txtAssignId.Text + "' ";      // 결재자
                    strQuery += ", @pASSIGN_STATUS = '" + strGwStaus + "' ";        // 결재상태
                    strQuery += ", @pREMARK = '" + txtRemark.Text + "' ";
                    strQuery += ", @pGW_STATUS = '" + cboGwStatus.SelectedValue.ToString() + "' ";      // 2022.05.03. hma 추가: 전표결재상태(재무팀결재상태)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    // 2022.02.04. hma 추가(Start): 결재단계가 승인인 결재자는 파랑색으로 지정
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자1_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자1")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자2_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자2")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자3_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자3")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자4_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자4")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자5_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자5")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자6_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자6")].ForeColor = Color.Blue;
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자7_3")].Text == "승인")
                        {
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자7")].ForeColor = Color.Blue;
                        }
                    }
                    // 2022.02.04. hma 추가(End)
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 부서코드 TextChanged
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

        #region 부서정보 팝업
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    strREORG_ID = Msgs[5].ToString();
                    //cboBizAreaCd.SelectedValue = Msgs[3].ToString();
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

        #region 전표조회
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text;
                    string strAssignRollYn = chkAssignRollYn.Checked == true ? "Y" : "N";       // 2022.02.10. hma 추가: 대리결재여부
                    string strAssignIdMan = txtAssignId.Text;           // 2022.02.10. hma 추가

                    ACD007P3 pu = new ACD007P3(strSLIP_NO, strAdminYn, strFinanceYn, strAssignRollYn, strAssignIdMan);      // 2022.02.10. hma 수정: strAssignRollYn, strAssignIdMan 추가                  
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

        #region PrintExec(): PRINT 버튼 클릭 이벤트. 전표 출력한다.
        protected override void PrintExec()
        {
            try
            {
                string strSLIP_NO = "";
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        if (rdoPrintG.Checked == true)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표번호")].Text != "")
                                strSLIP_NO += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text + ",";
                        }
                        else
                        {
                            strSLIP_NO += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text + ",";
                        }
                    }
                }
                if (strSLIP_NO == "")
                {
                    MessageBox.Show("출력할 전표가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001_2_2.rpt";    // 레포트경로+레포트명     // 2022.02.18. hma 수정: ACD001_2.rpt => ACD001_2_2.rpt로 변경.
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = strSLIP_NO;
                    if (rdoPrintT.Checked == true) RptParmValue[3] = "T";
                    if (rdoPrintG.Checked == true) RptParmValue[3] = "G";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnSlipPrt_Click(): 전표출력 버튼 클릭 이벤트. 선택된 건들을 한페이지에 여러 건이 일괄 출력되도록 함.
        private void btnSlipPrt_Click(object sender, EventArgs e)
        {
            try
            {
                string strSLIP_NO = "";
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        if (rdoPrintG.Checked == true)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계전표번호")].Text != "")
                                strSLIP_NO += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text + ",";
                        }
                        else
                        {
                            strSLIP_NO += fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text + ",";
                        }
                    }
                }
                if (strSLIP_NO == "")
                {
                    MessageBox.Show("출력할 전표가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001_2_3.rpt";    // 레포트경로+레포트명     // 2022.02.18. hma 수정: ACD001_2_1.rpt => ACD001_2_3.rpt로 변경
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = strSLIP_NO;
                    if (rdoPrintT.Checked == true) RptParmValue[3] = "T";
                    if (rdoPrintG.Checked == true) RptParmValue[3] = "G";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전

                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick(): 그리드 더블클릭시 해당 전표번호에 대한 전표조회 팝업 띄워준다.
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = e.Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text;

                    string strAssignRollYn = chkAssignRollYn.Checked == true ? "Y" : "N";       // 2022.02.10. hma 추가: 대리결재여부
                    string strAssignIdMan = txtAssignId.Text;           // 2022.02.10. hma 추가

                    //ACD007P3 pu = new ACD007P3(strSLIP_NO, strAdminYn, strFinanceYn, strAssignRollYn, strAssignIdMan);    // 2022.02.10. hma 수정: strAssignRollYn, strAssignIdMan 추가
                    ACD007P4 pu = new ACD007P4(strSLIP_NO, strAdminYn, strFinanceYn, strAssignRollYn, strAssignIdMan);      // 2022.02.25. 추가
                    pu.ShowDialog();

                    string[] Msgs = pu.ReturnVal;
                    if (Msgs != null && Msgs[0] == "OK")
                    {
                        // 결재선 다시 조회 처리
                        SearchExec();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        

        #region Check_RollGroup(): 사용자ID에 대한 관리자권한여부 체크해서 관리자권한이 없는 사용자인 경우 결재자 항목 및 검색 버튼 비활성화 처리.
        private void Check_RollGroup()
        {
            string strQuery = " usp_ASSIGN_DIALOG @pTYPE = 'C1'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strQuery += ", @pUSR_ID = '" + txtAssignId.Text.Trim() + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            txtAdminRollYn.ReadOnly = false;
            txtFinanceDeptYn.ReadOnly = false;

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtAdminRollYn.Value = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                txtFinanceDeptYn.Value = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();

                strAdminYn = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                strFinanceYn = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();
            }
            else
            {
                txtAdminRollYn.Value = "";
                txtFinanceDeptYn.Value = "";
            }

            txtAdminRollYn.ReadOnly = true;
            txtFinanceDeptYn.ReadOnly = true;

            // 결재자 항목 활성화/비활성화 처리.
            // 2022.04.26. hma 수정(Start): ADMIN만이 아닌 ADMIN권한그룹인 경우 전체를 조회하도록 함.(김노경 팀장 요청)
            //if ((SystemBase.Base.gstrUserID == "ADMIN") || 
            if ((txtAdminRollYn.Text == "Y") ||
            // 2022.04.26. hma 수정(End)
                (txtFinanceDeptYn.Text == "Y"))     // 관리자이거나 재무팀이면 결재자 입력 가능하게.
            {
                txtAssignId.Enabled = true;
                btnAssign.Enabled = true;
            }
            else
            {
                txtAssignId.Enabled = false;
                btnAssign.Enabled = false;
            }

            // 대리결재 항목: ADMIN만 보이게. ==> 운영에 적용하기 위해 빌드시엔 주석 처리후 빌드 필요!!
            //if (SystemBase.Base.gstrUserID == "ADMIN")
            //    chkAssignRollYn.Visible = true;
            //else
            //    chkAssignRollYn.Visible = false;
        }
        #endregion

        #region btnAssign_Click(): 결재자 버튼 클릭시 사용자조회 팝업 띄워줌.
        private void btnAssign_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE = 'B011', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAssignId.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtAssignId.Text = Msgs[0].ToString();
                    txtAssignNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region txtAssignId_TextChanged(): 결재자 항목에 입력시 해당 사용자ID의 사용자명을 가져와서 항목에 넣어준다.
        private void txtAssignId_TextChanged(object sender, EventArgs e)
        {
            txtAssignNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtAssignId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        // 2022.03.18. hma 추가(Start)
        #region btnAssignPop_Click(): 일괄결재 버튼 클릭시. 일괄결재를 위한 팝업 띄움.
        private void btnAssignPop_Click(object sender, EventArgs e)
        {
            // ADMIN이 아니면 로그인ID로 조회후 일괄결재 팝업 띄우기 위해 체크
            string strLoginId = "";
            strLoginId = SystemBase.Base.gstrUserID;

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (strLoginId != "ADMIN")
                {
                    if (strLoginId != fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text)
                    {
                        MessageBox.Show("로그인ID를 결재자 검색조건에 입력하여 조회한 후 일괄결재 가능합니다.");
                        return;
                    }
                }
                else
                {
                    if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결재자")].Text == "")
                    {
                        MessageBox.Show("결재자 검색조건에 입력하여 조회후 일괄결재 가능합니다.");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("조회된 데이터가 없으므로 일괄결재 할 수 없습니다.");
                return;
            }

            // 일괄결재 팝업 띄움
            ACD007P5 pu = new ACD007P5(fpSpread1, GHIdx1, strAdminYn, strFinanceYn, txtAssignId.Text, txtAssignNm.Text);
            pu.ShowDialog();

            string[] Msgs = pu.ReturnVal;
            if (Msgs != null && Msgs[0] == "Y")
            {
                SearchExec();
            }
        }
        #endregion

        #region btnCheckY_Click(): 전체선택 버튼 클릭시. 
        private void btnCheckY_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = true;
            }
        }
        #endregion

        #region btnCheckN_Click(): 선택해제 버튼 클릭시.
        private void btnCheckN_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = false;
            }
        }

        private void lblCheckY_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = true;
            }
        }

        private void lblCheckN_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = false;
            }
        }
        #endregion
        // 2022.03.18. hma 추가(End)
    }
}
