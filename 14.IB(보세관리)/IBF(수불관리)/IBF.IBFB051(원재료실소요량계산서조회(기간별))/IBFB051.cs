#region 작성정보
/*********************************************************************/
// 단위업무명 : 원재료실소요량계산서조회(기간별)
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-10
// 작성내용 : 원재료실소요량계산서조회(기간별) 및 관리
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

namespace IBF.IBFB051
{ 
    public partial class IBFB051 : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        #endregion

        #region 생성자
        public IBFB051()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void IBFB051_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
            
            dtpDT_FR.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0, 4) +"-01-01";
            dtpDT_TO.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddYears(-1).ToString().Substring(0, 4) + "-12-31";

            txtTRNo.Focus();
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            //그리드 초기화
            fpSpread1.Sheets[0].Rows.Count = 0;
            txtTRNo.Focus();

            dtpDT_FR.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-01-01";
            dtpDT_TO.Text = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 4) + "-12-31";
        }
        #endregion
        
        #region PrintExec() 그리드 출력 로직
        protected override void PrintExec()
        {
            try
            {
                string[] RptParmValue = new string[6];
                string RptName = "";

                if (fpSpread1.Sheets[0].Rows.Count <= 0) return;
                //--레포트 파일 선택

                RptName = @"Report\" + "IBFB34P.rpt";
                RptParmValue[0] = "R1";
                RptParmValue[1] = txtTRNo.Text;
                RptParmValue[2] = txtItemCd.Text;
                RptParmValue[3] = dtpDT_FR.Text;
                RptParmValue[4] = dtpDT_TO.Text;
                RptParmValue[5] = SystemBase.Base.gstrCOMCD;

                UIForm.PRINT10 frm = new UIForm.PRINT10(this.Text + " 출력", null, null, RptName, RptParmValue);	//공통크리스탈 10버전
                frm.ShowDialog();
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(f.ToString());
            }
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

                    string strQuery = " usp_IBFB051  'S1',";
                    strQuery = strQuery + " @pTRACKING_NO = '" + txtTRNo.Text + "',";
                    strQuery = strQuery + " @pITEM_CD = '" + txtItemCd.Text + "',";
                    strQuery = strQuery + " @pDT_FT = '" + dtpDT_FR.Text + "',";
                    strQuery = strQuery + " @pDT_TO = '" + dtpDT_TO.Text + "'";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 5, false);
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }

            }

            if (fpSpread1.Sheets[0].Rows.Count > 0) Set_Color();

            this.Cursor = Cursors.Default;
            fpSpread1.Focus();

        }

        private void Set_Color()
        {
            int j;
            int i;
            try
            {
                for (i = 0; i < fpSpread1.Sheets[0].Rows.Count - 3; i++)
                {
                    if (fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text == "zz")
                    {
                        for (j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                        {
                            fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor2;
                        }
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "품번")].Text = "";
                        fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "자품목")].Text = "합계";
                    }

                }

                for (i = fpSpread1.Sheets[0].Rows.Count - 3; i < fpSpread1.Sheets[0].Rows.Count; i++)
                {
                    for (j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].BackColor = SystemBase.Base.gColor1;
                    }
                }

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
                string[] strWhere = new string[] { "@pValue", "@pNAME" };
                string[] strSearch = new string[] { txtItemCd.Text, "" };
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
                    string strSql = "Select ENT_CD, ENT_NM  From MTMS_FT.dbo.UVW_S_PROJECT_ENT Where PROJECT_NO = '" + txtTRNo.Text.Trim() + "' AND BONDED_YN = 'Y' AND Rtrim(ENT_NM) <> '' ";
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

        private void txtItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void txtChildItemCd_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpDT_FR_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (dtpDT_FR.Text.Trim() != "" && e.KeyCode == Keys.Enter) SearchExec();

        }

        private void dtpDT_TO_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (dtpDT_TO.Text.Trim() != "" && e.KeyCode == Keys.Enter) SearchExec();

        }

        private void txtTRNo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }
        #endregion

        #region Form Activated & Deactivated
        private void IBFB051_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                txtTRNo.Focus();
            }
        }

        private void IBFB051_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion
           
        #region dtpDT_Leave
        private void dtpDT_FR_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_FR.Text.Trim() != "")
            {
                if (IsDate(dtpDT_FR.Text) == false)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
                    dtpDT_FR.Focus();
                    dtpDT_FR.SelectAll();
                }
            }
        }

        private void dtpDT_TO_Leave(object sender, System.EventArgs e)
        {
            if (dtpDT_TO.Text.Trim() != "")
            {
                if (IsDate(dtpDT_TO.Text) == false)
                {
                    MessageBox.Show(SystemBase.Base.MessageRtn("B023"));
                    dtpDT_TO.Focus();
                    dtpDT_TO.SelectAll();
                }
            }
        }

        public static bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        } 
        #endregion

    }
}
