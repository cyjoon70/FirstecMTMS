

#region 작성정보
/*********************************************************************/
// 단위업무명 : 차입금현황조회
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-15
// 작성내용 : 차입금현황조회
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

namespace AG.ACG202
{
    public partial class ACG202 : UIForm.FPCOMM1 
    {
        public ACG202()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG202_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboLoanType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A504', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //차입금종류
            SystemBase.ComboMake.C1Combo(cboLoanUseCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A501', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //차입용도
            SystemBase.ComboMake.C1Combo(cboCurcd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //통화
            SystemBase.ComboMake.C1Combo(cboConfirm, "SELECT 'N' CODE, '미승인' NAME, 'N' UNION SELECT 'Y', '승인', 'N' ", 9);      //승인상태
            SystemBase.ComboMake.C1Combo(cboEndYn, "SELECT 'N' CODE, '미상환' NAME, 'N' UNION SELECT 'Y', '상환완료', 'N' ", 9);      //진행상황

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpOpenDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpOpenDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            dtpBaseDt.Text = SystemBase.Base.ServerTime("YYMMDD");

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
                    string strQuery = " usp_ACG202  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pOPEN_DT_FROM = '" + dtpOpenDtFr.Text + "' ";
                    strQuery += ", @pOPEN_DT_TO = '" + dtpOpenDtTo.Text + "' ";
                    strQuery += ", @pBASE_DT = '" + dtpBaseDt.Text + "' ";
                    strQuery += ", @pEXP_DT_FROM = '" + dtpExpDtFr.Text + "' ";
                    strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "' ";
                    strQuery += ", @pBIZ_AREA_CD_FROM = '" + cboBizAreaCdFrom.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_TO = '" + cboBizAreaCdTo.SelectedValue.ToString() + "' ";
                    strQuery += ", @pLOAN_TYPE = '" + cboLoanType.SelectedValue.ToString() + "' ";
                    strQuery += ", @pLOAN_USE_CD = '" + cboLoanUseCd.SelectedValue.ToString() + "' ";
                    if (optBank.Checked == true)
                        strQuery += ", @pLOAN_DIV = 'BK' ";
                    else if (optCust.Checked == true)
                        strQuery += ", @pLOAN_DIV = 'BP' ";

                    strQuery += ", @pLOAN_BANK_CUST_CD = '" + txtBankCustCd.Text + "' ";
                    strQuery += ", @pCUR_CD = '" + cboCurcd.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCONFIRM_YN = '" + cboConfirm.SelectedValue.ToString() + "' ";
                    strQuery += ", @pREFUND_END_YN = '" + cboEndYn.SelectedValue.ToString() + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged 이벤트
        private void txtBankCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (optCust.Checked == true)
                {
                    txtBankCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (optBank.Checked == true)
                {
                    txtBankCustNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 차입처버튼 클릭
        private void btnBankCust_Click(object sender, EventArgs e)
        {
            if (optCust.Checked == true)
            {
                try
                {
                    WNDW.WNDW002 pu = new WNDW.WNDW002(txtBankCustCd.Text, "PS");
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtBankCustCd.Text = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else if (optBank.Checked == true)
            {
                try
                {
                    string strBANK_CD = txtBankCustCd.Text;

                    string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = 'BK' ";
                    string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                    string[] strSearch = new string[] { txtBankCustCd.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 조회");
                    pu.Width = 800;
                    pu.Height = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtBankCustCd.Text = Msgs[0].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "은행 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        private void optALL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optALL.Checked == true)
                {
                    txtBankCustCd.Tag = ";2;;";
                    btnBankCust.Tag = ";2;;";
                    SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

                    txtBankCustCd.Value = "";
                    txtBankCustNm.Value = "";
                }
                else
                {
                    txtBankCustCd.Tag = ";;;";
                    btnBankCust.Tag = ";;;";
                    SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void optBank_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optBank.Checked == true)
                {
                    txtBankCustCd_TextChanged(null, null);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void optCust_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optCust.Checked == true)
                {
                    txtBankCustCd_TextChanged(null, null);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
