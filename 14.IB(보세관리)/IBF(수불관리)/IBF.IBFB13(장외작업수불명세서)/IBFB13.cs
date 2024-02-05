#region 작성정보
/*********************************************************************/
// 단위업무명 : 장외작업수불명세서
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-11
// 작성내용 : 장외작업수불명세서 관리
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

namespace IBF.IBFB13
{
    public partial class IBFB13 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB13()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB13_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[7];
            string[] FormulaField2 = new string[2]; //formula 값
            string[] FormulaField1 = new string[2]; //formula 이름


            string RptName = "";

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            RptName = @"Report\" + "IBFB28P.rpt";

            RptParmValue[0] = "R1";

            RptParmValue[1] = txtTRNo.Text;

            if (txtBizPartner.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = txtBizPartner.Text;

            if (txtJOB_CD.Text.Trim() == "") RptParmValue[3] = " ";
            else RptParmValue[3] = txtJOB_CD.Text;

            if (dtpDT_FR.Text.Trim() == "") RptParmValue[4] = " ";
            else RptParmValue[4] = dtpDT_FR.Text;

            if (dtpDT_TO.Text.Trim() == "") RptParmValue[5] = " ";
            else RptParmValue[5] = dtpDT_TO.Text;

            RptParmValue[6] = SystemBase.Base.gstrCOMCD;

            FormulaField2[0] = "\"" + txtBUSINESS_NM.Text + "\"";
            FormulaField1[0] = "BUSI_NM";

            FormulaField2[1] = "\"" + txtBizPartner.Text + "\"";
            FormulaField1[1] = "CUST_NM";

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", FormulaField2, FormulaField1, RptName, RptParmValue);
            frm.ShowDialog();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Base.GroupBoxExceptions(groupBox1))
            {
                try
                {

                    string strQuery = " usp_IBFB13  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "', ";
                    strQuery = strQuery + " @pBP_CD = '" + txtBizPartner.Text + "', ";
                    strQuery = strQuery + " @pJOB_CD = '" + txtJOB_CD.Text + "', ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "',";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

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

        #region 버튼 Click
        private void btnBizPartner_Click(object sender, System.EventArgs e)
        {
            try
            {
                //외주처. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF03' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtBizPartner.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP003", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "외주처 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBizPartner.Text = Msgs[0].ToString();
                    txtBizPartnerNm.Value = Msgs[1].ToString();
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

        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
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
                }
                this.Cursor = Cursors.Default;
                getBUSINESS_NM();
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (MessageBox.Show("선택한 Tracking No.의 장외작업수불명세서를 생성하시겠습니까?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Cursor = Cursors.WaitCursor;
                    string RtnMsg = SystemBase.Base.MessageRtn("B0042");
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strQuery = "usp_IBFB13_P ";
                        strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                        strQuery = strQuery + " @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "', ";
                        strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        cmd.CommandText = strQuery;

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                        string strRETURN = ds.Tables[0].Rows[0][0].ToString();
                        string strMSG_CD = ds.Tables[0].Rows[0][1].ToString();

                        if (strRETURN == "ER")
                        {
                            MessageBox.Show(strMSG_CD, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Trans.Rollback();
                            goto exit;
                        }

                        Trans.Commit();
                        this.Cursor = Cursors.Default;

                    exit:
                        dbConn.Close();

                        SearchExec();

                    }

                    catch (Exception f)
                    {
                        Trans.Rollback();
                        RtnMsg = "에러가 발생되어 롤백되었습니다.\n\r\n\r" + f.ToString();
                    }
                    this.Cursor = Cursors.Default;
                    dbConn.Close();
                    MessageBox.Show(RtnMsg, SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void butJOB_Click(object sender, System.EventArgs e)
        {
            try
            {
                //외주공정 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF20' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtJOB_CD.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "외주처 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtJOB_CD.Text = Msgs[0].ToString();
                    txtJOB_NM.Value = Msgs[1].ToString();
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
        #endregion

        #region TextChanged
        private void txtBizPartner_TextChanged(object sender, System.EventArgs e)
        {
            txtBizPartnerNm.Value = SystemBase.Base.CodeName("CUST_CD", "CUST_NM", "B_CUST_INFO", txtBizPartner.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }

        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            getBUSINESS_NM();
        }

        private void getBUSINESS_NM()
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
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
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtJOB_CD_TextChanged(object sender, System.EventArgs e)
        {
            txtJOB_NM.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtJOB_CD.Text, " AND  MAJOR_CD= 'P001' AND CD_NM LIKE '외주%' AND COMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ");

        }

        private void dtpDT_FR_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_FR.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_FR.Text) == false)
//                {
//                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_FR.Focus();
//                    dtpDT_FR.SelectAll();
//                }
            }
        }

        private void dtpDT_TO_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_TO.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_TO.Text) == false)
//                {
//                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_TO.Focus();
//                    dtpDT_TO.SelectAll();
//                }
            }
        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtBizPartner_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtJOB_CD_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB13_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB13_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}








