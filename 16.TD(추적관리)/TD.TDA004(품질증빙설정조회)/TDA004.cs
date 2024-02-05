using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Spread;


namespace TD.TDA004
{
	public partial class TDA004 : UIForm.FPCOMM1
	{
		#region 필드
		/// <summary>필수문서표시 관리자</summary>
		RequirementManager requirementManager;
		#endregion

		#region 생성자
		public TDA004()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트
		private void TDA004_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스
			SystemBase.ComboMake.C1Combo(cboDocCd, "usp_T_DOC_CODE @pTYPE = 'S1', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", -1); // 문서코드

			// 그리드
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

			// 필수문서표시 관리자 초기화
			SheetView sheet = fpSpread1.ActiveSheet;
			requirementManager = new RequirementManager(sheet, "필수문서코드")
			{
				HideEmptyColumns = true,
			};
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			if (!SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1)) return;

			this.Cursor = Cursors.WaitCursor;
			string query = "usp_TDA004 'S1'"
					+ ", @pCO_CD= '" + SystemBase.Base.gstrCOMCD.ToString() + "'";
			if (!string.IsNullOrEmpty(txtProCd.Text)) query += ", @pJOB_CD = '" + txtProCd.Text + "'";
			if (!string.IsNullOrEmpty(txtProNm.Text)) query += ", @pJOB_NM = '" + txtProNm.Text + "'";
			if (!string.IsNullOrEmpty(cboDocCd.Text)) query += ", @pDOC_CD = '" + cboDocCd.SelectedValue.ToString() + "'";

			/*
			if (optMsYes.Checked) query += ", @pMILESTONE_FLG = 'Y'"; // 마일스톤 옵션버튼(전체, 예, 아니오)
			else if (optMsNo.Checked) query += ", @pMILESTONE_FLG = 'N'";
			if (optInspYes.Checked) query += ", @pINSP_FLG = 'Y'"; // 검사대상 옵션버튼(전체, 예, 아니오)
			else if (optInspNo.Checked) query += ", @pINSP_FLG = 'N'";
			if (optUseYes.Checked) query += ", @pUSE_FLG = 'Y'"; // 사용여부 옵션버튼(전체, 예, 아니오)
			else if (optUseNo.Checked) query += ", @pUSE_FLG = 'N'";
			*/

			UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
			requirementManager.AppendColumns();
			this.Cursor = Cursors.Default;
		}
		#endregion

	}
}
