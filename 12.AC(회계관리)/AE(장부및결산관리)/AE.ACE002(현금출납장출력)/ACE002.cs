

#region 작성정보
/*********************************************************************/
// 단위업무명 : 채권상세출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-07
// 작성내용 : 채권상세출력
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

namespace AE.ACE002
{
    public partial class ACE002 : UIForm.Buttons
    {
        public ACE002()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE002_Load(object sender, System.EventArgs e)
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

            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //화폐단위

            cboCurCd.SelectedValue = "KRW";
            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = YYMMDD;
            dtpSlipDtFr.Focus();
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACE002.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[7];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = cboBizAreaCdFrom.SelectedValue.ToString();
                    RptParmValue[3] = cboBizAreaCdTo.SelectedValue.ToString();
                    RptParmValue[4] = dtpSlipDtFr.Text;
                    RptParmValue[5] = dtpSlipDtTo.Text;
                    RptParmValue[6] = cboCurCd.SelectedValue.ToString();
                    
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
        //출력구분
        private void optCUR1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optCUR1.Checked == true)
                {
                    cboCurCd.Tag = "거래통화;2;;";
                    cboCurCd.SelectedValue = "KRW";
                }
                else
                {
                    cboCurCd.Tag = "거래통화;0;;";
                }
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #endregion
        
    }
}
