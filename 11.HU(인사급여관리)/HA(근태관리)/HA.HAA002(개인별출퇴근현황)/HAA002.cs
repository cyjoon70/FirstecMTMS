
#region 작성정보
/*********************************************************************/
// 단위업무명 : 개인별출퇴근현황
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 개인별출퇴근현황 및 관리
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

namespace HA.HAA002
{
    public partial class HAA002 : UIForm.FPCOMM1
    {
        #region 생성자
        public HAA002()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA002_Load(object sender, System.EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD");

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 8, false);
            //기타 셋팅
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD");
            dtpDateTo.Text = SystemBase.Base.ServerTime("YYMMDD");
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
                    string strQuery = " usp_HAA002  @pTYPE = 'S1', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                    strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                    strQuery = strQuery + " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
                    strQuery = strQuery + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 8);

                    //Merge
                    fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    fpSpread1.Sheets[0].SetColumnMerge(5, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    fpSpread1.Sheets[0].SetColumnMerge(6, FarPoint.Win.Spread.Model.MergePolicy.Restricted);

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

        #region 조회조건 팝업
        //부서코드
        private void btnDept_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H014', @pDATE = '" + dtpDate.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
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
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "부서 팝업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //사원번호
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
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

        //사원번호
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + txtEmpNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
            }
            else
            {
                txtEmpNm.Value = "";
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
                                case "I": strGbn = "I1"; break;
                                case "D": strGbn = "D1"; break;
                                default: strGbn = ""; break;
                            }

                            string strSHour = null, strSMin = null, strEHour = null, strEMin = null;

                            string strSql = " usp_HAA002 @pTYPE = '" + strGbn + "' ";
                            strSql = strSql + ", @pDATE = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "근태일자")].Text + "' ";
                            strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출근시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출근시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 출근시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strSHour = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출근시간")].Text.Substring(0, 2);
                                strSMin = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출근시간")].Text.Substring(3, 2);

                                strSql = strSql + ", @pSTRT_DATE  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "출근일자")].Text + "'";
                                strSql = strSql + ", @pSTRT_HOUR  = '" + strSHour + "'";
                                strSql = strSql + ", @pSTRT_MIN  = '" + strSMin + "'";
                            }

                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "퇴근시간")].Text.Replace("_", "").Replace(":", "") != "")
                            {
                                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "퇴근시간")].Text.Replace("_", "").Replace(":", "").Length != 4)
                                {
                                    Trans.Rollback();
                                    ERRCode = "WR";
                                    i = i + 1;
                                    MSGCode = i.ToString() + " 열의 퇴근시간 정보가 잘못되었습니다.";
                                    goto Exit;
                                }

                                strEHour = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "퇴근시간")].Text.Substring(0, 2);
                                strEMin = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "퇴근시간")].Text.Substring(3, 2);

                                strSql = strSql + ", @pEND_DATE  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "퇴근일자")].Text + "'";
                                strSql = strSql + ", @pEND_HOUR  = '" + strEHour + "'";
                                strSql = strSql + ", @pEND_MIN  = '" + strEMin + "'";
                            }

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

        #region Save_Create
        protected void Save_Create()
        {

            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                string strSql = " usp_HAA002 @pTYPE = 'C1' ";
                strSql = strSql + ", @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
                strSql = strSql + " , @pDATE = '" + dtpDate.Text + "' ";
                strSql = strSql + " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
                strSql = strSql + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
                strSql = strSql + " , @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "'";
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
        #endregion

        #region 작업일보 출퇴근생성 버튼클릭
        private void btnCreate_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;


            //근태재실행체크
            string strQuery = " usp_HAA002  @pTYPE = 'C2', @pINTERNAL_CD = '" + txtInternalCd.Text + "' ";
            strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
            strQuery = strQuery + " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
            strQuery = strQuery + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                string msg = SystemBase.Base.MessageRtn(dtpDate.Text + "~" + dtpDateTo.Text + "에 이미 생성된 데이터가 존재합니다. 재생성 하시겠습니까?");
                DialogResult dsMsg = MessageBox.Show(msg, "작업일보 출퇴근 생성", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dsMsg == DialogResult.Yes)
                {
                    Save_Create();
                }
            }
            else
            {
                Save_Create();
            }

            this.Cursor = Cursors.Default;
        }
        #endregion


    }
}
