#region 작성정보
/*********************************************************************/
// 단위업무명 : 생산진행 현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-12
// 작성내용 : 생산징행 현황 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace EI.EIS101
{
    public partial class EIS101 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public EIS101()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void EIS101_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //기타 세팅
            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(3).ToShortDateString();

            dtpGiDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(-1).ToShortDateString();
            panel1.Height = 112;

            string strQuery = " usp_EIS101 'S3'";
            strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
            DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

            if (Convert.ToString(dt.Rows[0][0]) != "") dtpDtFr.Text = dt.Rows[0][0].ToString();
            if (Convert.ToString(dt.Rows[0][1]) != "") dtpDtTo.Text = dt.Rows[0][1].ToString();
            if (Convert.ToString(dt.Rows[0][2]) != "") dtpGiDt.Text = dt.Rows[0][2].ToString();

            lblBus_Nm.Text = "";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            groupBox2.Controls.Clear();

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString();
            dtpDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(3).ToShortDateString();

            dtpGiDt.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddDays(-1).ToShortDateString();
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            fpSpread1.Sheets[0].RowCount = 0;
            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    groupBox2.Controls.Clear();

                    string strQuery = " usp_EIS101 'S1'";
                    strQuery += ", @pDT_FR ='" + dtpDtFr.Text.Trim() + "'";
                    strQuery += ", @pDT_TO ='" + dtpDtTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pGI_DT='" + dtpGiDt.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    if (dt.Rows.Count > 0)
                    {
                        int cnt = dt.Rows.Count;
                        int x = 8;
                        int y = 16;
                        int x_gap = 98;
                        int y_gap = 32;
                        int[] temp_x = new int[10] { 9, 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                        int idx = 0;
                        for (int i = 0; i < cnt; i++)
                        {

                            System.Windows.Forms.Control[] Cmd_Bt = new Control[cnt];

                            Cmd_Bt[i] = new C1.Win.C1Input.C1Button(); //System.Windows.Forms.Button();                  //버튼 객체 생성
                            Cmd_Bt[i].BackgroundImage = btnCreate.BackgroundImage;
                            Cmd_Bt[i].Cursor = Cursors.Hand;
                            Cmd_Bt[i].Name = "button" + i.ToString();
                            Cmd_Bt[i].Text = dt.Rows[i]["ENT_NM"].ToString();              //버튼 이름 설정

                            if (dt.Rows[i]["STATE"].ToString() == "-1")
                                Cmd_Bt[i].ForeColor = System.Drawing.Color.Red;
                            else if (dt.Rows[i]["STATE"].ToString() == "0")
                                Cmd_Bt[i].ForeColor = System.Drawing.Color.Blue;
                            else
                                Cmd_Bt[i].ForeColor = System.Drawing.Color.Black;

                            Cmd_Bt[i].Parent = groupBox2;

                            if ((i + 1) % 10 == 1 && i > 9) y += y_gap;

                            idx = (i + 1) % 10;
                            Cmd_Bt[i].Location = new Point(x + (x_gap * temp_x[idx]), y);

                            Cmd_Bt[i].Size = new Size(80, 25);         //버튼 크기 설정
                            Cmd_Bt[i].Tag = dt.Rows[i]["ENT_CD"].ToString();
                            Cmd_Bt[i].Click += new System.EventHandler(this.button_Click);  //버튼 이벤트 설정

                        }
                        if (cnt > 10)
                        {
                            if (cnt % 10 == 0)
                                panel1.Height = 112 + y_gap * ((cnt / 10) - 1);
                            else
                                panel1.Height = 112 + y_gap * (cnt / 10);
                        }
                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        private void button_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                System.Windows.Forms.Button c = (System.Windows.Forms.Button)sender;

                string strQuery = " usp_EIS101 'S2'";
                strQuery += ", @pENT_CD ='" + c.Tag + "'";
                strQuery += ", @pGI_DT ='" + dtpGiDt.Text + "'";
                strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                strQuery += ", @pDT_FR ='" + dtpDtFr.Text.Trim() + "'";
                strQuery += ", @pDT_TO ='" + dtpDtTo.Text.Trim() + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 5, true);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    Set_Color();
                    //					dtpGiDt.Text  = fpSpread1.Sheets[0].Cells[0,17].Text;
                }

                lblBus_Nm.Text = c.Text;

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;


        }

        private void Set_Color()
        {
            int cnt1 = 1;
            int cnt2 = 1;
            int cnt3 = 1;
            int row1 = 0;
            int row2 = 0;
            int row3 = 0;
            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (i > 0)
                {
                    if (fpSpread1.Sheets[0].Cells[i - 1, 1].Text == fpSpread1.Sheets[0].Cells[i, 1].Text)
                    {
                        cnt1++;
                        if (fpSpread1.Sheets[0].Cells[i - 1, 2].Text == fpSpread1.Sheets[0].Cells[i, 2].Text)
                        {
                            cnt2++;
                            if (fpSpread1.Sheets[0].Cells[i - 1, 3].Text == fpSpread1.Sheets[0].Cells[i, 3].Text)
                            {
                                cnt3++;

                            }
                            else
                            {
                                if (cnt3 > 1)
                                {
                                    fpSpread1.Sheets[0].Cells[row3, 6].RowSpan = cnt3;
                                    fpSpread1.Sheets[0].Cells[row3, 7].RowSpan = cnt3;
                                    fpSpread1.Sheets[0].Cells[row3, 8].RowSpan = cnt3;
                                    fpSpread1.Sheets[0].Cells[row3, 9].RowSpan = cnt3;
                                    fpSpread1.Sheets[0].Cells[row3, 10].RowSpan = cnt3;
                                    fpSpread1.Sheets[0].Cells[row3, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[row3, 7].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[row3, 8].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[row3, 9].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[row3, 10].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                }
                                cnt3 = 1;
                                row3 = i;

                            }

                        }
                        else
                        {
                            if (cnt2 > 1)
                            {
                                fpSpread1.Sheets[0].Cells[row2, 5].RowSpan = cnt2;
                                fpSpread1.Sheets[0].Cells[row2, 5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            }
                            if (cnt3 > 1)
                            {
                                fpSpread1.Sheets[0].Cells[row3, 6].RowSpan = cnt3;
                                fpSpread1.Sheets[0].Cells[row3, 7].RowSpan = cnt3;
                                fpSpread1.Sheets[0].Cells[row3, 8].RowSpan = cnt3;
                                fpSpread1.Sheets[0].Cells[row3, 9].RowSpan = cnt3;
                                fpSpread1.Sheets[0].Cells[row3, 10].RowSpan = cnt3;
                                fpSpread1.Sheets[0].Cells[row3, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                fpSpread1.Sheets[0].Cells[row3, 7].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                fpSpread1.Sheets[0].Cells[row3, 8].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                fpSpread1.Sheets[0].Cells[row3, 9].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                fpSpread1.Sheets[0].Cells[row3, 10].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            }
                            cnt2 = 1;
                            row2 = i;
                            cnt3 = 1;
                            row3 = i;

                        }
                    }
                    else
                    {
                        if (cnt1 > 1)
                        {
                            fpSpread1.Sheets[0].Cells[row1, 4].RowSpan = cnt1;
                            fpSpread1.Sheets[0].Cells[row1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        }
                        if (cnt2 > 1)
                        {
                            fpSpread1.Sheets[0].Cells[row2, 5].RowSpan = cnt2;
                            fpSpread1.Sheets[0].Cells[row2, 5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        }
                        if (cnt3 > 1)
                        {
                            fpSpread1.Sheets[0].Cells[row3, 6].RowSpan = cnt3;
                            fpSpread1.Sheets[0].Cells[row3, 7].RowSpan = cnt3;
                            fpSpread1.Sheets[0].Cells[row3, 8].RowSpan = cnt3;
                            fpSpread1.Sheets[0].Cells[row3, 9].RowSpan = cnt3;
                            fpSpread1.Sheets[0].Cells[row3, 10].RowSpan = cnt3;
                            fpSpread1.Sheets[0].Cells[row3, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[row3, 7].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[row3, 8].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[row3, 9].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                            fpSpread1.Sheets[0].Cells[row3, 10].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        }
                        cnt1 = 1;
                        row1 = i;
                        cnt2 = 1;
                        row2 = i;
                        cnt3 = 1;
                        row3 = i;

                    }
                }
                if (fpSpread1.Sheets[0].Cells[i, 11].Text == "계획") continue;
                for (int j = 12; j < fpSpread1.Sheets[0].ColumnCount; j++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, j].Text == "위험") fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                    if (fpSpread1.Sheets[0].Cells[i, j].Text == "관리") fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Blue;
                }
            }
            if (cnt3 > 1)
            {
                fpSpread1.Sheets[0].Cells[row3, 6].RowSpan = cnt3;
                fpSpread1.Sheets[0].Cells[row3, 7].RowSpan = cnt3;
                fpSpread1.Sheets[0].Cells[row3, 8].RowSpan = cnt3;
                fpSpread1.Sheets[0].Cells[row3, 9].RowSpan = cnt3;
                fpSpread1.Sheets[0].Cells[row3, 10].RowSpan = cnt3;
                fpSpread1.Sheets[0].Cells[row3, 6].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                fpSpread1.Sheets[0].Cells[row3, 7].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                fpSpread1.Sheets[0].Cells[row3, 8].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                fpSpread1.Sheets[0].Cells[row3, 9].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                fpSpread1.Sheets[0].Cells[row3, 10].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
            }

            if (cnt2 > 1)
            {
                fpSpread1.Sheets[0].Cells[row2, 5].RowSpan = cnt2;
                fpSpread1.Sheets[0].Cells[row2, 5].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
            }
            if (cnt1 > 1)
            {
                fpSpread1.Sheets[0].Cells[row1, 4].RowSpan = cnt1;
                fpSpread1.Sheets[0].Cells[row1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
            }
        }
        #endregion

        #region 버튼 Click
        // 프로젝트
        private void btnProject_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW007 pu = new WNDW007(txtProject_No.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;
                    txtProject_No.Text = Msgs[3].ToString();
                    txtProject_Nm.Value = Msgs[4].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region TextChanged
        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, "AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        #region 자료생성
        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string ERRCode = "ER", MSGCode = "";
            SqlConnection dbConn = SystemBase.DbOpen.DBCON();
            SqlCommand cmd = dbConn.CreateCommand();
            SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    if (MessageBox.Show(SystemBase.Base.MessageRtn("E0004", dtpGiDt.Text), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        string strQuery = "usp_EIS101  'P1',";
                        strQuery = strQuery + " @pGI_DT = '" + dtpGiDt.Text + "',";
                        strQuery = strQuery + " @pDT_FR = '" + dtpDtFr.Text + "',";
                        strQuery = strQuery + " @pDT_TO = '" + dtpDtTo.Text + "' ";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataSet ds = SystemBase.DbOpen.TranDataSet(strQuery, dbConn, Trans);

                        ERRCode = ds.Tables[0].Rows[0][0].ToString();
                        MSGCode = ds.Tables[0].Rows[0][1].ToString();

                        if (ERRCode != "OK") { Trans.Rollback(); goto Exit; }
                        Trans.Commit();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        Exit:
            dbConn.Close();

            if (ERRCode == "OK")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn(MSGCode), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                SearchExec();
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

        #region 그리드 CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

            if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "개발일정"))
            {
                try
                {
                    EIS101P01 myForm
                        = new EIS101P01(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    DialogResult dsMsg = MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "자재발주"))
            {
                try
                {
                    EIS101P07 myForm
                        = new EIS101P07(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                        dtpGiDt.Text);
                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "구매입고"))
            {
                try
                {
                    EIS101P08 myForm
                        = new EIS101P08(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);
                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "수입검사"))
            {
                try
                {
                    EIS101P09 myForm
                        = new EIS101P09(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산투입"))
            {
                try
                {
                    EIS101P11 myForm
                        = new EIS101P11(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "생산진행"))
            {
                try
                {
                    EIS101P12 myForm
                        = new EIS101P12(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "납기일"))
            {
                try
                {
                    EIS101P13 myForm
                        = new EIS101P13(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text,
                                        fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
            else if (e.Column == SystemBase.Base.GridHeadIndex(GHIdx1, "원가현황"))
            {
                try
                {
                    string strProjectNo = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "번호")].Text;
                    string ProjectSeq = fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "차수")].Text;

                    EIS101P14 myForm
                        = new EIS101P14(strProjectNo,
                                        ProjectSeq,
                                        "*", //fpSpread1.Sheets[0].Cells[e.Row,SystemBase.Base.GridHeadIndex(GHIdx1,"품목")].Text,
                                        dtpGiDt.Text);

                    myForm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //데이터 조회 중 오류가 발생하였습니다.

                }
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void EIS101_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProject_No.Focus();
        }

        private void EIS101_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
