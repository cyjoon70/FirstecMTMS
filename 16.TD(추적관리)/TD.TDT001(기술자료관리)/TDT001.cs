#region 작성정보
/*********************************************************************/
// 단위업무명 : 기술자료관리
// 작 성 자 : 이재광
// 작 성 일 : 2014-07-21
// 작성내용 : 기술자료와 자료파일 조회/열람/등록/관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument;
using EDocument.Spread;
using EDocument.Network;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Extensions.FpSpreadExtension;


namespace TD.TDT001
{
    public partial class TDT001 : UIForm.FPCOMM2
    {
        #region 필드
        // 마스터 컬럼(자료마스터)
        int colSrcId = -1;
        // 디테일 컬럼(자료파일)
        /// <summary>자료상태 열</summary>
        int colSrcfState = -1;        
        /// <summary>이전자료상태 열</summary>
        int colPrevSrcfState = -1;
        /// <summary>자료번호 열</summary>
        int colSrcfNo = -1;
        /// <summary>규격번호 열</summary>
        int colStndNo = -1;
        /// <summary>페이지 열</summary>
        int colPage = -1;
        /// <summary>매수 열</summary>
        int colTotalPage = -1;
        /// <summary>개정번호 열</summary>
        int colRevNo = -1;
        /// <summary>개정일 열</summary>
        int colRevDt = -1;
        /// <summary>제정일 열</summary>
        int colEstbDt = -1;
        /// <summary>규격종류 열</summary>
        int colStndCd = -1;
        /// <summary>출도일 열</summary>
        int colPubDt = -1;
        /// <summary>출도승인번호 열</summary>
        int colPubAprvNo = -1;
        /// <summary>형상식별자 버튼 열</summary>
        int colFormIdntButton = -1;
        /// <summary>형상통제자 버튼 열</summary>
        int colFormCtrlButton = -1;
        /// <summary>폐기사유 열</summary>
        int colDisuseReason = -1;
        /// <summary>폐기일</summary>
        int colDisuseDT = -1;
        /// <summary>자료파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;
        bool Commit = false;
        /// <summary>마스터그리드에서 선택된 행의 자료ID</summary>
        int selectedSrcId = -1;
        string strBtn = "N";
        #endregion
        
        public TDT001()
        {
            InitializeComponent();
        }

        #region 속성
        /// <summary>
        /// 입력 자료번호입니다.
        /// </summary>
        public string SourceNumber
        {
            get
            {
                return cboSrcNoHeader.Text + txtSrcNoYear.Text + txtSrcNoIdntChar.Text + "-" + txtSrcNoSeq.Text;
            }
            set
            {
                try
                {
                    cboSrcNoHeader.SelectedValue = value.Substring(0, 4);
                    txtSrcNoYear.Text = value.Substring(4, 2);
                    txtSrcNoIdntChar.Value = value.Substring(6, 1);
                    txtSrcNoSeq.Text = value.Substring(8, 3);
                }
                catch (Exception e) { }
            }
        }
        #endregion

        #region 폼 이벤트
        private void TDT001_Load(object sender, System.EventArgs e)
        {
            // 그룹박스 필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            // 콤보박스 설정
            SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); // 좌 공장코드
            SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'"); // 공장코드
            SystemBase.ComboMake.C1Combo(cboSPubTeam, "usp_B_COMMON @pType='COMM1', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", -1); // 좌 발행처
            SystemBase.ComboMake.C1Combo(cboPubTeam, "usp_B_COMMON @pType='COMM1', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", -1); // 발행처
            SystemBase.ComboMake.C1Combo(cboSDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_DEPT_CD = 'MT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 좌 문서코드
            SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_DEPT_CD = 'MT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드
            cboSrcNoHeader.SetItems(new Dictionary<string, string> { { "", "" }, { "ENGI", "ENGI" }, { "ENGD", "ENGD" }, { "RNDI", "RNDI" }, { "RNDD", "RNDD" } });


            // 그리드 정의
            G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "발행처")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "자료상태")] = "A#D|활성#폐기";
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "도면크기")] =
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "인쇄크기")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작성처")] =
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "승인처")] =
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출처")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "변경구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "규격종류")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 1);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM변경여부")] = "Y#N|Y#N";
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

            // 컬럼인덱스
            SheetView masterSheet = fpSpread2.ActiveSheet;
            colSrcId = masterSheet.FindHeaderColumnIndex("자료ID");
            SheetView sheet = fpSpread1.ActiveSheet;
            colSrcfState = sheet.FindHeaderColumnIndex("자료상태");
            colPrevSrcfState = sheet.FindHeaderColumnIndex("이전자료상태");
            colSrcfNo = sheet.FindHeaderColumnIndex("자료파일번호") + 2;
            colStndNo = sheet.FindHeaderColumnIndex("규격번호");
            colPage = sheet.FindHeaderColumnIndex("페이지");
            colTotalPage = sheet.FindHeaderColumnIndex("매수");
            colRevNo = sheet.FindHeaderColumnIndex("개정번호");
            colRevDt = sheet.FindHeaderColumnIndex("개정일");
            colEstbDt = sheet.FindHeaderColumnIndex("제정일");
            colStndCd = sheet.FindHeaderColumnIndex("규격종류");
            colPubDt = sheet.FindHeaderColumnIndex("출도일");
            colPubAprvNo = sheet.FindHeaderColumnIndex("출도승인번호");
            colFormIdntButton = sheet.FindHeaderColumnIndex("형상식별자");
            colFormCtrlButton = sheet.FindHeaderColumnIndex("형상통제자");
            colDisuseReason = sheet.FindHeaderColumnIndex("폐기사유");
            colDisuseDT = sheet.FindHeaderColumnIndex("폐기일");
            // 파일버튼 관리자 초기화
            int col = fpSpread1.ActiveSheet.FindHeaderColumnIndex("파일명");
            buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.SourceFile)
            {
                FilenameColumnIndex = col + 3,
                ServerPathColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("서버경로"),
                ServerFilenameColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("서버파일명"),
                FileSelectButtonColumnIndex = col,
                FileViewButtonColumnIndex = col + 1,
                FileDownloadButtonColumnIndex = col + 2,
                DocTypeNameColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("자료종류"),
                DocRevisionColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("개정번호"),
                DocNumberColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("규격번호"),
            };

            // 기타
            ShowSrcNoMessage();
            NewExec();
        }
        #endregion

        #region 행추가
        protected override void RowInsExec()
        {
            if (selectedSrcId < 0)
            {
                MessageBox.Show("먼저 자료를 선택해야합니다.", "행 추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 행추가
            UIForm.FPMake.RowInsert(fpSpread1);

            // 자동입력 처리
            SheetView masterSheet = fpSpread2.ActiveSheet;
            SheetView sheet = fpSpread1.ActiveSheet;
            int newRow = sheet.ActiveRowIndex;
            sheet.FindCell(newRow, "자료코드").Value = cboDocCd.SelectedValue;
            sheet.FindCell(newRow, "자료종류").Value = cboDocCd.Text;
            sheet.FindCell(newRow, "사업코드").Value = txtEntCd.Text;
            sheet.FindCell(newRow, "사업명").Value = txtEntNm.Text;
            sheet.FindCell(newRow, "개정번호").Value = "1.0";
            sheet.Cells[newRow, colSrcfNo - 2].Text = masterSheet.FindCell(masterSheet.ActiveRowIndex, "자료번호").Text;
            sheet.Cells[newRow, colSrcfNo - 1].Text = "-";
            sheet.FindCell(newRow, "출처").Value = cboPubTeam.SelectedValue;
            sheet.FindCell(newRow, "변경구분").Value = "NEW";
            buttonManager.UpdateButtons(newRow); // 버튼 업데이트

            // 그리드디자인에서 셀 속성이 '읽기전용/흰색'인 경우 행추가시 편집 가능 => 잠금
            sheet.Cells[newRow, sheet.FindHeaderColumnIndex("형상식별자") + 1].Locked = true;
            sheet.Cells[newRow, sheet.FindHeaderColumnIndex("형상통제자") + 1].Locked = true;

            fpSpread1.AllowDrop = true;
        }
        #endregion

        #region 입력 초기화
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dteSAcptDtFr.Value = DateTime.Now.AddMonths(-1);
            dteSAcptDtTo.Value = DateTime.Now;
            ResetDetailInputPanel();
            fpSpread2.ActiveSheet.RowCount = 0;
            fpSpread1.ActiveSheet.RowCount = 0;
            txtSrcNoYear.Value = DateTime.Now.Year.ToString().Substring(2, 2);
            selectedSrcId = -1;
            EnableEditing(true);
            UpdateSourceFileStatus();
        }
        #endregion

        #region 마스터 삭제
        protected override void DeleteExec()
        {
            string msg = SystemBase.Base.MessageRtn("B0027");
            DialogResult dsMsg = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = "usp_TDT001 'D1'"
                        + ", @pSRC_ID = " + selectedSrcId
                        + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
        }
        #endregion

        #region 마스터 조회
        protected override void SearchExec()
        {
            Search(-1);
        }

        private void Search(int intSrcId)
        {
            if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string query = "usp_TDT001 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                query += ", @pSTND_NO = '" + txtSTND_NO.Text + "'";
                if (intSrcId < 0)
                    query = AppendMasterQueryCondition(query);
                else
                {
                    query += ", @pSRC_ID = " + intSrcId;
                    selectedSrcId = intSrcId;
                }

                UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

                // 자료아이디가 지정된 경우 자동 선택
                selectedSrcId = -1;
                SheetView sheet = fpSpread2.ActiveSheet;
                if (intSrcId > 0 && sheet.RowCount > 0)
                {
                    int row = sheet.FindRowIndex(colSrcId, intSrcId.ToString());
                    if (row > -1)
                    {
                        sheet.SetActiveCell(row, colSrcId);
                        sheet.AddSelection(row, 0, 1, sheet.ColumnCount);
                        selectedSrcId = intSrcId;

                        SearchDetail(selectedSrcId);
                    }
                }

                // 선택 자료가 없는 경우 우측패널 클리어
                if (selectedSrcId < 0)
                    ResetDetailInputPanel();
                fpSpread1.ActiveSheet.RowCount = 0;
                UpdateSourceFileStatus();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 자료를 업데이트합니다.
        /// </summary>
        /// <param name="srcId"></param>
        void UpdateSource(int srcId)
        {
            SheetView sheet = fpSpread2.ActiveSheet;
            int row = sheet.FindRowIndex(sheet.FindHeaderColumnIndex("자료ID"), srcId.ToString());
            if (row < 0) return;

            string query = "usp_TDT001 'S1', @pSRC_ID = " + srcId;
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);
            bool ok = true;
            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < 10) ok = false;

            if (ok)
            {
                DataRow drow = dt.Rows[0];
                try
                {
                    sheet.FindCell(row, "자료번호").Value = drow["SRC_NO"].ToString();
                    sheet.FindCell(row, "자료코드").Value = drow["DOC_CD"].ToString();
                    sheet.FindCell(row, "자료종류").Value = drow["DOC_NM"].ToString();
                    sheet.FindCell(row, "사업코드").Value = drow["ENT_CD"].ToString();
                    sheet.FindCell(row, "사업명").Value = drow["ENT_NM"].ToString();
                    sheet.FindCell(row, "자료명").Value = drow["SRC_NM"].ToString();
                    sheet.FindCell(row, "발행처").Value = drow["PUB_TM_CD"].ToString();
                    sheet.FindCell(row, "발행일").Value = drow["PUB_DT"].ToString();
                    sheet.FindCell(row, "접수자ID").Value = drow["ACPT_USR_ID"].ToString();
                    sheet.FindCell(row, "접수자").Value = drow["ACPT_USR_NM"].ToString();
                    sheet.FindCell(row, "접수일").Value = drow["ACPT_DT"].ToString();
                    sheet.FindCell(row, "승인자ID").Value = drow["APP_USR_ID"].ToString();
                    sheet.FindCell(row, "승인자").Value = drow["APP_USR_NM"].ToString();
                    sheet.FindCell(row, "비고").Value = drow["REMARK"].ToString();
                }
                catch (Exception e)
                {
                    ok = false;
                }
            }

            if (!ok)
                MessageBox.Show("변경된 자료목록 아이템의 표시내용을 업데이트 하는데 실패했습니다.", "변경된 자료목록 아이템 업데이트", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region 디테일 조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            SheetView sheet = fpSpread2.ActiveSheet;
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                try
                {
                    int row = sheet.GetSelection(0).Row;
                    int srcId = Convert.ToInt32(sheet.FindCell(row, "자료ID").Value);
                    if (srcId == selectedSrcId) return; // 이미 선택한 행이면 스킵

                    selectedSrcId = srcId;
                    SearchDetail(selectedSrcId);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.		
                }
            }
        }

        private void SearchDetail(int srcId)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                SystemBase.Validation.GroupBox_Reset(groupBox2);
                fpSpread1.Sheets[0].Rows.Count = 0;

                //자료 리스트업
                if (srcId > 0)
                {
                    EnableEditing(true);

                    string sourceQuery = "usp_TDT001  'S1'"
                        + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                        + ", @pSRC_ID = '" + srcId + "'"
                        + ", @pSTND_NO = '" + txtSTND_NO.Text + "'"
                        + ", @pSRC_NM = '" + txtSSrcNm.Text + "'";
                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(sourceQuery);

                    cboPlant.SelectedValue = dt.Rows[0]["PLANT_CD"].ToString();
                    txtEntCd.Value = dt.Rows[0]["ENT_CD"].ToString();
                    this.SourceNumber = dt.Rows[0]["SRC_NO"].ToString();
                    txtSrcNm.Value = dt.Rows[0]["SRC_NM"].ToString();
                    cboDocCd.SelectedValue = dt.Rows[0]["DOC_CD"].ToString();
                    cboPubTeam.SelectedValue = dt.Rows[0]["PUB_TM_CD"].ToString();
                    txtPubTmInfo.Text = dt.Rows[0]["PUB_TM_INFO"].ToString();
                    dtePubDt.Value = dt.Rows[0]["PUB_DT"].ToString();
                    txtAcptUsrId.Value = dt.Rows[0]["ACPT_USR_ID"].ToString();
                    dteAcptDt.Value = dt.Rows[0]["ACPT_DT"].ToString();
                    txtAppUsrId.Value = dt.Rows[0]["APP_USR_ID"].ToString();
                    txtRemark.Value = dt.Rows[0]["REMARK"].ToString();
                }
                else
                    EnableEditing(false);

                //자료파일 리스트업
                string sfileQuery = "usp_TDT001  'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                sfileQuery += ", @pSTND_NO = '" + txtSTND_NO.Text + "'";
                if (srcId > 0) sfileQuery += ", @pSRC_ID = " + srcId;
                else
                    sfileQuery = AppendMasterQueryCondition(sfileQuery);
                UIForm.FPMake.grdCommSheet(fpSpread1, sfileQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UpdateSourceFileStatus(srcId > 0 ? false : true);

                // 드롭이 자꾸 비활성화된다. 수퍼클래스에서 뻘짓 하는 듯.
                SheetView sheet = fpSpread1.ActiveSheet;
                fpSpread1.AllowDrop = sheet.RowCount > 0;

                // 자료파일 그리드 셀 조정
                for (int row = 0; row < sheet.Rows.Count; row++)
                {
                    // 폐기된 자료행 강조
                    Cell stateCell = sheet.Cells[row, colSrcfState];
                    Cell colDisuseDTCell = sheet.Cells[row, colDisuseDT];
                    if (Convert.ToString(stateCell.Value) == "D")
                    {
                        Row oRow = sheet.Rows[row];
                        oRow.SetApprearance(CellAppearance.Discard);
                        oRow.Locked = true;

                        // 자료상태 컬럼 잠금해제
                        stateCell.Locked = false;
                        stateCell.SetApprearance(CellAppearance.Normal);
                    }
                    else
                    {
                        colDisuseDTCell.Locked = false;
                        colDisuseDTCell.SetApprearance(CellAppearance.Normal);
                    }

                    // 버튼 업데이트
                    buttonManager.UpdateButtons(row);
                }

                ((TextCellType)sheet.Columns[colSrcfNo].CellType).MaxLength = 9; // 자료파일번호
                ((TextCellType)sheet.Columns[colPage].CellType).MaxLength = 6; // 페이지정보
                ((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호
                ((TextCellType)sheet.Columns[colPubAprvNo].CellType).MaxLength = 30; // 출도승인번호

                // 자료파일이 있는 경우 자료종류 변경 불가능
                if (fpSpread1.ActiveSheet.RowCount < 1)
                {
                    cboDocCd.Enabled = true;
                    cboDocCd.EditorBackColor = UIColors.RequiredBackground;
                }
                else
                {
                    cboDocCd.Enabled = false;
                    cboDocCd.EditorBackColor = UIColors.ReadonlyBackground;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 저장
        protected override void SaveExec()
        {
            Commit = true;
            if ((panDetailMaster.Enabled && !(SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2)) || // 우측패널 상단 그룹박스 필수체크
                !SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))) // 우측패널 그리드 필수체크
                return;

            this.Cursor = Cursors.WaitCursor;

            string resultCode = "ER", resultMessage = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            bool isNewSource = selectedSrcId < 1; // 새로운 마스터항목(자료) 추가인지 여부
            int savedSrcId = selectedSrcId;

            try
            {
                //// Master 저장
                if (panDetailMaster.Enabled)
                {
                    string strMstType = isNewSource ? "I1" : "U1";
                    string strSqlMaster = "usp_TDT001 '" + strMstType + "'"
                        + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "
                        + ", @pPLANT_CD = '" + cboPlant.SelectedValue + "'"
                        + ", @pENT_CD = '" + txtEntCd.Text + "'"
                        + ", @pSRC_NO = '" + this.SourceNumber + "'"
                        + ", @pDOC_CD = '" + cboDocCd.SelectedValue + "'"
                        + ", @pSRC_NM = '" + txtSrcNm.Text + "'"
                        + ", @pPUB_TM_CD = '" + cboPubTeam.SelectedValue + "'"
                        + ", @pPUB_TM_INFO = '" + txtPubTmInfo.Text + "'"
                        + ", @pPUB_DT = '" + dtePubDt.Text + "'"
                        + ", @pACPT_USR_ID = '" + txtAcptUsrId.Text + "'"
                        + ", @pACPT_DT = '" + dteAcptDt.Text + "'"
                        + ", @pAPP_USR_ID = '" + txtAppUsrId.Text + "'"
                        + ", @pREMARK = '" + txtRemark.Text + "'"
                        + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    if (selectedSrcId > 0) strSqlMaster += ", @pSRC_ID = " + selectedSrcId;

                    DataTable dt = SystemBase.DbOpen.TranDataTable(strSqlMaster, dbConn, Trans);
                    resultCode = dt.Rows[0][0].ToString();
                    resultMessage = dt.Rows[0][1].ToString();
                    if (resultCode == "OK")
                    {
                        if (isNewSource) savedSrcId = Convert.ToInt32(dt.Rows[0][2]);
                    }
                    else { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                }

                //// Detail 저장
                string docCode = Convert.ToString(cboDocCd.SelectedValue);
                for (int row = 0; row < fpSpread1.Sheets[0].Rows.Count; row++)
                {
                    string strHead = fpSpread1.ActiveSheet.RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;
                    string strGbn = "";

                    switch (strHead)
                    {
                        case "U": strGbn = "U2"; break;
                        case "I": strGbn = "I2"; break;
                        case "D": strGbn = "D2"; break;
                        default: strGbn = ""; break;
                    }

                    SheetView sheet = fpSpread1.ActiveSheet;
                    if (strHead == "D")
                    {
                        // 자료파일 삭제
                        string strSql = string.Format("usp_TDT001 'D2', @pSRCF_ID = {0}", sheet.FindCell(row, "자료파일ID").Text);
                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                    else
                    {
                        if (strHead == "U" && sheet.Cells[row, colPrevSrcfState].Text == "A" && sheet.Cells[row, colSrcfState].Text == "활성" && (sheet.FindCell(row, "폐기일").Text != "" || sheet.FindCell(row, "폐기사유").Text != ""))
                        {
                            MessageBox.Show(SystemBase.Base.MessageRtn("취소되었습니다.."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Commit = false;
                            Trans.Rollback();
                            break;
                        }
                       
                        string query = "usp_TDT001 '" + strGbn + "'";
                        if (strHead == "I") query += ", @pSRC_ID = " + selectedSrcId;
                        else query += ", @pSRCF_ID = " + sheet.FindCell(row, "자료파일ID").Text;
                        query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                            + ", @pSRCF_NO = '" + sheet.Cells[row, sheet.FindHeaderColumnIndex("자료파일번호") + 2].Text + "'"
                            + ", @pSTND_NO = '" + sheet.FindCell(row, "규격번호").Text + "'"
                            + ", @pSTND_NM = '" + sheet.FindCell(row, "규격명").Text + "'"
                            + ", @pDISUSE_ID = '" + sheet.Cells[row, sheet.FindHeaderColumnIndex("자료파일번호")].Text + "'";
                        if (!string.IsNullOrEmpty(sheet.Cells[row, colStndCd].Text))
                            query += ", @pSTND_CD = '" + sheet.Cells[row, colStndCd].Value + "'";
                        query += ", @pPAGE_INFO = '" + sheet.FindCell(row, "페이지").Text + "'"
                            + ", @pREV_NO = '" + sheet.FindCell(row, "개정번호").Text + "'";


                        if (sheet.Cells[row, colPrevSrcfState].Text == "A" && sheet.Cells[row, colSrcfState].Text == "폐기")
                        {
                            if (sheet.FindCell(row, "폐기사유").Text == "")
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn("폐기사유를 입력하세요."), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Commit = false;
                                Trans.Rollback();
                                break;
                            }
                            else
                                query += ", @pDISUSE_REASON = '" + sheet.FindCell(row, "폐기사유").Text + "'";

                        }
                        else if (sheet.Cells[row, colPrevSrcfState].Text == "D" && sheet.Cells[row, colSrcfState].Text == "활성")
                        {
                            query += ", @pDISUSE_REASON = '" + "'";
                        }
                        if (sheet.Cells[row, colSrcfState].Text == "폐기")
                        {
                            if (sheet.FindCell(row, "폐기일").Text == "")
                            {
                                sheet.FindCell(row, "폐기일").Text = SystemBase.Base.ServerTime("YYMMDD");
                                query += ", @pREPL_DT = '" + sheet.FindCell(row, "폐기일").Text + "'";
                            }
                            else
                            {
                                query += ", @pREPL_DT = '" + sheet.FindCell(row, "폐기일").Text + "'";
                            }
                        }
                        if (!string.IsNullOrEmpty(sheet.Cells[row, colRevDt].Text))
                            query += ", @pREV_DT = '" + Convert.ToDateTime(sheet.Cells[row, colRevDt].Text).ToShortDateString() + "'";
                        if (!string.IsNullOrEmpty(sheet.Cells[row, colEstbDt].Text))
                            query += ", @pESTB_DT = '" + Convert.ToDateTime(sheet.Cells[row, colEstbDt].Text).ToShortDateString() + "'";
                        query += ", @pPAGE_SIZE = '" + sheet.FindCell(row, "도면크기").Value + "'"
                            + ", @pPRNT_SIZE_CD = '" + sheet.FindCell(row, "인쇄크기").Value + "'";
                        if (!string.IsNullOrEmpty(sheet.Cells[row, colTotalPage].Text))
                            query += ", @pPAGE_TOTAL = '" + sheet.Cells[row, colTotalPage].Text + "'";
                        query += ", @pWRT_TM_CD = '" + sheet.FindCell(row, "작성처").Value + "'"
                            + ", @pWRT_TM_INFO = '" + sheet.FindCell(row, "작성처 상세").Value + "'"
                            + ", @pAPRV_TM_CD = '" + sheet.FindCell(row, "승인처").Value + "'"
                            + ", @pAPRV_TM_INFO = '" + sheet.FindCell(row, "승인처 상세").Value + "'"
                            + ", @pAPRV_PERS = '" + sheet.FindCell(row, "승인자").Text + "'"
                            + ", @pDWG_PERS = '" + sheet.FindCell(row, "제도자").Text + "'"
                            + ", @pDSGN_PERS = '" + sheet.FindCell(row, "설계자").Text + "'"
                            + ", @pINSP_PERS_LT = '" + sheet.FindCell(row, "검도자(좌)").Text + "'"
                            + ", @pINSP_PERS_BTRT = '" + sheet.FindCell(row, "검도자(우하)").Text + "'"
                            + ", @pINSP_PERS_MDRT = '" + sheet.FindCell(row, "검도자(우중)").Text + "'"
                            + ", @pREL_ITEM = '" + sheet.FindCell(row, "관련품목").Text + "'"
                            + ", @pAPPL_ITEM = '" + sheet.FindCell(row, "적용품목").Text + "'"
                            + ", @pAPPL_INFO = '" + sheet.FindCell(row, "적용시점").Text + "'"
                            + ", @pORG_TM_CD = '" + sheet.FindCell(row, "출처").Value + "'";
                        if (!string.IsNullOrEmpty(sheet.Cells[row, colPubDt].Text))
                            query += ", @pPUB_DT = '" + Convert.ToDateTime(sheet.Cells[row, colPubDt].Text).ToShortDateString() + "'";
                        query += ", @pPUB_APRV_NO = '" + sheet.FindCell(row, "출도승인번호").Text + "'"
                            + ", @pAVLB_PERIOD = '" + sheet.FindCell(row, "유효기간").Text + "'"
                            + ", @pFORM_IDNT_ID = '" + sheet.FindCell(row, "형상식별자ID").Text + "'"
                            + ", @pFORM_CTRL_ID = '" + sheet.FindCell(row, "형상통제자ID").Text + "'"
                            + ", @pSRCF_STATE = '" + sheet.FindCell(row, "자료상태").Value + "'"
                            + ", @pMOD_CD = '" + sheet.FindCell(row, "변경구분").Value + "'"
                            + ", @pMOD_SPEC = '" + sheet.FindCell(row, "변경항목").Text + "'"
                            + ", @pMOD_FROM = '" + sheet.FindCell(row, "변경전").Text + "'"
                            + ", @pMOD_TO = '" + sheet.FindCell(row, "변경후").Text + "'"
                            + ", @pMOD_REASON = '" + sheet.FindCell(row, "변경사유").Text + "'"
                            + ", @pBOM_MOD_YN = '" + sheet.FindCell(row, "BOM변경여부").Text + "'"
                            + ", @pREMARK = '" + sheet.FindCell(row, "비고").Text + "'"
                            + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'"
                            + ", @pSRC_NO = '" + cboSrcNoHeader.SelectedValue + "'";
                        // 자료파일 정보 저장
                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 새 자료파일 추가인 경우 파일정보 업데이트
                        if (strHead == "I")
                        {
                            if (Server.UploadSourceFile(docCode, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            { Trans.Rollback(); goto Exit; }; // 실패시 롤백
                        }
                    }
                }
                if(Commit)
                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                resultCode = "ER";
                resultMessage = e.Message;
                //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (resultMessage != "")
            {
                if (resultCode == "OK" && Commit)
                {
                    // 미선택 저장은 새로 저장
                    if (selectedSrcId < 0)
                    {
                        selectedSrcId = savedSrcId;
                        Search(selectedSrcId);
                    }
                    // 선택후 저장은 업데이트
                    else
                    {
                        UpdateSource(selectedSrcId);
                        SearchDetail(selectedSrcId);
                    }

                    MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (resultCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (Commit)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    UpdateSource(selectedSrcId);
                    SearchDetail(selectedSrcId);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 공유기능
        /// <summary>
        /// 자료 조회조건 문자열을 추가합니다.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>조건이 추가된 쿼리</returns>
        string AppendMasterQueryCondition(string query)
        {
            return query
                + ", @pPLANT_CD = '" + cboSPlant.SelectedValue + "'"
                + ", @pACPT_DT_FROM = '" + dteSAcptDtFr.Text + "'"
                + ", @pACPT_DT_TO = '" + dteSAcptDtTo.Text + "'"
                + ", @pENT_CD = '" + txtSEntCd.Text + "'"
                + ", @pACPT_USR_ID = '" + txtSAcptUsrId.Text + "'"
                + ", @pPUB_TM_CD = '" + cboSPubTeam.SelectedValue + "'"
                + ", @pDOC_CD = '" + cboSDocCd.SelectedValue + "'"
                + ", @pSRC_NO = '" + txtSSrcNo.Text + "'"
                + ", @pSRC_NM = '" + txtSSrcNm.Text + "'";
        }

        /// <summary>
        /// 편집기능을 켜거나 끕니다.
        /// </summary>
        /// <param name="enabled"></param>
        void EnableEditing(bool enabled)
        {
            panDetailMaster.Enabled = enabled;
            if (!enabled)
                ResetDetailInputPanel();
        }

        /// <summary>
        /// 사용자/사업 버튼과 연결된 아이디/코드 텍스트박스를 찾습니다.
        /// </summary>
        /// <param name="sender">버튼</param>
        /// <returns>아이디/코드 텍스트박스</returns>
        C1.Win.C1Input.C1TextBox FindCodeTextbox(object sender)
        {
            if (sender == btnSAcptUsr || sender == txtSAcptUsrId) return txtSAcptUsrId;
            else if (sender == btnAcptUsr || sender == txtAcptUsrId) return txtAcptUsrId;
            else if (sender == btnAppUsr || sender == txtAppUsrId) return txtAppUsrId;
            else if (sender == btnSEnt || sender == txtSEntCd) return txtSEntCd;
            else if (sender == btnEnt || sender == txtEntCd) return txtEntCd;
            return null;
        }

        /// <summary>
        /// 사용자/사업 버튼 또는 아이디/코드 텍스트박스와 연결된 사용자명/사업명 텍스트박스를 찾습니다.
        /// </summary>
        /// <param name="sender">사용자/사업 버튼, 아이디/코드 텍스트박스</param>
        /// <returns>이름 텍스트박스</returns>
        C1.Win.C1Input.C1TextBox FindNameTextbox(object sender)
        {
            if (sender == btnSAcptUsr || sender == txtSAcptUsrId) return txtSAcptUsrNm;
            else if (sender == btnAcptUsr || sender == txtAcptUsrId) return txtAcptUsrNm;
            else if (sender == btnAppUsr || sender == txtAppUsrId) return txtAppUsrNm;
            else if (sender == btnSEnt || sender == txtSEntCd) return txtSEntNm;
            else if (sender == btnEnt || sender == txtEntCd) return txtEntNm;
            return null;
        }

        /// <summary>
        /// 디테일 영역의 입력 패널을 리셋합니다.
        /// </summary>
        void ResetDetailInputPanel()
        {
            ShowSrcNoMessage();
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
            fpSpread1.ActiveSheet.Rows.Count = 0;
            fpSpread2.ActiveSheet.ClearSelection();
            cboDocCd.Enabled = true;
            cboSrcNoHeader.Focus();
            txtSrcNoIdntChar.ReadOnly = true;
        }

        /// <summary>
        /// 자료번호 경고 메시지 레이블의 텍스트를 설정합니다.
        /// </summary>
        void ShowSrcNoMessage()
        {
            ShowSrcNoMessage("");
        }

        /// <summary>
        /// 자료번호 경고 메시지 레이블의 텍스트를 설정합니다.
        /// </summary>
        /// <param name="msg">메시지 텍스트</param>
        void ShowSrcNoMessage(string msg)
        {
            lblSrcNoMessage.Text = msg;
        }

        /// <summary>
        /// 사용자 선택 팝업을 실행합니다.
        /// </summary>
        /// <param name="usrCd">사용자분류코드</param>
        /// <param name="userId">자동입력할 사용자 ID</param>
        /// <param name="userNm">자동입력할 사용자명</param>
        /// <returns>사용자가 선택되었을 경우 {사용자ID, 사용자명}을, 그렇지 않은 경우 null을 반환합니다.</returns>
        string[] ShowUserPopup(string usrCd, string userId, string userNm)
        {
            try
            {
                string query = "usp_B_COMMON 'B015' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pUSR_CD = '" + usrCd + "'";
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", query, new string[] { "@pCODE", "@pNAME" }, new string[] { userId, userNm }, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] values = rx1.Split(pu.ReturnVal.ToString());
                    if (values != null || values.Length > 1)
                        return new string[] { values[0], values[1] };
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            return null;
        }

        /// <summary>
        /// 자료파일의 갯수 표시를 업데이트합니다.
        /// </summary>
        /// <param name="all">전체목록인지 여부</param>
        void UpdateSourceFileStatus(bool all)
        {
            int count = fpSpread1.ActiveSheet.RowCount;
            txtSrcfStatus.Text = (all ? "조회조건의 모든 자료파일 " : "") + (count > 0 ? "총 " + count + "개" : "");
        }

        /// <summary>
        /// 자료파일의 갯수 표시를 업데이트합니다.
        /// </summary>
        void UpdateSourceFileStatus()
        {
            UpdateSourceFileStatus(false);
        }
        #endregion

        #region 디테일 그리드 이벤트 핸들러
        /// <summary>
        /// 셀 버튼 클릭 핸들러
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        protected override void fpButtonClick(int row, int col)
        {
            // 01818	생산부 생산기술팀

            // 01657	기계개발부
            // 02001	기계개발부 기계1팀
            // 02029	기계개발부 기계2팀
            // 01700	전자개발부
            // 02120	전자개발부 S/W팀
            // 01982	전자개발부 전자1팀
            // 02028	전자개발부 전자2팀
            // 02033	전자개발부 전자3팀
            // 02277	전자개발부 전자4팀

            // 01652	기술센터장
            // 01713	전략개발부
            // 02254	전략개발부 대전/분당분소
            // 02003	전략개발부 전략1팀
            // 01999	전략개발부 전략2팀
            // 02058	전략개발부 전략3팀
            // 02252	전략개발부 전략4팀
            // 02253	전략개발부 전략5팀

            if (col == colFormIdntButton || // 형상식별자 버튼
                col == colFormCtrlButton) // 형상통제자 버튼
            {
                SheetView sheet = fpSpread1.ActiveSheet;
                string[] values = ShowUserPopup("DEVENG", sheet.Cells[row, col - 1].Text, "");
                if (values != null)
                {
                    sheet.Cells[row, col - 1].Value = values[0];
                    sheet.Cells[row, col + 1].Value = values[1];
                    if (string.IsNullOrEmpty(sheet.RowHeader.Cells[row, 0].Text))
                        sheet.RowHeader.Cells[row, 0].Text = "U"; // 이거 자동으로 안되네 ㅡ,.ㅡ
                }
            }
        }

        private void fpSpread1_DragEnter(object sender, DragEventArgs e)
        {
            if (fpSpread1.ActiveSheet.RowCount > 0 && e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 파일 드롭시 규격번호, 페이지 값을 파싱해서 자동으로 첨부파일명 입력
        /// </summary>
        private void fpSpread1_DragDrop(object sender, DragEventArgs e)
        {
            SheetView sheet = fpSpread1.ActiveSheet;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                // 값 추출
                string[] values = Path.GetFileNameWithoutExtension(filename).Split('_');
                if (values.Length < 2) continue;

                // 파일명을 규격번호/페이지와 대조
                for (int n = 0; n < values.Length; n++) values[n] = values[n].Trim();
                for (int row = 0; row < sheet.RowCount; row++)
                {
                    string stndNo = sheet.Cells[row, colStndNo].Text;
                    string page = sheet.Cells[row, colPage].Text;
                    bool stndNoMatched = false;
                    bool machedAll = false;
                    bool[] available = new bool[values.Length];
                    for (int n = 0; n < values.Length; n++) available[n] = true;

                    // 규격번호 대조
                    for (int n = 0; n < values.Length; n++)
                        if (available[n] && string.Compare(values[n], stndNo, true) == 0)
                        {
                            available[n] = false;
                            stndNoMatched = true;
                            break;
                        }

                    // 일치시 페이지 대조
                    if (stndNoMatched)
                        for (int n = 0; n < values.Length; n++)
                            if (available[n] && string.Compare(values[n], page, true) == 0)
                            {
                                machedAll = true;
                                break;
                            }

                    // 일치시 파일명 대입
                    if (machedAll && buttonManager.SetAttachedFilename(row, filename))
                        break;
                }
            }
        }
        #endregion

        #region 컨트롤 이벤트 핸들러
        /// <summary>
        /// 사업코드 팝업
        /// </summary>
        private void btnEnt_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { sender == btnSEnt ? txtSEntCd.Text : txtEntCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    FindCodeTextbox(sender).Value = Msgs[0];
                    FindNameTextbox(sender).Value = Msgs[1];
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 조회된 자료마스터에 포함된 모든 자료파일 조회
        /// </summary>
        private void btnSearchAllSrcf_Click(object sender, EventArgs e)
        {
            selectedSrcId = -1;
            fpSpread2.ActiveSheet.ResetSelection();
            SearchDetail(-1);
        }

        /// <summary>
        /// 사용자 팝업(조회 접수자, 접수자, 승인자)
        /// </summary>
        private void btnUser_Click(object sender, EventArgs e)
        {
            strBtn = "Y";
            string[] values = ShowUserPopup("ENG", FindCodeTextbox(sender).Text, "");
            if (values != null)
            {
                FindCodeTextbox(sender).Value = values[0];
                FindNameTextbox(sender).Value = values[1];
            }
            strBtn = "N";
        }

        private void cboDocCd_TextChanged(object sender, EventArgs e)
        {
            txtSrcNoIdntChar.Value = cboDocCd.GetItemText(cboDocCd.SelectedIndex, 2);
        }

        private void SourceNumber_Changed(object sender, EventArgs e)
        {
            // 자료번호 발리데이션
            string strFileSql = "usp_TDT001 'S3'"
                + ", @pSRC_ID = " + selectedSrcId
                + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                + ", @pPLANT_CD = '" + cboPlant.SelectedValue + "'"
                + ", @pSRC_NO = '" + this.SourceNumber + "'";
            DataSet dsf = SystemBase.DbOpen.NoTranDataSet(strFileSql);
            if (dsf.Tables.Count > 0 && dsf.Tables[0].Rows.Count > 0) ShowSrcNoMessage("이미 등록된 자료번호입니다.");
            else ShowSrcNoMessage();
        }

        private void txtEntCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                C1.Win.C1Input.C1TextBox entCdBox = (C1.Win.C1Input.C1TextBox)sender;
                if (entCdBox.Text != "")
                {
                    FindNameTextbox(sender).Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", entCdBox.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    FindNameTextbox(sender).Value = "";
                }
            }
            catch { }
        }

        private void txtUserId_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
            {
                FindNameTextbox(sender).Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", FindCodeTextbox(sender).Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
        }
        #endregion
    }

}