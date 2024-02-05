

#region 작성정보
/*********************************************************************/
// 단위업무명 : 고정자산정보
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-21
// 작성내용 : 고정자산정보
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

namespace AD.ACD001
{
    public partial class ACD001P5 : UIForm.FPCOMM1
    {
        #region 변수선언
        DataTable Dt = null;
        string strCUR_CD = "";
        double dEXCH_RATE = 0;
        string strACCT_CD = "";
        double iSlipAmt = 0;
        double iSlipAmtLoc = 0;
        string strSave_YN = "N";
        #endregion

        public ACD001P5()
        {
            InitializeComponent();
        }

        public ACD001P5(DataTable Asset_Dt, string CUR_CD, double EXCH_RATE, string ACCT_CD)
        {
            Dt = Asset_Dt;
            strCUR_CD = CUR_CD;
            dEXCH_RATE = EXCH_RATE;
            strACCT_CD = ACCT_CD;
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD001P5_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("011111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "고정자산정보";

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B029', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상각방법")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'A202', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                SearchExec();
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

                string strQuery = " usp_ACD001  'P9'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, false, 0, 0);
                GRID_RESEARCH();
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")].Text == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                                                           SystemBase.Base.GridHeadIndex(GHIdx1, "전용사업명").ToString() + "|1");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                                                           SystemBase.Base.GridHeadIndex(GHIdx1, "전용사업명").ToString() + "|0");
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

                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text = strCUR_CD;
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = dEXCH_RATE.ToString();
                            fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text = strACCT_CD;

                            for (int i = 1; i < fpSpread1.Sheets[0].ColumnCount; i++)
                            {
                                if (Dt.Rows[iDtRow][i].ToString() != "")
                                    fpSpread1.Sheets[0].Cells[SelectedRow, i].Value = Dt.Rows[iDtRow][i].ToString();
                                if (G1Color[i] == 4)
                                {
                                    fpSpread1.Sheets[0].Cells[iDtRow, i].Locked = true;
                                }
                                if (strCUR_CD == "KRW")
                                {
                                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Locked = true;
                                }
                            }
                        }
                        txtDEPR_ACCT_CD.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "누계계정")].Text;
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    UIForm.FPMake.RowInsert(fpSpread1);
                    fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text;
                }
                else
                {
                    UIForm.FPMake.RowInsert(fpSpread1);
                }
                int SelectedRow = fpSpread1.Sheets[0].ActiveRowIndex;
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐단위")].Text = strCUR_CD;
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Text = dEXCH_RATE.ToString();
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text = strACCT_CD;
                fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")].Text = "N";
                if (strCUR_CD == "KRW")
                {
                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Locked = true;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                UIForm.FPMake.RowCopy(fpSpread1);
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int SelectedRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                    fpSpread1.Sheets[0].RowHeader.Cells[SelectedRow, 0].Text = "";

                    fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text = "";
                    if (strCUR_CD == "KRW")
                    {
                        fpSpread1.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Locked = true;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    if (fpSpread1.Sheets[0].Cells[iDelRow, SystemBase.Base.GridHeadIndex(GHIdx1, "자산번호")].Text == "")
                    {
                        fpSpread1.Sheets[0].Rows.Remove(iDelRow, 1);
                    }
                    else
                    {
                        UIForm.FPMake.RowRemove(fpSpread1);
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
        
        #region SaveExec() 폼에 입력된 데이타 메인 화면으로 리턴
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        if (fpSpread1.Sheets[0].Rows.Count > 0)
                        {
                            iSlipAmt = 0;
                            iSlipAmtLoc = 0;

                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "D")
                                {
                                    fpSpread1.Sheets[0].Rows[i].Remove();
                                }
                                else
                                {
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "누계계정")].Text = txtDEPR_ACCT_CD.Text;
                                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text = strACCT_CD;

                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액")].Text != "")
                                        iSlipAmt += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액")].Value);
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Text != "")
                                        iSlipAmtLoc += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Value);
                                }
                            }
                        }
                        strSave_YN = "Y";
                        Dt = ((System.Data.DataTable)(fpSpread1.Sheets[0].DataSource));
                        
                        for (int iRow = 0; iRow < fpSpread1.Sheets[0].Rows.Count; iRow++)
                        {
                            fpSpread1.Sheets[0].RowHeader.Cells[iRow, 0].Text = "";
                        }
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

        #region 누계계정 TextChanged
        private void txtDEPR_ACCT_CD_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDEPR_ACCT_NM.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtDEPR_ACCT_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y' AND ACCT_TYPE = 'Q1' ");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region 누계계정 팝업
        private void btnDEPR_ACCT_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y', @pSPEC2 = 'Q1' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtDEPR_ACCT_CD.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        txtDEPR_ACCT_CD.Value = Msgs[0].ToString();
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

        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액"))
                {
                    if (strCUR_CD != "")
                    {
                        if (strCUR_CD == "KRW")
                        {
                            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Text = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액")].Text;
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액")].Text.Replace("-", "") != "")
                            {
                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Value = Math.Round(Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액")].Text.Replace(",", ""))  * dEXCH_RATE, 0);
                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "취득금액(자)")].Text = "0";
                            }
                        }
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부"))
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "전용여부")].Text == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, e.Row,
                                                           SystemBase.Base.GridHeadIndex(GHIdx1, "전용사업명").ToString() + "|1");
                    }
                    else
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, e.Row, 
                                                           SystemBase.Base.GridHeadIndex(GHIdx1, "전용사업명").ToString() + "|0");
                    }
                }
                else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드"))
                {
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public DataTable Asset_Dt { get { return Dt; } set { Dt = value; } }
        public double SLIP_AMT { get { return iSlipAmt; } set { iSlipAmt = value; } }
        public double SLIP_AMT_LOC { get { return iSlipAmtLoc; } set { iSlipAmtLoc = value; } }

        #region FormClosed
        private void ACD001P5_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (strSave_YN == "Y")
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 버튼클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드_2"))
                {
                    WNDW.WNDW001 pu = new WNDW.WNDW001();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text = Msgs[2].ToString();
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

    }
}
