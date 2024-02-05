#region 작성정보
/*********************************************************************/
// 단위업무명 : 보세수불장조회
// 작 성 자 : 권 순 철
// 작 성 일 : 2013-06-12
// 작성내용 : 보세수불장조회 관리
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
using WNDW;
using FarPoint.Win.Spread.CellType;

namespace IBF.IBFB15
{
    public partial class IBFB15 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB15()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void IBFB15_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[6];
            string[] FormulaField2 = new string[1]; //formula 값
            string[] FormulaField1 = new string[1]; //formula 이름


            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            string RptName = @"Report\" + "IBFB33P.rpt";
            RptParmValue[0] = "R1";
            RptParmValue[1] = txtTRNo.Text;

            if (txtItemCd.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = txtItemCd.Text;

            if (dtpDT_FR.Text.Trim() == "") RptParmValue[3] = " ";
            else RptParmValue[3] = dtpDT_FR.Text;

            if (dtpDT_TO.Text.Trim() == "") RptParmValue[4] = " ";
            else RptParmValue[4] = dtpDT_TO.Text;

            RptParmValue[5] = SystemBase.Base.gstrCOMCD;

            FormulaField2[0] = "\"" + txtBUSINESS_NM.Text + "\"";
            FormulaField1[0] = "BUSI_NM";

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", FormulaField2, FormulaField1, RptName, RptParmValue);	//공통크리스탈 11버전
            frm.ShowDialog();
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                try
                {

                    string strQuery = " usp_IBFB15  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "',  ";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "', ";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "', ";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    //					UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 6, false);					

                    System.Data.DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                    fpSpread1.Sheets[0].Rows.Count = 0;
                    decimal dblrest = 0;
                    //					decimal dblcal =0;
                    //					decimal dblloss = 0;
                    int cnt = 1;

                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {

                            fpSpread1.Sheets[0].AddRows(j, 1);
                            fpSpread1.Sheets[0].Cells[j, 1].Value = dt.Rows[j][1].ToString();
                            fpSpread1.Sheets[0].Cells[j, 2].Text = dt.Rows[j][2].ToString();
                            fpSpread1.Sheets[0].Cells[j, 3].Text = dt.Rows[j][3].ToString();
                            fpSpread1.Sheets[0].Cells[j, 4].Text = dt.Rows[j][4].ToString();

                            if (dt.Rows[j][5].ToString() == "0")
                            {
                                fpSpread1.Sheets[0].Cells[j, 5].Text = "";

                                if (dt.Rows[j][7].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 7].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 7].Value = dt.Rows[j][7].ToString();

                                if (dt.Rows[j][8].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 8].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 8].Value = dt.Rows[j][8].ToString();

                                if (dt.Rows[j][9].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 9].Text = "";
                                else
                                {
                                    decimal minus_qty = Convert.ToDecimal(dt.Rows[j][9].ToString());
                                    if (minus_qty < 0) minus_qty = Math.Abs(minus_qty);
                                    fpSpread1.Sheets[0].Cells[j, 9].Value = minus_qty;
                                }

                                if (dt.Rows[j][10].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 10].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 10].Value = dt.Rows[j][10].ToString();

                                if (dt.Rows[j][11].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 11].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 11].Value = dt.Rows[j][11].ToString();

                                //								dblloss =  Convert.ToDecimal(dt.Rows[j][14].ToString());

                                //								if(Convert.ToDecimal(dt.Rows[j][11].ToString()) < 0)
                                dblrest = dblrest - Convert.ToDecimal(dt.Rows[j][14].ToString()) + Convert.ToDecimal(dt.Rows[j][7].ToString()) + Convert.ToDecimal(dt.Rows[j][8].ToString()) - Convert.ToDecimal(dt.Rows[j][9].ToString()) - Convert.ToDecimal(dt.Rows[j][10].ToString()) - Convert.ToDecimal(dt.Rows[j][11].ToString());
                                //								else 
                                //									dblrest = dblrest + Convert.ToDecimal(dt.Rows[j][7].ToString()) + Convert.ToDecimal(dt.Rows[j][8].ToString()) - Convert.ToDecimal(dt.Rows[j][9].ToString()) - Convert.ToDecimal(dt.Rows[j][10].ToString()) - Convert.ToDecimal(dt.Rows[j][11].ToString());

                                fpSpread1.Sheets[0].Cells[j, 12].Value = dblrest;

                                if (dt.Rows[j][13].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 13].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 13].Value = dt.Rows[j][13].ToString();

                                if (dt.Rows[j][14].ToString() == "0") fpSpread1.Sheets[0].Cells[j, 14].Text = "";
                                else fpSpread1.Sheets[0].Cells[j, 14].Value = dt.Rows[j][14].ToString();

                            }
                            else
                            {
                                fpSpread1.Sheets[0].Cells[j, 5].Value = dt.Rows[j][5].ToString();
                                fpSpread1.Sheets[0].Cells[j, 7].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 8].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 9].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 10].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 11].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 12].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 13].Text = "";
                                fpSpread1.Sheets[0].Cells[j, 14].Text = "";
                                dblrest = 0;
                                //								dblloss = 0;
                            }

                            fpSpread1.Sheets[0].Cells[j, 6].Text = dt.Rows[j][6].ToString();

                            if ((j + 1) < dt.Rows.Count)
                            {
                                if (dt.Rows[j][1].ToString() != dt.Rows[j + 1][1].ToString())
                                {
                                    dblrest = 0;
                                    //									dblloss = 0;
                                }
                            }

                            //--같은 품목 rowspan
                            if (j >= 1)
                            {
                                if (fpSpread1.Sheets[0].Cells[j - 1, 1].Text == fpSpread1.Sheets[0].Cells[j, 1].Text)
                                {
                                    cnt++;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 1].RowSpan = cnt;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 2].RowSpan = cnt;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 3].RowSpan = cnt;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 4].RowSpan = cnt;

                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                    fpSpread1.Sheets[0].Cells[j - cnt + 1, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                                }
                                else
                                {
                                    cnt = 1;
                                }

                            }

                        }

                    }
                    else
                    {
                        MessageBox.Show(SystemBase.Base.MessageRtn("B0011"), SystemBase.Base.MessageRtn("Z0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);  //검색된 데이타가 없습니다.
                    }

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }
        #endregion

        #region 버튼 Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF11' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF21' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtItemCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Text = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }


        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBUSINESS_CD.Value = ds.Tables[0].Rows[0][0].ToString();
                        txtBUSINESS_NM.Value = ds.Tables[0].Rows[0][1].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dtpDT_FR_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_FR.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_FR.Text) == false)
//                {
//                    MessageBox(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_FR.Focus();
//                    dtpDT_FR.SelectAll();
//                }
            }
        }

        private void dtpDT_TO_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_TO.Text.Trim() != "")
            {
//                if (SystemBase.Base.IsDate(dtpDT_TO.Text) == false)
//                {
//                    MessageBox(SystemBase.Base.MessageRtn("B023"));
//                    dtpDT_TO.Focus();
//                    dtpDT_TO.SelectAll();
//                }
            }
        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }

        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region 폼 Activated & Deactivated
        private void IBFB15_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB15_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

    }
}








