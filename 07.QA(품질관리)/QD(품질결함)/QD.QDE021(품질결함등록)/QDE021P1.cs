using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using WNDW;
namespace QD.QDE021
{
    public partial class QDE021P1 : UIForm.FPCOMM1
    {
        #region 변수선언
       
        string QNC_NO = "";
        string END = "";
        bool New_Change = false;
        #endregion

        #region 생성자
        public QDE021P1()
        {
            InitializeComponent();

            string Query = " exec usp_QDE021 'S3'";
            Query += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(Query);

            QNC_NO = ds.Tables[0].Rows[0]["QNC_NO"].ToString();
            New_Change = true;
        }

        public QDE021P1(FarPoint.Win.Spread.FpSpread spread,string _QNC_NO, string _end)
        {
            InitializeComponent();

            QNC_NO = _QNC_NO;
            END = _end;
            New_Change = false;
        }
        #endregion

        #region Form Load 시
        private void QDE021P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "품질결함등록";
            GridCommGroupBox.Visible = false;
            //버튼 재정의
            UIForm.Buttons.ReButton("000000110001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
           
            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboDEFECT_CD, "usp_B_COMMON @pType='DEFECT', @pCODE = 'R',@pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'");//부적합코드

            SystemBase.ComboMake.C1Combo(cboDIVISION_CD, "usp_B_COMMON @pType='COMM2', @pCODE = 'Q029', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'", 0);//투입후구분
            
            if (New_Change)
            {
                txtDEFECT_QTY.Value = 0;
                txtQNCNo.Value = QNC_NO;
            
            }
            else
            {
                Init();
                txtQNCNo.Tag = ";2;;";
            }

            if (END == "True")
            {
                txtQNCNo.Tag = ";2;;";
                txtORDER_NO.Tag = ";2;;";
                cboDIVISION_CD.Tag = ";2;;";
                txtDeptCd.Tag = ";2;;";
                txtInspectorCd.Tag = ";2;;";
                txtSProjectNo.Tag = ";2;;";
                txtSProjectSeq.Tag = ";2;;";
                txtItemCd.Tag = ";2;;";
                txtItemSpec.Tag = ";2;;";
                txtWcCd.Tag = ";2;;";
                txtINSP_CLASS_CD.Tag = ";2;;";
                cboDEFECT_CD.Tag = ";2;;";
                txtDEFECT_QTY.Tag = ";2;;";
                txtQdefContent.Tag = ";2;;";
                txtQProcContent.Tag = ";2;;";
                rdoTdecInspYnYes.Tag = ";2;;";
                rdoTdecInspYnNo.Tag = ";2;;";
            }
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);


        }
        #endregion

        private void Init()
        {
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            string strSql = " usp_QDE021 'S2'";
            strSql += ", @pQNC_NO = '" + QNC_NO + "' ";
            strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";
            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strSql);

            if (dt.Rows.Count > 0)
            {
                txtQNCNo.Value = QNC_NO;
                txtORDER_NO.Text = dt.Rows[0]["ORDER_NO"].ToString();
                cboDIVISION_CD.SelectedValue = dt.Rows[0]["DIVITION_CD"].ToString();
                txtDeptCd.Text = dt.Rows[0]["DEPT_CD"].ToString();
                txtInspectorCd.Text = dt.Rows[0]["INSPECTOR_CD"].ToString();
                txtSProjectNo.Text = dt.Rows[0]["PROJECT_NO"].ToString();
                txtSProjectSeq.Text = dt.Rows[0]["PROJECT_SEQ"].ToString();
                txtWcCd.Text = dt.Rows[0]["WC_CD"].ToString();
                txtINSP_CLASS_CD.Text = dt.Rows[0]["INSP_CLASS_CD"].ToString();
                txtItemCd.Value = dt.Rows[0]["ITEM_CD"].ToString();

                if (dt.Rows[0]["TDEC_INSP_YN"].ToString() == "Y")
                    rdoTdecInspYnYes.Checked = true;
                else
                    rdoTdecInspYnNo.Checked = true;
                cboDEFECT_CD.SelectedValue = dt.Rows[0]["DEFECT_CD"].ToString();
                txtDEFECT_QTY.Text = dt.Rows[0]["DEFECT_QTY"].ToString();
                
                txtQdefContent.Text = dt.Rows[0]["QDEF_CONTENT"].ToString();
                txtQProcContent.Text = dt.Rows[0]["QPROC_CONTENT"].ToString();
            }

            dbConn.Close();
        }

        #region 팝업
        //귀책부서
        private void btnDeptCd_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'COMM_POP' ,@pSPEC1='Q026', @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00093", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "귀책부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Text = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //작성자
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

        private void btnWcCd_Click_1(object sender, EventArgs e)
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



        private void btnItemCd_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005("10");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "품목코드 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 프로젝트번호
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSProj_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW007 pu = new WNDW.WNDW007(txtSProjectNo.Text);
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtSProjectNo.Text = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "프로젝트번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 프로젝트 차수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSProjSeq_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtSProjectNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                //UIForm.PopUpSP pu = new UIForm.PopUpSP(strQuery, strWhere, strSearch, PHeadText7, PTxtAlign7, PCellType7, PHeadWidth7, PSearchLabel7);
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtSProjectSeq.Text = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
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
                //입력폼 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string str = "";
                    if (New_Change)
                        str = "I1";
                    else
                        str = "U1";

                    string strFlg = "";
                    if (rdoTdecInspYnYes.Checked == true)
                    { strFlg = "Y"; }
                    else
                    { strFlg = "N"; }

                    string strSql = " usp_QDE021 '" + str + "'";
                    strSql += ", @pQNC_NO = '" + txtQNCNo.Text + "'"; //QNC_NO
                    strSql += ", @pORDER_NO = '" + txtORDER_NO.Text + "' "; //ORDER_NO
                    strSql += ", @pITEM_CD = '" + txtItemCd.Text + "'";//품목코드
                    strSql += ", @pDEPT_CD = '" + txtDeptCd.Text + "'";//귀책부서
                    strSql += ", @pINSPECTOR_CD = '" + txtInspectorCd.Text + "'";// 작성자
                    strSql += ", @pQDEF_CONTENT = '" + txtQdefContent.Text + "'";//결함내용
                    strSql += ", @pQPROC_CONTENT = '" + txtQProcContent.Text + "'";//타프로세스및제품에대한영향성평가
                    strSql += ", @pPROJECT_NO = '" + txtSProjectNo.Text + "'";//프로젝트번호
                    strSql += ", @pPROJECT_SEQ = '" + txtSProjectSeq.Text + "'";//프로젝트차수
                    strSql += ", @pWC_CD = '" + txtWcCd.Text + "'";//작업장
                    strSql += ", @pINSP_CLASS_CD = '" + txtINSP_CLASS_CD.Text + "'";//발생장소
                    strSql += ", @pTDEC_INSP_YN = '" + strFlg + "'"; //기술판정검사여부
                    strSql += ", @pDIVITION_CD = '" + cboDIVISION_CD.SelectedValue.ToString() + "'"; //기술판정검사여부
                    if (cboDEFECT_CD.Text != "")
                        strSql += ", @pDEFECT_CD = '" + cboDEFECT_CD.SelectedValue.ToString() + "'";//부적합코드

                    strSql += ", @pDEFECT_QTY = '" + txtDEFECT_QTY.Text + "'";//부적합수량
                    //2020.11.03. ksh 수정 : 부적합코드가 반납(RTV)일때 재입고 여부가 Y가 들어가야됨(강인훈 부장 요청)
                    if (cboDEFECT_CD.SelectedValue.ToString() == "19")
                    {
                        strSql += ", @pRESTOCK_YN = 'Y'";   //재입고여부
                    }

                        strSql += ", @pIN_ID = '" + SystemBase.Base.gstrUserID + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pPLANT_CD = '" + SystemBase.Base.gstrPLANT_CD + "' ";

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
        #endregion


     }
}
