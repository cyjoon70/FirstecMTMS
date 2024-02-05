using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.Model;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;
using System.IO;
using System.Data.SqlClient;

namespace FilesCommon
{
    public partial class FilesForm : UIForm.FPCOMM1
    {

        #region Field

        /// <summary>키값</summary> 
        string KeyNo = string.Empty;

        /// <summary>삭제 여부</summary> 
        string DelYn = string.Empty;

        // 디테일 그리드 컬럼
        int colFileNo = -1;
        int colFileSeq = -1;
        int colFileChoice = -1;
        int colFileDel = -1;
        int colFileView = -1;
        int colFileDown = -1;
        int colFileNm = -1;
        int colFullFileNm = -1;
		int colSvrPath = -1;
		int colSvrFileNm = -1;

		FtpUtil ftpUtil = new FtpUtil();

        #endregion

        #region Initialize

        public FilesForm()
        {
            InitializeComponent();
        }

        public FilesForm(string KEY_NO, string DEL_YN) : this()
        {
            this.KeyNo = KEY_NO;
            this.DelYn = DEL_YN;

            InitializeComponent();
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
        private void FilesForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (DelYn == "N")
                    UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                else
                    UIForm.Buttons.ReButton("010101010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                
                this.Text = "첨부파일";
                this.Width = 800;
                this.Height = 300;

                UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                SheetView sheet = fpSpread1.ActiveSheet;
                colFileNo = sheet.FindHeaderColumnIndex("File No");
                colFileSeq = sheet.FindHeaderColumnIndex("File Seq");
                colFileChoice = sheet.FindHeaderColumnIndex("파일선택");
                colFileDel = sheet.FindHeaderColumnIndex("파일삭제");
                colFileView = sheet.FindHeaderColumnIndex("파일보기");
                colFileDown = sheet.FindHeaderColumnIndex("다운로드");
                colFileNm = sheet.FindHeaderColumnIndex("파일명");
                colFullFileNm = sheet.FindHeaderColumnIndex("FULL 파일명");
				colSvrPath = sheet.FindHeaderColumnIndex("서버 파일 경로");
				colSvrFileNm = sheet.FindHeaderColumnIndex("서버파일명");
				
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
                string query = "usp_B_IMAGE_SCM 'S1'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pFILES_NO = '" + KeyNo + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "파일선택")].Locked = true;
                fpSpread1.Sheets[0].Columns[SystemBase.Base.GridHeadIndex(GHIdx1, "파일삭제")].Locked = true;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        
        /// <summary>
        /// 행 추가
        /// </summary>
        protected override void RowInsExec()
        {            
            SheetView sheet = fpSpread1.ActiveSheet;
            fpSpread1.Focus();

            UIForm.FPMake.RowInsert(fpSpread1); // 행추가
            int newRow = sheet.ActiveRowIndex;
            sheet.Cells[newRow, colFileNo].Value = KeyNo;
            sheet.Cells[newRow, colFileChoice].Locked = false;
            sheet.Cells[newRow, colFileDel].Locked = false;
            sheet.Cells[newRow, colFileDown].Locked = true;
            sheet.Cells[newRow, colFileView].Locked = true;
        }

        /// <summary>
        /// 행 삭제
        /// </summary>
        protected override void DelExec()
        {
            SheetView sheet = fpSpread1.ActiveSheet;
            if (sheet.RowCount < 1) return;
            CellRange[] ranges = sheet.GetSelections();
            if (ranges.Length == 0) return;

            UIForm.FPMake.RowRemove(fpSpread1);
            DelExe();
        }
        
        /// <summary>
        /// 저장
        /// </summary>
        protected override void SaveExec()
        {
            string strResultMsg = "";       // 2022.02.19. hma 추가
            string resultCode = "WR", resultMessage = "P0000";  //처리할 내용이 없습니다.
            int resultSeq = 0;

            SheetView sheet = fpSpread1.ActiveSheet;

            if (sheet.Rows.Count < 1) return;
            if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

            this.Cursor = Cursors.WaitCursor;
            fpSpread1.Focus();

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                for (int row = 0; row < sheet.RowCount; row++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
                    if (string.IsNullOrEmpty(strHead)) continue;

                    string strGbn = "";
                    switch (strHead)
                    {
                        case "I": strGbn = "I1"; break;
                        case "D": strGbn = "D1"; break;
                        default: continue;
                    }

                    if (strHead == "D")
                    {
                        // 문서 삭제 CO_CD = @pCO_CD AND FILES_NO = @pFILES_NO AND FILES_SEQ = @pFILES_SEQ
                        string strSql = "usp_B_IMAGE_SCM 'D1'"
                            + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                            + ", @pFILES_NO = '" + KeyNo + "'"
                            + ", @pFILES_SEQ = " + sheet.Cells[row, colFileSeq].Text;

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        strResultMsg = resultMessage;       // 2022.02.18. hma 추가

						// 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
						if (!ftpUtil.DeleteFile(sheet.Cells[row, colSvrPath].Text + "/" + sheet.Cells[row, colSvrFileNm].Text, false))
						{
							Trans.Rollback(); goto Exit; // 실패시 롤백
						}
					}
                    else if ((strHead == "I"))
                    {

                        if(string.IsNullOrEmpty(sheet.Cells[row, colFileNm].Text))
                        {
                            MessageBox.Show("첨부파일을 선택해주세요.");
                            return;
                        }

                        string query = "usp_B_IMAGE_SCM 'I1'"
                            + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                            + ", @pFILES_NO = '" + KeyNo + "'"
                            + ", @pFILE_NAME = '" + sheet.Cells[row, colFileNm].Text + "'"
                            + ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        resultSeq = Convert.ToInt32(ds.Tables[0].Rows[0][2]);
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        strResultMsg = resultMessage;

                        // 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
                        if (!FileUpload(resultSeq, DateTime.Now, sheet.Cells[row, colFullFileNm].Text))
                        {
                            Trans.Rollback(); goto Exit; // 실패시 롤백
                        }
                    }
                }

                if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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

        #region 그리드 이벤트
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일선택"))
                {
					OpenFileDialog dlg = new OpenFileDialog();
					dlg.Filter = "전체(*.*)|*.*|gif 이미지(*.gif)|*.gif|jpg 이미지(*.jpg)|*.jpg|bmp 이미지(*.bmp)|*.bmp|xls Excel(*.xls)|*.xls";
					dlg.Multiselect = false;

					if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string fileNames = dlg.FileName;
                        string FileFullName = fileNames.Substring(fileNames.ToString().LastIndexOf(@"\") + 1, fileNames.Length - fileNames.ToString().LastIndexOf(@"\") - 1);
                        string FileName = FileFullName.Substring(0, FileFullName.ToString().LastIndexOf("."));
                        string FileKind = FileFullName.Substring(FileFullName.ToString().LastIndexOf(".") + 1, FileFullName.Length - FileFullName.ToString().LastIndexOf(".") - 1);

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일명")].Text = FileName + "." + FileKind;
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "FULL 파일명")].Text = fileNames;
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일삭제"))
                {
                    if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "File Seq")].Text))
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일명")].Text = "";
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "FULL 파일명")].Text = "";
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일보기"))
                {
					ftpUtil.ViewDocumentFile(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "서버 파일 경로")].Text
						+ "/" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "서버파일명")].Text);
				}
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "다운로드"))
                {
					SaveFileDialog dlg = new SaveFileDialog();
					dlg.FileName = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "파일명")].Text;
					dlg.DefaultExt = Path.GetExtension(dlg.FileName);
					dlg.Filter = dlg.DefaultExt.ToUpper() + " 파일" + "|*." + dlg.DefaultExt;
					if (dlg.ShowDialog() == DialogResult.OK)
						ftpUtil.DownloadFile(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "서버 파일 경로")].Text
							+ "/" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "서버파일명")].Text,
							dlg.FileName,
							true);
				}
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
            }
			finally
			{
				if ((Column == SystemBase.Base.GridHeadIndex(GHIdx1, "파일보기")) || (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "다운로드")))
				{
					fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";
				}
			}
        }
        #endregion

        #region File upload / download

        private bool FileUpload(int docSeq, DateTime docDate, string filepath)
        {
            bool bReturn = true;

            if (ftpUtil.UploadDocumentFile(docSeq, docDate, filepath) != FtpUtil.UploadResultState.Ok)
            {
                bReturn = false;
            }
            
            return bReturn;
        }

        private void FileDownload(int docSeq, DateTime docDate, string filepath)
        {
            
        }

        #endregion

    }
}
