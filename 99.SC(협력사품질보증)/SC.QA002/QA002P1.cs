using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;

namespace SC.QA002
{
	public partial class QA002P1 : UIForm.FPCOMM1
	{
		#region 생성자
		public QA002P1()
		{
			InitializeComponent();
			
		}
		#endregion

		#region Form Load
		private void QA002P1_Load(object sender, EventArgs e)
		{
			UIForm.Buttons.ReButton("110000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

			SetInit();

			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
			dtsDAY_TO.Text = SystemBase.Base.ServerTime("YYMMDD");
		}
		#endregion

		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
				{
					string strQuery = " usp_SC002  @pTYPE = 'S2'";
					strQuery += ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
					strQuery += ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
					strQuery += ", @sCUST_CD	= '" + txtsCUST_CD.Text + "' ";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
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

		private void btnsCUST_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW002 pu = new WNDW002("P");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtsCUST_CD.Text = Msgs[1].ToString();
					txtsCUST_NM.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
	}
}
