using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace QD.QDE022
{
    public partial class QDE022P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strItemCd = "";
        int iRow = 0;
        FarPoint.Win.Spread.FpSpread spd;
        string QNC_NO = "";
        string END = "";
        #endregion

        #region 생성자
        public QDE022P1()
        {
            InitializeComponent();
        }

        public QDE022P1(FarPoint.Win.Spread.FpSpread spread,string _QNC_NO, string _end)
        {
            InitializeComponent();

            spd = spread;
            QNC_NO = _QNC_NO;
            END = _end;
        }
        #endregion

        #region Form Load 시
        private void QDE022P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "결함원인등록";
            //버튼 재정의
            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDEFECT_CD, "usp_B_COMMON @pType='DEFECT', @pCODE = 'R',@pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "'");//부적합코드
            
            Init();

            
            //필수체크
                        panel2.Enabled = false;

            GridCommGroupBox.Visible = false;

            if (END == "True")
            {
                txtPrevContent.Tag = ";2;;";
                txtDcauContent.Tag = ";2;;";
                txtManagerCd.Tag = ";2;;";
                txtWorkerCd.Tag = ";2;;";
            }


            SystemBase.Validation.GroupBox_Setting(groupBox1);
        }
        #endregion

        private void Init()
        {
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            string strSql = " usp_QDE022 'S2'";
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

            }

            dbConn.Close();
        }


        #region 팝업
        private void btnWorkerCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP1', @pSPEC1 = 'RES_CD', @pSPEC2 = 'RES_DIS', @pSPEC3 = 'P_RESO_MANAGE', @pSPEC4 = 'RES_KIND', @pSPEC5 = 'L'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtWorkerCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWorkerCd.Text = Msgs[0].ToString();
                    txtWorkerNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnManagerCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP1', @pSPEC1 = 'RES_CD', @pSPEC2 = 'RES_DIS', @pSPEC3 = 'P_RESO_MANAGE', @pSPEC4 = 'RES_KIND', @pSPEC5 = 'L'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtManagerCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P05006", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업관리자 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtManagerCd.Text = Msgs[0].ToString();
                    txtManagerNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "작업관리자 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

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

                    string strSql = " usp_QDE022 'U1'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pQNC_NO = '" + txtQNCNo.Text + "'"; //QNC_NO
                    strSql += ", @pWORKER_CD = '" + txtWorkerCd.Text + "'"; //작업자
                    strSql += ", @pMANAGER_CD = '" + txtManagerCd.Text + "'"; //작업관리자
                    strSql += ", @pPREV_CONTENT = '" + txtPrevContent.Text + "'"; //재발방지대책
                    strSql += ", @pDCAU_CONTENT = '" + txtDcauContent.Text + "'"; //결함원인
                    strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "'"; //결함원인
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
            if (ERRCode == "OK")
            {
                spd.Sheets[0].Cells[iRow, 13].Value = 1;

                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
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

        private void btnWcCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON @pType='P042', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "', @pETC = 'P002' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtWcCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00025", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작업장 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtWcCd.Text = Msgs[0].ToString();
                    txtWcNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInspectorCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q005', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtInspectorCd.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00067", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "작성자 팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {

                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtInspectorCd.Text = Msgs[0].ToString();
                    txtInspectorNm.Value = Msgs[1].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
            

    }
}
