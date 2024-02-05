

#region 작성정보
/*********************************************************************/
// 단위업무명 : 감가상각결과반영
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-19
// 작성내용 : 감가상각결과반영
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

namespace AH.ACH011
{
    public partial class ACH011 : UIForm.Buttons
    {
        public ACH011()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACH011_Load(object sender, System.EventArgs e)
        {
            NewExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            try
            {
                ////////////////////////////그룹박스 초기화 //////////////////////////////////////////////////////////////////////////
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);

                string strFISC_TO_DT = SystemBase.Base.CodeName("CO_CD", "FISC_TO_DT", "B_COMP_INFO", SystemBase.Base.gstrCOMCD, "  ");
                txtYYMM.Value = strFISC_TO_DT.Substring(0, 7);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                if (txtYYMM.Text == "")
                {
                    MessageBox.Show("기준년월을 확인하세요.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strDelSql = " usp_ACH011  ";
                    strDelSql += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strDelSql += ", @pYYMM = '" + txtYYMM.Text.Replace("-","") + "' ";
                    if (optRun.Checked == true) strDelSql += ", @pACT_TYPE = 'R' ";
                    else if (optCancel.Checked == true) strDelSql += ", @pACT_TYPE = 'C' ";
                    strDelSql += ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                    strDelSql += ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                }
                catch (Exception e)
                {
                    SystemBase.Loggers.Log(this.Name, e.ToString());
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    ERRCode = "ER";
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

            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 작업구분 변경시
        private void optRun_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
