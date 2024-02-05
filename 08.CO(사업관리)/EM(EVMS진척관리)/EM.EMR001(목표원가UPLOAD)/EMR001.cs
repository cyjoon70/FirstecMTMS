#region 작성정보
/*********************************************************************/
// 단위업무명 : 목표원가UPLOAD
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-23
// 작성내용 : 목표원가UPLOAD 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;
using System.Threading;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace EM.EMR001
{
    public partial class EMR001 : UIForm.Buttons
    {
        #region 변수선언
        string strBtn = "N";
        bool form_act_chk = false;
        int sheet_su = 0;
        UIForm.ExcelWaiting Waiting_Form = null;
        Thread th;
        #endregion

        #region 생성자
        public EMR001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void EMR001_Load(object sender, System.EventArgs e)
        {
            //필수 체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            rdoAll.Checked = true;
            rdoEqual.Checked = true;
            rdo_chk(); 
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            //필수체크
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            rdoAll.Checked = true;
            rdoEqual.Checked = true;
            rdo_chk(); 
        }
        #endregion
        
        #region 팝업창 열기(품목)
        private void btnProj_Click(object sender, System.EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Value = Msgs[3].ToString();
                    txtProjNm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
            strBtn = "N";
        }

        private void btnFile_Click(object sender, System.EventArgs e)
        {
            try
            {
                string sTemp = "";
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";
                cboSheet.Items.Clear();	
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Value = dlg.FileName;
                    UIForm.VkExcel excel = new UIForm.VkExcel(false);
                    excel.OpenFile(dlg.FileName);
                    string[] sheet_name = excel.GetExcelSheetLists();
                    sheet_su = sheet_name.GetUpperBound(0) + 1;
                    for (int i = 0; i < sheet_su; i++)
                        cboSheet.Items.Add(sheet_name[i]);
                    excel.CloseFile();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void txtProjNo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (strBtn == "N")
                {
                    if (txtProjNo.Text != "")
                    {
                        txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND SO_CONFIRM_YN = 'Y' ");
                    }
                    else
                    {
                        txtProjNm.Value = "";
                    }
                }                
            }
            catch
            {

            }
        }
        #endregion
        
        #region 엑셀UPLOAD
        private void btnFileUpload_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                //기존에 값 0로 만들기
                bool delchk = DelData();
                if (delchk == false) return;


                string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=yes;""", txtFilePath.Text);
                OleDbConnection conn = new OleDbConnection(connectionString);

                string ERRCode = "";
                string MSGCode = "";
                string item_cd = "";
                string prj_seq = "";
                string[] temp = { "", "" };
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                string strCostClass = "", strCostElement = "";
                string TempCostClass = "";
                string temp1, temp2, temp3;
                string s_year = "";
                string s_ym = "";
                string s_mon = "";

                int idx = 0;

                try
                {

                    //전체-------------------------------------------
                    if (rdoAll.Checked == true)
                    {
                        th = new Thread(new ThreadStart(Show_Waiting));
                        th.Start();
                        Thread.Sleep(1000);
                        Waiting_Form.Activate();
                        Waiting_Form.label_temp.Text = "";
                        Waiting_Form.progressBar_temp.Maximum = sheet_su;

                        for (idx = 0; idx < sheet_su; idx++)
                        {
                            Waiting_Form.label_temp.Text = "총" + sheet_su + " Sheet 중 " + (idx + 1).ToString() + " Sheet 를 작업중입니다.";

                            conn.Open();
                            DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            item_cd = temp[0].Trim();
                            prj_seq = temp[1].Trim();

                            string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[idx]["TABLE_NAME"]);
                            OleDbCommand cmd = new OleDbCommand(commandString, conn);

                            OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                            DataSet ds = new DataSet();

                            dapt.Fill(ds);
                            conn.Close();

                            if (idx > 0) Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                            fpSpread1.Sheets[0].RowCount = 0;
                            fpSpread1.Sheets[0].DataSource = ds;
                            string strSql1 = "";
                            if (fpSpread1.Sheets[0].RowCount != 0)
                            {
                                for (int i = 5; i < fpSpread1.Sheets[0].RowCount; i++)
                                {
                                    temp1 = fpSpread1.Sheets[0].Cells[i, 0].Text.Trim();
                                    temp2 = fpSpread1.Sheets[0].Cells[i, 1].Text.Trim().Replace(" ", "");
                                    temp3 = fpSpread1.Sheets[0].Cells[i, 2].Text.Trim().Replace(" ", "");

                                    if (temp1 == "" && temp2 == "" && temp3 == "") break;

                                    if (temp1 == "수출보전금") continue;  //임시
                                    if (temp2 == "적용이윤") continue;  //임시

                                    if (temp1 == "") strCostClass = TempCostClass;
                                    else strCostClass = temp1;

                                    if (temp2 != "")
                                    {
                                        if (temp3 != "")
                                            strCostElement = temp3;
                                        else
                                            strCostElement = temp2;
                                    }
                                    else
                                    {
                                        if (temp3 != "")
                                        {
                                            if (temp3 == "소계") continue; //소계 코드화 안되어 있음
                                            strCostElement = temp3;
                                        }
                                        else
                                            strCostElement = strCostClass;
                                    }

                                    for (int k = 8; k < 32; k++)
                                    {
                                        if (fpSpread1.Sheets[0].Cells[i, k].Text.Trim() == "") continue;
                                        if (fpSpread1.Sheets[0].Cells[i, k].Value.ToString() == "0") continue;

                                        if (k >= 20) s_year = Convert.ToInt16(fpSpread1.Sheets[0].Cells[3, 20].Value).ToString();
                                        else s_year = Convert.ToInt16(fpSpread1.Sheets[0].Cells[3, 8].Value).ToString();

                                        s_mon = Convert.ToInt16(fpSpread1.Sheets[0].Cells[4, k].Value).ToString();

                                        if (s_mon.Length == 1) s_ym = s_year + "0" + s_mon;
                                        else s_ym = s_year + s_mon;

                                        strSql1 = " usp_EMR001 'I1 '";
                                        strSql1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' "; ;
                                        strSql1 += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                                        strSql1 += ", @pPROJECT_SEQ  = '" + prj_seq + "' ";
                                        strSql1 += ", @pITEM_CD = '" + item_cd + "' ";
                                        strSql1 += ", @pCOST_CLASS_NM = '" + strCostClass + "' ";
                                        strSql1 += ", @pCOST_ELEMENT_NM = '" + strCostElement + "' ";
                                        strSql1 += ", @pYYYYMM = '" + s_ym + "' ";
                                        strSql1 += ", @pCOST_PV_T = '" + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, k].Value) + "' ";
                                        strSql1 += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";

                                        DataSet ds2 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                        ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                                        MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                                        if (ERRCode != "OK") { th.Abort(); Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프 

                                    }
                                    TempCostClass = strCostClass;

                                }
                                strSql1 = " usp_EMR001 'U1 '";
                                strSql1 += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' "; ;
                                strSql1 += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                                strSql1 += ", @pITEM_CD = '" + item_cd + "' ";
                                strSql1 += ", @pPROJECT_SEQ   = '" + prj_seq + "' ";

                                DataSet ds4 = SystemBase.DbOpen.TranDataSet(strSql1, dbConn, Trans);
                                ERRCode = ds4.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds4.Tables[0].Rows[0][1].ToString();

                                Trans.Commit();
                                Trans.Dispose();
                            }
                            Waiting_Form.progressBar_temp.Value = idx + 1;
                        }
                        th.Abort();
                    }
                    //선택한 시트-------------------------------------------
                    else
                    {
                        if (cboSheet.Text.Trim() == "")
                        {
                            MSGCode = "워크시트를 선택하세요!";
                            ERRCode = "WR";
                            goto Exit;
                        }
                        conn.Open();

                        DataTable worksheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (rdoSheet.Checked == true) idx = cboSheet.SelectedIndex;

                        temp = cboSheet.Text.Split('&');
                        item_cd = temp[0].Trim();
                        prj_seq = temp[1].Trim();

                        string commandString = String.Format("SELECT * FROM [{0}]", worksheets.Rows[idx]["TABLE_NAME"]);
                        OleDbCommand cmd = new OleDbCommand(commandString, conn);

                        OleDbDataAdapter dapt = new OleDbDataAdapter(cmd);
                        DataSet ds = new DataSet();

                        dapt.Fill(ds);
                        conn.Close();

                        fpSpread1.Sheets[0].RowCount = 0;
                        fpSpread1.Sheets[0].DataSource = ds;
                        string strSql = "";
                        if (fpSpread1.Sheets[0].RowCount != 0)
                        {
                            for (int i = 5; i < fpSpread1.Sheets[0].RowCount; i++)
                            {
                                temp1 = fpSpread1.Sheets[0].Cells[i, 0].Text.Trim();
                                temp2 = fpSpread1.Sheets[0].Cells[i, 1].Text.Trim().Replace(" ", "");
                                temp3 = fpSpread1.Sheets[0].Cells[i, 2].Text.Trim().Replace(" ", "");

                                if (temp1 == "" && temp2 == "" && temp3 == "") break;

                                if (temp1 == "수출보전금") continue;  //임시
                                if (temp2 == "적용이윤") continue;  //임시

                                if (temp1 == "") strCostClass = TempCostClass;
                                else strCostClass = temp1;

                                if (temp2 != "")
                                {
                                    if (temp3 != "")
                                        strCostElement = temp3;
                                    else
                                        strCostElement = temp2;
                                }
                                else
                                {
                                    if (temp3 != "")
                                    {
                                        if (temp3 == "소계") continue; //소계 코드화 안되어 있음
                                        strCostElement = temp3;
                                    }
                                    else
                                        strCostElement = strCostClass;
                                }

                                for (int k = 8; k < 32; k++)
                                {
                                    if (fpSpread1.Sheets[0].Cells[i, k].Text.Trim() == "") continue;
                                    if (fpSpread1.Sheets[0].Cells[i, k].Value.ToString() == "0") continue;

                                    if (k >= 20) s_year = Convert.ToInt16(fpSpread1.Sheets[0].Cells[3, 20].Value).ToString();
                                    else s_year = Convert.ToInt16(fpSpread1.Sheets[0].Cells[3, 8].Value).ToString();

                                    s_mon = Convert.ToInt16(fpSpread1.Sheets[0].Cells[4, k].Value).ToString();

                                    if (s_mon.Length == 1) s_ym = s_year + "0" + s_mon;
                                    else s_ym = s_year + s_mon;

                                    strSql = " usp_EMR001 'I1 '";
                                    strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' "; ;
                                    strSql += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                                    strSql += ", @pCOST_CLASS_NM = '" + strCostClass + "' ";
                                    strSql += ", @pCOST_ELEMENT_NM = '" + strCostElement + "' ";
                                    strSql += ", @pITEM_CD = '" + item_cd + "' ";
                                    strSql += ", @pPROJECT_SEQ   = '" + prj_seq + "' ";
                                    strSql += ", @pYYYYMM = '" + s_ym + "' ";
                                    strSql += ", @pCOST_PV_T = '" + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, k].Value) + "' ";
                                    strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";


                                    DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds1.Tables[0].Rows[0][1].ToString();
                                    if (ERRCode == "WR")
                                    {
                                        DialogResult dsMsg = MessageBox.Show(MSGCode + Environment.NewLine + "계속진행하겠습니까?", SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                        if (dsMsg != DialogResult.Yes) { MSGCode = ""; Trans.Rollback(); goto Exit; }

                                    }
                                    if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프 

                                }
                                TempCostClass = strCostClass;

                            }
                            strSql = " usp_EMR001 'U1 '";
                            strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' "; ;
                            strSql += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                            strSql += ", @pITEM_CD = '" + item_cd + "' ";
                            strSql += ", @pPROJECT_SEQ = '" + prj_seq + "' ";

                            DataSet ds3 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds3.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds3.Tables[0].Rows[0][1].ToString();

                            Trans.Commit();
                        }

                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    Trans.Rollback();
                    ERRCode = "ER";
                    MSGCode = f.Message;
                    if (rdoAll.Checked == true) th.Abort();
                }
            Exit:
                dbConn.Close();
                Waiting_Form = null;
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
                    if (MSGCode != "")
                        MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            this.Cursor = Cursors.Default;

        }

        private void Show_Waiting()
        {
            Waiting_Form = new UIForm.ExcelWaiting("EVMS 파일 UPLOAD");
            Waiting_Form.ShowDialog();
        }
        #endregion

        #region Activated
        private void EMR001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProjNo.Focus();
        }

        private void EMR001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
        
        #region rdo CheckedChanged
        private void rdoSheet_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoSheet.Checked == true) rdo_chk();
        }

        private void rdoAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdoAll.Checked == true) rdo_chk();
        }

        private void rdo_chk()
        {
            if (rdoSheet.Checked == true) cboSheet.Enabled = true;
            else cboSheet.Enabled = false;
        }
        #endregion

        #region 버튼 Click
        private void btnFileDownload_Click(object sender, System.EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            //UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            //form1.ShowDialog();
        }
        #endregion

        #region Del() 빈값으로 넣기 로직
        private bool DelData()
        {

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_EMR001  'D1'";
                strSql += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                Trans.Rollback();
                ERRCode = "ER";
                MSGCode = f.Message;
                //MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();
            if (ERRCode != "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
            {
                return true;
            }


        }
        #endregion
 }
}
