#region 작성정보
/*********************************************************************/
// 단위업무명 : 입고현황
// 작 성 자 : 김현근
// 작 성 일 : 2013-04-19
// 작성내용 : 입고현황
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

namespace IN.INV108
{
    public partial class INV108 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        public INV108()
        {
            InitializeComponent();
        }

        #region Form Load 시
        private void INV108_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정
            dtpTranDt_Fr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpTranDt_To.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            dtpTranDt_Fr.Value = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToString();
            dtpTranDt_To.Text = SystemBase.Base.ServerTime("YYMMDD");
        }
        #endregion

        #region 조회조건 팝업
        //품목코드 from
        private void btnItemFr_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu1 = new WNDW001(txtItemCdFr.Text, cboItemAcct.SelectedValue.ToString());
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdFr.Text = Msgs[1].ToString();
                    txtItemNmFr.Value = Msgs[2].ToString();
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //품목코드 to
        private void btnItemTo_Click(object sender, EventArgs e)
        {
            try
            {
                WNDW001 pu1 = new WNDW001(txtItemCdTo.Text, cboItemAcct.SelectedValue.ToString());
                pu1.ShowDialog();
                if (pu1.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu1.ReturnVal;

                    txtItemCdTo.Text = Msgs[1].ToString();
                    txtItemNmTo.Value = Msgs[2].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }
        //프로젝트번호
        private void btnProjectNo_Click(object sender, EventArgs e)
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
        //사업
        private void btnEnt_Click(object sender, EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEnt_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEnt_CD.Text = Msgs[0].ToString();
                    txtEnt_NM.Value = Msgs[1].ToString();
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

        #region 조회조건 TextChanged
        //프로젝트번호
        private void txtProjectNo_TextChanged(object sender, EventArgs e)
        {
            txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 from
        private void txtItemCdFr_TextChanged(object sender, EventArgs e)
        {
            txtItemNmFr.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdFr.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 to
        private void txtItemCdTo_TextChanged(object sender, EventArgs e)
        {
            txtItemNmTo.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCdTo.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        //품목코드 사업
        private void txtEnt_CD_TextChanged(object sender, EventArgs e)
        {
            txtEnt_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEnt_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
        }
        #endregion

        private void INV108_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) dtpTranDt_Fr.Focus();
        }

        private void INV108_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }

        #region SearchExec()
        protected override void SearchExec()
        {
            //조회조건 필수 체크
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                this.Cursor = Cursors.WaitCursor;
                string strGbn = "";
                try
                {
                    string strQuery = " usp_INV108 'S1'";
                    strQuery += ", @pTRAN_DT_FR  ='" + dtpTranDt_Fr.Text + "'";
                    strQuery += ", @pTRAN_DT_TO  ='" + dtpTranDt_To.Text + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD_FR ='" + txtItemCdFr.Text.Trim() + "'";
                    strQuery += ", @pITEM_CD_TO ='" + txtItemCdTo.Text.Trim() + "'";
                    strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);

                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        //						Set_Span();
                        fpSpread1.Sheets[0].SetColumnMerge(1, FarPoint.Win.Spread.Model.MergePolicy.Always);
                        fpSpread1.Sheets[0].SetColumnMerge(2, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                        fpSpread1.Sheets[0].SetColumnMerge(3, FarPoint.Win.Spread.Model.MergePolicy.Restricted);
                        fpSpread1.Sheets[0].SetColumnMerge(4, FarPoint.Win.Spread.Model.MergePolicy.Restricted);

                        Set_Span();
                    }
                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이터 조회중 오류가 발생하였습니다.
                }

                this.Cursor = Cursors.Default;
            }
        }
        private void Set_Span()
        {
            int rowspan = 1;
            int rowfirst = 0;
            string temp_item = fpSpread1.Sheets[0].Cells[0, 1].Text;

            for (int i = 1; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (temp_item != fpSpread1.Sheets[0].Cells[i, 1].Text)
                {
                    if (rowspan != 1)
                    {
                        //						fpSpread1.Sheets[0].Cells[rowfirst,1].RowSpan = rowspan;
                        //						fpSpread1.Sheets[0].Cells[rowfirst,2].RowSpan = rowspan;
                        //						fpSpread1.Sheets[0].Cells[rowfirst,3].RowSpan = rowspan;
                        //						fpSpread1.Sheets[0].Cells[rowfirst,4].RowSpan = rowspan;
                        //						fpSpread1.Sheets[0].Cells[rowfirst,5].RowSpan = rowspan;

                        fpSpread1.Sheets[0].Cells[rowfirst, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        //						fpSpread1.Sheets[0].Cells[rowfirst,5].VerticalAlignment =  FarPoint.Win.Spread.CellVerticalAlignment.Top;

                        //for(int j = 7 ; j <  fpSpread1.Sheets[0].ColumnCount; j++)
                        //	fpSpread1.Sheets[0].Cells[i-1,j].BackColor = SystemBase.Base.gColor2;
                    }
                    rowfirst = i;
                    rowspan = 1;
                }
                else
                {
                    rowspan++;

                    if (i == fpSpread1.Sheets[0].RowCount - 1 && rowspan != 1)
                    {
                        fpSpread1.Sheets[0].Cells[rowfirst, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 3].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 4].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                    }
                }
                temp_item = fpSpread1.Sheets[0].Cells[i, 1].Text;
            }

            //			int row = fpSpread1.Sheets[0].RowCount - 1;
            //			for(int j = 0 ; j <  fpSpread1.Sheets[0].ColumnCount; j++)
            //			{
            //				fpSpread1.Sheets[0].Cells[row,j].BackColor = SystemBase.Base.gColor1;
            //			}

        }
        #endregion

    }
}
