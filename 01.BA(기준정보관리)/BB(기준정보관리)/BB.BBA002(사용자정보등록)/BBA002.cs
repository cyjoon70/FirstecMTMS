using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace BB.BBA002
{
    public partial class BBA002 : UIForm.FPCOMM1
    {
        #region Field
        string UserId = null;
        string UserNm = null;
        #endregion

        #region 생성자
        public BBA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBA002_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboUSE_FLAG, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B029', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", true);

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "접수담당사업장")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE='BIZ2', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'", 1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                //부서조회
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드_2"))
                {
                    string strQuery = " usp_B_COMMON 'D010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += " , @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += " , @pREORG_ID = '" + SystemBase.Base.gstrREORG_ID + "' ";

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서코드 조회");
                    pu.Width = 500;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text = Msgs[0].ToString();	//부서코드
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = Msgs[1].ToString();	//부서명
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text = Msgs[3].ToString();	//개편ID

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }

                //공장(사업장 소속)
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드_2"))
                {
                    string strQuery = " usp_B_COMMON 'B034', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text, "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "공장명")].Text = Msgs[1].ToString();

                        UIForm.FPMake.fpChange(fpSpread1, Row);//수정플래그
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false,false, 0, 0);
            txtUser.Focus();
        }
        #endregion

        #region 행추가 버튼 클릭 이벤트
        protected override void RowInsExec()
        {
            UIForm.FPMake.RowInsert(fpSpread1);

            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text = "True";

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BBA002  'S1'";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                strQuery = strQuery + ", @pUSR_ID ='" + txtUser.Text + "' ";
                strQuery = strQuery + ", @pUSR_NM ='" + txtUsernm.Text + "' ";
                strQuery = strQuery + ", @pUSE_FLAG ='" + cboUSE_FLAG.SelectedValue.ToString() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                if (fpSpread1.ActiveSheet.Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 3].Text != "" && fpSpread1.Sheets[0].Cells[i, 4].Text != "")
                        {
                            fpSpread1.Sheets[0].Cells[i, 3].Value = SystemBase.Base.DeCode(fpSpread1.Sheets[0].Cells[i, 3].Value.ToString());
                            fpSpread1.Sheets[0].Cells[i, 4].Value = SystemBase.Base.DeCode(fpSpread1.Sheets[0].Cells[i, 4].Value.ToString());
                        }
                    }
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1,this.Name, "fpSpread1", true))// 그리드 상단 필수항목 체크
            {
                string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.

                string strUSR_ID = "";
                string strResultMsg = "";       // 2022.05.02. hma 추가

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    // 2021.11.02. hma 추가(Start)
                    string QueryPwd = "";
                    string strCurPwd = "";
                    string strNewPwd = "";
                    string strPwdChgYn = "";
                    DataTable dtPwd;
                    // 2021.11.02. hma 추가(End)
                    string strSlipRcvYn = "";           // 2022.01.14. hma 추가
                    string strSlipRcvBiz = "";          // 2022.05.02. hma 추가
                    string strDiligYn = "";             // 2022.07.13. hma 추가: 근태관리여부

                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            strUSR_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text.ToString();
                            string strUSR_NM = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text.ToString();
                            string strPWD1 = SystemBase.Base.EnCode(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호")].Value.ToString());

                            if (i != 0)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호")].Text.ToString()
                                    != fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호확인")].Text.ToString())
                                {
                                    string msg = SystemBase.Base.MessageRtn("B0028");	//비밀번호가 일치하지 않습니다.
                                    SystemBase.MessageBoxComm.Show(msg);
                                    return;
                                }
                            }

                            // 2021.11.02. hma 추가(Start): 변경 저장하는 경우 비밀번호 변경여부 체크를 위해 기존 비밀번호
                            strPwdChgYn = "N";
                            if (strGbn == "U1")
                            {
                                QueryPwd = "";
                                QueryPwd = "EXEC usp_BBA002 @pType = 'S4', @pUSR_ID = '" + strUSR_ID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                                dtPwd = SystemBase.DbOpen.NoTranDataTable(QueryPwd);

                                strNewPwd = "";
                                strNewPwd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호")].Value.ToString();

                                strCurPwd = "";
                                if (dtPwd.Rows.Count > 0)
                                    strCurPwd = SystemBase.Base.DeCode(dtPwd.Rows[0][0].ToString());

                                if ((strCurPwd != "") && (strCurPwd != strNewPwd))
                                    strPwdChgYn = "Y";
                            }
                            // 2021.11.02. hma 추가(End)

                            string strDEPT_CD = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서코드")].Text.ToString();
                            string strREORG_ID = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text.ToString();
                            string strPlant_Cd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString();
                            string strInt_Tel = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구내전화")].Text.ToString();
                            string strEmail = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "E-MAIL")].Text.ToString();
                            string strEmpNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원코드")].Text.ToString();      // 2021.11.30. hma 추가

                            string strUSE_DT = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "유효일")].Text.ToString();
                            string strTRAN_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수불담당유무")].Text == "True") { strTRAN_YN = "Y"; }

                            string strROU_DEV_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정설계담당")].Text == "True") { strROU_DEV_YN = "Y"; }
                            string strROU_MFG_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정생산담당")].Text == "True") { strROU_MFG_YN = "Y"; }
                            string strROU_QUR_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정품질담당")].Text == "True") { strROU_QUR_YN = "Y"; }
                            string strROU_APP_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공정확인자")].Text == "True") { strROU_APP_YN = "Y"; }
                            string strBOM_DEV_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM설계담당")].Text == "True") { strBOM_DEV_YN = "Y"; }
                            string strBOM_MFG_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM생산담당")].Text == "True") { strBOM_MFG_YN = "Y"; }
                            string strBOM_QUR_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM품질담당")].Text == "True") { strBOM_QUR_YN = "Y"; }
                            string strBOM_APP_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "BOM확인자")].Text == "True") { strBOM_APP_YN = "Y"; }
                            string strASSIGN_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "알리미여부")].Text == "True") { strASSIGN_YN = "Y"; }
                            string strHUMAN_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "인사관리여부")].Text == "True") { strHUMAN_YN = "Y"; }
                            string strATTEND_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태출퇴근미적용")].Text == "True") { strATTEND_YN = "Y"; }
                            string strACCT_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계관리여부")].Text == "True") { strACCT_YN = "Y"; }
                            string strDEV_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개발작업일보사용")].Text == "True") { strDEV_YN = "Y"; }

                            // 2018.01.09. hma 추가(Start)
                            string strMANAGER_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "매니저여부")].Text == "True") { strMANAGER_YN = "Y"; }
                            // 2018.01.09. hma 추가(End)

                            // 2018.02.23. hma 추가(Start)
                            string strINSA_YN = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "인사정보송부여부")].Text == "True") { strINSA_YN = "Y"; }
                            // 2018.02.23. hma 추가(End)

                            // 2022.01.14. hma 추가(Start)
                            strSlipRcvYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표접수자")].Text == "True") { strSlipRcvYn = "Y"; }
                            // 2022.01.14. hma 추가(End)

                            // 2022.05.02. hma 추가(Start)
                            strSlipRcvBiz = "";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수담당사업장")].Text != "")
                            {
                                strSlipRcvBiz = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "접수담당사업장")].Value.ToString();
                            }
                            // 2022.05.02. hma 추가(End)

                            // 2022.07.13. hma 추가(Start)
                            strDiligYn = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태관리여부")].Text == "True") { strDiligYn = "Y"; }
                            // 2022.07.13. hma 추가(End)

                            string strIpFlag = "0";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "IP체크")].Text == "True") { strIpFlag = "1"; }
                            string strIpAddress = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "IP")].Text.ToString();
                            string strMacFlag = "0";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "랜카드체크")].Text == "True") { strMacFlag = "1"; }
                            string strMacAddress = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Mac Address")].Text.ToString();

                            string strSql = " usp_BBA002 '" + strGbn + "'";
                            strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
                            strSql = strSql + ", @pUSR_ID = '" + strUSR_ID.Trim() + "'";
                            strSql = strSql + ", @pUSR_NM = '" + strUSR_NM.Trim() + "'";
                            strSql = strSql + ", @pPWD = '" + strPWD1 + "'";
                            strSql = strSql + ", @pREORG_ID = '" + strREORG_ID.Trim() + "'";
                            strSql = strSql + ", @pDEPT_CD = '" + strDEPT_CD.Trim() + "'";
                            strSql = strSql + ", @pPLANT_CD = '" + strPlant_Cd.Trim() + "'";
                            strSql = strSql + ", @pUSE_DT = '" + strUSE_DT.Trim() + "'";
                            strSql = strSql + ", @pTRAN_YN = '" + strTRAN_YN + "'";
                            strSql = strSql + ", @pROU_DEV_YN = '" + strROU_DEV_YN + "'";
                            strSql = strSql + ", @pROU_MFG_YN = '" + strROU_MFG_YN + "'";
                            strSql = strSql + ", @pROU_QUR_YN = '" + strROU_QUR_YN + "'";
                            strSql = strSql + ", @pROU_APP_YN = '" + strROU_APP_YN + "'";
                            strSql = strSql + ", @pBOM_DEV_YN = '" + strBOM_DEV_YN + "'";
                            strSql = strSql + ", @pBOM_MFG_YN = '" + strBOM_MFG_YN + "'";
                            strSql = strSql + ", @pBOM_QUR_YN = '" + strBOM_QUR_YN + "'";
                            strSql = strSql + ", @pBOM_APP_YN = '" + strBOM_APP_YN + "'";
                            strSql = strSql + ", @pASSIGN_YN = '" + strASSIGN_YN + "'";
                            strSql = strSql + ", @pHUMAN_YN = '" + strHUMAN_YN + "'";
                            strSql = strSql + ", @pATTEND_YN = '" + strATTEND_YN + "'";
                            strSql = strSql + ", @pACCT_YN = '" + strACCT_YN + "'";
                            strSql = strSql + ", @pDEV_YN = '" + strDEV_YN + "'";
                            strSql = strSql + ", @pUNIERP_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "uniERP ID")].Text + "'";
                            strSql = strSql + ", @pUNIERP_PASS = '" + Convert.ToString(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "uniERP PASS")].Value) + "'";
                            strSql = strSql + ", @pIP_FLAG = '" + strIpFlag + "'";
                            strSql = strSql + ", @pIP_ADDRESS = '" + strIpAddress + "'";
                            strSql = strSql + ", @pMAC_FLAG = '" + strMacFlag + "'";
                            strSql = strSql + ", @pMAC_ADDRESS = '" + strMacAddress + "'";
                            strSql = strSql + ", @pINTERNAL_TEL = '" + strInt_Tel + "'";
                            strSql = strSql + ", @pEMAIL = '" + strEmail + "'";
                            strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pMANAGER_YN = '" + strMANAGER_YN + "'";   // 2018.01.09. hma 추가: 매니저여부
                            strSql = strSql + ", @pINSA_YN = '" + strINSA_YN + "'";         // 2018.02.23. hma 추가: 인사VIEW송부여부
                            strSql = strSql + ", @pPWD_CHG_YN = '" + strPwdChgYn + "'";     // 2021.11.02. hma 추가: 비밀번호변경여부
                            strSql = strSql + ", @pEMP_NO = '" + strEmpNo + "'";            // 2021.11.30. hma 추가: 사원코드
                            strSql = strSql + ", @pSLIP_RCV_YN = '" + strSlipRcvYn + "'";   // 2022.01.14. hma 추가: 전표접수자
                            strSql = strSql + ", @pSLIP_RCV_BIZ = '" + strSlipRcvBiz + "'"; // 2022.05.02. hma 추가: 전표접수사업장
                            strSql = strSql + ", @pDILIG_YN = '" + strDiligYn + "'";        // 2022.07.13. hma 추가: 근태관리여부

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            strResultMsg = MSGCode;     // 2022.05.02. hma 추가

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                        }
                    }

                    // 2022.05.02. hma 추가(Start): 사용자정보등록 전표접수자 및 전표접수담당사업장 체크
                    string strSql_Chk = " usp_BBA002 'C1'";
                    strSql_Chk = strSql_Chk + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql_Chk = strSql_Chk + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    DataSet ds_chk = SystemBase.DbOpen.TranDataSet(strSql_Chk, dbConn, Trans);
                    ERRCode = ds_chk.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds_chk.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "OK")
                        MSGCode = strResultMsg;

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    // 2022.05.02. hma 추가(End)

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002"; // 에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strUSR_ID);
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
        #endregion

        #region txtUser 변환시  txtUsernm 조회
        private void txtUser_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtUser.Text != "")
                {
                    txtUsernm.Text = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUser.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtUsernm.Text = "";
                }
            }
            catch { }
        }
        #endregion

        #region FrSpeadenu 컬럼 변환시 Name 조회
        private void fpSpread1_EditChange(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID"))
            {
                string strUsernm = SystemBase.Base.CodeName("USR_ID", "USR_NAME", "B_SYS_USER", fpSpread1.Sheets[0].Cells[e.Row, 1].Text.ToString(), " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text = strUsernm;
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호"))
            {
                string strPW1 = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호")].Text.ToString();
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호1")].Text = strPW1;
            }

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호확인"))
            {
                string strPW2 = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호확인")].Text.ToString();
                fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "비밀번호2")].Text = strPW2;
            }
        }
        #endregion

        #region 이력보기
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
            UserId = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text.ToString();
            UserNm = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text.ToString();

        }
        private void btnHistory_Click(object sender, EventArgs e)
        {
            // 2021.11.03. hma 수정(Start): 사용자ID 및 사용자명이 공백인 경우에도 팝업이 뜨도록 함.
            //if (string.IsNullOrEmpty(UserId)) return;
            if (string.IsNullOrEmpty(UserId))
            {
                UserId = "";
                UserNm = "";
            }
            // 2021.11.03. hma 수정(End)

            BBA002History pu = new BBA002History(UserId, UserNm);
            pu.ShowDialog();
        }
        #endregion

        // 2021.11.03. hma 추가(Start): 사용자ID 또는 사용자명에서 더블클릭시 해당 사용자에 대한 이력조회가 되도록 팝업 띄움.
        #region fpSpread1_CellDoubleClick()
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 사용자ID
                if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID") || e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명"))
                {
                    UserId = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자ID")].Text.ToString();
                    UserNm = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "사용자명")].Text.ToString();

                    BBA002History pu = new BBA002History(UserId, UserNm);
                    pu.ShowDialog();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion
        // 2021.11.03. hma 추가(End)
    }
}
