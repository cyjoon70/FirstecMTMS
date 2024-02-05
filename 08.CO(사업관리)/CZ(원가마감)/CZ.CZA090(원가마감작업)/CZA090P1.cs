using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

namespace CZ.CZA090
{
    public partial class CZA090P1 : Form
    {
        #region 변수선언
        string strWorkType = "";
        string strCloseMonth = "";

        int chk = 0;
        #endregion

        public CZA090P1()
        {
            InitializeComponent();
        }

        public CZA090P1(string WorkType, string CloseMonth)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //

            strWorkType = WorkType;
            strCloseMonth = CloseMonth;
        }

        #region Form Load시
        private void CZA090P1_Load(object sender, System.EventArgs e)
        {
            if (strWorkType == "R")
            { this.Text = "원가마감작업중...."; }
            else
            { this.Text = "원가마감취소중...."; }
        }
        #endregion

        #region 원가마감작업
        private void CZA090P1_Activated(object sender, System.EventArgs e)
        {
            chk ++ ;

            if (chk <= 1) //한번만 작업하기 위해서
            {
                string ERRCode = "WR", MSGCode = "P0000"; //처리할 내용이 없습니다.
                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    string strSql = " usp_CZA090 'I1' ";
                    strSql += ", @pWORK_TYPE = '" + strWorkType + "'";
                    strSql += ", @pYYMM = '" + strCloseMonth + "'";
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
                    Close();
                    this.DialogResult = DialogResult.OK;
                }
                else if (ERRCode == "ER")
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    this.DialogResult = DialogResult.Cancel;
                }
                else
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }
        #endregion

    }
}
