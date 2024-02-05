#region 작성정보
/*********************************************************************/
// 단위업무명 : 가공품실무게관리
// 작 성 자 : 김현근
// 작 성 일 : 2013-06-05
// 작성내용 : 가공품실무게관리 및 조회
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


namespace IBF.IBFB07U
{
    public partial class IBFB07U : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private bool chk = false;
        public IBFB07U()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBFB07U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
            SystemBase.Validation.GroupBox_Setting(groupBox2);	//컨트롤 필수 Setting
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            dtpDT.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Base.GroupBoxReset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                     string strQuery = " usp_IBFB07U  'S1',";
                     strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                     strQuery = strQuery + " @pUSE_CREATE_NO = '" + txtBASED_NO.Text + "'";
                     strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                     UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                     if (fpSpread1.Sheets[0].Rows.Count > 0) Spread_Sum();
                 }
             }
             catch (Exception f)
             {
                 SystemBase.Loggers.Log(this.Name, f.ToString());
                 MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
             }

            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        private void Spread_Sum()
        {
            decimal amt = 0;

            dtpDT.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고일자")].Text;
            txtNOTIFY_NO.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고번호")].Text;
            txtOUT_BASED_NO.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "반출근거번호")].Text;
            txtINVOICE_NO.Text = fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "반출송장번호")].Text;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                amt += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Text);
            }
            txtAMT.Value = amt;
        }

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            //Major 코드 필수항목 체크
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
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
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                case "I": strGbn = "I1"; break;
                                default: strGbn = ""; break;
                            }

                            string strQuery = " usp_IBFB07U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No")].Text + "'";
                            strQuery = strQuery + ", @pUSE_CREATE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "실소요량생성번호")].Text + "'";
                            strQuery = strQuery + ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text + "'";
                            strQuery = strQuery + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고일자")].Text + "'";
                            strQuery = strQuery + ", @pNOTIFY_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고번호")].Text + "'";
                            strQuery = strQuery + ", @pOUT_BASED_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출근거번호")].Text + "'";
                            strQuery = strQuery + ", @pINVOICE_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출송장번호")].Text + "'";
                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            else { int intRows = fpSpread_ReType(fpSpread1, strGbn, i); i = intRows; }
                        }
                    }
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
            }
        }
        #endregion

        #region 행쿼리후 그리드 재정의
        private static int fpSpread_ReType(FarPoint.Win.Spread.FpSpread baseGrid, string strGbn, int intRow)
        {
            if (strGbn == "U1")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                return intRow;
            }
            else if (strGbn == "I1")
            {
                baseGrid.Sheets[0].RowHeader.Cells[intRow, 0].Text = "";
                UIForm.FPMake.grdReMake(baseGrid, intRow, "1|3");
                return intRow;
            }
            else if (strGbn == "D1")
            {
                baseGrid.Sheets[0].Rows[intRow].Remove();
                return intRow - 1;
            }
            else return 0;
        }
        #endregion

        #region 팝업창 열기
        private void btnTRNo_Click(object sender, EventArgs e)
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
                MessageBox.Show(f.ToString());
            }
        }

        private void butBASED_NO_Click(object sender, EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF24' ";
                string[] strWhere = new string[] { "@pSPEC" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP013", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원재료실소요량 근거번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBASED_NO.Text = Msgs[2].ToString();

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

        #endregion

        private void butPacking_Ref_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtTRNo.Text != "" && txtBASED_NO.Text != "")
                {
                    IBFB07P frm = new IBFB07P(txtTRNo.Text, txtBASED_NO.Text);
                    frm.ShowDialog();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void butCompute_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고일자")].Text = dtpDT.Text;
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출신고번호")].Text = txtNOTIFY_NO.Text;
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출근거번호")].Text = txtOUT_BASED_NO.Text;
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반출송장번호")].Text = txtINVOICE_NO.Text;
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "U";
            }

        }

        private void IBFB07U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB07U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }

        private void txtBASED_NO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec(); 
        }

        private void txtTRNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From dbo.UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
                    strSql += " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Text = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Text = ds.Tables[0].Rows[0][1].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

    }
}
