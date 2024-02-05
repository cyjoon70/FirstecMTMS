

#region 작성정보
/*********************************************************************/
// 단위업무명 : 통합결산수정분개
// 작 성 자 : 유 재 규
// 작 성 일 : 2013-03-11
// 작성내용 : 통합결산수정분개
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

namespace AZ.ACZ003
{
    public partial class ACZ003 : UIForm.FPCOMM1 
    {
        public ACZ003()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void ACZ003_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);//필수 적용
            SystemBase.ComboMake.C1Combo(cboCoCd, "usp_B_COMMON @pTYPE ='CO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9);      //법인
            dtpSlipYYMM_Fr.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpSlipYYMM_To.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";

            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "법인")] = SystemBase.ComboMake.ComboOnGrid("usp_B_COMMON @pTYPE ='CO' , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //법인

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            SearchExec();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpSlipYYMM_Fr.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01";
            dtpSlipYYMM_To.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12";
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string strQuery = " usp_ACZ003  'S1'";
                strQuery += ", @pSLIP_F_YYMM = '" + dtpSlipYYMM_Fr.Text.Replace("-","") + "' ";
                strQuery += ", @pSLIP_T_YYMM = '" + dtpSlipYYMM_To.Text.Replace("-", "") + "' ";
                strQuery += ", @pCO_CD = '" + cboCoCd.SelectedValue.ToString() + "' ";
              
                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                UIForm.FPMake.grdReMake(fpSpread1, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드_2").ToString() + "|3");
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
                string strKeyCd = "";

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
                                case "U": strGbn = "U1"; break;
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            strKeyCd = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계년도")].Text + "_" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text;

                            string strSql = " usp_ACZ003 '" + strGbn + "'";
                            strSql = strSql + ", @pSLIP_YYMM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계일자")].Text.Substring(0, 7).Replace("-","") + "' ";
                            //strSql = strSql + ", @pSLIP_DD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "회계일자")].Text.Substring(8, 2) + "' "; 
                            strSql = strSql + ", @pCO_CD  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "법인")].Value + "' ";
                            strSql = strSql + ", @pACCT_CD = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text + "' "; 
                            if(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차변조정금액")].Text != "")
                                strSql = strSql + ", @pDR_AMT_RE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "차변조정금액")].Text.Replace(",", "") + "' ";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대변조정금액")].Text != "")
                                strSql = strSql + ", @pCR_AMT_RE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대변조정금액")].Text.Replace(",", "") + "' ";
                            strSql = strSql + ", @pUP_EMP_NO = '" + SystemBase.Base.gstrUserID + "'";
                            strSql = strSql + ", @pUP_IP = '" + SystemBase.Base.gstrUserIp + "'";

                            DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                            ERRCode = ds.Tables[0].Rows[0][0].ToString();
                            MSGCode = ds.Tables[0].Rows[0][1].ToString();

                            if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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
                    SearchExec();
                    UIForm.FPMake.GridSetFocus(fpSpread1, strKeyCd, SystemBase.Base.GridHeadIndex(GHIdx1, "KEY"));
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



        #region fpButtonClick() 그리드 버튼클릭
        protected override void fpButtonClick(int Row, int Column)
        {
            try
            {
                if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드_2"))
                {
                    string strQuery = " usp_A_COMMON @pTYPE = 'A030', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' , @pSPEC1 = 'Y' ";
                    string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                    string[] strSearch = new string[] { "", "" };
                    UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00110", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "계정코드 조회");
                    pu.Width = 800;
                    pu.ShowDialog();
                    if (pu.DialogResult == DialogResult.OK)
                    {
                        Regex rx1 = new Regex("#");
                        string[] Msgs = rx1.Split(pu.ReturnVal.ToString());
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text = Msgs[0].ToString();
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = Msgs[1].ToString();
                        
                        string strDR = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text;

                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Value = Msgs[2].ToString();

                        if (strDR != fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text)
                        {
                            AMT_RE_CAL(Row);
                        }
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
                if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드"))
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정명")].Text = SystemBase.Base.CodeName("ACCT_CD", "ACCT_NM", "A_ACCT_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    string strDR = fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text;
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text = SystemBase.Base.CodeName("ACCT_CD", "DR_CR", "A_ACCT_CODE", fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "계정코드")].Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                    if(strDR != fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text)
                    {
                        AMT_RE_CAL(Row);
                    }
                }
                else if (Col == SystemBase.Base.GridHeadIndex(GHIdx1, "차변조정금액") || Col == SystemBase.Base.GridHeadIndex(GHIdx1, "대변조정금액"))
                {
                    AMT_RE_CAL(Row);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        public void AMT_RE_CAL(int Row)
        {
            try
            {
                double dDR_AMT_OR = 0;  //차변원본금액
                double dDR_AMT_RE = 0;  //차변조정금액
                double dCR_AMT_OR = 0;  //대변원본금액
                double dCR_AMT_RE = 0;  //대변원본금액

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변원본금액")].Text != "")
                    dDR_AMT_OR = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변원본금액")].Text.Replace(",", ""));
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변조정금액")].Text != "")
                    dDR_AMT_RE = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변조정금액")].Text.Replace(",", ""));


                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변원본금액")].Text != "")
                    dCR_AMT_OR = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변원본금액")].Text.Replace(",", ""));
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변조정금액")].Text != "")
                    dCR_AMT_RE = Convert.ToDouble(fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변조정금액")].Text.Replace(",", ""));

                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차변금액")].Value = dDR_AMT_OR + dDR_AMT_RE;
                fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대변금액")].Value = dCR_AMT_OR + dCR_AMT_RE;

                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차대구분")].Text == "DR")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "조정잔액")].Value = (dDR_AMT_OR + dDR_AMT_RE) - (dCR_AMT_OR + dCR_AMT_RE);
                }
                else
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "조정잔액")].Value = (dCR_AMT_OR + dCR_AMT_RE) - (dDR_AMT_OR + dDR_AMT_RE);
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn(f.ToString()), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        #region RowInsExec 행 추가
        protected override void RowInsExec()
        {	// 행 추가
            try
            {
                UIForm.FPMake.RowInsert(fpSpread1);
                RowInsExe();
                int iRow = fpSpread1.Sheets[0].ActiveRow.Index;
                fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "회계일자")].Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 10);
                fpSpread1.Sheets[0].Cells[iRow, SystemBase.Base.GridHeadIndex(GHIdx1, "법인")].Value = SystemBase.Base.gstrCOMCD;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("SY008", "행추가"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion
        
    }
}
