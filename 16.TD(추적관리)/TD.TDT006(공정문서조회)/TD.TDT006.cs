#region 작성정보
/*********************************************************************/
// 단위업무명 : 라우팅변경조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-15
// 작성내용 : 라우팅변경조회 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Spread;
using EDocument.Extensions.FpSpreadExtension;
using WNDW;

namespace TD.TDT006
{
	public partial class TDT006 : UIForm.FPCOMM3
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "PRC";

		string selectedPlantCd = "";
		string selectedItemCd = "";
		string selectedItemNm = "";
		string selectedBomNo = "";
		string selectedNodeTag = "";

		/// <summary>공정 스프레드에서 선택된 행 인덱스</summary>
		int selectedProcRow = -1;

		// 마스터 컬럼
		int colRoutNo = -1;
		int colProcCode = -1;

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

        int chkStep = 0;

		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;

		/// <summary>자료파일목록 팝업의 자료파일상태 값 기억</summary>
		string sourceFileStateValue = "A";
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
			// 입력컨트롤 초기화
			SystemBase.Validation.GroupBox_Setting(groupBox8);	//컨트롤 필수 Setting
			SystemBase.ComboMake.C1Combo(cboSPLANT_CD, "usp_P_COMMON @pType='P510', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 공장
			dtpSVALID_DT.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

			// 그리드 초기화
			G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "공정타입")] = SystemBase.ComboMake.ComboOnGrid("usp_P_COMMON @pTYPE = 'P040', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCOM_CD = 'P028', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread3, null, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, 0, 0, false, false);        // 2018.10.23. hma 추가

            // 컬럼 인덱스
            SheetView subSheet = fpSpread2.ActiveSheet;
			colRoutNo = subSheet.FindHeaderColumnIndex("라우팅");
			colProcCode = subSheet.FindHeaderColumnIndex("공정");
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

			// 공정목록 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, docCtgCd, "첨부문서코드");

            rdoOperDoc.Checked = true;      // 2018.10.23. hma 추가: 조회구분 기본 선택을 '공정문서'로
            chkStep = 1;                    // 2018.10.23. hma 추가: 폼 로드 시점
            GridCommPanel1.Visible = true;       // 2018.10.23. hma 추가: 공정문서 조회 그리드
            GridCommPanel3.Visible = false;      // 2018.10.23. hma 추가: 기술자료 조회 그리드

        }
		#endregion

		#region SearchExec() 왼쪽 트리뷰 조회
		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			selectedNodeTag = e.Node.Tag.ToString();

			// 라우팅 정보 화면 출력
			SearchRouting();
		}

		protected override void SearchExec()
		{
			// TREE정보 설정
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox8))
				TreeViewSearch();
		}

		public void TreeViewSearch()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				treeView1.Nodes.Clear();
				string query = "usp_PBA172 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboSPLANT_CD.SelectedValue.ToString() + "'"
					+ ", @pITEM_CD = '" + txtSItemCd.Text + "'"
					+ ", @pVALID_DT = '" + dtpSVALID_DT.Text + "'"
					+ ", @pPRNT_BOM_NO = '1'";
				if (rdoLEVEL1.Checked == true)
					query += ", @pLEVEL = '1'";
				else
					query += ", @pLEVEL = '0'";

				DataSet ds = SystemBase.DbOpen.NoTranDataSet(query);

				if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
				{
					DataView dvwData = null;
					UIForm.TreeView.CommonTreeView(ds.Tables[0].Rows[0]["PRNT_ITEM_CD"].ToString()
						, ds.Tables[0].Rows[0]["FIGNO"].ToString()
						, (TreeNode)null
						, treeView1
						, ds
						, dvwData
						, imageList2
						, 0
						, true); // 라우팅없을 것에 대한 색깔 처리

					treeView1.Focus();
					treeView1.ExpandAll();
				}
				else
				{
					UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

					MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "TreeView 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 품목별 라우팅 조회
		private void SearchRouting()
		{
			cboRouting.DataSource = null;

			this.Cursor = Cursors.WaitCursor;
			try
			{
                string strRouting = "";
				string NODETAG = selectedNodeTag;
				string[] values = selectedNodeTag.Split(new string[] { "||" }, StringSplitOptions.None);

				selectedPlantCd = values[4];
				selectedItemCd = values[5];
				selectedBomNo = values[6];
				selectedItemNm = values[7];

				string query = "usp_TDT006 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD='" + selectedPlantCd + "'"
					+ ", @pITEM_CD='" + selectedItemCd + "'"
					+ ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";


				DataTable dt = SystemBase.DbOpen.NoTranDataTable(query);



                string query1 = "usp_TDT006 'S3'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pPLANT_CD='" + selectedPlantCd + "'"
                    + ", @pITEM_CD='" + selectedItemCd + "'";

                DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(query1);
                if (dt1.Rows.Count > 0)
                    strRouting = dt1.Rows[0][1].ToString();
                else
                    cboRouting.SelectedText = null;

				if (dt.Rows.Count > 0)
				{
					cboRouting.ValueMember =
					cboRouting.DisplayMember = "ROUT_NO";
					cboRouting.DataSource = dt;
					cboRouting.Splits[0].DisplayColumns[0].Width = cboRouting.Width;
					cboRouting.HScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.None;
					cboRouting.VScrollBar.Style = C1.Win.C1List.ScrollBarStyleEnum.Automatic;
					//cboRouting.SelectedIndex = 0;
                    for (int i = 0; i < 20; i++)
                    {
                        if (strRouting == cboRouting.GetItemText(i, 0))
                        {
                            cboRouting.SelectedIndex = i;
                        }
                    }
				}

				//SearchProcess(); // cboRouting은 아이템이 클리어될 때 텍스트체인지 이벤트가 발생하지 않으므로 멤버가 없을땐 업데이트 수동 호출
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "라우팅정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 라우팅별 공정 조회
		void SearchProcess()
		{
			ClearProcessSheet();
			ClearDocumentSheet();
			if (cboRouting.SelectedIndex < 0) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = " usp_TDT006 @pTYPE = 'S2'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD='" + selectedPlantCd + "'"
					+ ", @pITEM_CD ='" + selectedItemCd + "'"
					+ ", @pROUT_NO ='" + cboRouting.SelectedValue + "'";
				if (!string.IsNullOrEmpty(dtpSVALID_DT.Text)) query += ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0, true);
				attachmentManager.PlantCode = selectedPlantCd;
				attachmentManager.AppendColumns(); 	// 스프레드에 컬럼을 추가하고 문서첨부표시

				fpSpread1.ActiveSheet.RowCount = 0;
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정정보 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 공정별 첨부문서 조회
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (selectedProcRow == e.Range.Row) return;
			selectedProcRow = fpSpread2.ActiveSheet.RowCount > 0 ? e.Range.Row : -1;

			this.Cursor = Cursors.WaitCursor;
			try
			{
				SearchDocument();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공정별 첨부문서 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			this.Cursor = Cursors.Default;
		}

		/// <summary>
		/// 공정별 첨부문서를 조회합니다.
		/// </summary>
		/// <param name="row">공정시트의 행 인덱스</param>
		public void SearchDocument()
		{
			this.Cursor = Cursors.WaitCursor;
			try
			{
				ClearDocumentSheet();

                string query = " usp_T_DOC @pTYPE = 'S1'"
                    + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pPLANT_CD='" + selectedPlantCd + "'";

                if (selectedProcRow > -1)
                    query += ", @pATT_KEY = '" + GetSelectedAttKeyCombination() + "'";
                else
                {
                    // 전체조회 옵션. 현재 미사용.
                    query += ", @pITEM_CD ='" + selectedItemCd + "'"
                        + ", @pROUT_NO ='" + cboRouting.SelectedValue + "'";
                    if (!string.IsNullOrEmpty(dtpSVALID_DT.Text))
                        query += ", @pVALID_DT='" + dtpSVALID_DT.Text + "'";
                }

                UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

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

                // 2018.10.23. hma 추가(Start): 기술자료정보 데이터
                string query3 = " usp_TDT006 @pTYPE = 'S4'"
                    + ", @pDOC_CTG_CD = '" + docCtgCd + "'"
                    + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
                    + ", @pPLANT_CD='" + selectedPlantCd + "'";

                if (selectedProcRow > -1)
                    query3 += ", @pATT_KEY = '" + GetSelectedAttKeyCombination() + "'";

                UIForm.FPMake.grdCommSheet(fpSpread3, query3, G3Head1, G3Head2, G3Head3, G3Width, G3Align, G3Type, G3Color, G3Etc, G3HeadCnt, false, false, 0, 0, true);
                // 2018.10.23. hma 추가(End)

                this.Cursor = Cursors.Default;
			}
			catch (Exception f)
			{
				this.Cursor = Cursors.Default;
				SystemBase.Loggers.Log(this.Name, f.ToString());
				SystemBase.MessageBoxComm.Show(f.ToString());
			}
			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 공유 함수

		/// <summary>
		/// 공정 스프레드의 목록을 지웁니다.
		/// </summary>
		void ClearProcessSheet()
		{
			fpSpread2.ActiveSheet.RowCount = 0;
			selectedProcRow = -1;
		}

		/// <summary>
		/// 첨부문서 스프레드의 목록을 지웁니다.
		/// </summary>
		void ClearDocumentSheet()
		{
            // 2018.10.23. hma 수정(Start): 조회구분에 따라 Clear 시키는 그리드가 다르게 
            //fpSpread1.ActiveSheet.RowCount = 0;
            if (rdoOperDoc.Checked == true)         
                fpSpread1.ActiveSheet.RowCount = 0;
            else
                fpSpread3.ActiveSheet.RowCount = 0;
            // 2018.10.23. hma 수정(End)
        }

        /// <summary>
        /// 현재 선택된 마스터, 서브 항목에 대한 첨부키조합을 구합니다.
        /// </summary>
        /// <param name="procRow">공정목록 행 인덱스</param>
        /// <returns></returns>
        string GetSelectedAttKeyCombination()
		{
			return selectedProcRow > -1 ? selectedItemCd + "/" + GetSelectedRoutNo() + "/" + GetSelectedProcCode() : ""; // 키 = "아이템코드/라우팅번호/공정시퀀스"
		}

		/// <summary>
		/// 현재 선택된 공정의 공정코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedProcCode()
		{
			return selectedProcRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedProcRow, colProcCode].Text : "";
		}

		/// <summary>
		/// 현재 선택된 공정의 라우팅 번호를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedRoutNo()
		{
			return selectedProcRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedProcRow, colRoutNo].Text : "";
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 품목코드 조회
		/// </summary>
		private void btnSItemCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_P_COMMON @pTYPE = 'P030', @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
				string[] strSearch = new string[] { txtSItemCd.Text, txtSITEM_NM.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00001", strQuery, strWhere, strSearch, "품목코드 조회", new int[] { 1, 2 }, true);
				pu.Width = 500;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					txtSItemCd.Value = pu.ReturnValue[1].ToString();
					txtSITEM_NM.Value = pu.ReturnValue[2].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 품목코드 조회 변경시
		/// </summary>
		private void txtSItemCd_TextChanged(object sender, System.EventArgs e)
		{
			if (txtSItemCd.Text != "") txtSITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtSItemCd.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'");
			else txtSITEM_NM.Value = "";
		}

		private void txtSSCH_CD_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (Convert.ToInt32(e.KeyChar) == 13)
			{
				TreeViewSearch();
			}
		}
		#endregion

		private void cboRouting_TextChanged(object sender, EventArgs e)
		{
			SearchProcess();
		}

        // 2018.10.23. hma 추가(Start)
        private void rdoOperDoc_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOperDoc.Checked == true)
            {
                GridCommPanel1.Visible = true;       // 공정문서 조회 그리드
                GridCommPanel3.Visible = false;      // 기술자료 조회 그리드
            }
            else
            {
                GridCommPanel1.Visible = false;      // 공정문서 조회 그리드
                GridCommPanel3.Visible = true;       // 기술자료 조회 그리드
            }
            //SearchDocument();
        }

        private void rdoOperDocFile_CheckedChanged(object sender, EventArgs e)
        {
            //    if (rdoOperDocFile.Checked == true)
            //    {
            //        GridCommPanel1.Visible = false;      // 공정문서 조회 그리드
            //        GridCommPanel3.Visible = true;       // 기술자료 조회 그리드
            //    }
            //    else
            //    {
            //        GridCommPanel1.Visible = true;       // 공정문서 조회 그리드
            //        GridCommPanel3.Visible = false;      // 기술자료 조회 그리드
            //    }
            //    SearchDocument();
        }
        // 2018.10.23. hma 추가(End)
    }
}