#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP근거조회 및 확정
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-11
// 작성내용 : MRP근거조회 및 확정 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;

namespace PB.PSA023
{
    public partial class PSA023P1 : Form
    {
        Thread th;
        string LANGCD = "";
        string COMCD = "";
        string BIZCD = "";
        string PLANT_CD = "";
        string REORG_ID = "";
        string DEPT = "";
        string USERID = "";
        string MRP_ID = "";

        #region 생성자
        public PSA023P1(string langcd, string comcd, string bizcd, string plant_cd, string reorg_id, string dept, string userid, string mrp_id)
        {
            InitializeComponent();

            LANGCD = langcd;
            COMCD = comcd;
            BIZCD = bizcd;
            PLANT_CD = plant_cd;
            REORG_ID = reorg_id;
            DEPT = dept;
            USERID = userid;
            MRP_ID = mrp_id;
        }
        #endregion

        #region 폼로드 이벤트
        private void PSA023P1_Load(object sender, System.EventArgs e)
        {
            try
            {
                th = new Thread(new ThreadStart(SchStart));
                th.Start();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "MRP Thread 작업"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Thread 시작
        public void SchStart()
        {
            string ERRCode = "ER";
            string MSGCode = "P0000";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strQuery = "";
                strQuery = " usp_PSA023 'C1' ";
                strQuery += ", @pLANG_CD = '" + LANGCD + "' ";
                strQuery += ", @pCO_CD = '" + COMCD + "' ";
                strQuery += ", @pBIZ_CD = '" + BIZCD + "' ";
                strQuery += ", @pPLANT_CD = '" + PLANT_CD + "' ";
                strQuery += ", @pREORG_ID = '" + REORG_ID + "' ";
                strQuery += ", @pDEPT_CD = '" + DEPT + "' ";
                strQuery += ", @pUPDT_ID = '" + USERID + "'";
                strQuery += ", @pMRP_ID = '" + MRP_ID + "' ";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프

                Trans.Commit();
            }
            catch (Exception f)
            {
                Trans.Rollback();
                SystemBase.Loggers.Log(this.Name, f.ToString());
                ERRCode = "ER";
                MSGCode = "P0001";
            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            else if (ERRCode == "ER")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.No;
            }
            else
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.No;
            }
        }
        #endregion

        #region 닫기
        private void PSA023P1_FormClosing(object sender, FormClosingEventArgs e)
        {
            th.Abort();
        }
        #endregion  
    }
}
