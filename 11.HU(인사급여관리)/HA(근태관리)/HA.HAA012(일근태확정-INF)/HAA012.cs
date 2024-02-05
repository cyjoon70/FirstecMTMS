
#region 작성정보
/*********************************************************************/
// 단위업무명 : 일근태확정-INF
// 작 성 자 : 권순철
// 작 성 일 : 2013-04-04
// 작성내용 : 일근태확정-INF 및 관리
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

namespace HA.HAA012
{
    public partial class HAA012 : UIForm.FPCOMM1
    {
        #region 변수선언
        string ERRCode = "ER";
        string MSGCode = "P0000";	//처리할 내용이 없습니다.
        #endregion

        #region 생성자
        public HAA012()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void HAA012_Load(object sender, System.EventArgs e)
        {
            //필수 적용
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            dtpDateFr.Text = SystemBase.Base.ServerTime("YYMMDD");
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
            dtpDateFr.Text = SystemBase.Base.ServerTime("YYMMDD");
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
                    string strProcYn = "";
                    if (rdoYes.Checked == true)
                    {
                        strProcYn = "Y";
                    }
                    else if (rdoNo.Checked == true)
                    {
                        strProcYn = "N";
                    }

                    string strQuery = " usp_HAA012  @pTYPE = 'S1'";
                    strQuery = strQuery + " , @pDATE_FR = '" + dtpDateFr.Text + "' ";
                    strQuery = strQuery + " , @pDATE_TO = '" + dtpDateTo.Text + "' ";
                    strQuery = strQuery + " , @pEMP_NO = '" + txtEmpNo.Text + "' ";
                    strQuery = strQuery + " , @pPROC_YN = '" + strProcYn + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

                    //Merge
                    fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Always);
                    fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                    fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);

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
        //사원번호
        private void btnEmpNo_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_H_COMMON @pType='H003'";
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
        //사원번호
        private void txtEmpNo_TextChanged(object sender, EventArgs e)
        {
            string strQuery = "usp_H_COMMON @pType='H004', @pCOM_CD = '" + txtEmpNo.Text + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows.Count > 0)
            {
                txtEmpNm.Value = dt.Rows[0][1].ToString();
                txtEmpNo.Focus();
            }
            else
            {
                txtEmpNm.Value = "";
                txtEmpNo.Focus();
            }
        }
        #endregion

        #region 근태반영
        private void btnConfirm_Click(object sender, System.EventArgs e)
        {
            string TEMP_ERRCode = "";
            string TEMP_MSGCode = "";

            this.Cursor = Cursors.WaitCursor;

            Save_Oracle_SP("N");

            if (ERRCode != "ER")
            {
                Save("Y");

                if (ERRCode != "ER")
                {
                    Save_Oracle_SP("Y");
                    if (ERRCode == "ER")
                    {
                        TEMP_ERRCode = ERRCode;
                        TEMP_MSGCode = MSGCode;

                        Save("N"); //에러일때는 MTMS 자료만 삭제하고 INTERFACE 자료는 ORACLE에서 삭제처리하고 메세지만 던져준다.

                        ERRCode = TEMP_ERRCode;
                        MSGCode = TEMP_MSGCode;
                    }
                }
            }

            if (ERRCode == "OK")
            {
                MSGCode = "정상적으로 반영 되었습니다.";
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

            SearchExec();

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 근태반영취소
        private void btnUnConfirm_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            Save("N");

            if (ERRCode != "ER")
            {
                Save_Oracle_SP("N");
            }

            if (ERRCode == "OK")
            {
                MSGCode = "정상적으로 반영 취소 되었습니다.";
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

            SearchExec();

            this.Cursor = Cursors.Default;
        }
        #endregion

        #region Save
        private void Save(string APP_YN)
        {

            //ERRCode = "ER";
            //MSGCode = "P0000";	//처리할 내용이 없습니다.

            //SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            //SqlCommand cmd = dbConn.CreateCommand();
            //SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);


            try
            {

                string strSql = " usp_H_DAY_DILIG_APPLY_NEW";
                strSql = strSql + "  @pTYPE  = 'S1'";
                strSql = strSql + ", @pDILIG_DT_FR  = '" + dtpDateFr.Text.Replace("-", "") + "'";
                strSql = strSql + ", @pDILIG_DT_TO  = '" + dtpDateTo.Text.Replace("-", "") + "'";
                strSql = strSql + ", @pEMP_NO  = '" + txtEmpNo.Text + "'";
                strSql = strSql + ", @pAPP_YN  = '" + APP_YN + "'";
                strSql = strSql + ", @pERR_MSG = '' ";
                strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                //DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                //ERRCode = ds.Tables[0].Rows[0][0].ToString();
                //MSGCode	= ds.Tables[0].Rows[0][1].ToString();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        ERRCode = ds.Tables[0].Rows[j][0].ToString();
                        if (ds.Tables[0].Rows[j][1].ToString() != "" && ds.Tables[0].Rows[j][1].ToString() != "-1")
                        {
                            MSGCode = ds.Tables[0].Rows[j][1].ToString();
                        }
                        if (ERRCode != "OK") 
                        { 
                            //Trans.Rollback(); 
                            goto Exit; 
                        }	// ER 코드 Return시 점프
                    }
                }
                else
                {
                    //Trans.Rollback();
                    goto Exit;
                }

                //Trans.Commit();

                //Save_Oracle_SP("Y");


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:
            //dbConn.Close();

            if (ERRCode == "OK")
            {
                ERRCode = "OK";
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (ERRCode == "ER")
            {
                ERRCode = "ER";
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ERRCode = MSGCode;
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }	

        }
        #endregion

        #region Save_Oracle_SP
        private void Save_Oracle_SP(string APP_YN)
        {
            //ERRCode = "ER";
            //MSGCode = "P0000";	//처리할 내용이 없습니다.

            //SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            //SqlCommand cmd = dbConn.CreateCommand();
            //SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {

                string strSql = " usp_H_DAY_DILIG_APPLY_NEW";
                strSql = strSql + "  @pTYPE  = 'S2'";
                strSql = strSql + ", @pDILIG_DT_FR  = '" + dtpDateFr.Text.Replace("-", "") + "'";
                strSql = strSql + ", @pDILIG_DT_TO  = '" + dtpDateTo.Text.Replace("-", "") + "'";
                strSql = strSql + ", @pEMP_NO  = '" + txtEmpNo.Text + "'";
                strSql = strSql + ", @pAPP_YN  = '" + APP_YN + "'";
                strSql = strSql + ", @pERR_MSG = '' ";
                strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                //DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);
                //ERRCode = ds.Tables[0].Rows[0][0].ToString();
                //MSGCode	= ds.Tables[0].Rows[0][1].ToString();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        ERRCode = ds.Tables[0].Rows[j][0].ToString();
                        if (ds.Tables[0].Rows[j][1].ToString() != "" && ds.Tables[0].Rows[j][1].ToString() != "-1")
                        {
                            MSGCode = ds.Tables[0].Rows[j][1].ToString();
                        }
                        if (ERRCode != "OK") 
                        { 
                            //Trans.Rollback(); 
                            goto Exit; 
                        }	// ER 코드 Return시 점프
                    }
                }
                else
                {
                    //Trans.Rollback();
                    goto Exit;
                }

                //Trans.Commit();


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                //Trans.Rollback();
                MSGCode = "P0001";	//에러가 발생하여 데이터 처리가 취소되었습니다.
            }
        Exit:

            //dbConn.Close();

            if (ERRCode == "OK")
            {
                ERRCode = "OK";
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (ERRCode == "ER")
            {
                ERRCode = "ER";
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ERRCode = MSGCode;
                //MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        #endregion

        #region 마우스 커서
        private void btnConfirm_MouseEnter(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btnConfirm_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void btnUnConfirm_MouseEnter(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void btnUnConfirm_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
        #endregion


    }
}
