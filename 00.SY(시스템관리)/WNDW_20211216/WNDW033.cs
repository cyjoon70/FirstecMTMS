#region 작성정보
/*********************************************************************/
// 단위업무명 : 품질증빙 확인 popup
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-07-23
// 작성내용 : 품질증빙 문서를 조회하는 공통 화면
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

namespace WNDW
{
	public partial class WNDW033 : UIForm.FPCOMM1
	{

		#region 변수
		public string strKEY_NO = string.Empty;
		public string strKEY_SEQ = string.Empty;
		public string strREQ_TYPE = string.Empty;
		public string strDOC_TYPE = string.Empty;
		public string strFormGubn = string.Empty;
		#endregion

		#region 생성자
		public WNDW033()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void WNDW033_Load(object sender, EventArgs e)
		{

			SystemBase.Validation.GroupBox_Setting(groupBox1);

			UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SystemBase.ComboMake.C1Combo(cboPLANT, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);	//공장코드
			SystemBase.ComboMake.C1Combo(cboReqOrigin, "usp_B_COMMON @pType='COMM', @pCODE = 'TD010', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// 필수증빙 근거
			SystemBase.ComboMake.C1Combo(cboDocOrigin, "usp_B_COMMON @pType='COMM', @pCODE = 'TD004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// 문서첨부 근거

			cboPLANT.SelectedValue = SystemBase.Base.gstrPLANT_CD.ToString();
			cboReqOrigin.SelectedValue = strREQ_TYPE;
			cboDocOrigin.SelectedValue = strDOC_TYPE;
			txtKEY_NO.Value = strKEY_NO;

			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			SearchExec();

		}
		#endregion

		#region SearchExec() 조회
		protected override void SearchExec()
		{

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
				{
					string strQuery = " usp_WNDW033 ";
					strQuery += " @pTYPE = 'S1' ";
					strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
					strQuery += ",@pKEY_TYPE = '" + strREQ_TYPE + "' ";
					strQuery += ",@pATT_CTG  = '" + strDOC_TYPE + "' ";
					strQuery += ",@pATT_KEY1 = '" + strKEY_NO + "' ";
					strQuery += ",@pATT_KEY2 = '" + strKEY_SEQ + "' ";
					strQuery += ",@pATT_KEY3 = '' ";
					strQuery += ",@pATT_KEY4 = '' ";
					strQuery += ",@pATT_KEY5 = ''";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8);

					SetGridHeader();
					SetGridRowColor();


				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region 그리드 헤더 조정
		private void SetGridHeader()
		{
			try
			{
				c1Label2.Text = "발주번호";

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					if (string.Compare(strFormGubn, "MIM001", true) == 0)	// 구매입고등록 => 발주 연결
					{
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "발주번호";
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "발주순번";
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "품목코드";
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = "품목명";
						fpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "규격";

					}
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

		#region 그리드 라인 색상 조정
		private void SetGridRowColor()
		{
			// 기준번호1 + 기준번호2
			string strCurVal = string.Empty;	
			string strPreVal = string.Empty;
			int iColor = 0;

			try
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
					{

						strCurVal = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기준번호2")].Value.ToString();

						if (string.Compare(strPreVal, strCurVal, true) != 0)
						{
							if (iColor == 0)
								iColor = 1;
							else if (iColor == 1)
								iColor = 0;
						}

						if (iColor == 0)
							fpSpread1.Sheets[0].Rows[i].BackColor = Color.White;
						else
							fpSpread1.Sheets[0].Rows[i].BackColor = Color.WhiteSmoke;

						strPreVal = strCurVal;
					}
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

	}
}
