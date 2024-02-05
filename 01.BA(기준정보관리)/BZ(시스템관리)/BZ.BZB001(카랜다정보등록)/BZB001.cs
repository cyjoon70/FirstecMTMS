#region 작성정보
/*********************************************************************/
// 단위업무명 : 카렌다정보등록
// 작 성 자 : 김 현 근
// 작 성 일 : 2013-03-21
// 작성내용 : 카렌다정보등록 및 관리
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

namespace BZ.BZB001
{
    public partial class BZB001 : UIForm.FPCOMM1
    {
        #region 생성자
        public BZB001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BZB001_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.ComboMake.C1Combo(cboCalendar, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'Z006', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'");
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0,0);

            Form_Clear();
            CboYm.Text = int.Parse(SystemBase.Base.ServerTime("YM")).ToString("####-##");

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strStDt = CboYm.Text + "-01";
                int intLastDay = Last_Day(CboYm.Text);
                string strEnDt = CboYm.Text + "-" + Convert.ToSingle(intLastDay);

                string strQuery = "usp_BZB001  'S1'";
                strQuery = strQuery + ", @pCAL_TYPE ='" + cboCalendar.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @pSTART_DT ='" + strStDt + "' ";
                strQuery = strQuery + ", @pEND_DT	 ='" + strEnDt + "' ";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strQuery);
                int intDsCount = ds.Tables[0].Rows.Count;

                if (intDsCount > 0)
                {
                    int intGbn = Week_type(CboYm.Text);
                    int intRow = 0; int intColumn = 0; intColumn = intGbn;
                    for (int i = 0; i < intDsCount; i++)
                    {
                        string strCalType = ds.Tables[0].Rows[i]["CAL_TYPE"].ToString();
                        string strDay = Convert.ToDateTime(ds.Tables[0].Rows[i]["CAL_DT"]).Day.ToString();
                        string strOffType = ds.Tables[0].Rows[i]["OFF_TYPE"].ToString();

                        if (intColumn < 8)
                        {
                            Sub_ColorType(intRow, intColumn, strOffType);
                            fpSpread1.Sheets[0].Cells[intRow, intColumn++].Text = strDay;
                        }
                        else
                        {
                            intRow++; intColumn = 1;
                            Sub_ColorType(intRow, intColumn, strOffType);
                            fpSpread1.Sheets[0].Cells[intRow, intColumn++].Text = strDay;
                        }
                    }
                }
                else
                {
                    Form_Clear();
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0033"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmd_Click();
                }
            }

            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
        #endregion

        #region Sub_Search() 달력 조회
        private void Sub_Search()
        {
            int intLastDay = Last_Day(CboYm.Text);
            int intGbn = Week_type(CboYm.Text);
            int intRow = 0; int intColumn = 0; intColumn = intGbn;

            for (int i = 1; i < intLastDay + 1; i++)
            {
                if (intColumn < 8)
                {
                    Sub_Color(intRow, intColumn);
                    fpSpread1.Sheets[0].Cells[intRow, intColumn++].Text = Convert.ToString(i);
                }
                else
                {
                    intRow++; intColumn = 1;
                    Sub_Color(intRow, intColumn);
                    fpSpread1.Sheets[0].Cells[intRow, intColumn++].Text = Convert.ToString(i);
                }
            }
        }
        #endregion

        #region Cell Font에 색상주기 Sub_Color()
        private void Sub_Color(int Row, int Column)
        {
            if (Column == 1) fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Red;
            else if (Column == 7) fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Blue;
            else fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Black;
        }
        #endregion

        #region Cell Font에 색상주기(Type에 따른) Sub_ColorType
        private void Sub_ColorType(int Row, int Column, string Type)
        {
            if (Type == "2") fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Black;
            else if (Type == "1") fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Blue;
            else fpSpread1.Sheets[0].Cells[Row, Column].ForeColor = Color.Red;
        }
        #endregion

        #region 월에 마지막날 구하기 Last_Day()
        private static int Last_Day(string strYm)
        {
            System.DateTime myDate = System.DateTime.Now;
            myDate = Convert.ToDateTime(strYm + "-01");
            myDate = myDate.AddMonths(1);
            myDate = myDate.AddDays(-1);
            int intLastDay = Convert.ToInt32(myDate.Day);		//마지막날 일수

            return intLastDay;
        }
        #endregion

        #region 요일확인 Week_type()
        private static int Week_type(string strYm)
        {
            System.DateTime myDate = System.DateTime.Now;
            myDate = Convert.ToDateTime(strYm + "-01");
            string strWeek = Convert.ToString(myDate.DayOfWeek);		//첫날 요일

            int intGbn;
            switch (strWeek)
            {
                case "Sunday": intGbn = 1; break;
                case "Monday": intGbn = 2; break;
                case "Tuesday": intGbn = 3; break;
                case "Wednesday": intGbn = 4; break;
                case "Thursday": intGbn = 5; break;
                case "Friday": intGbn = 6; break;
                case "Saturday": intGbn = 7; break;
                default: intGbn = 0; break;
            }
            return intGbn;
        }
        #endregion

        #region Form 초기화
        private void Form_Clear()
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            fpSpread1.Font = new System.Drawing.Font("굴림", 12F, FontStyle.Bold);
            fpSpread1.Sheets[0].Rows.Count = 6;
            for (int j = 0; j < 6; j++) fpSpread1.Sheets[0].Rows[j].Height = 65;
        }
        #endregion

        #region 그리드 더블 클릭시 fpSpread1_CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            if (fpSpread1.Sheets[0].Cells[e.Row, e.Column].Text != "")
            {
                System.Drawing.Color CellColor = fpSpread1.Sheets[0].Cells[e.Row, e.Column].ForeColor;
                if (CellColor == Color.Blue) fpSpread1.Sheets[0].Cells[e.Row, e.Column].ForeColor = Color.Red;
                if (CellColor == Color.Red) fpSpread1.Sheets[0].Cells[e.Row, e.Column].ForeColor = Color.Black;
                if (CellColor == Color.Black) fpSpread1.Sheets[0].Cells[e.Row, e.Column].ForeColor = Color.Blue;
            }
        }
        #endregion

        #region 재생성클릭시 처리부 cmd_Click()
        private void cmd_Click()
        {
            DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0034"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (dsMsg == DialogResult.OK)
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    Form_Clear();
                    Sub_Search();
                    Save_Date();
                }
            }
        }
        #endregion

        #region 재생성클릭 cmdCreate_Click()
        private void cmdCreate_Click(object sender, EventArgs e)
        {
            int intTxtLen = CboYm.Text.Length;
            if (intTxtLen == 7 && (Convert.ToInt32(CboYm.Text.Substring(5, 2)) > 12 || Convert.ToInt32(CboYm.Text.Substring(5, 2)) < 1))
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0033"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            cmd_Click();
        }
        #endregion
       
        #region dtpYm_TextChanged 년월 텍스트 변경시
        private void CboYm_TextChanged(object sender, EventArgs e)
        {
            int intTxtLen = CboYm.Text.Length;
            if (intTxtLen == 7 && (Convert.ToInt32(CboYm.Text.Substring(5, 2)) > 12 || Convert.ToInt32(CboYm.Text.Substring(5, 2)) < 1))
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0033"), SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (intTxtLen == 7) SearchExec();
        }
        #endregion

        #region cboCalendar 선택 변경시
        private void cboCalendar_SelectedValueChanged(object sender, EventArgs e)
        {
            int intTxtLen = CboYm.Text.Length;
            if (intTxtLen == 7) SearchExec();
        }          
        #endregion

        #region Save_Date() 달력 저장 로직
        private void Save_Date()
        { 
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                int intTxtLen = CboYm.Text.Length;
                if (intTxtLen == 7 && (Convert.ToInt32(CboYm.Text.Substring(5, 2)) > 12 || Convert.ToInt32(CboYm.Text.Substring(5, 2)) < 1))
                {
                    SystemBase.MessageBoxComm.Show(SystemBase.Base.MessageRtn("B0033"));
                    return;
                }

                string ERRCode = "ER", MSGCode = "P0000";	//처리할 내용이 없습니다.

                SqlConnection dbConn = SystemBase.DbOpen.DBCON();
                SqlCommand cmd = dbConn.CreateCommand();
                SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //행수만큼 처리
                    for (int intRow = 0; intRow < fpSpread1.Sheets[0].Rows.Count; intRow++)
                    {
                        for (int intColumn = 1; intColumn < 8; intColumn++)
                        {
                            if (fpSpread1.Sheets[0].Cells[intRow, intColumn].Text != "")
                            {
                                string strCalType = ""; if (cboCalendar.SelectedValue.ToString() != "") strCalType = cboCalendar.SelectedValue.ToString();
                                string strDay = Convert.ToString("00" + fpSpread1.Sheets[0].Cells[intRow, intColumn].Text);
                                string strCalDt = CboYm.Text + "-" + strDay.ToString().Substring(strDay.Length - 2, 2);
                                string strOffType = "";
                                if (fpSpread1.Sheets[0].Cells[intRow, intColumn].ForeColor == Color.Black) strOffType = "2";
                                if (fpSpread1.Sheets[0].Cells[intRow, intColumn].ForeColor == Color.Blue) strOffType = "1";
                                if (fpSpread1.Sheets[0].Cells[intRow, intColumn].ForeColor == Color.Red) strOffType = "0";

                                string strSql = " usp_BZB001 'I1'";
                                strSql = strSql + ", @pCAL_TYPE  = '" + strCalType + "'";
                                strSql = strSql + ", @pCAL_DT	= '" + strCalDt + "'";
                                strSql = strSql + ", @pOFF_TYPE	= '" + strOffType + "'";
                                strSql += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                                DataSet ds = SystemBase.DbOpen.TranDataSet(strSql, dbConn, Trans);
                                ERRCode = ds.Tables[0].Rows[0][0].ToString();
                                MSGCode = ds.Tables[0].Rows[0][1].ToString();

                                if (ERRCode != "OK")
                                { Trans.Rollback(); goto Exit; }	// ER 코드 Return시 점프
                            }
                        }
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
                    SearchExec();
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
        }
        #endregion

        #region SaveExec() 폼에 입력된 데이타 저장 로직
        protected override void SaveExec()
        {
            Save_Date();
        }
        #endregion

    }
}
