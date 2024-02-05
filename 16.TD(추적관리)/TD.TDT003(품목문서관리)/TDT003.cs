#region 작성정보
/*********************************************************************/
// 단위업무명 : 품목문서관리
// 작 성 자 : 이재광
// 작 성 일 : 2014-8-15
// 작성내용 : 품목별 첨부문서(생산기술문서) 조회/등록/관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using WNDW;

namespace TD.TDT003
{
	public partial class TDT003 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "ITM";

		// 마스터 컬럼
		int colPlantCd = -1;
		int colItemCd = -1;

		// 디테일 컬럼
		int colDocId = -1;
		int colSrcfId = -1;
		int colSvrPath = -1;
		int colSvrFnm = -1;
		int colOrgFnm = -1;
		int colFileExt = -1;
		int colDocCd = -1;
		int colDocNm = -1;
		int colDocNo = -1;
		int colRevNo = -1;
		int colRemark = -1;
		int colSrcfState = -1;
		int colRegUsrId = -1;
		int colRegUsrNm = -1;

		/// <summary>현재 선택된 마스터 행</summary>
		int selectedMasterRow = -1;

		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>품목목록 첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;

		/// <summary>자료파일목록 팝업의 자료파일상태 값 기억</summary>
		string sourceFileStateValue = "A";
		#endregion

		#region 생성자
		public TDT003()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDT003_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);// 공장
			SystemBase.ComboMake.C1Combo(cboSItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);	// 품목계정
          
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
			colPlantCd = masterSheet.FindHeaderColumnIndex("공장코드");
			colItemCd = masterSheet.FindHeaderColumnIndex("품목코드");
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colSrcfId = sheet.FindHeaderColumnIndex("자료파일ID");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 2; // 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
			colFileExt = sheet.FindHeaderColumnIndex("파일확장자");
			colDocCd = sheet.FindHeaderColumnIndex("문서코드");
			colDocNm = sheet.FindHeaderColumnIndex("문서종류");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호");
			colRevNo = sheet.FindHeaderColumnIndex("개정번호");
			colRemark = sheet.FindHeaderColumnIndex("비고");
			colSrcfState = sheet.FindHeaderColumnIndex("자료상태");
			colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
			colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

			// 첨부파일목록 파일버튼 관리자 초기화
			buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.SourceFile)
			{
				FilenameColumnIndex = colOrgFnm,
				ServerPathColumnIndex = colSvrPath,
				ServerFilenameColumnIndex = colSvrFnm,
				FileViewButtonColumnIndex = colOrgFnm - 2,
				FileDownloadButtonColumnIndex = colOrgFnm - 1,
				DocTypeNameColumnIndex = colDocNm,
				DocRevisionColumnIndex = colRevNo,
				DocNumberColumnIndex = colDocNo,
			};

			// 품목목록 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, docCtgCd, "첨부문서코드");

			// 기타 세팅
			cboSPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			rdoAttachYes.Checked = true;
		}
		#endregion

		#region 마스터 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
                string search = "S1";
                if (chkSTEP.Checked == true)
                    search = "S2";

                string query = "usp_TDT003 " + "'" + search + "'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboSPlantCd.SelectedValue.ToString() + "'";
				if (!string.IsNullOrEmpty(txtSItemCd.Text)) query += ", @pITEM_CD = '" + txtSItemCd.Text + "' ";
				if (!string.IsNullOrEmpty(txtSItemSpec.Text)) query += ", @pITEM_SPEC = '" + txtSItemSpec.Text + "'";
				if (!string.IsNullOrEmpty(txtSDrawNo.Text)) query += ", @pDRAW_NO = '" + txtSDrawNo.Text + "'";
				if (!string.IsNullOrEmpty(cboSItemAcct.SelectedText)) query += ", @pITEM_ACCT = '" + cboSItemAcct.SelectedValue.ToString() + "'";
                if (!string.IsNullOrEmpty(txtSPEC_NO.Text)) query += ", @pSPEC_NO = '" + txtSPEC_NO.Text + "'";

				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);
                attachmentManager.PlantCode = (string)cboSPlantCd.SelectedValue;
				attachmentManager.AppendColumns(); 	// 스프레드에 컬럼을 추가하고 문서첨부표시
				selectedMasterRow = -1;
				fpSpread1.ActiveSheet.RowCount = 0;
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

		#region 디테일 조회(첨부문서)
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (selectedMasterRow == e.Range.Row) return;
			selectedMasterRow = e.Range.Row;
			SearchDocument();
		}

		/// <summary>
		/// 첨부문서를 리스트업합니다.
		/// </summary>
		private void SearchDocument()
		{
			if (fpSpread2.ActiveSheet.RowCount < 1)
			{
				fpSpread1.ActiveSheet.RowCount = 0;
				return;
			}

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread2.ActiveSheet;
				string query = "usp_TDT004 'S2'"
					+ ", @pDOC_CTG_CD = 'ITM'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + masterSheet.FindCell(0, "공장코드").Text + "'"
					+ ", @pATT_KEY = '" + masterSheet.FindCell(selectedMasterRow, "품목코드").Text + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

				// 자료파일 그리드 셀 조정
				SheetView sheet = fpSpread1.ActiveSheet;
				for (int row = 0; row < sheet.Rows.Count; row++)
                {
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출처")].Locked = true;
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출처")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인처")].Locked = true;
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "승인처")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "작성처")].Locked = true;
                    fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "작성처")].BackColor = System.Drawing.Color.FromArgb(239, 239, 239);
					// 폐기된 자료행 강조
					if (Convert.ToString(sheet.Cells[row, colSrcfState].Value) == "폐기")
					{
						Row oRow = sheet.Rows[row];
						oRow.SetApprearance(CellAppearance.Discard);
					}

					// 버튼 업데이트
					buttonManager.UpdateButtons(row);
				}
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

		#region 행추가
		protected override void RowInsExec()
		{
			if (selectedMasterRow < 0)
			{
				MessageBox.Show("먼저 품목을 선택해야합니다.", "행 추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
                WNDW034 dialog = new WNDW034(cboSPlantCd.SelectedValue.ToString(), "ITM", txtSPEC_NO.Text)
				{
					SourceFileState = sourceFileStateValue,
				};
				if (dialog.ShowDialog() != DialogResult.OK) return;
				sourceFileStateValue = dialog.SourceFileState;

				// 선택된 자료파일을 첨부파일로 추가
				SheetView sheet = fpSpread1.ActiveSheet;
				sheet.ClearSelection();
				sheet.AddSelection(sheet.RowCount - 1, 0, 1, sheet.ColumnCount); // 제일뒤에 추가하기 위해 마지막행 선택
				bool exists = false;
				foreach (WNDW034.SourceFileItem item in dialog.SelectedItems)
				{
					if (FindDetailRow(item.SrcfId) > -1)
					{
						exists = true;
						continue;
					}

					// 행 추가 및 내용 입력
					UIForm.FPMake.RowInsert(fpSpread1);
					int row = sheet.Rows.Count - 1;
					sheet.Cells[row, colSrcfId].Value = item.SrcfId;
					sheet.Cells[row, colSvrPath].Value = item.SvrPath;
					sheet.Cells[row, colSvrFnm].Value = item.SvrFnm;
					sheet.Cells[row, colOrgFnm].Value = item.OrgFnm;
					string fileExt = System.IO.Path.GetExtension(item.OrgFnm);
					if (!string.IsNullOrEmpty(fileExt)) fileExt = fileExt.Substring(1).ToUpper(); // 점 때고 대문자로 변환
					sheet.Cells[row, colFileExt].Value = fileExt;
					sheet.Cells[row, colDocCd].Value = item.DocCd;
					sheet.Cells[row, colDocNm].Value = item.DocNm;
					sheet.Cells[row, colRevNo].Value = item.RevNo;
					sheet.Cells[row, colDocNo].Value = item.DocNo;
					sheet.Cells[row, colRemark].Value = item.Remark;
					sheet.Cells[row, colSrcfState].Value = item.SrcfState;
					sheet.Cells[row, colRegUsrId].Value = SystemBase.Base.gstrUserID;
					sheet.Cells[row, colRegUsrNm].Value = SystemBase.Base.gstrUserName;

					// 폐기된 자료행 강조
					if (Convert.ToString(sheet.Cells[row, colSrcfState].Value) == "폐기")
					{
						Row oRow = sheet.Rows[row];
						oRow.SetApprearance(CellAppearance.Discard);
					}

					buttonManager.UpdateButtons(row); // 버튼 업데이트
				}
				if (exists) MessageBox.Show("이미 첨부된 항목은 첨부할 수 없습니다.", "기술자료파일 첨부", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 첨부문서목록에서 자료파일ID를 찾아 행 인덱스를 반환합니다.
		/// </summary>
		/// <param name="srcfId">자료파일ID</param>
		/// <returns></returns>
		int FindDetailRow(int srcfId)
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			string sid = srcfId.ToString();
			for (int row = 0; row < sheet.RowCount; row++)
				if (sheet.Cells[row, colSrcfId].Text == sid)
					return row;

			return -1;
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
			if (fpSpread1.Sheets[0].Rows.Count < 1) return;

			//그리드 상단 필수 체크
			if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

			this.Cursor = Cursors.WaitCursor;
			fpSpread1.Focus();

			string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				//행수만큼 처리
				SheetView sheet = fpSpread1.ActiveSheet;
				SheetView masterSheet = fpSpread2.ActiveSheet;
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

					string query = "usp_T_DOC '" + strGbn + "'";
					if (strHead == "I")
					{
						query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
							+ ", @pPLANT_CD = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "공장코드").Text + "' "
							+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
							+ ", @pATT_KEY = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "품목코드").Text + "'"
							+ ", @pATT_KEY1 = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "품목코드").Text + "'"
							+ ", @pSRCF_ID = " + sheet.Cells[row, colSrcfId].Text
							+ ", @pSVR_PATH = '" + sheet.Cells[row, colSvrPath].Text + "'"
							+ ", @pSVR_FNM = '" + sheet.Cells[row, colSvrFnm].Text + "'"
							+ ", @pORG_FNM = '" + sheet.Cells[row, colOrgFnm].Text + "'"
							+ ", @pFILE_EXT = '" + sheet.Cells[row, colFileExt].Text + "'"
							+ ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text + "'"
							+ ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
							+ ", @pREV_NO = '" + sheet.Cells[row, colRevNo].Text + "'"
							+ ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
							+ ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
					}
					else if (strHead == "D")
					{
						query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Value;
					}

					DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
					resultCode = ds.Tables[0].Rows[0][0].ToString();
					resultMessage = ds.Tables[0].Rows[0][1].ToString();

					if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
				}
				Trans.Commit();

				// 품목의 첨부문서 코드문자열 업데이트
				attachmentManager.ReloadData(masterSheet.ActiveRowIndex, new string[] { masterSheet.Cells[masterSheet.ActiveRowIndex, colItemCd].Text });
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
				SearchDocument();
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

		#region 공유기능
		/// <summary>
		/// 현재 선택된 마스터 항목에 대한 공장코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedPlantCd()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colPlantCd].Text : "";
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		private void btnSItemCd_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtSItemCd.Text, txtSItemSpec.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					txtSItemCd.Text = pu.ReturnValue[1].ToString();
					cboSItemAcct.SelectedIndex = 0;
					rdoAttachBoth.Checked = true;
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

	}

}