#region 작성정보
/*********************************************************************/
// 단위업무명 : 반제품 추적 조회
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-09-11
// 작성내용 : 반제품 추적 조회
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
using WNDW;

namespace PC.PCC020
{
	public partial class PCC020 : UIForm.FPCOMM2
	{

		#region 변수선언
		
		#endregion

		#region 생성자
		public PCC020()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void PCC020_Load(object sender, EventArgs e)
		{
			//공장
			SystemBase.ComboMake.C1Combo(cboPlant, "usp_B_COMMON @pTYPE='PLANT'");

			SystemBase.Validation.GroupBox_Setting(groupBox1);

			

			SetControls();
		}
		#endregion 

		#region Control Setting
		private void SetControls()
		{
			cboPlant.SelectedValue = SystemBase.Base.gstrPLANT_CD;

			txtWorkNo_FR.Text = "";
			txtWorkNo_TO.Text = "";
			txtITEM_CD.Text = "";

			//그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);
		}
		#endregion

		#region NewExec()
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SetControls();
		}
		#endregion

		#region Master 조회
		protected override void SearchExec()
		{

			try
			{
				
				this.Cursor = Cursors.WaitCursor;

				if (string.IsNullOrEmpty(txtITEM_CD.Text) && string.IsNullOrEmpty(txtWorkNo_FR.Text) && string.IsNullOrEmpty(txtWorkNo_TO.Text))
				{
					MessageBox.Show("검색 조건 중 적어도 하나 이상의 값을 입력해주세요.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				string strQuery = " usp_PCC020 ";
				strQuery += "  @pTYPE = 'S1'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pWORKORDER_NO_FR = '" + txtWorkNo_FR.Value + "'";
				strQuery += ", @pWORKORDER_NO_TO = '" + txtWorkNo_TO.Value + "'";
				strQuery += ", @pITEM_CD = '" + txtITEM_CD.Value + "'";

				UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false);

				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					SubSearch(0);
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002", SystemBase.Base.MessageRtn("Z0002")), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally 
			{
				this.Cursor = Cursors.Default;
			}

		}
		#endregion

		#region 제조오더번호 조회
		private void btnWoNoFr_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW006 pu = new WNDW006(txtWorkNo_FR.Text);
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtWorkNo_FR.Text = Msgs[1].ToString();
					txtWorkNo_FR.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnWoNoTo_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW006 pu = new WNDW006(txtWorkNo_TO.Text);
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtWorkNo_TO.Text = Msgs[1].ToString();
					txtWorkNo_TO.Focus();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Master Row Select
		private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			try
			{
				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					SubSearch(fpSpread2.Sheets[0].ActiveRowIndex);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "제조오더번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Detail 조회
		private void SubSearch(int row)
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{

				string strQuery = " usp_PCC020 ";
				strQuery += "  @pTYPE = 'S2'";
				strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ", @pPLANT_CD = '" + cboPlant.SelectedValue.ToString() + "'";
				strQuery += ", @pWORKORDER_NO_FR = '" + fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "'";
				strQuery += ", @pWORKORDER_NO_TO = '" + fpSpread2.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx2, "제조오더번호")].Text + "'";
				strQuery += ", @pITEM_CD = ''";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002", SystemBase.Base.MessageRtn("Z0002")), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = Cursors.Default;

		}
		#endregion

		#region 품목조회
		private void btnITEM_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW.WNDW001 pu = new WNDW.WNDW001();
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtITEM_CD.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtITEM_CD_TextChanged(object sender, EventArgs e)
		{
			string Query = " usp_M_COMMON @pTYPE = 'M013', @pCODE = '" + txtITEM_CD.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
			DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

			if (dt.Rows.Count > 0)
			{
				txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();
				txtITEM_SPEC.Value = dt.Rows[0]["ITEM_SPEC"].ToString();
			}
			else
			{
				txtITEM_NM.Value = "";
				txtITEM_SPEC.Value = "";
			}
		}
		#endregion

	}
}
