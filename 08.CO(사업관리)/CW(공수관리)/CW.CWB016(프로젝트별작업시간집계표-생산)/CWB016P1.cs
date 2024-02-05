#region 작성정보
/*********************************************************************/
// 단위업무명:  프로젝트별 작업시간 집계 데이터 엑셀 업로드
// 작 성 자  :  한 미 애
// 작 성 일  :  2017-02-21
// 작성내용  :  프로젝트별 월별 작업시간 엑셀 업로드 처리
// 수 정 일  :
// 수 정 자  :
// 수정내용  :
// 비    고  :
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

namespace CW.CWB016
{ 
    public partial class CWB016P1 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strPLANT_CD = "";
        #endregion

        #region 생성자
        public CWB016P1()
        {
            InitializeComponent();           
        }

        public CWB016P1(string sPLANT_CD)
        {
            InitializeComponent();
            strPLANT_CD = sPLANT_CD;
        }
        #endregion

        #region Form Load 시
        private void CWB016P1_Load(object sender, System.EventArgs e)
        {
            this.Text = "프로젝트별공수업로드";
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            UIForm.Buttons.ReButton("010000001000", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);

            cboYYYYMM.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYYYMM")).ToString().Substring(0, 7);
            //fpSpread1.Visible = false;
            //GridCommGroupBox.Visible = false;
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region 데이타 여부 체크
        private bool Exists_Check()
        {
            bool exists = false;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_CWB016 'C1'";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                    strQuery += ", @pPLANT_CD ='" + strPLANT_CD + "'";
                    strQuery += ", @pDEPT_TYPE = 'M' ";
                    strQuery += ", @pYYYYMM = '" + cboYYYYMM.Text + "' ";

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


        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {

                string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=no;""", txtFilePath.Text);
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();

                DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[0]["TABLE_NAME"]);
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

                try
                {
                    string strYYYYMM = "";
                    strYYYYMM = cboYYYYMM.Text.Replace("-", "");

                    if (Exists_Check())
                    {
                        string msg = " 데이타가 존재합니다. 전부 지우고 다시 생성하시겠습니까?";
                        DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (dsMsg1 == DialogResult.Yes)
                        {
                            string strDelSql = " usp_CWB016 'D1' ";
                            strDelSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";
                            strDelSql += ", @pPLANT_CD = '" + strPLANT_CD + "'";
                            strDelSql += ", @pDEPT_TYPE = 'M' ";
                            strDelSql += ", @pYYYYMM ='" + strYYYYMM + "'";

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프						 
                        }
                    }

                    string strProjectNo = "";
                    string fWorkHours = "0";

                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                        {
                            strProjectNo = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "프로젝트")].Text.Trim().Replace(" ", "");
                            fWorkHours = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "공수")].Text.Trim().Replace(" ", "");

                            string strSql = " usp_CWB016 'I1 '";
                            strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                            strSql += ", @pPLANT_CD ='" + SystemBase.Base.gstrPLANT_CD + "' ";
                            strSql += ", @pDEPT_TYPE = 'M' ";
                            strSql += ", @pLAB_TYPE = '직접' ";
                            strSql += ", @pPROJECT_NO = '" + strProjectNo + "' ";
                            strSql += ", @pYYYYMM = '" + strYYYYMM + "' ";
                            strSql += ", @pWORK_HOURS = '" + fWorkHours + "' ";
                            strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

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
            this.Cursor = Cursors.Default;
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = dlg.FileName;
            }
        }

        private void btnFileDownload_Click(object sender, EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();

        }


    }
}

