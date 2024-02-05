#region 작성정보
/*********************************************************************/
// 단위업무명 : 근태현황
// 작 성 자 : 김 현근
// 작 성 일 : 2013-04-09
// 작성내용 : 근태현황
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

namespace HA.HAA008
{
    public partial class HAA008 : UIForm.Buttons
    {
        #region 생성자
        public HAA008()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA008_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            txtBizCd.Text = SystemBase.Base.gstrBIZCD.ToString();
        }
        #endregion

        #region 사업장 TextChanged 이벤트
        private void txtBizCd_TextChanged(object sender, EventArgs e)
        {
            txtBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtBizCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 사업장 팝업
        private void btnBizCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pType='BIZ'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "", "" };
                string[] strSearch = new string[] { "", "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBizCd.Value = Msgs[0].ToString();
                    txtBizNm.Value = Msgs[1].ToString();
                    txtBizCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 미리보기
        private void button2_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //string[] FormulaField = new string[2];	  //formula 값			
                string RptName = "";    // 레포트경로+레포트명
                string[] RptParmValue = new string[3];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                RptName = SystemBase.Base.ProgramWhere + @"\Report\HAA008.rpt";
                RptParmValue[0] = txtBizCd.Text;
                RptParmValue[1] = dtpDate.Text.Replace("-", "");
                RptParmValue[2] = SystemBase.Base.gstrCOMCD;

                FormulaFieldValue[0] = "";
                FormulaFieldName[0] = "";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전	
                frm.ShowDialog();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
