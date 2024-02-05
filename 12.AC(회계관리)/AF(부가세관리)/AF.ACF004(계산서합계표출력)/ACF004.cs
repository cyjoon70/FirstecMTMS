

#region 작성정보
/*********************************************************************/
// 단위업무명 : 계산서합계표출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-07
// 작성내용 : 계산서합계표출력
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

namespace AF.ACF004
{
    public partial class ACF004 : UIForm.Buttons
    {
        public ACF004()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACF004_Load(object sender, System.EventArgs e)
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
            dtpIssueDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpIssueDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            txtFiscCnt.Value = SystemBase.Base.CodeName("CO_CD", "FISC_CNT", "B_COMP_INFO", SystemBase.Base.gstrCOMCD, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

                    //을지
                    if (optPrintDiv_B.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACF004_3.rpt";    // 레포트경로+레포트명
                    }
                    else //갑지
                    {
                        if (optIotype_I.Checked == true) //매입
                        {
                            RptName = SystemBase.Base.ProgramWhere + @"\Report\ACF004_2.rpt";    // 레포트경로+레포트명
                        }
                        else //매출
                        {
                            RptName = SystemBase.Base.ProgramWhere + @"\Report\ACF004_1.rpt";    // 레포트경로+레포트명
                        }
                    }
                    
                    string[] RptParmValue = new string[9];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    if(optIotype_I.Checked == true) RptParmValue[2] = "I";
                    else RptParmValue[2] = "O";

                    if(optVatRel_Y.Checked == true) RptParmValue[3] = "Y";
                    else RptParmValue[3] = "N";

                    if(optPrintDiv_A.Checked == true) RptParmValue[4] = "A";
                    else RptParmValue[4] = "B";

                    RptParmValue[5] = cboBizAreaCd.SelectedValue.ToString();
                    RptParmValue[6] = dtpIssueDtFr.Text;
                    RptParmValue[7] = dtpIssueDtTo.Text;
                    RptParmValue[8] = txtFiscCnt.Text;

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
