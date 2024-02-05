#region 작성정보
/*********************************************************************/
// 단위업무명 : 원재료실소요량계산서조회(기간별)
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-10
// 작성내용 : 원재료실소요량계산서조회(기간별) 및 관리
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

namespace IBF.IBFB18U
{ 
    public partial class IBFB18U : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB18U()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFB18U_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            
            dtpDT.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region 행추가
        protected override void RowInsExe()
        {
            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text = txtTRNo.Text;

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
                    string strQuery = " usp_IBFB18U  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "'  ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

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
                        if (strHead.Length > 0 && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;   //수정
                                case "D": strGbn = "D1"; break;   //삭제
                                case "I": strGbn = "I1"; break;   //입력
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB18U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text + "'";
                            strQuery = strQuery + ", @pCHILD_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고일자")].Text + "'";
                            strQuery = strQuery + ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_QTY = " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "과세신고수량")].Value;
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                    this.Cursor = Cursors.Default;
                    SearchExec();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "P0019";
                }
            Exit:
                this.Cursor = Cursors.Default;
                dbConn.Close();
                if(MSGCode != "")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0065"));
            }
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

        private void butCompute_Click(object sender, System.EventArgs e)
        {
            if (txtNotifyNo.Text.Trim() != "")
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text.Trim() != "" && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "신고번호")].Value = dtpDT.Text;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "과세신고수량")].Value = txtNotifyNo.Text;
                        fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
                    }
                }
            }
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (fpSpread1.Sheets[0].Cells[Row, 3].Locked.ToString() == "True") return;
                this.Cursor = Cursors.WaitCursor;
                if (Column == 4)
                {
                    string strQuery = " Nusp_BF_Comm 'BF04' ";
                    string[] strWhere = new string[] { "@pValue", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[2].ToString();
                        GetUnit(Row, Column);
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

        private void GetUnit(int Row, int Col)
        {
            try
            {
                if (Col.ToString() == "3" || Col.ToString() == "4")
                {
                    string strSql = "Select ITEM_NM, ITEM_SPEC  From MTMS_FT.dbo.B_ITEM_INFO(Nolock) Where ITEM_CD = '" + fpSpread1.Sheets[0].Cells[Row, 3].Text.Trim() + "'";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목명")].Text = ds.Tables[0].Rows[0][0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[0][1].ToString();
                    }
                    strSql = " select dbo.ufn_GetItemUnit('" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text.Trim() + "') ";
                    DataSet ds1 = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = ds1.Tables[0].Rows[0][0].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region dtpDT_Leave
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

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFB18U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB18U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion 

        #region fpSpread1_Change
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
            GetUnit(e.Row, e.Column);
        }
        #endregion

    }
}
