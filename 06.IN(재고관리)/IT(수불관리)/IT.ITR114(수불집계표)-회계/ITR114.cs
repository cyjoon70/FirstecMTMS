#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불집계표
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-18
// 작성내용 : 수불집계표 출력 및 관리
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


namespace IT.ITR114
{
    public partial class ITR114 : UIForm.FPCOMM1
    {
        bool form_act_chk = false;

        public ITR114()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ITR114_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();

            txtSlFr.Text = "W03";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);
            fpSpread1.Sheets[0].Rows.Count = 0;
            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    string strQuery = " usp_ITR114 'S1'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pINV_YY  ='" + dtpYear.Text + "'";
                    strQuery += ", @pSL_CD ='" + txtSlFr.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                        Set_Section();
                    else
                        SystemBase.Validation.GroupBox_Reset(groupBox2);
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
                    string strSql = " usp_ITR114 'D1'";
                    strSql += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strSql += ", @pITEM_CD ='" + txtItemCd.Text.Trim() + "'";
                    strSql += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strSql += ", @pINV_YY  ='" + dtpYear.Text + "'";
                    strSql += ", @pSL_CD ='" + txtSlFr.Text.Trim() + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                    Trans.Commit();

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


        #region 합계 그리드 재정의, 금액 표시
        private void Set_Section()
        {
            int iCnt = fpSpread1.Sheets[0].RowCount;

            //합계 컬럼 합치고 색 변경
            for (int i = 0; i < iCnt; i++)
            {

                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text == "합계")
                {

                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].ColumnSpan = 4;
                    fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;

                    //합계 색 변경
                    for (int j = 1; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                    }

                    //이월급액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월재고_2")].Text != "")
                        txtBasAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월재고_2")].Value;
                    else
                        txtBasAmt.Value = 0;

                    //입고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고_2")].Text != "")
                        txtMvmtAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고_2")].Value;
                    else
                        txtMvmtAmt.Value = 0;

                    //출고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고_2")].Text != "")
                        txtTranAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고_2")].Value;
                    else
                        txtTranAmt.Value = 0;

                    //재고금액
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고_2")].Text != "")
                        txtDnAmt.Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고_2")].Value;
                    else
                        txtDnAmt.Value = 0;

                }
            }
        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Text = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

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

        private void btnSlFr_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON 'B035', @pSPEC1 = '" + cboPlantCd.SelectedValue.ToString() + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtSlFr.Text, "" };

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00014", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "창고팝업");
                pu.ShowDialog();

                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtSlFr.Text = Msgs[0].ToString();
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
        #endregion

        #region 레포트 출력
        private void btnPreview_Click(object sender, System.EventArgs e)
        {
            //조회 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //string[] FormulaField = new string[2];	  //formula 값			
                string RptName = SystemBase.Base.ProgramWhere + @"\Report\ITR114.rpt";    // 레포트경로+레포트명
                string[] RptParmValue = new string[8];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                RptParmValue[0] = "R1";
                RptParmValue[1] = SystemBase.Base.gstrCOMCD;
                RptParmValue[2] = Convert.ToString(cboPlantCd.SelectedValue);
                RptParmValue[3] = txtItemCd.Text;
                RptParmValue[4] = Convert.ToString(cboItemAcct.SelectedValue);
                RptParmValue[5] = dtpYear.Text;
                RptParmValue[6] = txtSlFr.Text;

                FormulaFieldValue[0] += "\"" + cboItemAcct.Text + "\"";
                FormulaFieldName[0] = "ACCT_NM";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text, FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전				
                frm.ShowDialog();
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void ITR114_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void ITR114_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        private void btnUpLoad_Click(object sender, System.EventArgs e)
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

                    string commandString = String.Format("SELECT * FROM [수불집계표$]", worksheets.Rows[0]["TABLE_NAME"]);
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
                                //UIForm.FPMake.RowInsert(fpSpread1);
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "창고")].Text = ds.Tables[0].Rows[i][0].ToString().Trim();							// 0
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][1].ToString().Trim();						// 1
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text = ds.Tables[0].Rows[i][3].ToString().Trim();							// 3
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text = ds.Tables[0].Rows[i][4].ToString().Trim();							// 4
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월수량")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][5].ToString().Trim());    // 5
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "이월금액")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][6].ToString().Trim());    // 6
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고수량")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][7].ToString().Trim());    // 7
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "입고금액")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][8].ToString().Trim());    // 8
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고수량")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][9].ToString().Trim());    // 9
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출고금액")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][10].ToString().Trim());   // 10
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고수량")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][11].ToString().Trim());   // 11
                                //fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "재고금액")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][12].ToString().Trim());   // 12

                                string strSql = " usp_ITR114 'I1 '";
                                strSql += " , @pCO_CD  ='" +  SystemBase.Base.gstrCOMCD + "' ";
                                strSql += " , @pINV_YY ='" +  dtpYear.Text + "' ";
                                strSql += " , @SL_CD   ='" +  ds.Tables[0].Rows[i][0].ToString().Trim() + "' ";
                                strSql += " , @ITEM_CD ='" +  ds.Tables[0].Rows[i][1].ToString().Trim() + "' ";
                                strSql += " , @ITEM_SPEC='" + ds.Tables[0].Rows[i][3].ToString().Trim() + "' ";
                                strSql += " , @ITEM_UNIT='" + ds.Tables[0].Rows[i][4].ToString().Trim() + "' ";
                                strSql += " , @OVER_QTY =" + Convert.ToDouble(ds.Tables[0].Rows[i][5].ToString().Trim());
                                strSql += " , @OVER_AMT =" + Convert.ToDouble(ds.Tables[0].Rows[i][6].ToString().Trim());
                                strSql += " , @IN_QTY   =" + Convert.ToDouble(ds.Tables[0].Rows[i][7].ToString().Trim());
                                strSql += " , @IN_AMT   =" + Convert.ToDouble(ds.Tables[0].Rows[i][8].ToString().Trim());
                                strSql += " , @OUT_QTY  =" + Convert.ToDouble(ds.Tables[0].Rows[i][9].ToString().Trim());
                                strSql += " , @OUT_AMT  =" + Convert.ToDouble(ds.Tables[0].Rows[i][10].ToString().Trim());
                                strSql += " , @INV_QTY  =" + Convert.ToDouble(ds.Tables[0].Rows[i][11].ToString().Trim());
                                strSql += " , @INV_AMT  =" + Convert.ToDouble(ds.Tables[0].Rows[i][12].ToString().Trim());
                                strSql += " , @IN_ID    ='" + SystemBase.Base.gstrUserID + "'";


                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }   // ER 코드 Return시 점프 
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


        private void btnDnLoad_Click(object sender, System.EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();
        }
    

    }
}
