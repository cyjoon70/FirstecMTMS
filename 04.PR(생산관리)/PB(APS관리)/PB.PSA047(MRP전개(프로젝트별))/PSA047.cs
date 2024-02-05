#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP전개(프로젝트별)
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-14
// 작성내용 : MRP전개(프로젝트별) 및 관리
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

namespace PB.PSA047
{
    public partial class PSA047 : UIForm.FPCOMM1
    {
        public PSA047()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void PSA047_Load(object sender, System.EventArgs e)
        {
            dtpMrpDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);		
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {            
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpMrpDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
        }
        #endregion

        #region 화면 활성화시 이벤트
        private void PSA047_Activated(object sender, System.EventArgs e)
        {
            SystemBase.Base.RodeFormName = this.Name;
        }
        #endregion

        #region MRP 팝업
        private void btnMrpNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P220' ,@pCOM_NM = 'S', @pETC = '" + txtSchNo.Text + "', @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD" };
                string[] strSearch = new string[] { txtMrpNo.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00101", strQuery, strWhere, strSearch, new int[] { 0 }, "MRP ID 조회");
                pu.Width = 750;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtMrpNo.Value = Msgs[0].ToString();
                    txtSchNo.Value = Msgs[3].ToString();
                    txtUserId.Text = Msgs[4].ToString();
                    Sch_Search();
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
                string strQuery = " usp_P_COMMON 'P210' ,@pCOM_NM = 'S', @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD" };
                string[] strSearch = new string[] { txtSchNo.Text };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00102", strQuery, strWhere, strSearch, new int[] { 0 }, "SCH ID 조회");
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtSchNo.Value = Msgs[0].ToString();
                    Sch_Search();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "SCH 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 생산담당자 팝업
        private void btnUser_Click(object sender, System.EventArgs e)
        {
            string strQuery = " usp_P_COMMON @pType='P180', @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
            string[] strWhere = new string[] { };
            string[] strSearch = new string[] { };
            UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00095", strQuery, strWhere, strSearch, new int[] { 2, 3 }, "");
            pu.ShowDialog();
            if (pu.DialogResult == DialogResult.OK)
            {
                Regex rx1 = new Regex("#");
                string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                txtUserId.Text = Msgs[0].ToString();
                txtUserNm.Value = Msgs[1].ToString();
                Sch_Search();
            }
        }
        #endregion

        #region 전개
        private void btnMrpProc_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ErrCode = "", MSGText = "";

            if (Save_Check() == "Y")
            {
                string stock = rdoStock1.Checked == true ? "Y" : "N";
                string useStock = rdoUseStock1.Checked == true ? "Y" : "N";
                string useDt = rdoUseDt1.Checked == true ? "Y" : "N";
                string safeStock = rdoSafeStock1.Checked == true ? "Y" : "N";

                string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                if (txtSchNo.Text.ToString() != "")
                {
                    try
                    {
                        //행수만큼 처리
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            string strMrpFlag = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP FLAG")].Text;

                            if (strMrpFlag != "Y")
                            {
                                string Query = " usp_PSA047 @pType = 'I2' ";
                                Query += ",@pSCH_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "SCH NO")].Text + "'";
                                Query += ",@pDETAIL_SEQ = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                                string strSelectFlag = "N"; if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True") strSelectFlag = "Y";
                                Query += ",@pSELECT_FLAG = '" + strSelectFlag + "'";
                                Query += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
                        Trans.Commit();

                        if (MessageBox.Show(SystemBase.Base.MessageRtn("P0016", "전개"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            PSA047P1 frm = new PSA047P1(
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
                            txtMrpNo.Text = MSGText;
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
                        ERRCode = "ER";
                    }

                Exit:
                    dbConn.Close();
                }
                else
                {
                    MessageBox.Show("SCH NO를 선택하셔야 합니다.", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("MRP를 전개할 대상이 없습니다.", SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

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

        #region Sch_Search() SCH LOG 조회 (함수)
        public void Sch_Search()
        {
            try
            {
                string strSchNo = txtSchNo.Text;
                string strUserId = txtUserId.Text;
                string strMrpId = txtMrpNo.Text;

                if (strSchNo != "" && strUserId != "")
                {

                    string strSql = "";
                    strSql = " usp_PSA047 @pTYPE = 'S1'";
                    strSql += ", @pSCH_NO = '" + txtSchNo.Text + "' ";
                    strSql += ", @pMF_PLAN_USER = '" + txtUserId.Text + "' ";
                    strSql += ", @pMRP_ID = '" + txtMrpNo.Text + "' ";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);
                    GridReMake();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region GridReMake() 그리드 재정의 (함수)
        public void GridReMake()
        {
            try
            {
                string strMprFlag = "", strStatus = "";
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        strMprFlag = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP FLAG")].Text;
                        strStatus = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "STATUS FLAG")].Text;

                        if (strMprFlag == "Y" || (strStatus != "F" && strStatus != "R"))
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, "1|3");
                        }
                        else
                        {
                            UIForm.FPMake.grdReMake(fpSpread1, i, "1|0");
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "그리드 재정의"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Save_Check() 전개대상 확인 (함수)
        public string Save_Check()
        {
            string strCheck = "N";

            try
            {
                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strMrpFlag = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "MRP FLAG")].Text;
                        string strSelectFlag = "N"; if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True") strSelectFlag = "Y";

                        if (strMrpFlag != "Y" && strSelectFlag == "Y")
                        {
                            strCheck = "Y";
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "대상체크"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return strCheck;
        }
        #endregion	

    }
}
