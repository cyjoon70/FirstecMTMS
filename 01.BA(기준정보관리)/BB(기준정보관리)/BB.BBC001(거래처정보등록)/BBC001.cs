#region 작성정보
/*********************************************************************/
// 단위업무명 : 거래처정보등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-05
// 작성내용 : 거래처정보등록 및 관리
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

namespace BB.BBC001
{
    public partial class BBC001 : UIForm.FPCOMM1
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSearchData = "", strSaveData = ""; //컨트롤 저장 체크 변수
        #endregion

        #region 생성자
        public BBC001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBC001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);
            SystemBase.Validation.GroupBox_Setting(groupBox5);
            SystemBase.Validation.GroupBox_Setting(groupBox6);
            SystemBase.Validation.GroupBox_Setting(groupBox7);
            SystemBase.Validation.GroupBox_Setting(grpAcctNoInfo);      // 2022.04.18. hma 추가: 이체정보 탭

            //콤보박스 세팅
            Combo_Reset();

            TabSetting();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region TabSetting
        private void TabSetting()
        {
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage1); // 일반정보
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage2); // 업무정보
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage3); // 담당자
            UIForm.TabFPMake.TabPageColor(c1DockingTabPage4); // 2022.04.18. hma 추가: 이체정보 탭

            this.tabForms.SelectedIndex = 0;
        }
        #endregion
        
        #region 팝업창 열기
        private void cmdZipCode_Click(object sender, System.EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                //string strQuery = " usp_B_COMMON @pType = 'B020', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                //string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                //string[] strSearch = new string[] { txtZipCd.Text, "" };
                //UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "우편번호검사");
                //pu.Width = 500;
                //pu.ShowDialog();
                //if (pu.DialogResult == DialogResult.OK)
                //{
                //    Regex rx1 = new Regex("#");
                //    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                //    txtZipCd.Value = Msgs[0].ToString();
                //    txtAddr1.Value = Msgs[1].ToString();
                //    txtAddr2.Text = "";
                //    txtAddr2.Focus();
                //}

                WNDW030 pu = new WNDW030(txtZipCd.Text.ToString());
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtZipCd.Value = Msgs[1].ToString();
                    txtAddr1.Value = Msgs[2].ToString();
                    txtAddr2.Text = "";
                    txtAddr2.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "우편번호검사 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            Group_Reset();
            Combo_Reset();
            #region 컨트롤 활성화 유무
            Control_Enable("Y");
            #endregion
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            string strQuery = " usp_BBC001  'S1'";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pCUST_CD ='" + txtSCustCd.Text.Trim() + "' ";
            strQuery = strQuery + ", @pCUST_NM ='" + txtSCustNm.Text + "' ";
            strQuery = strQuery + ", @pCUST_TYPE ='" + cboSCustType.SelectedValue.ToString() + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            GroupBox[] gBox = null;

            //컨트롤 체크값 초기화
            strSaveData = "";
            //컨트롤 체크 함수
            gBox = new GroupBox[] { groupBox2, groupBox3, groupBox5, groupBox6, groupBox7, grpAcctNoInfo };     // 2022.04.18. hma 수정: grpAcctNoInfo 추가
            SystemBase.Validation.Control_Check(gBox, ref strSaveData);

            //기존 컨트롤 데이터와 현재 컨트롤 데이터 비교
            if (strSearchData == strSaveData && UIForm.FPMake.HasSaveData(fpSpread1) == false)
            {
                //변경되거나 처리할 데이터가 없습니다.
                MessageBox.Show(SystemBase.Base.MessageRtn("SY017"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Cursor = Cursors.Default;
                return;
            }

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                {
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox5))
                    {
                        if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox6))
                        {
                            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox7))
                            {
                                if (SystemBase.Validation.GroupBox_SaveSearchValidation(grpAcctNoInfo))     // 2022.04.18. hma 추가: 이체정보 탭
                                {
                                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                                    SqlCommand cmd = dbConn.CreateCommand();
                                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                                    try
                                    {
                                        string strSql = " usp_BBC001 'U1' ";
                                        strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                                        strSql = strSql + ", @pCUST_CD = '" + txtCustCd.Text.ToUpper().Trim() + "'";
                                        strSql = strSql + ", @pCUST_NM = '" + txtCustNm.Text + "'";
                                        if (cboCustType.Text.ToString() != "") strSql = strSql + ", @pCUST_TYPE = '" + cboCustType.SelectedValue.ToString() + "'";
                                        strSql = strSql + ", @pRGST_NO = '" + txtRgstNo.Text.ToString() + "'";
                                        strSql = strSql + ", @pCUST_FULL_NM = '" + txtCustFullNm.Text.ToString() + "'";
                                        strSql = strSql + ", @pCUST_ENG_NM = '" + txtCustEngNm.Text.ToString() + "'";
                                        strSql = strSql + ", @pREPRE_NM = '" + txtRepreNm.Text.ToString() + "'";
                                        strSql = strSql + ", @pCORP_RGST_NO = '" + txtCorpRgstNo.Text.ToString() + "'";
                                        strSql = strSql + ", @pFOUND_DT = '" + dtpFoundDt.Text + "'";
                                        strSql = strSql + ", @pAPPLY_DT = '" + dtpApplyDt.Text + "'";
                                        strSql = strSql + ", @pINDU_TYPE = '" + txtInduType.Text.ToString() + "'";
                                        strSql = strSql + ", @pINDU_KIND = '" + txtInduKind.Text.ToString() + "'";
                                        strSql = strSql + ", @pZIPCODE = '" + txtZipCd.Text.ToString() + "'";
                                        strSql = strSql + ", @pADDR1 = '" + txtAddr1.Text.ToString() + "'";
                                        strSql = strSql + ", @pADDR2 = '" + txtAddr2.Text.ToString() + "'";
                                        strSql = strSql + ", @pADDR1_ENG = '" + txtAddrEng1.Text.ToString() + "'";
                                        strSql = strSql + ", @pADDR2_ENG = '" + txtAddrEng2.Text.ToString() + "'";
                                        strSql = strSql + ", @pADDR3_ENG = '" + txtAddrEng3.Text.ToString() + "'";
                                        if (cboNatCd.Text.ToString() != "") strSql = strSql + ", @pNAT_CD = '" + cboNatCd.SelectedValue.ToString() + "'";
                                        strSql = strSql + ", @pTEL1 = '" + txtTel1.Text.ToString() + "'";
                                        strSql = strSql + ", @pTEL2 = '" + txtTel2.Text.ToString() + "'";
                                        strSql = strSql + ", @pFAX = '" + txtFax.Text.ToString() + "'";
                                        strSql = strSql + ", @pCHARGE_NM = '" + txtChargeNm.Text.ToString() + "'";
                                        strSql = strSql + ", @pCHARGE_TEL = '" + txtChargeTel.Text.ToString() + "'";
                                        string strUseFlag = "N"; if (chkUseFlag.Checked == true) strUseFlag = "Y";
                                        strSql = strSql + ", @pUSE_FLAG = '" + strUseFlag + "'";
                                        strSql = strSql + ", @pAREA_CD = '" + cboAreaCd.SelectedValue.ToString() + "'";
                                        strSql = strSql + ", @pCUST_GRP = '" + cboCustGrp.SelectedValue.ToString() + "'";
                                        strSql = strSql + ", @pHOME_URL = '" + txtHome.Text.ToString() + "'";
                                        strSql = strSql + ", @pE_MAIL = '" + txtEmail.Text.ToString() + "'";
                                        if (cboTradeType.Text.ToString() != "") strSql = strSql + ", @pTRADE_TYPE = '" + cboTradeType.SelectedValue.ToString() + "'";
                                        string strCred = "N"; if (chkCred.Checked == true) strCred = "Y";
                                        strSql = strSql + ", @pCRED_FLAG	  = '" + strCred + "'";
                                        strSql = strSql + ", @pCRED_GRP	  = '" + txtCredGrp.Text.ToString() + "'";
                                        if (cboGrade.Text.ToString() != "") strSql = strSql + ", @pCUST_GRADE = '" + cboGrade.SelectedValue.ToString() + "'";
                                        if (cboType1.Text.ToString() != "") strSql = strSql + ", @pCUST_TYPE1 = '" + cboType1.SelectedValue.ToString() + "'";
                                        if (cboType2.Text.ToString() != "") strSql = strSql + ", @pCUST_TYPE2 = '" + cboType2.SelectedValue.ToString() + "'";
                                        if (cboType3.Text.ToString() != "") strSql = strSql + ", @pCUST_TYPE3 = '" + cboType3.SelectedValue.ToString() + "'";
                                        if (cboBillIssType.Text.ToString() != "") strSql = strSql + ", @pBILL_ISS_TYPE = '" + cboBillIssType.SelectedValue.ToString() + "'";
                                        if (cboLimitChk.Text.ToString() != "") strSql = strSql + ", @pLIMIT_CHK = '" + cboLimitChk.SelectedValue.ToString() + "'";
                                        if (cboCurrCtl.Text.ToString() != "") strSql = strSql + ", @pCURR_CTL = '" + cboCurrCtl.SelectedValue.ToString() + "'";
                                        if (cboDelivType.Text.ToString() != "") strSql = strSql + ", @pDELIV_TYPE = '" + cboDelivType.SelectedValue.ToString() + "'";
                                        if (cboBillSumType.Text.ToString() != "") strSql = strSql + ", @pBILL_SUM_TYPE = '" + cboBillSumType.SelectedValue.ToString() + "'";
                                        string strAutoShip = "N"; if (chkAutoShip.Checked == true) strAutoShip = "Y";
                                        strSql = strSql + ", @pAUTO_SHIP = '" + strAutoShip + "'";
                                        strSql = strSql + ", @pCHARGE_DEPT = '" + txtChargeDept.Text + "'";
                                        strSql = strSql + ", @pCHARGE_POSITION = '" + txtChargePosition.Text + "'";
                                        strSql = strSql + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "'";

                                        strSql = strSql + ", @pSCM_YN = '" + chkScmYN.Checked + "'";
                                        strSql = strSql + ", @pSCM_PW = '" + SystemBase.Base.EnCode(txtScmPw.Text) + "'";

                                        strSql = strSql + ", @pBILL_ID = '" + txtBillId.Text + "'";
                                        strSql = strSql + ", @pBANK_ACCT_NO = '" + txtBankAcctNo.Text + "'";

                                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                                        // 2022.04.18. hma 추가(Start): 이체정보도 저장되도록 함.
                                        strSql = strSql + ", @pTRANS_NM = '" + txtTransNm.Text + "'";
                                        strSql = strSql + ", @pBANK_CD = '" + txtBankCd.Text + "'";
                                        strSql = strSql + ", @pACCOUNT_NO = '" + txtAcctNo.Text + "'";
                                        strSql = strSql + ", @pACCT_OWNER = '" + txtAcctOwner.Text + "'";
                                        strSql = strSql + ", @pREMARK = '" + txtRemark.Text + "'";
                                        strSql = strSql + ", @pEFF_START_DT = '" + dtpEffStartDt.Text + "'";
                                        strSql = strSql + ", @pEFF_END_DT = '" + dtpEffEndDt.Text + "'";
                                        if (chkUseYn.Checked == true)
                                            strSql = strSql + ", @pUSE_YN = 'Y'";
                                        else
                                            strSql = strSql + ", @pUSE_YN = 'N'";
                                        // 2022.04.18. hma 추가(End)

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

                                        //컨트롤 체크값 초기화
                                        strSearchData = "";
                                        //컨트롤 체크 함수
                                        gBox = new GroupBox[] { groupBox2, groupBox3, groupBox5, groupBox6, groupBox7 };
                                        SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                                        UIForm.FPMake.GridSetFocus(fpSpread1, txtCustCd.Text, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드"));
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
                        }
                    }
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region DelExec() 삭제 로직
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

                        string strSql = " usp_BBC001  'D1'";
                        strSql = strSql + ", @pCUST_CD  = '" + txtCustCd.Text + "'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

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

        #region 좌측 fpSpread 클릭시 우측상세조회
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                    //같은 Row 조회 되지 않게
                    if (intRow < 0)
                    {
                        return;
                    }

                    if (PreRow == intRow && PreRow != -1 && intRow != 0)   //현 Row에서 컬럼이동시는 조회 안되게
                    {
                        return;
                    }

                    string strCustCd = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "거래처코드")].Text.ToString();//수주번호

                    Group_Reset();

                    Right_Search(strCustCd);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion

        #region 우측 상세검색
        private void Right_Search(string strScode)
        {
            try
            {
                //현재 row값 설정
                PreRow = fpSpread1.ActiveSheet.ActiveRowIndex;

                string strSql = " usp_BBC001  'S2' ";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "'";
                strSql = strSql + ", @pCUST_CD = '" + strScode + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                if (ds.Tables[0].Rows[0]["CUST_TYPE"].ToString() != "") cboCustType.SelectedValue = ds.Tables[0].Rows[0]["CUST_TYPE"].ToString();
                txtCustCd.Value = ds.Tables[0].Rows[0]["CUST_CD"].ToString();
                txtCustNm.Value = ds.Tables[0].Rows[0]["CUST_NM"].ToString();
                txtCustFullNm.Value = ds.Tables[0].Rows[0]["CUST_FULL_NM"].ToString();
                txtCustEngNm.Value = ds.Tables[0].Rows[0]["CUST_ENG_NM"].ToString();
                txtRgstNo.Value = ds.Tables[0].Rows[0]["RGST_NO"].ToString();
                txtRepreNm.Value = ds.Tables[0].Rows[0]["REPRE_NM"].ToString();
                txtCorpRgstNo.Value = ds.Tables[0].Rows[0]["CORP_RGST_NO"].ToString();
                dtpFoundDt.Value = ds.Tables[0].Rows[0]["FOUND_DT"].ToString().Substring(0, 10);
                dtpApplyDt.Value = ds.Tables[0].Rows[0]["APPLY_DT"].ToString().Substring(0, 10);
                txtInduType.Value = ds.Tables[0].Rows[0]["INDU_TYPE"].ToString();
                txtInduKind.Value = ds.Tables[0].Rows[0]["INDU_KIND"].ToString();
                txtZipCd.Value = ds.Tables[0].Rows[0]["ZIPCODE"].ToString();
                txtAddr1.Value = ds.Tables[0].Rows[0]["ADDR1"].ToString();
                txtAddr2.Value = ds.Tables[0].Rows[0]["ADDR2"].ToString();
                txtAddrEng1.Value = ds.Tables[0].Rows[0]["ADDR1_ENG"].ToString();
                txtAddrEng2.Value = ds.Tables[0].Rows[0]["ADDR2_ENG"].ToString();
                txtAddrEng3.Value = ds.Tables[0].Rows[0]["ADDR3_ENG"].ToString();
                if (ds.Tables[0].Rows[0]["NAT_CD"].ToString() != "") cboNatCd.SelectedValue = ds.Tables[0].Rows[0]["NAT_CD"].ToString();
                txtFax.Value = ds.Tables[0].Rows[0]["FAX"].ToString();
                txtTel1.Value = ds.Tables[0].Rows[0]["TEL1"].ToString();
                txtTel2.Value = ds.Tables[0].Rows[0]["TEL2"].ToString();
                txtChargeNm.Value = ds.Tables[0].Rows[0]["CHARGE_NM"].ToString();
                txtChargeTel.Value = ds.Tables[0].Rows[0]["CHARGE_TEL"].ToString();
                if (ds.Tables[0].Rows[0]["USE_FLAG"].ToString() == "1") chkUseFlag.Checked = true;
                else chkUseFlag.Checked = false;
                if (ds.Tables[0].Rows[0]["AREA_CD"].ToString() != "") cboAreaCd.SelectedValue = ds.Tables[0].Rows[0]["AREA_CD"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_GRP"].ToString() != "") cboCustGrp.SelectedValue = ds.Tables[0].Rows[0]["CUST_GRP"].ToString();
                txtHome.Value = ds.Tables[0].Rows[0]["HOME_URL"].ToString();
                txtEmail.Value = ds.Tables[0].Rows[0]["E_MAIL"].ToString();
                if (ds.Tables[0].Rows[0]["TRADE_TYPE"].ToString() != "") cboTradeType.SelectedValue = ds.Tables[0].Rows[0]["TRADE_TYPE"].ToString();
                if (ds.Tables[0].Rows[0]["CRED_FLAG"].ToString() == "1") chkCred.Checked = true;
                else chkCred.Checked = false;
                txtCredGrp.Value = ds.Tables[0].Rows[0]["CRED_GRP"].ToString();
                txtCredGrpNm.Value = ds.Tables[0].Rows[0]["CRED_GRP_NM"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_GRADE"].ToString() != "") cboGrade.SelectedValue = ds.Tables[0].Rows[0]["CUST_GRADE"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_TYPE1"].ToString() != "") cboType1.SelectedValue = ds.Tables[0].Rows[0]["CUST_TYPE1"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_TYPE2"].ToString() != "") cboType2.SelectedValue = ds.Tables[0].Rows[0]["CUST_TYPE2"].ToString();
                if (ds.Tables[0].Rows[0]["CUST_TYPE3"].ToString() != "") cboType3.SelectedValue = ds.Tables[0].Rows[0]["CUST_TYPE3"].ToString();
                if (ds.Tables[0].Rows[0]["BILL_ISS_TYPE"].ToString() != "") cboBillIssType.SelectedValue = ds.Tables[0].Rows[0]["BILL_ISS_TYPE"].ToString();
                if (ds.Tables[0].Rows[0]["LIMIT_CHK"].ToString() != "") cboLimitChk.SelectedValue = ds.Tables[0].Rows[0]["LIMIT_CHK"].ToString();
                if (ds.Tables[0].Rows[0]["CURR_CTL"].ToString() != "") cboCurrCtl.SelectedValue = ds.Tables[0].Rows[0]["CURR_CTL"].ToString();
                if (ds.Tables[0].Rows[0]["DELIV_TYPE"].ToString() != "") cboDelivType.SelectedValue = ds.Tables[0].Rows[0]["DELIV_TYPE"].ToString();
                if (ds.Tables[0].Rows[0]["BILL_SUM_TYPE"].ToString() != "") cboBillSumType.SelectedValue = ds.Tables[0].Rows[0]["BILL_SUM_TYPE"].ToString();
                if (ds.Tables[0].Rows[0]["AUTO_SHIP"].ToString() == "1") chkAutoShip.Checked = true;
                else chkAutoShip.Checked = false;
                txtChargeDept.Value = ds.Tables[0].Rows[0]["CHARGE_DEPT"].ToString();
                txtChargePosition.Value = ds.Tables[0].Rows[0]["CHARGE_POSITION"].ToString();

                chkScmYN.Checked = Convert.ToBoolean(ds.Tables[0].Rows[0]["SCM_YN"].ToString());
                txtScmPw.Value = SystemBase.Base.DeCode(ds.Tables[0].Rows[0]["SCM_PW"].ToString());

                txtBillId.Value = ds.Tables[0].Rows[0]["BILL_ID"].ToString();
                txtBankAcctNo.Value = ds.Tables[0].Rows[0]["BANK_ACCT_NO"].ToString();

                // 2022.03.14. hma 추가(Start): 이체계좌정보
                txtTransNm.Value = ds.Tables[0].Rows[0]["TRANS_NM"].ToString();
                txtBankCd.Value = ds.Tables[0].Rows[0]["BANK_CD"].ToString();
                txtBankNm.Value = ds.Tables[0].Rows[0]["BANK_NM"].ToString();
                txtAcctNo.Value = ds.Tables[0].Rows[0]["ACCOUNT_NO"].ToString();
                txtAcctOwner.Value = ds.Tables[0].Rows[0]["ACCT_OWNER"].ToString();
                txtRemark.Value = ds.Tables[0].Rows[0]["REMARK"].ToString();
                dtpEffStartDt.Value = ds.Tables[0].Rows[0]["EFF_START_DT"].ToString();
                dtpEffEndDt.Value = ds.Tables[0].Rows[0]["EFF_END_DT"].ToString();
                chkUseYn.Checked = false;
                if (ds.Tables[0].Rows[0]["USE_YN"].ToString() == "Y")
                    chkUseYn.Checked = true;
                // 2022.03.14. hma 추가(End)

                SystemBase.Validation.GroupBox_SearchViewValidation(groupBox2); //Key값 컨트롤 세팅

                //컨트롤 체크값 초기화
                strSearchData = "";
                //컨트롤 체크 함수
                GroupBox[] gBox = new GroupBox[] { groupBox2, groupBox3, groupBox5, groupBox6, groupBox7 };
                SystemBase.Validation.Control_Check(gBox, ref strSearchData);

                #region 컨트롤 활성화 유무
                Control_Enable("N");
                #endregion

            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 컨트롤 활성화 유무
        private void Control_Enable(string UseYn)
        {
            if (UseYn == "Y")
            {
                //거래처코드
                txtCustCd.ReadOnly = false;
                txtCustCd.BackColor = SystemBase.Validation.Kind_LightCyan;
                //거래처구분
                cboCustType.ReadOnly = false;
                cboCustType.EditorBackColor = SystemBase.Validation.Kind_LightCyan;
                //거래처명
                txtCustNm.ReadOnly = false;
                txtCustNm.BackColor = SystemBase.Validation.Kind_LightCyan;
                //거래처전명
                txtCustFullNm.ReadOnly = false;
                txtCustFullNm.BackColor = SystemBase.Validation.Kind_LightCyan;
                //사업자등록번호
                txtRgstNo.ReadOnly = false;
                txtRgstNo.BackColor = SystemBase.Validation.Kind_LightCyan;
                //대표자명
                txtRepreNm.ReadOnly = false;
                txtRepreNm.BackColor = SystemBase.Validation.Kind_LightCyan;
                //적용시작일
                dtpApplyDt.ReadOnly = false;
                dtpApplyDt.BackColor = SystemBase.Validation.Kind_LightCyan;
                //업태
                txtInduType.ReadOnly = false;
                txtInduType.BackColor = SystemBase.Validation.Kind_LightCyan;
                //업종
                txtInduKind.ReadOnly = false;
                txtInduKind.BackColor = SystemBase.Validation.Kind_LightCyan;
                //우편번호
                cmdZipCode.Enabled = true;
                //주소
                txtAddr2.ReadOnly = false;
                //txtAddr2.BackColor = SystemBase.Validation.Kind_LightCyan;
            }
            else
            {
                //거래처코드
                txtCustCd.ReadOnly = true;
                txtCustCd.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //거래처구분
                cboCustType.ReadOnly = true;
                cboCustType.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;
                //거래처명
                txtCustNm.ReadOnly = true;
                txtCustNm.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //거래처전명
                txtCustFullNm.ReadOnly = true;
                txtCustFullNm.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //사업자등록번호
                txtRgstNo.ReadOnly = true;
                txtRgstNo.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //대표자명
                txtRepreNm.ReadOnly = true;
                txtRepreNm.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //적용시작일
                dtpApplyDt.ReadOnly = true;
                dtpApplyDt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //업태
                txtInduType.ReadOnly = true;
                txtInduType.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //업종
                txtInduKind.ReadOnly = true;
                txtInduKind.BackColor = SystemBase.Validation.Kind_Gainsboro;
                //우편번호
                cmdZipCode.Enabled = false;
                //주소
                txtAddr2.ReadOnly = true;
                //txtAddr2.BackColor = SystemBase.Validation.Kind_Gainsboro;
            }
        }
        #endregion

        #region txtCustNm Focus Leave시
        private void txtCustNm_Leave(object sender, System.EventArgs e)
        {
            if (txtCustFullNm.Text == "") txtCustFullNm.Text = txtCustNm.Text;
        }
        #endregion

        #region txtCustCd TextChanged시
        private void txtCustCd_TextChanged(object sender, System.EventArgs e)
        {
            string strCode = "";
            strCode = txtCustCd.Text;
            Right_Search(strCode);
        }
        #endregion
        
        #region 리셋
        private void Combo_Reset()
        {
            SystemBase.ComboMake.C1Combo(cboCustType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//거래처구분
            SystemBase.ComboMake.C1Combo(cboSCustType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B005', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 3);//거래처구분
            SystemBase.ComboMake.C1Combo(cboNatCd, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B006', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//국가구분
            SystemBase.ComboMake.C1Combo(cboAreaCd, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B010', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//지역코드
            SystemBase.ComboMake.C1Combo(cboCustGrp, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B009', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ");//거래처분류
            SystemBase.ComboMake.C1Combo(cboTradeType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B011', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//거래유형
            SystemBase.ComboMake.C1Combo(cboGrade, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B012', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//업체평가등급
            SystemBase.ComboMake.C1Combo(cboType1, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B013', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//업체분류1
            SystemBase.ComboMake.C1Combo(cboType2, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B014', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//업체분류2
            SystemBase.ComboMake.C1Combo(cboType3, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B015', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//업체분류3
            SystemBase.ComboMake.C1Combo(cboBillIssType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B040', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//VAT유형
            SystemBase.ComboMake.C1Combo(cboLimitChk, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'ZC01', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//한도적용시기
            SystemBase.ComboMake.C1Combo(cboCurrCtl, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'ZC02', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//원단위관리
            SystemBase.ComboMake.C1Combo(cboDelivType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B020', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//출고형태
            SystemBase.ComboMake.C1Combo(cboBillSumType, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B021', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 9);//계산서합산기준
        }

        // 2022.04.18. hma 추가(Start): 은행코드 팝업 및 코드 입력 처리
        #region btnBank_Click(): 은행코드 검색버튼 클릭시. 은행조회 팝업 띄우기
        private void btnBank_Click(object sender, EventArgs e)
        {
            try
            {
                string strBANK_CD = txtBankCd.Text;

                string strQuery = " usp_B_COMMON @pType = 'B070', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtBankCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BZB005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "은행 조회");
                pu.Width = 800;
                pu.Height = 800;
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBankCd.Value = Msgs[0].ToString();
                    txtBankNm.Value = Msgs[1].ToString();
                    txtBankCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "은행 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region txtBankCd_TextChanged(): 은행코드 항목 입력시. 해당 은행코드에 대한 은행명을 보여준다.
        private void txtBankCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBankCd.Text.Trim() != "")
                {
                    txtBankNm.Value = SystemBase.Base.CodeName("BANK_CD", "BANK_NM", "B_BANK", txtBankCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (txtBankCd.Text.Trim() == "")
                {
                    txtBankNm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion
        // 2022.04.18. hma 추가(End)

        private void Group_Reset()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);
            SystemBase.Validation.GroupBox_Reset(groupBox5);
            SystemBase.Validation.GroupBox_Reset(groupBox6);
            SystemBase.Validation.GroupBox_Reset(groupBox7);
            SystemBase.Validation.GroupBox_Reset(grpAcctNoInfo);        // 2022.04.18. hma 추가: 이체정보 탭
        }
        #endregion
        
    }
}
