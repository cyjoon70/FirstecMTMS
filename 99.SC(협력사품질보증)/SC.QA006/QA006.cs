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
using System.IO;

namespace SC.QA006
{
	public partial class QA006 : UIForm.FPCOMM1
	{

        #region 변수
        string strGAuth = string.Empty; // 원래는 승인권자 변수였으나, 특수공정은 승인권자가 없으므로 선행 저장 체크 변수로 사용
		#endregion

		#region 생성자
		public QA006()
		{
			InitializeComponent();
		}

		#endregion

		#region Form Load
		private void QA006_Load(object sender, EventArgs e)
		{
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);
			SystemBase.Validation.GroupBox_Setting(groupBox3);

			// 공정 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboPROCESS, "usp_B_COMMON @pType='COMM', @pCODE = 'SC170', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboPROC_CD, "usp_B_COMMON @pType='COMM', @pCODE = 'SC170', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

			// 날짜유형 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC006 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

			// 진행상태 콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC006 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);
			SystemBase.ComboMake.C1Combo(cboAPP_STATUS, "usp_SC006 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC006', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);

            SystemBase.ComboMake.C1Combo(cboEST_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC180', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);
            

            SetInit();
		}

		private void SetInit()
		{
			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
			dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			cbosDAY_TYPE.SelectedValue = "08";  // 접수일

			cdtREC_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

			SetCondition();
		}

		private void SetCondition()
		{
			
			// scm 등록부분 lock 처리
			SystemBase.Validation.GroupBoxControlsLock(groupBox2, true);

			SetGroupbox3("0");
			SetGroupbox4("0");

			// 전체 승인 lock 처리
			if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text) || chkEST_RESULT_Y.Checked)
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);
            }
            else
            {
                if (chkEST_TECH_RESULT_Y.Checked) // 기술검토 승인 체크
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, false);

                    cdtAPPROVAL_DT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString();

					SetGroupbox4("1");
				}
				else
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

					SetGroupbox3("1");
				}
            }

			SystemBase.Validation.GroupBox_Setting(groupBox3);
			SystemBase.Validation.GroupBox_Setting(groupBox4);

		}

		private void SetGroupbox3(string flag)
		{
			if (flag == "1")    // 기술검토 승인 영역 필수값 처리
			{
				txtREC_PERSON.Tag = "접수자;1;;";
				txtREC_PERSON_NM.Tag = ";2;;";
				cdtREC_DT.Tag = "접수일;1;;";
				txtEST_PERSON.Tag = "평가자;1;;";
				txtEST_PERSON_NM.Tag = ";2;;";
				cdtEST_PLAN_DT.Tag = "평가예정일;1;;";
				chkEST_TECH_RESULT_Y.Tag = "";
				chkEST_TECH_RESULT_N.Tag = "";
				txtTEST_PERSON.Tag = "검토자;1;;";
				txtTEST_PERSON_NM.Tag = ";2;;";
				cdtTEST_DT.Tag = "검토일;1;;";
				txtEST_TECH_MSG.Tag = "기술검토의견;1;;";
				panel3.BackColor = Color.LightSkyBlue;

				btnREC_PERSON.Tag = "";
				btnEST_PERSON.Tag = "";
				btnTEST_PERSON.Tag = "";
			}
			else    // 기술검토 승인 영역 Readonly
			{
				txtREC_PERSON.Tag = ";2;;";
				txtREC_PERSON_NM.Tag = ";2;;";
				cdtREC_DT.Tag = ";2;;";
				txtEST_PERSON.Tag = ";2;;";
				txtEST_PERSON_NM.Tag = ";2;;";
				cdtEST_PLAN_DT.Tag = ";2;;";
				chkEST_TECH_RESULT_Y.Tag = ";2;;";
				chkEST_TECH_RESULT_N.Tag = ";2;;";
				txtTEST_PERSON.Tag = ";2;;";
				txtTEST_PERSON_NM.Tag = ";2;;";
				cdtTEST_DT.Tag = ";2;;";
				txtEST_TECH_MSG.Tag = ";2;;";
				panel3.BackColor = SystemBase.Validation.Kind_Gainsboro;

				btnREC_PERSON.Tag = ";2;;";
				btnEST_PERSON.Tag = ";2;;";
				btnTEST_PERSON.Tag = ";2;;";
			}
		}

		private void SetGroupbox4(string flag)
		{
			// 첨부파일 처리
			if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text))
				btnFiles.Enabled = false;
			else
				btnFiles.Enabled = true;

			if (string.IsNullOrEmpty(txtAPPLICATION_NO.Text) || chkEST_RESULT_Y.Checked || flag == "0")
			{
				btnAppr.Enabled = false;
			}
			else
			{
				btnAppr.Enabled = true;
			}

			if (flag == "1")    // 평가결과 승인 영역 필수값 처리
			{
				panel2.BackColor = Color.LightSkyBlue;
				chkEST_RESULT_Y.Tag = "";
				chkEST_RESULT_N.Tag = "";
				cdtAPPROVAL_DT.Tag = "승인/반려일;1;;";
				cdtAVAILABLE_DT.Tag = "유효일;1;;";
				txtAPPROVAL_GUBUN.Tag = "승인구분;1;;";
				txtAPPROVAL_RANGE.Tag = "승인범위;1;;";

				txtADD_INFO.Tag = "";
				txtUserId.Tag = "파일승인자;1;;";
				txtUserNm.Tag = ";2;;";
				cdtAPPR_DT.Tag = ";2;;";

				btnUser.Tag = "";
			}
			else    // 평가결과 승인 영역 Readonly
			{
				panel2.BackColor = SystemBase.Validation.Kind_Gainsboro;
				chkEST_RESULT_Y.Tag = ";2;;";
				chkEST_RESULT_N.Tag = ";2;;";
				cdtAPPROVAL_DT.Tag = ";2;;";
				cdtAVAILABLE_DT.Tag = ";2;;";
				txtAPPROVAL_GUBUN.Tag = ";2;;";
				txtAPPROVAL_RANGE.Tag = ";2;;";

				txtADD_INFO.Tag = ";2;;";
				txtUserId.Tag = ";2;;";
				txtUserNm.Tag = ";2;;";
				cdtAPPR_DT.Tag = ";2;;";

				btnUser.Tag = ";2;;";
			}
		}

		// 승인자 권한 체크 - 사용 안함. 혹시 몰라서 남겨둠
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

		#region 접수자, 평가자, 검토자 조회
		private void btnREC_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtREC_PERSON, txtREC_PERSON_NM);
		}

		private void txtREC_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtREC_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtREC_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnEST_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtEST_PERSON, txtEST_PERSON_NM);
		}

		private void txtEST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtEST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtEST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void btnTEST_PERSON_Click(object sender, EventArgs e)
		{
			GetPerson(txtTEST_PERSON, txtTEST_PERSON_NM);
		}

		private void txtTEST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtTEST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtTEST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
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
			string strCRUD = string.Empty;

			if (cboAPP_STATUS.Text == "승인")
				strCRUD = "N#Y#N";
			else
			{
				if (chkEST_TECH_RESULT_Y.Checked)
					strCRUD = "Y#Y#Y";
				else
				{
					if (!string.IsNullOrEmpty(cdtAPPR_DT.Text))
						strCRUD = "N#Y#N";
					else
						strCRUD = "Y#Y#Y";
				}
			}					   		

			UIForm.FileUpDown fileUpDown = new UIForm.FileUpDown(txtAPPLICATION_NO.Text, strCRUD);
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
				strQuery = " usp_SC006 @pTYPE = 'S1' ";
				strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strQuery = strQuery + ", @sDAY_TYPE			= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sDAY_FR			= '" + dtsDAY_FR.Text + "' ";
				strQuery = strQuery + ", @sDAY_TO			= '" + dtsDAY_TO.Text + "' ";
				strQuery = strQuery + ", @sSTATUS			= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sPROCESS			= '" + cboPROCESS.SelectedValue.ToString() + "' ";
				strQuery = strQuery + ", @sCUST_CD			= '" + txtsCUST_CD.Text + "' ";
				strQuery = strQuery + ", @sAPPLICATION_NO	= '" + txtsAPPLICATION_NO.Text + "' ";

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
                SystemBase.Validation.GroupBox_Setting(groupBox2);
                SystemBase.Validation.GroupBox_Setting(groupBox3);
                SystemBase.Validation.GroupBox_Setting(groupBox4);

                SystemBase.Validation.GroupBox_Reset(groupBox2);
				SystemBase.Validation.GroupBox_Reset(groupBox3);
                SystemBase.Validation.GroupBox_Reset(groupBox4);

                string strSql = " usp_SC006 @pTYPE		= 'S2' ";
				strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @sAPPLICATION_NO	= '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// SCM 등록
				txtAPPLICATION_NO.Value = dt.Rows[0]["APPLICATION_NO"].ToString();      // 신청번호
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();					// 협력업체코드
				txtPROJECT_NO.Value = dt.Rows[0]["PROJ_NO"].ToString();                 // 프로젝트번호
				txtPROJECT_NM.Value = dt.Rows[0]["PROJECT_NM"].ToString();				// 프로젝트명
				txtAPP_PERSON.Value = dt.Rows[0]["APP_PERSON"].ToString();				// 신청자
				txtAPP_DT.Value = dt.Rows[0]["APP_DT"].ToString();                      // 신청일
				cboPROC_CD.SelectedValue = dt.Rows[0]["PROC_CD"].ToString();			// 공정
				cboEST_TYPE.SelectedValue = dt.Rows[0]["EST_TYPE"].ToString();			// 평가구분
				txtSPEC_NO.Value = dt.Rows[0]["SPEC_NO"].ToString();					// 규격번호
				cboAPP_STATUS.SelectedValue = dt.Rows[0]["APP_STATUS"].ToString();		// 상태
				txtACT_CUST_NM.Value = dt.Rows[0]["ACT_CUST_NM"].ToString();			// 수행업체-업체명
				txtACT_CUST_PERSON.Value = dt.Rows[0]["ACT_CUST_PERSON"].ToString();	// 수행업체-담당자
				txtACT_CUST_CONN.Value = dt.Rows[0]["ACT_CUST_CONN"].ToString();		// 수행업체-연락처
				txtACT_CUST_ADDR.Value = dt.Rows[0]["ACT_CUST_ADDR"].ToString();		// 수행업체-주소

				// 퍼스텍 등록
				txtREC_PERSON.Value = dt.Rows[0]["REC_PERSON"].ToString();              // 접수자
				txtREC_PERSON_NM.Value = dt.Rows[0]["REC_PERSON_NM"].ToString();		// 접수자명
				cdtREC_DT.Value = dt.Rows[0]["REC_DT"].ToString();						// 접수일
				txtEST_PERSON.Value = dt.Rows[0]["EST_PERSON"].ToString();              // 평가자
				txtEST_PERSON_NM.Value = dt.Rows[0]["EST_PERSON_NM"].ToString();		// 평가자명
				cdtEST_PLAN_DT.Value = dt.Rows[0]["EST_PLAN_DT"].ToString();            // 평가예정일

				if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "Y")                     // 기술검토결과
					chkEST_TECH_RESULT_Y.Checked = true;
				else if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "N")
					chkEST_TECH_RESULT_N.Checked = true;


				txtTEST_PERSON.Value = dt.Rows[0]["TEST_PERSON"].ToString();            // 검토자
				txtTEST_PERSON_NM.Value = dt.Rows[0]["TEST_PERSON_NM"].ToString();		// 검토자명
				cdtTEST_DT.Value = dt.Rows[0]["TEST_DT"].ToString();					// 검토일
				txtEST_TECH_MSG.Value = dt.Rows[0]["EST_TECH_MSG"].ToString();			// 기술검토의견


				if (dt.Rows[0]["EST_RESULT"].ToString() == "Y")                         // 평가결과
					chkEST_RESULT_Y.Checked = true;
				else if (dt.Rows[0]["EST_RESULT"].ToString() == "N")
					chkEST_RESULT_N.Checked = true;

				cdtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();			// 승인일
				cdtAVAILABLE_DT.Value = dt.Rows[0]["AVAILABLE_DT"].ToString();			// 유효일
				txtADD_INFO.Value = dt.Rows[0]["ADD_INFO"].ToString();					// 부가정보
				txtAPPROVAL_GUBUN.Value = dt.Rows[0]["APPROVAL_GUBUN"].ToString();		// 승인구분
				txtAPPROVAL_RANGE.Value = dt.Rows[0]["APPROVAL_RANGE"].ToString();		// 승인범위

				// 첨부파일 승인
				txtUserId.Value = dt.Rows[0]["FILE_APPR"].ToString();					// 승인자
				txtUserNm.Value = dt.Rows[0]["FILE_APPR_NM"].ToString();				// 승인자명
				cdtAPPR_DT.Value = dt.Rows[0]["APPR_DT"].ToString();					// 승인일

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
			string ERRCode = "ER", MSGCode = "", Seq = "", EST_TECH_RESULT = "", EST_RESULT = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (!string.IsNullOrEmpty(txtAPPLICATION_NO.Text) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3) && GetValidationDt())
			{

				if (chkEST_TECH_RESULT_Y.Checked)
					EST_TECH_RESULT = "Y";
				else if (chkEST_TECH_RESULT_N.Checked)
					EST_TECH_RESULT = "N";

				if (chkEST_RESULT_Y.Checked)
					EST_RESULT = "Y";
				else if (chkEST_RESULT_N.Checked)
					EST_RESULT = "N";

				try
				{
					if (!string.IsNullOrEmpty(txtREC_PERSON.Text) && string.IsNullOrEmpty(txtREC_PERSON_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "접수자를 입력해주세요.";
						goto Exit;
					}

					if (!string.IsNullOrEmpty(txtEST_PERSON.Text) && string.IsNullOrEmpty(txtEST_PERSON_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "평가자를 입력해주세요.";
						goto Exit;
					}

					if (!string.IsNullOrEmpty(txtTEST_PERSON.Text) && string.IsNullOrEmpty(txtTEST_PERSON_NM.Text))
					{
						Trans.Rollback();
						MSGCode = "검토자를 입력해주세요.";
						goto Exit;
					}

					if (!chkEST_TECH_RESULT_Y.Checked && !chkEST_TECH_RESULT_N.Checked)
					{
						Trans.Rollback();
						MSGCode = "승인 또는 반려 값에 체크해주세요.";
						goto Exit;
					}

					string strQuery = "";
					strQuery = " usp_SC006 @pTYPE = 'U1' ";
					strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD	+ "' ";		// 법인코드
					strQuery = strQuery + ", @pREC_PERSON		= '" + txtREC_PERSON.Text			+ "' ";     // 접수자
					strQuery = strQuery + ", @pREC_DT			= '" + cdtREC_DT.Text				+ "' ";		// 접수일
					strQuery = strQuery + ", @pEST_PERSON		= '" + txtEST_PERSON.Text			+ "' ";		// 평가자
					strQuery = strQuery + ", @pEST_PLAN_DT		= '" + cdtEST_PLAN_DT.Text			+ "' ";		// 평가예정일
					strQuery = strQuery + ", @pEST_TECH_RESULT	= '" + EST_TECH_RESULT				+ "' ";		// 기술검토결과
					strQuery = strQuery + ", @pTEST_PERSON		= '" + txtTEST_PERSON.Text			+ "' ";		// 검토자
					strQuery = strQuery + ", @pTEST_DT			= '" + cdtTEST_DT.Text				+ "' ";		// 검토일
					strQuery = strQuery + ", @pEST_TECH_MSG		= '" + txtEST_TECH_MSG.Text.Replace("'", "''") + "' ";		// 기술검토의견
					strQuery = strQuery + ", @pEST_RESULT		= '" + EST_RESULT					+ "' ";		// 평가결과
					strQuery = strQuery + ", @pAPPROVAL_DT		= '" + cdtAPPROVAL_DT.Text			+ "' ";		// 승인일
					strQuery = strQuery + ", @pAVAILABLE_DT		= '" + cdtAVAILABLE_DT.Text			+ "' ";		// 유효일
					strQuery = strQuery + ", @pADD_INFO			= '" + txtADD_INFO.Text.Replace("'", "''") + "' ";		// 부가정보
					strQuery = strQuery + ", @pAPPROVAL_GUBUN	= '" + txtAPPROVAL_GUBUN.Text.Replace("'", "''") + "' ";		// 승인구분
					strQuery = strQuery + ", @pAPPROVAL_RANGE	= '" + txtAPPROVAL_RANGE.Text.Replace("'", "''") + "' ";		// 승인범위
					strQuery = strQuery + ", @sAPPLICATION_NO	= '" + txtAPPLICATION_NO.Text		+ "' ";		// 신청번호
					strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID	+ "' ";     // 수정자
					strQuery = strQuery + ", @pFILE_APPR		= '" + txtUserId.Text				+ "' ";		// 첨부파일 승인자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					Seq = txtAPPLICATION_NO.Text;

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
			bool bRecDt = true;

			if (!string.IsNullOrEmpty(cdtREC_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(txtAPP_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtREC_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("접수일은 신청일 이전일 수 없습니다.");
					bRecDt = false;
				}
			}

			if (!string.IsNullOrEmpty(cdtTEST_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtREC_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtTEST_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("검토일은 접수일 이전일 수 없습니다.");
					bTestDt = false;
				}
			}

			if (!string.IsNullOrEmpty(cdtEST_PLAN_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtREC_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtEST_PLAN_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("평가예정일은 접수일 이전일 수 없습니다.");
					bEstDt = false;
				}
			}

			if (bTestDt && bEstDt && bRecDt)
				bReturn = true;
			else
				bReturn = false;

			return bReturn;
        }
        #endregion

        #region 화면 출력
        protected override void PrintExec()
		{
			if (txtAPPLICATION_NO.Text != "")
			{
				string strSheetPage1 = "특수공정";

				string strFileName = SystemBase.Base.ProgramWhere + @"\Report\특수공정.xls";

				try
				{
					this.Cursor = Cursors.WaitCursor;

					string strSql = " usp_SC006 @pTYPE		= 'R1' ";
					strSql = strSql + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
					strSql = strSql + ", @sAPPLICATION_NO	= '" + txtAPPLICATION_NO.Text + "' ";

					DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

					if (dt.Rows.Count > 0)
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

						excel.SetCell(2, 2, dt.Rows[0]["APPLICATION_NO"].ToString());   //	신청번호
						excel.SetCell(2, 4, dt.Rows[0]["CUST_NM"].ToString());          //	협력업체명
						excel.SetCell(2, 6, dt.Rows[0]["PROJ_NO"].ToString());          //	프로젝트번호
						excel.SetCell(2, 8, dt.Rows[0]["PROJECT_NM"].ToString());       //	프로젝트명
						excel.SetCell(3, 2, dt.Rows[0]["APP_PERSON"].ToString());       //	신청자
						excel.SetCell(3, 4, dt.Rows[0]["APP_DT"].ToString());           //	신청일
						excel.SetCell(3, 6, dt.Rows[0]["PROC_CD"].ToString());          //	공정
						excel.SetCell(3, 8, dt.Rows[0]["EST_TYPE"].ToString());         //	평가구분
						excel.SetCell(4, 2, dt.Rows[0]["SPEC_NO"].ToString());          //	규격번호
						excel.SetCell(6, 2, dt.Rows[0]["ACT_CUST_NM"].ToString());      //	수행업체-업체명
						excel.SetCell(6, 4, dt.Rows[0]["ACT_CUST_PERSON"].ToString());  //	수행업체-담당자
						excel.SetCell(6, 6, dt.Rows[0]["ACT_CUST_CONN"].ToString());    //	수행업체-연락처
						excel.SetCell(7, 2, dt.Rows[0]["ACT_CUST_ADDR"].ToString());    //	수행업체-주소
						excel.SetCell(9, 2, dt.Rows[0]["REC_PERSON_NM"].ToString());   //	접수자명
						excel.SetCell(9, 4, dt.Rows[0]["REC_DT"].ToString());          //	접수일
						excel.SetCell(9, 6, dt.Rows[0]["EST_PERSON_NM"].ToString());   //	평가자명
						excel.SetCell(9, 8, dt.Rows[0]["EST_PLAN_DT"].ToString());     //	평가예정일

						if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "Y")            //	기술검토결과
							excel.SetCell(10, 2, "승인"); 
						else if (dt.Rows[0]["EST_TECH_RESULT"].ToString() == "N")
							excel.SetCell(10, 2, "반려");

						excel.SetCell(10, 4, dt.Rows[0]["TEST_PERSON_NM"].ToString());  //	검토자명
						excel.SetCell(10, 6, dt.Rows[0]["TEST_DT"].ToString());         //	검토일
						excel.SetCell(11, 2, dt.Rows[0]["EST_TECH_MSG"].ToString());    //	기술검토의견

						if (dt.Rows[0]["EST_RESULT"].ToString() == "Y")					//	평가결과
							excel.SetCell(12, 2, "승인");
						else if (dt.Rows[0]["EST_RESULT"].ToString() == "N")
							excel.SetCell(10, 2, "반려");

						excel.SetCell(12, 4, dt.Rows[0]["APPROVAL_DT"].ToString());     //	승인일
						excel.SetCell(12, 6, dt.Rows[0]["AVAILABLE_DT"].ToString());    //	유효일
						excel.SetCell(13, 2, dt.Rows[0]["ADD_INFO"].ToString());        //	부가정보
						excel.SetCell(14, 2, dt.Rows[0]["APPROVAL_GUBUN"].ToString());  //	승인구분
						excel.SetCell(15, 2, dt.Rows[0]["APPROVAL_RANGE"].ToString());  //	승인범위

						excel.ShowExcel(true);
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "특수공정 출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
				}
				this.Cursor = Cursors.Default;

			}
			else
			{
				MessageBox.Show("출력할 신청번호를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
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

        #region 체크박스 이벤트
        private void chkEST_TECH_RESULT_Y_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEST_TECH_RESULT_Y.Checked)
                chkEST_TECH_RESULT_N.Checked = false;
        }

        private void chkEST_TECH_RESULT_N_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEST_TECH_RESULT_N.Checked)
                chkEST_TECH_RESULT_Y.Checked = false;
        }

        private void chkEST_RESULT_Y_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEST_RESULT_Y.Checked)
                chkEST_RESULT_N.Checked = false;
        }

        private void chkEST_RESULT_N_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEST_RESULT_N.Checked)
                chkEST_RESULT_Y.Checked = false;
        }
        #endregion

        #region 최종 승인/반려
        private void btnAppr_Click(object sender, EventArgs e)
        {
            if (GetValidationApprDt() && GetValidationLimitDt())
            {
                ProcApproval();
            }
        }

        private void ProcApproval()
        {
            string ERRCode = "ER", MSGCode = "", Seq = "", EST_RESULT = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
            {
                try
                {
                    if (chkEST_RESULT_Y.Checked)
                        EST_RESULT = "Y";
                    else if (chkEST_RESULT_N.Checked)
                        EST_RESULT = "N";
					
					if (string.IsNullOrEmpty(txtUserId.Text) || string.IsNullOrEmpty(txtUserNm.Text))
					{
						Trans.Rollback();
						MSGCode = "파일 승인자를 입력해주세요.";
						goto Exit;
					}

					if (!chkEST_RESULT_Y.Checked && !chkEST_RESULT_N.Checked)
                    {
                        Trans.Rollback();
                        MSGCode = "승인 또는 반려 값에 체크해주세요.";
                        goto Exit;
                    }

                    if (chkEST_TECH_RESULT_Y.Checked == false)
                    {
                        Trans.Rollback();
                        MSGCode = "기술검토 승인처리가 되어 있지 않습니다.";
                        goto Exit;
                    }

                    string strQuery = "";
                    strQuery = " usp_SC006 @pTYPE = 'U2' ";
                    strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";    // 법인코드
                    strQuery = strQuery + ", @pEST_RESULT		= '" + EST_RESULT + "' ";                   // 평가결과
                    strQuery = strQuery + ", @pAPPROVAL_DT		= '" + cdtAPPROVAL_DT.Text + "' ";          // 승인일
                    strQuery = strQuery + ", @pAVAILABLE_DT		= '" + cdtAVAILABLE_DT.Text + "' ";         // 유효일
                    strQuery = strQuery + ", @pADD_INFO			= '" + txtADD_INFO.Text.Replace("'", "''") + "' ";          // 부가정보
                    strQuery = strQuery + ", @pAPPROVAL_GUBUN	= '" + txtAPPROVAL_GUBUN.Text.Replace("'", "''") + "' ";    // 승인구분
                    strQuery = strQuery + ", @pAPPROVAL_RANGE	= '" + txtAPPROVAL_RANGE.Text.Replace("'", "''") + "' ";    // 승인범위
                    strQuery = strQuery + ", @sAPPLICATION_NO	= '" + txtAPPLICATION_NO.Text + "' ";       // 신청번호
                    strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자
					strQuery = strQuery + ", @pFILE_APPR		= '" + txtUserId.Text + "' ";				// 첨부파일 승인자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();
                    Seq = txtAPPLICATION_NO.Text;

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

        private bool GetValidationApprDt()
        {
            bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtAPPROVAL_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtTEST_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtAPPROVAL_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("승인/반려일은 검토일 이전일 수 없습니다.");
					bReturn = false;
				}
			}

            return bReturn;
        }

        private bool GetValidationLimitDt()
        {
            bool bReturn = true;

			if (!string.IsNullOrEmpty(cdtAVAILABLE_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(cdtTEST_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(cdtAVAILABLE_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("유효일은 검토일 이전일 수 없습니다.");
					bReturn = false;
				}
			}
			
            return bReturn;
        }
        #endregion
    }
}
