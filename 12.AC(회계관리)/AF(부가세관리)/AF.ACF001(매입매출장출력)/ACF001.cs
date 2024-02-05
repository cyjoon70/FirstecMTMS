

#region 작성정보
/*********************************************************************/
// 단위업무명 : 매입매출장출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-06
// 작성내용 : 매입매출장출력
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

namespace AF.ACF001
{
    public partial class ACF001 : UIForm.Buttons
    {
        public ACF001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACF001_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboVatType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //계산서유형
            dtpIssueDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpIssueDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACF001.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[9];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    if(optIOFlag_I.Checked == true) RptParmValue[2] = "I";
                    else if(optIOFlag_O.Checked == true) RptParmValue[2] = "O";
                    RptParmValue[3] = cboBizAreaCd.SelectedValue.ToString();
                    RptParmValue[4] = dtpIssueDtFr.Text;
                    RptParmValue[5] = dtpIssueDtTo.Text;
                    RptParmValue[6] = cboVatType.SelectedValue.ToString();
                    RptParmValue[7] = txtCustCdFrom.Text;
                    RptParmValue[8] = txtCustCdTo.Text;
                    
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

        #region 텍스트 체인지
        //거래처FROM
        private void txtCustCdFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNmFrom.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdFrom.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //거래처TO
        private void txtCustCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNmTo.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //거래처FROM
        private void BtnCustFrom_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCdFrom.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdFrom.Value = Msgs[1].ToString();
                    txtCustNmFrom.Value = Msgs[2].ToString();
                    txtCustCdFrom.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        //거래처TO
        private void BtnCustTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCdTo.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCdTo.Value = Msgs[1].ToString();
                    txtCustNmTo.Value = Msgs[2].ToString();
                    txtCustCdTo.Focus();
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
