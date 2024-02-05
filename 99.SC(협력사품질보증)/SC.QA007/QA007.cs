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

namespace SC.QA007
{
	public partial class QA007 : UIForm.FPCOMM1
	{

		#region 변수
		// 승인 권한
		string strGAuth = string.Empty;
		#endregion

		#region 생성자
		public QA007()
		{
			InitializeComponent();
		}
		#endregion

		#region Form Load
		private void QA007_Load(object sender, EventArgs e)
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);

			// 4M1E 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboFM, "usp_B_COMMON @pType='COMM', @pCODE = 'SC190', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC007 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC007 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);

			GetAuth();
			SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			cbosDAY_TYPE.SelectedValue = "10";  // 신고일

			cdtREC_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		private void SetCondition()
		{
			// scm 등록부분 lock 처리
			SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

			SetGroupbox3("0");

			if (string.IsNullOrEmpty(txtMGT_NO.Text) || chkAPPROVAL_Y.Checked)
			{
				SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
			}
			else
			{
				SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
				SetGroupbox3("1");
			}

			SystemBase.Validation.GroupBox_Setting(groupBox3);
		}

		private void SetGroupbox3(string flag)
		{
			// 첨부파일 처리
			if (string.IsNullOrEmpty(txtMGT_NO.Text))
				btnFiles.Enabled = false;
			else
				btnFiles.Enabled = true;

			if (flag == "1")    // 승인 영역 필수값 처리
			{
				txtQA_DEPT.Tag = "품질부서장;1;;";
				txtQA_DEPT_NM.Tag = ";2;;";
				cdtREC_DT.Tag = "접수일;1;;";
				chkAPPROVAL_Y.Tag = "";
				chkAPPROVAL_N.Tag = "";
				cdtAPPROVAL_DT.Tag = "처리일;1;;";
				txtUserId.Tag = "파일승인자;1;;";
				txtUserNm.Tag = ";2;;";
				cdtAPPR_DT.Tag = ";2;;";
				panel3.BackColor = Color.LightSkyBlue;

				btnQA_DEPT.Tag = "";
				btnUser.Tag = "";
			}
			else    // 승인 영역 Readonly
			{
				txtQA_DEPT.Tag = ";2;;";
				txtQA_DEPT_NM.Tag = ";2;;";
				cdtREC_DT.Tag = ";2;;";
				chkAPPROVAL_Y.Tag = ";2;;";
				chkAPPROVAL_N.Tag = ";2;;";
				cdtAPPROVAL_DT.Tag = ";2;;";
				txtUserId.Tag = ";2;;";
				txtUserNm.Tag = ";2;;";
				cdtAPPR_DT.Tag = ";2;;";
				panel3.BackColor = SystemBase.Validation.Kind_Gainsboro;

				btnQA_DEPT.Tag = ";2;;";
				btnUser.Tag = ";2;;";
			}
		}

		// 승인자 권한 체크
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
		#endregion

		#region 첨부파일
		private void btnFiles_Click(object sender, EventArgs e)
		{
			string strCRUD = string.Empty;

			if (txtMGT_STATUS.Text == "승인")
				strCRUD = "N#Y#N";
			else
				strCRUD = "Y#Y#Y";

			UIForm.FileUpDown fileUpDown = new UIForm.FileUpDown(txtMGT_NO.Text, strCRUD);
			fileUpDown.ShowDialog();
		}
		#endregion

		#region New
		protected override void NewExec()
		{
			SystemBase.Validation.GroupBox_Reset(groupBox1);
			SystemBase.Validation.GroupBox_Reset(groupBox2);
			SystemBase.Validation.GroupBox_Reset(groupBox3);

			SystemBase.Validation.GroupBoxControlsLock(groupBox2, false);
			SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);

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
				strQuery = " usp_SC007 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sENT_CD		= '" + txtsENT_CD.Text + "' ";
				strQuery = strQuery + ", @sITEM_CD		= '" + txtsITEM_CD.Text + "' ";
				strQuery = strQuery + ", @sPROJECT_NO	= '" + txtPROJ_NO.Text + "' ";
				strQuery = strQuery + ", @sFM			= '" + cboFM.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sMGT_NO		= '" + txtsMGT_NO.Text + "' ";

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
					string strSeq = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "신청번호")].Text.ToString();

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
				SystemBase.Validation.GroupBox_Reset(groupBox2);
				SystemBase.Validation.GroupBox_Reset(groupBox3);

				string strSql = " usp_SC007 @pTYPE		= 'S2' ";
				strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @sMGT_NO			= '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// SCM 등록
				txtMGT_NO.Value = dt.Rows[0]["MGT_NO"].ToString();				//관리번호
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();			//협력업체코드
				txtENT_NM.Value = dt.Rows[0]["ENT_NM"].ToString();				//사업명
				txtPROJECT_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();         //프로젝트번호
				txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();		//프로젝트명
				txtITEM_CD.Value = dt.Rows[0]["ITEM_CD"].ToString();            //품목코드
				txtITEM_NM.Value = dt.Rows[0]["ITEM_NM"].ToString();            //품목명
				txtREG_TYPE.Value = dt.Rows[0]["REG_TYPE"].ToString();			//등록구분
				txtMGT_STATUS.Value = dt.Rows[0]["MGT_STATUS"].ToString();		//상태
				txtFM.Value = dt.Rows[0]["FM"].ToString();						//4M
				txtDEC_MSG.Value = dt.Rows[0]["DEC_MSG"].ToString();			//신고내용
				txtREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();		//등록자
				txtREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();				//등록일
				txtREMARKS.Value = dt.Rows[0]["REMARKS"].ToString();            //비고

				// 퍼스텍 등록

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")				// 승인결과
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;

				cdtREC_DT.Value = dt.Rows[0]["REC_DT"].ToString();              //접수일
				cdtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();    //승인일
				txtAPPROVAL_MSG.Value = dt.Rows[0]["APPROVAL_MSG"].ToString();  //승인의견
				txtQA_DEPT.Value = dt.Rows[0]["QA_DEPT"].ToString();            //품질부서장
				txtQA_DEPT_NM.Value = dt.Rows[0]["QA_DEPT_NM"].ToString();      //품질부서장 이름

				// 첨부파일 승인
				txtUserId.Value = dt.Rows[0]["FILE_APPR"].ToString();			// 승인자
				txtUserNm.Value = dt.Rows[0]["FILE_APPR_NM"].ToString();		// 승인자명
				cdtAPPR_DT.Value = dt.Rows[0]["APPR_DT"].ToString();			// 승인일

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
			string ERRCode = "ER", MSGCode = "", Seq = "", APPROVAL_YN = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (!string.IsNullOrEmpty(txtMGT_NO.Text) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3) && GetValidationDt())
			{

				if (chkAPPROVAL_Y.Checked)
					APPROVAL_YN = "Y";
				else if (chkAPPROVAL_N.Checked)
					APPROVAL_YN = "N";

				try
				{
					if (!string.IsNullOrEmpty(txtQA_DEPT.Text) && string.IsNullOrEmpty(txtQA_DEPT_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "품질부서장 코드를 확인해주세요.";
						goto Exit;
					}

					if (!string.IsNullOrEmpty(txtUserId.Text) && string.IsNullOrEmpty(txtUserNm.Text))
					{
						Trans.Rollback();
						MSGCode = "파일승인자 코드를 확인해주세요.";
						goto Exit;
					}


					if (!chkAPPROVAL_Y.Checked && !chkAPPROVAL_N.Checked)
					{
						Trans.Rollback();
						MSGCode = "승인 또는 반려 값에 체크해주세요.";
						goto Exit;
					}

					string strQuery = "";
					strQuery = " usp_SC007 @pTYPE = 'U1' ";
					strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";	// 법인코드
					strQuery = strQuery + ", @pAPPROVAL_YN	= '" + APPROVAL_YN + "' ";					//승인결과
					strQuery = strQuery + ", @pREC_DT		= '" + cdtREC_DT.Text + "' ";				//접수일
					strQuery = strQuery + ", @pAPPROVAL_DT	= '" + cdtAPPROVAL_DT.Text + "' ";			//승인일
					strQuery = strQuery + ", @pAPPROVAL_MSG	= '" + txtAPPROVAL_MSG.Text.Replace("'", "''") + "' ";			//승인의견
					strQuery = strQuery + ", @pQA_DEPT		= '" + txtQA_DEPT.Text + "' ";				//품질부서장
					strQuery = strQuery + ", @sMGT_NO		= '" + txtMGT_NO.Text + "' ";				// 신청번호
					strQuery = strQuery + ", @pUP_ID		= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자
					strQuery = strQuery + ", @pFILE_APPR	= '" + txtUserId.Text + "' ";				// 첨부파일 승인자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					Seq = txtMGT_NO.Text;

					if (ERRCode == "ER")
					{
						Trans.Rollback();
						goto Exit;  // ER 코드 Return시 점프
					}

					Trans.Commit();
				}
				catch (Exception ex)
				{
					Trans.Rollback();
					MessageBox.Show(ex.ToString());
					MSGCode = "P0001";
					goto Exit;  // ER 코드 Return시 점프
				}
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
			bool bTestDt = true;
			bool bEstDt = true;

			if (!string.IsNullOrEmpty(cdtREC_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(txtREG_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtREC_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("접수일은 등록일 이전일 수 없습니다.");
					bTestDt = false;
				}
			}

			if (!string.IsNullOrEmpty(cdtAPPROVAL_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtREC_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtAPPROVAL_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("처리일은 접수일 이전일 수 없습니다.");
					bEstDt = false;
				}
			}

			if (bTestDt && bEstDt)
				bReturn = true;
			else
				bReturn = false;

			return bReturn;
		}
		#endregion

		#region 품질부서장 조회
		private void btnQA_DEPT_Click(object sender, EventArgs e)
		{
			GetPerson(txtQA_DEPT, txtQA_DEPT_NM);
		}

		private void txtQA_DEPT_TextChanged(object sender, EventArgs e)
		{
			txtQA_DEPT_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtQA_DEPT.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
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

		#region 협력업체 조회 
		private void btnsCust_Click(object sender, EventArgs e)
		{
			GetCustInfo(txtsCUST_CD, txtsCUST_NM);
		}

		private void txtsCUST_CD_TextChanged(object sender, EventArgs e)
		{
			txtsCUST_NM.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtsCUST_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
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
		private void btnPROJ_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_M_COMMON 'P001', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";   // 쿼리
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };                                    // 쿼리 인자값(조회조건)
				string[] strSearch = new string[] { txtPROJ_NO.Text, "" };									// 쿼리 인자값에 들어갈 데이타

				//UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00074", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트 조회", false);
				pu.Width = 500;
				pu.ShowDialog();    //공통 팝업 호출

				if (pu.DialogResult == DialogResult.OK)
				{
					string MSG = pu.ReturnVal.Replace("|", "#");
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(MSG);

					txtPROJ_NO.Text = Msgs[0].ToString();
					txtPROJ_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				//데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtPROJ_NO_TextChanged(object sender, EventArgs e)
		{
			txtPROJ_NM.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtPROJ_NO.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 사업 조회
		private void btnENT_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtsENT_CD.Text, "" };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtsENT_CD.Text = Msgs[0].ToString();
					txtsENT_NM.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void txtsENT_CD_TextChanged(object sender, EventArgs e)
		{
			txtsENT_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtsENT_CD.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 품목조회
		private void btnITEM_Click(object sender, EventArgs e)
		{
			try
			{
				WNDW005 pu = new WNDW005("FS1", true, txtITEM_CD.Text);
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

		private void txtsITEM_CD_TextChanged(object sender, EventArgs e)
		{
			txtsITEM_NM.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtsITEM_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 체크박스 라디오 이벤트
		private void chkAPPROVAL_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_Y.Checked && chkAPPROVAL_N.Checked)
				chkAPPROVAL_N.Checked = false;
		}

		private void chkAPPROVAL_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_N.Checked && chkAPPROVAL_Y.Checked)
				chkAPPROVAL_Y.Checked = false;
		}
		#endregion

		#region 첨부파일 승인자
		private void btnUser_Click(object sender, System.EventArgs e)   //사용자
		{
			try
			{
				string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtUserId.Text, txtUserNm.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtUserId.Value = Msgs[0].ToString();
					txtUserNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtUserId_TextChanged(object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtUserId.Text))
			{
				txtUserNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtUserId.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
			}
			else
			{
				txtUserNm.Value = "";
			}
		}
		#endregion

	}
}
