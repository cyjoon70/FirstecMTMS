#region 작성정보
/*********************************************************************/
// 단위업무명 :기초자료등록(주택자금)
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-30
// 작성내용 : 기초자료등록(주택자금)
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
namespace HC.HCA016
{
    public partial class HCA016 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEmpNo = "";
        #endregion

        #region 생성자
        public HCA016()
        {
            InitializeComponent();

            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }
        #endregion

        #region 로그인 사용자 체크 후 필수유무
        private void UsrCheck()
        {
            //로그인 유저가 'ADMIN' 이거나 'INSA' 인 경우 수정가능하게 한다.
            if (strEmpNo == "ADMIN" || strEmpNo == "INSA")
            {
                txtEmpNo.Tag = ";1;;";
               // btnEmpNo.Tag = "";
                btnEmpNo.Enabled = true;
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }
            else
            {
                txtEmpNo.Tag = ";2;;";
                //btnEmpNo.Tag = "2";
                btnEmpNo.Enabled = false;
                SystemBase.Validation.GroupBox_Setting(groupBox1);
            }

            txtEmpNo.Text = strEmpNo;

            EmpDataInput(strEmpNo);
        }
        #endregion

        #region 사원데이터 자동기입
        private void EmpDataInput(string EmpNo)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + EmpNo + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtDeptNm.Value = dt.Rows[0][2].ToString();
                txtRollPstn.Value = dt.Rows[0][3].ToString();
                txtEntrDt.Value = dt.Rows[0][4].ToString();
                txtPayGrd1.Value = dt.Rows[0][5].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtDeptNm.Value = "";
                txtRollPstn.Value = "";
                txtEntrDt.Value = "";
                txtPayGrd1.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region Form Load 시
        private void HCA016_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();

            //사용자 체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0340', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//주택자금소득
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0341', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//대출자구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0310' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//금융기관
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0342' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//상환유형
			
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            dtpYear.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).Year.ToString();

            //사용자 체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0340', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//주택자금소득
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0341', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//대출자구분
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0310' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//금융기관
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H0342' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//상환유형
		
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            try
            { 
                //조회조건 필수 체크
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
                {
                    string strQuery = " usp_HCA016  @pTYPE = 'S1', @pYY = '" + dtpYear.Text + "', @pEMP_NO = '" + txtEmpNo.Text + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
  
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    if (fpSpread1.Sheets[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                           UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호") + "|3");

                           if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                           {
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업자번호") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출일(최초차입일자)") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환예정일") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "저당권설정일") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환액계(연간합계액)") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "소득공제대상액") + "|3");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형") + "|3");
                           }
                           else
                           {
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업자번호") + "|0");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출일(최초차입일자)") + "|1");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환예정일") + "|0");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "저당권설정일") + "|0");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환액계(연간합계액)") + "|1");
                               UIForm.FPMake.grdReMake(fpSpread1, i, SystemBase.Base.GridHeadIndex(GHIdx1, "소득공제대상액") + "|1");

                               if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")].Value.ToString() == "J02")
                               {
                                   UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형") + "|1"
                                       );
                               }
                               else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")].Value.ToString() == "J01")
                               {
                                   UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형") + "|3"
                                       );
                               }


                               if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "1")
                               {
                                   UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|1"
                                       );
                               }
                               else if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "2")
                               {
                                   UIForm.FPMake.grdReMake(fpSpread1, i,
                                       SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|3"
                                       );
                               }
                           }
                        }
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
            }
           
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }       
        #endregion

        #region Save
        protected override void SaveExec()
        {
            if (SystemBase.Validation.FPGrid_SaveCheck(fpSpread1, this.Name, "fpSpread1", false) == true)
            {
                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
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

                            string strYear_flag = "N";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                            {
                                strYear_flag = "Y";
                            }

                            if (strYear_flag == "Y")
                            {
                                int Row = i + 1;

                                ERRCode = "WR";
                                MSGCode = "반영여부가 'Y'인 " + Row.ToString() + "행은 수정 또는 삭제할 수 없습니다.";
                                Trans.Rollback(); goto Exit;
                            }

                            string strSql = " usp_HCA016 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pYY = '" + dtpYear.Text + "'";
                            strSql = strSql + ", @pEMP_NO = '" + txtEmpNo.Text + "'";
                            strSql = strSql + ", @pHOUSING_FUND_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")].Value + "'";
                            strSql = strSql + ", @pLENDER_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value + "'";
                            strSql = strSql + ", @pFUND_BANK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Value + "'";
                            strSql = strSql + ", @pFUND_BANK_NM = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text + "'";
                            strSql = strSql + ", @pFUND_RGST_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사업자번호")].Text + "'";
                            strSql = strSql + ", @pFUND_ACCNT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "계좌번호")].Text + "'";
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출일(최초차입일자)")].Text != "")
                            {
                                strSql = strSql + ", @pSTART_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "대출일(최초차입일자)")].Text + "'";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환예정일")].Text != "")
                            {
                                strSql = strSql + ", @pEND_DT	 = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환예정일")].Text + "'";
                            }
                            strSql = strSql + ", @pREPAY_YEARS = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환기간")].Value + "'";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "저당권설정일")].Text != "")
                            {
                                strSql = strSql + ", @pMORT_SETUP_DT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "저당권설정일")].Text + "'";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환액계(연간합계액)")].Text == "")
                            {
                                strSql = strSql + ", @pREFUND_AMT = 0 ";
                            }
                            else
                            {
                                strSql = strSql + ", @pREFUND_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환액계(연간합계액)")].Value + "'";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소득공제대상액")].Text == "")
                            {
                                strSql = strSql + ", @pDEDUCTION_AMT = 0 ";
                            }
                            else
                            {
                                strSql = strSql + ", @pDEDUCTION_AMT = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "소득공제대상액")].Value + "'";
                            }

                            strSql = strSql + ", @pREFUND_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형")].Value + "'";


                            strSql = strSql + ", @pYEAR_FLAG = '" + strYear_flag + "'";
                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                            strSql = strSql + ", @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";

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
                    MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
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
        #endregion

        #region 조회조건 팝업
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtEmpNo.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00002", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "사원 조회");
                pu.Width = 700;
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEmpNo.Value = Msgs[0].ToString();
                    txtEmpNm.Value = Msgs[1].ToString();
                    txtDeptNm.Value = Msgs[2].ToString();
                    txtRollPstn.Value = Msgs[3].ToString();
                    txtEntrDt.Value = Msgs[4].ToString();
                    txtPayGrd1.Value = Msgs[5].ToString();
                    txtEmpNo.Focus();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "사원 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 조회조건 TextChanged  
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            EmpDataInput(txtEmpNo.Text);
        }
        #endregion

        #region 그리드 상 Change
        protected override void fpSpread1_ChangeEvent(int Row, int Column)
        {
            //주택자금소득
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")].Value.ToString() == "J01")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형")].Text = "";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text = "";

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분") + "|1"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|3"
                        );
                }
                else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "주택자금소득")].Value.ToString() == "J02")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value = "1";
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형")].Text = "";

                    if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "1")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text = "";

                        UIForm.FPMake.grdReMake(fpSpread1, Row,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|1"
                            );
                    }
                    else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "2")
                    {
                        fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text = "";

                        UIForm.FPMake.grdReMake(fpSpread1, Row,
                            SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|3"
                            );
                    }

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분") + "|3"
                        + "#" + SystemBase.Base.GridHeadIndex(GHIdx1, "상환유형") + "|1"
                        );
                }
            }

            //대출자구분
            if (Column == SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분"))
            {
                if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "1")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text = "";

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|1"
                        );
                }
                else if (fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "대출자구분")].Value.ToString() == "2")
                {
                    fpSpread1.Sheets[0].Cells[Row, SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관")].Text = "";

                    UIForm.FPMake.grdReMake(fpSpread1, Row,
                        SystemBase.Base.GridHeadIndex(GHIdx1, "금융기관") + "|3"
                        );
                }
            }

        }
        #endregion

    }
}
