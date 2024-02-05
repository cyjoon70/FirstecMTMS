#region 작성정보
/*********************************************************************/
// 단위업무명 : 구매요청 대비 미출고 현황
// 작 성 자 : 최용준
// 작 성 일 : 2017-06-13
// 작성내용 : 
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
using System.Text.RegularExpressions;
using WNDW;

namespace MT.MRT015
{
	public partial class MRT015 : UIForm.FPCOMM1
	{

		#region 생성자
		public MRT015()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void MRT015_Load(object sender, EventArgs e)
		{
			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			//기타 세팅	
			dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString();
			dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			dtpDeliveryDtFr.Text = "";
			dtpDeliveryDtTo.Text = "";
			dtpBasicDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
		}
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			//기타 세팅	
			dtpReqDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToShortDateString();
			dtpReqDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			dtpDeliveryDtFr.Text = "";
			dtpDeliveryDtTo.Text = "";
			dtpBasicDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			txtPlantCd.Text = SystemBase.Base.gstrPLANT_CD;
		}
		#endregion

		#region 조회조건 팝업
		//공장
		private void btnPlantCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = "usp_B_COMMON @pTYPE = 'TABLE_POP' ,@pSPEC1 = 'PLANT_CD', @pSPEC2 = 'PLANT_NM', @pSPEC3 = 'B_PLANT_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtPlantCd.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장코드 조회");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtPlantCd.Text = Msgs[0].ToString();
					txtPlantNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "공장코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);   //데이터 조회 중 오류가 발생하였습니다.

			}
		}

		//품목
		private void btnItemCd_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005(txtPlantCd.Text, true, txtItemCd.Text);
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
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

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

					txtReqDeptCd.Value = Msgs[0].ToString();
					txtReqDeptNm.Value = Msgs[1].ToString();
					txtReorgID.Value = Msgs[2].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "요청부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);   //데이터 조회 중 오류가 발생하였습니다.

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
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매요청번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

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

		//프로젝트차수 from
		private void btnProjectSeq_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";                                     // 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };            // 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { "", "" };       // 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
				pu.Width = 400;
				pu.ShowDialog();    //공통 팝업 호출

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

		//품목구분
		private void btnItemType_Click(object sender, System.EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON  @pTYPE = 'COMM_POP' ,@pSPEC1='P032', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtItemType.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00077", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목구분 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtItemType.Text = Msgs[0].ToString();
					txtItemTypeNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "구매담당자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		//프로젝트차수 to
		private void btnProjectSeqTo_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";                                     // 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };            // 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { "", "" };       // 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
				pu.Width = 400;
				pu.ShowDialog();    //공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);
					txtProjectSeqTo.Text = Msgs[0].ToString();
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

		#region 조회조건 TextChanged
		//공장
		private void txtPlantCd_TextChanged(object sender, System.EventArgs e)
		{
			txtPlantNm.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlantCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

		private void txtItemType_TextChanged(object sender, System.EventArgs e)
		{
			txtItemTypeNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtItemType.Text, " AND MAJOR_CD = 'P032' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");
		}
		#endregion

		#region SearchExec()
		protected override void SearchExec()
		{
			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
			{
				this.Cursor = Cursors.WaitCursor;

				try
				{
					string strQuery = "usp_MRT015 @pTYPE = 'S1' ";
					strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ",@pPLANT_CD = '" + txtPlantCd.Text + "'";
					strQuery += ",@pITEM_CD = '" + txtItemCd.Text + "'";
					strQuery += ",@pITEM_TYPE = '" + txtItemType.Text + "'";
					strQuery += ",@pREQ_FR_DT = '" + dtpReqDtFr.Text + "'";
					strQuery += ",@pREQ_TO_DT = '" + dtpReqDtTo.Text + "'";
					strQuery += ",@pDE_FR_DT = '" + dtpDeliveryDtFr.Text + "'";
					strQuery += ",@pDE_TO_DT = '" + dtpDeliveryDtTo.Text + "'";
					strQuery += ",@pBASIC_DT = '" + dtpBasicDt.Text + "'";
					strQuery += ",@pREQ_DEPT_CD = '" + txtReqDeptCd.Text + "'";
					//strQuery += ",@pREQ_DEPT_REORG_CD = '" + txtReorgID.Text + "'";   // 2017.06.14. hma 주석 처리 
					strQuery += ",@pREQ_USR_ID = '" + txtReqId.Text + "'";
					strQuery += ",@pPO_NO = '" + txtPoNo.Text + "'";
					strQuery += ",@pPROJECT_NO = '" + txtProjectNo.Text + "'";
					strQuery += ",@pPROJECT_SEQ_FR = '" + txtProjectSeq.Text + "'";
					strQuery += ",@pPROJECT_SEQ_TO = '" + txtProjectSeqTo.Text + "'";
					strQuery += ",@pREQ_NO = '" + txtReqNo.Text + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

					fpSpread1.ActiveSheet.FrozenColumnCount = 4;
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
				}
				finally
				{
					this.Cursor = Cursors.Default;
				}
			}

		}
		#endregion

		
	}
}
