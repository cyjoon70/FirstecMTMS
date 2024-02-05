
#region 작성정보
/*********************************************************************/
// 단위업무명 : 결의전표조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-25
// 작성내용 : 결의전표조회
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

namespace AD.ACD002
{
    public partial class ACD002 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        string strCheckResultMsg = "";  // 2022.03.21. hma 추가: 체크 결과 내용

        public ACD002()
        {
            InitializeComponent();
        }

        
        #region Form Load 시
        private void ACD002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

            SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   // 발생경로
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      // 사업장
            SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM3', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);    // 2022.03.18. hma 추가: 그룹웨어상태

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = YYMMDD;
            txtSlipAmtFr.Text = "";
            txtSlipAmtTo.Text = "";
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            // 2022.03.17. hma 추가(Start)
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "생성경로")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'A101', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            // 2022.03.17. hma 추가(End)

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            txtSlipAmtFr.Text = "";
            txtSlipAmtTo.Text = "";
            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = YYMMDD;
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ACD002  'S1'";
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
                    strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "' ";
                    strQuery += ", @pIN_EMP_NM = '" + txtInEmpNm.Text + "' ";
                    strQuery += ", @pREMARK = '" + txtRemark.Text + "' ";
                    strQuery += ", @pGW_STATUS = '" + cboGwStatus.SelectedValue.ToString() + "' ";      // 2022.03.18. hma 추가: 전표결재상태(재무팀결재상태)
                    // 2022.05.03. hma 추가(Start): 승인여부 검색조건
                    string strConfirm = "";
                    if (rdoConfirmY.Checked == true)
                        strConfirm = "Y";
                    if (rdoConfirmN.Checked == true)
                        strConfirm = "N";
                    strQuery += ", @pCONFIRM_YN = '" + strConfirm + "' ";
                    // 2022.05.03. hma 추가(End)

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
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

        #region PrintExec() PRINT 버튼 클릭 이벤트
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
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001_2.rpt";    // 레포트경로+레포트명
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

        // 2018.08.06. ksh 추가(Start): 전표출력 버튼 클릭시 선택된 건들을 한페이지에 여러 건이 일괄 출력되도록 함.
        #region btnSlipPrt_Click(): 전표출력 버튼 클릭 이벤트
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
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001_2_1.rpt";    // 레포트경로+레포트명
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
        // 2018.08.06. ksh 추가(End)

        // 2022.03.14. hma 추가(Start)
        #region btnAssign_Click(): 결재상신 버튼 클릭시. 선택된 건들에 대해 상신 처리
        private void btnAssign_Click(object sender, EventArgs e)
        {
            // 2022.03.21. hma 추가(Start): 선택건수 및 조회조건 체크
            AssignDataCheck();

            if (strCheckResultMsg != "")
            {
                MessageBox.Show(strCheckResultMsg);
                return;
            }
            // 2022.03.21. hma 추가(End)

            // 상신 팝업 띄움
            ACD002P1 pu = new ACD002P1(fpSpread1, GHIdx1, "R");
            pu.ShowDialog();

            string[] Msgs = pu.ReturnVal;
            if (Msgs != null && Msgs[0] == "Y")
            {
                SearchExec();
            }
        }
        #endregion

        #region btnAssignCancel_Click(): 상신취소 버튼 클릭시. 선택된 건들에 대해 상신취소 처리
        private void btnAssignCancel_Click(object sender, EventArgs e)
        {
            // 2022.03.21. hma 추가(Start): 선택건수 및 조회조건 체크
            AssignDataCheck();

            if (strCheckResultMsg != "")
            {
                MessageBox.Show(strCheckResultMsg);
                return;
            }
            // 2022.03.21. hma 추가(End)

            // 상신 팝업 띄움
            ACD002P1 pu = new ACD002P1(fpSpread1, GHIdx1, "C");
            pu.ShowDialog();

            string[] Msgs = pu.ReturnVal;
            if (Msgs != null && Msgs[0] == "Y")
            {
                SearchExec();
            }
        }
        #endregion

        #region btnSlipDoc_Click(): 지출증빙 버튼 클릭시. 선택된 건들에 대해 지출증빙등록 처리
        private void btnSlipDoc_Click(object sender, EventArgs e)
        {
            // 선택건수 및 조회조건 체크
            DocDataCheck();

            if (strCheckResultMsg != "")
            {
                MessageBox.Show(strCheckResultMsg);
                return;
            }

            // 지출증빙 팝업 띄움
            ACD002P3 pu = new ACD002P3(fpSpread1, GHIdx1);
            pu.ShowDialog();

            string[] Msgs = pu.ReturnVal;
            if (Msgs != null && Msgs[0] == "Y")
            {
                SearchExec();
            }
        }
        #endregion
        // 2022.03.14. hma 추가(End)

        // 2022.03.21. hma 추가(Start)
        #region CheckSelectedCount(): 그리드 선택 항목 체크된 건수 체크
        private void AssignDataCheck()
        {
            int ChkCnt = 0;
            strCheckResultMsg = "";

            if (txtInEmpNm.Text == "")
            {
                strCheckResultMsg = "입력자 항목에 상신자를 입력 후 결재상신/상신취소 가능합니다.";
                return;
            }

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입력자")].Text != txtInEmpNm.Text)
                {
                    strCheckResultMsg = "입력자 검색조건으로 조회하신 후 결재상신/상신취소 처리하세요.";
                    break;
                }

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                {
                    ChkCnt++;
                }                
            }

            // 선택건수 체크
            if (strCheckResultMsg == "")
            {
                if (ChkCnt == 0)
                {
                    strCheckResultMsg = "결재상신/상신취소 처리를 위하여 선택된 건이 없습니다.";
                }
            }

            // 로그인 사용자와 입력자 항목이 동일한지. 본인 전표에 대해서만 상신 처리하도록 하기 위해 체크.
            if (strCheckResultMsg == "")
            {
                string strLoginNm = "";
                string strLoginId = "";     // 2022.05.03. hma 추가

                strLoginNm = SystemBase.Base.gstrUserName;
                strLoginId = SystemBase.Base.gstrUserID;        // 2022.05.03. hma 추가

                if (strLoginId != "ADMIN")  // 2022.05.03. hma 수정: strLoginNm => strLoginId
                {
                    if (strLoginNm != fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "상신자")].Text)
                    {
                        strCheckResultMsg = "로그인 사용자명과 입력자 검색조건을 동일하게 입력하여 조회한 경우에만 결재상신/상신취소 가능합니다.";
                    }
                }
            }
        }
        #endregion

        private void DocDataCheck()
        {
            int ChkCnt = 0;
            strCheckResultMsg = "";

            if (txtInEmpNm.Text == "")
            {
                strCheckResultMsg = "입력자 항목에 등록자를 입력 후 지출증빙 가능합니다.";
                return;
            }

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입력자")].Text != txtInEmpNm.Text)
                {
                    strCheckResultMsg = "입력자 검색조건으로 조회하신 후 지출증빙등록 처리하세요.";
                    break;
                }

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                {
                    ChkCnt++;
                }
            }

            // 선택건수 체크
            if (strCheckResultMsg == "")
            {
                if (ChkCnt == 0)
                {
                    strCheckResultMsg = "지출증빙 처리를 위하여 선택된 건이 없습니다.";
                }
            }

            // 로그인 사용자와 입력자 항목이 동일한지. 본인 전표에 대해서만 지출증빙등록 처리하도록 하기 위해 체크.
            if (strCheckResultMsg == "")
            {
                string strLoginNm = "";
                string strLoginId = "";
                strLoginNm = SystemBase.Base.gstrUserName;
                strLoginId = SystemBase.Base.gstrUserID;

                if (strLoginId != "ADMIN")
                {
                    if (strLoginNm != fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "상신자")].Text)
                    {
                        strCheckResultMsg = "로그인 사용자명과 입력자 검색조건을 동일하게 입력하여 조회한 경우에만 지출증빙 가능합니다.";
                    }
                }
            }
        }        

        #region txtInEmpNm_KeyPress(): 입력자 검색조건에서 엔터키 치면 조회 처리되도록.
        private void txtInEmpNm_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick(): 그리드 더블클릭시에도 해당 라인 전표번호의 팝업이 뜨도록 함.
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
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
        // 2022.03.21. hma 추가(End)
    }

}
