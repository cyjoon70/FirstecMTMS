#region 작성정보
/*********************************************************************/
// 단위업무명 : 공정문서조회
// 작 성 자 : 이재광
// 작 성 일 : 2014-9-2
// 작성내용 : 공정별 첨부문서(생산기술문서) 조회/열람
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using WNDW;

namespace TD.TDT006
{
	public partial class TDT006 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "PRC";

		// 마스터 컬럼
		int colItemCd = -1;
		int colRoutNo = -1;
		int colProcCd = -1;
		int colAttDocCode = 0;

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
		/// <summary>첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;
		#endregion

		#region 생성자
		public TDT006()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDT006_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);// 공장
			SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_SECT_CD = 'MDP', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드
			//SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE = 'COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);	// 품목계정
			cboItemAcct.SetItems(new Dictionary<string, string>() { { "10", "제품" }, { "20", "반제품" } }); // 품목계정

			// 그리드초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView masterSheet = fpSpread2.ActiveSheet;
			colItemCd = masterSheet.FindHeaderColumnIndex("품목코드");
			colRoutNo = masterSheet.FindHeaderColumnIndex("라우팅");
			colProcCd = masterSheet.FindHeaderColumnIndex("공정");
			colAttDocCode = fpSpread2.ActiveSheet.FindHeaderColumnIndex("첨부문서코드");
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
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, docCtgCd, "첨부문서코드")
			{
				HideEmptyColumns = true,
			};

			// 기타 세팅
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
		}
		#endregion

		#region 마스터 조회
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (selectedMasterRow == e.Range.Row) return;
			selectedMasterRow = e.Range.Row;
			SearchDocument();
		}

		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDT006 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "'";
				if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "' ";
				if (!string.IsNullOrEmpty(txtItemSpec.Text)) query += ", @pITEM_SPEC = '" + txtItemSpec.Text + "' ";
				if (!string.IsNullOrEmpty(cboItemAcct.SelectedText)) query += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue.ToString() + "'";
				if (!string.IsNullOrEmpty(cboDocCd.Text)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue + "' ";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);
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
		/// 첨부문서를 조회해 첨부문서 그리드에 뿌립니다.
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
				string query = "usp_T_DOC 'S1'"
					+ ", @pDOC_CTG_CD = 'PRC'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + masterSheet.FindCell(0, "공장코드").Text + "'"
					+ ", @pATT_KEY = '" + GetSelectedAttKeyCombination() + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

				// 자료파일 그리드 셀 조정
				SheetView sheet = fpSpread1.ActiveSheet;
				for (int row = 0; row < sheet.Rows.Count; row++)
				{
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

		#region 공유기능
		/// <summary>
		/// 현재 선택된 공정에 대한 첨부키조합(아이템코드/라우팅번호/공정시퀀스)을 구합니다.
		/// </summary>
		/// <param name="procRow">공정목록 행 인덱스</param>
		/// <returns></returns>
		string GetSelectedAttKeyCombination()
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			return selectedMasterRow > -1 ? sheet.Cells[selectedMasterRow, colItemCd].Text + "/" + sheet.Cells[selectedMasterRow, colRoutNo].Text + "/" + sheet.Cells[selectedMasterRow, colProcCd].Text : ""; // 키 = "아이템코드/라우팅번호/공정시퀀스"
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 품목 팝업 처리
		/// </summary>
		private void btnSItemCd_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtItemCd.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
					txtItemCd.Text = pu.ReturnValue[1].ToString();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 아이템코드 입력시 자동 첨부필터(입력시 '전체', 미입력시 '첨부')
		/// </summary>
		private void txtItemCd_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtItemCd.Text)) rdoAttachBoth.Checked = true;
			else rdoAttachYes.Checked = true;
		}
		#endregion

	}

}