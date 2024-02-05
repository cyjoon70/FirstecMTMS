#region 작성정보
/*********************************************************************/
// 단위업무명 : 계약원가 UPLOAD
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-19
// 작성내용 : 계약원가 UPLOAD
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

namespace CS.CSA001
{
    public partial class CSA001 : UIForm.Buttons
    {
        #region 변수선언
        string strBtn = "N";
        bool form_act_chk = false;
        #endregion

        public CSA001()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void CSA001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //GroupBox3입력조건 콥보박스 세팅			
            SystemBase.ComboMake.C1Combo(cboContSeq, "usp_B_COMMON @pType='COMM', @pCODE = 'C003', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'", 0);

            dtpCont_App_Dt.Text = SystemBase.Base.ServerTime("YYMMDD");

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpCont_App_Dt.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion


        #region 팝업창 
        private void btnItem_Click_1(object sender, EventArgs e)
        {
            strBtn = "Y";
            try
            {
                WNDW007 pu = new WNDW007(txtProjNo.Text, "N");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProjNo.Text = Msgs[3].ToString();
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

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "통합 Excel 문서(*.xls)|*.xls|2007 Excel 문서(*.xlsx)|*.xlsx";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = dlg.FileName;
            }
        }
        #endregion

        #region TextChanged
        private void btnProj_TextChanged(object sender, EventArgs e)
        {
            if (strBtn == "N")
            {
                txtProjNm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProjNo.Text, " AND SO_CONFIRM_YN = 'Y'  AND CO_CD='" + SystemBase.Base.gstrCOMCD + "' ");

            }
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
                    string strQuery = " usp_CSA001 'S1'";
                    strQuery += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                    strQuery += ", @pPROJECT_NO='" + txtProjNo.Text.Trim() + "'";
                    strQuery += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";
                    strQuery += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

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

        #region 엑셀UPLOAD
        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                // 2017.10.19. hma 수정(Start): 윈도우 보안 업데이트후 문제가 생겨서 엑셀 업로드시 OLEDB 부분과 Excel 버전 부분 수정함.
                //string connectionString = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;Imex=1;hdr=no;""", txtFilePath.Text);
                string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;Imex=1;hdr=no;""", txtFilePath.Text);
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
                fpSpread1.Sheets[0].RowCount = 0;
                fpSpread1.Sheets[0].DataSource = ds;

                string ERRCode = "", MSGCode = "";
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd1 = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    if (Exists_is())
                    {
                        string msg = "프로젝트번호 : " + txtProjNo.Text.Trim() + " 의 데이타가 존재합니다. 전부 지우고 다시 생성하시겠습니까? (품목추가 시 NO)";
                        DialogResult dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (dsMsg1 == DialogResult.Yes)
                        {
                            string strDelSql = " usp_CSA001 'D1' ";
                            strDelSql += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                            strDelSql += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";
                            strDelSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

                            DataSet ds2 = SystemBase.DbOpen.TranDataSet(strDelSql, dbConn, Trans);
                            ERRCode = ds2.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds2.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프						 
                        }
                        else
                        {
                            msg = "기존데이타에 추가 생성하시겠습니까? (품목추가 시 Yes)";
                            dsMsg1 = MessageBox.Show(msg, SystemBase.Base.MessageRtn("Z0004"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (dsMsg1 == DialogResult.No)
                            {
                                dsMsg1 = MessageBox.Show(SystemBase.Base.MessageRtn("B0040"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //작업이 취소되었습니다.
                                this.Cursor = Cursors.Default;
                                return;
                            }
                        }
                    }

                    string[] strItemCd = null;
                    string strCostClass = "", strCostElement = "";
                    string TempCostClass = "";
                    string temp1, temp2;

                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        int row_idx = 0;
                        int item_su = 0;
                        int item_start_idx = 3;
                        int rate = 2;
                        strItemCd = new string[fpSpread1.Sheets[0].Columns.Count - item_start_idx];
                        for (int j = item_start_idx; j < fpSpread1.Sheets[0].Columns.Count; j++)
                        {
                            if (fpSpread1.Sheets[0].Cells[row_idx, j].Text.Trim() != "")
                            {
                                strItemCd[j - item_start_idx] = fpSpread1.Sheets[0].Cells[row_idx, j].Text.Trim();
                                item_su++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        for (int i = 2; i < ds.Tables[0].Rows.Count; i++)
                        {
                            temp1 = fpSpread1.Sheets[0].Cells[i, 0].Text.Trim().Replace(" ", "");
                            temp2 = fpSpread1.Sheets[0].Cells[i, 1].Text.Trim().Replace(" ", "");

                            if (temp1 == "" && temp2 == "") break;

                            if (temp1 == "") strCostClass = TempCostClass;
                            else strCostClass = temp1;

                            if (temp2 != "")
                            {
                                strCostElement = temp2;
                            }
                            else
                            {
                                strCostElement = strCostClass;
                            }

                            for (int k = 0; k < item_su; k++)
                            {
                                string strSql = " usp_CSA001 'I1 '";
                                strSql += ", @pLANG_CD = '" + SystemBase.Base.gstrLangCd + "' ";
                                strSql += ", @pCONT_SEQ = '" + cboContSeq.SelectedValue + "' ";
                                strSql += ", @pPROJECT_NO = '" + txtProjNo.Text.Trim() + "' ";
                                strSql += ", @pCOST_CLASS_NM = '" + strCostClass + "' ";
                                strSql += ", @pCOST_ELEMENT_NM = '" + strCostElement + "' ";
                                strSql += ", @pITEM_CD = '" + strItemCd[k] + "' ";
                                strSql += ", @pITEM_SEQ= '" + (k + 1) + "' ";
                                strSql += ", @pCONT_APP_DT= '" + dtpCont_App_Dt.Text + "' ";

                                if (fpSpread1.Sheets[0].Cells[i, item_start_idx + k].Text.Trim() == "")
                                    strSql += ", @pCOST_PRICE = 0 ";
                                else
                                {
                                    bool t1 = fpSpread1.Sheets[0].Cells[i, item_start_idx + k].Value.ToString().Contains("%");
                                    
                                    if (t1 == true)
                                    {
                                        string t2 = fpSpread1.Sheets[0].Cells[i, item_start_idx + k].Value.ToString().Replace("%", "");
                                        decimal dprice = Convert.ToDecimal(t1) / 100;
                                        strSql += ", @pCOST_PRICE = '" + dprice + "' ";
                                    }
                                    else
                                    {
                                        strSql += ", @pCOST_PRICE = '" + fpSpread1.Sheets[0].Cells[i, item_start_idx + k].Value.ToString().Replace(",", "").Replace("%", "") + "' ";
                                    }
                                }

                                if (fpSpread1.Sheets[0].Cells[i, rate].Text.Trim() == "")
                                    strSql += ", @pCOMMON_RATE = 0 ";
                                else
                                {
                                    bool t3 = fpSpread1.Sheets[0].Cells[i, rate].Value.ToString().Contains("%");
                                    if (t3 == true)
                                    {
                                        string t4 = fpSpread1.Sheets[0].Cells[i, rate].Value.ToString().Replace("%", "");

                                        decimal drate = Convert.ToDecimal(t4) / 100;
                                        strSql += ", @pCOMMON_RATE = '" + drate + "' ";
                                    }
                                    else
                                    {
                                        strSql += ", @pCOMMON_RATE = '" + fpSpread1.Sheets[0].Cells[i, rate].Value.ToString().Replace(",", "").Replace("%", "") + "' ";
                                    }
                                }
                                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                                strSql += ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "'";

                                DataSet ds1 = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds1.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds1.Tables[0].Rows[0][1].ToString();

                                TempCostClass = strCostClass;

                                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; } 	// ER 코드 Return시 점프 
                                if (strCostClass == "예가율(%)" && k == (item_su - 1)) break;
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
            this.Cursor = Cursors.Default;

        }
        #endregion


        #region 다운로드
        private void btnFileDownload_Click(object sender, EventArgs e)
        {
            string updndl = "";

            if (SystemBase.Base.gstrUserID == "ADMIN") updndl = "Y#Y#Y";
            else updndl = "N#Y#N";

            UIForm.FileUpDown form1 = new UIForm.FileUpDown(this.Name, updndl);
            form1.ShowDialog();

        }
        #endregion

        private void CSA001_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboContSeq.Focus();
        }

        private void CSA001_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

    }
}
