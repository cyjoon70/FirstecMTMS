#region 작성정보
/*********************************************************************/
// 단위업무명 : 전자세금계산서등록
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-09
// 작성내용 : 전자세금계산서등록
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

namespace XS.XSB001
{
    public partial class XSB001 : UIForm.FPCOMM1
    {
        public XSB001()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void XSB001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'B029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//마감여부
		
            //그리드초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpIssueDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7)+"-01";
            dtpIssueDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
         
            rdoTypeAll.Checked = true;
            rdoIssueAll.Checked = true;
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpIssueDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7) + "-01";
            dtpIssueDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");

            rdoTypeAll.Checked = true;
            rdoIssueAll.Checked = true;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string type = "";
                    string issue = "";

                    if (rdoType1.Checked == true) type = "1";
                    else if (rdoType2.Checked == true) type = "2";
                    else if (rdoType3.Checked == true) type = "3";
                   // else if (rdoType4.Checked == true) type = "4";


                    if (rdoIssueY.Checked == true) issue = "Y";
                    else if (rdoIssueN.Checked == true) issue = "N";

                    string strQuery = " usp_XSB001 'S1'";
                    strQuery += ", @pTAX_BIZ_CD ='" + txtTaxBizCd.Text.Trim() + "'";
                    strQuery += ", @pBILL_CUST  ='" + txtCustCd.Text.Trim() + "'";
                    strQuery += ", @pISSUE_DT_FR  ='" + dtpIssueDtFr.Text + "'";
                    strQuery += ", @pISSUE_DT_TO  ='" + dtpIssueDtTo.Text + "'";
                    strQuery += ", @pTAX_TYPE ='" + type + "'";
                    strQuery += ", @pISSUE_YN ='" + issue + "'";
                    strQuery += ", @pTAX_NO  ='" + txtSTaxNo.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 1);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;

                    GridReMake();                
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 데이타 저장 로직 계산서 수정
        protected override void SaveExec()
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0)
                            {
                                string strSql = " usp_XSB001 @pTYPE = 'U1'";
                                strSql += ", @pTAX_NO      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서번호")].Text + "' ";
                                strSql += ", @pISSUED_DT   = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발행일")].Text + "' ";
                                strSql += ", @pNET_LOC_AMT =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액")].Value + " ";
                                strSql += ", @pVAT_LOC_AMT =  " + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액")].Value + " ";
                                strSql += ", @pVAT_TYPE    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text + "' ";
                                strSql += ", @pCLOSE_FG    = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text + "' ";
                                strSql += ", @pUP_ID       = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region GridReMake() 그리드 재정의
        public void GridReMake()
        {
            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text.ToString().Trim() == "Y")
                    {
                        UIForm.FPMake.grdReMake(fpSpread1, i,
                            "1" + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|3"
                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|3");
                    }
                    else
                    {

                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Value.ToString().Trim() == "P")
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i,
                                "1" + "|3"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|1"
                                + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|3");

                        }
                        else
                        {

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전송여부")].Text.ToString().Trim() == "미발행")
                            {
                                UIForm.FPMake.grdReMake(fpSpread1, i,
                                    "1" + "|0"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|1"
                                    + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|0");
                            }
                            else
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전송여부")].Text.ToString().Trim() == "전송완료")
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서상태")].Text.ToString().Trim() == "반려")
                                    {
                                        UIForm.FPMake.grdReMake(fpSpread1, i,
                                            "1" + "|0"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|1"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|1"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|1"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|1"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|1"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|1");
                                    }
                                    //								else if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서상태")].Text.ToString().Trim() == "승인" ||
                                    //									fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서상태")].Text.ToString().Trim() == "승인취소" ||
                                    //									fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서상태")].Text.ToString().Trim() == "개봉" )
                                    //								{
                                    //									UIForm.FPMake.grdReMake(fpSpread1, i, 
                                    //										"1" + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|3"
                                    //										+ "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|3");
                                    //								}
                                    else
                                    {
                                        UIForm.FPMake.grdReMake(fpSpread1, i,
                                            "1" + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|3"
                                            + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|3");
                                    }
                                }
                                else
                                {
                                    UIForm.FPMake.grdReMake(fpSpread1, i,
                                        "1" + "|0"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "발행일") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "공급가액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "부가세금액") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2") + "|3"
                                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "적요") + "|3");
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
        #region 전자세금계산서발행
        private void btnSave_Click(object sender, System.EventArgs e)
        {

            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0 && fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                            {
                                string strSql = " usp_XSB001 @pTYPE = 'I1'";
                                strSql += ", @pTAX_NO   = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서번호")].Text + "' ";
                                strSql += ", @pOBJ      = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "적요")].Text + "' ";
                                strSql += ", @pCLOSE_FG = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "마감여부")].Text + "' ";
                                strSql += ", @pUP_ID    = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region 발행취소
        private void btnDelTax_Click(object sender, System.EventArgs e)
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0 && fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                            {
                                string strSql = " usp_XSB001 @pTYPE = 'D1'";
                                strSql += ", @pTAX_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서번호")].Text + "' ";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region 반려요청
        private void btnReturn_Click(object sender, System.EventArgs e)
        {
            fpSpread1.Focus();

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //그리드 상단 필수 체크
                    if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;

                            if (strHead.Length > 0 && fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                            {
                                string strSql = " usp_XSB001 @pTYPE = 'R1'";
                                strSql += ", @pTAX_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세금계산서번호")].Text + "' ";
                                strSql += ", @pUP_ID  = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();
                if (ERRCode == "OK")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Cursor = Cursors.Default;
            }

        }
        #endregion

        #region 조회조건 팝업
        private void butCust_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW002 pu = new WNDW002(txtCustCd.Text, "S");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtCustCd.Text = Msgs[1].ToString();
                    txtCustNm.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTaxBiz_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'BIZ_CD', @pSPEC2 = 'BIZ_NM', @pSPEC3 = 'B_BIZ_PLACE', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtTaxBizCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00010", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTaxBizCd.Text = Msgs[0].ToString();
                    txtTaxBizNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사업장 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged
        private void txtCustCd_TextChanged(object sender, EventArgs e)
        {
            txtCustNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtCustCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtTaxBizCd_TextChanged(object sender, EventArgs e)
        {
            txtTaxBizNm.Value = SystemBase.Base.CodeName("BIZ_CD", "BIZ_NM", "B_BIZ_PLACE", txtTaxBizCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 그리드상 체크박스, 버튼 선택시
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형_2"))
            {

                string strQuery = " usp_B_COMMON @pTYPE ='COMM_POP', @pLANG_CD = 'KOR', @pSPEC1 = 'B040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00103", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계산서유형");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    ChangeChkBox(e.Column, e.Row);

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text = Msgs[0].ToString();
                    fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형명")].Text = Msgs[1].ToString();

                }
            }
            else
            {
                ChangeChkBox(e.Column, e.Row);
            }

        }
        #endregion

        #region 그리드 상 데이터 변경시 연계데이터 자동입력
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //품목코드
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형"))
            {
                string Query = " usp_B_COMMON @pTYPE = 'B060', @pCODE = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

                if (dt.Rows.Count > 0)
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text = dt.Rows[0][0].ToString();
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형명")].Text = dt.Rows[0][1].ToString();

                }
                //				else
                //				{
                //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형")].Text = "";
                //					fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계산서유형명")].Text = "";
                //				}
            }


        }
        #endregion

        #region 체크선택시 수정플레그 변경
        private void ChangeChkBox(int Col, int Row)
        {
            try
            {
                if (Col == 1)
                {

                    if (fpSpread1.Sheets[0].Cells[Row, 1].Text != "False")
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "U";

                    }
                    else
                    {
                        fpSpread1.Sheets[0].RowHeader.Cells[Row, 0].Text = "";

                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "수정플래그등록"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 전체선택클릭시
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                if (fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).CellType != null)
                {
                    if (e.ColumnHeader == true)
                    {
                        if (fpSpread1.Sheets[0].ColumnHeader.Cells[0, e.Column].Text == "True")
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = true;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = true;
                                    ChangeChkBox(e.Column, i);
                                }
                            }
                        }
                        else
                        {
                            fpSpread1.Sheets[0].ColumnHeader.Cells.Get(0, e.Column).Value = false;
                            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                            {
                                if (fpSpread1.Sheets[0].Cells[i, e.Column].Locked == false)
                                {
                                    fpSpread1.Sheets[0].Cells[i, e.Column].Value = false;
                                    ChangeChkBox(e.Column, i);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
