#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정증빙등록
// 작 성 자 : 이재광
// 작 성 일 : 2014-09-16
// 작성내용 : 공정증빙 관련문서(품질문서) 조회/열람/등록/관리
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
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace TD.TDQ003
{
	public partial class TDQ003 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "OUT";

		/// <summary>현재 선택된 마스터 행</summary>
		int selectedMasterRow = -1;

		// 마스터 컬럼
		int colPlantCd = -1;
		int colWorkorderNo = -1;
		int colProcSeq = -1;
		int colSeq = -1;
		int colPoNo = -1;			// 발주번호
		int colPoSeq = -1;			// 발주순번
		int colCust = -1;				// 거래처

		// 디테일 컬럼(문서목록)
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
		/// <summary>품목목록 첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;
		#endregion

		#region 생성자
		public TDQ003()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDQ003_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 설정
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);// 공장

			// 그리드초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pTOP_DOC_CTG_CD = '" + docCtgCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView masterSheet = fpSpread2.ActiveSheet;
			colPlantCd = masterSheet.FindHeaderColumnIndex("공장코드");
			colWorkorderNo = masterSheet.FindHeaderColumnIndex("제조오더번호");
			colProcSeq = masterSheet.FindHeaderColumnIndex("공정번호");
			colSeq = masterSheet.FindHeaderColumnIndex("실적순번");
			colPoNo = masterSheet.FindHeaderColumnIndex("발주번호");
			colPoSeq = masterSheet.FindHeaderColumnIndex("발주순번");
			colCust = masterSheet.FindHeaderColumnIndex("거래처");
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
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

			// 기타 세팅
			docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류 정보
			dteReportDtFr.Value = DateTime.Now.AddMonths(-1);
			dteReportDtTo.Value = DateTime.Now;
            dteInspReqDtFr.Value = DateTime.Now.AddMonths(-1);
            dteInspReqDtTo.Value = DateTime.Now;

            dteRELEASE_FR.Value = null;
            dteRELEASE_TO.Value = null;
		}
		#endregion

		#region 마스터 조회
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDQ003 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";
				if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
				if (!string.IsNullOrEmpty(dteReportDtFr.Text)) query += ", @pREPORT_DT_FR = '" + dteReportDtFr.Text + "'";
				if (!string.IsNullOrEmpty(dteReportDtTo.Text)) query += ", @pREPORT_DT_TO = '" + dteReportDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtPoNo.Text)) query += ", @pPO_NO = '" + txtPoNo.Text + "'";
				if (!string.IsNullOrEmpty(txtWorkorderNo.Text)) query += ", @pWORKORDER_NO = '" + txtWorkorderNo.Text + "'";
				if (!string.IsNullOrEmpty(txtCustCd.Text)) query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
				if (!string.IsNullOrEmpty(txtEntCd.Text)) query += ", @pENT_CD = '" + txtEntCd.Text + "'";
				if (!string.IsNullOrEmpty(txtProjNo.Text)) query += ", @pPROJECT_NO = '" + txtProjNo.Text + "'";
				if (!string.IsNullOrEmpty(txtProjSeq.Text)) query += ", @pPROJECT_SEQ = '" + txtProjSeq.Text + "'";
                if (!string.IsNullOrEmpty(txtInspReqNo.Text)) query += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
                if (!string.IsNullOrEmpty(txtJobCd.Text)) query += ", @pJOB_CD = '" + txtJobCd.Text + "'";
                if (!string.IsNullOrEmpty(txtIN_ID.Text)) query += ", @pIN_CD = '" + txtIN_ID.Text + "'";

                if (!string.IsNullOrEmpty(dteRELEASE_FR.Text)) query += ", @pRELEASE_DT_FR = '" + dteRELEASE_FR.Text + "'";
                if (!string.IsNullOrEmpty(dteRELEASE_TO.Text)) query += ", @pRELEASE_DT_TO = '" + dteRELEASE_TO.Text + "'";
                if (!string.IsNullOrEmpty(txtInspectorCd.Text)) query += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";
                string strOut_YN = "";
                if (rdoOut_Y.Checked == true)
                    strOut_YN = "Y";
                else if (rdoOut_N.Checked == true)
                    strOut_YN = "N";
                query += ", @pOUT_YN = '" + strOut_YN + "'";


				if (rdoInsideFlgY.Checked) query += ", @pINSIDE_FLG = 'Y'";
				else if (rdoInsideFlgN.Checked) query += ", @pINSIDE_FLG = 'N'";
				if (!string.IsNullOrEmpty(dteDocInDtFr.Text)) query += ", @pDOC_IN_DT_FR = '" + dteDocInDtFr.Text + "'";
				if (!string.IsNullOrEmpty(dteDocInDtTo.Text)) query += ", @pDOC_IN_DT_TO = '" + dteDocInDtTo.Text + "'";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);

				// 그리드 내용 보정
				SheetView sheet = fpSpread2.ActiveSheet;
				if (rdoInsideFlgY.Checked) // 자작품인 경우 발주, 거래처 컬럼 숨김
				{
					sheet.Columns[colPoNo].Visible = false;
					sheet.Columns[colPoSeq].Visible = false;
					sheet.Columns[colCust].Visible = false;
				}
				attachmentManager.PlantCode = GetSelectedPlantCd();
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

		#region 첨부문서 조회
		/// <summary>
		/// 마스터 항목 선택시 디테일 조회 호출
		/// </summary>
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			if (e.Range.Row == selectedMasterRow) return;
			selectedMasterRow = sheet.RowCount > 0 ? e.Range.Row : -1;
			SearchDocument();
		}

		/// <summary>
		/// 첨부문서를 조회해 첨부문서 그리드에 뿌립니다.
		/// </summary>
		/// <param name="masterRow">마스터 그리드의 선택행</param>
		private void SearchDocument()
		{
			fpSpread1.ActiveSheet.RowCount = 0;
			if (fpSpread2.ActiveSheet.RowCount < 1) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread2.ActiveSheet;
				string query = "usp_T_DOC 'S1'"
					+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "'"
					+ ", @pATT_KEY = '" + GetSelectedAttKeyCombination() + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				buttonManager.UpdateButtons(); // 버튼 업데이트

				// 내용 변경
				SheetView sheet = fpSpread1.ActiveSheet;
				for (int row = 0; row < sheet.RowCount; row++)
					UpdateDocNoCellBackgroundColor(row); // 문서번호 배경색 업데이트
				((TextCellType)sheet.Columns[colRevNo].CellType).MaxLength = 5; // 개정번호 자릿수 제한
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
			SheetView sheet = fpSpread1.ActiveSheet;
			fpSpread1.Focus();

			// 필수문서 자동 추가 처리
			bool added = false;
			if (selectedMasterRow > -1) // 서브항목 단일선택시
				foreach (string docCd in attachmentManager.DocumentColumns.Keys)
				{
					if (attachmentManager.GetCellText(selectedMasterRow, docCd) == "0")
					{
						// 행추가후 자동입력 처리
						UIForm.FPMake.RowInsert(fpSpread1); // 행추가
						int row = sheet.ActiveRowIndex;
						sheet.Cells[row, colDocCd].Value = docCd;
						sheet.Cells[row, colDocNm].Value = docCd;
						sheet.Cells[row, colRegUsrId].Value = SystemBase.Base.gstrUserID;
						sheet.Cells[row, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
						buttonManager.UpdateButtons(row); // 버튼 업데이트
						UpdateDocNoCellBackgroundColor(row); // 문서번호 필수색 업데이트
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

		#region 저장
		protected override void SaveExec()
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			SheetView subSheet = fpSpread2.ActiveSheet;
			if (sheet.Rows.Count < 1) return;
			RowDataList attKeys = null;
			if (sheet.CheckRowInserted())
			{
				attKeys = subSheet.GetCheckedRowData(new int[] { colWorkorderNo, colProcSeq, colSeq });
				if (attKeys == null)
				{
					MessageBox.Show("먼저 첨부하려는 공정에 체크를 하십시오.", "첨부파일 저장", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

				// 첨부상태가 변경된 첨부대상키 리스트업
				RowDataList updatedKeys = new RowDataList();
				if (sheet.CheckRowDeleted()) updatedKeys.Add(selectedMasterRow, new string[] { GetMasterKey(), GetSecondKey(), GetThirdKey() }); // 문서가 삭제된 첨부대상키 추출
				updatedKeys.Add(sheet.CheckRowInserted() ? subSheet.GetCheckedRowData(new int[] { colWorkorderNo, colProcSeq, colSeq }) : null); // 행추가된 첨부대상키 추출

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
		/// 현재 선택된 마스터, 서브 항목에 대한 첨부키조합을 구합니다.
		/// </summary>
		/// <param name="procRow">공정목록 행 인덱스</param>
		/// <returns></returns>
		string GetSelectedAttKeyCombination()
		{
			return GetMasterKey() + "/" + GetSecondKey() + "/" + GetThirdKey(); // 키 = "제조오더번호/공정번호/실적순번"
		}

		/// <summary>
		/// 현재 선택된 마스터 항목에 대한 공장코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedPlantCd()
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			return sheet.RowCount > 0 ? sheet.Cells[0, colPlantCd].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 첨부키(제조오더번호)를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetMasterKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colWorkorderNo].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 첨부키(공정번호)를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSecondKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colProcSeq].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 두 번째 첨부키(실적순번)를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetThirdKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colSeq].Text : "";
		}

		/// <summary>
		/// 문서번호셀의 필수여부에 따른 배경색을 업데이트합니다.
		/// </summary>
		/// <param name="row"></param>
		void UpdateDocNoCellBackgroundColor(int row)
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			Cell docNoCell = sheet.Cells[row, colDocNo];
			if (docNoReqs[sheet.Cells[row, colDocCd].Text].ToUpper() == "Y")
				docNoCell.BackColor = SystemBase.Validation.Kind_LightCyan;
			else
				docNoCell.BackColor = Color.White;
		}
		#endregion

		#region 디테일 그리드 이벤트 핸들러
		protected override void fpButtonClick(int Row, int Column)
		{
			if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "대체품목_2"))
			{
				try
				{
					string strPlantCd = fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text;

					WNDW005 pu = new WNDW005(strPlantCd, true, fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대체품목")].Text);
					pu.MaximizeBox = false;
					pu.ShowDialog();
					if (pu.DialogResult == DialogResult.OK)
					{
						string[] Msgs = pu.ReturnVal;

						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대체품목")].Text = Msgs[2].ToString();
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대체품목명")].Text = Msgs[3].ToString();
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = Msgs[7].ToString();
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					//데이터 조회 중 오류가 발생하였습니다.
				}
			}
		}

		/// <summary>
		/// 셀값 변경 핸들러
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		protected override void fpSpread1_ChangeEvent(int row, int col)
		{
			try
			{
				// 문서종류
				if (col == colDocNm)
				{
					SheetView sheet = fpSpread1.ActiveSheet;
					sheet.Cells[row, colDocCd].Value = (string)sheet.Cells[row, colDocNm].Value; // 문서코드셀 업데이트
					UpdateDocNoCellBackgroundColor(row); // 문서번호 배경색 업데이트
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

		#region 컨트롤 이벤트핸들러
		/// <summary>
		/// 사업 팝업
		/// </summary>
		private void btnEnt_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP', @pSPEC1='ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtEntCd.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05008", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업코드 조회");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtEntCd.Text = Msgs[0].ToString();
					txtEntNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
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
		/// 거래처 팝업
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
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 품목 팝업
		/// </summary>
		private void btnItem_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtItemCd.Text, txtItemNm.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					txtItemCd.Text = pu.ReturnValue[1].ToString();
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
		/// 공정코드 팝업
		/// </summary>
		private void btnJob_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'COMM_POP', @pSPEC1 = 'P001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "; 	// 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { txtJobCd.Text, "" };		// 쿼리 인자값에 들어갈 데이타

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("PBA122P", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공정 조회", false);
				pu.Width = 400;
				pu.ShowDialog();	//공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);
					txtJobCd.Text = Msgs[0].ToString();
					txtJobNm.Value = Msgs[1].ToString();
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
		/// 발주번호 팝업
		/// </summary>
		private void btnPoNo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW018 pu = new WNDW018();
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;
					txtPoNo.Text = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 차수 팝업
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// 제조오더번호 팝업
		/// </summary>
		private void btnWorkorderNo_Click(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE ='P100', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtWorkorderNo.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00057", strQuery, strWhere, strSearch, new int[] { 0, 1 });
				pu.Width = 800;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtWorkorderNo.Text = Msgs[0].ToString();
					txtWorkorderNo.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f);
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			this.Cursor = Cursors.Default;
		}

		/// <summary>
		/// 주문처 코드 입력
		/// </summary>
		private void txtBpCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtCustNm.Value = !string.IsNullOrEmpty(txtCustCd.Text) ? SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'") : "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "주문처 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 사업 코드 입력
		/// </summary>
		private void txtEntCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtEntNm.Value = !string.IsNullOrEmpty(txtEntCd.Text) ? SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'") : "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
        private void txtIN_ID_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtIN_ID.Text != "")
                {
                    txtIN_IDNM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtIN_ID.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtIN_IDNM.Value = "";
                }
            }
            catch
            {

            }
        }
        private void txtInspectorCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtInspectorCd.Text != "")
                {
                    txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'CO006' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
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
		/// 품목코드 입력
		/// </summary>
		private void txtItemCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtItemNm.Value = !string.IsNullOrEmpty(txtItemCd.Text) ? SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'") : "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 공정코드 입력
		/// </summary>
		private void txtJobCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtJobNm.Value = !string.IsNullOrEmpty(txtJobCd.Text) ? SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJobCd.Text, " AND MAJOR_CD = 'P001' AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "'") : "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 프로젝트 코드 입력
		/// </summary>
		private void txtProjNo_TextChanged(object sender, EventArgs e)
		{
			try
			{
				txtProjNm.Value = !string.IsNullOrEmpty(txtProjNo.Text) ? SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'") : "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

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

        private void btnIN_ID_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP2'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtIN_ID.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "등록자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtIN_ID.Text = Msgs[0].ToString();
                    txtIN_IDNM.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
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

	}

}