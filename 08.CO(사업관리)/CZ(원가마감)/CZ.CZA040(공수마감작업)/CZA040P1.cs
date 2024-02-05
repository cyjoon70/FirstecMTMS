using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace CZ.CZA040
{
    public partial class CZA040P1 : Form
    {
        #region 변수선언
        string strWorkType = "";
        string strCloseMonthFr = "";
        string strCloseMonthTo = "";
        Thread th;
        #endregion

        public CZA040P1()
        {
            InitializeComponent();
        }

        public CZA040P1(string WorkType, string CloseMonthFr, string CloseMonthTo)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //

            strWorkType = WorkType;
            strCloseMonthFr = CloseMonthFr;
            strCloseMonthTo = CloseMonthTo;
        }

        #region Form Load시
        private void CZA040P1_Load(object sender, System.EventArgs e)
        {
            if (strWorkType == "R")
            { this.Text = "공수마감 작업중...."; }
            else
            { this.Text = "공수마감 취소중...."; }

            try
            {
                th = new Thread(new ThreadStart(SchStart));
                th.Start();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log("PSA010.SCHEDULE.Scheduld() ", f.ToString());
                SystemBase.MessageBoxComm.Show(f.ToString());
            }
        }
        #endregion

        #region 원가마감작업
        public void SchStart()
        {
            string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strSql = " usp_CZA040 'I1' ";
                strSql += ", @pWORK_TYPE = '" + strWorkType + "'";
                strSql += ", @pYYMM_FR = '" + strCloseMonthFr + "'";
                strSql += ", @pYYMM_TO = '" + strCloseMonthTo + "'";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";

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
                ERRCode = "ER";
                MSGCode = f.Message;
                //MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        #endregion

        private void CZA040P1_FormClosed(object sender, FormClosedEventArgs e)
        {
            th.Abort();
        }

    }
}
