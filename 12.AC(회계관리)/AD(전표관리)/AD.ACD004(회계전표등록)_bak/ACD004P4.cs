

#region 작성정보
/*********************************************************************/
// 단위업무명 : 차입금정보
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-02-21
// 작성내용 : 차입금정보
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
    public partial class ACD004P4 : UIForm.Buttons
    {
        #region 변수선언
        DataTable Dt = null;
        string strCUR_CD = "";
        string strLoan_No = "";
        string strLOAN_DT = "";
        string strACCT_CD = "";
        double iSlipAmt = 0;
        double iSlipAmtLoc = 0;
        string strRemark = "";
        #endregion

        public ACD004P4()
        {
            InitializeComponent();
        }

        public ACD004P4(DataTable Loan_Dt, string CUR_CD, string LOAN_NO, string LOAN_DT, string ACCT_CD)
        {
            Dt = Loan_Dt;
            strCUR_CD = CUR_CD;
            strLoan_No = LOAN_NO;
            strLOAN_DT = LOAN_DT;
            strACCT_CD = ACCT_CD;
            InitializeComponent();
        }

        #region Form Load 시
        private void ACD004P4_Load(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.Buttons.ReButton("010000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "차입금정보";
                SystemBase.ComboMake.C1Combo(cboCurCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //화폐단위
                SystemBase.ComboMake.C1Combo(cboLoanUseCd, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A501', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //차입용도
                SystemBase.ComboMake.C1Combo(cboLoanType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'A504', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //차입종류
                
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                dtpOpenDt.Text = strLOAN_DT;
                dtpOpenDt.Enabled = false;
                txtAcctCd.Value = strACCT_CD;
                SearchExec();
                //NewExec();
                //if (Dt != null)
                //{
                //    if(Dt.Rows.Count > 0)
                //        SearchExec();
                //}
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
                if (Dt.Rows.Count == 0)
                {
                    Dt.Rows.Add();
                    Dt.Rows[0]["CUR_CD"] = strCUR_CD;
                }
                if (strCUR_CD == "KRW")
                {
                    Dt.Rows[0]["EXCH_RATE"] = "1";
                }
                Dt.Rows[0]["OPEN_DT"] = strLOAN_DT;
                txtLoanNo.Value = Dt.Rows[0]["LOAN_NO"].ToString();
                txtLoanNm.Value = Dt.Rows[0]["LOAN_NM"].ToString();
                if (Dt.Rows[0]["LOAN_DIV"].ToString() == "BP")
                {
                    optBP.Checked = true;
                }
                else
                {
                    optBK.Checked = true;
                }
                cboLoanUseCd.SelectedValue = Dt.Rows[0]["LOAN_USE_CD"].ToString();
                cboLoanType.SelectedValue = Dt.Rows[0]["LOAN_TYPE"].ToString();
                txtLoanBankCustCd.Value = Dt.Rows[0]["LOAN_BANK_CUST_CD"].ToString();

                dtpOpenDt.Value = Dt.Rows[0]["OPEN_DT"].ToString();
                dtpExpDt.Value = Dt.Rows[0]["EXP_DT"].ToString();
                cboCurCd.SelectedValue = Dt.Rows[0]["CUR_CD"].ToString();
                txtLoanAmt.Value = Dt.Rows[0]["LOAN_AMT"].ToString();
                txtExch_Rate.Value = Dt.Rows[0]["EXCH_RATE"].ToString();
                txtLoanAmtLoc.Value = Dt.Rows[0]["LOAN_AMT_LOC"].ToString();
                txtDeferTerm.Value = Dt.Rows[0]["DEFER_TERM"].ToString();
                if (Dt.Rows[0]["REPAYMENT_METHOD"].ToString() == "EX")
                {
                    optREPAYMENT_METHOD_EX.Checked = true;
                }
                else
                {
                    optREPAYMENT_METHOD_EQ.Checked = true;
                }

                txtRepaymentCycle.Value = Dt.Rows[0]["REPAYMENT_CYCLE"].ToString();
                dtpFirst_C_Repayment_Dt.Value = Dt.Rows[0]["FIRST_C_REPAYMENT_DT"].ToString();
                txtDeferTerm.Value = Dt.Rows[0]["DEFER_TERM"].ToString();
                if (Dt.Rows[0]["INTEREST_PAYMENT_TYPE"].ToString() == "AI")
                {
                    optINTEREST_PAYMENT_TYPE_AI.Checked = true;
                }
                else
                {
                    optINTEREST_PAYMENT_TYPE_DI.Checked = true;
                }
                dtpFirst_I_Repayment_Dt.Value = Dt.Rows[0]["FIRST_I_REPAYMENT_DT"].ToString();
                if (Dt.Rows[0]["INTEREST_RATE_CHANGE"].ToString() == "X")
                {
                    optINTEREST_RATE_CHANGE_X.Checked = true;
                }
                else
                {
                    optINTEREST_RATE_CHANGE_F.Checked = true;
                }


                txtLoanInterestRate.Value = Dt.Rows[0]["LOAN_INTEREST_RATE"].ToString();
                txtRemark.Value = Dt.Rows[0]["REMARK"].ToString();
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
        private void txtLoanBankCustCd_TextChanged(object sender, EventArgs e)
        {
            //거래처
            if (optBP.Checked == true)
            {
                try
                {
                    txtLoanBankCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtLoanBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else //은행
            {
                try
                {
                    txtLoanBankCustNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtLoanBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void optBK_CheckedChanged(object sender, EventArgs e)
        {
            //거래처
            if (optBP.Checked == true)
            {
                try
                {
                    txtLoanBankCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtLoanBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else //은행
            {
                try
                {
                    txtLoanBankCustNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtLoanBankCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //화폐단위
        private void cboCurCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCurCd.Text != "")
                {
                    if (cboCurCd.SelectedValue.ToString() == "KRW")
                    {
                        txtExch_Rate.Value = 1;
                        txtLoanAmtLoc.Enabled = false;
                        txtExch_Rate.Enabled = false;
                        txtLoanAmtLoc.Value = txtLoanAmt.Text.Replace(",", "");
                    }
                    else
                    {

                        txtLoanAmtLoc.Enabled = true;
                        txtExch_Rate.Enabled = true;
                    }
                }
                else
                {
                    txtLoanAmtLoc.Enabled = true;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //차입금액
        private void txtLoanAmt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCurCd.Text != "")
                {
                    if (cboCurCd.SelectedValue.ToString() == "KRW")
                    {
                        txtLoanAmtLoc.Value = txtLoanAmt.Text.Replace(",", "");
                    }
                    else
                    {
                        if (txtLoanAmt.Text.Replace("-", "") != "" && txtExch_Rate.Text.Replace("-", "") != "")
                        {
                            txtLoanAmtLoc.Value = Math.Round(Convert.ToDecimal(txtLoanAmt.Text.Replace(",", "")) * Convert.ToDecimal(txtExch_Rate.Text.Replace(",", "")), 0);
                        }
                        else
                        {
                            txtLoanAmtLoc.Value = 0;
                        }
                    }

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //환율
        private void txtExch_Rate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtLoanAmt.Text.Replace("-", "") != "" && txtExch_Rate.Text.Replace("-", "") != "")
                {
                    txtLoanAmtLoc.Value = Math.Round(Convert.ToDecimal(txtLoanAmt.Text.Replace(",", "")) * Convert.ToDecimal(txtExch_Rate.Text.Replace(",", "")), 0);
                }
                else
                {
                    txtLoanAmtLoc.Value = 0;
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //거래처팝업
        private void btnLoanBankCust_Click(object sender, EventArgs e)
        {
            //거래처
            if (optBP.Checked == true)
            {
                try
                {
                    WNDW.WNDW002 pu = new WNDW.WNDW002(txtLoanBankCustCd.Text, "PS");
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtLoanBankCustCd.Text = Msgs[1].ToString();
                        txtLoanBankCustCd.Focus();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            else //은행
            {
                try
                {
                    string strBANK_CD = txtLoanBankCustCd.Text;

                    string strQuery = " usp_ACD001 @pType='P1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pCTRL_CD = 'BK' ";
                    string[] strWhere = new string[] { "@pCODE_CD1", "@pCODE_CD2" };
                    string[] strSearch = new string[] { txtLoanBankCustCd.Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("ACD004_P2", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 조회");
                    pu.Width = 800;
                    pu.Height = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtLoanBankCustCd.Text = Msgs[0].ToString();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "은행 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 메인 화면으로 리턴
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    if (txtLoanNo.Text == "")
                    {
                        string strSql = " usp_ACD001 'I51'";

                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pSLIP_DT = '" + dtpOpenDt.Text + "'";
                        strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                        strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        txtLoanNo.Value = ds.Tables[0].Rows[0][2].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                    Trans.Commit();

                    Dt.Rows[0]["LOAN_NO"] = txtLoanNo.Value;
                    Dt.Rows[0]["LOAN_NM"] = txtLoanNm.Value;
                    if (optBP.Checked == true)
                    {
                        Dt.Rows[0]["LOAN_DIV"] = "BP";

                    }
                    else
                    {
                        Dt.Rows[0]["LOAN_DIV"] = "BK";
                    }
                    Dt.Rows[0]["LOAN_USE_CD"] = cboLoanUseCd.SelectedValue.ToString();
                    Dt.Rows[0]["LOAN_TYPE"] = cboLoanType.SelectedValue.ToString();
                    Dt.Rows[0]["LOAN_BANK_CUST_CD"] = txtLoanBankCustCd.Value;
                    Dt.Rows[0]["OPEN_DT"] = dtpOpenDt.Text;
                    Dt.Rows[0]["EXP_DT"] = dtpExpDt.Text;
                    Dt.Rows[0]["CUR_CD"] = cboCurCd.SelectedValue;
                    Dt.Rows[0]["LOAN_AMT"] = txtLoanAmt.Text;
                    Dt.Rows[0]["EXCH_RATE"] = txtExch_Rate.Text.Replace(",","");
                    Dt.Rows[0]["LOAN_AMT_LOC"] = txtLoanAmtLoc.Text;
                    Dt.Rows[0]["DEFER_TERM"] = txtDeferTerm.Text;
                    if (optREPAYMENT_METHOD_EX.Checked == true)
                    {
                        Dt.Rows[0]["REPAYMENT_METHOD"] = "EX";
                    }
                    else
                    {
                        Dt.Rows[0]["REPAYMENT_METHOD"] = "EQ";
                    }

                    Dt.Rows[0]["REPAYMENT_CYCLE"] = txtRepaymentCycle.Text;
                    if (dtpFirst_C_Repayment_Dt.Text == "") Dt.Rows[0]["FIRST_C_REPAYMENT_DT"] = System.DBNull.Value;
                    else Dt.Rows[0]["FIRST_C_REPAYMENT_DT"] = dtpFirst_C_Repayment_Dt.Text;
                    if (optINTEREST_PAYMENT_TYPE_AI.Checked == true)
                    {
                        Dt.Rows[0]["INTEREST_PAYMENT_TYPE"] = "AI";
                    }
                    else
                    {
                        Dt.Rows[0]["REPAYMENT_METHOD"] = "DI";
                    }

                    if (dtpFirst_I_Repayment_Dt.Text == "") Dt.Rows[0]["FIRST_I_REPAYMENT_DT"] = System.DBNull.Value;
                    else Dt.Rows[0]["FIRST_I_REPAYMENT_DT"] = dtpFirst_I_Repayment_Dt.Text; 
                    if (optINTEREST_RATE_CHANGE_X.Checked == true)
                    {
                        Dt.Rows[0]["INTEREST_RATE_CHANGE"] = "X";
                    }
                    else
                    {
                        Dt.Rows[0]["INTEREST_RATE_CHANGE"] = "F";
                    }

                    Dt.Rows[0]["LOAN_INTEREST_RATE"] = txtLoanInterestRate.Text;
                    Dt.Rows[0]["REMARK"] = txtRemark.Text;

                    strCUR_CD = cboCurCd.SelectedValue.ToString();
                    strLoan_No = txtLoanNo.Text;
                    iSlipAmt = Convert.ToDouble(txtLoanAmt.Text.Replace(",", ""));
                    iSlipAmtLoc = Convert.ToDouble(txtLoanAmtLoc.Text.Replace(",", ""));
                    strRemark = txtRemark.Text;
                    ERRCode = "OK";
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
                    //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.OK;
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        public DataTable Loan_Dt { get { return Dt; } set { Dt = value; } }
        public string CUR_CD { get { return strCUR_CD; } set { strCUR_CD = value; } }
        public string LOAN_NO { get { return strLoan_No; } set { strLoan_No = value; } }
        public double SLIP_AMT { get { return iSlipAmt; } set { iSlipAmt = value; } }
        public double SLIP_AMT_LOC { get { return iSlipAmtLoc; } set { iSlipAmtLoc = value; } }
        public string REMARK { get { return strRemark; } set { strRemark = value; } }
    }
}
