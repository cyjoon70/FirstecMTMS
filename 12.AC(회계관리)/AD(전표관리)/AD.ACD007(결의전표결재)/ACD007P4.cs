#region 작성정보
/*********************************************************************/
// 단위업무명: 전표조회(결재용)
// 작 성 자  : 한 미 애
// 작 성 일  : 2
// 작성내용  : 전표조회(결재용)
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  : ACD007P3 팝업 복사하여 생성. S/P는 그대로 usp_ACD007P3 사용.
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
using System.IO;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

#region 예제 - 복사해서 쓰세요
/*
try
{
    WNDW.ACD007P4 pu = new WNDW.ACD007P4(txtSLIP_NO.Text);
    pu.ShowDialog();
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace AD.ACD007
{
    /// <summary>
    /// 전표정보 조회
    /// <para>예제는 소스안에서 복사해쓰세요</para>
    /// </summary>
    public partial class ACD007P4 : UIForm.FPCOMM3
    {
        #region 변수선언
        int PreRow = -1;   // SelectionChanged 시에 동일 Row에서 데이타변환 처리 안하도록 하기 위함.
        string strSLIPNO = "";
        string strFINANCE_YN = "";
        string strADMIN_YN = "";
        string strASSIGN_ROLL_YN = "";      // 2022.02.10. hma 추가
        string strASSIGN_ID_MAN = "";       // 2022.02.10. hma 추가

        string strLastAssignIdYn = "";      // 최종승인자여부(접수부서)
        string strResultType = "";
        string strResultMsg = "";
        string[] returnVal = null;
        string strChgFlag = "";

        /// <summary>첨부파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;

        /// <summary>문서카테고리 코드</summary>
        const string docCtgCd = "ACD";          //전표증빙

        // 디테일 그리드 컬럼(문서 목록)
        int colDocId = -1;
        int colSvrPath = -1;
        int colSvrFnm = -1;
        int colOrgFnm = -1;
        int colDocCd = -1;
        int colDocNm = -1;
        int colDocNo = -1;
        int colRevNo = -1;
        int colRemark = -1;
        int colRegUsrId = -1;
        int colRegUsrNm = -1;
        #endregion

        public ACD007P4()
        {
            InitializeComponent();
        }

        public ACD007P4(string SLIP_NO, string ADMIN_YN, string FINANCE_YN, 
                        string ASSIGN_ROLL_YN, string ASSIGN_ID_MAN)      // 2022.02.10. hma 추가
        {
            strSLIPNO = SLIP_NO;
            strADMIN_YN = ADMIN_YN;
            strFINANCE_YN = FINANCE_YN;
            strASSIGN_ROLL_YN = ASSIGN_ROLL_YN;     // 2022.02.10. hma 추가: 대리결재여부
            strASSIGN_ID_MAN = ASSIGN_ID_MAN;       // 2022.02.10. hma 추가: 대리결재자ID

            InitializeComponent();
        }

        # region Method
        /// <summary>
        /// 임시로 다운로드한 파일을 모두 삭제합니다.
        /// </summary>
        void ViewDeleteTempFiles()
        {
            foreach (FileInfo f in new DirectoryInfo(Path.GetTempPath()).GetFiles(ViewGetTempFilenamePrefix() + "*.*")) // 프리픽스파일 모두 삭제
            {
                try { f.Delete(); }
                catch { }
            }
        }
        /// <summary>
        /// 임시파일명의 프리픽스로 사용할 고정된 문자열을 반환합니다.
        /// </summary>
        /// <returns></returns>
        string ViewGetTempFilenamePrefix()
        {
            return string.Format("{0:X}", this.GetHashCode()) + "_";
        }
        #endregion

        #region Form Load 시
        private void ACD007P4_Load(object sender, System.EventArgs e)
        {
            //버튼 재정의
            UIForm.Buttons.ReButton("000000000101", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            this.Text = "결의전표결재 > 전표조회";

            SystemBase.ComboMake.C1Combo(cboTaskType, "usp_B_COMMON @pType='COMM', @pCODE = 'B092', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");       // 업무구분
            SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);     // 그룹웨어상태
            SystemBase.ComboMake.C1Combo(cboAssignStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B093', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); // 결재상태

            G3Etc[SystemBase.Base.GridHeadIndex(GHIdx3, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'ACD', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류

            NewExec();

            txtSlipNo.Value = strSLIPNO;
            txtAdminRollYn.Value = strADMIN_YN;
            txtFinanceDeptYn.Value = strFINANCE_YN;            

            dtpGlSlipDt.Value = SystemBase.Base.ServerTime("YYMMDD");   // 현재일자로 기본적 세팅

            txtAssignId.Value = SystemBase.Base.gstrUserID;             // 결재자 사용자ID
            txtAssignNm.Value = SystemBase.Base.gstrUserName;           // 결재자명

            // 2022.02.10. hma 추가(Start): 대리결재자ID가 넘어왔으면 그 결재자ID를 결재자 항목에.(테스트용)
            if (strASSIGN_ROLL_YN == "Y")
                txtAssignId.Value = strASSIGN_ID_MAN;
            // 2022.02.10. hma 추가(End)

            SheetView sheet = fpSpread3.ActiveSheet;
            colDocId = sheet.FindHeaderColumnIndex("문서ID");
            colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
            colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
            colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3;     // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
            colDocCd = sheet.FindHeaderColumnIndex("문서코드");
            colDocNm = sheet.FindHeaderColumnIndex("문서종류");
            colDocNo = sheet.FindHeaderColumnIndex("문서번호");
            colRevNo = sheet.FindHeaderColumnIndex("개정번호");
            colRemark = sheet.FindHeaderColumnIndex("비고");
            colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
            colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

            buttonManager = new FileButtonManager(fpSpread3.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
            {
                ServerPathColumnIndex = colSvrPath,
                ServerFilenameColumnIndex = colSvrFnm,
                FileSelectButtonColumnIndex = colOrgFnm - 3,
                FileViewButtonColumnIndex = colOrgFnm - 2,
                FileDownloadButtonColumnIndex = colOrgFnm - 1,
                FilenameColumnIndex = colOrgFnm,
                DocTypeNameColumnIndex = colDocNm,
                DocRevisionColumnIndex = colRevNo,
                DocNumberColumnIndex = colDocNo,
            };

            strChgFlag = "N";           // 변경여부를 N으로.

            SEARCH_SLIP(strSLIPNO);     // 마스터 조회
            SearchExec();               // 상세 조회
            SEARCH_DOC(strSLIPNO);      // 지출증빙조회
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);
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
                    string strQuery = " usp_ACD007P3  'S2'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";

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

        #region 전표 마스터 조회
        private void SEARCH_SLIP(string SLIP_NO)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACD007P3 @pTYPE = 'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pSLIP_NO = '" + SLIP_NO + "' ";
                strQuery += ", @pASSIGN_ID = '" + txtAssignId.Text + "'";     // 결재자ID도 넘겨서 최종승인자 여부 체크하게.

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtCreatePath.Value = dt.Rows[0]["CREATE_PATH"].ToString();
                    txtRefNo.Value = dt.Rows[0]["REF_NO"].ToString();
                    txtSlipDt.Value = dt.Rows[0]["SLIP_DT"].ToString();
                    txtDeptCd.Value = dt.Rows[0]["DEPT_CD"].ToString();
                    txtDeptNm.Value = dt.Rows[0]["DEPT_NM"].ToString();
                    txtSlipAmt.Value = dt.Rows[0]["DR_AMT_LOC"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                    txtUserId.Value = dt.Rows[0]["USR_ID"].ToString();
                    txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                    txtAssignNo.Value = dt.Rows[0]["ASSIGN_NO"].ToString();
                    cboGwStatus.SelectedValue = dt.Rows[0]["GW_STATUS"].ToString();
                    cboTaskType.SelectedValue = dt.Rows[0]["TASK_TYPE"].ToString();
                    cboAssignStatus.SelectedValue = dt.Rows[0]["ASSIGN_STATUS"].ToString();
                    dtpAssignDt.Value = dt.Rows[0]["ASSIGN_DT"].ToString();
                    txtComment.Text = dt.Rows[0]["COMMENT"].ToString();
                    strLastAssignIdYn = dt.Rows[0]["LAST_APPR_YN"].ToString();

                    if (cboGwStatus.SelectedValue.ToString() == "APPR")             // 결의전표가 승인된 경우 회계전표일자 항목에 해당일자로 들어가도록 함.
                        dtpGlSlipDt.Value = dt.Rows[0]["GL_SLIP_DT"].ToString();
                    else
                        dtpGlSlipDt.Value = dt.Rows[0]["ASSIGN_GL_SLIP_DT"].ToString();     // 2022.03.02. hma 추가: 결재선마스터의 회계전표일자로 조회되게.

                    txtAssignComment.ReadOnly = false;
                    txtAssignComment.Text = dt.Rows[0]["COMMENT_LINE"].ToString(); // 2022.02.04.  hma 추가: 결재자 코멘트
                    txtAssignComment.ReadOnly = true;

                    txtAssignNm.Value = dt.Rows[0]["MAN_ASSIGN_NM"].ToString();    // 2022.02.10. hma 추가: 대리결재자명
                }
                else
                {
                    NewExec();
                }

                // 2022.03.02. hma 추가(Start): 대리결재자로 처리하는 경우도 있어서 관리자권한 및 재무팀여부를 다시 체크함.
                strQuery = " usp_ASSIGN_DIALOG @pTYPE = 'C1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pUSR_ID = '" + txtAssignId.Text.Trim() + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtAdminRollYn.Value = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                    txtFinanceDeptYn.Value = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();

                    txtAdminRollYn.Value = ds.Tables[0].Rows[0]["ADMIN_YN"].ToString();
                    txtFinanceDeptYn.Value = ds.Tables[0].Rows[0]["FINANCE_YN"].ToString();
                }
                else
                {
                    txtAdminRollYn.Value = "";
                    txtFinanceDeptYn.Value = "";
                }
                // 2022.03.02. hma 추가(End)

                AssignCtrlEnable();     // 승인/반려 버튼 및 결재코멘트, 전표일자 활성화/비활성화 처리
                
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region AssignCtrlEnable(): 승인/반려 버튼 및 결재코멘트, 전표일자 활성화/비활성화 처리
        private void AssignCtrlEnable()
        {
            // 결재상태가 대기인 경우에만 승인/반려 버튼 활성화 처리.
            if (cboAssignStatus.SelectedValue.ToString() == "WAIT")
            {
                btnApprv.Enabled = true;
                btnReject.Enabled = true;

                txtComment.Enabled = true;
                dtpGlSlipDt.Enabled = false;
            }
            else
            {
                btnApprv.Enabled = false;
                btnReject.Enabled = false;

                txtComment.Enabled = false;
                dtpGlSlipDt.Enabled = false;
            }

            // 2022.03.02. hma 수정(Start): 결재가 승인 상태가 아닌 경우, 재무팀이면 활성화되도록 함. 최종승인자 이전에도 회계전표일자 변경 가능하도록.
            // 최종결재자인 경우 전표일자 항목 활성화.
            //if (strLastAssignIdYn == "Y")
            //{
            //    dtpGlSlipDt.Enabled = true;
            //    //dtpGlSlipDt.Visible = true;
            //}
            //else
            //{
            //    dtpGlSlipDt.Enabled = false;
            //    //dtpGlSlipDt.Visible = false;
            //}
            if (cboGwStatus.SelectedValue.ToString() != "APPR") 
            {
                if (txtFinanceDeptYn.Text == "Y")
                    dtpGlSlipDt.Enabled = true;
                else
                    dtpGlSlipDt.Enabled = false;
            }
            else
            {
                dtpGlSlipDt.Enabled = false;
            }
            // 2022.03.02. hma 수정(End)

            // 2022.04.26. hma 추가(Start): 회계예정일 항목이 활성화된 경우, 전표결재상태가 승인이 아니고 다음 결재자가 결재를 안한 상태이면 저장 버튼 활성화
            btnGlSlipDtSave.Enabled = false;
            btnGlSlipDtSave.Visible = false;

            if ((dtpGlSlipDt.Enabled == true) && (cboAssignStatus.SelectedValue.ToString() == "COMPLETE"))
            {
                string strQuery = " usp_ACD007P3 @pTYPE = 'C2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                strQuery += ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                strQuery += ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                if (ds.Tables[0].Rows[0]["RESULT_CODE"].ToString() == "OK")
                {
                    btnGlSlipDtSave.Enabled = true;
                    btnGlSlipDtSave.Visible = true;
                }
            }
            // 2022.04.26. hma 추가(End)

            // 2022.05.20. hma 추가(Start): 회계예정일 항목이 활성화되었거나(즉 재무팀이거나) ADMIN이면서, 전표결재상태가 승인이 아니고 다음 결재자가 결재를 안한 상태이면 결재승인취소 버튼 활성화
            btnApprvCancel.Enabled = false;
            btnApprvCancel.Visible = false;

            if ((dtpGlSlipDt.Enabled == true || SystemBase.Base.gstrUserID == "ADMIN") && (cboAssignStatus.SelectedValue.ToString() == "COMPLETE"))
            {
                string strQuery = " usp_ACD007P3 @pTYPE = 'C3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                strQuery += ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                strQuery += ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                if (ds.Tables[0].Rows[0]["RESULT_CODE"].ToString() == "OK")
                {
                    btnApprvCancel.Enabled = true;
                    btnApprvCancel.Visible = true;
                }
            }
            // 2022.05.20. hma 추가(End)

        }
        #endregion

        #region 전표정보 그리드 선택
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
                string strSLIP_SEQ = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx2, "순번")].Text;
                CTRL_SEARCH(strSLIP_SEQ);
                PreRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 관리항목정보 조회
        private void CTRL_SEARCH(string SLIP_SEQ)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACD007P3  'S3'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pSLIP_NO ='" + txtSlipNo.Text + "' ";
                strQuery = strQuery + ", @pSLIP_SEQ ='" + SLIP_SEQ + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SEARCH_DOC() 전표지출증빙 조회
        private void SEARCH_DOC(string SLIP_NO)
        {
            try
            {
                string query = "usp_T_DOC 'S1'"
                    + ", @pDOC_CTG_CD = 'ACD'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pATT_KEY = '" + txtSlipNo.Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread3, query, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);
                buttonManager.UpdateButtons(); // 버튼 업데이트

                SheetView sheet = fpSpread1.ActiveSheet;
                //((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            //pdfViewer.LoadFile("about:blank");        // 2022.02.25. hma 수정: 주석 처리
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SEARCH_ASSIGN_DATA(): 결재 관련 항목들을 가져와서 항목에 넣어준다.
        private void SEARCH_ASSIGN_DATA()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //상세조회 SQL
                string strQuery = " usp_ACD007P3 @pTYPE = 'S4'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pASSIGN_NO = '" + txtAssignNo.Text + "' ";
                strQuery += ", @pASSIGN_ID = '" + txtAssignId.Text + "'"; 

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtUserId.Value = dt.Rows[0]["USR_ID"].ToString();
                    txtUserNm.Value = dt.Rows[0]["USR_NM"].ToString();
                    txtAssignNo.Value = dt.Rows[0]["ASSIGN_NO"].ToString();
                    cboGwStatus.SelectedValue = dt.Rows[0]["GW_STATUS"].ToString();
                    cboTaskType.SelectedValue = dt.Rows[0]["TASK_TYPE"].ToString();
                    cboAssignStatus.SelectedValue = dt.Rows[0]["ASSIGN_STATUS"].ToString();
                    dtpAssignDt.Value = dt.Rows[0]["ASSIGN_DT"].ToString();
                    txtComment.Value = dt.Rows[0]["COMMENT"].ToString();        // 2022.05.20. hma 수정: txtComment.Text => txtComment.Value로 변경
                    strLastAssignIdYn = dt.Rows[0]["LAST_APPR_YN"].ToString();
                }

                AssignCtrlEnable();     // 승인/반려 버튼 및 결재코멘트, 전표일자 활성화/비활성화 처리
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region PrintExec() PRINT 버튼 클릭 이벤트
        protected override void PrintExec()
        {
            try
            {
                if (txtSlipNo.Text == "")
                {
                    MessageBox.Show("전표를 조회 후 출력하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string RptName = SystemBase.Base.ProgramWhere + @"\Report\ACD001.rpt";    // 레포트경로+레포트명
                    string[] RptParmValue = new string[4];   // SP 파라메타 값

                    RptParmValue[0] = "P1";
                    RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                    RptParmValue[2] = txtSlipNo.Text;
                    if (rdoPrintT.Checked == true) RptParmValue[3] = "T";
                    else RptParmValue[3] = "G";

                    UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, null, RptName, RptParmValue); //공통크리스탈 10버전
                    //UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", null, RptName, RptParmValue);	//공통크리스탈 10버전
                    frm.ShowDialog();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void fpSpread3_CellClick(object sender, CellClickEventArgs e)
        {

        }

        private void btnAssignLine_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSlipNo.Text == "")
                {
                    MessageBox.Show("전표 데이터를 저장후 조회하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 결재라인 팝업 띄움. 업무구분,부서,상신자를 매개변수로 넘김
                ACD007P1 pu = new ACD007P1(txtSlipNo.Text, cboGwStatus.SelectedValue.ToString(), cboTaskType.SelectedValue.ToString(), txtAssignNo.Text,
                                            txtDeptCd.Text, txtDeptNm.Text, txtUserId.Text, txtUserNm.Text, strADMIN_YN, strFINANCE_YN);

                pu.ShowDialog();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region btnApprv_Click(): 승인 버튼 클릭 이벤트. 결재 승인 처리
        private void btnApprv_Click(object sender, EventArgs e)
        {
            // 2022.03.18. hma 수정(Start): 결재 승인여부 체크. 여기에서 체크하도록 아래에서 위치 이동.
            string strMsg = "";

            if (strLastAssignIdYn == "N")
                strMsg = "결재승인 하시겠습니까?";
            else
                strMsg = dtpGlSlipDt.Text + "일자 회계전표 생성과 함께 결재승인 하시겠습니까?";

            if (MessageBox.Show(strMsg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            // 2022.03.18. hma 수정(End)

            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.

            // 데이터 체크
            Data_Check();
            if (strResultType != "OK")
            {
                MessageBox.Show(strResultMsg);
                this.Cursor = Cursors.Default;
                return;
            }

            // 2022.03.17. hma 추가(Start): 최종승인자 경우 회계전표일자가 현재일자보다 크면 메시지 띄우고 처리 안되게.
            if (strLastAssignIdYn == "Y")
            {
                string strCurDate = "";
                string strSlipResDate = "";
                strCurDate = SystemBase.Base.ServerTime("YYMMDD");
                strSlipResDate = dtpGlSlipDt.Text;

                if (Convert.ToInt32(strCurDate.Replace("-", "")) < Convert.ToInt32(strSlipResDate.Replace("-", "")))
                {
                    MessageBox.Show("회계전표일자가 현재일자보다 미래일자이므로 승인할 수 없습니다.");
                    this.Cursor = Cursors.Default;
                    return;
                }
            }
            // 2022.03.17. hma 추가(End)

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //// 최종승인인 경우 전표일자도 확인 메시지에 보여준다.   // 2022.03.18. hma 수정: 위치를 위로 이동
                //if (strLastAssignIdYn == "N")
                //    strMsg = "결재승인 하시겠습니까?";
                //else
                //    strMsg = dtpGlSlipDt.Text + "일자 회계전표 생성과 함께 결재승인 하시겠습니까?";

                //// 결재승인 처리
                //if (MessageBox.Show(strMsg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                //{
                    // 결재승인 호출
                    string strSql_Appr = " usp_ACD007P3 @pTYPE = 'U1' ";
                    strSql_Appr = strSql_Appr + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql_Appr = strSql_Appr + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                    strSql_Appr = strSql_Appr + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                    strSql_Appr = strSql_Appr + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                    strSql_Appr = strSql_Appr + ", @pASSIGN_COMMENT = '" + txtComment.Text + "'";
                    // 2022.03.02. hma 추가(Start): 재무팀이거나 관리자권한그룹인 경우 회계전표일자도 저장되게.
                    // 2022.05.10. hma 수정(Start): 최종결재자인 경우에도 회계예정일 저장되게 함.
                    //if (txtFinanceDeptYn.Text == "Y")        // txtAdminRollYn.Text == "Y" ||  2022.03.14. hma 수정: 관리자권한인 경우에는 회계전표일자 저장 안되게 제외함.
                    if ((txtFinanceDeptYn.Text == "Y") || (strLastAssignIdYn == "Y"))
                    // 2022.05.10. hma 수정(End)
                        strSql_Appr = strSql_Appr + ", @pASSIGN_GL_SLIP_DT = '" + dtpGlSlipDt.Text + "'";
                    else
                        strSql_Appr = strSql_Appr + ", @pASSIGN_GL_SLIP_DT = ''";
                    // 2022.03.02. hma 추가(End)

                    DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql_Appr, dbConn, Trans);
                    ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    // 접수부서 승인인 경우 전표승인 처리
                    if (strLastAssignIdYn == "Y")
                    {
                        string strSql = " usp_ACD003 'U1'";     // 결의전표승인
                        strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pSLIP_NO = '" + txtSlipNo.Text + "' ";
                        strSql = strSql + ", @pSLIP_DT = '" + dtpGlSlipDt.Text + "' ";      // 전표일자(회계전표)
                        strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                        strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }

                    Trans.Commit();
                //}
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                SEARCH_ASSIGN_DATA();       // 결재관련 항목 조회
                strChgFlag = "Y";           // 변경여부를 Y로.                

                RtnStr(txtAssignNo.Text, cboAssignStatus.SelectedValue.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
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

        private void Data_Check()
        {
            strResultType = "";
            strResultMsg = "";

            // 승인하기 위한 데이터 체크
            string strSql = " usp_ACD007P3 'C1' ";
            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
            strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                strResultType = dt.Rows[0]["RESULT_CODE"].ToString();
                strResultMsg = dt.Rows[0]["RESULT_MSG"].ToString();
            }
        }

        #region btnApprv_Click(): 반려 버튼 클릭 이벤트. 결재 반려 처리
        private void btnReject_Click(object sender, EventArgs e)
        {
            // 2022.03.18. hma 수정(Start): 결재반려 처리 체크. 여기에서 체크하도록 아래에서 위치 이동
            if (MessageBox.Show("결재반려 하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            // 2022.03.18. hma 수정(End)

            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.

            Data_Check();
            if (strResultType != "OK")
            {
                MessageBox.Show(strResultMsg);
                this.Cursor = Cursors.Default;
                return;
            }

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // 결재반려 확인  // 2022.03.18. hma 수정: 위치를 위로 이동
                //if (MessageBox.Show("결재반려 하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                //{
                    // 결재반려처리
                    string strSql = " usp_ACD007P3 @pTYPE = 'U2' ";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                    strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                    strSql = strSql + ", @pASSIGN_COMMENT = '" + txtComment.Text + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                //}
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SEARCH_ASSIGN_DATA();       // 결재관련 항목 조회
                strChgFlag = "Y";           // 변경여부를 Y로.

                RtnStr(txtAssignNo.Text, cboAssignStatus.SelectedValue.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
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

        private void ACD007P4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (strChgFlag == "Y")      // 승인/반려 처리를 한 경우.
            {
                RtnStr("", "OK");
            }
            else
            {
                RtnStr("", "Cancel");
            }
        }

        #region 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string AssignNo, string GwStatus)
        {
            returnVal = new string[2];
            returnVal[0] = GwStatus;
            returnVal[1] = AssignNo;
        }
        #endregion

        #region btnGlSlipDtSave_Click(): 저장 버튼 클릭시. 회계예정일 변경 저장
        private void btnGlSlipDtSave_Click(object sender, EventArgs e)
        {
            // 다음 결재자가 결재를 하지 않은 경우에만 저장 처리.
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // 회계예정일 저장
                string strSql_Appr = " usp_ACD007P3 @pTYPE = 'U3' ";
                strSql_Appr = strSql_Appr + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql_Appr = strSql_Appr + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                strSql_Appr = strSql_Appr + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                strSql_Appr = strSql_Appr + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                if (txtFinanceDeptYn.Text == "Y")
                    strSql_Appr = strSql_Appr + ", @pASSIGN_GL_SLIP_DT = '" + dtpGlSlipDt.Text + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql_Appr, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

                Trans.Commit();
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                strChgFlag = "Y";           // 변경여부를 Y로.
                RtnStr(txtAssignNo.Text, cboAssignStatus.SelectedValue.ToString());
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

        // 2022.05.20. hma 추가(Start) 
        #region btnApprvCancel_Click(): 결재승인취소 버튼 클릭시 처리. 검토 결재자가 승인 처리한 건에 대해서 취소 처리를 한다. 승인 결재자 및 반려 처리한 건은 제외.
        private void btnApprvCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("결재승인취소 하시겠습니까?", "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "SY001";   //처리할 내용이 없습니다.

            Cancel_Data_Check();
            if (strResultType != "OK")
            {
                MessageBox.Show(strResultMsg);
                this.Cursor = Cursors.Default;
                return;
            }

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // 결재승인취소처리
                string strSql = " usp_ACD007P3 @pTYPE = 'U4' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
                strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
                strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";
                strSql = strSql + ", @pASSIGN_COMMENT = ''";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

                Trans.Commit();
            }
            catch
            {
                Trans.Rollback();
                MSGCode = "SY002";	// 에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SEARCH_ASSIGN_DATA();       // 결재관련 항목 조회
                strChgFlag = "Y";           // 변경여부를 Y로.

                RtnStr(txtAssignNo.Text, cboAssignStatus.SelectedValue.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
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

        #region Cancel_Data_Check(): 승인취소 처리하기 위한 데이터 체크
        private void Cancel_Data_Check()
        {
            strResultType = "";
            strResultMsg = "";

            // 승인하기 위한 데이터 체크
            string strSql = " usp_ACD007P3 'C3' ";
            strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            strSql = strSql + ", @pTASK_TYPE = '" + cboTaskType.SelectedValue.ToString() + "'";
            strSql = strSql + ", @pASSIGN_NO = '" + txtAssignNo.Text + "'";
            strSql = strSql + ", @pASSIGN_ID = '" + txtAssignId.Text + "'";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                strResultType = dt.Rows[0]["RESULT_CODE"].ToString();
                strResultMsg = dt.Rows[0]["RESULT_MSG"].ToString();
            }
        }
        #endregion
        // 2022.05.20. hma 추가(End) 
    }
}
