#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매입고증빙조회
// 작 성 자 : 이재광
// 작 성 일 : 2014-9-15
// 작성내용 : 구매입고증빙 관련문서(품질문서) 조회/열람
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using EDocument.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;
using WNDW;

namespace TD.TDQ002
{
	public partial class TDQ002 : UIForm.FPCOMM2
	{
		#region 필드
		/// <summary>문서카테고리 코드</summary>
		const string docCtgCd = "PUR";

		// 마스터 컬럼
		int colPlantCd = -1;
		int colMvmtNo = -1;
		int colMvmtSeq = -1;
		int colBarCode = -1;

		// 디테일 컬럼
		int colDocId = -1;
		int colSrcfId = -1;
		int colSvrPath = -1;
		int colSvrFnm = -1;
		int colOrgFnm = -1;
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

		/// <summary>문서코드별 문서번호 유무</summary>
		Dictionary<string, string> docNoReqs = null;
		/// <summary>첨부파일목록 파일버튼 관리자</summary>
		FileButtonManager buttonManager;
		/// <summary>첨부문서표시 관리자</summary>
		AttachmentManager attachmentManager;
		#endregion

		#region 생성자
		public TDQ002()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void TDQ002_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);// 공장
			SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드

			// 그리드초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "문서종류")] = SystemBase.ComboMake.ComboOnGrid("usp_T_DOC_CODE @pTYPE = 'S1', @pTOP_DOC_DEPT_CD = 'QC', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0);
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView masterSheet = fpSpread2.ActiveSheet;
			colPlantCd = masterSheet.FindHeaderColumnIndex("공장코드");
			colMvmtNo = masterSheet.FindHeaderColumnIndex("입고번호");
			colMvmtSeq = masterSheet.FindHeaderColumnIndex("입고순번");
			colBarCode = masterSheet.FindHeaderColumnIndex("바코드");
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocId = sheet.FindHeaderColumnIndex("문서ID");
			colSrcfId = sheet.FindHeaderColumnIndex("자료파일ID");
			colSvrPath = sheet.FindHeaderColumnIndex("서버경로");
			colSvrFnm = sheet.FindHeaderColumnIndex("서버파일명");
			colOrgFnm = sheet.FindHeaderColumnIndex("파일명") + 3; // 미리보기 버튼, 다운로드 버튼 다음이 파일명 컬럼
			colDocCd = sheet.FindHeaderColumnIndex("문서코드");
			colDocNm = sheet.FindHeaderColumnIndex("문서종류");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호");
			colRevNo = sheet.FindHeaderColumnIndex("개정번호");
			colRemark = sheet.FindHeaderColumnIndex("비고");
			colSrcfState = sheet.FindHeaderColumnIndex("자료상태");
			colRegUsrId = sheet.FindHeaderColumnIndex("등록자ID");
			colRegUsrNm = sheet.FindHeaderColumnIndex("등록자");

			// 첨부파일목록 파일버튼 관리자 초기화
			buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.DocumentFile)
			{
				ServerPathColumnIndex = colSvrPath,
				ServerFilenameColumnIndex = colSvrFnm,
				FileViewButtonColumnIndex = colOrgFnm - 2,
				FileDownloadButtonColumnIndex = colOrgFnm - 1,
				FilenameColumnIndex = colOrgFnm,
				DocTypeNameColumnIndex = colDocNm,
				DocRevisionColumnIndex = colRevNo,
				DocNumberColumnIndex = colDocNo,
			};

			// 첨부문서표시 관리자 초기화
			attachmentManager = new AttachmentManager(fpSpread2.ActiveSheet, docCtgCd, null, "첨부문서코드", "필수문서코드")
			{
				HideEmptyColumns = true,
			};

			// 기타 세팅
			docNoReqs = SystemBase.Base.CreateDictionary("usp_T_DOC_CODE @pTYPE = 'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 문서번호 필수인 문서종류 정보
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			dteMvmtDtFrom.Value = DateTime.Now.AddMonths(-1);
			dteMvmtDtTo.Value = DateTime.Now;
            dteRELEASE_FR.Value = null;
            dteRELEASE_TO.Value = null;
		}
		#endregion

		#region 마스터 조회(입고목록)
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDQ002 'S1'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue.ToString() + "' ";

				if (!string.IsNullOrEmpty(txtMvmtNo.Text)) query += ", @pMVMT_NO = '" + txtMvmtNo.Text + "'";
				if (!string.IsNullOrEmpty(dteMvmtDtFrom.Text)) query += ", @pMVMT_DT_FR = '" + dteMvmtDtFrom.Text + "'";
				if (!string.IsNullOrEmpty(dteMvmtDtTo.Text)) query += ", @pMVMT_DT_TO = '" + dteMvmtDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtIOType.Text)) query += ", @pIO_TYPE = '" + txtIOType.Text + "'";
				if (!string.IsNullOrEmpty(txtInspReqNo.Text)) query += ", @pINSP_REQ_NO = '" + txtInspReqNo.Text + "'";
				if (!string.IsNullOrEmpty(dteInspReqDtFr.Text)) query += ", @pINSP_REQ_DT_FR = '" + dteInspReqDtFr.Text + "'";
				if (!string.IsNullOrEmpty(dteInspReqDtTo.Text)) query += ", @pINSP_REQ_DT_TO = '" + dteInspReqDtTo.Text + "'";
				if (!string.IsNullOrEmpty(dteDocInDtFr.Text)) query += ", @pDOC_IN_DT_FR = '" + dteDocInDtFr.Text + "'";
				if (!string.IsNullOrEmpty(dteDocInDtTo.Text)) query += ", @pDOC_IN_DT_TO = '" + dteDocInDtTo.Text + "'";
				if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
				if (!string.IsNullOrEmpty(txtCustCd.Text)) query += ", @pCUST_CD = '" + txtCustCd.Text + "'";
				if (!string.IsNullOrEmpty(txtProjNo.Text)) query += ", @pPROJ_NO = '" + txtProjNo.Text + "'";
				if (!string.IsNullOrEmpty(txtProjSeq.Text)) query += ", @pPROJ_SEQ = '" + txtProjSeq.Text + "'";
				if (!string.IsNullOrEmpty(cboDocCd.Text)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue + "'";
				if (!string.IsNullOrEmpty(txtDocNo.Text)) query += ", @pDOC_NO = '" + txtDocNo.Text + "'";
                if (!string.IsNullOrEmpty(dteRELEASE_FR.Text)) query += ", @pRELEASE_DT_FR = '" + dteRELEASE_FR.Text + "'";
                if (!string.IsNullOrEmpty(dteRELEASE_TO.Text)) query += ", @pRELEASE_DT_TO = '" + dteRELEASE_TO.Text + "'";
                if (!string.IsNullOrEmpty(txtInspectorCd.Text)) query += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";
                if (!string.IsNullOrEmpty(txtIN_ID.Text)) query += ", @pIN_CD = '" + txtIN_ID.Text + "'";
                string strSL_YN = "";
                if (rdoSL_Y.Checked == true)
                    strSL_YN = "Y";
                else if (rdoSL_N.Checked == true)
                    strSL_YN = "N";
                query += ", @pSL_YN = '" + strSL_YN + "'";
				if (rdoAttachYes.Checked) query += ", @pATT_YN = 'Y'";
				else if (rdoAttachNo.Checked) query += ", @pATT_YN = 'N'";

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1, true);
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

		#region 디테일 조회(첨부문서목록)
		private void fpSpread2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			if (sheet.ActiveRowIndex == selectedMasterRow) return;
			selectedMasterRow = sheet.RowCount > 0 ? sheet.ActiveRowIndex : -1;
			SearchDocument();
		}

		/// <summary>
		/// 첨부문서를 조회해 첨부문서 그리드에 뿌립니다.
		/// </summary>
		private void SearchDocument()
		{
			if (selectedMasterRow < 0)
			{
				fpSpread1.ActiveSheet.RowCount = 0;
				return;
			}

			this.Cursor = Cursors.WaitCursor;

			try
			{
				SheetView masterSheet = fpSpread2.ActiveSheet;
				string query = "usp_T_DOC 'S2'"
					+ ", @pDOC_CTG_CD = '" + docCtgCd + "'"
					+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
					+ ", @pPLANT_CD = '" + GetSelectedPlantCd() + "'"
					+ ", @pATT_KEY1 = '" + GetMasterKey() + "'"
					+ ", @pATT_KEY2 = '" + GetSecondKey() + "'"
                    + ", @pIN_ID = '" + txtIN_ID.Text + "'";
				string barcode = GetThirdKey();
				if (!string.IsNullOrEmpty(barcode)) query += ", @pATT_KEY3 = '" + barcode + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				buttonManager.UpdateButtons();

				// 문서번호 배경색 업데이트
				SheetView sheet = fpSpread1.ActiveSheet;
				for (int row = 0; row < sheet.RowCount; row++)
					UpdateDocNoCellBackgroundColor(row);
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
		/// 현재 선택된 마스터 항목에 대한 공장코드를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSelectedPlantCd()
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			return sheet.RowCount > 0 ? sheet.Cells[0, colPlantCd].Text : "";
		}

		/// <summary>
		/// 현재 선택된 마스터 그리드의 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetMasterKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colMvmtNo].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetSecondKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colMvmtSeq].Text : "";
		}

		/// <summary>
		/// 현재 선택된 서브 그리드의 두 번째 첨부키를 구합니다.
		/// </summary>
		/// <returns></returns>
		string GetThirdKey()
		{
			return selectedMasterRow > -1 ? fpSpread2.ActiveSheet.Cells[selectedMasterRow, colBarCode].Text : "";
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
		/// 프로젝트 코드 입력 처리
		/// </summary>
		private void txtProjNo_TextChanged(object sender, EventArgs e)
		{
			txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

		#endregion

        private void btnIN_ID_Click_1(object sender, EventArgs e)
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


	}

}