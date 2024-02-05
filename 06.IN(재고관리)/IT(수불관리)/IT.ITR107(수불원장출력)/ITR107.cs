#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불원장출력
// 작 성 자 : 이  태  규
// 작 성 일 : 2013-04-16
// 작성내용 : 수불원장출력 관리
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

namespace IT.ITR107
{
    public partial class ITR107 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public ITR107()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ITR107_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[11];
                string[] FormulaFieldName = new string[3]; //formula 값
                string[] FormulaFieldValue = new string[3]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR107.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = cboPlantCd.SelectedValue.ToString();
                RptParmValue[3] = dtpTranDt.Text;
                RptParmValue[4] = cboItemAcct.SelectedValue.ToString();

                if (txtItemCdFr.Text.Trim() == "") RptParmValue[5] = " ";
                else RptParmValue[5] = txtItemCdFr.Text;

                if (txtItemCdTo.Text.Trim() == "") RptParmValue[6] = " ";
                else RptParmValue[6] = txtItemCdTo.Text;

                if (txtEnt_CD.Text.Trim() == "") RptParmValue[7] = " ";
                else RptParmValue[7] = txtEnt_CD.Text;

                if (txtProject_No.Text.Trim() == "") RptParmValue[8] = " ";
                else RptParmValue[8] = txtProject_No.Text;

                RptParmValue[9] = dtpTranDtTo.Text;
                RptParmValue[10] = SystemBase.Base.gstrCOMCD;

                if (txtEnt_NM.Text.Trim() == "") FormulaFieldValue[0] = "\"전체\"";
                else FormulaFieldValue[0] = "\"" + txtEnt_NM.Text + "\"";

                FormulaFieldName[0] = "ENT_NM";

                FormulaFieldValue[1] = "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[1] = "ITEM_ACCT_NM";

                if (txtProject_No.Text.Trim() == "") FormulaFieldValue[2] = "\"전체\"";
                else FormulaFieldValue[2] = "\"" + txtProject_No.Text + "\"";
                FormulaFieldName[2] = "PROJ_NM";

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
                string[] RptParmValue = new string[11];
                string[] FormulaFieldName = new string[3]; //formula 값
                string[] FormulaFieldValue = new string[3]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "ITR107.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = cboPlantCd.SelectedValue.ToString();
                RptParmValue[3] = dtpTranDt.Text;
                RptParmValue[4] = cboItemAcct.SelectedValue.ToString();

                if (txtItemCdFr.Text.Trim() == "") RptParmValue[5] = " ";
                else RptParmValue[5] = txtItemCdFr.Text;

                if (txtItemCdTo.Text.Trim() == "") RptParmValue[6] = " ";
                else RptParmValue[6] = txtItemCdTo.Text;

                if (txtEnt_CD.Text.Trim() == "") RptParmValue[7] = " ";
                else RptParmValue[7] = txtEnt_CD.Text;

                if (txtProject_No.Text.Trim() == "") RptParmValue[8] = " ";
                else RptParmValue[8] = txtProject_No.Text;

                RptParmValue[9] = dtpTranDtTo.Text;
                RptParmValue[10] = SystemBase.Base.gstrCOMCD;

                if (txtEnt_NM.Text.Trim() == "") FormulaFieldValue[0] = "\"전체\"";
                else FormulaFieldValue[0] = "\"" + txtEnt_NM.Text + "\"";

                FormulaFieldName[0] = "ENT_NM";

                FormulaFieldValue[1] = "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[1] = "ITEM_ACCT_NM";

                if (txtProject_No.Text.Trim() == "") FormulaFieldValue[2] = "\"전체\"";
                else FormulaFieldValue[2] = "\"" + txtProject_No.Text + "\"";
                FormulaFieldName[2] = "PROJ_NM";

                //UIForm.Print frm = new UIForm.Print(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);

            }
        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItemFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdFr.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdFr.Value = Msgs[2].ToString();
                    txtItemNmFr.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnItemTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdTo.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdTo.Value = Msgs[2].ToString();
                    txtItemNmTo.Value = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        // 사업
        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEnt_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEnt_CD.Value = Msgs[0].ToString();
                    txtEnt_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        // 프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtItemCdFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdFr.Text != "")
                {
                    txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCdTo.Text != "")
                {
                    txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNmTo.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtEnt_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEnt_CD.Text != "")
                {
                    txtEnt_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEnt_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEnt_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR107_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR107_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
