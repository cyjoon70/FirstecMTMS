#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불현황조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-19
// 작성내용 : 수불현황조회 관리
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
using FarPoint.Win.Spread.CellType;
using System.Data.OleDb;                //엑셀 업로드 연결

namespace IT.ITR115
{
    public partial class ITR115 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        int SDown = 1;		// 조회 횟수
        int AddRow = 100;
        #endregion

        #region 생성자
        public ITR115()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ITR115_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboTranType, "usp_B_COMMON @pType='COMM', @pCODE = 'I001', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);//수불구분
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3); //품목계정

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);
            txtSlFr.Value = "W03";
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ITR115 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pTRAN_DT_FR ='" + dtpTranDtFr.Text + "'";
                    strQuery += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pSL_CD_FR ='" + txtSlFr.Text.Trim() + "'";
                    strQuery += ", @pMOVE_TYPE ='" + txtMoveType.Text.Trim() + "'";
                    strQuery += ", @pTRAN_TYPE ='" + cboTranType.SelectedValue.ToString() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                    strQuery += ", @pWORKORDER_NO_FR ='" + txtWorkOrderNo_FR.Text.Trim() + "'";
                    strQuery += ", @pWORKORDER_NO_TO ='" + txtWorkOrderNo_TO.Text.Trim() + "'";
                    strQuery += ", @pTOPCOUNT ='" + AddRow + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
 
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 100건씩 조회
        private void fpSpread1_TopChange(object sender, FarPoint.Win.Spread.TopChangeEventArgs e)
        {
            int FPHeight = (fpSpread1.Size.Height - 28) / 20;
            if (e.NewTop >= ((AddRow * SDown) - FPHeight))
            {
                SDown++;

                this.Cursor = Cursors.WaitCursor;

                string strQuery = " usp_ITR115 'S1'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                strQuery += ", @pTRAN_DT_FR ='" + dtpTranDtFr.Text + "'";
                strQuery += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text + "'";
                strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                strQuery += ", @pSL_CD_FR ='" + txtSlFr.Text.Trim() + "'";
                strQuery += ", @pMOVE_TYPE ='" + txtMoveType.Text.Trim() + "'";
                strQuery += ", @pTRAN_TYPE ='" + cboTranType.SelectedValue.ToString() + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_FR ='" + txtProject_Seq.Text.Trim() + "'";
                strQuery += ", @pPROJECT_SEQ_TO ='" + txtProject_Seq1.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_FR ='" + txtWorkOrderNo_FR.Text.Trim() + "'";
                strQuery += ", @pWORKORDER_NO_TO ='" + txtWorkOrderNo_TO.Text.Trim() + "'";
                strQuery += ", @pTOPCOUNT ='" + AddRow * SDown + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery);

                this.Cursor = Cursors.Default;
            }
        }
        #endregion


        #region DelExec() 삭제 로직
        protected override void DeleteExec()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("SY010"), SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.Yes)
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_ITR115 'D1'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strSql += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strSql += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    //strSql += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strSql += ", @pTRAN_DT_FR ='" + dtpTranDtFr.Text + "'";
                    strSql += ", @pTRAN_DT_TO ='" + dtpTranDtTo.Text + "'";


                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();
                    //NewExec();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {
                    //txtSSlipNo.Value = "";
                    //NewExec();
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fpSpread1.Sheets[0].Rows.Count = 0;

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
        }
        #endregion




        #region 버튼 Click
        private void btnSlFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlFr.Value = Msgs[0].ToString();
                    txtSlNmFr.Value = Msgs[1].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnMoveType_Click(object sender, System.EventArgs e)
        {
            DialogResult dsMsg;
            try
            {
                if (cboTranType.SelectedValue.ToString() == "")
                {
                    dsMsg = MessageBox.Show("수불구분을 먼저 선택하세요!", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboTranType.Focus();
                    return;
                }
                string strQuery
                    = "usp_B_COMMON @pTYPE = 'TABLE_POP1', @pSPEC1 = 'MOVE_TYPE', @pSPEC2 = 'MOVE_TYPE_NM', @pSPEC3 = 'I_MOVE_TYPE', @pSPEC4 = 'TRAN_TYPE' , @pSPEC5 = '" + cboTranType.SelectedValue.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtMoveType.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00054", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "수불유형 조회");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtMoveType.Value = Msgs[0].ToString();
                    txtMoveTypeNm.Value = Msgs[1].ToString();

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }


        // 프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                    if (txtProject_Seq.Text != "*") txtProject_Seq.Value = "";
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectSeq_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProject_Seq.Value = Msgs[0].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void btnProjectSeq1_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'PROJ_SEQ', @pSPEC1 = '" + txtProject_No.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";										// 쿼리
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };			// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { "", "" };		// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P09001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "프로젝트차수 조회", false);
                pu.Width = 400;
                pu.ShowDialog();	//공통 팝업 호출

                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);
                    txtProject_Seq1.Value = Msgs[0].ToString();
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
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtSlFr_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtSlFr.Text != "")
                {
                    txtSlNmFr.Value = SystemBase.Base.CodeName("SL_CD", "SL_NM", "B_STORAGE_INFO", txtSlFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtSlNmFr.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtMoveType_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtMoveType.Text != "")
                {
                    txtMoveTypeNm.Value = SystemBase.Base.CodeName("MOVE_TYPE", "MOVE_TYPE_NM", "I_MOVE_TYPE", txtMoveType.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtMoveTypeNm.Value = "";
                }
            }
            catch
            {

            }
        }



        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
                if (txtProject_Seq.Text != "*")
                { txtProject_Seq.Value = ""; txtProject_Seq1.Value = ""; }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR115_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR115_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        private void btnUpLoad_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    conn.Open();

                    DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    string commandString = String.Format("SELECT * FROM [구매입고$]", worksheets.Rows[0]["TABLE_NAME"]);
                    OleDbCommand cmd = new OleDbCommand(commandString, conn);

                    OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    dapt.Fill(ds);
                    conn.Close();

                    fpSpread1.Sheets[0].RowCount = 0;
                    fpSpread1.Sheets[0].DataSource = ds;

                    string ERRCode = "", MSGCode = "";
                    SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                    SqlCommand cmd1 = dbConn.CreateCommand();
                    SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    try
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                if (ds.Tables[0].Rows[i][0].ToString().Trim() != "")  //값이 있는것만 처리할수 있도록 함 
                                {
                                    string strSql = " usp_ITR115 'I1 '";
                                    strSql += " , @pCO_CD  ='" + SystemBase.Base.gstrCOMCD + "' ";
                                    strSql += " , @ITEM_CD ='" + ds.Tables[0].Rows[i][0].ToString().Trim() + "' ";                  // 품목
                                    //strSql += " , @p  ='" + ds.Tables[0].Rows[i][1].ToString().Trim() + "' ";                     // 품목명
                                    strSql += " , @ITEM_SPEC ='" + ds.Tables[0].Rows[i][2].ToString().Trim() + "' ";                // 규격
                                    strSql += " , @INV_UNIT ='" + ds.Tables[0].Rows[i][3].ToString().Trim() + "' ";                 // 단위
                                    strSql += " , @TRAN_DT ='" + ds.Tables[0].Rows[i][4].ToString().Trim() + "' ";                  // 수불일자
                                    strSql += " , @ITEM_STATUS ='" + ds.Tables[0].Rows[i][5].ToString().Trim() + "' ";              // 재고상태
                                    strSql += " , @INV_DCR_FLAG ='" + ds.Tables[0].Rows[i][6].ToString().Trim() + "' ";             // 증감
                                    strSql += " , @TRAN_QTY = " + Convert.ToDouble(ds.Tables[0].Rows[i][7].ToString().Trim());      // 수량
                                    strSql += " , @TRAN_PRICE = " + Convert.ToDouble(ds.Tables[0].Rows[i][8].ToString().Trim());    // 단가
                                    strSql += " , @TRAN_AMT = " + Convert.ToDouble(ds.Tables[0].Rows[i][9].ToString().Trim());      // 금액
                                    strSql += " , @INCIDENTAL_EXP =" + Convert.ToDouble(ds.Tables[0].Rows[i][10].ToString().Trim());    // 부대비

                                    strSql += " , @PROJECT_NO ='" + ds.Tables[0].Rows[i][11].ToString().Trim() + "' ";              // 프로젝트번호
                                    strSql += " , @PROJECT_SEQ ='" + ds.Tables[0].Rows[i][12].ToString().Trim() + "' ";             // 프로젝트차수
                                    strSql += " , @GROUP_CD ='" + ds.Tables[0].Rows[i][13].ToString().Trim() + "' ";                // 모품목
                                    //strSql += " ,@p  ='" + ds.Tables[0].Rows[i][14].ToString().Trim() + "' ";                     // 모품목명
                                    strSql += " , @GROUP_SPEC ='" + ds.Tables[0].Rows[i][15].ToString().Trim() + "' ";              // 모품목규격
                                    strSql += " , @LOT_NO ='" + ds.Tables[0].Rows[i][16].ToString().Trim() + "' ";                  // Lot
                                    strSql += " , @LOT_SEQ = " + Convert.ToDouble(ds.Tables[0].Rows[i][17].ToString().Trim());      // Lot순번
                                    strSql += " , @SL_CD ='" + ds.Tables[0].Rows[i][18].ToString().Trim() + "' ";                   // 창고
                                    //strSql += " ,@p  ='" + ds.Tables[0].Rows[i][19].ToString().Trim() + "' ";                     // 창고명
                                    strSql += " , @TRAN_TYPE ='" + ds.Tables[0].Rows[i][20].ToString().Trim() + "' ";               // 수불구분        코드화?
                                    strSql += " , @MOVE_TYPE ='" + ds.Tables[0].Rows[i][21].ToString().Trim() + "' ";               // 수불유형        코드화?
                                    strSql += " , @TRAN_NO ='" + ds.Tables[0].Rows[i][22].ToString().Trim() + "' ";                 // 수불번호
                                    strSql += " , @TRAN_SEQ = " + Convert.ToDouble(ds.Tables[0].Rows[i][23].ToString().Trim());     // 수불상세
                                    strSql += " , @SO_NO = '" + ds.Tables[0].Rows[i][24].ToString().Trim() + "' ";                  // 수주번호
                                    strSql += " , @SO_SEQ ='" + ds.Tables[0].Rows[i][25].ToString().Trim() + "' ";                  // 수주상세
                                    strSql += " , @PO_NO ='" + ds.Tables[0].Rows[i][26].ToString().Trim() + "' ";                   // 발주번호
                                    strSql += " , @PO_SEQ ='" + ds.Tables[0].Rows[i][27].ToString().Trim() + "' ";                  // 발주순번
                                    strSql += " , @TRAN_REMARK ='" + ds.Tables[0].Rows[i][28].ToString().Trim() + "' ";             // 수불 비고
                                    strSql += " , @CURRENCY ='" + ds.Tables[0].Rows[i][29].ToString().Trim() + "' ";                // 화폐단위
                                    if (ds.Tables[0].Rows[i][30].ToString().Trim() == "")
                                        strSql += " , @TRAN_PRICE_FOR = 0 ";
                                    else
                                        strSql += " , @TRAN_PRICE_FOR =" + ds.Tables[0].Rows[i][30].ToString().Trim();    // 외화단가
                                    if (ds.Tables[0].Rows[i][31].ToString().Trim() == "")
                                        strSql += " , @TRAN_AMT_FOR = 0 ";
                                    else
                                        strSql += " , @TRAN_AMT_FOR =" + ds.Tables[0].Rows[i][31].ToString().Trim();    // 외화단가

                                    strSql += " , @WORKORDER_NO ='" + ds.Tables[0].Rows[i][32].ToString().Trim() + "' ";            // 제조오더번호
                                    strSql += " , @REMARK ='" + ds.Tables[0].Rows[i][33].ToString().Trim() + "' ";                  // 제조오더 비고
                                    //strSql += " , @ ='" + ds.Tables[0].Rows[i][34].ToString().Trim() + "' ";                      // 품목구분
                                    strSql += " , @UP_USR ='" + ds.Tables[0].Rows[i][35].ToString().Trim() + "' ";                  // 작업자
                                    strSql += " , @IN_ID    ='" + SystemBase.Base.gstrUserID + "'";

                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프 
                                }
                            }
                            Trans.Commit();
                        }
                    }
                    catch (Exception f)
                    {
                        SystemBase.Loggers.Log(this.Name, f.ToString());
                        Trans.Rollback();
                        ERRCode = "ER";
                        MSGCode = f.Message;
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
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SearchExec();
        }


        private void btnDnLoad_Click(object sender, EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();
        }


    }
}
