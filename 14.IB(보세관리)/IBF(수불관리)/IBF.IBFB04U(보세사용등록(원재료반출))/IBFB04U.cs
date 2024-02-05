#region 작성정보
/*********************************************************************/
// 단위업무명 : 보세사용등록(원재료반출)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-06-10
// 작성내용 : 보세사용등록(원재료반출) 및 관리
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
using System.Data.SqlClient;
using WNDW;

namespace IBF.IBFB04U
{
    public partial class IBFB04U : UIForm.FPCOMM2
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB04U()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFB04U_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            //기타 세팅	
            dtpDT.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;

            //기타 세팅	
            dtpDT.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion	
                
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strQuery = " usp_IBFB04U  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pUSE_CREATE_NO = '" + txtBASED_NO.Text + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
                    UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 2, false);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(SystemBase.Base.GridHeadIndex(GHIdx1, "품번"), true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0) Spread_Relock();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }
            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }

        private void Spread_Relock()
        {
            decimal amt = 0;
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원재료출고")].Text.Trim() == "Y")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].BackColor = Color.Gainsboro;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Locked = true;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].CanFocus = false;
                }

                amt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);

            }
            txtAMT.Value = amt;
        }
        #endregion

        #region DeleteExe 전체 삭제로직
        protected override void DeleteExec()
        {
            try
            {
                if (fpSpread1.Sheets[0].Rows.Count <= 0) return;

                if (MessageBox.Show(SystemBase.Base.MessageRtn("P0003"), "DELETE", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strSql = " usp_IBFB04U  'D2',";
                    strSql = strSql + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strSql = strSql + " @pUSE_CREATE_NO = '" + txtBASED_NO.Text + "' ";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    string MSGCode = SystemBase.DbOpen.TranNonQuery(strSql, "P0010");
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

                    this.Cursor = Cursors.Default;

                    SearchExec();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion
        
        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false)) // 그리드 상단 필수항목 체크
            {
                string ERRCode, MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;   //수정
                                case "D": strGbn = "D1"; break;   //삭제
                                case "I": strGbn = "I1"; break;   //입력
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB04U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No")].Text + "'";
                            strQuery = strQuery + ", @pSO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text + "'";
                            strQuery = strQuery + ", @pSO_SEQ = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text;
                            strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text + "'";
                            strQuery = strQuery + ", @pSO_SUM_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총수주수량")].Value;					
                            strQuery = strQuery + ", @pIN_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출수량")].Value;
                            strQuery = strQuery + ", @pSO_UNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value != null)
                                strQuery = strQuery + ", @pSO_PRICE = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;
                            else
                                strQuery = strQuery + ", @pSO_PRICE = " + 0;
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value != null)
                                strQuery = strQuery + ", @pNET_AMT = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value;
                            else
                                strQuery = strQuery + ", @pNET_AMT = " + 0;
                            strQuery = strQuery + ", @pDLVY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text + "'";
                            strQuery = strQuery + ", @pUSE_CREATE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실소요량생성번호")].Text + "'";
                            strQuery = strQuery + ", @pUSE_CHECK_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용일자")].Text + "'";
                            strQuery = strQuery + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            else { int intRows = fpSpread_ReType(fpSpread1, strGbn, i); i = intRows; }

                        }
                    }

                    Trans.Commit();
                    SearchExec();
                    this.Cursor = Cursors.Default;
                }
                catch
                {
                    this.Cursor = Cursors.Default;
                    Trans.Rollback();
                    MSGCode = "P0019";
                }
            Exit:
                this.Cursor = Cursors.Default;
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            }
        }
        #endregion

        #region 행쿼리후 그리드 재정의
        private static int fpSpread_ReType(FarPoint.Win.Spread.FpSpread baseGrid, string strGbn, int intRow)
        {
            if (strGbn == "U1")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                return intRow;
            }
            else if (strGbn == "I1")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                UIForm.FPMake.grdReMake(baseGrid, intRow, "1|3");
                return intRow;
            }
            else if (strGbn == "D1")
            {
                baseGrid.Sheets[0].Rows[intRow].Remove();
                return intRow - 1;
            }
            else return 0;
        }
        #endregion

        #region RowInsExec 행 삭제, 추가
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Locked.ToString() == "True") return;
                this.Cursor = Cursors.WaitCursor;
                if (Column == 5)
                {
                    string strQuery = " Nusp_BF_Comm 'BF04' ";
                    string[] strWhere = new string[] { "@pValue", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, 6].Text = Msgs[1].ToString();

                    }
                }
                this.Cursor = Cursors.Default;

            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion
            
        #region fpSpread1_CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            Seach_Sub(e.Row);
        }

        private void Seach_Sub(int Row)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strQuery = " usp_IBFB04U  'S2' ";
                strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No")].Text + "'";
                strQuery = strQuery + ", @pUSE_CREATE_NO = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "실소요량생성번호")].Text + "' ";
                strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text + "' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 2);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (fpSpread2.Sheets[0].Rows.Count > 0) Spread_ForeColor_Set();
            this.Cursor = Cursors.Default;
        }

        private void Spread_ForeColor_Set()
        {
            for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "과부족수량")].Value) > 0)
                {
                    fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "과부족수량")].ForeColor = Color.Red;
                }
            }
        }
        #endregion

        #region fpSpread1_EditChange
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column.ToString() == "12" || e.Column.ToString() == "14")
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, 12].Value) * Convert.ToDecimal(fpSpread1.Sheets[0].Cells[e.Row, 14].Value);

        }
        #endregion

        #region TextBox event
        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From MTMS_FT.dbo.UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }

        }

        private void dtpDT_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT.Text.Trim() != "")
            {
                if (IsDate(dtpDT.Text) == false)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
                    dtpDT.Focus();
                    dtpDT.SelectAll();
                }
            }
        }

        public static bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        } 

        private void txtTRNo_TextChanged(object sender, System.EventArgs e)
        {
            txtBASED_NO.Value = "";
        }      

        private void txtBASED_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region Button Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Value = Msgs[0].ToString();
                    txtSO_NO.Value = Msgs[1].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void butBASED_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF19' ";
                string[] strWhere = new string[] { "@pSPEC" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP013", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원재료실소요량 근거번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBASED_NO.Value = Msgs[2].ToString();

                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void btnAllSelect_Click(object sender, System.EventArgs e)
        {

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Locked == false) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
            }
        }

        private void btnAllCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Locked == false) fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
            }
        }

        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
            }

            string ERRCode, MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                    {
                        string strQuery = " usp_IBFB04U 'P1' ";
                        strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No")].Text + "'";
                        strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text + "'";
                        strQuery = strQuery + ", @pUSE_CREATE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실소요량생성번호")].Text + "'";
                        strQuery = strQuery + ", @pUSE_CHECK_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용일자")].Text + "'";
                        strQuery = strQuery + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                        strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    }
                }

                Trans.Commit();

            }
            catch
            {
                Trans.Rollback();
                MSGCode = "P0019";
            }

        Exit:
            this.Cursor = Cursors.Default;
            dbConn.Close();
            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            SearchExec();
        }

        private void butCompute_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용일자")].Text = dtpDT.Text;
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
            }
        }
        #endregion 
        
        #region Form Activated & Deactivated
        private void IBFB04U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB04U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion
                
    }
}
