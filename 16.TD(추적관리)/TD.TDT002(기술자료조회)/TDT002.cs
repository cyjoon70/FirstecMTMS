#region 작성정보
/*********************************************************************/
// 단위업무명 : 기술자료관리
// 작 성 자 : 이재광
// 작 성 일 : 2014-07-21
// 작성내용 : 기술자료와 자료파일 조회/열람/등록/관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument;
using EDocument.Spread;
using EDocument.Network;
using EDocument.Extensions.C1ComboExtension;
using EDocument.Extensions.FpSpreadExtension;


namespace TD.TDT002
{
	public partial class TDT002 : UIForm.FPCOMM2
	{
		#region 필드
		// 디테일 컬럼(자료파일)
		int colSrcfState = -1;
		int colSrcfNo = -1;
		int colPage = -1;
		int colRevNo = -1;
		int colFormIdntButton = -1;
		int colFormCtrlButton = -1;
        int colFileViewBtn = -1;        // 2020.01.30. hma 추가: 파일뷰어 버튼 컬럼번호
        int colFileDownBtn = -1;        // 2020.01.30. hma 추가: 파일다운 버튼 컬럼번호
        int colEntViewChkYN = -1;       // 2020.02.07. hma 추가: 파일조회 체크 사업여부 컬럼번호
        int colEntViewUsrYN = -1;       // 2020.02.07. hma 추가: 파일조회 체크 사업 조회권한여부 컬럼번호

        /// <summary>자료파일목록 파일버튼 관리자</summary>
        FileButtonManager buttonManager;

		/// <summary>마스터그리드에서 선택된 행의 자료ID</summary>
		int selectedSrcId = -1;
		string strBtn = "N";
		#endregion

		public TDT002()
		{
			InitializeComponent();
		}

		#region 폼 이벤트
		private void TDT002_Load(object sender, System.EventArgs e)
		{
			// 그룹박스 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 설정
			SystemBase.ComboMake.C1Combo(cboSPlant, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
			SystemBase.ComboMake.C1Combo(cboSPubTeam, "usp_B_COMMON @pType='COMM1', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", -1);
			SystemBase.ComboMake.C1Combo(cboSDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pDOC_DEPT_CD = 'MT', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1);


			// 그리드 정의
			G2Etc[SystemBase.Base.GridHeadIndex(GHIdx2, "발행처")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "자료상태")] = "A#D|활성#폐기";
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "도면크기")] =
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "인쇄크기")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD008', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "작성처")] =
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "승인처")] =
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "출처")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM1', @pCODE = 'TD007', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "변경구분")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "규격종류")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'TD006', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "BOM변경여부")] = "Y#N|Y#N";
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
			
			// 컬럼인덱스
			SheetView srcfSheet = fpSpread1.ActiveSheet;
			colSrcfState = srcfSheet.FindHeaderColumnIndex("자료상태");
			colSrcfNo = srcfSheet.FindHeaderColumnIndex("자료파일번호") + 2;
			colPage = srcfSheet.FindHeaderColumnIndex("페이지");
			colRevNo = srcfSheet.FindHeaderColumnIndex("개정번호");
			colFormIdntButton = srcfSheet.FindHeaderColumnIndex("형상식별자");
			colFormCtrlButton = srcfSheet.FindHeaderColumnIndex("형상통제자");


			// 스프레드 파일버튼 관리자 초기화
			int col = fpSpread1.ActiveSheet.FindHeaderColumnIndex("파일명");
			buttonManager = new FileButtonManager(fpSpread1.ActiveSheet, FileButtonManager.ServerFileType.SourceFile)
			{
				FilenameColumnIndex = col + 3,
				ServerPathColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("서버경로"),
				ServerFilenameColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("서버파일명"),
				FileSelectButtonColumnIndex = col,
				FileViewButtonColumnIndex = col + 1,
				FileDownloadButtonColumnIndex = col + 2,
				DocTypeNameColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("자료종류"),
				DocRevisionColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("개정번호"),
				DocNumberColumnIndex = fpSpread1.ActiveSheet.FindHeaderColumnIndex("규격번호"),
			};

            // 2020.01.30. hma 추가(Start): 파일뷰어/파일다운 버튼, 사업코드 컬럼번호
            colFileViewBtn = buttonManager.FileViewButtonColumnIndex;        
            colFileDownBtn = buttonManager.FileDownloadButtonColumnIndex;

            colEntViewChkYN = fpSpread1.ActiveSheet.FindHeaderColumnIndex("VIEW체크사업여부");        // 2020.02.07. hma 수정: 항목명 및 변수명 변경
            colEntViewUsrYN = fpSpread1.ActiveSheet.FindHeaderColumnIndex("VIEW권한여부");            // 2020.02.07. hma 수정: 항목명 및 변수명 변경
            // 2020.01.30. hma 추가(End)

            // 기타
            NewExec();
			dteSAcptDtFr.Value = DateTime.Now.AddMonths(-1);
			dteSAcptDtTo.Value = DateTime.Now;
		}
		#endregion

		#region 입력 초기화
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			fpSpread2.ActiveSheet.RowCount = 0;
			fpSpread1.ActiveSheet.RowCount = 0;
			selectedSrcId = -1;
			UpdateSourceFileStatus();
		}
		#endregion

		#region 마스터 조회
		protected override void SearchExec()
		{
			Search(-1);
		}

		private void Search(int intSrcId)
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				string query = "usp_TDT002 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                query += ", @pSTND_NO = '" + txtSTND_NO.Text + "'";
				if (intSrcId < 0)
					query = AppendMasterQueryCondition(query);
				else
				{
					query += ", @pSRC_ID = " + intSrcId;
					selectedSrcId = intSrcId;
				}

				UIForm.FPMake.grdCommSheet(fpSpread2, query, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
				fpSpread2.ActiveSheet.Lock(true); // 읽기전용

				// 자료아이디가 지정된 경우 자동 선택
				selectedSrcId = -1;
				SheetView sheet = fpSpread2.ActiveSheet;
				if (intSrcId > 0 && sheet.RowCount > 0)
				{
					int col = sheet.FindHeaderColumnIndex("자료ID");
					int row = sheet.FindRowIndex(col, intSrcId.ToString());
					if (row > -1)
					{
						sheet.SetActiveCell(row, col);
						sheet.AddSelection(row, 0, 1, sheet.ColumnCount);
						selectedSrcId = intSrcId;

						SearchDetail(selectedSrcId);
					}
				}

				// 선택 자료가 없는 경우 우측패널 클리어
				fpSpread1.ActiveSheet.RowCount = 0;
				UpdateSourceFileStatus();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 디테일 조회
		private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			SheetView sheet = fpSpread2.ActiveSheet;
			if (fpSpread2.Sheets[0].Rows.Count > 0)
			{
				try
				{
					int row = sheet.GetSelection(0).Row;
					int srcId = Convert.ToInt32(sheet.Cells[row, sheet.FindHeaderColumnIndex("자료ID")].Value);
					if (srcId == selectedSrcId) return; // 이미 선택한 행이면 스킵

					selectedSrcId = srcId;
					SearchDetail(selectedSrcId);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					//데이터 조회 중 오류가 발생하였습니다.		
				}
			}
		}

		private void SearchDetail(int srcId)
		{
			this.Cursor = Cursors.WaitCursor;
            try
            {
                fpSpread1.Sheets[0].Rows.Count = 0;

                //자료파일 리스트업
                string sfileQuery = "usp_TDT002  'S2', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                sfileQuery += ", @pSTND_NO = '" + txtSTND_NO.Text + "'";
                sfileQuery += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";   // 2020.01.31. hma 추가: 사용자ID

                if (srcId > 0) sfileQuery += ", @pSRC_ID = " + srcId;
                else sfileQuery = AppendMasterQueryCondition(sfileQuery);

                UIForm.FPMake.grdCommSheet(fpSpread1, sfileQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UpdateSourceFileStatus(srcId > 0 ? false : true);

                // 자료파일 그리드 셀 조정
                SheetView sheet = fpSpread1.ActiveSheet;
                sheet.Lock(true); // 읽기전용
                for (int row = 0; row < sheet.Rows.Count; row++)
                {
                    // 폐기된 자료행 강조                    
                    if (Convert.ToString(sheet.Cells[row, colSrcfState].Value) == "D")    // 2020.01.30. hma 수정: 폐기=>D로 변경. 자료상태 명세가 아닌 코드로 비교하도록.
                    {
                        Row oRow = sheet.Rows[row];
                        oRow.SetApprearance(CellAppearance.Discard);
                        oRow.Locked = true;
                    }

                    // 버튼 업데이트
                    buttonManager.UpdateButtons(row);

                    // 2020.01.30. hma 추가(Start): 폐기된 자료인 경우 자료보기 및 자료다운이 안되도록 하고, ZZ001 사업은 지정된 사원만 파일조회/파일다운 가능하도록 함.
                    if (Convert.ToString(sheet.Cells[row, colSrcfState].Value) == "D")      // 자료상태 = D(폐기)이면
                    {
                        sheet.Cells[row, colFileViewBtn].Locked = true;
                        sheet.Cells[row, colFileDownBtn].Locked = true;
                    }

                    if (sheet.Cells[row, colEntViewChkYN].Text == "Y")      // 자료조회 체크 사업인 경우 공통코드에 등록된 사용자가 아니면 파일보기와 파일다운 버튼 LOCK 처리
                    {
                        if (sheet.Cells[row, colEntViewUsrYN].Text == "N")
                        {
                            sheet.Cells[row, colFileViewBtn].Locked = true;
                            sheet.Cells[row, colFileDownBtn].Locked = true;
                        }
                    }
                    // 2020.01.30. hma 추가(End)
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 공유기능
		/// <summary>
		/// 자료 조회조건 문자열을 추가합니다.
		/// </summary>
		/// <param name="query"></param>
		/// <returns>조건이 추가된 쿼리</returns>
		string AppendMasterQueryCondition(string query)
		{
			return query
				+ ", @pPLANT_CD = '" + cboSPlant.SelectedValue + "'"
				+ ", @pACPT_DT_FROM = '" + dteSAcptDtFr.Text + "'"
				+ ", @pACPT_DT_TO = '" + dteSAcptDtTo.Text + "'"
				+ ", @pENT_CD = '" + txtSEntCd.Text + "'"
				+ ", @pACPT_USR_ID = '" + txtSAcptUsrId.Text + "'"
				+ ", @pPUB_TM_CD = '" + cboSPubTeam.SelectedValue + "'"
				+ ", @pDOC_CD = '" + cboSDocCd.SelectedValue + "'"
				+ ", @pSRC_NO = '" + txtSSrcNo.Text + "'"
				+ ", @pSRC_NM = '" + txtSSrcNm.Text + "'";
		}

		/// <summary>
		/// 사용자/사업 버튼과 연결된 아이디/코드 텍스트박스를 찾습니다.
		/// </summary>
		/// <param name="sender">버튼</param>
		/// <returns>아이디/코드 텍스트박스</returns>
		C1.Win.C1Input.C1TextBox FindCodeTextbox(object sender)
		{
			if (sender == btnSAcptUsr || sender == txtSAcptUsrId) return txtSAcptUsrId;
			else if (sender == btnSEnt || sender == txtSEntCd) return txtSEntCd;
			return null;
		}

		/// <summary>
		/// 사용자/사업 버튼 또는 아이디/코드 텍스트박스와 연결된 사용자명/사업명 텍스트박스를 찾습니다.
		/// </summary>
		/// <param name="sender">사용자/사업 버튼, 아이디/코드 텍스트박스</param>
		/// <returns>이름 텍스트박스</returns>
		C1.Win.C1Input.C1TextBox FindNameTextbox(object sender)
		{
			if (sender == btnSAcptUsr || sender == txtSAcptUsrId) return txtSAcptUsrNm;
			else if (sender == btnSEnt || sender == txtSEntCd) return txtSEntNm;
			return null;
		}

		/// <summary>
		/// 사용자 선택 팝업을 실행합니다.
		/// </summary>
		/// <param name="usrCd">사용자분류코드</param>
		/// <param name="userId">자동입력할 사용자 ID</param>
		/// <param name="userNm">자동입력할 사용자명</param>
		/// <returns>사용자가 선택되었을 경우 {사용자ID, 사용자명}을, 그렇지 않은 경우 null을 반환합니다.</returns>
		string[] ShowUserPopup(string usrCd, string userId, string userNm)
		{
			try
			{
				string query = "usp_B_COMMON 'B015' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "', @pUSR_CD = '" + usrCd + "'";
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", query, new string[] { "@pCODE", "@pNAME" }, new string[] { userId, userNm }, new int[] { 0, 1 }, "사용자 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] values = rx1.Split(pu.ReturnVal.ToString());
					if (values != null || values.Length > 1)
						return new string[] { values[0], values[1] };
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
			return null;
		}

		/// <summary>
		/// 자료파일의 갯수 표시를 업데이트합니다.
		/// </summary>
		/// <param name="all">전체목록인지 여부</param>
		void UpdateSourceFileStatus(bool all)
		{
			int count = fpSpread1.ActiveSheet.RowCount;
			txtSrcfStatus.Text = (all ? "조회조건의 모든 자료파일 " : "") + (count > 0 ? "총 " + count + "개" : "");
		}

		/// <summary>
		/// 자료파일의 갯수 표시를 업데이트합니다.
		/// </summary>
		void UpdateSourceFileStatus()
		{
			UpdateSourceFileStatus(false);
		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 사업코드 팝업
		/// </summary>
		private void btnEnt_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = "usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtSEntCd.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					FindCodeTextbox(sender).Value = Msgs[0];
					FindNameTextbox(sender).Value = Msgs[1];
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 조회된 자료마스터에 포함된 모든 자료파일 조회
		/// </summary>
		private void btnSearchAllSrcf_Click(object sender, EventArgs e)
		{
			selectedSrcId = -1;
			fpSpread2.ActiveSheet.ResetSelection();
			SearchDetail(-1);
		}

		/// <summary>
		/// 사용자 팝업(조회 접수자, 접수자, 승인자)
		/// </summary>
		private void btnUser_Click(object sender, EventArgs e)
		{
			strBtn = "Y";
			string[] values = ShowUserPopup("ENG", FindCodeTextbox(sender).Text, "");
			if (values != null)
			{
				FindCodeTextbox(sender).Value = values[0];
				FindNameTextbox(sender).Value = values[1];
			}
			strBtn = "N";
		}

		private void txtEntCd_TextChanged(object sender, EventArgs e)
		{
			try
			{
				C1.Win.C1Input.C1TextBox entCdBox = (C1.Win.C1Input.C1TextBox)sender;
				if (entCdBox.Text != "")
				{
					FindNameTextbox(sender).Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", entCdBox.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
				}
				else
				{
					FindNameTextbox(sender).Value = "";
				}
			}
			catch { }
		}

		private void txtUserId_TextChanged(object sender, EventArgs e)
		{
			if (strBtn == "N")
			{
				FindNameTextbox(sender).Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", FindCodeTextbox(sender).Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
			}
		}
		#endregion

	}
}