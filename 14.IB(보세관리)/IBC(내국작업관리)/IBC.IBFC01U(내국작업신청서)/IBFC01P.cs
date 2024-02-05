#region 작성정보
/*********************************************************************/
// 단위업무명 : 내국작업신청서
// 작 성 자 : 이태규
// 작 성 일 : 2013-06-12
// 작성내용 : 내국작업신청서 및 관리
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

namespace IBC.IBFC01U
{ 
    public partial class IBFC01P : UIForm.FPCOMM1
    {
        #region 변수선언
        private bool chk = false;
        private FarPoint.Win.Spread.FpSpread spd;
        #endregion

        #region 생성자
        public IBFC01P()
        {
            InitializeComponent();
        }

        public IBFC01P(FarPoint.Win.Spread.FpSpread spread)
        {
            InitializeComponent();
            spd = spread;
        }
        #endregion

        #region Form Load 시
        private void IBFC01P_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);
            UIForm.Buttons.ReButton("010000000001", BtnNew, BtnSearch, BtnRCopy, BtnRowIns, BtnCancel, BtnDel, BtnDelete, BtnInsert, BtnExcel, BtnPrint, BtnHelp, BtnClose);
            this.Text = "수주참조팝업";
            SearchExec();
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

                    strQuery = " usp_IBFC01U  'P1', ";
                    strQuery = strQuery + " @pSO_DT_FR = '" + dtpSoFrDt.Text + "',";
                    strQuery = strQuery + " @pSO_DT_TO = '" + dtpSoToDt.Text + "',";
                    strQuery = strQuery + " @pREQ_DLVY_DT_FR = '" + dtpReqDlvyFrDt.Text + "',";
                    strQuery = strQuery + " @pREQ_DLVY_DT_TO = '" + dtpReqDlvyToDt.Text + "' ";
                    strQuery = strQuery + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "' ";

                    UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 4, false, false);
                    fpSpread1.Sheets[0].SetColumnAllowAutoSort(5, 2, true);

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
        }

        private void btnAllCancel_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
            {
                fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "선택")].Value = 0;
            }
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
                    int j = spd.Sheets[0].Rows.Count;
                    for (int i = 0; i < fpSpread1.Sheets[0].Rows.Count; i++)
                    {
                        if (fpSpread1.Sheets[0].Cells[i, 1].Text == "True")
                        {
                            spd.Sheets[0].Rows.Count = j + 1;
                            spd.Sheets[0].RowHeader.Cells[j, 0].Text = "I";

                            spd.Sheets[0].Cells[j, 3].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "T/R No.")].Text;
                            spd.Sheets[0].Cells[j, 4].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품처_2")].Text;
                            spd.Sheets[0].Cells[j, 5].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납품처")].Text;
                            spd.Sheets[0].Cells[j, 6].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "수주일")].Text;
                            spd.Sheets[0].Cells[j, 7].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "납기일")].Text;
                            spd.Sheets[0].Cells[j, 10].Text = fpSpread1.Sheets[0].Cells[i, SystemBase.Base.GridHeadIndex(GHIdx1, "작업의종류")].Text;

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

        #endregion

        #region KeyDown Event
        private void dtpSoFrDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpSoToDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpReqDlvyFrDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }

        private void dtpReqDlvyToDt_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SearchExec();
        }
        #endregion

        #region Activated & Deactivated
        private void IBFC01P_Activated(object sender, System.EventArgs e)
        {
            if (chk == false)
            {
                dtpSoFrDt.Focus();
            }
        }

        private void IBFC01P_Deactivate(object sender, System.EventArgs e)
        {
            chk = true;
        }
        #endregion

        #region Enter Event
        private void dtpSoFrDt_Enter(object sender, System.EventArgs e)
        {
            dtpSoFrDt.Select(0, 10);
        }

        private void dtpSoToDt_Enter(object sender, System.EventArgs e)
        {
            dtpSoToDt.Select(0, 10);
        }
        #endregion

    }
}
