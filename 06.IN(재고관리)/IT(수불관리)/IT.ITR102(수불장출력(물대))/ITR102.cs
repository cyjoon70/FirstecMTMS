#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불장출력(물대)
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 수불장출력(물대) 및 관리
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

namespace IT.ITR102
{
    public partial class ITR102 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public ITR102()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void ITR102_Load(object sender, System.EventArgs e)
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
                string[] RptParmValue = new string[8];
                string[] FormulaFieldName = new string[2]; //formula 값
                string[] FormulaFieldValue = new string[2]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR102.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = SystemBase.Base.gstrLangCd;
                RptParmValue[3] = cboPlantCd.SelectedValue.ToString();

                if (txtItemCd.Text.Trim() == "") RptParmValue[4] = " ";
                else RptParmValue[4] = txtItemCd.Text;

                RptParmValue[5] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[6] = mskDT_Fr.Text.Replace("-", "");
                RptParmValue[7] = mskDT_To.Text.Replace("-", "");

                FormulaFieldValue[0] = "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[0] = "ACCT_NM";

                FormulaFieldValue[1] = "\"" + cboPlantCd.Text + "\"";
                FormulaFieldName[1] = "PLANT_NM";


                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);
                frm.ShowDialog();

            }
        }
        #endregion

        #region Print
        protected override void PrintExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[8];
                string[] FormulaFieldName = new string[2]; //formula 값
                string[] FormulaFieldValue = new string[2]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR102.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = SystemBase.Base.gstrLangCd;
                RptParmValue[3] = cboPlantCd.SelectedValue.ToString();

                if (txtItemCd.Text.Trim() == "") RptParmValue[4] = " ";
                else RptParmValue[4] = txtItemCd.Text;

                RptParmValue[5] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[6] = mskDT_Fr.Text.Replace("-", "");
                RptParmValue[7] = mskDT_To.Text.Replace("-", "");

                FormulaFieldValue[0] = "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[0] = "ACCT_NM";

                FormulaFieldValue[1] = "\"" + cboPlantCd.Text + "\"";
                FormulaFieldName[1] = "PLANT_NM";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);

            }
        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void ITR102_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR102_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
    }
}
