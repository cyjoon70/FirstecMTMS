using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;

namespace SC.QA002
{
	public partial class QA002 : UIForm.FPCOMM1
	{
		#region 변수선언
		
		#endregion

		#region 생성자
		public QA002()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QA002_Load(object sender, EventArgs e)
		{
			//필수체크
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);

			//콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosINSP_TYPE, "usp_B_COMMON @pType='COMM', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCODE = 'SC130', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 3); //검사분류
			SystemBase.ComboMake.C1Combo(cmbsPROC_STATUS, "usp_SC002 @pType='C1'", 3); // 진행상태

			//그리드 콤보박스 세팅			
			G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "판정")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'Q004', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);

			//그리드초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

			SetInit();
		}
		#endregion

		#region info popup
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

		private void btnsITEM_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005("FS1", true, txtsITEM_CD.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtsITEM_CD.Text = Msgs[2].ToString();
					txtsITEM_NM.Value = Msgs[3].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void btnsPROJECT_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_M_COMMON 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";	// 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };									// 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { txtsPROJECT_NO.Text, "" };								// 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00074", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트 조회", false);
				pu.Width = 500;
				pu.ShowDialog();    //공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);

					txtsPROJECT_NO.Text = Msgs[0].ToString();
					txtsPROJECT_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void btnsEMP_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtpIN_INSPECTOR.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtpIN_INSPECTOR.Text = Msgs[0].ToString();
					txtpIN_INSPECTOR_NM.Value = Msgs[1].ToString();
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

		#region 전진검사의뢰서 호출
		private void btnReportPopup_Click(object sender, EventArgs e)
		{
			QA002P1 myForm = new QA002P1();
			myForm.ShowDialog();
		}
		#endregion

		#region 그리드 이벤트
		protected override void fpButtonClick(int Row, int Column)
		{
			string strPoNo = string.Empty;
			string strPoSeq = string.Empty;
			string strInspSeq = string.Empty;

			try
			{
				// 첨부파일
				if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "증빙_2"))
				{
					strPoNo = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Value.ToString();
					strPoSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Value.ToString();
					strInspSeq = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰순번")].Value.ToString();

					// 첨부파일 팝업 띄움.
					WNDWS01 pu = new WNDWS01(strPoNo + "/" + strPoSeq + "/" + strInspSeq, strPoNo, strPoSeq, strInspSeq, "", "", false, "", "검사의뢰");
					pu.ShowDialog();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		protected override void fpSpread1_ChangeEvent(int Row, int Column)
		{
			decimal dInspQty = 0;
			decimal dRejQty = 0;

			try
			{
				if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "검사(의뢰)수량") || Column == SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량"))
				{
					dInspQty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "검사(의뢰)수량")].Value);
					dRejQty = Convert.ToDecimal(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value);
					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "합격수량")].Value = dInspQty - dRejQty;

					if (dRejQty > 0)
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "R";
					else
						fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value = "A";
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region NewExec()
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
			dtsDAY_TO.Text = SystemBase.Base.ServerTime("YYMMDD");

			rdoINSP_ALL.Checked = true;

			//txtpIN_INSPECTOR.Text = SystemBase.Base.gstrUserID;
			//txtpIN_INSPECTOR_NM.Value = SystemBase.Base.gstrUserName;
		}
		#endregion

		#region SearchExec() 그리드 조회 로직
		protected override void SearchExec()
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
				{
					string strYn = "";
					if (rdoINSP_Y.Checked == true)
					{
						strYn = "Y";
					}
					else if (rdoINSP_N.Checked == true)
					{
						strYn = "N";
					}

					string strQuery = " usp_SC002  @pTYPE = 'S1'";
					strQuery += ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery += ", @sINSP_TYPE		= '" + cbosINSP_TYPE.SelectedValue.ToString() + "' ";
					strQuery += ", @sDAY_FR			= '" + dtsDAY_FR.Text + "' ";
					strQuery += ", @sDAY_TO			= '" + dtsDAY_TO.Text + "' ";
					strQuery += ", @sINSP_YN		= '" + strYn + "' ";
					strQuery += ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
					strQuery += ", @sITEM_CD		= '" + txtsITEM_CD.Text + "' ";
					strQuery += ", @sPROJECT_NO		= '" + txtsPROJECT_NO.Text + "' ";
					strQuery += ", @sINSP_FR		= '" + cdtInspFr.Text + "' ";
					strQuery += ", @sINSP_TO		= '" + cdtInspTo.Text + "' ";
					strQuery += ", @sIN_INSPECTOR	= '" + txtpIN_INSPECTOR.Text + "' ";
					strQuery += ", @sPROC_STATUS	= '" + cmbsPROC_STATUS.SelectedValue.ToString() + "' ";

					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);


					for(int i=0; i< fpSpread1.Sheets[0].Rows.Count; i++)
					{
						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "검사완료")
						{
							// readonly 처리
							UIForm.FPMake.grdReMake(fpSpread1, i,
								SystemBase.Base.GridHeadIndex(GHIdx1, "검사확정여부") + "|3"
								+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "검사계획일정") + "|3");
						}

						if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰 취소여부")].Text == "True")
						{
							fpSpread1.Sheets[0].Rows[i].BackColor = Color.Red;
							fpSpread1.Sheets[0].Rows[i].ForeColor = Color.White;
						}
					}
					
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

		#region SaveExec() 폼에 입력된 데이타 저장 로직
		protected override void SaveExec()
		{
			txtpIN_INSPECTOR.Focus();

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) == true)
			{
				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					this.Cursor = Cursors.WaitCursor;

					string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
					string strGbn = string.Empty;
					string MSGErr = string.Empty;

					SqlConnection dbConn = SystemBase.DbOpen.DBCON();
					SqlCommand cmd = dbConn.CreateCommand();
					SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

					try
					{
						//그리드 필수 체크
						if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
						{

							//행수만큼 처리
							for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
							{
								string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

								if (strHead.Length > 0)
								{
									switch (strHead)
									{
										case "U": strGbn = "U1"; break;
										case "I": strGbn = "I1"; break;
										case "D": strGbn = "D1"; break;
										default: strGbn = ""; break;
									}

									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상태")].Text == "검사완료"
											&& strGbn == "D1")
									{
										Trans.Rollback();
										ERRCode = "IN-ER";
										MSGErr = (i + 1).ToString() + " 행 : 검사완료된 건은 검사의뢰 취소(삭제)할 수 없습니다.";
										goto Exit;
									}

									string strSql = " usp_SCM032 @pTYPE = '" + strGbn + "' ";
									strSql += ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
									strSql += ", @pPO_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text + "' ";
									strSql += ", @pPO_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주순번")].Text + "' ";
									strSql += ", @pINS_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사의뢰순번")].Text + "' ";
									strSql += ", @pINSP_CONFIRM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사확정여부")].Text + "' ";

									if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text))
										strSql += ", @pINSP_DT  = NULL ";
									else
										strSql += ", @pINSP_DT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사일")].Text + "' ";

									if (string.IsNullOrEmpty(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사확정일")].Text))
										strSql += ", @pINSP_PLAN_DT  = NULL ";
									else
										strSql += ", @pINSP_PLAN_DT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사확정일")].Text + "' ";

									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사(의뢰)수량")].Text != "")
										strSql += ", @pINSP_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "검사(의뢰)수량")].Value + "' ";
									else
										strSql += ", @pINSP_QTY = 0.00 ";

									if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Text != "")
										strSql += ", @pREJECT_QTY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "불량수량")].Value + "' ";
									else
										strSql += ", @pREJECT_QTY = 0.00 ";

									strSql += ", @pJUDGMENT  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "판정")].Value + "' ";
									strSql += ", @pIN_INSPECTOR  = '" + txtpIN_INSPECTOR.Text + "' ";
									strSql += ", @pREMARKS  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기타")].Text.Replace("'", "''") + "' ";
									strSql += ", @pUP_ID  = '" + SystemBase.Base.gstrUserID  + "' ";
									
									DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
									ERRCode = ds.Tables[0].Rows[0][0].ToString();
									MSGCode = ds.Tables[0].Rows[0][1].ToString();

									if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프

								}
							}
						}
						else
						{
							Trans.Rollback();
							this.Cursor = Cursors.Default;
							return;
						}
						Trans.Commit();
					}
					catch (Exception e)
					{
						SystemBase.Loggers.Log(this.Name, e.ToString());
						Trans.Rollback();
						ERRCode = "ER";
						MSGCode = e.Message;
						//MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
					}
				Exit:
					dbConn.Close();
					if (ERRCode == "OK")
					{
						SearchExec();
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else if (ERRCode == "ER")
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else if (ERRCode == "IN-ER")
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGErr), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					this.Cursor = Cursors.Default;
				}
			}
		}
		#endregion

		#region TextBox text changed
		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void txtsITEM_CD_TextChanged(object sender, EventArgs e)
		{
			txtsITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtsITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void txtsPROJECT_NO_TextChanged(object sender, EventArgs e)
		{
			txtsPROJECT_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtsPROJECT_NO.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void txtpIN_INSPECTOR_TextChanged(object sender, EventArgs e)
		{
			txtpIN_INSPECTOR_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtpIN_INSPECTOR.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

	}
}
