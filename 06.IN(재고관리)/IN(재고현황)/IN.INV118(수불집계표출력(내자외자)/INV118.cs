#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불집계표출력(내자/외자)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-19
// 작성내용 : 수불집계표출력(내자/외자)
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
using WNDW;

namespace IN.INV118
{
    public partial class INV118 : UIForm.Buttons
    {
        bool form_act_chk = false;

        public INV118()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV118_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD");

            cboItemAcct.SelectedValue = "30";

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 7);
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[6];
                string[] FormulaFieldName = new string[2]; //formula 값
                string[] FormulaFieldValue = new string[2]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV118.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = cboPlantCd.SelectedValue.ToString();
                RptParmValue[2] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[3] = mskDT_Fr.Text;
                RptParmValue[4] = mskDT_To.Text;
                RptParmValue[5] = SystemBase.Base.gstrCOMCD;

                FormulaFieldValue[0] = "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[0] = "ITEM_ACCT_NM";

                FormulaFieldValue[1] = "\"" + cboPlantCd.Text + "\"";
                FormulaFieldName[1] = "PLANT_NM";


                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);
                frm.ShowDialog();
            }
        }
        #endregion

        #region 폼 활성화/비활성화 시 변수 설정
        private void INV118_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV118_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
