   using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;

namespace BA.BAA007
{
    public partial class BAA007P1 : System.Windows.Forms.Form
    {
        #region 변수선언
        double dblRow = 0;
        #endregion

        #region 생성자
        public BAA007P1(double dblNum)
        {
            dblRow = dblNum;
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BAA007P1_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);	//필수적용
            SystemBase.Validation.GroupBox_Setting(groupBox2);


            string strSql = " usp_BAA007  'S2'";
            strSql = strSql + ", @pNum ='" + dblRow + "' ";

            DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);


            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["LOG_TIME"].ToString() != "") lblDate.Value = ds.Tables[0].Rows[0]["LOG_TIME"].ToString();
                if (ds.Tables[0].Rows[0]["FORM_NAME"].ToString() != "") lblMenuId.Value = ds.Tables[0].Rows[0]["FORM_NAME"].ToString();
                if (ds.Tables[0].Rows[0]["ERR_MSG"].ToString() != "") txtMemo.Value = ds.Tables[0].Rows[0]["ERR_MSG"].ToString();
            }
        }
        #endregion

        #region Close 버튼
        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
