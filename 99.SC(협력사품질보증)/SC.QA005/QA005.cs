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
using SC.QA004;

/// <summary>
/// 품질보증계획서
/// </summary>
namespace SC.QA005
{
	public partial class QA005 : UIForm.FPCOMM1
	{

		#region 변수
		// 권한
		string strGAuth = string.Empty;
		#endregion

		#region 생성자
		public QA005()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QA005_Load(object sender, EventArgs e)
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);
			SystemBase.Validation.GroupBox_Setting(groupBox4);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC005 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC005', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC005 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC005', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);

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
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();

			txtpFST_PERSON.Value = SystemBase.Base.gstrUserID;
			txtpFST_PERSON_NM.Value = SystemBase.Base.gstrUserName;

			cbosDAY_TYPE.SelectedValue = "07";  // 요청일

			cdtpREQ_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		// 화면 모드(strGProc)에 따라 컨트롤 설정
		private void SetCondition()
		{

			btnApproval.Enabled = true;

			// 첨부파일 처리
			if (string.IsNullOrEmpty(txtpSEQ.Text))
			{
				btnFiles.Enabled = false;
				btnApproval.Enabled = false;
			}
			else
			{
				btnFiles.Enabled = true;

				if (GetExsistFile())
					btnApproval.Enabled = true;
				else
					btnApproval.Enabled = false;
			}

			// scm 등록부분 lock 처리
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

			// 승인권자 권한은 없고 담당자가 바로 승인 가능
			SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);
						
			// 승인건은 모두 lock 처리. 단, 이력관리 안하므로 반려후라도 승인 가능하도록...
			if (chkpAPPROVAL_Y.Checked || strGAuth == "S")
			{
				SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);
				SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

				btnApproval.Enabled = false;
			}

			// 컨트롤 back color 설정
			foreach (System.Windows.Forms.Control c in groupBox2.Controls)
			{
				#region 컨트롤 체크
				if (c.GetType().Name == "C1Combo")
				{
					C1.Win.C1List.C1Combo cbo = (C1.Win.C1List.C1Combo)c;

					if (!cbo.Enabled)
						cbo.EditorBackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1TextBox")
				{
					C1.Win.C1Input.C1TextBox ctb = (C1.Win.C1Input.C1TextBox)c;

					if (ctb.ReadOnly)
						ctb.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1NumericEdit")
				{
					C1.Win.C1Input.C1NumericEdit cne = (C1.Win.C1Input.C1NumericEdit)c;

					if (cne.ReadOnly)
						cne.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				else if (c.GetType().Name == "C1DateEdit")
				{
					C1.Win.C1Input.C1DateEdit cde = (C1.Win.C1Input.C1DateEdit)c;

					if (cde.ReadOnly)
						cde.BackColor = SystemBase.Validation.Kind_Gainsboro;
					
				}
				#endregion
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
			GetCustInfo(txtpCUST_CD, txtpCUST_NM);
		}

		private void txtpCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtpCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtpCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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

		#region 프로젝트 조회
		private void btnsProj_Click(object sender, EventArgs e)
		{
			GetProjInfo(txtsPROJ_NO, txtsPROJ_NM);
		}

		private void txtsPROJ_NO_TextChanged(object sender, EventArgs e)
		{
			txtsPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtsPROJ_NO.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnProj_Click(object sender, EventArgs e)
		{
			GetProjInfo(txtpPROJ_NO, txtpPROJ_NM);
		}

		private void txtpPROJ_NO_TextChanged(object sender, EventArgs e)
		{
			txtpPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtpPROJ_NO.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void GetProjInfo(C1.Win.C1Input.C1TextBox id, C1.Win.C1Input.C1TextBox name)
		{
			try
			{
				string strQuery = " usp_M_COMMON 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";   // 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };                                    // 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { id.Text, "" };                              // 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00074", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트 조회", false);
				pu.Width = 500;
				pu.ShowDialog();    //공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);

					id.Text = Msgs[0].ToString();
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

		#region 품목 조회
		private void btnItem_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005("FS1", true, txtpITEM_CD.Text);
				pu.MaximizeBox = false;
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtpITEM_CD.Text = Msgs[2].ToString();
					txtpITEM_NM.Value = Msgs[3].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtpITEM_CD_TextChanged(object sender, EventArgs e)
		{
			txtpITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtpITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 담당자, 승인자 조회
		private void btnPerson_Click(object sender, EventArgs e)
		{
			GetPerson(txtpFST_PERSON, txtpFST_PERSON_NM);
		}

		private void txtpFST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtpFST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtpFST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnAppr_Click(object sender, EventArgs e)
		{
			GetPerson(txtpFST_APPR, txtpFST_APPR_NM);
		}

		private void txtpFST_APPR_TextChanged(object sender, EventArgs e)
		{
			txtpFST_APPR_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtpFST_APPR.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
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
			UIForm.FileUpDown fileUpDown = new UIForm.FileUpDown("SC05" + txtpSEQ.Text, "N#Y#N");
			fileUpDown.ShowDialog();
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);
			SystemBase.Validation.GroupBox_Reset(groupBox3);
			SystemBase.Validation.GroupBox_Reset(groupBox4);

			SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);

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
				strQuery = " usp_SC005 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sPROJECT_NO	= '" + txtsPROJ_NO.Text + "' ";
				strQuery = strQuery + ", @sPO_NO		= '" + txtsPO_NO.Text + "' ";

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

				string strSql = " usp_SC005 @pTYPE	 = 'S2' ";
				strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @pSEQ = '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// 최초 등록
				txtpSEQ.Value = dt.Rows[0]["SEQ"].ToString();						// 일련번호
				txtpPROJ_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();				// 프로젝트번호
				txtpPROJ_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();			// 프로젝트명
				txtpBIZ_CD.Value = dt.Rows[0]["BIZ_CD"].ToString();					// 사업코드
				txtpBIZ_NM.Value = dt.Rows[0]["ENT_NM"].ToString();					// 사업명
				txtpITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();				// 품목코드
				txtpITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();				// 품목명
				txtpPO_NO.Value = dt.Rows[0]["PO_NO"].ToString();					// 발주번호
				txtpCUST_CD.Value = dt.Rows[0]["CUST_CD"].ToString();				// 업체코드
				txtpCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();				// 업체명
				cdtpREG_LIMIT_DT.Value = dt.Rows[0]["REG_LIMIT_DT"].ToString();		// 등록기한
				txtpFST_PERSON.Value = dt.Rows[0]["FST_PERSON"].ToString();         // 퍼스텍 담당자
				txtpFST_PERSON_NM.Value = dt.Rows[0]["FST_PERSON_NM"].ToString();   // 퍼스텍 담당자명
				cdtpREQ_DT.Value = dt.Rows[0]["REQ_DT"].ToString();                 // 요구일
				txtpFST_REMARKS.Value = dt.Rows[0]["FST_REMARKS"].ToString();		// 퍼스텍 담당자 의견

				// SCM 업체 등록
				cdtpREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();					// 등록일
				txtpREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();			// 등록자

				// 퍼스텍 승인권자 등록
				txtpFST_APPR.Value = dt.Rows[0]["FST_APPR"].ToString();				// 승인자
				txtpFST_APPR_NM.Value = dt.Rows[0]["FST_APPR_NM"].ToString();		// 승인자 이름
				cdtpAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();       // 처리일

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkpAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkpAPPROVAL_N.Checked = true;


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

					if (string.IsNullOrEmpty(txtpCUST_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "협력업체 코드를 확인해주세요.";
						goto Exit;
					}

					if (string.IsNullOrEmpty(txtpFST_PERSON_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "담당자 코드를 확인해주세요.";
						goto Exit;
					}

					if (!GetValidationLimDt())
					{
						Trans.Rollback();
						MSGCode = "등록기한은 요청일보다 이전일 수 없습니다.";
						goto Exit;
					}

					if (string.IsNullOrEmpty(txtpSEQ.Text))
						pType = "I1";
					else
						pType = "U1";

					string strQuery = "";
					strQuery = " usp_SC005 @pTYPE = '" + pType  + "' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pSEQ			= '" + txtpSEQ.Text + "' ";                 // 일련번호
					strQuery = strQuery + ", @pPROJ_NO		= '" + txtpPROJ_NO.Text +"' ";				// 프로젝트번호
					strQuery = strQuery + ", @pBIZ_CD		= '" + txtpBIZ_CD.Text +"' ";				// 사업코드
					strQuery = strQuery + ", @pITEM_CD		= '" + txtpITEM_CD.Text + "' ";				// 품목코드
					strQuery = strQuery + ", @pPO_NO		= '" + txtpPO_NO.Text + "' ";				// 발주번호
					strQuery = strQuery + ", @pCUST_CD		= '" + txtpCUST_CD.Text + "' ";				// 협력업체코드
					strQuery = strQuery + ", @pREG_LIMIT_DT	= '" + cdtpREG_LIMIT_DT.Text + "' ";		// 등록기한
					strQuery = strQuery + ", @pFST_PERSON	= '" + txtpFST_PERSON.Text + "' ";			// 퍼스텍 담당자
					strQuery = strQuery + ", @pREQ_DT		= '" + cdtpREQ_DT.Text + "' ";				// 요구일
					strQuery = strQuery + ", @pFST_REMARKS	= '" + txtpFST_REMARKS.Text.Replace("'", "''") + "' ";			// 퍼스텍 담당자 의견
					strQuery = strQuery + ", @pUP_ID		= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();

					if (pType == "I1")
						Seq = ds.Tables[0].Rows[0][2].ToString();
					else
						Seq = txtpSEQ.Text;

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

		private bool GetValidationLimDt()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtpREG_LIMIT_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtpREQ_DT.Text);
				DateTime dtReq = Convert.ToDateTime(cdtpREG_LIMIT_DT.Text);
				TimeSpan dateDiff = dtReq - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("등록기한은 요청일보다 이전일 수 없습니다.");
					bReturn = false;
				}
			}

			return bReturn;
		}
		#endregion

		#region 승인처리
		private void btnApproval_Click(object sender, EventArgs e)
		{
			string ERRCode = "ER", MSGCode = "", Seq = "", Appr_YN = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (!string.IsNullOrEmpty(txtpSEQ.Text) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
			{
				try
				{
					if (chkpAPPROVAL_Y.Checked)
						Appr_YN = "Y";
					else if (chkpAPPROVAL_N.Checked)
						Appr_YN = "N";

					if (!GetExsistFile())
					{
						Trans.Rollback();
						MSGCode = "업체에서 등록한 파일이 없습니다.";
						goto Exit;
					}

					if (!chkpAPPROVAL_Y.Checked && !chkpAPPROVAL_N.Checked)
					{
						Trans.Rollback();
						MSGCode = "승인 또는 반려 값에 체크해주세요.";
						goto Exit;
					}
					
					if (!GetValidationDt())
					{
						Trans.Rollback();
						MSGCode = "승인/반려 처리일은 업체 등록일 이전일 수 없습니다.";
						goto Exit;
					}

					string strQuery = "";
					strQuery = " usp_SC005 @pTYPE = 'U2' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pSEQ			= '" + txtpSEQ.Text + "' ";					// 일련번호
					strQuery = strQuery + ", @pFST_APPR		= '" + txtpFST_APPR.Text + "' ";			// 승인자
					strQuery = strQuery + ", @pAPPROVAL_YN	= '" + Appr_YN + "' ";						// 승인/반려
					strQuery = strQuery + ", @pAPPROVAL_DT	= '" + cdtpAPPROVAL_DT.Text + "' ";			// 처리일
					strQuery = strQuery + ", @pUP_ID		= '" + SystemBase.Base.gstrUserID + "' ";	// 수정자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					Seq = txtpSEQ.Text;

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

		private bool GetExsistFile()
		{
			bool bReturn = true;
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT COUNT(*) B_IMAGE(NOLOCK) WHERE FILES_NO = 'SC05' + '" + txtpSEQ.Text + "'";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null)
			{
				if (dt.Rows[0][0].ToString() == "0")
				{
					bReturn = false;
				}
			}
			else
				bReturn = false;

			return bReturn;
		}

		private bool GetValidationDt()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtpAPPROVAL_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtpREG_DT.Text);
				DateTime dtReq = Convert.ToDateTime(cdtpAPPROVAL_DT.Text);
				TimeSpan dateDiff = dtReq - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("승인/반려 처리일은 업체 등록일 이전일 수 없습니다.");
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

			DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				SqlConnection dbConn = SystemBase.DbOpen.DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

				try
				{
					string strQuery = "";
					strQuery = " usp_SC005 @pTYPE = 'D1' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pSEQ			= " + txtpSEQ.Text;

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
			try
			{
				string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtpBIZ_CD.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtpBIZ_CD.Text = Msgs[0].ToString();
					txtpBIZ_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtpBIZ_CD_TextChanged(object sender, EventArgs e)
		{
			txtpBIZ_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtpBIZ_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 발주번호 팝업
		private void btnPo_Click(object sender, EventArgs e)
		{
			try
			{
				QA004P1 pu = new QA004P1(txtpCUST_CD.Text);
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtpPO_NO.Value = Msgs[1].ToString();
					txtpCUST_CD.Value = Msgs[4].ToString();
					txtpCUST_NM.Value = Msgs[5].ToString();
					txtpPROJ_NO.Value = Msgs[6].ToString();
					txtpPROJ_NM.Value = Msgs[7].ToString();
					txtpITEM_CD.Value = Msgs[10].ToString();
					txtpITEM_NM.Value = Msgs[11].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnsPo_Click(object sender, EventArgs e)
		{
			try
			{
				QA004P1 pu = new QA004P1("");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					string[] Msgs = pu.ReturnVal;

					txtsPO_NO.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "발주정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 체크이벤트
		private void chkpAPPROVAL_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkpAPPROVAL_N.Checked)
			{
				chkpAPPROVAL_Y.Checked = false;
				SetApprValue(true);
			}
			else
			{
				SetApprValue(false);
			}
		}

		private void chkpAPPROVAL_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkpAPPROVAL_Y.Checked)
			{
				chkpAPPROVAL_N.Checked = false;
				SetApprValue(true);
			}
			else
			{
				SetApprValue(false);
			}
		}

		private void SetApprValue(bool bTF)
		{
			if (bTF)
			{
				txtpFST_APPR.Value = txtpFST_PERSON.Text;
				txtpFST_APPR_NM.Value = txtpFST_PERSON_NM.Text;
				cdtpAPPROVAL_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();
			}
			else
			{
				txtpFST_APPR.Value = "";
				txtpFST_APPR_NM.Value = "";
				cdtpAPPROVAL_DT.Value = "";
			}			
		}
		#endregion
	}
}
