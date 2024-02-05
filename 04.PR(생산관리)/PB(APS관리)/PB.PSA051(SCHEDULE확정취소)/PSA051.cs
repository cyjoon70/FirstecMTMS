#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 확정취소
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-04
// 작성내용 : SCHEDULE 확정취소
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

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
using WNDW;

namespace PB.PSA051
{
    public partial class PSA051 : UIForm.FPCOMM2
    {
        string strSchNo = "";

        public PSA051()
        {
            InitializeComponent();
        }

        public PSA051(string Div)
        {
            strSchNo = Div;
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA051_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);

            SystemBase.ComboMake.C1Combo(cboSch_Type, "usp_P_COMMON @pTYPE = 'P040',@pLANG_CD = 'KOR', @pCOM_CD = 'P058', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");  //SCH TYPE

            cboSch_Type.SelectedValue = "S";

            if (strSchNo != "")
            {
                txtSch_No.Text = strSchNo;
                SearchExec();
            }
        }
        #endregion

        #region 스케쥴 NO 팝업 조회
        private void btnSch_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_PSA051 'P1' ,@pSCH_TYPE='" + cboSch_Type.SelectedValue.ToString() + "',  @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pSCH_NO" };
                string[] strSearch = new string[] { "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("PSA026P1", strQuery, strWhere, strSearch, new int[] { 0 });
                pu.Width = 1200;
                pu.FormBorderStyle = FormBorderStyle.Sizable;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSch_No.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
            UIForm.FPMake.grdCommSheet(fpSpread2, null, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_PSA051 @pTYPE = 'S1'";
                    strQuery += ",            @pSCH_NO = '" + txtSch_No.Text + "' ";
                    strQuery += ",            @pSCH_TYPE = '" + cboSch_Type.SelectedValue.ToString() + "' ";
                    strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, false, true, 0, 0);

                    if (fpSpread2.Sheets[0].Rows.Count > 0)
                    {
                        int Row = 0;
                        SubSearch(Row);
                    }
                    else
                    {
                        fpSpread1.Sheets[0].Rows.Count = 0;
                        txtSch_No.Text = "";
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region fpSpread2 Select 이벤트
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            int Row = 0;
            if (fpSpread2.Sheets[0].Rows.Count > 0)
            {
                Row = fpSpread2.Sheets[0].ActiveRowIndex;

                SubSearch(Row);
            }
            else
            {
                Row = 0;
            }
        }
        #endregion

        #region fpSpread1 조회
        private void SubSearch(int Row)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = "";
                    strQuery = "   usp_PSA051 @pTYPE = 'S2'";
                    strQuery += ",            @pSCH_NO = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_NO")].Text + "' ";
                    strQuery += ",            @pSCH_ID = '" + fpSpread2.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx2, "SCH_ID")].Text + "' ";
                    strQuery += ",            @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string Query1 = "";
                            Query1 = " SELECT 1 FROM P_MRP_RESULT_DETAIL(NOLOCK) WHERE CO_CD='" + SystemBase.Base.gstrCOMCD + "' AND MAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' ";
                            DataTable dt1 = new DataTable();
                            dt1 = SystemBase.DbOpen.NoTranDataTable(Query1);

                            if (dt1.Rows.Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].Columns.Count - 1].ForeColor = Color.Blue;
                                fpSpread1.Sheets[0].Cells[i, 1].Locked = true;
                            }

                            string Query = "";
                            Query = " SELECT 1 FROM P_WORKORDER_MASTER(NOLOCK) WHERE CO_CD='" + SystemBase.Base.gstrCOMCD + "' AND MAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' AND ISNULL(REPORT_DT,'') <> '' ";
                            DataTable dt = new DataTable();
                            dt = SystemBase.DbOpen.NoTranDataTable(Query);

                            if (dt.Rows.Count > 0)
                            {
                                fpSpread1.Sheets[0].Cells[i, 0, i, fpSpread1.Sheets[0].Columns.Count - 1].ForeColor = Color.Red;
                                fpSpread1.Sheets[0].Cells[i, 1].Locked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region SaveExec() 그리드 저장 로직
        protected override void SaveExec()
        {
            string fcsStr = "";
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                    {
                        fcsStr = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text;

                        string strQuery = "";
                        strQuery = "   usp_PSA051 @pTYPE = 'D1'";
                        strQuery += ",            @pSCH_NO = '" + txtSch_No.Text + "' ";
                        strQuery += ",            @pSCH_TYPE = '" + cboSch_Type.SelectedValue.ToString() + "' ";
                        strQuery += ",            @pMAKEORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "제품오더번호")].Text + "' ";
                        strQuery += ",            @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                        strQuery += ",            @pSCH_ID = '" + fpSpread2.Sheets[0].Cells[0, 2].Text + "' "; ;
                        strQuery += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK")
                        {
                            Trans.Rollback();
                            goto Exit;
                        }	// ER 코드 Return시 점프
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
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
                UIForm.FPMake.GridSetFocus(fpSpread1, fcsStr); //저장 후 그리드 포커스 이동
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

    }
}
