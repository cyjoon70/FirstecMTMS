

#region 작성정보
/*********************************************************************/
// 단위업무명 : 계정별보조부조회(집계)
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-28
// 작성내용 : 계정별보조부조회(집계)
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

namespace AE.ACE011
{
    public partial class ACE011 : UIForm.FPCOMM1 
    {
        public ACE011()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE011_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboBizAreaCdFrom, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
            SystemBase.ComboMake.C1Combo(cboBizAreaCdTo, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장

            SystemBase.ComboMake.C1Combo(cboSummary1, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A410', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //Summary #1
            SystemBase.ComboMake.C1Combo(cboSummary2, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A410', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //Summary #2
            SystemBase.ComboMake.C1Combo(cboSummary3, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A410', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //Summary #3
            SystemBase.ComboMake.C1Combo(cboSummary4, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A410', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //Summary #4
            SystemBase.ComboMake.C1Combo(cboSummary5, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A410', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //Summary #5

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD").ToString()).AddMonths(-1).ToShortDateString();
            dtpSlipDtTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Text = "1";
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Visible = false;
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Text = "2";
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Visible = false;
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Text = "3";
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Visible = false;
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Text = "4";
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Visible = false;
            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Text = "5";
            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Visible = false;
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
                    if (cboSummary1.SelectedValue.ToString() +
                        cboSummary2.SelectedValue.ToString() +
                        cboSummary3.SelectedValue.ToString() +
                        cboSummary4.SelectedValue.ToString() +
                        cboSummary5.SelectedValue.ToString() == "")
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("집계조건중 한개이상 입력하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboSummary1.Focus();
                        return;
                    }
                    if (cboSummary1.SelectedValue.ToString() != "")
                    {
                        if (cboSummary1.SelectedValue.ToString() == cboSummary2.SelectedValue.ToString() ||
                            cboSummary1.SelectedValue.ToString() == cboSummary3.SelectedValue.ToString() ||
                            cboSummary1.SelectedValue.ToString() == cboSummary4.SelectedValue.ToString() ||
                            cboSummary1.SelectedValue.ToString() == cboSummary5.SelectedValue.ToString())
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("같은 집계조건이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSummary1.Focus();
                            return;
                        }
                    }
                    if (cboSummary2.SelectedValue.ToString() != "")
                    {
                        if (cboSummary2.SelectedValue.ToString() == cboSummary3.SelectedValue.ToString() ||
                            cboSummary2.SelectedValue.ToString() == cboSummary4.SelectedValue.ToString() ||
                            cboSummary2.SelectedValue.ToString() == cboSummary5.SelectedValue.ToString() )
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("같은 집계조건이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSummary2.Focus();
                            return;
                        }
                    }
                    if (cboSummary3.SelectedValue.ToString() != "")
                    {
                        if (cboSummary3.SelectedValue.ToString() == cboSummary4.SelectedValue.ToString() ||
                            cboSummary3.SelectedValue.ToString() == cboSummary5.SelectedValue.ToString())
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("같은 집계조건이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSummary3.Focus();
                            return;
                        }
                    }
                    if (cboSummary4.SelectedValue.ToString() != "")
                    {
                        if (cboSummary4.SelectedValue.ToString() == cboSummary5.SelectedValue.ToString())
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show("같은 집계조건이 있습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboSummary4.Focus();
                            return;
                        }
                    }

                    string strQuery = " usp_ACE011  'S2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pACCT_CD_FROM = '" + txtAcctCdFrom.Text + "' ";
                    strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";
                    strQuery += ", @pSUMMARY1 = '" + cboSummary1.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY2 = '" + cboSummary2.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY3 = '" + cboSummary3.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY4 = '" + cboSummary4.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY5 = '" + cboSummary5.SelectedValue.ToString() + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() != "OK")
                        {
                            this.Cursor = Cursors.Default;
                            MessageBox.Show(dt.Rows[0][1].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtAcctCdFrom.Focus();
                            return;
                        }
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show("조회중 예기치 못한 에러가 발생했습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtAcctCdFrom.Focus();
                        return;
                    }

                    strQuery = " usp_ACE011  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    strQuery += ", @pACCT_CD_FROM = '" + txtAcctCdFrom.Text + "' ";
                    strQuery += ", @pACCT_CD_TO = '" + txtAcctCdTo.Text + "' ";
                    strQuery += ", @pBIZ_AREA_CD_FROM = '" + cboBizAreaCdFrom.SelectedValue.ToString() + "' ";
                    strQuery += ", @pBIZ_AREA_CD_TO = '" + cboBizAreaCdTo.SelectedValue.ToString() + "' ";


                    strQuery += ", @pSUMMARY1 = '" + cboSummary1.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY2 = '" + cboSummary2.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY3 = '" + cboSummary3.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY4 = '" + cboSummary4.SelectedValue.ToString() + "' ";
                    strQuery += ", @pSUMMARY5 = '" + cboSummary5.SelectedValue.ToString() + "' ";


                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (cboSummary1.SelectedValue.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Visible = true;
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Text = cboSummary1.SelectedText;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Text = "1";
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Visible = false;
                        }

                        if (cboSummary2.SelectedValue.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Visible = true;
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Text = cboSummary2.SelectedText;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Text = "2";
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Visible = false;
                        }
                        if (cboSummary3.SelectedValue.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Visible = true;
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Text = cboSummary3.SelectedText;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Text = "3";
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Visible = false;
                        }

                        if (cboSummary4.SelectedValue.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Visible = true;
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Text = cboSummary4.SelectedText;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Text = "4";
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Visible = false;
                        }

                        if (cboSummary5.SelectedValue.ToString() != "")
                        {
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Visible = true;
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Text = cboSummary5.SelectedText;
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Text = "5";
                            fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Visible = false;
                        }
                    }
                    else
                    {
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Text = "1";
                        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "1")].Visible = false;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Text = "2";
                        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "2")].Visible = false;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Text = "3";
                        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "3")].Visible = false;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Text = "4";
                        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "4")].Visible = false;
                        fpSpread1.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Text = "5";
                        fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "5")].Visible = false;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region TextChanged
        private void txtAcctCdFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNmFrom.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCdFrom.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAcctCdTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNmTo.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼클릭 이벤트
        private void btnAcctFrom_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtAcctCdFrom.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 1000;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        txtAcctCdFrom.Value = Msgs[0].ToString();
                        txtAcctNmFrom.Value = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnAcctTo_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtAcctCdTo.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 1000;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        txtAcctCdTo.Value = Msgs[0].ToString();
                        txtAcctNmTo.Value = Msgs[1].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


    }
}
