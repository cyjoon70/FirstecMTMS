#region 작성정보
/*********************************************************************/
// 단위업무명 : 원재료실소요량계산서생성
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-10
// 작성내용 : 원재료실소요량계산서생성 및 관리
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

namespace IBF.IBFB05U 
{ 
    public partial class IBFB05P : UIForm.FPCOMM1
    {
        #region 변수선언
        private FarPoint.Win.Spread.FpSpread spd;
        private System.Windows.Forms.TextBox txtNO;
        private string strBASED_NO;
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB05P()
        {
            InitializeComponent();
        }

        public IBFB05P(string strTR_NO, string strSO_NO, System.Windows.Forms.TextBox txtBASED_NO, FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            txtTRNo.Value = strTR_NO;
            if (strSO_NO == "") txtSoNo.Value = strTR_NO;
            else txtSoNo.Value = strSO_NO;
            spd = spread;
            txtNO = txtBASED_NO;
            strBASED_NO = txtBASED_NO.Text;
        }
        #endregion

        #region Form Load 시
        private void IBFB05P_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "수주참조팝업";
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);

            dtpDT_FR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpDT_TO.Value = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;

            dtpDT_FR.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString().Substring(0, 10);
            dtpDT_TO.Value = SystemBase.Base.ServerTime("YYMMDD");
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
                    // 조회 Exceptions 체크
                    string strQuery;

                    strQuery = "usp_IBFB05P ";
                    strQuery = strQuery + " @pType  = 'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pSO_NO = '" + txtSoNo.Text + "',";
                    strQuery = strQuery + " @pDT_FR = '" + dtpDT_FR.Text + "',";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 4, false, false);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(4, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region 버튼 클릭 이벤트
        private void btnAllSelect_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 1;
            }
            Compute_Sum();
        }

        private void btnAllCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
            }
            neSUM_QTY.Value = 0;
            neSUM_AMT.Value = 0;
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].RowHeader.Cells[i, 0].Text = "";
            }

            try
            {

                if (fpSpread1.Sheets[0].Rows.Count > 0)
                {
                    //------근거번호 생성
                    //------BF + 납기일(8) + seq(2)
                    if (strBASED_NO == "")
                    {
                        string temp_no = "";

                        for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                        {
                            if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                            {
                                temp_no = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text.ToString().Substring(0, 4) + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text.ToString().Substring(5, 2) + fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text.ToString().Substring(8, 2);   //납기일자

                                string strSql = "Select ISNULL(MAX(USE_CREATE_NO),0)   From BF_BONDED_HDR(Nolock) Where TRACKING_NO = '" + txtTRNo.Text.Trim() + "' AND SO_NO = '" + txtSoNo.Text.Trim() + "' AND USE_CREATE_NO LIKE 'BF" + temp_no + "' + '%' ";
                                DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                                if (ds.Tables[0].Rows[0][0].ToString() == "0")
                                {
                                    txtNO.Text = "BF" + temp_no + "01";
                                }
                                else
                                {
                                    int seq = Convert.ToInt16(ds.Tables[0].Rows[0][0].ToString().Substring(10, 2));
                                    seq++;
                                    string strSEQ = Convert.ToString(seq);
                                    if (strSEQ.Length == 1) txtNO.Text = "BF" + temp_no + "0" + strSEQ;
                                    else txtNO.Text = "BF" + temp_no + strSEQ;
                                }
                                break;
                            }
                        }
                    }

                    int j = spd.Sheets[0].Rows.Count;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Text == "True")
                        {
                            spd.Sheets[0].Rows.Count = j + 1;
                            spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                            spd.Sheets[0].Cells[j, 1].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No")].Text;  //Tracking No.
                            spd.Sheets[0].Cells[j, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주번호")].Text;  //SO번호
                            spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주순번")].Text;  //SO_SEQ
                            spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;  //품목코드
                            spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목명")].Text;  //품목명
                            spd.Sheets[0].Cells[j, 7].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "총수주수량")].Value;  //총수주수량
                            spd.Sheets[0].Cells[j, 8].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value;   //수주수량
                            spd.Sheets[0].Cells[j, 9].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;    //단위
                            spd.Sheets[0].Cells[j, 10].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;  //단가
                            spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value; // 금액
                            spd.Sheets[0].Cells[j, 12].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일자")].Text;   //납기일자

                            j++;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            Close();
        }

        private void btnSo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF01' ";
                string[] strWhere = new string[] { "@pValue" };
                string[] strSearch = new string[] { txtSoNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP001", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Value = Msgs[0].ToString();
                    txtSoNo.Value = Msgs[1].ToString();
                    txtSoldToParty.Value = Msgs[2].ToString();
                    txtSO_DT.Value = Msgs[3].ToString();
                    txtREMARK.Value = Msgs[5].ToString();

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
                
        #region fpSpread1_CellClick
        private void Compute_Sum()
        {
            double qty = 0;
            double amt1 = 0;

            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, 1].Value.ToString() == "1")
                {
                    qty += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수량")].Value);
                    amt1 += Convert.ToDouble(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                }
            }
            neSUM_QTY.Value = qty;
            neSUM_AMT.Value = amt1;
        }

        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            Compute_Sum();
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFB05P_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB05P_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

        #region KeyDown
        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpDT_FR_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }        

        private void txtSoNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }
        #endregion
    }
}
