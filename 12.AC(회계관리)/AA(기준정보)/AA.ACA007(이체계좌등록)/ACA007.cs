#region 작성정보
/*********************************************************************/
// 단위업무 :  이체계좌등록
// 작 성 자 :  한 미 애
// 작 성 일 :  2022-03-08
// 작성내용 :  거래처 또는 사원에 대한 이체계좌 조회 및 저장
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

namespace AA.ACA007
{
    public partial class ACA007 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        #endregion

        #region 생성자
        public ACA007()
        {
            InitializeComponent();
        }
        #endregion

        #region ACA007_Load(): Form Load 시
        private void ACA007_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(grpCustInfo);

            SystemBase.ComboMake.C1Combo(cboSTransType, "usp_B_COMMON @pType = 'COMM', @pCODE = 'A131', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3);   // 이체대상구분
            SystemBase.ComboMake.C1Combo(cboTransType, "usp_B_COMMON @pType = 'COMM', @pCODE = 'A131', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");       // 이체대상구분

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "이체대상구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'A131', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            NewExec();
        }
        #endregion

        #region TabSetting(): 탭 초기화 처리
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage1); //거래처정보
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage2); //사용자정보

            this.tabForms.SelectedIndex = 0;
        }
        #endregion

        #region NewExec(): New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            Group_Reset();
            Combo_Reset();

            txtTransCd.Value = "";       // 위의 그룹박스 및 콤보박스 초기화시 제외되어 별도 초기화.

            //컨트롤 활성화 유무
            Control_Enable("Y");

            dtpEffStartDt.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpEffEndDt.Text = "2999-12-31";

            chkUseFlag.Checked = true;
        }
        #endregion

        #region SearchExec(): 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string strQuery = " usp_ACA007  'S1'";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
            strQuery = strQuery + ", @pTRANS_CD ='" + txtSTransCd.Text.Trim() + "' ";
            strQuery = strQuery + ", @pTRANS_NM ='" + txtSTransNm.Text + "' ";
            strQuery = strQuery + ", @pTRANS_TYPE ='" + cboSTransType.SelectedValue.ToString() + "' ";            

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec(): 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(grpCustInfo))
                {
                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_ACA007 'U1' ";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        strSql = strSql + ", @pTRANS_CD = '" + txtTransCd.Text.Trim() + "'";
                        strSql = strSql + ", @pTRANS_NM = '" + txtTransNm.Text + "'";
                        strSql = strSql + ", @pTRANS_TYPE = '" + cboTransType.SelectedValue.ToString() + "'";
                        strSql = strSql + ", @pBANK_CD = '" + txtBankCd.Text + "'";
                        strSql = strSql + ", @pACCOUNT_NO = '" + txtAcctNo.Text.ToString() + "'";
                        strSql = strSql + ", @pACCT_OWNER = '" + txtAcctOwner.Text.ToString() + "'";
                        strSql = strSql + ", @pREMARK = '" + txtRemark.Text.ToString() + "'";
                        strSql = strSql + ", @pEFF_START_DT = '" + dtpEffStartDt.Text + "'";
                        strSql = strSql + ", @pEFF_END_DT = '" + dtpEffEndDt.Text + "'";

                        string strUseFlag = "N";
                        if (chkUseFlag.Checked == true)
                            strUseFlag = "Y";

                        strSql = strSql + ", @pUSE_YN = '" + strUseFlag + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec(): 삭제 로직
        protected override void DeleteExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {

                string msg = SystemBase.Base.MessageRtn("B0027");
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(msg), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                        string strSql = " usp_ACA007  'D1'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                        strSql = strSql + ", @pTRANS_CD  = '" + txtTransCd.Text + "'";                        

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                        this.Cursor = System.Windows.Forms.Cursors.Default;
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

                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
        }
        #endregion

        #region fpSpread1_SelectionChanged(): 좌측 그리드 클릭시. 우측상세조회
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                    // 같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    string strTransCd = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "이체대상코드")].Text.ToString();

                    Right_Search(strTransCd);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region Right_Search(): 우측 상세검색
        private void Right_Search(string strTransCd)
        {
            try
            {
                //현재 row값 설정
                PreRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                string strSql = " usp_ACA007  'S2' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                strSql = strSql + ", @pTRANS_CD = '" + strTransCd + "'";                

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                cboTransType.SelectedValue = ds.Tables[0].Rows[0]["TRANS_TYPE"].ToString();
                txtTransCd.Value = ds.Tables[0].Rows[0]["TRANS_CD"].ToString();

                txtTransNm.Value = ds.Tables[0].Rows[0]["TRANS_NM"].ToString();
                txtBankCd.Value = ds.Tables[0].Rows[0]["BANK_CD"].ToString();
                txtBankNm.Value = ds.Tables[0].Rows[0]["BANK_NM"].ToString();
                txtAcctNo.Value = ds.Tables[0].Rows[0]["ACCOUNT_NO"].ToString();
                txtAcctOwner.Value = ds.Tables[0].Rows[0]["ACCT_OWNER"].ToString();
                txtRemark.Value = ds.Tables[0].Rows[0]["REMARK"].ToString();
                dtpEffStartDt.Value = ds.Tables[0].Rows[0]["EFF_START_DT"].ToString();
                dtpEffEndDt.Value = ds.Tables[0].Rows[0]["EFF_END_DT"].ToString();
                if (ds.Tables[0].Rows[0]["USE_YN"].ToString() == "1") chkUseFlag.Checked = true;
                else chkUseFlag.Checked = false;
 
                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                Cust_User_Info_Enable(true);

                // 거래처정보
                txtCustFullNm.Value = ds.Tables[0].Rows[0]["CUST_FULL_NM"].ToString();
                cboCustType.SelectedValue = ds.Tables[0].Rows[0]["CUST_TYPE"].ToString();
                txtRepreNm.Value = ds.Tables[0].Rows[0]["REPRE_NM"].ToString();
                dtpApplyDt.Value = ds.Tables[0].Rows[0]["APPLY_DT"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_USE_FLAG"].ToString() == "1") chkCustUseFlag.Checked = true;
                else chkCustUseFlag.Checked = false;
                txtAddr1.Value = ds.Tables[0].Rows[0]["ADDR1"].ToString();

                // 사원정보
                txtUserNm.Value = ds.Tables[0].Rows[0]["USR_NM"].ToString();
                txtDeptCd.Value = ds.Tables[0].Rows[0]["DEPT_CD"].ToString();
                txtDeptNm.Value = ds.Tables[0].Rows[0]["DEPT_NM"].ToString();
                dtpUserEndDt.Value = ds.Tables[0].Rows[0]["USE_DT"].ToString();

                Cust_User_Info_Enable(false);

                // 이체대상이 거래처이면 거래처 탭 활성화
                if (cboTransType.SelectedValue.ToString() == "CUST")
                    tabForms.SelectedIndex = 0;
                else 
                    tabForms.SelectedIndex = 1;

                Control_Enable("N");        // 상단 항목 비활성화
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Control_Enable(): 컨트롤 활성화 유무 처리
        private void Control_Enable(string UseYn)
        {
            if (UseYn == "Y")
            {
                //이체대상코드
                txtTransCd.ReadOnly = false;
                txtTransCd.BackColor = SystemBase.Validation.Kind_LightCyan;
                //이체대상코드 검색버튼
                btnRef.Enabled = true;
                //이체대상구분
                cboTransType.ReadOnly = false;
                cboTransType.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
            }
            else
            {
                //이체대상코드
                txtTransCd.ReadOnly = true;
                txtTransCd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //이체대상코드 검색버튼
                btnRef.Enabled = false;
                //이체대상구분
                cboTransType.ReadOnly = true;
                cboTransType.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;
            }
        }
        #endregion

        #region Cust_User_Info_Enable()
        private void Cust_User_Info_Enable(bool EnableFlag)
        {
            txtCustFullNm.Enabled = EnableFlag;
            cboCustType.Enabled = EnableFlag;
            txtRepreNm.Enabled = EnableFlag;
            dtpApplyDt.Enabled = EnableFlag;
            chkCustUseFlag.Enabled = EnableFlag;
            txtAddr1.Enabled = EnableFlag;

            // 사원정보
            txtUserNm.Enabled = EnableFlag;
            txtDeptCd.Enabled = EnableFlag;
            txtDeptNm.Enabled = EnableFlag;
            dtpUserEndDt.Enabled = EnableFlag;
        }
        #endregion

        #region Combo_Reset(): 리셋
        private void Combo_Reset()
        {
            SystemBase.ComboMake.C1Combo(cboSTransType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A131', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3);   //이체대상구분
            // 동일한 이체대상구분을 여러 건 등록시 매번 변경해야 되므로 번거로워서 주석 처리함.
            //SystemBase.ComboMake.C1Combo(cboTransType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'A131', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");     //이체대상구분
            SystemBase.ComboMake.C1Combo(cboCustType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//거래처구분
        }
        #endregion

        #region Group_Reset(): 그룹박스 초기화
        private void Group_Reset()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox5);
            SystemBase.Validation.GroupBox_Reset(groupBox6);
            SystemBase.Validation.GroupBox_Reset(grpCustInfo);
        }
        #endregion

        #region btnBank_Click(): 은행코드 버튼 클릭시. 은행코드 팝업 띄우기
        private void btnBank_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B070', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBankCd.Text, txtBankNm.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00036", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행코드 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBankCd.Text = Msgs[0].ToString();
                    txtBankNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region btnTransCd_Click(): 이체대상 검색 버튼 클릭시. 이체대상 팝업 띄워줌.
        private void btnTransCd_Click(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                WNDW051 pu = new WNDW051(txtTransCd.Text.ToString());
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtSTransCd.Value = Msgs[1].ToString();
                    txtSTransNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "이체대상조회 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region btnRef_Click(): 이체대상코드 버튼 클릭시. 대상구분에 해당하는 팝업 띄우기
        private void btnRef_Click(object sender, EventArgs e)
        {
            try
            {
                // 이체대상구분이 거래처인 경우 거래처조회 팝업 띄움.
                if (cboTransType.SelectedValue.ToString() == "CUST")
                {               
                    WNDW002 pu = new WNDW002(txtTransCd.Text, "P");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        txtTransCd.Text = Msgs[1].ToString();
                        txtTransNm.Value = Msgs[2].ToString();          // 이체대상명
                        txtAcctOwner.Value = Msgs[2].ToString();        // 예금주
                    }
                }
                else if (cboTransType.SelectedValue.ToString() == "USER")  // 이체대상구분이 사용자인 경우 사용자조회 팝업 띄움.
                {
                    string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { txtTransCd.Text, "" };

                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
                    pu.ShowDialog();

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        txtTransCd.Text = Msgs[0].ToString();
                        txtTransNm.Value = Msgs[1].ToString();          // 이체대상명
                        txtAcctOwner.Value = Msgs[1].ToString();        // 예금주
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
