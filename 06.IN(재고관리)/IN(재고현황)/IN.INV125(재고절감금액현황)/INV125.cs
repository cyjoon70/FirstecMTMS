#region 작성정보
/*********************************************************************/
// 단위업무명 : 재고절감금액현황
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-04-11
// 작성내용 : 재고절감금액현황 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using WNDW;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace IN.INV125
{
    public partial class INV125 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        string[] mon = new string[12];
        #endregion

        public INV125()
        {
            InitializeComponent();
        }

        #region Form Load시
        private void INV125_Load(object sender, System.EventArgs e)
        {
            try
            {
                SystemBase.Validation.GroupBox_Setting(groupBox1);
                SystemBase.Validation.GroupBox_Setting(groupBox2);

                SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ", 0);//공장
                SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

                string year = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();
                mskDT_Fr.Text = year + "-01-01";
                mskDT_To.Text = year + "-12-31";

                cboItemAcct.SelectedValue = "30"; //원자재
                Set_First_Column();

                lnkJump1.Text = "재고절감자재현황";
                strJumpFileName1 = "MR.MRQ507.MRQ507";
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            string year = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).Year.ToString();
            mskDT_Fr.Text = year + "-01-01";
            mskDT_To.Text = year + "-12-31";

            fpSpread1.Sheets[0].ColumnCount = 4;

            cboItemAcct.SelectedValue = "30";
        }
        #endregion

        #region SearchExec 그리드 조회
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {

                    if (Convert.ToInt32(Convert.ToDateTime(mskDT_To.Text).ToShortDateString().Replace("-", "")) > Convert.ToInt32(Convert.ToDateTime(mskDT_Fr.Text).AddYears(1).ToShortDateString().Replace("-", "")))
                    {
                        MessageBox.Show("기준일자는 1년이상이 되면 안됩니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        mskDT_To.Focus();
                        return;
                    }

                    string strQuery1 = " usp_INV125  'S1'";
                    string strQuery = ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue + "'";
                    strQuery += ", @pDT_ST  ='" + mskDT_Fr.Text + "'";
                    strQuery += ", @pDT_ED  ='" + mskDT_To.Text + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataTable dt1 = SystemBase.DbOpen.NoTranDataTable(strQuery1 + strQuery);
                    fpSpread1.Sheets[0].ColumnCount = 4;

                    decimal[] sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    int col_idx = 0;
                    int row_idx = 0;
                    string temp_mon = "";
                    if (dt1.Rows.Count > 0)
                    {
                        Set_Column_Header();
                        for (int i = 0; i < dt1.Rows.Count; i++)
                        {
                            if (dt1.Rows[i]["REQ_YM"].ToString() != temp_mon)
                            {
                                for (int j = 0; j < 12; j++)
                                {
                                    if (mon[j] == dt1.Rows[i]["REQ_YM"].ToString())
                                    {
                                        col_idx = 4 + j; break;
                                    }
                                }
                            }
                            for (int k = 0; k < fpSpread1.Sheets[0].RowCount; k++)
                            {
                                if (fpSpread1.Sheets[0].Cells[k, 1].Text == dt1.Rows[i]["MINOR_CD"].ToString())
                                { row_idx = k; break; }
                            }

                            temp_mon = dt1.Rows[i]["REQ_YM"].ToString();
                            if (dt1.Rows[i]["MINOR_CD"].ToString() != "zb1" && dt1.Rows[i]["MINOR_CD"].ToString() != "zb2")
                                sum[row_idx] += Convert.ToDecimal(dt1.Rows[i]["G_STOCK_REF_AMT"]);
                            fpSpread1.Sheets[0].Cells[row_idx, col_idx].Value = dt1.Rows[i]["G_STOCK_REF_AMT"];
                        }
                    }

                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        strQuery1 = " usp_INV125  'S21' ";
                        strQuery1 += strQuery;
                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if (dt.Rows.Count > 0) txtItemCnt.Value = dt.Rows[0][0];
                        else txtItemCnt.Value = 0;

                        strQuery1 = " usp_INV125  'S22' ";
                        strQuery1 += strQuery;
                        DataTable dt3 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if (dt3.Rows.Count > 0) txtTotalAmt.Value = dt3.Rows[0][0];
                        else txtTotalAmt.Value = 0;


                        for (int j = 0; j < fpSpread1.Sheets[0].RowCount - 2; j++)
                        {
                            fpSpread1.Sheets[0].Cells[j, fpSpread1.Sheets[0].ColumnCount - 1].Value = sum[j];
                        }

                        strQuery1 = " usp_INV125  'S5'";
                        strQuery1 += strQuery;
                        DataTable dt2 = SystemBase.DbOpen.NoTranDataTable(strQuery1);

                        if (dt2.Rows.Count > 0)
                        {
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 3, fpSpread1.Sheets[0].ColumnCount - 1].Value = dt2.Rows[0][1];
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 2, fpSpread1.Sheets[0].ColumnCount - 1].Value = dt2.Rows[1][1];
                        }
                        else
                        {
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 3, fpSpread1.Sheets[0].ColumnCount - 1].Value = 0;
                            fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 2, fpSpread1.Sheets[0].ColumnCount - 1].Value = 0;
                        }
                        fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 1, fpSpread1.Sheets[0].ColumnCount - 1].Value =
                            Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 3, fpSpread1.Sheets[0].ColumnCount - 1].Value.ToString())
                            + Convert.ToDecimal(fpSpread1.Sheets[0].Cells[fpSpread1.Sheets[0].RowCount - 2, fpSpread1.Sheets[0].ColumnCount - 1].Value.ToString());
                    }
                    else
                    {
                        txtItemCnt.Value = 0;
                        txtTotalAmt.Value = 0;
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
            this.Cursor = Cursors.Default;
        }

        private void Set_Column_Header()
        {
            try
            {
                int mon_fr = Convert.ToDateTime(mskDT_Fr.Text).Month;
                int year = Convert.ToDateTime(mskDT_Fr.Text).Year;
                int mon_to = Convert.ToDateTime(mskDT_To.Text).Month;
                int cnt = fpSpread1.Sheets[0].ColumnCount;
                int i = mon_fr;
                int j = 0;
                FarPoint.Win.Spread.CellType.NumberCellType num = new FarPoint.Win.Spread.CellType.NumberCellType();
                while (i <= mon_to && j < 12)
                {
                    cnt++;
                    fpSpread1.Sheets[0].ColumnCount = cnt;
                    fpSpread1.Sheets[0].ColumnHeader.Cells[0, cnt - 1].Text = i.ToString() + "월";
                    if (i.ToString().Length == 1)
                        mon[j] = year.ToString() + "0" + i.ToString();
                    else
                        mon[j] = year.ToString() + i.ToString();
                    i++; j++;
                    if (i == 13) { i = 1; year++; }

                    num.DecimalSeparator = ".";
                    num.DecimalPlaces = 2;
                    num.FixedPoint = true;
                    num.Separator = ",";
                    num.ShowSeparator = true;
                    num.MaximumValue = 99999999999999;
                    num.MinimumValue = -99999999999999;
                    fpSpread1.Sheets[0].Columns[cnt - 1].CellType = num;
                    fpSpread1.Sheets[0].Columns[cnt - 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                    fpSpread1.Sheets[0].Columns[cnt - 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                    fpSpread1.Sheets[0].Columns[cnt - 1].Locked = true;
                    fpSpread1.Sheets[0].Columns[cnt - 1].BackColor = Color.LightGray;
                    fpSpread1.Sheets[0].Columns[cnt - 1].Width = 90;
                }
                cnt++;
                fpSpread1.Sheets[0].ColumnCount = cnt;
                fpSpread1.Sheets[0].ColumnHeader.Cells[0, cnt - 1].Text = "합계";

                num.DecimalSeparator = ".";
                num.DecimalPlaces = 2;
                num.FixedPoint = true;
                num.Separator = ",";
                num.ShowSeparator = true;
                num.MaximumValue = 99999999999999;
                num.MinimumValue = -99999999999999;
                fpSpread1.Sheets[0].Columns[cnt - 1].CellType = num;
                fpSpread1.Sheets[0].Columns[cnt - 1].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread1.Sheets[0].Columns[cnt - 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
                fpSpread1.Sheets[0].Columns[cnt - 1].Locked = true;
                fpSpread1.Sheets[0].Columns[cnt - 1].BackColor = Color.LightGray;
                fpSpread1.Sheets[0].Columns[cnt - 1].Width = 100;
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        private void Set_First_Column()
        {
            try
            {

                string strQuery = " usp_INV125  'S4'";
                strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                if (fpSpread1.Sheets[0].RowCount > 0)
                {
                    fpSpread1.Sheets[0].Cells[0, 2].RowSpan = 3;
                    fpSpread1.Sheets[0].Cells[3, 2].RowSpan = 3;
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

        #endregion

        #region mskDT_To_TextChanged
        private void mskDT_To_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Convert.ToDateTime(mskDT_To.Text).ToShortDateString().Replace("-", "")) > Convert.ToInt32(Convert.ToDateTime(mskDT_Fr.Text).AddYears(1).ToShortDateString().Replace("-", "")))
                {
                    MessageBox.Show("기준일자는 1년이상이 되면 안됩니다!", SystemBase.Base.MessageRtn("Z0003"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mskDT_To.Focus();
                    return;
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.Message, SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region Form Activated & Deactivate
        private void INV125_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV125_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion

        #region Link1Exec
        protected override void Link1Exec()
        {

            param = new object[3];					// 파라메터수 
            param[0] = cboPlantCd.SelectedValue;
            param[1] = mskDT_Fr.Text;
            param[2] = mskDT_To.Text;

            SystemBase.Base.RodeFormID = "MRQ507";			// 이동할 formid를 적어준다.(메뉴id)
            SystemBase.Base.RodeFormText = "재고감안자재현황"; 	// 이동할 폼명을 적어준다(메뉴명)
        }
        #endregion

        #region lnkJump_Click 점프 클릭 이벤트
        private void lnkJump1_Click(object sender, EventArgs e)
        {
            try
            {
                if (strJumpFileName1.Length > 0)
                {
                    string DllName = strJumpFileName1.Substring(0, strJumpFileName1.IndexOf("."));
                    string FrmName = strJumpFileName1.Substring(strJumpFileName1.IndexOf(".") + 1, strJumpFileName1.Length - strJumpFileName1.IndexOf(".") - 1);

                    for (int k = 0; k < this.MdiParent.MdiChildren.Length; k++)
                    {	// 폼이 이미 열려있으면 닫기
                        if (MdiParent.MdiChildren[k].Name == FrmName.Substring(0, 6))
                        {
                            MdiParent.MdiChildren[k].BringToFront();
                            MdiParent.MdiChildren[k].Close();
                            break;
                        }
                    }

                    Link1Exec();

                    Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + DllName + "." + FrmName.Substring(0, 6) + ".dll");
                    Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType(strJumpFileName1), param);
                    myForm.MdiParent = this.MdiParent;
                    myForm.Show();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0050", "화면 링크"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
