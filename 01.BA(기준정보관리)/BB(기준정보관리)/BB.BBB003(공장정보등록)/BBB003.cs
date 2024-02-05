#region 작성정보
/*********************************************************************/
// 단위업무명 : 공장정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-25
// 작성내용 : 공장정보등록 및 관리
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

namespace BB.BBB003
{
    public partial class BBB003 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBB003()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
            SystemBase.Validation.GroupBox_Setting(groupBox4);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboBizCd, "usp_B_COMMON @pTYPE ='BIZ', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboCalType, "usp_B_COMMON @pType='COMM', @pCODE = 'Z006', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");//카렌다구분

            dtpUseDt.Text = "2999-12-31";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
            SystemBase.Validation.GroupBox_Reset(groupBox4);
            dtp_ReType("l2");
            dtpUseDt.Text = "2999-12-31";
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string strQuery = " usp_BBB003  'S1'";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pPLANT_CD ='" + txtSPlantCd.Text.Trim() + "' ";
            strQuery = strQuery + ", @pPLANT_NM ='" + txtSPlantNm.Text + "' ";
            strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox3))
                {
                    if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox4))
                    {
                        this.Cursor = Cursors.WaitCursor;

                        string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                        SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                        SqlCommand cmd = dbConn.CreateCommand();
                        SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            string strSql = " usp_BBB003 'U1' ";
                            strSql = strSql + ", @pLANG_CD	  = '" + SystemBase.Base.gstrLangCd + "'";
                            strSql = strSql + ", @pCO_CD	  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strSql = strSql + ", @pPLANT_CD	  = '" + txtPlantCd.Text.ToUpper().Trim() + "'";
                            strSql = strSql + ", @pPLANT_NM	  = '" + txtPlantNm.Text + "'";
                            strSql = strSql + ", @pBIZ_CD	  = '" + cboBizCd.SelectedValue.ToString() + "'";
                            strSql = strSql + ", @pCAL_TYPE   = '" + cboCalType.SelectedValue.ToString() + "'";
                            strSql = strSql + ", @pSTK_STR_YM = '" + dtpStrYM.Text.Replace("-", "") + "'";
                            strSql = strSql + ", @pUSE_DT     = '" + dtpUseDt.Text.Replace("-", "") + "'";
                            int intPlanScope = 0;
                            if (txtPlanScope.Text != "Null" && txtPlanScope.Text.Trim() != "") intPlanScope = Convert.ToInt32(txtPlanScope.Text);
                            strSql = strSql + ", @pPLAN_SCOPE = '" + intPlanScope + "'";
                            int intMpsDtf = 0;
                            if (txtMpsDtf.Text != "Null" && txtMpsDtf.Text.Trim() != "") intMpsDtf = Convert.ToInt32(txtMpsDtf.Text);
                            strSql = strSql + ", @pMPS_DTF    = '" + txtMpsDtf.Text + "'";
                            int intMpsPtf = 0;
                            if (txtMpsPtf.Text != "Null" && txtMpsPtf.Text.Trim() != "") intMpsPtf = Convert.ToInt32(txtMpsPtf.Text);
                            strSql = strSql + ", @pMPS_PTF    = '" + txtMpsPtf.Text + "'";
                            int intMrpDtf = 0;
                            if (txtMrpDtf.Text != "Null" && txtMrpDtf.Text.Trim() != "") intMrpDtf = Convert.ToInt32(txtPlanScope.Text);
                            strSql = strSql + ", @pMRP_DTF    = '" + txtMrpDtf.Text + "'";
                            strSql = strSql + ", @pUP_ID		 = '" + SystemBase.Base.gstrUserID + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                            Trans.Commit();
                        }
                        catch (Exception e)
                        {
                            SystemBase.Loggers.Log(this.Name, e.ToString());
                            Trans.Rollback();
                            MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.

                            this.Cursor = Cursors.Default;
                        }
                    Exit:
                        dbConn.Close();

                        if (ERRCode == "OK")
                        {
                            SearchExec();
                            dtp_ReType("E");
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
                }
            }
        }
        #endregion

        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                string msg = SystemBase.Base.MessageRtn("B0027");
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn(msg), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {

                        this.Cursor = Cursors.WaitCursor;

                        string strSql = " usp_BBB003  'D1'";
                        strSql = strSql + ", @pLANG_CD  = '" + SystemBase.Base.gstrLangCd + "'";
                        strSql = strSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                        strSql = strSql + ", @pPLANT_CD = '" + txtPlantCd.Text.ToUpper() + "'";
                        strSql = strSql + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                        Trans.Commit();
                    }
                    catch (Exception e)
                    {
                        SystemBase.Loggers.Log(this.Name, e.ToString());
                        Trans.Rollback();
                        MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                        this.Cursor = Cursors.Default;
                    }
                Exit:
                    dbConn.Close();

                    if (ERRCode == "OK")
                    {
                        SearchExec();
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

            }
        }
        #endregion

        #region 좌측 fpSpread 클릭시 우측상세조회
        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {           
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string strCode = "";
                int intRow = e.Row;

                strCode = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString();
                if (strCode != "") Right_Search(strCode);
            }
        }

        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string strCode = "";
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                strCode = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "공장코드")].Text.ToString();
                if (strCode != "") Right_Search(strCode);
            }
        }
        #endregion

        #region 우측 상세검색
        private void Right_Search(string strScode)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                dtp_ReType("I1");
                string strSql = " usp_BBB003  'S2' ";
                strSql = strSql + ", @pPLANT_CD = '" + strScode + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                txtPlantCd.Text = ds.Tables[0].Rows[0]["PLANT_CD"].ToString();
                txtPlantNm.Text = ds.Tables[0].Rows[0]["PLANT_NM"].ToString();
                if (ds.Tables[0].Rows[0]["BIZ_CD"].ToString() != "") cboBizCd.SelectedValue = ds.Tables[0].Rows[0]["BIZ_CD"].ToString();
                if (ds.Tables[0].Rows[0]["CAL_TYPE"].ToString() != "") cboCalType.SelectedValue = ds.Tables[0].Rows[0]["CAL_TYPE"].ToString();
                dtpStrYM.Text = ds.Tables[0].Rows[0]["STK_STR_YM"].ToString();
                dtpEndYM.Text = ds.Tables[0].Rows[0]["STK_CLS_YM"].ToString();
                if (ds.Tables[0].Rows[0]["USE_DT"].ToString().Length >9) dtpUseDt.Text = ds.Tables[0].Rows[0]["USE_DT"].ToString().Substring(0,10);
                txtPlanScope.Text = ds.Tables[0].Rows[0]["PLAN_SCOPE"].ToString();
                txtMpsDtf.Text = ds.Tables[0].Rows[0]["MPS_DTF"].ToString();
                txtMpsPtf.Text = ds.Tables[0].Rows[0]["MPS_PTF"].ToString();
                txtMrpDtf.Text = ds.Tables[0].Rows[0]["MRP_DTF"].ToString();

                dtp_ReType("E");

            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtp_ReType("I2");

                this.Cursor = Cursors.Default;
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region dtp일정 재정의
        private void dtp_ReType(string strTpe)
        {
            if (strTpe == "E")
            {
                dtpStrYM.ReadOnly = true;
                dtpEndYM.ReadOnly = true;
                dtpStrYM.BackColor = Color.Gainsboro;
                dtpEndYM.BackColor = Color.Gainsboro;
            }
            else
            {
                if (strTpe == "I1")
                {
                    dtpStrYM.BackColor = Color.Gainsboro;
                    dtpEndYM.BackColor = Color.Gainsboro;
                    dtpStrYM.ReadOnly = false;
                    dtpEndYM.ReadOnly = false;
                }
                else
                {
                    dtpStrYM.BackColor = Color.White;
                    dtpStrYM.ReadOnly = false; dtpStrYM.Text = "";
                    dtpEndYM.BackColor = Color.White;
                    dtpEndYM.ReadOnly = false; dtpEndYM.Text = ""; 
                }
            }
        }
        #endregion

    }
}
