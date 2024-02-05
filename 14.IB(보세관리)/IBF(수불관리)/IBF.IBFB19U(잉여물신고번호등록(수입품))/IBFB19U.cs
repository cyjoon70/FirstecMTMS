#region 작성정보
/*********************************************************************/
// 단위업무명 : 잉여물신고번호등록(수입품)
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-11
// 작성내용 : 잉여물신고번호등록(수입품) 및 관리
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

namespace IBF.IBFB19U
{ 
    public partial class IBFB19U : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB19U()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFB19U_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            //dtpDT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-3).ToString().Substring(0,10);
            //dtpDT_TO.Text = SystemBase.Base.ServerTime("YYMMDD");
            //dtpDT.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[7];


            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            string RptName = @"Report\" + "IBFB19P.rpt";
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

            RptParmValue[6] = SystemBase.Base.gstrCOMCD;

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
                    string strQuery = " usp_IBFB19U  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pMATTER_CD = '" + txtMATTER_CD.Text + "', ";
                    strQuery = strQuery + " @pNOTIFY_NO = '" + txtNOTIFY_NO.Text + "'  ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
                  
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true)) // 그리드 상단 필수항목 체크
            {
                string ERRCode = "", MSGCode = "", MSGNM = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";
                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "U": strGbn = "U1"; break;   //수정
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB19U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text + "'";
                            strQuery = strQuery + ", @pUSE_CREATE_NO       = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실소요량생성번호")].Text + "'";
                            strQuery = strQuery + ", @pITEM_CD1 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품코드")].Text + "'";
                            strQuery = strQuery + ", @pITEM_CD2 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목코드")].Text + "'";
                            strQuery = strQuery + ", @pITEM_CD3= '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "원자재코드")].Text + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입통관번호")].Text.Trim() != "")
                                strQuery = strQuery + ", @pCC_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입통관번호")].Text + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관일")].Text.Trim() != "")
                                strQuery = strQuery + ", @pCC_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관일")].Text + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();
                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        }
                    }
                    Trans.Commit();
                    if (txtTRNo.Text.Trim() != "") SearchExec();
                    this.Cursor = Cursors.Default;
                }
                catch (Exception f)
                {
                    Trans.Rollback();
                    MSGCode = "P0001";
                }
            Exit:
                this.Cursor = Cursors.Default;
                dbConn.Close();
                if (MSGNM.Length > 10) MessageBox.Show(MSGNM);
                else MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));


            }
        }
        #endregion

        #region Button Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF10' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Value = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        
        private void butMATTER_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " Nusp_BF_Comm 'BF06' ";

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
                MessageBox.Show(f.ToString());
            }
        }

        private void butNOTIFY_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF18' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME", "@pSPEC" };
                string[] strSearch = new string[] { txtNOTIFY_NO.Text, "", txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP012", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "반출번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtNOTIFY_NO.Value = Msgs[1].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void butCompute_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수입통관번호")].Value = txtCCNo.Text;
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관일")].Value = dtpDT.Text;
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
            }
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFB19U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB19U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

        #region TextBox event
        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From MTMS_FT.dbo.UVW_S_PROJECT_ENT  Where PROJECT_NO = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }


                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
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

        #region dtpDT_Leave
        private void dtpDT_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT.Text.Trim() != "")
            {
                if (IsDate(dtpDT.Text) == false)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
                    dtpDT.Focus();
                    dtpDT.SelectAll();
                }
            }
        }

        public static bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        }
        #endregion
    }
}
