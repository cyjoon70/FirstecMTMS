#region 작성정보
/*********************************************************************/
// 단위업무명 : 일근태확정
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-24
// 작성내용 : 일근태확정 및 관리
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

namespace HA.HAA007
{
    public partial class HAA007 : UIForm.FPCOMM1
    {
        #region 생성자
        public HAA007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void HAA007_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            dtpDate.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0,10);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion
        
        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
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

                string strQuery = " usp_HAA007  @pTYPE = 'S1'";
                strQuery = strQuery + " , @pDATE = '" + dtpDate.Text + "' ";
                strQuery = strQuery + " , @pPROC_YN = '" + strProcYn + "' ";
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
        #endregion

        #region Save
        private void Save()
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    string strApply = "N";
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "반영여부")].Text == "True")
                    {
                        strApply = "Y";
                    }

                    string strSql = " usp_H_DAY_DILIG_APPLY";
                    strSql = strSql + " @pDILIG_DT  = '" + dtpDate.Text.Replace("-", "") + "'";
                    strSql = strSql + ", @pEMP_NO  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "사원번호")].Text + "'";
                    strSql = strSql + ", @pDILIG_SEQ  = '" + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "순번")].Text + "'";
                    strSql = strSql + ", @pAPP_YN  = '" + strApply + "'";
                    strSql = strSql + ", @pERR_MSG = '' ";
                    strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
                    strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                    ERRCode = ds.Tables[0].Rows[0][0].ToString();
                    MSGCode = ds.Tables[0].Rows[0][1].ToString();

                    if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
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

        #region 마우스 커서
        private void button2_MouseEnter(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void button2_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 근태반영
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            Save();

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
