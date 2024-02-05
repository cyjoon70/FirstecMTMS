#region 작성정보
/*********************************************************************/
// 단위업무명 : 수주문서관리
// 작 성 자 : 이재광
// 작 성 일 : 2014-08-26
// 작성내용 : 수주관련문서(영업문서) 조회/열람/등록/관리
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

namespace TD.TDS001
{
	public partial class TDS001 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "SOD";

		/// <summary>현재 선택된 마스터 행</summary>
		int selectedMasterRow = -1;

		// 마스터 컬럼
		int colSoNo = -1;

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

		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>품목목록 첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;
		#endregion

		#region 생성자
		public TDS001()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDS001_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 설정
			SystemBase.ComboMake.C1Combo(cboSaleDuty, "usp_S_COMMON @pTYPE = 'S010' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); // 영업담당

            chkASType.Checked = false;      // 2015.10.19. hma 추가: A/S여부 항목 디폴트를 체크안한걸로

			// 그리드초기화
			G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "수주형태")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'SO_TYPE', @pSPEC2 = 'SO_TYPE_NM', @pSPEC3 = 'S_SO_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 수주형태
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_CTG_CD = 'SOD', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); // 문서종류
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView masterSheet = fpSpread2.ActiveSheet;
			colSoNo = fpSpread2.ActiveSheet.FindHeaderColumnIndex("수주번호");
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
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, docCtgCd, "첨부문서코드");

			// 기타 세팅
			dteRefDelvDtFr.Value = DateTime.Now;
			dteRefDelvDtTo.Value = DateTime.Now.AddYears(1);
		}
		#endregion

		#region 마스터 조회
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDS001 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				if (!string.IsNullOrEmpty(txtProjNo.Text)) query += ", @pPROJECT_NO = '" + txtProjNo.Text + "'";
				if (!string.IsNullOrEmpty(txtSoNo.Text)) query += ", @pSO_NO = '" + txtSoNo.Text + "'";
				if (!string.IsNullOrEmpty(txtSoldCustCd.Text)) query += ", @pSOLD_CUST = '" + txtSoldCustCd.Text + "'";
				if (!string.IsNullOrEmpty(cboSaleDuty.SelectedValue.ToString())) query += ", @pSALE_DUTY = '" + cboSaleDuty.SelectedValue + "'";
				if (!string.IsNullOrEmpty(dteRefDelvDtFr.Text)) query += ", @pREF_DELV_DT_FR = '" + dteRefDelvDtFr.Text + "'";
				if (!string.IsNullOrEmpty(dteRefDelvDtTo.Text)) query += ", @pREF_DELV_DT_TO = '" + dteRefDelvDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtSoTypeCd.Text)) query += ", @pSO_TYPE = '" + txtSoTypeCd.Text + "'";
                // 2015.10.19. hma 추가(Start): AS여부 검색조건
                if (chkASType.Checked) query += ", @pAS_SO_TYPE = 'Y'";
                else query += ", @pAS_SO_TYPE = 'N'";
                // 2015.10.19. hma 추가(End)
				if (!string.IsNullOrEmpty(txtEntCd.Text)) query += ", @pENT_CD = '" + txtEntCd.Text + "'";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

			    UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1);// 2015.10.19. hma 수정:읽기전용모드 제외 ,true
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
					+ ", @pDOC_CTG_CD = 'SOD'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pATT_KEY = '" + masterSheet.Cells[selectedMasterRow, colSoNo].Text + "'";

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

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 행추가
		protected override void RowInsExec()
		{
			if (selectedMasterRow < 0)
			{
				MessageBox.Show("먼저 수주를 선택해야합니다.", "행 추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			// 행추가
			UIForm.FPMake.RowInsert(fpSpread1);
			fpSpread1.Focus();

			// 자동입력 처리
			SheetView masterSheet = fpSpread2.ActiveSheet;
			SheetView sheet = fpSpread1.ActiveSheet;
			int newRow = sheet.ActiveRowIndex;
			//sheet.Cells[newRow, colRevNo].Value = "1.0";
			sheet.Cells[newRow, colRegUsrId].Value = SystemBase.Base.gstrUserID;
			sheet.Cells[newRow, colRegUsrNm].Value = SystemBase.Base.gstrUserName;
			buttonManager.UpdateButtons(newRow); // 버튼 업데이트
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
            string sSaveGrid1 = ""; string sSaveGrid2 = "";     // 2015.10.19. hma 추가
            int dEditRows = 0;

            //if (fpSpread1.Sheets[0].Rows.Count < 1) return;
            //그리드 상단 필수 체크
            //if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;

			this.Cursor = Cursors.WaitCursor;
			fpSpread1.Focus();

			string resultCode = "WR", resultMessage = "P0000"; //처리할 내용이 없습니다.
			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
                // 2015.10.19. hma 추가(Start): 상단그리드 저장
                for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;

                    string sRemark = "";
                    sRemark = fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "비고")].Text;

                    if (strHead.Length > 0)
                    {
                        string strSql = " usp_TDS001 'U1'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql += ", @pSO_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "수주번호")].Text + "' ";
                        strSql += ", @pSO_REMARK = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "비고")].Text + "' ";
                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();

                        sSaveGrid1 = "Y";
                        if (resultCode != "OK")
                        {
                            fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text = "";
                            Trans.Rollback();
                            goto Exit;
                        }	// ER 코드 Return시 점프
                    }
                }
                // 2015.10.19. hma 추가(End)


                //if (fpSpread1.Sheets[0].Rows.Count < 1) return;       // 2015.10.19. hma 주석처리
 
                //행수만큼 처리
                SheetView sheet = fpSpread1.ActiveSheet;
                SheetView masterSheet = fpSpread2.ActiveSheet;

                for (int row = 0; row < sheet.RowCount; row++)
                {
                    if (fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text.Length > 0)
                        dEditRows += 1;
                }

                if (dEditRows > 0)
                {
                    if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return;
                }


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
                        string strSql = string.Format("usp_T_DOC '" + strGbn + "', @pDOC_ID = {0}", sheet.Cells[row, colDocId].Value);
                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        resultCode = ds.Tables[0].Rows[0][0].ToString();
                        resultMessage = ds.Tables[0].Rows[0][1].ToString();
                        if (resultCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                    else
                    {
                        string query = "usp_T_DOC '" + strGbn + "'";
                        if (strHead == "I") // 새로 추가
                            query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                                + ", @pPLANT_CD = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "공장코드").Text + "' "
                                + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                                + ", @pATT_KEY = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "수주번호").Text + "'"
                                + ", @pATT_KEY1 = '" + masterSheet.FindCell(masterSheet.ActiveRowIndex, "수주번호").Text + "'"
                                + ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text + "'";
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

                        sSaveGrid2 = "Y";       // 2015.10.19. hma 추가

                        // 새 문서 추가인 경우 파일 업로드 및 정보 업데이트
                        if (strHead == "I")
                        {
                            if (Server.UploadDocumentFile(docCtgCd, sheet.Cells[row, colDocCd].Text, Convert.ToInt32(ds.Tables[0].Rows[0][2]), Convert.ToDateTime(ds.Tables[0].Rows[0][3]), buttonManager.GetAttachedFilename(row), dbConn, Trans) != Server.UploadResultState.Ok)
                            { Trans.Rollback(); goto Exit; }; // 실패시 롤백
                        }
                    }
				}

				Trans.Commit();

				// 품목의 첨부문서 코드문자열 업데이트
				attachmentManager.ReloadData(masterSheet.ActiveRowIndex, new string[] { masterSheet.Cells[masterSheet.ActiveRowIndex, colSoNo].Text });
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
                // 2015.10.19. hma 추가(Start): 상단 그리드 수정인 경우 다시 조회하도록 함.
                if (sSaveGrid1 == "Y")
                    SearchExec();
                else if (sSaveGrid2 == "Y")
                // 2015.10.19. hma 추가(End)
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

		#region 디테일 그리드 이벤트 핸들러
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
					sheet.Cells[row, colDocCd].Value = sheet.Cells[row, colDocNm].Value;
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
		/// 사업 팝업 처리
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
		/// 프로젝트 팝업 처리
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
		/// 주문처 팝업 처리
		/// </summary>
		private void btnSoldCust_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW.WNDW002 pu = new WNDW.WNDW002(txtSoldCustCd.Text, "");
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtSoldCustCd.Text = Msgs[1].ToString();
					txtSoldCustNm.Value = Msgs[2].ToString();
					txtSoldCustCd.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SOB004", "주문처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		/// <summary>
		/// 수주번호 팝업 처리
		/// </summary>
		private void btnSoNo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW.WNDW012 pu = new WNDW.WNDW012();
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtSoNo.Value = Msgs[1].ToString();
					txtSoNo.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 수주형태 팝업 처리
		/// </summary>
		private void btnSoType_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = "usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'SO_TYPE', @pSPEC2 = 'SO_TYPE_NM', @pSPEC3 = 'S_SO_TYPE', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtSoTypeCd.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수주형태조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtSoTypeCd.Text = Msgs[0].ToString();
					txtSoTypeNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "팝업 호출"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 사업 코드 입력 처리
		/// </summary>
		private void txtEntCd_TextChanged(object sender, EventArgs e)
		{
			txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		/// <summary>
		/// 프로젝트 코드 입력 처리
		/// </summary>
		private void txtProjNo_TextChanged(object sender, EventArgs e)
		{
			txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		/// <summary>
		/// 주문처 코드 입력 처리
		/// </summary>
		private void txtSoldCustCd_TextChanged(object sender, EventArgs e)
		{
			txtSoldCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtSoldCustCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		/// <summary>
		/// 수주형태 코드 입력 처리
		/// </summary>
		private void txtSoTypeCd_TextChanged(object sender, EventArgs e)
		{
			txtSoTypeNm.Value = SystemBase.Base.CodeName("SO_TYPE", "SO_TYPE_NM", "S_SO_TYPE", txtSoTypeCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

        // 2015.10.19. hma 추가(Start)
        /// <summary>
        /// 상단 그리드 수정시 이벤트 처리
        /// </summary>
        private void fpSpread2_Change(object sender, ChangeEventArgs e)
        {
            fpSpread2.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "U";
        }
        // 2015.10.19. hma 추가(End)
        
        #endregion



	}

}