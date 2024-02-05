using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SC.QA001
{
    public partial class QA001 : UIForm.FPCOMM1
    {
        #region 생성자
        public QA001()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Load
        private void QA001_Load(object sender, EventArgs e)
        {
            // 업무구분 콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboJobType, "usp_B_COMMON @pType='COMM', @pCODE = 'NO_JOB_TYP', @pLANG_CD = '" + SystemBase.Base.gstrLangCd.ToString() + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            SelectExec(false);
        }
        #endregion

        #region SelectExec() 그리드 조회 로직
        private void SelectExec(bool Msg)
        {
            try
            {
                string strQuery = "";
                strQuery = " usp_SC001 @pTYPE = 'S1' ";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @sJOB_TYPE ='" + cboJobType.SelectedValue.ToString() + "' ";
                strQuery = strQuery + ", @ssEARCH_WORDS ='" + txtWords.Text.ToString().Trim() + "' ";
                strQuery = strQuery + ", @sREG_USER ='" + txtRegUser.Text.ToString().Trim() + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread1, strQuery, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false, Msg);

            }
            catch (Exception f)
            {
                SystemBase.Loggers.Log(this.Name, f.ToString());
                MessageBox.Show(SystemBase.Base.MessageRtn("B0002"), SystemBase.Base.MessageRtn("Z0002"), MessageBoxButtons.OK, MessageBoxIcon.Error); //데이타 조회 중 오류가 발생하였습니다.
            }
        }
        #endregion

        #region fpSpread1_CellDoubleClick
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {

            HitUpdate(fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Seq")].Text);
            SelectExec(false);
            QA001P1 myForm = new QA001P1("R", fpSpread1.Sheets[0].Cells[e.Row, SystemBase.Base.GridHeadIndex(GHIdx1, "Seq")].Text);
            myForm.ShowDialog();
            SelectExec(false);

        }

        private void HitUpdate(string idx)
        {
            string strQuery = "";
            strQuery = " usp_SC001 @pTYPE = 'H1' ";
                        strQuery = strQuery + ", @pSEQ = " + idx + "";
            strQuery = strQuery + ", @pUP_ID = '" + SystemBase.Base.gstrUserID + "'";
            strQuery = strQuery + ", @pCOMP_CODE = '" + SystemBase.Base.gstrCOMCD + "' ";

            SystemBase.DbOpen.NoTranDataTable(strQuery);
        }
        #endregion

        #region NewExec()
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            //그리드 초기화
            UIForm.FPMake.grdCommSheet(fpSpread1, null, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
        }
        #endregion

        #region SearchExec() -- 검색
        protected override void SearchExec()
        {
            SelectExec(true);
        }
        #endregion

        #region RowInsExec() -- 등록
        protected override void RowInsExec()
        {
            QA001P1 myForm = new QA001P1("W", "");
            myForm.ShowDialog();
            SelectExec(false);
        }
        #endregion

        #region QA001_Activated
        private void QA001_Activated(object sender, System.EventArgs e)
        {
            SelectExec(false);
        }
        #endregion

    }
}
