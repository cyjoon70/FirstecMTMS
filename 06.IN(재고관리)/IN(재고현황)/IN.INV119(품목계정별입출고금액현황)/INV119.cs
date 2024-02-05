#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목계정별입출고금액현황
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-19
// 작성내용 : 품목계정별입출고금액현황
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

namespace IN.INV119
{
    public partial class INV119 : UIForm.FPCOMM1
    {
        #region 변수선언       
        bool form_act_chk = false;
        #endregion

        public INV119()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV119_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboItemAcct.SelectedValue = "30";
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            mskDT_Fr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            mskDT_To.Text = SystemBase.Base.ServerTime("YYMMDD");
            cboItemAcct.SelectedValue = "30";
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
                    string strQuery = " usp_INV119  'S1'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pDT_FR  ='" + mskDT_Fr.Text + "'";
                    strQuery += ", @pDT_TO  ='" + mskDT_To.Text + "'";
                    strQuery += ", @pENT_CD	='" + txtEntCd.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO	 ='" + txtProjectNo.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0) Set_CellSpan();
                    else txtTotalAmt.Value = 0;
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        private void Set_CellSpan()
        {
            int cnt0 = 1;
            int cnt1 = 1;

            for (int i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i - 1, 1].Text == fpSpread1.Sheets[0].Cells[i, 1].Text)
                {
                    cnt0++;
                    fpSpread1.Sheets[0].Cells[i - cnt0 + 1, 1].RowSpan = cnt0;
                    fpSpread1.Sheets[0].Cells[i - cnt0 + 1, 2].RowSpan = cnt0;

                    fpSpread1.Sheets[0].Cells[i - cnt0 + 1, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                    fpSpread1.Sheets[0].Cells[i - cnt0 + 1, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                    if (fpSpread1.Sheets[0].Cells[i - 1, 3].Text == fpSpread1.Sheets[0].Cells[i, 3].Text)
                    {
                        cnt1++;
                        fpSpread1.Sheets[0].Cells[i - cnt1 + 1, 3].RowSpan = cnt1;
                        fpSpread1.Sheets[0].Cells[i - cnt1 + 1, 4].RowSpan = cnt1;

                        fpSpread1.Sheets[0].Cells[i - cnt1 + 1, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[i - cnt1 + 1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                    }
                    else
                    {
                        cnt1 = 1;
                    }

                }
                else
                {
                    cnt0 = 1;
                    cnt1 = 1;
                }

                if (fpSpread1.Sheets[0].Cells[i, 1].Text == "총계")
                {
                    for (int k = 1; k < fpSpread1.Sheets[0].ColumnCount; k++)
                    {
                        fpSpread1.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor3;
                    }
                    txtTotalAmt.Value = fpSpread1.Sheets[0].Cells[i, fpSpread1.Sheets[0].ColumnCount - 1].Value;
                }
                else if (fpSpread1.Sheets[0].Cells[i, 5].Text == "소계") //소계
                {
                    for (int j = 5; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                    }
                }
                else if (fpSpread1.Sheets[0].Cells[i, 3].Text == "합계") //합계
                {
                    for (int k = 3; k < fpSpread1.Sheets[0].ColumnCount; k++)
                    {
                        fpSpread1.Sheets[0].Cells[i, k].BackColor = SystemBase.Base.gColor1;
                    }
                }
            }
        }
        #endregion

        #region 조회조건 팝업
        //프로젝트번호
        private void btnProjectNo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProjectNo.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjectNo.Text = Msgs[3].ToString();
                    txtProjectNm.Value = Msgs[4].ToString();
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
                string[] strSearch = new string[] { txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEntCd.Text = Msgs[0].ToString();
                    txtEntNm.Value = Msgs[1].ToString();
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
            txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }       
        //품목코드 사업
        private void txtEnt_CD_TextChanged(object sender, EventArgs e)
        {
            txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void INV119_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) mskDT_Fr.Focus();
        }

        private void INV119_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
