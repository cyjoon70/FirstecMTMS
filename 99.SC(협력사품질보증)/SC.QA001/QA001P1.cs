using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using WNDW;
using System.Text.RegularExpressions;

namespace SC.QA001
{
    public partial class QA001P1 : UIForm.Buttons
    {
        #region 변수선언
        private string Gubun;
        private string Seq;

        // 파일 임시저장을 위한 Random number
        int iRan = 0;
        #endregion

        #region 생성자
        public QA001P1(string gubun, string seq)
        {
            InitializeComponent();

            Gubun = gubun;
            Seq = seq;
        }
        #endregion

        #region Form Load
        private void QA001P1_Load(object sender, EventArgs e)
        {
            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

			//콤보박스 세팅
			SystemBase.ComboMake.C1Combo(cboJobType, "usp_B_COMMON @pType='COMM', @pCODE = 'NO_JOB_TYP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9);

            SetInit();

            if (Gubun == "W")
            {
                SelectMode();
            }
            else
            {
                SelectExec();
                SelectMode();
            }

        }

        private void SetInit()
        {
            Random randomObj = new Random();
            iRan = randomObj.Next(100000, 999999);

            txtRegUser.Value = SystemBase.Base.gstrUserName;
            dtRegDt.Text = SystemBase.Base.ServerTime("YYMMDD");
			dtLimitDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(2).ToString().Substring(0, 10);

			txtRegUser.ReadOnly = true;
            txtReadCnt.ReadOnly = true;

            txtRegUser.BackColor = Color.WhiteSmoke;
            txtReadCnt.BackColor = Color.WhiteSmoke;
            txtCustNm.BackColor = Color.WhiteSmoke;
        }

        private void SelectMode()
        {
            if (Gubun == "W")
            {
                UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else if (Gubun == "R" && txtRegUser.Text == SystemBase.Base.gstrUserName)
            {
                UIForm.Buttons.ReButton("000000110000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
            else if (Gubun == "R" && txtRegUser.Text != SystemBase.Base.gstrUserName)
            {
                UIForm.Buttons.ReButton("000000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            }
        }

        private void SelectExec()
        {
            string strQuery = "";
            strQuery = " usp_SC001 @pTYPE = 'S2' ";
            strQuery = strQuery + ", @pSEQ = " + Seq;
            strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

            txtSeq.Value = ds.Tables[0].Rows[0][1].ToString();              //Seq
            cboJobType.SelectedValue = ds.Tables[0].Rows[0][2].ToString();  //업무구분
            txtTitle.Text = ds.Tables[0].Rows[0][3].ToString();             //제목
            txtContents.Text = ds.Tables[0].Rows[0][4].ToString();          //내용
            txtRegUser.Value = ds.Tables[0].Rows[0][6].ToString();          //등록자
            dtRegDt.Text = ds.Tables[0].Rows[0][7].ToString();              //등록일
            dtLimitDt.Text = ds.Tables[0].Rows[0][8].ToString();            //공지만료일
            txtCustCd.Text = ds.Tables[0].Rows[0][9].ToString();            //공개업체코드
            txtCustNm.Value = ds.Tables[0].Rows[0][10].ToString();          //공개업체명
            txtReadCnt.Value = ds.Tables[0].Rows[0][11].ToString();         //조회수
            txtRegUserId.Value = ds.Tables[0].Rows[0][5].ToString();        //등록자 ID
			txtUserId.Value = ds.Tables[0].Rows[0][12].ToString();			//첨부파일 승인자 ID
			txtUserNm.Value = ds.Tables[0].Rows[0][13].ToString();			//첨부파일 승인자
			cdtAPPR_DT.Value = ds.Tables[0].Rows[0][14].ToString();         //첨부파일 승인일

			SetValidAddFileAppr();
		}
        #endregion

        #region SaveExec()
        protected override void SaveExec()
        {
            string ERRCode = "ER", MSGCode = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) && SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2) && GetValidationExDt())
            {
                                
                if (txtUserId.Text == SystemBase.Base.gstrUserID)
                {
                    MessageBox.Show("등록자는 파일 승인자가 될 수 없습니다.");
                    txtUserId.Value = "";
                    txtUserNm.Value = "";
                    return;
                }

                if (Gubun == "W")
                {
                    try
                    {

                        string strQuery = "";
                        strQuery = " usp_SC001 @pTYPE = 'I1' ";
                        strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery = strQuery + ", @pJOB_TYPE = '" + cboJobType.SelectedValue + "' ";
                        strQuery = strQuery + ", @pOPEN_CUST_CD = '" + txtCustCd.Text +"' ";
                        strQuery = strQuery + ", @pREG_DT = '" + dtRegDt.Text + "' ";
                        strQuery = strQuery + ", @pEXPIRE_DT = '" + dtLimitDt.Text +"' ";
                        strQuery = strQuery + ", @pTITLE ='" + txtTitle.Text + "' ";
                        strQuery = strQuery + ", @pCONTENTS ='" + txtContents.Text + "' ";
						strQuery = strQuery + ", @pFILE_APPR ='" + txtUserId.Text + "' ";
						strQuery = strQuery + ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strQuery = strQuery + ", @pFILES_NO = '" + "SC01" + iRan.ToString() + "'";
                        

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

                    if (ERRCode != "")
                        Dispose(true);



                }
                else if (Gubun == "R" && txtRegUser.Text == SystemBase.Base.gstrUserName)
                {
                    try
                    {

                        if (txtRegUserId.Text != SystemBase.Base.gstrUserID)
                        {
                            MessageBox.Show("내용 수정은 등록자만 가능합니다.");
                            txtUserId.Value = "";
                            txtUserNm.Value = "";
                            return;
                        }

                        string strQuery = "";
                        strQuery = " usp_SC001 @pTYPE = 'U1' ";
                        strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery = strQuery + ", @pSEQ = '" + txtSeq.Text + "' ";
                        strQuery = strQuery + ", @pJOB_TYPE = '" + cboJobType.SelectedValue + "' ";
                        strQuery = strQuery + ", @pOPEN_CUST_CD = '" + txtCustCd.Text + "' ";
                        strQuery = strQuery + ", @pREG_DT = '" + dtRegDt.Text + "' ";
                        strQuery = strQuery + ", @pEXPIRE_DT = '" + dtLimitDt.Text + "' ";
                        strQuery = strQuery + ", @pTITLE ='" + txtTitle.Text.Replace("'", "''") + "' ";
                        strQuery = strQuery + ", @pCONTENTS ='" + txtContents.Text.Replace("'", "''") + "' ";
						strQuery = strQuery + ", @pFILE_APPR ='" + txtUserId.Text + "' ";
						strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            Trans.Rollback();
                            goto Exit;  // ER 코드 Return시 점프
                        }
                        else
                        {

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
                    if (ERRCode != "")
                        Dispose(true);

                }
            }

        }

        private bool GetValidationExDt()
        {
            bool bReturn = true;

            DateTime dtReg = Convert.ToDateTime(dtRegDt.Text);
            DateTime dtLimit = Convert.ToDateTime(dtLimitDt.Text);
            TimeSpan dateDiff = dtLimit - dtReg;
            int diffDay = dateDiff.Days;
            if (diffDay <= 0)
            {
                MessageBox.Show("공지만료일은 등록일보다 이전일 수 없습니다.");
                bReturn = false;
            }
            return bReturn;
        }
        #endregion

        #region DeleteExec()
        protected override void DeleteExec()
        {
            DialogResult result = SystemBase.MessageBoxComm.Show("삭제 하시겠습니까?", "삭제", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string ERRCode, MSGCode = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                if (Gubun == "R" && txtRegUser.Text == SystemBase.Base.gstrUserName)
                {
                    try
                    {
                        string strQuery = "";
                        strQuery = " usp_SC001 @pTYPE = 'D1' ";
                        strQuery = strQuery + ", @pSeq =" + txtSeq.Text + "";
                        strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER")
                        {
                            Trans.Rollback();
                            goto Exit;	// ER 코드 Return시 점프
                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        MessageBox.Show(ex.ToString());
                        MSGCode = "P0001";
                        goto Exit;	// ER 코드 Return시 점프
                    }
                    Trans.Commit();

                Exit:
                    dbConn.Close();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
                    Dispose(true);
                }
            }
        }
        #endregion

        #region 공개업체 선택
        private void btnCustCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "");
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Value = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "거래처 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

		private void txtCustCd_TextChanged(object sender, EventArgs e)
		{
			txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
		}
		#endregion

		#region 첨부파일 처리
		private void btnPopup_Click(object sender, EventArgs e)
        {

            try
            {
                
                if (string.IsNullOrEmpty(txtSeq.Text))
                {
                    MessageBox.Show("저장된 공지사항 데이터를 조회 후 등록하시기 바랍니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                

                // 첨부파일 팝업 띄움. 
                QA001P2 pu = new QA001P2("SC01" + txtSeq.Text, txtUserId.Text, true);
                pu.ShowDialog();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetValidAddFileAppr();

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

		#region 첨부파일 유무에 따라 파일 승인자 필수값 처리
		private void SetValidAddFileAppr()
		{
			DataTable dt;
			string strQuery = string.Empty;
			strQuery = "SELECT dbo.ufn_GetAddFileCnt('" + SystemBase.Base.gstrCOMCD + "', '" + txtSeq.Text + "')";

			dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

			if (dt != null)
			{
				if (dt.Rows[0][0].ToString() == "Y")
				{
					txtUserId.Tag = "파일승인자;1;;";
					SystemBase.Validation.GroupBox_Setting(groupBox1);
				}
				else
				{
					txtUserId.Tag = "";
					SystemBase.Validation.GroupBox_Setting(groupBox1);
				}
			}
		}
		#endregion
	}
}
