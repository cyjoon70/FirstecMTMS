#region 작성정보
/*********************************************************************/
// 단위업무명 : 원재료실소요량계산서조회
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-10
// 작성내용 : 원재료실소요량계산서조회 및 관리
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

namespace IBF.IBFB05
{
    public partial class IBFB05 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB05()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFB05_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
        }
        #endregion

        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {

            string[] RptParmValue = new string[5];
            string RptName = "";

            if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
            //--레포트 파일 선택

            RptName = @"Report\" + "IBFB22P.rpt";
            RptParmValue[0] = "R1";
            RptParmValue[1] = txtTRNo.Text;
            if (txtItemCd.Text.Trim() == "") RptParmValue[2] = " ";
            else RptParmValue[2] = txtItemCd.Text;
            RptParmValue[3] = txtBASED_NO.Text;
            RptParmValue[4] = SystemBase.Base.gstrCOMCD;

            UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 10버전
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

                    string strQuery = " usp_IBFB05  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "', ";
                    strQuery = strQuery + " @pUSE_CREATE_NO = '" + txtBASED_NO.Text + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 6, false);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }

            }
            if (fpSpread1.Sheets[0].Rows.Count > 0) Spread_Sum();
            this.Cursor = Cursors.Default;
            fpSpread1.Focus();
        }

        private void Spread_Sum()
        {
            decimal amt1 = 0, amt2 = 0, amt3 = 0;
            decimal tot1 = 0, tot2 = 0, tot3 = 0;
            int i = 0;

            try
            {
                if (fpSpread1.Sheets[0].Rows.Count == 1)
                {
                    amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                    amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                    amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[0, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                    tot1 += amt1;
                    tot2 += amt2;
                    tot3 += amt3;

                    fpSpread1.Sheets[0].Rows.Add(1, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                    fpSpread1.Sheets[0].Rows[1].BackColor = SystemBase.Base.gColor2;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "합계";
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                    fpSpread1.Sheets[0].Cells[1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;


                }
                else
                {
                    for (i = 1; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text == fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text)
                        {
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                            amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                                amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                                tot1 += amt1;
                                tot2 += amt2;
                                tot3 += amt3;

                                fpSpread1.Sheets[0].Rows.Add(i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                                fpSpread1.Sheets[0].Rows[i + 1].BackColor = SystemBase.Base.gColor2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "합계";
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                                amt1 = 0; amt2 = 0; amt3 = 0;

                                i = i + 1;
                            }

                        }
                        else
                        {
                            amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                            amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                            amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i - 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                            tot1 += amt1;
                            tot2 += amt2;
                            tot3 += amt3;

                            fpSpread1.Sheets[0].Rows.Add(i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                            fpSpread1.Sheets[0].Rows[i].BackColor = SystemBase.Base.gColor2;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "합계";
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                            fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                            amt1 = 0; amt2 = 0; amt3 = 0;
                            i = i + 1;

                            if (i == fpSpread1.Sheets[0].Rows.Count - 1)
                            {
                                amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value);
                                amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value);
                                amt3 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value);

                                tot1 += amt1;
                                tot2 += amt2;
                                tot3 += amt3;

                                fpSpread1.Sheets[0].Rows.Add(i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                                fpSpread1.Sheets[0].Rows[i + 1].BackColor = SystemBase.Base.gColor2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "합계";
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = amt1;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = amt2;
                                fpSpread1.Sheets[0].Cells[i + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = amt3;

                                amt1 = 0; amt2 = 0; amt3 = 0;

                                i = i + 1;
                            }

                        }

                    }

                }
                int cnt = fpSpread1.Sheets[0].Rows.Count;
                fpSpread1.Sheets[0].Rows.Add(cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                fpSpread1.Sheets[0].Rows[cnt].BackColor = SystemBase.Base.gColor1;
                //				fpSpread1.Sheets[0].Cells[cnt, 13].CellType = new FarPoint.Win.Spread.CellType.TextCellType();
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "총합계";
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = tot1;
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "미화금액")].Value = tot2;
                fpSpread1.Sheets[0].Cells[cnt, SystemBase.Base.GridHeadIndex(GHIdx1, "외국재금액")].Value = tot3;

                fpSpread1.Sheets[0].Rows.Add(cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                fpSpread1.Sheets[0].Rows[cnt + 1].BackColor = SystemBase.Base.gColor1;
                fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text = "제조비율";
                fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "내국재";
                if (tot1 + tot3 == 0)
                    fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = 0;
                else
                    fpSpread1.Sheets[0].Cells[cnt + 1, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = (tot1 / (tot1 + tot3)) * 100;

                fpSpread1.Sheets[0].Rows.Add(cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No"));
                fpSpread1.Sheets[0].Rows[cnt + 2].BackColor = SystemBase.Base.gColor1;
                fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text = "제조비율";
                fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "외국재";
                if (tot1 + tot3 == 0)
                    fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = 0;
                else
                    fpSpread1.Sheets[0].Cells[cnt + 2, SystemBase.Base.GridHeadIndex(GHIdx1, "내국재금액")].Value = (tot3 / (tot1 + tot3)) * 100;


            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region Button Click
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF10' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Value = Msgs[0].ToString();
                    txtBUSINESS_CD.Value = Msgs[7].ToString();
                    txtBUSINESS_NM.Value = Msgs[8].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }


        private void btnItemCd_Click(object sender, System.EventArgs e)
        {
            try
            {
                //품목 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF04' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtItemCd.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP004", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "품목 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtItemCd.Value = Msgs[0].ToString();
                    txtItemNm.Value = Msgs[1].ToString();
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }

        private void butBASED_NO_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF19' ";
                string[] strWhere = new string[] { "@pSPEC" };
                string[] strSearch = new string[] { txtTRNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP013", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "원재료실소요량 근거번호 팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtBASED_NO.Value = Msgs[2].ToString();

                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception f)
            {
                this.Cursor = Cursors.Default;
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
        }
        #endregion

        #region TextBox event
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "MTMS_FT.dbo.B_ITEM_INFO", txtItemCd.Text, "");
        }

        private void txtTRNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtTRNo.Text.Trim() != "")
                {
                    string strSql = "Select ENT_CD, ENT_NM  From MTMS_FT.dbo.UVW_S_PROJECT_ENT  Where PROJECT_NO  = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
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
                MessageBox.Show(f.ToString());
            }

        }

        private void txtBASED_NO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "" && txtBASED_NO.Text.Trim() != "") SearchExec();
        }

        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtTRNo.Text.Trim() != "" && txtBASED_NO.Text.Trim() != "") SearchExec();
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFB05_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB05_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion
    }
}
