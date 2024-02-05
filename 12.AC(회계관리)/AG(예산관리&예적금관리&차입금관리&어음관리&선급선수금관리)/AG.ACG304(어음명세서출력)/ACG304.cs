

#region 작성정보
/*********************************************************************/
// 단위업무명 : 어음명에서출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-19
// 작성내용 : 어음명에서출력
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

namespace AG.ACG304
{
    public partial class ACG304 : UIForm.Buttons
    {
        public ACG304()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG304_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //은행
            SystemBase.ComboMake.C1Combo(cboNoteKind, "SELECT MINOR_CD, CD_NM, 'N' FROM B_COMM_CODE(NOLOCK) WHERE MAJOR_CD IN ('A502','A507') AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //어음구분
            
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpExpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpExpDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = "";
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG304.rpt";    // 레포트경로+레포트명

                    string[] RptParmValue = new string[11];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    if (optExp.Checked == true) RptParmValue[3] = "EXP";
                    else RptParmValue[3] = "ISSUE";
                    RptParmValue[4] = cboBizAreaCdFrom.SelectedValue.ToString();
                    RptParmValue[5] = cboBizAreaCdTo.SelectedValue.ToString();
                    RptParmValue[6] = dtpExpDtFr.Text;
                    RptParmValue[7] = dtpExpDtTo.Text;
                    RptParmValue[8] = txtCustCd.Text;
                    RptParmValue[9] = cboBankCd.SelectedValue.ToString();
                    RptParmValue[10] = cboNoteKind.SelectedValue.ToString();
                    

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        

        #region TextChanged
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void optExp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optExp.Checked == true)
                {
                    c1Label3.Text = "만기일자";
                }
                else
                {
                    c1Label3.Text = "발행일자";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼 클릭
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
