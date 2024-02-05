

#region 작성정보
/*********************************************************************/
// 단위업무명 : 예산정보등록
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-12
// 작성내용 : 예산정보등록
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

namespace AG.ACG003
{
    public partial class ACG003 : UIForm.FPCOMM1 
    {
        string strREORG_ID = "";
        public ACG003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACG003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            dtpEst_yyyy.Value = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpEst_yyyy.Value = SystemBase.Base.ServerTime("YYMMDD");
            strREORG_ID = SystemBase.Base.gstrREORG_ID;

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ACG003  'S1'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    strQuery += ", @pEST_YYYY = '" + dtpEst_yyyy.Text + "' ";
                    if (txtDeptCd.Text != "")
                    {
                        strQuery += ", @pREORG_ID = '" + strREORG_ID + "' ";
                        strQuery += ", @pDEPT_CD = '" + txtDeptCd.Text + "' ";
                    }
                    
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드_2")].Locked = true;
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서_2")].Locked = true;
                    }
                    
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            this.Cursor = Cursors.WaitCursor;

            if ((SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", true) == true))// 그리드 필수항목 체크 
            {
                string ERRCode = "ER", MSGCode = "SY001";	//처리할 내용이 없습니다.
                string strKey = "";

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        string strHead = fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text;
                        string strGbn = "";

                        if (strHead.Length > 0)
                        {
                            switch (strHead)
                            {
                                case "I": strGbn = "I1"; break;
                                case "U": strGbn = "U1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            strKey = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드")].Text + "_" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text + "_" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text;

                            string strTempSql = " usp_ACG003 '" + strGbn + "'";
                            strTempSql = strTempSql + ", @pCO_CD  = '" + SystemBase.Base.gstrCOMCD + "'";
                            strTempSql = strTempSql + ", @pEST_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드")].Text + "' ";
                            strTempSql = strTempSql + ", @pREORG_ID = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text + "' ";
                            strTempSql = strTempSql + ", @pDEPT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text + "' ";

                            for (int j = 1; j <= 12; j++)
                            {
                                int col = (j * 4 - 2) + SystemBase.Base.GridHeadIndex(GHIdx1, "합계");

                                double dPre_Amt = 0;
                                double dThis_Amt = 0;

                                if(fpSpread1.Sheets[0].Cells[i, col - 1].Text != "") dPre_Amt = Convert.ToDouble( fpSpread1.Sheets[0].Cells[i, col - 1].Text.Replace(",", ""));
                                if(fpSpread1.Sheets[0].Cells[i, col].Text != "") dThis_Amt = Convert.ToDouble( fpSpread1.Sheets[0].Cells[i, col].Text.Replace(",", ""));

                                if (dPre_Amt != dThis_Amt || strHead == "D")
                                {
                                    double dAmt = 0;
                                    double dSlipAmt = 0;
                                    if(fpSpread1.Sheets[0].Cells[i, col].Text != "") dAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, col].Text.Replace(",", ""));
                                    if(fpSpread1.Sheets[0].Cells[i, col+1].Text != "") dSlipAmt = Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, col+1].Text.Replace(",", ""));
                                    if (dSlipAmt > dAmt)
                                    {
                                        ERRCode = "ER";
                                        MSGCode = "계획금액이 결의금액보다 작을 수 없습니다.";
                                        fpSpread1.Sheets[0].SetActiveCell(i, col);
                                        Trans.Rollback(); goto Exit;
                                    }
                                    string Month = "0" + (j).ToString();
                                    if (Month.Length > 2)
                                    {
                                        Month = Month.Substring(1, 2);
                                    }

                                    string strEST_YYMM = dtpEst_yyyy.Text + Month;


                                    string strSql = strTempSql;
                                    strSql = strSql + ", @pEST_YYMM = '" + strEST_YYMM + "' ";
                                    if (fpSpread1.Sheets[0].Cells[i, col].Text != "" && fpSpread1.Sheets[0].Cells[i, col].Text != "0")
                                    {
                                        strSql = strSql + ", @pEST_AMT = '" + fpSpread1.Sheets[0].Cells[i, col].Text.Replace(",", "") + "' ";
                                    }

                                    strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                                    strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                                }
                            }
                        }
                    }
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    MSGCode = "SY002";	//에러가 발생하여 데이터 처리가 취소되었습니다.
                }
            Exit:
                dbConn.Close();

                if (ERRCode == "OK")
                {   
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKey, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY"));
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

        #region 텍스트 체인지
        //부서
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtDeptNm.Value = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT_INFO", txtDeptCd.Text, " AND REORG_ID = '" + strREORG_ID + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString(), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 팝업 클릭
        //부서
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW.WNDW011 pu = new WNDW.WNDW011(SystemBase.Base.ServerTime("YYMMDD"));
                pu.MaximizeBox = false;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtDeptCd.Value = Msgs[1].ToString();
                    strREORG_ID = Msgs[5].ToString();
                    txtDeptCd.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드_2"))
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A040', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { "", "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00114", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "예산코드 조회");
                    pu.Width = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "예산명")].Text = Msgs[1].ToString();
                    }
                }
                else if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "부서_2"))
                {
                    //WNDW.WNDW011 pu = new WNDW.WNDW011(SystemBase.Base.ServerTime("YYMMDD"));
                    WNDW.WNDW011 pu = new WNDW.WNDW011();
                    pu.MaximizeBox = false;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        string[] Msgs = pu.ReturnVal;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text = Msgs[5].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text = Msgs[1].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = Msgs[2].ToString();
                        txtDeptCd.Focus();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 그리드 change
        protected override void fpSpread1_ChangeEvent(int Row, int Col)
        {
            try
            {
                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "예산명")].Text = SystemBase.Base.CodeName("EST_CD", "EST_NM", "A_ESTIMATE_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "예산코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "부서"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서명")].Text = SystemBase.Base.CodeName("DEPT_CD", "DEPT_NM", "B_DEPT", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "부서")].Text, " AND REORG_ID = '" + fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion


        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "개편ID")].Text = SystemBase.Base.gstrREORG_ID;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region RCopyExec 그리드 Row 복사
        protected override void RCopyExec()
        {
            try
            {
                UIForm.FPMake.RowCopy(fpSpread1);
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "합계")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "1월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "2월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "3월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "4월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "5월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "6월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "7월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "8월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "9월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "10월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "11월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "12월")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "1월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "2월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "3월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "4월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "5월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "6월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "7월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "8월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "9월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "10월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "11월_3")].Text = "";
                fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].ActiveRow.Index, SystemBase.Base.GridHeadIndex(GHIdx1, "12월_3")].Text = "";

                
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행복사"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        

        
    }
}
