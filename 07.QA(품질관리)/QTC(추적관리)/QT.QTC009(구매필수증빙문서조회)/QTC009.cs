
#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청상세조회
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-09
// 작성내용 : 구매요청상세조회 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using FarPoint.Win.Spread;
using WNDW;
using EDocument.Extensions.FpSpreadExtension;
using EDocument.Extensions.C1ComboExtension;

namespace QT.QTC009
{
	public partial class QTC009 : UIForm.FPCOMM1
	{
		#region 필드
		// 컬럼 인덱스
		int colStdDoc = -1;
		int colReqDoc = -1;
		int colPoDoc = -1;
		#endregion

		#region 생성자
		public QTC009()
		{
			InitializeComponent();
		}
		#endregion

		#region 폼 이벤트 핸들러
		private void QTC009_Load(object sender, System.EventArgs e)
		{
			// 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			// 콤보박스 설정
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='TABLE', @pCODE = 'PLANT_CD', @pNAME = 'PLANT_NM', @pSPEC1 = 'B_PLANT_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); // 공장
			cboReqStatus.SetItems("usp_B_COMMON @pTYPE = 'COMM_POP' ,@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pSPEC1 = 'M004', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", new string[] { "MINOR_CD", "CD_NM" }, 0, 1, new string[] { "" }); // 요청진행상태
			cboReqType.SetItems("usp_B_COMMON @pTYPE = 'COMM_POP' ,@pLANG_CD = '" + SystemBase.Base.gstrLangCd + "',@pSPEC1 = 'M003', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", new string[] { "MINOR_CD", "CD_NM" }, 0, 1, new string[] { "" }); // 구매요청유형
			cboItemAcct.SetItems("usp_B_COMMON @pTYPE = 'COMM_POP', @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pSPEC1 = 'B036', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", new string[] { "MINOR_CD", "CD_NM" }, 0, 1, new string[] { "" }); // 품목계정
			cboItemType.SetItems("usp_B_COMMON  @pTYPE = 'COMM_POP' ,@pSPEC1='P032', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", new string[] { "MINOR_CD", "CD_NM" }, 0, 1, new string[] { "" }); // 품목구분
			SystemBase.ComboMake.C1Combo(cboItemDiv, "usp_M_COMMON @pTYPE = 'M031', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'"); // 내외자 구분

			// 그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			// 컬럼 인덱스
			SheetView sheet = fpSpread1.ActiveSheet;
			colStdDoc = sheet.FindHeaderColumnIndex("표준증빙문서");
			colReqDoc = sheet.FindHeaderColumnIndex("요청증빙문서");
			colPoDoc = sheet.FindHeaderColumnIndex("발주증빙문서");

			NewExec();
		}
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			// 입력 컨트롤 초기화
			cboPlantCd.SelectedValue = SystemBase.Base.gstrPLANT_CD;
			txtProjectNo.Text =
			txtPoNo.Text =
			txtItemCd.Text =
			txtReqDeptCd.Text =
			txtReqId.Text =
			txtReqNo.Text = "";
			cboItemDiv.SelectedIndex =
			cboItemAcct.SelectedIndex =
			cboItemType.SelectedIndex =
			cboReqStatus.SelectedIndex =
			cboReqType.SelectedIndex = 0;
			dtpReqDtFr.Value =
			dtpDeliveryDtFr.Value = DateTime.Now.AddMonths(-1);
			dtpReqDtTo.Value =
			dtpDeliveryDtTo.Value = DateTime.Now;
			rdoAttMatchNo.Checked = true;
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
			{
				this.Cursor = Cursors.WaitCursor;

				try
				{
					string query = "usp_QTC009 @pTYPE = 'S1'"
						+ ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' "
						+ ", @pPLANT_CD = '" + cboPlantCd.SelectedValue + "'"
						+ ", @pREQ_PART = '" + (rdoReqPartM.Checked ? "MPR" : "SPR") + "'";
					if (rdoCfmYes.Checked) query += ", @pCONFIRM_YN = 'Y'";
					else if (rdoCfmNo.Checked) query += ", @pCONFIRM_YN = 'N'";
					if (!string.IsNullOrEmpty(txtItemCd.Text)) query += ", @pITEM_CD = '" + txtItemCd.Text + "'";
					if (!string.IsNullOrEmpty(dtpReqDtFr.Text)) query += ", @pREQ_DT_FR = '" + dtpReqDtFr.Text + "'";
					if (!string.IsNullOrEmpty(dtpReqDtTo.Text)) query += ", @pREQ_DT_TO = '" + dtpReqDtTo.Text + "'";
					if (!string.IsNullOrEmpty(dtpDeliveryDtFr.Text)) query += ", @pDELIVERY_DT_FR = '" + dtpDeliveryDtFr.Text + "'";
					if (!string.IsNullOrEmpty(dtpDeliveryDtTo.Text)) query += ", @pDELIVERY_DT_TO = '" + dtpDeliveryDtTo.Text + "'";
					if (!string.IsNullOrEmpty(cboReqStatus.Text)) query += ", @pREQ_STATUS = '" + cboReqStatus.SelectedValue + "'";
					if (!string.IsNullOrEmpty(txtReqDeptCd.Text)) query += ", @pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "'";
					if (!string.IsNullOrEmpty(cboReqType.Text)) query += ", @pREQ_TYPE = '" + cboReqType.SelectedValue + "'";
					if (!string.IsNullOrEmpty(txtProjectNo.Text)) query += ", @pPROJECT_NO = '" + txtProjectNo.Text + "'";
					if (!string.IsNullOrEmpty(txtProjectSeq.Text)) query += ", @pPROJECT_SEQ = '" + txtProjectSeq.Text + "'";
					if (!string.IsNullOrEmpty(txtReqNo.Text)) query += ", @pREQ_NO = '" + txtReqNo.Text + "'";
					if (!string.IsNullOrEmpty(txtReqId.Text)) query += ", @pREQ_ID = '" + txtReqId.Text + "'";
					if (!string.IsNullOrEmpty(txtPoNo.Text)) query += ", @pPO_NO = '" + txtPoNo.Text + "'";
					if (!string.IsNullOrEmpty(cboItemAcct.Text)) query += ", @pITEM_ACCT = '" + cboItemAcct.SelectedValue + "'";
					if (!string.IsNullOrEmpty((string)cboItemDiv.SelectedValue)) query += ", @pITEM_DIV = '" + cboItemDiv.SelectedValue + "'";
					if (!string.IsNullOrEmpty(cboItemType.Text)) query += ", @pITEM_TYPE = '" + cboItemType.SelectedValue + "'";
					if (rdoAttMatchNo.Checked) query += ", @pATT_MATCH_YN = 'N'";

					UIForm.FPMake.grdCommSheet(fpSpread1, query, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
					SheetView sheet = fpSpread1.ActiveSheet;
					sheet.Lock(true); // 읽기전용

					// 그리드 내용 수정
					for (int row = 0; row < sheet.RowCount; row++)
					{
						// 3문서 불일치시 강조
						Cell stdDocCell = sheet.Cells[row, colStdDoc];
						Cell reqDocCell = sheet.Cells[row, colReqDoc];
						Cell poDocCell = sheet.Cells[row, colPoDoc];
						string stdDoc = stdDocCell.Text;
						string reqDoc = reqDocCell.Text;
						string poDoc = poDocCell.Text;
						if (stdDoc != reqDoc || stdDoc != poDoc)
						{
							stdDocCell.ForeColor =
							reqDocCell.ForeColor =
							poDocCell.ForeColor = Color.Red;
						}
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
				}

				this.Cursor = Cursors.Default;
			}

		}
		#endregion

		#region 컨트롤 이벤트 핸들러
		/// <summary>
		/// 품목 팝업
		/// </summary>
		private void btnItemCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtItemCd.Text = Msgs[2].ToString();
					txtItemNm.Value = Msgs[3].ToString();
					txtItemCd.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

			}
		}

		//요청부서
		private void btnReqDeptCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = "usp_B_COMMON @pTYPE = 'D022', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtReqDeptCd.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00015", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "요청부서 조회");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtReqDeptCd.Text = Msgs[0].ToString();
					txtReqDeptNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

			}
		}

		//구매요청번호
		private void btnReqNo_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = "usp_MRQ499 @pTYPE = 'P1'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				string[] strWhere = new string[] { "@pCODE", "" };
				string[] strSearch = new string[] { txtReqNo.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00085", strQuery, strWhere, strSearch, new int[] { 0 }, "구매요청번호 조회");
				pu.Width = 580;
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtReqNo.Text = Msgs[0].ToString();

				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);	//데이터 조회 중 오류가 발생하였습니다.

			}
		}

		//구매요청자
		private void btnReqId_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'B011' ,@pSPEC1='" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtReqId.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00031", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "구매요청자 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtReqId.Text = Msgs[0].ToString();
					txtReqIdNm.Value = Msgs[1].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청자 팝업"), SystemBase.Base.MessageRtn("Z002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		//발주번호
		private void btnPoNo_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = "usp_M_COMMON @pTYPE = 'M070', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "" };
				string[] strSearch = new string[] { txtPoNo.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00078", strQuery, strWhere, strSearch, new int[] { 0 }, "발주번호 조회");
				pu.Width = 680;
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtPoNo.Text = Msgs[0].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//프로젝트번호
		private void btnProjectNo_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW007 pu = new WNDW007(txtProjectNo.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtProjectNo.Text = Msgs[3].ToString();
					txtProjectNm.Value = Msgs[4].ToString();
					txtProjectSeq.Text = "";
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//프로젝트차수
		private void btnProjectSeq_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
				pu.Width = 400;
				pu.ShowDialog();	//공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);
					txtProjectSeq.Text = Msgs[0].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		//품목
		private void txtItemCd_TextChanged(object sender, System.EventArgs e)
		{
			txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		//요청부서
		private void txtReqDeptCd_TextChanged(object sender, System.EventArgs e)
		{
			txtReqDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtReqDeptCd.Text, " AND REORG_ID = (SELECT REORG_ID FROM B_REORG_INFO WHERE USE_FLAG = 'Y' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "')");
		}

		//구매요청자
		private void txtReqId_TextChanged(object sender, System.EventArgs e)
		{
			txtReqIdNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtReqId.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		//프로젝트번호
		private void txtProjectNo_TextChanged(object sender, System.EventArgs e)
		{
			txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

			if (txtProjectNm.Text == "")
				txtProjectSeq.Text = "";
		}
		#endregion

	}
}
