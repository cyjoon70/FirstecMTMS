

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예적금입출내역출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-15
// 작성내용 : 예적금입출내역출력
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

namespace AG.ACG105
{
    public partial class ACG105 : UIForm.Buttons
    {
        public ACG105()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG105_Load(object sender, System.EventArgs e)
        {
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //은행
            SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //통화
            SystemBase.ComboMake.C1Combo(cboAcctPart, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B018', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //예적금유형

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpSlipDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(-1).ToShortDateString();
            dtpSlipDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG105.rpt";    // 레포트경로+레포트명

                    string[] RptParmValue = new string[8];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = SystemBase.Base.gstrLangCd;
                    RptParmValue[3] = dtpSlipDtFr.Text;
                    RptParmValue[4] = dtpSlipDtTo.Text;
                    RptParmValue[5] = cboBankCd.SelectedValue.ToString();
                    RptParmValue[6] = cboCurCd.SelectedValue.ToString();
                    RptParmValue[7] = cboAcctPart.SelectedValue.ToString();

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
