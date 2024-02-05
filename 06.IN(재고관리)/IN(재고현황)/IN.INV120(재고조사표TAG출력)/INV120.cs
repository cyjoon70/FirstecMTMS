#region 작성정보
/*********************************************************************/
// 단위업무명 : 재고조사표TAG출력
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 재고조사표TAG출력 및 관리
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

namespace IN.INV120
{
    public partial class INV120 : UIForm.Buttons
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public INV120()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void INV120_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            mskDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            mskDT.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
        }
        #endregion

        #region 미리보기
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string[] RptParmValue = new string[9];
                    string RptName = "";

                    //--레포트 파일 선택
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV120.rpt";

                    if (rdoY.Checked == true) RptParmValue[0] = "R2";
                    else RptParmValue[0] = "R1";

                    RptParmValue[1] = cboPlantCd.SelectedValue.ToString();

                    RptParmValue[2] = cboItemAcct.SelectedValue.ToString();

                    RptParmValue[3] = mskDT.Text;

                    if (txtItemCdFr.Text.Trim() == "") RptParmValue[4] = " ";
                    else RptParmValue[4] = txtItemCdFr.Text.Trim();

                    if (txtItemCdTo.Text.Trim() == "") RptParmValue[5] = " ";
                    else RptParmValue[5] = txtItemCdTo.Text.Trim();

                    if (txtProjectNoFr.Text.Trim() == "") RptParmValue[6] = " ";
                    else RptParmValue[6] = txtProjectNoFr.Text.Trim();

                    if (txtProjectNoTo.Text.Trim() == "") RptParmValue[7] = " ";
                    else RptParmValue[7] = txtProjectNoTo.Text.Trim();
                    RptParmValue[8] = SystemBase.Base.gstrCOMCD;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);
                    frm.ShowDialog();
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }

            }
        }
        #endregion

        #region Print
        protected override void PrintExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string[] RptParmValue = new string[9];
                    string RptName = "";

                    //--레포트 파일 선택
                    RptName = SystemBase.Base.ProgramWhere + @"\Report\" + "INV120.rpt";

                    if (rdoY.Checked == true) RptParmValue[0] = "R2";
                    else RptParmValue[0] = "R1";

                    RptParmValue[1] = cboPlantCd.SelectedValue.ToString();

                    RptParmValue[2] = cboItemAcct.SelectedValue.ToString();

                    RptParmValue[3] = mskDT.Text;

                    if (txtItemCdFr.Text.Trim() == "") RptParmValue[4] = " ";
                    else RptParmValue[4] = txtItemCdFr.Text.Trim();

                    if (txtItemCdTo.Text.Trim() == "") RptParmValue[5] = " ";
                    else RptParmValue[5] = txtItemCdTo.Text.Trim();

                    if (txtProjectNoFr.Text.Trim() == "") RptParmValue[6] = " ";
                    else RptParmValue[6] = txtProjectNoFr.Text.Trim();

                    if (txtProjectNoTo.Text.Trim() == "") RptParmValue[7] = " ";
                    else RptParmValue[7] = txtProjectNoTo.Text.Trim();
                    RptParmValue[8] = SystemBase.Base.gstrCOMCD;

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, null, null, RptName, RptParmValue);
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItemFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCdFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdFr.Text = Msgs[2].ToString();
                    txtItemNmFr.Value = Msgs[3].ToString();
                    txtItemCdFr.Focus();
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
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemNmTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCdTo.Text = Msgs[2].ToString();
                    txtItemNmTo.Value = Msgs[3].ToString();
                    txtItemCdTo.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoFr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNoFr.Text = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjecTo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNoTo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNoTo.Text = Msgs[3].ToString();
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
            txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtItemCdTo_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void INV120_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV120_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
    }
}
