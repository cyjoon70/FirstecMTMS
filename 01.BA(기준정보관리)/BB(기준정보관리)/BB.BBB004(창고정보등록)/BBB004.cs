#region 작성정보
/*********************************************************************/
// 단위업무명 : 창고정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-25
// 작성내용 : 창고정보등록 및 관리
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

namespace BB.BBB004
{
    public partial class BBB004 : UIForm.FPCOMM1
    {
        #region 생성자
        public BBB004()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BBB004_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);
            SystemBase.Validation.GroupBox_Setting(groupBox3);
           
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboSlType, "usp_B_COMMON @pType='COMM', @pCODE = 'B008', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            SystemBase.ComboMake.C1Combo(cboSPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='PLANT', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");   
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            SystemBase.Validation.GroupBox_Reset(groupBox3);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string strQuery = " usp_BBB004  'S1'";
            strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
            strQuery = strQuery + ", @pPLANT_CD ='" + cboSPlantCd.SelectedValue.ToString().Trim() + "' ";
            strQuery = strQuery + ", @pSL_CD ='" + txtSSlCd.Text.Trim() + "' ";
            strQuery = strQuery + ", @pSL_NM ='" + txtSSlNm.Text + "' ";
            strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox2))
            {
                this.Cursor = Cursors.WaitCursor;

                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strMrpCheck = "N"; if (chkGbn.Checked == true) strMrpCheck = "Y";
                    string strSql = " usp_BBB004 'U1' ";
                    strSql = strSql + ", @pLANG_CD	  = '" + SystemBase.Base.gstrLangCd + "'";
                    strSql = strSql + ", @pSL_CD	  = '" + txtSlCd.Text.ToUpper().Trim() + "'";
                    strSql = strSql + ", @pSL_NM	  = '" + txtSlNm.Text + "'";
                    strSql = strSql + ", @pSL_TYPE = '" + cboSlType.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pPLANT_CD   = '" + cboPlantCd.SelectedValue.ToString() + "'";
                    strSql = strSql + ", @pMRP_USE_FLAG  = '" + strMrpCheck + "'";
                    strSql = strSql + ", @pUP_ID		 = '" + SystemBase.Base.gstrUserID + "'";
                    strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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
                    this.Cursor = Cursors.WaitCursor;

                    string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                    try
                    {
                        string strSql = " usp_BBB004  'D1'";
                        strSql = strSql + ",@pSL_CD = '" + txtSlCd.Text.ToUpper() + "'";
                        strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

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

                strCode = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text.ToString();
                if (strCode != "") Right_Search(strCode);
            }
        }

        private void fpSpread1_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {
                string strCode = "";
                int intRow = fpSpread1.ActiveSheet.GetSelection(0).Row;

                strCode = fpSpread1.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "창고코드")].Text.ToString();
                if (strCode != "") Right_Search(strCode);
            }
        }
        #endregion

        #region 우측 상세검색
        private void Right_Search(string strScode)
        {
            this.Cursor = Cursors.WaitCursor;

            SystemBase.Validation.GroupBox_Reset(groupBox3);

            try
            {
                string strSql = " usp_BBB004  'S2' ";
                strSql = strSql + ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strSql = strSql + ", @pSL_CD = '" + strScode + "'";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                txtSlCd.Text = ds.Tables[0].Rows[0]["SL_CD"].ToString();
                txtSlNm.Text = ds.Tables[0].Rows[0]["SL_NM"].ToString();
                if (ds.Tables[0].Rows[0]["SL_TYPE"].ToString() != "") cboSlType.SelectedValue = ds.Tables[0].Rows[0]["SL_TYPE"].ToString();
                if (ds.Tables[0].Rows[0]["PLANT_CD"].ToString() != "") cboPlantCd.SelectedValue = ds.Tables[0].Rows[0]["PLANT_CD"].ToString();
                if (ds.Tables[0].Rows[0]["MRP_USE_FLAG"].ToString() == "1") chkGbn.Checked = true;
                else chkGbn.Checked = false;
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSlCd.Focus();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
