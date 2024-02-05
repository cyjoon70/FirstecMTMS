#region 작성정보
/*********************************************************************/
// 단위업무명 : 수불건수조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-19
// 작성내용 : 수불건수조회 관리
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

namespace IT.ITR108
{
    public partial class ITR108 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public ITR108()
        {
            InitializeComponent();
        }
        #endregion 

        #region Form Load 시
        private void ITR108_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            SystemBase.Validation.GroupBox_Setting(groupBox2);

            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 9); //품목계정

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Validation.GroupBox_Reset(groupBox2);

            fpSpread1.Sheets[0].Rows.Count = 0;

            //기타 세팅
            dtpTranDtFr.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).AddMonths(-1).ToShortDateString().Substring(0,10);
            dtpTranDtTo.Text = Convert.ToDateTime(SystemBase.Base.ServerTime("YYMMDD")).ToShortDateString().Substring(0,10);
        }
        #endregion

        #region SearchExec()  그리드 조회 로직
        protected override void SearchExec()
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
                {
                    string strQuery = " usp_ITR108 'S1'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_CD  ='" + txtItemCd.Text.Trim() + "'";
                    strQuery += ", @pDT_FR  ='" + dtpTranDtFr.Text.Trim() + "'";
                    strQuery += ", @pDT_TO ='" + dtpTranDtTo.Text.Trim() + "'";
                    strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                    strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                    if (txtInQty.Text.Trim() != "" && txtInQty.Value.ToString() != "0")
                        strQuery += ", @pIN_QTY ='" + txtInQty.Text + "'";
                    if (txtOutQty.Text.Trim() != "" && txtOutQty.Value.ToString() != "0")
                        strQuery += ", @pOUT_QTY ='" + txtOutQty.Value + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0)
                    {
                        strQuery = " usp_ITR108 'S2'";
                        strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                        strQuery += ", @pITEM_CD  ='" + txtItemCd.Text.Trim() + "'";
                        strQuery += ", @pDT_FR  ='" + dtpTranDtFr.Text.Trim() + "'";
                        strQuery += ", @pDT_TO ='" + dtpTranDtTo.Text.Trim() + "'";
                        strQuery += ", @pPROJECT_NO ='" + txtProject_No.Text.Trim() + "'";
                        strQuery += ", @pENT_CD ='" + txtEnt_CD.Text.Trim() + "'";
                        if (txtInQty.Text.Trim() != "" && txtInQty.Value.ToString() != "0")
                            strQuery += ", @pIN_QTY ='" + txtInQty.Text + "'";
                        if (txtOutQty.Text.Trim() != "" && txtOutQty.Value.ToString() != "0")
                            strQuery += ", @pOUT_QTY ='" + txtOutQty.Value + "'";
                        strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                        DataTable dt = SystemBase.DbOpen.NoTranDataTable(strQuery);

                        if (dt.Rows.Count > 0)
                        {
                            txtInAmt.Value = dt.Rows[0][0];
                            txtOutAmt.Value = dt.Rows[0][1];
                            txtStockAmt.Value = dt.Rows[0][2];
                        }
                        else
                        {
                            txtInAmt.Value = "";
                            txtOutAmt.Value = "";
                            txtStockAmt.Value = "";

                        }
                        Set_Color();
                    }
                }
            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }

            this.Cursor = Cursors.Default;
        }

        private void Set_Color()
        {
            int col_idx = SystemBase.Base.GridHeadIndex(GHIdx1, "출고횟수");

            for (int i = 0; i < fpSpread1.Sheets[0].RowCount; i++)
            {
                if (fpSpread1.Sheets[0].Cells[i, col_idx].Text == "0")
                {
                    for (int j = 0; j < fpSpread1.Sheets[0].ColumnCount; j++)
                    {
                        fpSpread1.Sheets[0].Cells[i, j].ForeColor = Color.Red;
                    }
                }
            }

        }
        #endregion
        
        #region 버튼 Click
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(txtItemCd.Text, "");
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

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
                    txtProject_No.Value = Msgs[3].ToString();
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

        private void btnEnt_Click(object sender, System.EventArgs e)
        {
            try
            {
                string strQuery = " usp_B_COMMON @pTYPE ='TABLE_POP', @pSPEC1 = 'ENT_CD', @pSPEC2 = 'ENT_NM', @pSPEC3 = 'S_ENTERPRISE_INFO', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'";
                string[] strWhere = new string[] { "@pCODE", "@pNAME" };
                string[] strSearch = new string[] { txtEnt_CD.Text, "" };
                UIForm.FPPOPUP pu = new UIForm.FPPOPUP("P00007", strQuery, strWhere, strSearch, new int[] { 0, 1 });
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    Regex rx1 = new Regex("#");
                    string[] Msgs = rx1.Split(pu.ReturnVal.ToString());

                    txtEnt_CD.Value = Msgs[0].ToString();
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

        #region TextChanged
        private void txtItemCd_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtItemCd.Text != "")
                {
                    txtItemNm.Value = SystemBase.Base.CodeName("ITEM_CD", "ITEM_NM", "B_ITEM_INFO", txtItemCd.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtItemNm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtProject_No_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtProject_No.Text != "")
                {
                    txtProject_Nm.Value = SystemBase.Base.CodeName("PROJECT_NO", "PROJECT_NM", "S_SO_MASTER", txtProject_No.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtProject_Nm.Value = "";
                }
            }
            catch
            {

            }
        }

        private void txtEnt_CD_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (txtEnt_CD.Text != "")
                {
                    txtEnt_NM.Value = SystemBase.Base.CodeName("ENT_CD", "ENT_NM", "S_ENTERPRISE_INFO", txtEnt_CD.Text, " AND CO_CD = '" + SystemBase.Base.gstrCOMCD + "'");
                }
                else
                {
                    txtEnt_NM.Value = "";
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 폼 Activated & Deactivate
        private void ITR108_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) txtProject_No.Focus();
        }

        private void ITR108_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
