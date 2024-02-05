#region 작성정보
/*********************************************************************/
// 단위업무명 : TOUCH 수동 일마감 전개/취소
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-10
// 작성내용 : TOUCH 수동 일마감 전개/취소
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
using System.Text.RegularExpressions;

namespace PE.PEA007
{
    public partial class PEA007 : Form
    {
        public PEA007()
        {
            InitializeComponent();
        }

        #region  폼 로드시
        private void PEA007_Load(object sender, EventArgs e)
        {
            this.Text = SystemBase.Base.RodeFormText;

            dtpWorkDt.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 화면 활성화시 이벤트
        private void PEA007_Activated(object sender, EventArgs e)
        {
            SystemBase.Base.RodeFormName = this.Name;
        }
        #endregion

        #region 닫기
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 전개
        private void btnTouchProc_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                string strSql = " usp_PEA007 'I1'";
                strSql += ", @pWORK_DT = '" + dtpWorkDt.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
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

        #region 팝업창
        //작업자
        private void btnWorkDuty_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text == "")
                {
                    MessageBox.Show("소속 작업장이 선택되지 않았습니다. 작업장을 먼저 선택하십시오.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string strQuery = " usp_P_COMMON @pTYPE = 'P121', @pETC = '" + txtWc_Cd.Text + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";				// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWorkDutyId.Text, "" };			// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00071", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWorkDutyId.Text = Msgs[0].ToString();
                    txtWorkDutyNm.Value = Msgs[1].ToString();
                    txtWorkDutyId.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //작업장
        private void btnWc_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pTYPE = 'P042', @pLANG_CD = 'KOR', @pETC = 'P061' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";					// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };					// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtWc_Cd.Text, "" };								// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회", false);
                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtWc_Cd.Text = Msgs[0].ToString();
                    txtWc_Nm.Value = Msgs[1].ToString();
                    txtWc_Cd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Touch 강제마감
        private void btnWorkClose_Click(object sender, EventArgs e)
        {
            if (txtWORKORDER_NO.Text == "" && TXTINSPREQ_NO.Text == "")
            {

                if (txtWc_Cd.Text == "")
                {
                    MessageBox.Show("작업장은 필수 입력 입니다.", "TOUCH 강제 마감", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtWorkDutyId.Text == "")
                {
                    MessageBox.Show("작업자는 필수 입력 입니다.", "TOUCH 강제 마감", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                string strSql = " usp_PEA007 'I2'";
                strSql += ", @pWC_CD = '" + txtWc_Cd.Text + "' ";
                strSql += ", @pWORK_DUTY = '" + txtWorkDutyId.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strSql += ", @pWORKORDER_NO = '" + txtWORKORDER_NO.Text + "' ";
                strSql += ", @pINSPREQ_NO = '" + TXTINSPREQ_NO.Text + "' ";
                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
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


        #region 코드입력시 코드명 자동입력
        //작업자
        private void txtWorkDutyId_TextChanged(object sender, System.EventArgs e)
        {

            try
            {
                if (txtWorkDutyId.Text != "")
                {
                    txtWorkDutyNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkDutyId.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWorkDutyNm.Value = "";
                }
            }
            catch
            {

            }
        }
        //작업장
        private void txtWc_Cd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtWc_Cd.Text != "")
                {
                    txtWc_Nm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWc_Cd.Text, " AND MAJOR_CD = 'P061'  AND COMP_CODE='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtWc_Nm.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}
