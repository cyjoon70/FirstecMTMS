
#region 작성정보
/*********************************************************************/
// 단위업무명: 결의전표반제승인
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-01-21
// 작성내용  : 결의전표반제승인
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;

namespace AD.ACD008
{
    public partial class ACD008 : UIForm.FPCOMM2
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strREORG_ID = "";
        #endregion

        public ACD008()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD008_Load(object sender, System.EventArgs e)
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
                    string strQuery = " usp_ACD008  'S1'";
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
                        strQuery += ", @pCONFIRM_FLAG = 'Y' ";
                    }
                    else
                    {
                        strQuery += ", @pCONFIRM_FLAG = 'N' ";
                    }

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                    PreRow = -1;
                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                string strQuery = " usp_ACD008  'S2'";
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

            string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strChkFlag = "";

                //행수만큼 처리
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text != fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "이전상태")].Text)
                    {
                        string strSql = " usp_ACD008 'U1'";
                        strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pSLIP_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text + "' ";
                        if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Value.ToString() == "True")
                            strChkFlag = "Y";
                        else
                            strChkFlag = "N";
                        strSql = strSql + ", @pMINUS_CONFIRM_YN = '" + strChkFlag + "' ";
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

            this.Cursor = Cursors.Default;
        }
        #endregion
        


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
                ACD008P1 pu = new ACD008P1();
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
                ACD008P1 pu = new ACD008P1();
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
