using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;


namespace TD.TDA009
{
	public partial class TDA009 : UIForm.FPCOMM1
	{
		#region 필드
		/// <summary>
		/// 정리파일정보
		/// </summary>
		struct DeletedFileInfo
		{
			public int Id;
			public string Filename;

			public DeletedFileInfo(int id, string filename)
			{
				this.Id = id;
				this.Filename = filename;
			}
		};

		// 그리드 컬럼(문서목록)
		int colSvrPath = -1;
		int colSvrFnm = -1;
		int colOrgFnm = -1;
		int colDocNm = -1;
		int colDocNo = -1;
		int colRevNo = -1;

		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		#endregion

		#region 생성자
		public TDA009()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트
		private void TDA004_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 공장
			SystemBase.ComboMake.C1Combo(cboDocCtgCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서카테고리
			rdoDocFile_CheckedChanged(rdoDocFile, null);

			// 그리드 초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

			// 컬럼 인덱스
			SheetView sheet = fpSpread1.ActiveSheet;
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
			colDocNm = sheet.FindHeaderColumnIndex("문서종류");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호");
			colRevNo = sheet.FindHeaderColumnIndex("개정번호");

			// 첨부파일목록 파일버튼 관리자 초기화
			buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
			{
				FileSelectButtonEnabled = false,
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
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;

            // 2021.01.29. hma 추가(Start): 조회된 데이터가 문서파일인지 기술자료파일인지 그룹박스 텍스트에 보여주도록 함.
            if (rdoDocFile.Checked == true)
                GridCommGroupBox.Text = rdoDocFile.Text;
            else if (rdoSourceFile.Checked == true)
                GridCommGroupBox.Text = rdoSourceFile.Text;
            // 2021.01.29. hma 추가(End)

            string query = "usp_TDA009 '" + (rdoDocFile.Checked ? "S1" : "S2") + "'"
				+ ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'"
				+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
			if (!string.IsNullOrEmpty(cboDocCtgCd.Text)) query += ", @pDOC_CTG_CD = '" + cboDocCtgCd.SelectedValue.ToString() + "'";
			if (!string.IsNullOrEmpty(cboDocCd.Text)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue.ToString() + "'";
			if (!string.IsNullOrEmpty(dteDocInDtFr.Text)) query += ", @pDOC_IN_DT_FR = '" + dteDocInDtFr.Text + "'";
			if (!string.IsNullOrEmpty(dteDocInDtTo.Text)) query += ", @pDOC_IN_DT_TO = '" + dteDocInDtTo.Text + "'";

			UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            fpSpread1.ActiveSheet.Lock(true);

            // 2021.02.16. hma 추가(Start): 선택 항목은 활성화 처리
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Locked = false;
                }
            }
            // 2021.02.16. hma 추가(End)

            buttonManager.ServerFileDomain = rdoDocFile.Checked ? FileButtonManager.ServerFileType.DocumentFile : FileButtonManager.ServerFileType.SourceFile;
			buttonManager.UpdateButtons(); // 버튼 업데이트
			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 공용함수
		/// <summary>
		/// 메시지를 표시합니다.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="icon"></param>
		void ShowMessage(string msg, MessageBoxIcon icon)
		{
			MessageBox.Show(msg, "삭제파일정리", MessageBoxButtons.OK, icon);
		}

		/// <summary>
		/// 오류 메시지를 표시합니다.
		/// </summary>
		/// <param name="msg">메시지</param>
		void ShowErrorMessage(string msg)
		{
			ShowMessage(msg, MessageBoxIcon.Exclamation);
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 삭제파일 영구제거
		/// </summary>
		private void btnRemoveAllDeletedFiles_Click(object sender, EventArgs e)
		{
            // 2021.02.17. hma 수정(Start): 전체파일삭제 버튼의 Enabled 속성을 false로 하여 클릭이 안되긴 하지만 혹시라도 처리를 못하도록 주석 처리하고 메시지 박스 뜨도록 함. 
            //DeleteDialog dialog = new DeleteDialog();
            //if (dialog.ShowDialog() != DialogResult.OK) return;

            //this.Cursor = Cursors.WaitCursor;

            //try
            //{
            //	// 문서파일 정리
            //	while (dialog.DocumentFileChecked)
            //	{
            //		DataTable filesTable = SystemBase.DbOpen.NoTranDataTable("usp_TDA009 'S3', @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //		if (filesTable.Rows.Count < 1) break;

            //		// 삭제파일 리스트업
            //		List<DeletedFileInfo> files = new List<DeletedFileInfo>();
            //		foreach (DataRow row in filesTable.Rows)
            //			files.Add(new DeletedFileInfo((int)row[0], (string)row[1]));

            //        // 2021.01.29. hma 추가: 테스트 위해 url 강제 지정. IP 뒤에 있는 PORT번호 때문에 FTP 연결시 오류 발생함. 테스트 후 원복 필요!! => 2021-02-17 원복 처리함. 향후 테스트시 주석 풀고 ftp 다음 공백 없애면 됨.
            //        //string strNewDocFtpUrl = "ftp ://112.222.217.252:44001/Archive/Document/";

            //        // 정리
            //        foreach (DeletedFileInfo file in files)
            //		{
            //			// 레코드 삭제
            //			DataTable table = SystemBase.DbOpen.NoTranDataTable("usp_TDA009 'D1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pFILE_ID = " + file.Id);
            //			string resultCode = table.Rows[0][0].ToString();
            //			string resultMessage = table.Rows[0][1].ToString();

            //            // 레코드 삭제 오류
            //            if (resultCode == "ER")
            //            {
            //                ShowErrorMessage(resultMessage);
            //                this.Cursor = Cursors.Default;
            //                return;
            //            }
            //            // 레코드 삭제 성공시 파일 삭제
            //            else if (resultCode == "OK" && table.Rows[0][2].ToString() == "Y")
            //            {
            //                // 2021.01.29. hma 수정(Start):  지맥스 서버로 연결하여 테스트 위해 DocumentUrl 변경함. 테스트 후 원복 필요!! => 2021-02-17 원복 처리함. 향후 테스트시 주석 변경하면 됨.
            //                Ftp.DeleteFile(Url.Combine(Server.DocumentUrl, file.Filename), Server.AccountName, Server.AccountPassword);
            //                //Ftp.DeleteFile(Url.Combine(strNewDocFtpUrl, file.Filename), Server.AccountName, Server.AccountPassword);
            //                // 2021.01.29. hma 수정(End)
            //            }
            //        }

            //		break;
            //	}
            //}
            //catch (Exception ex)
            //{
            //	ShowErrorMessage("문서파일 삭제중 얘기치 못한 오류가 발생했습니다.");
            //	this.Cursor = Cursors.Default;
            //	return;
            //}

            //try
            //{
            //	// 기술자료파일 정리
            //	while (dialog.SourceFileChecked)
            //	{
            //		DataTable filesTable = SystemBase.DbOpen.NoTranDataTable("usp_TDA009 'S4', @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
            //		if (filesTable.Rows.Count < 1) break;

            //		// 삭제파일 리스트업
            //		List<DeletedFileInfo> files = new List<DeletedFileInfo>();
            //		foreach (DataRow row in filesTable.Rows)
            //			files.Add(new DeletedFileInfo((int)row[0], (string)row[1]));

            //		// 정리
            //		foreach (DeletedFileInfo file in files)
            //		{
            //			// 레코드 삭제
            //			DataTable table = SystemBase.DbOpen.NoTranDataTable("usp_TDA009 'D2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pFILE_ID = " + file.Id);
            //			string resultCode = table.Rows[0][0].ToString();
            //			string resultMessage = table.Rows[0][1].ToString();

            //			// 레코드 삭제 오류
            //			if (resultCode == "ER")
            //			{
            //				ShowErrorMessage(resultMessage);
            //				this.Cursor = Cursors.Default;
            //				return;
            //			}
            //			// 레코드 삭제 성공시 파일 삭제
            //			else if (resultCode == "OK" && table.Rows[0][2].ToString() == "Y")
            //				Ftp.DeleteFile(Url.Combine(Server.SourceUrl, file.Filename), Server.AccountName, Server.AccountPassword);
            //		}

            //		break;
            //	}
            //}
            //catch (Exception ex)
            //{
            //	ShowErrorMessage("기술자료파일 삭제중 얘기치 못한 오류가 발생했습니다. " + ex.ToString());     // 2021.02.16. hma 수정: ex.ToString() 추가: 경고 메시지가 나와서..
            //    this.Cursor = Cursors.Default;
            //	return;
            //}

            //this.Cursor = Cursors.Default;
            //ShowMessage("파일제거가 완료되었습니다.", MessageBoxIcon.Information);

            MessageBox.Show("전체 파일 삭제 처리를 사용하실 수 없습니다. 대상 건을 선택하시어 처리하시기 바랍니다.");
            return;
            // 2021.02.17. hma 수정(End)
        }

        /// <summary>
        /// 파일구분에 따른 카테고리 콤보박스 처리
        /// </summary>
        private void rdoDocFile_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoDocFile.Checked)
			{
				cboDocCtgCd.Enabled = true;
				SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드
			}
			else
			{
				cboDocCtgCd.Enabled = false;
				cboDocCtgCd.SelectedIndex = 0;
				SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_DEPT_CD = 'MT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드
			}
		}
        #endregion

        // 2021.01.29. hma 추가(Start): 선택된 파일들에 대한 삭제 처리
        #region btnCheckedFilesRemove_Click(): 선택된파일정리 버튼 클릭시 이벤트 처리. 선택된 파일들을 DB 데이터와 서버의 파일 삭제 처리
        private void btnCheckedFilesRemove_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            // 그리드 상단 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
            {
                string ERRCode = "WR", MSGCode = "B0070"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strFileName = "";

                    string strDelType = "";
                    if (GridCommGroupBox.Text == rdoDocFile.Text)       // 문서파일을 조회한 경우
                    {
                        strDelType = "D1";
                    }
                    else //GridCommGroupBox.Text == rdoSourceFile.Text) // 기술자료파일을 조회한 경우
                    {
                        strDelType = "D2";
                    }

                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                        {
                            string strSql = " usp_TDA009 '" + strDelType + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql += ", @pFILE_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "문서ID")].Text + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

                            // 테스트 위해 문서url 강제 지정. IP 뒤에 있는 PORT번호 때문에 FTP 연결시 오류 발생함. 향후 테스트시 주석 풀고 ftp 다음 공백 없애면 됨.
                            //string strNewDocFtpUrl = "ftp ://112.222.217.252:44001/Archive/Document/";

                            strFileName = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "서버경로및파일명")].Text;
                            //Ftp.DeleteFile(Url.Combine(strNewDocFtpUrl, strFileName), Server.AccountName, Server.AccountPassword);        // 향후 테스트 필요시 주석 풀고 아래 부분 주석 처리하면 됨.
                            Ftp.DeleteFile(Url.Combine(Server.DocumentUrl, strFileName), Server.AccountName, Server.AccountPassword);                            
                        }
                    }
                    Trans.Commit();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

                this.Cursor = Cursors.Default;
            }
        }
        #endregion
        // 2021.01.29. hma 추가(End)
    }
}
