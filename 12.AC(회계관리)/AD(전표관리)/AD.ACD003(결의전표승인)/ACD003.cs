

#region 작성정보
/*********************************************************************/
// 단위업무명 : 결의전표승인
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-25
// 작성내용 : 결의전표승인
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
using FarPoint.Win.Spread;

namespace AD.ACD003
{
    public partial class ACD003 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strREORG_ID = "";
        #endregion

        public ACD003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD003_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            string YYMMDD = SystemBase.Base.ServerTime("YYMMDD");
            dtpSlipDtFr.Text = YYMMDD.Substring(0, 7) + "-01";
            dtpSlipDtTo.Text = YYMMDD;
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            PreRow = -1;
            strREORG_ID = SystemBase.Base.gstrREORG_ID;
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
                    string strQuery = " usp_ACD003  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_DT_FROM = '" + dtpSlipDtFr.Text + "' ";
                    strQuery += ", @pSLIP_DT_TO = '" + dtpSlipDtTo.Text + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    strQuery += ", @pSLIP_NO_FROM = '" + txtSSlipNoFr.Text + "' ";
                    strQuery += ", @pSLIP_NO_TO = '" + txtSSlipNoTo.Text + "' ";
                    strQuery += ", @pIN_EMP_NM = '" + txtInEmpNm.Text + "' ";
                    strQuery += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "' ";
                    
                    if (optConfirm_Y.Checked == true)
                    {
                        strQuery += ", @pCONFIRM_YN = 'Y' ";
                    }
                    else
                    {
                        strQuery += ", @pCONFIRM_YN = 'N' ";
                    }

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                    PreRow = -1;
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    if(fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        if (optConfirm_Y.Checked == true)
                        {
                            fpSpread2.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].BackColor = Color.White;
                            fpSpread2.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Locked = true;
                        }
                        else
                        {
                            fpSpread2.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].BackColor = System.Drawing.Color.FromArgb(242, 252, 254); //Color.LightCyan
                            fpSpread2.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Locked = false;
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

        #region 전표현황 그리드 선택
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            try
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                if (intRow < 0)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }

                if (PreRow == intRow && PreRow != -1 && intRow != -1)   //현 Row에서 컬럼이동시는 조회 안되게
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                    return;
                }
                string strSLIP_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text;
                DETAIL_SEARCH(strSLIP_NO);
                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 전표내역 조회
        private void DETAIL_SEARCH(string SLIP_NO)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACD003  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_NO ='" + SLIP_NO + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 부서코드 TextChanged
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

        #region 부서정보 팝업
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

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((GRID_SaveCheck(fpSpread2, this.Name, "fpSpread2", true) == true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text != fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이전상태")].Text)
                        {
                            string strSql = " usp_ACD003 'U1'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pSLIP_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text + "' ";
                            strSql = strSql + ", @pSLIP_DT = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text + "' ";
                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
        
        #region FPGrid_SaveCheck - 그리드 데이타 필수항목,Length Check
        public bool GRID_SaveCheck(FarPoint.Win.Spread.FpSpread FPGrid, string FormID, string GridNM, bool Msg)
        {
            bool ChkGrid = true;
            int UpCount = 0;

            try
            {
                string Query = " usp_BAA004 'S7',@PFORM_ID='" + FormID.ToString() + "' , @PGRID_NAME='" + GridNM + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                DataTable dt = SystemBase.DbOpen.TranDataTable(Query);

                //필수입력사항 체크
                for (int i = 0; i < FPGrid.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text != fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이전상태")].Text)
                    {
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text == "True")
                        {
                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Value == null || fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text.Length == 0)
                            {
                                MessageBox.Show(Convert.ToString(i + 1) + "번째 Row의 [ " + FPGrid.Sheets[0].ColumnHeader.Cells[0, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text.ToString() + " ] 항목은 필수입력 항목입니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                FPGrid.Focus();
                                FPGrid.ActiveSheet.SetActiveCell(i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자"));
                                ChkGrid = false;
                                break;
                            }
                        }
                        UpCount++;
                    }
                    
                    if (ChkGrid == false)
                        break;
                }

                if (UpCount == 0 && Msg == true)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);//변경되거나 처리 할 자료가 없습니다.
                    ChkGrid = false;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("FPGrid_SaveCheck (그리드 필수항목 체크시 에러발생)", f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY018"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ChkGrid;
        }
        #endregion

        #region 그리드 CellClick
        private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, " "))
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
                                            fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = "";
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

                                            if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text == fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이전상태")].Text)
                                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이전전표일자")].Text;
                                            else
                                                fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "결의일자")].Text;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region 그리드 ChangeEvent
        protected virtual void fpSpread2_ChangeEvent(int Row, int Col) {

            if (Col == SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자"))
            {
                if (fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text == "")
                {
                    fpSpread2.Sheets[0].Cells[Row, 1].Text = "Flase";
                }
                else
                {
                    fpSpread2.Sheets[0].Cells[Row, 1].Text = "True";
                }
            }
        }
        private void fpSpread2_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread2_ChangeEvent(e.Row, e.Column);
        }
        #endregion
        #region 컨트롤 C+V        
        private void fpSpread2_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.Handled = true;
                    Clipboard.Clear();
                    fpSpread2.Sheets[0].ClipboardCopy(ClipboardCopyOptions.All);
                }

                if (e.Control && e.KeyCode == Keys.V)
                {
                    fpSpread2.Sheets[0].ClipboardPaste(ClipboardPasteOptions.Values);

                    // 복사된 행의 열을 구하기 위하여 클립보드 사용.

                    IDataObject iData = Clipboard.GetDataObject();

                    string strClp = (string)iData.GetData(DataFormats.Text);

                    if (strClp != "" && strClp != null && strClp.Length > 0)
                    {
                        Regex rx1 = new Regex("\r\n");
                        string[] arrData = rx1.Split(strClp.ToString());


                        int DataCount = 0;
                        if (arrData.Length > 1)
                            DataCount = arrData.Length - 1;
                        else
                            DataCount = arrData.Length;

                        if (DataCount > 0)
                        {
                            int STRow = fpSpread2.ActiveSheet.ActiveRowIndex;
                            if (STRow < 0)
                                STRow = 0;

                            int ClipRowCount = STRow + DataCount;
                            if (fpSpread2.Sheets[0].RowCount < DataCount)
                                ClipRowCount = fpSpread2.Sheets[0].RowCount - STRow;


                            for (int i = STRow; i < ClipRowCount; i++)
                            {
                                if (i < fpSpread2.Sheets[0].RowCount
                                    || fpSpread2.Sheets[0].Cells[i, fpSpread2.ActiveSheet.ActiveColumnIndex].Locked != true)
                                {
                                    if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I")
                                    {
                                        fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                                    }

                                    fpSpread2_ChangeEvent(i, fpSpread2.ActiveSheet.ActiveColumnIndex);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "Clipboard 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
        }

        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, " "))
                {
                    if (fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text == "True")
                    {
                        if(fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, " ")].Text == fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "이전상태")].Text)
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "이전전표일자")].Text;
                        else
                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "결의일자")].Text;
                    }
                    else
                    {
                        fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "전표일자")].Text = "";
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 전표조회
        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread2.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = fpSpread2.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text;

                    WNDW.WNDW026 pu = new WNDW.WNDW026(strSLIP_NO);
                    pu.ShowDialog();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (fpSpread2.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = fpSpread2.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text;

                    if (strSLIP_NO == "")
                    {
                        MessageBox.Show("전표선택 후 출력하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001.rpt";    // 레포트경로+레포트명
                        string[] RptParmValue = new string[3];   // SP 파라메타 값

                        RptParmValue[0] = "P1";
                        RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                        RptParmValue[2] = strSLIP_NO;
                        RptParmValue[3] = "T";

                        UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                        //UIForm.PRINT10 frm = new UIForm.PRINT10( this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                        frm.ShowDialog();
                    }
                }

                
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnSSlipFr_Click(object sender, EventArgs e)
        {
            try
            {
                ACD003P1 pu = new ACD003P1();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSSlipNoFr.Value = Msgs[1].ToString();
                    txtSSlipNoFr.Focus();
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnSSlipTo_Click(object sender, EventArgs e)
        {
            try
            {
                ACD003P1 pu = new ACD003P1();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSSlipNoTo.Value = Msgs[1].ToString();
                    txtSSlipNoTo.Focus();
                    SearchExec();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

    }
}
