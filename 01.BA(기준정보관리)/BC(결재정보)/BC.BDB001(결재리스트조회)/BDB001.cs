#region 작성정보
/*********************************************************************/
// 단위업무명 : 결재리스트조회
// 작 성 자 : 이 태 규
// 작 성 일 : 2013-04-24
// 작성내용 : 결재리스트조회 및 관리
// 수 정 일 :
// 수 정 자 :
// 수정내용 :
// 비    고 :
/*********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using WNDW;
using System.Reflection;

namespace BC.BDB001
{
    public partial class BDB001 : UIForm.FPCOMM2
    {
        #region 생성자
        public BDB001()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Load 시
        private void BDB001_Load(object sender, System.EventArgs e)
        {
            //필수체크
            SystemBase.Validation.GroupBox_Setting(groupBox1);

            //콤보박스 세팅
            SystemBase.ComboMake.C1Combo(cboGbn, "usp_BDB001 @pType='C1' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);
            SystemBase.ComboMake.C1Combo(cboStatus, "usp_BDB001 @pType='C2' ,@pLANG_CD ='" + SystemBase.Base.gstrLangCd + "', @pCO_CD='" + SystemBase.Base.gstrCOMCD + "'", 3);

            dtpFrDt.Text = DateTime.Now.AddDays(-7).ToShortDateString().Substring(0,10);
            dtpToDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
            cboGbn.SelectedValue = "N";

        }
        #endregion
        
        #region PrintExec()
        protected override void PrintExec()
        {
            if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화면ID")].Text == "MOB001")
            {

            }
            else if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화면ID")].Text == "MRB001")
            {

            }
            else if (fpSpread2.Sheets[0].Cells[fpSpread2.Sheets[0].ActiveRowIndex, SystemBase.Base.GridHeadIndex(GHIdx1, "화면ID")].Text == "PRA002_SH")
            {

            }

        }
        #endregion

        #region SearchExec() 그리드 조회 로직
        protected override void SearchExec()
        {
            if (SystemBase.Validation.GroupBox_SaveSearchValidation(groupBox1))
            {
                string strQuery = " usp_BDB001  'S1'";
                strQuery = strQuery + ", @pLANG_CD ='" + SystemBase.Base.gstrLangCd + "' ";
                strQuery = strQuery + ", @pCO_CD ='" + SystemBase.Base.gstrCOMCD + "' ";
                strQuery = strQuery + ", @pYMFR ='" + dtpFrDt.Text + "' ";
                strQuery = strQuery + ", @pYMTO ='" + dtpToDt.Text + "' ";

                if (cboGbn.SelectedValue.ToString() != "") strQuery = strQuery + ", @pGBN ='" + cboGbn.SelectedValue.ToString() + "' ";
                if (cboStatus.SelectedValue.ToString() != "") strQuery = strQuery + ", @pSTATUS ='" + cboStatus.SelectedValue.ToString() + "'";

                strQuery = strQuery + ", @pDOCUNM ='" + txtDocuNm.Text + "' ";
                strQuery = strQuery + ", @pDOCUNO ='" + txtDocuNo.Text + "' ";
                strQuery = strQuery + ", @pWRITERNM ='" + txtWriter.Text + "' ";
                strQuery = strQuery + ", @pUP_ID ='" + SystemBase.Base.gstrUserID + "' ";

                UIForm.FPMake.grdCommSheet(fpSpread2, strQuery, G2Head1, G2Head2, G2Head3, G2Width, G2Align, G2Type, G2Color, G2Etc, G2HeadCnt, 0, 0, false, false);
                
                if (fpSpread2.Sheets[0].Rows.Count > 0)
                {
                    string strCode = fpSpread2.Sheets[0].Cells[0, 11].Text.ToString();
                    fpSpread2.ActiveSheet.SetActiveCell(0, 1);
                    Right_Search(strCode);
                }
                else
                    fpSpread1.Sheets[0].RowCount = 0;
            }
        }
        #endregion

        #region NewExec() New 버튼 클릭 이벤트
        protected override void NewExec()
        {
            SystemBase.Validation.GroupBox_Reset(groupBox1);

            fpSpread1.Sheets[0].Rows.Count = 0;
            fpSpread2.Sheets[0].Rows.Count = 0;	

            dtpFrDt.Text = DateTime.Now.AddDays(-7).ToShortDateString().Substring(0, 10);
            dtpToDt.Text = SystemBase.Base.ServerTime("YYMMDD").ToString().Substring(0, 10);
        }
        #endregion

        #region 좌측그리드 방향키 이동시 우측조회
        private void fpSpread2_SelectionChanged(object sender, FarPoint.Win.Spread.SelectionChangedEventArgs e)
        {
            if (fpSpread2.Sheets[0].Rows.Count > 0 && fpSpread2.Sheets[0].RowHeaderSelectorIndex >0)
            {
                int intRow = fpSpread2.ActiveSheet.GetSelection(0).Row;
                string strCode = fpSpread2.Sheets[0].Cells[intRow, SystemBase.Base.GridHeadIndex(GHIdx1, "결재번호")].Text.ToString();
                Right_Search(strCode);
            }
        }
        #endregion

        #region 하위 그리드 조회
        private void Right_Search(string strNo)
        {
            if (strNo.ToString() != "")
            {
                string strSql = " usp_BDB001  'S2'";
                strSql = strSql + ", @pLANG_CD='" + SystemBase.Base.gstrLangCd + "' ";
                strSql = strSql + ", @pCO_CD = '" + SystemBase.Base.gstrCOMCD + "'";
                strSql = strSql + ", @pDOCUNO = '" + strNo + "'";

                UIForm.FPMake.grdCommSheet(fpSpread1, strSql, G1Head1, G1Head2, G1Head3, G1Width, G1Align, G1Type, G1Color, G1Etc, G1HeadCnt, 0, 0, false);
                fpSpread1.Sheets[0].OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            }
        }
        #endregion

        #region fpSpread2_CellDoubleClick
        private void fpSpread2_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            object[] param = new object[1];
            param[0] = fpSpread2.Sheets[0].Cells[e.Row, 1].Text;

            Assembly ServiceAssembly = Assembly.LoadFile(SystemBase.Base.ProgramWhere.ToString() + "\\" + "SO." + fpSpread2.Sheets[0].Cells[e.Row, 12].Text + ".dll");
            Form myForm = (Form)System.Activator.CreateInstance(ServiceAssembly.GetType("SO." + fpSpread2.Sheets[0].Cells[e.Row, 12].Text.ToString() + "." + fpSpread2.Sheets[0].Cells[e.Row, 12].Text.ToString()), param);

            myForm.ShowDialog();
        }
        #endregion

        #region fpSpread2_ButtonClicked
        private void fpSpread2_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            if (e.Column == 10)
            {
                try
                {
                    //UIForm.FileUpDown frm = new UIForm.FileUpDown(fpSpread2.Sheets[0].Cells[e.Row, 1].Text, "N#Y#N");
                    //frm.ShowDialog();

                }
                catch (Exception f)
                {
                    SystemBase.Loggers.Log(this.Name, f.ToString());
                    MessageBox.Show(f.ToString());
                }
            }
        }
        #endregion
    }
}
