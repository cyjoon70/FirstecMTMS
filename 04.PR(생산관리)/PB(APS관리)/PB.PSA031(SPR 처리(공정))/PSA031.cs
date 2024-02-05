#region 작성정보
/*********************************************************************/
// 단위업무명 : SPR 처리(공정)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-04
// 작성내용 : SPR 처리(공정) 및 관리
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

namespace PB.PSA031
{
    public partial class PSA031 : UIForm.FPCOMM1
    {
        public PSA031()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA031_Load(object sender, System.EventArgs e)
        {

            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "품목계정")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B036',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "조달구분")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'B011',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");

            SystemBase.ComboMake.C1Combo(cboSCH_ID, "usp_P_COMMON 'P520',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
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

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_PSA031 'S1', ";
                    strQuery += " @pPROJECT_NO = '" + txtPROJ_NO.Text + "', ";
                    strQuery += " @pPROJECT_SEQ = '" + txtMAKE_NO.Text + "', ";
                    strQuery += " @pGROUP_CD = '" + txtITEM_CD.Text + "', ";
                    strQuery += " @pSCH_ID = '" + cboSCH_ID.SelectedValue.ToString() + "', ";
                    strQuery += " @pMAKEORDER_NO = '" + txtMakeorderNo.Text + "'";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 1, false, true);

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT_ORDER")].Text == "L")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번") + "|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번") + "|0");
                        }

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이전자원코드")].Text != ""
                            && fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text
                            != fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이전자원코드")].Text)
                        {
                            for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                            {
                                fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 조회조건팝업
        //품목코드
        private void btnITEM_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtITEM_CD.Text = Msgs[2].ToString();
                    txtITEM_NM.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //프로젝트번호
        private void btn_PROJ_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtPROJ_NO.Text, "S1", "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtPROJ_NO.Text = Msgs[3].ToString();
                    txtPROJ_NM.Value = Msgs[4].ToString();
                    txtMAKE_NO.Text = Msgs[5].ToString();
                    txtITEM_CD.Text = Msgs[6].ToString();
                    txtITEM_NM.Value = Msgs[7].ToString();
                    txtMakeorderNo.Text = Msgs[13].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //제품오더번호
        private void btnMakeorderNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW008 pu = new WNDW008(txtMakeorderNo.Text, "R");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtMakeorderNo.Text = Msgs[1].ToString();
                    txtPROJ_NO.Text = Msgs[6].ToString();
                    txtPROJ_NM.Value = Msgs[7].ToString();
                    txtMAKE_NO.Text = Msgs[8].ToString();
                    txtITEM_CD.Text = Msgs[9].ToString();
                    txtITEM_NM.Value = Msgs[10].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제품오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtPROJ_NO_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPROJ_NO.Text != "")
                {
                    txtPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJ_NO.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtPROJ_NM.Value = "";
                }
                if (txtPROJ_NM.Text == "")
                {
                    txtMAKE_NO.Text = "";
                    txtITEM_CD.Text = "";
                    txtMakeorderNo.Text = "";
                }
            }
            catch
            {

            }
        }

        //품목코드
        private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtITEM_CD.Text != "")
                {
                    txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtITEM_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region btnSPR_Click
        private void btnSPR_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("P0013"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text.ToString() == "True")
                        {
                            string QUERY = " usp_PSA031 @pType = 'U1', ";
                            QUERY += " @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROJECT_NO")].Text + "', ";
                            QUERY += " @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROJECT_SEQ")].Text + "', ";
                            QUERY += " @pGROUP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "GROUP_CD")].Text + "', ";
                            QUERY += " @pPRNT_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PRNT_ITEM_CD")].Text + "', ";
                            QUERY += " @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PLANT_CD")].Text + "', ";
                            QUERY += " @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', ";
                            QUERY += " @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT_NO")].Text + "', ";
                            QUERY += " @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "', ";
                            QUERY += " @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROC_SEQ")].Text + "', ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text == "0" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text == "")
                            {
                                ERRCode = "ER";
                                MSGCode = "P0040";
                                Trans.Rollback(); goto Exit;
                            }
                            QUERY += " @pMFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text + "', ";
                            QUERY += " @pUP_KIND = 'OK', ";
                            QUERY += " @pSCH_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "스케쥴 ID")].Text + "', ";
                            QUERY += " @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "WORKORDER_NO")].Text + "', ";
                            QUERY += " @pWORKORDER_NO_RS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "WORKORDER_NO_RS")].Text + "' ";
                            QUERY += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(QUERY, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                    SearchExec();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "B0015";
                }
            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            }
        }

        #endregion

        #region fpSpread1_ButtonClicked
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (ckAuto.Checked == true)
            {
                ////////////////////////////// 선택한 FIG NO 하단 자동체크 //////////////////////////////////////
                int TmpRow = 0;
                int TmpColunm = 0;
                if (fpSpread1.ActiveSheet.GetSelection(0) == null)
                {
                    TmpRow = fpSpread1.ActiveSheet.ActiveRowIndex;
                    TmpColunm = fpSpread1.ActiveSheet.ActiveColumnIndex;
                }
                else
                {
                    TmpRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
                    TmpColunm = fpSpread1.ActiveSheet.GetSelection(0).Column;
                }
                string FIGNO = fpSpread1.Sheets[0].Cells[TmpRow, SystemBase.Base.GridHeadIndex(GHIdx1, "FIG NO")].Value.ToString();

                for (int j = TmpRow; j < fpSpread1.Sheets[0].Rows.Count; j++)
                {

                    if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT_ORDER")].Text != "L")
                    {
                        if (fpSpread1.Sheets[0].Cells[TmpRow, TmpColunm].Text == "True")
                        {
                            if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "FIG NO")].Value.ToString().Length >= FIGNO.Length &&
                                FIGNO == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "FIG NO")].Value.ToString().Substring(0, FIGNO.Length))
                            {
                                fpSpread1.Sheets[0].Cells[j, 1].Value = "True";
                            }
                        }
                        else
                        {
                            if (fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "FIG NO")].Value.ToString().Length >= FIGNO.Length &&
                                FIGNO == fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "FIG NO")].Value.ToString().Substring(0, FIGNO.Length))
                            {
                                fpSpread1.Sheets[0].Cells[j, 1].Value = "False";
                            }
                        }
                    }
                }
                ////////////////////////////// 선택한 FIG NO 하단 자동체크 //////////////////////////////////////
            }
        }
        #endregion

        #region btnSPR_CanCle_Click
        private void btnSPR_CanCle_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(SystemBase.Base.MessageRtn("P0014"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text.ToString() == "True")
                        {
                            string QUERY = " usp_PSA031 @pType = 'U1', ";
                            QUERY += " @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROJECT_NO")].Text + "', ";
                            QUERY += " @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROJECT_SEQ")].Text + "', ";
                            QUERY += " @pGROUP_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "GROUP_CD")].Text + "', ";
                            QUERY += " @pPRNT_ITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PRNT_ITEM_CD")].Text + "', ";
                            QUERY += " @pPLANT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PLANT_CD")].Text + "', ";
                            QUERY += " @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "', ";
                            QUERY += " @pROUT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "ROUT_NO")].Text + "', ";
                            QUERY += " @pRES_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자원코드")].Text + "', ";
                            QUERY += " @pPROC_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "PROC_SEQ")].Text + "', ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text == "0" || fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text == "")
                            {
                                ERRCode = "ER";
                                MSGCode = "P0040";
                                Trans.Rollback(); goto Exit;
                            }
                            QUERY += " @pMFG_LT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외주L/T(일)")].Text + "', ";
                            QUERY += " @pUP_KIND = 'ER', ";
                            QUERY += " @pSCH_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "스케쥴 ID")].Text + "', ";
                            QUERY += " @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "WORKORDER_NO")].Text + "', ";
                            QUERY += " @pWORKORDER_NO_RS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "WORKORDER_NO_RS")].Text + "' ";
                            QUERY += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(QUERY, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                    SearchExec();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "B0015";
                }
            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            }
        }
        #endregion		

    }
}
