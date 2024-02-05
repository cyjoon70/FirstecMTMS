#region 작성정보
/*********************************************************************/
// 단위업무명 : 바코드 재발행
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-09-01
// 작성내용 : 바코드 재발행
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
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using WNDW;

namespace IN.INV132
{
	public partial class INV132 : UIForm.FPCOMM1
	{

		#region 변수
		DataTable dtPrint = new DataTable();
		#endregion

		#region 생성자
		public INV132()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void INV132_Load(object sender, EventArgs e)
		{
			SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
			Control_Setting();
		}
		#endregion

		#region ControlSetting()
		private void Control_Setting()
		{

			// 프린터 포트 ComboBox 설정
			SystemBase.RawPrinterHelper.SetPortCombo(cboPort);

			// 그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

			dtPrint.Clear();

		}
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
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			string strQuery = string.Empty;

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
			{

				try
				{
					strQuery = "usp_INV132 ";
					strQuery += " @pTYPE = 'S1'";
					strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
					strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'";
					strQuery += ",@pPROJECT_NO = '" + txtProjectNo.Text + "'";
					strQuery += ",@pITEM_CD = '" + txtITEM_CD.Text + "'";
					strQuery += ",@pLOT_NO = '" + txtLot_No.Text + "'";
					strQuery += ",@pBAR_CODE = '" + txtBarCode.Text + "'";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					// 데이터 조회중 오류가 발생하였습니다.
				}
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region Grid Button Click Event
		private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
		{

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				if (cboPort.SelectedText == "선택")
				{
					MessageBox.Show("프린터 포트를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				GetPrintData(e.Row, "E");

				fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
			
		}
		#endregion

		#region 바코드 정보 조회
		private void GetPrintData(int row, string flag)
		{
			string strQuery = string.Empty;

			dtPrint.Clear();

			/*
			바코드, 출고수량, LOT NO, 품목코드, 제조오더번호  
			*/

			if (flag == "A")
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += " @pTYPE = 'P5' ";
				strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ",@pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
				strQuery += ",@pGUBUN = 'A' ";
			}
			else
			{
				strQuery = " usp_T_IN_INFO_CUDR ";
				strQuery += " @pTYPE = 'P5' ";
				strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
				strQuery += ",@pMVMT_NO = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고번호")].Text + "' ";
				strQuery += ",@pMVMT_SEQ = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "입고순번")].Text + "' ";
				strQuery += ",@pLOT_NO = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "Lot No")].Text + "' ";
				strQuery += ",@pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "' ";
				strQuery += ",@pITEM_CD = '" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "' ";
				strQuery += ",@pGUBUN = 'E' ";
			}

			dtPrint = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dtPrint.Rows.Count > 0)
			{
				print(row);
			}
			else
			{
				MessageBox.Show("검색된 데이터가 없습니다.", SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		#endregion

		#region Print
		private void print(int row)
		{

			try
			{

				string strZPL = string.Empty;

				int X = -30;
				int Y = 5;

				if (dtPrint.Rows.Count > 0)
				{

					for (int i = 0; i <= dtPrint.Rows.Count - 1; i++)
					{
						strZPL = "";

						strZPL += "^XA";					// start format

						strZPL += "^LL440";					// label hight
						strZPL += "^PW600";					// print length

						strZPL += "^LS0";					// print length
						strZPL += "^LH5,5";					// label home location - 최초 시작 위치(x, y)

						strZPL += "^SEE:UHANGUL.DAT^FS";	// 인코딩 지정, ^FS:field separator, ^FO:field origin
						strZPL += "^CWJ,E:KFONT3.FNT^FS";	// 폰트

						// FO : 인쇄할 항목의 인쇄 위치 지정(X,Y)
						// GB500(라인 길이),150(라인 높이),7(라인 두께),(라인 색상),5(라인 모서리 둥글기)
						//strZPL += "^FO50,0^GB550,140,7,,5^FS";	//라인 박스 그리기

						// BY2,2,80 - 바코드 속성 중 좁은 바 넓이를 2로 하고, 넓은 바는 좁은 바의 2배로 지정. 바코드 높이는 80 
						// BCN(문자회전 NORMAL, R:90도, I:180도, B:270도),80(바코드 높이),Y(바코드 밑에 문자인쇄 여부),N(바코드 위에 문자인쇄 여부),N(CHECK DIGIT 사용 여부) 
						strZPL += "^FO" + (X + 80) + "," + (Y + 10) + "^BY2,2.2,90^BCN,90,Y,N,N^FD" + dtPrint.Rows[i]["BAR_CODE"].ToString() + "^FS";	//^BC:Code 128(USD-6)체계

						strZPL += "^FO" + (X + 80) + "," + (Y + 140) + "^CI28^AJN,25,25^FDPrj No^FS" + "^FO" + (X + 180) + "," + (Y + 140) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["PROJECT_NO"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 170) + "^CI28^AJN,40,40^FDCode No : " + dtPrint.Rows[i]["ITEM_CD"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 220) + "^CI28^AJN,25,25^FDDesc^FS" + "^FO" + (X + 180) + "," + (Y + 220) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_NM"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 250) + "^CI28^AJN,25,25^FDPart No^FS" + "^FO" + (X + 180) + "," + (Y + 250) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["ITEM_SPEC"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 280) + "^CI28^AJN,25,25^FDRec No^FS" + "^FO" + (X + 180) + "," + (Y + 280) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["MVMT_NO"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 310) + "^CI28^AJN,25,25^FDLot No^FS" + "^FO" + (X + 180) + "," + (Y + 310) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["LOT_NO"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 340) + "^CI28^AJN,25,25^FDVendor^FS" + "^FO" + (X + 180) + "," + (Y + 340) + "^CI28^AJN,25,25^FD : " + dtPrint.Rows[i]["VENDOR"].ToString() + "^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 370) + "^CI28^AJN,25,25^FDQ'ty^FS" + "^FO" + (X + 180) + "," + (Y + 370) + "^CI28^AJN,25,25^FD : " + SetConvert(Convert.ToDecimal(dtPrint.Rows[i]["STOCK_QTY"])) + " "
																								   + dtPrint.Rows[i]["STOCK_UNIT"].ToString() + "^FS"
																								   + "^FO" + (X + 370) + "," + (Y + 370) + "^CI28^AJN,25,25^FD(" + SystemBase.Base.gstrUserName + ")^FS";
						strZPL += "^FO" + (X + 80) + "," + (Y + 400) + "^CI28^AJN,25,25^FDPrint^FS" + "^FO" + (X + 180) + "," + (Y + 400) + "^CI28^AJN,25,25^FD : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^FS";

						strZPL += "^PQ" + fpSpread1.Sheets[0].Cells[row, SystemBase.Base.GridHeadIndex(GHIdx1, "출력매수")].Text + "^FS";	// 라벨 인쇄 매수
						strZPL += "^XZ";		// end format


						if (string.Compare(cboPort.SelectedText.Substring(0, 3), "LPT", true) == 0)
						{
							if (SystemBase.RawPrinterHelper.SendStringToPrinter("LPT1", strZPL) == false)
							{
								throw new Exception("바코드 발행 중 오류가 발생했습니다.");
							}
						}
						else
						{
							if (SystemBase.RawPrinterHelper.PrintZPL(cboPort.SelectedText, strZPL) == false)
							{
								throw new Exception("바코드 발행 중 오류가 발생했습니다.");
							}
						}
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

		#region 프린터 포트 저장
		private void cboPort_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(cboPort.SelectedText) == false && cboPort.SelectedText != "선택")
				{
					SystemBase.RawPrinterHelper.SavePrinterPort(cboPort.SelectedText);
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 수량 형식 변경
		private string SetConvert(decimal dNumber)
		{
			string strReturn = string.Empty;

			strReturn = double.Parse(dNumber.ToString()).ToString();

			return strReturn;
		}
		#endregion

	}
}
