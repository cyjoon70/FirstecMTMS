#region 작성정보
/*********************************************************************/
// 단위업무명 : 문서코드등록
// 작 성 자 : 이재광
// 작 성 일 : 2014-07-15
// 작성내용 : 문서코드등록 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using FarPoint.Win.Spread.CellType;
using EDocument.Extensions.FpSpreadExtension;

namespace TD.TDA001
{
	public partial class TDA001 : UIForm.FPCOMM1
	{
		#region 필드
		/// <summary>문서카테고리(부서, 분류 목록)</summary>
		TreeNode docCategory = new TreeNode("root");

		// 컬럼
		int colDocDeptCd = -1;
		int colDocSectCd = -1;
		int colDocCd = -1;
		int colDocNm = -1;
		int colIdntChar = -1;
		int colDocNo = -1;
		#endregion

		#region 생성자
		public TDA001()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트
		private void TDA001_Load(object sender, System.EventArgs e)
		{
			string deptQuery = "usp_B_COMMON @pType='COMM', @pCODE = 'TD001', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

			// 부서목록 로드
			Dictionary<string, string> deps = SystemBase.Base.CreateDictionary(deptQuery);
			docCategory.Nodes.Clear();
			foreach (string key in deps.Keys)
			{
				TreeNode node = new TreeNode();
				node.Name = key;
				node.Text = deps[key];
				docCategory.Nodes.Add(node);
			}

			// 부서별 분류목록 로드
			foreach (TreeNode node in docCategory.Nodes)
			{
				Dictionary<string, string> secs = SystemBase.Base.CreateDictionary("usp_B_COMMON @pType='REL1', @pCODE = 'TD002', @pSPEC1 = '" + node.Name + "', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
				foreach (string key in secs.Keys)
				{
					TreeNode childNode = new TreeNode();
					childNode.Name = key;
					childNode.Text = secs[key];
					node.Nodes.Add(childNode);
				}
			}

			// 그리드 초기화
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "부서")] = SystemBase.ComboMake.ComboOnGrid(deptQuery, 0); // 부서	
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

			// 컬럼인덱스
			SheetView sheet = fpSpread1.ActiveSheet;
			colDocDeptCd = sheet.FindHeaderColumnIndex("부서");
			colDocSectCd = sheet.FindHeaderColumnIndex("분류");
			colDocCd = sheet.FindHeaderColumnIndex("문서코드");
			colDocNm = sheet.FindHeaderColumnIndex("문서명");
			colIdntChar = sheet.FindHeaderColumnIndex("문서번호 식별문자");
			colDocNo = sheet.FindHeaderColumnIndex("문서번호입력");
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			string strQuery = "usp_TDA001 @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
			UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

			// 그리드 조정
			SheetView sheet = fpSpread1.ActiveSheet;
			for (int row = 0; row < sheet.Rows.Count; row++)
				sheet.Cells[row, colDocSectCd].CellType = CreateSectionComboType(sheet.Cells[row, colDocDeptCd].Value.ToString()); // 부서별 문서코드 콤보 할당

			((TextCellType)sheet.Columns[colDocCd].CellType).MaxLength = 10; // 문서코드
			((TextCellType)sheet.Columns[colIdntChar].CellType).MaxLength = 1; // 문서코드

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region 저장
		protected override void SaveExec()
		{
			if (!SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) return; // 그리드 상단 필수항목 체크

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
			string strKeyCd = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				//행수만큼 처리
				for (int row = 0; row < fpSpread1.Sheets[0].Rows.Count; row++)
				{
					string strHead = fpSpread1.Sheets[0].RowHeader.Cells[row, 0].Text;
					string strGbn = "";
					if (strHead.Length > 0)
					{
						switch (strHead)
						{
							case "U": strGbn = "U1"; break;
							case "D": strGbn = "D1"; break;
							case "I": strGbn = "I1"; break;
							default: strGbn = ""; break;
						}

						SheetView sheet = fpSpread1.ActiveSheet;
						string query = " usp_TDA001 @pTYPE = '" + strGbn + "'"
							+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"
							+ ", @pDOC_CD = '" + sheet.Cells[row, colDocCd].Text.Trim() + "'";
						if (strHead == "U")
							query += ", @pDOC_NM = '" + sheet.Cells[row, colDocNm].Text.Trim() + "'"
								+ ", @pIDNT_CHAR = '" + sheet.Cells[row, colIdntChar].Text.Trim() + "'";
						else if (strHead == "I")
							query += ", @pDOC_NM = '" + sheet.Cells[row, colDocNm].Text.Trim() + "'"
								+ ", @pIDNT_CHAR = '" + sheet.Cells[row, colIdntChar].Text.Trim() + "'"
								+ ", @pDOC_NO_YN = '" + (Convert.ToString(sheet.Cells[row, colDocNo].Value) == "1" ? "Y" : "N") + "'"
								+ ", @pDOC_DEPT_CD = '" + sheet.Cells[row, colDocDeptCd].Value.ToString() + "'"
								+ ", @pDOC_SECT_CD = '" + sheet.Cells[row, colDocSectCd].Value.ToString() + "'"
								+ ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

						DataSet ds = SystemBase.DbOpen.TranDataSet(query, dbConn, Trans);
						ERRCode = ds.Tables[0].Rows[0][0].ToString();
						MSGCode = ds.Tables[0].Rows[0][1].ToString();

						if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
					}
				}
				Trans.Commit();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				Trans.Rollback();
				MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
			}
		Exit:
			dbConn.Close();

			if (ERRCode == "OK")
			{
				SearchExec();
				UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd); //그리드 위치를 가져온다

				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else if (ERRCode == "ER")
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			else
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region 해당 부서의 분류 콤보박스 정의 생성
		protected ComboBoxCellType CreateSectionComboType(string deptCode)
		{
			TreeNode depNode = docCategory.Nodes.Find(deptCode, false)[0];
			string[] keys = new string[depNode.Nodes.Count];
			string[] values = new string[depNode.Nodes.Count];
			for (int index = 0; index < depNode.Nodes.Count; index++)
			{
				TreeNode node = depNode.Nodes[index];
				keys[index] = node.Name;
				values[index] = node.Text;
			}

			ComboBoxCellType comboType = new ComboBoxCellType();
			comboType.MaxDrop = 20;
			comboType.ItemData = keys;
			comboType.Items = values;
			comboType.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

			return comboType;
		}
		#endregion

		#region 그리드 상 데이터 변경시 연계데이터 자동입력
		protected override void fpSpread1_ChangeEvent(int Row, int Column)
		{
			// 부서 변경시 분류값 초기화 및 콤보리스트 변경
			if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부서"))
			{
				TreeNode depNode = docCategory.Nodes.Find(fpSpread1.Sheets[0].Cells[Row, Column].Value.ToString(), false)[0];
				string[] keys = new string[depNode.Nodes.Count];
				string[] values = new string[depNode.Nodes.Count];
				for (int index = 0; index < depNode.Nodes.Count; index++)
				{
					TreeNode node = depNode.Nodes[index];
					keys[index] = node.Name;
					values[index] = node.Text;
				}

				Cell cell = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "분류")];
				cell.Text = string.Empty;
				cell.CellType = CreateSectionComboType(fpSpread1.Sheets[0].Cells[Row, Column].Value.ToString());
			}
		}
		#endregion

		#region RowInsExec 행 추가
		protected virtual void RowInsExe() { }
		protected override void RowInsExec()
		{	// 행 추가
			try
			{
				UIForm.FPMake.RowInsert(fpSpread1);

				int TmpRow = 0;
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					if (fpSpread1.ActiveSheet.GetSelection(0) == null)
					{
						TmpRow = fpSpread1.ActiveSheet.ActiveRowIndex;
					}
					else
					{
						TmpRow = fpSpread1.ActiveSheet.GetSelection(0).Row;
					}
					if (TmpRow != 0)
					{
						fpSpread1.Sheets[0].Cells[TmpRow, 1].Text = fpSpread1.Sheets[0].Cells[TmpRow - 1, 1].Text;	//행추가시 분류 콤보박스 설정

						Cell cell = fpSpread1.Sheets[0].Cells[TmpRow, SystemBase.Base.GridHeadIndex(GHIdx1, "분류")];
						cell.Text = string.Empty;
						cell.CellType = CreateSectionComboType(fpSpread1.Sheets[0].Cells[TmpRow, 1].Value.ToString());
					}
				}
				RowInsExe();
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion
	}
}
