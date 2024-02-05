#region 작성정보
/*********************************************************************/
// 단위업무명 : 세금계산서출력
// 작 성 자 : 김 현근
// 작 성 일 : 2013-04-09
// 작성내용 : 세금계산서출력
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
using UIForm;
namespace SS.SSC018
{
    public partial class SSC018 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public SSC018()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void SSC018_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            mskDT_To.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region 미리보기
        private void btnConfirmOk_Click(object sender, EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[5];
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "SSC018.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = txtTaxNo.Text.Trim();
                RptParmValue[2] = mskDT_Fr.Text;
                RptParmValue[3] = mskDT_To.Text;
                RptParmValue[4] = SystemBase.Base.gstrCOMCD;

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);
                frm.ShowDialog();

            }
        }
        #endregion     

        #region 팝업창 열기(품목)
        private void btnTaxNo_Click(object sender, EventArgs e)
        {
            try
            {
                SSC018P1 frm1 = new SSC018P1();
                frm1.ShowDialog();
                if (frm1.DialogResult == DialogResult.OK)
                {
                    string Msgs = frm1.ReturnVal;
                    txtTaxNo.Text = Msgs;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Form Activated & Deactivated
        private void SSC018_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) mskDT_Fr.Focus();
        }

        private void SSC018_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
