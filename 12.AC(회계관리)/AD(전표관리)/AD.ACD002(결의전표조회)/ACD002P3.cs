#region 작성정보
/*********************************************************************/
// 단위업무명: 지출증빙등록
// 작 성 자  : 한 미 애
// 작 성 일  : 2022-03-21
// 작성내용  : 전표에 대한 지출증빙 일괄등록 처리
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/
#endregion


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

namespace AD.ACD002
{
    public partial class ACD002P3 : UIForm.FPCOMM2
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread fpAssignGrid;
        string[,] strHdx = null;

        string[] returnVal = null;
        string strChgFlag = "";
        string strCheckResultMsg = "";

        /// <summary>문서카테고리 코드</summary>
        const string docCtgCd = "ACD";          //전표증빙

        /// <summary>서브 그리드의 현재 선택된 행</summary>
        int selectedSubRow = -1;

        // 서브 그리드 컬럼(전표번호 목록)
        int colSlipNo = -1;
        int colAssignNo = -1;

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

        /// <summary>첨부파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;
        #endregion

        public ACD002P3()
        {
            InitializeComponent();
        }

        public ACD002P3(FarPoint.Win.Spread.FpSpread ASSIGN_GRID, string[,] GRID_IDX)
        {
            fpAssignGrid = ASSIGN_GRID;
            strHdx = GRID_IDX;

            InitializeComponent();
        }

        #region ACD002P3_Load(): Form Load 시
        private void ACD002P3_Load(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.Buttons.ReButton("000111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "결의전표조회 > 지출증빙일괄등록";

                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "결재상태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B094', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
                G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "생성경로")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'A101', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'ACD', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류

                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                SheetView subSheet = fpSpread2.ActiveSheet;
                colSlipNo = subSheet.FindHeaderColumnIndex("결의번호");
                colAssignNo = subSheet.FindHeaderColumnIndex("결재요청번호");

                SheetView sheet = fpSpread1.ActiveSheet;
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

                // 첨부파일목록 파일버튼 관리자 초기화
                buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
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

                SearchExec();
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
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);
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
                int SelectedRow;

                for (int i = 0; i < fpAssignGrid.Sheets[0].Rows.Count; i++)
                {
                    if (fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "선택")].Text == "True")
                    {
                        // 그리드에 행추가
                        UIForm.FPMake.RowInsert(fpSpread2);
                        SelectedRow = fpSpread2.Sheets[0].ActiveRowIndex;

                        // 선택된 행의 항목들을 행추가된 항목에 복사
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text = "True";      // 기본적으로 선택된 상태로 조회되게
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의일자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의일자")].Text;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의번호")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "전표형태")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "전표형태")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "차변금액")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "차변금액")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결의부서")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결의부서")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "생성경로")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "생성경로")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "관련번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "관련번호")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결재요청번호")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재요청번호")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "상신일자")].Text = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "상신일자")].Text;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "결재상태")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "결재상태")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "증빙건수")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "증빙건수")].Value;
                        fpSpread2.Sheets[0].Cells[SelectedRow, SystemBase.Base.GridHeadIndex(GHIdx2, "등록자")].Value = fpAssignGrid.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(strHdx, "상신자")].Value;

                        fpSpread2.Sheets[0].RowHeader.Cells[SelectedRow, 0].Value = "";     // 행추가하여 만들어진 I 없애기

                        // 그리드 라인 Lock 처리
                        if (fpSpread2.Sheets[0].Rows.Count > 0)
                        {
                            UIForm.FPMake.grdReMake(fpSpread1,
                                                  SystemBase.Base.GridHeadIndex(GHIdx2, "결의일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "전표형태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "차변금액") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "결의부서") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "생성경로") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "관련번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "결재요청번호") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "상신일자") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "결재상태") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "증빙건수") + "|3"
                                                      + "#" + SystemBase.Base.GridHeadIndex(GHIdx2, "등록자") + "|3"
                                                   );
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec(): 저장 버튼 클릭시 문서 저장 처리
        protected override void SaveExec()
        {
            GridDataCheck();

            if (strCheckResultMsg != "")
            {
                MessageBox.Show(strCheckResultMsg);
                return;
            }

            SheetView sheet = fpSpread1.ActiveSheet;
            SheetView subSheet = fpSpread2.ActiveSheet;

            if (sheet.Rows.Count < 1) return;
            RowDataList attKeys = null;

            if (sheet.CheckRowInserted())
            {
                attKeys = subSheet.GetCheckedRowData(new int[] { colSlipNo, colAssignNo });
                if (attKeys == null)
                {
                    MessageBox.Show("먼저 첨부하려는 결의전표에 체크를 하십시오.", "첨부파일 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

            this.Cursor = Cursors.WaitCursor;
            fpSpread1.Focus();

            string strResultMsg = "";

            string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSlipNo = "";
                int iActiveRow = 0;
                string strHead;

                // 수정 또는 삭제건 처리
                for (int row = 0; row < sheet.RowCount; row++)
                {
                    strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;

                    string strGbn = "";
                    switch (strHead)
                    {
                        case "U": strGbn = "U1"; break;
                        case "D": strGbn = "D1"; break;
                        default: continue;
                    }

                    if (strHead == "D")
                    {
                        iActiveRow = subSheet.ActiveRowIndex;
                        strSlipNo = subSheet.Cells[iActiveRow, colSlipNo].Text;

                        // 지출증빙 저장하기 전에 체크(결재상태)
                        string query_Chk = "usp_ACD001P11 @pTYPE = 'C1' ";
                        query_Chk += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        query_Chk += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        query_Chk += ", @pSLIP_NO = '" + strSlipNo + "'";
                        query_Chk += ", @pHEAD_TYPE = '" + strHead + "' ";        // 2022.04.19. hma 추가: 라인 헤더문자

                        DataSet ds_Chk = SystemBase.DbOpen.TranDataSet(query_Chk, dbConn, Trans);
                        resultCode = ds_Chk.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds_Chk.Tables[0].Rows[0][1].ToString();
                        strResultMsg = resultMessage;

                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 문서 삭제
                        string strSql = string.Format("usp_T_DOC @pTYPE = '" + strGbn + "', @pDOC_ID = {0}", sheet.Cells[row, colDocId].Value);

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }    // ER 코드 Return시 점프

                        strResultMsg = resultMessage;
                    }
                    else if (strHead == "U") // 수정한 경우
                    {
                        string query = "usp_T_DOC @pTYPE = '" + strGbn + "'";                        
                        query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Text;
                        query += ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
                                + ", @pREV_NO = '" + sheet.Cells[row, colRevNo].Text + "'"
                                + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        // 문서정보 저장
                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }    // ER 코드 Return시 점프

                        strResultMsg = resultMessage;
                    }
                }

                // 추가건 처리
                int iNewPid, iSlipCnt;
                string strCheckYn;

                for (int row = 0; row < sheet.RowCount; row++)
                {
                    strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;

                    string strGbn = "";
                    switch (strHead)
                    {
                        case "I": strGbn = "I1"; break;    
                        default: continue;
                    }
                        
                    if (!(Path.GetExtension(sheet.Cells[row, colOrgFnm].Text).Equals(".pdf")))
                    {
                        MessageBox.Show("PDF파일만 업로드 가능합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Trans.Rollback();
                        resultCode = "WR";
                        goto Exit;
                    }

                    iNewPid = 0;        // 신규 증빙등록된 문서ID
                    iSlipCnt = 0;       // 증빙등록한 전표건수
                    strCheckYn = "";

                    for (int srow = 0; srow < subSheet.Rows.Count; srow++)
                    {
                        strSlipNo = subSheet.Cells[srow, colSlipNo].Text;
                        strCheckYn = subSheet.Cells[srow, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text;

                        // 선택된 건이 아니면 통과
                        if (strCheckYn != "True")
                            continue;

                        // 지출증빙 저장하기 전에 체크
                        string query_Chk = "usp_ACD001P11 @pTYPE = 'C1' ";
                        query_Chk += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        query_Chk += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        query_Chk += ", @pSLIP_NO = '" + strSlipNo + "'";
                        query_Chk += ", @pHEAD_TYPE = '" + strHead + "' ";        // 2022.04.19. hma 추가: 라인 헤더문자

                        DataSet ds_Chk = SystemBase.DbOpen.TranDataSet(query_Chk, dbConn, Trans);
                        resultCode = ds_Chk.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds_Chk.Tables[0].Rows[0][1].ToString();
                        strResultMsg = resultMessage;

                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }    // ER 코드 Return시 점프

                        // 추가한 문서 저장
                        string query = "usp_T_DOC @pTYPE = '" + strGbn + "'";
                        query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                                + ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";

                        // 첫번째 전표건인 경우
                        if (iSlipCnt == 0)
                        {
                            query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                                    + ", @pATT_KEY = '" + strSlipNo + "'"
                                    + ", @pATT_KEY1 = '" + strSlipNo + "'"
                                    + ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text + "'"
                                    + ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
                                    + ", @pREV_NO = '" + sheet.Cells[row, colRevNo].Text + "'"
                                    + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                    + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        }
                        else    // 두번째 이상 전표건인 경우
                        {
                            query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                                    + ", @pDOC_PID = " + iNewPid 
                                    + ", @pATT_KEY = '" + strSlipNo + "'"
                                    + ", @pATT_KEY1 = '" + strSlipNo + "'"
                                    + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                    + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        }

                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }    // ER 코드 Return시 점프

                        strResultMsg = resultMessage;

                        // 지출증빙 갯수가 제한갯수 초과하는지 체크
                        string query_Chk2 = "usp_ACD001P11 @pTYPE = 'C2' ";
                        query_Chk2 += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        query_Chk2 += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                        query_Chk2 += ", @pSLIP_NO = '" + strSlipNo + "'";

                        DataSet ds_Chk2 = SystemBase.DbOpen.TranDataSet(query_Chk2, dbConn, Trans);
                        resultCode = ds_Chk2.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds_Chk2.Tables[0].Rows[0][1].ToString();

                        if (resultCode == "OK") resultMessage = strResultMsg;       // 문제 없을 경우 등록 관련 결과 메시지로 보여지도록 함.
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }    // ER 코드 Return시 점프

                        // 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
                        if ((strHead == "I") && (iSlipCnt == 0))
                        {
                            iNewPid = Convert.ToInt32(ds.Tables[0].Rows[0][2]);     // 새로 추가된 문서레코드 ID
                            if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            { Trans.Rollback(); goto Exit; }; // 실패시 롤백
                            iSlipCnt++;
                        }
                    }
                }

                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                resultCode = "ER";
                resultMessage = e.Message;
            }
        Exit:
            dbConn.Close();
            if (resultCode == "OK")
            {
                strChgFlag = "Y";
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                RtnStr();

                DocRowHdrClear();
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
            else if (resultCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Cursor = Cursors.Default;

        }
        #endregion

        #region GridDataCheck(): 그리드 데이터 체크
        private void GridDataCheck()
        {
            int iChkCnt = 0;
            int iInsRowCnt = 0;
            string strSlipNo = "";
            strCheckResultMsg = "";

            // 선택건수 체크
            if (strCheckResultMsg == "")
            {
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    // 2022.04.19. hma 수정(Start): 결재상태가 승인이라도 증빙 추가는 가능하도록 하기로 해서 이 부분 주석 처리함. 저장시에도 체크하므로 여기에서 안해도 될듯.
                    // 결재상태 체크. 승인 상태인 건만 제외. 반려된 건은 재상신을 하기 위해 등록을 해야될 수도 있으므로 허용.
                    //if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "결재상태")].Value.ToString() == "APPR")
                    //{
                    //    strSlipNo = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "결의번호")].Text;
                    //    strCheckResultMsg = strSlipNo + ": 결재상태가 승인이므로 지출증빙등록 할 수 없습니다.";
                    //    break;
                    //}
                    // 2022.04.19. hma 수정(End)

                    if (fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text == "True")
                    {
                        iChkCnt++;
                    }
                }

                // 상단 전표 그리드에 선택된 건이 없는 경우, 하단 문서 그리드에 추가 라인이 있는지 체크하여 메시지 띄움.
                if ((strCheckResultMsg == "") &&(iChkCnt == 0))
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text == "I")
                        {
                            iInsRowCnt++;
                        }
                    }

                    if (iInsRowCnt > 0)
                        strCheckResultMsg = "지출증빙등록을 위하여 선택된 건이 없습니다.";
                }
            }
        }
        #endregion

        #region RowInsExec(): 행추가/행삭제 버튼 클릭시. 하단 문서 그리드에 행추가/행삭제 처리
        protected override void RowInsExec()
        {
            SheetView sheet = fpSpread1.ActiveSheet;
            fpSpread1.Focus();

            UIForm.FPMake.RowInsert(fpSpread1); // 행추가
            int newRow = sheet.ActiveRowIndex;
            sheet.Cells[newRow, colRegUsrId].Value = SystemBase.Base.gstrUserID;
            sheet.Cells[newRow, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
            buttonManager.UpdateButtons(newRow); // 버튼 업데이트
        }

        protected override void DelExec()
        {
            SheetView sheet = fpSpread1.ActiveSheet;
            if (sheet.RowCount < 1) return;
            CellRange[] ranges = sheet.GetSelections();
            if (ranges.Length == 0) return;

            base.DelExec();
        }
        #endregion

        #region ReturnVal(), RtnStr(): 그리드 선택값 입력 및 전송
        public string[] ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr()
        {
            returnVal = new string[2];
            returnVal[0] = strChgFlag;
        }
        #endregion

        #region fpSpread2_SelectionChanged(): 상단 전표 그리드 클릭시. 하단 문서 그리드에 해당 전표에 대한 데이터 조회되게.
        private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SheetView sheet = fpSpread2.ActiveSheet;
            if (sheet.ActiveRowIndex == selectedSubRow) return;
            selectedSubRow = sheet.RowCount > 0 ? sheet.ActiveRowIndex : -1;

            fpSpread1.ActiveSheet.RowCount = 0;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                int SelectedRow = fpSpread2.Sheets[0].ActiveRowIndex;

                string query = "usp_T_DOC 'S1'"
                    + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pATT_KEY = '" + fpSpread2.ActiveSheet.Cells[selectedSubRow, colSlipNo].Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                buttonManager.UpdateButtons(); // 버튼 업데이트=

                SheetView Docsheet = fpSpread1.ActiveSheet;

                // 개정번호 자릿수 제한
                ((FarPoint.Win.Spread.CellType.TextCellType)Docsheet.Columns[colRevNo].CellType).MaxLength = 5;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region fpSpread1_ChangeEvent(): 그리드 항목 변경시. 
        protected override void fpSpread1_ChangeEvent(int row, int col)
        {
            try
            {
                // 문서종류
                if (col == colDocNm)
                {
                    SheetView sheet = fpSpread1.ActiveSheet;
                    sheet.Cells[row, colDocCd].Value = sheet.Cells[row, colDocNm].Value;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region DocRowHdrClear(): 문서를 추가 또는 수정한 경우 처리후 행의 I/U 문자 없애기
        private void DocRowHdrClear()
        {
            SheetView sheet = fpSpread1.ActiveSheet;
            string strHead = "";
            for (int row = 0; row < sheet.RowCount; row++)
            {
                strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                if (string.IsNullOrEmpty(strHead)) continue;
                else fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text = "";
            }
        }
        #endregion
    }
}
