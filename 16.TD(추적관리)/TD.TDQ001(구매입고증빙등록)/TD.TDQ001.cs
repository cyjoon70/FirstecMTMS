#region 작성정보
/*********************************************************************/
// 단위업무명 : 불량유형등록(최종)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-03
// 작성내용 : 불량유형등록(최종) 및 관리
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
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace TD.TDQ001
{
	public partial class TDQ001 : UIForm.FPCOMM3
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "PUR";

		/// <summary>마스터 그리드의 현재 선택된 행</summary>
		int selectedMasterRow = -1;
		/// <summary>서브 그리드의 현재 선택된 행</summary>
		int selectedSubRow = -1;

		// 마스터 그리드 컬럼(입고 목록)
		int colPlantCd = -1;
		int colMvmtNo = -1;

		// 서브 그리드 컬럼(입고품목 목록)
		int colSubMvmtNo = -1;
		int colSubMvmtSeq = -1;
		int colSubBarCode = -1;
		int colSubItemType = -1;

		// 디테일 그리드 컬럼(문서 목록)
		int colSlMvmtYn = -1;
		int colDocId = -1;
		int colDocMvmtSeq = -1;
		int colDocBarCode = -1;
		int colDocItemCd = -1;
		int colDocItemNm = -1;
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

		#region 생성자
		public TDQ001()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDQ001_Load(object sender, System.EventArgs e)
		{
			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 공장
            SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드

			//그리드초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pTOP_DOC_CTG_CD = '" + docCtgCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			colPlantCd = fpSpread3.ActiveSheet.FindHeaderColumnIndex("공장코드");
			colMvmtNo = fpSpread3.ActiveSheet.FindHeaderColumnIndex("입고번호");
			SheetView subSheet = fpSpread2.ActiveSheet;
			colSubMvmtNo = subSheet.FindHeaderColumnIndex("입고번호");
			colSubMvmtSeq = subSheet.FindHeaderColumnIndex("입고순번");
			colSubBarCode = subSheet.FindHeaderColumnIndex("바코드");
			colSubItemType = subSheet.FindHeaderColumnIndex("품목구분");
			SheetView sheet = fpSpread1.ActiveSheet;
			colSlMvmtYn = sheet.FindHeaderColumnIndex("창고입고여부");
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colDocMvmtSeq = sheet.FindHeaderColumnIndex("입고순번");
			colDocBarCode = sheet.FindHeaderColumnIndex("바코드");
			colDocItemCd = sheet.FindHeaderColumnIndex("품목코드");
			colDocItemNm = sheet.FindHeaderColumnIndex("품목명");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
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

			// 품목목록 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, null, "첨부문서코드", "필수문서코드");

			//기타 세팅
			docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류 정보
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			dteInspReqDtFr.Value = DateTime.Now.AddMonths(-1);
			dteInspReqDtTo.Value = DateTime.Now;
            dteRELEASE_FR.Value  = null;
            dteRELEASE_TO.Value = null;
		}
		#endregion

		#region 마스터 그리드 조회
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDQ001 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "'";
				if (!string.IsNullOrEmpty(txtMvmtNo.Text)) query += ", @pMVMT_NO = '" + txtMvmtNo.Text + "'";
				if (!string.IsNullOrEmpty(dteMvmtDtFrom.Text)) query += ", @pMVMT_DT_FR = '" + dteMvmtDtFrom.Text + "'";
				if (!string.IsNullOrEmpty(dteMvmtDtTo.Text)) query += ", @pMVMT_DT_TO = '" + dteMvmtDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtIOType.Text)) query += ", @pIO_TYPE = '" + txtIOType.Text + "'";
				if (!string.IsNullOrEmpty(txtCustCd.Text)) query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
				if (!string.IsNullOrEmpty(txtProjNo.Text)) query += ", @pPROJ_NO = '" + txtProjNo.Text + "'";
				if (!string.IsNullOrEmpty(txtProjSeq.Text)) query += ", @pPROJ_SEQ = '" + txtProjSeq.Text + "'";
				if (!string.IsNullOrEmpty(txtInspReqNo.Text)) query += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
				if (!string.IsNullOrEmpty(dteInspReqDtFr.Text)) query += ", @pINSP_REQ_DT_FR = '" + dteInspReqDtFr.Text + "'";
                if (!string.IsNullOrEmpty(dteInspReqDtTo.Text)) query += ", @pINSP_REQ_DT_TO = '" + dteInspReqDtTo.Text + "'";
                if (!string.IsNullOrEmpty(dteDocInDtFr.Text)) query += ", @pDOC_IN_DT_FR = '" + dteDocInDtFr.Text + "'";
                if (!string.IsNullOrEmpty(dteDocInDtTo.Text)) query += ", @pDOC_IN_DT_TO = '" + dteDocInDtTo.Text + "'";
                if (!string.IsNullOrEmpty(dteRELEASE_FR.Text)) query += ", @pRELEASE_DT_FR = '" + dteRELEASE_FR.Text + "'";
                if (!string.IsNullOrEmpty(dteRELEASE_TO.Text)) query += ", @pRELEASE_DT_TO = '" + dteRELEASE_TO.Text + "'";
				if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
                if (!string.IsNullOrEmpty(cboDocCd.Text)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue + "'";

                if (!string.IsNullOrEmpty(txtIN_ID.Text)) query += ", @pIN_CD = '" + txtIN_ID.Text + "'";
                if (!string.IsNullOrEmpty(txtInspectorCd.Text)) query += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";
                string strSL_YN = "";
                if (rdoSL_Y.Checked == true)
                    strSL_YN = "Y";
                else if (rdoSL_N.Checked == true)
                    strSL_YN = "N";
                query += ", @pSL_YN = '" + strSL_YN + "'";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread3, query, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 1, true);
				selectedMasterRow = -1;
				selectedSubRow = -1;
				fpSpread2.ActiveSheet.RowCount = 0;
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

		#region 서브 그리드 조회
		/// <summary>
		/// 마스터 그리드 항목 선택시 서브 조회 호출
		/// </summary>
		private void fpSpread3_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread3.ActiveSheet;
			//if (sheet.ActiveRowIndex == selectedMasterRow) return;
			selectedMasterRow = sheet.RowCount > 0 ? sheet.ActiveRowIndex : -1;
			SearchSub();
		}

		private void SearchSub()
		{
			selectedSubRow = -1;
			fpSpread1.ActiveSheet.RowCount = 0;
			fpSpread2.ActiveSheet.RowCount = 0;
			if (selectedMasterRow < 0) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread3.ActiveSheet;
				string query = "usp_TDQ001 'S2'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "'"
					+ ", @pMVMT_NO = '" + masterSheet.Cells[selectedMasterRow, colMvmtNo].Text + "'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
				fpSpread2.ActiveSheet.Lock(true, true); // 편집 잠금
				attachmentManager.PlantCode = GetSelectedPlantCd();
				attachmentManager.AppendColumns(); 	// 스프레드에 컬럼을 추가하고 문서첨부표시

				SearchDocument();
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

		#region 디테일 그리드 조회
		/// <summary>
		/// 서브 그리드 항목 선택시 디테일 조회 호출
		/// </summary>
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			if (sheet.ActiveRowIndex == selectedSubRow) return;
			selectedSubRow = sheet.RowCount > 0 ? sheet.ActiveRowIndex : -1;
			SearchDocument();
		}
		private void SearchDocument()
		{
			fpSpread1.ActiveSheet.RowCount = 0;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_T_DOC 'S1'"
					+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "'"
                    + ", @pIN_ID = '" + txtIN_ID.Text + "'";

				query += ", @pATT_KEY1 = '" + fpSpread3.ActiveSheet.Cells[selectedMasterRow, colMvmtNo].Text + "'";
				if (selectedSubRow > -1)
				{
					query += ", @pATT_KEY2 = '" + GetSecondKey() + "'";
					string barcode = GetThirdKey();
					if (!string.IsNullOrEmpty(barcode)) query += ", @pATT_KEY3 = '" + barcode + "'";
				}

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				buttonManager.UpdateButtons(); // 버튼 업데이트=

				// 서브항목 선택인 경우 입고순번, 바코드 컬럼 숨김
				SheetView sheet = fpSpread1.ActiveSheet;
				if (selectedSubRow > -1)
				{
					sheet.Columns[colDocMvmtSeq].Visible = false;
					sheet.Columns[colDocBarCode].Visible = false;
					sheet.Columns[colDocItemCd].Visible = false;
					sheet.Columns[colDocItemNm].Visible = false;
				}

				// 그리드 내용 수정
				for (int row = 0; row < sheet.RowCount; row++)
				{
					if (!CheckEditable(row)) // 창고입고되면 문서 삭제/편집 금지
					{
						sheet.Rows[row].BackColor = EDocument.UIColors.ReadonlyBackground;
						sheet.Rows[row].Locked = true;
					}
					else UpdateDocNoCellBackgroundColor(row); // 문서번호 배경색 업데이트
				}

				// 개정번호 자릿수 제한
				((FarPoint.Win.Spread.CellType.TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5;
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
				MessageBox.Show("먼저 입고를 선택해야합니다.", "행 추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			SheetView sheet = fpSpread1.ActiveSheet;
			fpSpread1.Focus();

			// 필수문서 자동 추가 처리
			bool added = false;
			if (selectedSubRow > -1) // 서브항목 단일선택시
				foreach (string docCd in attachmentManager.DocumentColumns.Keys)
				{
					if (attachmentManager.GetCellText(selectedSubRow, docCd) == "0")
					{
						// 행추가후 자동입력 처리
						UIForm.FPMake.RowInsert(fpSpread1); // 행추가
						int row = sheet.ActiveRowIndex;
						sheet.Cells[row, colDocCd].Value = docCd;
						sheet.Cells[row, colDocNm].Value = docCd;
						sheet.Cells[row, colRegUsrId].Value = SystemBase.Base.gstrUserID;
						sheet.Cells[row, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
						buttonManager.UpdateButtons(row); // 버튼 업데이트
						UpdateDocNoCell(row); // 문서번호셀 업데이트
						added = true;
					}
				}

			// 그냥 행추가 처리
			if (!added)
			{
				// 행추가후 자동입력 처리
				UIForm.FPMake.RowInsert(fpSpread1); // 행추가
				int newRow = sheet.ActiveRowIndex;
				sheet.Cells[newRow, colRegUsrId].Value = SystemBase.Base.gstrUserID;
				sheet.Cells[newRow, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
				buttonManager.UpdateButtons(newRow); // 버튼 업데이트
			}
		}
		#endregion

		#region 행삭제
		protected override void DelExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			if (sheet.RowCount < 1) return;
			CellRange[] ranges = sheet.GetSelections();
			if (ranges.Length == 0) return;

			// 선택된 행에 대해 루프
			foreach (CellRange range in ranges)
				for (int row = range.Row; row < range.Row + range.RowCount; row++)
					if (!CheckEditable(row, true)) return;

			base.DelExec();
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			SheetView subSheet = fpSpread2.ActiveSheet;
			if (sheet.Rows.Count < 1) return;
			RowDataList attKeys = null;
			if (sheet.CheckRowInserted())
			{
				attKeys = subSheet.GetCheckedRowData(new int[] { colSubMvmtNo, colSubMvmtSeq, colSubBarCode });
				if (attKeys == null)
				{
					MessageBox.Show("먼저 첨부하려는 입고품목에 체크를 하십시오.", "첨부파일 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
			}

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
				SheetView masterSheet = fpSpread3.ActiveSheet;
				List<int> newDocIds = new List<int>(); // 새로 추가된 문서레코드의 ID

				// 모든 체크건에 첨부 저장
				int attIndex = 0;
				do // while AttIndex
				{
					RowData key = attKeys != null ? attKeys[attIndex] : new RowData();
					int insertIndex = 0;

					//행수만큼 처리
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

						string query = "usp_T_DOC '" + strGbn + "'"
							+ ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
						switch (strHead)
						{
							case "D":
								query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Value;
								break;

							case "U":
								query += ", @pDOC_ID = " + sheet.Cells[row, colDocId].Value
									+ ", @pDOC_CD = '" + sheet.Cells[row, colDocNm].Value + "'"
									+ ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
									+ ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'";
								break;

							case "I":
								// 인서트인 경우 체크된 대상 모두에 첨부
								if (attIndex == 0)
								{
									// 새 파일 첨부 쿼리
									query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
										+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
										+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "' "
										+ ", @pDOC_CD = '" + sheet.Cells[row, colDocNm].Value + "'"
										+ ", @pDOC_NO = '" + sheet.Cells[row, colDocNo].Text + "'"
										+ ", @pREMARK = '" + sheet.Cells[row, colRemark].Text + "'"
										+ ", @pATT_KEY = '" + key.KeyCombination + "'"
										+ ", @pATT_KEY1 = '" + key.Values[0] + "'"
										+ ", @pATT_KEY2 = '" + key.Values[1] + "'";
								}
								else
								{
									// 기존 파일 첨부 쿼리
									query += ", @pDOC_CTG_CD = '" + docCtgCd + "'"
										+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
										+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "' "
										+ ", @pDOC_PID = " + newDocIds[insertIndex]
										+ ", @pATT_KEY = '" + key.KeyCombination + "'"
										+ ", @pATT_KEY1 = '" + key.Values[0] + "'"
										+ ", @pATT_KEY2 = '" + key.Values[1] + "'";
								}
								// 공통파라메터
								if (!string.IsNullOrEmpty(key.Values[2])) query += ", @pATT_KEY3 = '" + key.Values[2] + "'";
								break;
						}

						// 문서정보 저장
						DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
						resultCode = ds.Tables[0].Rows[0][0].ToString();
						resultMessage = ds.Tables[0].Rows[0][1].ToString();
						if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

						// 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
						if (strHead == "I" && attIndex == 0)
						{
							// 최초로 생성된 첨부파일인 경우만 파일정보 저장
							int newDocId = Convert.ToInt32(ds.Tables[0].Rows[0][2]); // 새로 추가된 문서레코드 ID
							newDocIds.Add(newDocId); // 새 문서레코드 ID 저장
							if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, newDocId, Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
							{ Trans.Rollback(); goto Exit; }; // 실패시 롤백

							insertIndex++;
						}
					} // for row

					attIndex++;
				} while (attKeys != null && attIndex < attKeys.Count);
				Trans.Commit();

				// 첨부상태가 변경된 입고품목의 첨부대상키 리스트업
				RowDataList updatedKeys = new RowDataList();
				RowDataList deletedKeys = sheet.GetDeletedRowData(new int[] { colDocMvmtSeq, colDocMvmtSeq, colDocBarCode }); // 문서가 삭제된 첨부대상키 추출
				if (deletedKeys != null)
				{
					// 삭제된 문서행으로부터 첨부대상키 추출
					string mvmtNo = GetMasterKey();
					foreach (RowData rdata in deletedKeys)
					{
						int row = subSheet.FindText(new int[] { colSubMvmtSeq, colSubBarCode }, new string[] { rdata.Values[1], rdata.Values[2] });
						if (row > -1)
						{
							rdata.Values[0] = mvmtNo; // 문서행에는 입고번호가 없으므로 선택된 입고번호를 넣어줌
							rdata.Row = row;
							updatedKeys.Add(rdata);
						}
					}
				}
				updatedKeys.Add(sheet.CheckRowInserted() ? subSheet.GetCheckedRowData(new int[] { colSubMvmtNo, colSubMvmtSeq, colSubBarCode }) : null); // 행추가된 첨부대상키 추출

				// 첨부정보 다시 로드
				if (updatedKeys.Count > 0)
					foreach (RowData key in updatedKeys)
						attachmentManager.ReloadData(key.Row, key.Values);
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
		/// 문서번호가 필수로 요구되는지 확인합니다.
		/// </summary>
		/// <param name="row"></param>
		/// <returns>문서번호 필수여부</returns>
		bool CheckDocNoRequired(int row)
		{
			return docNoReqs[fpSpread1.ActiveSheet.Cells[row, colDocCd].Text].ToUpper() == "Y";
		}

		/// <summary>
		/// 지정한 행이 편집 가능한지 확인합니다.
		/// </summary>
		/// <param name="row">확인할 행 인덱스</param>
		/// <param name="showAlert">편집 불가인 경우 경고 메시지를 표시할지 여부</param>
		/// <returns></returns>
		bool CheckEditable(int row, bool showAlert)
		{
			if (fpSpread1.ActiveSheet.Cells[row, colSlMvmtYn].Text == "Y") // 창고 입고 여부
			{
				//if (showAlert) MessageBox.Show("창고입고가 완료되어 선택한 첨부문서를 삭제할 수 없습니다.", "행삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);
				//return false;
                return true;
			}
			/* // 등록자만 편집 허가용
			else if (fpSpread1.ActiveSheet.Cells[row, colRegUsrId].Text != SystemBase.Base.gstrUserID) // 등록자 여부
			{
				if (showAlert) MessageBox.Show("등록자가 아니므로 해당 항목을 삭제할 수 없습니다.", "행삭제", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
			*/
			return true;
		}

		/// <summary>
		/// 지정한 행이 편집 가능한지 확인합니다.
		/// </summary>
		/// <param name="row">확인할 행 인덱스</param>
		/// <returns></returns>
		bool CheckEditable(int row)
		{
			return CheckEditable(row, false);
		}

		/// <summary>
		/// 현재 선택된 마스터, 서브 항목에 대한 첨부키조합을 구합니다.
		/// </summary>
		/// <param name="procRow">공정목록 행 인덱스</param>
		/// <returns></returns>
		string GetSelectedAttKeyCombination()
		{
			return GetMasterKey() + "/" + GetSecondKey() + "/" + GetThirdKey(); // 키 = "입고번호/입고순번/바코드"
		}

		/// <summary>
		/// 현재 선택된 마스터 항목에 대한 공장코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedPlantCd()
		{
			return selectedMasterRow > -1 ? fpSpread3.ActiveSheet.Cells[selectedMasterRow, colPlantCd].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetMasterKey()
		{
			return selectedMasterRow > -1 ? fpSpread3.ActiveSheet.Cells[selectedMasterRow, colMvmtNo].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSecondKey()
		{
			return selectedSubRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedSubRow, colSubMvmtSeq].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 두 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetThirdKey()
		{
			return selectedSubRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedSubRow, colSubBarCode].Text : "";
		}

		/// <summary>
		/// 문서번호 필수여부에 따라 문서번호셀의 내용을 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <returns>필수여부</returns>
		bool UpdateDocNoCell(int row)
		{
			if (UpdateDocNoCellBackgroundColor(row) && selectedSubRow > -1) // 문서번호 배경색 업데이트
			{
				SheetView subSheet = fpSpread2.ActiveSheet;
				string itemType = subSheet.Cells[selectedSubRow, colSubItemType].Text;
				if ((itemType == "VB" || itemType == "PB" || itemType == "ER"))
					fpSpread1.ActiveSheet.Cells[row, colDocNo].Text = subSheet.Cells[selectedSubRow, colSubMvmtNo].Text;
				return true;
			}
			else fpSpread1.ActiveSheet.Cells[row, colDocNo].Text = "";
			return false;
		}

		/// <summary>
		/// 문서번호 필수여부에 따라 문서번호셀의 배경색을 업데이트합니다.
		/// </summary>
		/// <param name="row">행 인덱스</param>
		/// <returns>필수여부</returns>
		bool UpdateDocNoCellBackgroundColor(int row)
		{
			Cell docNoCell = fpSpread1.ActiveSheet.Cells[row, colDocNo];
			if (CheckDocNoRequired(row))
			{
				docNoCell.BackColor = SystemBase.Validation.Kind_LightCyan;
				return true;
			}
			else
			{
				docNoCell.BackColor = Color.White;
				return false;
			}
		}
		#endregion

		#region 그리드 이벤트 핸들러
		/// <summary>
		/// 디테일 그리드 셀값 변경 핸들러(첨부문서 목록)
		/// </summary>
		protected override void fpSpread1_ChangeEvent(int row, int col)
		{
			try
			{
				// 문서종류
				if (col == colDocNm)
				{
					SheetView sheet = fpSpread1.ActiveSheet;
					sheet.Cells[row, colDocCd].Value = (string)sheet.Cells[row, colDocNm].Value; // 문서코드셀 업데이트
					UpdateDocNoCell(row); // 문서번호셀 업데이트
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 공급처 팝업
		/// </summary>
		private void btnCust_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW002 pu = new WNDW002(txtCustCd.Text, "P");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtCustCd.Text = Msgs[1].ToString();
					txtCustNm.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 검사의뢰번호 팝업
		/// </summary>
		private void btnInspReqNo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW009 pu = new WNDW009(Convert.ToString(cboPlantCd.SelectedValue)
														, txtInspReqNo.Text
														, ""
														, ""
														, dteInspReqDtFr.Text
														, dteInspReqDtTo.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtInspReqNo.Text = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "검사의뢰번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 입고형태 팝업
		/// </summary>
		private void btnIOType_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_M_COMMON 'M020' , @pSPEC1 = '' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtIOType.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P01001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "입고형태 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtIOType.Text = Msgs[0].ToString();
					txtIOTypeNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 품목코드 조회
		/// </summary>
		private void btnItemCd_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					txtItemCd.Value = pu.ReturnValue[1].ToString();
					txtItemNm.Value = pu.ReturnValue[2].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 입고번호 팝업
		/// </summary>
		private void btnMvmtNo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW019 dialog = new WNDW019();
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					string[] Msgs = dialog.ReturnVal;
					txtMvmtNo.Text = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 프로젝트 팝업
		/// </summary>
		private void btnProj_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW007 pu = new WNDW007(txtProjNo.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtProjNo.Text = Msgs[3].ToString();
					txtProjNm.Value = Msgs[4].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 프로젝트차수 팝업
		/// </summary>
		private void btnProjSeq_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtProjNm.Text))
			{
				MessageBox.Show("먼저 프로젝트를 선택해야 합니다.", "프로젝트 차수 지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
				string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
				pu.Width = 400;
				pu.ShowDialog();	//공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);
					txtProjSeq.Text = Msgs[0].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 공급처 코드 입력 처리
		/// </summary>
		private void txtCustCd_TextChanged(object sender, EventArgs e)
		{
			txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		/// <summary>
		/// 입고형태 코드 입력 처리
		/// </summary>
		private void txtIOType_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (txtIOType.Text != "")
				{
					txtIOTypeNm.Value = SystemBase.Base.CodeName("IO_TYPE", "IO_TYPE_NM", "M_MVMT_TYPE", txtIOType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
				}
				else
				{
					txtIOTypeNm.Value = "";
				}
			}
			catch { }
		}

                //검사원
        private void txtInspectorCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtInspectorCd.Text != "")
                {
                    txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtInspectorNm.Value = "";
                }
            }
            catch
            {

            }
        }
		/// <summary>
		/// 프로젝트 코드 입력 처리
		/// </summary>
		private void txtProjNo_TextChanged(object sender, EventArgs e)
		{
			txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		/// <summary>
		/// 품목코드 조회 변경시
		/// </summary>
		private void txtItemCd_TextChanged(object sender, EventArgs e)
		{
			if (txtItemCd.Text != "") txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
			else txtItemNm.Value = "";
		}

        private void btnInspectorCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspectorCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspectorCd.Text = Msgs[0].ToString();
                    txtInspectorNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnIN_ID_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP2'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIN_ID.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIN_ID.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

	}
}
