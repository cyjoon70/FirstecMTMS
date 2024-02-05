#region 작성정보
/*********************************************************************/
// 단위업무명 : LOT 재고이동등록
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-08-25
// 작성내용 : LOT 재고이동등록 및 관리
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
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;

namespace IT.ITR003
{
	public partial class ITR003P2 : UIForm.FPCOMM1
	{

		#region 변수
		public bool bReadOnly = true;
		public bool bSave = true;
		public string strCO_CD = string.Empty;
		public string strPLANT_CD = string.Empty;
		public string strITEM_CD = string.Empty;
		public string strPROJECT_NO = string.Empty;
		public string strLOT_NO = string.Empty;
		public string[] ReturnVal = null;
		#endregion

		#region 생성자
		public ITR003P2()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void ITR003P2_Load(object sender, EventArgs e)
		{
			this.Text = "재고 이동";

			UIForm.Buttons.ReButton("110000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			Control_Setting();
			SearchExec();
		}
		#endregion

		#region ControlSetting()
		private void Control_Setting()
		{

			//txtITEM_CD.Enabled = bReadOnly;
			//txtProjectNo.Enabled = bReadOnly;
			//txtITEM_NM.Enabled = bReadOnly;
			//txtProjectNm.Enabled = bReadOnly;
			//btnITEM.Enabled = bReadOnly;
			//btnProject.Enabled = bReadOnly;

			txtITEM_CD.Value = strITEM_CD;
			txtProjectNo.Value = strPROJECT_NO;

			// 그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

		}
		#endregion

		#region 사업코드
		
		#endregion

		#region 프로젝트 조회
		private void btnProject_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW.WNDW007 pu = new WNDW.WNDW007(txtProjectNo.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtProjectNo.Text = Msgs[3].ToString();
					txtProjectNm.Value = Msgs[4].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtProjectNo_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if (txtProjectNo.Text != "")
				{
					txtProjectNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjectNo.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
					if (txtProjectNm.Value.ToString() == "")
					{
					}
				}
				else
				{
					txtProjectNm.Value = "";
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 품목 조회
		private void btnITEM_Click(object sender, System.EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005();
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtITEM_CD.Text = Msgs[2].ToString();
					txtITEM_NM.Value = Msgs[3].ToString();
					txtITEM_CD.Focus();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtITEM_CD_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (txtITEM_CD.Text != "")
				{
					txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
				}
				else
				{
					txtITEM_NM.Value = "";
				}
			}
			catch
			{

			}
		}
		#endregion

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			Control_Setting();
			SearchExec();
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			bSave = true;

			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
			{

				try
				{
					string strQuery = "usp_ITR003 ";
					strQuery += " @pTYPE = 'P2'";
					strQuery += ",@pCO_CD = '" + strCO_CD + "'";
					strQuery += ",@pPLANT_CD = '" + strPLANT_CD + "'";
					strQuery += ",@pLOT_NO = '" + strLOT_NO + "'";
					strQuery += ",@pITEM_CD = '" + strITEM_CD + "'";
					strQuery += ",@pPROJECT_NO = '" + strPROJECT_NO + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
				}
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region SaveExec() 폼에 입력된 데이타 저장 로직
		protected override void SaveExec()
		{
			if ((SystemBase.Validation.FPGrid_SaveCheck_NEW(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
			{
				if (bSave)
				{
					RtnStr(fpSpread1.Sheets[0].ActiveRowIndex);

					for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
					{
						fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
					}

					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else
				{
					bSave = true;
				}
			}
		}
		#endregion

		#region 값 전송
		public void RtnStr(int R)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				ReturnVal = new string[fpSpread1.Sheets[0].Columns.Count];

				for (int i = 0; i < fpSpread1.Sheets[0].Columns.Count; i++)
				{
					ReturnVal[i] = fpSpread1.Sheets[0].Cells[R, i].Value.ToString();
				}
			}
		}
		#endregion

		#region 수량 유효성 체크
		private void fpSpread1_EditModeOff(object sender, EventArgs e)
		{

			try
			{
				if (fpSpread1.Sheets[0].ActiveColumnIndex == SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량"))
				{
					if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Text) == false &&
						string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Text) == false)
					{
						if (Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value)
							- Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value) < 0)
						{
							string msg = "이동수량이 ";
							msg += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value).ToString("###,###,###,###,##0.0000");
							msg += " 보다 크면 안됩니다!";

							MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
							fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;

							//fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value =
							//    Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value);

							bSave = false;
						}

					}
				}

				// 이동수량은 한번에 하나의 Lot만 가능
				for (int i = 0; i <= fpSpread1.Sheets[0].Rows.Count - 1; i++)
				{
					if (i != fpSpread1.Sheets[0].ActiveRowIndex)
					{
						fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이동수량")].Value = 0;
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion

	}
}
