using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace QD.QDE023
{
    public partial class QDE023P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strInspReqNo = "";
        string strItemCd = "";
        FarPoint.Win.Spread.FpSpread spd;
        string QNC_NO = "";
        string END = "";
        #endregion

        #region 생성자
        public QDE023P1()
        {
            InitializeComponent();
        }

        public QDE023P1(FarPoint.Win.Spread.FpSpread spread, string _QNC_NO, string _end)
        {
            InitializeComponent();

            spd = spread;
            QNC_NO = _QNC_NO;
            END = _end;
        }
        #endregion

        #region Form Load 시
        private void QDE023P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "기술판정등록";

            //버튼 재정의
            UIForm.Buttons.ReButton("000000010001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDEFECT_CD, "usp_B_COMMON @pType='DEFECT', @pCODE = 'R',@pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//부적합코드

            Init();
            if (END == "True")
            {
                txtTdecContent.Tag = ";2;;";
            }
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);


            panel2.Enabled = false;
            GridCommGroupBox.Visible = false;
        }
        #endregion
        private void Init()
        {
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            string strSql = " usp_QDE023 'S2'";
            strSql += ", @pQNC_NO = '" + QNC_NO + "' ";
            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                txtQNCNo.Value = QNC_NO;
                txtItemCd.Value = dt.Rows[0]["ITEM_CD"].ToString();
                txtDeptCd.Text = dt.Rows[0]["DEPT_CD"].ToString();
                txtInspectorCd.Text = dt.Rows[0]["INSPECTOR_CD"].ToString();
                txtWcCd.Text = dt.Rows[0]["WC_CD"].ToString();
                if (dt.Rows[0]["TDEC_INSP_YN"].ToString() == "Y")
                    rdoTdecInspYnYes.Checked = true;
                else
                    rdoTdecInspYnNo.Checked = true;
                txtWorkerCd.Text = dt.Rows[0]["WORKER_CD"].ToString();
                txtManagerCd.Text = dt.Rows[0]["MANAGER_CD"].ToString();
                txtQdefContent.Text = dt.Rows[0]["QDEF_CONTENT"].ToString();
                txtPrevContent.Text = dt.Rows[0]["PREV_CONTENT"].ToString();
                txtDcauContent.Text = dt.Rows[0]["DCAU_CONTENT"].ToString();
                cboDEFECT_CD.SelectedValue = dt.Rows[0]["DEFECT_CD"].ToString();
                txtDEFECT_QTY.Text = dt.Rows[0]["DEFECT_QTY"].ToString();
                txtPrevContent.Text = dt.Rows[0]["PREV_CONTENT"].ToString();
                txtDcauContent.Text = dt.Rows[0]["DCAU_CONTENT"].ToString();
                txtTdecContent.Text = dt.Rows[0]["TDEC_CONTENT"].ToString();
                if (dt.Rows[0]["RESTOCK_YN"].ToString() == "Y")
                    rdoRESTOCK_YES.Checked = true;
                else
                    rdoRESTOCK_NO.Checked = true;
            }

            dbConn.Close();
        }

        #region TextChanged
        //품목코드
        private void txtItemCd_TextChanged(object sender, EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
            txtItemSpec.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_SPEC", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //귀책부서
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            txtDeptNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtDeptCd.Text, " AND MAJOR_CD = 'Q026'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작성자
        private void txtInspectorCd_TextChanged(object sender, EventArgs e)
        {
            txtInspectorNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtInspectorCd.Text, " AND MAJOR_CD = 'Q005' AND LANG_CD = '" + SystemBase.Base.gstrLangCd + "' AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업자
        private void txtWorkerCd_TextChanged(object sender, EventArgs e)
        {
            txtWorkerNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtWorkerCd.Text, " AND RES_KIND = 'L' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        //작업관리자
        private void txtManagerCd_TextChanged(object sender, EventArgs e)
        {
            txtManagerNm.Value = SystemBase.Base.CodeName("RES_CD", "RES_DIS", "P_RESO_MANAGE", txtManagerCd.Text, " AND RES_KIND = 'L' AND CO_CD ='" + SystemBase.Base.gstrCOMCD + "'");
        }

        //작업장
        private void txtWcCd_TextChanged(object sender, EventArgs e)
        {
            txtWcNm.Value = SystemBase.Base.CodeName("MINOR_CD", "CD_NM", "B_COMM_CODE", txtWcCd.Text, " AND MAJOR_CD = 'P002'  AND COMP_CODE ='" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region SaveExec() 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //입력폼 필수 체트
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strSql = " usp_QDE023 'U1'";
                    strSql += ", @pQNC_NO = '" + txtQNCNo.Text + "'"; //QNC_NO
                    strSql += ", @pTDEC_CONTENT = '" + txtTdecContent.Text + "'";
                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프							
                }
                else
                {
                    Trans.Rollback();
                    this.Cursor = Cursors.Default;
                    return;
                }
                Trans.Commit();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception e)
            {
                SystemBase.Loggers.Log(this.Name, e.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = e.Message;
                //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode == "ER")
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

    }
}
