

#region 작성정보
/*********************************************************************/
// 단위업무명 : 관리항목별원장
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-28
// 작성내용 : 관리항목별원장
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

namespace AE.ACE012
{
    public partial class ACE012 : UIForm.FPCOMM1 
    {
        public ACE012()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE012_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

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
                    string strQuery = " usp_ACE012  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    strQuery += ", @pBIZ_AREA_CD_FROM = '" + cboBizAreaCdFrom.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_TO = '" + cboBizAreaCdTo.SelectedValue.ToString() + "' ";
                    strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                    strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";     // 2016.07.13. hma 추가: 계정코드TO
                    strQuery += ", @pCTRL_CD = '" + txtCtrlCd.Text + "' ";
                    strQuery += ", @pCTRL_VAL = '" + txtCtrlValue.Text + "' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
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

        #region TextChanged
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
                txtCtrlCd_TextChanged(null, null);
                txtCtrlValue_TextChanged(null, null);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        //관리항목
        private void txtCtrlCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_ACE012  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";     // 2016.07.13. hma 추가: 계정코드TO
                strQuery += ", @pCTRL_CD = '" + txtCtrlCd.Text + "' ";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtCtrlNm.Value = dt.Rows[0]["CTRL_NM"].ToString();
                }
                else
                {
                    txtCtrlNm.Value = "";
                }
                txtCtrlValue_TextChanged(null, null);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        //관리항목값
        private void txtCtrlValue_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_ACE012  'S3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";     // 2016.07.13. hma 추가: 계정코드TO
                strQuery += ", @pCTRL_CD = '" + txtCtrlCd.Text + "' ";
                strQuery += ", @pCTRL_VAL = '" + txtCtrlValue.Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtCtrlValueNm.Value = dt.Rows[0]["CTRL_VAL_NM"].ToString();
                }
                else
                {
                    txtCtrlValueNm.Value = "";
                }
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
                try
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtAcctCd.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 1000;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        txtAcctCd.Value = Msgs[0].ToString();
                        txtAcctNm.Value = Msgs[1].ToString();

                        // 2016.07.13. hma 추가(Start): 계정코드TO가 공백일경우 FROM에서 선택한 값을 TO 기본값으로 넣어줌.
                        if (txtAcctCdTo.Text == "")
                        {
                            txtAcctCdTo.Value = Msgs[0].ToString();
                            txtAcctNmTo.Value = Msgs[1].ToString();
                        }
                        // 2016.07.13. hma 추가(End)
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        //관리항목
        private void btnCtrl_Click(object sender, EventArgs e)
        {
            try
            {
                // 2016.07.13. hma 수정(Start)
                //string strQuery = " usp_ACE012 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pACCT_CD = '" + txtAcctCd.Text + "' ";
                string strQuery = " usp_ACE012 @pTYPE = 'P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pACCT_CD = '" + txtAcctCd.Text + "', @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";
                // 2016.07.13. hma 수정(End)
                string[] strWhere = new string[] { "@pCTRL_CD", "@pCTRL_NM" };
                string[] strSearch = new string[] { txtCtrlCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00111", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "관리항목 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtCtrlCd.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "관리항목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //관리항목값
        private void btnCtrlValue_Click(object sender, EventArgs e)
        {
            try
            {
                // 2016.07.13. hma 수정(Start)
                //string strQuery = " usp_ACE012 @pTYPE = 'P2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pACCT_CD = '" + txtAcctCd.Text + "' , @pCTRL_CD = '" + txtCtrlCd.Text + "' , @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' , @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                string strQuery = " usp_ACE012 @pTYPE = 'P2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pACCT_CD = '" + txtAcctCd.Text + "' , @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' , @pCTRL_CD = '" + txtCtrlCd.Text + "' , @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' , @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                // 2016.07.13. hma 수정(End)
                string[] strWhere = new string[] { "@pCTRL_VAL", "@pCTRL_VAL_NM" };
                string[] strSearch = new string[] { txtCtrlValue.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00112", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "관리항목값 조회");
                pu.Width = 1000;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtCtrlValue.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "관리항목값 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        // 2016.07.13. hma 추가(Start)
        #region btnAcctTo_Click(): 계정코드TO 더블클릭시 처리. 계정코드 팝업 띄우기
        private void btnAcctTo_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtAcctCdTo_TextChanged(): 계정코드TO 내용변경시 처리. 입력된 계정코드에 대한 명세를 가져와서 명세 항목에 보여주기
        private void txtAcctCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNmTo.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
                txtCtrlCd_TextChanged(null, null);
                txtCtrlValue_TextChanged(null, null);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        // 2016.07.13. hma 추가(End)
    }
}
