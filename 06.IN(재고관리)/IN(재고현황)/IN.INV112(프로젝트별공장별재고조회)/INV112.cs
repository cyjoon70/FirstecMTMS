#region 작성정보
/*********************************************************************/
// 단위업무명 : 프로젝트별고장별재고조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-19
// 작성내용 : 프로젝트별고장별재고조회
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

namespace IN.INV112
{
    public partial class INV112 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public INV112()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV112_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoN.Checked = true;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            rdoN.Checked = true;
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    string strQuery = " usp_INV112 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pTRAN_DT  ='" + dtpTranDt.Text + "'";
                    strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    if (rdoY.Checked == true)
                        strQuery += ", @pPOSITIVE_YN ='Y'";
                    //					else if(rdoQty_YN_P.Checked == true)
                    //						strQuery += ", @pQTY_YN_P ='Y'";
                    //					strQuery += ", @pTOPCOUNT ='"+ AddRow +"'";

                    if (rdoYes.Checked == true)
                        strQuery += ", @pCLOSE_YN ='Y'";
                    else if (rdoNo.Checked == true)
                        strQuery += ", @pCLOSE_YN ='N'";

                    if (rdoSl.Checked == true)
                        strQuery += ", @pSL_YN ='Y'";
                    else if (rdoPl.Checked == true)
                        strQuery += ", @pSL_YN ='N'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        strQuery = " usp_INV112 'S2'";
                        strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                        strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                        strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                        strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                        strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                        strQuery += ", @pTRAN_DT  ='" + dtpTranDt.Text + "'";
                        strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                        strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        if (rdoY.Checked == true)
                            strQuery += ", @pPOSITIVE_YN ='Y'";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        if (dt.Rows.Count > 0) txtSum.Value = dt.Rows[0][0];
                        else txtSum.Value = "";

                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 조회조건 팝업
        //품목코드 from
        private void btnItemFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu1 = new WNDW001(txtItemCdFr.Text, cboItemAcct.SelectedValue.ToString());
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdFr.Text = Msgs[1].ToString();
                    txtItemNmFr.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //품목코드 to
        private void btnItemTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu1 = new WNDW001(txtItemCdTo.Text, cboItemAcct.SelectedValue.ToString());
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdTo.Text = Msgs[1].ToString();
                    txtItemNmTo.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //프로젝트번호
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Text = Msgs[3].ToString();
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
        //사업
        private void btnEnt_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEnt_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEnt_CD.Text = Msgs[0].ToString();
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
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 from
        private void txtItemCdFr_TextChanged(object sender, EventArgs e)
        {
            txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 to
        private void txtItemCdTo_TextChanged(object sender, EventArgs e)
        {
            txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 사업
        private void txtEnt_CD_TextChanged(object sender, EventArgs e)
        {
            txtEnt_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEnt_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void INV112_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpTranDt.Focus();
        }

        private void INV112_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
