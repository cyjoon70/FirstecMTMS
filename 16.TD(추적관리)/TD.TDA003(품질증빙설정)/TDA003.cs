
#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질증빙설정
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-07-17
// 작성내용 : 품목/공정 조회 후, 품질 문서 필수 여부 지정
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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using TDA003P;
namespace TD.TDA003
{
    public partial class TDA003 : UIForm.FPCOMM2T
    {

        #region 필드
        /// <summary>
        /// 필수설정 대상의 정보입니다.
        /// </summary>
        struct TargetInformation
        {
            public string Key;
            public string Type;

            /// <summary>
            /// 키값을 가지는지 여부입니다.
            /// </summary>
            public bool HasKey
            {
                get { return !string.IsNullOrEmpty(Key); }
            }

            /// <summary>
            /// 형식값을 가지는지 여부입니다.
            /// </summary>
            public bool HasType
            {
                get { return !string.IsNullOrEmpty(Type); }
            }

            /// <summary>
            /// 키와 형식값을 모두 가지는지 여부입니다.
            /// </summary>
            public bool HasValues
            {
                get { return !(string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Type)); }
            }

            public void Clear()
            {
                Key = "";
                Type = "";
            }
        }

        // fpSpread3 관련 변수 정의
        string[] G3Head1 = null;	// 첫번째 Head Text
        string[] G3Head2 = null;	// 두번째 Head Text
        string[] G3Head3 = null;	// 세번째 Head Text
        int[] G3Width = null;		// Cell 넓이
        string[] G3Align = null;	// Cell 데이타 정렬방식
        string[] G3Type = null;		// CellType 지정
        int[] G3Color = null;		// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
        string[] G3Etc = null;		// Mask 양식 등
        int G3HeadCnt = 0;			// Head 수
        int[] G3SEQ = null;			// 키

        // 문서 조회 키값 변수 정의
        /// <summary>품목 타겟</summary>
        TargetInformation itemTarget = new TargetInformation();
        /// <summary>공정 타겟</summary>
        TargetInformation procTarget = new TargetInformation();

        #endregion

        #region 생성자
        public TDA003()
        {
            InitializeComponent();
        }
        #endregion

        #region 폼 이벤트
        private void TDA003_Load(object sender, System.EventArgs e)
        {
            try
            {
                // 필수체크
                SystemBase.Validation.GroupBox_Setting(groupBox5);
                SystemBase.Validation.GroupBox_Setting(groupBox7);

                // 품목탭 콤보박스
                SystemBase.ComboMake.C1Combo(cboPlant_I, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); // 공장코드
                SystemBase.ComboMake.C1Combo(cboItemAcct_I, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 0); // 품목계정
                SystemBase.ComboMake.C1Combo(cboItemType_I, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'TD003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3); // 품목그룹
                SystemBase.ComboMake.C1Combo(cboDocCd_I, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드

                // 공정탭 콤보박스
                SystemBase.ComboMake.C1Combo(cboPlant_P, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); // 공장코드
                SystemBase.ComboMake.C1Combo(cboDocCd_P, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드

                // 우측패널 콤보박스
                SystemBase.ComboMake.C1Combo(cboDept, "usp_B_COMMON @pTYPE ='COMM', @pCODE = 'TD001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ", 3); // 부서

                SetControl();

                // 그리드 초기화
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

                // FORM TEMPLATE에 없는 그리드 추가
                SetFpSpread3(null);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Setting(groupBox5);
            SystemBase.Validation.GroupBox_Setting(groupBox7);

            itemTarget = new TargetInformation();
            procTarget = new TargetInformation();

            SetControl();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

            // FORM TEMPLATE에 없는 그리드 추가
            SetFpSpread3(null);
        }
        #endregion

        #region 컨트롤 초기화 설정
        private void SetControl()
        {
            cboDept.SelectedIndex = 0;
            cboDocCd_I.SelectedIndex = 0;
            cboDocCd_P.SelectedIndex = 0;

            optConfigYes_I.Checked = true;
            optConfigYes_P.Checked = true;
            optMsAll_P.Checked = true;
            optInspAll_P.Checked = true;
            optUseAll_P.Checked = true;

            try
            {
                txtItemCd_I.Text = "";
                txtItemNM_I.Text = "";
                txtProCd_P.Text = "";
                txtProNm_P.Text = "";
            }
            catch { }

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            string query = string.Empty;

            // 품목
            if (c1DockingTab1.SelectedTab.TabIndex == 0)
            {
                query = "usp_TDA003 'S1'"
                    + ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'"
                    + ", @pPLANT_CD ='" + cboPlant_I.SelectedValue.ToString() + "'";
                if (!string.IsNullOrEmpty(txtItemCd_I.Text)) query += ", @pITEM_CD ='" + txtItemCd_I.Text + "'";
                if (!string.IsNullOrEmpty(txtItemNM_I.Text)) query += ", @pITEM_NM ='" + txtItemNM_I.Text + "'";
                if (!string.IsNullOrEmpty(cboItemAcct_I.Text)) query += ", @pITEM_ACCT = '" + cboItemAcct_I.SelectedValue.ToString() + "'";
                if (!string.IsNullOrEmpty(cboItemType_I.Text)) query += ", @pBOM_FLAG = '" + cboItemType_I.SelectedValue.ToString() + "'";
                if (!string.IsNullOrEmpty(cboDocCd_I.Text)) query += ", @pDOC_CD = '" + cboDocCd_I.SelectedValue.ToString() + "'";
                if (optConfigYes_I.Checked) query += ", @pATT_CONFIG_YN = 'Y'";
                else if (optConfigNo_I.Checked) query += ", @pATT_CONFIG_YN = 'N'";
                if (chkOUT.Checked) query += ", @pOUT = 'Y'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                SetRowBackColor(fpSpread1, 1);
                itemTarget.Clear();
                SearchDoc();
            }

            // 공정
            else if (c1DockingTab1.SelectedTab.TabIndex == 1)
            {
                query = "usp_TDA003 'S2'"
                    + ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'"
                    + ", @pPLANT_CD = '" + cboPlant_P.SelectedValue.ToString() + "'";
                if (!string.IsNullOrEmpty(txtProCd_P.Text)) query += ", @pJOB_CD = '" + txtProCd_P.Text + "'";
                if (!string.IsNullOrEmpty(txtProNm_P.Text)) query += ", @pJOB_NM = '" + txtProNm_P.Text + "'";
                if (!string.IsNullOrEmpty(cboDocCd_P.Text)) query += ", @pDOC_CD = '" + cboDocCd_P.SelectedValue.ToString() + "'";
                if (optMsYes_P.Checked) query += ", @pMILESTONE_FLG = 'Y'";
                else if (optMsNo_P.Checked) query += ", @pMILESTONE_FLG = 'N'";
                if (optInspYes_P.Checked) query += ", @pINSP_FLG = 'Y'";
                else if (optInspNo_P.Checked) query += ", @pINSP_FLG = 'N'";
                if (optUseYes_P.Checked) query += ", @pUSE_FLG = 'Y'";
                else if (optUseNo_P.Checked) query += ", @pUSE_FLG = 'N'";
                if (optConfigYes_P.Checked) query += ", @pATT_CONFIG_YN = 'Y'";
                else if (optConfigNo_P.Checked) query += ", @pATT_CONFIG_YN = 'N'";

                SetFpSpread3(query);
                SetRowBackColor(fpSpread3, 3);
                procTarget.Clear();
                SearchDoc();
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;

        }
        #endregion

        #region 설정완료시 Row BackColor 강조.
        private void SetRowBackColor(FarPoint.Win.Spread.FpSpread fpSpread, int iType)
        {

            if (fpSpread.ActiveSheet.Rows.Count > 0)
            {

                for (int i = 0; i <= fpSpread.ActiveSheet.Rows.Count - 1; i++)
                {

                    if (iType == 1) // 품목
                    {
                        if (fpSpread.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "첨부설정")].Text.ToString() == "설정완료")
                        {
                            fpSpread.ActiveSheet.Rows[i].BackColor = Color.LightGreen;
                        }
                    }
                    else
                    {
                        if (fpSpread.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "첨부설정")].Text.ToString() == "설정완료")
                        {
                            fpSpread.ActiveSheet.Rows[i].BackColor = Color.LightGreen;
                        }
                    }

                }

            }

        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {

            int iMasterCnt = 0;

            string ERRCode = "ER", MSGCode = "SY001"; //처리할 내용이 없습니다.
            string strQuery = string.Empty;
            string strReq_YN = string.Empty;

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {

                if (fpSpread2.ActiveSheet.Rows.Count > 0)
                {

                    // 품목
                    if (c1DockingTab1.SelectedTab.TabIndex == 0)
                    {

                        for (int i = 0; i < fpSpread1.ActiveSheet.Rows.Count; i++)
                        {
                            if (fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text.ToString() != "True") continue;

                            iMasterCnt++;
                            for (int j = 0; j < fpSpread2.ActiveSheet.Rows.Count; j++)
                            {
                                strQuery = string.Empty;
                                strReq_YN = string.Empty;

                                if (fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "필수여부")].Text.ToString() == "True")
                                    strReq_YN = "Y";
                                else
                                    strReq_YN = "N";

                                strQuery = "usp_TDA003 ";
                                strQuery = strQuery + " @pTYPE = 'I1' ";
                                strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                                strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant_I.SelectedValue.ToString() + "' ";
                                strQuery = strQuery + ",@pTARGET_KEY ='" + fpSpread1.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString() + "' ";
                                strQuery = strQuery + ",@pDOC_CD ='" + fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "문서코드")].Text.ToString() + "' ";
                                strQuery = strQuery + ",@pDOC_REQ_YN ='" + strReq_YN + "' ";
                                strQuery = strQuery + ",@pTARGET_TYPE ='I' ";
                                strQuery = strQuery + ",@pREG_ID ='" + SystemBase.Base.gstrUserID.ToString() + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                    }

                    // 공정
                    iMasterCnt = 0;
                    if (c1DockingTab1.SelectedTab.TabIndex == 1)
                    {

                        for (int i = 0; i < fpSpread3.ActiveSheet.Rows.Count; i++)
                        {
                            if (fpSpread3.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "선택")].Text.ToString() != "True") continue;

                            iMasterCnt++;
                            for (int j = 0; j < fpSpread2.ActiveSheet.Rows.Count; j++)
                            {
                                strQuery = string.Empty;
                                strReq_YN = string.Empty;

                                if (fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "필수여부")].Text.ToString() == "True")
                                    strReq_YN = "Y";
                                else
                                    strReq_YN = "N";

                                strQuery = "usp_TDA003 ";
                                strQuery = strQuery + " @pTYPE = 'I1' ";
                                strQuery = strQuery + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                                strQuery = strQuery + ",@pPLANT_CD ='" + cboPlant_I.SelectedValue.ToString() + "' ";
                                strQuery = strQuery + ",@pTARGET_KEY ='" + fpSpread3.ActiveSheet.Cells[i, SystemBase.Base.GridHeadIndex(GHIdx3, "공정코드")].Text.ToString() + "' ";
                                strQuery = strQuery + ",@pDOC_CD ='" + fpSpread2.ActiveSheet.Cells[j, SystemBase.Base.GridHeadIndex(GHIdx2, "문서코드")].Text.ToString() + "' ";
                                strQuery = strQuery + ",@pDOC_REQ_YN ='" + strReq_YN + "' ";
                                strQuery = strQuery + ",@pTARGET_TYPE ='R' ";
                                strQuery = strQuery + ",@pREG_ID ='" + SystemBase.Base.gstrUserID.ToString() + "' ";

                                DataTable dt = SystemBase.DbOpen.TranDataTable(strQuery, dbConn, Trans);
                                ERRCode = dt.Rows[0][0].ToString();
                                MSGCode = dt.Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }

                    }

                    Trans.Commit();

                    if (iMasterCnt > 0)
                    {
                        ERRCode = "OK";
                        SearchDoc();
                    }
                    else
                    {
                        ERRCode = "WR";
                    }
                }
                else
                {
                    ERRCode = "WR";
                }

            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
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

            this.Cursor = Cursors.Default; ;

        }
        #endregion

        #region 품목 선택
        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.ActiveSheet.Rows.Count > 0)
            {
                itemTarget.Key = fpSpread1.ActiveSheet.Cells[fpSpread1.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
                itemTarget.Type = "I";

                SearchDoc();
            }
        }
        #endregion

        #region 공정 선택
        private void fpSpread3_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread3.ActiveSheet.Rows.Count > 0)
            {
                procTarget.Key = fpSpread3.ActiveSheet.Cells[fpSpread3.ActiveSheet.ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx3, "공정코드")].Text.ToString();
                procTarget.Type = "R";

                SearchDoc();
            }
        }
        #endregion

        #region 문서 조회
        private void SearchDoc()
        {
            string query = string.Empty;

            if (c1DockingTab1.SelectedIndex == 0) // 품목 I
            {
                if (itemTarget.HasValues)
                {
                    query = "usp_TDA003 'S3'";
                    query = query + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    query = query + ",@pPLANT_CD ='" + cboPlant_I.SelectedValue.ToString() + "' ";
                    if (cboDept.SelectedIndex > 0) query = query + ",@pDOC_DEPT_CD ='" + cboDept.SelectedValue.ToString() + "' ";
                    query = query + ",@pTARGET_kEY ='" + itemTarget.Key + "' ";
                    query = query + ",@pTARGET_TYPE ='" + itemTarget.Type + "' ";
                }
            }
            else // 공정 R
            {
                if (procTarget.HasValues)
                {
                    query = "usp_TDA003 'S3'";
                    query = query + ",@pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    query = query + ",@pPLANT_CD ='" + cboPlant_I.SelectedValue.ToString() + "' ";
                    if (cboDept.SelectedIndex > 0) query = query + ",@pDOC_DEPT_CD ='" + cboDept.SelectedValue.ToString() + "' ";
                    query = query + ",@pTARGET_kEY ='" + procTarget.Key + "' ";
                    query = query + ",@pTARGET_TYPE ='" + procTarget.Type + "' ";
                }
            }

            SheetView sheet = fpSpread2.ActiveSheet;
            if (!string.IsNullOrEmpty(query))
                UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
            else sheet.RowCount = 0;
            sheet.Columns[2].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;
            sheet.Columns[3].MergePolicy = FarPoint.Win.Spread.Model.MergePolicy.Always;

        }
        #endregion

        #region 부서필터 자동 조회
        private void cboDept_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                SearchDoc();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "COMBOBOX CHANGE 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 텝 인덱스 변경시 이벤트
        private void c1DockingTab1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboDept.SelectedIndex = 0;
                SearchDoc();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "TAB INDEX CHANGE 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread1(품목), fpSpread2(문서), fpSpread3(공정) 기능 추가 작업

        #region  헤더 체크박스 클릭시 전체행 체크 처리
        private void fpSpreads_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            // 헤더 체크박스 클릭시 전체행 체크 처리
            if (e.ColumnHeader)
                if (((FarPoint.Win.Spread.FpSpread)sender).ActiveSheet.ToggleCheckAll(e.Column))
                    e.Cancel = true;
        }

        #endregion

        #region fpSpread2 데이타 수정시 U 플래그 등록
        private void fpSpread2_EditChange(int iRow)
        {
            try
            {
                UIForm.FPMake.fpChange(fpSpread2, iRow);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region fpSpread3 그리드 설정
        private void SetFpSpread3(string strSql)
        {

            string Query3 = " usp_BAA004 'S3', @PFORM_ID='" + this.Name.ToString() + "', @PGRID_NAME='fpSpread3', @PIN_ID='" + SystemBase.Base.gstrUserID + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
            DataTable dt3 = SystemBase.DbOpen.TranDataTable(Query3);
            int G3RowCount = dt3.Rows.Count + 1;

            if (G3RowCount > 1)
            {
                G3Head1 = new string[G3RowCount];// 첫번째 Head Text
                G3Head2 = new string[G3RowCount];// 두번째 Head Text
                G3Head3 = new string[G3RowCount];// 세번째 Head Text
                G3Width = new int[G3RowCount];// Cell 넓이
                G3Align = new string[G3RowCount];// Cell 데이타 정렬방식
                G3Type = new string[G3RowCount];// CellType 지정
                G3Color = new int[G3RowCount];// Cell 색상 및 ReadOnly 설정(0:일반, 1:필수, 2:ReadOnly)
                G3Etc = new string[G3RowCount];
                G3HeadCnt = Convert.ToInt32(dt3.Rows[0][0].ToString());
                G3SEQ = new int[G3RowCount];// 키

                /********************1번째 숨김필드 정의******************/
                G3Head1[0] = "";
                if (Convert.ToInt32(dt3.Rows[0][0].ToString()) >= 1)
                    G3Head2[0] = "";
                if (Convert.ToInt32(dt3.Rows[0][0].ToString()) >= 2)
                    G3Head3[0] = "";
                G3Width[0] = 0;
                G3Align[0] = "";
                G3Type[0] = "";
                G3Color[0] = 0;
                G3Etc[0] = "";
                /********************1번째 숨김필드 정의******************/

                //####################그리드 Head 순번######################
                GHIdx3 = new string[G3RowCount - 1, 2];	// 그리드 Head Index 변수 길이
                //string OldHeadName2 = null;
                int OldHeadNameCount3 = 1;
                //####################그리드 Head 순번######################
                for (int i = 1; i < G3RowCount; i++)
                {
                    G3Head1[i] = dt3.Rows[i - 1][1].ToString();
                    if (Convert.ToInt32(dt3.Rows[i - 1][0].ToString()) >= 1)
                        G3Head2[i] = dt3.Rows[i - 1][2].ToString();
                    if (Convert.ToInt32(dt3.Rows[i - 1][0].ToString()) >= 2)
                        G3Head3[i] = dt3.Rows[i - 1][3].ToString();

                    G3Width[i] = Convert.ToInt32(dt3.Rows[i - 1][4].ToString());
                    G3Align[i] = dt3.Rows[i - 1][5].ToString();
                    G3Type[i] = dt3.Rows[i - 1][6].ToString();
                    G3Color[i] = Convert.ToInt32(dt3.Rows[i - 1][7].ToString());
                    G3Etc[i] = dt3.Rows[i - 1][8].ToString();

                    G3SEQ[i] = Convert.ToInt32(dt3.Rows[i - 1][9].ToString());


                    //####################그리드 Head 순번######################                            
                    OldHeadNameCount3 = 1;
                    GHIdx3[0, 0] = dt3.Rows[0][1].ToString().ToUpper();
                    for (int k = 0; k < i - 1; k++)
                    {
                        if (dt3.Rows[i - 1][1].ToString().ToUpper() == GHIdx3[k, 0].ToUpper())
                        {
                            OldHeadNameCount3++;
                        }
                        else if (GHIdx3[k, 0].ToUpper().LastIndexOf("_") > 0 && dt3.Rows[i - 1][1].ToString().ToUpper() == GHIdx3[k, 0].ToUpper().Substring(0, GHIdx3[k, 0].ToUpper().LastIndexOf("_")))
                        {
                            OldHeadNameCount3++;
                        }
                    }

                    if (OldHeadNameCount3 > 1)
                    {
                        GHIdx3[i - 1, 0] = dt3.Rows[i - 1][1].ToString().ToUpper() + "_" + OldHeadNameCount3.ToString();	// 그리드 Head명
                    }
                    else
                    {
                        GHIdx3[i - 1, 0] = dt3.Rows[i - 1][1].ToString().ToUpper();	// 그리드 Head명
                    }

                    GHIdx3[i - 1, 1] = Convert.ToString(i);			    // 그리드 Head 위치
                    //####################그리드 Head 순번######################
                }

                UIForm.FPMake.grdCommSheet(fpSpread3, strSql, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0);

            }

        }
        #endregion

        #region fpSpread3 데이타 수정시 U 플래그 등록
        private void fpSpread3_EditChange(int iRow)
        {
            try
            {
                UIForm.FPMake.fpChange(fpSpread3, iRow);
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "그리드 EditChange 이벤트"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #endregion


        #region 그리드 상단 팝업
        protected override void fpButtonClick(int Row, int Column)
        {
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "보기"))
            {
                string strITEM_CD = fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text.ToString();
                TDA003P.TDA003P pu = new TDA003P.TDA003P(strITEM_CD);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {

                }
            }
        }
        #endregion
    }

}
