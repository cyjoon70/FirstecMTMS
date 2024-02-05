#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-11
// 작성내용 : MRP 전개 및 관리
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
using WNDW;
using System.Data.SqlClient;

namespace PB.PSA022
{
    public partial class PSA022 : UIForm.Buttons
    {
        public PSA022()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PSA022_Load(object sender, System.EventArgs e)
        {            
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpMrpDt.Value = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 화면 활성화시 이벤트
        private void PSA022_Activated(object sender, System.EventArgs e)
        {
            SystemBase.Base.RodeFormName = this.Name;
        }
        #endregion

        #region MRP 팝업
        private void btnMrpNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P130' ,@pETC = '', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD" };
                string[] strSearch = new string[] { txtMrpNo.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00064", strQuery, strWhere, strSearch, new int[] { 0 }, "MRP ID 조회");
                pu.Width = 500;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtMrpNo.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SCH 팝업
        private void btnSchNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P250' ,@pCOM_NM = 'S', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCOM_CD", "@pETC" };
                string[] strSearch = new string[] { txtSchNo.Text, txtMrpNo.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00098", strQuery, strWhere, strSearch, new int[] { 0, 5 }, "SCH ID 조회");
                pu.Width = 600;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSchNo.Text = Msgs[0].ToString();
                    txtMrpNo.Value = Msgs[5].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "SCH 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 전개
        private void btnMrpProc_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ErrCode = "", MSGText = "";

            string stock = rdoStock1.Checked == true ? "Y" : "N";
            string useStock = rdoUseStock1.Checked == true ? "Y" : "N";
            string useDt = rdoUseDt1.Checked == true ? "Y" : "N";
            string safeStock = rdoSafeStock1.Checked == true ? "Y" : "N";

            if (txtSchNo.Text.ToString() != "")
            {
                try
                {
                    if (MessageBox.Show(SystemBase.Base.MessageRtn("P0016", "전개"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        PSA022P1 frm = new PSA022P1(
                            SystemBase.Base.gstrLangCd,
                            SystemBase.Base.gstrCOMCD,
                            SystemBase.Base.gstrBIZCD,
                            SystemBase.Base.gstrPLANT_CD,
                            SystemBase.Base.gstrREORG_ID,
                            SystemBase.Base.gstrDEPT,
                            stock,
                            useStock,
                            useDt,
                            safeStock,
                            SystemBase.Base.gstrUserID,
                            dtpMrpDt.Text,
                            txtRemark.Text,
                            txtSchNo.Text
                            );
                        frm.ShowDialog();

                        ErrCode = frm.errCode;
                        MSGText = frm.msgText;
                    }

                    if (ErrCode == "OK")
                    {
                        txtMrpNo.Value = MSGText;
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP 전개"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("SCH NO를 선택하셔야 합니다.");
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 전개취소
        private void btnMrpCancel_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (MessageBox.Show(SystemBase.Base.MessageRtn("P0016", "전개취소"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (txtMrpNo.Text != "")
                {
                    string ERRCode = "ER";
                    string MSGCode = "P0000";

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strQuery = "";
                        strQuery = " usp_PSA022 'D1' ";
                        strQuery += ", @pMRP_ID = '" + txtMrpNo.Text + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();

                    }
                    catch (Exception f)
                    {
                        Trans.Rollback();
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        ERRCode = "ER";
                        MSGCode = "P0001";
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

                    txtMrpNo.Value = "";

                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("P0020"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            this.Cursor = Cursors.Default;
        }
        #endregion
 }
}
