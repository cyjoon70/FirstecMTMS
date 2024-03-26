using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WNDW;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

/// <summary>
/// 부적합품관리
/// </summary>
namespace SC.QA008
{
	public partial class QA008 : UIForm.FPCOMM1
	{

		#region 변수
		// 권한
		string strGAuth = string.Empty;
		#endregion

		#region 생성자
		public QA008()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load 
		private void QA008_Load(object sender, EventArgs e)
		{
			
			// 발생공정 세팅
			SystemBase.ComboMake.C1Combo(cboOCCUR_PROC, "usp_B_COMMON @pType='COMM', @pCODE = 'SC210', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 불량유형 세팅
			SystemBase.ComboMake.C1Combo(cbosDefectType, "usp_B_COMMON @pType='COMM', @pCODE = 'SC220', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboDefectType, "usp_B_COMMON @pType='COMM', @pCODE = 'SC220', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC008 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC008', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);

			GetAuth();
			SetInit();
		}

		private void GetAuth()
		{
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT dbo.ufn_GetApprovalAuth ('" + SystemBase.Base.gstrCOMCD + "', '" + SystemBase.Base.gstrUserID + "')";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null)
			{
				if (dt.Rows[0][0].ToString() == "Y")
				{
					strGAuth = "S"; // 승인권자
				}
			}
		}

		private void SetInit()
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);
			SystemBase.Validation.GroupBox_Setting(groupBox4);

			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();

			txtREG_ID.Value = SystemBase.Base.gstrUserID;
			txtREG_NM.Value = SystemBase.Base.gstrUserName;

			cdtOCCUR_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		// 화면 모드(strGProc)에 따라 컨트롤 설정
		private void SetCondition()
		{
			btnApproval.Enabled = false;

			// 검사진행 참조
			if (string.IsNullOrEmpty(txtSEQ.Text) || string.IsNullOrEmpty(txtCUST_PERSON.Text))
				btnRef.Enabled = true;
			else
				btnRef.Enabled = false;

			// 첨부파일 처리
			if (string.IsNullOrEmpty(txtSEQ.Text))
				btnFiles.Enabled = false;
			else
				btnFiles.Enabled = true;

			// scm 등록부분 lock 처리
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

			// 승인권자 권한
            if (strGAuth == "S")
                SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

            if (strGAuth == "S" && !string.IsNullOrEmpty(txtSEQ.Text) && !string.IsNullOrEmpty(txtCUST_PERSON.Text))
			{
				btnApproval.Enabled = true;
				SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);

				txtAPPROVAL_ID.Tag = "승인자;1;;";
				cdtAPPROVAL_DT.Tag = "종료일;1;;";

                cdtAPPROVAL_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

                SystemBase.Validation.GroupBox_Setting(groupBox4);
			}
			else
			{
				SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
			}

			// 승인건은 모두 lock 처리.
			if (chkAPPROVAL_Y.Checked)
			{
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
				SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

				btnRef.Enabled = false;
				btnApproval.Enabled = false;
			}

		}
		#endregion

		#region 협력업체 조회 
		private void btnsCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtsCUST_CD, txtsCUST_NM);
		}

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtCUST_CD, txtCUST_NM);
		}

		private void txtCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetCustInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				WNDW002 pu = new WNDW002(id.Text, "");
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					id.Value = Msgs[1].ToString();
					name.Value = Msgs[2].ToString();
				}

			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}
		#endregion

		#region 품목 조회
		private void btnItem_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005("FS1", true, txtITEM_CD.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtITEM_CD.Text = Msgs[2].ToString();
					txtITEM_NM.Value = Msgs[3].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtITEM_CD_TextChanged(object sender, EventArgs e)
		{
			txtITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 발행자, 승인자 조회
		private void btnsReg_Click(object sender, EventArgs e)
		{
			GetPerson(txtsREG_ID, txtsREG_NM);
		}

		private void txtsREG_ID_TextChanged(object sender, EventArgs e)
		{
			txtsREG_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtsREG_ID.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnReg_Click(object sender, EventArgs e)
		{
			GetPerson(txtREG_ID, txtREG_NM);
		}

		private void txtREG_ID_TextChanged(object sender, EventArgs e)
		{
			txtREG_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtREG_ID.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnAppr_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtAPPROVAL_ID.Text, txtAPPROVAL_NM.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtAPPROVAL_ID.Value = Msgs[0].ToString();
					txtAPPROVAL_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtAPPROVAL_ID_TextChanged(object sender, EventArgs e)
		{
			txtAPPROVAL_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtAPPROVAL_ID.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetPerson(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { id.Text, "" };

				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "검사원 팝업");
				pu.ShowDialog();

				if (pu.DialogResult == DialogResult.OK)
				{

					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					id.Value = Msgs[0].ToString();
					name.Value = Msgs[1].ToString();
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

		#region 첨부파일
		private void btnFiles_Click(object sender, EventArgs e)
		{
			WNDWS01 pu = new WNDWS01(txtSEQ.Text, txtSEQ.Text, "", "", "", "", false, "", "부적합품관리", "SCMNP");
			pu.ShowDialog();
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);
			SystemBase.Validation.GroupBox_Reset(groupBox3);
			SystemBase.Validation.GroupBox_Reset(groupBox4);

			SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
			SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
			SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);

			fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}
		#endregion

		#region 조회
		protected override void SearchExec()
		{
			SelectExec("");
		}

		private void SelectExec(string SEQ)
		{
			try
			{
				string strQuery = "";
				strQuery = " usp_SC008 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sENT_CD		= '" + txtsBIZ_CD.Text + "' ";
				strQuery = strQuery + ", @sREG_USER		= '" + txtsREG_ID.Text + "' ";
				strQuery = strQuery + ", @sDEFECT_TYPE	= '" + cbosDefectType.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";

				UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
				fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

				if (fpSpread1.Sheets[0].Rows.Count > 0)
				{
					int x = 0, y = 0;

					if (!string.IsNullOrEmpty(SEQ))
					{
						fpSpread1.Search(0, SEQ, false, false, false, false, 0, 0, ref x, ref y);

						if (x >= 0)
						{
							fpSpread1.Sheets[0].SetActiveCell(x, y);
							fpSpread1.Sheets[0].AddSelection(x, 1, 1, fpSpread1.Sheets[0].ColumnCount);

							//상세정보조회
							SubSearch(SEQ);
						}
					}
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
			}
		}

		#region 상세 정보 조회
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				try
				{
					int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
					string strSeq = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "일련번호")].Text.ToString();

					SubSearch(strSeq);
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
					//데이터 조회 중 오류가 발생하였습니다.				
				}
			}
		}

		private void SubSearch(string strNo)
		{
			this.Cursor = Cursors.WaitCursor;

			try
			{
				SystemBase.Validation.GroupBox_Setting(groupBox2);
				SystemBase.Validation.GroupBox_Setting(groupBox3);
				SystemBase.Validation.GroupBox_Setting(groupBox4);

				SystemBase.Validation.GroupBox_Reset(groupBox2);
				SystemBase.Validation.GroupBox_Reset(groupBox3);
				SystemBase.Validation.GroupBox_Reset(groupBox4);

				string strSql = " usp_SC008 @pTYPE	 = 'S2' ";
				strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @sSEQ = '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// 최초 등록
				txtSEQ.Value = dt.Rows[0]["SEQ"].ToString();						// 일련번호
				txtCUST_CD.Value = dt.Rows[0]["CUST_CD"].ToString();				// 협력사코드
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();				// 협력사명
				txtBIZ_CD.Value = dt.Rows[0]["BIZ_CD"].ToString();					// 사업코드
				txtBIZ_NM.Value = dt.Rows[0]["BIZ_NM"].ToString();					// 사업명
				txtREG_ID.Value = dt.Rows[0]["REGISTER_ID"].ToString();				// 발행자
				txtREG_NM.Value = dt.Rows[0]["REGISTER_NM"].ToString();				// 발행자명
				txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();				// 품목코드
				txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();				// 품목명
				cboOCCUR_PROC.SelectedValue = dt.Rows[0]["OCCUR_PROC"].ToString();	// 발생공정
				txtOCCUR_QTY.Value = dt.Rows[0]["OCCUR_QTY"].ToString();			// 수량
				txtCONTENTS.Value = dt.Rows[0]["CONTENTS"].ToString();				// 부적합 발생 내용
				cdtOCCUR_DT.Value = dt.Rows[0]["OCCUR_DT"].ToString();				// 발생일자
				cdtREPLY_REQ_DT.Value = dt.Rows[0]["REPLY_REQ_DT"].ToString();      // 회신요구일자
				txtPO_NO.Value = dt.Rows[0]["PO_NO"].ToString();                    // 발주번호
				txtPO_SEQ.Value = dt.Rows[0]["PO_SEQ"].ToString();                  // 발주순번
				txtINS_SEQ.Value = dt.Rows[0]["INS_SEQ"].ToString();                // 검사의뢰순번
				cboDefectType.SelectedValue = dt.Rows[0]["DEFECT_TYPE"].ToString(); // 불량유형

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;

				// SCM 등록
				txtCUST_PERSON.Value = dt.Rows[0]["CUST_PERSON"].ToString();		// 회신 작성자
                txtCUST_APPR.Value = dt.Rows[0]["CUST_APPR"].ToString();            // 업체 승인자
                txtCAUSES.Value = dt.Rows[0]["CAUSES"].ToString();					// 발생원인
				txtMEASURES.Value = dt.Rows[0]["MEASURES"].ToString();				// 대책

				// 승인처리
				txtAPPROVAL_ID.Value = dt.Rows[0]["APPROVAL_ID"].ToString();		// 검토 및 승인자
				txtAPPROVAL_NM.Value = dt.Rows[0]["APPROVAL_NM"].ToString();		// 검토 및 승인자 이름
				cdtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();		// 승인일자
				txtREMARKS.Value = dt.Rows[0]["REMARKS"].ToString();                // 비고

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;
				else
				{
					chkAPPROVAL_Y.Checked = false;
					chkAPPROVAL_N.Checked = false;
				}

				SetCondition();
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

		#endregion

		#region 저장
		protected override void SaveExec()
		{
			string ERRCode = "ER", MSGCode = "", Seq = "", pType = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
			{
				try
				{
					if (string.IsNullOrEmpty(txtCUST_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "협력업체 코드를 확인해주세요.";
						goto Exit;
					}

					if (string.IsNullOrEmpty(txtREG_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "발행자 코드를 확인해주세요.";
						goto Exit;
					}

					if (!GetValidationDt())
					{
						Trans.Rollback();
						MSGCode = "회신요구일은 발행일 이전일 수 없습니다.";
						goto Exit;
					}

					if (string.IsNullOrEmpty(txtSEQ.Text))
						pType = "I1";
					else
						pType = "U1";

					string strQuery = "";
					strQuery = " usp_SC008 @pTYPE = '" + pType + "' ";
					strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sSEQ = '" + txtSEQ.Text + "' ";									// 일련번호
					strQuery = strQuery + ", @pCUST_CD = '" + txtCUST_CD.Text + "' ";							// 협력사코드
					strQuery = strQuery + ", @pBIZ_CD = '" + txtBIZ_CD.Text + "' ";								// 사업코드
					strQuery = strQuery + ", @pREGISTER_ID = '" + txtREG_ID.Text + "' ";						// 발행자
					strQuery = strQuery + ", @pITEM_CD = '" + txtITEM_CD.Text + "' ";							// 품목코드
					strQuery = strQuery + ", @pOCCUR_PROC = '" + cboOCCUR_PROC.SelectedValue.ToString() + "' ";	// 발생공정
					strQuery = strQuery + ", @pOCCUR_QTY = " + txtOCCUR_QTY.Text + " ";							// 수량
					strQuery = strQuery + ", @pCONTENTS = '" + txtCONTENTS.Text.Replace("'", "''") + "' ";		// 부적합 발생 내용
					strQuery = strQuery + ", @pOCCUR_DT = '" + cdtOCCUR_DT.Text + "' ";							// 발생일자
					strQuery = strQuery + ", @pREPLY_REQ_DT = '" + cdtREPLY_REQ_DT.Text + "' ";					// 회신요구일자
					strQuery = strQuery + ", @pPO_NO = '" + txtPO_NO.Text + "' ";								// 발주번호
					strQuery = strQuery + ", @pPO_SEQ = '" + txtPO_SEQ.Text + "' ";								// 발주순번
					strQuery = strQuery + ", @pINS_SEQ = '" + txtINS_SEQ.Text + "' ";							// 검사의뢰순번
					strQuery = strQuery + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";                  // 수정자
					strQuery = strQuery + ", @pDEFECT_TYPE = '" + cboDefectType.SelectedValue.ToString() + "' ";// 불량유형

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (pType == "I1")
						Seq = ds.Tables[0].Rows[0][2].ToString();
					else
						Seq = txtSEQ.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
				Trans.Commit();

			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

				if (ERRCode == "OK")
					SelectExec(Seq);
			}

		}

		private bool GetValidationDt()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtREPLY_REQ_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtOCCUR_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtREPLY_REQ_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("회신요구일은 발행일 이전일 수 없습니다.");
					bReturn = false;
				}
			}

			return bReturn;
		}
		#endregion

		#region 승인처리
		private void btnApproval_Click(object sender, EventArgs e)
		{
			string ERRCode = "ER", MSGCode = "", Seq = "", pType = "", strAPPROVAL_YN = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
			{
				try
				{
					if (!chkAPPROVAL_Y.Checked && !chkAPPROVAL_N.Checked)
					{
						Trans.Rollback();
						MSGCode = "승인 또는 반려 값에 체크해주세요.";
						goto Exit;
					}

					if (string.IsNullOrEmpty(txtAPPROVAL_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "승인자 코드를 확인해주세요.";
						goto Exit;
					}

					if (!GetValidationFinDt())
					{
						Trans.Rollback();
						MSGCode = "종료일은 발행일 이전일 수 없습니다.";
						goto Exit;
					}

					string strQuery = "";
					strQuery = " usp_SC008 @pTYPE = 'U2' ";
					strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sSEQ = '" + txtSEQ.Text + "' ";						// 일련번호
					strQuery = strQuery + ", @pAPPROVAL_ID = '" + txtAPPROVAL_ID.Text + "' ";		// 협력사코드
					strQuery = strQuery + ", @pAPPROVAL_DT = '" + cdtAPPROVAL_DT.Text + "' ";		// 사업코드
					strQuery = strQuery + ", @pREMARKS = '" + txtREMARKS.Text.Replace("'", "''") + "' ";				// 발행자
					strQuery = strQuery + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";      // 수정자

					if (chkAPPROVAL_Y.Checked)
						strAPPROVAL_YN = "Y";
					else if (chkAPPROVAL_N.Checked)
						strAPPROVAL_YN = "N";

					strQuery = strQuery + ", @pAPPROVAL_YN	= '" + strAPPROVAL_YN + "' ";       // 승인여부   

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					Seq = txtSEQ.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
				Trans.Commit();

			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

				if (ERRCode == "OK")
					SelectExec(Seq);
			}
		}

		private bool GetValidationFinDt()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtAPPROVAL_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtOCCUR_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtAPPROVAL_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("종료일은 발행일보다 이전일 수 없습니다.");
					bReturn = false;
				}
			}

			return bReturn;
		}
		#endregion

		#region 삭제
		protected override void DeleteExec()
		{
			string ERRCode = "", MSGCode = "";

			if (String.IsNullOrEmpty(txtSEQ.Text)) return;

			DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					string strQuery = "";
					strQuery = " usp_SC008 @pTYPE = 'D1' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @sSEQ			= " + txtSEQ.Text;

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
				Trans.Commit();

			Exit:
				dbConn.Close();
				MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));

				if (ERRCode == "OK")
				{
					SystemBase.Validation.GroupBox_Reset(groupBox2);
					SystemBase.Validation.GroupBox_Reset(groupBox3);
					SystemBase.Validation.GroupBox_Reset(groupBox4);

					SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
					SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

					SelectExec("");
				}

			}
		}
		#endregion

		#region 사업 조회
		private void btnBIZ_Click(object sender, EventArgs e)
		{
			GetBizInfo(txtBIZ_CD, txtBIZ_NM);
		}

		private void txtBIZ_CD_TextChanged(object sender, EventArgs e)
		{
			txtBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtBIZ_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnsBiz_Click(object sender, EventArgs e)
		{
			GetBizInfo(txtsBIZ_CD, txtsBIZ_NM);
		}

		private void txtsBIZ_CD_TextChanged(object sender, EventArgs e)
		{
			txtsBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtsBIZ_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetBizInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { id.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					id.Text = Msgs[0].ToString();
					name.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 검사진행 참조
		private void btnRef_Click(object sender, EventArgs e)
		{
			try
			{
				QA008P1 frm1 = new QA008P1();
				frm1.WindowState = FormWindowState.Normal;
				frm1.ShowDialog();

				if (frm1.DialogResult == DialogResult.OK)
				{
					string[] Msgs = frm1.ReturnVal;

					txtCUST_CD.Value = Msgs[19];
					txtCUST_NM.Value = Msgs[18];
					txtBIZ_CD.Value = Msgs[7];
					txtBIZ_NM.Value = Msgs[8];
					txtITEM_CD.Value = Msgs[9];
					txtITEM_NM.Value = Msgs[10];
					cboOCCUR_PROC.SelectedValue = "02"; // 검사진행 참조시 발생공정은 전진으로 고정
					txtOCCUR_QTY.Value = Msgs[17];

					txtPO_NO.Value = Msgs[1];
					txtPO_SEQ.Value = Msgs[2];
					txtINS_SEQ.Value = Msgs[3];
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 승인여부 체크박스
		private void chkAPPROVAL_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_Y.Checked)
				chkAPPROVAL_N.Checked = false;
		}

		private void chkAPPROVAL_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_N.Checked)
				chkAPPROVAL_Y.Checked = false;
		}
		#endregion

	}
}
