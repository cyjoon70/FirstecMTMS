#region 작성정보
/*********************************************************************/
// 단위업무명 : MRP 전개
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-03-11
// 작성내용 : MRP 전개 및 관리
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

namespace PB.PSA022
{
    public partial class PSA022P1 : Form
    {
        Thread th;
        string LANGCD = "";
        string COMCD = "";
        string BIZCD = "";
        string PLANT_CD = "";
        string REORG_ID = "";
        string DEPT = "";
        string STOCK = "";
        string ENBLESTOCK = "";
        string ENBLEDAY = "";
        string SAFESTOCK = "";
        string USERID = "";
        string MRP_DT = "";
        string BIGO = "";
        string SCH_NO = "";
        public string errCode = "", msgText = "";

        #region 생성자
        public PSA022P1(
            string langcd,
            string comcd,
            string bizcd,
            string plant_cd,
            string reorg_id,
            string dept,
            string stock,
            string enblestock,
            string enbleday,
            string safestock,
            string userid,
            string mrp_dt,
            string bigo,
            string SchNo
            )
        {
            InitializeComponent();
            LANGCD = langcd;
            COMCD = comcd;
            BIZCD = bizcd;
            PLANT_CD = plant_cd;
            REORG_ID = reorg_id;
            DEPT = dept;
            STOCK = stock;
            ENBLESTOCK = enblestock;
            ENBLEDAY = enbleday;
            SAFESTOCK = safestock;
            USERID = userid;
            MRP_DT = mrp_dt;
            BIGO = bigo;
            SCH_NO = SchNo;
        }
        #endregion

        #region 폼로드 이벤트
        private void PSA022P1_Load(object sender, System.EventArgs e)
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
            string MSGText = "";

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                string strQuery = "";

                strQuery = " usp_PSA022 'I1' ";
                strQuery += ", @pLANG_CD = '" + LANGCD + "'";
                strQuery += ", @pCO_CD = '" + COMCD + "'";
                strQuery += ", @pBIZ_CD = '" + BIZCD + "'";
                strQuery += ", @pPLANT_CD = '" + PLANT_CD + "'";
                strQuery += ", @pREORG_ID = '" + REORG_ID + "'";
                strQuery += ", @pDEPT_CD = '" + DEPT + "'";
                strQuery += ", @pSTOCK_YN = '" + STOCK + "'";
                strQuery += ", @pENABLE_STOCK_YN = '" + ENBLESTOCK + "'";
                strQuery += ", @pENABLE_DAY_YN = '" + ENBLEDAY + "'";
                strQuery += ", @pSAFE_STOCK_YN = '" + SAFESTOCK + "'";
                strQuery += ", @pUP_ID = '" + USERID + "'";
                strQuery += ", @pMRP_DT = '" + MRP_DT + "'";
                strQuery += ", @pETC = '" + BIGO + "'";
                strQuery += ", @pSCH_NO = '" + SCH_NO + "'";

                DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                MSGCode = ds.Tables[0].Rows[0][1].ToString();
                MSGText = ds.Tables[0].Rows[0][2].ToString();

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
                errCode = ERRCode;
                msgText = MSGText;
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

            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 닫기
        private void PSA022P1_FormClosing(object sender, FormClosingEventArgs e)
        {
            th.Abort();
        }
        #endregion  
    }
}
