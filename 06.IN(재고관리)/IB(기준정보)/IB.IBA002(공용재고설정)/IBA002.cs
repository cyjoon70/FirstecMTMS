#region 작성정보
/*********************************************************************/
// 단위업무명 : 공용 재고 설정
// 작 성 자 : 최 용 준
// 작 성 일 : 2014-08-18
// 작성내용 : 공용재고 설정/수정 및 조회
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IB.IBA002
{
	public partial class IBA002 : UIForm.FPCOMM2
	{


		#region 생성자
		public IBA002()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void IBA002_Load(object sender, EventArgs e)
		{
			//그룹박스 필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			
			SystemBase.ComboMake.C1Combo(cboGROUP_CD, "usp_B_COMMON @pType='COMM', @pCODE = 'M023', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");	// 그룹코드

			//폼 컨트롤 초기화
			Control_Setting();

			

		}
		#endregion

		#region ControlSetting()
		private void Control_Setting()
		{
			// 그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
			UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 0);

		}
		#endregion

		#region 사업코드
		private void txtEntCd_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				if (txtEntCd.Text != "")
				{
					txtEntNm.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEntCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
				}
				else
				{
					txtEntNm.Value = "";
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnEnt_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtEntCd.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtEntCd.Text = Msgs[0].ToString();
					txtEntNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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

		#region NewExec() New 버튼 클릭 이벤트
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			Control_Setting();			
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			//조회조건 필수 체크
			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
			{

				try
				{
					string strQuery = "usp_IBA002 ";
					strQuery += " @pTYPE = 'S1'";
					strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
					strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'";
					strQuery += ",@pGROUP_CD = '" + cboGROUP_CD.SelectedValue.ToString() + "'";
					strQuery += ",@pENT_CD = '" + txtEntCd.Text + "'";
					strQuery += ",@pPROJECT_NO = '" + txtProjectNo.Text + "'";

					UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, false, 0, 1);

					SubSearch();
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

		#region 기 등록된 설정 내용 조회
		private void SubSearch()
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				string strQuery = "usp_IBA002 ";
				strQuery += " @pTYPE = 'S2'";
				strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'";
				strQuery += ",@pGROUP_CD = '" + cboGROUP_CD.SelectedValue.ToString() + "'";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 4);

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region SaveExec() 폼에 입력된 데이타 저장 로직
		protected override void SaveExec()
		{
			this.Cursor = Cursors.WaitCursor;

			if ((SystemBase.Validation.FPGrid_SaveCheck_NEW(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
			{
				string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
				string strItemCd = "";

				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					//행수만큼 처리
					for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
					{
						string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
						string strGbn = "";

						if (strHead.Length > 0)
						{
							switch (strHead)
							{
								case "I": strGbn = "I1"; break;
								case "U": strGbn = "U1"; break;
								case "D": strGbn = "D1"; break;
								default: strGbn = ""; break;
							}

							string strQuery = "usp_IBA002 ";
							strQuery += " @pTYPE = '" + strGbn + "'";
							strQuery += ",@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
							strQuery += ",@pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'";
							strQuery += ",@pGROUP_CD = '" + cboGROUP_CD.SelectedValue.ToString() + "'";
							strQuery += ",@pENT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text + "'";
							strQuery += ",@pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'";
							strQuery += ",@pUSER_ID = '" + SystemBase.Base.gstrUserID + "'";

							DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
							ERRCode = ds.Tables[0].Rows[0][0].ToString();
							MSGCode = ds.Tables[0].Rows[0][1].ToString();

							if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
						}
					}

					Trans.Commit();
					SearchExec();

				}
				catch
				{
					Trans.Rollback();
					MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
				}
			Exit:
				dbConn.Close();

				if (ERRCode == "OK")
				{
					SearchExec();
					UIForm.FPMake.GridSetFocus(fpSpread1, strItemCd);
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else if (ERRCode == "ER")
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			this.Cursor = Cursors.Default;
		}
		#endregion

		#region 조회 데이터 전체 선택 / 해제
		private void fpSpread2_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{
					int HeadCnt = 0;
					if (fpSpread2.Sheets[0].ColumnHeader.RowCount > 2)
					{
						HeadCnt = 2;
					}
					else if (fpSpread2.Sheets[0].ColumnHeader.RowCount > 1)
					{
						HeadCnt = 1;
					}

					if (fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).CellType != null)
					{
						if (e.ColumnHeader == true)
						{
							if (fpSpread2.Sheets[0].ColumnHeader.Cells[HeadCnt, e.Column].Text == "True")
							{
								fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = false;
								for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
								{
									if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
									{
										fpSpread2.Sheets[0].Cells[i, e.Column].Value = false;
									}
								}
							}
							else
							{
								fpSpread2.Sheets[0].ColumnHeader.Cells.Get(HeadCnt, e.Column).Value = true;
								for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
								{
									if (fpSpread2.Sheets[0].Cells[i, e.Column].Locked == false)
									{
										fpSpread2.Sheets[0].Cells[i, e.Column].Value = true;
									}
								}
							}
						}
					}
				}
				
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region Project Add / Remove
		private void btnAdd_Click(object sender, EventArgs e)
		{
			int iCnt = 0;
			int iSameCnt = 0;
			
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				if (fpSpread2.Sheets[0].Rows.Count > 0)
				{

					for (int i = 0; i <= fpSpread2.Sheets[0].Rows.Count - 1; i++)
					{

						iSameCnt = 0;

						if (string.Compare(fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text, "true", true) == 0)
						{

							if (fpSpread1.Sheets[0].Rows.Count == 0)
								iCnt = 0;
							else
								iCnt = fpSpread1.Sheets[0].Rows.Count - 1;


							// 중복 체크
							for (int j = 0; j <= fpSpread1.Sheets[0].Rows.Count - 1;j++)
							{
								if (string.Compare(fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text,
												   fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text, true) == 0)
									iSameCnt++;
							}

							if (iSameCnt == 0)
							{
								fpSpread1.Sheets[0].Rows.Add(iCnt, 1);

								fpSpread1.Sheets[0].RowHeader.Cells[iCnt, 0].Text = "I";

								fpSpread1.Sheets[0].Cells[iCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "사업코드")].Text =
									fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "사업코드")].Text;

								fpSpread1.Sheets[0].Cells[iCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "사업명")].Text =
									fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "사업명")].Text;

								fpSpread1.Sheets[0].Cells[iCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text =
									fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트번호")].Text;

								fpSpread1.Sheets[0].Cells[iCnt, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트명")].Text =
									fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "프로젝트명")].Text;

							}

							fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "선택")].Text = "False";

						}

					}

				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{

					for (int i = fpSpread1.Sheets[0].Rows.Count - 1; i >= 0; i--)
					{

						if (string.Compare(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text, "true", true) == 0)
						{
							if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text == "1")
							{
								fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "D";
							}
							else
							{
								fpSpread1.Sheets[0].RemoveRows(i, 1);
							}
						}

					}

				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;
		}
		#endregion

		#region 기 등록 데이터 전체 선택 / 해제
		private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
		{

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

			try
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
					{
						if (e.ColumnHeader == true && e.Column == 4)
						{
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
								fpSpread1.Sheets[0].RowHeader.Rows[i].BackColor = SystemBase.Base.Color_Update;
							}
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			this.Cursor = System.Windows.Forms.Cursors.Default;

		}
		#endregion

	}
}
