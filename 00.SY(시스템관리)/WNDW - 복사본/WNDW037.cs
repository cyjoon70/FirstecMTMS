#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질증빙 확인 popup - 외주공정
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-09-16
// 작성내용 : 품질증빙 문서를 조회하는 공통 화면 - 외주 공정
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Network;
using EDocument.Spread;
using WNDW;

namespace WNDW
{
	public partial class WNDW037 : UIForm.FPCOMM2
	{

		#region 필드

		public string strWORKORDER_NO = string.Empty;
		public string strPROC_SEQ = string.Empty;
		public string strSEQ = string.Empty;
		public string strREQ_TYPE = string.Empty;
		public string strDOC_TYPE = string.Empty;
		public string strFormGubn = string.Empty;
		public string strPre_PROC_SEQ = string.Empty;

		const string docCtgCd = "OUT";

		/// <summary>마스터 그리드의 현재 선택된 행</summary>
		int selectedMasterRow = -1;
		/// <summary>서브 그리드의 현재 선택된 행</summary>
		int selectedSubRow = -1;

		// 마스터 그리드 컬럼(입고 목록)
		/// <summary>첨부문서 코드문자열 컬럼</summary>
		int colPlantCd = -1;
		int colWorkOrderNo = -1;
		int colAttDocCode = -1;
		int colPoNo = -1;
		int colPoSeq = -1;

		// 서브 그리드 컬럼(입고품목 목록)
		int colSubCheck = -1;
		int colSubWorkOrderNo = -1;
		int colSubProcSeq = -1;
		int colSubSeq = -1;

		// 디테일 그리드 컬럼(문서 목록)
		int colDocId = -1;
		int colDocMvntSeq = -1;
		int colDocSeq = -1;
		int colDocItemCd = -1;
		int colDocItemNm = -1;
		int colSvrPath = -1;
		int colSvrFnm = -1;
		int colOrgFnm = -1;
		int colFileExt = -1;
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
		public WNDW037()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void WNDW037_Load(object sender, EventArgs e)
		{
			UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			#region 첨부파일 처리를 위한 초기화

			// 컬럼 인덱스
			colSubWorkOrderNo = fpSpread2.ActiveSheet.FindHeaderColumnIndex("제조오더번호");
			colSubProcSeq = fpSpread2.ActiveSheet.FindHeaderColumnIndex("공정번호");
			colSubSeq = fpSpread2.ActiveSheet.FindHeaderColumnIndex("실적순번");
			colPoNo = fpSpread2.ActiveSheet.FindHeaderColumnIndex("발주번호");
			colPoSeq = fpSpread2.ActiveSheet.FindHeaderColumnIndex("발주순번");

			SheetView sheet = fpSpread1.ActiveSheet;
			colAttDocCode = fpSpread1.ActiveSheet.FindHeaderColumnIndex("문서코드");
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 파일선택 버튼, 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
			colFileExt = sheet.FindHeaderColumnIndex("파일확장자");
			colDocCd = sheet.FindHeaderColumnIndex("문서코드");
			colDocNm = sheet.FindHeaderColumnIndex("문서종류");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호");
			colRevNo = sheet.FindHeaderColumnIndex("개정번호");
			colRemark = sheet.FindHeaderColumnIndex("비고");
			colRegUsrId = sheet.FindHeaderColumnIndex("등록자");
			colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

			colWorkOrderNo = fpSpread2.Sheets[0].FindHeaderColumnIndex("제조오더번호");

			// 디테일 그리드 콤보박스
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pTOP_DOC_DEPT_CD = 'QC', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);

			// 첨부파일목록 파일버튼 관리자 초기화
			buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
			{
				FilenameColumnIndex = colOrgFnm,
				ServerPathColumnIndex = colSvrPath,
				ServerFilenameColumnIndex = colSvrFnm,
				FileSelectButtonColumnIndex = colOrgFnm - 3,
				FileViewButtonColumnIndex = colOrgFnm - 2,
				FileDownloadButtonColumnIndex = colOrgFnm - 1,
				DocTypeNameColumnIndex = colDocNm,
				DocRevisionColumnIndex = colRevNo,
				DocNumberColumnIndex = colDocNo,
			};

			// 품목목록 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, "OUT", null, "첨부문서코드", "필수문서코드")
			{
				HideEmptyColumns = true,
			};

			#endregion

			SearchExec();
		}
		#endregion

		#region 그리드 조회
		protected override void SearchExec()
		{
			selectedSubRow = -1;
			fpSpread1.ActiveSheet.RowCount = 0;
			fpSpread2.ActiveSheet.RowCount = 0;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread2.ActiveSheet;

				string query = "usp_TDQ003"
					+ "  @pTYPE = 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'"
					+ ", @pWORKORDER_NO = '" + strWORKORDER_NO + "'"
					+ ", @pPROC_SEQ = '" + strPROC_SEQ + "'"
					+ ", @pPROC_SEQ2 = '" + strPre_PROC_SEQ + "'"
					+ ", @pSEQ = '" + strSEQ + "'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
				attachmentManager.PlantCode = SystemBase.Base.gstrPLANT_CD;
				attachmentManager.AppendColumns(); // 스프레드에 컬럼을 추가하고 문서첨부표시

				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					SearchDocument();
				}
				else
				{
					this.Close();
					return;
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

		#region 첨부파일 조회
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					SearchDocument();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void SearchDocument()
		{
			fpSpread1.ActiveSheet.RowCount = 0;

			string query = "usp_T_DOC 'S1'"
				+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
				+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
				+ ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'"
				+ ", @pATT_KEY = '" + fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, colWorkOrderNo].Text + "/"
									+ fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, colSubProcSeq].Text + "/"
									+ fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, colSubSeq].Text + "'";

			UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
			buttonManager.UpdateButtons(); // 버튼 업데이트

			// 서브항목 선택인 경우 입고순번, 바코드 컬럼 숨김
			SheetView sheet = fpSpread1.ActiveSheet;
			if (selectedSubRow > -1)
			{
				sheet.Columns[colDocMvntSeq].Visible = false;
				sheet.Columns[colDocSeq].Visible = false;
				sheet.Columns[colDocItemCd].Visible = false;
				sheet.Columns[colDocItemNm].Visible = false;
			}

		}
		#endregion

	}
}
