#region 작성정보
/*********************************************************************/
// 단위업무명 : T/R별 재고이동
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-12
// 작성내용 : T/R별 재고이동 관리
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
using FarPoint.Win.Spread.CellType;

namespace IBF.IBFB17U
{
    public partial class IBFB17U : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        private string prev_tracking_no = "";
        #endregion

        #region 생성자
        public IBFB17U()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB17U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            dtpDT.Value = DateTime.Today.ToString().Substring(0, 10);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            string strQuery = "";
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    strQuery = " usp_IBFB17U  S1, ";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "', ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);


                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region 버튼 Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF28' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    //					txtSO_NO.Text = Msgs[1].ToString();		
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTRNo_TO_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF27' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pName", "@pSPEC" };
                string[] strSearch = new string[] { txtTRNo_TO.Text, txtBUSINESS_CD.Text, txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP016", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo_TO.Value = Msgs[0].ToString();

                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;

                string strQuery = " Nusp_BF_Comm 'BF22' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtItemCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butExec_Click(object sender, System.EventArgs e)
        {
            string ERRCode = "", MSGCode = "", MSGNM = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                {
                    if (prev_tracking_no == "" && prev_tracking_no != txtTRNo_FR.Text)
                    {


                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strQuery = " usp_IBFB17U 'I1' ";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo_FR.Text + "'";
                            strQuery = strQuery + ", @pTRACKING_NO_TO  = '" + txtTRNo_TO.Text + "'";
                            strQuery = strQuery + ", @pDT   = '" + dtpDT.Text + "'";
                            strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text + "'";
                            strQuery = strQuery + ", @pUNIT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text + "'";
                            strQuery = strQuery + ", @pQTY = " + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고량")].Value);
                            strQuery = strQuery + ", @pCUR  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text + "'";
                            strQuery = strQuery + ", @pPRICE  = " + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value);
                            strQuery = strQuery + ", @pDOC_AMT  = " + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                            strQuery = strQuery + ", @pLOC_AMT  = " + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);
                            strQuery = strQuery + ", @pEXCH_RATE  = " + Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value);
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            MSGNM = ds.Tables[0].Rows[0][3].ToString();
                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }
                        }
                        Trans.Commit();
                        MessageBox.Show("[" + txtTRNo_FR.Text + "]에서 [" + txtTRNo_TO.Text + "]으로 재고이동되었습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SearchExec();
                    }
                    else
                    {
                        MessageBox.Show("[" + prev_tracking_no + "]로 [재고이동취소]하고 실행하세요.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "P0019";
            }
        Exit:
            this.Cursor = Cursors.Default;
            dbConn.Close();

        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            string ERRCode = "", MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string strQuery = " usp_IBFB17U 'D1'";
                strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo_FR.Text + "'";
                strQuery = strQuery + ", @pTRACKING_NO_TO = '" + txtTRNo_TO.Text + "'";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                if (ERRCode == "ER")
                {
                    Trans.Rollback();
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Trans.Commit();
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    prev_tracking_no = "";
                    SearchExec();
                }
            }

        }
        #endregion

        #region TextChanged
        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }
                    txtTRNo_FR.Value = txtTRNo.Text.Trim();

                    strSql = " usp_IBFB17U 'S2'";
                    strSql = strSql + ", @pTRACKING_NO = '" + txtTRNo_FR.Text + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds1 = SystemBase.DbOpen.NoTranDataSet(strSql);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        txtTRNo_TO.Text = ds1.Tables[0].Rows[0][0].ToString();
                        MessageBox.Show("[" + txtTRNo_TO.Text + "]로 재고 이동되었습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        prev_tracking_no = txtTRNo_TO.Text;
                    }
                    else
                    {
                        prev_tracking_no = "";
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtTRNo_TextChanged(object sender, System.EventArgs e)
        {
            txtTRNo_FR.Value = txtTRNo.Text.Trim();
            try
            {
                if (txtTRNo.Text.Trim() != "" && txtTRNo.Text.ToString().Length >= 13)
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }
                    txtTRNo_FR.Value = txtTRNo.Text.Trim();

                    strSql = " usp_IBFB17U 'S2'";
                    strSql = strSql + ", @pTRACKING_NO = '" + txtTRNo_FR.Text + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds1 = SystemBase.DbOpen.NoTranDataSet(strSql);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        txtTRNo_TO.Text = ds1.Tables[0].Rows[0][0].ToString();
                        MessageBox.Show("[" + txtTRNo_TO.Text + "]로 재고 이동되었습니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        prev_tracking_no = txtTRNo_TO.Text;
                        SearchExec();

                    }
                    else
                    {
                        txtTRNo_TO.Text = "";
                        prev_tracking_no = "";
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpDT_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT.Text) == false)
//                {
//                    MessageBox(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT.Focus();
//                    dtpDT.SelectAll();
//                }
            }
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB17U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB17U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}








