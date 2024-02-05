#region 작성정보
/******************************************************************
	단위업무명 : 기술자료조회 공통팝업
	작 성 자 : 이재광
	작 성 일 : 2014-8-18
	작성내용 : 기술자료 조회 및 항목 선택
	수 정 일 :
	수 정 자 :
	수정내용 :
	비    고 :
******************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using FarPoint.Win.Spread;

#region 사용방법 예제
/*
try
{
    WNDW.WNDW034 dialog = new WNDW.WNDW034(plantCd);
    dialog.ShowDialog();
    if (dialog.DialogResult == DialogResult.OK)
    {
        string[] Msgs = pu.ReturnVal;

		foreach (WNDW034.SourceFileItem item in dialog.SelectedItems)
		{
			textBox1.Text = item.SrcfId;
			textBox2.Text = item.SvrPath;
			...
    }
}
catch (Exception f)
{
    SystemBase.Loggers.Log(this.Name, f.ToString());
    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "기술자료조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
}
 */
#endregion

namespace WNDW
{
	/// <summary>
	/// 기술자료조회 팝업창
	/// </summary>
	public partial class WNDW034 : UIForm.FPCOMM1
	{
		#region 필드
		/// <summary>
		/// 자료파일 아이템
		/// </summary>
		public struct SourceFileItem
		{
			/// <summary>자료파일 아이디</summary>
			public int SrcfId;
			/// <summary>서버 경로</summary>
			public string SvrPath;
			/// <summary>서버 파일명</summary>
			public string SvrFnm;
			/// <summary>원본 파일명</summary>
			public string OrgFnm;
			/// <summary>문서코드</summary>
			public string DocCd;
			/// <summary>문서명</summary>
			public string DocNm;
			/// <summary>문서번호</summary>
			public string DocNo;
			/// <summary>개정번호</summary>
			public string RevNo;
			/// <summary>비고</summary>
			public string Remark;
			/// <summary>자료상태</summary>
			public string SrcfState;
		}

		/// <summary>폼이 로드되었는지 여부입니다.</summary>
		bool formIsLoaded = false;
		/// <summary>자료파일상태 값</summary>
		string sourceFileStateValue = "";
		SourceFileItem[] selectedItems = new SourceFileItem[0];
		/// <summary>조회 행 순번</summary>
		int curRowNum = 1;
		/// <summary>페이지당 행 수</summary>
		int rowsPerPage = 100;
		string plantCd = "";
		string docCtgCd = "";
        string itemSpecNo = "";
		#endregion

		#region 속성
		/// <summary>
		/// 선택된 자료파일 목록입니다.
		/// </summary>
		public SourceFileItem[] SelectedItems
		{
			get { return selectedItems; }
		}
		#endregion

		#region 생성자
		/// <summary>
		/// 기술자료조회 팝업창을 생성합니다.
		/// </summary>
		/// <param name="plantCd">공장 코드</param>
		/// <param name="docCtgCd">문서카테고리 코드</param>
		public WNDW034(string plantCd, string docCtgCd)
		{
			this.plantCd = plantCd;
			this.docCtgCd = docCtgCd;

			InitializeComponent();
		}

        public WNDW034(string plantCd, string docCtgCd, string itemSpecNo)
        {
            this.plantCd = plantCd;
            this.docCtgCd = docCtgCd;
            this.itemSpecNo = itemSpecNo;
            InitializeComponent();
        }
		#endregion

		#region 폼 이벤트 핸들러
		private void WNDW003_Load(object sender, System.EventArgs e)
		{
			formIsLoaded = true;
			strFormClosingMsg = false; // 종료시 내용변경 확인하지 않기

			//버튼 재정의
			UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.Validation.GroupBox_Setting(groupBox1);//필수적용

			// 콤보박스 설정
			SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = '" + docCtgCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 자료종류
			cboSrcfState.SetItems(new Dictionary<string, string>() { { "", "" }, { "A", "활성" }, { "D", "폐기" } }); // 자료상태
			cboSrcfState.SelectedValue = sourceFileStateValue;

			// 그리드 정의
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

			// 그리드 콤보박스 정의
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "발행처")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);

			FarPoint.Win.Spread.Cell cell = fpSpread1.ActiveSheet.ColumnHeader.Cells[0, 2];


            txtStndNo.Text = itemSpecNo;
		}
		#endregion

		#region 속성
		/// <summary>
		/// 자료파일상태 값
		/// </summary>
		public string SourceFileState
		{
			get { return formIsLoaded ? cboSrcfState.SelectedValue.ToString() : sourceFileStateValue; }
			set
			{
				if (formIsLoaded) cboSrcfState.SelectedValue = value;
				sourceFileStateValue = value;
			}
		}
		#endregion

		#region 마스터 조회
		protected override void SearchExec()
		{ Grid_search(); }

		private void Grid_search()
		{
			this.Cursor = Cursors.WaitCursor;

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
			{
				try
				{
					curRowNum = 1;
					UIForm.FPMake.grdCommSheet(fpSpread1, GetQuery(), G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

					// 자료파일 그리드 셀값 조정
					SheetView sheet = fpSpread1.ActiveSheet;
					int colDeptCd = sheet.FindHeaderColumnIndex("자료상태");
					for (int row = 0; row < sheet.Rows.Count; row++)
					{
						// 폐기된 자료 표시
						if (Convert.ToString(sheet.Cells[row, colDeptCd].Value) == "폐기")
							sheet.Rows[row].SetApprearance(CellAppearance.Discard);
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
				}
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 공용함수
		string GetQuery()
		{
			string query = "usp_WNDW034 @pTYPE = 'S1'"
				+ ", @pROW_NUM = " + curRowNum
				+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'"
				+ ", @pPLANT_CD = '" + plantCd + "'"
				+ ", @pDOC_CTG_CD = '" + docCtgCd + "'";
			if (!string.IsNullOrEmpty(txtStndNo.Text)) query += ", @pSTND_NO = '" + txtStndNo.Text + "'";
			if (!string.IsNullOrEmpty(cboSrcfState.SelectedText)) query += ", @pSRCF_STATE = '" + cboSrcfState.SelectedValue + "'";
			if (!string.IsNullOrEmpty(cboDocCd.SelectedText)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue + "'";
			if (!string.IsNullOrEmpty(txtSrcfNo.Text)) query += ", @pSRCF_NO = '" + txtSrcfNo.Text + "'";
			if (!string.IsNullOrEmpty(txtSrcNm.Text)) query += ", @pSRC_NM = '" + txtSrcNm.Text + "'";
			if (!string.IsNullOrEmpty(txtEntCd.Text)) query += ", @pENT_CD = '" + txtEntCd.Text + "'";
			if (!string.IsNullOrEmpty(txtRegUserName.Text)) query += ", @pIN_ID = '" + txtRegUserName.Text + "'";
			if (!string.IsNullOrEmpty(txtPubTm.Text)) query += ", @pPUB_TM_CD = '" + txtPubTm.Text + "'";
			if (!string.IsNullOrEmpty(dtpRegDateFrom.Text)) query += ", @pIN_DT_FROM = '" + Convert.ToDateTime(dtpRegDateFrom.Value).ToShortDateString() + "'";
			if (!string.IsNullOrEmpty(dtpRegDateTo.Text)) query += ", @pIN_DT_TO = '" + Convert.ToDateTime(dtpRegDateTo.Value).ToShortDateString() + "'";

			return query;
		}
		#endregion

		#region 그리드 컨트롤 이벤트 핸들러
		/// <summary>
		/// 100건씩 조회
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
		{
			int FPHeight = (fpSpread1.Size.Height - 28) / 20;
			if (e.NewTop >= ((rowsPerPage + curRowNum) - FPHeight))
			{
				this.Cursor = Cursors.WaitCursor;

				curRowNum += rowsPerPage;
				UIForm.FPMake.grdCommSheet(fpSpread1, GetQuery());

				this.Cursor = Cursors.Default;
			}
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 사업코드 팝업
		/// </summary>
		private void btnEnt_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtEntCd.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
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
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 사용자 팝업
		/// </summary>
		private void btnRegUser_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { "", "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					//txtRegUserName.Value = Msgs[0].ToString();
					txtRegUserName.Text = Msgs[1].ToString();
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
		/// Submit 버튼 클릭시 선택목록 리스트업후 창 닫기
		/// </summary>
		private void btnSubmitDialog_Click(object sender, EventArgs e)
		{
			SheetView sheet = fpSpread1.ActiveSheet;
			if (sheet.RowCount > 0)
			{
				// 선택된 아이템 배열 초기화
				int itemCount = 0;
				for (int row = 0; row < sheet.RowCount; row++)
					if (sheet.Cells[row, 1].Text == "True") itemCount++;
				selectedItems = new SourceFileItem[itemCount];

				int colSrcfId = sheet.FindHeaderColumnIndex("자료파일ID");
				int colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
				int colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
				int colOrgFnm = sheet.FindHeaderColumnIndex("파일명");
				int colDocCd = sheet.FindHeaderColumnIndex("자료코드");
				int colDocNm = sheet.FindHeaderColumnIndex("자료종류");
				int colStndNo = sheet.FindHeaderColumnIndex("규격번호");
				int colPage = sheet.FindHeaderColumnIndex("페이지");
				int colRevNo = sheet.FindHeaderColumnIndex("개정번호");
				int colRevDt = sheet.FindHeaderColumnIndex("개정일");
				int colRemark = sheet.FindHeaderColumnIndex("비고");
				int colSrcfState = sheet.FindHeaderColumnIndex("자료상태");

				int itemIndex = 0;
				for (int row = 0; row < sheet.Rows.Count; row++)
				{
					if (sheet.Cells[row, 1].Text != "True") continue;

					selectedItems[itemIndex].SrcfId = Convert.ToInt32(sheet.Cells[row, colSrcfId].Text);
					selectedItems[itemIndex].SvrPath = sheet.Cells[row, colSvrPath].Text;
					selectedItems[itemIndex].SvrFnm = sheet.Cells[row, colSvrFnm].Text;
					selectedItems[itemIndex].OrgFnm = sheet.Cells[row, colOrgFnm].Text;
					selectedItems[itemIndex].DocCd = sheet.Cells[row, colDocCd].Text;
					selectedItems[itemIndex].DocNm = sheet.Cells[row, colDocNm].Text;
					selectedItems[itemIndex].DocNo = sheet.Cells[row, colStndNo].Text + "_" + sheet.Cells[row, colPage].Text;
					selectedItems[itemIndex].RevNo = sheet.Cells[row, colRevNo].Text;
					selectedItems[itemIndex].Remark = sheet.Cells[row, colRemark].Text;
					selectedItems[itemIndex].SrcfState = sheet.Cells[row, colSrcfState].Text;
					itemIndex++;
				}
			}
			else
				selectedItems = new SourceFileItem[0];

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// 사업명 자동 검색
		/// </summary>
		private void txtEntCd_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (txtEntCd.Text != "")
				{
					txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");
				}
				else
				{
					txtEntNm.Value = "";
				}
			}
			catch { }
		}

		/// <summary>
		/// 텍스트박스 컨트롤에서 엔터를 누르면 조회 수행
		/// </summary>
		private void txtSearchInput_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) Grid_search();
		}
		#endregion

	}

}
