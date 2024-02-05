#region 작성정보
/*********************************************************************/
// 단위업무명 : SCHEDULE 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-28
// 작성내용 : SCHEDULE 전개 및 관리
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
using System.Data.SqlClient;
using WNDW;

namespace PB.PSA010
{
    public partial class PSA010P4 : UIForm.FPCOMM1
    {
        private string SCH_ID; // 스케쥴 ID

        public PSA010P4(string pSCH_ID)
        {
            InitializeComponent();
            SCH_ID = pSCH_ID;
            // 그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }

        #region 폼로드
        private void PSA010P4_Load(object sender, EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//컨트롤 필수 Setting
        }
        #endregion

        #region 조회
        private void btnSrch_Click(object sender, System.EventArgs e)
        {
            srchOrderInfo();
        }

        private void srchOrderInfo()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                string strSql = " usp_PSA010P4 'S1'";

                strSql += ", @pSCH_ID = '" + SCH_ID + "'";
                strSql += ", @pPROJECT_NO = '" + txtProject_NO.Text + "'";
                strSql += ", @pITEM_CD = '" + txtItem_CD.Text + "'";
                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

                strSql = " usp_PSA010P4 'S2'";

                strSql += ", @pSCH_ID = '" + SCH_ID + "'";
                strSql += ", @pPROJECT_NO = '" + txtProject_NO.Text + "'";
                strSql += ", @pITEM_CD = '" + txtItem_CD.Text + "'";
                strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

                if (dt.Rows.Count > 0)
                {
                    txtRepsn_Ord_NO.Value = dt.Rows[0]["WORKORDER_NO_RS"].ToString();
                    txtOrder_cnt.Value = dt.Rows[0]["ORDER_QTY"].ToString();

                    txtRepsn_Ord_NO.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    txtRepsn_Ord_NO.ReadOnly = true;
                    txtOrder_cnt.BackColor = SystemBase.Validation.Kind_Gainsboro;
                    txtOrder_cnt.ReadOnly = true;

                    btnCreate.Enabled = false;
                    ButtonCancel.Enabled = true;
                }
                else
                {
                    txtRepsn_Ord_NO.Value = "";
                    txtOrder_cnt.Value = "0";

                    txtRepsn_Ord_NO.BackColor = SystemBase.Validation.Kind_White;
                    txtRepsn_Ord_NO.ReadOnly = false;
                    txtOrder_cnt.BackColor = SystemBase.Validation.Kind_White;
                    txtOrder_cnt.ReadOnly = false;

                    btnCreate.Enabled = true;
                    ButtonCancel.Enabled = false;
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

        #region 닫기
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 오더 통합
        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            string ERRCode = "ER";
            string MSGCode = "P0000";

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {

                    string strSql = "";
                    strSql = " usp_PSA010P4 'P1'";
                    strSql += ", @pWORKORDER_NO_RS = '" + txtRepsn_Ord_NO.Text + "'";
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    // 대표번호 생성
                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    // 대표 번호설정
                    txtRepsn_Ord_NO.Value = ds.Tables[0].Rows[0][2].ToString();

                    // 오류코드 재설정
                    ERRCode = "ER";
                    MSGCode = "P0000";

                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {

                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "False")
                            continue;

                        strSql = " usp_PSA010P4 'I1'";

                        strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql += ", @pPROJECT_NO  = '" + fpSpread1.Sheets[0].Cells[i, 3].Text + "'";
                        strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, 4].Text + "'";
                        strSql += ", @pORDER_QTY = '" + fpSpread1.Sheets[0].Cells[i, 5].Text + "'";
                        strSql += ", @pWORKORDER_NO = '" + fpSpread1.Sheets[0].Cells[i, 2].Text + "'";
                        strSql += ", @pWORKORDER_NO_OG = '" + fpSpread1.Sheets[0].Cells[i, 0].Text + "'";
                        strSql += ", @pWORKORDER_NO_RS = '" + txtRepsn_Ord_NO.Text + "'";
                        strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                        // 사용자 정보 셋팅 (제조번호생성을 위해)
                        strSql += ", @pUSR_ID    = '" + SystemBase.Base.gstrUserID + "' ";

                        ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }

                    Trans.Commit();
                    srchOrderInfo();
                }
                catch
                {
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";
                }
            Exit:
                dbConn.Close();
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information); 
            }

        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            string ERRCode = "ER";
            string MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {

                try
                {

                    string strSql = " usp_PSA010P4 'D1' ";
                    strSql += ", @pWORKORDER_NO_RS = '" + txtRepsn_Ord_NO.Text + "'";
                    strSql += ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    srchOrderInfo();

                }
                catch
                {
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = "P0001";
                }
            }

        Exit:
            dbConn.Close();
            MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information); 
        }
        #endregion
        
        #region 프로젝트 조회
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_NO.Text = Msgs[3].ToString();
                    txtProject_NM.Value = Msgs[4].ToString();
                    txtProject_SEQ.Value = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 품목코드 조회
        private void btnItem_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(SystemBase.Base.gstrPLANT_CD, true);
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItem_CD.Text = Msgs[2].ToString();
                    txtItem_NM.Value = Msgs[3].ToString();

                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region 오더 총 수량 계산
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == 1) // 선택 버튼을 클릭했을 경우
            {
                if (fpSpread1.Sheets[0].Cells[e.Row, 1].Text == "True")
                    txtOrder_cnt.Value = Convert.ToString(Convert.ToDouble(txtOrder_cnt.Text) + Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, 5].Text));
                else
                    txtOrder_cnt.Value = Convert.ToString(Convert.ToDouble(txtOrder_cnt.Text) - Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, 5].Text));
            }
        }
        #endregion

    }
}
