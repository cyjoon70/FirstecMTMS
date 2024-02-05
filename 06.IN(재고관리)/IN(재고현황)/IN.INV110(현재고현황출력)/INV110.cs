#region 작성정보
/*********************************************************************/
// 단위업무명 : 현재고현황출력
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-19
// 작성내용 : 현재고현황출력
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

namespace IN.INV110
{
    public partial class INV110 : UIForm.Buttons
    {
        bool form_act_chk = false;

        public INV110()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV110_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            //기타 세팅
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoN.Checked = true;

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoN.Checked = true;
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string[] RptParmValue = new string[8];

                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV110.rpt";

                RptParmValue[0] = "R1";

                RptParmValue[1] = cboPlantCd.SelectedValue.ToString();

                if (txtItemCdFr.Text.Trim() == "") RptParmValue[2] = " ";
                else RptParmValue[2] = txtItemCdFr.Text;

                if (txtItemCdTo.Text.Trim() == "") RptParmValue[3] = " ";
                else RptParmValue[3] = txtItemCdTo.Text;

                RptParmValue[4] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[5] = dtpTranDt.Text;

                if (rdoY.Checked == true)
                    RptParmValue[6] = "Y";
                else
                    RptParmValue[6] = "";
                RptParmValue[7] = SystemBase.Base.gstrCOMCD;

                //UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);

                PRINT frm = new PRINT(this.Text, null, null, RptName, RptParmValue);

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

                string RptName = "";

                //--레포트 파일 선택
                RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV110.rpt";

                if (rdoN.Checked == true) RptParmValue[0] = "R1";
                else RptParmValue[0] = "R2";

                RptParmValue[1] = cboPlantCd.SelectedValue.ToString();

                if (txtItemCdFr.Text.Trim() == "") RptParmValue[2] = " ";
                else RptParmValue[2] = txtItemCdFr.Text;

                if (txtItemCdTo.Text.Trim() == "") RptParmValue[3] = " ";
                else RptParmValue[3] = txtItemCdTo.Text;

                RptParmValue[4] = cboItemAcct.SelectedValue.ToString();
                RptParmValue[5] = dtpTranDt.Text;
                if (rdoY.Checked == true)
                    RptParmValue[6] = "Y";
                else
                    RptParmValue[6] = "";
                RptParmValue[7] = SystemBase.Base.gstrCOMCD;

                //UIForm.Print frm = new UIForm.Print(this.Text, null, null, RptName, RptParmValue);

                //UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);

                PRINT frm = new PRINT(this.Text + "출력", null, RptName, RptParmValue);

                frm.ShowDialog();

            }
        }
        #endregion

        #region 팝업창 
        private void btnItemFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdFr.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdFr.Text = Msgs[2].ToString();
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

        private void btnItemTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu1 = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdTo.Text);
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdTo.Text = Msgs[2].ToString();
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
        #endregion

        #region TextChanged
        private void txtItemCdFr_TextChanged(object sender, EventArgs e)
        {
            txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtItemCdTo_TextChanged(object sender, EventArgs e)
        {
            txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 폼 활성화/비활성화 시 변수 설정
        private void INV110_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV110_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

    }
}
