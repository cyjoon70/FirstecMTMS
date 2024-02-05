

#region 작성정보
/*********************************************************************/
// 단위업무명 : 결산마감및이월
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-07
// 작성내용 : 결산마감및이월
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

namespace AE.ACE009
{
    public partial class ACE009 : UIForm.Buttons
    {
        public ACE009()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACE009_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtMaxCloseMM.Value = SystemBase.Base.CodeName("CO_CD", "MAX(CLOSE_YYMM)", "A_SLIP_MONTH_CLOSE", SystemBase.Base.gstrCOMCD, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");

            if (txtMaxCloseMM.Text != "")
            {
                DateTime DtMaxClose = Convert.ToDateTime(txtMaxCloseMM.Text.Substring(0, 4) + "-" + txtMaxCloseMM.Text.Substring(4, 2) + "-01");

                dtpYYMM_FR.Value = DtMaxClose.AddMonths(1).ToShortDateString();
                dtpYYMM_TO.Value = DtMaxClose.AddMonths(1).ToShortDateString(); ;
            }
            dtpYYMM_TO.Focus();
        }
        #endregion

        #region 실행버튼 클릭
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {

                        string strQuery = " usp_ACE009  '" + SystemBase.Base.gstrCOMCD + "' ";
                        strQuery += ", @pYYMM_FR = '" + dtpYYMM_FR.Text.Replace("-", "") + "' ";
                        strQuery += ", @pYYMM_TO = '" + dtpYYMM_TO.Text.Replace("-", "") + "' ";
                        if (optWORK_DIV1.Checked == true) strQuery += ", @pWORK_DIV = '1' ";
                        else if (optWORK_DIV2.Checked == true) strQuery += ", @pWORK_DIV = '2' ";
                        strQuery = strQuery + ", @pEMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                        strQuery = strQuery + ", @pIP_NO = '" + SystemBase.Base.gstrUserIp + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();
                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

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
                    NewExec();
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

            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}
