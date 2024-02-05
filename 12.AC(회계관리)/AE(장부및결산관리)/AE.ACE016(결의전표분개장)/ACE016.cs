

#region 작성정보
/*********************************************************************/
// 단위업무명 : 결의전표분개장
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-04
// 작성내용 : 결의전표분개장
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

namespace AE.ACE016
{
    public partial class ACE016 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        public ACE016()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE016_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;
            SystemBase.ComboMake.C1Combo(cboCreathPath, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A101', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //전표입력경로
            SystemBase.ComboMake.C1Combo(cboConfirm_YN, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B029', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //승인상태

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            strREORG_ID = SystemBase.Base.gstrREORG_ID;
            optSlip_Type_A.AutoCheck = true;
            optSlip_Div_A.AutoCheck = true;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ACE016  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pBIZ_AREA_CD = '" + cboBizAreaCd.SelectedValue.ToString() + "' "; 
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    strQuery += ", @pCREATE_PATH = '" + cboCreathPath.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCONFIRM_YN = '" + cboConfirm_YN.SelectedValue.ToString() + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    if (optSlip_Type_3.Checked == true) strQuery += ", @pSLIP_TYPE = 'B' ";
                    else if (optSlip_Type_12.Checked == true) strQuery += ", @pSLIP_TYPE = 'A' ";

                    if (optSlip_Div_T.Checked == true) strQuery += ", @pSLIP_DIV = 'T' ";
                    else if (optSlip_Div_G.Checked == true) strQuery += ", @pSLIP_DIV = 'G' ";
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    if (cboConfirm_YN.SelectedValue.ToString() == "Y")
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자")].Text = "회계일자";
                    }
                    else
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "결의일자")].Text = "결의일자";
                    }
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        //전표계
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "2")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;
                        }
                        //일계
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "3")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor3;
                        }
                        //합계
                        else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "4")
                        {
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor1;
                        }
                    }
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼클릭 이벤트
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011();
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    strREORG_ID = Msgs[5].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        private void cboConfirm_YN_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboConfirm_YN.SelectedValue.ToString() == "Y")
                {
                    c1Label12.Text = "회계일자";
                }
                else
                {
                    c1Label12.Text = "결의일자";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        
    }
}
