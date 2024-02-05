
#region 작성정보
/*********************************************************************/
// 단위업무명 : 잔업특근신청서
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 잔업특근신청서 및 관리
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

namespace HA.HAA003
{
    public partial class HAA003 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strEmpNo = "";
        #endregion

        #region 생성자
        public HAA003()
        {
            InitializeComponent();
            strEmpNo = SystemBase.Base.gstrUserID.Replace("FST", "").ToString();
        }
        #endregion

        #region 로그인 사용자 체크 후 필수유무
        private void UsrCheck()
        {
            EmpDataInput(strEmpNo);
        }
        #endregion

        #region 사원데이터 자동기입
        private void EmpDataInput(string EmpNo)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + EmpNo + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtDeptCd.Value = dt.Rows[0][6].ToString();
                txtDeptNm.Value = dt.Rows[0][2].ToString();
                txtInternalCd.Value = dt.Rows[0][7].ToString();
                txtDeptCd.Focus();
            }
            else
            {
                txtDeptCd.Value = "";
                txtDeptNm.Value = "";
                txtInternalCd.Value = "";
                txtDeptCd.Focus();
            }
        }
        #endregion

        #region Form Load 시
        private void HAA003_Load(object sender, System.EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            //사용자체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "근태구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H997', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //기타 세팅
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");

            //사용자체크
            UsrCheck();

            //콤보박스 세팅
            G1Etc[SystemBase.Base.GridHeadIndex(GHIdx1, "근태구분")] = SystemBase.ComboMake.ComboOnGrid("usp_H_COMMON @pTYPE = 'H007', @pCOM_CD = 'H997', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//구분

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                try
                {
                    string strQuery = " usp_HAA003  @pTYPE = 'S1', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                    strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
        }
        #endregion

        #region Save
        protected override void SaveExec()
        {
            if (UIForm.FPMake.FPUpCheck(fpSpread1) == true)// 그리드 상단 필수항목 체크
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
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string strSDt = null, strEDt = null;

                            string strSql = " usp_HAA003 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pDATE = '" + dtpDate.Text + "' ";
                            strSql = strSql + ", @pOVER_WORK_LIST = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "잔업내용")].Text + "' ";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 시작시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strSDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "시작시간")].Text;

                                strSql = strSql + ", @pSTR_DT  = '" + strSDt + "'";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 종료시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strEDt = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "종료시간")].Text;

                                strSql = strSql + ", @pEND_DT  = '" + strEDt + "'";
                            }

                            strSql = strSql + ", @pOVER_WORK_TYPE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태구분")].Value + "'";
                            strSql = strSql + ", @pREMARK = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "비고")].Text + "'";
                            strSql = strSql + ", @pEMP_NO = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                            strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";

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
        //부서코드
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = "";

                if (HumanRoll(SystemBase.Base.gstrUserID.ToString()) == "Y")
                {
                    strQuery = " usp_H_COMMON @pType='H014', @pDATE = '" + dtpDate.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                }
                else
                {
                    strQuery = " usp_H_COMMON @pType='H001', @pDATE = '" + dtpDate.Text + "', @pSPEC1 = '" + txtInternalCd.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                }
                string[] strWhere = new string[] { "@pCOM_CD", "@pCOM_NM" };
                string[] strSearch = new string[] { txtDeptCd.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("H00001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "부서 조회");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtDeptCd.Text = Msgs[0].ToString();
                    txtDeptNm.Value = Msgs[1].ToString();
                    txtInternalCd.Value = Msgs[2].ToString();

                    //버튼 권한
                    //					ButtonRoll();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        //부서코드
        private void txtDeptCd_TextChanged(object sender, EventArgs e)
        {

            string strQuery = "usp_H_COMMON @pType='H002', @pDATE = '" + dtpDate.Text + "', @pCOM_CD = '" + txtDeptCd.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtDeptNm.Value = dt.Rows[0][1].ToString();
                txtInternalCd.Value = dt.Rows[0][2].ToString();
            }
            else
            {
                txtDeptNm.Value = "";
                txtInternalCd.Value = "";
            }
        }
        #endregion

        #region 마우스 커서
        private void button1_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void button1_MouseEnter(object sender, System.EventArgs e)
        {
            if (button1.Enabled == true)
            {
                this.Cursor = Cursors.Hand;
            }
        }
        #endregion

        #region 잔업특근신청서 버튼클릭
        private void button1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //조회 필수 체크
            if (fpSpread1.Sheets[0].Rows.Count > 0)
            {		
                string RptName = "";    // 레포트경로+레포트명
                string[] RptParmValue = new string[11];   // SP 파라메타 값
                string[] FormulaFieldName = new string[1]; //formula 값
                string[] FormulaFieldValue = new string[1]; //formula 이름

                RptName = SystemBase.Base.ProgramWhere + @"\Report\HAA003.rpt";
                RptParmValue[0] = "P1";
                RptParmValue[1] = dtpDate.Text;
                RptParmValue[2] = "";
                RptParmValue[3] = "";
                RptParmValue[4] = "";
                RptParmValue[5] = "";
                RptParmValue[6] = "";
                RptParmValue[7] = "";
                RptParmValue[8] = "";
                RptParmValue[9] = txtInternalCd.Text;
                RptParmValue[10] = SystemBase.Base.gstrCOMCD;

                FormulaFieldValue[0] = "";
                FormulaFieldName[0] = "";

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + "출력", FormulaFieldValue, FormulaFieldName, RptName, RptParmValue); //공통크리스탈 10버전	
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("출력 대상을 찾을 수 없습니다. 조회 후 출력하세요", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 인사관리 권한
        private static string HumanRoll(string strEmpNo)
        {
            string strRoll = "N";

            string Query = "usp_H_COMMON 'H016', @pCOM_CD = '" + strEmpNo + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(Query);

            if (dt.Rows.Count > 0)
            {
                strRoll = dt.Rows[0][0].ToString();
            }

            return strRoll;
        }
        #endregion
    }
}
