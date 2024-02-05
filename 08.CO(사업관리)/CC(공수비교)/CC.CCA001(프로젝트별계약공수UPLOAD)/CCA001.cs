#region 작성정보
/*********************************************************************/
// 단위업무명 :프로젝트별 계약공수 upLoad
// 작 성 자 : 조 홍 태
// 작 성 일 : 2013-08-27
// 작성내용 : 프로젝트별 계약공수 upLoad
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

namespace CC.CCA001
{
    public partial class CCA001 : UIForm.FPCOMM1
    {
        #region 생성자
        public CCA001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void CCA001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD.ToString();

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            txtPlant_CD.Text = SystemBase.Base.gstrPLANT_CD.ToString();

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
                    string strQuery = "usp_CCA001 @pTYPE = 'S1'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD.ToString() + "' ";
                    strQuery = strQuery + ", @pPLANT_CD = '" + txtPlant_CD.Text + "' ";
                    strQuery = strQuery + ", @pPROJECT_NO = '" + txtProject_No.Text + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, true, true, 0, 0, true);
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
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
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

                        // 2017.10.19. hma 수정(Start): 윈도우 보안 업데이트후 문제가 생겨서 엑셀 업로드시 OLEDB 및 Excel 버전 부분 수정함.
                        //string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", dlg.FileName);
                        string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;Imex=1;hdr=yes;""", dlg.FileName);
                        // 2017.10.19. hma 수정(End)
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

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                            SqlCommand cmd1 = dbConn.CreateCommand();
                            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

                            //기존 저장 PROJECT 데이터 확인 후 삭제 OR 취소
                            if (Exists_is())
                            {
                                string msg = "프로젝트번호 : " + txtProject_No.Text.Trim() + " 의 데이타가 존재합니다. 다시 생성하시겠습니까?";
                                DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (dsMsg1 == DialogResult.Yes)
                                {
                                    string strDelSql = " usp_CCA001 'D1' ";
                                    strDelSql += ", @pPLANT_CD = '" + txtPlant_CD.Text + "'";
                                    strDelSql += ", @pPROJECT_NO = '" + txtProject_No.Text.Trim() + "' ";
                                    strDelSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

                                    DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                                    ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프						 
                                }
                                else
                                {
                                    MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //작업이 취소되었습니다.
                                    this.Cursor = Cursors.Default;
                                    return;
                                }
                            }

                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                UIForm.FPMake.RowInsert(fpSpread1);

                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text = ds.Tables[0].Rows[i][0].ToString().Trim();						                //0.품목코드
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][1].ToString().Trim());	    //1.기계
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_2")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][2].ToString().Trim());		//2.전자
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_3")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][3].ToString().Trim());		//3.검사
                                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_4")].Value = Convert.ToDouble(ds.Tables[0].Rows[i][4].ToString().Trim());		//4.기술센터
                            }

                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // 저장
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            try
                            {
                                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                                {
                                    string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                                    string strGbn = "";

                                    if (strHead == "I") { strGbn = "i1"; }

                                    string strSql = "";
                                    strSql += " usp_CCA001 '" + strGbn + "'";
                                    strSql += ", @pPLANT_CD = '" + txtPlant_CD.Text + "'";
                                    strSql += ", @pPROJECT_NO = '" + txtProject_No.Text + "'";
                                    strSql += ", @pITEM_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목코드")].Text + "'";
                                    strSql += ", @pDIR_M_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)")].Value + "'";
                                    strSql += ", @pDIR_E_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_2")].Value + "'";
                                    strSql += ", @pDIR_Q_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_3")].Value + "'";
                                    strSql += ", @pDIR_S_TM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "직접작업시간(1개기준)_4")].Value + "'";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프


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
                            }
                            else
                            {
                                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #region 데이타 여부 체크
        private bool Exists_is()
        {
            bool exists = false;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_CCA001 'C1'";
                    strQuery += ", @pPLANT_CD = '" + txtPlant_CD.Text.Trim() + "' ";
                    strQuery += ", @pPROJECT_NO='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD.ToString() + "'";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);
                    if (dt.Rows[0][0].ToString() == "1") exists = true;

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            return exists;
        }
        #endregion

        #region 버튼 Click
        //공장 팝업
        private void btnPlant_CD_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_P_COMMON 'P011' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";								// 쿼리
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };				// 쿼리 인자값(조회조건)
                string[] strSearch = new string[] { txtPlant_CD.Text, "" };	// 쿼리 인자값에 들어갈 데이타

                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00005", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "공장 조회", false);

                pu.ShowDialog();	//공통 팝업 호출
                if (pu.DialogResult == DialogResult.OK)
                {
                    string MSG = pu.ReturnVal.Replace("|", "#");
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(MSG);

                    txtPlant_CD.Value = Msgs[0].ToString();
                    txtPlant_NM.Value = Msgs[1].ToString();
                }


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //프로젝트팝업
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW003 pu = new WNDW003(txtProject_No.Text, "S1", "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtProject_No.Value = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged 이벤트
        // 공장
        private void txtPlant_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtPlant_CD.Text != "")
                {
                    txtPlant_NM.Value = SystemBase.Base.CodeName("PLANT_CD", "PLANT_NM", "B_PLANT_INFO", txtPlant_CD.Text, " AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");
                }
                else
                {
                    txtPlant_NM.Value = "";
                }
            }
            catch
            {

            }
        }

        //프로젝트번호
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            string Query = "SELECT TOP 1 PROJECT_NM FROM S_SO_MASTER(NOLOCK) WHERE PROJECT_NO = '" + txtProject_No.Text + "'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                txtProject_Nm.Value = dt.Rows[0][0].ToString();
            }
            else
            {
                txtProject_Nm.Value = "";
            }
        }
        #endregion
    }
}
