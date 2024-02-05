

#region 작성정보
/*********************************************************************/
// 단위업무명 : 고정자산변동내역출력
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-19
// 작성내용 : 고정자산변동내역출력
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

namespace AH.ACH008
{
    public partial class ACH008 : UIForm.Buttons
    {
        public ACH008()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACH008_Load(object sender, System.EventArgs e)
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

            dtpDeprDtFr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01-01";
            dtpDeprDtTo.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region TextChanged
        private void txtAcctCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAcctNm.Value = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", txtAcctCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' AND ENTRY_YN = 'Y' AND ACCT_TYPE = 'K0' ");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtAssetNo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtAssetNm.Value = SystemBase.Base.CodeName("ASSET_NO", "ASSET_NM", "A_ASSET_INFO", txtAssetNo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 버튼 클릭

        private void btnAcct_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y', @pSPEC2 = 'K0' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtAcctCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                pu.Width = 800;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                    txtAcctCd.Value = Msgs[0].ToString();
                    txtAcctNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "계정코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnAsset_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW027 pu = new WNDW.WNDW027();
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtAssetNo.Text = Msgs[1].ToString();
                    txtAssetNm.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "자산정보 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // 2018.10.17. hma 추가(Start): 상각계산처리조회 버튼 클릭 이벤트 처리
        private void btnDeprPopup_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW042 pu = new WNDW042(dtpDeprDtFr.Text, dtpDeprDtTo.Text);       // 감가상각계산처리조회 팝업
                pu.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 2018.10.17. hma 추가(End)
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    string strDelSql = " usp_ACH008  ";
                    strDelSql += " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strDelSql += ", @pDEPR_YYMM_FR = '" + dtpDeprDtFr.Text.Replace("-","") + "' ";
                    strDelSql += ", @pDEPR_YYMM_TO = '" + dtpDeprDtTo.Text.Replace("-", "") + "' ";
                    if (optRun.Checked == true) strDelSql += ", @pACT_TYPE = 'R' ";
                    else if (optCancel.Checked == true) strDelSql += ", @pACT_TYPE = 'C' ";
                    strDelSql += ", @pACCT_CD = '" + txtAcctCd.Text + "' ";
                    strDelSql += ", @pASSET_NO = '" + txtAssetNo.Text + "' ";
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

    }
}
