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

namespace MX.MEX001
{
    public partial class MEX001P1 : UIForm.FPCOMM2_2
    {
        #region 변수선언
        string returnVal;
        string strExpNo;
        string strIs = "N";
        string cfmYN = "N";
        int actRow = 0;
        decimal xrate = 1;
        decimal amt = 0;
        decimal amt_loc = 0;
        string strEspRefNo = ""; // 
        string steps = "";
        string stepsNm = "";
        bool tempID = false;
        #endregion

        #region 생성자
        public MEX001P1()
        {
            InitializeComponent();
        }

        public MEX001P1(string ExpNo, string ExpStep, string ExpStepNm, string Is, decimal amt_loc1, decimal xrate1, string cfm, decimal amt1, bool div)
        {
            InitializeComponent();

            strExpNo = ExpNo;
            steps = ExpStep;
            stepsNm = ExpStepNm;

            strIs = Is;

            xrate = xrate1;
            amt_loc = amt_loc1;
            cfmYN = cfm;
            amt = amt1;
            tempID = div;
        }

        public MEX001P1(string ExpNo, string ExpStep, string ExpStepNm, string Is, decimal amt_loc1, decimal xrate1, string cfm, decimal amt1)
        {
            InitializeComponent();

            strExpNo = ExpNo;
            steps = ExpStep;
            stepsNm = ExpStepNm;

            strIs = Is;

            xrate = xrate1;
            amt_loc = amt_loc1;
            cfmYN = cfm;
            amt = amt1;
        }
        #endregion
        
        #region 폼로드 이벤트
        private void MEX001P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "발생근거번호 팝업";

            SystemBase.Validation.GroupBox_Setting(groupBox1);
            if (cfmYN == "True")
                UIForm.Buttons.ReButton("010000001001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            else
                UIForm.Buttons.ReButton("011111111001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            txtExpSteps.Value = steps;
            txtExpStepsNm.Value = stepsNm;

            txtTotAmtLoc.Value = Convert.ToInt32(amt);
            txtTotAmtLoc.ReadOnly = true;
            txtTotAmtLoc.BackColor = SystemBase.Validation.Kind_Gainsboro;

            if (strIs == "Y") SearchExec();
        }
        #endregion
        
        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            bool is_chang = true;
            try
            {
                if (fpSpread2.Focused == true)
                {
                    fpSpread1.Sheets[0].RowCount = 0;
                    UIForm.FPMake.RowInsert(fpSpread2);
                }
                else
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        if (strHead == "I" || strHead == "U" || strHead == "D")
                        {
                            is_chang = false;
                            break;
                        }
                    }
                    if (is_chang) UIForm.FPMake.RowInsert(fpSpread1);
                    else MessageBox.Show("발생근거번호를 먼저 저장하세요!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                RowInsExe();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            Search("");
        }


        private void Search(string strRefNo)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                fpSpread1.Sheets[0].RowCount = 0;

                string strQuery = " usp_MEX001  @pTYPE = 'S2'";
                strQuery += ", @pEXP_NO = '" + strExpNo + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, true, true, 0, 0);
                UIForm.FPMake.grdReMake(fpSpread2, fpSpread2.Sheets[0].ColumnCount, 3);

                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    int x = 0, y = 0;

                    if (strRefNo != "")
                    {
                        fpSpread2.Search(0, strRefNo, false, false, false, false, 0, 0, ref x, ref y);

                        if (x > 0)
                        {
                            fpSpread2.Sheets[0].SetActiveCell(x, y);
                        }
                        else
                        {
                            x = 0;
                        }

                    }
                    fpSpread2.Sheets[0].AddSelection(x, 1, 1, fpSpread2.Sheets[0].ColumnCount);
                    strEspRefNo = fpSpread2.Sheets[0].Cells[x, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text;

                    //상세정보조회
                    SubSearch(strEspRefNo);
                }
                else
                {
                    strEspRefNo = "";
                    NewExec();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"));
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }


        private void SubSearch(string strCode)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                fpSpread1.Sheets[0].RowCount = 0;
                string strQuery = " usp_MEX001  @pTYPE = 'S3'";
                strQuery += ", @pEXP_NO = '" + strExpNo + "' ";
                strQuery += ", @pEXP_REF_NO = '" + strCode + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0);
                UIForm.FPMake.grdReMake(fpSpread1, fpSpread1.Sheets[0].ColumnCount, 3);
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
            string div = "";
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = "";
                bool fp2 = false;
                bool fp2_insert = false;
                bool fp1 = false;
                bool fp1_insert = false;

                //if (UIForm.FPMake.FPUpCheck(fpSpread2, false) == true)
                if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread2, this.Name, "fpSpread2", false))
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread2.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U2"; break;
                                case "I": strGbn = "I2"; fp2_insert = true; break;
                                case "D": strGbn = "D2"; break;
                                default: strGbn = ""; break;
                            }
                            if (strGbn == "") continue;
                            strSql = " usp_MEX001 '" + strGbn + "'";
                            strSql += ", @pEXP_STEPS = '" + steps + "' ";
                            strSql += ", @pEXP_NO = '" + strExpNo + "' ";
                            strSql += ", @pEXP_REF_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text + "'";
                            
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            div = "1";
                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


                            if (fp2_insert && tempID == false)
                            {
                                strSql = " usp_MEX001 'T4'";  //I5 
                                strSql += ", @pEXP_STEPS = '" + steps + "'";
                                strSql += ", @pEXP_NO = '" + strExpNo + "' ";
                                strSql += ", @pEXP_REF_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                            fp2 = true; fp2_insert = false;
                        }
                    }
                }
                //else if (UIForm.FPMake.FPUpCheck(fpSpread1, false) == true)
                else if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false))
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
                                case "U": strGbn = "U3"; break;
                                case "I": strGbn = "I3"; fp1_insert = true; break;
                                case "D": strGbn = "D3"; break;
                                default: strGbn = ""; break;
                            }
                            if (strGbn == "") continue;

                            strSql = " usp_MEX001 '" + strGbn + "'";
                            strSql += ", @pEXP_STEPS = '" + steps + "'";
                            strSql += ", @pEXP_NO = '" + strExpNo + "' ";
                            strSql += ", @pEXP_REF_NO = '" + strEspRefNo + "'";
                            strSql += ", @pPROJECT_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "'";
                            strSql += ", @pPROJECT_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text + "'";
                            //								strSql += ", @pEXP_REF_AMT_LOC = '" +  fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생근거금액")].Value + "'";					
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds3.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds3.Tables[0].Rows[0][1].ToString();
                            div = "2";
                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            if (fp1_insert && tempID == false)
                            {
                                strSql = " usp_MEX001 'T4'";  //I6
                                strSql += ", @pEXP_STEPS = '" + steps + "'";
                                strSql += ", @pEXP_NO = '" + strExpNo + "' ";
                                strSql += ", @pEXP_REF_NO = '" + fpSpread2.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text + "'";
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds4.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds4.Tables[0].Rows[0][1].ToString();
                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                            fp1 = true; fp1_insert = false;

                        }
                    }
                }

                Trans.Commit();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "OK")
            {
                if (div == "1") SearchExec();
                else SubSearch(strEspRefNo);

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
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 그리드 버튼 클릭
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호_2"))
            {
                try
                {
                    string strExpSteps = txtExpSteps.Text;
                    //발주
                    if (strExpSteps == "PO")
                    {
                        WNDW.WNDW018 pu = new WNDW.WNDW018();
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text = Msgs[1].ToString();
                        }
                    }
                    else if (strExpSteps == "VB") // 수입선적
                    {
                        WNDW.WNDW022 pu = new WNDW.WNDW022();
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text = Msgs[2].ToString();
                        }
                    }
                    else if (strExpSteps == "VD") // 통관
                    {
                        WNDW.WNDW023 pu = new WNDW.WNDW023();
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text = Msgs[1].ToString();
                        }
                    }
                    else if (strExpSteps == "VL") // L/C
                    {
                        WNDW.WNDW021 pu = new WNDW.WNDW021();
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text = Msgs[1].ToString();
                        }
                    }
                    else if (strExpSteps == "VO") // LOCAL L/C
                    {
                        WNDW.WNDW021 pu = new WNDW.WNDW021();
                        pu.ShowDialog();
                        if (pu.DialogResult == DialogResult.OK)
                        {
                            string[] Msgs = pu.ReturnVal;

                            fpSpread2.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text = Msgs[1].ToString();
                        }
                    }
                    fpSpread2.Select();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.

                }
            }
        }

        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호_2"))
            {
                try
                {
                    string strQuery = " usp_M_COMMON 'P004' , @pSPEC1 = '" + strEspRefNo + "', @pSPEC3 ='" + steps + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";	// 쿼리
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "", "" };
                    
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[3].ToString();
                        UIForm.FPMake.fpChange(fpSpread1, e.Row);//수정플래그
                    }
                    fpSpread1.Select();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
            //프로젝트번호차수
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수_2"))
            {
                try
                {
                    string strQuery = " usp_M_COMMON 'P005' , @pSPEC1 = '" + strEspRefNo + "' , @pSPEC2 = '" + fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트번호")].Text + "', @pSPEC3 ='" + steps + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";	// 쿼리
                    string[] strWhere = new string[] { "@pCODE" };			// 쿼리 인자값(조회조건)
                    string[] strSearch = new string[] { "" };
                    
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00094", strQuery, strWhere, strSearch, new int[] { 0 }, "프로젝트차수 조회", false);
                    pu.Width = 400;
                    pu.ShowDialog();	//공통 팝업 호출

                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string MSG = pu.ReturnVal.Replace("|", "#");
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(MSG);

                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트차수")].Text = Msgs[0].ToString();
                    }
                    fpSpread1.Select();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.
                }
            }
        }
        #endregion
        
        #region 버튼 Click
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            int rowcnt = 0;
            for (int i = 0; i < fpSpread2.Sheets[0].RowCount; i++)
            {
                if (fpSpread2.Sheets[0].RowHeader.Cells[i, 0].Text != "I") rowcnt++;
            }
            if (rowcnt > 0) RtnStr("Y");
            else RtnStr("N");

            this.Close();
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 값 전송
        public string ReturnVal { get { return returnVal; } set { returnVal = value; } }

        public void RtnStr(string strCode)
        {
            returnVal = strCode;
        }
        #endregion

        #region fpSpread2_LeaveCell
        private void fpSpread2_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            if (actRow != e.NewRow && fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text != "")
                SubSearch(fpSpread2.Sheets[0].Cells[e.NewRow, SystemBase.Base.GridHeadIndex(GHIdx2, "발생근거번호")].Text);
            actRow = e.NewRow;
        }
        #endregion

    }
}
