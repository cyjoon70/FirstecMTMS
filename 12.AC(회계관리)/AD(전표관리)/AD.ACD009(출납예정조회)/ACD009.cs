#region 작성정보
/*********************************************************************/
// 단위업무 :  출납예정대상조회
// 작 성 자 :  한 미 애
// 작 성 일 :  2022-03-11
// 작성내용 :  현금 계정이 대변에 있으면서 승인이 안된 건을 조회한다.
//             출납(이체) 대상건을 조회하여 처리하고자 함.
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

namespace AD.ACD009
{
    public partial class ACD009 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        public ACD009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD009_Load(object sender, System.EventArgs e)
        {
            this.Text = "출납예정조회";

            SystemBase.Validation.GroupBox_Setting(groupBox1);  //필수 적용

            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);     // 그룹웨어상태(재무팀결재상태)

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            // 결의일자는 이전월1일 ~ 현재년월 말일
            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpResSlipDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 7) + "-01";
            dtpResSlipDtTo.Value = Convert.ToDateTime(Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToString().Substring(0, 7) + "-01").AddDays(-1).ToShortDateString();

            // 회계예정일은 현재일~1주일후
            dtpGlSlipDtFr.Value = YYMMDD;
            dtpGlSlipDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(7).ToString();

            // 계정코드는 현금 계정코드로 고정하고 비활성화 처리.
            txtAcctCd.Text = "11110001";
            txtAcctCd.Enabled = false;

            cboGwStatus.SelectedValue = "ING";      // 재무팀결재상태는 결재진행으로 세팅

            strREORG_ID = SystemBase.Base.gstrREORG_ID;
        }
        #endregion

        #region NewExec(): New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);            

            // 결의일자는 이전월1일 ~ 현재년월 말일
            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpResSlipDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToString().Substring(0, 7) + "-01";
            dtpResSlipDtTo.Value = Convert.ToDateTime(Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToString().Substring(0, 7) + "-01").AddDays(-1).ToShortDateString();

            // 회계예정일은 현재일~1주일후
            dtpGlSlipDtFr.Value = YYMMDD;
            dtpGlSlipDtTo.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(7).ToShortDateString();

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
                    string strQuery = " usp_ACD009  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pRES_SLIP_DT_FROM = '" + dtpResSlipDtFr.Text + "' ";
                    strQuery += ", @pRES_SLIP_DT_TO = '" + dtpResSlipDtTo.Text + "' ";
                    strQuery += ", @pGL_SLIP_DT_FROM = '" + dtpGlSlipDtFr.Text + "' ";
                    strQuery += ", @pGL_SLIP_DT_TO = '" + dtpGlSlipDtTo.Text + "' ";
                    strQuery += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";

                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }

                    strQuery += ", @pIN_EMP_NM = '" + txtInEmpNm.Text + "' ";
                    strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "' ";
                    strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                    strQuery += ", @pGW_STATUS = '" + cboGwStatus.SelectedValue.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);        // 2022.03.15. hma 수정: 합계액 나오도록 false=>true로 변경.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region btnAcct_Click(): 계정코드 검색 버튼 클릭시. 계정코드 팝업 띄움.
        private void btnAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A033', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
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
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region btnDept_Click(): 부서코드 검색 버튼 클릭시. 부서정보 팝업 띄움.
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

        #region btnSlipView_Click(): 전표조회 버튼 클릭시 전표상세조회 팝업 띄움.
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            SlipViewPopUp();
        }
        #endregion

        #region txtDeptCd_TextChanged(): 부서코드 검색조건 입력 변경시
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

        #region txtAcctCd_TextChanged(): 계정코드 입력 변경시
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick(): 그리드 더블클릭시. 전표조회 팝업 루틴 호출
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            SlipViewPopUp();
        }
        #endregion

        #region SlipViewPopUp(): 전표조회 팝업 띄우기
        private void SlipViewPopUp()
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

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;

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
