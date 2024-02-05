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

namespace TD.TDA002
{
	public partial class TDA002 : UIForm.FPCOMM1
	{
		#region 필드
		/// <summary>문서카테고리(부서, 분류 목록)</summary>
		TreeNode docCategory = new TreeNode("root");

		// 컬럼
		int colDocDeptCd = -1;
		int colDocSectCd = -1;
		#endregion

		#region 생성자
		public TDA002()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트
		private void TDA002_Load(object sender, System.EventArgs e)
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

	}
}
