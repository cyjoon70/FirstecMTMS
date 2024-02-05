

#region 작성정보
/*********************************************************************/
// 단위업무명 : 만기어음일괄처리
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-18
// 작성내용 : 만기어음일괄처리
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

namespace AG.ACG301
{
    public partial class ACG301 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        public ACG301()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG301_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox2);//필수 적용
            SystemBase.ComboMake.C1Combo(cboNoteKind, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A502', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //어음구분
            SystemBase.ComboMake.C1Combo(cboBankCd, "SELECT BANK_CD, BANK_NM, 'N' FROM B_BANK(NOLOCK) WHERE CO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //은행

            // 2022.01.28. hma 추가(Start): 그리드 결재상태
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);
            // 2022.01.28. hma 추가(End)

            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            dtpExpDtFr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToShortDateString();
            dtpExpDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
            cboNoteKind.SelectedValue = "D1";

            dtpSlipDt.Value = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;
            txtDeptCd.Text = SystemBase.Base.gstrDEPT;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                    string strQuery = " usp_ACG301  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pNOTE_KIND = '" + cboNoteKind.SelectedValue.ToString() + "' ";
                    strQuery += ", @pEXP_DT_FROM = '" + dtpExpDtFr.Text + "' ";
                    strQuery += ", @pEXP_DT_TO = '" + dtpExpDtTo.Text + "' ";

                    if (optCreate.Checked == true) strQuery += ", @pACT_TYPE = 'R' ";
                    else if (optCancel.Checked == true) strQuery += ", @pACT_TYPE = 'C' ";
                    strQuery += ", @pCUST_CD = '" + txtCustCd.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    // 2022.01.28. hma 추가(Start): 확정 상태이지만 확정취소를 할 수 없거나, 미확정 상태이지만 확정 처리를 할 수 없는 건에 대해서는 선택 항목 비활성화 처리
                    string strSlipNo = "";
                    string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                    string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        strSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
                        strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();
                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();

                        // 확정상태인 경우 결재상태가 상신대기/반려/승인(반제승인Y) 상태이면 확정취소 가능.
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정상태")].Text == "Y")
                        {
                            if ((strSlipNo != "" && strCSlipNo == "") ||
                               ((strCSlipNo != "") && 
                                 ((strCSlipConfirm == "N" && (strCSlipGwStatus == "READY" || strCSlipGwStatus == "REJECT" )) ||     // 확정전표결재상태가 상신대기/반려 이거나
                                  (strCSlipConfirm == "Y" && strCSlipGwStatus == "APPR" && strMinusConfirm == "Y"))))               // 확정전표결재상태가 승인이면서 반제승인이 Y인 경우
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, " ") + "|0"      // 일반
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"     // 확정건은 반제취소 버튼 비활성화
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, " ") + "|3"       // 읽기전용이면서 필수항목에서 제외
                                       + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"     // 확정건은 반제취소 버튼 비활성화
                                   );
                            }
                        }
                        else
                        {
                            // 미확정 상태인 경우
                            // 미확정상태이지만 반제전표 결재상태가 승인이면서 반제승인이 Y인 경우에도 확정 가능.
                            if ((strMSlipNo == "") ||
                                (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "APPR" && strMinusConfirm == "Y")))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, " ") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, " ") + "|3"
                                    );
                            }

                            // 미확정건이지만 반제전표가 생성되어 결재상태가 상신대기/반려이면 반제취소 버튼 활성화하여 반제전표 삭제하고 승인 상태로 변경되게.
                            if (strMSlipNo != "" &&
                                 (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|0"
                                    );
                            }
                            else
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                        SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소") + "|3"
                                    );
                            }
                        }
                    }
                    // 2022.01.28. hma 추가(End)
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "조회TYPE")].Text == "R")
                {
                    if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
            }

            string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                {
                    string strSLIPNO = "";
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, " ")].Text == "True")
                        {
                            string strSql = " usp_ACG301 'I1'";
                            strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pACT_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조회TYPE")].Text + "' ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "조회TYPE")].Text == "C")
                            {
                                strSql = strSql + ", @pSLIP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text + "' ";
                            }
                            else
                            {
                                strSql = strSql + ", @pSLIP_NO = '" + strSLIPNO + "' ";
                            }
                            strSql = strSql + ", @pBIZ_AREA_CD = '" + SystemBase.Base.gstrBIZCD + "' ";
                            strSql = strSql + ", @pSLIP_DT = '" + dtpSlipDt.Text + "' ";
                            strSql = strSql + ", @pREORG_ID = '" + strREORG_ID + "' ";
                            strSql = strSql + ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                            strSql = strSql + ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                            strSql = strSql + ", @pACCT_NO = '" + txtAcctNo.Text + "' ";
                            strSql = strSql + ", @pBANK_CD = '" + cboBankCd.SelectedValue.ToString() + "' ";
                            strSql = strSql + ", @pMASTER_REMARK = '" + txtRemark.Text + "' ";
                            strSql = strSql + ", @pNOTE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "어음번호")].Text + "' ";
                            strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "' ";
                            string strEndFlag = "Y";
                            for (int iRow = i + 1; iRow < fpSpread1.Sheets[0].Rows.Count; iRow++)
                            {
                                if (fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, " ")].Text == "True")
                                {
                                    strEndFlag = "N";
                                }
                            }
                            strSql = strSql + ", @pEND_FLAG = '" + strEndFlag + "'";
                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            strSLIPNO = ds.Tables[0].Rows[0][2].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }
                    Trans.Commit();
                }
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

        #region TextChanged 이벤트
        //거래처
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //부서
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
        //계정
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strPreAcctNm = txtAcctNm.Text;
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y'");

                bool bAcctNo = false;
                bool bBank = false;
                //상세조회 SQL
                string strSql = " usp_A_COMMON  'A031'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql = strSql + ", @pCODE ='" + txtAcctCd.Text + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["CTRL_CD1"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD2"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD3"].ToString().Trim() == "BK"
                        || dt.Rows[0]["CTRL_CD4"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD5"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD6"].ToString().Trim() == "BK"
                        || dt.Rows[0]["CTRL_CD7"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD8"].ToString().Trim() == "BK") bBank = true;

                    if (dt.Rows[0]["CTRL_CD1"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD2"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD3"].ToString().Trim() == "BA"
                        || dt.Rows[0]["CTRL_CD4"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD5"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD6"].ToString().Trim() == "BA"
                        || dt.Rows[0]["CTRL_CD7"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD8"].ToString().Trim() == "BA") bAcctNo = true;
                }
                if (bAcctNo == true)
                {
                    txtAcctNo.Tag = "계좌번호;1;;";
                    btnAcctNo.Tag = ";;;";
                }
                else
                {
                    txtAcctNo.Tag = "계좌번호;2;;";
                    txtAcctNo.Value = "";
                    btnAcctNo.Tag = ";2;;";
                }

                if (bBank == true)
                {
                    cboBankCd.Tag = "은행코드;1;;";
                }
                else
                {
                    cboBankCd.Tag = "은행코드;2;;";
                    cboBankCd.SelectedValue = "";
                }
                SystemBase.Validation.GroupBox_Setting(groupBox2);//필수 적용

            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //회계일자
        private void dtpSlipDt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_ACD001  'P5'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_DT ='" + dtpSlipDt.Text + "' ";
                strQuery = strQuery + ", @pDEPT_CD ='" + txtDeptCd.Text + "' ";
                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                if (ds.Tables.Count == 2)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        strREORG_ID = ds.Tables[1].Rows[0]["REORG_ID"].ToString();
                    }
                    else
                    {
                        strREORG_ID = "";
                    }
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtDeptNm.Value = ds.Tables[0].Rows[0]["DEPT_NM"].ToString();
                }
                else
                {
                    txtDeptNm.Value = "";
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 버튼 클릭
        //거래처
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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //부서
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011(dtpSlipDt.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //계정
        private void btnAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A032', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCd.Value = Msgs[0].ToString();
                    txtAcctNm.Value = Msgs[1].ToString();

                    bool bAcctNo = false;
                    bool bBank = false;
                    //상세조회 SQL
                    string strSql = " usp_A_COMMON  'A031'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pCODE ='" + txtAcctCd.Text + "' ";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["CTRL_CD1"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD2"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD3"].ToString().Trim() == "BK"
                            || dt.Rows[0]["CTRL_CD4"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD5"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD6"].ToString().Trim() == "BK"
                            || dt.Rows[0]["CTRL_CD7"].ToString().Trim() == "BK" || dt.Rows[0]["CTRL_CD8"].ToString().Trim() == "BK") bBank = true;

                        if (dt.Rows[0]["CTRL_CD1"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD2"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD3"].ToString().Trim() == "BA"
                            || dt.Rows[0]["CTRL_CD4"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD5"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD6"].ToString().Trim() == "BA"
                            || dt.Rows[0]["CTRL_CD7"].ToString().Trim() == "BA" || dt.Rows[0]["CTRL_CD8"].ToString().Trim() == "BA") bAcctNo = true;
                    }
                    if (bAcctNo == true)
                    {
                        txtAcctNo.Tag = "계좌번호;1;;";
                        btnAcctNo.Tag = ";;;";
                    }
                    else
                    {
                        txtAcctNo.Tag = "계좌번호;2;;";
                        txtAcctNo.Value = "";
                        btnAcctNo.Tag = ";2;;";
                    }

                    if (bBank == true)
                    {
                        cboBankCd.Tag = "은행코드;1;;";
                    }
                    else
                    {
                        cboBankCd.Tag = "은행코드;2;;";
                        cboBankCd.SelectedValue = "";
                    }
                    SystemBase.Validation.GroupBox_Setting(groupBox2);//필수 적용
                }
                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //계좌번호
        private void btnAcctNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = 'BA' ";
                string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                string[] strSearch = new string[] { txtAcctNo.Text, cboBankCd.SelectedValue.ToString() };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD001_P1", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계좌번호 조회");
                pu.Width = 800;
                pu.Height = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtAcctNo.Text = Msgs[0].ToString();
                    cboBankCd.SelectedValue = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnSlipView_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1.Sheets[0].GetSelection(0) != null)
                {
                    int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
                    if (intRow < 0)
                    {
                        return;
                    }

                    string strSLIP_NO = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;

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

        // 2022.01.28. hma 추가(Start)
        #region fpSpread1_ButtonClicked() 그리드 버튼 클릭
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "반제취소"))
                {
                    // 2022.02.16. hma 추가: 메시지 확인
                    DialogResult dsMsg = MessageBox.Show("반제취소 처리하시겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dsMsg == DialogResult.Yes)
                    {
                        string strCSlipNo = "", strCSlipConfirm = "", strCSlipGwStatus = "", strMinusConfirm = "";
                        string strMSlipNo = "", strMSlipConfirm = "", strMSlipGwStatus = "";

                        int i;
                        i = e.Row;                    strCSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표번호")].Text;
                        strCSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표승인")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Text == "")
                            strCSlipGwStatus = "";
                        else
                            strCSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "확정전표결재")].Value.ToString();

                        strMinusConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제승인")].Text;
                        strMSlipNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표번호")].Text;
                        strMSlipConfirm = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표승인")].Text;
                        strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text;
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Text == "")
                            strMSlipGwStatus = "";
                        else
                            strMSlipGwStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반제전표결재")].Value.ToString();

                        // 확정전표 결재상태가 승인이고, 반제전표 결재상태가 상신대기/반려인 경우 반제전표 삭제 가능하게 함.
                        if (strCSlipGwStatus == "APPR" && strMinusConfirm == "Y" && strMSlipNo != "" &&
                            (strMSlipGwStatus == "READY" || strMSlipGwStatus == "REJECT"))
                        {
                            MinusSlipDelete(strMSlipNo);
                        }
                        else
                        {
                            MessageBox.Show("확정전표 결재상태가 승인이고 반제전표 결재상태가 상신대기/반려인 경우 반제취소 가능합니다.");
                            return;
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "결재자 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                SystemBase.Loggers.Log(this.Name, f.ToString());
            }
        }
        #endregion


        #region MinusSlipDelete(): 해당 전표번호에 대한 반제전표 삭제 처리
        private void MinusSlipDelete(string SLIP_NO)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_ACG301  'D1'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pSLIP_NO = '" + SLIP_NO + "' ";
                strSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
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
        #endregion
        // 2022.01.28. hma 추가(End)

    }
}
