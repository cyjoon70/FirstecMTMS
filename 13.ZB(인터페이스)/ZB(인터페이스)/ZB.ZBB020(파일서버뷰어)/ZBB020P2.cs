

#region 작성정보
/*********************************************************************/
// 단위업무명 : 파일서버정보등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-05-23
// 작성내용 : 파일서버정보등록
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

namespace ZB.ZBB020
{
    public partial class ZBB020P2 : UIForm.Buttons
    {
        #region 변수선언
        string strServer_Change = "";
        #endregion

        public ZBB020P2()
        {
            InitializeComponent();
        }
        #region Form Load 시
        private void ZBB020P2_Load(object sender, System.EventArgs e)
        {
            try
            {
                UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
                this.Text = "파일서버정보 등록";
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Reset(groupBox1);
                SERVER_INFO_GET();
                txtServerDomain.Focus();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        public void SERVER_INFO_GET()
        {
            try
            {
                string strQuery = " usp_ZBB020  'S2'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtServerDomain.Value = dt.Rows[0]["SERVER_DOMAIN"].ToString();
                    txtUserId.Value = dt.Rows[0]["USER_ID"].ToString();
                    txtPassWord.Value = SystemBase.Base.Decode(dt.Rows[0]["PASSWORD"].ToString());
                    txtRootDrive.Value = dt.Rows[0]["ROOT_DRIVE"].ToString();
                }
            }
            catch(Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region SaveExec() 폼에 입력된 데이타 메인 화면으로 리턴
        protected override void SaveExec()
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
                    string strSql = " usp_ZBB020 'I2'";

                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    strSql = strSql + ", @pSERVER_DOMAIN = '" + @txtServerDomain.Text + "'";
                    strSql = strSql + ", @pUSER_ID = '" + txtUserId.Text + "'";
                    strSql = strSql + ", @pPASSWORD = '" + SystemBase.Base.Encode(txtPassWord.Text) + "'";
                    strSql = strSql + ", @pROOT_DRIVE = '" + txtRootDrive.Text + "'";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
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
                    strServer_Change = "Y";
                    this.Cursor = Cursors.Default;
                    //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Cursor = Cursors.Default;
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        public string SERVER_CHANGE { get { return strServer_Change; } set { strServer_Change = value; } }
        
    }
}
