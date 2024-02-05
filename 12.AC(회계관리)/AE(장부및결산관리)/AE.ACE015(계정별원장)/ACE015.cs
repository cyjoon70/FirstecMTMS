

#region 작성정보
/*********************************************************************/
// 단위업무명 : 계정별원장
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-04
// 작성내용 : 계정별원장
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

namespace AE.ACE015
{
    public partial class ACE015 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        public ACE015()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE015_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

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
                    string strQuery = " usp_ACE015  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' "; 
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    strQuery += ", @pACCT_CD_FROM = '" + txtAcctCdFr.Text + "' ";
                    strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        //일계
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "2")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;
                        }
                        //합계
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "3")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor1;
                        }
                    }
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNmFr.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
                if (txtAcctNmTo.Text == "")
                {
                    txtAcctCdTo.Value = txtAcctCdFr.Text;
                    txtAcctNmTo.Value = txtAcctNmFr.Text;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtAcctCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNmTo.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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

        #region 버튼클릭 이벤트
        private void btnAcct_Click(object sender, EventArgs e)
        {

            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCdFr.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 1000;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCdFr.Value = Msgs[0].ToString();
                    txtAcctNmFr.Value = Msgs[1].ToString();
                    if (txtAcctNmTo.Text == "")
                    {
                        txtAcctCdTo.Value = Msgs[0].ToString();
                        txtAcctNmTo.Value = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            
        }
        private void btnAcctTo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCdTo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 1000;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCdTo.Value = Msgs[0].ToString();
                    txtAcctNmTo.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

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
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {   
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    DataTable dt;

                    string strQuery2 = " usp_ACE015P  'S1'";
                    strQuery2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery2 += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' ";
                    strQuery2 += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery2 += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    strQuery2 += ", @pACCT_CD_FROM = '" + txtAcctCdFr.Text + "' ";
                    strQuery2 += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery2 += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery2 += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    dt = SystemBase.DbOpen.NoTranDataTable(strQuery2);
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("출력중 에러가 발생했습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (dt.Rows[0][0].ToString() == "ER")
                        {
                            MessageBox.Show(dt.Rows[0][1].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        

                        string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACE015.rpt";    // 레포트경로+레포트명
                        string[] RptParmValue = new string[9];   // SP 파라메타 값

                        RptParmValue[0] = "P1";
                        RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                        RptParmValue[2] = cboBizAreaCd.SelectedValue.ToString();
                        RptParmValue[3] = dtpSlipDtFr.Text;
                        RptParmValue[4] = dtpSlipDtTo.Text;
                        if (txtDeptCd.Text != "")
                        {
                            RptParmValue[5] = strREORG_ID;
                            RptParmValue[6] = txtDeptCd.Text;
                        }
                        else
                        {
                            RptParmValue[5] = strREORG_ID;
                            RptParmValue[6] = txtDeptCd.Text;
                        }

                        RptParmValue[7] = txtAcctCdFr.Text;
                        RptParmValue[8] = txtAcctCdTo.Text;

                        UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                        //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                        frm.ShowDialog();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

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
    }
}
