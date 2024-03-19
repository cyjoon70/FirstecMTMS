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

/// <summary>
/// 시정조치
/// </summary>
namespace SC.QA003
{
    public partial class QA003 : UIForm.FPCOMM1
    {

		#region 변수
		// 승인 권한
		string strGAuth = string.Empty;

		// 파일 임시저장을 위한 number
		string strRan = string.Empty;
		#endregion

		#region 생성자
		public QA003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 
        private void QA003_Load(object sender, EventArgs e)
        {
			SystemBase.Validation.GroupBox_Setting(groupBox1);
			SystemBase.Validation.GroupBox_Setting(groupBox2);

            // 발행유형 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosACTION_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC100', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboACTION_TYPE, "usp_B_COMMON @pType='COMM', @pCODE = 'SC100', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

            // 날짜유형 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosDAY_TYPE, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC110', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'");

            // 진행상태 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cbosSTATUS, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboCORR_STATUS, "usp_SC003 @pType='C1', @pMAJOR_CD = 'SC120', @pREL_CD1 = 'SC003', @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "'", 9);

			GetAuth();
			SetInit();
			

		}

        private void SetInit()
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);
            SystemBase.Validation.GroupBox_Setting(groupBox5);
            SystemBase.Validation.GroupBox_Setting(groupBox6);

			strRan = Regex.Replace(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), @"[^0-9a-zA-Z가-힣]", "");

			txtREG_DEPT.Value = SystemBase.Base.gstrDEPTNM;
			txtREG_PERSON_NM.Value = SystemBase.Base.gstrUserName;
			txtREG_PERSON.Value = SystemBase.Base.gstrUserID;

			dtsDAY_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtsDAY_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(1).ToShortDateString();
            dtREG_DT.Value = SystemBase.Base.ServerTime("YYMMDD");

            // scm 등록부분 lock 처리
            SystemBase.Validation.GroupBoxControlsLock(groupBox4, true);

            SetCondition(true);
		}

		// 조건에 따라 LOCK 처리
		private void SetCondition(bool bOnLoad)
		{
						
			SetGroupbox5("0");
			SetGroupbox6("0");
			SetGroupbox7("0");

			if (chkFST_APPROVAL_Y.Checked)  // 최종 승인
            {
                SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);
                SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
                SystemBase.Validation.GroupBoxControlsLock(groupBox6, true);
				SystemBase.Validation.GroupBoxControlsLock(groupBox7, true);
			}
            else
            {
                if (strGAuth == "S")    // 최종 승인권자
                {
                    SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                    SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
					SystemBase.Validation.GroupBoxControlsLock(groupBox7, true);
					SystemBase.Validation.GroupBoxControlsLock(groupBox6, false);

					if (!bOnLoad && chkAPPROVAL_Y.Checked) SetGroupbox6("1");
				}
                else
                {
                    if (bOnLoad)
                    {
                        SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
                        SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
                        SystemBase.Validation.GroupBoxControlsLock(groupBox6, true);
						SystemBase.Validation.GroupBoxControlsLock(groupBox7, true);
					}
                    else
                    {
                        if (string.IsNullOrEmpty(dtCUST_REG_DT.Text))
						{
							SystemBase.Validation.GroupBoxControlsLock(groupBox3, false);
							SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
							SystemBase.Validation.GroupBoxControlsLock(groupBox7, true);
						}
						else
						{
							SystemBase.Validation.GroupBoxControlsLock(groupBox3, true);

							
							if (chkAPPROVAL_Y.Checked)
							{
								SystemBase.Validation.GroupBoxControlsLock(groupBox5, true);
								SetGroupbox5("0");
							}
							else
							{
								SystemBase.Validation.GroupBoxControlsLock(groupBox5, false);
								SetGroupbox5("1");
							}

							//SystemBase.Validation.GroupBoxControlsLock(groupBox5, false);
							//SetGroupbox5("1");

							if (chkAPPROVAL_Y.Checked)
							{
								SetGroupbox7("1");
							}
							else
							{
								SetGroupbox7("0");
							}

						}

						SystemBase.Validation.GroupBoxControlsLock(groupBox6, true);

					}
                    
                }
            }

			SystemBase.Validation.GroupBox_Setting(groupBox5);
			SystemBase.Validation.GroupBox_Setting(groupBox6);
			SystemBase.Validation.GroupBox_Setting(groupBox7);

		}

		private void SetGroupbox5(string flag)
		{
			if (flag == "1")	// 퍼스텍 담당자 승인 영역 필수값 처리
			{
				txtFST_PERSON.Tag = "퍼스텍담당자;1;;";
				dtAPPROVAL_DT.Tag = "처리일;1;;";
				txtCORR_RESULT.Tag = "시정조치 확인평가;1;;";
				chkAPPROVAL_Y.Tag = "";
				chkAPPROVAL_N.Tag = "";
				txtFST_PERSON_NM.Tag = ";2;;";
				btnAppr1.Tag = "";
				btnFST_PERSON.Tag = "";
				panel2.BackColor = Color.LightSkyBlue;
			}
			else    // 퍼스텍 담당자 승인 영역 Readonly
			{
				txtFST_PERSON.Tag = ";2;;";
				dtAPPROVAL_DT.Tag = ";2;;";
				txtCORR_RESULT.Tag = ";2;;";
				chkAPPROVAL_Y.Tag = ";2;;";
				chkAPPROVAL_N.Tag = ";2;;";
				txtFST_PERSON_NM.Tag = ";2;;";
				btnAppr1.Tag = ";2;;";
				btnFST_PERSON.Tag = ";2;;";
				panel2.BackColor = SystemBase.Validation.Kind_Gainsboro;

				// ------------------------------------------------------------------------------------
				// 입력시 엔터키 문제로, 내용 수정을 위하여 상태를 바꿈. 반드시 배포시에는 주석 처리
				// ------------------------------------------------------------------------------------
				//txtCORR_RESULT.Tag = "시정조치 확인평가;1;;";
				//btnAppr1.Tag = "";
				// ------------------------------------------------------------------------------------
			}

		}

		private void SetGroupbox6(string flag)
		{
			if (flag == "1")    // 퍼스텍 승인자 승인 영역 필수값 처리
			{
				txtFST_APPROVAL.Tag = "퍼스텍승인자;1;;";
				txtFST_APPROVAL_NM.Tag = ";2;;";
				dtFST_APPR_DT.Tag = "처리일;1;;";
				txtFINAL_REMARKS.Tag = "";
				chkFST_APPROVAL_Y.Tag = "";
				chkFST_APPROVAL_N.Tag = "";
				btnAppr2.Tag = "";
				btnFST_APPROVAL.Tag = "";
				panel4.BackColor = Color.LightSkyBlue;
			}
			else    // 퍼스텍 승인자 승인 영역 Readonly
			{
				txtFST_APPROVAL.Tag = ";2;;";
				txtFST_APPROVAL_NM.Tag = ";2;;";
				dtFST_APPR_DT.Tag = ";2;;";
				txtFINAL_REMARKS.Tag = ";2;;";
				chkFST_APPROVAL_Y.Tag = ";2;;";
				chkFST_APPROVAL_N.Tag = ";2;;";
				btnAppr2.Tag = ";2;;";
				btnFST_APPROVAL.Tag = ";2;;";
				panel4.BackColor = SystemBase.Validation.Kind_Gainsboro;
			}
		}

		private void SetGroupbox7(string flag)
		{
			if (flag == "1")    //효과성 확인 영역 필수값 처리
			{
				dtCORR_EFFECTS_DT.Tag = "효과성확인일;1;;";
				txtCORR_EFFECTS.Tag = "효과성확인;1;;";
				btnConfirmDt.Tag = "";
			}
			else    // 효광성 확인 영역 Readonly
			{
				txtFST_APPROVAL.Tag = ";2;;";
				txtCORR_EFFECTS.Tag = ";2;;";
				dtCORR_EFFECTS_DT.Tag = ";2;;";
				btnConfirmDt.Tag = ";2;;";
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

		// 부서장 사번 조회
		private string GetEmpNo(string cd)
		{
			string strReturn = string.Empty;
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT TOP 1 REL_CD2 FROM B_COMM_CODE WHERE COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' AND MAJOR_CD = 'Q005' AND MINOR_CD = '" + cd + "'";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null)
			{
				strReturn = dt.Rows[0][0].ToString();
			}

			return strReturn;
		}
		#endregion

		#region New
		protected override void NewExec()
        {
			SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);
            SystemBase.Validation.GroupBox_Reset(groupBox5);
            SystemBase.Validation.GroupBox_Reset(groupBox6);

            fpSpread1.Sheets[0].Rows.Count = 0;

			SetInit();
		}
        #endregion

        #region 협력사 조회
        private void btnSCust_Click(object sender, EventArgs e)
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

        #region 조회

		// 리스트 조회
        protected override void SearchExec()
        {
            SelectExec("");
        }
        
        private void SelectExec(string CORR_NO)
        {
            try
            {
                string strQuery = "";
                strQuery = " usp_SC003 @pTYPE = 'S1' ";
                strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @sACTION_TYPE	= '" + cbosACTION_TYPE.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sDAY_TYPE		= '" + cbosDAY_TYPE.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sDAY_FR		= '" + dtsDAY_FR.Text + "' ";
                strQuery = strQuery + ", @sDAY_TO		= '" + dtsDAY_TO.Text + "' ";
                strQuery = strQuery + ", @sSTATUS		= '" + cbosSTATUS.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @sCUST_CD		= '" + txtsCUST_CD.Text +"' ";
                strQuery = strQuery + ", @sTITLE		= '" + txtsTITLE.Text +"' ";
                strQuery = strQuery + ", @sCORR_NO		= '" + txtsCORR_NO.Text +"' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (!string.IsNullOrEmpty(CORR_NO))
                    {
                        fpSpread1.Search(0, CORR_NO, false, false, false, false, 0, 0, ref x, ref y);

                        if (x >= 0)
                        {
                            fpSpread1.Sheets[0].SetActiveCell(x, y);
                            fpSpread1.Sheets[0].AddSelection(x, 1, 1, fpSpread1.Sheets[0].ColumnCount);

                            //상세정보조회
                            SubSearch(CORR_NO);
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

		// 상세 정보 조회
		private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
		{
			if (fpSpread1.Sheets[0].Rows.Count > 0)
			{
				try
				{
					int intRow = fpSpread1.Sheets[0].GetSelection(0).Row;
					string strCorrNo = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "시정조치번호")].Text.ToString();

					SubSearch(strCorrNo);
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

				string strSql = " usp_SC003 @pTYPE	 = 'S2' ";
				strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
				strSql = strSql + ", @pCORR_NO = '" + strNo + "' ";

				DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

				// 최초 등록
				txtCORR_NO.Value = dt.Rows[0]["CORR_NO"].ToString();
				cboACTION_TYPE.SelectedValue = dt.Rows[0]["ACTION_TYPE"].ToString();
				txtREG_DEPT.Value = dt.Rows[0]["REG_DEPT"].ToString();
				txtREG_PERSON.Value = dt.Rows[0]["REG_PERSON"].ToString();
				txtREG_PERSON_NM.Value = dt.Rows[0]["REG_PERSON_NM"].ToString();
				dtREG_DT.Value = dt.Rows[0]["REG_DT"].ToString();
				dtCOMP_REQ_DT.Value = dt.Rows[0]["COMP_REQ_DT"].ToString();
				txtTITLE.Value = dt.Rows[0]["TITLE"].ToString();
				cboCORR_STATUS.SelectedValue = dt.Rows[0]["CORR_STATUS"].ToString();
				txtREQ_MSG.Value = dt.Rows[0]["REQ_MSG"].ToString();
				txtDEPT_PERSON.Value = dt.Rows[0]["DEPT_PERSON"].ToString();
				txtDEPT_PERSON_NM.Value = dt.Rows[0]["DEPT_PERSON_NM"].ToString();
				txtDEPT_REMARKS.Value = dt.Rows[0]["DEPT_REMARKS"].ToString();
				txtCUST_CD.Value = dt.Rows[0]["CUST_CD"].ToString();
				txtCUST_NM.Value = dt.Rows[0]["CUST_NM"].ToString();
				txtFileApprId.Value = dt.Rows[0]["FILE_APPR"].ToString();
				txtFileApprNm.Value = dt.Rows[0]["FILE_APPR_NM"].ToString();

				// 업체 등록
				txtCUST_DEPT.Value = dt.Rows[0]["CUST_DEPT"].ToString();
				txtCUST_POSITION.Value = dt.Rows[0]["CUST_POSITION"].ToString();
				txtCUST_PERSON.Value = dt.Rows[0]["CUST_PERSON"].ToString();
				dtCUST_REG_DT.Value = dt.Rows[0]["CUST_REG_DT"].ToString();

				if (dt.Rows[0]["CONN_PROC_YN"].ToString() == "Y")
					chkCONN_PROC_Y.Checked = true;
				else if (dt.Rows[0]["CONN_PROC_YN"].ToString() == "N")
					chkCONN_PROC_N.Checked = true;

				txtIMMED_MSG.Value = dt.Rows[0]["IMMED_MSG"].ToString().Replace("\n", "\r\n");
				txtROOT_CAUSE.Value = dt.Rows[0]["ROOT_CAUSE"].ToString();
				txtCAUSE_TYPE.Value = dt.Rows[0]["CAUSE_TYPE"].ToString();
				txtROOT_CAUSE_MSG.Value = dt.Rows[0]["ROOT_CAUSE_MSG"].ToString();

				if (dt.Rows[0]["ADD_BAD_YN"].ToString() == "Y")
					chkADD_BAD_Y.Checked = true;
				else if (dt.Rows[0]["ADD_BAD_YN"].ToString() == "N")
					chkADD_BAD_N.Checked = true;

				txtACTION_DEPT.Value = dt.Rows[0]["ACTION_DEPT"].ToString();
				dtACTION_DT.Value = dt.Rows[0]["ACTION_DT"].ToString();
				txtACTION_MSG.Value = dt.Rows[0]["ACTION_MSG"].ToString();

				// 퍼스텍 담당자 등록
				txtFST_PERSON.Value = dt.Rows[0]["FST_PERSON"].ToString();
				txtFST_PERSON_NM.Value = dt.Rows[0]["FST_PERSON_NM"].ToString();

				if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
					chkAPPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
					chkAPPROVAL_N.Checked = true;

				dtAPPROVAL_DT.Value = dt.Rows[0]["APPROVAL_DT"].ToString();
				txtCORR_RESULT.Value = dt.Rows[0]["CORR_RESULT"].ToString();
				txtCORR_EFFECTS.Value = dt.Rows[0]["CORR_EFFECTS"].ToString();
				dtCORR_EFFECTS_DT.Value = dt.Rows[0]["CORR_EFFECTS_DT"].ToString();

				// 퍼스텍 승인권자 등록
				txtFST_APPROVAL.Value = dt.Rows[0]["FST_APPROVAL"].ToString();
				txtFST_APPROVAL_NM.Value = dt.Rows[0]["FST_APPROVAL_NM"].ToString();

				if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "Y")
					chkFST_APPROVAL_Y.Checked = true;
				else if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "N")
					chkFST_APPROVAL_N.Checked = true;

				dtFST_APPR_DT.Value = dt.Rows[0]["FST_APPR_DT"].ToString();
				txtFINAL_REMARKS.Value = dt.Rows[0]["FINAL_REMARKS"].ToString();

				
				SetCondition(false);
				SetValidAddFileAppr();

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

		#region 저장
		protected override void SaveExec()
        {
            string ERRCode = "ER", MSGCode = "", CorrNo = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) && GetValidationExDt())
            {
                if (string.IsNullOrEmpty(txtCORR_NO.Text))
                {
                    try
                    {
						if (string.IsNullOrEmpty(txtCUST_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "협력업체 코드를 확인해주세요.";
							goto Exit;
						}

						if (!string.IsNullOrEmpty(txtFileApprId.Text) && string.IsNullOrEmpty(txtFileApprNm.Text))
						{
							Trans.Rollback();
							MSGCode = "파일승인자 코드를 확인해주세요.";
							goto Exit;
						}

						if (!string.IsNullOrEmpty(txtDEPT_PERSON.Text) && string.IsNullOrEmpty(txtDEPT_PERSON_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "발행부서장 코드를 확인해주세요.";
							goto Exit;
						}
						
						string strQuery = "";
                        strQuery = " usp_SC003 @pTYPE = 'I1' ";
                        strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";							// 시정조치번호
                        strQuery = strQuery + ", @pACTION_TYPE		= '" + cboACTION_TYPE.SelectedValue + "' ";				// 발행유형
                        strQuery = strQuery + ", @pREG_DEPT			= '" + txtREG_DEPT.Text + "' ";							// 발행부서
                        strQuery = strQuery + ", @pREG_PERSON		= '" + txtREG_PERSON.Text + "' ";						// 발행인
                        strQuery = strQuery + ", @pREG_DT			= '" + dtREG_DT.Text + "' ";							// 발행일
                        strQuery = strQuery + ", @pCOMP_REQ_DT		= '" + dtCOMP_REQ_DT.Text + "' ";						// 완료요구일
                        strQuery = strQuery + ", @pTITLE			= '" + txtTITLE.Text.Replace("'", "''") + "' ";			// 제목
                        strQuery = strQuery + ", @pREQ_MSG			= '" + txtREQ_MSG.Text.Replace("'", "''") + "' ";		// 조치요구내용
                        strQuery = strQuery + ", @pDEPT_PERSON		= '" + txtDEPT_PERSON.Text + "' ";						// 발행부서장
                        strQuery = strQuery + ", @pDEPT_REMARKS		= '" + txtDEPT_REMARKS.Text.Replace("'", "''") + "' ";	// 발행부서장 의견
                        strQuery = strQuery + ", @pCUST_CD			= '" + txtCUST_CD.Text + "' ";							// 협력업체코드
						strQuery = strQuery + ", @pFILE_APPR		= '" + txtFileApprId.Text + "' ";						// 첨부파일 승인자
                        strQuery = strQuery + ", @pFILES_NO		    = '" + strRan + "' ";									// 첨부파일 임시 FILES_NO
                        

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        CorrNo  = ds.Tables[0].Rows[0][2].ToString();

                        if (ERRCode == "ER")
                        {
                            Trans.Rollback();
                            goto Exit;  // ER 코드 Return시 점프
                        }
						else
						{
							strQuery = "";
							strQuery = " usp_SC003 @pTYPE = 'UF'";
							strQuery = strQuery + ", @pCOMP_CODE	= '" + SystemBase.Base.gstrCOMCD + "' ";
							strQuery = strQuery + ", @pCORR_NO		= '" + CorrNo + "' ";						// 시정조치번호
							strQuery = strQuery + ", @pFILE_APPR	= '" + txtFileApprId.Text + "' ";			// 첨부파일 승인자
							strQuery = strQuery + ", @pFILES_NO		= '" + strRan + "' ";						// 첨부파일 임시 FILES_NO

							DataSet ds2 = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
							ERRCode = ds2.Tables[0].Rows[0][0].ToString();
							MSGCode = ds2.Tables[0].Rows[0][1].ToString();

							if (ERRCode == "ER")
							{
								Trans.Rollback();
								goto Exit;  // ER 코드 Return시 점프
							}
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
                        SelectExec(CorrNo);
                }
                else
                {
                    try
                    {
						if (txtREG_PERSON.Text == SystemBase.Base.gstrUserID || strGAuth == "S" || txtDEPT_PERSON.Text == SystemBase.Base.gstrUserID || GetEmpNo(txtDEPT_PERSON.Text) == SystemBase.Base.gstrUserID)
						{

						}
						else
						{
							Trans.Rollback();
							MSGCode = "내용 수정 권한이 없습니다.";
							goto Exit;
						}

						if (string.IsNullOrEmpty(txtCUST_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "협력업체 코드를 확인해주세요.";
							goto Exit;
						}

						if (!string.IsNullOrEmpty(txtFileApprId.Text) && string.IsNullOrEmpty(txtFileApprNm.Text))
						{
							Trans.Rollback();
							MSGCode = "파일승인자 코드를 확인해주세요.";
							goto Exit;
						}

						if (!string.IsNullOrEmpty(txtDEPT_PERSON.Text) && string.IsNullOrEmpty(txtDEPT_PERSON_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "발행부서장 코드를 확인해주세요.";
							goto Exit;
						}
						
						string strQuery = "";
                        strQuery = " usp_SC003 @pTYPE = 'U1' ";
                        strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";              // 시정조치번호
                        strQuery = strQuery + ", @pACTION_TYPE		= '" + cboACTION_TYPE.SelectedValue + "' "; // 발행유형
                        strQuery = strQuery + ", @pREG_DT			= '" + dtREG_DT.Text + "' ";                // 발행일
                        strQuery = strQuery + ", @pCOMP_REQ_DT		= '" + dtCOMP_REQ_DT.Text + "' ";           // 완료요구일
                        strQuery = strQuery + ", @pTITLE			= '" + txtTITLE.Text.Replace("'", "''") + "' ";                // 제목
                        strQuery = strQuery + ", @pREQ_MSG			= '" + txtREQ_MSG.Text.Replace("'", "''") + "' ";              // 조치요구내용
                        strQuery = strQuery + ", @pDEPT_PERSON		= '" + txtDEPT_PERSON.Text + "' ";          // 발행부서장
                        strQuery = strQuery + ", @pDEPT_REMARKS		= '" + txtDEPT_REMARKS.Text.Replace("'", "''") + "' ";         // 발행부서장 의견
                        strQuery = strQuery + ", @pCUST_CD			= '" + txtCUST_CD.Text + "' ";              // 협력업체코드
						strQuery = strQuery + ", @pFILE_APPR		= '" + txtFileApprId.Text + "' ";           // 첨부파일 승인자

						DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        CorrNo = txtCORR_NO.Text;

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
                        SelectExec(CorrNo);
                }
                
            }

        }

        private bool GetValidationExDt()
        {
            bool bReturn = true;

			if (!String.IsNullOrEmpty(dtCOMP_REQ_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(dtREG_DT.Text);
				DateTime dtReq = Convert.ToDateTime(dtCOMP_REQ_DT.Text);
				TimeSpan dateDiff = dtReq - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					MessageBox.Show("완료요구일은 발행일보다 이전일 수 없습니다.");
					bReturn = false;
				}
			}
            
            return bReturn;
        }
        #endregion

        #region 삭제
        protected override void DeleteExec()
        {
			if (string.IsNullOrEmpty(txtCORR_NO.Text)) return;

			DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string ERRCode, MSGCode = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strQuery = "";
                    strQuery = " usp_SC003 @pTYPE = 'D1' ";
                    strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery = strQuery + ", @pCORR_NO =" + txtCORR_NO.Text + "";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER")
                    {
                        Trans.Rollback();
						dbConn.Close();
						MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
						return;
					}
					else
					{
						Trans.Commit();
						goto Exit;
					}
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.ToString());
                    MSGCode = "P0001";
					dbConn.Close();
					MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
					return;
				}
                    
                Exit:
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
					SystemBase.Validation.GroupBox_Reset(groupBox2);
					SelectExec("");

            }
        }
        #endregion

        #region 검사원 POPUP. 공통코드 검사원에서 조회함.
        private void btnDEPT_PERSON_Click(object sender, EventArgs e)
        {
            GetPerson(txtDEPT_PERSON, txtDEPT_PERSON_NM);
        }

        private void btnFST_PERSON_Click(object sender, EventArgs e)
        {
            GetPerson(txtFST_PERSON, txtFST_PERSON_NM);
        }

        private void btnFST_APPROVAL_Click(object sender, EventArgs e)
        {
            GetPerson(txtFST_APPROVAL, txtFST_APPROVAL_NM);
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

		private void txtDEPT_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtDEPT_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDEPT_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void txtFST_PERSON_TextChanged(object sender, EventArgs e)
		{
			txtFST_PERSON_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFST_PERSON.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		private void txtFST_APPROVAL_TextChanged(object sender, EventArgs e)
		{
			txtFST_APPROVAL_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtFST_APPROVAL.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
		}

		#endregion

		#region 첨부파일 처리
		private void btnAddFiles_Click(object sender, EventArgs e)
		{
			try
			{
				// 첨부파일 팝업 띄움.
				WNDWS01 pu = new WNDWS01(txtCORR_NO.Text, txtCORR_NO.Text, "", "", "", txtFileApprId.Text, true, strRan, "시정조치", "SCMCA");
				pu.ShowDialog();

				SetValidAddFileAppr();
			}
			catch (Exception f)
			{
				MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region 첨부파일 유무에 따라 파일 승인자 필수값 처리
		private void SetValidAddFileAppr()
		{
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT dbo.ufn_GetAddFileYN('" + SystemBase.Base.gstrCOMCD + "', '" + txtCORR_NO.Text + "', 'SCMCA', '" + strRan + "')";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null)
			{
				if (dt.Rows[0][0].ToString() == "Y")
				{
					txtFileApprId.Tag = "파일승인자;1;;";
					SystemBase.Validation.GroupBox_Setting(groupBox3);

					if (string.IsNullOrEmpty(txtFileApprId.Text))
						MessageBox.Show("첨부파일이 있으므로 파일 승인자를 지정해주세요.");
				}
				else
				{
					txtFileApprId.Tag = "";
					SystemBase.Validation.GroupBox_Setting(groupBox3);
				}
			}
		}
		#endregion

		#region long text popup 처리
		private void txtREQ_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("조치요구내용");
		}

		private void txtDEPT_REMARKS_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("발행부서의견");
		}

		private void txtIMMED_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("즉시조치");
		}

		private void txtROOT_CAUSE_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("근본원인");
		}

		private void txtROOT_CAUSE_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("근본원인시정및조치걔획");
		}

		private void txtACTION_MSG_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("조치부서의견");
		}

		private void txtCORR_RESULT_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("시정조치확인결과");
		}

		private void txtFINAL_REMARKS_DoubleClick(object sender, EventArgs e)
		{
			popupMsg("승인부서의견");
		}

		private void popupMsg(string msg)
		{
			if (!string.IsNullOrEmpty(txtCORR_NO.Text))
			{
				QA003P1 myForm = new QA003P1(txtCORR_NO.Text, msg);
				myForm.ShowDialog();
			}
		}


		#endregion

		#region 화면 출력
		protected override void PrintExec()
		{
			if (txtCORR_NO.Text != "")
			{
				string strSheetPage1 = "시정조치";

				string strFileName = SystemBase.Base.ProgramWhere + @"\Report\시정조치요구서.xls";

				try
				{
					this.Cursor = Cursors.WaitCursor;

					string strSql = " usp_SC003 @pTYPE	 = 'R1' ";
					strSql = strSql + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
					strSql = strSql + ", @pCORR_NO = '" + txtCORR_NO.Text + "' ";

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

						excel.SetCell(2, 2, dt.Rows[0]["CORR_NO"].ToString());			// 시정조치번호
						excel.SetCell(3, 10, dt.Rows[0]["ACTION_TYPE"].ToString());		// 발행유형
						excel.SetCell(2, 4, dt.Rows[0]["REG_DEPT"].ToString());			// 발행부서
						excel.SetCell(2, 6, dt.Rows[0]["REG_PERSON_NM"].ToString());	// 발행인
						excel.SetCell(2, 8, dt.Rows[0]["REG_DT"].ToString());			// 발행일
						excel.SetCell(2, 10, dt.Rows[0]["COMP_REQ_DT"].ToString());		// 완료요구일
						excel.SetCell(3, 2, dt.Rows[0]["TITLE"].ToString());			// 제목
						excel.SetCell(4, 2, dt.Rows[0]["REQ_MSG"].ToString());			// 조치요구내용
						excel.SetCell(6, 2, dt.Rows[0]["DEPT_PERSON_NM"].ToString());	// 발행부서장
						excel.SetCell(6, 4, dt.Rows[0]["DEPT_REG_DT"].ToString());		// 등록일
						excel.SetCell(7, 2, dt.Rows[0]["DEPT_REMARKS"].ToString());		// 발행부서장 의견
						excel.SetCell(8, 2, dt.Rows[0]["CUST_NM"].ToString());			// 협력업체명
						excel.SetCell(8, 4, dt.Rows[0]["CUST_DEPT"].ToString());		// 협력업체-부서
						excel.SetCell(8, 6, dt.Rows[0]["CUST_POSITION"].ToString());	// 협력업체-직책
						excel.SetCell(8, 8, dt.Rows[0]["CUST_PERSON"].ToString());		// 협력업체-담당자
						excel.SetCell(8, 10, dt.Rows[0]["CUST_REG_DT"].ToString());		// 협력업체-등록일
						excel.SetCell(9, 3, dt.Rows[0]["CONN_PROC_YN"].ToString());		// 타공정 영향성 유무
						excel.SetCell(10, 2, dt.Rows[0]["IMMED_MSG"].ToString());		// 즉시조치
						excel.SetCell(11, 2, dt.Rows[0]["ROOT_CAUSE"].ToString());		// 근본원인
						excel.SetCell(12, 2, dt.Rows[0]["CAUSE_TYPE"].ToString());		// 원인분류코드
						excel.SetCell(13, 2, dt.Rows[0]["ROOT_CAUSE_MSG"].ToString());	// 근본원인 시정 및 조치 계획
						excel.SetCell(14, 3, dt.Rows[0]["ADD_BAD_YN"].ToString());		// 추가 부적합 유무
						excel.SetCell(15, 2, dt.Rows[0]["ACTION_DEPT"].ToString());		// 조치부서장
						excel.SetCell(15, 4, dt.Rows[0]["ACTION_DT"].ToString());		// 조치부서 승인일
						excel.SetCell(15, 6, dt.Rows[0]["ADD_FILES"].ToString());		// 첨부파일
						excel.SetCell(16, 2, dt.Rows[0]["ACTION_MSG"].ToString());		// 조치부서장 의견
						excel.SetCell(17, 2, dt.Rows[0]["FST_PERSON_NM"].ToString());	// 퍼스텍 담당자

						// 퍼스텍 담당자 승인여부
						if (dt.Rows[0]["APPROVAL_YN"].ToString() == "Y")
							excel.SetCell(17, 4, "승인");        
						else if (dt.Rows[0]["APPROVAL_YN"].ToString() == "N")
							excel.SetCell(17, 4, "반려");        

						excel.SetCell(17, 8, dt.Rows[0]["APPROVAL_DT"].ToString());		// 처리일
						excel.SetCell(18, 2, dt.Rows[0]["CORR_RESULT"].ToString());		// 시정조치 확인결과
						excel.SetCell(19, 2, dt.Rows[0]["CORR_EFFECTS"].ToString());	// 조치사항 효과성 확인
						excel.SetCell(19, 8, dt.Rows[0]["CORR_EFFECTS_DT"].ToString());	// 확인일
						excel.SetCell(20, 2, dt.Rows[0]["FST_APPROVAL_NM"].ToString());	// 퍼스텍 승인자

						// 퍼스텍 승인자 승인여부
						if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "Y")
							excel.SetCell(20, 4, "승인");
						else if (dt.Rows[0]["FST_APPROVAL_YN"].ToString() == "N")
							excel.SetCell(20, 4, "반려");

						excel.SetCell(20, 8, dt.Rows[0]["FST_APPR_DT"].ToString());		// 퍼스텍 승인자 처리일
						excel.SetCell(21, 2, dt.Rows[0]["FINAL_REMARKS"].ToString());	// 최종 의견
						
						excel.ShowExcel(true);
					}
				}
				catch (Exception f)
				{
					SystemBase.Loggers.Log(this.Name, f.ToString());
					MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "시정조치요구서 출력"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
				}
				this.Cursor = Cursors.Default;

			}
			else
			{
				MessageBox.Show("출력할 시정조치 번호를 선택해주세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}
		#endregion

		#region 승인/반려 처리
		private void btnAppr1_Click(object sender, EventArgs e)
		{
			ProcApproval("U2");
		}

		private void btnAppr2_Click(object sender, EventArgs e)
		{
			ProcApproval("U3");
		}

		private void ProcApproval(string procType)
		{
			string ERRCode = "ER", MSGCode = "", CorrNo = "", strAPPROVAL_YN = "", strFST_APPROVAL_YN = "", strQuery = "", strValidationMsg = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox5) && GetApprovalValidation(procType))
			{
				try
				{
					if (procType == "U2" && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox5))   // 퍼스텍 담당자 승인처리
					{
						if (chkAPPROVAL_Y.Checked)
							strAPPROVAL_YN = "Y";
						else if (chkAPPROVAL_N.Checked)
							strAPPROVAL_YN = "N";

						if (!chkAPPROVAL_Y.Checked && !chkAPPROVAL_N.Checked)
						{
							Trans.Rollback();
							MSGCode = "승인 또는 반려 값에 체크해주세요.";
							goto Exit;
						}

						if (string.IsNullOrEmpty(txtFST_PERSON.Text) || string.IsNullOrEmpty(txtFST_PERSON_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "퍼스텍담당자를 입력해주세요.";
							goto Exit;
						}

                        if (string.IsNullOrEmpty(dtCUST_REG_DT.Text))
                        {
                            Trans.Rollback();
                            MSGCode = "업체 회신 내용이 없습니다.";
                            goto Exit;
                        }

						if (!GetValidationApprDt())
						{
							Trans.Rollback();
							MSGCode = "승인/반려 처리일은 업체 승인일보다 이전일 수 없습니다.";
							goto Exit;
						}

						strQuery = "";
						strQuery = " usp_SC003 @pTYPE = '" + procType + "' ";
						strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
						strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";              // 시정조치번호
						strQuery = strQuery + ", @pFST_PERSON		= '" + txtFST_PERSON.Text + "' ";           // 퍼스텍 담당자
						strQuery = strQuery + ", @pAPPROVAL_YN		= '" + strAPPROVAL_YN + "' ";               // 승인여부
						strQuery = strQuery + ", @pAPPROVAL_DT		= '" + dtAPPROVAL_DT.Text + "' ";           // 처리일
						strQuery = strQuery + ", @pCORR_RESULT		= '" + txtCORR_RESULT.Text + "' ";          // 시정조치 확인결과
						strQuery = strQuery + ", @pCORR_EFFECTS		= '" + txtCORR_EFFECTS.Text + "' ";         // 조치사항 효과성 확인
						strQuery = strQuery + ", @pCORR_EFFECTS_DT	= '" + dtCORR_EFFECTS_DT.Text + "' ";       // 확인일
						strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자
					}
					else if (procType == "U3" && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox6))   // 퍼스텍 승인자 승인처리
					{
						if (chkFST_APPROVAL_Y.Checked)
							strFST_APPROVAL_YN = "Y";
						else if (chkFST_APPROVAL_N.Checked)
							strFST_APPROVAL_YN = "N";

						if (string.IsNullOrEmpty(txtFST_APPROVAL.Text) || string.IsNullOrEmpty(txtFST_APPROVAL_NM.Text))
						{
							Trans.Rollback();
							MSGCode = "퍼스텍 승인자를 입력해주세요.";
							goto Exit;
						}

						if (!chkFST_APPROVAL_Y.Checked && !chkFST_APPROVAL_N.Checked)
						{
							Trans.Rollback();
							MSGCode = "승인 또는 반려 값에 체크해주세요.";
							goto Exit;
						}

                        if (chkAPPROVAL_Y.Checked == false)
                        {
                            Trans.Rollback();
                            MSGCode = "퍼스텍 담당자 승인처리가 되어 있지 않습니다.";
                            goto Exit;
                        }

						if (!GetValidationApprDt())
						{
							Trans.Rollback();
							MSGCode = "승인/반려 처리일은 퍼스텍 담당자 승인일보다 이전일 수 없습니다.";
							goto Exit;
						}

						strQuery = "";
						strQuery = " usp_SC003 @pTYPE = '" + procType + "' ";
						strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
						strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";              // 시정조치번호
						strQuery = strQuery + ", @pFST_APPROVAL		= '" + txtFST_APPROVAL.Text + "' ";         // 퍼스텍 승인자                      
						strQuery = strQuery + ", @pFST_APPROVAL_YN	= '" + strFST_APPROVAL_YN + "' ";           // 승인여부                           
						strQuery = strQuery + ", @pFST_APPR_DT		= '" + dtFST_APPR_DT.Text + "' ";           // 승인일                             
						strQuery = strQuery + ", @pFINAL_REMARKS	= '" + txtFINAL_REMARKS.Text + "' ";        // 최종 의견                          
						strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자     
					}

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					CorrNo = txtCORR_NO.Text;

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
					SelectExec(CorrNo);
			}

		}

		private bool GetApprovalValidation(string procType)
		{
			bool bReturn = true;
			string strReturn = string.Empty;

			if (procType == "U2")   // 퍼스텍 담당자 승인처리
			{
				if (!chkAPPROVAL_Y.Checked && !chkAPPROVAL_N.Checked)
				{
					strReturn = "승인 또는 반려 값을 체크해주세요.";
					bReturn = false;
					goto Exit;
				}
			}
			else if (procType == "U3")   // 퍼스텍 승인자 승인처리
			{
				if (!chkFST_APPROVAL_Y.Checked && !chkFST_APPROVAL_N.Checked)
				{
					strReturn = "승인 또는 반려 값을 체크해주세요.";
					bReturn = false;
					goto Exit;
				}
			}

		Exit:
			if (!string.IsNullOrEmpty(strReturn))
				MessageBox.Show(strReturn);

			return bReturn;
		}

		private bool GetValidationDt()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(dtCORR_EFFECTS_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(dtAPPROVAL_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(dtCORR_EFFECTS_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("효과성 확인일은 처리일보다 이전일 수 없습니다.");
					bReturn = false;
				}
			}

			return bReturn;
		}

		private bool GetValidationApprDt()
		{
			bool bReturn = true;
			bool bAppr = true;
			bool bFstAppr = true;

			if (!string.IsNullOrEmpty(dtAPPROVAL_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(dtACTION_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(dtAPPROVAL_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("효과성 확인일은 업체 승인일보다 이전일 수 없습니다.");
					bAppr = false;
				}
			}

			if (!string.IsNullOrEmpty(dtFST_APPR_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(dtAPPROVAL_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(dtFST_APPR_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("효과성 확인일은 업체 승인일보다 이전일 수 없습니다.");
					bFstAppr = false;
				}
			}

			if (bAppr && bFstAppr)
				bReturn = true;
			else
				bReturn = false;

			return bReturn;
		}
		#endregion

		#region 체크박스 설정
		private void chkAPPROVAL_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_Y.Checked)
            {
                chkAPPROVAL_N.Checked = false;
                dtAPPROVAL_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            }

            if (chkAPPROVAL_N.Checked == false && chkAPPROVAL_Y.Checked == false)
                dtAPPROVAL_DT.Value = "";
        }

		private void chkAPPROVAL_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAPPROVAL_N.Checked)
            {
                chkAPPROVAL_Y.Checked = false;
                dtAPPROVAL_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            }

            if (chkAPPROVAL_N.Checked == false && chkAPPROVAL_Y.Checked == false)
                dtAPPROVAL_DT.Value = "";
        }

		private void chkFST_APPROVAL_Y_CheckedChanged(object sender, EventArgs e)
		{
			if (chkFST_APPROVAL_Y.Checked)
            {
                chkFST_APPROVAL_N.Checked = false;
                dtFST_APPR_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            }

            if (chkFST_APPROVAL_Y.Checked == false && chkFST_APPROVAL_N.Checked == false)
                dtFST_APPR_DT.Value = "";

        }

		private void chkFST_APPROVAL_N_CheckedChanged(object sender, EventArgs e)
		{
			if (chkFST_APPROVAL_N.Checked)
            {
                chkFST_APPROVAL_Y.Checked = false;
                dtFST_APPR_DT.Value = SystemBase.Base.ServerTime("YYMMDD");
            }

            if (chkFST_APPROVAL_Y.Checked == false && chkFST_APPROVAL_N.Checked == false)
                dtFST_APPR_DT.Value = "";

        }
		#endregion

		#region 첨부파일 승인자
		private void btnFileAppr_Click(object sender, EventArgs e)
		{
			try
			{
				string strQuery = " usp_B_COMMON 'B010', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
				string[] strWhere = new string[] { "@pCODE", "@pNAME" };
				string[] strSearch = new string[] { txtFileApprId.Text, txtFileApprNm.Text };
				UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P04003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사용자 조회");
				pu.ShowDialog();
				if (pu.DialogResult == DialogResult.OK)
				{
					Regex rx1 = new Regex("#");
					string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

					txtFileApprId.Value = Msgs[0].ToString();
					txtFileApprNm.Value = Msgs[1].ToString();
				}
			}
			catch (Exception f)
			{
				SystemBase.Loggers.Log(this.Name, f.ToString());
				MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사용자조회"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
			}
		}

		private void txtFileApprId_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtCORR_NO.Text))
			{
				if (txtFileApprId.Text == SystemBase.Base.gstrUserID)
				{
					MessageBox.Show("등록자는 파일 승인자가 될 수 없습니다.");
					txtFileApprId.Value = "";
					txtFileApprNm.Value = "";
					return;
				}
			}
			else
			{
				if (txtFileApprId.Text == txtREG_PERSON.Text)
				{
					MessageBox.Show("등록자는 파일 승인자가 될 수 없습니다.");
					txtFileApprId.Value = "";
					txtFileApprNm.Value = "";
					return;
				}
			}


			

			if (!string.IsNullOrEmpty(txtFileApprId.Text))
			{
				txtFileApprNm.Value = SystemBase.Base.CodeName("USR_ID", "USR_NM", "B_SYS_USER", txtFileApprId.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
			}
			else
			{
				txtFileApprNm.Value = "";
			}
		}
		#endregion

		#region 효과성 확인 처리
		private void btnConfirmDt_Click(object sender, EventArgs e)
		{
			try
			{
				ProcEffConfirm();
			}
			catch(Exception eX)
			{
				MessageBox.Show(eX.ToString());
			}
		}

		private void ProcEffConfirm()
		{
			string ERRCode = "ER", MSGCode = "", CorrNo = "", strAPPROVAL_YN = "", strFST_APPROVAL_YN = "", strQuery = "", strValidationMsg = "";

			SqlConnection dbConn = SystemBase.DbOpen.DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

			if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox7))
			{
				try
				{

					if (!GetValidationDt())
					{
						Trans.Rollback();
						MSGCode = "효과성 확인일은 승인/반려 처리일보다 이전일 수 없습니다.";
						goto Exit;
					}

					strQuery = "";
					strQuery = " usp_SC003 @pTYPE = 'U4' ";
					strQuery = strQuery + ", @pCOMP_CODE		= '" + SystemBase.Base.gstrCOMCD + "' ";
					strQuery = strQuery + ", @pCORR_NO			= '" + txtCORR_NO.Text + "' ";              // 시정조치번호
					strQuery = strQuery + ", @pCORR_EFFECTS		= '" + txtCORR_EFFECTS.Text + "' ";         // 조치사항 효과성 확인
					strQuery = strQuery + ", @pCORR_EFFECTS_DT	= '" + dtCORR_EFFECTS_DT.Text + "' ";       // 확인일
					strQuery = strQuery + ", @pUP_ID			= '" + SystemBase.Base.gstrUserID + "' ";   // 수정자

					DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
					ERRCode = ds.Tables[0].Rows[0][0].ToString();
					MSGCode = ds.Tables[0].Rows[0][1].ToString();
					CorrNo = txtCORR_NO.Text;

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
					SelectExec(CorrNo);
			}

		}

		private bool GetEffConfirmVal()
		{
			bool bReturn = true;

			if (!string.IsNullOrEmpty(dtCORR_EFFECTS_DT.Text))
			{
				DateTime dtReg = Convert.ToDateTime(dtAPPROVAL_DT.Text);
				DateTime dtLimit = Convert.ToDateTime(dtCORR_EFFECTS_DT.Text);
				TimeSpan dateDiff = dtLimit - dtReg;
				int diffDay = dateDiff.Days;
				if (diffDay < 0)
				{
					//MessageBox.Show("효과성 확인일은 승인일보다 이전일 수 없습니다.");
					bReturn = false;
				}
			}

			return bReturn;
		}
		#endregion
		
	}
}
