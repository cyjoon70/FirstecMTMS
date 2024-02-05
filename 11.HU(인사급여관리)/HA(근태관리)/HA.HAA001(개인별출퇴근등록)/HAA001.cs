#region 작성정보
/*********************************************************************/
// 단위업무명 : 개인별출퇴근등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-04-09
// 작성내용 : 개인별출퇴근등록
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

namespace HA.HAA001
{
    public partial class HAA001 : UIForm.FPCOMM1
    {
        #region 생성자
        public HAA001()
        {
            InitializeComponent();

        }
        #endregion

        #region Form Load 시
        private void HAA001_Load(object sender, System.EventArgs e)
        {
            //GroupBox1 초기화
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            
            //기타 세팅	
            lblTime.Text = SystemBase.Base.ServerTime("YYMMDD") + " " + SystemBase.Base.ServerTime("TM");
            dtpYM.Text = SystemBase.Base.ServerTime("Y") + "-" + SystemBase.Base.ServerTime("M");

            //버튼 상태
            BtnStatus();

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            SearchExec();

        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
			UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);

            dtpYM.Text = SystemBase.Base.ServerTime("Y") + "-" + SystemBase.Base.ServerTime("M");

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor =Cursors.WaitCursor;

            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1) == true)
            {
                try
                {
                    string strQuery = " usp_HAA001  @pTYPE = 'S1'";
                    strQuery = strQuery + " , @pYM_DT = '" + dtpYM.Text + "' ";
                    strQuery = strQuery + " , @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0, true);

                    BtnStatus();
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 타이머
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = SystemBase.Base.ServerTime("YYMMDD") + " " + SystemBase.Base.ServerTime("TM");
        }
        #endregion

        #region 출근
        private void button1_Click(object sender, EventArgs e)
        {
            Save("I1");

            SearchExec();
        }
        #endregion

        #region 퇴근
        private void button2_Click(object sender, EventArgs e)
        {
            Save("U1");

            SearchExec();
        }
        #endregion

        #region Save
        private void Save(string strType)
        {
            string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_HAA001 @pTYPE = '" + strType + "' ";
                strSql = strSql + ", @pATTEND_DT  = '" + lblTime.Text.Substring(0,10) + "'";
                strSql = strSql + ", @pDATE  = '" + lblTime.Text.Substring(0, 10) + "'";
                strSql = strSql + ", @pHOUR  = '" + lblTime.Text.Substring(11, 2) + "'";
                strSql = strSql + ", @pMIN  = '" + lblTime.Text.Substring(14, 2) + "'";
                strSql = strSql + ", @pUP_ID  = '" + SystemBase.Base.gstrUserID.ToString() + "'";
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
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //			}

        }
        #endregion

        #region 근태에 따른 버튼 상태
        private void BtnStatus()
        {
            string strQuery = " usp_HAA001  @pTYPE = 'S2', @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "', @pDATE = '" + lblTime.Text.Substring(0, 10) + "' ";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (dt.Rows[0][0].ToString() == "1")
            {
                button1.Enabled = false;
                button2.Enabled = true;
                //button1.BackColor = Color.Gainsboro;
                //button2.BackColor = Color.Gray;
            }
            else if (dt.Rows[0][0].ToString() == "2")
            {
                button1.Enabled = false;
                button2.Enabled = false;
                //button1.BackColor = Color.Gainsboro;
                //button2.BackColor = Color.Gainsboro;
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = false;
                //button1.BackColor = Color.Gray;
                //button2.BackColor = Color.Gainsboro;
            }
        }
        #endregion

        #region 마우스 커서
        private void button1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (button1.Enabled == true)
            {
                this.Cursor = Cursors.Hand;
            }
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            if (button2.Enabled == true)
            {
                this.Cursor = Cursors.Hand;
            }
        }
        #endregion

        #region dtpYM_TextChanged
        private void dtpYM_TextChanged(object sender, EventArgs e)
        {
            lblTime.Text = SystemBase.Base.ServerTime("YYMMDD") + " " + SystemBase.Base.ServerTime("TM");

            SearchExec();
        }
        #endregion

        #region HAA001_Activated
        private void HAA001_Activated(object sender, EventArgs e)
        {
            lblTime.Text = SystemBase.Base.ServerTime("YYMMDD") + " " + SystemBase.Base.ServerTime("TM");

            SearchExec();
        }
        #endregion
    }
}
