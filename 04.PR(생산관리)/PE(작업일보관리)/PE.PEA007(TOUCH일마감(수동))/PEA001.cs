#region 작성정보
/*********************************************************************/
// 단위업무명 : TOUCH 수동 일마감 전개/취소
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-10
// 작성내용 : TOUCH 수동 일마감 전개/취소
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
using System.Data.SqlClient;

namespace PE.PEA007
{
    public partial class PEA007 : Form
    {
        public PEA007()
        {
            InitializeComponent();
        }

        #region  폼 로드시
        private void PEA007_Load(object sender, EventArgs e)
        {
            this.Text = SystemBase.Base.RodeFormText;

            dtpWorkDt.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 화면 활성화시 이벤트
        private void PEA007_Activated(object sender, EventArgs e)
        {
            SystemBase.Base.RodeFormName = this.Name;
        }
        #endregion

        #region 닫기
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 전개
        private void btnTouchProc_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            string ERRCode = "ER", MSGCode = "P0000"; //처리할 내용이 없습니다.

            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                //행수만큼 처리
                string strSql = " usp_PEA007 'I1'";
                strSql += ", @pWORK_DT = '" + dtpWorkDt.Text + "' ";
                strSql += ", @pUP_ID = '" + SystemBase.Base.gstrUserID.ToString() + "' ";

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
                MSGCode = "P0001";	//에러가 발생되어 데이터 처리가 취소되었습니다.
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

            this.Cursor = Cursors.Default;
        }
        #endregion

    }
}
