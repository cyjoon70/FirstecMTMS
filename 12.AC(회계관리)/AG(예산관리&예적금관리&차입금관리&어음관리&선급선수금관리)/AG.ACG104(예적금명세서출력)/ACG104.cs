

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예적금명세서출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-13
// 작성내용 : 예적금명세서출력
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

namespace AG.ACG104
{
    public partial class ACG104 : UIForm.Buttons
    {
        public ACG104()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG104_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboAcctType, "SELECT MINOR_CD, CD_NM, 'N' FROM B_COMM_CODE(NOLOCK) WHERE COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'B017' AND MINOR_CD IN ('DP','SV') ", 9);   //통화
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //은행
            SystemBase.ComboMake.C1Combo(cboOpenStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A506'", 9);   //거래상태
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpBasicDt.Value = SystemBase.Base.ServerTime("YYMMDD");
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
                    if(optCurKrw_Y.Checked == true)
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG104_1.rpt";    // 레포트경로+레포트명
                    else if(optCurKrw_N.Checked == true)
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG104_2.rpt";    // 레포트경로+레포트명

                    string[] RptParmValue = new string[10];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = cboAcctType.SelectedValue.ToString();
                    if (optCurKrw_Y.Checked == true)
                        RptParmValue[4] = "Y";
                    else if (optCurKrw_N.Checked == true)
                        RptParmValue[4] = "N";
                    
                    RptParmValue[5] = cboBizAreaCdFrom.SelectedValue.ToString();
                    RptParmValue[6] = cboBizAreaCdTo.SelectedValue.ToString();
                    RptParmValue[7] = cboBankCd.SelectedValue.ToString();
                    RptParmValue[8] = cboOpenStatus.SelectedValue.ToString();
                    RptParmValue[9] = dtpBasicDt.Text;

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
        
    }
}
