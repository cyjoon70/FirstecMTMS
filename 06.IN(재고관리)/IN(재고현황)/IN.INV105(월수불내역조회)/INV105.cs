#region 작성정보
/*********************************************************************/
// 단위업무명 : 월수불내역조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-02-19
// 작성내용 : 월수불내역조회 및 관리
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

namespace IN.INV105
{
    public partial class INV105 : UIForm.FPCOMM1
    {
        #region 변수선언
        bool form_act_chk = false;
        #endregion

        #region 생성자
        public INV105()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load시
        private void INV105_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            SystemBase.ComboMake.C1Combo(cboPlantCd, "usp_B_COMMON @pType='B031', @pSPEC1 = '" + SystemBase.Base.gstrBIZCD + "' , @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0);//공장
            SystemBase.ComboMake.C1Combo(cboItemAcct, "usp_B_COMMON @pTYPE='COMM', @pCODE = 'B036', @pLANG_CD='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 0); //품목계정
            mskDT.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
        }
        #endregion
        
        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            fpSpread1.Sheets[0].Rows.Count = 0;
            mskDT.Value = SystemBase.Base.ServerTime("YYMMDD").Substring(0, 7);
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
                    string strQuery = " usp_INV105 'S1'";
                    strQuery += ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "'";
                    strQuery += ", @pPLANT_CD ='" + cboPlantCd.SelectedValue.ToString() + "'";
                    strQuery += ", @pITEM_ACCT ='" + cboItemAcct.SelectedValue.ToString() + "'";
                    strQuery += ", @pYEAR_MON  ='" + mskDT.Text.Replace("-", "") + "'";
                    strQuery += ", @pITEM_CD  ='" + mskDT.Text.Replace("-", "") + "'";
                    strQuery += ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";
                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 2, true);
                    fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.Normal;
                    if (fpSpread1.Sheets[0].RowCount > 0) Set_Span();
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
                        fpSpread1.Sheets[0].Cells[rowfirst, 1].RowSpan = rowspan;
                        fpSpread1.Sheets[0].Cells[rowfirst, 2].RowSpan = rowspan;

                        fpSpread1.Sheets[0].Cells[rowfirst, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                        fpSpread1.Sheets[0].Cells[rowfirst, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;

                    }
                    rowfirst = i;
                    rowspan = 1;
                }
                else
                {
                    rowspan++;
                }
                temp_item = fpSpread1.Sheets[0].Cells[i, 1].Text;
            }

            if (rowspan > 1)
            {
                fpSpread1.Sheets[0].Cells[rowfirst, 1].RowSpan = rowspan;
                fpSpread1.Sheets[0].Cells[rowfirst, 2].RowSpan = rowspan;

                fpSpread1.Sheets[0].Cells[rowfirst, 1].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
                fpSpread1.Sheets[0].Cells[rowfirst, 2].VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Top;
            }

            int row = fpSpread1.Sheets[0].RowCount - 1;
            for (int j = 4; j < fpSpread1.Sheets[0].ColumnCount; j++)
            {
                fpSpread1.Sheets[0].Cells[row, j].BackColor = SystemBase.Base.gColor2;
            }

        }
        #endregion

        #region 팝업창 열기(품목)
        private void btnItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                WNDW005 pu = new WNDW005(cboPlantCd.SelectedValue.ToString(), true, txtItemCd.Text);
                pu.ShowDialog();
                if (pu.DialogResult == DialogResult.OK)
                {
                    string[] Msgs = pu.ReturnVal;

                    txtItemCd.Value = Msgs[2].ToString();
                    txtItemNm.Value = Msgs[3].ToString();
                    txtItemCd.Focus();
                }

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                DialogResult dsMsg = MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //데이터 조회 중 오류가 발생하였습니다.
            }
        }

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
        #endregion

        #region 폼 Activated & Deactivated
        private void INV105_Activated(object sender, System.EventArgs e)
        {
            if (form_act_chk == false) cboPlantCd.Focus();
        }

        private void INV105_Deactivate(object sender, System.EventArgs e)
        {
            form_act_chk = true;
        }
        #endregion
    }
}
