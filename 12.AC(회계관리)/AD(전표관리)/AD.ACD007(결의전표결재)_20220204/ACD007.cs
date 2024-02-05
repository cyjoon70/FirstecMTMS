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

            SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //발생경로
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = YYMMDD;

            txtSlipAmtFr.Text = "";
            txtSlipAmtTo.Text = "";
            rdoGwStatusWait.Checked = true;     // 기본적으로 미결 상태가 조회되도록 함.

            strREORG_ID = SystemBase.Base.gstrREORG_ID;

MessageBox.Show("Load");

            txtAssignId.Value = SystemBase.Base.gstrUserID;
            Check_RollGroup();          // 관리자권한여부 체크

MessageBox.Show("Load2");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec(): New 버튼 클릭 이벤트
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

        #region SearchExec(): 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strGwStaus = "";
                    if (rdoGwStatusWait.Checked == true)
                        strGwStaus = "WAIT";
                    else if (rdoGwStatusCmpl.Checked == true)
                        strGwStaus = "COMPLETE";

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

                    //WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIP_NO);
                    ACD007P3 pu = new ACD007P3(strSLIP_NO, strAdminYn, strFinanceYn);
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

        #region fpSpread1_CellDoubleClick(): 그리드 더블클릭시 해당 전표번호에 대한 전표조회 팝업 띄워준다.
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = e.Row;     // fpSpread1.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결의번호")].Text;

                    ACD007P3 pu = new ACD007P3(strSLIP_NO, strAdminYn, strFinanceYn);
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

            if ((txtAdminRollYn.Text == "Y") || (txtFinanceDeptYn.Text == "Y"))     // 관리자 권한그룹이거나 재무팀이면 결재자 입력 가능하게.
            {
                txtAssignId.Enabled = true;
                btnAssign.Enabled = true;
            }
            else
            {
                txtAssignId.Enabled = false;
                btnAssign.Enabled = false;
            }
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
    }
}
