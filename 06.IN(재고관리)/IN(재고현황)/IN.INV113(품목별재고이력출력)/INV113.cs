#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별 재고이력조회
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-19
// 작성내용 : 품목별 재고이력조회
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

namespace IN.INV113
{
    public partial class INV113 : UIForm.Buttons
    {
        bool form_act_chk = false;

        public INV113()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV113_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장

            mskDT.Text = SystemBase.Base.ServerTime("YYMMDD");

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            mskDT.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[6];
                string[] FormulaFieldName = new string[4]; //formula 값
                string[] FormulaFieldValue = new string[4]; //formula 이름
                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV113.rpt";

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrLangCd;
                RptParmValue[2] = cboPlantCd.SelectedValue.ToString();
                RptParmValue[3] = txtItemCd.Text;
                RptParmValue[4] = mskDT.Text;
                RptParmValue[5] = SystemBase.Base.gstrLangCd;

                FormulaFieldValue[0] = "\"" + cboPlantCd.Text + "\"";
                FormulaFieldName[0] = "PLANT_NM";

                FormulaFieldValue[1] = "\"" + txtItemNm.Text + "\"";
                FormulaFieldName[1] = "ITEM_NM";

                FormulaFieldValue[2] = "\"" + txtSpec.Text + "\"";
                FormulaFieldName[2] = "ITEM_SPEC";

                FormulaFieldValue[3] = "\"" + txtUnit.Text + "\"";
                FormulaFieldName[3] = "ITEM_UNIT";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue);
                frm.ShowDialog();
            }
        }
        #endregion

        #region 팝업창 
        private void btnItem_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, this.txtItemCd.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtSpec.Value = Msgs[7].ToString();
                    txtUnit.Value = Msgs[8].ToString();
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
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            string Query = " usp_M_COMMON 'M030', @pCODE = '" + txtItemCd.Text + "', @pNAME = '" + cboPlantCd.SelectedValue.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtItemNm.Value = dt.Rows[0]["ITEM_NM"].ToString();
                txtSpec.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
                txtUnit.Value = dt.Rows[0]["ITEM_UNIT"].ToString();
            }
            else
            {
                txtItemNm.Value = "";
                txtSpec.Value = "";
                txtUnit.Value = "";
            }
        }
        #endregion

        #region 폼 활성화/비활성화 시 변수 설정
        private void INV113_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV113_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
