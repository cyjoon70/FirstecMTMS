#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불유형별수불장출력(물대)
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 수불유형별수불장출력(물대) 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace IT.ITR104
{
    public partial class ITR104 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public ITR104()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void ITR104_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0, 7);
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
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
                string[] RptParmValue = new string[7];
                //				string[] FormulaFieldName = new string[1]; //formula 값
                //				string[] FormulaFieldValue = new string[1]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR104.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = cboPlantCd.SelectedValue.ToString();

                RptParmValue[3] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[4] = mskDT_Fr.Text.Replace("-", "");
                RptParmValue[5] = mskDT_To.Text.Replace("-", "");
                RptParmValue[6] = SystemBase.Base.gstrCOMCD;

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);
                frm.ShowDialog();

            }
        }
        #endregion

        #region Print
        protected override void PrintExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[7];
                //				string[] FormulaFieldName = new string[1]; //formula 값
                //				string[] FormulaFieldValue = new string[1]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR104.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = cboPlantCd.SelectedValue.ToString();

                RptParmValue[3] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[4] = mskDT_Fr.Text.Replace("-", "");
                RptParmValue[5] = mskDT_To.Text.Replace("-", "");
                RptParmValue[6] = SystemBase.Base.gstrCOMCD;

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);

            }
        }
        #endregion

        private void ITR104_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR104_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
    }
}
