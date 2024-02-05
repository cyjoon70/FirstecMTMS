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


namespace IBB.IBFA04U
{
    public partial class IBFA04U : UIForm.FPCOMM1
    {
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private bool chk = false;
        public IBFA04U()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void IBFA04U_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
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
                     string strQuery = " usp_IBFA04U  'S1',";
                     strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "' ";		
                     strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                     UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                     fpSpread1.Sheets[0].SetColumnAllowAutoSort(2, true);
                 }
             }
             catch (Exception f)
             {
                 SystemBase.Loggers.Log(this.Name, f.ToString());
                 MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
             }

            this.Cursor = Cursors.Default;
        }
        #endregion

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

                            string strQuery = " usp_IBFA04U '" + strGbn + "'";
                            strQuery = strQuery + ", @pTRACKING_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text + "'";
                            strQuery = strQuery + ", @pBL_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L 번호")].Text + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사용여부")].Text.ToString() == "True") strQuery = strQuery + ", @pUSE_YN = 'Y' ";
                            else strQuery = strQuery + ", @pUSE_YN = 'N' ";

                            strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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

        #region 팝업창 열기
        private void btnTRNo_Click(object sender, EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
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
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void IBFA04U_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFA04U_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

        #region txtTRNo_KeyDown
        private void txtTRNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec(); 
        }
        #endregion

        #region btnCreate_Click
        private void btnCreate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (txtTRNo.Text.Trim() == "")
            {
                MessageBox.Show("먼저 Tracking No. 입력하세요!");
                txtTRNo.Focus();
                this.Cursor = Cursors.Default;
                return;
            }

            string ERRCode, MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                string strQuery = " usp_IBFA04U 'P1' ";
                strQuery = strQuery + ", @pTRACKING_NO = '" + txtTRNo.Text + "'";
                strQuery = strQuery + ", @pINSRT_USER_ID = '" + SystemBase.Base.gstrUserID + "'";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();

            }
            catch
            {
                Trans.Rollback();
                MSGCode = "P0019";
            }

        Exit:
            this.Cursor = Cursors.Default;
            dbConn.Close();
            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode));
            SearchExec();
        }
        #endregion

        #region txtTRNo_TextChanged
        private void txtTRNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtTRNo.Text == "")
                {
                    txtBUSINESS_CD.Value = "";
                    txtBUSINESS_NM.Value = "";
                }
            }
            catch { }
        }
        #endregion
    }
}
