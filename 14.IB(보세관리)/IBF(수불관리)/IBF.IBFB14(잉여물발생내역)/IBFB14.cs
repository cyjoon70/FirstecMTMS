#region 작성정보
/*********************************************************************/
// 단위업무명 : 잉여물발생내역
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-12
// 작성내용 : 잉여물발생내역 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;
using FarPoint.Win.Spread.CellType;

namespace IBF.IBFB14
{
    public partial class IBFB14 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB14()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB14_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            rdoV.Checked = true;
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
            rdoV.Checked = true;
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[9];


            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            string RptName = @"Report\" + "IBFB26P.rpt";
            RptParmValue[0] = "R1";
            RptParmValue[1] = txtTRNo.Text;

            if (dtpDT_FR.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = dtpDT_FR.Text;

            if (dtpDT_TO.Text.Trim() == "") RptParmValue[3] = " ";
            else RptParmValue[3] = dtpDT_TO.Text;

            if (txtNOTIFY_NO.Text.Trim() == "") RptParmValue[4] = " ";
            else RptParmValue[4] = txtNOTIFY_NO.Text;

            if (txtMATTER_CD.Text.Trim() == "") RptParmValue[5] = " ";
            else RptParmValue[5] = txtMATTER_CD.Text;

            RptParmValue[6] = SystemBase.Base.gstrUserID;

            string strDiv = "V";
            if (rdoP.Checked == true) strDiv = "P";

            RptParmValue[7] = strDiv;

            RptParmValue[8] = SystemBase.Base.gstrCOMCD;

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 11버전
            frm.ShowDialog();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {
                    string strDiv = "V";
                    if (rdoP.Checked == true) strDiv = "P";

                    string strQuery = " usp_IBFB14  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pMATTER_CD = '" + txtMATTER_CD.Text + "', ";
                    strQuery = strQuery + " @pNOTIFY_NO = '" + txtNOTIFY_NO.Text + "', ";
                    strQuery = strQuery + " @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "',";
                    strQuery = strQuery + " @pDIV = '" + strDiv + "',";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
                    //					fpSpread1.Sheets[0].OperationMode =  FarPoint.Win.Spread.OperationMode.SingleSelect;
                    if (fpSpread1.Sheets[0].RowCount > 0) set_Color();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        private void set_Color()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;
            for (int i = 0; i < iCnt; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재코드")].Text == "zzzzzzzzzz")
                {
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재코드")].Text = "";
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재명")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재규격")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "형태")].Text = "";
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = Color.LightSteelBlue; ;
                    }
                }
            }
        }

        #region 버튼 Click
        //신고번호
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF10' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //재질
        private void butMATTER_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " Nusp_BF_Comm 'BF06' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtMATTER_CD.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "형태 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMATTER_CD.Value = Msgs[0].ToString();
                    txtMATTER_NM.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //신고번호
        private void butNOTIFY_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF18' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME", "@pSPEC" };
                string[] strSearch = new string[] { txtNOTIFY_NO.Text, "", txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP012", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "반출번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtNOTIFY_NO.Text = Msgs[1].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //반제품의 제품생성
        private void btnCreate_Click(object sender, System.EventArgs e)
        {

            string RtnMsg = SystemBase.Base.MessageRtn("B0042");
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " usp_IBFB14  'P1' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";


                cmd.CommandText = strQuery;

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                string strRETURN = ds.Tables[0].Rows[0][0].ToString();
                string strMSG_CD = ds.Tables[0].Rows[0][1].ToString();

                if (strRETURN == "ER")
                {
                    MessageBox.Show(strMSG_CD, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trans.Rollback();
                    goto exit;
                }

                Trans.Commit();
                this.Cursor = Cursors.Default;
                MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

            exit:
                this.Cursor = Cursors.Default;
                dbConn.Close();

            }

            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                Trans.Rollback();
                RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dbConn.Close();
        }

        //잉여물생성단계1
        private void butCreate_Info_Click(object sender, System.EventArgs e)
        {
            fpSpread1.Sheets[0].RowCount = 0;
            string RtnMsg = SystemBase.Base.MessageRtn("B0042");
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strDiv = "V";
                    if (rdoP.Checked == true) strDiv = "P";

                    string strQuery = " usp_IBFB14  'P2', ";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pMATTER_CD = '" + txtMATTER_CD.Text + "', ";
                    strQuery = strQuery + " @pNOTIFY_NO = '" + txtNOTIFY_NO.Text + "', ";
                    strQuery = strQuery + " @pDIV = '" + strDiv + "',";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    cmd.CommandText = strQuery;

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                    string strRETURN = ds.Tables[0].Rows[0][0].ToString();
                    string strMSG_CD = ds.Tables[0].Rows[0][1].ToString();

                    if (strRETURN == "ER")
                    {
                        MessageBox.Show(ds.Tables[0].Rows[0][3].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Trans.Rollback();
                        goto exit;
                    }

                    Trans.Commit();
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dbConn.Close();
                    SearchExec();
                exit:
                    this.Cursor = Cursors.Default;
                    dbConn.Close();
                }

            }

            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                Trans.Rollback();
                dbConn.Close();
                RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //잉여물생성단계2
        private void butCreate_Info2_Click(object sender, System.EventArgs e)
        {
            string RtnMsg = SystemBase.Base.MessageRtn("B0042");
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    this.Cursor = Cursors.WaitCursor;

                    string strDiv = "V";
                    if (rdoP.Checked == true) strDiv = "P";

                    string strQuery = " usp_IBFB14  'P3', ";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pMATTER_CD = '" + txtMATTER_CD.Text + "', ";
                    strQuery = strQuery + " @pNOTIFY_NO = '" + txtNOTIFY_NO.Text + "', ";
                    strQuery = strQuery + " @pDIV = '" + strDiv + "', ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    cmd.CommandText = strQuery;

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                    string strRETURN = ds.Tables[0].Rows[0][0].ToString();
                    string strMSG_CD = ds.Tables[0].Rows[0][1].ToString();

                    if (strRETURN == "ER")
                    {
                        MessageBox.Show(ds.Tables[0].Rows[0][3].ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Trans.Rollback();
                        goto exit;
                    }

                    Trans.Commit();
                    this.Cursor = Cursors.Default;
                    MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dbConn.Close();
                    SearchExec();

                exit:
                    this.Cursor = Cursors.Default;
                    dbConn.Close();
                }

            }

            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                Trans.Rollback();
                dbConn.Close();
                RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }
                    txtSO_NO.Value = txtTRNo.Text.Trim();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtMATTER_CD_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtNOTIFY_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB14_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB14_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}








