#region 작성정보
/*********************************************************************/
// 단위업무명 :개발비 이력관리
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-27
// 작성내용 : 개발비 이력관리
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
using System.Data.OleDb;

namespace AH.ACH019
{
    public partial class ACH019 : UIForm.FPCOMM1
    {
        #region 생성자
        public ACH019()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void ACH019_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboResearchType, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'D031', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);   //비목
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "비목")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pType='COMM', @pCODE = 'D031', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//비목

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    string strQuery = "usp_ACH019 @pTYPE = 'S1'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    strQuery = strQuery + ", @pISSUE_DT_FR = '" + dtpIssueDtFr.Text + "' ";
                    strQuery = strQuery + ", @pISSUE_DT_TO = '" + dtpIssueDtTo.Text + "' ";
                    strQuery = strQuery + ", @pRESEARCH_TYPE = '" + cboResearchType.SelectedValue.ToString() + "' ";
                    strQuery = strQuery + ", @pRESEARCH_NAME = '" + txtResearchNm.Text + "' ";
                    strQuery = strQuery + ", @pDEPR_ST_YY = '" + txtDeprStYY.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 엑셀/저장
        private void btnUpLoad_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // 엑셀 upload
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    conn.Open();

                    DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[0]["TABLE_NAME"]);
                    OleDbCommand cmd = new OleDbCommand(commandString, conn);

                    OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    dapt.Fill(ds);
                    conn.Close();

                    UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

                    int j = 0; 

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[i][1].ToString() != "" && ds.Tables[0].Rows[i][1].ToString() != null)
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);

                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "비목")].Value = ds.Tables[0].Rows[i][0].ToString().Trim();					                //0.비목
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].Text = ds.Tables[0].Rows[i][1].ToString().Trim();			                    //1.발생일자
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "상각시작년도")].Text = ds.Tables[0].Rows[i][2].ToString().Trim();						    //2.상각시작년도
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "세부내역")].Text = ds.Tables[0].Rows[i][3].ToString().Trim();	                            //3.세부내역
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value = ds.Tables[0].Rows[i][4];		                                        //4.발생금액
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_2")].Value = ds.Tables[0].Rows[i][5];		                                        //5.국고보조금
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_3")].Value = ds.Tables[0].Rows[i][6];		                                        //6.상각대상금액
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "상각년수")].Text = ds.Tables[0].Rows[i][7].ToString().Trim();		                        //7.상각년수
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "구분")].Text = ds.Tables[0].Rows[i][8].ToString().Trim();		                            //8.구분
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "전년도상각누계액")].Value = ds.Tables[0].Rows[i][9];	                                    //9.전년도상각누계액
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액")].Value = ds.Tables[0].Rows[i][10];		                                    //10.순개발비
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_2")].Value = ds.Tables[0].Rows[i][11];		                                    //11.국고보조금
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_3")].Value = ds.Tables[0].Rows[i][12];		                                    //12.상각비
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "상각누계액")].Value = ds.Tables[0].Rows[i][13];		                                        //13.상각누계액
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "기말잔액")].Value = ds.Tables[0].Rows[i][14];		                                        //14.기말잔액
                                fpSpread1.Sheets[0].Cells[j, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text = ds.Tables[0].Rows[i][15].ToString().Trim();		                        //15.전표번호

                                j++;
                            }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 저장
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            string gl_no = "", aaa = "";

            try
            {
               

                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                    string strGbn = "";

                    if (strHead != "")
                    {
                        if (strHead == "I") { strGbn = "I1"; }
                        else if (strHead == "U") { strGbn = "U1"; }
                        else if (strHead == "D") { strGbn = "D1"; }

                        string strSql = "";
                        strSql += " usp_ACH019 '" + strGbn + "'";

                        if (strGbn == "U1" || strGbn == "D1")
                        {
                            strSql += ", @pIDX = '" + fpSpread1.Sheets[0].Cells[i, 0].Value + "'";
                        }
                        strSql += ", @pRESEARCH_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비목")].Value + "'";
                        strSql += ", @pISSUE_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생일자")].Text + "'";
                        strSql += ", @pDEPR_ST_YY = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상각시작년도")].Text + "'";
                        strSql += ", @pRESEARCH_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세부내역")].Text + "'";
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Text != "")
                        {
                            strSql += ", @pISSUE_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_2")].Text != "")
                        {
                            strSql += ", @pSUBSIDY_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_2")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_3")].Text != "")
                        {
                            strSql += ", @pDEPR_TARGET_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발생금액_3")].Value + "'";
                        }
                        strSql += ", @pDEPR_YEAR = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상각년수")].Text + "'";
                        strSql += ", @pCUST_NAME = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "협약업체")].Text + "'";
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전년도상각누계액")].Text != "")
                        {
                            strSql += ", @pPRE_YEAR_TOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전년도상각누계액")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액")].Text != "")
                        {
                            strSql += ", @pCUR_RESEARCH_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_2")].Text != "")
                        {
                            strSql += ", @pCUR_SUBSIDY_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_2")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_3")].Text != "")
                        {
                            strSql += ", @pCUR_DEPR_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "당기상각금액_3")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상각누계액")].Text != "")
                        {
                            strSql += ", @pDEPR_TOT_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상각누계액")].Value + "'";
                        }
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기말잔액")].Text != "")
                        {
                            strSql += ", @pEND_REMAIN_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "기말잔액")].Value + "'";
                        }
                        strSql += ", @pSLIP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text + "'";

                        strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                        strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        gl_no = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호")].Text;
                        aaa = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "세부내역")].Text;

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                    }
                }
                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                SearchExec();
            }
            else if (ERRCode == "ER")
            { 
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); 
                MessageBox.Show(gl_no + " : " + aaa);
                
            }
            else
            { MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            this.Cursor = this.DefaultCursor;
        }
        #endregion

        #region 양식 다운로드
        private void btnDnLoad_Click(object sender, System.EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();
        }

        #endregion

        #region 그리드 상 팝업
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            //전표조회
            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "전표번호_2"))
            {
                try
                {
                    if (fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결의전표")].Text != "")
                    {
                        WNDW.WNDW026 pu = new WNDW.WNDW026(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "결의전표")].Text);
                        pu.ShowDialog();
                    }

                    fpSpread1.Sheets[0].RowHeader.Cells[e.Row, 0].Text = "";
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "전표번호 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
