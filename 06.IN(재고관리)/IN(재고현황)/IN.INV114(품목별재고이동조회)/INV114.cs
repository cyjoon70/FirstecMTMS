#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목별재고이동조회
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-19
// 작성내용 : 품목별재고이동조회
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

namespace IN.INV114
{
    public partial class INV114 : UIForm.FPCOMM1
    {
        #region 변수선언
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        bool form_act_chk = false;
        #endregion

        public INV114()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV114_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 9);//공장			
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            cboPlantCd.SelectedIndex = 1;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            cboPlantCd.SelectedIndex = 1;
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
                    string strQuery = " usp_INV114 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pTRAN_DT_FR  ='" + dtpTranDtFr.Text.Trim() + "'";
                    strQuery += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO_FR ='" + txtProject_No_Fr.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO_TO ='" + txtProject_No_To.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        int ItemCnt = 0;
                        double TotAmt = 0;
                        string strItemCd = "";

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (strItemCd != fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보내준품목")].Text)
                            {
                                ItemCnt++;
                            }

                            strItemCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "보내준품목")].Text;
                            TotAmt = TotAmt + Convert.ToInt32(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                        }

                        txtItemCnt.Value = ItemCnt.ToString();
                        txtTotAmt.Value = TotAmt.ToString();
                    }
                    else
                    {
                        txtItemCnt.Value = 0;
                        txtTotAmt.Value = 0;
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

        private void btnProjectFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No_Fr.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No_Fr.Text = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No_To.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No_To.Text = Msgs[3].ToString();
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
        #endregion

        private void INV114_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV114_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
