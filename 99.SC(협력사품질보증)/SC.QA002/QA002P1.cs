using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.IO;

namespace SC.QA002
{
	public partial class QA002P1 : UIForm.Buttons
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
			UIForm.Buttons.ReButton("100000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

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

		#region 초기화
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);

			SetInit();
		}
		#endregion

		#region 거래처 조회
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
		#endregion

		#region 엑셀양식 출력
		private void butExcel_Click(object sender, EventArgs e)
		{
			string strSheetPage1 = "전진검사의뢰서";
			string strFileName = SystemBase.Base.ProgramWhere + @"\Report\전진검사의뢰서.xls";

			try
			{
				this.Cursor = Cursors.WaitCursor;

				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
				{
					string strQuery = " usp_SCM032 @pTYPE = 'S2'";
					strQuery += ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
					strQuery += ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
					strQuery += ", @sCUST_CD	= '" + txtsCUST_CD.Text + "' ";

					DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

					if (dt != null && dt.Rows.Count > 0)
					{

						UIForm.VkExcel excel = null;

						if (File.Exists(strFileName))
						{
							File.SetAttributes(strFileName, System.IO.FileAttributes.ReadOnly);
						}
						else
						{
							// 엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다.
							MessageBox.Show("엑셀 데이터를 생성할 수 없습니다. 원본 파일이 존재하지 않습니다."); ;
							return;
						}

						excel = new UIForm.VkExcel(false);

						excel.OpenFile(strFileName);
						// 현재 시트 선택

						excel.FindExcelWorksheet(strSheetPage1);


						// 엑셀쓰기---------------------------------------------------------

						int iUseRow = 0;
						int j = 0;
						int vTotAmt = 0;

						excel.SetCell(4, 4, dt.Rows[0]["CUST_NM"].ToString());  //업체명

						for (int x = 1; x < dt.Rows.Count; x++) //행추가 및 셀병합
						{
							excel.SetAddRow("A" + (6 + x), "L" + (6 + x));
							excel.CellBorder("C" + (6 + x) + ":L" + (6 + x));
						}

						for (int i = 0; i < dt.Rows.Count; i++) //내용입력
						{
							excel.SetCell(6 + i, 3, dt.Rows[i]["ROW_NO"].ToString());       //순번
							excel.SetCell(6 + i, 4, dt.Rows[i]["PO_NO"].ToString());        //-- 발주번호
							excel.SetCell(6 + i, 5, dt.Rows[i]["ENT_NM"].ToString());       //-- 사업명
							excel.SetCell(6 + i, 6, dt.Rows[i]["ITEM_CD"].ToString());      //-- 품목코드
							excel.SetCell(6 + i, 7, dt.Rows[i]["ITEM_NM"].ToString());      //-- 품목명
							excel.SetCell(6 + i, 8, dt.Rows[i]["PO_QTY"].ToString());       //-- 수량
							excel.SetCell(6 + i, 9, dt.Rows[i]["DRAW_REV"].ToString());     //-- 도면 Rev.
							excel.SetCell(6 + i, 10, dt.Rows[i]["DELIVERY_DT"].ToString()); //-- 발주납기일
							excel.SetCell(6 + i, 11, dt.Rows[i]["INSP_REQ_DT"].ToString()); //-- 검사요청일
							excel.SetCell(6 + i, 12, dt.Rows[i]["REMARKS"].ToString());     //-- 비고	
						}

						excel.ShowExcel(true);
					}
					else
					{
						MessageBox.Show("조회된 데이터가 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전진검사의뢰서"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
			}
			this.Cursor = Cursors.Default;
		}
		#endregion
	}
}
