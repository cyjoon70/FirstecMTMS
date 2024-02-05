using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace BA.BAA007
{
    public partial class BAA007 : UIForm.FPCOMM1
    {
        #region 변수선언
        string strQuery_pop = "";
        #endregion

        #region 생성자
        public BAA007()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BAA007_Load(object sender, System.EventArgs e)
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);
            SystemBase.Base.gstrFromLoading = "N";
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            FROMDT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));
            TODT.Value = Convert.ToDateTime(SystemBase.Base.ServerTime(""));

            SystemBase.Validation.GroupBox_GlobalApply(groupBox1);
            SystemBase.Base.gstrFromLoading = "Y";
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, false, 0, 0);
        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            string strQuery = "dbo.usp_BAA007  'S1'";
            strQuery = strQuery + ", @pFORM_NAME ='" + txtMenuId.Text + "' ";
            strQuery = strQuery + ", @pERR_MSG ='" + txtErrMemo.Text + "' ";
            strQuery = strQuery + ", @pFROMDT ='" + FROMDT.Text + "'";
            strQuery = strQuery + ", @pTODT ='" + TODT.Text + "'";

            strQuery_pop = strQuery;

            UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, false, true, 0, 0, false);

            fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.ExtendedSelect;
            fpSpread1.Sheets[0].SetColumnVisible(4, false);
        }

        #endregion

        #region 출력 버튼
        protected override void PrintExec()
        {
            if (strQuery_pop == "")
            {
                MessageBox.Show(SystemBase.Base.MessageRtn("SY001"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                c1Report1.DataSource.ConnectionString = "Provider=SQLOLEDB.1;Password=huneeddics;Persist Security Info=True;User ID=sa;Initial Catalog=ANDCOST;Data Source=222.99.80.25";
                c1Report1.DataSource.RecordSource = "exec " + strQuery_pop;

                C1.Win.C1Preview.C1PrintPreviewDialog ppv = new C1.Win.C1Preview.C1PrintPreviewDialog();
                ppv.Document = c1Report1;
                ppv.PreviewPane.ZoomMode = C1.Win.C1Preview.ZoomModeEnum.PageWidth;
                ppv.ShowDialog();
            }
        }

        #endregion

        #region 레포트 생성시 파라메터 팝업 뜨는 것 방지
        private void c1Report1_InitializeParametersDialog(object sender, C1.C1Report.DialogEventArgs e)
        {
            e.ShowDialog = false;
        }
        #endregion

        #region 그리드 더블클릭시 팝업창
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            double dblNum = Convert.ToDouble(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "에러번호")].Text);
            System.Windows.Forms.Form frm = new BAA007P1(dblNum);
            frm.ShowDialog();
        }
        #endregion

        #region 엔터키 이벤트
        private void BAA007_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchExec();
            }
        }
        #endregion

        #region **************************************  업체, 공장, 결산기간(From~To)은 변경시 초기화 및 변경여부체크 ******************************

        private void FROMDT_ValueChanged(object sender, EventArgs e)
        {
            if (SystemBase.Base.gstrFromLoading == "Y")
            {
                SystemBase.Base.gstrPERIOD_FROM = SystemBase.Validation.C1DataEdit_WriteFormat(FROMDT.Value.ToString(), "YYYY-MM-DD");
            }
        }
        private void TODT_ValueChanged(object sender, EventArgs e)
        {
            if (SystemBase.Base.gstrFromLoading == "Y")
            {
                SystemBase.Base.gstrPERIOD_TO = SystemBase.Validation.C1DataEdit_WriteFormat(TODT.Value.ToString(), "YYYY-MM-DD");
            }
        }
        private void FROMDT_BeforeDropDownOpen(object sender, CancelEventArgs e)
        {
            Value_Selected(e, null, null);
        }
        private void FROMDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 48 && e.KeyChar <= 57)  // 숫자 키보드만 체크
            {
                Value_Selected(null, e, null);
            }
        }
        private void FROMDT_UpDownButtonClick(object sender, C1.Win.C1Input.UpDownButtonClickEventArgs e)
        {
            Value_Selected(null, null, e);
        }

        private void TODT_BeforeDropDownOpen(object sender, CancelEventArgs e)
        {
            Value_Selected(e, null, null);
        }
        private void TODT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 48 && e.KeyChar <= 57)  // 숫자 키보드만 체크
            {
                Value_Selected(null, e, null);
            }
        }
        private void TODT_UpDownButtonClick(object sender, C1.Win.C1Input.UpDownButtonClickEventArgs e)
        {
            Value_Selected(null, null, e);
        }

        private void Value_Selected(CancelEventArgs e, KeyPressEventArgs f, C1.Win.C1Input.UpDownButtonClickEventArgs g)
        {
            NewExec();
            strQuery_pop = "";
        }

        #endregion
    }
}
