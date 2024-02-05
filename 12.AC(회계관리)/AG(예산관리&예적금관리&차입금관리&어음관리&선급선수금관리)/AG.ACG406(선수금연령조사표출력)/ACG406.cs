

#region 작성정보
/*********************************************************************/
// 단위업무명 : 선수금연령표출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-13
// 작성내용 : 선수금연령표출력
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

namespace AG.ACG406
{
    public partial class ACG406 : UIForm.Buttons
    {
        public ACG406()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG406_Load(object sender, System.EventArgs e)
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

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpStdDt.Value = YYMMDD;
            //dtpArDtFrom.Value = YYMMDD.Substring(0,4) + "-01-01";
            //dtpArDtTo.Value = YYMMDD;
            dtpApDtFrom.Value = "";
            dtpApDtTo.Value = "";
            txtTermDay.Text = "30";
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
                    if (optPrtinP1.Checked == true)
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG406_1.rpt";    // 레포트경로+레포트명
                    }
                    else
                    {
                        RptName = SystemBase.Base.ProgramWhere + @"\Report\ACG406_2.rpt";    // 레포트경로+레포트명
                    }
                    string[] RptParmValue = new string[6];   // SP 파라메타 값

                    if (optPrtinP1.Checked == true)
                    {
                        RptParmValue[0] = "P1";
                    }
                    else
                    {
                        RptParmValue[0] = "P2";
                    }

                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    if (dtpApDtFrom.Text == "")
                    {
                        RptParmValue[2] = "1900-01-01";
                    }
                    else
                    {
                        RptParmValue[2] = dtpApDtFrom.Text;
                    }
                    if (dtpApDtTo.Text == "")
                    {
                        RptParmValue[3] = "2999-12-31";
                    }
                    else
                    {
                        RptParmValue[3] = dtpApDtTo.Text;
                    }
                    RptParmValue[4] = dtpStdDt.Text;
                    RptParmValue[5] = txtTermDay.Text.Replace(",","");
                    
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
