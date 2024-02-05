#region 작성정보
/*********************************************************************/
// 단위업무명: 지출증빙등록
// 작 성 자  : 한 미 애
// 작 성 일  : 2021-12-02
// 작성내용  : 전표에 대한 지출증빙문서를 등록한다.
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;

namespace AD.ACD001
{
    public partial class ACD001P8 : UIForm.FPCOMM1
    {
        #region Field
        /// <summary>전표번호</summary> 
        string SlipNo = null;
        /// <summary>부서코드</summary> 
        string DeptCd = null;
        /// <summary>부서명</summary> 
        string DeptNm = null;
        /// <summary>DocCd</summary> 
        string UserId = null;
        /// <summary>DocType</summary> 
        string UserNm = null;
        string GwStatus = null;

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

        /// <summary>문서코드별 문서번호 유무</summary>
        Dictionary<string, string> docNoReqs = null;
        /// <summary>첨부파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;
        /// <summary>첨부문서표시 관리자</summary>
        AttachmentManager attachmentManager;
        #endregion

        # region Initialize
        public ACD001P8()
        {
            InitializeComponent();
        }
        public ACD001P8(string SLIP_NO, string DEPT_CD, string DEPT_NM, string USER_ID, string USER_NM, string GW_STATUS) : this() 
        {
            this.SlipNo = SLIP_NO;
            this.DeptCd = DEPT_CD;
            this.DeptNm = DEPT_NM;
            this.UserId = USER_ID;
            this.UserNm = USER_NM;
            this.GwStatus = GW_STATUS;

            this.Size = new System.Drawing.Size(1240, 785);
        }
        #endregion

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

        #region Form 핸들러
        /// <summary>
        /// Form Load
        /// </summary>
        private void ACD001P8_Load(object sender, System.EventArgs e)
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                UIForm.Buttons.ReButton("010111010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "전표지출증빙등록";

                SystemBase.ComboMake.C1Combo(cboGwStatus, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B094', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);

                G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'ACD', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                txtSlipNo.Value = this.SlipNo;
                txtDeptCd.Value = this.DeptCd;
                txtDeptNm.Value = this.DeptNm;
                txtUserId.Value = this.UserId;
                txtUserNm.Value = this.UserNm;
                cboGwStatus.SelectedValue = this.GwStatus;

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

        /// <summary>
        /// 조회
        /// </summary>        
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string query = "usp_T_DOC 'S1'"
                    + ", @pDOC_CTG_CD = 'ACD'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pATT_KEY = '" + txtSlipNo.Text + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                buttonManager.UpdateButtons(); // 버튼 업데이트

                SheetView sheet = fpSpread1.ActiveSheet;
                ((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            //webBrowser.Navigate("about:blank", false);
            pdfViewer.LoadFile("about:blank");
            this.Cursor = Cursors.Default;
        }


        /// <summary>
        /// 행 추가
        /// </summary>
        protected override void RowInsExec()
        {
            if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
            {
                MessageBox.Show("결재 승인/반려된 건이므로 지출증빙을 수정할 수 없습니다.");
                return;
            }

            SheetView sheet = fpSpread1.ActiveSheet;
            fpSpread1.Focus();

            UIForm.FPMake.RowInsert(fpSpread1); // 행추가
            int newRow = sheet.ActiveRowIndex;
            sheet.Cells[newRow, colRegUsrId].Value = SystemBase.Base.gstrUserID;
            sheet.Cells[newRow, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
            buttonManager.UpdateButtons(newRow); // 버튼 업데이트
        }

        /// <summary>
        /// 행 삭제
        /// </summary>
        protected override void DelExec()
        {
            if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
            {
                MessageBox.Show("결재 승인/반려된 건이므로 지출증빙을 수정할 수 없습니다.");
                return;
            }

            SheetView sheet = fpSpread1.ActiveSheet;
            if (sheet.RowCount < 1) return;
            CellRange[] ranges = sheet.GetSelections();
            if (ranges.Length == 0) return;    
            
            base.DelExec();
        }

        /// <summary>
        /// 저장
        /// </summary>
        protected override void SaveExec()
        {
            if (cboGwStatus.SelectedValue.ToString() == "APPR" || cboGwStatus.SelectedValue.ToString() == "REJECT")
            {
                MessageBox.Show("결재 승인/반려된 건이므로 지출증빙을 수정할 수 없습니다.");
                return;
            }

            SheetView sheet = fpSpread1.ActiveSheet;
            if (sheet.Rows.Count < 1) return;
            if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

            this.Cursor = Cursors.WaitCursor;
            fpSpread1.Focus();

            string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                // 지출증빙 저장하기 전에 체크
                string query_Chk = "usp_ACD001_P8 @pTYPE = 'C1' ";
                query_Chk += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                query_Chk += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
                query_Chk += ", @pSLIP_NO = '" + txtSlipNo.Value.ToString() + "' ";

                DataSet ds_Chk = SystemBase.DbOpen.TranDataSet(query_Chk, dbConn, Trans);
                resultCode = ds_Chk.Tables[0].Rows[0][0].ToString();
                resultMessage = ds_Chk.Tables[0].Rows[0][1].ToString();

                if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                for (int row = 0; row < sheet.RowCount; row++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;

                    string strGbn = "";
                    switch (strHead)
                    {
                        case "U": strGbn = "U1"; break;
                        case "I": strGbn = "I1"; break;
                        case "D": strGbn = "D1"; break;
                        default: continue;
                    }                    

                    if (strHead == "D")
                    {
                        // 문서 삭제
                        string strSql = string.Format("usp_T_DOC @pTYPE = '" + strGbn + "', @pDOC_ID = {0}", sheet.Cells[row, colDocId].Value);

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                    else
                    {
                        if (!(Path.GetExtension(sheet.Cells[row, colOrgFnm].Text).Equals(".pdf") ) )
                        {
                            MessageBox.Show("PDF파일만 업로드 가능합니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Trans.Rollback();
                            resultCode = "WR";
                            goto Exit;
                        }

                        string query = "usp_T_DOC @pTYPE = '" + strGbn + "'";
                        if (strHead == "I") // 새로 추가
                            query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                                + ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' "
                                + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                                + ", @pATT_KEY = '" + txtSlipNo.Text + "'"
                                + ", @pATT_KEY1 = '" + txtSlipNo.Text + "'"
                                + ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text + "'";     //txtDocCode.Text + "'";
                        else // 내용 변경
                            query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Text;

                        query += ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
                                + ", @pREV_NO = '" + sheet.Cells[row, colRevNo].Text + "'"
                                + ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
                                + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        // 문서정보 저장
                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        // 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
                        if (strHead == "I")
                        {
                            //if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            { Trans.Rollback(); goto Exit; }; // 실패시 롤백
                        }
                    }
                }

                Trans.Commit();

                //// 품목의 첨부문서 코드문자열 업데이트
                //attachmentManager.ReloadData(0, new string[] { txtSlipNo.Text});
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
                MessageBox.Show(SystemBase.Base.MessageRtn(resultMessage), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
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

        #region 그리드 이벤트 핸들러
        /// <summary>
        /// 그리드 Change 이벤트
        /// </summary>
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

        /// <summary>
        /// 셀클릭시 View에 증빙자료 표시
        /// </summary>
        private void fpSpread1_CellClick(object sender, CellClickEventArgs e)
        {
            if (e.Column == colOrgFnm)      // 파일명 항목을 클릭했을때만 
            {
                SheetView sheet = fpSpread1.ActiveSheet;
                if (sheet.RowCount <= 0) return;
                int row = e.Row;
                string filename;
                string msg = null;
                string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                //webBrowser.Navigate("about:blank", false);
                pdfViewer.LoadFile("about:blank");

                string ftppath = Url.Combine(Server.DocumentUrl, sheet.Cells[row, colSvrPath].Text + "/" + sheet.Cells[row, colSvrFnm].Text);
                string ext = Path.GetExtension(sheet.Cells[row, colOrgFnm].Text); // 확장자
                if (!string.IsNullOrEmpty(ext)) ext = ext.Substring(1);

                // 파일삭제 ( _*.* )
                ViewDeleteTempFiles();

                // 미리보기시 파일이 사용중이므로 삭제처리를 위해 랜덤파일명 사용
                do { filename = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), ViewGetTempFilenamePrefix() + Path.GetRandomFileName()), ext); } while (File.Exists(filename));

                bool ok = Ftp.DownloadFile(filename, ftppath, Server.AccountName, Server.AccountPassword, ref msg);
                if (ok)
                {
                    // webBrowser.Navigate(filename);
                    pdfViewer.LoadFile(filename);
                }
            }
        }

        #endregion

    }
}
