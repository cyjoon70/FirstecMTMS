

#region 작성정보
/*********************************************************************/
// 단위업무명 : 채무반제등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-19
// 작성내용 : 채무반제등록
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

namespace AD.ACD004
{
    public partial class ACD004P3 : UIForm.FPCOMM2
    {
        #region 변수선언
        DataTable Dt = null;
        string strCUR_CD = "";
        string strCustCd = "";
        string strBizAreaCd = "";
        double iSlipAmt = 0;
        double iSlipAmtLoc = 0;
        string strDeptCd = "";
        string strReorgId = "";
        string strAcctCd = "";
        string strAcctNm = "";
        #endregion

        public ACD004P3()
        {
            InitializeComponent();
        }

        public ACD004P3(DataTable Ap_Dt, string CUR_CD, string CUST_CD, string BIZ_AREA_CD, string ACCT_CD, string ACCT_NM)
        {
            Dt = Ap_Dt;
            strCUR_CD = CUR_CD;
            strCustCd = CUST_CD;
            strBizAreaCd = BIZ_AREA_CD;
            strAcctCd = ACCT_CD;
            strAcctNm = ACCT_NM;
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD004P3_Load(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.Buttons.ReButton("010001010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "채무반제";
                SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //화폐단위
                SystemBase.ComboMake.C1Combo(cboBizArea, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //사업장
                NewExec();
                if (Dt != null)
                {
                    if (Dt.Rows.Count > 0)
                        SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
                dtpApDtFrom.Text = Convert.ToDateTime(YYMMDD).AddMonths(-3).ToShortDateString();
                dtpApDtTo.Text = YYMMDD;
                txtCustCd.Text = strCustCd;
                cboCurCd.SelectedValue = strCUR_CD;
                cboBizArea.SelectedValue = strBizAreaCd;
                txtAcctCd.Value = strAcctCd;
                txtAcctNm.Value = strAcctNm;

                if (Dt != null)
                {
                    if (Dt.Rows.Count > 0)
                    {
                        if (txtCustCd.Text != "")
                        {
                            txtCustCd.Enabled = false;
                            btnCust.Enabled = false;
                        }
                        if (strCUR_CD != "")
                        {
                            cboCurCd.Enabled = false;
                        }
                    }
                }
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    string strQuery2 = " usp_ACD001  'P81'";
                    string strQuery = " usp_ACD001  'P8'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";
                    strQuery += ", @pAP_DT_FROM = '" + dtpApDtFrom.Text + "' ";
                    strQuery += ", @pAP_DT_TO = '" + dtpApDtTo.Text + "' ";
                    strQuery += ", @pCUR_CD = '" + cboCurCd.SelectedValue.ToString() + "' ";
                    //strQuery += ", @pBIZ_AREA_CD = '" + cboBizArea.SelectedValue.ToString() + "' ";
                    strQuery += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery2, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0);
                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    if (Dt != null)
                    {
                        GRID_RESEARCH();
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

        #region 텍스트 체인지
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 팝업 클릭
        //거래처팝업
        private void btnCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW002 pu = new WNDW.WNDW002(txtCustCd.Text, "PS");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 메인 화면으로 리턴
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    iSlipAmt = 0;
                    iSlipAmtLoc = 0;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count - 1; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text != "")
                            iSlipAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Value);
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text != "")
                            iSlipAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Value);

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text != "")
                            iSlipAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Value);
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text != "")
                            iSlipAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Value);
                        strReorgId = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text;
                        strDeptCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text;
                    }
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        fpSpread1.Sheets[0].Rows.Remove(fpSpread1.Sheets[0].Rows.Count - 1, 1);
                    Dt = ((System.Data.DataTable)(fpSpread1.Sheets[0].DataSource));

                    strCUR_CD = cboCurCd.SelectedValue.ToString();
                    strCustCd = txtCustCd.Text;

                    for (int iRow = 0; iRow < fpSpread1.Sheets[0].Rows.Count; iRow++)
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[iRow, 0].Text = "";
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 확인버튼 클릭
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                for (int iRow = 0; iRow < fpSpread2.Sheets[0].Rows.Count; iRow++)
                {
                    if (fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text == "True")
                    {
                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text == "합계")
                                fpSpread1.Sheets[0].SetActiveCell(fpSpread1.Sheets[0].Rows.Count - 2, SystemBase.Base.GridHeadIndex(GHIdx1, "채무번호"));
                        }
                        UIForm.FPMake.RowInsert(fpSpread1);
                        int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                        for (int i = 2; i < fpSpread2.Sheets[0].ColumnCount; i++)
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow, i].Value = fpSpread2.Sheets[0].Cells[iRow, i].Value;
                            if (G1Color[i] == 4)
                            {
                                fpSpread1.Sheets[0].Cells[SelectedRow, i].Locked = true;
                            }
                        }
                        GRID_READONLY(SelectedRow);

                        if (fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Locked = true;
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Locked = true;
                        }

                        fpSpread2.Sheets[0].Rows.Remove(iRow, 1);
                        iRow--;
                    }
                }
                GRID_SUM();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 재조회
        protected void GRID_RESEARCH()
        {
            try
            {
                if (Dt != null)
                {
                    if (Dt.Rows.Count > 0)
                    {
                        for (int iDtRow = 0; iDtRow < Dt.Rows.Count; iDtRow++)
                        {
                            UIForm.FPMake.RowInsert(fpSpread1);
                            int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                            fpSpread1.Sheets[0].RowHeader.Cells[SelectedRow, 0].Text = "";

                            for (int i = 1; i < fpSpread1.Sheets[0].ColumnCount - 1; i++)
                            {
                                if (Dt.Rows[iDtRow][i].ToString() != "")
                                    fpSpread1.Sheets[0].Cells[SelectedRow, i].Text = Dt.Rows[iDtRow][i].ToString();
                                if (G1Color[i] == 4)
                                {
                                    fpSpread1.Sheets[0].Cells[iDtRow, i].Locked = true;
                                }
                                if (fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                                {
                                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Locked = true;
                                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Locked = true;
                                }
                            }
                            GRID_READONLY(SelectedRow);

                            for (int iRow = 0; iRow < fpSpread2.Sheets[0].Rows.Count; iRow++)
                            {
                                if (Dt.Rows[iDtRow]["AP_NO"].ToString() == fpSpread2.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx2, "채무번호")].Text)
                                {
                                    fpSpread2.Sheets[0].Rows.Remove(iRow, 1);
                                    continue;
                                }
                            }
                        }
                        GRID_SUM();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DelExec 행 삭제
        protected override void DelExec()
        {	// 행 삭제
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int iDelRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                    if (fpSpread1.Sheets[0].RowHeader.Cells[iDelRow, 0].Text != "합계")
                    {

                        UIForm.FPMake.RowInsert(fpSpread2);
                        int SelectedRow = fpSpread2.ActiveSheet.ActiveRowIndex;

                        for (int i = 2; i <= SystemBase.Base.GridHeadIndex(GHIdx1, "기존비고"); i++)
                        {
                            fpSpread2.Sheets[0].Cells[SelectedRow, i].Value = fpSpread1.Sheets[0].Cells[iDelRow, i].Value;
                            if (G2Color[i] == 4)
                            {
                                fpSpread2.Sheets[0].Cells[SelectedRow, i].Locked = true;
                            }
                        }
                        fpSpread2.Sheets[0].RowHeader.Cells[SelectedRow, 0].Text = "";

                        fpSpread1.Sheets[0].Rows.Remove(iDelRow, 1);

                        if (fpSpread1.Sheets[0].Rows.Count == 1)
                            fpSpread1.Sheets[0].Rows.Remove(fpSpread1.ActiveSheet.ActiveRowIndex, 1);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 그리드 더블클릭
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                int iRow = e.Row;
                if (fpSpread2.ActiveSheet.GetSelection(0).Row > -1)
                {
                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text == "합계")
                            fpSpread1.Sheets[0].SetActiveCell(fpSpread1.Sheets[0].Rows.Count - 2, SystemBase.Base.GridHeadIndex(GHIdx1, "채무번호"));
                    }

                    UIForm.FPMake.RowInsert(fpSpread1);
                    int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                    for (int i = 2; i < fpSpread2.Sheets[0].ColumnCount; i++)
                    {
                        fpSpread1.Sheets[0].Cells[SelectedRow, i].Value = fpSpread2.Sheets[0].Cells[iRow, i].Value;
                        if (G1Color[i] == 4)
                        {
                            fpSpread1.Sheets[0].Cells[SelectedRow, i].Locked = true;
                        }
                    }
                    GRID_READONLY(SelectedRow);

                    if (fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                    {
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Locked = true;
                    }

                    fpSpread2.Sheets[0].Rows.Remove(iRow, 1);

                    GRID_SUM();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                int Column = e.Column;
                int Row = e.Row;

                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드_2"))
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { "", "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = Msgs[1].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 change
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {
                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "반제액") || Col == SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)") || Col == SystemBase.Base.GridHeadIndex(GHIdx1, "할인액") || Col == SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)"))
                {
                    double dDC_AMT = 0;
                    double dDC_AMT_LOC = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text != "")
                        dDC_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text.Replace(",", ""));
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text != "")
                        dDC_AMT_LOC = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text.Replace(",", ""));

                    double dCLS_AMT = 0;
                    double dCLS_AMT_LOC = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text != "")
                        dCLS_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text.Replace(",", ""));
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text != "")
                        dCLS_AMT_LOC = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text.Replace(",", ""));

                    double dPRE_BAL_AMT_LOC = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text != "")
                        dPRE_BAL_AMT_LOC = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", ""));

                    if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "반제액"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text == "")
                        {
                            MessageBox.Show("잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text = "";
                            return;
                        }
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text != "")
                        {
                            if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) < Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text.Replace(",", "")) + dDC_AMT)
                            {
                                MessageBox.Show("잔액보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text = "";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) - dDC_AMT;
                                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                                {
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text;
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dDC_AMT;
                                }
                                return;
                            }
                        }
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액")].Text;
                            dCLS_AMT_LOC = dCLS_AMT;

                            //반제액체크
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text == "")
                            {
                                MessageBox.Show("자국잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = "";
                                return;
                            }
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text != "")
                            {
                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) < dCLS_AMT_LOC + dDC_AMT_LOC)
                                {
                                    MessageBox.Show("잔액(자)보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = "";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dDC_AMT_LOC;
                                    return;
                                }
                            }
                            double dBAL_AMT_LOC = 0;

                            dBAL_AMT_LOC = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dCLS_AMT_LOC - dDC_AMT_LOC;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Text = Convert.ToString(dBAL_AMT_LOC);
                        }
                        double dBAL_AMT = 0;

                        dBAL_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) - dCLS_AMT - dDC_AMT;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Text = Convert.ToString(dBAL_AMT);
                    }

                    else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                        {
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text == "")
                            {
                                MessageBox.Show("자국잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = "";
                                return;
                            }
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text != "")
                            {
                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) < dCLS_AMT_LOC + dDC_AMT_LOC)
                                {
                                    MessageBox.Show("잔액(자)보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제액(자)")].Text = "";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dDC_AMT_LOC;
                                    return;
                                }
                            }
                        }

                        double dBAL_AMT_LOC = 0;

                        dBAL_AMT_LOC = dPRE_BAL_AMT_LOC - dCLS_AMT_LOC - dDC_AMT_LOC;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Text = Convert.ToString(dBAL_AMT_LOC);
                    }


                    else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "할인액"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text == "")
                        {
                            MessageBox.Show("잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text = "";
                            GRID_READONLY(Row);
                            return;
                        }
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text != "")
                        {
                            if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) < dDC_AMT + dCLS_AMT)
                            {
                                MessageBox.Show("잔액보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text = "";
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) - dCLS_AMT;
                                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                                {
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text;
                                }
                                GRID_READONLY(Row);
                                return;
                            }
                        }
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text;
                            dDC_AMT_LOC = dDC_AMT;
                            //할인액체크
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text == "")
                            {
                                MessageBox.Show("자국잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = "";
                                GRID_READONLY(Row);
                                return;
                            }
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text != "")
                            {
                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) < dDC_AMT_LOC + dCLS_AMT_LOC)
                                {
                                    MessageBox.Show("잔액(자)보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = "";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dCLS_AMT_LOC;
                                    GRID_READONLY(Row);
                                    return;
                                }
                            }
                            double dBAL_AMT_LOC = 0;

                            dBAL_AMT_LOC = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dDC_AMT_LOC - dCLS_AMT_LOC;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Text = Convert.ToString(dBAL_AMT_LOC);
                        }
                        double dBAL_AMT = 0;

                        dBAL_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액")].Text.Replace(",", "")) - dDC_AMT - dCLS_AMT;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Text = Convert.ToString(dBAL_AMT);
                        GRID_READONLY(Row);
                    }


                    else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)"))
                    {
                        if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text == "KRW")
                        {
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text == "")
                            {
                                MessageBox.Show("자국잔액이 없습니다", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = "";
                                GRID_READONLY(Row);
                                return;
                            }
                            if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text != "")
                            {
                                if (Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) < dDC_AMT_LOC + dCLS_AMT_LOC)
                                {
                                    MessageBox.Show("잔액(자)보다 많이 반제, 할인할 수 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액(자)")].Text = "";
                                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Value = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "기존잔액(자)")].Text.Replace(",", "")) - dCLS_AMT_LOC;
                                    GRID_READONLY(Row);
                                    return;
                                }
                            }
                        }
                        double dBAL_AMT_LOC = 0;

                        dBAL_AMT_LOC = dPRE_BAL_AMT_LOC - dDC_AMT_LOC - dCLS_AMT_LOC;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액(자)")].Text = Convert.ToString(dBAL_AMT_LOC);
                    }

                }

                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }



            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        protected void GRID_READONLY(int Row)
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    double dDC_AMT = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text != "")
                        dDC_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인액")].Text.Replace(",", ""));

                    double dBAL_AMT = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Text != "")
                        dBAL_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "잔액")].Text.Replace(",", ""));

                    double dCLS_AMT = 0;
                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제금액")].Text != "")
                        dCLS_AMT = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "반제금액")].Text.Replace(",", ""));

                    if (dDC_AMT > 0)
                    {
                        if (dBAL_AMT == 0 && dCLS_AMT != 0)
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = "";
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Locked = true;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Locked = true;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드_2")].Locked = true;

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Locked = true;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Locked = false;
                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드_2")].Locked = false;

                            fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].BackColor = System.Drawing.Color.FromArgb(242, 252, 254);
                        }
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Locked = true;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].Locked = true;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드_2")].Locked = true;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "할인계정코드")].BackColor = System.Drawing.Color.FromArgb(238, 238, 238);
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 합계라인 생성
        protected void GRID_SUM()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text != "합계")
                    {
                        UIForm.FPMake.RowInsert(fpSpread1);

                        fpSpread1.Sheets[0].FrozenTrailingRowCount = 1;	//하단 Column 1줄 고정

                        fpSpread1.Sheets[0].RowHeader.Cells[fpSpread1.Sheets[0].Rows.Count - 1, 0].Text = "합계";
                        fpSpread1.Sheets[0].Rows[fpSpread1.Sheets[0].Rows.Count - 1].BackColor = System.Drawing.Color.FromArgb(242, 244, 246); // .FromName("Beige");
                    }

                    for (int i = 1; i < fpSpread1.Sheets[0].ColumnCount; i++)
                    {
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].Rows.Count - 1, i].Locked = true;

                        FarPoint.Win.ComplexBorder complexBorder1 = new FarPoint.Win.ComplexBorder(new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.ThinLine, System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(100)), ((System.Byte)(100)))), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None), new FarPoint.Win.ComplexBorderSide(FarPoint.Win.ComplexBorderSideStyle.None));
                        fpSpread1.Sheets[0].Cells.Get(fpSpread1.Sheets[0].Rows.Count - 1, i).Border = complexBorder1;

                        if (fpSpread1.Sheets[0].GetCellType(fpSpread1.Sheets[0].Rows.Count - 1, i).ToString() == "NumberCellType" ||
                            fpSpread1.Sheets[0].GetCellType(fpSpread1.Sheets[0].Rows.Count - 1, i).ToString() == "PercentCellType")
                        {
                            string Str = UIForm.FPMake.IntToString(i);
                            string Area = Str + "1:" + Str + Convert.ToString(fpSpread1.Sheets[0].Rows.Count - 1);
                            FarPoint.Win.Spread.Cell r = fpSpread1.ActiveSheet.Cells[fpSpread1.Sheets[0].Rows.Count - 1, i];

                            r.Formula = "SUM(" + Area + ")";
                        }

                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread2.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
                                }
                            }
                        }
                        else
                        {
                            fpSpread2.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public DataTable Ap_Dt { get { return Dt; } set { Dt = value; } }
        public string CUR_CD { get { return strCUR_CD; } set { strCUR_CD = value; } }
        public string CUST_CD { get { return strCustCd; } set { strCustCd = value; } }
        public double SLIP_AMT { get { return iSlipAmt; } set { iSlipAmt = value; } }
        public double SLIP_AMT_LOC { get { return iSlipAmtLoc; } set { iSlipAmtLoc = value; } }
        public string REORG_ID { get { return strReorgId; } set { strReorgId = value; } }
        public string DEPT_CD { get { return strDeptCd; } set { strDeptCd = value; } }
    }
}
