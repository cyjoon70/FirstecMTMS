using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WNDW;

namespace IBF.IBFB02U
{
    public partial class IBFB02P : UIForm.FPCOMM1
    {
        #region 변수선언
        FarPoint.Win.Spread.FpSpread spd;
        bool chk = false;
        #endregion

        #region 생성자
        public IBFB02P()
        {
            InitializeComponent();
        }

        public IBFB02P(string strBL_NO, string strTRNo, FarPoint.Win.Spread.FpSpread spread)
        {
            //
            // Windows Form 디자이너 지원에 필요합니다.
            //
            InitializeComponent();
            txtBLNo.Value = strBL_NO;
            txtTRNo.Value = strTRNo;
            spd = spread;

            //
            // TODO: InitializeComponent를 호출한 다음 생성자 코드를 추가합니다.
            //
        }
        #endregion

        #region 폼로드 이벤트
        private void IBFB02P_Load(object sender, EventArgs e)
        {
            this.Text = "B/L참조팝업";
            SystemBase.Validation.GroupBox_Setting(groupBox1); //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox2); //필수체크
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Base.GroupBoxReset(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, false);
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

                    if (txtBLNo.Text.Trim() != "")
                    {
                        string strSql = "usp_IBFB02P ";
                        strSql = strSql + " @pType  = 'S2',";
                        strSql = strSql + " @pBL_NO = '" + txtBLNo.Text + "',";
                        strSql = strSql + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                        DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            txtBeneficiaryNm.Value = ds.Tables[0].Rows[0][1].ToString();
                            txtIssueDt.Value = ds.Tables[0].Rows[0][2].ToString();
                            txtCUR.Value = ds.Tables[0].Rows[0][3].ToString();
                            neBL_AMT.Value = ds.Tables[0].Rows[0][4].ToString();
                        }
                    }
                    string strQuery;
                    strQuery = "usp_IBFB02P ";
                    strQuery = strQuery + " @pType  = 'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pBL_NO = '" + txtBLNo.Text + "',";
                    strQuery = strQuery + " @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(2, true);

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            this.Cursor = Cursors.Default;
            //			fpSpread1.Focus();

        }
        #endregion

        #region 버튼 클릭 이벤트
        private void btnTRNo_Click(object sender, System.EventArgs e)
        {
            try
            {
                //Tracking No. 팝업
                this.Cursor = Cursors.WaitCursor;
                string strQuery = " Nusp_BF_Comm 'BF23' ";
                strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                string[] strWhere = new string[] { "@pValue", "@pNAME" };
                string[] strSearch = new string[] { txtTRNo.Text, txtBLNo.Text };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("BFP009", strQuery, strWhere, strSearch, new int[] { 0, 1 }, "Tracking No.팝업");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtTRNo.Text = Msgs[0].ToString();
                    txtBLNo.Text = Msgs[1].ToString();
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
            neBL_AMT.Value = 0;
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
                    //					spd.Sheets[0].Rows.Count=0;
                    //					int j=0;
                    int j = spd.Sheets[0].Rows.Count;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            spd.Sheets[0].Rows.Count = j + 1;
                            spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";
                            spd.Sheets[0].Cells[j, 1].Locked = false;
                            spd.Sheets[0].Cells[j, 1].BackColor = Color.LightYellow;
                            spd.Sheets[0].Cells[j, 1].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "Tracking No.")].Text;  //Msgs[10].ToString(); //Tracking No.
                            spd.Sheets[0].Cells[j, 2].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L번호")].Text;  //Msgs[11].ToString(); //B/L번호
                            spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품목")].Text;  //Msgs[0].ToString();  //품목
                            spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품명")].Text;  //Msgs[1].ToString();  //품목코드
                            spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "규격")].Text;  //Msgs[2].ToString();  //규격
                            spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단위")].Text;  //Msgs[3].ToString();  //단위
                            spd.Sheets[0].Cells[j, 8].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value;  //Msgs[4].ToString();  //반입수량
                            spd.Sheets[0].Cells[j, 11].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "단가")].Value;  //Msgs[5].ToString();  //단가
                            spd.Sheets[0].Cells[j, 12].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value;  //Msgs[6].ToString();  //금액
                            spd.Sheets[0].Cells[j, 15].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value;  //Msgs[7].ToString();  //자국금액
                            spd.Sheets[0].Cells[j, 16].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L순번")].Text;  //Msgs[8].ToString();  //B/L순번
                            spd.Sheets[0].Cells[j, 17].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "발주번호")].Text;  //Msgs[9].ToString();  //발주번호	
                            spd.Sheets[0].Cells[j, 13].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "화폐")].Text;  //Msgs[12].ToString();  //화폐	
                            spd.Sheets[0].Cells[j, 14].Value = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "환율")].Value;  //Msgs[13].ToString();  //환율
                            spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관신고일자")].Text;  // Msgs[14].ToString();   //반입일자(통관정보의 신고일자)
                            spd.Sheets[0].Cells[j, 18].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "통관신고일자")].Text;
                            j++;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            Close();
        }
        #endregion

        private void Compute_Sum()
        {
            decimal qty = 0;
            decimal amt1 = 0;
            decimal amt2 = 0;
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value.ToString() == "1")
                {
                    qty += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "B/L수량")].Value);
                    amt1 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "금액")].Value);
                    amt2 += Convert.ToDecimal(fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자국금액")].Value);
                }
            }
            neSUM_QTY.Value = qty;
            neSUM_AMT.Value = amt1;
            neBL_AMT.Value = amt2;
        }

        private void fpSpread1_CellClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            Compute_Sum();
        }

        private void txtBLNo_Leave(object sender, System.EventArgs e)
        {
            try
            {
                if (txtBLNo.Text.Trim() != "")
                {
                    string strSql = "Select top 1 PROJECT_NO  From M_BL_DETAIL(Nolock) Where BL_NO = '" + txtBLNo.Text.Trim() + "' AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                    DataSet ds = SystemBase.DbOpen.NoTranDataSet(strSql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtTRNo.Text = ds.Tables[0].Rows[0][0].ToString();
                    }

                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBLNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        #region 폼 Activated & Deactivated
        private void IBFB02P_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtBLNo.Focus();
            }
        }

        private void IBFB02P_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion
    }
}
