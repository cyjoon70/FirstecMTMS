using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace PA.PBA102
{
    public partial class PBA102P2 : Form
    {
        #region 변수선언
        #endregion

        public PBA102P2()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void PBA102P2_Load(object sender, System.EventArgs e)
        {
            cboYear.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 4);
        }
        #endregion

        #region 스케쥴 캐린더 생성
        private void btnCreate_Cal_Click(object sender, System.EventArgs e)
        {
            if (cboYear.Text.Length > 0)
            {
                if (MessageBox.Show(cboYear.Text + "년 기준달력를 새로 생성하시겠습니까?", "기준달력 생성", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Thread th = new Thread(new ThreadStart(Create_Cal));
                    th.Start();
                }
            }
            else
                MessageBox.Show("생성년도을 선택하세요.", "기준달력 생성", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        public void Create_Cal()
        {
            this.Cursor = Cursors.WaitCursor;

            string Query = "";
            string ERRCode = "ER";
            string MSGCode = "B0021";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
            cmd.Transaction = Trans;
            cmd.CommandTimeout = 10000;

            // PROGRESS 표시
            Thread calProgTh;
            CAL_PROG calProg = new CAL_PROG();
            calProg.CalProg = progressBar1;

            // PROGRESS ID
            string ProcId = calProg.GenProcId();
            calProg.CalProcId = ProcId;

            calProgTh = new Thread(new ThreadStart(calProg.CalProcChk));
            calProgTh.Start();

            try
            {

                Query += " usp_P_CRT_SCH_CAL  @pPROC_ID = '" + ProcId + "',";
                Query += "                    @pYEAR    = '" + cboYear.Text + "',";
                Query += "                    @pUSR_ID  = '" + SystemBase.Base.gstrUserID + "'";
                Query += "                  , @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(Query, dbConn, Trans);

                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode == "ER")
                {
                    Trans.Rollback();
                    return;
                }
                Trans.Commit();

            }
            catch (Exception f)
            {
                Trans.Rollback();
                SystemBase.Loggers.Log("PBA102P2", f.ToString());

                ERRCode = "ER";
            }
            finally
            {

                if (dbConn != null)
                    dbConn.Close();

                this.Cursor = Cursors.Default;

                // PROGRESS 표시
                if (calProg != null)
                {
                    calProg.SchProcStop();
                    calProgTh.Join();
                }

                if (ERRCode == "ER")
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), "기준달력 생성", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), "기준달력 생성", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
        }
        #endregion

        #region 창닫기 버튼클릭
        private void btnCLOSE_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
